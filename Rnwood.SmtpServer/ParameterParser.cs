// <copyright file="ParameterParser.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ParameterParser" />
    /// </summary>
    public class ParameterParser
    {
        /// <summary>
        /// Defines the parameters
        /// </summary>
        private readonly List<Parameter> parameters = new List<Parameter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterParser"/> class.
        /// </summary>
        /// <param name="arguments">The arguments<see cref="string"/></param>
        public ParameterParser(params string[] arguments)
        {
            this.Parse(arguments);
        }

        /// <summary>
        /// Gets the Parameters
        /// </summary>
        public IReadOnlyCollection<Parameter> Parameters => this.parameters.ToArray();

        /// <summary>
        ///
        /// </summary>
        /// <param name="tokens">The tokens<see cref="string"/></param>
        private void Parse(string[] tokens)
        {
            foreach (string token in tokens)
            {
                string[] tokenParts = token.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string key = tokenParts[0];
                string value = tokenParts.Length > 1 ? tokenParts[1] : null;
                this.parameters.Add(new Parameter(key, value));
            }
        }
    }
}
