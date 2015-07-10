using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Turbocharged.NSQ
{
    public class NsqStatistics
    {
        [JsonProperty("health")]
        public string Health { get; internal set; }

        [JsonProperty("version")]
        public string Version { get; internal set; }

        [JsonProperty("start_time")]
        public int StartTime { get; internal set; }

        [JsonProperty("topics")]
        public List<TopicStatistics> Topics { get; internal set; }

        internal NsqStatistics() { }
    }

    public class TopicStatistics
    {
        [JsonProperty("topic_name")]
        public Topic Name { get; internal set; }

        [JsonProperty("depth")]
        public long Depth { get; internal set; }

        [JsonProperty("backend_depth")]
        public long BackendDepth { get; internal set; }

        [JsonProperty("message_count")]
        public long MessageCount { get; internal set; }

        [JsonProperty("paused")]
        public bool Paused { get; internal set; }

        [JsonProperty("channels")]
        public List<ChannelStatistics> Channels { get; internal set; }

        internal TopicStatistics() { }
    }

    public class ChannelStatistics
    {
        [JsonProperty("channel_name")]
        public Channel Name { get; set; }

        [JsonProperty("depth")]
        public long Depth { get; internal set; }

        [JsonProperty("backend_depth")]
        public long BackendDepth { get; internal set; }

        [JsonProperty("in_flight_count")]
        public long InFlightCount { get; internal set; }

        [JsonProperty("deferred_count")]
        public long DeferredCount { get; internal set; }

        [JsonProperty("message_count")]
        public long MessageCount { get; internal set; }

        [JsonProperty("timeout_count")]
        public long TimeoutCount { get; internal set; }

        [JsonProperty("paused")]
        public bool Paused { get; internal set; }

        internal ChannelStatistics() { }
    }
}
