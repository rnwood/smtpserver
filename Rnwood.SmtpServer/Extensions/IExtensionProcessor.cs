namespace Rnwood.SmtpServer.Extensions
{
    using System.Threading.Tasks;

    public interface IExtensionProcessor
    {
        Task<string[]> GetEHLOKeywords();
    }
}