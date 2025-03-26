using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace vlc_works
{
    public partial class ScriptEditor: Form
    {
        private ClientForm clientForm { get; set; }
        private AccountingForm accountingForm { get; set; }
        private Dictionary<DataGridViewRow, GameScript> rowToScript { get; set; } = new Dictionary<DataGridViewRow, GameScript>();
        private bool IsInit { get; set; } = true;
        public GameMode tableMode { get; set; } = GameMode.LOW;
        public void SetGameModeAndScript(GameMode mode, GameScript[] gameScripts)
        {
            tableMode = mode;
            InitScript(gameScripts);
        }

        public ScriptEditor(AccountingForm accountingForm, ClientForm clientForm)
        {
            InitializeComponent();

            this.clientForm = clientForm;
            this.accountingForm = accountingForm;
            Owner = accountingForm;

            gamesCounterLabel.Text = clientForm.gameInfo.GamesCounter.ToString();
            gamesCounterBox.Text = clientForm.gameInfo.GamesCounterCheck.ToString();
            midBorder.Text = clientForm.gameInfo.ModeBalanceBorders[GameMode.MID].ToString();
            lowBorder.Text = clientForm.gameInfo.ModeBalanceBorders[GameMode.LOW].ToString();

            styles[GameMode.LOW][GS.DEFAULT] = scriptEditorGrid.DefaultCellStyle.Clone();
            InitScript(clientForm.gameInfo.GameModeScripts);
        }

        private Dictionary<GameType, string> GameTypeToLetter { get; } = new Dictionary<GameType, string>() {
            { GameType.Guard, "C" },
            { GameType.Painting, "K" },
            { GameType.Mario, "M" },
        };

        #region CELLS_STYLES
        private DataGridViewCellStyle CurStyle(GS gc) => styles[tableMode][gc];

        private enum GS // GameStyle
        {
            DEFAULT,
            CHANGED,
            ERROR,

            PASSED,
            CURRENT,
            FUTURE,
        }

        private Dictionary<GameMode, Dictionary<GS, DataGridViewCellStyle>> styles { get; set; } = new Dictionary<GameMode, Dictionary<GS, DataGridViewCellStyle>>() {
            {
                GameMode.LOW, new Dictionary<GS, DataGridViewCellStyle>() {
                    { GS.DEFAULT, new DataGridViewCellStyle() },
                    { GS.CHANGED, new DataGridViewCellStyle() {
                        BackColor = Color.GreenYellow,
                        SelectionBackColor = Color.Olive,
                    } },
                    { GS.ERROR, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(244, 67, 54),
                        SelectionBackColor = Color.FromArgb(198, 40, 40),
                    } },
                    { GS.PASSED, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(34, 85, 34),
                        SelectionBackColor = Color.FromArgb(44, 115, 44),
                    } },
                    { GS.CURRENT, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(200, 230, 201),
                        SelectionBackColor = Color.FromArgb(165, 214, 167),
                    } },
                    { GS.FUTURE, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(76, 175, 80),
                        SelectionBackColor = Color.FromArgb(102, 211, 106),
                    } },
                }
            },
            {
                GameMode.MID, new Dictionary<GS, DataGridViewCellStyle>() {
                    { GS.DEFAULT, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(255, 224, 130),
                        SelectionBackColor = Color.FromArgb(255, 204, 102),
                    } },
                    { GS.CHANGED, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(255, 235, 59),
                        SelectionBackColor = Color.FromArgb(253, 216, 53),
                    } },
                    { GS.ERROR, new DataGridViewCellStyle() {
                        BackColor = Color.OrangeRed,
                        SelectionBackColor = Color.FromArgb(198, 40, 40),
                    } },
                    { GS.PASSED, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(155, 123, 27),
                        SelectionBackColor = Color.FromArgb(255, 160, 0),
                    } },
                    { GS.CURRENT, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(255, 224, 130),
                        SelectionBackColor = Color.FromArgb(255, 204, 102),
                    } },
                    { GS.FUTURE, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(255, 193, 7),
                        SelectionBackColor = Color.FromArgb(255, 179, 0),
                    } },
                }
            },
            {
                GameMode.HIGH, new Dictionary<GS, DataGridViewCellStyle>() {
                    { GS.DEFAULT, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(255, 235, 208, 205),
                        SelectionBackColor = Color.DarkRed,
                    } },
                    { GS.CHANGED, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(239, 83, 80),
                        SelectionBackColor = Color.FromArgb(229, 57, 53),
                    } },
                    { GS.ERROR, new DataGridViewCellStyle() {
                        BackColor = Color.Aquamarine,
                        SelectionBackColor = Color.BlueViolet,
                    } },
                    { GS.PASSED, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(143, 10, 49),
                        SelectionBackColor = Color.FromArgb(96, 7, 26),
                    } },
                    { GS.CURRENT, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(255, 235, 208, 205),
                        SelectionBackColor = Color.DarkRed,
                    } },
                    { GS.FUTURE, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(255, 23, 68),
                        SelectionBackColor = Color.FromArgb(213, 0, 50),
                    } },
                }
            },
        };

        #endregion

        #region INIT

        private DataGridViewCellStyle indexRowStyle(int index) => (
            tableMode == clientForm.gameInfo.GameMode
            ? clientForm.gameInfo.GameIndex == index
                ? CurStyle(GS.CURRENT)
                : clientForm.gameInfo.GameIndex < index
                    ? CurStyle(GS.FUTURE)
                    : CurStyle(GS.PASSED)
            : CurStyle(GS.FUTURE)
            ).Clone();

        private void InitScript(GameScript[] gameScripts)
        {
            scriptEditorGrid.Rows.Clear(); // for dynamic
            rowToScript.Clear();

            int currentScriptRow = -1;
            for (int i = 0; i < gameScripts.Length; i++) {
                GameScript script = gameScripts[i];
                DataGridViewRow row = new DataGridViewRow() { Height = 32 };
                DataGridViewButtonCell typeCell = new DataGridViewButtonCell() {
                    Value = GameTypeToLetter[script.GameType],
                    Style = CurStyle(GS.DEFAULT)
                };

                DataGridViewCellStyle cellStyle = indexRowStyle(i);
                if (clientForm.gameInfo.GameIndex == i)
                    currentScriptRow = i;

                row.Cells.AddRange(new DataGridViewCell[] {
                    typeCell,
                    new DataGridViewTextBoxCell() { Value = script.Lvl, Style = cellStyle.Clone() },
                    new DataGridViewTextBoxCell() { Value = script.Prize, Style = cellStyle.Clone() },
                    new DataGridViewTextBoxCell() { Value = script.Price, Style = cellStyle.Clone() },
                });

                scriptEditorGrid.Rows.Add(row);
                rowToScript[row] = script;
            }

            if (currentScriptRow != -1)
                scriptEditorGrid.FirstDisplayedScrollingRowIndex = currentScriptRow;

            IsInit = false;
        }

        #endregion INIT

        #region LOGIC

        private void saveBut_Click(object sender, EventArgs e)
        {
            foreach(KeyValuePair<DataGridViewRow, GameScript> rowScipt in rowToScript)
                if (rowScipt.Key.Cells.Cast<DataGridViewCell>()
                    .Any(c => c.Style.BackColor == CurStyle(GS.CHANGED).BackColor)
                    &&
                    !rowScipt.Key.Cells.Cast<DataGridViewCell>()
                    .Any(c => c.Style.BackColor == CurStyle(GS.ERROR).BackColor)
                    )
                    SaveChanges(rowScipt);

            int[] values = clientForm.gameInfo.TryParseValues(new string[] {
                gamesCounterBox.Text, midBorder.Text, lowBorder.Text
            });
            if (values.Length != 0)
                clientForm.gameInfo.SetAndSaveBalanceValues(values);
        }

        private void SaveChanges(KeyValuePair<DataGridViewRow, GameScript> rowScipt)
        {
            long.TryParse(rowScipt.Key.Cells[2].Value.ToString(), out long prize);
            long.TryParse(rowScipt.Key.Cells[3].Value.ToString(), out long price);

            rowScipt.Value.Prize = prize;
            rowScipt.Value.Price = price;

            foreach (DataGridViewCell cell in rowScipt.Key.Cells)
                cell.Style = indexRowStyle(cell.RowIndex);
        }

        private void scriptEditorGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IsInit)
                return;

            DataGridViewCell cell = scriptEditorGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            cell.Style = (
                long.TryParse(cell.Value.ToString(), out long some)
                ? CurStyle(GS.CHANGED)
                : CurStyle(GS.ERROR)
                ).Clone();
        }

        #endregion LOGIC

        #region GAME_MODE_BUTS

        private void SetTableModeAndInitScript(GameMode modeSet)
        {
            tableMode = modeSet;
            IsInit = true;
            InitScript(clientForm.gameInfo.ModeScripts[tableMode]);
        }

        private void easyBut_Click(object sender, EventArgs e) =>
            SetTableModeAndInitScript(GameMode.LOW);

        private void mediumBut_Click(object sender, EventArgs e) =>
            SetTableModeAndInitScript(GameMode.MID);

        private void hardBut_Click(object sender, EventArgs e) =>
            SetTableModeAndInitScript(GameMode.HIGH);

        #endregion GAME_MODE_BUTS

        private void resetGamesCounterBut_Click(object sender, EventArgs e)
        {
            clientForm.gameInfo.ResetGamesCounter();
        }
    }
}
