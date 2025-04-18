﻿namespace vlc_works
{
    partial class DebugForm
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
            this.winBut = new System.Windows.Forms.Button();
            this.loseBut = new System.Windows.Forms.Button();
            this.wc = new System.Windows.Forms.Label();
            this.lc = new System.Windows.Forms.Label();
            this.gis = new System.Windows.Forms.Label();
            this.gm = new System.Windows.Forms.Label();
            this.w = new System.Windows.Forms.Label();
            this.gi = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.udpScriptIndexBox = new System.Windows.Forms.TextBox();
            this.sendUdpBut = new System.Windows.Forms.Button();
            this.vlcSkip = new System.Windows.Forms.Button();
            this.cs = new System.Windows.Forms.Label();
            this.deleteVideoFilesBut = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // winBut
            // 
            this.winBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.winBut.Location = new System.Drawing.Point(0, 0);
            this.winBut.Name = "winBut";
            this.winBut.Size = new System.Drawing.Size(128, 32);
            this.winBut.TabIndex = 0;
            this.winBut.Text = "WIN";
            this.winBut.UseVisualStyleBackColor = true;
            this.winBut.Click += new System.EventHandler(this.winBut_Click);
            // 
            // loseBut
            // 
            this.loseBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loseBut.Location = new System.Drawing.Point(0, 32);
            this.loseBut.Name = "loseBut";
            this.loseBut.Size = new System.Drawing.Size(128, 32);
            this.loseBut.TabIndex = 1;
            this.loseBut.Text = "LOSE";
            this.loseBut.UseVisualStyleBackColor = true;
            this.loseBut.Click += new System.EventHandler(this.loseBut_Click);
            // 
            // wc
            // 
            this.wc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.wc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.wc.Location = new System.Drawing.Point(160, 0);
            this.wc.Name = "wc";
            this.wc.Size = new System.Drawing.Size(256, 32);
            this.wc.TabIndex = 2;
            this.wc.Text = "won counter";
            // 
            // lc
            // 
            this.lc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lc.Location = new System.Drawing.Point(160, 32);
            this.lc.Name = "lc";
            this.lc.Size = new System.Drawing.Size(256, 32);
            this.lc.TabIndex = 3;
            this.lc.Text = "lose counter";
            // 
            // gis
            // 
            this.gis.AutoSize = true;
            this.gis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gis.Location = new System.Drawing.Point(160, 192);
            this.gis.Name = "gis";
            this.gis.Size = new System.Drawing.Size(148, 27);
            this.gis.TabIndex = 4;
            this.gis.Text = "game indexes";
            // 
            // gm
            // 
            this.gm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gm.Location = new System.Drawing.Point(224, 64);
            this.gm.Name = "gm";
            this.gm.Size = new System.Drawing.Size(448, 32);
            this.gm.TabIndex = 5;
            this.gm.Text = "game mode";
            // 
            // w
            // 
            this.w.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.w.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.w.Location = new System.Drawing.Point(160, 128);
            this.w.Name = "w";
            this.w.Size = new System.Drawing.Size(256, 32);
            this.w.TabIndex = 7;
            this.w.Text = "won";
            // 
            // gi
            // 
            this.gi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gi.Location = new System.Drawing.Point(224, 96);
            this.gi.Name = "gi";
            this.gi.Size = new System.Drawing.Size(448, 32);
            this.gi.TabIndex = 8;
            this.gi.Text = "game index";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.deleteVideoFilesBut);
            this.panel1.Controls.Add(this.udpScriptIndexBox);
            this.panel1.Controls.Add(this.sendUdpBut);
            this.panel1.Controls.Add(this.vlcSkip);
            this.panel1.Controls.Add(this.cs);
            this.panel1.Controls.Add(this.loseBut);
            this.panel1.Controls.Add(this.w);
            this.panel1.Controls.Add(this.gi);
            this.panel1.Controls.Add(this.winBut);
            this.panel1.Controls.Add(this.wc);
            this.panel1.Controls.Add(this.lc);
            this.panel1.Controls.Add(this.gm);
            this.panel1.Controls.Add(this.gis);
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 448);
            this.panel1.TabIndex = 9;
            // 
            // udpScriptIndexBox
            // 
            this.udpScriptIndexBox.Location = new System.Drawing.Point(128, 160);
            this.udpScriptIndexBox.Name = "udpScriptIndexBox";
            this.udpScriptIndexBox.Size = new System.Drawing.Size(32, 31);
            this.udpScriptIndexBox.TabIndex = 12;
            this.udpScriptIndexBox.Text = "3";
            // 
            // sendUdpBut
            // 
            this.sendUdpBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendUdpBut.Location = new System.Drawing.Point(0, 160);
            this.sendUdpBut.Name = "sendUdpBut";
            this.sendUdpBut.Size = new System.Drawing.Size(128, 32);
            this.sendUdpBut.TabIndex = 11;
            this.sendUdpBut.Text = "send udp";
            this.sendUdpBut.UseVisualStyleBackColor = true;
            this.sendUdpBut.Click += new System.EventHandler(this.sendUdpBut_Click);
            // 
            // vlcSkip
            // 
            this.vlcSkip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.vlcSkip.Location = new System.Drawing.Point(0, 64);
            this.vlcSkip.Name = "vlcSkip";
            this.vlcSkip.Size = new System.Drawing.Size(160, 96);
            this.vlcSkip.TabIndex = 10;
            this.vlcSkip.Text = "skip vlc control time to 3:55";
            this.vlcSkip.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.vlcSkip.UseVisualStyleBackColor = true;
            this.vlcSkip.Click += new System.EventHandler(this.vlcSkip_Click);
            // 
            // cs
            // 
            this.cs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cs.Location = new System.Drawing.Point(160, 160);
            this.cs.Name = "cs";
            this.cs.Size = new System.Drawing.Size(512, 32);
            this.cs.TabIndex = 9;
            this.cs.Text = "current script";
            // 
            // deleteVideoFilesBut
            // 
            this.deleteVideoFilesBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteVideoFilesBut.Location = new System.Drawing.Point(0, 192);
            this.deleteVideoFilesBut.Name = "deleteVideoFilesBut";
            this.deleteVideoFilesBut.Size = new System.Drawing.Size(160, 32);
            this.deleteVideoFilesBut.TabIndex = 13;
            this.deleteVideoFilesBut.Text = "delete video";
            this.deleteVideoFilesBut.UseVisualStyleBackColor = true;
            this.deleteVideoFilesBut.Click += new System.EventHandler(this.deleteVideoFilesBut_Click);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "DebugForm";
            this.Text = "DebugForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button winBut;
        public System.Windows.Forms.Button loseBut;
        public System.Windows.Forms.Label wc;
        public System.Windows.Forms.Label lc;
        public System.Windows.Forms.Label gis;
        public System.Windows.Forms.Label gm;
        public System.Windows.Forms.Label w;
        public System.Windows.Forms.Label gi;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label cs;
        private System.Windows.Forms.Button vlcSkip;
        private System.Windows.Forms.TextBox udpScriptIndexBox;
        private System.Windows.Forms.Button sendUdpBut;
        private System.Windows.Forms.Button deleteVideoFilesBut;
    }
}