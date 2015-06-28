using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    /// <summary>
    /// A message delivered from NSQ.
    /// </summary>
    public class Message
    {
        public string Id { get; set; }
        public short Attempts { get; set; }
        public long Timestamp { get; set; }
        public byte[] Data { get; set; }

        internal Message(Frame frame)
        {
            if (frame.Type != FrameType.Message)
                throw new ArgumentException("Frame must have FrameType 'Message'", "frame");

            byte[] timestampBuffer = new byte[8];
            byte[] attemptsBuffer = new byte[2];
            byte[] idBuffer = new byte[16];

            Array.ConstrainedCopy(frame.Data, 0, timestampBuffer, 0, 8);
            Array.ConstrainedCopy(frame.Data, 8, attemptsBuffer, 0, 2);
            Array.ConstrainedCopy(frame.Data, 10, idBuffer, 0, 16);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBuffer);
                Array.Reverse(attemptsBuffer);
            }

            Timestamp = BitConverter.ToInt64(timestampBuffer, 0);
            Attempts = BitConverter.ToInt16(attemptsBuffer, 0);
            Id = Encoding.ASCII.GetString(idBuffer);

            // Data
            const int DATA_OFFSET = 8 + 2 + 16;
            var dataLength = frame.Data.Length - DATA_OFFSET;
            Data = new byte[dataLength];
            Array.ConstrainedCopy(frame.Data, DATA_OFFSET, Data, 0, dataLength);
        }

        public void Finish()
        {
        }

        public void ReQueue()
        {
            ReQueue(TimeSpan.Zero);
        }

        public void ReQueue(TimeSpan delay)
        {
        }

        public void Touch()
        {
        }
    }
}
