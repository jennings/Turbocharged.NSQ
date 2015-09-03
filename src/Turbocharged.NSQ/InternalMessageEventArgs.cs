using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public class InternalMessageEventArgs : EventArgs
    {
        public InternalMessageEventArgs(string message)
            : base()
        {
            this.Message = message;
        }

        public string Message { get; private set; }
    }
}
