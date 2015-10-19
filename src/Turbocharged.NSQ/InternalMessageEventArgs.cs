using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    /// <summary>
    /// EventArgs for a message raised by the internals of Turbocharged.NSQ.
    /// </summary>
    public class InternalMessageEventArgs : EventArgs
    {
        internal InternalMessageEventArgs(string message)
            : base()
        {
            this.Message = message;
        }

        /// <summary>
        /// The message raised.
        /// </summary>
        public string Message { get; private set; }
    }
}
