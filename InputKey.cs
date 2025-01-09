using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vlc_works
{
	public class InputKey: IDisposable
	{
		public Keys Key { get; set; }
		Label InputLabel { get; set; }
		System.Threading.Timer CeaseTimer { get; set; }
		// static
		public static readonly TimeSpan MinusOneMilisecond = TimeSpan.FromMilliseconds(-1);

		public InputKey(Keys key, TimeSpan fadeTime, Label inputLabel)
		{
			Key = key;
			InputLabel = inputLabel;

			CeaseTimer = new System.Threading.Timer(CeaseTimerCallback, null,  fadeTime, MinusOneMilisecond);
		}

		private void CeaseTimerCallback(object state)
		{
			if (InputLabel.Text != "")
			{
				ClientForm cf = InputLabel.FindForm() as ClientForm;
				cf.Invoke(new Action(() =>
				{
					InputLabel.Text = InputLabel.Text.Substring(1, InputLabel.Text.Length - 1);
					cf.keysStream.RemoveAt(0);

					cf.print($"DELETED: {VLCChecker.ktos[Key]}\n\tNOW STREAM: {cf.keysStreamtos()}");
				}));
			}
			Dispose();
		}

		public void Dispose() 
		{
			CeaseTimer?.Dispose();
		}
	}
}
