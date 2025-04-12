using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Vlc.DotNet;
using Vlc.DotNet.Core;

namespace vlc_works
{
	public partial class ClientForm : Form
	{
		#region VAR
		#region UNCHANGING_VAR
		private IKeyboardEvents hook { get; set; } // hook for hook keys
		public AccountingForm accountingForm { get; set; }
        public GameInfo gameInfo { get; set; }
        public ScriptParser scriptParser { get; set; }
        public GameDirectory gameDirectory { get; set; }
        #endregion UNCHANGING_VAR
        #region CONSTS
        private Keys[] NumKeys { get; } = new Keys[] // keys of numpad
		{
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
			Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
		};
		readonly TimeSpan fadeTime = TimeSpan.FromSeconds(10); // key fade time
		#endregion CONSTS
		public List<InputKey> keysStream { get; set; } = new List<InputKey>(); // stream of keys not stream but it gets keysd in runtime so be it
		public Stage stage { get; set; } // current stage
        #region SOME_VAR
        private bool isFullScreen { get; set; } = false;
		public void print(object str = null)
		{
			string stroke = str == null ? "" : str.ToString();
			Console.WriteLine(stroke);
		}
		public string keysStreamtos() => string.Join("", keysStream.Select(k => Utils.ktos[k.Key])); // get string of keys stream
		public static Uri url2mrl(string url) => new Uri(url);
		#endregion SOME_VAR
		#endregion VAR

		public ClientForm()
		{
			//this.vlcControl.VlcLibDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libvlc\\win-x86"));
			InitializeComponent();
			Show();
			// key logger
			hook = Hook.GlobalEvents();
			hook.KeyUp += OnWinKeyDown;
            // accounting form and RelayChecker
            accountingForm = new AccountingForm(this);
			accountingForm.Show();
			RelayChecker.Constructor(accountingForm);
			// set vlcControl
			vlcControl.EndReached += EndReached;
			vlcControl.MediaChanged += MediaChanged;
            // parse game script
            InitGameScript();
            // VLCChecker and VideoChecker
            VLCChecker.Constructor(this, accountingForm);
			// set form
			Form1_SizeChanged(inputLabel, EventArgs.Empty); // includes align inputLabel
			inputLabel.SizeChanged += AlignInputLabel;
			DeleteInput();
			SetFormFullScreen();
		}

        #region SCRIPT

        private void InitGameScript()
        {
            try {
                scriptParser = new ScriptParser("gameScript.txt");
                gameInfo = scriptParser.Parse(accountingForm);

                Utils.print(string.Join("\n", gameInfo.ModeScripts.ToArray().Select(p => 
                $"[{p.Key}]\n\t{string.Join("\n\t", p.Value.Select(s => s.ToString()))}\n[///]")));
            } catch (Exception e) {
                MessageBox.Show(e.Message);
                Environment.Exit(1);
            }
        }

        #endregion SCRIPT
        #region SCREEN
        void SetFormFullScreen()
		{
            // of course its better but im not sure in screens order
            /* 
			Screen[] screens = Screen.AllScreens;
			screen = screens.Length > 1 
                ? screens[1] 
                : screens[0];
			StartPosition = FormStartPosition.Manual;
			Location = PointToScreen(new Point(
				(screens.Length > 1 ? screens[0].Bounds.Width : 0) + hmh(screen.Bounds.Width), 0));
			*/
            // need right monitor
            //Location = new Point(2000, 100);
            // need upper monitor
            foreach(Screen screen in Screen.AllScreens)
            {
                Console.WriteLine($"###{screen.DeviceName} {screen.WorkingArea} {screen.Bounds}");
            }
            Location = new Point(100, -1000);
            print($"X: {Location.X}; Y: {Location.Y}");
			FullScreen();
		}

