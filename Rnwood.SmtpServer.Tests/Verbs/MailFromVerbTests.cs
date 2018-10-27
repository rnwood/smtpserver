using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    public class MailFromVerbTests
    {
        [Fact]
        public async Task Process_AlreadyGivenFrom_ErrorResponse()
        {
            Mocks mocks = new Mocks();
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(new Mock<IMessageBuilder>().Object);

            MailFromVerb mailFromVerb = new MailFromVerb();
            await mailFromVerb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("FROM <foo@bar.com>")).ConfigureAwait(false);

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.BadSequenceOfCommands);
        }

        [Fact]
        public async Task Process_MissingAddress_ErrorResponse()
        {
            Mocks mocks = new Mocks();

            MailFromVerb mailFromVerb = new MailFromVerb();
            await mailFromVerb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("FROM")).ConfigureAwait(false);

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.SyntaxErrorInCommandArguments);
        }

        [Fact]
        public async Task Process_Address_Plain()
        {
            await this.Process_AddressAsync("rob@rnwood.co.uk", "rob@rnwood.co.uk", StandardSmtpResponseCode.OK).ConfigureAwait(false);
        }

        [Fact]
        public async Task Process_Address_Bracketed()
        {
            await this.Process_AddressAsync("<rob@rnwood.co.uk>", "rob@rnwood.co.uk", StandardSmtpResponseCode.OK).ConfigureAwait(false);
        }

        [Fact]
        public async Task Process_Address_BracketedWithName()
        {
            await this.Process_AddressAsync("<Robert Wood <rob@rnwood.co.uk>>", "Robert Wood <rob@rnwood.co.uk>", StandardSmtpResponseCode.OK).ConfigureAwait(false);
        }

        private async Task Process_AddressAsync(string address, string expectedParsedAddress, StandardSmtpResponseCode expectedResponse)
        {
            Mocks mocks = new Mocks();
            Mock<IMessageBuilder> message = new Mock<IMessageBuilder>();
            IMessageBuilder currentMessage = null;
            mocks.Connection.Setup(c => c.NewMessage()).ReturnsAsync(() =>
            {
                currentMessage = message.Object;
                return currentMessage;
            });
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(() => currentMessage);

            MailFromVerb mailFromVerb = new MailFromVerb();
            await mailFromVerb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("FROM " + address)).ConfigureAwait(false);

            mocks.VerifyWriteResponseAsync(expectedResponse);
            message.VerifySet(m => m.From = expectedParsedAddress);
        }
    }
}