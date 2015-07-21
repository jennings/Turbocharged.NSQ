using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public enum ConnectionDiscoveryMode
    {
        Lookupd,
        Nsqd,
    }

    public class ConsumerOptions
    {
        /// <summary>
        /// Parses a connection string of the form:
        ///     "Lookupd=10.0.0.1:4160,10.0.0.2:4160"
        /// or:
        ///     "Nsqd=10.0.0.1:4150,10.0.0.2:4150"
        /// </summary>

        public ConnectionDiscoveryMode DiscoveryMode { get; set; }
        public HashSet<DnsEndPoint> LookupdEndPoints { get; private set; }
        public HashSet<DnsEndPoint> NsqdEndPoints { get; private set; }
        public Topic Topic { get; set; }
        public Channel Channel { get; set; }
        public string ClientId { get; set; }
        public string HostName { get; set; }
        public int MaxInFlight { get; set; }
        public TimeSpan ReconnectionDelay { get; set; }
        public TimeSpan ReconnectionMaxDelay { get; set; }

        public ConsumerOptions()
        {
            ClientId = "Turbocharged.NSQ";
            HostName = Environment.MachineName;
            MaxInFlight = 2500;
            ReconnectionDelay = TimeSpan.FromSeconds(1);
            ReconnectionMaxDelay = TimeSpan.FromSeconds(30);
            LookupdEndPoints = new HashSet<DnsEndPoint>();
            NsqdEndPoints = new HashSet<DnsEndPoint>();
        }

        public static ConsumerOptions Parse(string connectionString)
        {
            var parts =
                connectionString.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries))
                .Where(part => part.Length == 2)
                .ToDictionary(
                    part => part[0].ToLowerInvariant().Trim(),
                    part => part[1].Trim());

            if (!parts.ContainsKey("lookupd") && !parts.ContainsKey("nsqd"))
                throw new ArgumentException("Must provide either Lookupd or Nsqd endpoints in the connection string");

            if (parts.ContainsKey("lookupd") && parts.ContainsKey("nsqd"))
                throw new ArgumentException("Cannot provide both Lookupd and Nsqd endpoints in a connection string");

            var options = new ConsumerOptions();

            if (parts.ContainsKey("lookupd"))
            {
                options.DiscoveryMode = ConnectionDiscoveryMode.Lookupd;
                foreach (var endpoint in ParseEndPoints(parts["lookupd"], 4161))
                {
                    options.LookupdEndPoints.Add(endpoint);
                }
            }
            else
            {
                options.DiscoveryMode = ConnectionDiscoveryMode.Nsqd;
                foreach (var endpoint in ParseEndPoints(parts["nsqd"], 4150))
                {
                    options.NsqdEndPoints.Add(endpoint);
                }
            }

            if (parts.ContainsKey("clientid"))
                options.ClientId = parts["clientid"];

            if (parts.ContainsKey("hostname"))
                options.ClientId = parts["hostname"];

            if (parts.ContainsKey("maxinflight"))
                options.MaxInFlight = int.Parse(parts["maxinflight"]);

            if (parts.ContainsKey("topic"))
                options.Topic = parts["topic"];

            if (parts.ContainsKey("channel"))
                options.Channel = parts["channel"];

            return options;
        }

        static IEnumerable<DnsEndPoint> ParseEndPoints(string list, int defaultPort)
        {
            return
                list.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(endpoint => endpoint.Trim())
                .Select(endpoint => endpoint.Split(new[] { ':' }, 2))
                .Select(endpointParts => new DnsEndPoint(endpointParts[0], endpointParts.Length == 2 ? int.Parse(endpointParts[1]) : defaultPort));
        }
    }
}
