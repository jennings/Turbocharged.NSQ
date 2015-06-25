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
    public class LookupdConnection
    {
        readonly WebClient _webClient = new WebClient();
        readonly SemaphoreSlim _webClientLock = new SemaphoreSlim(1, 1);

        public LookupdConnection(string host, int port)
        {
            _webClient.BaseAddress = new UriBuilder()
            {
                Scheme = "http",
                Host = host,
                Port = port,
            }.ToString();
        }

        public Task<List<NsqdAddress>> LookupAsync(Topic topic)
        {
            return RequestListAsync("/lookup?topic=" + topic, response =>
            {
                return
                    ((JArray)response["data"]["producers"])
                    .Select(producer => new NsqdAddress
                    {
                        BroadcastAddress = (string)producer["broadcast_address"],
                        HostName = (string)producer["hostname"],
                        HttpPort = (int)producer["http_port"],
                        TcpPort = (int)producer["tcp_port"],
                    })
                    .ToList();
            });
        }

        public Task<List<Topic>> TopicsAsync()
        {
            return RequestListAsync("/topics", response =>
            {
                return response["data"]["topics"]
                    .Select(t => new Topic((string)t))
                    .ToList();
            });
        }

        public Task<List<Channel>> ChannelsAsync(Topic topic)
        {
            return RequestListAsync("/channels?topic=" + topic, response =>
            {
                return response["data"]["channels"]
                    .Select(t => new Channel((string)t))
                    .ToList();
            });
        }

        async Task<List<T>> RequestListAsync<T>(string url, Func<JObject, List<T>> handler)
        {
            var result = await RequestAsync(url, handler).ConfigureAwait(false);
            return result ?? new List<T>();
        }

        async Task<T> RequestAsync<T>(string url, Func<JObject, T> handler)
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
            catch (WebException)
            {
                return default(T);
            }
            finally
            {
                _webClientLock.Release();
            }
        }
    }
}
