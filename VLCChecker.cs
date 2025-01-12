﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
	public class VLCChecker
	{
		// forms
		ClientForm clientForm { get; set; }
		AccountingForm accountingForm { get; set; }
		// media
		long videoGameTimeWas { get; set; }
		// vlc things
		string vlcPath { get; set; }
		Process vlcProcess { get; set; }
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
		};
		// video paths
		string videoFileName { get; set; } = string.Empty;
		string winPath { get; set; }
		string defeatPath { get; set; }
		// uri
		Uri gameVideoUri { get; set; }
		Uri victoryVideoUri { get; set; }
		Uri errorVideoUri { get; set; }
		// game things
		string code { get; set; }
		public bool blockInput { get; set; } = false;
		public bool gameEnded { get; set; } = true;
		// some
		public static event EventHandler ProcessCommandLineChanged;
		protected virtual void OnProcessCommandLineChanged() => ProcessCommandLineChanged?.Invoke(this, EventArgs.Empty);
		void print(object str = null)
		{
			string stroke = str == null ? "" : str.ToString();
			Console.WriteLine(stroke);
			//accountingForm.BeginInvoke(new Action(() => { accountingForm.DEBUG(stroke); }));
		}
		void DeleteInput() => clientForm.Invoke((MethodInvoker)delegate { clientForm.DeleteInput(); });

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

		void GetVideoNames()
		{
			try
			{
				string fileText;
				using (var stream = new FileStream(videonamestxt, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (var reader = new StreamReader(stream))
					fileText = reader.ReadToEnd();
				string[] lines = fileText.Split('\n')
					.Select(l => string.Join("=", l.Trim('\r').Split('=').Skip(1)))
					.ToArray();

				Console.WriteLine(string.Join("\n", lines));

				winPath = lines[0];
				victoryVideoUri = ClientForm.url2mrl(winPath);

				defeatPath = lines[1];
				errorVideoUri = ClientForm.url2mrl(defeatPath);
			}
			catch (Exception exception)
			{
				LogMessageException(exception);
			}
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

		void VlcChecker()
		{
			//			             also gets lastVlcProcess        0       1               2               3        4
			string commandLine = GetCommandLine(processToCheckName); // "vlc path" --started-from-file "video path"
			string[] commandArgs = ParseCommandLineArguments(commandLine);
			if (commandArgs.Length == 5 && // have correct video file path
				commandArgs[3] != winPath && commandArgs[3] != defeatPath &&
				commandArgs[3].Length > 0
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
			clientForm.BeginInvoke(new Action(() =>
			{
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

		void ProceedWin()
		{
			gameEnded = true; // good ending
			print("GAME ENDED");

			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.vlcControl.Play(victoryVideoUri);
			}));
		}

		void ProceedDefeat()
		{
			blockInput = true;
			print("BLOCKED INPUT");
			DeleteInput();

			clientForm.BeginInvoke(new Action(() =>
			{
				videoGameTimeWas = clientForm.vlcControl.Time;
				clientForm.vlcControl.Play(errorVideoUri);
			}));
			print($"TIME BEFORE DEFEAT WAS: {videoGameTimeWas}");
		}

		public void MediaIndeedEnded(string endedVideoMrl)
		{
			print($"ENDED PLAY: {endedVideoMrl}");
			if (endedVideoMrl == errorVideoUri.AbsoluteUri)
				EndDefeatVideo();
			else if (endedVideoMrl == victoryVideoUri.AbsoluteUri)
				EndVictoryVideo();
			else if (gameVideoUri != null && endedVideoMrl == gameVideoUri.AbsoluteUri)
				EndGameVideo();
			else if (endedVideoMrl == ClientForm.ParamsVideoUri.AbsoluteUri)
				EndParamsShowVideo();
			else
				SafeStop();
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
	}
}
