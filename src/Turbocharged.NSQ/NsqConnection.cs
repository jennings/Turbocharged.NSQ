using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public class NsqConnection
    {
        NsqConnection()
        {
        }

        public ConnectionOptions ConnectionOptions { get; private set; }

        public static NsqConnection ConnectAsync(string connectionString)
        {
            return ConnectAsync(ConnectionOptions.Parse(connectionString));
        }

        public static NsqConnection ConnectAsync(ConnectionOptions options)
        {
            var connection = new NsqConnection();
            connection.ConnectionOptions = options;
            return connection;
        }
    }

    public interface IPublisher
    {
        Task PublishAsync(string topic, byte[] message);
    }

    public interface IMessageFinisher
    {
        void Finish();
        void Requeue();
        void Touch();
    }

    public interface IConsumer
    {
        Task SubscribeAsync(string topic, string channel, Action<string, IMessageFinisher> handler);
    }
}
