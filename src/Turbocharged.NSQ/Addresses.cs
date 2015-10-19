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
    public class LookupAddress
    {
        public string HostName { get; set; }
        public int HttpPort { get; set; }

        public override string ToString()
        {
            return "HostName = " + HostName + ", HttpPort = " + HttpPort;
        }
    }

    /// <summary>
    /// The address of an nsqd instance.
    /// </summary>
    public class NsqAddress
    {
        public string BroadcastAddress { get; set; }
        public string HostName { get; set; }
        public int HttpPort { get; set; }
        public int TcpPort { get; set; }

        public override string ToString()
        {
            return "BroadcastAddress = " + BroadcastAddress + ", TcpPort = " + TcpPort;
        }
    }
}
