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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.scriptEditorGrid = new System.Windows.Forms.DataGridView();
            this.type = new System.Windows.Forms.DataGridViewButtonColumn();
            this.level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.saveBut = new System.Windows.Forms.Button();
            this.hardBut = new System.Windows.Forms.Button();
            this.mediumBut = new System.Windows.Forms.Button();
            this.easyBut = new System.Windows.Forms.Button();
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
            this.scriptEditorGrid.BackgroundColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.YellowGreen;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.DarkOliveGreen;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.scriptEditorGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.scriptEditorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.scriptEditorGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.type,
            this.level,
            this.prize,
            this.price});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.PaleGreen;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.Green;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.scriptEditorGrid.DefaultCellStyle = dataGridViewCellStyle8;
            this.scriptEditorGrid.GridColor = System.Drawing.Color.DarkGreen;
            this.scriptEditorGrid.Location = new System.Drawing.Point(0, 32);
            this.scriptEditorGrid.Name = "scriptEditorGrid";
            this.scriptEditorGrid.RowHeadersVisible = false;
            this.scriptEditorGrid.Size = new System.Drawing.Size(352, 455);
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
            this.saveBut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveBut.Location = new System.Drawing.Point(0, 0);
            this.saveBut.Name = "saveBut";
            this.saveBut.Size = new System.Drawing.Size(160, 32);
            this.saveBut.TabIndex = 60;
            this.saveBut.Text = "Сохранить";
            this.saveBut.UseVisualStyleBackColor = true;
            this.saveBut.Click += new System.EventHandler(this.saveBut_Click);
            // 
            // hardBut
            // 
            this.hardBut.BackColor = System.Drawing.Color.LightCoral;
            this.hardBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hardBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hardBut.Location = new System.Drawing.Point(288, 0);
            this.hardBut.Name = "hardBut";
            this.hardBut.Size = new System.Drawing.Size(64, 32);
            this.hardBut.TabIndex = 61;
            this.hardBut.Text = "СЛ";
            this.hardBut.UseVisualStyleBackColor = false;
            this.hardBut.Click += new System.EventHandler(this.hardBut_Click);
            // 
            // mediumBut
            // 
            this.mediumBut.BackColor = System.Drawing.Color.Khaki;
            this.mediumBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mediumBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mediumBut.Location = new System.Drawing.Point(224, 0);
            this.mediumBut.Name = "mediumBut";
            this.mediumBut.Size = new System.Drawing.Size(64, 32);
            this.mediumBut.TabIndex = 62;
            this.mediumBut.Text = "СР";
            this.mediumBut.UseVisualStyleBackColor = false;
            this.mediumBut.Click += new System.EventHandler(this.mediumBut_Click);
            // 
            // easyBut
            // 
            this.easyBut.BackColor = System.Drawing.Color.LightGreen;
            this.easyBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.easyBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.easyBut.Location = new System.Drawing.Point(160, 0);
            this.easyBut.Name = "easyBut";
            this.easyBut.Size = new System.Drawing.Size(64, 32);
            this.easyBut.TabIndex = 63;
            this.easyBut.Text = "Л";
            this.easyBut.UseVisualStyleBackColor = false;
            this.easyBut.Click += new System.EventHandler(this.easyBut_Click);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 489);
            this.Controls.Add(this.easyBut);
            this.Controls.Add(this.mediumBut);
            this.Controls.Add(this.hardBut);
            this.Controls.Add(this.saveBut);
            this.Controls.Add(this.scriptEditorGrid);
            this.Name = "ScriptEditor";
            this.Text = "Редактор скриптов";
            ((System.ComponentModel.ISupportInitialize)(this.scriptEditorGrid)).EndInit();
            this.ResumeLayout(false);

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
    }
}