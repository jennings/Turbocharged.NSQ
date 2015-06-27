using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    class Nop : ICommand
    {
        static readonly byte[] NOP = new[] { 'N', 'O', 'P', '\n' }.Select(ch => (byte)ch).ToArray();

        public byte[] ToByteArray()
        {
            return NOP.ToArray();
        }
    }
}
