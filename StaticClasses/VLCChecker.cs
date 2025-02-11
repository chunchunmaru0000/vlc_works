using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows.Forms;

namespace vlc_works
{
	public static class VLCChecker
	{
		#region VAR
		// vlc things
		private static string vlcPath { get; set; } // path to vlc executable now not in use
		private static Process vlcProcess { get; set; } // vlc process object
		private static Thread vlcCheckerThread { get; set; }
		private static Process lastVlcProcess { get; set; }
		private static string lastCommandLine { get; set; } = string.Empty;
		// constants
		private const string processToCheckName = "vlc";
		public const string videonamestxt = "videonames.txt";
		// some
		static void print(object str = null)
		{
			string stroke = str == null ? "" : str.ToString();
			Console.WriteLine(stroke);
		}
		#endregion

		public static void Constructor(ClientForm clientForm, AccountingForm accountingForm)
		{
			// construct VideoChecker
			VideoChecker.Constructor(clientForm, accountingForm, Utils.GetVideoNames(videonamestxt));
			// do checker thread
			vlcCheckerThread = new Thread(() => { while (true) { VlcChecker(); Thread.Sleep(50); } });
			vlcCheckerThread.Start();
		}

		#region VLCCECKER
		private static string GetCommandLine(string processName)
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

		private static void KillVLC()
		{
			if (vlcProcess != null && !vlcProcess.HasExited)
			{
				vlcProcess.Kill();
				vlcProcess.Dispose();
			}
		}

		private static bool IsValidCommandArgs(string[] args) =>
			args.Length == 5 && // have correct video file path
			VideoChecker.IsNotUsedPath(args[3]) && // not used in other urls but for now its depricated ???
			!string.IsNullOrEmpty(args[3]); // not empty

		private static void VlcChecker()
		{
			//			             also gets lastVlcProcess        0       1               2               3        4
			string commandLine = GetCommandLine(processToCheckName); // "vlc path" --started-from-file "video path"
			string[] commandArgs = Utils.ParseCommandLineArguments(commandLine);

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

		private static void BeginPlaySomeVideo(string videoFile)
		{
			vlcProcess = lastVlcProcess;
			KillVLC();
			VideoChecker.PlaySomeVideo(videoFile);
			lastCommandLine = string.Empty;
		}

		private static void BeginVlcChanged(string videoFile, string cmdPath)
		{
			VideoChecker.videoFileName = videoFile; // game video path
			VideoChecker.gameVideoUri = ClientForm.url2mrl(VideoChecker.videoFileName);
			if (lastCommandLine != VideoChecker.videoFileName)
			{
				vlcPath = cmdPath; // vlc path
				vlcProcess = lastVlcProcess;
				VlcChanged(); // calls VlcChanged
			}
			lastCommandLine = VideoChecker.videoFileName;
		}

		private static void VlcChanged()
		{
			// VideoChecker.videoFileName is allready changed here to new one
			print($"LAST: {lastCommandLine}\n\tCURRENT: {VideoChecker.videoFileName}");
			KillVLC();
			VideoChecker.VlcChanged();
		}
		#endregion
	}
}
