// <copyright file="HeloVerb.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer
{
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Verbs;

    /// <summary>
    /// Defines the <see cref="HeloVerb" />
    /// </summary>
    public class HeloVerb : IVerb
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <param name="command">The command<see cref="SmtpCommand"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            if (!string.IsNullOrEmpty(connection.Session.ClientName))
            {
                await connection.WriteResponse(new SmtpResponse(
                    StandardSmtpResponseCode.BadSequenceOfCommands,
                                                                   "You already said HELO")).ConfigureAwait(false);
                return;
            }

            connection.Session.ClientName = command.ArgumentsText ?? string.Empty;
            await connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.OK, "Nice to meet you")).ConfigureAwait(false);
        }
    }
}
