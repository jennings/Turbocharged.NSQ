using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    /// <summary>
    /// The address of an nsqlookupd instance.
    /// </summary>
    public struct LookupAddress : IEquatable<LookupAddress>
    {
        public string HostName { get; private set; }
        public int HttpPort { get; private set; }

        public LookupAddress(string hostName, int httpPort)
            : this()
        {
            HostName = hostName;
            HttpPort = httpPort;
        }

        public override string ToString()
        {
            return "HostName = " + HostName + ", HttpPort = " + HttpPort;
        }

        public override bool Equals(object obj)
        {
            if (obj is LookupAddress)
                return Equals((LookupAddress)obj);

            return false;
        }

        /// <summary>
        /// The address of an nsqd instance.
        /// </summary>
        public bool Equals(LookupAddress other)
        {
            return HttpPort == other.HttpPort
                && string.Equals(HostName, other.HostName, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    /// <summary>
    /// The address of an nsqd instance.
    /// </summary>
    public struct NsqAddress : IEquatable<NsqAddress>
    {
        public string BroadcastAddress { get; private set; }
        public string HostName { get; private set; }
        public int HttpPort { get; private set; }
        public int TcpPort { get; private set; }

        public NsqAddress(string broadcastAddress, string hostName, int tcpPort, int httpPort)
            : this()
        {
            BroadcastAddress = broadcastAddress;
            HostName = hostName;
            TcpPort = tcpPort;
            HttpPort = httpPort;
        }

        public override string ToString()
        {
            return "BroadcastAddress = " + BroadcastAddress + ", TcpPort = " + TcpPort;
        }

        public override bool Equals(object obj)
        {
            if (obj is NsqAddress)
                return Equals((NsqAddress)obj);

            return false;
        }

        public bool Equals(NsqAddress other)
        {
            return HttpPort == other.HttpPort
                && TcpPort == other.TcpPort
                && string.Equals(BroadcastAddress, other.BroadcastAddress, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(HostName, other.HostName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
