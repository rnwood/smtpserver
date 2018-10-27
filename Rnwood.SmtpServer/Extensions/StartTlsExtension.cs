namespace Rnwood.SmtpServer.Extensions
{
    using System;
    using System.Threading.Tasks;

    public class StartTlsExtension : IExtension
    {
        public IExtensionProcessor CreateExtensionProcessor(IConnection connection)
        {
            return new StartTlsExtensionProcessor(connection);
        }

        private class StartTlsExtensionProcessor : IExtensionProcessor
        {
            public StartTlsExtensionProcessor(IConnection connection)
            {
                this.Connection = connection;
                this.Connection.VerbMap.SetVerbProcessor("STARTTLS", new StartTlsVerb());
            }

            public IConnection Connection { get; private set; }

            public Task<string[]> GetEHLOKeywords()
            {

                string[] result;

                if (!this.Connection.Session.SecureConnection)
                {
                    result = new[] { "STARTTLS" };
                }
                result = Array.Empty<string>();

                return Task.FromResult(result);

            }
        }
    }
}