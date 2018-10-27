namespace Rnwood.SmtpServer
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public class DefaultServer : Server
    {
        /// <summary>
        /// Initializes a new SMTP over SSL server on port 465 using the
        /// supplied SSL certificate.
        /// </summary>
        /// <param name="sslCertificate">The SSL certificate to use for the server.</param>
        public DefaultServer(bool allowRemoteConnection, X509Certificate sslCertificate)
            : this(allowRemoteConnection, 465, sslCertificate)
        {
        }

        /// <summary>
        /// Initializes a new SMTP server on port 25.
        /// </summary>
        public DefaultServer(bool allowRemoteConnection)
            : this(allowRemoteConnection, 25, null)
        {
        }

        /// <summary>
        /// Initializes a new SMTP server on the specified port number.
        /// </summary>
        /// <param name="portNumber">The port number.</param>
        public DefaultServer(bool allowRemoteConnection, int portNumber)
            : this(allowRemoteConnection, portNumber, null)
        {
        }

        /// <summary>
        /// Initializes a new SMTP over SSL server on the specified port number
        /// using the supplied SSL certificate.
        /// </summary>
        /// <param name="portNumber">The port number.</param>
        /// <param name="sslCertificate">The SSL certificate.</param>
        public DefaultServer(bool allowRemoteConnection, int portNumber, X509Certificate sslCertificate)
            : this(new DefaultServerBehaviour(allowRemoteConnection, portNumber, sslCertificate))
        {
        }

        /// <summary>
        /// Initializes a new SMTP over SSL server on the specified standard port number
        /// </summary>
        /// <param name="port">The standard port (or auto) to use.</param>
        public DefaultServer(bool allowRemoteConnection, Ports port)
            : this(new DefaultServerBehaviour(allowRemoteConnection, (int)port))
        {
        }

        private DefaultServer(DefaultServerBehaviour behaviour) : base(behaviour)
        {
        }

        new protected DefaultServerBehaviour Behaviour => (DefaultServerBehaviour)base.Behaviour;

        public event EventHandler<MessageEventArgs> MessageReceived
        {
            add { this.Behaviour.MessageReceived += value; }
            remove { this.Behaviour.MessageReceived -= value; }
        }

        public event EventHandler<SessionEventArgs> SessionCompleted
        {
            add { this.Behaviour.SessionCompleted += value; }
            remove { this.Behaviour.SessionCompleted -= value; }
        }

        public event EventHandler<SessionEventArgs> SessionStarted
        {
            add { this.Behaviour.SessionStarted += value; }
            remove { this.Behaviour.SessionStarted -= value; }
        }

        public event Func<object, AuthenticationCredentialsValidationEventArgs, Task> AuthenticationCredentialsValidationRequiredAsync
        {
            add { this.Behaviour.AuthenticationCredentialsValidationRequiredAsync += value; }
            remove { this.Behaviour.AuthenticationCredentialsValidationRequiredAsync -= value; }
        }

        public event EventHandler<ConnectionEventArgs> MessageCompleted
        {
            add { this.Behaviour.MessageCompleted += value; }
            remove { this.Behaviour.MessageCompleted -= value; }
        }
    }
}