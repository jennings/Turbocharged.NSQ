using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Turbocharged.NSQ
{
    enum Compression
    {
        None,
        Snappy,
        Deflate,
    }

    class Identify : ICommand
    {
        public byte[] ToByteArray()
        {
            byte[] body;
            using (var stream = new MemoryStream(1024))
            using (var writer = new System.IO.StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, this);
                jsonWriter.Flush();
                writer.Flush();
                body = stream.ToArray();
            }

            byte[] length = BitConverter.GetBytes(body.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(length);

            return new[] { 'I', 'D', 'E', 'N', 'T', 'I', 'F', 'Y', '\n' }
                .Select(ch => (byte)ch)
                .Concat(length)
                .Concat(body)
                .ToArray();
        } 

        public Identify()
        {
            // Defaults
            ClientId = "Turbocharged.NSQ";
#pragma warning disable 0618
            ShortId = ClientId;
            LongId = ClientId;
#pragma warning restore 0618
            HostName = Environment.MachineName;
            FeatureNegotiation = true;
            TLS_V1 = false;
            Compression = NSQ.Compression.None;
        }

        /// <summary>
        /// (Deprecated in favor of client_id as of nsqd v0.2.28+) An identifier used as a short-form descriptor (i.e. short hostname).
        /// </summary>
        [Obsolete("Use ClientId")]
        [JsonProperty("short_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ShortId { get; set; }

        /// <summary>
        /// (Deprecated in favor of hostname as of nsqd v0.2.28+) An identifier used as a long-form descriptor (i.e. fully-qualified hostname).
        /// </summary>
        [Obsolete("Use ClientId")]
        [JsonProperty("long_id", NullValueHandling = NullValueHandling.Ignore)]
        public string LongId { get; set; }

        /// <summary>
        /// An identifier used to disambiguate this client (i.e. something specific to the consumer).
        /// </summary>
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// The hostname where the client is deployed.
        /// </summary>
        [JsonProperty("hostname")]
        public string HostName { get; set; }

        /// <summary>
        /// Used to indicate that the client supports feature negotiation. If the server is capable, it will send back a JSON payload of supported features and metadata.
        /// </summary>
        [JsonProperty("feature_negotiation")]
        public bool FeatureNegotiation { get; set; }

        /// <summary>
        /// Milliseconds between heartbeats.
        /// Valid range: 1000 <= heartbeat_interval <= configured_max (-1 disables heartbeats)
        /// --max-heartbeat-interval (nsqd flag) controls the max
        /// Defaults to --client-timeout / 2
        /// </summary>
        [JsonProperty("heartbeat_interval", NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan? HeartbeatInterval { get; set; }

        /// <summary>
        /// The size in bytes of the buffer nsqd will use when writing to this client.
        /// Valid range: 64 <= output_buffer_size <= configured_max (-1 disables output buffering)
        /// Defaults to 16kb
        /// </summary>
        [JsonProperty("output_buffer_size", NullValueHandling = NullValueHandling.Ignore)]
        public int? OutputBufferSize { get; set; }

        /// <summary>
        /// The timeout after which any data that nsqd has buffered will be flushed to this client.
        /// Valid range: 1ms <= output_buffer_timeout <= configured_max (-1 disables timeouts)
        /// </summary>
        [JsonIgnore]
        public TimeSpan? OutputBufferTimeout { get; set; }

        [JsonProperty("output_buffer_timeout", NullValueHandling = NullValueHandling.Ignore)]
        public int? OutputBufferTimeoutMilliseconds { get { return OutputBufferTimeout.HasValue ? OutputBufferTimeout.Value.Milliseconds : (int?)null; } }

        /// <summary>
        /// Enable TLS for this connection.
        /// </summary>
        [JsonProperty("tls_v1")]
        public bool TLS_V1 { get; set; }

        /// <summary>
        /// The type of compression to use on this connection.
        /// </summary>
        [JsonIgnore]
        public Compression Compression { get; set; }

        /// <summary>
        /// Use Snappy compression
        /// </summary>
        [JsonProperty("snappy")]
        public bool Snappy { get { return Compression == NSQ.Compression.Snappy; } }

        /// <summary>
        /// Use DEFLATE compression.
        /// </summary>
        [JsonProperty("deflate")]
        public bool Deflate { get { return Compression == NSQ.Compression.Deflate; } }

        /// <summary>
        /// Configure the deflate compression level for this connection.
        /// </summary>
        [JsonProperty("deflate_level", NullValueHandling = NullValueHandling.Ignore)]
        public int? DeflateLevel { get; set; }

        /// <summary>
        /// Deliver a percentage of all messages received to this connection.
        /// </summary>
        [JsonProperty("sample_rate")]
        public int SampleRate { get; set; }

        /// <summary>
        /// A string identifying the agent for this client in the spirit of HTTP
        /// Default: [client_library_name]/[version]
        /// </summary>
        [JsonProperty("user_agent")]
        public string UserAgent { get { return "Turbocharged.NSQ/1.0"; } }

        /// <summary>
        /// Configure the server-side message timeout in milliseconds for messages delivered to this client.
        /// </summary>
        [JsonIgnore]
        public TimeSpan? MessageTimeout { get; set; }

        [JsonProperty("msg_timeout")]
        public int? MessageTimeoutMilliseconds { get { return MessageTimeout.HasValue ? (int)MessageTimeout.Value.TotalMilliseconds : (int?)null; } }
    }
}
