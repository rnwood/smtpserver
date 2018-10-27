namespace Rnwood.SmtpServer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    public class MemorySession : AbstractSession
    {
        public MemorySession(IPAddress clientAddress, DateTime startDate) : base(clientAddress, startDate)
        {
        }

        private readonly StringBuilder log = new StringBuilder();

        public override TextReader GetLog()
        {
            return new StringReader(this.log.ToString());
        }

        public override void AppendToLog(string text)
        {
            this.log.AppendLine(text);
        }

        public override void Dispose()
        {
        }
    }
}