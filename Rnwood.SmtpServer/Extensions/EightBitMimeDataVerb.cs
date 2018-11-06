// <copyright file="EightBitMimeDataVerb.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="EightBitMimeDataVerb" />
    /// </summary>
    public class EightBitMimeDataVerb : DataVerb, IParameterProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EightBitMimeDataVerb"/> class.
        /// </summary>
        public EightBitMimeDataVerb()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <param name="command">The command<see cref="SmtpCommand"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        public override async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            if (connection.CurrentMessage != null && connection.CurrentMessage.EightBitTransport)
            {
                connection.SetReaderEncoding(Encoding.UTF8);
            }

            try
            {
                await base.ProcessAsync(connection, command).ConfigureAwait(false);
            }
            finally
            {
                await connection.SetReaderEncodingToDefault().ConfigureAwait(false);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <param name="key">The key<see cref="string"/></param>
        /// <param name="value">The value<see cref="string"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        public Task SetParameter(IConnection connection, string key, string value)
        {
            if (key.Equals("BODY", StringComparison.OrdinalIgnoreCase))
            {
                if (value.Equals("8BITMIME", StringComparison.CurrentCultureIgnoreCase))
                {
                    connection.CurrentMessage.EightBitTransport = true;
                }
                else if (value.Equals("7BIT", StringComparison.OrdinalIgnoreCase))
                {
                    connection.CurrentMessage.EightBitTransport = false;
                }
                else
                {
                    throw new SmtpServerException(
                        new SmtpResponse(
                            StandardSmtpResponseCode.SyntaxErrorInCommandArguments,
                                         "BODY parameter value invalid - must be either 7BIT or 8BITMIME"));
                }
            }

            return Task.CompletedTask;
        }
    }
}
