using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    class Subscribe : ICommand<Unit>
    {
        TaskCompletionSource<Unit> _tcs = new TaskCompletionSource<Unit>();
        public Task<Unit> Task { get { return _tcs.Task; } }

        readonly Topic _topic;
        readonly Channel _channel;

        public Subscribe(Topic topic, Channel channel)
        {
            _topic = topic;
            _channel = channel;
        }

        public void Complete(byte[] data)
        {
            var str = Encoding.UTF8.GetString(data);
            switch (str)
            {
                case "OK":
                    _tcs.SetResult(Unit.Default);
                    return;

                case "E_INVALID":
                case "E_BAD_TOPIC":
                case "E_BAD_CHANNEL":
                    _tcs.SetException(new InvalidOperationException(str));
                    return;

                default:
                    _tcs.SetException(new InvalidOperationException("Unexpected response: " + str));
                    return;
            }
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
