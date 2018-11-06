// <copyright file="MemorySession.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="MemorySession" />
    /// </summary>
    public class MemorySession : AbstractSession
    {
        /// <summary>
        /// Defines the log
        /// </summary>
        private readonly StringBuilder log = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySession"/> class.
        /// </summary>
        /// <param name="clientAddress">The clientAddress<see cref="IPAddress"/></param>
        /// <param name="startDate">The startDate<see cref="DateTime"/></param>
        public MemorySession(IPAddress clientAddress, DateTime startDate)
            : base(clientAddress, startDate)
        {
        }

        public override void AppendToLog(string text)
        {
            this.log.AppendLine(text);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>The <see cref="TextReader"/></returns>
        public override TextReader GetLog()
        {
            return new StringReader(this.log.ToString());
        }
    }
}
