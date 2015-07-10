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
    public sealed class NsqProducer
    {
        readonly WebClient _webClient = new WebClient();
        readonly SemaphoreSlim _webClientLock = new SemaphoreSlim(1, 1);

        readonly static byte[] EMPTY = new byte[0];

        public NsqProducer(string host, int port)
        {
            _webClient.BaseAddress = new UriBuilder()
            {
                Scheme = "http",
                Host = host,
                Port = port,
            }.ToString();
        }

        public Task CreateTopicAsync(Topic topic)
        {
            return PostAsync("/topic/create?topic=" + topic, _ => true);
        }

        public Task DeleteTopicAsync(Topic topic)
        {
            return PostAsync("/topic/delete?topic=" + topic, _ => true);
        }

        public Task EmptyTopicAsync(Topic topic)
        {
            return PostAsync("/topic/empty?topic=" + topic, _ => true);
        }

        public Task PauseTopicAsync(Topic topic)
        {
            return PostAsync("/topic/pause?topic=" + topic, _ => true);
        }

        public Task UnpauseTopicAsync(Topic topic)
        {
            return PostAsync("/topic/unpause?topic=" + topic, _ => true);
        }

        public Task CreateChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/create?topic=" + topic + "&channel=" + channel, _ => true);
        }

        public Task DeleteChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/delete?topic=" + topic + "&channel=" + channel, _ => true);
        }

        public Task EmptyChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/empty?topic=" + topic + "&channel=" + channel, _ => true);
        }

        public Task PauseChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/pause?topic=" + topic + "&channel=" + channel, _ => true);
        }

        public Task UnpauseChannelAsync(Topic topic, Channel channel)
        {
            return PostAsync("/channel/unpause?topic=" + topic + "&channel=" + channel, _ => true);
        }

        public Task PingAsync()
        {
            return GetAsync("/ping", _ => true);
        }

        public Task<NsqStatistics> StatisticsAsync()
        {
            return GetAsync("/stats?format=json", response => response["data"].ToObject<NsqStatistics>());
        }

        public Task PublishAsync(Topic topic, byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentOutOfRangeException("data", "Must provide data to publish");

            return PostAsync("/pub?topic=" + topic, data, _ => true);
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
