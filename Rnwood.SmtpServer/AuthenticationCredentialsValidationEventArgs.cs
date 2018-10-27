namespace Rnwood.SmtpServer
{
    using System;
    using Rnwood.SmtpServer.Extensions.Auth;

    public class AuthenticationCredentialsValidationEventArgs : EventArgs
    {
        public AuthenticationCredentialsValidationEventArgs(IAuthenticationCredentials credentials)
        {
            this.Credentials = credentials;
        }

        public IAuthenticationCredentials Credentials { get; private set; }

        public AuthenticationResult AuthenticationResult { get; set; }
    }
}