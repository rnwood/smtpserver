using Moq;
using Rnwood.SmtpServer.Verbs;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public async Task Process_GreetingWritten()
        {
            Mocks mocks = new Mocks();
            mocks.ConnectionChannel.Setup(c => c.WriteLineAsync(It.IsAny<string>())).Callback(() => mocks.Connection.Object.CloseConnection().Wait());

            Connection connection = await Connection.Create(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object).ConfigureAwait(false);
            await connection.ProcessAsync().ConfigureAwait(false);

            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.IsRegex("220 .*", RegexOptions.IgnoreCase)));
        }

        [Fact]
        public async Task Process_SmtpServerExceptionThrow_ResponseWritten()
        {
            Mocks mocks = new Mocks();
            Mock<IVerb> mockVerb = new Mock<IVerb>();
            mocks.VerbMap.Setup(v => v.GetVerbProcessor(It.IsAny<string>())).Returns(mockVerb.Object);
            mockVerb.Setup(v => v.ProcessAsync(It.IsAny<IConnection>(), It.IsAny<SmtpCommand>())).Returns(Task.FromException(new SmtpServerException(new SmtpResponse(500, "error"))));

            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("GOODCOMMAND").Callback(() => mocks.Connection.Object.CloseConnection().Wait());

            Connection connection = await Connection.Create(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object).ConfigureAwait(false);
            await connection.ProcessAsync().ConfigureAwait(false);

            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.IsRegex("500 error", RegexOptions.IgnoreCase)));
        }

        [Fact]
        public async Task Process_EmptyCommand_NoResponse()
        {
            Mocks mocks = new Mocks();

            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("").Callback(() => mocks.Connection.Object.CloseConnection().Wait());

            Connection connection = await Connection.Create(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object).ConfigureAwait(false);
            await connection.ProcessAsync().ConfigureAwait(false);

            // Should only print service ready message
            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.Is<string>(s => !s.StartsWith("220 ", StringComparison.OrdinalIgnoreCase))), Times.Never());
        }

        [Fact]
        public async Task Process_GoodCommand_Processed()
        {
            Mocks mocks = new Mocks();
            Mock<IVerb> mockVerb = new Mock<IVerb>();
            mocks.VerbMap.Setup(v => v.GetVerbProcessor(It.IsAny<string>())).Returns(mockVerb.Object).Callback(() => mocks.Connection.Object.CloseConnection().Wait());

            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("GOODCOMMAND");

            Connection connection = await Connection.Create(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object).ConfigureAwait(false);
            await connection.ProcessAsync().ConfigureAwait(false);

            mockVerb.Verify(v => v.ProcessAsync(It.IsAny<IConnection>(), It.IsAny<SmtpCommand>()));
        }

        [Fact]
        public async Task Process_BadCommand_500Response()
        {
            Mocks mocks = new Mocks();
            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("BADCOMMAND").Callback(() => mocks.Connection.Object.CloseConnection().Wait());

            Connection connection = await Connection.Create(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object).ConfigureAwait(false);
            await connection.ProcessAsync().ConfigureAwait(false);

            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.IsRegex("500 .*", RegexOptions.IgnoreCase)));
        }

        [Fact]
        public async Task Process_TooManyBadCommands_Disconnected()
        {
            Mocks mocks = new Mocks();
            mocks.ServerBehaviour.SetupGet(b => b.MaximumNumberOfSequentialBadCommands).Returns(2);

            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("BADCOMMAND");

            Connection connection = await Connection.Create(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object).ConfigureAwait(false);
            await connection.ProcessAsync().ConfigureAwait(false);

            mocks.ConnectionChannel.Verify(c => c.ReadLineAsync(), Times.Exactly(2));
            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.IsRegex("221 .*", RegexOptions.IgnoreCase)));
        }

        [Fact]
        public async Task AbortMessage()
        {
            Mocks mocks = new Mocks();

            Connection connection = await Connection.Create(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object).ConfigureAwait(false);
            await connection.NewMessage().ConfigureAwait(false);

            await connection.AbortMessage().ConfigureAwait(false);
            Assert.Null(connection.CurrentMessage);
        }

        [Fact]
        public async Task CommitMessage()
        {
            Mocks mocks = new Mocks();

            Connection connection = await Connection.Create(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object).ConfigureAwait(false);
            IMessageBuilder messageBuilder = await connection.NewMessage().ConfigureAwait(false);
            IMessage message = await messageBuilder.ToMessage().ConfigureAwait(false);

            await connection.CommitMessage().ConfigureAwait(false);
            mocks.Session.Verify(s => s.AddMessage(message));
            mocks.ServerBehaviour.Verify(b => b.OnMessageReceived(connection, message));
            Assert.Null(connection.CurrentMessage);
        }
    }
}