namespace Rnwood.SmtpServer.Verbs
{
    using System.Threading.Tasks;

    public class NoopVerb : IVerb
    {
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            await connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.OK, "Successfully did nothing")).ConfigureAwait(false);
        }
    }
}