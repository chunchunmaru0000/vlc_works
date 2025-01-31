using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vlc.DotNet.Forms;

namespace vlc_works
{
	public enum Stage
	{
		IDLE,

		SELECT_LANG,
		RULES,
		COST_AND_PRIZE,
		GAME,

		ERROR,
		GAME_CANT_INPUT,
		VICTORY,
	}

	public enum Langs
	{
		RUSSIAN,
		ENGLISH,
		HEBREW,
	}

	public class VLCChecker
	{
		#region VAR
		// forms
		ClientForm clientForm { get; set; }
		AccountingForm accountingForm { get; set; }
		// COM
		SerialPort port;
		public string COMPort; // com port str like COM3
		// media
		long videoGameTimeWas { get; set; } // time video game was stopped
		// vlc things
		string vlcPath { get; set; } // path to vlc executable now not in use
		Process vlcProcess { get; set; } // vlc process object
		Thread vlcCheckerThread { get; set; }
		Process lastVlcProcess { get; set; }
		string lastCommandLine { get; set; } = string.Empty;
		// constants
		const int strFrom = 3;
		const int strTo = 5;
		const string processToCheckName = "vlc";
		const string videonamestxt = "videonames.txt";
		public static readonly Dictionary<Keys, string> ktos = new Dictionary<Keys, string>()
		{
			{ Keys.D0, "0" }, { Keys.D1, "1" },
			{ Keys.D2, "2" }, { Keys.D3, "3" },
			{ Keys.D4, "4" }, { Keys.D5, "5" },
			{ Keys.D6, "6" }, { Keys.D7, "7" },
			{ Keys.D8, "8" }, { Keys.D9, "9" },
			{ Keys.Enter, "E" }
		}; // keys to string
		public static readonly Dictionary<Keys, Langs> ktol = new Dictionary<Keys, Langs>()
		{
			{ Keys.D1, Langs.HEBREW },
			{ Keys.D2, Langs.ENGLISH },
			{ Keys.D3, Langs.RUSSIAN },
		}; // select lang stage nums to lang
		// video paths
		string videoFileName { get; set; } = string.Empty; // game video path
		public Uri gameVideoUri { get; set; }
		string defeatPath { get; set; }
		public Uri errorVideoUri { get; set; }
		string idlePath { get; set; }
		public Uri idleUri { get; set; }
		string selectLangPath { get; set; }
		public Uri selectLangUri { get; set; }
		public Dictionary<Langs, Language> langs { get; set; } = new Dictionary<Langs, Language>();
		// game things
		string code { get; set; } // inputed code like 01234E
		public Langs language { get; set; } // currently selected language
		public bool blockInput { get; set; } = false; // block input althought can be done the same via stage variable
		public bool gameEnded { get; set; } = true; // also bad thing and better to do via stage
		public int errorsCount { get; set; } // how much errors inputed this game
		// some
		public static event EventHandler ProcessCommandLineChanged; // event to check vlc launch
		protected virtual void OnProcessCommandLineChanged() => ProcessCommandLineChanged?.Invoke(this, EventArgs.Empty);
		void print(object str = null)
		{
			string stroke = str == null ? "" : str.ToString();
			Console.WriteLine(stroke);
			//accountingForm.BeginInvoke(new Action(() => { accountingForm.DEBUG(stroke); }));
		}
		void DeleteInput() => clientForm.Invoke((MethodInvoker)delegate { clientForm.DeleteInput(); });
		#endregion
		public VLCChecker(ClientForm clientForm, AccountingForm accountingForm)
		{
			// forms
			this.clientForm = clientForm;
			this.accountingForm = accountingForm;
			// read videonames.txt
			GetVideoNames();
			// do checker thread
			vlcCheckerThread = new Thread(() => { while (true) { VlcChecker(); Thread.Sleep(50); } });
			vlcCheckerThread.Start();
			ProcessCommandLineChanged += VlcChanged;
		}
		#region VLCCECKER
		void GetVideoNames()
		{
			try
			{
				string fileText;
				using (var stream = new FileStream(videonamestxt, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))//, Encoding.UTF8))
					using (var reader = new StreamReader(stream, Encoding.UTF8))
						fileText = reader.ReadToEnd();

				string[] lines = fileText.Split('\n')
					.Select(l => 
						string.Join("=", l.Trim('\r').Split('=').Skip(1))
						.Replace("\u202A", "")) // he is from Israel so there is a possibility of him using this symbol
					.ToArray();

				//Console.WriteLine(string.Join("|", Encoding.UTF8.GetBytes(lines[0]).Select(b => Convert.ToString(b))));
				Console.WriteLine(string.Join("\n", lines));

				SetPathsAndUri(lines);
			}
			catch (Exception exception)
			{
				LogMessageException(exception);
			}
		}

