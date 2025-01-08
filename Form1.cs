using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
	public partial class Form1 : Form
	{
		// global
		IKeyboardEvents hook { get; set; }
		VLCChecker VLCChecker { get; set; }
		OperatorForm operatorForm { get; set; }
		// consts
		Keys[] NumKeys { get; } = new Keys[] {
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
			Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
		};
		// input
		List<Keys> keysStream { get; set; } = new List<Keys>();
		DateTime lastInput = DateTime.Now;
		// some
		bool isFullScreen { get; set; } = false;
		void print(object str = null) => Console.WriteLine(str);
		readonly TimeSpan fadeTime = TimeSpan.FromSeconds(3);
		public static Uri url2mrl(string url) => new Uri(url);

		public Form1()
		{
			//this.vlcControl.VlcLibDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libvlc\\win-x86"));
			InitializeComponent();
			new Thread(() => { Thread.Sleep(3000); AutoDelete(); }).Start();
			Show();
			// key logger
			hook = Hook.GlobalEvents();
			hook.KeyUp += OnWinKeyDown;
			// operator form
			operatorForm = new OperatorForm(this);
			operatorForm.Show();
			// set vlc
			vlcControl.EndReached += EndReached;
			// cheker
			VLCChecker = new VLCChecker(this, operatorForm);
			// set form
			Form1_SizeChanged(inputLabel, EventArgs.Empty); // includes align inputLabel
			inputLabel.SizeChanged += AlignInputLabel;
			DeleteInput();
			SetFormFullScreen();
		}

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

		private void EndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
		{
			VLCChecker.MediaIndeedEnded(vlcControl.GetCurrentMedia().Mrl);
		}

		private void OnWinKeyDown(object sender, KeyEventArgs e)
		{
			Keys k = e.KeyCode;

			if (k == Keys.F11)
			{
				FullScreen();
				return;
			}

			if (NumKeys.Contains(k) || k == Keys.Enter)
				DrawNum(k);
			if (k == Keys.Enter)
				ProceedInput();

			print($"\tKEY DOWN: {k}\n\t\tBLOCKED: {VLCChecker.blockInput}\n\t\tGAME ENDED: {VLCChecker.gameEnded}");
		}

		void ProceedInput()
		{
			if (VLCChecker.blockInput || VLCChecker.gameEnded)
				return;

			VLCChecker.ProceedKeys(keysStream.ToArray());
		}

		void DrawNum(Keys key)
		{
			keysStream.Add(key);
			lastInput = DateTime.Now + fadeTime;
			inputLabel.Text += VLCChecker.ktos[key];
		}

		public void DeleteInput()
		{
			inputLabel.Text = "";
			operatorForm.Invoke((MethodInvoker)delegate {
				operatorForm.DeleteInput();
			});
			keysStream.Clear();
		}

		void AutoDelete()
		{
			new Thread(() =>
			{
				while (true)
				{
					if (DateTime.Now >= lastInput)
						this.Invoke((MethodInvoker)delegate { DeleteInput(); });
					Thread.Sleep(30);
				}
			}).Start();
		}

		void FullScreen()
		{
			if (isFullScreen)
			{
				FormBorderStyle = FormBorderStyle.Sizable;
				WindowState = FormWindowState.Normal;
				Size = operatorForm.Size;
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

		int hmh(int global, int local = 0) => local == 0 ?
			global :
			global / 2 - local / 2;// Half Minus Half = hmh 

		private void AlignInputLabel(object sender, EventArgs e)
		{
			inputLabel.Location = new Point(
				hmh(Size.Width, inputLabel.Width),
				hmh(Size.Height, inputLabel.Height));

			operatorForm.Invoke((MethodInvoker)delegate {
				operatorForm.GotInput(inputLabel.Text);
			});
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Environment.Exit(0);
		}
	}
}
