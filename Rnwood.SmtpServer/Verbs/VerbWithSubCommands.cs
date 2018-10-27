namespace Rnwood.SmtpServer.Verbs
{
    using System.Threading.Tasks;

    public abstract class VerbWithSubCommands : IVerb
    {
        protected VerbWithSubCommands() : this(new VerbMap())
        {
        }

        protected VerbWithSubCommands(IVerbMap subVerbMap)
        {
            this.SubVerbMap = subVerbMap;
        }

        public IVerbMap SubVerbMap { get; private set; }

        public async virtual Task ProcessAsync(IConnection connection, SmtpCommand command)
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