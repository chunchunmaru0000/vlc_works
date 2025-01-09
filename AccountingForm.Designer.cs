namespace vlc_works
{
	partial class AccountingForm
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
			this.titleLabel = new System.Windows.Forms.Label();
			this.goldInSafeLabel = new System.Windows.Forms.Label();
			this.balanceTextLabel = new System.Windows.Forms.Label();
			this.balanceLabel = new System.Windows.Forms.Label();
			this.labelNowSelected = new System.Windows.Forms.Label();
			this.selectedPanel = new System.Windows.Forms.Panel();
			this.awardPanel = new System.Windows.Forms.Panel();
			this.awardLabel = new System.Windows.Forms.Label();
			this.awardTextLabel = new System.Windows.Forms.Label();
			this.priceLabel = new System.Windows.Forms.Label();
			this.levelLabel = new System.Windows.Forms.Label();
			this.priceTextLabel = new System.Windows.Forms.Label();
			this.levelTextLabel = new System.Windows.Forms.Label();
			this.showButton = new System.Windows.Forms.Button();
			this.levelPanel = new System.Windows.Forms.Panel();
			this.pricePanel = new System.Windows.Forms.Panel();
			this.selectedPanel.SuspendLayout();
			this.awardPanel.SuspendLayout();
			this.levelPanel.SuspendLayout();
			this.pricePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// titleLabel
			// 
			this.titleLabel.AutoSize = true;
			this.titleLabel.BackColor = System.Drawing.Color.Khaki;
			this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.titleLabel.Location = new System.Drawing.Point(0, 0);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(896, 31);
			this.titleLabel.TabIndex = 0;
			this.titleLabel.Text = "ПРОГРАММА УЧЕТА БАЛАНСА ВЫИГРЫШЕЙ И ОПЛАЧЕННЫХ ИГР";
			// 
			// goldInSafeLabel
			// 
			this.goldInSafeLabel.AutoSize = true;
			this.goldInSafeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.goldInSafeLabel.Location = new System.Drawing.Point(0, 31);
			this.goldInSafeLabel.Name = "goldInSafeLabel";
			this.goldInSafeLabel.Size = new System.Drawing.Size(228, 31);
			this.goldInSafeLabel.TabIndex = 1;
			this.goldInSafeLabel.Text = "GOLDinSAFE 2.0";
			// 
			// balanceTextLabel
			// 
			this.balanceTextLabel.AutoSize = true;
			this.balanceTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.balanceTextLabel.Location = new System.Drawing.Point(65, 62);
			this.balanceTextLabel.Name = "balanceTextLabel";
			this.balanceTextLabel.Size = new System.Drawing.Size(598, 31);
			this.balanceTextLabel.TabIndex = 2;
			this.balanceTextLabel.Text = "ВНИМАНИЕ !!! ТЕКУЩИЙ БАЛАНС СЕЙЧАС: ";
			// 
			// balanceLabel
			// 
			this.balanceLabel.BackColor = System.Drawing.Color.YellowGreen;
			this.balanceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.balanceLabel.Location = new System.Drawing.Point(660, 62);
			this.balanceLabel.Name = "balanceLabel";
			this.balanceLabel.Size = new System.Drawing.Size(140, 32);
			this.balanceLabel.TabIndex = 3;
			this.balanceLabel.Text = "#####";
			// 
			// labelNowSelected
			// 
			this.labelNowSelected.AutoSize = true;
			this.labelNowSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelNowSelected.Location = new System.Drawing.Point(112, 0);
			this.labelNowSelected.Name = "labelNowSelected";
			this.labelNowSelected.Size = new System.Drawing.Size(272, 31);
			this.labelNowSelected.TabIndex = 0;
			this.labelNowSelected.Text = "СЕЙЧАС ВЫБРАНО";
			// 
			// selectedPanel
			// 
			this.selectedPanel.BackColor = System.Drawing.Color.Silver;
			this.selectedPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.selectedPanel.Controls.Add(this.pricePanel);
			this.selectedPanel.Controls.Add(this.levelPanel);
			this.selectedPanel.Controls.Add(this.awardPanel);
			this.selectedPanel.Controls.Add(this.labelNowSelected);
			this.selectedPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.selectedPanel.Location = new System.Drawing.Point(832, 32);
			this.selectedPanel.Name = "selectedPanel";
			this.selectedPanel.Size = new System.Drawing.Size(480, 96);
			this.selectedPanel.TabIndex = 5;
			// 
			// awardPanel
			// 
			this.awardPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.awardPanel.Controls.Add(this.awardLabel);
			this.awardPanel.Controls.Add(this.awardTextLabel);
			this.awardPanel.Location = new System.Drawing.Point(0, 32);
			this.awardPanel.Name = "awardPanel";
			this.awardPanel.Size = new System.Drawing.Size(160, 64);
			this.awardPanel.TabIndex = 7;
			// 
			// awardLabel
			// 
			this.awardLabel.AutoSize = true;
			this.awardLabel.Location = new System.Drawing.Point(0, 32);
			this.awardLabel.Name = "awardLabel";
			this.awardLabel.Size = new System.Drawing.Size(74, 31);
			this.awardLabel.TabIndex = 4;
			this.awardLabel.Text = "####";
			// 
			// awardTextLabel
			// 
			this.awardTextLabel.AutoSize = true;
			this.awardTextLabel.Location = new System.Drawing.Point(0, 0);
			this.awardTextLabel.Name = "awardTextLabel";
			this.awardTextLabel.Size = new System.Drawing.Size(78, 31);
			this.awardTextLabel.TabIndex = 1;
			this.awardTextLabel.Text = "Приз";
			// 
			// priceLabel
			// 
			this.priceLabel.AutoSize = true;
			this.priceLabel.Location = new System.Drawing.Point(0, 32);
			this.priceLabel.Name = "priceLabel";
			this.priceLabel.Size = new System.Drawing.Size(74, 31);
			this.priceLabel.TabIndex = 6;
			this.priceLabel.Text = "####";
			// 
			// levelLabel
			// 
			this.levelLabel.AutoSize = true;
			this.levelLabel.Location = new System.Drawing.Point(0, 32);
			this.levelLabel.Name = "levelLabel";
			this.levelLabel.Size = new System.Drawing.Size(74, 31);
			this.levelLabel.TabIndex = 5;
			this.levelLabel.Text = "####";
			// 
			// priceTextLabel
			// 
			this.priceTextLabel.AutoSize = true;
			this.priceTextLabel.Location = new System.Drawing.Point(0, 0);
			this.priceTextLabel.Name = "priceTextLabel";
			this.priceTextLabel.Size = new System.Drawing.Size(150, 31);
			this.priceTextLabel.TabIndex = 3;
			this.priceTextLabel.Text = "Стоимость";
			// 
			// levelTextLabel
			// 
			this.levelTextLabel.AutoSize = true;
			this.levelTextLabel.Location = new System.Drawing.Point(0, 0);
			this.levelTextLabel.Name = "levelTextLabel";
			this.levelTextLabel.Size = new System.Drawing.Size(119, 31);
			this.levelTextLabel.TabIndex = 2;
			this.levelTextLabel.Text = "Уровень";
			// 
			// showButton
			// 
			this.showButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.showButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.showButton.Location = new System.Drawing.Point(1312, 32);
			this.showButton.Name = "showButton";
			this.showButton.Size = new System.Drawing.Size(160, 96);
			this.showButton.TabIndex = 6;
			this.showButton.Text = "Показать";
			this.showButton.UseVisualStyleBackColor = false;
			// 
			// levelPanel
			// 
			this.levelPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.levelPanel.Controls.Add(this.levelLabel);
			this.levelPanel.Controls.Add(this.levelTextLabel);
			this.levelPanel.Location = new System.Drawing.Point(160, 32);
			this.levelPanel.Name = "levelPanel";
			this.levelPanel.Size = new System.Drawing.Size(160, 64);
			this.levelPanel.TabIndex = 8;
			// 
			// pricePanel
			// 
			this.pricePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pricePanel.Controls.Add(this.priceLabel);
			this.pricePanel.Controls.Add(this.priceTextLabel);
			this.pricePanel.Location = new System.Drawing.Point(320, 32);
			this.pricePanel.Name = "pricePanel";
			this.pricePanel.Size = new System.Drawing.Size(160, 64);
			this.pricePanel.TabIndex = 9;
			// 
			// AccountingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1474, 692);
			this.Controls.Add(this.showButton);
			this.Controls.Add(this.selectedPanel);
			this.Controls.Add(this.balanceLabel);
			this.Controls.Add(this.balanceTextLabel);
			this.Controls.Add(this.goldInSafeLabel);
			this.Controls.Add(this.titleLabel);
			this.Name = "AccountingForm";
			this.Text = "AccountingForm";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AccountingForm_FormClosed);
			this.selectedPanel.ResumeLayout(false);
			this.selectedPanel.PerformLayout();
			this.awardPanel.ResumeLayout(false);
			this.awardPanel.PerformLayout();
			this.levelPanel.ResumeLayout(false);
			this.levelPanel.PerformLayout();
			this.pricePanel.ResumeLayout(false);
			this.pricePanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Label goldInSafeLabel;
		private System.Windows.Forms.Label balanceTextLabel;
		private System.Windows.Forms.Label balanceLabel;
		private System.Windows.Forms.Label labelNowSelected;
		private System.Windows.Forms.Panel selectedPanel;
		private System.Windows.Forms.Label awardLabel;
		private System.Windows.Forms.Label priceTextLabel;
		private System.Windows.Forms.Label levelTextLabel;
		private System.Windows.Forms.Label awardTextLabel;
		private System.Windows.Forms.Label priceLabel;
		private System.Windows.Forms.Label levelLabel;
		private System.Windows.Forms.Button showButton;
		private System.Windows.Forms.Panel awardPanel;
		private System.Windows.Forms.Panel levelPanel;
		private System.Windows.Forms.Panel pricePanel;
	}
}