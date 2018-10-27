using Moq;
using Rnwood.SmtpServer.Extensions.Auth;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Extensions.Auth
{
    public class LoginMechanismProcessorTests : AuthMechanismTest
    {
        [Fact]
        public async Task ProcessRepsonse_NoUsername_GetUsernameChallenge()
        {
            Mocks mocks = new Mocks();

            LoginMechanismProcessor processor = this.Setup(mocks);
            AuthMechanismProcessorStatus result = await processor.ProcessResponseAsync(null).ConfigureAwait(false);

            Assert.Equal(AuthMechanismProcessorStatus.Continue, result);
            mocks.Connection.Verify(c =>
                c.WriteResponse(
                        It.Is<SmtpResponse>(r =>
                           r.Code == (int)StandardSmtpResponseCode.AuthenticationContinue &&
                           VerifyBase64Response(r.Message, "Username:")
                        )
                )
            );
        }

        [Fact]
        public async Task ProcessRepsonse_Username_GetPasswordChallenge()
        {
            Mocks mocks = new Mocks();

            LoginMechanismProcessor processor = this.Setup(mocks);
            AuthMechanismProcessorStatus result = await processor.ProcessResponseAsync(EncodeBase64("rob")).ConfigureAwait(false);

            Assert.Equal(AuthMechanismProcessorStatus.Continue, result);

            mocks.Connection.Verify(c =>
                c.WriteResponse(
                    It.Is<SmtpResponse>(r =>
                        VerifyBase64Response(r.Message, "Password:")
                        && r.Code == (int)StandardSmtpResponseCode.AuthenticationContinue
                    )
                )
            );
        }

        [Fact]
        public async Task ProcessResponse_PasswordAcceptedAfterUserNameInInitialRequest()
        {
            Mocks mocks = new Mocks();

            LoginMechanismProcessor processor = this.Setup(mocks);
            AuthMechanismProcessorStatus result = await processor.ProcessResponseAsync(EncodeBase64("rob")).ConfigureAwait(false);

            Assert.Equal(AuthMechanismProcessorStatus.Continue, result);

            mocks.Connection.Verify(c =>
                c.WriteResponse(
                    It.Is<SmtpResponse>(r =>
                        VerifyBase64Response(r.Message, "Password:")
                        && r.Code == (int)StandardSmtpResponseCode.AuthenticationContinue
                    )
                )
            );

            result = await processor.ProcessResponseAsync(EncodeBase64("password")).ConfigureAwait(false);
            Assert.Equal(AuthMechanismProcessorStatus.Success, result);
        }


        [Fact]
        public async Task ProcessResponse_Response_BadBase64()
        {
            await Assert.ThrowsAsync<BadBase64Exception>(async () =>
            {
                Mocks mocks = new Mocks();

                LoginMechanismProcessor processor = this.Setup(mocks);
                await processor.ProcessResponseAsync(null).ConfigureAwait(false);
                await processor.ProcessResponseAsync("rob blah").ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        private LoginMechanismProcessor Setup(Mocks mocks)
        {
            return new LoginMechanismProcessor(mocks.Connection.Object);
        }
    }
}