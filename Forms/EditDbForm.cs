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

        #region COMMON

        private void print(object obj)
        {
            string str = obj == null ? "" : obj.ToString();

            Console.WriteLine(str);
        }

        private string MainGridRowToString(int i)
        {
            if (i < mainGrid.Rows.Count == false) // well
                return "";

            string[] cells = 
                mainGrid.Rows[i]
                .Cells
                .Cast<DataGridViewCell>()
                .Select(cell => cell.Value.ToString())
                .Take(5)
                .ToArray();

            return string.Join(" | ", cells);
        }

        private void EditDbForm_SizeChanged(object sender, EventArgs e)
        {
            mainGrid.Size = new Size(Size.Width - 16, Size.Height - 39);
        }

        #endregion COMMON

        #region GRID_ADD
        private bool isManuallyAdded { get; set; } = true;

        private void mainGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!isManuallyAdded)
                return;

            for (int i = e.RowIndex - 1; i < e.RowIndex + e.RowCount - 1; i++)
            {
                DataGridViewRow row = mainGrid.Rows[i];

                row.Cells["id"].Value = "_";
                row.Cells["player_id"].Value = "_";
                row.Cells["C"].Value = 0;
                row.Cells["K"].Value = 0;
                row.Cells["M"].Value = 0;
                row.Cells["photo"].Value = "Выбрать фото";
                row.Cells["save"].Value = "Сохранить";
                row.Cells["delete"].Value = "УДАЛИТЬ";
                row.Height = 32;
            }
        }

        private void mainGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!isManuallyAdded || e.RowIndex < 0)
                return;
            Console.WriteLine($"{e.RowIndex}, {e.ColumnIndex}, {mainGrid.Rows.Count}");

            DataGridViewCell cell = mainGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell is DataGridViewButtonCell)
                return;

            cell.Style = new DataGridViewCellStyle()
            {
                BackColor = Color.Khaki,
                SelectionBackColor = Color.DarkKhaki
            };
        }

        private void SelectPlayersFromDb()
        {
            isManuallyAdded = false;

            foreach (DbPlayer player in Db.SelectAllPlayers())
            {
                print(player);
                DataGridViewRow row = new DataGridViewRow()
                { Height = 32 };

                row.Cells.AddRange(new DataGridViewCell[]
                {
                    new DataGridViewTextBoxCell(){ Value = player.Id },
                    new DataGridViewTextBoxCell(){ Value = player.PlayerIdInt },
                    new DataGridViewTextBoxCell(){ Value = player.C },
                    new DataGridViewTextBoxCell(){ Value = player.K },
                    new DataGridViewTextBoxCell(){ Value = player.M },
                    new DataGridViewButtonCell() { Value = "Выбрать фото", FlatStyle = FlatStyle.Flat },
                    new DataGridViewButtonCell() { Value = "Сохранить", FlatStyle = FlatStyle.Flat },
                    new DataGridViewButtonCell() { Value = "УДАЛИТЬ", FlatStyle = FlatStyle.Flat },
                });

                mainGrid.Rows.Add(row);
            }

            isManuallyAdded = true;
        }
        #endregion GRID_ADD

        #region GRID_BUTTONS

        private void mainGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (mainGrid.Columns[e.ColumnIndex].Name)
            {
                case "photo": SelectPhoto(e.RowIndex); break;
                case "save": SetPlayer(e.RowIndex); break;
                case "delete": DeletePlayer(e.RowIndex); break;
                default:
                    break;
            }
        }

        private void DeletePlayer(int rowIndex)
        {
            if (MessageBox.Show(
                $"ВЫ УВЕРЕНЫ ЧТО ХОТИТЕ УДАЛИТЬ ЗАПИСЬ: [ {MainGridRowToString(rowIndex)} ]?",
                "ВЫ УВЕРЕНЫ???",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
                ) == DialogResult.Yes)
                DefinetlyDeletePlayer(mainGrid.Rows[rowIndex]);
        }

        private void DefinetlyDeletePlayer(DataGridViewRow playerRow)
        {
            object[] cells = playerRow
                .Cells
                .Cast<DataGridViewCell>()
                .Select(cell => cell.Value)
                .ToArray();

            if (cells[0] is string) // id cell is "_"
                mainGrid.Rows.Remove(playerRow);
            else
            {
                long id = Convert.ToInt64(cells[0]);

                if (DeleteEnrollmentFromAiDevice(id))
                    Db.DeletePlayerWhomIdEquals(id);
            }
        }

        private bool DeleteEnrollmentFromAiDevice(long id) =>
            axFP_CLOCK.DeleteEnrollData(
                machineNumber, 
                Convert.ToInt32(id), 
                machineNumber, 
                (int)BackupNum.AIFace);

        private void SetPlayer(int rowIndex)
        {
            return;
        }

        private void SelectPhoto(int rowIndex)
        {
            return;
        }

        #endregion GRID_BUTTONS
    }
}
