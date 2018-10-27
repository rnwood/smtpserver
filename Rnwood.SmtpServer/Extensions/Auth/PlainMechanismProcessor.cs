namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System.Threading.Tasks;

    public class PlainMechanismProcessor : AuthMechanismProcessor, IAuthMechanismProcessor
    {
        public enum States
        {
            Initial,
            AwaitingResponse
        }

        public PlainMechanismProcessor(IConnection connection) : base(connection)
        {
        }

        private States State { get; set; }

        public async override Task<AuthMechanismProcessorStatus> ProcessResponseAsync(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                if (this.State == States.AwaitingResponse)
                {
                    throw new SmtpServerException(new SmtpResponse(
                        StandardSmtpResponseCode.AuthenticationFailure,
                                                                   "Missing auth data"));
                }

                await this.Connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.AuthenticationContinue, "")).ConfigureAwait(false);
                this.State = States.AwaitingResponse;
                return AuthMechanismProcessorStatus.Continue;
            }

            string decodedData = DecodeBase64(data);
            string[] decodedDataParts = decodedData.Split('\0');

            if (decodedDataParts.Length != 3)
            {
                throw new SmtpServerException(new SmtpResponse(
                    StandardSmtpResponseCode.AuthenticationFailure,
                                                               "Auth data in incorrect format"));
            }

            string username = decodedDataParts[1];
            string password = decodedDataParts[2];

            this.Credentials = new PlainAuthenticationCredentials(username, password);

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
    }
}