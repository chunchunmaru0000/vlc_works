using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Vlc.DotNet;
using Vlc.DotNet.Core;

namespace vlc_works
{
	public partial class ClientForm : Form
	{
		#region VAR
		// global
		IKeyboardEvents hook { get; set; }
		public VLCChecker VLCChecker { get; set; }
		// forms
		public AccountingForm accountingForm { get; set; }
		// consts
		Keys[] NumKeys { get; } = new Keys[] {
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
			Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
		};
		readonly TimeSpan fadeTime = TimeSpan.FromSeconds(10);
		// input
		public List<InputKey> keysStream { get; set; } = new List<InputKey>();
		public Stage stage { get; set; }
		// some
		bool isFullScreen { get; set; } = false;
		public void print(object str = null)
		{
			string stroke = str == null ? "" : str.ToString();
			Console.WriteLine(stroke);
			//accountingForm.BeginInvoke(new Action(() => { accountingForm.DEBUG(stroke); }));
		}
		public string keysStreamtos() => string.Join("", keysStream.Select(k => VLCChecker.ktos[k.Key]));
		public static Uri url2mrl(string url) => new Uri(url);
		#endregion

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
			VLCChecker = new VLCChecker(this, accountingForm);
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
			if (stage == Stage.RULES && k == Keys.Enter)
			{
				SkipRules();
				return;
			}

			if (NumKeys.Contains(k) || k == Keys.Enter)
				DrawNum(k);
			if (k == Keys.Enter)
				ProceedInput();

			try
			{ // can error if you press keyboard while app is launching
				print($"\tKEY DOWN: {k}\n\t\tBLOCKED: {VLCChecker.blockInput}\n\t\tGAME ENDED: {VLCChecker.gameEnded}");
			} catch { }
		}

		void ProceedInput()
		{
			print($"TRYED TO INPUT: {keysStreamtos()}");

			if (VLCChecker.blockInput || VLCChecker.gameEnded || VLCChecker.errorsCount > 2) // til 3 errors
				return;

			VLCChecker.ProceedKeys(keysStream.Select(k => k.Key).ToArray());
		}

		void DrawNum(Keys key)
		{
			keysStream.Add(new InputKey(key, fadeTime, inputLabel));
			inputLabel.Text += VLCChecker.ktos[key];
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

		private void BeginLanguageInput()
		{
			stage = Stage.SELECT_LANG;
			DeleteInput();
		}

		private void ProceedSelectLang(Keys key)
		{
			DeleteInput();
			if (VLCChecker.ktol.ContainsKey(key))
				VLCChecker.language = VLCChecker.ktol[key];
			else
				return;

			stage = Stage.RULES;

			BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => vlcControl.Play(VLCChecker.ltour[VLCChecker.language]));
			}));
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
			VLCChecker.MediaIndeedEnded(vlcControl.GetCurrentMedia().Mrl);
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
			if (!VLCChecker.IsParamsMrl(e.NewMedia.Mrl))
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

			vlcControl.Play(VLCChecker.ltoup[VLCChecker.language]);
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
					vlcControl.Time = 0;
					vlcControl.Play(vlcControl.GetCurrentMedia().Mrl);
				});
			}));
		}

		public void PlayIdle()
		{
			BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => vlcControl.Play(VLCChecker.idleUri));
			}));
		}

		public void Stop()
		{
			BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => vlcControl.Stop());
			}));
		}

		public void StartGame()
		{
			BeginInvoke(new Action(() =>
			{
				ThreadPool.QueueUserWorkItem(_ => vlcControl.Play(VLCChecker.selectLangUri));
				BeginLanguageInput();
			}));
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
						VLCChecker.language = Langs.HEBREW;
						stage = Stage.RULES;
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
						break;
					default:
						return;
				}
			}));
		}
		#endregion
	}
}
