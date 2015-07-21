using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    class Ready : ICommand
    {
        readonly int _count;

        public Ready(int count)
        {
            _count = count;
        }

        static readonly byte[] RDY_SPACE = Encoding.ASCII.GetBytes("RDY ");

        public byte[] ToByteArray()
        {
            return RDY_SPACE
                .Concat(Encoding.UTF8.GetBytes(_count.ToString()))
                .Concat(ByteArrays.LF)
                .ToArray();
        }
    }
}
