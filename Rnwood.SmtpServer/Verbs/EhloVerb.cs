namespace Rnwood.SmtpServer
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Verbs;

    public class EhloVerb : IVerb
    {
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            if (!string.IsNullOrEmpty(connection.Session.ClientName))
            {
                await connection.WriteResponse(
                    new SmtpResponse(
                        StandardSmtpResponseCode.BadSequenceOfCommands,
                        "You already said HELO")).ConfigureAwait(false);
                return;
            }

            connection.Session.ClientName = command.ArgumentsText ?? string.Empty;

            StringBuilder text = new StringBuilder();
            text.AppendLine("Nice to meet you.");

            foreach (Extensions.IExtensionProcessor extensionProcessor in connection.ExtensionProcessors)
            {
                foreach (string ehloKeyword in await extensionProcessor.GetEHLOKeywords().ConfigureAwait(false))
                {
                    text.AppendLine(ehloKeyword);
                }
            }

            await connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.OK, text.ToString().TrimEnd())).ConfigureAwait(false);
        }
    }
}