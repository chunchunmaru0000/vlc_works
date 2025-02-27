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
            this.saveBut = new System.Windows.Forms.Button();
            this.type = new System.Windows.Forms.DataGridViewButtonColumn();
            this.level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.price = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.scriptEditorGrid.GridColor = System.Drawing.Color.DarkGreen;
            this.scriptEditorGrid.Location = new System.Drawing.Point(0, 32);
            this.scriptEditorGrid.Name = "scriptEditorGrid";
            this.scriptEditorGrid.RowHeadersVisible = false;
            this.scriptEditorGrid.Size = new System.Drawing.Size(352, 455);
            this.scriptEditorGrid.TabIndex = 59;
            this.scriptEditorGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.scriptEditorGrid_CellValueChanged);
            // 
            // saveBut
            // 
            this.saveBut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveBut.Location = new System.Drawing.Point(0, 0);
            this.saveBut.Name = "saveBut";
            this.saveBut.Size = new System.Drawing.Size(352, 32);
            this.saveBut.TabIndex = 60;
            this.saveBut.Text = "Сохранить";
            this.saveBut.UseVisualStyleBackColor = true;
            this.saveBut.Click += new System.EventHandler(this.saveBut_Click);
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
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 489);
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
    }
}