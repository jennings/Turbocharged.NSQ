using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    class Finish : ICommand
    {
        Message _message;

        public Finish(Message message)
        {
            _message = message;
        }

        public byte[] ToByteArray()
        {
            return new[] { 'F', 'I', 'N', ' ' }
                .Select(ch => (byte)ch)
                .Concat(Encoding.UTF8.GetBytes(_message.Id))
                .Concat(new[] { (byte)'\n' })
                .ToArray();
        }
    }
}
