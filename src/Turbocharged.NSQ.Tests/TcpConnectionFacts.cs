using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Turbocharged.NSQ.Tests
{
    public class TcpConnectionFacts : IDisposable
    {
        #region Setup

        DnsEndPoint endPoint;
        ConsumerOptions options;
        NsqTcpConnection conn;
        NsqProducer prod;

        public TcpConnectionFacts()
        {
            endPoint = new DnsEndPoint(Settings.NsqdHostName, Settings.NsqdTcpPort);
            options = new ConsumerOptions();
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
        public async Task ConnectionClosesProperly()
        {
            var topic = "foo";
            var channel = "bar";
            conn = NsqTcpConnection.Connect(endPoint, options, topic, channel, msg => msg.FinishAsync());
            conn.Dispose();
        }

        [Fact]
        public async Task CanReceiveAMessage()
        {
            var topic = "foo";
            var channel = "bar";
            byte[] expectedData = new byte[] { 1, 2, 3 };

            await EmptyChannelAsync(topic, channel);

            var tcs = new TaskCompletionSource<Message>();
            var task = tcs.Task;
            conn = NsqTcpConnection.Connect(endPoint, options, topic, channel, async msg =>
            {
                await msg.FinishAsync();
                tcs.TrySetResult(msg);
            });
            await conn.SetMaxInFlightAsync(100);
            await prod.PublishAsync(topic, expectedData);
            await Task.WhenAny(task, Task.Delay(1000));

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);

            byte[] receivedData = task.Result.Data;
            Assert.NotNull(receivedData);
            Assert.True(expectedData.SequenceEqual(receivedData));
        }

        [Fact]
        public async Task CanReceiveAtAnAcceptableRate()
        {
            var topic = "load_test";
            var channel = "load_test";
            await EmptyChannelAsync(topic, channel);

            int messagesReceived = 0;
            conn = NsqTcpConnection.Connect(endPoint, options, topic, channel, async msg =>
            {
                await msg.FinishAsync().ConfigureAwait(false);
                Interlocked.Increment(ref messagesReceived);
            });

            await conn.SetMaxInFlightAsync(100);
            var messages = Enumerable.Range(0, 1000).Select(BitConverter.GetBytes).ToArray();
            await prod.PublishAsync(topic, messages);
            await Task.Delay(1000);
            conn.Dispose();
            Assert.InRange(messagesReceived, 500, int.MaxValue);
        }
    }
}
