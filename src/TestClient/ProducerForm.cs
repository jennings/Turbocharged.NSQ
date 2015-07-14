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

            var count = (int)CountControl.Value;
            var tasks = new Task[count];
            var messages = GenerateMessages(count).ToList();
            for (int i = 0; i < count; i++)
            {
                tasks[i] = Publishmessage(producer, topic, messages[i]);
            }
            await Task.WhenAll(tasks);
            StatusLabel.Text = "Done! (Idle)";
        }

        IEnumerable<byte[]> GenerateMessages(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var message = BitConverter.GetBytes(i);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(message);
                yield return message;
            }
        }

        async void SendMultipleButton_Click(object sender, EventArgs e)
        {
            var host = Host.Text;
            var port = int.Parse(Port.Text);
            var topic = TopicTextBox.Text;
            var producer = new NsqProducer(host, port);

            var count = (int)CountControl.Value;

            StatusLabel.Text = "Generating messages";
            var messages = GenerateMessages(count).ToArray();

            StatusLabel.Text = "Publishing";
            await producer.PublishAsync(topic, messages);

            StatusLabel.Text = "Done! (Idle)";
        }

        async Task Publishmessage(NsqProducer producer, Topic topic, byte[] message)
        {
            await producer.PublishAsync(topic, message);
            StatusLabel.Text = "Published message: " + BitConverter.ToString(message);
        }
    }
}
