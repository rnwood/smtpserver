namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class CramMd5AuthenticationCredentials : IAuthenticationCredentials
    {
        public CramMd5AuthenticationCredentials(string username, string challenge, string challengeResponse)
        {
            this.Username = username;
            this.ChallengeResponse = challengeResponse;
            this.Challenge = challenge;
        }

        public string Username { get; private set; }

        public string ChallengeResponse { get; private set; }

        public string Challenge { get; private set; }


        public bool ValidateResponse(string password)
        {
#pragma warning disable CA5351
            HMACMD5 hmacmd5 = new HMACMD5(ASCIIEncoding.ASCII.GetBytes(password));
            string expectedResponse = BitConverter.ToString(hmacmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(this.Challenge))).Replace("-", "");
#pragma warning restore CA5351

            return string.Equals(expectedResponse, this.ChallengeResponse, StringComparison.OrdinalIgnoreCase);
        }
    }
}