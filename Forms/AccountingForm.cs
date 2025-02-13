using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using UsbRelayNet.RelayLib;

namespace vlc_works
{
	public partial class AccountingForm : Form
	{
		#region VAR
		private TextSettings settings { get; set; }
		// until here operator form
		public ClientForm clientForm { get; set; }
		// selects
		public long SelectedAward { get; set; }
		public long SelectedLevel { get; set; }
		public long SelectedPrice { get; set; }
		public long CoinsInStock { get; set; }
		public GameType SelectedGameType { get; set; } = GameType.Painting;
		private string SelectedTimeString { get => 
				DateTimeOffset.Now
				.ToString("HH:mm     d MMMM yyyy", new System.Globalization.CultureInfo("ru")); }
		// some long values
		private long GameBalance { get; set; } // all balance that is PaysSum - WinsSum + Balance
		private long PayedBalance { get; set; } // balance of shekels payed
		private long WinsSum { get; set; }
		private long PaysSum { get; set; }
		public static long Game_id { get; set; }
		// consts
		private const string NullText = "####";
		private Dictionary<Button, long> LevelBut2long { get; set; }
		private Dictionary<Button, long> PriceBut2long { get; set; }
		private Dictionary<Button, GameType> ButToGameType { get; set; }
		public const int oneCoinShekels = 10;
		public const int oneCommandCoins = 1;
		// some
		public bool isFirstGame { get; set; } = true;
		#endregion

		public AccountingForm(ClientForm clientForm)
		{
			InitializeComponent();
			InitTimeLabelThread();
			this.clientForm = clientForm;

			InitSettings();
			InitDictionares();
			InitAwardGrid();
			InitButtons();
			InitBalance();
			StartTables();

			InitClearFocusThread();
		}

		#region SOME_INITS
		private void InitTimeLabelThread()
		{
			new Thread(() =>
			{
				Thread.Sleep(5000);
				while (true)
				{
					Invoke(new Action(() => timeLabel.Text = SelectedTimeString));
					Thread.Sleep(500);
				}
			}).Start();
		}

