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
        static readonly byte[] HEARTBEAT = Encoding.ASCII.GetBytes("_heartbeat_");
        static readonly byte[] MAGIC_V2 = Encoding.ASCII.GetBytes("  V2");

        public EventHandler<InternalMessageEventArgs> InternalMessages;

        public bool Connected { get; private set; }

        readonly CancellationTokenSource _connectionClosedSource;
        readonly ConsumerOptions _options;
        readonly DnsEndPoint _endPoint;
        readonly HandlerFunc _messageHandler;
        readonly IBackoffStrategy _backoffStrategy;
        readonly Thread _workerThread;

        readonly object _connectionSwapLock = new object();
        readonly object _connectionSwapInProgressLock = new object();

        IdentifyResponse _identifyResponse;
        NetworkStream _stream;
        TaskCompletionSource<bool> _nextReconnectionTaskSource = new TaskCompletionSource<bool>();

        internal void OnInternalMessage(string format, object arg0)
        {
            var handler = InternalMessages;
            if (handler != null)
            {
                handler(this, new InternalMessageEventArgs(string.Format(format, arg0)));
            }
        }

        internal void OnInternalMessage(string format, params object[] args)
        {
            var handler = InternalMessages;
            if (handler != null)
            {
                handler(this, new InternalMessageEventArgs(string.Format(format, args)));
            }
        }

        internal NsqTcpConnection(DnsEndPoint endPoint, ConsumerOptions options, IBackoffStrategy backoffStrategy, HandlerFunc handler)
        {
            _endPoint = endPoint;
            _options = options;
            _messageHandler = handler;
            _backoffStrategy = backoffStrategy;

            _connectionClosedSource = new CancellationTokenSource();

            _workerThread = new Thread(WorkerLoop);
            _workerThread.Name = "Turbocharged.NSQ Worker";
        }

        public static NsqTcpConnection Connect(DnsEndPoint endPoint, ConsumerOptions options, HandlerFunc handler)
        {
            var backoffStrategy = new ExponentialBackoffStrategy(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30));
            return Connect(endPoint, options, backoffStrategy, handler);
        }

        internal static NsqTcpConnection Connect(DnsEndPoint endPoint, ConsumerOptions options, IBackoffStrategy backoffStrategy, HandlerFunc handler)
        {
            var nsq = new NsqTcpConnection(endPoint, options, backoffStrategy, handler);

            // The worker thread is responsible for:
            //   a. Connecting to NSQ
            //   b. Handshaking
            //   c. Dispatching received messages to the thread pool

            nsq.OnInternalMessage("Worker thread starting");
            nsq._workerThread.Start();
            nsq.OnInternalMessage("Worker thread started");

            return nsq;
        }

        object _disposeLock = new object();
        bool _disposed = false;

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (_disposed) return;
                _disposed = true;
                _connectionClosedSource.Cancel();
                _connectionClosedSource.Dispose();
            }
        }

        public async Task WriteAsync(MessageBody message)
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
                        byte[] buffer = message;
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
            TcpClient client = null;
            FrameReader reader = null;
            IBackoffLimiter backoffLimiter = null;
            IDisposable cancellationRegistration = Disposable.Empty;

            while (true)
            {
                try
                {
                    if (_connectionClosedSource.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!Connected)
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
                                    OnInternalMessage("Delaying {0} ms before reconnecting", (int)delay.TotalMilliseconds);
                                    Thread.Sleep(delay);
                                }
                                else
                                {
                                    // We give up
                                    OnInternalMessage("Abandoning connection");
                                    Dispose();
                                    return;
                                }
                            }

                            lock (_connectionSwapInProgressLock)
                            {
                                CancellationToken cancellationToken;
                                lock (_disposeLock)
                                {
                                    if (_disposed) return;

                                    if (client != null)
                                    {
                                        cancellationRegistration.Dispose();
                                        ((IDisposable)client).Dispose();
                                    }

                                    cancellationToken = _connectionClosedSource.Token;
                                }

                                OnInternalMessage("TCP client starting");
                                client = new TcpClient(_endPoint.Host, _endPoint.Port);
                                cancellationRegistration = cancellationToken.Register(() => ((IDisposable)client).Dispose(), false);
                                Connected = true;
                                OnInternalMessage("TCP client started");

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
                                OnInternalMessage("Heartbeat");
                                SendCommandAsync(new Nop())
                                    .ContinueWith(t => Dispose(), TaskContinuationOptions.OnlyOnFaulted);
                            }
                            else
                            {
                                OnInternalMessage("Received result. Length = {0}", frame.MessageSize);
                            }
                        }
                        else if (frame.Type == FrameType.Message)
                        {
                            OnInternalMessage("Received message. Length = {0}", frame.MessageSize);
                            var message = new Message(frame, this);
                            // TODO: Rethink this
                            ThreadPool.QueueUserWorkItem(new WaitCallback(_ => { _messageHandler(message); }));
                        }
                        else if (frame.Type == FrameType.Error)
                        {
                            OnInternalMessage("Received error. Length = {0}", frame.MessageSize);
                        }
                        else
                        {
                            OnInternalMessage("Unknown message type: {0}", frame.Type);
                            throw new InvalidOperationException("Unknown message type " + frame.Type);
                        }
                    }
                }
                catch (IOException ex)
                {
                    OnInternalMessage("EXCEPTION: {0}", ex.Message);
                    Connected = false;
                    continue;
                }
                catch (SocketException ex)
                {
                    OnInternalMessage("EXCEPTION: {0}", ex.Message);
                    Connected = false;
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

            SendCommand(stream, new Subscribe(_options.Topic, _options.Channel));
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
