using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    class FrameReader
    {
        const int FRAME_SIZE_LENGTH = 4;
        const int FRAME_TYPE_LENGTH = 4;
        const int FRAME_HEADER_TOTAL_LENGTH = FRAME_SIZE_LENGTH + FRAME_TYPE_LENGTH;

        const int MESSAGE_TIMESTAMP_LENGTH = 4;
        const int MESSAGE_ATTEMPTS_LENGTH = 4;
        const int MESSAGE_ID_LENGTH = 4;
        readonly NetworkStream _stream;

        readonly object _lock = new object();
        readonly byte[] _frameSizeBuffer = new byte[FRAME_SIZE_LENGTH];
        readonly byte[] _frameTypeBuffer = new byte[FRAME_TYPE_LENGTH];

        public FrameReader(NetworkStream stream)
        {
            _stream = stream;
        }

        public Frame ReadFrame()
        {
            lock (_lock)
            {
                // MESSAGE FRAME FORMAT:
                //   4 bytes - Int32, size of the frame, excluding this field
                //   4 bytes - Int32, frame type
                //   N bytes - data
                //      8 bytes - Int64, timestamp
                //      2 bytes - UInt16, attempts
                //     16 bytes - Hex-string encoded message ID
                //      N bytes - message body

                // Get the size of the incoming frame
                ReadBytes(_frameSizeBuffer, 0, FRAME_SIZE_LENGTH);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(_frameSizeBuffer);
                var frameLength = BitConverter.ToInt32(_frameSizeBuffer, 0);

                // Read the rest of the frame
                var frame = ReadBytesWithAllocation(frameLength);

                // Get the frame type
                Array.ConstrainedCopy(frame, 0, _frameTypeBuffer, 0, FRAME_TYPE_LENGTH);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(_frameTypeBuffer);
                var frameType = (FrameType)BitConverter.ToInt32(_frameTypeBuffer, 0);

                // Get the data portion of the frame
                var dataLength = frameLength - FRAME_TYPE_LENGTH;
                byte[] dataBuffer = new byte[dataLength];
                Array.ConstrainedCopy(frame, FRAME_TYPE_LENGTH, dataBuffer, 0, dataLength);

                return new Frame
                {
                    MessageSize = frameLength,
                    Type = frameType,
                    Data = dataBuffer,
                };
            }
        }

        void ReadBytes(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            int bytesLeft = count;

            while ((bytesRead = _stream.Read(buffer, offset, bytesLeft)) > 0)
            {
                offset += bytesRead;
                bytesLeft -= bytesRead;
                if (offset > count) throw new InvalidOperationException("Read too many bytes");
                if (offset == count) break;
            }

            if (bytesLeft > 0)
                throw new SocketException((int)SocketError.SocketError);
        }

        byte[] ReadBytesWithAllocation(int count)
        {
            byte[] buffer = new byte[count];
            int offset = 0;
            ReadBytes(buffer, offset, count);
            return buffer;
        }
    }
}
