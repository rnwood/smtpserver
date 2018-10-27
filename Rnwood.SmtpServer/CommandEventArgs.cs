namespace Rnwood.SmtpServer
{
    using System;

    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(SmtpCommand command)
        {
            this.Command = command;
        }

        public SmtpCommand Command { get; private set; }
    }
}