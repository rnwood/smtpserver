using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Rnwood.SmtpServer.Tests
{
    public partial class ClientTests
    {
        ITestOutputHelper output;

        public ClientTests (ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task SmtpClient_NonSSL()
        {
            using (DefaultServer server = new DefaultServer(false, Ports.AssignAutomatically))
            {
                ConcurrentBag<IMessage> messages = new ConcurrentBag<IMessage>();

                server.MessageReceived += (o, ea) =>
                {
                    messages.Add(ea.Message);
                };
                server.Start();

                await this.SendMessageAsync(server, "to@to.com").WithTimeout("sending message").ConfigureAwait(false);

                Assert.Single(messages);
                Assert.Equal("from@from.com", messages.First().From);
            }
        }

        /*
        [Fact]
        public async Task SmtpClient_UTF8Address()
        {
            using (DefaultServer server = new DefaultServer(false, Ports.AssignAutomatically))
            {
                ConcurrentBag<IMessage> messages = new ConcurrentBag<IMessage>();

                server.MessageReceived += (o, ea) =>
                {
                    messages.Add(ea.Message);
                };
                server.Start();

                await SendMessageAsync(server, "квіточка@пошта.укр").WithTimeout("sending message");

                Assert.Single(messages);
                Assert.Equal("квіточка@пошта.укр", messages.First().To.SingleOrDefault());
            }
        }*/

        [Fact]
        public async Task SmtpClient_NonSSL_StressTest()
        {
            using (DefaultServer server = new DefaultServer(false, Ports.AssignAutomatically))
            {
                ConcurrentBag<IMessage> messages = new ConcurrentBag<IMessage>();

                server.MessageReceived += (o, ea) =>
                {
                    messages.Add(ea.Message);
                };
                server.Start();

                List<Task> sendingTasks = new List<Task>();

                int numberOfThreads = 10;
                int numberOfMessagesPerThread = 50;

                for (int threadId = 0; threadId < numberOfThreads; threadId++)
                {
                    int localThreadId = threadId;

                    sendingTasks.Add(Task.Run(async () =>
                    {
                        using (SmtpClient client = new SmtpClient())
                        {
                            await client.ConnectAsync("localhost", server.PortNumber).ConfigureAwait(false);

                            for (int i = 0; i < numberOfMessagesPerThread; i++)
                            {
                                MimeMessage message = NewMessage(i + "@" + localThreadId);

                                await client.SendAsync(message).ConfigureAwait(false);
                                ;
                            }

                            await client.DisconnectAsync(true).ConfigureAwait(false);
                        }
                    }));
                }

                await Task.WhenAll(sendingTasks).WithTimeout(120, "sending messages").ConfigureAwait(false);
                Assert.Equal(numberOfMessagesPerThread * numberOfThreads, messages.Count);

                for (int threadId = 0; threadId < numberOfThreads; threadId++)
                {
                    for (int i = 0; i < numberOfMessagesPerThread; i++)
                    {
                        Assert.Contains(messages, m => m.To.Any(t => t == i + "@" + threadId));
                    }
                }
            }
        }

        private async Task SendMessageAsync(DefaultServer server, string toAddress)
        {
            MimeMessage message = NewMessage(toAddress);

            using (SmtpClient client = new SmtpClient(new SmtpClientLogger(this.output)))
            {
                

                await client.ConnectAsync("localhost", server.PortNumber).ConfigureAwait(false);
                await client.SendAsync(new FormatOptions { International = true }, message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        private static MimeMessage NewMessage(string toAddress)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("", "from@from.com"));
            message.To.Add(new MailboxAddress("", toAddress));
            message.Subject = "subject";
            message.Body = new TextPart("plain")
            {
                Text = "body"
            };
            return message;
        }
    }
}