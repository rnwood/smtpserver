namespace Rnwood.SmtpServer
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Verbs;

    public class DataVerb : IVerb
    {
        public async virtual Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            if (connection.CurrentMessage == null)
            {
                await connection.WriteResponse(new SmtpResponse(
                    StandardSmtpResponseCode.BadSequenceOfCommands,
                                                                   "Bad sequence of commands")).ConfigureAwait(false);
                return;
            }

            connection.CurrentMessage.SecureConnection = connection.Session.SecureConnection;
            await connection.WriteResponse(new SmtpResponse(
                StandardSmtpResponseCode.StartMailInputEndWithDot,
                                                               "End message with period")).ConfigureAwait(false);

            using (StreamWriter writer = new StreamWriter(await connection.CurrentMessage.WriteData().ConfigureAwait(false), connection.ReaderEncoding))
            {
                bool firstLine = true;

                do
                {
                    string line = await connection.ReadLine().ConfigureAwait(false);

                    if (line != ".")
                    {
                        line = this.ProcessLine(line);

                        if (!firstLine)
                        {
                            writer.Write(Environment.NewLine);
                        }

                        writer.Write(line);
                    }
                    else
                    {
                        break;
                    }

                    firstLine = false;
                } while (true);

                writer.Flush();
                long? maxMessageSize =
                    await connection.Server.Behaviour.GetMaximumMessageSize(connection).ConfigureAwait(false);

                if (maxMessageSize.HasValue && writer.BaseStream.Length > maxMessageSize.Value)
                {
                    await connection.WriteResponse(
                        new SmtpResponse(
                            StandardSmtpResponseCode.ExceededStorageAllocation,
                                         "Message exceeds fixed size limit")).ConfigureAwait(false);
                }
                else
                {
                    writer.Dispose();
                    await connection.Server.Behaviour.OnMessageCompleted(connection).ConfigureAwait(false);
                    await connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.OK, "Mail accepted")).ConfigureAwait(false);
                    await connection.CommitMessage().ConfigureAwait(false);
                }
            }
        }

        protected virtual string ProcessLine(string line)
        {
            // Remove escaping of end of message character
            if (line.StartsWith(".", StringComparison.Ordinal))
            {
                line = line.Remove(0, 1);
            }
            return line;
        }
    }
}