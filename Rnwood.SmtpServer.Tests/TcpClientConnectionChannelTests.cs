using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    public class TcpClientConnectionChannelTests
    {
        [Fact]
        public async Task ReadLineAsync_ThrowsOnConnectionClose()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);

            try
            {
                listener.Start();
                Task<TcpClient> acceptTask = listener.AcceptTcpClientAsync();

                TcpClient client = new TcpClient();
                await client.ConnectAsync(IPAddress.Loopback, ((IPEndPoint)listener.LocalEndpoint).Port).ConfigureAwait(false);

                using (TcpClient serverTcpClient = await acceptTask.ConfigureAwait(false))
                {
                    TcpClientConnectionChannel channel = new TcpClientConnectionChannel(serverTcpClient);
                    client.Dispose();

                    await Assert.ThrowsAsync<ConnectionUnexpectedlyClosedException>(async () =>
                    {
                        await channel.ReadLineAsync().ConfigureAwait(false);
                    }).ConfigureAwait(false);
                }
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}