		void FullScreen()
		{
			if (isFullScreen)
			{
				FormBorderStyle = FormBorderStyle.Sizable;
				WindowState = FormWindowState.Normal;
				Size = accountingForm.Size;
				Location = new Point(Location.X, 100);
			}
			else
			{
				FormBorderStyle = FormBorderStyle.None;
				WindowState = FormWindowState.Maximized;
			}
			isFullScreen = !isFullScreen;
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			vlcControl.Size = Size;
			AlignInputLabel(sender, e);
		}
		#endregion
		#region INPUT
		public void Play(Uri uri, Stage s)
		{
			Invoke(new Action(() => {
				stage = s;
				print($"NOW IS [{stage}]");

				VideoChecker.currentVideoPlayCount = 0;
				ThreadPool.QueueUserWorkItem(_ => vlcControl.Play(uri));
				print($"PLAY [{uri}]");
				print($"CURRENT VIDEO PLAY COUNT = {VideoChecker.currentVideoPlayCount}");
			}));
		}

		private readonly string[] inputNames = new string[] 
		{
			"playerNameBox",
			"cBox", "kBox", "mBox"
		};

		private bool IsInputingUserData { get => 
				accountingForm != null &&
                accountingForm.ActiveControl != null &&
                inputNames.Contains(accountingForm.ActiveControl.Name); }

        private void OnWinKeyDown(object sender, KeyEventArgs e)
		{
			Keys k = e.KeyCode;

			if (k == Keys.F11) {
				FullScreen();
				return;
			}

            if (IsInputingUserData)
                return;

            if (stage == Stage.SELECT_LANG)
			{
				ProceedSelectLang(k);
				return;
			}

			if (k == Keys.Enter) {
				if (stage == Stage.RULES) {
					SkipRules();
					return;
				}
				if (stage == Stage.GAME_RULES) {
					ProceedGameRulesSkip();
					return;
				}
				if (stage == Stage.PLAY_AGAIN) {
					PlayAgainSkip();
					return; 
				}
			}

			if (NumKeys.Contains(k) || k == Keys.Enter)
				DrawNum(k);
			if (k == Keys.Enter)
				ProceedInput();

			try { // can error if you press keyboard while app is launching
				//print($"\tKEY DOWN: {k}\n\t\tBLOCKED: {VideoChecker.blockInput}\n\t\tGAME ENDED: {VideoChecker.gameEnded}");
			} catch { }
		}

		private void EmulateShowParamsButton() => 
			accountingForm.Invoke(new Action(() => 
			accountingForm.showButton_Click(null, EventArgs.Empty)));

		private void PlayAgainSkip()
		{
			VideoChecker.continued = true;

			DoDataBaseGameRecord();
			EmulateShowParamsButton();
			//VideoChecker.SafeStop();
			// here need to play game video
		}

		private long SelectedGameTypeIs(GameType gameType) =>
			DbCurrentRecord.SelectedGameType == gameType 
				? DbCurrentRecord.SelectedLvl 
				: -1;

        public void SetBox(GameType gameType, long lvl) {
            accountingForm.Invoke(new Action(() => {
                switch (gameType) {
                    case GameType.Guard:
                        accountingForm.cBox.Text = lvl.ToString(); break;
                    case GameType.Painting:
                        accountingForm.kBox.Text = lvl.ToString(); break;
                    case GameType.Mario:
                        accountingForm.mBox.Text = lvl.ToString(); break;
                }
            }));
        }

        private void GameIndexOperations()
        {
            if (accountingForm.isFirstGame) {
                accountingForm.SetIsFirstGame(false);

                if (VideoChecker.won)
                    gameInfo.ClearGameIndicesAndSetFirst(0);
                gameInfo.IncGameIndex();
            }
            else if (VideoChecker.won)
                gameInfo.IncGameIndex();
            else
                gameInfo.IncLostCounter();
        }

