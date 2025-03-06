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
        private GameMode tableMode { get; set; } = GameMode.ALL;

        public ScriptEditor(AccountingForm accountingForm, ClientForm clientForm)
        {
            InitializeComponent();

            this.clientForm = clientForm;

            this.accountingForm = accountingForm;
            Owner = accountingForm;

            styles[GameMode.ALL][GS.DEFAULT] = scriptEditorGrid.DefaultCellStyle.Clone();
            InitScript(clientForm.gameInfo.GameScripts);
            IsInit = false;
        }

        private Dictionary<GameType, string> GameTypeToLetter { get; } = new Dictionary<GameType, string>() {
            { GameType.Guard, "C" },
            { GameType.Painting, "K" },
            { GameType.Mario, "M" },
        };

        #region CELLS_STYLES

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
                GameMode.ALL, new Dictionary<GS, DataGridViewCellStyle>() {
                    { GS.DEFAULT, new DataGridViewCellStyle() },
                    { GS.CHANGED, new DataGridViewCellStyle() {
                            BackColor = Color.GreenYellow,
                            SelectionBackColor = Color.Olive,
                            ForeColor = Color.Black,
                            SelectionForeColor = Color.White,
                    } },
                    { GS.ERROR, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(244, 67, 54),
                        ForeColor = Color.Black,
                        SelectionBackColor = Color.FromArgb(198, 40, 40),
                        SelectionForeColor = Color.White,
                    } },
                    { GS.PASSED, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(34, 85, 34),
                        ForeColor = Color.Black,
                        SelectionBackColor = Color.FromArgb(44, 115, 44),
                        SelectionForeColor = Color.White,
                    } },
                    { GS.CURRENT, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(200, 230, 201),
                        ForeColor = Color.Black,
                        SelectionBackColor = Color.FromArgb(165, 214, 167),
                        SelectionForeColor = Color.White,
                    } },
                    { GS.FUTURE, new DataGridViewCellStyle() {
                        BackColor = Color.FromArgb(76, 175, 80),
                        ForeColor = Color.Black,
                        SelectionBackColor = Color.FromArgb(102, 211, 106),
                        SelectionForeColor = Color.White,
                    } },
                }
            },
            {
                GameMode.MEDIUM, new Dictionary<GS, DataGridViewCellStyle>() {

                }
            },
            {
                GameMode.HARD, new Dictionary<GS, DataGridViewCellStyle>() {

                }
            },
        };

        private DataGridViewCellStyle CurStyle(GS gc) => styles[tableMode][gc];

        #endregion

        private DataGridViewCellStyle indexRowStyle(int index) => (
            clientForm.gameIndex == index
            ? CurStyle(GS.CURRENT)
            : clientForm.gameIndex < index
                ? CurStyle(GS.FUTURE)
                : CurStyle(GS.PASSED)
            ).Clone();

        public void InitScript(GameScript[] gameScripts)
        {
            scriptEditorGrid.Rows.Clear(); // for dynamic
            rowToScript.Clear();

            for (int i = 0; i < gameScripts.Length; i++) {
                GameScript script = gameScripts[i];
                DataGridViewRow row = new DataGridViewRow() { Height = 32 };
                DataGridViewButtonCell typeCell = new DataGridViewButtonCell() {
                    Value = GameTypeToLetter[script.GameType],
                    Style = CurStyle(GS.DEFAULT)
                };

                DataGridViewCellStyle cellStyle = indexRowStyle(i);

                row.Cells.AddRange(new DataGridViewCell[] {
                    typeCell,
                    new DataGridViewTextBoxCell() { Value = script.Lvl, Style = cellStyle.Clone() },
                    new DataGridViewTextBoxCell() { Value = script.Prize, Style = cellStyle.Clone() },
                    new DataGridViewTextBoxCell() { Value = script.Price, Style = cellStyle.Clone() },
                });

                scriptEditorGrid.Rows.Add(row);
                rowToScript[row] = script;
            }
        }

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
        private void easyBut_Click(object sender, EventArgs e)
        {
            GameMode mode = tableMode;
            tableMode = GameMode.ALL;
            IsInit = true;

            switch (mode) {
                case GameMode.ALL: 
                    break;
                case GameMode.MEDIUM: 
                    InitScript(clientForm.gameInfo.GameScripts); break;
                case GameMode.HARD: 
                    InitScript(clientForm.gameInfo.GameScripts); break;
                default: break;
            }
        }

        private void mediumBut_Click(object sender, EventArgs e)
        {
            GameMode mode = tableMode;
            tableMode = GameMode.MEDIUM;
            IsInit = true;

            switch (mode) {
                case GameMode.ALL: 
                    InitScript(clientForm.gameInfo.ModeScripts[tableMode]); break;
                case GameMode.MEDIUM: 
                    break;
                case GameMode.HARD: 
                    InitScript(clientForm.gameInfo.ModeScripts[tableMode]); break;
                default: break;
            }
        }

        private void hardBut_Click(object sender, EventArgs e)
        {
            GameMode mode = tableMode;
            tableMode = GameMode.HARD;
            IsInit = true;

            switch (mode) {
                case GameMode.ALL:
                    InitScript(clientForm.gameInfo.ModeScripts[tableMode]); break;
                case GameMode.MEDIUM:
                    InitScript(clientForm.gameInfo.ModeScripts[tableMode]); break;
                case GameMode.HARD: break;
                default: break;
            }
        }
        #endregion GAME_MODE_BUTS
    }
}
