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

        public static INsqConsumer Connect(ConsumerOptions options, MessageHandler handler)
        {
            if (options.LookupEndPoints.Any())
            {
                throw new NotImplementedException();
            }
            else
            {
                return NsqTcpConnection.Connect(options.NsqEndPoint, options, handler);
            }
        }
    }
}
