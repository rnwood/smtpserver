// <copyright file="AnonymousMechanismProcessor.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="AnonymousMechanismProcessor" />
    /// </summary>
    public class AnonymousMechanismProcessor : IAuthMechanismProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousMechanismProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        public AnonymousMechanismProcessor(IConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Gets the Credentials
        /// </summary>
        public IAuthenticationCredentials Credentials { get; private set; }

        /// <summary>
        /// Gets the Connection
        /// </summary>
        protected IConnection Connection { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data">The data<see cref="string"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        public async Task<AuthMechanismProcessorStatus> ProcessResponseAsync(string data)
        {
            this.Credentials = new AnonymousAuthenticationCredentials();

            AuthenticationResult result =
                await this.Connection.Server.Behaviour.ValidateAuthenticationCredentialsAsync(this.Connection, this.Credentials).ConfigureAwait(false);

            switch (result)
            {
                case AuthenticationResult.Success:
                    return AuthMechanismProcessorStatus.Success;

                default:
                    return AuthMechanismProcessorStatus.Failed;
            }
        }
    }
}
