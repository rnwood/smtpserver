// <copyright file="RcptVerb.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer
{
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Verbs;

    /// <summary>
    /// Defines the <see cref="RcptVerb" />
    /// </summary>
    public class RcptVerb : IVerb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RcptVerb"/> class.
        /// </summary>
        public RcptVerb()
        {
            this.SubVerbMap = new VerbMap();
            this.SubVerbMap.SetVerbProcessor("TO", new RcptToVerb());
        }

        /// <summary>
        /// Gets the SubVerbMap
        /// </summary>
        public VerbMap SubVerbMap { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <param name="command">The command<see cref="SmtpCommand"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            SmtpCommand subrequest = new SmtpCommand(command.ArgumentsText);
            IVerb verbProcessor = this.SubVerbMap.GetVerbProcessor(subrequest.Verb);

            if (verbProcessor != null)
            {
                await verbProcessor.ProcessAsync(connection, subrequest).ConfigureAwait(false);
            }
            else
            {
                await connection.WriteResponse(
                    new SmtpResponse(
                        StandardSmtpResponseCode.CommandParameterNotImplemented,
                                     "Subcommand {0} not implemented",
                                     subrequest.Verb)).ConfigureAwait(false);
            }
        }
    }
}
