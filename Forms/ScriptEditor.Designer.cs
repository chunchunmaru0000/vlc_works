namespace vlc_works
{
    partial class ScriptEditor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.scriptEditorGrid = new System.Windows.Forms.DataGridView();
            this.type = new System.Windows.Forms.DataGridViewButtonColumn();
            this.level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saveBut = new System.Windows.Forms.Button();
            this.hardBut = new System.Windows.Forms.Button();
            this.mediumBut = new System.Windows.Forms.Button();
            this.easyBut = new System.Windows.Forms.Button();
            this.resetGamesCounterBut = new System.Windows.Forms.Button();
            this.gamesCounterLabel = new System.Windows.Forms.Label();
            this.lowBorder = new System.Windows.Forms.TextBox();
            this.mediumBorder = new System.Windows.Forms.TextBox();
            this.gamesCounterBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.scriptEditorGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // scriptEditorGrid
            // 
            this.scriptEditorGrid.AllowUserToAddRows = false;
            this.scriptEditorGrid.AllowUserToDeleteRows = false;
            this.scriptEditorGrid.AllowUserToResizeColumns = false;
            this.scriptEditorGrid.AllowUserToResizeRows = false;
            this.scriptEditorGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scriptEditorGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.scriptEditorGrid.BackgroundColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.YellowGreen;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DarkOliveGreen;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.scriptEditorGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.scriptEditorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.scriptEditorGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.type,
            this.level,
            this.prize,
            this.price});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.PaleGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Green;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.scriptEditorGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.scriptEditorGrid.GridColor = System.Drawing.Color.Black;
            this.scriptEditorGrid.Location = new System.Drawing.Point(0, 64);
            this.scriptEditorGrid.Name = "scriptEditorGrid";
            this.scriptEditorGrid.RowHeadersVisible = false;
            this.scriptEditorGrid.Size = new System.Drawing.Size(352, 460);
            this.scriptEditorGrid.TabIndex = 59;
            this.scriptEditorGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.scriptEditorGrid_CellValueChanged);
            // 
            // type
            // 
            this.type.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.type.HeaderText = "Тип";
            this.type.Name = "type";
            this.type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // level
            // 
            this.level.HeaderText = "Уров.";
            this.level.Name = "level";
            this.level.ReadOnly = true;
            // 
            // prize
            // 
            this.prize.HeaderText = "Приз";
            this.prize.Name = "prize";
            // 
            // price
            // 
            this.price.HeaderText = "Цена";
            this.price.Name = "price";
            // 
            // saveBut
            // 
            this.saveBut.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveBut.Location = new System.Drawing.Point(0, 32);
            this.saveBut.Name = "saveBut";
            this.saveBut.Size = new System.Drawing.Size(160, 32);
            this.saveBut.TabIndex = 60;
            this.saveBut.Text = "Сохранить";
            this.saveBut.UseVisualStyleBackColor = true;
            this.saveBut.Click += new System.EventHandler(this.saveBut_Click);
            // 
            // hardBut
            // 
            this.hardBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hardBut.BackColor = System.Drawing.Color.LightCoral;
            this.hardBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hardBut.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hardBut.Location = new System.Drawing.Point(288, 32);
            this.hardBut.Name = "hardBut";
            this.hardBut.Size = new System.Drawing.Size(64, 32);
            this.hardBut.TabIndex = 61;
            this.hardBut.Text = "ВЫС";
            this.hardBut.UseVisualStyleBackColor = false;
            this.hardBut.Click += new System.EventHandler(this.hardBut_Click);
            // 
            // mediumBut
            // 
            this.mediumBut.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.mediumBut.BackColor = System.Drawing.Color.Khaki;
            this.mediumBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mediumBut.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mediumBut.Location = new System.Drawing.Point(224, 32);
            this.mediumBut.Name = "mediumBut";
            this.mediumBut.Size = new System.Drawing.Size(64, 32);
            this.mediumBut.TabIndex = 62;
            this.mediumBut.Text = "СРЕД";
            this.mediumBut.UseVisualStyleBackColor = false;
            this.mediumBut.Click += new System.EventHandler(this.mediumBut_Click);
            // 
            // easyBut
            // 
            this.easyBut.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.easyBut.BackColor = System.Drawing.Color.LightGreen;
            this.easyBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.easyBut.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.easyBut.Location = new System.Drawing.Point(160, 32);
            this.easyBut.Name = "easyBut";
            this.easyBut.Size = new System.Drawing.Size(64, 32);
            this.easyBut.TabIndex = 63;
            this.easyBut.Text = "НИЗК";
            this.easyBut.UseVisualStyleBackColor = false;
            this.easyBut.Click += new System.EventHandler(this.easyBut_Click);
            // 
            // resetGamesCounterBut
            // 
            this.resetGamesCounterBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetGamesCounterBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.resetGamesCounterBut.Location = new System.Drawing.Point(0, 0);
            this.resetGamesCounterBut.Name = "resetGamesCounterBut";
            this.resetGamesCounterBut.Size = new System.Drawing.Size(96, 32);
            this.resetGamesCounterBut.TabIndex = 64;
            this.resetGamesCounterBut.Text = "↩️";
            this.resetGamesCounterBut.UseVisualStyleBackColor = true;
            // 
            // gamesCounterLabel
            // 
            this.gamesCounterLabel.Font = new System.Drawing.Font("Cascadia Code", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gamesCounterLabel.Location = new System.Drawing.Point(96, 0);
            this.gamesCounterLabel.Name = "gamesCounterLabel";
            this.gamesCounterLabel.Size = new System.Drawing.Size(64, 32);
            this.gamesCounterLabel.TabIndex = 65;
            this.gamesCounterLabel.Text = "####";
            // 
            // lowBorder
            // 
            this.lowBorder.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lowBorder.Location = new System.Drawing.Point(288, 0);
            this.lowBorder.Name = "lowBorder";
            this.lowBorder.Size = new System.Drawing.Size(64, 31);
            this.lowBorder.TabIndex = 66;
            // 
            // mediumBorder
            // 
            this.mediumBorder.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mediumBorder.Location = new System.Drawing.Point(224, 0);
            this.mediumBorder.Name = "mediumBorder";
            this.mediumBorder.Size = new System.Drawing.Size(64, 31);
            this.mediumBorder.TabIndex = 67;
            // 
            // gamesCounterBox
            // 
            this.gamesCounterBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gamesCounterBox.Location = new System.Drawing.Point(160, 0);
            this.gamesCounterBox.Name = "gamesCounterBox";
            this.gamesCounterBox.Size = new System.Drawing.Size(64, 31);
            this.gamesCounterBox.TabIndex = 68;
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 526);
            this.Controls.Add(this.gamesCounterBox);
            this.Controls.Add(this.mediumBorder);
            this.Controls.Add(this.lowBorder);
            this.Controls.Add(this.gamesCounterLabel);
            this.Controls.Add(this.resetGamesCounterBut);
            this.Controls.Add(this.easyBut);
            this.Controls.Add(this.mediumBut);
            this.Controls.Add(this.hardBut);
            this.Controls.Add(this.saveBut);
            this.Controls.Add(this.scriptEditorGrid);
            this.Name = "ScriptEditor";
            this.Text = "Редактор скриптов";
            ((System.ComponentModel.ISupportInitialize)(this.scriptEditorGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView scriptEditorGrid;
        private System.Windows.Forms.Button saveBut;
        private System.Windows.Forms.DataGridViewButtonColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn level;
        private System.Windows.Forms.DataGridViewTextBoxColumn prize;
        private System.Windows.Forms.DataGridViewTextBoxColumn price;
        private System.Windows.Forms.Button hardBut;
        private System.Windows.Forms.Button mediumBut;
        private System.Windows.Forms.Button easyBut;
        private System.Windows.Forms.Button resetGamesCounterBut;
        private System.Windows.Forms.Label gamesCounterLabel;
        private System.Windows.Forms.TextBox lowBorder;
        private System.Windows.Forms.TextBox mediumBorder;
        private System.Windows.Forms.TextBox gamesCounterBox;
    }
}