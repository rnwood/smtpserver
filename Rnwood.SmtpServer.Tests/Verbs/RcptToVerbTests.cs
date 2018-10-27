using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    public class RcptToVerbTests
    {
        [Fact]
        public async Task EmailAddressOnly()
        {
            await this.TestGoodAddressAsync("<rob@rnwood.co.uk>", "rob@rnwood.co.uk").ConfigureAwait(false);
        }

        [Fact]
        public async Task EmailAddressWithDisplayName()
        {
            //Should this format be accepted????
            await this.TestGoodAddressAsync("<Robert Wood<rob@rnwood.co.uk>>", "Robert Wood<rob@rnwood.co.uk>").ConfigureAwait(false);
        }

        private async Task TestGoodAddressAsync(string address, string expectedAddress)
        {
            Mocks mocks = new Mocks();
            MemoryMessage.Builder messageBuilder = new MemoryMessage.Builder();
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(messageBuilder);

            RcptToVerb verb = new RcptToVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("TO " + address)).ConfigureAwait(false);

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.OK);
            Assert.Equal(expectedAddress, messageBuilder.To.First());
        }

        [Fact]
        public async Task UnbraketedAddress_ReturnsError()
        {
            await this.TestBadAddressAsync("rob@rnwood.co.uk").ConfigureAwait(false);
        }

        [Fact]
        public async Task MismatchedBraket_ReturnsError()
        {
            await this.TestBadAddressAsync("<rob@rnwood.co.uk").ConfigureAwait(false);
            await this.TestBadAddressAsync("<Robert Wood<rob@rnwood.co.uk>").ConfigureAwait(false);
        }

        [Fact]
        public async Task EmptyAddress_ReturnsError()
        {
            await this.TestBadAddressAsync("<>").ConfigureAwait(false);
        }

        private async Task TestBadAddressAsync(string address)
        {
            Mocks mocks = new Mocks();
            MemoryMessage.Builder messageBuilder = new MemoryMessage.Builder();
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(messageBuilder);

            RcptToVerb verb = new RcptToVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("TO " + address)).ConfigureAwait(false);

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.SyntaxErrorInCommandArguments);
            Assert.Equal(0, messageBuilder.To.Count);
        }
    }
}