using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Turbocharged.NSQ;

namespace TestClient
{
    public partial class LookupForm : Form
    {
        public LookupForm(string hostname, int port)
        {
            InitializeComponent();
            Host.Text = hostname;
            Port.Text = port.ToString();
        }

        NsqLookup CreateConnection()
        {
            var host = Host.Text;
            var port = int.Parse(Port.Text);
            return new NsqLookup(host, port);
        }

        async Task DoUIStuff(Func<NsqLookup, Task> action)
        {
            try
            {
                ResultsTextBox.Text = "Clearing...";
                await Task.Delay(10);
                ResultsTextBox.Text = "Querying...";

                var connection = CreateConnection();
                await action(connection);
            }
            catch (Exception ex)
            {
                ResultsTextBox.Text = ex.Message;
            }
        }

        async void LookupButton_Click(object sender, EventArgs e)
        {
            await DoUIStuff(async connection =>
            {
                var topic = LookupTopic.Text;
                var result = await connection.LookupAsync(topic);
                ResultsTextBox.Text = string.Join("\r\n", result.Select(x => x.ToString()).DefaultIfEmpty("(empty)").ToArray());
            });
        }

        async void TopicsButton_Click(object sender, EventArgs e)
        {
            await DoUIStuff(async connection =>
            {
                var result = await connection.TopicsAsync();
                ResultsTextBox.Text = string.Join("\r\n", result.Select(t => (string)t).DefaultIfEmpty("(empty)").ToArray());
            });
        }

        async void ChannelsButton_Click(object sender, EventArgs e)
        {
            await DoUIStuff(async connection =>
            {
                var topic = ChannelsTopic.Text;
                var result = await connection.ChannelsAsync(topic);
                ResultsTextBox.Text = string.Join("\r\n", result.Select(c => (string)c).DefaultIfEmpty("(empty)").ToArray());
            });
        }

        async void DeleteTopicButton_Click(object sender, EventArgs e)
        {
            await DoUIStuff(async connection =>
            {
                var topic = DeleteTopicTopic.Text;
                await connection.DeleteTopicAsync(topic);
                ResultsTextBox.Text = "Done!";
            });
        }

        async void DeleteChannelButton_Click(object sender, EventArgs e)
        {
            await DoUIStuff(async connection =>
            {
                var topic = DeleteChannelTopic.Text;
                var channel = DeleteChannelChannel.Text;
                await connection.DeleteChannelAsync(topic, channel);
                ResultsTextBox.Text = "Done!";
            });
        }
    }
}
