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

        public byte[] ToByteArray()
        {
            return new[] { 'S', 'U', 'B', ' ' }
                .Select(ch => (byte)ch)
                .Concat(Encoding.UTF8.GetBytes(_topic))
                .Concat(new[] { (byte)' ' })
                .Concat(Encoding.UTF8.GetBytes(_channel))
                .Concat(new[] { (byte)'\n' })
                .ToArray();
        }
    }
}
