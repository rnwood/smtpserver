namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System.Threading.Tasks;

    public class AnonymousMechanismProcessor : IAuthMechanismProcessor
    {
        public AnonymousMechanismProcessor(IConnection connection)
        {
            this.Connection = connection;
        }

        protected IConnection Connection { get; private set; }

        public async Task<AuthMechanismProcessorStatus> ProcessResponseAsync(string data)
        {
            this.Credentials = new AnonymousAuthenticationCredentials();

            AuthenticationResult result =
                await this.Connection.Server.Behaviour.ValidateAuthenticationCredentialsAsync(this.Connection, this.Credentials).ConfigureAwait(false);

            switch (result)
            {
                case AuthenticationResult.Success:
                    return AuthMechanismProcessorStatus.Success;

                default:
                    return AuthMechanismProcessorStatus.Failed;
            }
        }

        public IAuthenticationCredentials Credentials
        {
            get;
            private set;
        }
    }
}