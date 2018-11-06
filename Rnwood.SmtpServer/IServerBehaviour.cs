// <copyright file="IServerBehaviour.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

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

    /// <summary>
    /// Defines the <see cref="IServerBehaviour" />
    /// </summary>
    public interface IServerBehaviour
    {
        /// <summary>
        /// Gets the DomainName
        /// Gets domain name reported by the server to clients.
        /// </summary>
        string DomainName { get; }

        /// <summary>
        /// Gets the IP address on which to listen for connections.
        /// </summary>
        IPAddress IpAddress { get; }

        /// <summary>
        /// Gets the MaximumNumberOfSequentialBadCommands
        /// </summary>
        int MaximumNumberOfSequentialBadCommands { get; }

        /// <summary>
        /// Gets the TCP port number on which to listen for connections.
        /// </summary>
        int PortNumber { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<Encoding> GetDefaultEncoding(IConnection connection);

        /// <summary>
        /// Gets the extensions that should be enabled for the specified connection.
        /// </summary>
        /// <param name="connectionChannel">The connectionChannel<see cref="IConnectionChannel"/></param>
        /// <returns></returns>
        Task<IEnumerable<IExtension>> GetExtensions(IConnectionChannel connectionChannel);

        /// <summary>
        /// Gets the maximum allowed size of the message for the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        Task<long?> GetMaximumMessageSize(IConnection connection);

        /// <summary>
        /// Gets the receive timeout that should be used for the specified connection.
        /// </summary>
        /// <param name="connectionChannel">The connection channel.</param>
        /// <returns></returns>
        Task<TimeSpan> GetReceiveTimeout(IConnectionChannel connectionChannel);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionChannel">The connectionChannel<see cref="IConnectionChannel"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<TimeSpan> GetSendTimeout(IConnectionChannel connectionChannel);

        /// <summary>
        /// Gets the SSL certificate that should be used for the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        Task<X509Certificate> GetSSLCertificate(IConnection connection);

        /// <summary>
        /// Determines whether the speficied auth mechanism should be enabled for the specified connecton.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="authMechanism">The auth mechanism.</param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<bool> IsAuthMechanismEnabled(IConnection connection, IAuthMechanism authMechanism);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<bool> IsSessionLoggingEnabled(IConnection connection);

        /// <summary>
        /// Gets a value indicating whether to run in SSL mode.
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection" /></param>
        /// <returns>
        ///   A <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        Task<bool> IsSSLEnabled(IConnection connection);

        /// <summary>
        /// Called when a command received in the specified SMTP session.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="command">The command.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task OnCommandReceived(IConnection connection, SmtpCommand command);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<IMessageBuilder> OnCreateNewMessage(IConnection connection);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionChannel">The connectionChannel<see cref="IConnectionChannel"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<IEditableSession> OnCreateNewSession(IConnectionChannel connectionChannel);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task OnMessageCompleted(IConnection connection);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <param name="message">The message<see cref="IMessage"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task OnMessageReceived(IConnection connection, IMessage message);

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <param name="message">The message<see cref="IMessageBuilder"/></param>
        /// <param name="recipient">The recipient<see cref="string"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task OnMessageRecipientAdding(IConnection connection, IMessageBuilder message, string recipient);

        /// <summary>
        /// Called when a new message is started in the specified session.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="from">From.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task OnMessageStart(IConnection connection, string from);

        /// <summary>
        /// Called when a SMTP session is completed.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="session">The session.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task OnSessionCompleted(IConnection connection, ISession session);

        /// <summary>
        /// Called when a new SMTP session is started.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="session">The session.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task OnSessionStarted(IConnection connection, ISession session);

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
    }
}
