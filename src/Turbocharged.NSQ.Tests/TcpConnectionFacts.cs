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
    public class TcpConnectionFacts : IDisposable
    {
        #region Setup

        Topic topic = "foo";
        Channel channel = "bar";
        NsqTcpConnection conn;
        NsqProducer prod;

        public TcpConnectionFacts()
        {
            var options = new ConsumerOptions();
            conn = new NsqTcpConnection(new DnsEndPoint(Settings.NsqdHostName, Settings.NsqdTcpPort), options);
            prod = new NsqProducer(Settings.NsqdHostName, Settings.NsqdHttpPort);
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
            await Task.WhenAny(task, Task.Delay(1000));

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);

            byte[] receivedData = task.Result.Data;
            Assert.NotNull(receivedData);
            Assert.True(expectedData.SequenceEqual(receivedData));
        }
    }
}
