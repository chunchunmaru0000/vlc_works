using System;
using System.Windows.Forms;

namespace vlc_works015
{
	public class InputKey: IDisposable
	{
		public Keys Key { get; set; }
		private Label InputLabel { get; set; }
		private System.Threading.Timer CeaseTimer { get; set; }
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

					cf.print($"DELETED: {Utils.ktos[Key]}\n\tNOW STREAM: {cf.keysStreamtos()}");
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
