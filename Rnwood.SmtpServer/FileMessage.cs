// <copyright file="FileMessage.cs" company="Rnwood.SmtpServer project contributors">
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
    /// Defines the <see cref="FileMessage" />
    /// </summary>
    public class FileMessage : IMessage
    {
        /// <summary>
        /// Defines the file
        /// </summary>
        private readonly FileInfo file;

        /// <summary>
        /// Defines the keepOnDispose
        /// </summary>
        private readonly bool keepOnDispose;

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Defines the to
        /// </summary>
        private List<string> to = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMessage"/> class.
        /// </summary>
        /// <param name="file">The file<see cref="FileInfo"/></param>
        /// <param name="keepOnDispose">The keepOnDispose<see cref="bool"/></param>
        public FileMessage(FileInfo file, bool keepOnDispose)
        {
            this.file = file;
            this.keepOnDispose = keepOnDispose;
        }

        /// <summary>
        /// Gets the DeclaredMessageSize
        /// </summary>
        public long? DeclaredMessageSize { get; private set; }

        /// <summary>
        /// Gets a value indicating whether EightBitTransport
        /// </summary>
        public bool EightBitTransport { get; private set; }

        /// <summary>
        /// Gets the From
        /// </summary>
        public string From { get; private set; }

        /// <summary>
        /// Gets the ReceivedDate
        /// </summary>
        public DateTime ReceivedDate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether SecureConnection
        /// </summary>
        public bool SecureConnection { get; private set; }

        /// <summary>
        /// Gets the Session
        /// </summary>
        public ISession Session { get; private set; }

        /// <summary>
        /// Gets the To
        /// </summary>
        public IReadOnlyCollection<string> Recipients => this.to.AsReadOnly();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets a stream which returns the message data.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the async operation
        /// </returns>
        public Task<Stream> GetData()
        {
            return Task.FromResult<Stream>(new FileStream(this.file.FullName, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.Read));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (!this.keepOnDispose && this.file.Exists)
                    {
                        this.file.Delete();
                    }
                }

                this.disposedValue = true;
            }
        }

        internal class Builder : IMessageBuilder
        {
            private FileMessage message;

            private bool disposedValue = false; // To detect redundant calls

            public Builder(FileInfo file, bool keepOnDispose)
            {
                this.message = new FileMessage(file, keepOnDispose);
            }

           /// <summary>
           /// Gets or sets the DeclaredMessageSize
           /// </summary>
            public long? DeclaredMessageSize
            {
                get
                {
                    return this.message.DeclaredMessageSize;
                }

                set
                {
                    this.message.DeclaredMessageSize = value;
                }
            }

           /// <summary>
           /// Gets or sets a value indicating whether EightBitTransport
           /// </summary>
            public bool EightBitTransport
            {
                get
                {
                    return this.message.EightBitTransport;
                }

                set
                {
                    this.EightBitTransport = value;
                }
            }

           /// <summary>
           /// Gets or sets the From
           /// </summary>
            public string From
            {
                get
                {
                    return this.message.From;
                }

                set
                {
                    this.message.From = value;
                }
            }

           /// <summary>
           /// Gets or sets the ReceivedDate
           /// </summary>
            public DateTime ReceivedDate
            {
                get
                {
                    return this.message.ReceivedDate;
                }

                set
                {
                    this.message.ReceivedDate = value;
                }
            }

           /// <summary>
           /// Gets or sets a value indicating whether SecureConnection
           /// </summary>
            public bool SecureConnection
            {
                get
                {
                    return this.message.SecureConnection;
                }

                set
                {
                    this.message.SecureConnection = value;
                }
            }

           /// <summary>
           /// Gets or sets the Session
           /// </summary>
            public ISession Session
            {
                get
                {
                    return this.message.Session;
                }

                set
                {
                    this.message.Session = value;
                }
            }

           /// <summary>
           /// Gets the To
           /// </summary>
            public ICollection<string> Recipients => this.message.to;

           /// <summary>
           ///
           /// </summary>
           /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
            public async Task<Stream> GetData()
            {
                return await this.message.GetData().ConfigureAwait(false);
            }

           /// <summary>
           ///
           /// </summary>
           /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
            public Task<IMessage> ToMessage()
            {
                return Task.FromResult<IMessage>(this.message);
            }

           /// <summary>
           ///
           /// </summary>
           /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
            public Task<Stream> WriteData()
            {
                return Task.FromResult<Stream>(this.message.file.OpenWrite());
            }

           /// <summary>
           /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
           /// </summary>
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

           /// <summary>
           /// Releases unmanaged and - optionally - managed resources.
           /// </summary>
           /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposedValue)
                {
                    if (disposing)
                    {
                    }

                    this.disposedValue = true;
                }
            }
        }
    }
}
