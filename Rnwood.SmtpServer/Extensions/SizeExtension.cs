namespace Rnwood.SmtpServer.Extensions
{
    using System;
    using System.Threading.Tasks;

    public class SizeExtension : IExtension
    {
        public IExtensionProcessor CreateExtensionProcessor(IConnection connection)
        {
            return new SizeExtensionProcessor(connection);
        }

        private class SizeExtensionProcessor : IExtensionProcessor, IParameterProcessor
        {
            public SizeExtensionProcessor(IConnection connection)
            {
                this.Connection = connection;
                this.Connection.MailVerb.FromSubVerb.ParameterProcessorMap.SetProcessor("SIZE", this);
            }

            public IConnection Connection { get; private set; }

            public async Task SetParameter(IConnection connection, string key, string value)
            {
                if (key.Equals("SIZE", StringComparison.OrdinalIgnoreCase))
                {
                    if (long.TryParse(value, out long messageSize) && messageSize > 0)
                    {
                        long? maxMessageSize = await this.Connection.Server.Behaviour.GetMaximumMessageSize(this.Connection).ConfigureAwait(false);
                        connection.CurrentMessage.DeclaredMessageSize = messageSize;

                        if (maxMessageSize.HasValue && messageSize > maxMessageSize)
                        {
                            throw new SmtpServerException(
                                new SmtpResponse(
                                    StandardSmtpResponseCode.ExceededStorageAllocation,
                                                 "Message exceeds fixes size limit"));
                        }
                    }
                    else
                    {
                        throw new SmtpServerException(new SmtpResponse(StandardSmtpResponseCode.SyntaxErrorInCommandArguments, "Bad message size specified"));
                    }
                }
            }

            public async Task<string[]> GetEHLOKeywords()
            {
                
                    long? maxMessageSize = await this.Connection.Server.Behaviour.GetMaximumMessageSize(this.Connection).ConfigureAwait(false);

                    if (maxMessageSize.HasValue)
                    {
                        return new[] { string.Format("SIZE={0}", maxMessageSize.Value) };
                    }
                    else
                    {
                        return new[] { "SIZE" };
                    }
                
            }
        }

    }
}