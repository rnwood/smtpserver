

namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class CramMd5Mechanism : IAuthMechanism
    {
        public string Identifier => "CRAM-MD5";

        public IAuthMechanismProcessor CreateAuthMechanismProcessor(IConnection connection)
        {
            return new CramMd5MechanismProcessor(connection, new RandomIntegerGenerator(), new CurrentDateTimeProvider());
        }

        public bool IsPlainText => false;
    }
}