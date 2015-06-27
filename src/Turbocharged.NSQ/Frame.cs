using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    enum FrameType
    {
        Result,
        Error,
        Message,
    }

    /// <summary>
    /// A frame of data received from nsqd.
    /// </summary>
    class Frame
    {
        public FrameType Type { get; set; }
        public int MessageSize { get; set; }

        public byte[] Data { get; set; }

        public string GetReadableData()
        {
            if (Data == null) return "(null)";
            return Encoding.ASCII.GetString(Data, 0, Data.Length);
        }
    }
}
