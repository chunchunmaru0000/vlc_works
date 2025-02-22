using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxFP_CLOCKLib;

namespace vlc_works
{
    public partial class EditDbForm: Form
    {
        private FaceForm faceForm;
        private AxFP_CLOCK axFP_CLOCK { get; set; }
        private int machineNumber = 1;

        public EditDbForm(FaceForm faceForm, AxFP_CLOCK axFP_CLOCK, int machineNumber)
        {
            InitializeComponent();

            this.faceForm = faceForm;
            this.axFP_CLOCK = axFP_CLOCK;
            this.machineNumber = machineNumber;

            SelectPlayersFromDb();
        }

        private void EditDbForm_SizeChanged(object sender, EventArgs e)
        {
            mainGrid.Size = new Size(Size.Width - 16, Size.Height - 39);
        }

        private void mainGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex - 1; i < e.RowIndex + e.RowCount - 1; i++)
            {
                mainGrid.Rows[i].Cells["C"].Value = 0;
                mainGrid.Rows[i].Cells["K"].Value = 0;
                mainGrid.Rows[i].Cells["M"].Value = 0;
                mainGrid.Rows[i].Cells["photo"].Value = "Выбрать фото";
                mainGrid.Rows[i].Cells["save"].Value = "Сохранить";
                mainGrid.Rows[i].Cells["delete"].Value = "УДАЛИТЬ";
                
            }
        }

        private void SelectPlayersFromDb()
        {
            foreach (DbPlayer player in Db.SelectAllPlayers()) ;

        }
    }
}
