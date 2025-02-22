using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AxFP_CLOCKLib;

namespace vlc_works
{
    public partial class EditDbForm: Form
    {
        #region VAR
        private FaceForm faceForm;
        private AxFP_CLOCK axFP_CLOCK { get; set; }
        private int machineNumber = 1;
        #endregion VAR

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
                    new DataGridViewButtonCell(){ Value = player.Id, FlatStyle = FlatStyle.Flat },
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
                case "id":     ShowPhoto    (e.RowIndex); break;
                case "photo":  SelectPhoto  (e.RowIndex); break;
                case "save":   SetPlayer    (e.RowIndex); break;
                case "delete": DeletePlayer (e.RowIndex); break;
                default:
                    break;
            }
        }

        #region DeletePlayer

        private void DeletePlayer(int rowIndex)
        {
            if (MessageBox.Show(
                $"ВЫ УВЕРЕНЫ ЧТО ХОТИТЕ УДАЛИТЬ ЗАПИСЬ: [ {MainGridRowToString(rowIndex)} ]?",
                "ВЫ УВЕРЕНЫ???",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
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
                {
                    Db.DeletePlayerWhomIdEquals(id);
                    mainGrid.Rows.Remove(playerRow);
                }
                else
                    MessageBox.Show(
                        "ЗАПИСЬ НЕ БЫЛА УДАЛЕНА\n" +
                        "- Проверьте подключение");
            }
        }

        private bool DeleteEnrollmentFromAiDevice(long id) =>
            axFP_CLOCK.DeleteEnrollData(
                machineNumber, 
                Convert.ToInt32(id), 
                machineNumber, 
                (int)BackupNum.AIFace);

        #endregion

        private void SetPlayer(int rowIndex)
        {
            return;
        }

        private Dictionary<int, byte[]> rowIndexToSelectedImage = new Dictionary<int, byte[]>();

        private void SelectPhoto(int rowIndex)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string selectedFilePath = openFileDialog.FileName;
            byte[] photoBytes = new byte[0];

            try {
                if (!selectedFilePath.ToLower().EndsWith(".jpg") &&
                    !selectedFilePath.ToLower().EndsWith(".jpeg"))
                    throw new Exception("ИЗОБРАЖЕНИЕ ДОЛЖНО БЫТЬ С РАСШИРЕНИЕМ .jpg ИЛИ .jpeg");

                photoBytes = System.IO.File.ReadAllBytes(selectedFilePath);

                if (photoBytes.Length > 153_600)
                    throw new Exception(
                        $"РАЗМЕР ИЗОБРАЖЕНИЯ: {photoBytes.Length / 1024}КБ\n" +
                        $"РАЗМЕР ИЗОБРАЖЕНИЯ НЕ ДОЛЖЕН ПРЕВЫШАТЬ 150КБ");
            }
            catch (Exception e) {
                MessageBox.Show(e.Message, "ОШИБКА ПРИ ВЫБОРЕ ФОТО");
                return;
            }
            rowIndexToSelectedImage[rowIndex] = photoBytes;
        }

        private void ShowPhoto(int rowIndex)
        {
            
        }

        #endregion GRID_BUTTONS
    }
}