		void SetPathsAndUri(string[] lines)
		{
			langs[Langs.RUSSIAN] = new Language(Langs.RUSSIAN, lines[0], lines[3], lines[6]);
			langs[Langs.ENGLISH] = new Language(Langs.ENGLISH, lines[1], lines[4], lines[7]);
			langs[Langs.HEBREW] =  new Language(Langs.HEBREW,  lines[2], lines[5], lines[8]);
			defeatPath = lines[9]; errorVideoUri = ClientForm.url2mrl(defeatPath);
			idlePath = lines[10]; idleUri = ClientForm.url2mrl(idlePath);
			selectLangPath = lines[11]; selectLangUri = ClientForm.url2mrl(selectLangPath);
		}

		void LogMessageException(Exception exception)
		{
			if (exception is FileNotFoundException)
				MessageBox.Show(
					$"FILE {videonamestxt} WASNT FOUND \n" +
					$"ФАЙЛ {videonamestxt} НЕ БЫЛ НАЙДЕН");
			else if (exception is UnauthorizedAccessException)
				MessageBox.Show(
					$"INSUFFICIENT PERMISSIONS TO READ THE FILE {videonamestxt} \n" +
					$"НЕДОСТАТОЧНО ПРАВ ДЛЯ ЧТЕНИЯ ФАЙЛА {videonamestxt}");
			else if (exception is IOException)
				MessageBox.Show(
					$"UNKNOWN ERROR WHILE READING FILE {videonamestxt} \n" +
					$"НЕИЗВЕСТНАЯ ОШИБКА ПРИ ЧТЕНИИ ФАЙЛА {videonamestxt}");
			else
				MessageBox.Show($"ОШИБКА:\n{exception.Message}");
			Environment.Exit(0);
		}

		static string GetCodeFromName(string codename)
		{
			try
			{
				// "51012345 only 5 nums matter eng.mp4"
				return codename.Substring(strFrom, strTo);
			}
			catch
			{
				return "#";
			}
		}

		static string GetSafeFileName(string path)
		{
			try { return Path.GetFileName(path); } // sometimes errors
			catch { return path.Split('\\').Last().Split('/').Last(); }
		}

		string GetCommandLine(string processName)
		{
			foreach (Process process in Process.GetProcessesByName(processName))
			{
				using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
				using (ManagementObjectCollection objects = searcher.Get())
				{
					lastVlcProcess = process;
					return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
				}
			}
			return string.Empty;
		}

		static string[] ParseCommandLineArguments(string commandLine) =>
			commandLine is null || commandLine == "" ?
				new string[] { } :
				commandLine.Split('"');

		void KillVLC()
		{
			if (vlcProcess != null && !vlcProcess.HasExited)
			{
				vlcProcess.Kill();
				vlcProcess.Dispose();
			}
		}

		bool IsNotUsedPath(string path) =>
			path != defeatPath && path != selectLangPath && path != idlePath &&
			langs.Values.All(l => l.RulesPath != path && l.ParamsPath != path && l.VictoryPath != path);

