using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ.Tests
{
    static class Settings
    {
        public static string NsqdHostName
        {
            get
            {
                var hostname = Environment.GetEnvironmentVariable("NSQD_HOSTNAME");
                return hostname ?? ConfigurationManager.AppSettings["NSQ.Hostname"];
            }
        }

        public static int NsqdTcpPort
        {
            get
            {
                var tcpPortStr = Environment.GetEnvironmentVariable("NSQD_TCP_PORT");
                return int.Parse(tcpPortStr ?? ConfigurationManager.AppSettings["NSQ.TcpPort"]);
            }
        }

        public static int NsqdHttpPort
        {
            get
            {
                var httpPortStr = Environment.GetEnvironmentVariable("NSQD_HTTP_PORT");
                return int.Parse(httpPortStr ?? ConfigurationManager.AppSettings["NSQ.HttpPort"]);
            }
        }
    }
}
