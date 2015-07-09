using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Turbocharged.NSQ.Tests
{
    public class ConnectionFacts : IDisposable
    {
        #region Setup

        Topic topic = "foo";
        Channel channel = "bar";
        NsqTcpConnection conn;
        NsqProducer prod;

        public ConnectionFacts()
        {
            var hostname = Environment.GetEnvironmentVariable("NSQD_HOSTNAME");
            var tcpPortStr = Environment.GetEnvironmentVariable("NSQD_TCPPORT");
            var httpPortStr = Environment.GetEnvironmentVariable("NSQD_HTTPPORT");

            hostname = hostname ?? ConfigurationManager.AppSettings["NSQ.Hostname"];
            int tcpPort = int.Parse(tcpPortStr ?? ConfigurationManager.AppSettings["NSQ.TcpPort"]);
            int httpPort = int.Parse(httpPortStr ?? ConfigurationManager.AppSettings["NSQ.HttpPort"]);

            var options = new ConsumerOptions
            {
                ClientId = "Turbocharged.NSQ.Tests",
                NsqdEndPoints = { new DnsEndPoint(hostname, tcpPort) }
            };

            conn = new NsqTcpConnection(options);
            prod = new NsqProducer(hostname, httpPort);
        }

        public void Dispose()
        {
            conn.Dispose();
        }

        Task EmptyChannelAsync(Topic topic, Channel channel)
        {
            return prod.EmptyChannelAsync(topic, channel);
        }

        #endregion

        [Fact]
        public async Task CanReceiveAMessage()
        {
            byte[] expectedData = new byte[] { 1, 2, 3 };

            await EmptyChannelAsync(topic, channel);

            var tcs = new TaskCompletionSource<Message>();
            var task = tcs.Task;
            await conn.ConnectAsync(topic, channel, async msg =>
            {
                if (tcs.TrySetResult(msg))
                    await msg.FinishAsync();
            });
            await prod.PublishAsync(topic, expectedData);
            await conn.ReadyAsync(1);
            await Task.WhenAny(task, Task.Delay(100));

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);

            byte[] receivedData = task.Result.Data;
            Assert.NotNull(receivedData);
            Assert.True(expectedData.SequenceEqual(receivedData));
        }
    }
}
