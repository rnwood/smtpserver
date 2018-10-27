namespace Rnwood.SmtpServer
{
    using System;

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(IMessage message)
        {
            this.Message = message;
        }

        public IMessage Message { get; private set; }
    }
}