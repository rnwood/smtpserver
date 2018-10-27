namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Rnwood.SmtpServer.Verbs;

    public class Server : IServer
    {
        private readonly ILogger logger = Logging.Factory.CreateLogger<Server>();
        private TcpListener listener;

        public Server(IServerBehaviour behaviour)
        {
            this.Behaviour = behaviour;
        }

        public IServerBehaviour Behaviour { get; private set; }

        private bool isRunning;

        /// <summary>
        /// Gets or sets a value indicating whether the server is currently running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }

            private set
            {
                this.isRunning = value;
                this.IsRunningChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsRunningChanged;

        public int PortNumber => ((IPEndPoint)this.listener.LocalEndpoint).Port;

        private IVerbMap GetVerbMap()
        {
            VerbMap verbMap = new VerbMap();
            verbMap.SetVerbProcessor("HELO", new HeloVerb());
            verbMap.SetVerbProcessor("EHLO", new EhloVerb());
            verbMap.SetVerbProcessor("QUIT", new QuitVerb());
            verbMap.SetVerbProcessor("MAIL", new MailVerb());
            verbMap.SetVerbProcessor("RCPT", new RcptVerb());
            verbMap.SetVerbProcessor("DATA", new DataVerb());
            verbMap.SetVerbProcessor("RSET", new RsetVerb());
            verbMap.SetVerbProcessor("NOOP", new NoopVerb());

            return verbMap;
        }

        private async void Core()
        {
            this.logger.LogDebug("Core task running");

            while (this.IsRunning)
            {
                this.logger.LogDebug("Waiting for new client");

                await this.AcceptNextClient().ConfigureAwait(false);
                this.nextConnectionEvent.Set();
            }
        }

        public void WaitForNextConnection()
        {
            this.nextConnectionEvent.WaitOne();
        }

        private async Task AcceptNextClient()
        {
            TcpClient tcpClient = null;
            try
            {
                tcpClient = await this.listener.AcceptTcpClientAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                if (this.IsRunning)
                {
                    throw;
                }

                this.logger.LogDebug("Got InvalidOperationException on listener, shutting down");
                // normal - caused by _listener.Stop();
            }

            if (this.IsRunning)
            {
                this.logger.LogDebug("New connection from {0}", tcpClient.Client.RemoteEndPoint);

                TcpClientConnectionChannel connectionChannel = new TcpClientConnectionChannel(tcpClient);
                connectionChannel.ReceiveTimeout = await this.Behaviour.GetReceiveTimeout(connectionChannel).ConfigureAwait(false);
                connectionChannel.SendTimeout = await this.Behaviour.GetSendTimeout(connectionChannel).ConfigureAwait(false);


                Connection connection = await Connection.Create(this, connectionChannel, this.GetVerbMap()).ConfigureAwait(false);
            this.activeConnections.Add(connection);
            connection.ConnectionClosed += (s, ea) =>
            {
                this.logger.LogDebug("Connection {0} handling completed removing from active connections", connection);
                this.activeConnections.Remove(connection);
            };
#pragma warning disable 4014
            connection.ProcessAsync();
#pragma warning restore 4014
        }
    }

    /// <summary>
    /// Runs the server asynchronously. This method returns once the server has been started.
    /// To stop the server call the <see cref="Stop()"/> method.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">if the server is already running.</exception>
    public void Start()
    {
        if (this.IsRunning)
            {
                throw new InvalidOperationException("Already running");
            }

            this.logger.LogDebug("Starting server on {0}:{1}", this.Behaviour.IpAddress, this.Behaviour.PortNumber);

        this.listener = new TcpListener(this.Behaviour.IpAddress, this.Behaviour.PortNumber);
        this.listener.Start();

        this.IsRunning = true;

        this.logger.LogDebug("Listener active. Starting core task");

        this.coreTask = Task.Run(() => this.Core());
    }

    /// <summary>
    /// Stops the running server. Any existing connections are terminated.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">if the server is not running.</exception>
    public void Stop()
    {
        this.Stop(true);
    }

    /// <summary>
    /// Stops the running server.
    /// This method blocks until all connections have terminated, either by normal completion or timeout,
    /// or if <paramref name="killConnections"/> has been specified then once all of the threads
    /// have been killed.
    /// </summary>
    /// <param name="killConnections">True if existing connections should be terminated.</param>
    /// <exception cref="System.InvalidOperationException">if the server is not running.</exception>
    public void Stop(bool killConnections)
    {
        if (!this.IsRunning)
        {
            return;
        }

        this.logger.LogDebug("Stopping server");

        this.IsRunning = false;
        this.listener.Stop();

        this.logger.LogDebug("Listener stopped. Waiting for core task to exit");
        this.coreTask.Wait();

        if (killConnections)
        {
            this.KillConnections();

            this.logger.LogDebug("Server is stopped");
        }
        else
        {
            this.logger.LogDebug("Server is stopped but existing connections may still be active");
        }
    }

    public void KillConnections()
    {
        this.logger.LogDebug("Killing client connections");

        List<Task> killTasks = new List<Task>();
        foreach (Connection connection in this.activeConnections.Cast<Connection>().ToArray())
        {
            this.logger.LogDebug("Killing connection {0}", connection);
            killTasks.Add(connection.CloseConnection());
        }
        Task.WaitAll(killTasks.ToArray());
    }

    private readonly IList activeConnections = ArrayList.Synchronized(new List<Connection>());

        public IEnumerable<IConnection> ActiveConnections => this.activeConnections.Cast<IConnection>();

        private Task coreTask;
        private bool disposedValue = false; // To detect redundant calls
    private readonly AutoResetEvent nextConnectionEvent = new AutoResetEvent(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.Stop();
            }

            this.disposedValue = true;
        }
    }

    // ~Server() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        this.Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    }
}