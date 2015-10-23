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
    public partial class LookupConsumerForm : Form
    {
        BindingList<string> _messages = new BindingList<string>();
        BindingList<string> _endPoints = new BindingList<string>();
        NsqLookupConsumer _nsq;

        public LookupConsumerForm(string connectionString)
        {
            InitializeComponent();
            ConnectionString.Text = connectionString;
            ReceivedMessages.DataSource = _messages;
            DiscoveredEndPoints.DataSource = _endPoints;
        }

        void _nsq_DiscoveryCompleted(object s, DiscoveryEventArgs e)
        {
            if (IsDisposed) return;

            InvokeIfRequired(() =>
            {
                _endPoints.Clear();
                foreach (var address in e.NsqAddresses)
                {
                    _endPoints.Add(address.BroadcastAddress + ":" + address.TcpPort);
                }

                LastDiscoveryTime.Text = DateTime.Now.ToString("T");
            });
        }

        void _nsq_InternalMessages(object s, InternalMessageEventArgs e)
        {
            if (IsDisposed) return;
            PostMessage("INTERNAL: " + e.Message);
        }

        void InvokeIfRequired(Action action)
        {
            if (InvokeRequired)
            {
                if (IsDisposed) return;
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        void ConnectButton_Click(object sender, EventArgs e)
        {
            var connectionString = ConnectionString.Text;
            var options = ConsumerOptions.Parse(connectionString);

            _nsq = new NsqLookupConsumer(options);
            _nsq.DiscoveryCompleted += _nsq_DiscoveryCompleted;
            _nsq.InternalMessages += _nsq_InternalMessages;
            _nsq.Connect(async msg =>
            {
                await c_MessageReceived(msg);
                await msg.FinishAsync();
            });
        }

        Task c_MessageReceived(Turbocharged.NSQ.Message obj)
        {
            var data = BitConverter.ToString(obj.Body, 0);
            var message = string.Format("RECEIVED MESSAGE. Id = {0}, Msg = {1}", obj.Id, data);
            PostMessage(message);
            return Task.FromResult(0);
        }

        void ConsumerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_nsq != null)
                _nsq.Dispose();
        }

        void PostMessage(string obj)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            InvokeIfRequired(() =>
            {
                _messages.Add(obj + " (ThreadId = " + threadId + ")");
                ReceivedMessages.SelectedIndex = ReceivedMessages.Items.Count - 1;
            });
        }

        void DisconnectButton_Click(object sender, EventArgs e)
        {
            _nsq.Dispose();
        }

        async void ReadyButton_Click(object sender, EventArgs e)
        {
            var maxInFlight = int.Parse(ReadyTextBox.Text);
            await _nsq.SetMaxInFlightAsync(maxInFlight);
        }
    }
}
