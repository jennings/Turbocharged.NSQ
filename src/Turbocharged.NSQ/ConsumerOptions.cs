using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    /// <summary>
    /// Configures the behavior of an NSQ consumer connection.
    /// </summary>
    public class ConsumerOptions
    {
        /// <summary>
        /// EndPoints for nsqlookupd instances to use. If any are present,
        /// this overrides the NsqEndPoint property.
        /// </summary>
        public HashSet<DnsEndPoint> LookupEndPoints { get; private set; }

        /// <summary>
        /// The EndPoint to a single nsqd service to use. If any Lookup endpoints
        /// are present, this setting is ignored.
        /// </summary>
        public DnsEndPoint NsqEndPoint { get; set; }

        /// <summary>
        /// The topic to which to subscribe.
        /// </summary>
        public Topic Topic { get; set; }

        /// <summary>
        /// The channel to which to subscribe.
        /// </summary>
        public Channel Channel { get; set; }


        /// <summary>
        /// An identifier for this particular consumer.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The hostname of the computer connected to NSQ.
        /// </summary>
        public string HostName { get; set; }

        public TimeSpan LookupPeriod { get; set; }

        /// <summary>
        /// The maximum number of messages allowed to be in-flight to this consumer. When
        /// connected to several NSQ instances, this number is distributed across the connected
        /// clients evenly.
        /// </summary>
        public int MaxInFlight { get; set; }

        /// <summary>
        /// The initial delay before attempting reconnection if the connection to NSQ fails.
        /// By default, the delay will be doubled on each attempt until reconnection, up to
        /// a maximum of <c>ReconnectionMaxDelay</c>.
        /// </summary>
        public TimeSpan ReconnectionDelay { get; set; }


        /// <summary>
        /// The maximum delay between reconnection attempts.
        /// </summary>
        public TimeSpan ReconnectionMaxDelay { get; set; }

        const string LOOKUPD_KEY = "lookupd";
        const string NSQD_KEY = "nsqd";
        const string TOPIC_KEY = "topic";
        const string CHANNEL_KEY = "channel";
        const string CLIENTID_KEY = "clientid";
        const string HOSTNAME_KEY = "hostname";
        const string LOOKUPPERIOD_KEY = "lookupperiod";
        const string MAXINFLIGHT_KEY = "maxinflight";
        const string RECONNECTIONDELAY_KEY = "reconnectiondelay";
        const string RECONNECTIONMAXDELAY_KEY = "reconnectionmaxdelay";

        const int DEFAULT_LOOKUPD_HTTP_PORT = 4061;
        const int DEFAULT_NSQD_TCP_PORT = 4050;

        /// <summary>
        /// Creates a default set of options.
        /// </summary>
        public ConsumerOptions()
        {
            LookupEndPoints = new HashSet<DnsEndPoint>();

            ClientId = "Turbocharged.NSQ";
            HostName = Environment.MachineName;
            LookupPeriod = TimeSpan.FromSeconds(15);
            MaxInFlight = 2500;
            ReconnectionDelay = TimeSpan.FromSeconds(1);
            ReconnectionMaxDelay = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Parses a connection string into a <c>ConsumerOptions</c> instance.
        /// </summary>
        /// <param name="connectionString">A semicolon-delimited list of key=value pairs of connection options.</param>
        /// <returns></returns>
        public static ConsumerOptions Parse(string connectionString)
        {
            var options = new ConsumerOptions();
            var parts = ParseIntoSegments(connectionString);

            var sb = new StringBuilder();

            if (parts.Contains(LOOKUPD_KEY))
            {
                foreach (var endPoint in ParseEndPoints(parts[LOOKUPD_KEY], DEFAULT_LOOKUPD_HTTP_PORT))
                {
                    options.LookupEndPoints.Add(endPoint);
                }

            }
            else if (parts.Contains(NSQD_KEY))
            {
                options.NsqEndPoint = ParseEndPoints(parts[NSQD_KEY], DEFAULT_NSQD_TCP_PORT).Last();
            }
            else
            {
                throw new ArgumentException("Must provide either nsqlookupd or nsqd endpoints");
            }

            if (parts.Contains(CLIENTID_KEY))
            {
                options.ClientId = parts[CLIENTID_KEY].Last();
            }

            if (parts.Contains(HOSTNAME_KEY))
            {
                options.HostName = parts[HOSTNAME_KEY].Last();
            }

            if (parts.Contains(LOOKUPPERIOD_KEY))
            {
                options.LookupPeriod = TimeSpan.FromSeconds(int.Parse(parts[LOOKUPPERIOD_KEY].Last()));
            }

            if (parts.Contains(MAXINFLIGHT_KEY))
            {
                options.MaxInFlight = int.Parse(parts[MAXINFLIGHT_KEY].Last());
            }

            if (parts.Contains(TOPIC_KEY))
            {
                options.Topic = parts[TOPIC_KEY].Last();
            }

            if (parts.Contains(CHANNEL_KEY))
            {
                options.Channel = parts[CHANNEL_KEY].Last();
            }

            if (parts.Contains(RECONNECTIONDELAY_KEY))
            {
                options.ReconnectionDelay = TimeSpan.FromSeconds(int.Parse(parts[RECONNECTIONDELAY_KEY].Last()));
            }

            if (parts.Contains(RECONNECTIONMAXDELAY_KEY))
            {
                options.ReconnectionMaxDelay = TimeSpan.FromSeconds(int.Parse(parts[RECONNECTIONMAXDELAY_KEY].Last()));
            }

            return options;
        }

        static ILookup<string, string> ParseIntoSegments(string connectionString)
        {
            return
                connectionString.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries))
                .Where(part => part.Length == 1 || part.Length == 2)
                .Select(part =>
                {
                    if (part.Length == 2)
                        return part;
                    else
                        return new[] { LOOKUPD_KEY, part[0] };
                })
                .ToLookup(
                    part => part[0].ToLowerInvariant().Trim(),
                    part => part[1].Trim());

        }

        static IEnumerable<DnsEndPoint> ParseEndPoints(IEnumerable<string> list, int defaultPort)
        {
            return list
                .Select(endpoint => endpoint.Trim())
                .Select(endpoint => endpoint.Split(new[] { ':' }, 2))
                .Select(endpointParts => new DnsEndPoint(endpointParts[0], endpointParts.Length == 2 ? int.Parse(endpointParts[1]) : defaultPort));
        }
    }
}
