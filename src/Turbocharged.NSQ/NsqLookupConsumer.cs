using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public class NsqLookupConsumer : INsqConsumer
    {
        readonly Dictionary<DnsEndPoint, NsqTcpConnection> _connections = new Dictionary<DnsEndPoint, NsqTcpConnection>();
        readonly List<NsqLookup> _lookupServers = new List<NsqLookup>();
        readonly System.Threading.Timer _lookupTimer;
        readonly ConsumerOptions _options;
        readonly MessageHandler _handler;
        readonly Task _firstConnectionTask;

        bool firstDiscoveryCycle = true;
        int _maxInFlight = 0;

        // No need to ever reconnect, we'll reconnect on the next lookup cycle
        static readonly NoRetryBackoffStrategy _noRetryBackoff = new NoRetryBackoffStrategy();

        public EventHandler<InternalMessageEventArgs> InternalMessages;
        public EventHandler<DiscoveryEventArgs> DiscoveryCompleted;

        internal void OnDiscoveryCompleted(List<NsqAddress> addresses)
        {
            var handler = DiscoveryCompleted;
            if (handler != null)
            {
                handler(this, new DiscoveryEventArgs(addresses));
            }
        }

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

        NsqLookupConsumer(ConsumerOptions options, MessageHandler handler)
        {
            _options = options;
            _handler = handler;

            foreach (var lookupEndPoint in options.LookupEndPoints)
            {
                _lookupServers.Add(new NsqLookup(lookupEndPoint.Host, lookupEndPoint.Port));
            }

            var tcs = new TaskCompletionSource<bool>();
            _firstConnectionTask = tcs.Task;

            // Start the lookup timer
            _lookupTimer = new System.Threading.Timer(LookupTask, tcs, TimeSpan.Zero, _options.LookupPeriod);
        }

        public static async Task<NsqLookupConsumer> ConnectAndWaitAsync(string connectionString, MessageHandler handler)
        {
            var consumer = Connect(connectionString, handler);
            await consumer._firstConnectionTask.ConfigureAwait(false);
            return consumer;
        }

        public static async Task<NsqLookupConsumer> ConnectAndWaitAsync(ConsumerOptions options, MessageHandler handler)
        {
            var consumer = Connect(options, handler);
            await consumer._firstConnectionTask.ConfigureAwait(false);
            return consumer;
        }

        public static NsqLookupConsumer Connect(string connectionString, MessageHandler handler)
        {
            return Connect(ConsumerOptions.Parse(connectionString), handler);
        }

        public static NsqLookupConsumer Connect(ConsumerOptions options, MessageHandler handler)
        {
            var consumer = new NsqLookupConsumer(options, handler);
            return consumer;
        }

        void LookupTask(object firstConnection)
        {
            var firstConnectionTcs = (TaskCompletionSource<bool>)firstConnection;

            OnInternalMessage("Begin lookup cycle");
            int beginningCount, endingCount,
                added = 0, removed = 0;

            lock (_connections)
            {
                var tasks = _lookupServers.Select(server => server.LookupAsync(_options.Topic)).ToList();
                var delay = Task.Delay(5000);
                Task.WhenAny(Task.WhenAll(tasks), delay).Wait();

                var nsqAddresses =
                    tasks.Where(t => t.Status == TaskStatus.RanToCompletion)
                    .SelectMany(t => t.Result)
                    .Distinct()
                    .ToList();

                var servers =
                    nsqAddresses
                    .Select(add => new DnsEndPoint(add.BroadcastAddress, add.TcpPort))
                    .ToList();

                var currentEndPoints = _connections.Keys;
                var newEndPoints = servers.Except(currentEndPoints);
                var removedEndPoints = currentEndPoints.Except(servers);

                foreach (var endPoint in removedEndPoints)
                {
                    var connection = _connections[endPoint];
                    _connections.Remove(endPoint);
                    connection.Dispose();
                    removed++;
                }

                foreach (var endPoint in newEndPoints)
                {
                    if (!_connections.ContainsKey(endPoint))
                    {
                        var connection = NsqTcpConnection.Connect(endPoint, _options, _noRetryBackoff, _handler);
                        connection.InternalMessages +=
                            ((EventHandler<InternalMessageEventArgs>)((sender, e) => OnInternalMessage("{0}: {1}", endPoint, e.Message)));
                        _connections[endPoint] = connection;
                        added++;
                    }
                }

                beginningCount = currentEndPoints.Count;
                endingCount = _connections.Count;

                OnDiscoveryCompleted(nsqAddresses.ToList());

                if (firstDiscoveryCycle)
                {
                    firstConnectionTcs.TrySetResult(true);
                    firstDiscoveryCycle = false;
                }
            }

            OnInternalMessage("End lookup cycle. BeginningCount = {0}, EndingCount = {1}, Added = {2}, Removed = {3}", beginningCount, endingCount, added, removed);
        }

        public async Task WriteAsync(MessageBody message)
        {
            await _firstConnectionTask.ConfigureAwait(false);

            List<NsqTcpConnection> connections;
            lock (_connections)
            {
                connections = _connections.Values.ToList();
            }

            if (connections.Count == 0)
                throw new CommunicationException("No NSQ connections are available");

            foreach (var thing in connections)
            {
                try
                {
                    await thing.WriteAsync(message).ConfigureAwait(false);
                    return;
                }
                catch
                {
                    continue;
                }
            }

            throw new CommunicationException("Write failed against all NSQ connections");
        }

        public async Task SetMaxInFlightAsync(int maxInFlight)
        {
            _maxInFlight = maxInFlight;
            await _firstConnectionTask.ConfigureAwait(false);

            List<NsqTcpConnection> connections;
            lock (_connections)
            {
                connections = _connections.Values.ToList();
            }

            if (connections.Count == 0) return;

            int maxInFlightPerServer = maxInFlight / connections.Count;
            int leftover = maxInFlight - (maxInFlightPerServer * connections.Count);

            var tasks = new List<Task>(connections.Count);
            foreach (var connection in connections)
            {
                int max = maxInFlightPerServer;
                if (leftover > 0) max += leftover--;
                var setMaxTask = connection.SetMaxInFlightAsync(max)
                    .ContinueWith(t =>
                    {
                        if (t.Status == TaskStatus.Faulted)
                        {
                            OnInternalMessage("Setting MaxInFlight on {0} threw: {1}", connection._endPoint, t.Exception.GetBaseException().Message);
                        }
                    });
                tasks.Add(connection.SetMaxInFlightAsync(max));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public void Dispose()
        {
            lock (_connections)
            {
                _lookupTimer.Dispose();

                foreach (var connection in _connections.Values)
                    connection.Dispose();
            }
        }
    }
}
