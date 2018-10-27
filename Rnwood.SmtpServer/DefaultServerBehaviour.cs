namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Extensions;
    using Rnwood.SmtpServer.Extensions.Auth;

    public class DefaultServerBehaviour : IServerBehaviour
    {
        private readonly X509Certificate sslCertificate;
        private readonly bool allowRemoteConnections;

        public DefaultServerBehaviour(bool allowRemoteConnections, X509Certificate sslCertificate)
            : this(allowRemoteConnections, 587, sslCertificate)
        {
        }

        public DefaultServerBehaviour(bool allowRemoteConnections)
            : this(allowRemoteConnections, 25, null)
        {
        }

        public DefaultServerBehaviour(bool allowRemoteConnections, int portNumber)
            : this(allowRemoteConnections, portNumber, null)
        {
        }

        public DefaultServerBehaviour(bool allowRemoteConnections, int portNumber, X509Certificate sslCertificate)
        {
            this.PortNumber = portNumber;
            this.sslCertificate = sslCertificate;
            this.allowRemoteConnections = allowRemoteConnections;
        }

        public Task<IEditableSession> OnCreateNewSession(IConnectionChannel connection)
        {
            return Task.FromResult<IEditableSession>(new MemorySession(connection.ClientIPAddress, DateTime.Now));
        }

        public virtual Task<Encoding> GetDefaultEncoding(IConnection connection)
        {
            return Task.FromResult<Encoding>(new ASCIISevenBitTruncatingEncoding());
        }

        public virtual Task OnMessageReceived(IConnection connection, IMessage message)
        {
            this.MessageReceived?.Invoke(this, new MessageEventArgs(message));

            return Task.CompletedTask;
        }

        public virtual string DomainName => Environment.MachineName;

        public virtual IPAddress IpAddress => this.allowRemoteConnections ? IPAddress.Any : IPAddress.Loopback;

        public virtual int PortNumber { get; private set; }

        public Task<bool> IsSSLEnabled(IConnection connection)
        {
            return Task.FromResult(this.sslCertificate != null);
        }

        public Task<bool> IsSessionLoggingEnabled(IConnection connection)
        {
            return Task.FromResult(false);
        }

        public virtual Task<long?> GetMaximumMessageSize(IConnection connection)
        {
            return Task.FromResult<long?>(null);
        }

        public virtual Task<X509Certificate> GetSSLCertificate(IConnection connection)
        {
            return Task.FromResult(this.sslCertificate);
        }

        public virtual Task OnMessageRecipientAdding(IConnection connection, IMessageBuilder message, string recipient)
        {
            return Task.CompletedTask;
        }

        public virtual Task<IEnumerable<IExtension>> GetExtensions(IConnectionChannel connectionChannel)
        {
            List<IExtension> extensions = new List<IExtension>(new IExtension[] { new EightBitMimeExtension(), new SizeExtension() });

            if (this.sslCertificate != null)
            {
                extensions.Add(new StartTlsExtension());
            }

            return Task.FromResult<IEnumerable<IExtension>>(extensions);
        }

        public virtual Task OnSessionCompleted(IConnection connection, ISession session)
        {
            this.SessionCompleted?.Invoke(this, new SessionEventArgs(session));

            return Task.CompletedTask;
        }

        public virtual Task OnSessionStarted(IConnection connection, ISession session)
        {
            this.SessionStarted?.Invoke(this, new SessionEventArgs(session));

            return Task.CompletedTask;
        }

        public virtual Task<TimeSpan> GetReceiveTimeout(IConnectionChannel connectionChannel)
        {
            return Task.FromResult(new TimeSpan(0, 0, 30));
        }

        public virtual Task<TimeSpan> GetSendTimeout(IConnectionChannel connectionChannel)
        {
            return Task.FromResult(new TimeSpan(0, 0, 30));
        }

        public int MaximumNumberOfSequentialBadCommands => 10;

        public async virtual Task<AuthenticationResult> ValidateAuthenticationCredentialsAsync(
            IConnection connection,
                                                                          IAuthenticationCredentials request)
        {
            var handlers = this.AuthenticationCredentialsValidationRequiredAsync;

            if (handlers != null)
            {
                var tasks = handlers.GetInvocationList()
                    .Cast<Func<object, AuthenticationCredentialsValidationEventArgs, Task>>()
                    .Select(h =>
                    {
                        AuthenticationCredentialsValidationEventArgs args = new AuthenticationCredentialsValidationEventArgs(request);
                        return new { Args = args, Task = h(this, args) };
                    });

                await Task.WhenAll(tasks.Select(t => t.Task).ToArray()).ConfigureAwait(false);

                AuthenticationResult? failureResult = tasks.Select(t => t.Args.AuthenticationResult)
                    .Where(r => r != AuthenticationResult.Success)
                    .FirstOrDefault();

                return failureResult ?? AuthenticationResult.Success;
            }

            return AuthenticationResult.Failure;
        }

        public virtual Task OnMessageStart(IConnection connection, string from)
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> IsAuthMechanismEnabled(IConnection connection, IAuthMechanism authMechanism)
        {
            return Task.FromResult(false);
        }

        public virtual Task OnCommandReceived(IConnection connection, SmtpCommand command)
        {
            this.CommandReceived?.Invoke(this, new CommandEventArgs(command));

            return Task.CompletedTask;
        }

        public virtual Task<IMessageBuilder> OnCreateNewMessage(IConnection connection)
        {
            return Task.FromResult<IMessageBuilder>(new MemoryMessage.Builder());
        }

        public virtual Task OnMessageCompleted(IConnection connection)
        {
            this.MessageCompleted?.Invoke(this, new ConnectionEventArgs(connection));

            return Task.CompletedTask;
        }

        public event EventHandler<CommandEventArgs> CommandReceived;

        public event EventHandler<ConnectionEventArgs> MessageCompleted;

        public event EventHandler<MessageEventArgs> MessageReceived;

        public event EventHandler<SessionEventArgs> SessionCompleted;

        public event EventHandler<SessionEventArgs> SessionStarted;

        public event Func<object, AuthenticationCredentialsValidationEventArgs, Task> AuthenticationCredentialsValidationRequiredAsync;
    }
}