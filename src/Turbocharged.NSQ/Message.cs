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
        public string Id { get; private set; }
        public short Attempts { get; private set; }
        public long Timestamp { get; private set; }
        public MessageBody Body { get; private set; }

        readonly NsqTcpConnection _connection;

        internal Message(Frame frame, NsqTcpConnection connection)
        {
            _connection = connection;

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
            Body = new byte[dataLength];
            Array.ConstrainedCopy(frame.Data, DATA_OFFSET, Body, 0, dataLength);
        }

        public Task FinishAsync()
        {
            return _connection.SendCommandAsync(new Finish(this));
        }

        public Task RequeueAsync()
        {
            throw new NotImplementedException();
        }

        public Task TouchAsync()
        {
            throw new NotImplementedException();
        }
    }
}
