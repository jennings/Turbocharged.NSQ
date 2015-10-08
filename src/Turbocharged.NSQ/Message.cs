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

        const int TIMESTAMP_START = 0;
        const int TIMESTAMP_COUNT = 8;
        const int ATTEMPTS_START = 8;
        const int ATTEMPTS_COUNT = 2;
        const int ID_START = 10;
        const int ID_COUNT = 16;
        const int DATA_START = TIMESTAMP_COUNT + ATTEMPTS_COUNT + ID_COUNT;

        internal Message(Frame frame, NsqTcpConnection connection)
        {
            _connection = connection;

            if (frame.Type != FrameType.Message)
                throw new ArgumentException("Frame must have FrameType 'Message'", "frame");

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(frame.Data, TIMESTAMP_START, TIMESTAMP_COUNT);
                Array.Reverse(frame.Data, ATTEMPTS_START, ATTEMPTS_COUNT);
            }

            Timestamp = BitConverter.ToInt64(frame.Data, TIMESTAMP_START);
            Attempts = BitConverter.ToInt16(frame.Data, ATTEMPTS_START);
            Id = Encoding.ASCII.GetString(frame.Data, ID_START, ID_COUNT);

            // Data
            var dataLength = frame.Data.Length - DATA_START;
            Body = new byte[dataLength];
            Array.ConstrainedCopy(frame.Data, DATA_START, Body, 0, dataLength);
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
