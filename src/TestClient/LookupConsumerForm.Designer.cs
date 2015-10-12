namespace TestClient
{
    partial class LookupConsumerForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.ReceivedMessages = new System.Windows.Forms.ListBox();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.ReadyButton = new System.Windows.Forms.Button();
            this.ReadyTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ConnectionString = new System.Windows.Forms.TextBox();
            this.DiscoveredEndPoints = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.LastDiscoveryTime = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "ConnectionString";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(360, 69);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(134, 49);
            this.ConnectButton.TabIndex = 9;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // ReceivedMessages
            // 
            this.ReceivedMessages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ReceivedMessages.FormattingEnabled = true;
            this.ReceivedMessages.ItemHeight = 20;
            this.ReceivedMessages.Location = new System.Drawing.Point(12, 196);
            this.ReceivedMessages.Name = "ReceivedMessages";
            this.ReceivedMessages.Size = new System.Drawing.Size(622, 424);
            this.ReceivedMessages.TabIndex = 10;
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(500, 69);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(134, 49);
            this.DisconnectButton.TabIndex = 11;
            this.DisconnectButton.Text = "Close";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // ReadyButton
            // 
            this.ReadyButton.Location = new System.Drawing.Point(500, 141);
            this.ReadyButton.Name = "ReadyButton";
            this.ReadyButton.Size = new System.Drawing.Size(134, 49);
            this.ReadyButton.TabIndex = 17;
            this.ReadyButton.Text = "Ready";
            this.ReadyButton.UseVisualStyleBackColor = true;
            this.ReadyButton.Click += new System.EventHandler(this.ReadyButton_Click);
            // 
            // ReadyTextBox
            // 
            this.ReadyTextBox.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReadyTextBox.Location = new System.Drawing.Point(360, 164);
            this.ReadyTextBox.Name = "ReadyTextBox";
            this.ReadyTextBox.Size = new System.Drawing.Size(134, 26);
            this.ReadyTextBox.TabIndex = 18;
            this.ReadyTextBox.Text = "100";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(356, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 20);
            this.label5.TabIndex = 19;
            this.label5.Text = "Ready";
            // 
            // ConnectionString
            // 
            this.ConnectionString.Location = new System.Drawing.Point(12, 37);
            this.ConnectionString.Name = "ConnectionString";
            this.ConnectionString.Size = new System.Drawing.Size(622, 26);
            this.ConnectionString.TabIndex = 20;
            // 
            // DiscoveredEndPoints
            // 
            this.DiscoveredEndPoints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DiscoveredEndPoints.FormattingEnabled = true;
            this.DiscoveredEndPoints.ItemHeight = 20;
            this.DiscoveredEndPoints.Location = new System.Drawing.Point(640, 196);
            this.DiscoveredEndPoints.Name = "DiscoveredEndPoints";
            this.DiscoveredEndPoints.Size = new System.Drawing.Size(221, 424);
            this.DiscoveredEndPoints.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(640, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 20);
            this.label2.TabIndex = 23;
            this.label2.Text = "Last Discovery";
            // 
            // LastDiscoveryTime
            // 
            this.LastDiscoveryTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LastDiscoveryTime.Location = new System.Drawing.Point(640, 164);
            this.LastDiscoveryTime.Name = "LastDiscoveryTime";
            this.LastDiscoveryTime.Size = new System.Drawing.Size(221, 26);
            this.LastDiscoveryTime.TabIndex = 24;
            // 
            // LookupConsumerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 653);
            this.Controls.Add(this.LastDiscoveryTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DiscoveredEndPoints);
            this.Controls.Add(this.ConnectionString);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ReadyTextBox);
            this.Controls.Add(this.ReadyButton);
            this.Controls.Add(this.DisconnectButton);
            this.Controls.Add(this.ReceivedMessages);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.label1);
            this.Name = "LookupConsumerForm";
            this.Text = "ConsumerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsumerForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.ListBox ReceivedMessages;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.Button ReadyButton;
        private System.Windows.Forms.TextBox ReadyTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ConnectionString;
        private System.Windows.Forms.ListBox DiscoveredEndPoints;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox LastDiscoveryTime;
    }
}