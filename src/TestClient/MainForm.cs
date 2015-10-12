using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestClient
{
    public partial class MainForm : Form
    {
        string lookupHostName;
        int lookupPort;

        string nsqHostName;
        int nsqTcpPort, nsqHttpPort;

        public MainForm()
        {
            lookupHostName = Environment.GetEnvironmentVariable("NSQLOOKUPD_HOSTNAME") ?? "localhost";
            var lookupPortString = Environment.GetEnvironmentVariable("NSQLOOKUPD_HTTP_PORT") ?? "4161";
            lookupPort = int.Parse(lookupPortString);

            nsqHostName = Environment.GetEnvironmentVariable("NSQD_HOSTNAME") ?? "localhost";
            var nsqTcpPortString = Environment.GetEnvironmentVariable("NSQD_TCP_PORT") ?? "4150";
            var nsqHttpPortString = Environment.GetEnvironmentVariable("NSQD_HTTP_PORT") ?? "4151";
            nsqTcpPort = int.Parse(nsqTcpPortString);
            nsqHttpPort = int.Parse(nsqHttpPortString);

            InitializeComponent();
        }

        void button1_Click(object sender, EventArgs e)
        {
            var form = new LookupForm(lookupHostName, lookupPort);
            form.Show();
            form.Activate();
        }

        void button3_Click(object sender, EventArgs e)
        {
            var form = new ConsumerForm(nsqHostName, nsqTcpPort);
            form.Show();
            form.Activate();
        }

        void button2_Click(object sender, EventArgs e)
        {
            var form = new ProducerForm(nsqHostName, nsqHttpPort);
            form.Show();
            form.Activate();
        }

        void button4_Click(object sender, EventArgs e)
        {
            var connectionString = string.Format("{0}:{1}; topic={2}; channel={3}", lookupHostName, lookupPort, "foo", "bar");
            var form = new LookupConsumerForm(connectionString);
            form.Show();
            form.Activate();
        }
    }
}
