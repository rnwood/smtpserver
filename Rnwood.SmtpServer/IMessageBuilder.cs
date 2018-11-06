// <copyright file="IMessageBuilder.cs" company="Rnwood.SmtpServer project contributors">
// Copyright (c) Rnwood.SmtpServer project contributors. All rights reserved.
// Licensed under the BSD license. See LICENSE.md file in the project root for full license information.
// </copyright>

namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IMessageBuilder" />
    /// </summary>
    public interface IMessageBuilder : IDisposable
    {
        /// <summary>
        /// Gets or sets the DeclaredMessageSize
        /// </summary>
        long? DeclaredMessageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether EightBitTransport
        /// </summary>
        bool EightBitTransport { get; set; }

        /// <summary>
        /// Gets or sets the From
        /// </summary>
        string From { get; set; }

        /// <summary>
        /// Gets or sets the ReceivedDate
        /// </summary>
        DateTime ReceivedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SecureConnection
        /// </summary>
        bool SecureConnection { get; set; }

        /// <summary>
        /// Gets or sets the Session
        /// </summary>
        ISession Session { get; set; }

        ICollection<string> Recipients { get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<Stream> GetData();

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<IMessage> ToMessage();

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
        Task<Stream> WriteData();
    }
}
