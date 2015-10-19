using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    /// <summary>
    /// Represends the body of an NSQ message. NSQ does not interpret the body
    /// of a message, so this is equivalent to a byte array. The library will
    /// automatically convert a MessageBody to and from a string in UTF-8 encoding.
    /// </summary>
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

        /// <summary>
        /// Converts a byte array to a MessageBody.
        /// </summary>
        public static implicit operator MessageBody(byte[] msg)
        {
            if (msg == null) return default(MessageBody);
            return new MessageBody(msg);
        }

        /// <summary>
        /// Encodes a string as UTF-8 and converts it to a MessageBody.
        /// </summary>
        public static implicit operator MessageBody(string msg)
        {
            if (msg == null) return default(MessageBody);
            var data = Encoding.UTF8.GetBytes(msg);
            return new MessageBody(data);
        }

        /// <summary>
        /// Converts a message body to the underlying byte array.
        /// </summary>
        public static implicit operator byte[](MessageBody msg)
        {
            return msg._data;
        }

        /// <summary>
        /// Converts a message from a UTF-8 encoded byte array to a string.
        /// </summary>
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
