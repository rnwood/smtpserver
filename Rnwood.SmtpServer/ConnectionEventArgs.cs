namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ConnectionEventArgs : EventArgs
    {
        public ConnectionEventArgs(IConnection connection)
        {
            this.Connection = connection;
        }

        public IConnection Connection { get; private set; }
    }
}