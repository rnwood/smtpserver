using Xunit;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rnwood.SmtpServer.Tests
{
    public abstract class MessageBuilderTests
    {
        [Fact]
        public void AddTo()
        {
            IMessageBuilder builder = this.GetInstance();

            builder.To.Add("foo@bar.com");
            builder.To.Add("bar@foo.com");

            Assert.Equal(2, builder.To.Count);
            Assert.Equal("foo@bar.com", builder.To.ElementAt(0));
            Assert.Equal("bar@foo.com", builder.To.ElementAt(1));
        }

        protected abstract IMessageBuilder GetInstance();

        [Fact]
        public async Task WriteData_Accepted()
        {
            IMessageBuilder builder = this.GetInstance();

            byte[] writtenBytes = new byte[64 * 1024];
            new Random().NextBytes(writtenBytes);

            using (Stream stream = await  builder.WriteData().ConfigureAwait(false))
            {
                stream.Write(writtenBytes, 0, writtenBytes.Length);
            }

            byte[] readBytes;
            using (Stream stream = await builder.GetData().ConfigureAwait(false))
            {
                readBytes = new byte[stream.Length];
                stream.Read(readBytes, 0, readBytes.Length);
            }

            Assert.Equal(writtenBytes, readBytes);
        }
    }
}