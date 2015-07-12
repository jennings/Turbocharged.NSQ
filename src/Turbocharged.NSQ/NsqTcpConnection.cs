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
        static readonly byte[] HEARTBEAT = new byte[] { 95, 104, 101, 97, 114, 116, 98, 101, 97, 116, 95 }; // "_heartbeat_"

        public event Action<string> InternalMessages = _ => { };

        ConsumerOptions _consumerOptions;
        DnsEndPoint _endPoint;
        ReliableConnection _connection;
        Thread _workerThread;
        HandlerFunc _messageHandler;
        bool _disposed = false;

        public NsqTcpConnection(DnsEndPoint endPoint, ConsumerOptions consumerOptions)
        {
            _endPoint = endPoint;
            _consumerOptions = consumerOptions;
        }

        public async Task ConnectAsync(Topic topic, Channel channel, HandlerFunc handler)
        {
            _messageHandler = handler;

            InternalMessages("TCP client starting");
            _disposed = false;
            _connection = new ReliableConnection(_endPoint, _consumerOptions);
            _connection.InternalMessages += InternalMessages;
            await _connection.ConnectAsync(topic, channel).ConfigureAwait(false);
            InternalMessages("TCP client started");

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
            if (_connection != null)
                _connection.Dispose();
        }

        async void MessageReceiverLoop()
        {
            try
            {
                Frame frame;
                while ((frame = await _connection.ReadFrameAsync()) != null)
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
            return _connection.WriteAsync(msg, 0, msg.Length);
        }
    }
}
