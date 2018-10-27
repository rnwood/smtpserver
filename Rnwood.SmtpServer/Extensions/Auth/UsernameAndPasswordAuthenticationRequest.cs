namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class UsernameAndPasswordAuthenticationRequest : IAuthenticationCredentials
    {
        public UsernameAndPasswordAuthenticationRequest(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public string Username { get; private set; }

        public string Password { get; private set; }
    }
}