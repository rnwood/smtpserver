using MailKit;
using System;
using System.Globalization;
using System.Text;
using Xunit.Abstractions;

namespace Rnwood.SmtpServer.Tests
{
    public partial class ClientTests
    {
        internal class SmtpClientLogger : IProtocolLogger
        {
            private readonly ITestOutputHelper testOutput;

            public SmtpClientLogger(ITestOutputHelper testOutput)
            {
                this.testOutput = testOutput;
            }

            public void Dispose()
            {
                this.testOutput.WriteLine($"*** DISCONNECT");
            }

            public void LogClient(byte[] buffer, int offset, int count)
            {
                

                this.testOutput.WriteLine(">>> " + Encoding.UTF8.GetString(buffer, offset, count).Replace(Environment.NewLine, "\n"));
            }

            public void LogConnect(Uri uri)
            {
                this.testOutput.WriteLine($"*** CONNECT {uri}");
            }

            public void LogServer(byte[] buffer, int offset, int count)
            {
                this.testOutput.WriteLine("<<< " + Encoding.UTF8.GetString(buffer, offset, count).Replace(Environment.NewLine, "\\n"));
            }
        }
    }
}