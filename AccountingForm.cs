using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace vlc_works
{
	public partial class AccountingForm : Form
	{
		ClientForm clientForm { get; set; }
		// sql
		List<DbSelectGamesItem> GamesItems { get; set; }
		// selects
		long SelectedAward { get; set; }
		long SelectedLevel { get; set; }
		long SelectedPrice { get; set; }
		// consts
		Dictionary<Button, long> AwardBut2long { get; set; }
		Dictionary<Button, long> LevelBut2long { get; set; }
		Dictionary<Button, long> PriceBut2long { get; set; }
		// some
		long Now { get { return DateTimeOffset.Now.ToUnixTimeSeconds(); } }
		DateTimeOffset SecToTime(long unixSeconds) => DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

		public AccountingForm(ClientForm clientForm)
		{
			InitializeComponent();
			InitDictionares();
			InitButtons();

			this.clientForm = clientForm;

			Db.BeginSQL();
			StartTables();
		}

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
			award30But.Click += OnAwardButClicked;
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

		private DataGridViewRow GetRowWithTextCell(string cellText)
		{
			DataGridViewRow row = new DataGridViewRow();
			DataGridViewTextBoxCell rowCell = new DataGridViewTextBoxCell()
			{
				Value = cellText,
			};
			row.Cells.Add(rowCell);
			return row;
		}

		private void StartTables()
		{
			GamesItems = Db.SelectAllGames().ToList();

			foreach(DbSelectGamesItem game in GamesItems)
			{
				if (game.GameAward > 0)
					winsDataGridView.Rows.Add(GetRowWithTextCell(game.GameAward.ToString()));

				priceDataGridView.Rows.Add(GetRowWithTextCell(game.GamePrice.ToString()));
			}
		}

		private void AccountingForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Db.EndSQL();
			Environment.Exit(0);
		}
	}
}
