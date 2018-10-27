namespace Rnwood.SmtpServer
{
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Verbs;

    public class RcptVerb : IVerb
    {
        public RcptVerb()
        {
            this.SubVerbMap = new VerbMap();
            this.SubVerbMap.SetVerbProcessor("TO", new RcptToVerb());
        }

        public VerbMap SubVerbMap { get; private set; }

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