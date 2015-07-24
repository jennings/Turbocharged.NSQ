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
            options.Topic = "foo";
            options.Channel = "bar";
            conn = NsqTcpConnection.Connect(endPoint, options, msg => msg.FinishAsync());
            conn.Dispose();
        }

        [Fact]
        public async Task CanReceiveAMessage()
        {
            options.Topic = "foo";
            options.Channel = "bar";
            byte[] expectedData = new byte[] { 1, 2, 3 };

            await EmptyChannelAsync(options.Topic, options.Channel);

            var tcs = new TaskCompletionSource<Message>();
            var task = tcs.Task;
            conn = NsqTcpConnection.Connect(endPoint, options, async msg =>
            {
                await msg.FinishAsync();
                tcs.TrySetResult(msg);
            });
            await conn.SetMaxInFlightAsync(100);
            await prod.PublishAsync(options.Topic, expectedData);
            await Task.WhenAny(task, Task.Delay(1000));

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);

            byte[] receivedData = task.Result.Body;
            Assert.NotNull(receivedData);
            Assert.True(expectedData.SequenceEqual(receivedData));
        }

        [Fact]
        public async Task CanReceiveAtAnAcceptableRate()
        {
            options.Topic = "load_test";
            options.Channel = "load_test";
            await EmptyChannelAsync(options.Topic, options.Channel);

            int messagesReceived = 0;
            conn = NsqTcpConnection.Connect(endPoint, options, async msg =>
            {
                await msg.FinishAsync().ConfigureAwait(false);
                Interlocked.Increment(ref messagesReceived);
            });

            await conn.SetMaxInFlightAsync(100);
            var messages = Enumerable.Range(0, 1000).Select(i => (MessageBody)BitConverter.GetBytes(i)).ToArray();
            await prod.PublishAsync(options.Topic, messages);
            await Task.Delay(1000);
            conn.Dispose();
            Assert.InRange(messagesReceived, 500, int.MaxValue);
        }
    }
}
