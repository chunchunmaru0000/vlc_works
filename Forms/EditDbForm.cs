using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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

        private bool isAddedRow { get; set; } = false;
        private long playerAutoincerentCounter { get; set; }
        #endregion VAR

        public EditDbForm(FaceForm faceForm, AxFP_CLOCK axFP_CLOCK, int machineNumber)
        {
            InitializeComponent();

            this.faceForm = faceForm;
            this.axFP_CLOCK = axFP_CLOCK;
            this.machineNumber = machineNumber;
            playerAutoincerentCounter = Db.AutoincrementCounter(Db.PlayersTableName);

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
        private DataGridViewCellStyle khakiStyle { get; set; } = new DataGridViewCellStyle()
        {
            BackColor = Color.Khaki,
            SelectionBackColor = Color.DarkKhaki
        };

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

            cell.Style = khakiStyle.Clone();
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
            faceForm.PerformOperation(() => 
                axFP_CLOCK.DeleteEnrollData(
                    machineNumber, 
                    Convert.ToInt32(id), 
                    machineNumber, 
                    (int)BackupNum.AIFace));

        #endregion


        #region SetPlayer

        private readonly Dictionary<string, string> columnNameToDbColumnName = new Dictionary<string, string>()
        {
            { "player_id", "player_id_int" },
            { "C", "c_lvl_int" },
            { "K", "k_lvl_int" },
            { "M", "m_lvl_int" },
        };

        private void SetPlayer(int rowIndex)
        {
            DataGridViewRow row = mainGrid.Rows[rowIndex];
            DataGridViewCell[] changedCells = 
                row
                .Cells
                .Cast<DataGridViewCell>()
                .Where(cell => cell.Style.BackColor == khakiStyle.BackColor)
                .ToArray();

            if (changedCells.Length == 0)
            {
                MessageBox.Show("НЕЧЕГО СОХРАНЯТЬ");
                return;
            }

            // TODO:
            // - do new row player id = Db.Autoincrement
            long rowPlayerId = Convert.ToInt64(row.Cells[1].Value);

            foreach (DataGridViewCell cell in changedCells)
            {
                string cellColumnName = mainGrid.Columns[cell.ColumnIndex].Name;

                // TODO:
                // - cant change PlayerId
                // - cant add more than 1 player to table at once

                switch (cellColumnName)
                {
                    case "id": 
                        AddNewPlayer(changedCells);
                        return; // return because nothing will be left to set
                    case "photo":
                        UpdatePlayerPhoto(cell);
                        break;
                    default:
                        Db.UpdatePlayerIntData(
                            rowPlayerId,
                            Convert.ToInt64(cell.Value),
                            columnNameToDbColumnName[cellColumnName]
                            );
                        cell.Style = mainGrid.DefaultCellStyle;
                        break;
                }
            }
        }

        private void AddNewPlayer(DataGridViewCell[] changedCells)
        {

        }

        private void UpdatePlayerPhoto(DataGridViewCell choosePhotoButCell)
        {

        }

        #endregion SetPlayer

        #region SelectPhoto_AND_ShowPhoto
        private Dictionary<int, byte[]> rowIndexToSelectedImage { get; set; } = new Dictionary<int, byte[]>();

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
                        $"РАЗМЕР ИЗОБРАЖЕНИЯ: {Math.Round(photoBytes.Length / 1024.0, 1)} КБ\n" +
                        $"РАЗМЕР ИЗОБРАЖЕНИЯ НЕ ДОЛЖЕН ПРЕВЫШАТЬ 150.0 КБ");
            }
            catch (Exception e) {
                MessageBox.Show(e.Message, "ОШИБКА ПРИ ВЫБОРЕ ФОТО");
                return;
            }

            mainGrid.Rows[rowIndex].Cells[5].Style = khakiStyle.Clone();
            rowIndexToSelectedImage[rowIndex] = photoBytes;
        }

        private PhotoForm photoForm { get; set; }

        private void ShowPhoto(int rowIndex)
        {
            if (photoForm != null && !photoForm.IsDisposed)
                return;

            byte[] photoBytes = new byte[0];

            if (rowIndexToSelectedImage.ContainsKey(rowIndex))
                photoBytes = rowIndexToSelectedImage[rowIndex];
            else if (mainGrid.Rows[rowIndex].Cells[0].Value is string) {
                MessageBox.Show("ФОТО ЕЩЕ НЕ БЫЛО ВЫБРАНО ДЛЯ ДАННОГО ИГРОКА");
                return;
            }
            else {
                int dwEnrollNumber = Convert.ToInt32(mainGrid.Rows[rowIndex].Cells[0].Value);
                int dwPhotoSize = 0;
                IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(400800);

                bool successffulyReadedPhoto =
                    faceForm.PerformOperation(() =>
                        axFP_CLOCK.GetEnrollPhotoCS(
                            machineNumber,
                            dwEnrollNumber,
                            ref dwPhotoSize,
                            ptrIndexFacePhoto
                            ));

                if (successffulyReadedPhoto)
                {
                    photoBytes = new byte[dwPhotoSize];
                    Marshal.Copy(ptrIndexFacePhoto, photoBytes, 0, dwPhotoSize);

                    rowIndexToSelectedImage[rowIndex] = photoBytes;
                } else {
                    MessageBox.Show("ФОТО НЕ БЫЛО УСПЕШНО ПРОЧИТАНО ИЗ УСТРОЙСТВА");
                    return;
                }
            }

            photoForm = new PhotoForm(photoBytes);
            photoForm.Show();
            photoForm.Location = new Point(2000, 100);
        }

        #endregion SelectPhoto_AND_ShowPhoto

        #endregion GRID_BUTTONS
    }
}
