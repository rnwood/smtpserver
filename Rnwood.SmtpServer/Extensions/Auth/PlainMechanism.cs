// <copyright file="PlainMechanism.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions.Auth
{
    /// <summary>
    /// Defines the <see cref="PlainMechanism" />
    /// </summary>
    public class PlainMechanism : IAuthMechanism
    {
        /// <summary>
        /// Gets the Identifier
        /// </summary>
        public string Identifier => "PLAIN";

        /// <summary>
        /// Gets a value indicating whether IsPlainText
        /// </summary>
        public bool IsPlainText => true;

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <returns>The <see cref="IAuthMechanismProcessor"/></returns>
        public IAuthMechanismProcessor CreateAuthMechanismProcessor(IConnection connection)
        {
            return new PlainMechanismProcessor(connection);
        }
    }
}
