using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    class ReliableConnection : IDisposable
    {
        static readonly byte[] MAGIC_V2 = new byte[] { 32, 32, 86, 50 }; // "  V2"
        DnsEndPoint _endPoint;
        TcpClient _client;
        NetworkStream _stream;
        IdentifyResponse _identifyResponse;
        ConsumerOptions _options;
        Topic _topic;
        Channel _channel;

        public event Action<string> InternalMessages;

        public ReliableConnection(DnsEndPoint endPoint, ConsumerOptions options)
        {
            _endPoint = endPoint;
            _options = options;
        }

        public void Dispose()
        {
            if (_client != null)
            {
                ((IDisposable)_client).Dispose();
            }
        }

        internal async Task ConnectAsync(Topic topic, Channel channel, bool initialDelay = false)
        {
            _topic = topic;
            _channel = channel;
            const int RECONNECT_TIMEOUT = 15000;

            if (initialDelay)
            {
                PublishInternalMessage("Waiting 15s to connect");
                Task.Delay(RECONNECT_TIMEOUT).Wait();
            }

            do
            {
                try
                {
                    _client = new TcpClient();
                    PublishInternalMessage("Connecting");
                    await _client.ConnectAsync(_endPoint.Host, _endPoint.Port).ConfigureAwait(false);
                    PublishInternalMessage("Connected");
                    _stream = _client.GetStream();
                    await HandshakeAsync().ConfigureAwait(false);
                    break;
                }
                catch (SocketException ex)
                {
                    // TODO: Backoff strategy
                    PublishInternalMessage("SocketException (waiting 15s): " + ex.Message);
                    Task.Delay(RECONNECT_TIMEOUT).Wait();
                }
            } while (true);
        }

        async Task HandshakeAsync()
        {
            // Initiate the V2 protocol
            await _stream.WriteAsync(MAGIC_V2, 0, MAGIC_V2.Length).ConfigureAwait(false);
            _identifyResponse = await IdentifyAsync().ConfigureAwait(false);
            if (_identifyResponse.AuthRequired)
            {
                Dispose();
                throw new NotSupportedException("Authorization is not supported");
            }

            await SendCommandAsync(new Subscribe(_topic, _channel)).ConfigureAwait(false);
        }

        async Task<IdentifyResponse> IdentifyAsync()
        {
            var identify = new Identify(_options);
            await SendCommandAsync(identify).ConfigureAwait(false);
            var frame = await ReadFrameAsync().ConfigureAwait(false);
            if (frame.Type != FrameType.Result)
            {
                throw new InvalidOperationException("Unexpected frame type after IDENTIFY");
            }
            return identify.ParseIdentifyResponse(frame.Data);
        }

        internal Task SendCommandAsync(ICommand command)
        {
            var msg = command.ToByteArray();
            return WriteAsync(msg, 0, msg.Length);
        }

        internal async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            await ExecuteWithReconnectionAsync(async () =>
            {
                await _stream.WriteAsync(buffer, offset, count).ConfigureAwait(false);
                return 0;
            }).ConfigureAwait(false);
        }

        internal async Task<Frame> ReadFrameAsync()
        {
            return await ExecuteWithReconnectionAsync(() =>
            {
                var reader = new FrameReader(_stream);
                return Task.FromResult(reader.ReadFrame());
            }).ConfigureAwait(false);
        }

        async Task<T> ExecuteWithReconnectionAsync<T>(Func<Task<T>> func)
        {
            if (_client == null || _stream == null || !_client.Connected)
                throw new InvalidOperationException("Not connected");

            do
            {
                try
                {
                    return await func().ConfigureAwait(false);
                }
                catch (SocketException ex)
                {
                    PublishInternalMessage("SocketException: " + ex.Message);
                    // Continue outside the catch...
                }
                await ConnectAsync(_topic, _channel, initialDelay: true).ConfigureAwait(false);
            } while (true);
        }

        void PublishInternalMessage(string message)
        {
            var delegates = InternalMessages;
            if (delegates != null)
            {
                delegates(message);
            }
        }
    }
}
