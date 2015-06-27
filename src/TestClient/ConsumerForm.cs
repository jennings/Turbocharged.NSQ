using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Turbocharged.NSQ;

namespace TestClient
{
    public partial class ConsumerForm : Form
    {
        public ConsumerForm(string host, int port)
        {
            InitializeComponent();
            Host.Text = host;
            Port.Text = port.ToString();
        }

        async void ConnectButton_Click(object sender, EventArgs e)
        {
            var host = Host.Text;
            var port = int.Parse(Port.Text);
            var c = await NsqConnection.ConnectAsync(string.Format("nsqd={0}:{1}", host, port));
            await c.IdentifyAsync();
        }
    }
}
