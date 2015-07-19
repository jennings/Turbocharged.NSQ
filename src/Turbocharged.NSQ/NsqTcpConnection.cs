using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        static readonly byte[] HEARTBEAT = new byte[] { 95, 104, 101, 97, 114, 116, 98, 101, 97, 116, 95 }; // "_heartbeat_"
        static readonly byte[] MAGIC_V2 = new byte[] { 32, 32, 86, 50 }; // "  V2"

        public event Action<string> InternalMessages = _ => { };

        readonly CancellationTokenSource _connectionClosedSource;
        readonly CancellationToken _connectionClosedToken;
        readonly ConsumerOptions _options;
        readonly Topic _topic;
        readonly Channel _channel;
        readonly DnsEndPoint _endPoint;
        readonly HandlerFunc _messageHandler;
        readonly IBackoffStrategy _backoffStrategy;
        readonly Thread _workerThread;

        readonly object _connectionSwapLock = new object();
        readonly object _connectionSwapInProgressLock = new object();

        IdentifyResponse _identifyResponse;
        NetworkStream _stream;
        TaskCompletionSource<bool> _nextReconnectionTaskSource = new TaskCompletionSource<bool>();


        NsqTcpConnection(DnsEndPoint endPoint, ConsumerOptions options, Topic topic, Channel channel, HandlerFunc handler)
        {
            _endPoint = endPoint;
            _options = options;
            _topic = topic;
            _channel = channel;
            _messageHandler = handler;
            _backoffStrategy = new ExponentialBackoffStrategy(_options.ReconnectionDelay, _options.ReconnectionMaxDelay);

            _connectionClosedSource = new CancellationTokenSource();
            _connectionClosedToken = _connectionClosedSource.Token;

            _workerThread = new Thread(WorkerLoop);
            _workerThread.Name = "Turbocharged.NSQ Worker";
        }

        public static NsqTcpConnection Connect(DnsEndPoint endPoint, ConsumerOptions options, Topic topic, Channel channel, HandlerFunc handler)
        {
            var nsq = new NsqTcpConnection(endPoint, options, topic, channel, handler);

            // The worker thread is responsible for:
            //   a. Connecting to NSQ
            //   b. Handshaking
            //   c. Dispatching received messages to the thread pool

            nsq.InternalMessages("Worker thread starting");
            nsq._workerThread.Start();
            nsq.InternalMessages("Worker thread started");

            return nsq;
        }

        object _disposeLock = new object();

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (!_connectionClosedToken.IsCancellationRequested)
                {
                    _connectionClosedSource.Cancel();
                    _connectionClosedSource.Dispose();
                }
            }
        }

        public async Task WriteAsync(byte[] buffer)
        {
            while (true)
            {
                NetworkStream stream;
                Task reconnectionTask;
                lock (_connectionSwapInProgressLock)
                {
                    stream = _stream;
                    reconnectionTask = _nextReconnectionTaskSource.Task;
                }

                try
                {
                    if (stream != null)
                    {
                        await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                        return;
                    }
                }
                catch (IOException)
                {
                    continue;
                }
                catch (SocketException)
                {
                    continue;
                }

                await reconnectionTask.ConfigureAwait(false);
            }
        }

        public Task SetMaxInFlightAsync(int maxInFlight)
        {
            return SendCommandAsync(new Ready(maxInFlight));
        }

        void WorkerLoop()
        {
            bool firstConnectionAttempt = true;
            bool connected = false;
            TcpClient client = null;
            FrameReader reader = null;
            IBackoffLimiter backoffLimiter = null;
            IDisposable cancellationRegistration = Disposable.Empty;

            while (true)
            {
                try
                {
                    if (_connectionClosedToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!connected)
                    {
                        lock (_connectionSwapLock)
                        {
                            if (firstConnectionAttempt)
                            {
                                firstConnectionAttempt = false;
                            }
                            else
                            {
                                if (backoffLimiter == null)
                                    backoffLimiter = _backoffStrategy.Create();

                                TimeSpan delay;
                                if (backoffLimiter.ShouldReconnect(out delay))
                                {
                                    InternalMessages("Delaying " + (int)delay.TotalMilliseconds + "ms before reconnecting");
                                    Thread.Sleep(delay);
                                }
                                else
                                {
                                    // We give up
                                    InternalMessages("Abandoning connection");
                                    Dispose();
                                    return;
                                }
                            }

                            lock (_connectionSwapInProgressLock)
                            {
                                if (client != null)
                                {
                                    cancellationRegistration.Dispose();
                                    ((IDisposable)client).Dispose();
                                }

                                InternalMessages("TCP client starting");
                                client = new TcpClient(_endPoint.Host, _endPoint.Port);
                                cancellationRegistration = _connectionClosedToken.Register(() => ((IDisposable)client).Dispose(), false);
                                connected = true;
                                InternalMessages("TCP client started");

                                _stream = client.GetStream();
                                reader = new FrameReader(_stream);

                                Handshake(_stream, reader);


                                // Start a new backoff cycle next time we disconnect
                                backoffLimiter = null;

                                _nextReconnectionTaskSource.SetResult(true);
                                _nextReconnectionTaskSource = new TaskCompletionSource<bool>();
                            }
                        }
                    }

                    Frame frame;
                    while ((frame = reader.ReadFrame()) != null)
                    {
                        if (frame.Type == FrameType.Result)
                        {
                            if (HEARTBEAT.SequenceEqual(frame.Data))
                            {
                                InternalMessages("Heartbeat");
                                SendCommandAsync(new Nop())
                                    .ContinueWith(t => Dispose(), TaskContinuationOptions.OnlyOnFaulted);
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
                catch (IOException ex)
                {
                    InternalMessages("EXCEPTION: " + ex.Message);
                    connected = false;
                    continue;
                }
                catch (SocketException ex)
                {
                    InternalMessages("EXCEPTION: " + ex.Message);
                    connected = false;
                    continue;
                }
            }
        }

        void Handshake(NetworkStream stream, FrameReader reader)
        {
            // Initiate the V2 protocol
            stream.Write(MAGIC_V2, 0, MAGIC_V2.Length);
            _identifyResponse = Identify(stream, reader);
            if (_identifyResponse.AuthRequired)
            {
                Dispose();
                throw new NotSupportedException("Authorization is not supported");
            }

            SendCommand(stream, new Subscribe(_topic, _channel));
        }

        IdentifyResponse Identify(NetworkStream stream, FrameReader reader)
        {
            var identify = new Identify(_options);
            SendCommand(stream, identify);
            var frame = reader.ReadFrame();
            if (frame.Type != FrameType.Result)
            {
                throw new InvalidOperationException("Unexpected frame type after IDENTIFY");
            }
            return identify.ParseIdentifyResponse(frame.Data);
        }

        void SendCommand(NetworkStream stream, ICommand command)
        {
            var msg = command.ToByteArray();
            stream.Write(msg, 0, msg.Length);
        }

        internal Task SendCommandAsync(ICommand command)
        {
            var msg = command.ToByteArray();
            return WriteAsync(msg);
        }
    }
}
