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
	public static class VLCChecker
	{
		#region VAR
		// forms
		static private ClientForm clientForm { get; set; }
		static private AccountingForm accountingForm { get; set; }
		// media
		static private long videoGameTimeWas { get; set; } // time video game was stopped
		// vlc things
		static private string vlcPath { get; set; } // path to vlc executable now not in use
		static private Process vlcProcess { get; set; } // vlc process object
		static private Thread vlcCheckerThread { get; set; }
		static private Process lastVlcProcess { get; set; }
		static private string lastCommandLine { get; set; } = string.Empty;
		// constants
		const int strFrom = 3;
		const int strTo = 5;
		const string processToCheckName = "vlc";
		const string videonamestxt = "videonames.txt";
		// video paths
		static string videoFileName { get; set; } = string.Empty; // game video path
		static public Uri gameVideoUri { get; set; }
		//static public PathUri gameVideo { get; set; } = new PathUri(""); // a good thing to think about

		static public PathUri errorVideo { get; set; }
		static public PathUri idle { get; set; }
		static public PathUri selectLang { get; set; }

		static public Dictionary<Langs, Language> langs { get; set; } = new Dictionary<Langs, Language>();
		// game things
		static string code { get; set; } // inputed code like 01234E
		static public Langs language { get; set; } // currently selected language
		static public bool blockInput { get; set; } = false; // block input althought can be done the same via stage variable
		static public bool gameEnded { get; set; } = true; // also bad thing and better to do via stage
		static public int errorsCount { get; set; } // how much errors inputed this game
		// some
		static void print(object str = null)
		{
			string stroke = str == null ? "" : str.ToString();
			Console.WriteLine(stroke);
			//accountingForm.BeginInvoke(new Action(() => { accountingForm.DEBUG(stroke); }));
		}
		static void DeleteInput() => clientForm.Invoke((MethodInvoker)delegate { clientForm.DeleteInput(); });
		#endregion

		static public void Constructor(ClientForm clientForm, AccountingForm accountingForm)
		{
			// forms
			VLCChecker.clientForm = clientForm;
			VLCChecker.accountingForm = accountingForm;
			// read videonames.txt
			SetPathsAndUri(Utils.GetVideoNames(videonamestxt));
			// do checker thread
			vlcCheckerThread = new Thread(() => { while (true) { VlcChecker(); Thread.Sleep(50); } });
			vlcCheckerThread.Start();
		}

		#region VLCCECKER
		static private void SetPathsAndUri(string[] lines)
		{
			langs[Langs.RUSSIAN] = Language.Get(Langs.RUSSIAN, lines, 0);
			langs[Langs.ENGLISH] = Language.Get(Langs.ENGLISH, lines, 1);
			langs[Langs.HEBREW] = Language.Get(Langs.HEBREW, lines, 2);

			int afterLangsLinesOffset = 18;

			errorVideo = new PathUri(lines[afterLangsLinesOffset++]);
			idle = new PathUri(lines[afterLangsLinesOffset++]);
			selectLang = new PathUri(lines[afterLangsLinesOffset++]);
		}

		static private string GetCommandLine(string processName)
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

		static private void KillVLC()
		{
			if (vlcProcess != null && !vlcProcess.HasExited)
			{
				vlcProcess.Kill();
				vlcProcess.Dispose();
			}
		}

		static private bool IsNotUsedPath(string path) =>
			path != errorVideo.Path && path != selectLang.Path && path != idle.Path &&
			langs.Values.All(l => l.Rules.Path != path && l.Params.Path != path && l.Victory.Path != path);

		static private bool IsValidCommandArgs(string[] args) =>
			args.Length == 5 && // have correct video file path
			IsNotUsedPath(args[3]) && // not used in other urls but for now its depricated ???
			!string.IsNullOrEmpty(args[3]); // not empty

		static private void VlcChecker()
		{
			//			             also gets lastVlcProcess        0       1               2               3        4
			string commandLine = GetCommandLine(processToCheckName); // "vlc path" --started-from-file "video path"
			print(commandLine);
			string[] commandArgs = Utils.ParseCommandLineArguments(commandLine);
			print(IsValidCommandArgs(commandArgs));

			if (IsValidCommandArgs(commandArgs))
			{
				if (char.IsNumber(Utils.GetSafeFileName(commandArgs[3])[0])) // game name starts with number
					BeginVlcChanged(commandArgs[3], commandArgs[1]);
				else
					BeginPlaySomeVideo(commandArgs[3]);
			}
			else
				lastCommandLine = string.Empty;
		}

		static private void BeginVlcChanged(string videoFile, string cmdPath)
		{
			videoFileName = videoFile; // game video path
			gameVideoUri = ClientForm.url2mrl(videoFileName);
			if (lastCommandLine != videoFileName)
			{
				vlcPath = cmdPath; // vlc path
				vlcProcess = lastVlcProcess;
				VlcChanged(); // calls VlcChanged
			}
			lastCommandLine = videoFileName;
		}

		static private void BeginPlaySomeVideo(string videoFile)
		{
			vlcProcess = lastVlcProcess;
			KillVLC();
			PlaySomeVideo(videoFile);
			lastCommandLine = string.Empty;
		}

		static private void VlcChanged()
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
		#endregion
		#region PLAY_VIDEOS
		static private void PlaySomeVideo(string videoUrl)
		{
			print($"PLAY: {videoUrl}");
			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.prizeLabel.Hide();
				clientForm.costLabel.Hide();
				clientForm.vlcControl.Play(ClientForm.url2mrl(videoUrl));
			}));
		}

		static public void ProceedKeys(Keys[] keysStream)
		{
			print($"INPUT BLOCKED: {blockInput}\nGAME ENDED: {gameEnded}");

			string inputCode = string.Join("", keysStream.Select(k => Utils.ktos[k]));
			bool guess = inputCode == code;

			print($"_INPUTED: {inputCode}\n\tCODE: {code}\n\tGUESS: {guess}");

			if (guess)
				ProceedWin();
			else
				ProceedDefeat();
		}

		static private void ProceedDefeat()
		{
			blockInput = true;
			clientForm.stage = Stage.ERROR;

			print($"BLOCKED INPUT, ERRORS: {++errorsCount}");
			DeleteInput();

			clientForm.BeginInvoke(new Action(() =>
			{
				if (clientForm.vlcControl.GetCurrentMedia().Mrl != errorVideo.Uri.AbsoluteUri)
					videoGameTimeWas = clientForm.vlcControl.Time;
				clientForm.vlcControl.Play(errorVideo.Uri);
			}));
			print($"TIME BEFORE DEFEAT WAS: {videoGameTimeWas}");
		}

		static private void ProceedWin()
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
		static public void MediaIndeedEnded(string endedVideoMrl)
		{
			print($"ENDED PLAY: {endedVideoMrl}");

			// cant use switch because its not constant values
			if (endedVideoMrl == errorVideo.Uri.AbsoluteUri)
				EndDefeatVideo();
			else if (endedVideoMrl == langs[language].Victory.Uri.AbsoluteUri)
				EndVictoryVideo();
			else if (gameVideoUri != null && endedVideoMrl == gameVideoUri.AbsoluteUri)
				EndGameVideo();
			else if (langs.Values.Any(l => l.Rules.Uri.AbsoluteUri == endedVideoMrl))
				SafeStop();
			else if (langs.Values.Any(l => l.Params.Uri.AbsoluteUri == endedVideoMrl))
				EndParamsShowVideo();
			else if (endedVideoMrl == selectLang.Uri.AbsoluteUri)
				Replay();
			else if (endedVideoMrl == idle.Uri.AbsoluteUri)
				Replay();
			else
				SafeStop();
		}

		static private void Replay()
		{
			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.Replay();
			}));
		}

		static private void EndParamsShowVideo()
		{

		}

		static private void EndDefeatVideo()
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

		static private void EndVictoryVideo()
		{
			// to do; he said smthng about idle video
			SafeStop();
		}

		static private void EndGameVideo()
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

		static private void SafeStop()
		{
			clientForm.BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => clientForm.vlcControl.Stop());
			}));
		}
		#endregion
	}
}
