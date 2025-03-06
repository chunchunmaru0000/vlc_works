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
        private const bool IS_DEBUG = false;
        #region VAR
        private FaceForm faceForm;
        private AxFP_CLOCK axFP_CLOCK { get; set; }
        private int machineNumber = 1;

        private long playerTableAutoincerentCounter { get; set; }
        #endregion VAR

        public EditDbForm(FaceForm faceForm, AxFP_CLOCK axFP_CLOCK, int machineNumber)
        {
            InitializeComponent();

            this.faceForm = faceForm;
            this.axFP_CLOCK = axFP_CLOCK;
            this.machineNumber = machineNumber;
            playerTableAutoincerentCounter = Db.AutoincrementCounter(Db.PlayersTableName);

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
            mainGrid.Size = new Size(Size.Width - 16, Size.Height - 39 - 32);
        }

        #endregion COMMON

        #region GRID_ADD

        private bool isManuallyAdded { get; set; } = true;
        private DataGridViewCellStyle khakiStyle { get; set; } = new DataGridViewCellStyle()
        {
            BackColor = Color.Khaki,
            SelectionBackColor = Color.DarkKhaki
        };

        private DataGridViewCellStyle defaultStyle { get; set; } = new DataGridViewCellStyle()
        {
            BackColor = Color.White,
            SelectionBackColor = Color.Blue
        };

        private void newPlayerBut_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = new DataGridViewRow()
            { Height = 32 };

            playerTableAutoincerentCounter++;

            row.Cells.AddRange(new DataGridViewCell[] {
                new DataGridViewButtonCell(){ 
                    Value = playerTableAutoincerentCounter.ToString(),
                    FlatStyle = FlatStyle.Flat },
                new DataGridViewTextBoxCell(){ 
                    Value = playerTableAutoincerentCounter },
                new DataGridViewTextBoxCell(){ Value = 0 },
                new DataGridViewTextBoxCell(){ Value = 0 },
                new DataGridViewTextBoxCell(){ Value = 0 },
                new DataGridViewButtonCell() { 
                    Value = "Выбрать фото", 
                    FlatStyle = FlatStyle.Flat },
                new DataGridViewButtonCell() { 
                    Value = "Сохранить", 
                    FlatStyle = FlatStyle.Flat },
                new DataGridViewButtonCell() { 
                    Value = "УДАЛИТЬ", 
                    FlatStyle = FlatStyle.Flat },
            });

            mainGrid.Rows.Add(row);

            foreach (DataGridViewCell cell in row.Cells)
                if (!(cell is DataGridViewButtonCell) || cell.ColumnIndex == 0) // id cell
                    cell.Style = khakiStyle.Clone();
            
            newPlayerBut.Enabled = false;
        }

        private void mainGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!isManuallyAdded || e.RowIndex < 0)
                return;
            //print($"{e.RowIndex}, {e.ColumnIndex}, {mainGrid.Rows.Count} {mainGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value}");

            DataGridViewCell cell = mainGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (!(cell is DataGridViewButtonCell))
                cell.Style = khakiStyle.Clone();
        }

        public void SelectPlayersFromDb()
        {
            isManuallyAdded = false;
            mainGrid.Rows.Clear();

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
            try {
                print($"PRESSED BITTON ROW INDEX: {e.RowIndex}");
                switch (mainGrid.Columns[e.ColumnIndex].Name) {
                    case "id":     ShowPhoto    (e.RowIndex); break;
                    case "photo":  SelectPhoto  (e.RowIndex); break;
                    case "save":   SetPlayer    (e.RowIndex); break;
                    case "delete": DeletePlayer (e.RowIndex); break;
                    default:
                        break;
                }
            }
            catch (Exception exception) {
                MessageBox.Show(
                    $"ПРОИЗОШЛА НЕОЖИДАННАЯ ОШИБКА ПРИ НАЖАТИИ КНОПКИ\n" +
                    $"РАБОТА ПИЛОЖЕНИЯ НЕ БУДЕТ ОСТАНОВЛЕНА НО ВОТ ТЕКСТ ОШИБКИ:\n" +
                    $"{exception.Message}"
                    );
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

            print(MainGridRowToString(playerRow.Cells[0].RowIndex));

            if (cells[0] is string) { // id cell is "_{autoincrement}"
                print($"deleted {playerRow.Index} row");

                if (rowIndexToSelectedImage.ContainsKey(playerRow.Index))
                    rowIndexToSelectedImage.Remove(playerRow.Index);

                mainGrid.Rows.Remove(playerRow);
                playerTableAutoincerentCounter--;

                newPlayerBut.Enabled = true;
            }

            else {
                long id = Convert.ToInt64(cells[0]);

                if (DeleteEnrollmentFromAiDevice(id) || IS_DEBUG) {
                    Db.DeletePlayerWhomIdEquals(id);
                    mainGrid.Rows.Remove(playerRow);
                }
                else
                    MessageBox.Show(
                        "ЗАПИСЬ НЕ БЫЛА УДАЛЕНА\n" +
                        "- Проверьте подключение к устройству");
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

            if (changedCells.Length == 0) {
                MessageBox.Show("НЕЧЕГО СОХРАНЯТЬ");
                return;
            }

            long rowPlayerId = Convert.ToInt64(row.Cells[1].Value);

            foreach (DataGridViewCell cell in changedCells) {
                string cellColumnName = mainGrid.Columns[cell.ColumnIndex].Name;
                // TODO:
                // - cant change PlayerId <?> is it really of any need
                switch (cellColumnName) {
                    case "id": 
                        AddNewPlayer(changedCells, rowIndex);
                        return; // cease loop because nothing will be left to set
                    case "photo":
                        UpdatePlayerPhoto(rowIndex);
                        break;
                    default:
                        Db.UpdatePlayerIntData(
                            rowPlayerId,
                            Convert.ToInt64(cell.Value),
                            columnNameToDbColumnName[cellColumnName]
                            );
                        cell.Style = defaultStyle.Clone();
                        break;
                }
            }
        }

        private void AddNewPlayer(DataGridViewCell[] changedCells, int rowIndex)
        {
            if (!rowIndexToSelectedImage.ContainsKey(rowIndex) && 
                !SelectPhoto(rowIndex)) {
                MessageBox.Show(
                    "ФОТО НЕ БЫЛО ВЫБРАНО И ПОЭТОМУ ИГРОК НЕ МОЖЕТ БЫТЬ ДОБАВЛЕН\n" +
                    "ВЫБЕРИТЕ ФОТО ДЛЯ НЕГО И ПОПЫТАЙТЕСЬ ЕЩЕ РАЗ");
                return;
            }
            DbPlayer dbPlayer = 
                DbPlayer.FromArray(
                    changedCells
                    .Select(c => c.Value).ToArray());

            if (SetEnrollmentToAiDevice(
                rowIndexToSelectedImage[rowIndex], 
                Convert.ToInt32(dbPlayer.PlayerIdInt))
                ) {
                Db.InsertPlayer(dbPlayer);
                changedCells[0].Value = dbPlayer.Id;
                foreach (DataGridViewCell cell in changedCells)
                    cell.Style = defaultStyle.Clone();

                newPlayerBut.Enabled = true;
            }
            else
                MessageBox.Show(
                    "НЕ УДАЛОСЬ ОТПРАВИТЬ ФОТО В УСТРОЙСТВО\n" +
                    "ПРОВЕРЬТЕ ПОДКЛЮЧЕНИЕ ИЛИ ПОМЕНЯЙТЕ ФОТО И ПОПРОБУЙТЕ ЕЩЕ РАЗ");
        }

        private bool SetEnrollmentToAiDevice(byte[] photoBytes, int enrollId)
        {
            IntPtr ptrIndexFacePhoto = Marshal.AllocHGlobal(photoBytes.Length);
            Marshal.Copy(photoBytes, 0, ptrIndexFacePhoto, photoBytes.Length);

            return faceForm.PerformOperation(() => axFP_CLOCK
                .SetEnrollPhotoCS(
                    machineNumber,
                    enrollId,
                    photoBytes.Length,
                    ptrIndexFacePhoto
                    )) || IS_DEBUG;
        }

        private void UpdatePlayerPhoto(int rowIndex)
        {
            if (!rowIndexToSelectedImage.ContainsKey(rowIndex)) { // unreachable though
                MessageBox.Show(
                    "ФОТО НЕ БЫЛО ВЫБРАНО И ПОЭТОМУ ОНО НЕ МОЖЕТ БЫТЬ СОХРАНЕНО\n" +
                    "ВЫБЕРИТЕ ФОТО И ПОПЫТАЙТЕСЬ ЕЩЕ РАЗ");
                return;
            }

            if (SetEnrollmentToAiDevice(
                rowIndexToSelectedImage[rowIndex],
                Convert.ToInt32(mainGrid.Rows[rowIndex].Cells["player_id"].Value)
                ))
                mainGrid.Rows[rowIndex].Cells["photo"].Style = defaultStyle;
            else
                MessageBox.Show(
                    "НЕ УДАЛОСЬ ОТПРАВИТЬ ФОТО В УСТРОЙСТВО\n" +
                    "ПРОВЕРЬТЕ ПОДКЛЮЧЕНИЕ ИЛИ ПОМЕНЯЙТЕ ФОТО И ПОПРОБУЙТЕ ЕЩЕ РАЗ");
        }

        #endregion SetPlayer

        #region SelectPhoto_AND_ShowPhoto

        private Dictionary<int, byte[]> rowIndexToSelectedImage { get; set; } = new Dictionary<int, byte[]>();

        private byte[] SelectPhotoBytes()
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return null;

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
                return null;
            }

            return photoBytes;
        }

        private bool SelectPhoto(int rowIndex)
        {
            byte[] photoBytes = SelectPhotoBytes();
            if (photoBytes == null)
                return false;

            mainGrid.Rows[rowIndex].Cells[5].Style = khakiStyle.Clone();
            rowIndexToSelectedImage[rowIndex] = photoBytes;

            return true;
        }

        private PhotoForm photoForm { get; set; }

        private void ShowPhoto(int rowIndex)
        {
            //print(string.Join("|", rowIndexToSelectedImage.Keys.Select(k => k.ToString())) + " KEYS");
            if (Utils.IsFormAlive(photoForm))
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
