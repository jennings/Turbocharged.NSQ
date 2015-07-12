using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        static readonly byte[] HEARTBEAT = new byte[] { 95, 104, 101, 97, 114, 116, 98, 101, 97, 116, 95 }; // "_heartbeat_"

        public event Action<string> InternalMessages = _ => { };

        ConsumerOptions _consumerOptions;
        DnsEndPoint _endPoint;
        TcpClient _tcpClient;
        NetworkStream _stream;
        Thread _workerThread;
        IdentifyResponse _identifyResponse;
        HandlerFunc _messageHandler;
        bool _disposed = false;

        public NsqTcpConnection(DnsEndPoint endPoint, ConsumerOptions consumerOptions)
        {
            _endPoint = endPoint;
            _consumerOptions = consumerOptions;
        }

        public async Task ConnectAsync(Topic topic, Channel channel, HandlerFunc handler)
        {
            var host = _endPoint.Host;
            var port = _endPoint.Port;
            InternalMessages("TCP client starting");
            _disposed = false;
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
            Dispose();
        }

        public void Dispose()
        {
            _disposed = true;
            if (_tcpClient != null)
                ((IDisposable)_tcpClient).Dispose();
        }

        async Task<IdentifyResponse> IdentifyAsync()
        {
            var identify = new Identify(_consumerOptions);
            await SendCommandAsync(identify).ConfigureAwait(false);
            var frameReader = new FrameReader(_stream);
            var frame = frameReader.ReadFrame();
            if (frame.Type != FrameType.Result)
            {
                throw new InvalidOperationException("Unexpected frame type after IDENTIFY");
            }
            return identify.ParseIdentifyResponse(frame.Data);
        }

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
                // Don't bother anyone if we've been disposed
                if (!_disposed)
                {
                    InternalMessages("Worker thread threw exception: " + ex.Message);
                }
            }
        }

        public Task SetMaxInFlight(int maxInFlight)
        {
            return SendCommandAsync(new Ready(maxInFlight));
        }

        internal Task SendCommandAsync(ICommand command)
        {
            var msg = command.ToByteArray();
            return _stream.WriteAsync(msg, 0, msg.Length);
        }
    }
}
