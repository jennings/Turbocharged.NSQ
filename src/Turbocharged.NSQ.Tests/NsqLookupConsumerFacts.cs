using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Turbocharged.NSQ.Tests
{
    public class NsqLookupConsumerFacts
    {
        DnsEndPoint lookupEndPoint;
        ConsumerOptions options;
        NsqProducer prod;

        public NsqLookupConsumerFacts()
        {
            lookupEndPoint = new DnsEndPoint(Settings.LookupHostName, Settings.LookupPort);
            options = new ConsumerOptions
            {
                LookupEndPoints = { lookupEndPoint }
            };
            prod = new NsqProducer(Settings.NsqdHostName, Settings.NsqdHttpPort);
        }

        [Fact]
        public async Task CanConnectViaLookupConnection()
        {
            options.Topic = "foo";
            options.Channel = "bar";
            var tcs = new TaskCompletionSource<bool>();
            using (var consumer = await NsqLookupConsumer.ConnectAndWaitAsync(options, msg =>
            {
                tcs.TrySetResult(true);
                return msg.FinishAsync();
            }))
            {
                consumer.InternalMessages += OutputMessage;
                await Task.WhenAll(
                    consumer.SetMaxInFlightAsync(100),
                    consumer.WriteAsync("hello"));
                var task = tcs.Task;
                var done = await Task.WhenAny(task, Task.Delay(1000));
                Assert.Same(task, done);
            }
        }

        void OutputMessage(object sender, InternalMessageEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }
}
