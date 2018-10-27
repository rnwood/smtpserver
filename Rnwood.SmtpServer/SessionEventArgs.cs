namespace Rnwood.SmtpServer
{
    using System;

    public class SessionEventArgs : EventArgs
    {
        public SessionEventArgs(ISession session)
        {
            this.Session = session;
        }

        public ISession Session { get; private set; }
    }
}