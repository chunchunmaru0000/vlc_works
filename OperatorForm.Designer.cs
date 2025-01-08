namespace vlc_works
{
	partial class OperatorForm
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
			this.inputLabel = new System.Windows.Forms.Label();
			this.codeLabel = new System.Windows.Forms.Label();
			this.winLabel = new System.Windows.Forms.Label();
			this.errorLabel = new System.Windows.Forms.Label();
			this.videoLabel = new System.Windows.Forms.Label();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.textТекстToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.foreColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fontDialog = new System.Windows.Forms.FontDialog();
			this.debugLabel = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// inputLabel
			// 
			this.inputLabel.AutoSize = true;
			this.inputLabel.BackColor = System.Drawing.Color.Black;
			this.inputLabel.Font = new System.Drawing.Font("Cascadia Code", 108F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.inputLabel.ForeColor = System.Drawing.Color.LightGray;
			this.inputLabel.Location = new System.Drawing.Point(12, 370);
			this.inputLabel.Name = "inputLabel";
			this.inputLabel.Size = new System.Drawing.Size(920, 191);
			this.inputLabel.TabIndex = 0;
			this.inputLabel.Text = "INPUT HERE";
			// 
			// codeLabel
			// 
			this.codeLabel.AutoSize = true;
			this.codeLabel.BackColor = System.Drawing.Color.Black;
			this.codeLabel.Font = new System.Drawing.Font("Cascadia Code", 108F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.codeLabel.ForeColor = System.Drawing.Color.LightGray;
			this.codeLabel.Location = new System.Drawing.Point(12, 171);
			this.codeLabel.Name = "codeLabel";
			this.codeLabel.Size = new System.Drawing.Size(920, 191);
			this.codeLabel.TabIndex = 1;
			this.codeLabel.Text = " CODE HERE";
			// 
			// winLabel
			// 
			this.winLabel.AutoSize = true;
			this.winLabel.BackColor = System.Drawing.Color.Black;
			this.winLabel.Font = new System.Drawing.Font("Cascadia Code", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.winLabel.ForeColor = System.Drawing.Color.LightGray;
			this.winLabel.Location = new System.Drawing.Point(12, 54);
			this.winLabel.Name = "winLabel";
			this.winLabel.Size = new System.Drawing.Size(357, 39);
			this.winLabel.TabIndex = 2;
			this.winLabel.Text = "Victory video path: ";
			// 
			// errorLabel
			// 
			this.errorLabel.AutoSize = true;
			this.errorLabel.BackColor = System.Drawing.Color.Black;
			this.errorLabel.Font = new System.Drawing.Font("Cascadia Code", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.errorLabel.ForeColor = System.Drawing.Color.LightGray;
			this.errorLabel.Location = new System.Drawing.Point(12, 93);
			this.errorLabel.Name = "errorLabel";
			this.errorLabel.Size = new System.Drawing.Size(323, 39);
			this.errorLabel.TabIndex = 3;
			this.errorLabel.Text = "Error video path: ";
			// 
			// videoLabel
			// 
			this.videoLabel.AutoSize = true;
			this.videoLabel.BackColor = System.Drawing.Color.Black;
			this.videoLabel.Font = new System.Drawing.Font("Cascadia Code", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.videoLabel.ForeColor = System.Drawing.Color.LightGray;
			this.videoLabel.Location = new System.Drawing.Point(12, 132);
			this.videoLabel.Name = "videoLabel";
			this.videoLabel.Size = new System.Drawing.Size(306, 39);
			this.videoLabel.TabIndex = 4;
			this.videoLabel.Text = "Game video path: ";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textТекстToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1147, 45);
			this.menuStrip1.TabIndex = 6;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// textТекстToolStripMenuItem
			// 
			this.textТекстToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.foreColorToolStripMenuItem,
            this.backToolStripMenuItem,
            this.sizeToolStripMenuItem,
            this.saveToolStripMenuItem});
			this.textТекстToolStripMenuItem.Name = "textТекстToolStripMenuItem";
			this.textТекстToolStripMenuItem.Size = new System.Drawing.Size(164, 41);
			this.textТекстToolStripMenuItem.Text = "Text / Текст";
			// 
			// foreColorToolStripMenuItem
			// 
			this.foreColorToolStripMenuItem.Name = "foreColorToolStripMenuItem";
			this.foreColorToolStripMenuItem.Size = new System.Drawing.Size(306, 42);
			this.foreColorToolStripMenuItem.Text = "ForeColor / Цвет";
			this.foreColorToolStripMenuItem.Click += new System.EventHandler(this.foreColorToolStripMenuItem_Click);
			// 
			// backToolStripMenuItem
			// 
			this.backToolStripMenuItem.Name = "backToolStripMenuItem";
			this.backToolStripMenuItem.Size = new System.Drawing.Size(306, 42);
			this.backToolStripMenuItem.Text = "Back / Фон";
			this.backToolStripMenuItem.Click += new System.EventHandler(this.backToolStripMenuItem_Click);
			// 
			// sizeToolStripMenuItem
			// 
			this.sizeToolStripMenuItem.Name = "sizeToolStripMenuItem";
			this.sizeToolStripMenuItem.Size = new System.Drawing.Size(306, 42);
			this.sizeToolStripMenuItem.Text = "Font / Шрифт";
			this.sizeToolStripMenuItem.Click += new System.EventHandler(this.sizeToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(306, 42);
			this.saveToolStripMenuItem.Text = "Save / Сохранить";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// debugLabel
			// 
			this.debugLabel.AutoSize = true;
			this.debugLabel.Font = new System.Drawing.Font("Cascadia Code", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.debugLabel.ForeColor = System.Drawing.Color.LightGray;
			this.debugLabel.Location = new System.Drawing.Point(12, 561);
			this.debugLabel.Name = "debugLabel";
			this.debugLabel.Size = new System.Drawing.Size(264, 49);
			this.debugLabel.TabIndex = 7;
			this.debugLabel.Text = "DEBUG PLACE";
			// 
			// OperatorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(1147, 741);
			this.Controls.Add(this.debugLabel);
			this.Controls.Add(this.videoLabel);
			this.Controls.Add(this.errorLabel);
			this.Controls.Add(this.winLabel);
			this.Controls.Add(this.codeLabel);
			this.Controls.Add(this.inputLabel);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "OperatorForm";
			this.Text = "OperatorForm";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OperatorForm_FormClosed);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label inputLabel;
		private System.Windows.Forms.Label codeLabel;
		private System.Windows.Forms.Label winLabel;
		private System.Windows.Forms.Label errorLabel;
		private System.Windows.Forms.Label videoLabel;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem textТекстToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem foreColorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem backToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sizeToolStripMenuItem;
		private System.Windows.Forms.FontDialog fontDialog;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.Label debugLabel;
	}
}