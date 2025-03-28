﻿using System;
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
		public FaceForm faceForm { get; set; }
		public ScriptEditor scriptEditor { get; set; }
        public DevicesSettings devicesSettings { get; set; }
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
		public long GameBalance { get; private set; } // all balance that is PaysSum - WinsSum + Balance
		private long PayedBalance { get; set; } // balance of shekels payed
		private long WinsSum { get; set; }
		private long PaysSum { get; set; }
		public static long Game_id { get; set; }
		// consts
		private const string NullText = "####";
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
            InitLvlGrid();
            InitPriceGrid();
            doOnlyDark(cBut);
			StartTables();

			InitClearFocusThread();

            faceControlBut_Click(this, EventArgs.Empty);
            InitDevices();
        }

		#region SOME_INITS

        private void InitDevices()
        {
            new Thread(() => {
                Thread.Sleep(3000);
                devicesSettings = new DevicesSettings("devicesSettings.txt");
                if (devicesSettings.Parse())
                    Invoke(new Action(InitParsedDevices));
                else
                    Utils.print("NOT PARSED devicesSettings.txt");
            }).Start();
        }

        private void InitParsedDevices()
        {
            Action<ComboBox, string, EventHandler> initBox = (box, param, handlerDropDown) => {
                handlerDropDown(null, EventArgs.Empty);
                box.SelectedIndex = IndexOfItemInDevicesSettings(param, box);
            };
            initBox(comBox, "MONEY", comBox_DropDown);
            initBox(relayBox, "RELAY", relayBox_DropDown);
            initBox(laserBox, "LASER", laserBox_DropDown);

            faceForm?.Invoke(new Action(() => {
                initBox(faceForm.camBox, "WEB_CAMERA", faceForm.camBox_DropDown);

                faceForm.textPort.Text =     devicesSettings.Parameters["LOCAL_PORT"];
                faceForm.ipAdressBox.Text =  devicesSettings.Parameters["MACHINE_IP"];
                faceForm.ipPortBox.Text =    devicesSettings.Parameters["MACHINE_PORT"];
                faceForm.passwordBox.Text =  devicesSettings.Parameters["MACHINE_PASSWORD"];
                faceForm.machineIdBox.Text = devicesSettings.Parameters["MACHINE_NUMBER"];
                faceForm.Connect_Click(null, EventArgs.Empty);
            }));
        }

        private int IndexOfItemInDevicesSettings(string param, ComboBox box)
        {
            if (!devicesSettings.Parameters.TryGetValue(param, out string setting))
                return -1;

            object[] objects =
                box.Items
                .Cast<object>()
                .Where(o => o.ToString().Contains(setting))
                .ToArray();

            return
                objects.Length > 0
                ? box.Items.IndexOf(objects[0])
                : -1;
        }

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
			{ "comBox", "cBox", "kBox", "mBox", "playerNameBox", "relayBox", "laserBox" };

			new Thread(() => {
				Thread.Sleep(2000);
				while (true)
				{
					Thread.Sleep(33);
					if (ActiveControl != null &&
                        !excludeControls.Contains(ActiveControl.Name)// &&
                        //!excludeControls.Any(ec => ActiveControl.Name.StartsWith(ec))
                        )
					{
						Thread.Sleep(100);
						Console.WriteLine($"CLEARED {ActiveControl.Name} {ActiveControl.GetType()}");
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

		private void InitDictionares()
		{
			ButToGameType = new Dictionary<Button, GameType>() {
				{ cBut, GameType.Guard },
				{ kBut, GameType.Painting },
				{ mBut, GameType.Mario },
			};
		}

        private DataGridViewRow GetGridRow(object value)
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
			long[] values = new long[] { 
				0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 
				110, 120, 130, 140, 150, 160, 170, 180, 190, 
                200, 250, 300, 350, 400, 450, 500, 
                1000, 3000 
			};
			prizeButsGrid.Rows.AddRange(values.Select(v => GetGridRow(v)).ToArray());
		}

        private void InitLvlGrid()
        {
            lvlButsGrid
            .Rows
            .AddRange(
                Enumerable.Range(0, 10)
                .Select(l => GetGridRow($"Уровень {l}"))
                .ToArray());
        }

        private void InitPriceGrid()
        {
            long[] values = new long[] {
                0, 10, 20, 30, 40, 50
            };
            priceButsGrid.Rows.AddRange(values.Select(v => GetGridRow(v)).ToArray());
        }

        private void prizeButsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			long prize = (long)prizeButsGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
			SetAward(prize);
		}

        private void lvlButsGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string lvlStr = lvlButsGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            long lvl = Convert.ToInt64(lvlStr.Split(' ')[1]);
            SetLvl(lvl);
        }

        private void priceGridButs_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            long price = (long)priceButsGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            SetPrice(price);
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

        public void SetGameScript(GameScript gameScript)
        {
            SetLvl(gameScript.Lvl);
            SetPrice(gameScript.Price);
            SetAward(gameScript.Prize);
            SetGameType(gameScript.GameType);
        }

        public void SetLangLabel(string text) =>
            Invoke(new Action(() => langLabel.Text = text));

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

        #region SCRIPT
        private void scriptEdititorBut_Click(object sender, EventArgs e)
        {
            if (Utils.IsFormAlive(scriptEditor))
                return;

            scriptEditor = new ScriptEditor(this, clientForm);
            scriptEditor.Show();
            scriptEditor.Location = new Point(1920 - scriptEditor.Width, 1080 - scriptEditor.Height);
        }
        #endregion SCRIPT

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

            GameBalance = PaysSum - WinsSum;// + CoinsInStock * oneCoinShekels;
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
            RelayChecker.CameraDownTrue();
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
			clientForm.PlayGamePayed();
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

        public void draawPayed_Click(object sender, EventArgs e)
		{
			DecBalance(PayedBalance);
		}

		private void playIdleBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(clientForm.PlayIdle));
		}

		private void stopBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(clientForm.Stop));
		}

		private void replayBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(clientForm.Replay));
		}

		private void startGameBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(clientForm.StartGame));
		}

		private void skipStageBut_Click(object sender, EventArgs e)
		{
			clientForm.Invoke(new Action(clientForm.SkipStage));
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
            string port = comBox.Text;
			clientForm.Invoke(new Action(() => COMPort.TryConnectPort(port, this)));
			ActiveControl = null;

            devicesSettings.Add("MONEY", port);
		}

		private void comBox_DropDown(object sender, EventArgs e)
		{
			comBox.Items.Clear();
			comBox.Items.AddRange(SerialPort.GetPortNames());
		}

		public void resetCounterBut_Click(object sender, EventArgs e)
		{
			COMPort.Execute("Reset counter");
			IncCoinsInStock(-CoinsInStock);
        }
		#endregion

		#region FACE_CONTROL

		private void faceControlBut_Click(object sender, EventArgs e)
		{
            if (Utils.IsFormAlive(faceForm))
                return;

			faceForm = new FaceForm(this);
            faceForm.Show();
            faceForm.Location = new Point(1920, 0);
		}

        public void RefreshDbForm()
        {
            EditDbForm editDb = faceForm.editDbForm;
            if (Utils.IsFormAlive(editDb))
                editDb.Invoke(new Action(() =>
                editDb.SelectPlayersFromDb(false)));
        }

        #endregion FACE_CONTROL

        #region DEBUG_FORM

        public DebugForm debugForm { get; set; }

        private void debugBut_Click(object sender, EventArgs e)
        {
            if (Utils.IsFormAlive(debugForm))
                return;

            debugForm = new DebugForm(this);
            debugForm.Show();
            debugForm.Location = new Point(0, 0);
        }

        #endregion DEBUG_FORM

        #region TEMPORAL_CONTROLS

        public void SetUserId(long id)
        {
            Invoke(new Action(() => playerNameBox.Text = id.ToString()));
        }

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

        public void SetGameType(GameType gameType)
        {
            switch (gameType) {
                case GameType.Guard:    DoCKMButClick(cBut); break;
                case GameType.Painting: DoCKMButClick(kBut); break;
                case GameType.Mario:    DoCKMButClick(mBut); break;
            }
        }

		private void cBut_Click(object sender, EventArgs e) => DoCKMButClick(cBut);

		private void kBut_Click(object sender, EventArgs e) => DoCKMButClick(kBut);

		private void mBut_Click(object sender, EventArgs e) => DoCKMButClick(mBut);

		public void requestDbUserDataBut_Click(object sender, EventArgs e)
		{
			long playerIdInt;
			if (long.TryParse(playerNameBox.Text.HebrewTrim().Trim(), out long playerId))
				playerIdInt = playerId;
			else {
				MessageBox.Show("НЕВЕРНЫЙ ID ИГРОКА");
				return;
			}

			DbPlayer player = Db.FindPlayer(playerIdInt);

			if (player == null) {
				Db.InsertPlayer(playerIdInt, 0, 0, 0);
				cBox.Text = "0";
				kBox.Text = "0";
				mBox.Text = "0";

                clientForm.gameInfo.ClearCounters();
                clientForm.gameInfo.ClearGameIndicesAndSetFirst(-1);
                SetIsFirstGame(true);
                SetGameScript(clientForm.gameInfo.FirstGame);
			}
			else {
				cBox.Text = player.C.ToString();
				kBox.Text = player.K.ToString();
				mBox.Text = player.M.ToString();
                Console.WriteLine($"C{cBox.Text};K{kBox.Text};M{mBox.Text}");

                int[] counters = Db.GetCounters(player);
                clientForm.gameInfo.SetWonCounter(counters[0]);
                clientForm.gameInfo.SetLostCounter(counters[1]);

                clientForm.gameInfo.ClearGameIndicesAndSetFirst(
                    DecideGameIndex(
                        clientForm.gameInfo.ModeScripts[GameMode.LOW].ToList(), 
                        player));
                SetIsFirstGame(false);

                // p 4 7 0  --X-> s<4>4 0
                // p 4 7 0  ----> s 4<7>0
                GameScript script = clientForm.gameInfo.CurrentScript;

                Console.WriteLine($"C{cBox.Text};K{kBox.Text};M{mBox.Text}");
                Console.WriteLine(player);
                Console.WriteLine(script);
                Console.WriteLine("\t" + string.Join("\n\t", 
                    clientForm.gameInfo.ModeScripts[GameMode.LOW]
                    .Where(s => s.Lvl - 1 <= script.Lvl)
                    .Select(Convert.ToString)));

                SetGameScript(clientForm.gameInfo.CurrentScript);

                Console.WriteLine($"C{cBox.Text};K{kBox.Text};M{mBox.Text}");
            }

            RefreshDbForm();
        }

        private int DecideGameIndex(List<GameScript> gameScripts, DbPlayer player)
        {
            /*
            return 
                gameScripts
                .IndexOf(gameScripts
                .FindLast(s => 
                    s.Lvl == player.C ||
                    s.Lvl == player.K ||
                    s.Lvl == player.M));
                */

            for (int i = 0; i < gameScripts.Count; i++) {
                GameScript gameScript = gameScripts[i];

                switch (gameScript.GameType) {
                    case GameType.Guard:
                        if (gameScript.Lvl == player.C)
                            return i;
                        break;
                    case GameType.Painting:
                        if (gameScript.Lvl == player.K)
                            return i;
                        break;
                    case GameType.Mario:
                        if (gameScript.Lvl == player.M)
                            return i;
                        break;
                    default:
                        break;
                }
            }

            return 0;
        }

        private bool LongParseTextBox(TextBox box, out long res, string param)
		{
			if (long.TryParse(box.Text.HebrewTrim().Trim(), out long result))
			{
				res = result;
				return true;
			}
            MessageBox.Show($"НЕВЕРНЫЙ [{param}] ИГРОКА");
            res = -1;
			return false;
		}

        private void writePlayerBut_Click(object sender, EventArgs e)
        {
			if (!LongParseTextBox(playerNameBox, out long playerIdInt, "ID")) return;
            if (!LongParseTextBox(cBox, out long cLvlInt, "C")) return;
            if (!LongParseTextBox(kBox, out long kLvlInt, "K")) return;
            if (!LongParseTextBox(mBox, out long mLvlInt, "M")) return;

			if (Db.FindPlayer(playerIdInt) == null) {
                DbPlayer player = new DbPlayer(-1, playerIdInt, cLvlInt, kLvlInt, mLvlInt);
                int playerWouldPlayScript = DecideGameIndex(
                    clientForm.gameInfo.ModeScripts[GameMode.LOW].ToList(), 
                    player);
                SetIsFirstGame(playerWouldPlayScript < 1);

                Db.InsertPlayer(playerIdInt, cLvlInt, kLvlInt, mLvlInt);
            }
			else
				Db.UpdatePlayer(playerIdInt, cLvlInt, kLvlInt, mLvlInt);

            RefreshDbForm();
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
            devicesSettings.Add("RELAY", RelayChecker.SelectedRelay.ToString());
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

        #region LASER

        public MODBUS modbus { get; set; }
        public ushort laserValue { get; set; } = 10;
        private bool lastIsLaserIntersected { get; set; } = false;
        private Thread laserThread { get; set; }

        private void laserBox_DropDown(object sender, EventArgs e)
        {
            laserBox.Items.Clear();
            laserBox.Items.AddRange(SerialPort.GetPortNames());
        }

        private void laserBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string portName = laserBox.Items[laserBox.SelectedIndex].ToString();

            if (modbus != null)
                modbus.Close();

            modbus = new MODBUS(portName);
            if (modbus.Open()) {
                if (laserThread != null && laserThread.IsAlive)
                    laserThread.Abort();

                laserOnOffLabel.Text = "ON";
                laserThread = InitLaserThread();
                laserThread.Start();

                devicesSettings.Add("LASER", portName);
            }
            else
                laserOnOffLabel.Text = "OFF";
        }

        private bool IsLaserIntersected() =>
            laserValue > 2045;// (4090 / 2);

        private Thread InitLaserThread() =>
            new Thread(() => { while (true) {
                Thread.Sleep(200);

                ushort[] registers = modbus.ReadReg(1, 0, 2); // dont know why 1, 0, 2

                if (registers != null && registers.Length > 0) { Invoke(new Action(() => {
                    laserValue = registers[0];
                    laserValueLabel.Text = laserValue.ToString();

                    bool isLaserIntersected = IsLaserIntersected();
                    laserValueLabel.BackColor =
                        isLaserIntersected
                        ? Color.LightGreen
                        : Color.LightCoral;

                    if (!lastIsLaserIntersected &&
                        isLaserIntersected &&
                        clientForm.stage == Stage.IDLE
                        ) {
                        if (Utils.IsFormAlive(faceForm))
                            faceForm.SetToRecognize(true);
                        startGameBut_Click(null, EventArgs.Empty);
                    }

                    lastIsLaserIntersected = isLaserIntersected;
                }));}
            }});

        #endregion LASER
    }
}
