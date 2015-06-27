using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public class NsqConnection
    {
        static readonly byte[] MAGIC_V2 = new byte[] { 32, 32, 86, 50 }; // "  V2"

        TcpClient _tcpClient;
        NetworkStream _stream;
        Thread _workerThread;
        BlockingCollection<ICommandWithResponse> _commandQueue;
        readonly SemaphoreSlim _commandQueueLock = new SemaphoreSlim(1, 1);

        public NsqConnection(string connectionString)
            : this(ConnectionOptions.Parse(connectionString))
        {
        }

        public NsqConnection(ConnectionOptions options)
        {
            ConnectionOptions = options;
        }

        public event Action<string> InternalMessages = _ => { };
        public event Action<Message> MessageReceived;
        public ConnectionOptions ConnectionOptions { get; private set; }

        public async Task ConnectAsync()
        {
            var ep = ConnectionOptions.NsqdEndPoints.First();
            var host = ep.Host;
            var port = ep.Port;
            InternalMessages("TCP client starting");
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(host, port).ConfigureAwait(false);
            InternalMessages("TCP client started");
            _stream = _tcpClient.GetStream();

            // Use the V2 protocol
            await _stream.WriteAsync(MAGIC_V2, 0, MAGIC_V2.Length).ConfigureAwait(false);

            // Begin the worker thread which receives and dispatches messages
            _commandQueue = new BlockingCollection<ICommandWithResponse>(new ConcurrentQueue<ICommandWithResponse>());
            _workerThread = new Thread(MessageReceiverLoop);
            InternalMessages("Worker thread starting");
            _workerThread.Start();
            InternalMessages("Worker thread started");

            // IDENTIFY
            await SendCommandAsync(new Identify()).ConfigureAwait(false);
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
                            ICommandWithResponse command;
                            if (!_commandQueue.TryTake(out command))
                            {
                                throw new InvalidOperationException("Received a response but wasn't waiting for a command");
                            }

                            InternalMessages("Received result. Length = " + frame.MessageSize);
                            command.Complete(frame.Data);
                        }
                    }
                    else if (frame.Type == FrameType.Message)
                    {
                        InternalMessages("Received message. Length = " + frame.MessageSize);
                        var listeners = MessageReceived;
                        if (listeners != null)
                        {
                            var message = new Message(frame);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(_ => listeners(message)));
                        }
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

        public Task SubscribeAsync(Topic topic, Channel channel)
        {
            return SendCommandAsync(new Subscribe(topic, channel));
        }
        public Task ReadyAsync(int count)
        {
            return SendCommandAsync(new Ready(count));
        }

        public void Close()
        {
            _tcpClient.Close();
        }

        async Task SendCommandAsync(ICommand command)
        {
            var msg = command.ToByteArray();
            var readableMsg = Encoding.UTF8.GetString(msg);

            var willUseCommandQueue = command is ICommandWithResponse;
            if (willUseCommandQueue)
            {
                await _commandQueueLock.WaitAsync().ConfigureAwait(false);
            }

            try
            {
                await _stream.WriteAsync(msg, 0, msg.Length).ConfigureAwait(false);
                if (willUseCommandQueue)
                {
                    _commandQueue.Add(command as ICommandWithResponse);
                }
            }
            finally
            {
                if (willUseCommandQueue)
                    _commandQueueLock.Release();
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
