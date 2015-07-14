using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

            var options = ConsumerOptions.Parse(string.Format("nsqd={0}:{1}", host, port));
            _nsq = new NsqTcpConnection(new DnsEndPoint(host, port), options);
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
            var data = BitConverter.ToString(obj.Data, 0);
            var message = string.Format("RECEIVED MESSAGE. Id = {0}, Msg = {1}", obj.Id, data);
            PostMessage(message);
            return Task.FromResult(0);
        }

        void ConsumerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _nsq.Close();
        }

        void PostMessage(string obj)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Invoke((Action)(() =>
            {
                _messages.Add(obj + " (ThreadId = " + threadId + ")");
                ReceivedMessages.SelectedIndex = ReceivedMessages.Items.Count - 1;
            }));
        }

        void DisconnectButton_Click(object sender, EventArgs e)
        {
            _nsq.Close();
        }

        async void ReadyButton_Click(object sender, EventArgs e)
        {
            var maxInFlight = int.Parse(ReadyTextBox.Text);
            await _nsq.SetMaxInFlight(maxInFlight);
        }
    }
}
