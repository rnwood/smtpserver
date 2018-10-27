namespace Rnwood.SmtpServer.Extensions.Auth
{
    public abstract class UsernameAndPasswordAuthenticationCredentials : IAuthenticationCredentials
    {
        public UsernameAndPasswordAuthenticationCredentials(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public string Username { get; private set; }

        public string Password { get; private set; }
    }
}