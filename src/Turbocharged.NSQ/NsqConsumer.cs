using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public interface INsqConsumer : IDisposable
    {
        Task WriteAsync(MessageBody message);
        Task SetMaxInFlightAsync(int maxInFlight);
    }

    public delegate Task MessageHandler(Message message);

    public static class NsqConsumer
    {
        public static INsqConsumer Connect(string connectionString, MessageHandler handler)
        {
            return Connect(ConsumerOptions.Parse(connectionString), handler);
        }

        public static Task<INsqConsumer> ConnectAndWaitAsync(string connectionString, MessageHandler handler)
        {
            return ConnectAndWaitAsync(ConsumerOptions.Parse(connectionString), handler);
        }

        public static INsqConsumer Connect(ConsumerOptions options, MessageHandler handler)
        {
            if (options.LookupEndPoints.Any())
            {
                return NsqLookupConsumer.Connect(options, handler);
            }
            else
            {
                return NsqTcpConnection.Connect(options.NsqEndPoint, options, handler);
            }
        }

        public static async Task<INsqConsumer> ConnectAndWaitAsync(ConsumerOptions options, MessageHandler handler)
        {
            if (options.LookupEndPoints.Any())
            {
                return await NsqLookupConsumer.ConnectAndWaitAsync(options, handler).ConfigureAwait(false);
            }
            else
            {
                return await NsqTcpConnection.ConnectAndWaitAsync(options.NsqEndPoint, options, handler).ConfigureAwait(false);
            }
        }
    }
}
