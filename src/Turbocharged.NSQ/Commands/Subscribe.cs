using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    class Subscribe : ICommand
    {
        readonly Topic _topic;
        readonly Channel _channel;

        public Subscribe(Topic topic, Channel channel)
        {
            _topic = topic;
            _channel = channel;
        }

        static readonly byte[] SUB_SPACE = Encoding.ASCII.GetBytes("SUB ");

        public byte[] ToByteArray()
        {
            return SUB_SPACE
                .Concat(_topic.ToUTF8())
                .Concat(ByteArrays.SPACE)
                .Concat(_channel.ToUTF8())
                .Concat(ByteArrays.LF)
                .ToArray();
        }
    }
}
