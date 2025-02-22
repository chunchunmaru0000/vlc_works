using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UsbRelayNet.RelayLib;
// https://github.com/riuson/usb-relay-hid

namespace vlc_works
{
	public static class RelayChecker
	{
		private static AccountingForm AccountingForm { get; set; }
		private static Dictionary<int, Label> ChannelLabels { get; set; }

		public static Relay SelectedRelay { get; set; }
		private static bool IsOpen { get => SelectedRelay != null && SelectedRelay.IsOpened; }

		public static void Constructor(AccountingForm accountingForm)
		{
			AccountingForm = accountingForm;
			ChannelLabels = new Dictionary<int, Label>
			{
				{ 1, AccountingForm.relayCh1 }, // camera UP
				{ 2, AccountingForm.relayCh2 }, // camera DOWN
				{ 3, AccountingForm.relayCh3 }, // coins light on
				{ 4, AccountingForm.relayCh4 }, // apparat light on
			};
		}

		public static void Close()
		{
			if (IsOpen)
				SelectedRelay.Dispose();
		}

		private static Color BoolToColor(bool b) => b ? Color.LightGreen : Color.LightCoral;

		public static void Transmit(int channel, bool state)
		{
			if (!IsOpen) 
				return;

			SelectedRelay.WriteChannel(channel, state);

			AccountingForm.Invoke(new Action(() => 
			ChannelLabels[channel].BackColor = BoolToColor(state)));
		}
	}
}
