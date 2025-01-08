using System.IO;
using System;

namespace vlc_works
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.vlcControl = new Vlc.DotNet.Forms.VlcControl();
			this.inputLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.vlcControl)).BeginInit();
			this.SuspendLayout();
			// 
			// vlcControl
			// 
			this.vlcControl.BackColor = System.Drawing.Color.Black;
			this.vlcControl.Location = new System.Drawing.Point(0, 0);
			this.vlcControl.Name = "vlcControl";
			this.vlcControl.Size = new System.Drawing.Size(658, 374);
			this.vlcControl.Spu = -1;
			this.vlcControl.TabIndex = 0;
			this.vlcControl.Text = "vlcControl1";
			this.vlcControl.VlcLibDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libvlc\\win-x86"));
			//this.vlcControl.VlcLibDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libvlc\\win-x86"));
			this.vlcControl.VlcMediaplayerOptions = null;
			// 
			// inputLabel
			// 
			this.inputLabel.AutoSize = true;
			this.inputLabel.BackColor = System.Drawing.SystemColors.Desktop;
			this.inputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 159.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.inputLabel.ForeColor = System.Drawing.Color.Red;
			this.inputLabel.Location = new System.Drawing.Point(322, 267);
			this.inputLabel.Name = "inputLabel";
			this.inputLabel.Size = new System.Drawing.Size(552, 241);
			this.inputLabel.TabIndex = 1;
			this.inputLabel.Text = "label";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(765, 437);
			this.Controls.Add(this.inputLabel);
			this.Controls.Add(this.vlcControl);
			this.Name = "Form1";
			this.Text = "Form1";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
			((System.ComponentModel.ISupportInitialize)(this.vlcControl)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public Vlc.DotNet.Forms.VlcControl vlcControl;
		public System.Windows.Forms.Label inputLabel;
	}
}

