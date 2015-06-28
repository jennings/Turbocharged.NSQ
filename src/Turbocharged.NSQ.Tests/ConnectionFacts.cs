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
    public class ConnectionFacts
    {
        #region Setup

        NsqConnection conn;

        public ConnectionFacts()
        {
            var hostname = Environment.GetEnvironmentVariable("NSQ_NSQ_HOSTNAME");
            var portStr = Environment.GetEnvironmentVariable("NSQ_NSQ_PORT");

            hostname = hostname ?? ConfigurationManager.AppSettings["NSQ.Hostname"];
            int port = int.Parse(portStr ?? ConfigurationManager.AppSettings["NSQ.Port"]);

            var options = new ConnectionOptions
            {
                ClientId = "Turbocharged.NSQ.Tests",
                NsqdEndPoints = { new DnsEndPoint(hostname, port) }
            };

            conn = new NsqConnection(options);
        }

        async Task ConnectAsync(Topic topic, Channel channel)
        {
            await conn.ConnectAsync(topic, channel);
        }

        #endregion

        [Fact]
        public async Task Foo()
        {
            byte[] expectedData = new byte[]{1,2,3};
            byte[] receivedData = null;
            await ConnectAsync("foo", "bar");

            conn.MessageReceived += (msg) => { receivedData = msg.Data; };
            await Task.Delay(100);

            Assert.NotNull(receivedData);
            Assert.True(expectedData.SequenceEqual(receivedData));
        }
    }
}
