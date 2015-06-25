namespace TestClient
{
    partial class LookupForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Host = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Port = new System.Windows.Forms.TextBox();
            this.LookupButton = new System.Windows.Forms.Button();
            this.ResultsTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LookupTopic = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TopicsButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ChannelsTopic = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ChannelsButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.DeleteChannelChannel = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.DeleteChannelTopic = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DeleteChannelButton = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.DeleteTopicTopic = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.DeleteTopicButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // Host
            // 
            this.Host.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Host.Location = new System.Drawing.Point(138, 12);
            this.Host.Name = "Host";
            this.Host.Size = new System.Drawing.Size(278, 26);
            this.Host.TabIndex = 0;
            this.Host.Text = "localhost";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Host";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // Port
            // 
            this.Port.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Port.Location = new System.Drawing.Point(138, 44);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(97, 26);
            this.Port.TabIndex = 4;
            this.Port.Text = "4161";
            // 
            // LookupButton
            // 
            this.LookupButton.Location = new System.Drawing.Point(406, 25);
            this.LookupButton.Name = "LookupButton";
            this.LookupButton.Size = new System.Drawing.Size(160, 46);
            this.LookupButton.TabIndex = 5;
            this.LookupButton.Text = "Go";
            this.LookupButton.UseVisualStyleBackColor = true;
            this.LookupButton.Click += new System.EventHandler(this.LookupButton_Click);
            // 
            // ResultsTextBox
            // 
            this.ResultsTextBox.Location = new System.Drawing.Point(26, 614);
            this.ResultsTextBox.Multiline = true;
            this.ResultsTextBox.Name = "ResultsTextBox";
            this.ResultsTextBox.Size = new System.Drawing.Size(618, 320);
            this.ResultsTextBox.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LookupTopic);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.LookupButton);
            this.groupBox1.Location = new System.Drawing.Point(16, 76);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(572, 94);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "/lookup";
            // 
            // LookupTopic
            // 
            this.LookupTopic.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LookupTopic.Location = new System.Drawing.Point(122, 25);
            this.LookupTopic.Name = "LookupTopic";
            this.LookupTopic.Size = new System.Drawing.Size(278, 26);
            this.LookupTopic.TabIndex = 8;
            this.LookupTopic.Text = "foo";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Topic";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TopicsButton);
            this.groupBox2.Location = new System.Drawing.Point(16, 176);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(572, 94);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "/topics";
            // 
            // TopicsButton
            // 
            this.TopicsButton.Location = new System.Drawing.Point(406, 25);
            this.TopicsButton.Name = "TopicsButton";
            this.TopicsButton.Size = new System.Drawing.Size(160, 46);
            this.TopicsButton.TabIndex = 5;
            this.TopicsButton.Text = "Go";
            this.TopicsButton.UseVisualStyleBackColor = true;
            this.TopicsButton.Click += new System.EventHandler(this.TopicsButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ChannelsTopic);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.ChannelsButton);
            this.groupBox3.Location = new System.Drawing.Point(16, 276);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(572, 94);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "/channels";
            // 
            // ChannelsTopic
            // 
            this.ChannelsTopic.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChannelsTopic.Location = new System.Drawing.Point(122, 25);
            this.ChannelsTopic.Name = "ChannelsTopic";
            this.ChannelsTopic.Size = new System.Drawing.Size(278, 26);
            this.ChannelsTopic.TabIndex = 8;
            this.ChannelsTopic.Text = "foo";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "Topic";
            // 
            // ChannelsButton
            // 
            this.ChannelsButton.Location = new System.Drawing.Point(406, 25);
            this.ChannelsButton.Name = "ChannelsButton";
            this.ChannelsButton.Size = new System.Drawing.Size(160, 46);
            this.ChannelsButton.TabIndex = 5;
            this.ChannelsButton.Text = "Go";
            this.ChannelsButton.UseVisualStyleBackColor = true;
            this.ChannelsButton.Click += new System.EventHandler(this.ChannelsButton_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.DeleteChannelChannel);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.DeleteChannelTopic);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.DeleteChannelButton);
            this.groupBox4.Location = new System.Drawing.Point(16, 476);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(572, 94);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "/delete_channel";
            // 
            // DeleteChannelChannel
            // 
            this.DeleteChannelChannel.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteChannelChannel.Location = new System.Drawing.Point(122, 57);
            this.DeleteChannelChannel.Name = "DeleteChannelChannel";
            this.DeleteChannelChannel.Size = new System.Drawing.Size(278, 26);
            this.DeleteChannelChannel.TabIndex = 9;
            this.DeleteChannelChannel.Text = "bar";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "Channel";
            // 
            // DeleteChannelTopic
            // 
            this.DeleteChannelTopic.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteChannelTopic.Location = new System.Drawing.Point(122, 25);
            this.DeleteChannelTopic.Name = "DeleteChannelTopic";
            this.DeleteChannelTopic.Size = new System.Drawing.Size(278, 26);
            this.DeleteChannelTopic.TabIndex = 8;
            this.DeleteChannelTopic.Text = "foo";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Topic";
            // 
            // DeleteChannelButton
            // 
            this.DeleteChannelButton.Location = new System.Drawing.Point(406, 25);
            this.DeleteChannelButton.Name = "DeleteChannelButton";
            this.DeleteChannelButton.Size = new System.Drawing.Size(160, 46);
            this.DeleteChannelButton.TabIndex = 5;
            this.DeleteChannelButton.Text = "Go";
            this.DeleteChannelButton.UseVisualStyleBackColor = true;
            this.DeleteChannelButton.Click += new System.EventHandler(this.DeleteChannelButton_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.DeleteTopicTopic);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.DeleteTopicButton);
            this.groupBox5.Location = new System.Drawing.Point(16, 376);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(572, 94);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "/delete_topic";
            // 
            // DeleteTopicTopic
            // 
            this.DeleteTopicTopic.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteTopicTopic.Location = new System.Drawing.Point(122, 25);
            this.DeleteTopicTopic.Name = "DeleteTopicTopic";
            this.DeleteTopicTopic.Size = new System.Drawing.Size(278, 26);
            this.DeleteTopicTopic.TabIndex = 8;
            this.DeleteTopicTopic.Text = "foo";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "Topic";
            // 
            // DeleteTopicButton
            // 
            this.DeleteTopicButton.Location = new System.Drawing.Point(406, 25);
            this.DeleteTopicButton.Name = "DeleteTopicButton";
            this.DeleteTopicButton.Size = new System.Drawing.Size(160, 46);
            this.DeleteTopicButton.TabIndex = 5;
            this.DeleteTopicButton.Text = "Go";
            this.DeleteTopicButton.UseVisualStyleBackColor = true;
            this.DeleteTopicButton.Click += new System.EventHandler(this.DeleteTopicButton_Click);
            // 
            // LookupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 989);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ResultsTextBox);
            this.Controls.Add(this.Port);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Host);
            this.Name = "LookupForm";
            this.Text = "LookupForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Host;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Port;
        private System.Windows.Forms.Button LookupButton;
        private System.Windows.Forms.TextBox ResultsTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox LookupTopic;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button TopicsButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox ChannelsTopic;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ChannelsButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox DeleteChannelTopic;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button DeleteChannelButton;
        private System.Windows.Forms.TextBox DeleteChannelChannel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox DeleteTopicTopic;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button DeleteTopicButton;
    }
}