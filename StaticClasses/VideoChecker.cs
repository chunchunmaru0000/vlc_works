using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace vlc_works
{
	public static class VideoChecker
	{
		#region VAR

		// consts
		private const int strFrom = 3;

		private const int strTo = 5;
		private const int maxVideoRepeatTimes = 5;

		// forms
		private static ClientForm clientForm { get; set; }

		private static AccountingForm accountingForm { get; set; }

		// media
		private static long videoGameTimeWas { get; set; } // time video game was stopped

		// video paths
		public static bool awaitGameVideo { get; set; } = false;
		public static List<PathUri> gameVideosQueue { get; set; } = new List<PathUri>();

		public static PathUri errorVideo { get; set; }
		public static PathUri idle { get; set; }
		public static PathUri selectLang { get; set; }

		public static Dictionary<Langs, Language> langs { get; set; } = new Dictionary<Langs, Language>();
		public static Language currentLanguage { get => langs[language]; }

		// game things
		public static bool won { get; set; }
		public static bool continued { get; set; }
		private static string code { get; set; } // inputed code like 01234E

		public static Langs language { get; set; } // currently selected language
		public static bool blockInput { get; set; } = false; // block input althought can be done the same via stage variable
		public static bool gameEnded { get; set; } = true; // also bad thing and better to do via stage
		public static int errorsCount { get; set; } // how much errors inputed this game

		public static int currentVideoPlayCount { get; set; } = 0;

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
			try
			{
				langs[Langs.RUSSIAN] = Language.Get(Langs.RUSSIAN, lines, 0);
				langs[Langs.ENGLISH] = Language.Get(Langs.ENGLISH, lines, 1);
				langs[Langs.HEBREW] = Language.Get(Langs.HEBREW, lines, 2);

				int afterLangsLinesOffset = 18;

				errorVideo = new PathUri(lines[afterLangsLinesOffset++]);
				idle = new PathUri(lines[afterLangsLinesOffset++]);
				selectLang = new PathUri(lines[afterLangsLinesOffset++]);
			}
			catch
			{
				MessageBox.Show(
					$"ФАЙЛ {VLCChecker.videonamestxt} НЕ БЫЛ УСПЕШНО ПРОЧИТАН\n" +
					$"ТАК КАК ВЕРОЯТНО БЫЛО НЕДОСТАТОЧНО ПУТЕЙ");
				Environment.Exit(0);
			}
		}

		public static bool IsNotUsedPath(string path) =>
			path != errorVideo.Path && path != selectLang.Path && path != idle.Path &&
			langs.Values.All(l => l.Rules.Path != path && l.Params.Path != path && l.Victory.Path != path);

		public static void SetCode(string path)
		{
            code = Utils.GetCodeFromName(Utils.GetSafeFileName(path), strFrom, strTo).TrimEnd(' ') + "E";
            accountingForm.Invoke(new Action(() => accountingForm.GotGameVideo(path, code)));
        }

		public static void VlcChanged(PathUri gamePathUri)
		{
			SetCode(gamePathUri.Path);

			gameVideosQueue.Clear();
			gameVideosQueue.Add(gamePathUri);

			if (awaitGameVideo)
				StartVideoInQueue();
			awaitGameVideo = false;
		}

		public static void StartVideoInQueue()
		{
			if (gameVideosQueue.Count == 0)
			{
				awaitGameVideo = true;
				return;
			}

			PathUri gameVideo = gameVideosQueue[0];
            //code = Utils.GetCodeFromName(Utils.GetSafeFileName(gameVideo.Path), strFrom, strTo).TrimEnd(' ') + "E";
            //accountingForm.Invoke(new Action(() => accountingForm.GotGameVideo(gamePathUri.Path, code)));

            videoGameTimeWas = 0;
			errorsCount = 0;
			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.prizeLabel.Hide();
				clientForm.costLabel.Hide();
				clientForm.Play(gameVideo.Uri, Stage.GAME);
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
			won = true;
			print("GAME ENDED");

			clientForm.Play(currentLanguage.Victory.Uri, Stage.VICTORY);

			// insert win in db and get coins out
			accountingForm.Invoke(new Action(() =>
			{
				accountingForm.StartTables(); // refresh tables
				COMPort.MoneyOut(DbCurrentRecord.SelectedPrize, accountingForm);
			}));

			new Thread(() =>
			{
				RelayChecker.Transmit(3, true); // 10 seconds on to 3 channel
				Thread.Sleep(10000);
				RelayChecker.Transmit(3, false); // off
			}).Start();
		}

		#endregion PLAY_VIDEOS

		#region END_VIDEOS

		public static void MediaIndeedEnded(string endedVideoMrl)
		{
			print($"ENDED PLAY: {endedVideoMrl}");
			//print($"VIEOS IN QUEUE: {gameVideosQueue.Count}");
			//print($"QUEUE:\n\t{string.Join("\t\n", gameVideosQueue.Select(p => p.Uri.AbsoluteUri))}");

			if (currentVideoPlayCount >= maxVideoRepeatTimes)
				HandleInfinitePlay(endedVideoMrl);

			else if (endedVideoMrl == errorVideo.Uri.AbsoluteUri)
				EndDefeatVideo();
			else if (endedVideoMrl == langs[language].Victory.Uri.AbsoluteUri)
				EndVictoryVideo();
			else if (gameVideosQueue.Any(v => v.Uri.AbsoluteUri == endedVideoMrl))
				EndGameVideo();
			else if (langs.Values.Any(l => l.Rules.Uri.AbsoluteUri == endedVideoMrl))
				Replay();
			else if (langs.Values.Any(l => l.Params.Uri.AbsoluteUri == endedVideoMrl))
				EndParamsShowVideo();
			else if (endedVideoMrl == selectLang.Uri.AbsoluteUri)
				Replay();
			else if (endedVideoMrl == idle.Uri.AbsoluteUri)
				Replay();
			else if (langs.Values.Any(l => l.PlayAgain.Uri.AbsoluteUri == endedVideoMrl))
				Replay();
			else if (langs.Values.Any(l => l.HowToPay.Uri.AbsoluteUri == endedVideoMrl))
				Replay();
			else if (langs.Values.Any(l => l.GamePayed.Uri.AbsoluteUri == endedVideoMrl))
				EndGamePayed();
			else
				SafeStop();
		}

		private static void Replay()
		{
			clientForm.BeginInvoke(new Action(() =>
			{
				if (clientForm.stage != Stage.IDLE) 
					currentVideoPlayCount++;
				print($"CURRENT VIDEO REPLAY COUNT {currentVideoPlayCount}");
				clientForm.Replay();
			}));
		}

		private static void HandleInfinitePlay(string endedVideoMrl)
		{
			print(
				$"HANDLE INFINITE REPLAY, REPLAY COUNT: {currentVideoPlayCount}\n" +
				$"CEASES TO INFINITE PLAY {endedVideoMrl}");
			currentVideoPlayCount = 0;
			clientForm.PlayIdle();

			// continued = false; commented because it will be false already here
			if (endedVideoMrl == currentLanguage.PlayAgain.Uri.AbsoluteUri)
				clientForm.DoDataBaseGameRecord();
		}

		private static void EndParamsShowVideo()
		{
			clientForm.PlayHowToPay();
		}

		private static void EndDefeatVideo()
		{
			print($"WAS GAME BEFORE START DEFEAT: {videoGameTimeWas}");

			clientForm.BeginInvoke(new Action(() =>
			{
				clientForm.vlcControl.Play(gameVideosQueue[0].Uri);
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
			//SafeStop();
			won = true;
			clientForm.PlayPlayAgain();
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
					//SafeStop();
					clientForm.PlayPlayAgain();
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
				print("THIS IS DEPRICATED AND WHY IS THAT EVEN PRINTED");
				DeleteInput();
				gameEnded = true; // bad ending
				SafeStop();
				// also here idle video if will be
			}
			print($"BLOCK INPUT AT THE END OF THE END OF VIDEO GAME: {blockInput} AND GAME ENDED: {gameEnded}");
		}

		private static void EndGamePayed()
		{
			StartVideoInQueue();
		}

		public static void SafeStop()
		{
			clientForm.BeginInvoke(new Action(
				() => ThreadPool.QueueUserWorkItem(_ => clientForm.vlcControl.Stop())));
		}

		#endregion END_VIDEOS
	}
}