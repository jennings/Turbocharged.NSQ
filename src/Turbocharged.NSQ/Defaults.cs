using System;
using System.Net.Http;

namespace Turbocharged.NSQ
{
    static class Defaults
    {
        public static Lazy<HttpClient> HttpClient { get; } = new Lazy<HttpClient>(() => new HttpClient());
    }
}
