using System;
using System.Windows.Forms;

namespace vlc_works015
{
    public static class App
    {
        public static ClientForm Client { get; set; }
        public static AccountingForm Accounting { get; set; }

        public static void Init(ClientForm client, AccountingForm accounting)
        {
            Client = client;
            Accounting = accounting;

            RelayOffOnLabel = Accounting.relayOffOnLabel;
            LaserOffOnLabel = Accounting.laserOnOffLabel;
            Rs232OffOnLabel = Accounting.connectedLabel;
        }

        #region LABELS
        private static Label RelayOffOnLabel { get; set; }
        private static Label LaserOffOnLabel { get; set; }
        private static Label Rs232OffOnLabel { get; set; }

        private static bool IsLabelOff(Label label) => label.Text == "OFF";

        public static void SetLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
                label.Invoke(new Action(() => label.Text = text));
            else
                label.Text = text;

            if (IsLabelOff(RelayOffOnLabel) ||
                IsLabelOff(LaserOffOnLabel) ||
                IsLabelOff(Rs232OffOnLabel)
                )
                TabloPlayer.Write(TabloText.NotWorking);
            else if (TabloPlayer.CurrentPlaying == TabloText.NotWorking)
                TabloPlayer.Write(TabloText.GuideToStart);
        }
        #endregion LABELS
    }
}
