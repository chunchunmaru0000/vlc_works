namespace vlc_works015
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.midBorder = new System.Windows.Forms.TextBox();
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
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.YellowGreen;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.DarkOliveGreen;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.scriptEditorGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.scriptEditorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.scriptEditorGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.type,
            this.level,
            this.prize,
            this.price});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.PaleGreen;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.Green;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.scriptEditorGrid.DefaultCellStyle = dataGridViewCellStyle6;
            this.scriptEditorGrid.GridColor = System.Drawing.Color.Black;
            this.scriptEditorGrid.Location = new System.Drawing.Point(0, 64);
            this.scriptEditorGrid.Name = "scriptEditorGrid";
            this.scriptEditorGrid.RowHeadersVisible = false;
            this.scriptEditorGrid.Size = new System.Drawing.Size(512, 456);
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
            this.saveBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveBut.Location = new System.Drawing.Point(0, 32);
            this.saveBut.Name = "saveBut";
            this.saveBut.Size = new System.Drawing.Size(192, 32);
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
            this.hardBut.Location = new System.Drawing.Point(320, 32);
            this.hardBut.Name = "hardBut";
            this.hardBut.Size = new System.Drawing.Size(64, 32);
            this.hardBut.TabIndex = 61;
            this.hardBut.Text = "ВЫСОКИЙ";
            this.hardBut.UseVisualStyleBackColor = false;
            this.hardBut.Click += new System.EventHandler(this.hardBut_Click);
            // 
            // mediumBut
            // 
            this.mediumBut.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.mediumBut.BackColor = System.Drawing.Color.Khaki;
            this.mediumBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mediumBut.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mediumBut.Location = new System.Drawing.Point(256, 32);
            this.mediumBut.Name = "mediumBut";
            this.mediumBut.Size = new System.Drawing.Size(64, 32);
            this.mediumBut.TabIndex = 62;
            this.mediumBut.Text = "СРЕДНИЙ";
            this.mediumBut.UseVisualStyleBackColor = false;
            this.mediumBut.Click += new System.EventHandler(this.mediumBut_Click);
            // 
            // easyBut
            // 
            this.easyBut.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.easyBut.BackColor = System.Drawing.Color.LightGreen;
            this.easyBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.easyBut.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.easyBut.Location = new System.Drawing.Point(192, 32);
            this.easyBut.Name = "easyBut";
            this.easyBut.Size = new System.Drawing.Size(64, 32);
            this.easyBut.TabIndex = 63;
            this.easyBut.Text = "НИЗКИЙ";
            this.easyBut.UseVisualStyleBackColor = false;
            this.easyBut.Click += new System.EventHandler(this.easyBut_Click);
            // 
            // resetGamesCounterBut
            // 
            this.resetGamesCounterBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetGamesCounterBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.resetGamesCounterBut.Location = new System.Drawing.Point(0, 0);
            this.resetGamesCounterBut.Name = "resetGamesCounterBut";
            this.resetGamesCounterBut.Size = new System.Drawing.Size(64, 32);
            this.resetGamesCounterBut.TabIndex = 64;
            this.resetGamesCounterBut.Text = "↩️";
            this.resetGamesCounterBut.UseVisualStyleBackColor = true;
            this.resetGamesCounterBut.Click += new System.EventHandler(this.resetGamesCounterBut_Click);
            // 
            // gamesCounterLabel
            // 
            this.gamesCounterLabel.Font = new System.Drawing.Font("Cascadia Code", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gamesCounterLabel.Location = new System.Drawing.Point(64, 0);
            this.gamesCounterLabel.Name = "gamesCounterLabel";
            this.gamesCounterLabel.Size = new System.Drawing.Size(64, 32);
            this.gamesCounterLabel.TabIndex = 65;
            this.gamesCounterLabel.Text = "0";
            // 
            // lowBorder
            // 
            this.lowBorder.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lowBorder.Location = new System.Drawing.Point(192, 0);
            this.lowBorder.Name = "lowBorder";
            this.lowBorder.Size = new System.Drawing.Size(64, 31);
            this.lowBorder.TabIndex = 66;
            this.lowBorder.TextChanged += new System.EventHandler(this.lowBorder_TextChanged);
            // 
            // midBorder
            // 
            this.midBorder.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.midBorder.Location = new System.Drawing.Point(256, 0);
            this.midBorder.Name = "midBorder";
            this.midBorder.Size = new System.Drawing.Size(64, 31);
            this.midBorder.TabIndex = 67;
            this.midBorder.TextChanged += new System.EventHandler(this.midBorder_TextChanged);
            // 
            // gamesCounterBox
            // 
            this.gamesCounterBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gamesCounterBox.Location = new System.Drawing.Point(128, 0);
            this.gamesCounterBox.Name = "gamesCounterBox";
            this.gamesCounterBox.Size = new System.Drawing.Size(64, 31);
            this.gamesCounterBox.TabIndex = 68;
            this.gamesCounterBox.TextChanged += new System.EventHandler(this.gamesCounterBox_TextChanged);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 522);
            this.Controls.Add(this.gamesCounterBox);
            this.Controls.Add(this.midBorder);
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
        public System.Windows.Forms.Label gamesCounterLabel;
        private System.Windows.Forms.TextBox lowBorder;
        private System.Windows.Forms.TextBox midBorder;
        private System.Windows.Forms.TextBox gamesCounterBox;
    }
}