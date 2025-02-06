using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace vlc_works
{
	public static class COMPort
	{
		#region PRIVATE
		private static byte[] fifeCoins = new byte[] { 0x30, 0x31, 0x32, 0x33, 0x00, 0x0A };
		private static List<byte> notParsed = new List<byte>();
		#endregion
		#region PUBLIC 
		public static SerialPort port;
		public static string portName; // com port str like COM3
		public static AccountingForm accountingForm;
		#endregion

		public static void MoneyOut(AccountingForm accountingForm = null)
		{
			if (port == null || !port.IsOpen)
				return;
			if (COMPort.accountingForm == null)
				if (accountingForm == null)
				{
					MessageBox.Show("accountingForm = null and COMPort.accountingForm = null");
					Environment.Exit(0);
				}
				else
					COMPort.accountingForm = accountingForm;

			long times = accountingForm.SelectedAward / 50;

			new Thread(() =>
			{
				for (int i = 0; i < times; i++)
				{
					port.Write(fifeCoins, 0, fifeCoins.Length);
					Thread.Sleep(1000);
				}
			}).Start();
		}

		public static void TryConnectPort(string com, AccountingForm accountingForm)
		{
			portName = com;
			if (port != null && port.IsOpen)
			{
				port.Close();
				port.Dispose();
			}
			try
			{
				COMPort.accountingForm = accountingForm;

				port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
				port.DataReceived += DataRecieved;
				port.Open();

				accountingForm.Invoke(new Action(() =>
				{
					accountingForm.connectedLabel.Text = "Подключен";
				}));
			}
			catch
			{
				accountingForm.Invoke(new Action(() =>
				{
					accountingForm.connectedLabel.Text = "Не подключилось";
				}));
			}
		}

		private static void DataRecieved(object sender, SerialDataReceivedEventArgs e)
		{
			byte[] bytes = new byte[port.BytesToRead];
			int res = port.Read(bytes, 0, port.BytesToRead);
			notParsed.AddRange(bytes);

			TryParseCommand();
		}

		private static void TryParseCommand()
		{

		}
	}
}
