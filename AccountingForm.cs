using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace vlc_works
{
	public partial class AccountingForm : Form
	{
		TextSettings settings { get; set; }
		// until here operator form
		ClientForm clientForm { get; set; }
		// sql
		List<DbSelectGamesItem> GamesItems { get; set; }
		// selects
		public long SelectedAward { get; set; }
		public long SelectedLevel { get; set; }
		public long SelectedPrice { get; set; }
		// some long values
		long Balance { get; set; }
		long WinsSum { get; set; }
		long PaysSum { get; set; }
		// consts
		const string NullText = "####";
		Dictionary<Button, long> AwardBut2long { get; set; }
		Dictionary<Button, long> LevelBut2long { get; set; }
		Dictionary<Button, long> PriceBut2long { get; set; }
		// some

		public AccountingForm(ClientForm clientForm)
		{
			InitializeComponent();
			this.clientForm = clientForm;

			InitSettings();
			InitDictionares();
			InitButtons();
			InitBalance();
			StartTables();
		}

		#region SOME_INITS
		private void InitSettings()
		{
			settings = TextSettings.ReadSettings();

			clientForm.Invoke((MethodInvoker)delegate
			{
				clientForm.inputLabel.Font = settings.Font;
				clientForm.inputLabel.ForeColor = settings.ForeColor;
				clientForm.inputLabel.BackColor = settings.BackColor;
			});
		}

		private void InitBalance()
		{
			//throw new NotImplementedException();
		}
		#endregion
		#region SELECT_BUTTONS	
		private void InitDictionares()
		{
			AwardBut2long = new Dictionary<Button, long>()
			{
				{ award30But, 30 },     { award50But, 50 },
				{ award80But, 80 },     { award100But, 100 },
				{ award150But, 150 },   { award200But, 200 },
				{ award300But, 300 },   { award500But, 500 },
				{ award1000But, 1000 }, { award3000But, 3000 },
			};
			LevelBut2long = new Dictionary<Button, long>()
			{
				{ lvl0But, 0 },         { lvl1But, 1 },
				{ lvl2But, 2 },         { lvl3But, 3 },
				{ lvl4But, 4 },         { lvl5But, 5 },
				{ lvl6But, 6 },         { lvl7But, 7 },
				{ lvl8But, 8 },         { lvl9But, 9 },
			};
			PriceBut2long = new Dictionary<Button, long>()
			{
				{ price0But, 0 },       { price20But, 20 },
				{ price30But, 30 },     { price40But, 40 },
				{ price50But, 50 },     { price100But, 100 },
				{ price200But, 200 },
			};
		}

		private void InitButtons()
		{
			award30But.Click +=   OnAwardButClicked;
			award50But.Click +=   OnAwardButClicked;
			award80But.Click +=   OnAwardButClicked;
			award100But.Click +=  OnAwardButClicked;
			award150But.Click +=  OnAwardButClicked;
			award200But.Click +=  OnAwardButClicked;
			award300But.Click +=  OnAwardButClicked;
			award500But.Click +=  OnAwardButClicked;
			award1000But.Click += OnAwardButClicked;
			award3000But.Click += OnAwardButClicked;

			lvl0But.Click += OnLevelButClicked;
			lvl1But.Click += OnLevelButClicked;
			lvl2But.Click += OnLevelButClicked;
			lvl3But.Click += OnLevelButClicked;
			lvl4But.Click += OnLevelButClicked;
			lvl5But.Click += OnLevelButClicked;
			lvl6But.Click += OnLevelButClicked;
			lvl7But.Click += OnLevelButClicked;
			lvl8But.Click += OnLevelButClicked;
			lvl9But.Click += OnLevelButClicked;

			price0But.Click +=   OnPriceButClicked;
			price20But.Click +=  OnPriceButClicked;
			price30But.Click +=  OnPriceButClicked;
			price40But.Click +=  OnPriceButClicked;
			price50But.Click +=  OnPriceButClicked;
			price100But.Click += OnPriceButClicked;
			price200But.Click += OnPriceButClicked;
		}

		private void OnAwardButClicked(object sender, EventArgs e)
		{
			Button awardButton = sender as Button;

			SelectedAward = AwardBut2long[awardButton];
			awardLabel.Text = SelectedAward.ToString();
		}

		private void OnLevelButClicked(object sender, EventArgs e)
		{
			Button levelButton = sender as Button;

			SelectedLevel = LevelBut2long[levelButton];
			levelLabel.Text = SelectedLevel.ToString();
		}

		private void OnPriceButClicked(object sender, EventArgs e)
		{
			Button priceButton = sender as Button;

			SelectedPrice = PriceBut2long[priceButton];
			priceLabel.Text = SelectedPrice.ToString();
		}
		#endregion
		#region TABLES
		private void LogData()
		{
			Console.WriteLine("<------- DATABASE DATA ------->");
			Console.WriteLine(string.Join("\n", GamesItems.Select(g => g.ToString())));
			Console.WriteLine("<------- DATABASE  END ------->");
		}

		private DataGridViewRow GetRowWithTextCell(string cellText)
		{
			DataGridViewRow row = new DataGridViewRow();
			DataGridViewTextBoxCell rowCell = new DataGridViewTextBoxCell()
			{
				Value = cellText,
			};
			row.Cells.Add(rowCell);
			row.Height = 32;
			return row;
		}

		private long RefreshGridReturnSum(ref DataGridView grid, string commandString)
		{
			grid.Rows.Clear();

			long[] allSomething = Db.SelectAllLong(commandString);

			grid.Rows.AddRange(allSomething
				.Select(l => GetRowWithTextCell(l.ToString()))
				.ToArray());

			return allSomething.Sum();
		}

		private void StartTables()
		{
			Db.BeginSQL();

			WinsSum = RefreshGridReturnSum(ref winsDataGridView, Db.selectAllAwards);
			PaysSum = RefreshGridReturnSum(ref priceDataGridView, Db.selectAllPrices);

			winSumLabel.Text = WinsSum.ToString();
			priceSumLabel.Text = PaysSum.ToString();
			balanceLabel.Text = (PaysSum - WinsSum).ToString();
		}
		#endregion
		#region FORM_CLOSED
		private void AccountingForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Db.EndSQL();
			Environment.Exit(0);
		}
		#endregion
		#region OPERATOR_FORM
		// <-------------- OPERATOR FORM BELOW -------------->
		// <-------------- OPERATOR FORM BELOW -------------->
		// <-------------- OPERATOR FORM BELOW -------------->
		// <-------------- OPERATOR FORM BELOW -------------->
		// <-------------- OPERATOR FORM BELOW -------------->

		//public void DEBUG(string mesasge) => debugLabel.Text = mesasge;

		public void DeleteInput()
		{
			inputLabel.Text = "";
		}

		public void GotGameVideo(string videoGamePath, string code)
		{
			codeLabel.Text = code;
		}

		public void GotInput(string input)
		{
			inputLabel.Text = input;

			inputLabel.ForeColor = codeLabel.Text.TrimEnd('E') == input.TrimEnd('E') ?
				Color.Green : // good
				Color.Black;   // usual
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

		// <-------------- OPERATOR FORM ABOVE -------------->
		// <-------------- OPERATOR FORM ABOVE -------------->
		// <-------------- OPERATOR FORM ABOVE -------------->
		// <-------------- OPERATOR FORM ABOVE -------------->
		// <-------------- OPERATOR FORM ABOVE -------------->
		#endregion
		#region DOWN_PART_BUTTONS
		private void showButton_Click(object sender, EventArgs e)
		{
			if (
				awardLabel.Text != NullText &&
				levelLabel.Text != NullText &&
				priceLabel.Text != NullText
				)
			{
				Db.Insert(SelectedAward, SelectedPrice, SelectedLevel, Db.Now);
				StartTables();
				clientForm.Invoke(new Action(() =>
				{
					clientForm.ShowGameParams(SelectedAward, SelectedPrice);
				}));
			}
			else
				MessageBox.Show(
					$"НЕ ВСЕ ЗНАЧЕНИЯ ВЫБРАНЫ\n" +
					$"ПРИЗ:      {awardLabel.Text}\n" + 
					$"УРОВЕНЬ:   {levelLabel.Text}\n" +
					$"СТОИМОСТЬ: {priceLabel.Text}");
		}

		private void dropWinsBut_Click(object sender, EventArgs e)
		{
			if (winsDataGridView.Rows.Count == 0)
				return;

			Db.DropTable("awards");
			StartTables();
		}

		private void dropPriceBut_Click(object sender, EventArgs e)
		{
			if (priceDataGridView.Rows.Count == 0)
				return;

			Db.DropTable("prices");
			StartTables();
		}

		private void payBut_Click(object sender, EventArgs e)
		{

		}

		private void giveCardBut_Click(object sender, EventArgs e)
		{
			// throw new NotImplementedException();
		}

		private void returnMoneyBut_Click(object sender, EventArgs e)
		{
			// throw new NotImplementedException();
		}
		#endregion
	}
}
