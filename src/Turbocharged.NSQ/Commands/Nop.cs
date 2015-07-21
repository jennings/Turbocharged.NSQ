using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    class Nop : ICommand
    {
        static readonly byte[] NOP_LF = Encoding.ASCII.GetBytes("NOP\n");

        public byte[] ToByteArray()
        {
            return NOP_LF.ToArray(); // Make a new one
        }
    }
}
