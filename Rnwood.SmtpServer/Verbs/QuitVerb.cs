namespace Rnwood.SmtpServer
{
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Verbs;

    public class QuitVerb : IVerb
    {
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            await connection.WriteResponse(new SmtpResponse(
                StandardSmtpResponseCode.ClosingTransmissionChannel,
                                                               "Goodbye")).ConfigureAwait(false);
            await connection.CloseConnection().ConfigureAwait(false);
            connection.Session.CompletedNormally = true;
        }
    }
}