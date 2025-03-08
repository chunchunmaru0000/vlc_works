namespace vlc_works
{
    partial class EditDbForm
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
            this.mainGrid = new System.Windows.Forms.DataGridView();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.newPlayerBut = new System.Windows.Forms.Button();
            this.face = new System.Windows.Forms.DataGridViewImageColumn();
            this.id = new System.Windows.Forms.DataGridViewButtonColumn();
            this.player_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.C = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.K = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.M = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.photo = new System.Windows.Forms.DataGridViewButtonColumn();
            this.save = new System.Windows.Forms.DataGridViewButtonColumn();
            this.delete = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.mainGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // mainGrid
            // 
            this.mainGrid.AllowUserToAddRows = false;
            this.mainGrid.AllowUserToResizeRows = false;
            this.mainGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.mainGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.mainGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mainGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.face,
            this.id,
            this.player_id,
            this.C,
            this.K,
            this.M,
            this.photo,
            this.save,
            this.delete});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.mainGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.mainGrid.Location = new System.Drawing.Point(0, 32);
            this.mainGrid.Name = "mainGrid";
            this.mainGrid.RowHeadersVisible = false;
            this.mainGrid.Size = new System.Drawing.Size(640, 641);
            this.mainGrid.TabIndex = 0;
            this.mainGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.mainGrid_CellContentClick);
            this.mainGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.mainGrid_CellValueChanged);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // newPlayerBut
            // 
            this.newPlayerBut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newPlayerBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.newPlayerBut.Location = new System.Drawing.Point(0, 0);
            this.newPlayerBut.Name = "newPlayerBut";
            this.newPlayerBut.Size = new System.Drawing.Size(288, 32);
            this.newPlayerBut.TabIndex = 1;
            this.newPlayerBut.Text = "Добавить нового игрока";
            this.newPlayerBut.UseVisualStyleBackColor = true;
            this.newPlayerBut.Click += new System.EventHandler(this.newPlayerBut_Click);
            // 
            // face
            // 
            this.face.FillWeight = 50F;
            this.face.HeaderText = "Фото";
            this.face.Name = "face";
            this.face.ReadOnly = true;
            // 
            // id
            // 
            this.id.FillWeight = 50F;
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.id.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // player_id
            // 
            this.player_id.FillWeight = 50F;
            this.player_id.HeaderText = "id игрока";
            this.player_id.Name = "player_id";
            // 
            // C
            // 
            this.C.FillWeight = 25F;
            this.C.HeaderText = "C";
            this.C.Name = "C";
            // 
            // K
            // 
            this.K.FillWeight = 25F;
            this.K.HeaderText = "K";
            this.K.Name = "K";
            // 
            // M
            // 
            this.M.FillWeight = 25F;
            this.M.HeaderText = "M";
            this.M.Name = "M";
            // 
            // photo
            // 
            this.photo.FillWeight = 85.27919F;
            this.photo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.photo.HeaderText = "Выбор";
            this.photo.Name = "photo";
            this.photo.Text = "Выбор фото";
            // 
            // save
            // 
            this.save.FillWeight = 85.27919F;
            this.save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.save.HeaderText = "Сохранить";
            this.save.Name = "save";
            this.save.Text = "Сохранить";
            // 
            // delete
            // 
            this.delete.FillWeight = 85.27919F;
            this.delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.delete.HeaderText = "Удаление";
            this.delete.Name = "delete";
            this.delete.Text = "УДАЛИТЬ";
            // 
            // EditDbForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 673);
            this.Controls.Add(this.newPlayerBut);
            this.Controls.Add(this.mainGrid);
            this.Name = "EditDbForm";
            this.Text = "EditDbForm";
            ((System.ComponentModel.ISupportInitialize)(this.mainGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView mainGrid;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button newPlayerBut;
        private System.Windows.Forms.DataGridViewImageColumn face;
        private System.Windows.Forms.DataGridViewButtonColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn player_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn C;
        private System.Windows.Forms.DataGridViewTextBoxColumn K;
        private System.Windows.Forms.DataGridViewTextBoxColumn M;
        private System.Windows.Forms.DataGridViewButtonColumn photo;
        private System.Windows.Forms.DataGridViewButtonColumn save;
        private System.Windows.Forms.DataGridViewButtonColumn delete;
    }
}