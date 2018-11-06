// <copyright file="StartTlsExtension.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="StartTlsExtension" />
    /// </summary>
    public class StartTlsExtension : IExtension
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        /// <returns>The <see cref="IExtensionProcessor"/></returns>
        public IExtensionProcessor CreateExtensionProcessor(IConnection connection)
        {
            return new StartTlsExtensionProcessor(connection);
        }

        /// <summary>
        /// Defines the <see cref="StartTlsExtensionProcessor" />
        /// </summary>
        private class StartTlsExtensionProcessor : IExtensionProcessor
        {
           /// <summary>
           /// Initializes a new instance of the <see cref="StartTlsExtensionProcessor"/> class.
           /// </summary>
           /// <param name="connection">The connection<see cref="IConnection"/></param>
            public StartTlsExtensionProcessor(IConnection connection)
            {
                this.Connection = connection;
                this.Connection.VerbMap.SetVerbProcessor("STARTTLS", new StartTlsVerb());
            }

           /// <summary>
           /// Gets the Connection
           /// </summary>
            public IConnection Connection { get; private set; }

           /// <summary>
           ///
           /// </summary>
           /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
            public Task<string[]> GetEHLOKeywords()
            {
                string[] result;

                if (!this.Connection.Session.SecureConnection)
                {
                    result = new[] { "STARTTLS" };
                }

                result = Array.Empty<string>();

                return Task.FromResult(result);
            }
        }
    }
}
