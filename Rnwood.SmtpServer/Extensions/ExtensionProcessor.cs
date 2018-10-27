namespace Rnwood.SmtpServer.Extensions
{
    using System.Threading.Tasks;

    public abstract class ExtensionProcessor : IExtensionProcessor
    {
        public ExtensionProcessor(IConnection connection)
        {
            this.Connection = connection;
        }

        public IConnection Connection { get; private set; }

        public abstract Task<string[]> GetEHLOKeywords();
    }
}