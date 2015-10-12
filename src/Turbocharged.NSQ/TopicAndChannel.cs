using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    /// <summary>
    /// A topic name in NSQ. Topics should represent a type of message,
    /// for example, "new-users" or "order-updated".
    /// </summary>
    public struct Topic
    {
        public string Name { get; private set; }

        public Topic(string topic)
            : this()
        {
            if (topic == null) throw new ArgumentNullException("topic");
            Name = topic;
        }

        public override string ToString()
        {
            return Name;
        }

        internal byte[] ToUTF8()
        {
            return Encoding.UTF8.GetBytes(Name);
        }

        public static implicit operator string(Topic topic)
        {
            return topic.Name;
        }

        public static implicit operator Topic(string topic)
        {
            return new Topic(topic);
        }
    }

    /// <summary>
    /// A channel name in NSQ. Channels should represent the action of a consumer,
    /// for example, "send_email" or "create_database_record".
    /// </summary>
    public struct Channel
    {
        public string Name { get; private set; }

        public Channel(string channel)
            : this()
        {
            if (channel == null) throw new ArgumentNullException("channel");
            Name = channel;
        }

        public override string ToString()
        {
            return Name;
        }

        internal byte[] ToUTF8()
        {
            return Encoding.UTF8.GetBytes(Name);
        }

        public static implicit operator string(Channel channel)
        {
            return channel.Name;
        }

        public static implicit operator Channel(string channel)
        {
            return new Channel(channel);
        }
    }
}
