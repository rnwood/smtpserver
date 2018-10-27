namespace Rnwood.SmtpServer.Extensions
{
    using System.Threading.Tasks;

    public class EightBitMimeExtension : IExtension
    {
        public EightBitMimeExtension()
        {
        }

        public IExtensionProcessor CreateExtensionProcessor(IConnection connection)
        {
            return new EightBitMimeExtensionProcessor(connection);
        }

        private class EightBitMimeExtensionProcessor : ExtensionProcessor
        {
            public EightBitMimeExtensionProcessor(IConnection connection)
                : base(connection)
            {
                EightBitMimeDataVerb verb = new EightBitMimeDataVerb();
                connection.VerbMap.SetVerbProcessor("DATA", verb);

                MailVerb mailVerbProcessor = connection.MailVerb;
                MailFromVerb mailFromProcessor = mailVerbProcessor.FromSubVerb;
                mailFromProcessor.ParameterProcessorMap.SetProcessor("BODY", verb);
            }

            public override Task<string[]> GetEHLOKeywords()
            {
                return Task.FromResult(new[] { "8BITMIME" });
            }
        }
    }
}