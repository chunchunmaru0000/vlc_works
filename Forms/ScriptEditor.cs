using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vlc_works
{
    public partial class ScriptEditor: Form
    {
        ClientForm clientForm { get; set; }
        AccountingForm accountingForm { get; set; }

        public ScriptEditor(AccountingForm accountingForm, ClientForm clientForm)
        {
            InitializeComponent();

            this.clientForm = clientForm;
            this.accountingForm = accountingForm;

            InitScript(clientForm.gameScripts);
        }

        private Dictionary<GameType, string> GameTypeToLetter { get; } = new Dictionary<GameType, string>() {
            { GameType.Guard, "C" },
            { GameType.Painting, "K" },
            { GameType.Mario, "M" },
        };

        public void InitScript(GameScript[] gameScripts)
        {
            foreach (GameScript script in gameScripts) {
                DataGridViewRow row = new DataGridViewRow() { Height = 32 };

                DataGridViewButtonCell typeCell = new DataGridViewButtonCell() {
                    Value = GameTypeToLetter[script.GameType],
                };
                //typeCell.Items.AddRange(new string[] { "C", "K", "M" });

                row.Cells.AddRange(new DataGridViewCell[] {
                    typeCell,
                    new DataGridViewTextBoxCell() { Value = script.Prize },
                    new DataGridViewTextBoxCell() { Value = script.Lvl },
                    new DataGridViewTextBoxCell() { Value = script.Price },
                });

                scriptEditorGrid.Rows.Add(row);
            }
        }
    }
}
