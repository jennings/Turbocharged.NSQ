using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Turbocharged.NSQ
{
    /// <summary>
    /// A client for querying an instance of nsqlookupd.
    /// </summary>
    public class NsqLookup
    {
        readonly string _host;
        readonly int _port;
        readonly HttpClient _httpClient;
        readonly SemaphoreSlim _httpClientLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Creates a new instance of <c>NsqLookup</c>.
        /// </summary>
        /// <param name="host">The host name or IP address of the nsqlookupd instance.</param>
        /// <param name="port">The HTTP port of the nsqlookupd instance.</param>
        public NsqLookup(string host, int port)
            : this(host, port, Defaults.HttpClient.Value)
        {
        }

        /// <summary>
        /// Creates a new instance of <c>NsqLookup</c>.
        /// </summary>
        /// <param name="host">The host name or IP address of the nsqlookupd instance.</param>
        /// <param name="port">The HTTP port of the nsqlookupd instance.</param>
        /// <param name="httpClient">The HttpClient to use for requests.</param>
        public NsqLookup(string host, int port, HttpClient httpClient)
        {
            _host = host;
            _port = port;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Looks up the nsqd instances which are producing a topic.
        /// </summary>
        public Task<List<NsqAddress>> LookupAsync(Topic topic)
        {
            return RequestListAsync("/lookup?topic=" + topic, response =>
            {
                return
                    ((JArray)response["producers"])
                    .Select(producer => new NsqAddress(
                            (string)producer["broadcast_address"],
                            (string)producer["hostname"],
                            (int)producer["tcp_port"],
                            (int)producer["http_port"]))
                    .ToList();
            });
        }

        /// <summary>
        /// Queries the list of topics known to this nsqlookupd instance.
        /// </summary>
        public Task<List<Topic>> TopicsAsync()
        {
            return RequestListAsync("/topics", response =>
            {
                return response["topics"]
                    .Select(t => new Topic((string)t))
                    .ToList();
            });
        }

        /// <summary>
        /// Queries the channels known to this nsqlookupd instance.
        /// </summary>
        /// <param name="topic">The topic to query.</param>
        public Task<List<Channel>> ChannelsAsync(Topic topic)
        {
            return RequestListAsync("/channels?topic=" + topic, response =>
            {
                return response["channels"]
                    .Select(t => new Channel((string)t))
                    .ToList();
            });
        }

        /// <summary>
        /// Queries the nsqd nodes known to this nsqlookupd instance.
        /// </summary>
        public Task<List<NsqAddress>> NodesAsync()
        {
            return RequestListAsync("/nodes", response =>
            {
                return
                    ((JArray)response["producers"])
                    .Select(producer => new NsqAddress(
                            (string)producer["broadcast_address"],
                            (string)producer["hostname"],
                            (int)producer["tcp_port"],
                            (int)producer["http_port"]))
                    .ToList();
            });
        }

        /// <summary>
        /// Deletes a topic.
        /// </summary>
        public Task DeleteTopicAsync(Topic topic)
        {
            return RequestAsync("/delete_topic?topic=" + topic, _ => true);
        }

        /// <summary>
        /// Deletes a channel.
        /// </summary>
        public Task DeleteChannelAsync(Topic topic, Channel channel)
        {
            var url = "/delete_channel?topic=" + topic + "&channel=" + channel;
            return RequestAsync(url, _ => true);
        }

        /// <summary>
        /// Tombstones a topic for an nsqd instance.
        /// </summary>
        public Task TombstoneTopicProducerAsync(Topic topic, NsqAddress producer)
        {
            var url = string.Format("/tombstone_topic_producer?topic={0}&node={1}:{2}", topic, producer.BroadcastAddress, producer.HttpPort);
            return RequestAsync(url, _ => true);
        }

        /// <summary>
        /// Queries the version of the nsqlookupd instance.
        /// </summary>
        public Task<Version> VersionAsync()
        {
            return RequestAsync("/info", response =>
            {
                var version = (string)response["data"]["version"];
                return new Version(version);
            });
        }

        /// <summary>
        /// Queries the nsqlookupd instance for liveliness.
        /// </summary>
        /// <returns>True if nsqlookupd returns "OK".</returns>
        public Task<bool> PingAsync()
        {
            return RequestAsync("/ping", response =>
            {
                return (string)response == "OK";
            });
        }

        async Task<List<T>> RequestListAsync<T>(string path, Func<JObject, List<T>> handler)
        {
            var result = await RequestAsync(path, handler).ConfigureAwait(false);
            return result ?? new List<T>();
        }

        async Task<T> RequestAsync<T>(string path, Func<JObject, T> handler)
        {
            await _httpClientLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var responseMessage = await _httpClient.GetAsync(BuildUrl(path)).ConfigureAwait(false);
                var content = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                var response = JObject.Parse(content);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return handler(response);
                }
                else
                {
                    throw new Exception((string)response["message"] ?? "Unknown response");
                }

            }
            finally
            {
                _httpClientLock.Release();
            }
        }

        string BuildUrl(string path)
        {
            return $"http://{_host}:{_port}{path}";
        }
    }
}
