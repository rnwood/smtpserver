// <copyright file="RcptToVerb.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Verbs;

    /// <summary>
    /// Defines the <see cref="RcptToVerb" />
    /// </summary>
    public class RcptToVerb : IVerb
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <param name="command">The command<see cref="SmtpCommand"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            if (connection.CurrentMessage == null)
            {
                await connection.WriteResponse(new SmtpResponse(
                    StandardSmtpResponseCode.BadSequenceOfCommands,
                                                                   "No current message")).ConfigureAwait(false);
                return;
            }

            if (command.ArgumentsText == "<>" || !command.ArgumentsText.StartsWith("<", StringComparison.Ordinal) ||
                !command.ArgumentsText.EndsWith(">", StringComparison.Ordinal) || command.ArgumentsText.Count(c => c == '<') != command.ArgumentsText.Count(c => c == '>'))
            {
                await connection.WriteResponse(
                    new SmtpResponse(
                        StandardSmtpResponseCode.SyntaxErrorInCommandArguments,
                                     "Must specify to address <address>")).ConfigureAwait(false);
                return;
            }

            string address = command.ArgumentsText.Remove(0, 1).Remove(command.ArgumentsText.Length - 2);
            await connection.Server.Behaviour.OnMessageRecipientAdding(connection, connection.CurrentMessage, address).ConfigureAwait(false);
            connection.CurrentMessage.Recipients.Add(address);
            await connection.WriteResponse(new SmtpResponse(StandardSmtpResponseCode.OK, "Recipient accepted")).ConfigureAwait(false);
        }
    }
}