		public void DoDataBaseGameRecord(bool DEBUG = false)
		{
            print("DOES DATABASE RECORD");
            long gameCLvl = SelectedGameTypeIs(GameType.Guard);
            long gameKLvl = SelectedGameTypeIs(GameType.Painting);
            long gameMLvl = SelectedGameTypeIs(GameType.Mario);

            long playerCLvl = long.Parse(accountingForm.cBox.Text);
            long playerKLvl = long.Parse(accountingForm.kBox.Text);
            long playerMLvl = long.Parse(accountingForm.mBox.Text);

            int[] counters = gameInfo.GetCounters();
            bool isFirstGame = accountingForm.isFirstGame;
            bool won = VideoChecker.won;
            long unixTimeInt = Db.Now;
            long prizeInt = DbCurrentRecord.SelectedPrize;
            long priceInt = DbCurrentRecord.SelectedPrice;

            GameIndexOperations();
            Console.WriteLine(gameInfo.CurrentScript);
            Console.WriteLine($"{accountingForm.cBox.Text}|{accountingForm.kBox.Text}|{accountingForm.mBox.Text}");

            long playerUpdCLvl = long.Parse(accountingForm.cBox.Text);
            long playerUpdKLvl = long.Parse(accountingForm.kBox.Text);
            long playerUpdMLvl = long.Parse(accountingForm.mBox.Text);

            long playerIdInt = long.Parse(accountingForm.playerNameBox.Text);

            Db.InsertInAllTables(
                playerIdInt: playerIdInt,
                unixTimeInt: unixTimeInt,
                playerCLvl: playerCLvl,
                playerKLvl: playerKLvl,
                playerMLvl: playerMLvl,

                playerUpdCLvl: playerUpdCLvl,
                playerUpdKLvl: playerUpdKLvl,
                playerUpdMLvl: playerUpdMLvl,

                gameCLvl: gameCLvl,
                gameKLvl: gameKLvl,
                gameMLvl: gameMLvl,

                wonBoolInt: won,
                continuedBoolInt: VideoChecker.continued,

                prizeInt: prizeInt,
                priceInt: priceInt,

                isFirstGame: isFirstGame,
                counters: counters
            );

            if (!VideoChecker.continued)
                accountingForm.SetLangLabel("#");

            VideoChecker.won = false;
            VideoChecker.continued = false;

            print($"REFRESH TABLES AFTER DOING DATABASE RECORD");
            accountingForm.Invoke(new Action(accountingForm.StartTables));
            accountingForm.RefreshDbForm();

            // after accountingForm.StartTables because refreshes GameBalance
            gameInfo.GameBalance = accountingForm.GameBalance;
            gameInfo.IncGameBalanceCounter();

            // after IncGameBalanceCounter becasue it changes GameMode so CurrentScript too
            GameScript nextGameScript = gameInfo.CurrentScript;
            accountingForm.SetGameScript(nextGameScript);

            if (!DEBUG) {
                VideoChecker
                .VlcChanged(
                    gameDirectory
                    .GetRandomGame(nextGameScript, VideoChecker.language));
            }

            Db.AppendBalanceSheet(unixTimeInt, won, priceInt, prizeInt, accountingForm.GameBalance);
        }

        private void ProceedInput()
		{
			print($"TRYED TO INPUT: {keysStreamtos()}");

			if (VideoChecker.blockInput || VideoChecker.gameEnded || VideoChecker.errorsCount > 2) // til 3 errors
			{
				print(
					$"VLCChecker.blockInput {VideoChecker.blockInput} || " +
					$"VLCChecker.gameEnded {VideoChecker.gameEnded} || " +
					$"VLCChecker.errorsCount > 2 {VideoChecker.errorsCount > 2}"
					);
				return;
			}

			VideoChecker.ProceedKeys(keysStream.Select(k => k.Key).ToArray());
		}

		private void ProceedGameRulesSkip() 
		{
            VideoChecker.StartPlayGameMainVideo();
		}

		public void PlayPlayAgain()
		{
			if (VideoChecker.gameVideosQueue.Count != 0) // its impossible but for sure
			{
				VideoChecker.gameVideosQueue.RemoveAt(0);
				if (VideoChecker.gameVideosQueue.Count > 0)
					VideoChecker.SetCode(VideoChecker.gameVideosQueue[0].Path);
				else
					accountingForm.Invoke(new Action(() => accountingForm.GotGameVideo("", "")));
			}
			Play(VideoChecker.currentLanguage.PlayAgain.Uri, Stage.PLAY_AGAIN);
		}

		public void PlayHowToPay()
		{
			Play(VideoChecker.currentLanguage.HowToPay.Uri, Stage.HOW_PO_PAY);
		}

		public void PlayGamePayed()
		{
			Play(VideoChecker.currentLanguage.GamePayed.Uri, Stage.GAME_PAYED);
        }

