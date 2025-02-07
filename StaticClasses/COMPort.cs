using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace vlc_works
{
	public static class COMPort
	{
		#region PRIVATE
		private static List<byte> notParsed = new List<byte>();
		private static Dictionary<string, byte[]> cmd = new Dictionary<string, byte[]>() // command
		{
			{ "Output/ no output 5V", new byte[]      { 0xF2, 0x01, 0x30, 0x31, 0xF3 } },
			{ "Coins out", new byte[]                 { 0xF2, 0x01, 0x31, 0x30, 0xF3 } },
			{ "Check income", new byte[]              { 0xF2, 0x01, 0x32, 0x33, 0xF3 } },
			{ "Reset counter", new byte[]             { 0xF2, 0x01, 0x35, 0x34, 0xF3 } },
		};
		private static Dictionary<string, byte[]> rsp = new Dictionary<string, byte[]>() // response
		{
			{ "Output/ no output 5V", new byte[]      { 0xF2, 0x02, 0x30, 0x00, 0x32, 0xF3 } },
			//                                        { 0xF2, 0x09, 0x32, (0x03, 0x00, 0x00, 0x00), (0x03, 0x00, 0x00, 0x00), 0x38, 0xF3 }
			{ "Coins out", new byte[]                 { 0xF2, 0x02, 0x31, 0x00, 0x33, 0xF3 } },
			{ "Reset counter", new byte[]             { 0xF2, 0x02, 0x35, 0x00, 0x37, 0xF3 } },
			{ "Received coin", new byte[]             { 0xF2, 0x03, 0x01, 0x01, 0x00, 0x03, 0xF3 } }
		};
		private static Func<IEnumerable<byte>, string> batos = ba => string.Join(" ", ba.Select(b => $"{b:X2}"));
		#endregion
		#region PUBLIC 
		public static SerialPort port;
		public static string portName; // com port str like COM3
		public static AccountingForm accountingForm;
		#endregion


		#region PUBLIC_METHODS
		public static void Execute(string command) { 
			if (cmd.ContainsKey(command))
			{
				byte[] cmdBytes = cmd[command];
				port.Write(cmdBytes, 0, cmdBytes.Length); 
			}
		}

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

			long times = accountingForm.SelectedAward / (AccountingForm.oneCommandCoins * AccountingForm.oneCoinShekels);

			new Thread(() =>
			{
				for (int i = 0; i < times; i++)
				{
					Execute("Coins out");
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

				accountingForm.Invoke(new Action(() => accountingForm.connectedLabel.Text = "Подключен"));
			}
			catch
			{
				accountingForm.Invoke(new Action(() => accountingForm.connectedLabel.Text = "Не подключилось"));
			}
		}
		#endregion PUBLIC_METHODS
		#region STATIC_METHODS
		private static void DataRecieved(object sender, SerialDataReceivedEventArgs e)
		{
			byte[] bytes = new byte[port.BytesToRead];
			int res = port.Read(bytes, 0, port.BytesToRead);
			notParsed.AddRange(bytes);

			Console.WriteLine($"    RECEIVED: {batos(bytes)}");
			TryParseCommand();
		}

		private static void TryParseCommand()
		{
			Console.WriteLine($"TRY TO PARSE: {batos(notParsed)}");
			if (notParsed.Count < 6)
				return;
			else if (notParsed[1] == 0x09)
				ParseIncome();
			else if (notParsed[1] == 0x02)
				ParseResponse();
			else if (notParsed[1] == 0x03)
				ParseCoinIn();
			else
				HandleUnknownInput(0);
		}

		private static void HandleUnknownInput(int index)
		{
			Console.WriteLine($"UNKNOWN COM PORT INPUT {notParsed[index]:X2}");
			notParsed.RemoveAt(index);
			TryParseCommand();
		}

		private static void ParseIncome()
		{
			if (notParsed.Count < 13)
				return;

			byte[] firstCounterBytes = notParsed.Skip(3).Take(4).ToArray();
			// still dont know for what stands second counter
			// byte[] secondCounterBytes = notParsed.Skip(7).Take(4).ToArray();
			notParsed.RemoveRange(0, 13);

			int firstCounter = BitConverter.ToInt32(firstCounterBytes, 0);
			Console.WriteLine($"HAVE {firstCounter} COINS IN STOCK");

			if (accountingForm != null)
				accountingForm.SetCoinsInStock(firstCounter);
		}

		private static bool isCommand(byte[] command)
		{
			bool res = false;
			foreach (byte[] ba in rsp.Values)
			{
				bool resForNow = true;
				for (int i = 0; i < ba.Length; i++)
					if (command[i] != ba[i])
					{
						resForNow = false;
						break;
					}
				if (resForNow)
				{
					res = true;
					break;
				}
			}
			return res;
		}

		private static bool isReceivedCoin() => 
			notParsed.Count < 7 ? 
				false : 
				notParsed.Take(7).ToArray()
					.SequenceEqual(rsp["Received coin"]);

		private static void ParseResponse()
		{
			bool res = isCommand(notParsed.ToArray());

			Console.WriteLine(res);
			if (res)
			{
				int rspLen = 6;
				Console.WriteLine($"PARSED: {batos(notParsed.Take(rspLen))}");
				notParsed.RemoveRange(0, rspLen); // remove all command
			}
			else
			{
				notParsed.RemoveAt(0);
				TryParseCommand();
			}
		}

		private static void ParseCoinIn()
		{
			if (isReceivedCoin())
			{
				Console.WriteLine("1 COIN RECEIVED");
				accountingForm.IncBalance();
				notParsed.RemoveRange(0, rsp["Received coin"].Length);
			}
		}
		#endregion STATIC_METHODS
	}
}
