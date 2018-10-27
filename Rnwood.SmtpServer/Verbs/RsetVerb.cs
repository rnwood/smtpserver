namespace Rnwood.SmtpServer.Verbs
{
    using System.Threading.Tasks;

    public class RsetVerb : IVerb
    {
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            await connection.AbortMessage().ConfigureAwait(false);
            await connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.OK, "Rset completed")).ConfigureAwait(false);
        }
    }
}