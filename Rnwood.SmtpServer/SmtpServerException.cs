namespace Rnwood.SmtpServer
{
    using System;

    public class SmtpServerException : Exception
    {
        public SmtpServerException(SmtpResponse smtpResponse) : base(smtpResponse.Message)
        {
            this.SmtpResponse = smtpResponse;
        }

        public SmtpServerException(SmtpResponse smtpResponse, Exception innerException)
            : base(smtpResponse.Message, innerException)
        {
            this.SmtpResponse = smtpResponse;
        }

        public SmtpResponse SmtpResponse { get; private set; }
    }
}