// <copyright file="IAuthMechanism.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions.Auth
{
    /// <summary>
    /// Defines the <see cref="IAuthMechanism" />
    /// </summary>
    public interface IAuthMechanism
    {
        /// <summary>
        /// Gets the Identifier
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets a value indicating whether IsPlainText
        /// </summary>
        bool IsPlainText { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <returns>The <see cref="IAuthMechanismProcessor"/></returns>
        IAuthMechanismProcessor CreateAuthMechanismProcessor(IConnection connection);
    }
}
