namespace TestClient
{
    partial class ConsumerForm
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
            this.Port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Host = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.ReceivedMessages = new System.Windows.Forms.ListBox();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TopicTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ChannelTextBox = new System.Windows.Forms.TextBox();
            this.ReadyButton = new System.Windows.Forms.Button();
            this.ReadyTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Port
            // 
            this.Port.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Port.Location = new System.Drawing.Point(127, 44);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(72, 26);
            this.Port.TabIndex = 8;
            this.Port.Text = "4150";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Host";
            // 
            // Host
            // 
            this.Host.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Host.Location = new System.Drawing.Point(127, 12);
            this.Host.Name = "Host";
            this.Host.Size = new System.Drawing.Size(221, 26);
            this.Host.TabIndex = 5;
            this.Host.Text = "localhost";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(354, 12);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(140, 58);
            this.ConnectButton.TabIndex = 9;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // ReceivedMessages
            // 
            this.ReceivedMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReceivedMessages.FormattingEnabled = true;
            this.ReceivedMessages.ItemHeight = 20;
            this.ReceivedMessages.Location = new System.Drawing.Point(12, 196);
            this.ReceivedMessages.Name = "ReceivedMessages";
            this.ReceivedMessages.Size = new System.Drawing.Size(622, 424);
            this.ReceivedMessages.TabIndex = 10;
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(500, 12);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(134, 58);
            this.DisconnectButton.TabIndex = 11;
            this.DisconnectButton.Text = "Close";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 20);
            this.label3.TabIndex = 13;
            this.label3.Text = "Topic";
            // 
            // TopicTextBox
            // 
            this.TopicTextBox.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TopicTextBox.Location = new System.Drawing.Point(127, 76);
            this.TopicTextBox.Name = "TopicTextBox";
            this.TopicTextBox.Size = new System.Drawing.Size(221, 26);
            this.TopicTextBox.TabIndex = 12;
            this.TopicTextBox.Text = "signups";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 20);
            this.label4.TabIndex = 15;
            this.label4.Text = "Channel";
            // 
            // ChannelTextBox
            // 
            this.ChannelTextBox.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChannelTextBox.Location = new System.Drawing.Point(127, 108);
            this.ChannelTextBox.Name = "ChannelTextBox";
            this.ChannelTextBox.Size = new System.Drawing.Size(221, 26);
            this.ChannelTextBox.TabIndex = 14;
            this.ChannelTextBox.Text = "email";
            // 
            // ReadyButton
            // 
            this.ReadyButton.Location = new System.Drawing.Point(354, 108);
            this.ReadyButton.Name = "ReadyButton";
            this.ReadyButton.Size = new System.Drawing.Size(134, 58);
            this.ReadyButton.TabIndex = 17;
            this.ReadyButton.Text = "Ready";
            this.ReadyButton.UseVisualStyleBackColor = true;
            this.ReadyButton.Click += new System.EventHandler(this.ReadyButton_Click);
            // 
            // ReadyTextBox
            // 
            this.ReadyTextBox.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReadyTextBox.Location = new System.Drawing.Point(127, 140);
            this.ReadyTextBox.Name = "ReadyTextBox";
            this.ReadyTextBox.Size = new System.Drawing.Size(221, 26);
            this.ReadyTextBox.TabIndex = 18;
            this.ReadyTextBox.Text = "3";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 20);
            this.label5.TabIndex = 19;
            this.label5.Text = "Ready";
            // 
            // ConsumerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 653);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ReadyTextBox);
            this.Controls.Add(this.ReadyButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ChannelTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TopicTextBox);
            this.Controls.Add(this.DisconnectButton);
            this.Controls.Add(this.ReceivedMessages);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.Port);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Host);
            this.Name = "ConsumerForm";
            this.Text = "ConsumerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsumerForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Host;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.ListBox ReceivedMessages;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TopicTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ChannelTextBox;
        private System.Windows.Forms.Button ReadyButton;
        private System.Windows.Forms.TextBox ReadyTextBox;
        private System.Windows.Forms.Label label5;
    }
}