using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vlc_works
{
	public partial class OperatorForm : Form
	{
		ClientForm clientForm { get; set; }
		TextSettings settings { get; set; }

		public OperatorForm(ClientForm clientForm)
		{
			InitializeComponent();

			this.clientForm = clientForm;
			settings = TextSettings.ReadSettings();

			clientForm.Invoke((MethodInvoker)delegate 
			{
				clientForm.inputLabel.Font = settings.Font;
				clientForm.inputLabel.ForeColor = settings.ForeColor;
				clientForm.inputLabel.BackColor = settings.BackColor;
			});
		}

		public void DEBUG(string mesasge)
		{
			debugLabel.Text = mesasge;
		}

		public void DeleteInput()
		{
			inputLabel.Text = "";
		}

		public void GotWinErrPaths(string winPath, string defeatPath)
		{
			winLabel.Text =   $"Victory video path: {winPath}";
			errorLabel.Text = $"Error video path:   {defeatPath}";
		}

		public void GotGameVideo(string videoGamePath, string code)
		{
			videoLabel.Text = $"Game video path:    {videoGamePath}";
			codeLabel.Text = code;
		}

		public void GotInput(string input)
		{
			inputLabel.Text = input;

			inputLabel.ForeColor = codeLabel.Text.TrimEnd('E') == input.TrimEnd('E') ? 
				Color.LightGreen : // good
				Color.LightGray;   // usual
		}

		private void foreColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				clientForm.Invoke((MethodInvoker)delegate { 
					clientForm.inputLabel.ForeColor = colorDialog.Color; 
				});
				settings.ForeColor = colorDialog.Color;
			}
		}

		private void backToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				clientForm.Invoke((MethodInvoker)delegate {
					clientForm.inputLabel.BackColor = colorDialog.Color;
				});
				settings.BackColor = colorDialog.Color;
			}
		}

		private void sizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (fontDialog.ShowDialog() == DialogResult.OK)
			{
				clientForm.Invoke((MethodInvoker)delegate {
					clientForm.inputLabel.Font = fontDialog.Font;
				});
				settings.Font = fontDialog.Font;
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settings.Save();
			Console.WriteLine("SAVED SETTINGS");
		}

		private void OperatorForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Environment.Exit(0);
		}
	}
}
