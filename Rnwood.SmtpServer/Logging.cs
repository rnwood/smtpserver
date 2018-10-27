namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public static class Logging
    {
        private static readonly ILoggerFactory FactoryValue = new LoggerFactory();

        public static ILoggerFactory Factory => FactoryValue;
    }
}