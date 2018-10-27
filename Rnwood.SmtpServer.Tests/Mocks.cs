using Moq;
using Rnwood.SmtpServer.Verbs;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Rnwood.SmtpServer.Tests
{
    public class Mocks
    {
        public Mocks()
        {
            this.Connection = new Mock<IConnection>();
            this.ConnectionChannel = new Mock<IConnectionChannel>();
            this.Session = new Mock<IEditableSession>();
            this.Server = new Mock<IServer>();
            this.ServerBehaviour = new Mock<IServerBehaviour>();
            this.MessageBuilder = new Mock<IMessageBuilder>();
            this.VerbMap = new Mock<IVerbMap>();

            this.ServerBehaviour.Setup(
                sb => sb.OnCreateNewSession(It.IsAny<IConnectionChannel>())).
                ReturnsAsync(this.Session.Object);
            this.ServerBehaviour.Setup(sb => sb.OnCreateNewMessage(It.IsAny<IConnection>())).ReturnsAsync(this.MessageBuilder.Object);

            this.Connection.SetupGet(c => c.Session).Returns(this.Session.Object);
            this.Connection.SetupGet(c => c.Server).Returns(this.Server.Object);
            this.Connection.SetupGet(c => c.ReaderEncoding).Returns(new ASCIISevenBitTruncatingEncoding());
            this.Connection.Setup(s => s.CloseConnection()).Returns(() => this.ConnectionChannel.Object.CloseAync());

            this.Server.SetupGet(s => s.Behaviour).Returns(this.ServerBehaviour.Object);

            bool isConnected = true;
            this.ConnectionChannel.Setup(s => s.IsConnected).Returns(() => isConnected);
            this.ConnectionChannel.Setup(s => s.CloseAync()).Returns(() => Task.Run(() => isConnected = false));
            this.ConnectionChannel.Setup(s => s.ClientIPAddress).Returns(IPAddress.Loopback);
        }

        public Mock<IConnection> Connection { get; private set; }

        public Mock<IConnectionChannel> ConnectionChannel { get; private set; }

        public Mock<IEditableSession> Session { get; private set; }

        public Mock<IServer> Server { get; private set; }

        public Mock<IServerBehaviour> ServerBehaviour { get; private set; }

        public Mock<IMessageBuilder> MessageBuilder { get; private set; }

        public Mock<IVerbMap> VerbMap { get; private set; }

        public void VerifyWriteResponseAsync(StandardSmtpResponseCode responseCode)
        {
            this.Connection.Verify(c => c.WriteResponse(It.Is<SmtpResponse>(r => r.Code == (int)responseCode)));
        }
    }
}