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
        int nsqPort;

        public MainForm()
        {
            lookupHostName = Environment.GetEnvironmentVariable("NSQLOOKUPD_HOSTNAME") ?? "localhost";
            var lookupPortString = Environment.GetEnvironmentVariable("NSQLOOKUPD_PORT") ?? "4161";
            lookupPort = int.Parse(lookupPortString);

            nsqHostName = Environment.GetEnvironmentVariable("NSQD_HOSTNAME") ?? "localhost";
            var nsqPortString = Environment.GetEnvironmentVariable("NSQD_PORT") ?? "4150";
            nsqPort = int.Parse(nsqPortString);

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
            var form = new ConsumerForm(nsqHostName, nsqPort);
            form.Show();
            form.Activate();
        }
    }
}
