using System;
using System.Threading;
using System.Windows.Forms;

namespace vlc_works
{
    public partial class DebugForm: Form
    {
        private AccountingForm accountingForm { get; set; }
        private ClientForm clientForm { get; set; }

        private void print(object obj)
        {
            string str = obj == null ? "" : obj.ToString();

            Console.WriteLine(str);
        }

        public DebugForm(AccountingForm accountingForm)
        {
            InitializeComponent();

            this.accountingForm = accountingForm;
            Owner = accountingForm;

            this.clientForm = accountingForm.clientForm;
        }

        private void winBut_Click(object sender, EventArgs e)
        {
            print($"[[[ BEGIN DEBUG WIN ]]]");
            clientForm.Invoke(new Action(() => {
                DbCurrentRecord.SetPricePrizeLvl(
                    clientForm.gameInfo.CurrentScript.Price,
                    clientForm.gameInfo.CurrentScript.Prize,
                    clientForm.gameInfo.CurrentScript.Lvl,
                    clientForm.gameInfo.CurrentScript.GameType);
                VideoChecker.won = true;
                clientForm.DoDataBaseGameRecord(DEBUG: true);
            }));
            print($"[[[ _END_ DEBUG WIN ]]]");
        }

        private void loseBut_Click(object sender, EventArgs e)
        {
            print($"[[[ BEGIN DEBUG LOSE ]]]");
            clientForm.Invoke(new Action(() => {
                DbCurrentRecord.SetPricePrizeLvl(
                    clientForm.gameInfo.CurrentScript.Price,
                    clientForm.gameInfo.CurrentScript.Prize,
                    clientForm.gameInfo.CurrentScript.Lvl,
                    clientForm.gameInfo.CurrentScript.GameType);
                VideoChecker.won = false;
                clientForm.DoDataBaseGameRecord(DEBUG: true);
            }));
            print($"[[[ _END_ DEBUG LOSE ]]]");
        }

        private void vlcSkip_Click(object sender, EventArgs e)
        {
            clientForm.BeginInvoke(new Action(
                () => ThreadPool.QueueUserWorkItem(_ => 
                clientForm.vlcControl.Time = (3 * 60 + 55) * 1000)));
        }
    }
}
