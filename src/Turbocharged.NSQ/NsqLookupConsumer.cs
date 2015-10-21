using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public class NsqLookupConsumer : INsqConsumer
    {
        readonly Dictionary<DnsEndPoint, NsqTcpConnection> _connections = new Dictionary<DnsEndPoint, NsqTcpConnection>();
        readonly List<NsqLookup> _lookupServers = new List<NsqLookup>();
        readonly ConsumerOptions _options;
        readonly Task _firstConnectionTask;
        readonly TaskCompletionSource<bool> _firstConnectionTaskCompletionSource = new TaskCompletionSource<bool>();

        System.Threading.Timer _lookupTimer;
        bool _firstDiscoveryCycle = true;
        int _maxInFlight = 0;
        int _started = 0;

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

        public NsqLookupConsumer(ConsumerOptions options)
        {
            _options = options;

            foreach (var lookupEndPoint in options.LookupEndPoints)
            {
                _lookupServers.Add(new NsqLookup(lookupEndPoint.Host, lookupEndPoint.Port));
            }

            _firstConnectionTask = _firstConnectionTaskCompletionSource.Task;
        }

        public NsqLookupConsumer(string connectionString)
            : this(ConsumerOptions.Parse(connectionString))
        {
        }

        public async Task ConnectAndWaitAsync(MessageHandler handler)
        {
            Connect(handler);
            await _firstConnectionTask.ConfigureAwait(false);
        }

        public void Connect(MessageHandler handler)
        {
            var wasStarted = Interlocked.CompareExchange(ref _started, 1, 0);
            if (wasStarted != 0) return;

            _lookupTimer = new System.Threading.Timer(LookupTask, handler, TimeSpan.Zero, _options.LookupPeriod);
        }

        void LookupTask(object messageHandler)
        {
            MessageHandler handler = (MessageHandler)messageHandler;

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
                        var connection = new NsqTcpConnection(endPoint, _options, _noRetryBackoff);
                        connection.InternalMessages +=
                            ((EventHandler<InternalMessageEventArgs>)((sender, e) => OnInternalMessage("{0}: {1}", endPoint, e.Message)));
                        connection.Connect(handler);
                        _connections[endPoint] = connection;
                        added++;
                    }
                }

                beginningCount = currentEndPoints.Count;
                endingCount = _connections.Count;

                OnDiscoveryCompleted(nsqAddresses.ToList());

                if (_firstDiscoveryCycle)
                {
                    _firstConnectionTaskCompletionSource.TrySetResult(true);
                    _firstDiscoveryCycle = false;
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
