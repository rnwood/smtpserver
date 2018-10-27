namespace Rnwood.SmtpServer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class TcpClientConnectionChannel : IConnectionChannel
    {
        public TcpClientConnectionChannel(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.stream = tcpClient.GetStream();
            this.IsConnected = true;
            this.SetReaderEncoding(Encoding.ASCII);
        }

        private readonly TcpClient tcpClient;
        private StreamReader reader;
        private Stream stream;
        private StreamWriter writer;

        public Encoding ReaderEncoding
        {
            get;
            private set;
        }

        public void SetReaderEncoding(Encoding encoding)
        {
            this.ReaderEncoding = encoding;
            this.SetupReaderAndWriter();
        }

        public bool IsConnected
        {
            get; private set;
        }

        public event EventHandler Closed;

        public async Task FlushAsync()
        {
            await this.writer.FlushAsync().ConfigureAwait(false);
        }

        public Task CloseAync()
        {
            if (this.IsConnected)
            {
                this.IsConnected = false;
                this.tcpClient.Dispose();

                this.Closed?.Invoke(this, EventArgs.Empty);
            }

            return Task.CompletedTask;
        }

        public TimeSpan ReceiveTimeout
        {
            get { return TimeSpan.FromMilliseconds(this.tcpClient.ReceiveTimeout); }
            set { this.tcpClient.ReceiveTimeout = (int)Math.Min(int.MaxValue, value.TotalMilliseconds); }
        }

        public TimeSpan SendTimeout
        {
            get { return TimeSpan.FromMilliseconds(this.tcpClient.SendTimeout); }
            set { this.tcpClient.SendTimeout = (int)Math.Min(int.MaxValue, value.TotalMilliseconds); }
        }

        public IPAddress ClientIPAddress => ((IPEndPoint)this.tcpClient.Client.RemoteEndPoint).Address;

        public async Task ApplyStreamFilterAsync(Func<Stream, Task<Stream>> filter)
        {
            this.stream = await filter(this.stream).ConfigureAwait(false);
            this.SetupReaderAndWriter();
        }

        private void SetupReaderAndWriter()
        {
            this.writer = new StreamWriter(this.stream, this.ReaderEncoding) { AutoFlush = true, NewLine = "\r\n" };
            this.reader = new StreamReader(this.stream, this.ReaderEncoding);
        }

        public async Task<string> ReadLineAsync()
        {
            try
            {
                string text = await this.reader.ReadLineAsync().ConfigureAwait(false);

                if (text == null)
                {
                    throw new IOException("Reader returned null string"); ;
                }

                return text;
            }
            catch (IOException e)
            {
                await this.CloseAync().ConfigureAwait(false);
                throw new ConnectionUnexpectedlyClosedException("Read failed", e);
            }
        }

        public async Task WriteLineAsync(string text)
        {
            try
            {
                await this.writer.WriteLineAsync(text).ConfigureAwait(false);
            }
            catch (IOException e)
            {
                await this.CloseAync().ConfigureAwait(false);
                throw new ConnectionUnexpectedlyClosedException("Write failed", e);
            }
        }
    }
}