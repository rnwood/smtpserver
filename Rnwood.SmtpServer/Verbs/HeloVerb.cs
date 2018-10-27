namespace Rnwood.SmtpServer
{
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Verbs;

    public class HeloVerb : IVerb
    {
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            if (!string.IsNullOrEmpty(connection.Session.ClientName))
            {
                await connection.WriteResponse(new SmtpResponse(
                    StandardSmtpResponseCode.BadSequenceOfCommands,
                                                                   "You already said HELO")).ConfigureAwait(false);
                return;
            }

            connection.Session.ClientName = command.ArgumentsText ?? "";
            await connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.OK, "Nice to meet you")).ConfigureAwait(false);
        }
    }
}