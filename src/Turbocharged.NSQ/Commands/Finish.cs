using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    class Finish : ICommand
    {
        static readonly byte[] FIN_SPACE = Encoding.ASCII.GetBytes("FIN ");

        Message _message;

        public Finish(Message message)
        {
            _message = message;
        }

        public byte[] ToByteArray()
        {
            return FIN_SPACE
                .Concat(Encoding.UTF8.GetBytes(_message.Id))
                .Concat(ByteArrays.LF)
                .ToArray();
        }
    }
}
