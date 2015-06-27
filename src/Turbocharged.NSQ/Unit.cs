using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    /// <summary>
    /// A "void" value so everything can be a Func instead of Funcs and Actions.
    /// </summary>
    struct Unit
    {
        public static readonly Unit Default = new Unit();
    }
}
