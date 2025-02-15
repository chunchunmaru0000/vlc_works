using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.IO;

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
		private static Func<long, long> ShekelsToTimes = shekels => shekels / (AccountingForm.oneCommandCoins * AccountingForm.oneCoinShekels);
		private static void print(object value)
		{
			string str = value.ToString();

			Console.WriteLine(str);
			//File.AppendAllText("test.txt", $"{DateTimeOffset.Now}: {str}\n");
		}
		#endregion
		#region PUBLIC 
		public static SerialPort port;
		public static string portName; // com port str like COM3
		public static AccountingForm accountingForm;
		#endregion


		#region PUBLIC_METHODS
		public static void Execute(string command) {
			print($"TRY TO EXECUTE COMMAND: [{command}]");
			if (cmd.ContainsKey(command))
			{
				byte[] cmdBytes = cmd[command];
				port.Write(cmdBytes, 0, cmdBytes.Length);
				print($"EXECUTED COMMAND: [{command}]");
			}
			else
				print($"COMMAND NOT FOUND: [{command}]");
		}

		public static void MoneyOut(long shekels, AccountingForm accountingForm = null)
		{
			if (port == null || !port.IsOpen)
			{
				print($"TRY TO MONEY OUT WHILE PORT IS NOT EVEN OPEN");
				return;
			}
			if (COMPort.accountingForm == null)
				if (accountingForm == null)
				{
					MessageBox.Show("[accountingForm = null] AND [COMPort.accountingForm = null], NEED TO INIT [accountingForm] HERE");
					return;
				}
				else
					COMPort.accountingForm = accountingForm;

			long times = ShekelsToTimes(shekels);
			print(
				$"times = {shekels} / ({AccountingForm.oneCommandCoins} * {AccountingForm.oneCoinShekels}) =" +
				$" {shekels} / ({AccountingForm.oneCommandCoins * AccountingForm.oneCoinShekels}) =" +
				$" {shekels / (AccountingForm.oneCommandCoins * AccountingForm.oneCoinShekels)}");
			print($"{times} TIMES TO EXECUTE OUT [{AccountingForm.oneCommandCoins}] COINS");
			new Thread(() =>
			{
				for (int i = 0; i < times; i++)
				{
					Execute("Coins out");
					Thread.Sleep(1500);
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
				print($"PORT IS OPENED: {port.IsOpen}");

				accountingForm.Invoke(new Action(() => accountingForm.connectedLabel.Text = "Подключен"));
				Execute("Check income");
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

			print($"    RECEIVED: {batos(bytes)}");
			TryParseCommand();
		}

		private static void TryParseCommand()
		{
			print($"TRY TO PARSE: {batos(notParsed)}");
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
			if (index >= notParsed.Count)
				return;
			print($"UNKNOWN COM PORT INPUT {notParsed[index]:X2}");
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
			print($"HAVE {firstCounter} COINS IN STOCK");

			if (accountingForm != null)
				accountingForm.IncCoinsInStock(firstCounter);

			TryParseCommand();
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

		private static void ParseResponse()
		{
			bool res = isCommand(notParsed.ToArray());

			print(res);
			if (res)
			{
				int rspLen = 6;
				print($"PARSED: {batos(notParsed.Take(rspLen))}");
				notParsed.RemoveRange(0, rspLen); // remove all command
			}
			else
				notParsed.RemoveAt(0);
			TryParseCommand();
		}

		private static void ParseCoinIn()
		{
			if (notParsed.Count < 7)
			{
				print($"STILL {notParsed.Count} < 7 TOO LEES BYTES TO PARSE COIN IN");
				return;
			}
			else if (notParsed.Take(7).ToArray().SequenceEqual(rsp["Received coin"]))
			{
				print("1 COIN RECEIVED");
				accountingForm.IncCoinsInStock(AccountingForm.oneCommandCoins);
				notParsed.RemoveRange(0, rsp["Received coin"].Length);
			}
			else
			{
				print($"HOW THIS COMMAND HAVE SECOND BYTE 0x03 AND LEN 7 AND STILL NOT [COIN IN]");
				HandleUnknownInput(0);
			}

			TryParseCommand();
		}
		#endregion STATIC_METHODS
	}
}
