namespace Rnwood.SmtpServer
{
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Verbs;

    public class MailVerb : IVerb
    {
        public MailVerb()
        {
            this.SubVerbMap = new VerbMap();
            this.SubVerbMap.SetVerbProcessor("FROM", new MailFromVerb());
        }

        public VerbMap SubVerbMap { get; private set; }

        public MailFromVerb FromSubVerb => (MailFromVerb)this.SubVerbMap.GetVerbProcessor("FROM");

        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            SmtpCommand subrequest = new SmtpCommand(command.ArgumentsText);
            IVerb verbProcessor = this.SubVerbMap.GetVerbProcessor(subrequest.Verb);

            if (verbProcessor != null)
            {
                await verbProcessor.ProcessAsync(connection, subrequest).ConfigureAwait(false);
            }
            else
            {
                await connection.WriteResponse(
                    new SmtpResponse(
                        StandardSmtpResponseCode.CommandParameterNotImplemented,
                                     "Subcommand {0} not implemented", subrequest.Verb)).ConfigureAwait(false);
            }
        }
    }
}