namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public class LoginMechanismProcessor : AuthMechanismProcessor
    {
        public LoginMechanismProcessor(IConnection connection) : base(connection)
        {
            this.State = States.Initial;
        }

        private States State { get; set; }

        private string username;

        public async override Task<AuthMechanismProcessorStatus> ProcessResponseAsync(string data)
        {
            if (this.State == States.Initial && data != null)
            {
                this.State = States.WaitingForUsername;
            }

            switch (this.State)
            {
                case States.Initial:
                    await this.Connection.WriteResponse(new SmtpResponse(
                        StandardSmtpResponseCode.AuthenticationContinue,
                                                              Convert.ToBase64String(
                                                                  Encoding.ASCII.GetBytes("Username:")))).ConfigureAwait(false);
                    this.State = States.WaitingForUsername;
                    return AuthMechanismProcessorStatus.Continue;

                case States.WaitingForUsername:

                    this.username = DecodeBase64(data);

                    await this.Connection.WriteResponse(new SmtpResponse(
                        StandardSmtpResponseCode.AuthenticationContinue,
                                                              Convert.ToBase64String(
                                                                  Encoding.ASCII.GetBytes("Password:")))).ConfigureAwait(false);
                    this.State = States.WaitingForPassword;
                    return AuthMechanismProcessorStatus.Continue;

                case States.WaitingForPassword:
                    string password = DecodeBase64(data);
                    this.State = States.Completed;

                    this.Credentials = new LoginAuthenticationCredentials(this.username, password);

                    AuthenticationResult result =
                        await this.Connection.Server.Behaviour.ValidateAuthenticationCredentialsAsync(
                            this.Connection,
                                                                                      this.Credentials).ConfigureAwait(false);

                    switch (result)
                    {
                        case AuthenticationResult.Success:
                            return AuthMechanismProcessorStatus.Success;

                        default:
                            return AuthMechanismProcessorStatus.Failed;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private enum States
        {
            Initial,
            WaitingForUsername,
            WaitingForPassword,
            Completed
        }
    }
}