        public void PlayGameRules()
        {
            Play(VideoChecker.currentLanguage.GameRules.Uri, Stage.GAME_RULES);
        }

		private void DrawNum(Keys key)
		{
			keysStream.Add(new InputKey(key, fadeTime, inputLabel));
			inputLabel.Text += Utils.ktos[key];
		}

		public void DeleteInput()
		{
			inputLabel.Text = "";
			accountingForm.Invoke((MethodInvoker)delegate {
				accountingForm.DeleteInput();
			});
			foreach(InputKey key in keysStream)
				key.Dispose();
			keysStream.Clear();
		}

		private void ProceedSelectLang(Keys key)
		{
			DeleteInput();
			if (Utils.ktol.ContainsKey(key))
				VideoChecker.language = Utils.ktol[key];
			else
				return;

			Play(VideoChecker.currentLanguage.Rules.Uri, Stage.RULES);

            PathUri pathUri =
                accountingForm.isFirstGame
                ? gameDirectory.GetRandomGame(gameInfo.FirstGame, VideoChecker.language)
                : gameDirectory.GetRandomGame(gameInfo.CurrentScript, VideoChecker.language);
            VideoChecker.VlcChanged(pathUri);

            accountingForm.SetLangLabel(VideoChecker.language.View());

            new Thread(() =>
			{
				RelayChecker.Transmit(Channel.CAMERA_DOWN, true); // 5 seconds on to 2 channel
				Thread.Sleep(5000);
				RelayChecker.Transmit(Channel.CAMERA_DOWN, false); // off
			}).Start();
		}

		private void SkipRules()
		{
			DeleteInput();

			if (accountingForm.isFirstGame)
			{
				DbCurrentRecord.SetPricePrizeLvl(
						accountingForm.SelectedPrice,
						accountingForm.SelectedAward,
						accountingForm.SelectedLevel,
						accountingForm.SelectedGameType);
				VideoChecker.StartVideoInQueue();
			}
			else
				EmulateShowParamsButton(); // after EndGamePayed does StartVideoInQueue
	}
		#endregion
		#region SOME
		private void EndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
		{
			VideoChecker.MediaIndeedEnded(vlcControl.GetCurrentMedia().Mrl);
		}

		void TryDisposeAndNull(IDisposable disposable)
		{
			try
			{
				if (disposable != null)
				{
					disposable.Dispose();
					disposable = null;
				}
			} catch { }
		}

		private void MediaChanged(object sender, VlcMediaPlayerMediaChangedEventArgs e)
		{
			if (!VideoChecker.langs.Values.Any(l => l.Params.Uri.AbsoluteUri == e.NewMedia.Mrl))
			{
				BeginInvoke(new Action(() =>
				{
					TryDisposeAndNull(PrizeShowTimer);
					TryDisposeAndNull(CostShowTimer);
					prizeLabel.Hide();
					costLabel.Hide();
				}));
			}
		}

		int hmh(int global, int local = 0) => local == 0 ?
			global :
			global / 2 - local / 2;// Half Minus Half = hmh 

		private void AlignInputLabel(object sender, EventArgs e)
		{
			inputLabel.Location = new Point(
				hmh(Size.Width, inputLabel.Width),
				hmh(Size.Height, inputLabel.Height));

			accountingForm.Invoke((MethodInvoker)delegate {
				accountingForm.GotInput(inputLabel.Text);
			});
		}
		#endregion
		#region FORM_CLOSED
		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Environment.Exit(0);
		}
		#endregion
		#region SHOW_GAME_PARAMS_TO_PLAYER
		// consts
		const string paramsSpaces = "     ";

		const int heightCostOffset = 400;
		const int heightPrizeOffset = -452;

		readonly TimeSpan TimeToShowCost = TimeSpan.FromMilliseconds(6900);
		readonly TimeSpan TimeToShowPrize = TimeSpan.FromMilliseconds(9700);
		// time
		System.Threading.Timer CostShowTimer { get; set; }
		System.Threading.Timer PrizeShowTimer { get; set; }

