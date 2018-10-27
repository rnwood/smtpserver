namespace Rnwood.SmtpServer
{
    using System;

    public class CurrentDateTimeProvider : ICurrentDateTimeProvider
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}