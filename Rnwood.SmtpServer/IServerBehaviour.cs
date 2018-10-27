namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Extensions;
    using Rnwood.SmtpServer.Extensions.Auth;

    public interface IServerBehaviour
    {
        /// <summary>
        /// Gets domain name reported by the server to clients.
        /// </summary>
        /// <value>The domain name report by the server to clients.</value>
        string DomainName { get; }

        /// <summary>
        /// Gets the IP address on which to listen for connections.
        /// </summary>
        /// <value>The IP address.</value>
        IPAddress IpAddress { get; }

        /// <summary>
        /// Gets the TCP port number on which to listen for connections.
        /// </summary>
        /// <value>The TCP port number.</value>
        int PortNumber { get; }

        int MaximumNumberOfSequentialBadCommands { get; }

        /// <summary>
        /// Gets a value indicating whether to run in SSL mode.
        /// </summary>
        /// <value><c>true</c> if the server should run in SSL mode otherwise, <c>false</c>.</value>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task<bool> IsSSLEnabled(IConnection connection);

        Task OnMessageReceived(IConnection connection, IMessage message);

        Task OnMessageRecipientAdding(IConnection connection, IMessageBuilder message, string recipient);

        /// <summary>
        /// Gets the maximum allowed size of the message for the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        Task<long?> GetMaximumMessageSize(IConnection connection);

        /// <summary>
        /// Gets the SSL certificate that should be used for the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        Task<X509Certificate> GetSSLCertificate(IConnection connection);

        Task<bool> IsSessionLoggingEnabled(IConnection connection);

        /// <summary>
        /// Gets the extensions that should be enabled for the specified connection.
        /// </summary>
        /// <param name="connection">The connection channel.</param>
        /// <returns></returns>
        Task<IEnumerable<IExtension>> GetExtensions(IConnectionChannel connectionChannel);

        /// <summary>
        /// Called when a SMTP session is completed.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="Session">The session.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task OnSessionCompleted(IConnection connection, ISession Session);

        /// <summary>
        /// Called when a new SMTP session is started.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="session">The session.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task OnSessionStarted(IConnection connection, ISession session);

        /// <summary>
        /// Gets the receive timeout that should be used for the specified connection.
        /// </summary>
        /// <param name="connectionChannel">The connection channel.</param>
        /// <returns></returns>
        Task<TimeSpan> GetReceiveTimeout(IConnectionChannel connectionChannel);

        Task<TimeSpan> GetSendTimeout(IConnectionChannel connectionChannel);

        /// <summary>
        /// Validates the authentication request to determine if the supplied details
        /// are correct.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="authenticationRequest">The authentication request.</param>
        /// <returns></returns>
        Task<AuthenticationResult> ValidateAuthenticationCredentialsAsync(
            IConnection connection,
                                                           IAuthenticationCredentials authenticationRequest);

        /// <summary>
        /// Called when a new message is started in the specified session.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="from">From.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task OnMessageStart(IConnection connection, string from);

        Task<IMessageBuilder> OnCreateNewMessage(IConnection connection);

        /// <summary>
        /// Determines whether the speficied auth mechanism should be enabled for the specified connecton.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="authMechanism">The auth mechanism.</param>
        /// <returns>
        /// 	<c>true</c> if the specified auth mechanism should be enabled otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsAuthMechanismEnabled(IConnection connection, IAuthMechanism authMechanism);

        /// <summary>
        /// Called when a command received in the specified SMTP session.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="command">The command.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task OnCommandReceived(IConnection connection, SmtpCommand command);

        Task OnMessageCompleted(IConnection connection);

        Task<Encoding> GetDefaultEncoding(IConnection connection);

        Task<IEditableSession> OnCreateNewSession(IConnectionChannel connectionChannel);
    }
}