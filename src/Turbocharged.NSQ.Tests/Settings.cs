using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ.Tests
{
    static class Settings
    {
        static IConfigurationRoot Configuration { get; } =
            new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json")
            .Build();

        public static string NsqdHostName
        {
            get
            {
                var hostname = Environment.GetEnvironmentVariable("NSQD_HOSTNAME");
                return hostname ?? Configuration["NSQ.Hostname"];
            }
        }

        public static int NsqdTcpPort
        {
            get
            {
                var tcpPortStr = Environment.GetEnvironmentVariable("NSQD_TCP_PORT");
                return int.Parse(tcpPortStr ?? Configuration["NSQ.TcpPort"]);
            }
        }

        public static int NsqdHttpPort
        {
            get
            {
                var httpPortStr = Environment.GetEnvironmentVariable("NSQD_HTTP_PORT");
                return int.Parse(httpPortStr ?? Configuration["NSQ.HttpPort"]);
            }
        }

        public static string LookupHostName
        {
            get
            {
                var hostname = Environment.GetEnvironmentVariable("NSQLOOKUPD_HOSTNAME");
                return hostname ?? Configuration["Lookup.Hostname"];
            }
        }

        public static int LookupPort
        {
            get
            {
                var tcpPortStr = Environment.GetEnvironmentVariable("NSQLOOKUPD_TCP_PORT");
                return int.Parse(tcpPortStr ?? Configuration["Lookup.Port"]);
            }
        }
    }
}
