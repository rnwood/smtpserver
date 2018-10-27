namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class MemoryMessage : IMessage
    {
        public MemoryMessage()
        {
        }

        internal byte[] Data { get; set; }

        public DateTime ReceivedDate
        {
            get; private set;
        }

        public ISession Session
        {
            get; private set;
        }

        public string From
        {
            get; private set;
        }

        private List<string> to = new List<string>();

        public string[] To => this.to.ToArray();

        public bool SecureConnection
        {
            get; private set;
        }

        public bool EightBitTransport
        {
            get; private set;
        }

        public long? DeclaredMessageSize
        {
            get; private set;
        }

        public virtual void Dispose()
        {
        }

        public Task<Stream> GetData()
        {
            return Task.FromResult<Stream>(
                new MemoryStream(this.Data ?? Array.Empty<byte>(),
                false));
        }

        internal class Builder : IMessageBuilder
        {
            public Builder() : this(new MemoryMessage())
            {
            }

            protected Builder(MemoryMessage message)
            {
                this.message = message;
            }

            private readonly MemoryMessage message;

            public Task<Stream> WriteData()
            {
                CloseNotifyingMemoryStream stream = new CloseNotifyingMemoryStream();
                stream.Closing += (s, ea) =>
                {
                    this.message.Data = stream.ToArray();
                };

                return Task.FromResult<Stream>(stream);
            }

            internal class CloseNotifyingMemoryStream : MemoryStream
            {
                public event EventHandler Closing;

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        this.Closing?.Invoke(this, EventArgs.Empty);
                    }

                    base.Dispose(disposing);
                }
            }

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

            public ICollection<string> To => this.message.to;

            public virtual Task<IMessage> ToMessage()
            {
                return Task.FromResult<IMessage>(this.message);
            }

            public async Task<Stream> GetData()
            {
                return await this.message.GetData().ConfigureAwait(false);
            }
        }
    }
}