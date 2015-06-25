using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public class Topic
    {
        readonly string _topic;
        public Topic(string topic)
        {
            if (topic == null) throw new ArgumentNullException("topic");
            _topic = topic;
        }

        public override string ToString()
        {
            return _topic;
        }

        public static implicit operator string(Topic topic)
        {
            return topic._topic;
        }

        public static implicit operator Topic(string topic)
        {
            return new Topic(topic);
        }
    }

    public class Channel
    {
        readonly string _channel;
        public Channel(string channel)
        {
            if (channel == null) throw new ArgumentNullException("channel");
            _channel = channel;
        }

        public override string ToString()
        {
            return _channel;
        }

        public static implicit operator string(Channel channel)
        {
            return channel._channel;
        }

        public static implicit operator Channel(string channel)
        {
            return new Channel(channel);
        }
    }
}