		void VlcChecker()
		{
			//			             also gets lastVlcProcess        0       1               2               3        4
			string commandLine = GetCommandLine(processToCheckName); // "vlc path" --started-from-file "video path"
			string[] commandArgs = ParseCommandLineArguments(commandLine);
			if (
				commandArgs.Length == 5 && // have correct video file path
				IsNotUsedPath(commandArgs[3]) && // not used
				commandArgs[3].Length > 0 // not empty
				)
			{
				if (char.IsNumber(GetSafeFileName(commandArgs[3])[0]))// game name starts with number
				{
					videoFileName = commandArgs[3]; // game video path
					gameVideoUri = ClientForm.url2mrl(videoFileName);
					if (lastCommandLine != videoFileName)
					{
						vlcPath = commandArgs[1]; // vlc path
						vlcProcess = lastVlcProcess;
						OnProcessCommandLineChanged(); // calls VlcChanged
					}
					lastCommandLine = videoFileName;
				}
				else
				{
					PlaySomeVideo(commandArgs[3]);
					lastCommandLine = string.Empty;
				}
			}
			else
				lastCommandLine = string.Empty;
		}
		#endregion
		#region PLAY_VIDEOS
		void PlaySomeVideo(string videoUrl)
		{
			vlcProcess = lastVlcProcess;
			print($"PLAY: {videoUrl}");
			KillVLC();
			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.prizeLabel.Hide();
				clientForm.costLabel.Hide();
				clientForm.vlcControl.Play(ClientForm.url2mrl(videoUrl));
			}));
		}

		void VlcChanged(object sender, EventArgs e)
		{

			print($"LAST: {lastCommandLine}\n\tCURRENT: {videoFileName}");
			code = GetCodeFromName(GetSafeFileName(videoFileName)).TrimEnd(' ') + "E";

			DeleteInput();
			accountingForm.Invoke((MethodInvoker)delegate {
				accountingForm.GotGameVideo(videoFileName, code);
			});

			KillVLC();
			videoGameTimeWas = 0;
			errorsCount = 0;
			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.stage = Stage.GAME;
				clientForm.prizeLabel.Hide();
				clientForm.costLabel.Hide();
				clientForm.vlcControl.Play(gameVideoUri);
			}));

			new Thread(() => { 
				Thread.Sleep(1000); 
				gameEnded = false; 
				blockInput = false; 
			}).Start();
		}

		public void ProceedKeys(Keys[] keysStream)
		{
			print($"INPUT BLOCKED: {blockInput}\nGAME ENDED: {gameEnded}");

			string inputCode = string.Join("", keysStream.Select(k => ktos[k]));
			bool guess = inputCode == code;

			print($"_INPUTED: {inputCode}\n\tCODE: {code}\n\tGUESS: {guess}");

			if (guess)
				ProceedWin();
			else
				ProceedDefeat();
		}

		void ProceedDefeat()
		{
			blockInput = true;
			clientForm.stage = Stage.ERROR;

			print($"BLOCKED INPUT, ERRORS: {++errorsCount}");
			DeleteInput();

			clientForm.BeginInvoke(new Action(() =>
			{
				if (clientForm.vlcControl.GetCurrentMedia().Mrl != errorVideoUri.AbsoluteUri)
					videoGameTimeWas = clientForm.vlcControl.Time;
				clientForm.vlcControl.Play(errorVideoUri);
			}));
			print($"TIME BEFORE DEFEAT WAS: {videoGameTimeWas}");
		}

		void ProceedWin()
		{
			gameEnded = true; // good ending
			clientForm.stage = Stage.VICTORY;
			print("GAME ENDED");

			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.vlcControl.Play(langs[language].VictoryVideoUri);
			}));

			// insert win in db
			Db.InsertAward(Db.GetMaxGamesId(), accountingForm.SelectedAward);
			accountingForm.Invoke(new Action(() =>
			{
				accountingForm.StartTables(); // refresh tables
			}));

			MoneyOut();
		}
		#endregion
		#region COM
		private readonly byte[] fifeCoins = new byte[] { 0x30, 0x31, 0x32, 0x33, 0x00, 0x0A };

		private void MoneyOut()
		{
			if (port == null || !port.IsOpen)
				return;

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

		public void TryConnectPort(string com)
		{
			COMPort = com;
			if (port != null && port.IsOpen)
			{
				port.Close();
				port.Dispose();
			}
			try
			{
				port = new SerialPort(COMPort, 9600, Parity.None, 8, StopBits.One);
				port.Handshake = Handshake.RequestToSendXOnXOff;
				//port.DataReceived += DataRecieved;
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
		#endregion
		#region END_VIDEOS
		public void MediaIndeedEnded(string endedVideoMrl)
		{
			print($"ENDED PLAY: {endedVideoMrl}");

			// cant use switch because its not constant values
			if (endedVideoMrl == errorVideoUri.AbsoluteUri)
				EndDefeatVideo();
			else if (endedVideoMrl == langs[language].VictoryVideoUri.AbsoluteUri)
				EndVictoryVideo();
			else if (gameVideoUri != null && endedVideoMrl == gameVideoUri.AbsoluteUri)
				EndGameVideo();
			else if (langs.Values.Any(l => l.RulesUri.AbsoluteUri == endedVideoMrl))
				SafeStop();
			else if (langs.Values.Any(l => l.ParamsUri.AbsoluteUri == endedVideoMrl))
				EndParamsShowVideo();
			else if (endedVideoMrl == selectLangUri.AbsoluteUri)
				Replay();
			else if (endedVideoMrl == idleUri.AbsoluteUri)
				Replay();
			else
				SafeStop();
		}

		private void Replay()
		{
			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.Replay();
			}));
		}

		void EndParamsShowVideo()
		{

		}

		void EndDefeatVideo()
		{
			print($"WAS GAME BEFORE START DEFEAT: {videoGameTimeWas}");

			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.vlcControl.Play(gameVideoUri);
				clientForm.vlcControl.Time = videoGameTimeWas;
				clientForm.stage = Stage.GAME;
			}));

			Console.WriteLine($"TIME NOW: {clientForm.vlcControl.Time}");
			Console.WriteLine($"PROCEEDS: GAME VIDEO");

			new Thread(() =>
			{
				print("STARTED PLAY GAME");
				Thread.Sleep(500); // а вот
				blockInput = false;
				print("UNBLOCKED INPUT");
			}).Start();
			print($"BLOCK INPUT AT THE END OF DEFEAT: {blockInput} AND GAME ENDED: {gameEnded}");
		}

		void EndVictoryVideo()
		{
			// to do; he said smthng about idle video
			SafeStop();
		}

		void EndGameVideo()
		{
			print($"BLOCK INPUT AT THE END OF VIDEO GAME: {blockInput} AND GAME ENDED: {gameEnded}");
			if (blockInput || !gameEnded) // then game goes on because input blocked before defeat video
			{
				if (!blockInput)
				{
					blockInput = true;
					print("BLOCKED INPUT");
					print($"BAD ENDING");
					gameEnded = true; // bad ending
					SafeStop();
				}
				print($"PROCEEDS GAME BUT ERROR");
				DeleteInput();
			}
			else if (!blockInput && gameEnded)
			{
				// good ending and gameEnded already true
				print($"GOOD ENDING");
			}
			else
			{
				print($"BAD ENDING");
				print("GAME ENDED");
				DeleteInput();
				gameEnded = true; // bad ending
				SafeStop();
				// also here idle video if will be
			}
			print($"BLOCK INPUT AT THE END OF THE END OF VIDEO GAME: {blockInput} AND GAME ENDED: {gameEnded}");
		}

		void SafeStop()
		{
			clientForm.BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => clientForm.vlcControl.Stop());
			}));
		}

		#endregion
	}
}
