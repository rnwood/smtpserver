namespace Rnwood.SmtpServer
{
    using System;

    public interface ICurrentDateTimeProvider
    {
        DateTime GetCurrentDateTime();
    }
}