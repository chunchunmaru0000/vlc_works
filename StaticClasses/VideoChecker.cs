﻿using NAudio.Wave;
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
		private const int maxVideoRepeatTimes = 6 - 1;

		// forms
		private static ClientForm clientForm { get; set; }

		private static AccountingForm accountingForm { get; set; }

		// media
		private static long videoGameTimeWas { get; set; } // time video game was stopped

		// video paths
		public static bool awaitGameVideo { get; set; } = false;
		public static List<GameVideo> gameVideosQueue { get; set; } = new List<GameVideo>();

		public static PathUri errorVideo { get; set; }
		public static PathUri idle { get; set; }
		public static PathUri selectLang { get; set; }
        public static PathUri gameEnd { get; set; }

		public static Dictionary<Langs, Language> langs { get; set; } = new Dictionary<Langs, Language>();
		public static Language currentLanguage { get => langs[language]; }

		// game things
		public static bool won { get; set; }
		public static bool continued { get; set; }
		private static string code { get; set; } // inputed code like 01234E
        private static Stage prevStage { get; set; } = Stage.IDLE;

		public static Langs language { get; set; } // currently selected language
		public static bool blockInput { get; set; } = false; // block input althought can be done the same via stage variable
        public static TimeSpan blockSpan { get; } = TimeSpan.FromSeconds(1.25);
		public static bool gameEnded { get; set; } = true; // also bad thing and better to do via stage
		public static int errorsCount { get; set; } // how much errors inputed this game

		public static int currentVideoPlayCount { get; set; } = 0;
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
			try {
				langs[Langs.RUSSIAN] = Language.Get(Langs.RUSSIAN, lines, 0);
				langs[Langs.ENGLISH] = Language.Get(Langs.ENGLISH, lines, 1);
				langs[Langs.HEBREW] = Language.Get(Langs.HEBREW, lines, 2);

				int afterLangsLinesOffset = 18;

				errorVideo = new PathUri(lines[afterLangsLinesOffset++]);
				idle = new PathUri(lines[afterLangsLinesOffset++]);
				selectLang = new PathUri(lines[afterLangsLinesOffset++]);

                string gameDirectoryPath = lines[afterLangsLinesOffset++];
                clientForm.gameDirectory = new GameDirectory(gameDirectoryPath);

                string[] errors = 
                    clientForm.gameDirectory
                    .AssertScriptDirectoryFolders(
                        clientForm.gameInfo.FirstGame,
                        clientForm.gameInfo.ModeScripts[GameMode.LOW]);
                if (errors.Length > 0)
                    throw new Exception(string.Join("\n\t", errors));

                Sheet gamesSheet = new Sheet(lines[afterLangsLinesOffset++], lines[afterLangsLinesOffset++]);
                Sheet balanceSheet = new Sheet(lines[afterLangsLinesOffset++], lines[afterLangsLinesOffset++]);
                Db.InitSheets(gamesSheet, balanceSheet);

                gameEnd = new PathUri(lines[afterLangsLinesOffset++]);

                afterLangsLinesOffset = 36;

                if (!int.TryParse(lines[afterLangsLinesOffset++], out int portSend))
                    throw new Exception($"НЕ ПОЛУЧИЛОСЬ СКОНВЕРТИРОВАТЬ PORT1 [{lines[afterLangsLinesOffset-1]}]");
                if (!int.TryParse(lines[afterLangsLinesOffset++], out int portRecv))
                    throw new Exception($"НЕ ПОЛУЧИЛОСЬ СКОНВЕРТИРОВАТЬ PORT2 [{lines[afterLangsLinesOffset-1]}]");
                UDPChecker.Constructor(portSend, portRecv, clientForm.gameDirectory);

            } catch (Exception e) {
				MessageBox.Show(
					$"ФАЙЛ {VLCChecker.videonamestxt} НЕ БЫЛ УСПЕШНО ПРОЧИТАН\n" +
					$"ОШИБКА: \n\t{e.Message}");
				Environment.Exit(0);
			}
		}

		public static bool IsNotUsedPath(string path) =>
			path != errorVideo.Path && path != selectLang.Path && path != idle.Path &&
			langs.Values.All(l => l.Rules.Path != path && l.Params.Path != path && l.Victory.Path != path);

        public static void SetCode(string path)
        {
            string fileName = Utils.GetSafeFileName(path);
            // 1|01|12345
            code = Utils.GetCodeFromName(fileName, strFrom, strTo).TrimEnd(' ') + "E";

            accountingForm.Invoke(new Action(() => {
                accountingForm.GotGameVideo(path, code);
            }));
        }

        public static void VlcChanged(GameScript script)
		{
            GameVideo gamePathUri = clientForm.gameDirectory.GetRandomGame(script);
            SetCode(gamePathUri.Game.Path);

			gameVideosQueue.Clear();
			gameVideosQueue.Add(gamePathUri);

			if (awaitGameVideo)
				StartVideoInQueue();
			awaitGameVideo = false;

            UDPChecker.Send(script);
		}

		public static void StartVideoInQueue()
		{
			if (gameVideosQueue.Count == 0) {
				awaitGameVideo = true;
				return;
			}

            videoGameTimeWas = 0;
            errorsCount = 0;
            ToBlockInput();
            clientForm.BeginInvoke(new Action(() => {
                clientForm.prizeLabel.Hide();
                clientForm.costLabel.Hide();
                clientForm.PlayGameRules();
            }));
            new Thread(() => {
				Thread.Sleep(blockSpan);
				gameEnded = false;
			}).Start();
		}

        public static void StartPlayGameMainVideo()
        {
            PathUri gameVideo = gameVideosQueue[0].Game;

            ToBlockInput();
            videoGameTimeWas = 0;
            errorsCount = 0;
            clientForm.BeginInvoke(new Action(() => {
                clientForm.Play(gameVideo.Uri, Stage.GAME);
            }));
        }

        public static void ToBlockInput()
        {
            print($"BLOCK INPUT, WAS {blockInput}");
            blockInput = true;
            new Thread(() => {
                Thread.Sleep(blockSpan);
                blockInput = false;
                print($"UNBLOCKED INPUT, WAS {blockInput}");
            }).Start();
        }

        private static AudioFileReader audioFile { get; set; } = null;
        public static WaveOutEvent outputSound = new WaveOutEvent();
        public static void PlayGameStopVideo()
        {
            audioFile?.Dispose();
            audioFile = new AudioFileReader(currentLanguage.GameStopSound.Path);
            outputSound.Init(audioFile);
            outputSound.Play();

            clientForm.Play(gameVideosQueue[0].Stop.Uri, Stage.GAME_STOP);
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
			bool guess = inputCode.Trim('E') == code.Trim('E');

			print($"_INPUTED: {inputCode}\n\tCODE: {code}\n\tGUESS: {guess}");

			if (guess)
				ProceedWin();
			else
				ProceedDefeat();
		}

		private static void ProceedDefeat()
		{
			blockInput = true;
            prevStage = clientForm.stage;
			clientForm.stage = Stage.ERROR;

			print($"BLOCKED INPUT, ERRORS: {++errorsCount}");
			DeleteInput();

			clientForm.BeginInvoke(new Action(() => {
                if (errorsCount >= 3) {
                    if (prevStage == Stage.GAME || prevStage == Stage.LEFT_SECONDS) {
                        gameEnded = true;
                        PlayGameStopVideo();
                    }
                }
                else {
                    if (clientForm.vlcControl.GetCurrentMedia().Mrl != errorVideo.Uri.AbsoluteUri)
                        videoGameTimeWas = clientForm.vlcControl.Time;
				    clientForm.vlcControl.Play(errorVideo.Uri);
                }
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
			accountingForm.Invoke(new Action(() => {
				accountingForm.StartTables(); // refresh tables
				COMPort.MoneyOut(DbCurrentRecord.SelectedPrize, accountingForm);
			}));

            // relay
			new Thread(() => {
				RelayChecker.Transmit(Channel.COINS_LIGHT, true); // 15 seconds on to 3 channel
				Thread.Sleep(TimeSpan.FromSeconds(30));
				RelayChecker.Transmit(Channel.COINS_LIGHT, false); // off
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
            else if (gameVideosQueue.Any(v => v.Game.Uri.AbsoluteUri == endedVideoMrl))
                EndGameVideo();
            else if (langs.Values.Any(l => l.GameLeftSeconds.Uri.AbsoluteUri == endedVideoMrl))
                EndLeftSeconds();
            else if (gameVideosQueue.Any(v => v.Stop.Uri.AbsoluteUri == endedVideoMrl))
                EndStopVideo();
            else if (endedVideoMrl == gameEnd.Uri.AbsoluteUri)
                clientForm.PlayPlayAgain();
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
                StartVideoInQueue();
            else if (langs.Values.Any(l => l.GameRules.Uri.AbsoluteUri == endedVideoMrl))
                StartPlayGameMainVideo();
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

			// continued = false; commented because it will be false already here
			if (endedVideoMrl == currentLanguage.PlayAgain.Uri.AbsoluteUri)
				clientForm.DoDataBaseGameRecord();

			clientForm.PlayIdle();
		}

		private static void EndParamsShowVideo()
		{
            currentVideoPlayCount = 0;
            clientForm.PlayHowToPay();
		}

		private static void EndDefeatVideo()
		{
			print($"WAS GAME BEFORE START DEFEAT: {videoGameTimeWas}");
			print($"PREV STAGE: {prevStage}");
            currentVideoPlayCount = 0;

            clientForm.BeginInvoke(new Action(() => {
                if (prevStage == Stage.GAME) {
                    clientForm.vlcControl.Play(gameVideosQueue[0].Game.Uri);
                    clientForm.stage = Stage.GAME;
                }
                else {
                    clientForm.vlcControl.Play(currentLanguage.GameLeftSeconds.Uri);
                    clientForm.stage = Stage.LEFT_SECONDS;
                }
                //clientForm.Play(gameVideosQueue[0].Game.Uri, Stage.GAME);
                clientForm.vlcControl.Time = videoGameTimeWas;
			}));

			Console.WriteLine($"TIME NOW: {clientForm.vlcControl.Time}");
			Console.WriteLine($"PROCEEDS: GAME VIDEO");

            ToBlockInput();
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
			if (blockInput || !gameEnded) {// then game goes on because input blocked before defeat video
				//if (!blockInput) {
                    ToBlockInput();
                    clientForm.PlayLeftSeconds();
				//}
				print($"PROCEEDS GAME BUT ERROR");
				DeleteInput();
			}
			else if (!blockInput && gameEnded) {
				// good ending and gameEnded already true
				print($"GOOD ENDING");
			}
			else {
				print("THIS IS DEPRICATED AND WHY IS THAT EVEN PRINTED");
				DeleteInput();
				gameEnded = true; // bad ending
				SafeStop();
				// also here idle video if will be
			}
			print($"BLOCK INPUT AT THE END OF THE END OF VIDEO GAME: {blockInput} AND GAME ENDED: {gameEnded}");
		}

		public static void SafeStop()
		{
			clientForm.BeginInvoke(new Action(
				() => ThreadPool.QueueUserWorkItem(_ => clientForm.vlcControl.Stop())));
		}

        private static void EndLeftSeconds()
        {
            blockInput = true;
            print("BLOCKED INPUT");
            print($"BAD ENDING");
            gameEnded = true; // bad ending
            //SafeStop();
            //clientForm.PlayPlayAgain();
            print($"BLOCK INPUT AT THE END OF THE END OF LeftSeconds: {blockInput} AND GAME ENDED: {gameEnded}");

            PlayGameStopVideo();
        }

        private static void EndStopVideo()
        {
            clientForm.Play(gameEnd.Uri, Stage.GAME_END);
        }

        #endregion END_VIDEOS
    }
}