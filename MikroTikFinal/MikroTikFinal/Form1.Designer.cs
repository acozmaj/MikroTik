namespace MikroTikFinal
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.textIpAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textUsername = new System.Windows.Forms.TextBox();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.labelPingStatus = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.labelMailAddress = new System.Windows.Forms.Label();
            this.textMailAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textUserData = new System.Windows.Forms.TextBox();
            this.buttonThread = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textPartnerPassword = new System.Windows.Forms.TextBox();
            this.labelPartnerskaSifra = new System.Windows.Forms.Label();
            this.labelIdManager = new System.Windows.Forms.Label();
            this.textIdManager = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unesite IP adresu:";
            // 
            // textIpAddress
            // 
            this.textIpAddress.Location = new System.Drawing.Point(38, 54);
            this.textIpAddress.Name = "textIpAddress";
            this.textIpAddress.Size = new System.Drawing.Size(100, 20);
            this.textIpAddress.TabIndex = 1;
            this.textIpAddress.Text = "192.168.88.1";
            this.textIpAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textIpAddress.TextChanged += new System.EventHandler(this.textIpAddress_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 165);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Unesite korisnicki nalog:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(194, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Unesite korisnicku lozinku:";
            // 
            // textUsername
            // 
            this.textUsername.Location = new System.Drawing.Point(38, 181);
            this.textUsername.Name = "textUsername";
            this.textUsername.Size = new System.Drawing.Size(113, 20);
            this.textUsername.TabIndex = 3;
            this.textUsername.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(197, 181);
            this.textPassword.Name = "textPassword";
            this.textPassword.Size = new System.Drawing.Size(130, 20);
            this.textPassword.TabIndex = 4;
            this.textPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelPingStatus
            // 
            this.labelPingStatus.AutoSize = true;
            this.labelPingStatus.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPingStatus.ForeColor = System.Drawing.Color.Red;
            this.labelPingStatus.Location = new System.Drawing.Point(165, 55);
            this.labelPingStatus.Name = "labelPingStatus";
            this.labelPingStatus.Size = new System.Drawing.Size(67, 19);
            this.labelPingStatus.TabIndex = 6;
            this.labelPingStatus.Text = "no ping";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(620, 411);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 6;
            this.buttonStart.Text = "Izvrsi";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Visible = false;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(64, 324);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(240, 79);
            this.textBox1.TabIndex = 8;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(61, 304);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Proces izvrsavanja:";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(288, 51);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 15;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // labelMailAddress
            // 
            this.labelMailAddress.AutoSize = true;
            this.labelMailAddress.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMailAddress.Location = new System.Drawing.Point(149, 61);
            this.labelMailAddress.Name = "labelMailAddress";
            this.labelMailAddress.Size = new System.Drawing.Size(86, 19);
            this.labelMailAddress.TabIndex = 11;
            this.labelMailAddress.Text = "@zona.ba";
            // 
            // textMailAddress
            // 
            this.textMailAddress.Location = new System.Drawing.Point(21, 62);
            this.textMailAddress.Name = "textMailAddress";
            this.textMailAddress.Size = new System.Drawing.Size(122, 20);
            this.textMailAddress.TabIndex = 5;
            this.textMailAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Unesite partnerski e-mail:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(166, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(160, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Unesite korisnicko prezime i ime:";
            // 
            // textUserData
            // 
            this.textUserData.Location = new System.Drawing.Point(169, 117);
            this.textUserData.Name = "textUserData";
            this.textUserData.Size = new System.Drawing.Size(157, 20);
            this.textUserData.TabIndex = 2;
            this.textUserData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonThread
            // 
            this.buttonThread.Location = new System.Drawing.Point(113, 239);
            this.buttonThread.Name = "buttonThread";
            this.buttonThread.Size = new System.Drawing.Size(119, 23);
            this.buttonThread.TabIndex = 7;
            this.buttonThread.Text = "Izvrsi";
            this.buttonThread.UseVisualStyleBackColor = true;
            this.buttonThread.Click += new System.EventHandler(this.buttonThread_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textPartnerPassword);
            this.groupBox1.Controls.Add(this.labelPartnerskaSifra);
            this.groupBox1.Controls.Add(this.labelMailAddress);
            this.groupBox1.Controls.Add(this.textMailAddress);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(420, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(263, 186);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Partnerski podaci:";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // textPartnerPassword
            // 
            this.textPartnerPassword.Location = new System.Drawing.Point(21, 109);
            this.textPartnerPassword.Name = "textPartnerPassword";
            this.textPartnerPassword.Size = new System.Drawing.Size(122, 20);
            this.textPartnerPassword.TabIndex = 6;
            this.textPartnerPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textPartnerPassword.UseSystemPasswordChar = true;
            // 
            // labelPartnerskaSifra
            // 
            this.labelPartnerskaSifra.AutoSize = true;
            this.labelPartnerskaSifra.Location = new System.Drawing.Point(18, 92);
            this.labelPartnerskaSifra.Name = "labelPartnerskaSifra";
            this.labelPartnerskaSifra.Size = new System.Drawing.Size(121, 13);
            this.labelPartnerskaSifra.TabIndex = 14;
            this.labelPartnerskaSifra.Text = "Unesite partnersku sifru:";
            // 
            // labelIdManager
            // 
            this.labelIdManager.AutoSize = true;
            this.labelIdManager.Location = new System.Drawing.Point(35, 101);
            this.labelIdManager.Name = "labelIdManager";
            this.labelIdManager.Size = new System.Drawing.Size(63, 13);
            this.labelIdManager.TabIndex = 17;
            this.labelIdManager.Text = "Id manager:";
            this.labelIdManager.Click += new System.EventHandler(this.label7_Click);
            // 
            // textIdManager
            // 
            this.textIdManager.Location = new System.Drawing.Point(35, 116);
            this.textIdManager.Name = "textIdManager";
            this.textIdManager.Size = new System.Drawing.Size(63, 20);
            this.textIdManager.TabIndex = 18;
            this.textIdManager.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textIdManager.TextChanged += new System.EventHandler(this.textIdManager_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 446);
            this.Controls.Add(this.textIdManager);
            this.Controls.Add(this.labelIdManager);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonThread);
            this.Controls.Add(this.textUserData);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.labelPingStatus);
            this.Controls.Add(this.textPassword);
            this.Controls.Add(this.textUsername);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textIpAddress);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Partner";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textIpAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textUsername;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.Label labelPingStatus;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Label labelMailAddress;
        private System.Windows.Forms.TextBox textMailAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textUserData;
        private System.Windows.Forms.Button buttonThread;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textPartnerPassword;
        private System.Windows.Forms.Label labelPartnerskaSifra;
        private System.Windows.Forms.Label labelIdManager;
        private System.Windows.Forms.TextBox textIdManager;
    }
}

