using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandlerFunc = System.Func<Turbocharged.NSQ.Message, System.Threading.Tasks.Task>;

namespace Turbocharged.NSQ
{
    public sealed class NsqTcpConnection : IDisposable
    {
        static readonly byte[] MAGIC_V2 = new byte[] { 32, 32, 86, 50 }; // "  V2"

        public event Action<string> InternalMessages = _ => { };
        public ConsumerOptions ConsumerOptions { get; private set; }

        TcpClient _tcpClient;
        NetworkStream _stream;
        Thread _workerThread;
        IdentifyResponse _identifyResponse;
        HandlerFunc _messageHandler;

        public NsqTcpConnection(string connectionString)
            : this(ConsumerOptions.Parse(connectionString))
        {
        }

        public NsqTcpConnection(ConsumerOptions options)
        {
            if (options.NsqdEndPoints.Count != 1) throw new ArgumentException("Must provide exactly one nsqd endpoint");
            ConsumerOptions = options;
        }

        public void Dispose()
        {
            ((IDisposable)_tcpClient).Dispose();
        }

        public async Task ConnectAsync(Topic topic, Channel channel, HandlerFunc handler)
        {
            var ep = ConsumerOptions.NsqdEndPoints.First();
            var host = ep.Host;
            var port = ep.Port;
            InternalMessages("TCP client starting");
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(host, port).ConfigureAwait(false);
            InternalMessages("TCP client started");
            _stream = _tcpClient.GetStream();

            // Initiate the V2 protocol
            await _stream.WriteAsync(MAGIC_V2, 0, MAGIC_V2.Length).ConfigureAwait(false);
            _identifyResponse = await IdentifyAsync().ConfigureAwait(false);
            if (_identifyResponse.AuthRequired)
            {
                Close();
                throw new NotSupportedException("Authorization is not supported");
            }

            _messageHandler = handler;
            await SendCommandAsync(new Subscribe(topic, channel)).ConfigureAwait(false);

            // Begin the worker thread which receives and dispatches messages
            _workerThread = new Thread(MessageReceiverLoop);
            InternalMessages("Worker thread starting");
            _workerThread.Start();
            InternalMessages("Worker thread started");
        }

        public void Close()
        {
            _tcpClient.Close();
        }

        async Task<IdentifyResponse> IdentifyAsync()
        {
            var identify = new Identify(ConsumerOptions);
            await SendCommandAsync(identify).ConfigureAwait(false);
            var frameReader = new FrameReader(_stream);
            var frame = frameReader.ReadFrame();
            if (frame.Type != FrameType.Result)
            {
                throw new InvalidOperationException("Unexpected frame type after IDENTIFY");
            }
            return identify.ParseIdentifyResponse(frame.Data);
        }

        static readonly byte[] HEARTBEAT = new byte[] { 95, 104, 101, 97, 114, 116, 98, 101, 97, 116, 95 }; // "_heartbeat_"

        void MessageReceiverLoop()
        {
            try
            {
                var reactor = new FrameReader(_stream);

                Frame frame;
                while ((frame = reactor.ReadFrame()) != null)
                {
                    if (frame.Type == FrameType.Result)
                    {
                        if (HEARTBEAT.SequenceEqual(frame.Data))
                        {
                            InternalMessages("Heartbeat");
                            SendCommandAsync(new Nop())
                                .ContinueWith(t => Close(), TaskContinuationOptions.OnlyOnFaulted);
                        }
                        else
                        {
                            InternalMessages("Received result. Length = " + frame.MessageSize);
                        }
                    }
                    else if (frame.Type == FrameType.Message)
                    {
                        InternalMessages("Received message. Length = " + frame.MessageSize);
                        var message = new Message(frame, this);
                        // TODO: Rethink this
                        ThreadPool.QueueUserWorkItem(new WaitCallback(_ => { _messageHandler(message); }));
                    }
                    else if (frame.Type == FrameType.Error)
                    {
                        InternalMessages("Received error. Length = " + frame.MessageSize);
                    }
                    else
                    {
                        InternalMessages("Unknown message type: " + frame.Type);
                        throw new InvalidOperationException("Unknown message type " + frame.Type);
                    }
                }
            }
            catch (Exception ex)
            {
                InternalMessages("Worker thread threw exception: " + ex.Message);
            }
        }

        public Task ReadyAsync(int count)
        {
            return SendCommandAsync(new Ready(count));
        }

        internal Task SendCommandAsync(ICommand command)
        {
            var msg = command.ToByteArray();
            var readableMsg = Encoding.UTF8.GetString(msg);
            return _stream.WriteAsync(msg, 0, msg.Length);
        }
    }
}
