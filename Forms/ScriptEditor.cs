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
        private bool IsInit { get; set; } = true;

        public ScriptEditor(AccountingForm accountingForm, ClientForm clientForm)
        {
            InitializeComponent();

            this.clientForm = clientForm;
            this.accountingForm = accountingForm;

            defaultStyle = scriptEditorGrid.DefaultCellStyle.Clone();
            InitScript(clientForm.gameScripts);
            IsInit = false;
        }

        private Dictionary<GameType, string> GameTypeToLetter { get; } = new Dictionary<GameType, string>() {
            { GameType.Guard, "C" },
            { GameType.Painting, "K" },
            { GameType.Mario, "M" },
        };

        private DataGridViewCellStyle defaultStyle { get; set; }
        private DataGridViewCellStyle changedStyle { get; } = new DataGridViewCellStyle()
        {
            BackColor = Color.GreenYellow,
            SelectionBackColor = Color.Olive,
            ForeColor = Color.Black,
            SelectionForeColor = Color.White
        };

        public void InitScript(GameScript[] gameScripts)
        {
            foreach (GameScript script in gameScripts) {
                DataGridViewRow row = new DataGridViewRow() { Height = 32 };

                DataGridViewButtonCell typeCell = new DataGridViewButtonCell() {
                    Value = GameTypeToLetter[script.GameType],
                    Style = defaultStyle
                };
                //typeCell.Items.AddRange(new string[] { "C", "K", "M" });

                row.Cells.AddRange(new DataGridViewCell[] {
                    typeCell,
                    new DataGridViewTextBoxCell() { Value = script.Lvl },
                    new DataGridViewTextBoxCell() { Value = script.Prize },
                    new DataGridViewTextBoxCell() { Value = script.Price },
                });

                scriptEditorGrid.Rows.Add(row);
            }
        }

        private void saveBut_Click(object sender, EventArgs e)
        {
            
        }

        private void scriptEditorGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IsInit)
                return;

            scriptEditorGrid
            .Rows[e.RowIndex]
            .Cells[e.ColumnIndex]
            .Style = changedStyle.Clone();
        }
    }
}
