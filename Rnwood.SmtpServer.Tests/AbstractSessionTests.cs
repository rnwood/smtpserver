using Moq;
using System;
using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    public abstract class AbstractSessionTests
    {
        protected abstract IEditableSession GetSession();

        [Fact]
        public void AppendToLog()
        {
            IEditableSession session = this.GetSession();
            session.AppendToLog("Blah1");
            session.AppendToLog("Blah2");

            Assert.Equal(new[] { "Blah1", "Blah2", "" },
                                    session.GetLog().ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None));
        }

        [Fact]
        public void GetMessages_InitiallyEmpty()
        {
            IEditableSession session = this.GetSession();
            Assert.Empty( session.GetMessages());
        }

        [Fact]
        public void AddMessage()
        {
            IEditableSession session = this.GetSession();
            Mock<IMessage> message = new Mock<IMessage>();

            session.AddMessage(message.Object);

            Assert.Single(session.GetMessages());
            Assert.Same(message.Object, session.GetMessages()[0]);
        }
    }
}