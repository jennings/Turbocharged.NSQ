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
    public partial class ProducerForm : Form
    {
        public ProducerForm(string host, int port)
        {
            InitializeComponent();
            Host.Text = host;
            Port.Text = port.ToString();
        }

        async void ConnectButton_Click(object sender, EventArgs e)
        {
            var host = Host.Text;
            var port = int.Parse(Port.Text);
            var topic = TopicTextBox.Text;
            var producer = new NsqProducer(host, port);
            var message = Encoding.UTF8.GetBytes(MessageTextBox.Text);
            await producer.PublishAsync(topic, message);
        }
    }
}
