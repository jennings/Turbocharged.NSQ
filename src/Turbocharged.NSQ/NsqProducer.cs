using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Turbocharged.NSQ
{
    /// <summary>
    /// A client for nsqd which delivers messages using the HTTP protocol.
    /// </summary>
    public sealed class NsqProducer
    {
        readonly WebClient _webClient = new WebClient();
        readonly SemaphoreSlim _webClientLock = new SemaphoreSlim(1, 1);

        readonly static byte[] EMPTY = new byte[0];

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="host">The host name or IP address of the nsqd instance.</param>
        /// <param name="port">The HTTP port of the nsqd instance.</param>
        public NsqProducer(string host, int port)
        {
            _webClient.BaseAddress = new UriBuilder()
            {
                Scheme = "http",
                Host = host,
                Port = port,
            }.ToString();
        }

        /// <summary>
        /// Creates a new topic on the nsqd instance.
        /// </summary>
        public Task CreateTopicAsync(Topic topic)
        {
            return PostAsync("/topic/create?topic=" + topic, _ => true);
        }

        /// <summary>
        /// Deletes a topic from the nsqd instance.
        /// </summary>
        public Task DeleteTopicAsync(Topic topic)
        {
            return PostAsync("/topic/delete?topic=" + topic, _ => true);
        }

        /// <summary>
        /// Clears all messages from the topic on the nsqd instance.
        /// </summary>
        public Task EmptyTopicAsync(Topic topic)
        {
            return PostAsync("/topic/empty?topic=" + topic, _ => true);
        }

        /// <summary>
        /// Pauses a topic on the nsqd instance.
        /// </summary>
        public Task PauseTopicAsync(Topic topic)
        {
            return PostAsync("/topic/pause?topic=" + topic, _ => true);
        }

        /// <summary>
        /// Unpauses a topic on the nsqd instance.
        /// </summary>
        public Task UnpauseTopicAsync(Topic topic)
        {
            return PostAsync("/topic/unpause?topic=" + topic, _ => true);
        }

        /// <summary>
        /// Creates a channel on the nsqd instance.
        /// </summary>
        public Task CreateChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/create?topic=" + topic + "&channel=" + channel, _ => true);
        }

        /// <summary>
        /// Deletes a channel from a topic on the nsqd instance.
        /// </summary>
        public Task DeleteChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/delete?topic=" + topic + "&channel=" + channel, _ => true);
        }

        /// <summary>
        /// Clears all messages from a channel on the nsqd instance.
        /// </summary>
        public Task EmptyChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/empty?topic=" + topic + "&channel=" + channel, _ => true);
        }

        /// <summary>
        /// Pauses a channel on the nsqd instance.
        /// </summary>
        public Task PauseChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/pause?topic=" + topic + "&channel=" + channel, _ => true);
        }

        /// <summary>
        /// Unpauses a channel on the nsqd instance.
        /// </summary>
        public Task UnpauseChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/unpause?topic=" + topic + "&channel=" + channel, _ => true);
        }

        /// <summary>
        /// Determines the liveliness of the nsqd instance.
        /// </summary>
        public Task PingAsync()
        {
            return GetAsync("/ping", _ => true);
        }

        /// <summary>
        /// Queries for runtime statistics of the nsqd instance.
        /// </summary>
        public Task<NsqStatistics> StatisticsAsync()
        {
            return GetAsync("/stats?format=json", response => response["data"].ToObject<NsqStatistics>());
        }

        /// <summary>
        /// Publishes a message to the nsqd instance.
        /// </summary>
        public Task PublishAsync(Topic topic, MessageBody data)
        {
            if (data == null || data.IsNull)
                throw new ArgumentOutOfRangeException("data", "Must provide data to publish");

            return PostAsync("/pub?topic=" + topic, data, _ => true);
        }

        /// <summary>
        /// Publishes multiple messages to the nsqd instance in a single HTTP request.
        /// </summary>
        public Task PublishAsync(Topic topic, MessageBody[] messages)
        {
            byte[] totalArray;
            checked
            {
                var dataLength = messages.Sum(msg => ((byte[])msg).Length);
                var totalLength = dataLength + (4 * messages.Length) + 4;
                totalArray = new byte[totalLength];
            }

            Array.Copy(BitConverter.GetBytes(messages.Length), totalArray, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(totalArray, 0, 4);

            int messageLength, offsetIntoTotalArray = 4;
            for (int i = 0; i < messages.Length; i++)
            {
                messageLength = ((byte[])messages[i]).Length;
                Array.Copy(BitConverter.GetBytes(messageLength), 0, totalArray, offsetIntoTotalArray, 4);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(totalArray, offsetIntoTotalArray, 4);
                offsetIntoTotalArray += 4;
                Array.Copy(messages[i], 0, totalArray, offsetIntoTotalArray, messageLength);
                offsetIntoTotalArray += messageLength;
            }

            return PostAsync("/mpub?binary=true&topic=" + topic, totalArray, _ => true);
        }

        Task<T> PostAsync<T>(string url, Func<byte[], T> handler)
        {
            return PostAsync<T>(url, EMPTY, handler);
        }

        async Task<T> PostAsync<T>(string url, byte[] data, Func<byte[], T> handler)
        {
            await _webClientLock.WaitAsync().ConfigureAwait(false);
            try
            {
                byte[] response = await _webClient.UploadDataTaskAsync(url, data).ConfigureAwait(false);
                return handler(response);
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                    throw;

                // Even if we get a 4xx, we should be able to continue
                using (var responseStream = ex.Response.GetResponseStream())
                {
                    // Even 4xx errors should send us content
                    var responseLength = (int)responseStream.Length;
                    if (responseLength == 0)
                        throw;

                    var response = new byte[responseLength];
                    responseStream.Read(response, 0, responseLength);
                    return handler(response);
                }
            }
            finally
            {
                _webClientLock.Release();
            }
        }

        async Task<T> GetAsync<T>(string url, Func<JObject, T> handler)
        {
            await _webClientLock.WaitAsync().ConfigureAwait(false);
            try
            {
                string data = await _webClient.DownloadStringTaskAsync(url).ConfigureAwait(false);
                var response = JObject.Parse(data);
                if (response["data"] == null)
                {
                    return default(T);
                }

                return handler(response);
            }
            finally
            {
                _webClientLock.Release();
            }
        }
    }
}
