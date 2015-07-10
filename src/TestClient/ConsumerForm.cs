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
        BindingList<string> _messages = new BindingList<string>();
        NsqTcpConnection _nsq;

        public ConsumerForm(string host, int port)
        {
            InitializeComponent();
            Host.Text = host;
            Port.Text = port.ToString();
            ReceivedMessages.DataSource = _messages;

            _nsq = new NsqTcpConnection(string.Format("nsqd={0}:{1}", host, port));
            _nsq.InternalMessages += _nsq_InternalMessages;
        }

        void _nsq_InternalMessages(string obj)
        {
            PostMessage("INTERNAL: " + obj);
        }

        async void ConnectButton_Click(object sender, EventArgs e)
        {
            var topic = TopicTextBox.Text;
            var channel = ChannelTextBox.Text;
            await _nsq.ConnectAsync(topic, channel, async msg =>
            {
                await c_MessageReceived(msg);
                await msg.FinishAsync();
            });
        }

        Task c_MessageReceived(Turbocharged.NSQ.Message obj)
        {
            PostMessage("RECEIVED MESSAGE. Id = " + obj.Id + ", Attempts = " + obj.Attempts);
            return Task.FromResult(0);
        }

        void ConsumerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _nsq.Close();
        }

        void PostMessage(string obj)
        {
            Invoke((Action)(() => _messages.Add(obj)));
        }

        void DisconnectButton_Click(object sender, EventArgs e)
        {
            _nsq.Close();
        }
    }
}
