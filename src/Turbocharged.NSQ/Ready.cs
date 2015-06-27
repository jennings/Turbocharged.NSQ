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

        public byte[] ToByteArray()
        {
            return new[] { 'R', 'D', 'Y', ' ' }
                .Select(ch => (byte)ch)
                .Concat(Encoding.UTF8.GetBytes(_count.ToString()))
                .Concat(new[] { (byte)'\n' })
                .ToArray();
        }
    }
}
