namespace SMBBrowser
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            buttonConnect = new Button();
            textBoxIP = new TextBox();
            textBoxPort = new TextBox();
            textBoxUsername = new TextBox();
            textBoxPassword = new TextBox();
            treeView1 = new TreeView();
            richTextBoxLog = new RichTextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(98, 34);
            label1.Name = "label1";
            label1.Size = new Size(20, 15);
            label1.TabIndex = 0;
            label1.Text = "IP:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(86, 63);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 1;
            label2.Text = "Port:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(20, 92);
            label3.Name = "label3";
            label3.Size = new Size(98, 15);
            label3.TabIndex = 2;
            label3.Text = "Server Username:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(23, 121);
            label4.Name = "label4";
            label4.Size = new Size(95, 15);
            label4.TabIndex = 3;
            label4.Text = "Server Password:";
            // 
            // buttonConnect
            // 
            buttonConnect.Location = new Point(135, 163);
            buttonConnect.Name = "buttonConnect";
            buttonConnect.Size = new Size(75, 23);
            buttonConnect.TabIndex = 4;
            buttonConnect.Text = "Connect";
            buttonConnect.UseVisualStyleBackColor = true;
            buttonConnect.Click += buttonConnect_Click;
            // 
            // textBoxIP
            // 
            textBoxIP.Location = new Point(124, 31);
            textBoxIP.Name = "textBoxIP";
            textBoxIP.Size = new Size(100, 23);
            textBoxIP.TabIndex = 5;
            // 
            // textBoxPort
            // 
            textBoxPort.Location = new Point(124, 60);
            textBoxPort.Name = "textBoxPort";
            textBoxPort.Size = new Size(100, 23);
            textBoxPort.TabIndex = 6;
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(124, 89);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(100, 23);
            textBoxUsername.TabIndex = 7;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(124, 118);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(100, 23);
            textBoxPassword.TabIndex = 8;
            textBoxPassword.UseSystemPasswordChar = true;
            // 
            // treeView1
            // 
            treeView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeView1.Location = new Point(243, 23);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(308, 337);
            treeView1.TabIndex = 9;
            treeView1.BeforeExpand += treeView1_BeforeExpand;
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            richTextBoxLog.Location = new Point(20, 202);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.Size = new Size(204, 158);
            richTextBoxLog.TabIndex = 10;
            richTextBoxLog.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(575, 379);
            Controls.Add(richTextBoxLog);
            Controls.Add(treeView1);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxUsername);
            Controls.Add(textBoxPort);
            Controls.Add(textBoxIP);
            Controls.Add(buttonConnect);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "SMB Browser";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button buttonConnect;
        private TextBox textBoxIP;
        private TextBox textBoxPort;
        private TextBox textBoxUsername;
        private TextBox textBoxPassword;
        private TreeView treeView1;
        private RichTextBox richTextBoxLog;
    }
}
