namespace Rnwood.SmtpServer
{
    using System;
    using System.IO;
    using System.Net;

    public class FileSession : AbstractSession
    {
        public FileSession(IPAddress clientAddress, DateTime startDate, FileInfo file, bool keepOnDispose) : base(clientAddress, startDate)
        {
            this.file = file;
            this.keepOnDispose = keepOnDispose;
        }

        private readonly FileInfo file;
        private readonly bool keepOnDispose;

        public override TextReader GetLog()
        {
            return this.file.OpenText();
        }

        public override void AppendToLog(string text)
        {
            using (StreamWriter writer = this.file.AppendText())
            {
                writer.WriteLine(text);
            }
        }

        public override void Dispose()
        {
            if (!this.keepOnDispose)
            {
                if (this.file.Exists)
                {
                    this.file.Delete();
                }
            }
        }
    }
}