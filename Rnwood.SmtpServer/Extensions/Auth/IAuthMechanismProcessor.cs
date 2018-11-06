// <copyright file="IAuthMechanismProcessor.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IAuthMechanismProcessor" />
    /// </summary>
    public interface IAuthMechanismProcessor
    {
        /// <summary>
        /// Gets the Credentials
        /// </summary>
        IAuthenticationCredentials Credentials { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data">The data<see cref="string"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<AuthMechanismProcessorStatus> ProcessResponseAsync(string data);
    }
}
