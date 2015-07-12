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

        Topic topic = "foo";
        Channel channel = "bar";
        NsqTcpConnection conn;
        NsqProducer prod;

        public NsqProducerFacts()
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
        public async Task ProducerCanEmptyAChannel()
        {
            // Put something in the channel
            await prod.PublishAsync(topic, new byte[] { 1 });

            // Now erase it
            await EmptyChannelAsync(topic, channel);

            // Now try to receive anything
            bool receivedData = false;
            await conn.ConnectAsync(topic, channel, async msg =>
            {
                receivedData = true;
                await msg.FinishAsync();
            });
            await Task.Delay(100);

            Assert.False(receivedData);
        }

        [Fact]
        public async Task CanGetProducerStats()
        {
            // Ensure something is there
            await prod.PublishAsync(topic, new byte[] { 1 });
            var stats = await prod.StatisticsAsync();
            Assert.NotNull(stats.Version);
            Assert.NotEmpty(stats.Topics);
        }
    }
}
