using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Vlc.DotNet.Core;

namespace vlc_works
{
	public partial class ClientForm : Form
	{
		// consts
		const string paramsSpaces = "     ";
		const string paramsVideoPath = "showParamsVideo.mp4";
		public static readonly Uri ParamsVideoUri = new Uri(
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, paramsVideoPath));

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

			vlcControl.Play(ParamsVideoUri);
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
			}));
		}

		private void PrizeShowCallback(object state)
		{
			Invoke(new Action(() =>
			{
				prizeLabel.Show();
				PrizeShowTimer.Dispose();
			}));
		}
	}
}