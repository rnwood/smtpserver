namespace Rnwood.SmtpServer.Verbs
{
    using System.Threading.Tasks;

    public interface IVerb
    {
        Task ProcessAsync(Rnwood.SmtpServer.IConnection connection, Rnwood.SmtpServer.SmtpCommand command);
    }
}