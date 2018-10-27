namespace Rnwood.SmtpServer
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Security;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Extensions;
    using Rnwood.SmtpServer.Verbs;

    public class Connection : IConnection
    {
        public IConnectionChannel ConnectionChannel { get; private set; }

        private readonly string id;

        public static async Task<Connection> Create(IServer server, IConnectionChannel connectionChannel, IVerbMap verbMap)
        {
            IEditableSession session = await server.Behaviour.OnCreateNewSession(connectionChannel).ConfigureAwait(false);
            var extensions = await server.Behaviour.GetExtensions(connectionChannel).ConfigureAwait(false);
            IExtensionProcessor[] createConnectionExtensions(IConnection c) => extensions.Select(e => e.CreateExtensionProcessor(c)).ToArray();
            Connection result = new Connection(server, session, connectionChannel, verbMap, createConnectionExtensions);
            await result.SetReaderEncodingToDefault().ConfigureAwait(false);
            

            return result;
        }

        internal Connection(IServer server, IEditableSession session, IConnectionChannel connectionChannel, IVerbMap verbMap, Func<IConnection, IExtensionProcessor[]> extensionProcessors)
        {
            this.id = string.Format("[RemoteIP={0}]", connectionChannel.ClientIPAddress.ToString());

            this.ConnectionChannel = connectionChannel;
            this.ConnectionChannel.Closed += this.OnConnectionChannelClosed;

            this.VerbMap = verbMap;
            this.Session = session;
            this.Server = server;
            this.ExtensionProcessors = extensionProcessors(this).ToArray(); ;
        }

        private void OnConnectionChannelClosed(object sender, EventArgs e)
        {
            this.ConnectionClosed?.Invoke(this, EventArgs.Empty);
        }

        public override string ToString()
        {
            return this.id;
        }

        public IServer Server { get; private set; }

        public event EventHandler ConnectionClosed;

        public void SetReaderEncoding(Encoding encoding)
        {
            this.ConnectionChannel.SetReaderEncoding(encoding);
        }

        public Encoding ReaderEncoding => this.ConnectionChannel.ReaderEncoding;

        public async Task SetReaderEncodingToDefault()
        {
            this.SetReaderEncoding(await this.Server.Behaviour.GetDefaultEncoding(this).ConfigureAwait(false));
        }

        public IExtensionProcessor[] ExtensionProcessors { get; private set; }

        public async Task CloseConnection()
        {
            await this.ConnectionChannel.CloseAync().ConfigureAwait(false);
        }

        public IVerbMap VerbMap { get; private set; }

        public async Task ApplyStreamFilter(Func<Stream, Task<Stream>> filter)
        {
            await this.ConnectionChannel.ApplyStreamFilterAsync(filter).ConfigureAwait(false);
        }

        public MailVerb MailVerb => (MailVerb)this.VerbMap.GetVerbProcessor("MAIL");

        protected async Task WriteLineAndFlush(string text, params object[] arg)
        {
            string formattedText = string.Format(text, arg);
            this.Session.AppendToLog(formattedText);
            await this.ConnectionChannel.WriteLineAsync(formattedText).ConfigureAwait(false);
            await this.ConnectionChannel.FlushAsync().ConfigureAwait(false);
        }

        public async Task WriteResponse(SmtpResponse response)
        {
            await this.WriteLineAndFlush(response.ToString().TrimEnd()).ConfigureAwait(false);
        }

        public async Task<string> ReadLine()
        {
            string text = await this.ConnectionChannel.ReadLineAsync().ConfigureAwait(false);
            this.Session.AppendToLog(text);
            return text;
        }

        public IEditableSession Session { get; private set; }

        public IMessageBuilder CurrentMessage { get; private set; }

        public async Task<IMessageBuilder> NewMessage()
        {
            this.CurrentMessage = await this.Server.Behaviour.OnCreateNewMessage(this).ConfigureAwait(false);
            this.CurrentMessage.Session = this.Session;
            return this.CurrentMessage;
        }

        public async Task CommitMessage()
        {
            IMessage message = await this.CurrentMessage.ToMessage().ConfigureAwait(false);
            this.Session.AddMessage(message);
            this.CurrentMessage = null;

            await this.Server.Behaviour.OnMessageReceived(this, message).ConfigureAwait(false);
        }

        public Task AbortMessage()
        {
            this.CurrentMessage = null;
            return Task.CompletedTask;
        }

        public async Task ProcessAsync()
        {
            try
            {
                await this.Server.Behaviour.OnSessionStarted(this, this.Session).ConfigureAwait(false);
                this.SetReaderEncoding(await this.Server.Behaviour.GetDefaultEncoding(this).ConfigureAwait(false));

                if (await this.Server.Behaviour.IsSSLEnabled(this).ConfigureAwait(false))
                {
                    await this.ConnectionChannel.ApplyStreamFilterAsync(async s =>
                    {
                        SslStream sslStream = new SslStream(s);
                        await sslStream.AuthenticateAsServerAsync(await this.Server.Behaviour.GetSSLCertificate(this).ConfigureAwait(false)).ConfigureAwait(false);
                        return sslStream;
                    }).ConfigureAwait(false);

                    this.Session.SecureConnection = true;
                }

                await this.WriteResponse(new SmtpResponse(
                    StandardSmtpResponseCode.ServiceReady,
                                               this.Server.Behaviour.DomainName + " smtp4dev ready")).ConfigureAwait(false);

                int numberOfInvalidCommands = 0;
                while (this.ConnectionChannel.IsConnected)
                {
                    bool badCommand = false;
                    SmtpCommand command = new SmtpCommand(await this.ReadLine().ConfigureAwait(false));
                    await this.Server.Behaviour.OnCommandReceived(this, command).ConfigureAwait(false);

                    if (command.IsValid)
                    {
                        IVerb verbProcessor = this.VerbMap.GetVerbProcessor(command.Verb);

                        if (verbProcessor != null)
                        {
                            try
                            {
                                await verbProcessor.ProcessAsync(this, command).ConfigureAwait(false);
                            }
                            catch (SmtpServerException exception)
                            {
                                await this.WriteResponse(exception.SmtpResponse).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            badCommand = true;
                        }
                    }
                    else if (command.IsEmpty)
                    {
                    }
                    else
                    {
                        badCommand = true;
                    }

                    if (badCommand)
                    {
                        numberOfInvalidCommands++;

                        if (this.Server.Behaviour.MaximumNumberOfSequentialBadCommands > 0 &&
                        numberOfInvalidCommands >= this.Server.Behaviour.MaximumNumberOfSequentialBadCommands)
                        {
                            await this.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.ClosingTransmissionChannel, "Too many bad commands. Bye!")).ConfigureAwait(false);
                            await this.CloseConnection().ConfigureAwait(false);
                        }
                        else
                        {
                            await this.WriteResponse(new SmtpResponse(
                                StandardSmtpResponseCode.SyntaxErrorCommandUnrecognised,
                                                           "Command unrecognised")).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (IOException ioException)
            {
                this.Session.SessionError = ioException;
                this.Session.SessionErrorType = SessionErrorType.NetworkError;
            }
            catch (Exception exception)
            {
                this.Session.SessionError = exception;
                this.Session.SessionErrorType = SessionErrorType.UnexpectedException;
            }

            await this.CloseConnection().ConfigureAwait(false);

            this.Session.EndDate = DateTime.Now;
            await this.Server.Behaviour.OnSessionCompleted(this, this.Session).ConfigureAwait(false);
        }
    }
}