namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class FileMessage : IMessage
    {
        public FileMessage(FileInfo file, bool keepOnDispose)
        {
            this.file = file;
            this.keepOnDispose = keepOnDispose;
        }

        private readonly FileInfo file;
        private readonly bool keepOnDispose;

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
            if (!this.keepOnDispose)
            {
                if (this.file.Exists)
                {
                    this.file.Delete();
                }
            }
        }

        public Task<Stream> GetData()
        {
            return Task.FromResult<Stream>( new FileStream(this.file.FullName, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.Read));
        }

        internal class Builder : IMessageBuilder
        {
            public Builder(FileInfo file, bool keepOnDispose)
            {
                this.message = new FileMessage(file, keepOnDispose);
            }

            private FileMessage message;

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

            public ICollection<string> To => this.message.to;

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

            public async Task<Stream> GetData()
            {
                return await this.message.GetData().ConfigureAwait(false);
            }

            public Task<IMessage> ToMessage()
            {
                return Task.FromResult<IMessage>(this.message);
            }

            public Task<Stream> WriteData()
            {
                return Task.FromResult<Stream>(this.message.file.OpenWrite());
            }
        }
    }
}