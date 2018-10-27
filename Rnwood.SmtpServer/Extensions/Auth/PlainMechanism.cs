

namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class PlainMechanism : IAuthMechanism
    {
        public string Identifier => "PLAIN";

        public IAuthMechanismProcessor CreateAuthMechanismProcessor(IConnection connection)
        {
            return new PlainMechanismProcessor(connection);
        }

        public bool IsPlainText => true;
    }
}