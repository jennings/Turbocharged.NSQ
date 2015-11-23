using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ.Commands
{
    class Publish : ICommand
    {
        static readonly byte[] PREFIX = Encoding.ASCII.GetBytes("PUB ");

        readonly Topic _topic;
        readonly MessageBody _message;


        public Publish(Topic topic, MessageBody message)
        {
            _message = message;
            _topic = topic;
        }

        public byte[] ToByteArray()
        {
            byte[] messageBody = _message;
            byte[] size = BitConverter.GetBytes(messageBody.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(size);

            return PREFIX
                .Concat(_topic.ToUTF8())
                .Concat(ByteArrays.LF)
                .Concat(size)
                .Concat(messageBody)
                .ToArray();
        }
    }
}
