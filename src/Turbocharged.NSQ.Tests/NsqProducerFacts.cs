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
    public class NsqProducerFacts : IDisposable
    {
        #region Setup

        DnsEndPoint endPoint;
        ConsumerOptions options;
        NsqTcpConnection conn;
        NsqProducer prod;

        public NsqProducerFacts()
        {
            endPoint = new DnsEndPoint(Settings.NsqdHostName, Settings.NsqdTcpPort);
            options = new ConsumerOptions()
            {
                Topic = "foo",
                Channel = "bar",
            };
            prod = new NsqProducer(Settings.NsqdHostName, Settings.NsqdHttpPort);
        }

        public void Dispose()
        {
            if (conn != null)
                conn.Dispose();
        }

        Task EmptyChannelAsync(Topic topic, Channel channel)
        {
            return prod.EmptyChannelAsync(topic, channel);
        }

        #endregion

        [Fact]
        public async Task ProducerCanEmptyAChannel()
        {
            // Put something in the channel
            await prod.PublishAsync(options.Topic, new byte[] { 1 });

            // Now erase it
            await EmptyChannelAsync(options.Topic, options.Channel);

            // Now try to receive anything
            bool receivedData = false;
            conn = NsqTcpConnection.Connect(endPoint, options, async msg =>
            {
                receivedData = true;
                await msg.FinishAsync();
            });
            await conn.SetMaxInFlightAsync(100);
            await Task.Delay(100);

            Assert.False(receivedData);
        }

        [Fact]
        public async Task CanGetProducerStats()
        {
            // Ensure something is there
            await prod.PublishAsync(options.Topic, new byte[] { 1 });
            var stats = await prod.StatisticsAsync();
            Assert.NotNull(stats.Version);
            Assert.NotEmpty(stats.Topics);
        }

        [Fact]
        public async Task PublishMultipleWorks()
        {
            var message1 = new byte[] { 1, 1, 1 };
            var message2 = new byte[] { 2, 2, 2 };
            byte[][] messages = new[] { message1, message2 };
            await prod.PublishAsync(options.Topic, messages);
        }
    }
}
