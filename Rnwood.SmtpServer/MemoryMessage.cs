// <copyright file="MemoryMessage.cs" company="Rnwood.SmtpServer project contributors">
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
    /// Defines the <see cref="MemoryMessage" />
    /// </summary>
    public class MemoryMessage : IMessage
    {
        private List<string> to = new List<string>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryMessage"/> class.
        /// </summary>
        public MemoryMessage()
        {
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
        /// Gets a value indicating whether if message was received over a secure connection.
        /// </summary>
        public bool SecureConnection { get; private set; }

        /// <summary>
        /// Gets the Session message was received on.
        /// </summary>
        public ISession Session { get; private set; }

        /// <summary>
        /// Gets the recipient of the message as specified by the client when sending RCPT TO command.
        /// </summary>
        public IReadOnlyCollection<string> Recipients => this.to.AsReadOnly();

        internal byte[] Data { get; set; }

        /// <summary>
        /// Gets a stream which returns the message data.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the async operation
        /// </returns>
        public Task<Stream> GetData()
        {
            return Task.FromResult<Stream>(
                new MemoryStream(
                    this.Data ?? Array.Empty<byte>(),
                    false));
        }

        /// <summary>
        /// Defines the <see cref="Builder" />
        /// </summary>
        internal class Builder : IMessageBuilder
        {
            private readonly MemoryMessage message;

            private bool disposedValue = false; // To detect redundant calls

           /// <summary>
           /// Initializes a new instance of the <see cref="Builder"/> class.
           /// </summary>
            public Builder()
                : this(new MemoryMessage())
            {
            }

           /// <summary>
           /// Initializes a new instance of the <see cref="Builder"/> class.
           /// </summary>
           /// <param name="message">The message<see cref="MemoryMessage"/></param>
            protected Builder(MemoryMessage message)
            {
                this.message = message;
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
            public virtual Task<IMessage> ToMessage()
            {
                return Task.FromResult<IMessage>(this.message);
            }

           /// <summary>
           ///
           /// </summary>
           /// <returns>A <see cref="Task{T}"/> representing the async operation</returns>
            public Task<Stream> WriteData()
            {
                CloseNotifyingMemoryStream stream = new CloseNotifyingMemoryStream();
                stream.Closing += (s, ea) =>
                {
                    this.message.Data = stream.ToArray();
                };

                return Task.FromResult<Stream>(stream);
            }

           /// <summary>
           /// Defines the <see cref="CloseNotifyingMemoryStream" />
           /// </summary>
            internal class CloseNotifyingMemoryStream : MemoryStream
            {
                

               /// <summary>
               /// Defines the Closing
               /// </summary>
                public event EventHandler Closing;

               /// <summary>
               ///
               /// </summary>
               /// <param name="disposing">The disposing<see cref="bool"/></param>
                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        this.Closing?.Invoke(this, EventArgs.Empty);
                    }

                    base.Dispose(disposing);
                }
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

        private bool disposedValue = false; // To detect redundant calls

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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
