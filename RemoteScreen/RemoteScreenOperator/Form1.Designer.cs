namespace RemoteScreenOperator
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox_ip_address = new System.Windows.Forms.TextBox();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.labelIpAddress = new System.Windows.Forms.Label();
            this.labelPort = new System.Windows.Forms.Label();
            this.buttonGetClientList = new System.Windows.Forms.Button();
            this.listViewClients = new System.Windows.Forms.ListView();
            this.columnDeviceName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnIpAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPort = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Green;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Location = new System.Drawing.Point(1139, 810);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(174, 69);
            this.button1.TabIndex = 0;
            this.button1.Text = "connect";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Black;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.richTextBox1.Location = new System.Drawing.Point(809, 10);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1298, 790);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Green;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(2, 814);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(202, 56);
            this.button2.TabIndex = 8;
            this.button2.Text = "get client screen";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.ButtonGetClients_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.checkBox1.Location = new System.Drawing.Point(1966, 827);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(114, 29);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "use log";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textBox_ip_address
            // 
            this.textBox_ip_address.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_ip_address.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.textBox_ip_address.Location = new System.Drawing.Point(937, 814);
            this.textBox_ip_address.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_ip_address.Name = "textBox_ip_address";
            this.textBox_ip_address.Size = new System.Drawing.Size(173, 24);
            this.textBox_ip_address.TabIndex = 10;
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(937, 844);
            this.textBox_port.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(111, 31);
            this.textBox_port.TabIndex = 11;
            // 
            // labelIpAddress
            // 
            this.labelIpAddress.AutoSize = true;
            this.labelIpAddress.ForeColor = System.Drawing.Color.White;
            this.labelIpAddress.Location = new System.Drawing.Point(805, 815);
            this.labelIpAddress.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelIpAddress.Name = "labelIpAddress";
            this.labelIpAddress.Size = new System.Drawing.Size(112, 25);
            this.labelIpAddress.TabIndex = 12;
            this.labelIpAddress.Text = "ip address";
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.ForeColor = System.Drawing.Color.White;
            this.labelPort.Location = new System.Drawing.Point(805, 844);
            this.labelPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(49, 25);
            this.labelPort.TabIndex = 13;
            this.labelPort.Text = "port";
            // 
            // buttonGetClientList
            // 
            this.buttonGetClientList.BackColor = System.Drawing.Color.Green;
            this.buttonGetClientList.FlatAppearance.BorderSize = 0;
            this.buttonGetClientList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonGetClientList.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.buttonGetClientList.Location = new System.Drawing.Point(256, 814);
            this.buttonGetClientList.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.buttonGetClientList.Name = "buttonGetClientList";
            this.buttonGetClientList.Size = new System.Drawing.Size(172, 59);
            this.buttonGetClientList.TabIndex = 14;
            this.buttonGetClientList.Text = "get client list";
            this.buttonGetClientList.UseVisualStyleBackColor = false;
            this.buttonGetClientList.Click += new System.EventHandler(this.ButtonGetClientList_Click);
            // 
            // listViewClients
            // 
            this.listViewClients.AutoArrange = false;
            this.listViewClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDeviceName,
            this.columnIpAddress,
            this.columnPort});
            this.listViewClients.FullRowSelect = true;
            this.listViewClients.HideSelection = false;
            this.listViewClients.Location = new System.Drawing.Point(2, 10);
            this.listViewClients.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.listViewClients.MultiSelect = false;
            this.listViewClients.Name = "listViewClients";
            this.listViewClients.Size = new System.Drawing.Size(800, 777);
            this.listViewClients.TabIndex = 15;
            this.listViewClients.UseCompatibleStateImageBehavior = false;
            this.listViewClients.View = System.Windows.Forms.View.Details;
            // 
            // columnDeviceName
            // 
            this.columnDeviceName.Text = "device name";
            this.columnDeviceName.Width = 200;
            // 
            // columnIpAddress
            // 
            this.columnIpAddress.Text = "ip address";
            this.columnIpAddress.Width = 100;
            // 
            // columnPort
            // 
            this.columnPort.Text = "port";
            this.columnPort.Width = 50;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(2110, 906);
            this.Controls.Add(this.listViewClients);
            this.Controls.Add(this.buttonGetClientList);
            this.Controls.Add(this.labelPort);
            this.Controls.Add(this.labelIpAddress);
            this.Controls.Add(this.textBox_port);
            this.Controls.Add(this.textBox_ip_address);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "Form1";
            this.Text = "Remote Screen Receiver";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox_ip_address;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label labelIpAddress;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Button buttonGetClientList;
        private System.Windows.Forms.ListView listViewClients;
        private System.Windows.Forms.ColumnHeader columnDeviceName;
        private System.Windows.Forms.ColumnHeader columnIpAddress;
        private System.Windows.Forms.ColumnHeader columnPort;
    }
}

