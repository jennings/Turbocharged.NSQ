using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public class NsqConnection
    {
        static readonly byte[] MAGIC_V2 = new byte[] { 32, 32, 86, 50 }; // "  V2"

        TcpClient _tcpClient = new TcpClient();
        NetworkStream _stream;
        IncomingMessageProcessor _messageProcessor;
        NsqConnection()
        {
        }

        public ConnectionOptions ConnectionOptions { get; private set; }

        public static Task<NsqConnection> ConnectAsync(string connectionString)
        {
            return ConnectAsync(ConnectionOptions.Parse(connectionString));
        }

        public static async Task<NsqConnection> ConnectAsync(ConnectionOptions options)
        {
            var connection = new NsqConnection();
            connection.ConnectionOptions = options;

            var ep = options.NsqdEndPoints.First();
            var host = ep.Host;
            var port = ep.Port;
            await connection._tcpClient.ConnectAsync(host, port).ConfigureAwait(false);
            connection._stream = connection._tcpClient.GetStream();
            connection._messageProcessor = new IncomingMessageProcessor(connection._stream);
            await connection._stream.WriteAsync(MAGIC_V2, 0, MAGIC_V2.Length).ConfigureAwait(false);

            return connection;
        }

        public async Task IdentifyAsync()
        {
            var cmd = new Identify();
            var msg = cmd.ToByteArray();
            var readableMsg = Encoding.UTF8.GetString(msg);
            await _stream.WriteAsync(msg, 0, msg.Length).ConfigureAwait(false);

            Message message;
            while ((message = await _messageProcessor.ReadMessageAsync().ConfigureAwait(false)) != null)
            {
                var a = 1;
            }
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
