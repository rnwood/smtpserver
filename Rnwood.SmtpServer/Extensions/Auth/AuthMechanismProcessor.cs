// <copyright file="AuthMechanismProcessor.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="AuthMechanismProcessor" />
    /// </summary>
    public abstract class AuthMechanismProcessor : IAuthMechanismProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthMechanismProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        public AuthMechanismProcessor(IConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Gets the Connection
        /// </summary>
        public IConnection Connection { get; private set; }

        /// <summary>
        /// Gets or sets the Credentials
        /// </summary>
        public IAuthenticationCredentials Credentials { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data">The data<see cref="string"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        public abstract Task<AuthMechanismProcessorStatus> ProcessResponseAsync(string data);

        /// <summary>
        /// The DecodeBase64
        /// </summary>
        /// <param name="data">The data<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        protected static string DecodeBase64(string data)
        {
            try
            {
                return Encoding.ASCII.GetString(Convert.FromBase64String(data));
            }
            catch (FormatException)
            {
                throw new BadBase64Exception(new SmtpResponse(
                    StandardSmtpResponseCode.AuthenticationFailure,
                                                               "Bad Base64 data"));
            }
        }

        /// <summary>
        /// The EncodeBase64
        /// </summary>
        /// <param name="asciiString">The asciiString<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        private static string EncodeBase64(string asciiString)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(asciiString));
        }
    }
}
