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
			SetPathsAndUri(Utils.GetVideoNames(videonamestxt));
			// do checker thread
			vlcCheckerThread = new Thread(() => { while (true) { VlcChecker(); Thread.Sleep(50); } });
			vlcCheckerThread.Start();
			ProcessCommandLineChanged += VlcChanged;
		}
		#region VLCCECKER
		private void SetPathsAndUri(string[] lines)
		{
			// new Language(Langs.RUSSIAN, lines[0], lines[3], lines[6], lines[9], lines[12], lines[15]);
			langs[Langs.RUSSIAN] = Language.Get(Langs.RUSSIAN, lines, 0);
			langs[Langs.ENGLISH] = Language.Get(Langs.ENGLISH, lines, 1);
			langs[Langs.HEBREW] = Language.Get(Langs.HEBREW, lines, 2);

			int afterLangsLinesOffset = 18;

			defeatPath = lines[afterLangsLinesOffset++]; 
			errorVideoUri = ClientForm.url2mrl(defeatPath);

			idlePath = lines[afterLangsLinesOffset++]; 
			idleUri = ClientForm.url2mrl(idlePath);

			selectLangPath = lines[afterLangsLinesOffset++]; 
			selectLangUri = ClientForm.url2mrl(selectLangPath);
		}

		private string GetCommandLine(string processName)
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

		private void KillVLC()
		{
			if (vlcProcess != null && !vlcProcess.HasExited)
			{
				vlcProcess.Kill();
				vlcProcess.Dispose();
			}
		}

		private bool IsNotUsedPath(string path) =>
			path != defeatPath && path != selectLangPath && path != idlePath &&
			langs.Values.All(l => l.Rules.Path != path && l.Params.Path != path && l.Victory.Path != path);

		private void VlcChecker()
		{
			//			             also gets lastVlcProcess        0       1               2               3        4
			string commandLine = GetCommandLine(processToCheckName); // "vlc path" --started-from-file "video path"
			string[] commandArgs = Utils.ParseCommandLineArguments(commandLine);
			if (
				commandArgs.Length == 5 && // have correct video file path
				IsNotUsedPath(commandArgs[3]) && // not used
				commandArgs[3].Length > 0 // not empty
				)
			{
				if (char.IsNumber(Utils.GetSafeFileName(commandArgs[3])[0]))// game name starts with number
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
		private void PlaySomeVideo(string videoUrl)
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

		private void VlcChanged(object sender, EventArgs e)
		{

			print($"LAST: {lastCommandLine}\n\tCURRENT: {videoFileName}");
			code = Utils.GetCodeFromName(Utils.GetSafeFileName(videoFileName), strFrom, strTo).TrimEnd(' ') + "E";

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

		private void ProceedDefeat()
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

		private void ProceedWin()
		{
			gameEnded = true; // good ending
			clientForm.stage = Stage.VICTORY;
			print("GAME ENDED");

			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.vlcControl.Play(langs[language].Victory.Uri);
			}));

			// insert win in db
			Db.InsertAward(Db.GetMaxGamesId(), accountingForm.SelectedAward);
			accountingForm.Invoke(new Action(() =>
			{
				accountingForm.StartTables(); // refresh tables
			}));
			
			COMPort.MoneyOut(accountingForm);
		}
		#endregion
		#region END_VIDEOS
		public void MediaIndeedEnded(string endedVideoMrl)
		{
			print($"ENDED PLAY: {endedVideoMrl}");

			// cant use switch because its not constant values
			if (endedVideoMrl == errorVideoUri.AbsoluteUri)
				EndDefeatVideo();
			else if (endedVideoMrl == langs[language].Victory.Uri.AbsoluteUri)
				EndVictoryVideo();
			else if (gameVideoUri != null && endedVideoMrl == gameVideoUri.AbsoluteUri)
				EndGameVideo();
			else if (langs.Values.Any(l => l.Rules.Uri.AbsoluteUri == endedVideoMrl))
				SafeStop();
			else if (langs.Values.Any(l => l.Params.Uri.AbsoluteUri == endedVideoMrl))
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

		private void EndParamsShowVideo()
		{

		}

		private void EndDefeatVideo()
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

		private void EndVictoryVideo()
		{
			// to do; he said smthng about idle video
			SafeStop();
		}

		private void EndGameVideo()
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

		private void SafeStop()
		{
			clientForm.BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => clientForm.vlcControl.Stop());
			}));
		}

		#endregion
	}
}
