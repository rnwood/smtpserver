namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public class CramMd5MechanismProcessor : AuthMechanismProcessor
    {
        private readonly IRandomIntegerGenerator random;
        private readonly ICurrentDateTimeProvider dateTimeProvider;

        private string challenge;

        public CramMd5MechanismProcessor(IConnection connection, IRandomIntegerGenerator random, ICurrentDateTimeProvider dateTimeProvider) : base(connection)
        {
            this.random = random;
            this.dateTimeProvider = dateTimeProvider;
        }

        public CramMd5MechanismProcessor(IConnection connection, IRandomIntegerGenerator random, ICurrentDateTimeProvider dateTimeProvider, string challenge)
            : this(connection, random, dateTimeProvider)
        {
            this.challenge = challenge;
        }

        public async override Task<AuthMechanismProcessorStatus> ProcessResponseAsync(string data)
        {
            if (this.challenge == null)
            {
                StringBuilder challenge = new StringBuilder();
                challenge.Append(this.random.GenerateRandomInteger(0, Int16.MaxValue));
                challenge.Append(".");
                challenge.Append(this.dateTimeProvider.GetCurrentDateTime().Ticks.ToString());
                challenge.Append("@");
                challenge.Append(this.Connection.Server.Behaviour.DomainName);
                this.challenge = challenge.ToString();

                string base64Challenge = Convert.ToBase64String(Encoding.ASCII.GetBytes(challenge.ToString()));
                await this.Connection.WriteResponse(new SmtpResponse(
                    StandardSmtpResponseCode.AuthenticationContinue,
                                                          base64Challenge)).ConfigureAwait(false);
                return AuthMechanismProcessorStatus.Continue;
            }
            else
            {
                string response = DecodeBase64(data);
                string[] responseparts = response.Split(' ');

                if (responseparts.Length != 2)
                {
                    throw new SmtpServerException(new SmtpResponse(
                        StandardSmtpResponseCode.AuthenticationFailure,
                                                                   "Response in incorrect format - should be USERNAME RESPONSE"));
                }

                string username = responseparts[0];
                string hash = responseparts[1];

                this.Credentials = new CramMd5AuthenticationCredentials(username, this.challenge, hash);

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

        private enum States
        {
            Initial,
            AwaitingResponse
        }
    }
}