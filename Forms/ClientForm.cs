﻿using System;
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
		#endregion UNCHANGING_VAR
		#region CONSTS
		private Keys[] NumKeys { get; } = new Keys[] // keys of numpad
		{
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
			Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
		};
		readonly TimeSpan fadeTime = TimeSpan.FromSeconds(10); // key fade time
		readonly TimeSpan NSeconds = TimeSpan.FromSeconds(67); // N secongs - 1 because for sure
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
			// accounting form
			accountingForm = new AccountingForm(this);
			accountingForm.Show();
			// set vlc
			vlcControl.EndReached += EndReached;
			vlcControl.MediaChanged += MediaChanged;
			// cheker
			//VLCChecker = new VLCChecker(this, accountingForm);
			VLCChecker.Constructor(this, accountingForm);
			// set form
			Form1_SizeChanged(inputLabel, EventArgs.Empty); // includes align inputLabel
			inputLabel.SizeChanged += AlignInputLabel;
			DeleteInput();
			SetFormFullScreen();
		}

		#region SCREEN
		void SetFormFullScreen()
		{
			// of course its better but im not sure in screens order
			/* 
			Screen[] screens = Screen.AllScreens;
			screen = screens.Length > 1 ? 
				screens[1] : 
				screens[0];
			StartPosition = FormStartPosition.Manual;
			Location = PointToScreen(new Point(
				(screens.Length > 1 ? screens[0].Bounds.Width : 0) + hmh(screen.Bounds.Width), 0));
			*/
			// need right monitor
			Location = new Point(2000, 100);
			print($"X: {Location.X}");
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
			Invoke(new Action(() =>
			{
				stage = s;
				ThreadPool.QueueUserWorkItem(_ => vlcControl.Play(uri));
			}));
		}

		private void OnWinKeyDown(object sender, KeyEventArgs e)
		{
			Keys k = e.KeyCode;

			if (k == Keys.F11)
			{
				FullScreen();
				return;
			}

			if (stage == Stage.SELECT_LANG)
			{
				ProceedSelectLang(k);
				return;
			}

			if (k == Keys.Enter)
			{
				if (stage == Stage.RULES)
				{
					SkipRules();
					return;
				}
				if (stage == Stage.GAME && vlcControl.Time < NSeconds.TotalMilliseconds)
				{
					ProceedVideoBeginSkip();
					return;
				}
				if (stage == Stage.PLAY_AGAIN)
				{
					PlayAgainSkip();
					return; 
				}
			}

			if (NumKeys.Contains(k) || k == Keys.Enter)
				DrawNum(k);
			if (k == Keys.Enter)
				ProceedInput();

			try
			{ // can error if you press keyboard while app is launching
				print($"\tKEY DOWN: {k}\n\t\tBLOCKED: {VideoChecker.blockInput}\n\t\tGAME ENDED: {VideoChecker.gameEnded}");
			} catch { }
		}

		private void PlayAgainSkip()
		{
			// according to Aizen's plan here operator will show params so just SafeStop()
			VideoChecker.SafeStop();
			VideoChecker.AbortThreads(); // abort threads because here afterPlayAgainWaitThread can be alive
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

		private void ProceedVideoBeginSkip() 
		{
			vlcControl.Time = Convert.ToInt64(NSeconds.TotalMilliseconds) + 1000;
		}

		public void PlayPlayAgain()
		{
			print($"PLAY AGAIN? {VideoChecker.currentLanguage.PlayAgain.Uri.AbsolutePath}");
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
		}

		private void SkipRules()
		{
			DeleteInput();
			stage = Stage.COST_AND_PRIZE;
			BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => vlcControl.Stop());
			}));
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
		}

		public void Stop()
		{
			VideoChecker.AbortThreads();
			BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => vlcControl.Stop());
			}));
		}

		public void StartGame()
		{
			print(accountingForm.isFirstGame);
			VideoChecker.AbortThreads();

			if (accountingForm.isFirstGame)
			{
				accountingForm.SetAward(0); // in the first game award = 0
			}
			else
			{
				// the price will be setted by operator but anyway here need to do 
				// use Recommendator class for operator recommendations
			}

			Play(VideoChecker.selectLang.Uri, Stage.SELECT_LANG);
			DeleteInput();
		}

		public void SkipStage()
		{
			BeginInvoke(new Action(() =>
			{
				VideoChecker.AbortThreads();
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
						break;
					case Stage.ERROR:
						break;
					case Stage.GAME_CANT_INPUT:
						break;
					case Stage.VICTORY:
						//PlayIdle();
						break;
					case Stage.PLAY_AGAIN:
						//VideoChecker.PlayAgain();
						break;
					case Stage.HOW_PO_PAY:
						PlayGamePayed();
						break; 
					case Stage.GAME_PAYED:
						break; // operator starts
					default:
						return;
				}
			}));
		}
		#endregion
	}
}
