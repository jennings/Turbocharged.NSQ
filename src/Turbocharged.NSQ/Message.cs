using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    enum MessageType
    {
        Result,
        Error,
        Message,
    }

    class Message
    {
        public MessageType Type { get; set; }
        public int MessageSize { get; set; }

        public byte[] Bytes { get; set; }
        public string Readable { get; set; }

        public long Timestamp { get; set; }
        public short Attempts { get; set; }
        public string MessageId { get; set; }
        public byte[] MessageBody { get; set; }
    }
}
