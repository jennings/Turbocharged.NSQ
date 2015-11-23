using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public interface INsqConsumer : IDisposable
    {
        void Connect(MessageHandler handler);
        Task ConnectAndWaitAsync(MessageHandler handler);
        Task PublishAsync(Topic topic, MessageBody message);
        Task SetMaxInFlightAsync(int maxInFlight);
    }

    public delegate Task MessageHandler(Message message);

    public static class NsqConsumer
    {
        public static INsqConsumer Create(string connectionString)
        {
            return Create(ConsumerOptions.Parse(connectionString));
        }

        public static INsqConsumer Create(ConsumerOptions options)
        {
            if (options.LookupEndPoints.Any())
            {
                return new NsqLookupConsumer(options);
            }
            else
            {
                return new NsqTcpConnection(options.NsqEndPoint, options);
            }
        }
    }
}
