using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public class DiscoveryEventArgs : EventArgs
    {
        public List<NsqAddress> NsqAddresses { get; private set; }

        public DiscoveryEventArgs(List<NsqAddress> addresses)
        {
            NsqAddresses = addresses;
        }
    }
}
