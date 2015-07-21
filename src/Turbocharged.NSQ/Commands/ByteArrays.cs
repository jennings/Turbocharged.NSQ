using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    static class ByteArrays
    {
        public static readonly byte[] LF = Encoding.ASCII.GetBytes("\n");
        public static readonly byte[] SPACE = Encoding.ASCII.GetBytes(" ");
    }
}