		private void InitClearFocusThread()
		{
			string[] excludeControls = new string[] 
			{ "comBox", "cBox", "kBox", "mBox", "playerNameBox", "relayBox" };

			new Thread(() => {
				Thread.Sleep(2000);
				while (true)
				{
					Thread.Sleep(33);
					if (ActiveControl != null && !excludeControls.Contains(ActiveControl.Name))
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

		private void InitDictionares()
		{
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
			ButToGameType = new Dictionary<Button, GameType>()
			{
				{ cBut, GameType.Guard },
				{ kBut, GameType.Painting },
				{ mBut, GameType.Mario },
			};
		}

		private DataGridViewRow GetAwardGridRow(long value)
		{
			DataGridViewRow row = new DataGridViewRow();
			row.Cells.Add(new DataGridViewButtonCell() 
			{ 
				Value = value,
				FlatStyle = FlatStyle.Popup,
			});
			row.Height = 32;
			return row;
		}

		private void InitAwardGrid()
		{
			long[] values = new long[] 
			{ 
				0, 20, 30, 40, 50, 60, 70, 80, 90, 100, 
				150, 200, 250, 300, 500, 1000, 3000 
			};

			prizeButsGrid.Rows.AddRange(values.Select(v => GetAwardGridRow(v)).ToArray());
		}

		private void prizeButsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			long prize = (long)prizeButsGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
			SetAward(prize);
		}

		private void InitButtons()
		{
			doOnlyDark(cBut);

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
		#endregion
		#region SET_INC_DEC
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

		public void SetLvl(long lvl)
		{
			SelectedLevel = lvl;
			Invoke(new Action(() => levelLabel.Text = SelectedLevel.ToString()));
		}

		public void IncCoinsInStock(long coins)
		{
			Invoke(new Action(() =>
			{
				CoinsInStock += coins;
				IncBalance(coins * oneCoinShekels);
			}));
		}

		public void DecBalance(long shekels) => IncBalance(-shekels);

		public void IncBalance(long shekels)
		{
			Invoke(new Action(() =>
			{
				PayedBalance += shekels;
				Console.WriteLine($"BALANCE + {shekels} NOW IS {PayedBalance} AND CURRENT STAGE IS {clientForm.stage}");
				Console.WriteLine($"SELECTED PRICE NOW IS {DbCurrentRecord.SelectedPrice}");

				if (PayedBalance >= SelectedPrice)
				{
					if (clientForm.stage == Stage.HOW_PO_PAY || clientForm.stage == Stage.COST_AND_PRIZE)
					{
						clientForm.PlayGamePayed();
						PayedBalance -= SelectedPrice;
					}
				}

				StartTables(); // refersh tables
			}));
		}
		#endregion
		#region SELECT_BUTTONS	
		private void OnLevelButClicked(object sender, EventArgs e)
		{
			Button levelButton = sender as Button;

			SetLvl(LevelBut2long[levelButton]);
		}

		private void OnPriceButClicked(object sender, EventArgs e)
		{
			Button priceButton = sender as Button;

			SetPrice(PriceBut2long[priceButton]);
		}
		#endregion
		#region TABLES
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

			long[] allSomething = Db.SelectIntColumnArray(commandString);

			grid.Rows
				.AddRange(
					allSomething
					.Select(l => GetRowWithTextCell(l.ToString()))
					.ToArray());

			return allSomething.Sum();
		}

		public void StartTables()
		{
			Db.BeginSQL();

			WinsSum = RefreshGridReturnSum(ref winsDataGridView, Db.SelectAllTempPrizes);
			PaysSum = RefreshGridReturnSum(ref priceDataGridView, Db.SelectAllTempPrices);

			winSumLabel.Text = WinsSum.ToString();
			priceSumLabel.Text = PaysSum.ToString();

			GameBalance = PaysSum - WinsSum + CoinsInStock * oneCoinShekels;
			StartPayedBalance();
		}

		private void StartPayedBalance()
		{
			if (PayedBalance > 0 && 
				!( clientForm.stage == Stage.HOW_PO_PAY 
				|| clientForm.stage == Stage.COST_AND_PRIZE
				|| clientForm.stage == Stage.GAME_PAYED)
				)
				payedCountLabel.Text = PayedBalance.ToString() + " ПЕРЕПЛАТА";
			else
				payedCountLabel.Text = PayedBalance.ToString();

			balanceLabel.Text = GameBalance.ToString();

			if (GameBalance >= 0)
			{
				warningLabel.Text = "СТОИМОСТЬ ИЛИ УРОВЕНЬ ИГРЫ В НОРМЕ";
				warningLabel.ForeColor = Color.Green;
				balanceLabel.BackColor = Color.LightGreen;
			}
			else
			{
				warningLabel.Text = "СРОЧНО ИЗМЕНИТЬ СТОИМОСТЬ ИЛИ УРОВЕНЬ ИГРЫ";
				warningLabel.ForeColor = Color.Red;
				balanceLabel.BackColor = Color.Red;
			}
		}
		#endregion
		#region FORM_CLOSED
		private void AccountingForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Db.EndSQL();
			RelayChecker.Close();
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
		private void dropWinsBut_Click(object sender, EventArgs e)
		{
			if (winsDataGridView.Rows.Count == 0)
				return;

			Db.EraseTableData(Db.TempPrizesTableName);
			StartTables();
		}

		private void dropPriceBut_Click(object sender, EventArgs e)
		{
			if (priceDataGridView.Rows.Count == 0)
				return;

			Db.EraseTableData(Db.TempPricesTableName);
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
			//Db.InsertGame(SelectedLevel, Db.Now);
			//Game_id = Db.GetMaxGamesId();
			//Db.InsertPrice(Game_id, SelectedPrice);
			//StartTables(); // refresh tables

			// does its depricated now just because have no meaning
		}

		private void giveCardBut_Click(object sender, EventArgs e)
		{
			// gives card but its not even know
			// will it be or not in the future
			// throw new NotImplementedException();
		}

		private void ReturnCoins(int coins)
		{
			COMPort.MoneyOut(coins * oneCoinShekels, this);
			CoinsInStock -= coins;

			if (PayedBalance - oneCoinShekels * coins >= 0)
				DecBalance(oneCoinShekels * coins);
		}

		private void returnMoneyBut_Click(object sender, EventArgs e)
		{
			if (PayedBalance - oneCoinShekels * oneCommandCoins >= 0)
				ReturnCoins(oneCommandCoins);
			else if (
				MessageBox.Show(
					$"НЕТ ПЕРЕПЛАТЫ, НО ЕСТЬ {CoinsInStock} МОНЕТ В АППАРАТЕ\n" +
					$"ВСЕ ЕЩЕ ХОТИТЕ ПОПЫТАТЬСЯ ВЫДАТЬ МОНЕТУ?", 
					"ВЫБОР", 
					MessageBoxButtons.YesNo) == DialogResult.Yes)
				ReturnCoins(oneCommandCoins);
		}
		#endregion
		#region UPPER_PART_BUTTONS
		public void showButton_Click(object sender, EventArgs e)
		{
			if (
				awardLabel.Text != NullText &&
				levelLabel.Text != NullText &&
				priceLabel.Text != NullText
				)
			{
				clientForm.Invoke(new Action(() =>
				{
					clientForm.ShowGameParams(SelectedAward, SelectedPrice);
					clientForm.stage = Stage.COST_AND_PRIZE;

					DbCurrentRecord.SetPricePrizeLvl(
						SelectedPrice, 
						SelectedAward, 
						SelectedLevel,
						SelectedGameType);
				}));
			}
			else
				MessageBox.Show(
					$"НЕ ВСЕ ЗНАЧЕНИЯ ВЫБРАНЫ\n" +
					$"ПРИЗ:      {awardLabel.Text}\n" + 
					$"УРОВЕНЬ:   {levelLabel.Text}\n" +
					$"СТОИМОСТЬ: {priceLabel.Text}");
		}

		private void draawPayed_Click(object sender, EventArgs e)
		{
			DecBalance(PayedBalance);
			StartTables();
		}

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

		private void resetCounterBut_Click(object sender, EventArgs e)
		{
			COMPort.Execute("Reset counter");
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

			isFirstGame = !isFirstGame;
			if (isFirstGame)
			{
				isFirstGameBut.BackColor = Color.Coral;
				isFirstGameBut.Text = on;
			}
			else
			{
				isFirstGameBut.BackColor = Color.LawnGreen;
				isFirstGameBut.Text = off;
			}
		}

		private void doOnlyDark(Button button)
		{
			foreach (Button but in new Button[] { cBut, kBut, mBut })
				but.BackColor = 
					but.Name == button.Name 
						? Color.OrangeRed 
						: Color.Orange;
		}

		private void DoCKMButClick(Button but)
		{
			doOnlyDark(but);
			SelectedGameType = ButToGameType[but];
		}

		private void cBut_Click(object sender, EventArgs e) => DoCKMButClick(cBut);

		private void kBut_Click(object sender, EventArgs e) => DoCKMButClick(kBut);

		private void mBut_Click(object sender, EventArgs e) => DoCKMButClick(mBut);

		private void requestDbUserDataBut_Click(object sender, EventArgs e)
		{
			string playerIdStr = playerNameBox.Text.HebrewTrim().Trim();
			DbPlayer player = Db.FindPlayer(playerIdStr);

			if (player == null)
			{
				if (string.IsNullOrEmpty(playerIdStr))
				{
					MessageBox.Show("ПУСТОЙ ID ИГРОКА");
					Console.WriteLine("\n\n123\n\n");
					return;
				}

				Db.InsertPlayer(playerIdStr, 0, 0, 0);
				cBox.Text = "0";
				kBox.Text = "0";
				mBox.Text = "0";
			}
			else
			{
				cBox.Text = player.C.ToString();
				kBox.Text = player.K.ToString();
				mBox.Text = player.M.ToString();
			}
		}
		#endregion TEMPORAL_CONTROLS
		#region RELAY
		private RelaysEnumerator relaysEnumerator { get; } = new RelaysEnumerator();

		private void relayBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			RelayChecker.Close();

			RelayInfo relayInfo = relayBox.SelectedItem as RelayInfo;

			RelayChecker.SelectedRelay = new Relay(relayInfo);
			RelayChecker.SelectedRelay.Open();

			relayOffOnLabel.Text = "ON";
		}

		private void relayBox_DropDown(object sender, EventArgs e)
		{
			relayBox.Items.Clear();
			relayBox.Items.AddRange(
				relaysEnumerator
				.CollectInfo()
				.ToArray());
		}
		#endregion
	}
}
