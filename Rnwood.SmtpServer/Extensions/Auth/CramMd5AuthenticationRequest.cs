namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class CramMd5AuthenticationRequest : IAuthenticationCredentials
    {
        public CramMd5AuthenticationRequest(string username, string challenge, string challengeResponse)
        {
            this.Username = username;
            this.ChallengeResponse = challengeResponse;
            this.Challenge = challenge;
        }

        public string Username { get; private set; }

        public string ChallengeResponse { get; private set; }

        public string Challenge { get; private set; }
    }
}