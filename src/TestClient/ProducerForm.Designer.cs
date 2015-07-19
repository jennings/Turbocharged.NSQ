namespace TestClient
{
    partial class ProducerForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.TopicTextBox = new System.Windows.Forms.TextBox();
            this.Port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Host = new System.Windows.Forms.TextBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.CountControl = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.SendMultipleButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.CountControl)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 20);
            this.label3.TabIndex = 21;
            this.label3.Text = "Topic";
            // 
            // TopicTextBox
            // 
            this.TopicTextBox.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TopicTextBox.Location = new System.Drawing.Point(125, 76);
            this.TopicTextBox.Name = "TopicTextBox";
            this.TopicTextBox.Size = new System.Drawing.Size(253, 26);
            this.TopicTextBox.TabIndex = 20;
            this.TopicTextBox.Text = "signups";
            // 
            // Port
            // 
            this.Port.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Port.Location = new System.Drawing.Point(125, 44);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(72, 26);
            this.Port.TabIndex = 19;
            this.Port.Text = "4151";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 18;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "Host";
            // 
            // Host
            // 
            this.Host.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Host.Location = new System.Drawing.Point(125, 12);
            this.Host.Name = "Host";
            this.Host.Size = new System.Drawing.Size(253, 26);
            this.Host.TabIndex = 16;
            this.Host.Text = "localhost";
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(125, 140);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(182, 58);
            this.SendButton.TabIndex = 24;
            this.SendButton.Text = "Send Individually";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendSingleButton_Click);
            // 
            // CountControl
            // 
            this.CountControl.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.CountControl.Location = new System.Drawing.Point(125, 108);
            this.CountControl.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.CountControl.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CountControl.Name = "CountControl";
            this.CountControl.Size = new System.Drawing.Size(120, 26);
            this.CountControl.TabIndex = 27;
            this.CountControl.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 20);
            this.label4.TabIndex = 28;
            this.label4.Text = "Count";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 254);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 20);
            this.label5.TabIndex = 29;
            this.label5.Text = "Status";
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(121, 254);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(35, 20);
            this.StatusLabel.TabIndex = 30;
            this.StatusLabel.Text = "Idle";
            // 
            // SendMultipleButton
            // 
            this.SendMultipleButton.Location = new System.Drawing.Point(313, 140);
            this.SendMultipleButton.Name = "SendMultipleButton";
            this.SendMultipleButton.Size = new System.Drawing.Size(182, 58);
            this.SendMultipleButton.TabIndex = 31;
            this.SendMultipleButton.Text = "Send Multiple";
            this.SendMultipleButton.UseVisualStyleBackColor = true;
            this.SendMultipleButton.Click += new System.EventHandler(this.SendMultipleButton_Click);
            // 
            // ProducerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 379);
            this.Controls.Add(this.SendMultipleButton);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CountControl);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TopicTextBox);
            this.Controls.Add(this.Port);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Host);
            this.Name = "ProducerForm";
            this.Text = "ProducerForm";
            ((System.ComponentModel.ISupportInitialize)(this.CountControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TopicTextBox;
        private System.Windows.Forms.TextBox Port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Host;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.NumericUpDown CountControl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button SendMultipleButton;
    }
}