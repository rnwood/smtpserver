

namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class LoginMechanism : IAuthMechanism
    {
        public string Identifier => "LOGIN";

        public IAuthMechanismProcessor CreateAuthMechanismProcessor(IConnection connection)
        {
            return new LoginMechanismProcessor(connection);
        }

        public bool IsPlainText => true;
    }
}