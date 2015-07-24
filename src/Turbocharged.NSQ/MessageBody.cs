using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public struct MessageBody
    {
        static readonly byte[] EMPTY = new byte[0];

        readonly byte[] _data;

        MessageBody(byte[] data)
        {
            _data = data;
        }

        public override string ToString()
        {
            return (string)this;
        }

        public bool IsNull { get { return _data == null || _data.Length == 0; } }

        public static implicit operator MessageBody(byte[] msg)
        {
            if (msg == null) return default(MessageBody);
            return new MessageBody(msg);
        }

        public static implicit operator MessageBody(string msg)
        {
            if (msg == null) return default(MessageBody);
            var data = Encoding.UTF8.GetBytes(msg);
            return new MessageBody(data);
        }

        public static implicit operator byte[](MessageBody msg)
        {
            return msg._data;
        }

        public static implicit operator string(MessageBody msg)
        {
            if (msg._data == null)
                return null;

            try
            {
                return Encoding.UTF8.GetString(msg._data);
            }
            catch
            {
                return BitConverter.ToString(msg._data);
            }
        }
    }
}