		internal void ShowGameParams(long prize, long cost)
		{
			DeleteInput();
			prizeLabel.BringToFront();
			costLabel.BringToFront();
			prizeLabel.Hide();
			costLabel.Hide();

			prizeLabel.Text = paramsSpaces + prize.ToString() + paramsSpaces;
			costLabel.Text = paramsSpaces + cost.ToString() + paramsSpaces;

			Size vs = vlcControl.Size;

			prizeLabel.Location = new Point(
				hmh(vs.Width, prizeLabel.Size.Width),
				hmh(vs.Height, heightPrizeOffset));
			costLabel.Location = new Point(
				hmh(vs.Width, costLabel.Size.Width),
				hmh(vs.Height, heightCostOffset));

			vlcControl.Play(VideoChecker.currentLanguage.Params.Uri);

			CostShowTimer = new System.Threading.Timer(
				CostShowCallback, null, TimeToShowCost, InputKey.MinusOneMilisecond);
			PrizeShowTimer = new System.Threading.Timer(
				PrizeShowCallback, null, TimeToShowPrize, InputKey.MinusOneMilisecond);

			print(
				$"COST:  {accountingForm.SelectedPrice}\n" +
				$"PRIZE: {accountingForm.SelectedAward}\n" +
				$"LEVEL: {accountingForm.SelectedLevel}\n");
		}

		private void CostShowCallback(object state)
		{
			Invoke(new Action(() =>
			{
				costLabel.Show();
				CostShowTimer.Dispose();
				CostShowTimer = null;
			}));
		}

		private void PrizeShowCallback(object state)
		{
			Invoke(new Action(() =>
			{
				prizeLabel.Show();
				PrizeShowTimer.Dispose();
				PrizeShowTimer = null;
			}));
		}
		#endregion
		#region UPPER_PART_BUTTONS
		public void Replay()
		{
			BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => {
					if (vlcControl.GetCurrentMedia() != null)
					{
						vlcControl.Time = 0;
						vlcControl.Play(vlcControl.GetCurrentMedia().Mrl);
					}
				});
			}));
		}

		public void PlayIdle()
		{
			Play(VideoChecker.idle.Uri, Stage.IDLE);
			RelayChecker.Transmit(Channel.APPARAT_LIGHT, true); // highligh on

            //gameInfo.ClearGameIndicesAndSetFirst(0);
            gameInfo.ClearCounters();
		}

		public void Stop()
		{
			PlayIdle();
			VideoChecker.SafeStop();
            RelayChecker.CameraDownTrue();
        }

		public void StartGame()
		{
			print(accountingForm.isFirstGame);

			Play(VideoChecker.selectLang.Uri, Stage.SELECT_LANG);
			DeleteInput();

			new Thread(() =>
			{
				RelayChecker.Transmit(Channel.APPARAT_LIGHT, false); // highligh off
				Thread.Sleep(100);
				RelayChecker.Transmit(Channel.CAMERA_UP, true); // 5 seconds on to 1 channel
				Thread.Sleep(5000);
				RelayChecker.Transmit(Channel.CAMERA_UP, false); // off
			}).Start();
		}

		public void SkipStage()
		{
			BeginInvoke(new Action(() =>
			{
				switch (stage)
				{
					case Stage.IDLE: // how can skip this one
						break;
					case Stage.SELECT_LANG:
						ProceedSelectLang(Keys.D1);
						break;
					case Stage.RULES:
						SkipRules();
						break;
					case Stage.COST_AND_PRIZE:
						break;
					case Stage.GAME:
						PlayPlayAgain();
						break;
					case Stage.ERROR:
						break;
					case Stage.VICTORY:
						PlayPlayAgain();
						break;
                    case Stage.GAME_RULES:
						PlayPlayAgain();
						break;
                    case Stage.LEFT_SECONDS:
                        PlayPlayAgain();
                        break;
                    case Stage.GAME_NOT_WON:
                        PlayPlayAgain();
                        break;
                    case Stage.GAME_END:
                        PlayPlayAgain();
                        break;
                    case Stage.PLAY_AGAIN:
						//VideoChecker.PlayAgain(); // either idle or operator shows
						break;
					case Stage.HOW_PO_PAY:
						PlayGamePayed();
						break; 
					case Stage.GAME_PAYED:
						break; // operator starts game
					default:
						return;
				}
			}));
		}
		#endregion
	}
}
