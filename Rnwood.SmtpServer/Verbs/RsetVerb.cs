// <copyright file="RsetVerb.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Verbs
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="RsetVerb" />
    /// </summary>
    public class RsetVerb : IVerb
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <param name="command">The command<see cref="SmtpCommand"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            await connection.AbortMessage().ConfigureAwait(false);
            await connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.OK, "Rset completed")).ConfigureAwait(false);
        }
    }
}
