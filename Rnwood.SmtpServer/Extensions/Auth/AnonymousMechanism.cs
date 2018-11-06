// <copyright file="AnonymousMechanism.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions.Auth
{
    /// <summary>
    /// Defines the <see cref="AnonymousMechanism" />
    /// </summary>
    public class AnonymousMechanism : IAuthMechanism
    {
        /// <summary>
        /// Gets the Identifier
        /// </summary>
        public string Identifier => "ANONYMOUS";

        /// <summary>
        /// Gets a value indicating whether IsPlainText
        /// </summary>
        public bool IsPlainText => false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <returns>The <see cref="IAuthMechanismProcessor"/></returns>
        public IAuthMechanismProcessor CreateAuthMechanismProcessor(IConnection connection)
        {
            return new AnonymousMechanismProcessor(connection);
        }
    }
}
