using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vlc_works
{
    public partial class DebugForm: Form
    {
        private AccountingForm accountingForm { get; set; }
        private ClientForm clientForm { get; set; }

        public DebugForm(AccountingForm accountingForm)
        {
            InitializeComponent();

            this.accountingForm = accountingForm;
            Owner = accountingForm;

            this.clientForm = accountingForm.clientForm;
        }

        private void winBut_Click(object sender, EventArgs e)
        {
            clientForm.Invoke(new Action(() => {
                DbCurrentRecord.SetPricePrizeLvl(
                    clientForm.gameInfo.CurrentScript.Price,
                    clientForm.gameInfo.CurrentScript.Prize,
                    clientForm.gameInfo.CurrentScript.Lvl,
                    clientForm.gameInfo.CurrentScript.GameType);
                VideoChecker.won = true;
                clientForm.DoDataBaseGameRecord(DEBUG: true);
            }));
        }

        private void loseBut_Click(object sender, EventArgs e)
        {
            clientForm.Invoke(new Action(() => {
                DbCurrentRecord.SetPricePrizeLvl(
                    clientForm.gameInfo.CurrentScript.Price,
                    clientForm.gameInfo.CurrentScript.Prize,
                    clientForm.gameInfo.CurrentScript.Lvl,
                    clientForm.gameInfo.CurrentScript.GameType);
                VideoChecker.won = false;
                clientForm.DoDataBaseGameRecord(DEBUG: true);
            }));
        }
    }
}
