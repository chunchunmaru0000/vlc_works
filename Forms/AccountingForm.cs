using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace vlc_works
{
	public partial class AccountingForm : Form
	{
		#region VAR
		private TextSettings settings { get; set; }
		// until here operator form
		public ClientForm clientForm { get; set; }
		// sql
		private List<DbSelectGamesItem> GamesItems { get; set; }
		// selects
		public long SelectedAward { get; set; }
		public long SelectedLevel { get; set; }
		public long SelectedPrice { get; set; }
		public long CoinsInStock { get; set; }
		// some long values
		private long Balance { get; set; }
		private long WinsSum { get; set; }
		private long PaysSum { get; set; }
		public static long Game_id { get; set; }
		// consts
		private const string NullText = "####";
		private Dictionary<Button, long> AwardBut2long { get; set; }
		private Dictionary<Button, long> LevelBut2long { get; set; }
		private Dictionary<Button, long> PriceBut2long { get; set; }
		public const int oneCoinShekels = 10;
		public const int oneCommandCoins = 1;
		// some
		public bool isFirstGame { get; set; } = true;
		#endregion

		public AccountingForm(ClientForm clientForm)
		{
			InitializeComponent();
			this.clientForm = clientForm;

			InitSettings();
			InitDictionares();
			InitButtons();
			InitBalance();
			StartTables();

			InitClearFocusThread();
		}

		#region SOME_INITS
		private void InitClearFocusThread()
		{
			new Thread(() => {
				Thread.Sleep(2000);
				while (true)
				{
					Thread.Sleep(33);
					if (ActiveControl != null && ActiveControl.Name != "comBox")
					{
						Thread.Sleep(100);
						Console.WriteLine("CLEARED");
						Invoke(new Action(() =>
						{
							ActiveControl = null; // clear focus
							//Focus();
						}));
					}
				}
			}).Start();
		}

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
		        { award50But, 50 },
				{ award250But, 250 },   { award100But, 100 },
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
			award50But.Click +=   OnAwardButClicked;
			award250But.Click +=   OnAwardButClicked;
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

		public void SetAward(long award)
		{
			SelectedAward = award;
			Invoke(new Action(() => awardLabel.Text = SelectedAward.ToString()));
		}

		public void SetPrice(long price)
		{
			SelectedPrice = price;
			Invoke(new Action(() => priceLabel.Text = SelectedPrice.ToString()));
		}

		public void SetCoinsInStock(long coins)
		{
			Invoke(new Action(() =>
			{
				CoinsInStock = coins;
			}));
		}

		public void IncBalance()
		{
			Invoke(new Action(() =>
			{
				Balance += oneCoinShekels;
				Console.WriteLine($"BALANCE + {oneCoinShekels} NOW IS {Balance} AND CURRENT STAGE IS {clientForm.stage}");
				Console.WriteLine($"SELECTED PRICE NOW IS {SelectedPrice}");

				if (Balance >= SelectedPrice)
				{
					if (clientForm.stage == Stage.HOW_PO_PAY)
					{
						clientForm.PlayGamePayed();
						Balance -= SelectedPrice;

						Db.InsertPrice(Game_id, SelectedPrice);
						StartTables(); // refersh tables
					}
					payedCountLabel.Text = Balance.ToString() + " ПЕРЕПЛАТА";
				}
				else
					payedCountLabel.Text = Balance.ToString();
			}));
		}

		private void OnAwardButClicked(object sender, EventArgs e)
		{
			Button awardButton = sender as Button;

			SetAward(AwardBut2long[awardButton]);
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

		public void StartTables()
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
				//Db.InsertAll(SelectedAward, SelectedPrice, SelectedLevel, Db.Now);
				//StartTables();
				clientForm.Invoke(new Action(() =>
				{
					clientForm.ShowGameParams(SelectedAward, SelectedPrice);
					clientForm.stage = Stage.COST_AND_PRIZE;
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
			// does nothing for now
			// because operator himself starts game video
			// when payed or dunno onle case for now is
			// when the price is zero
			// if game automat itself will have function to say that game is payed
			// then will be another function for that
			Db.InsertGame(SelectedLevel, Db.Now);
			Game_id = Db.GetMaxGamesId();
			Db.InsertPrice(Game_id, SelectedPrice);
			
			StartTables(); // refresh tables
		}

		private void giveCardBut_Click(object sender, EventArgs e)
		{
			// gives card but its not even know
			// will it be or not in the future
			// throw new NotImplementedException();
		}

		private void returnMoneyBut_Click(object sender, EventArgs e)
		{
			// also should be used via COM port
			// throw new NotImplementedException();
		}
		#endregion
		#region UPPER_PART_BUTTONS
		private void playIdleBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(() => clientForm.PlayIdle()));
		}

		private void stopBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(() => clientForm.Stop()));
		}

		private void replayBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(() => clientForm.Replay()));
		}

		private void startGameBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(() => clientForm.StartGame()));
		}

		private void skipStageBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(() => clientForm.SkipStage()));
		}

		private void hideCodeBut_Click(object sender, EventArgs e)
		{
			if (codeLabel.Visible)
			{
				codeLabel.Hide();
				inputLabel.Hide();
			}
			else
			{
				codeLabel.Show();
				inputLabel.Show();
			}
		}
		#endregion
		#region COM
		private void comBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(() => COMPort.TryConnectPort(comBox.Text, this)));
			ActiveControl = null;
		}

		private void comBox_DropDown(object sender, EventArgs e)
		{
			comBox.Items.Clear();
			comBox.Items.AddRange(SerialPort.GetPortNames());
		}
		#endregion
		#region FACE_CONTROL
		private void faceControlBut_Click(object sender, EventArgs e)
		{

		}
		#endregion FACE_CONTROL
		#region TEMPORAL_CONTROLS
		public void SetIsFirstGame(bool firstGame)
		{
			Invoke(new Action(() =>
			{
				isFirstGame = !firstGame;
				isFirstGameBut_Click(null, EventArgs.Empty);
			}));
		}

		private void isFirstGameBut_Click(object sender, EventArgs e)
		{
			const string on = "Режим первой игры: Включено";
			const string off = "Режим первой игры: Выключено";

			if (isFirstGame)
			{
				isFirstGameBut.BackColor = Color.LawnGreen;
				isFirstGameBut.Text = off;
			}
			else
			{
				isFirstGameBut.BackColor = Color.Coral;
				isFirstGameBut.Text = on;
			}

			isFirstGame = !isFirstGame;
		}
		#endregion TEMPORAL_CONTROLS
	}
}
