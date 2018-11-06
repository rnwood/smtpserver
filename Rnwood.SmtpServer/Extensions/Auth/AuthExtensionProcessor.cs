// <copyright file="AuthExtensionProcessor.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AuthExtensionProcessor : IExtensionProcessor
    {
        /// <summary>
        /// Defines the connection
        /// </summary>
        private readonly IConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthExtensionProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection<see cref="IConnection"/></param>
        public AuthExtensionProcessor(IConnection connection)
        {
            this.connection = connection;
            this.MechanismMap = new AuthMechanismMap();
            this.MechanismMap.Add(new CramMd5Mechanism());
            this.MechanismMap.Add(new PlainMechanism());
            this.MechanismMap.Add(new LoginMechanism());
            this.MechanismMap.Add(new AnonymousMechanism());
            connection.VerbMap.SetVerbProcessor("AUTH", new AuthVerb(this));
        }

        public AuthMechanismMap MechanismMap { get; private set; }

        /// <inheritdoc/>
        public async Task<string[]> GetEHLOKeywords()
        {
            IEnumerable<IAuthMechanism> mechanisms = await this.GetEnabledAuthMechanisms().ConfigureAwait(false);

            if (mechanisms.Any())
            {
                string mids = string.Join(" ", mechanisms);

                return new[]
                           {
                                   "AUTH=" + mids,
                                   "AUTH " + mids
                               };
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mechanism">The mechanism<see cref="IAuthMechanism"/></param>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        public async Task<bool> IsMechanismEnabled(IAuthMechanism mechanism)
        {
            return await this.connection.Server.Behaviour.IsAuthMechanismEnabled(this.connection, mechanism).ConfigureAwait(false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        protected async Task<IEnumerable<IAuthMechanism>> GetEnabledAuthMechanisms()
        {
            List<IAuthMechanism> result = new List<IAuthMechanism>();

            foreach (var mechanism in this.MechanismMap.GetAll())
            {
                if (await this.connection.Server.Behaviour.IsAuthMechanismEnabled(this.connection, mechanism).ConfigureAwait(false))
                {
                    result.Add(mechanism);
                }
            }

            return result;
        }
    }
}
