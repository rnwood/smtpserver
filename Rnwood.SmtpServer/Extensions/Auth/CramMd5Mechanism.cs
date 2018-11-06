// <copyright file="CramMd5Mechanism.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions.Auth
{
    /// <summary>
    /// Defines the <see cref="CramMd5Mechanism" />
    /// </summary>
    public class CramMd5Mechanism : IAuthMechanism
    {
        /// <summary>
        /// Gets the Identifier
        /// </summary>
        public string Identifier => "CRAM-MD5";

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
            return new CramMd5MechanismProcessor(connection, new RandomIntegerGenerator(), new CurrentDateTimeProvider());
        }
    }
}
