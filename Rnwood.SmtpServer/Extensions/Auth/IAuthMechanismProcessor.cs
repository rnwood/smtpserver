namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System.Threading.Tasks;

    public interface IAuthMechanismProcessor
    {
        Task<AuthMechanismProcessorStatus> ProcessResponseAsync(string data);

        IAuthenticationCredentials Credentials
        {
            get;
        }
    }
}