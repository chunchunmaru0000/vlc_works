﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace vlc_works
{
	public static class VideoChecker
	{
		#region VAR

		// consts
		private const int strFrom = 3;

		private const int strTo = 5;

		// forms
		private static ClientForm clientForm { get; set; }

		private static AccountingForm accountingForm { get; set; }

		// media
		private static long videoGameTimeWas { get; set; } // time video game was stopped

		// video paths
		public static string videoFileName { get; set; } = string.Empty; // game video path

		public static Uri gameVideoUri { get; set; }
		//public static PathUri gameVideo { get; set; } = new PathUri(""); // a good thing to think about

		public static PathUri errorVideo { get; set; }
		public static PathUri idle { get; set; }
		public static PathUri selectLang { get; set; }

		public static Dictionary<Langs, Language> langs { get; set; } = new Dictionary<Langs, Language>();
		public static Language currentLanguage { get => langs[language]; }

		// game things
		private static string code { get; set; } // inputed code like 01234E

		public static Langs language { get; set; } // currently selected language
		public static bool blockInput { get; set; } = false; // block input althought can be done the same via stage variable
		public static bool gameEnded { get; set; } = true; // also bad thing and better to do via stage
		public static int errorsCount { get; set; } // how much errors inputed this game

		// game with ai datas
		public static bool isFirstGame { get; set; } = true;

		// some
		private static void print(object str = null)
		{
			string stroke = str == null ? "" : str.ToString();
			Console.WriteLine(stroke);
		}

		private static void DeleteInput() => clientForm.Invoke((MethodInvoker)delegate { clientForm.DeleteInput(); });

		#endregion VAR

		public static void Constructor(ClientForm clientForm, AccountingForm accountingForm, string[] lines)
		{
			VideoChecker.clientForm = clientForm;
			VideoChecker.accountingForm = accountingForm;
			SetPathsAndUri(lines);
		}

		#region INIT

		private static void SetPathsAndUri(string[] lines)
		{
			langs[Langs.RUSSIAN] = Language.Get(Langs.RUSSIAN, lines, 0);
			langs[Langs.ENGLISH] = Language.Get(Langs.ENGLISH, lines, 1);
			langs[Langs.HEBREW] = Language.Get(Langs.HEBREW, lines, 2);

			int afterLangsLinesOffset = 18;

			errorVideo = new PathUri(lines[afterLangsLinesOffset++]);
			idle = new PathUri(lines[afterLangsLinesOffset++]);
			selectLang = new PathUri(lines[afterLangsLinesOffset++]);
		}

		public static bool IsNotUsedPath(string path) =>
			path != errorVideo.Path && path != selectLang.Path && path != idle.Path &&
			langs.Values.All(l => l.Rules.Path != path && l.Params.Path != path && l.Victory.Path != path);

		public static void VlcChanged()
		{
			code = Utils.GetCodeFromName(Utils.GetSafeFileName(videoFileName), strFrom, strTo).TrimEnd(' ') + "E";
			accountingForm.Invoke((MethodInvoker)delegate
			{
				accountingForm.GotGameVideo(videoFileName, code);
			});

			videoGameTimeWas = 0;
			errorsCount = 0;
			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.stage = Stage.GAME;
				clientForm.prizeLabel.Hide();
				clientForm.costLabel.Hide();
				clientForm.vlcControl.Play(gameVideoUri);
			}));

			new Thread(() =>
			{
				Thread.Sleep(1000);
				gameEnded = false;
				blockInput = false;
			}).Start();
		}

		#endregion INIT

		#region PLAY_VIDEOS

		public static void PlaySomeVideo(string videoUrl)
		{
			print($"PLAY: {videoUrl}");
			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.prizeLabel.Hide();
				clientForm.costLabel.Hide();
				clientForm.vlcControl.Play(ClientForm.url2mrl(videoUrl));
			}));
		}

		public static void ProceedKeys(Keys[] keysStream)
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

		private static void ProceedDefeat()
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

		private static void ProceedWin()
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

		#endregion PLAY_VIDEOS

		#region END_VIDEOS

		public static void MediaIndeedEnded(string endedVideoMrl)
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

		private static void Replay()
		{
			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.Replay();
			}));
		}

		private static void EndParamsShowVideo()
		{
		}

		private static void EndDefeatVideo()
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

		private static void EndVictoryVideo()
		{
			// to do; he said smthng about idle video
			SafeStop();
		}

		private static void EndGameVideo()
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

		private static void SafeStop()
		{
			clientForm.BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => clientForm.vlcControl.Stop());
			}));
		}

		#endregion END_VIDEOS
	}
}