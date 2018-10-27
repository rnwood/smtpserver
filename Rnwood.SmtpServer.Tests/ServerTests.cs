
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Rnwood.SmtpServer.Tests
{
    public class ServerTests
    {
        private Server StartServer()
        {
            Server server = this.NewServer();
            server.Start();
            return server;
        }

        private Server NewServer()
        {
            return new DefaultServer(false, Ports.AssignAutomatically);
        }

        [Fact]
        public void Start_IsRunning()
        {
            using (Server server = this.StartServer())
            {
                Assert.True(server.IsRunning);
            }
        }

        [Fact]
        public void StartOnInusePort_StartupExceptionThrown()
        {
            using (Server server1 = new DefaultServer(false, Ports.AssignAutomatically))
            {
                server1.Start();

                using (Server server2 = new DefaultServer(false, server1.PortNumber))
                {
                    Assert.Throws<SocketException>(() =>
                    {
                        server2.Start();
                    });
                }
            }
        }

        [Fact]
        public void Stop_NotRunning()
        {
            using (Server server = this.StartServer())
            {
                server.Stop();
                Assert.False(server.IsRunning);
            }
        }

        [Fact]
        public async Task Stop_CannotConnect()
        {
            using (Server server = this.StartServer())
            {
                int portNumber = server.PortNumber;
                server.Stop();

                TcpClient client = new TcpClient();
                await Assert.ThrowsAnyAsync<SocketException>(async () =>
                    await client.ConnectAsync("localhost", portNumber).ConfigureAwait(false)
                ).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task Stop_KillConnectionsTrue_ConnectionsKilled()
        {
            {
                Server server = this.StartServer();

                Task serverTask = Task.Run(async () =>
                {
                    await Task.Run(() => server.WaitForNextConnection()).WithTimeout("waiting for next server connection").ConfigureAwait(false);
                    Assert.Single(server.ActiveConnections);
                    await Task.Run(() => server.Stop(true)).WithTimeout("stopping server").ConfigureAwait(false);
                    Assert.Empty(server.ActiveConnections);
                });

                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("localhost", server.PortNumber).WithTimeout("waiting for client to connect").ConfigureAwait(false);
                    await serverTask.WithTimeout(30, "waiting for server task to complete").ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public async Task Stop_KillConnectionFalse_ConnectionsNotKilled()
        {
            Server server = this.StartServer();

            Task serverTask = Task.Run(async () =>
            {
                await Task.Run(() => server.WaitForNextConnection()).WithTimeout("waiting for next server connection").ConfigureAwait(false);
                Assert.Single(server.ActiveConnections);

                await Task.Run(() => server.Stop(false)).WithTimeout("stopping server").ConfigureAwait(false);
                ;
                Assert.Single(server.ActiveConnections);
                await Task.Run(() => server.KillConnections()).WithTimeout("killing connections").ConfigureAwait(false);
                Assert.Empty(server.ActiveConnections);
            });

            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync("localhost", server.PortNumber).WithTimeout("waiting for client to connect").ConfigureAwait(false);
                await serverTask.WithTimeout(30, "waiting for server task to complete").ConfigureAwait(false);
            }
        }

        [Fact]
        public async void Start_CanConnect()
        {
            using (Server server = this.StartServer())
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("localhost", server.PortNumber).ConfigureAwait(false);
                }

                server.Stop();
            }
        }
    }
}