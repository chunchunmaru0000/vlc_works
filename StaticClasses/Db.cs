using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading;

namespace vlc_works
{
	public static class Db
	{
		#region CONSTANTS
		private const string dbName = "goldInSafe.db";
		private const string connectionString = "Data Source=" + dbName + ";Version=3;";

		public const string PlayersTableName = "players";
		public const string GameRecordsTableName = "game_records";
		public const string TempPrizesTableName = "temp_prizes";
		public const string TempPricesTableName = "temp_prices";
		#endregion CONSTANTS
		#region MAIN_TABLES
		private static string createPlayersTable = $@"
CREATE TABLE IF NOT EXISTS {PlayersTableName} (
	id INTEGER PRIMARY KEY AUTOINCREMENT,

	player_id_int INTEGER NOT NULL,
	c_lvl_int INTEGER NOT NULL,
	k_lvl_int INTEGER NOT NULL,
	m_lvl_int INTEGER NOT NULL
);
";
		public static void InsertPlayer(long playerIdInt, long cLvlInt, long kLvlInt, long mLvlInt)
		{ 
			string command = $@"
INSERT INTO {PlayersTableName} (player_id_int, c_lvl_int, k_lvl_int, m_lvl_int)
VALUES (@playerIdInt, @cLvlInt, @kLvlInt, @mLvlInt);
";
			using (SQLiteCommand cmd = new SQLiteCommand(command, SqLiteConnection))
			{
				cmd.Parameters.AddWithValue("@playerIdInt", playerIdInt);
				cmd.Parameters.AddWithValue("@cLvlInt", cLvlInt);
				cmd.Parameters.AddWithValue("@kLvlInt", kLvlInt);
				cmd.Parameters.AddWithValue("@mLvlInt", mLvlInt);
				cmd.ExecuteNonQuery();
			}
		}

        public static void InsertPlayer(DbPlayer dbPlayer) =>
            InsertPlayer(dbPlayer.PlayerIdInt, dbPlayer.C, dbPlayer.K, dbPlayer.M);

		private static string createGameRecordsTable = $@"
CREATE TABLE IF NOT EXISTS {GameRecordsTableName} (
	id INTEGER PRIMARY KEY AUTOINCREMENT,

	player_id_int INTEGER NOT NULL,
	unix_time_int INTEGER NOT NULL,

	player_c_lvl INTEGER NOT NULL,
	player_k_lvl INTEGER NOT NULL,
	player_m_lvl INTEGER NOT NULL,

	game_c_lvl INTEGER NOT NULL,
	game_k_lvl INTEGER NOT NULL,
	game_m_lvl INTEGER NOT NULL,
	
	won_bool_int INTEGER NOT NULL,
	continued_bool_int INTEGER NOT NULL,
	price_int INTEGER NOT NULL,
	prize_int INTEGER NOT NULL,

	FOREIGN KEY (player_id_int) REFERENCES {PlayersTableName}(player_id_int)
);
";
		public static void InsertGameRecord(
			long player_id_int, long unix_time_int,
			long player_c_lvl, long player_k_lvl, long player_m_lvl,
			long game_c_lvl, long game_k_lvl, long game_m_lvl,
			bool won_bool_int, bool continued_bool_int, long price_int, long prize_int
			)
		{
			string command = $@"
INSERT INTO {GameRecordsTableName} (
	player_id_int,
	unix_time_int,
	player_c_lvl,
	player_k_lvl,
	player_m_lvl,
	game_c_lvl,
	game_k_lvl,
	game_m_lvl,
	won_bool_int,
	continued_bool_int,
	price_int,
	prize_int
) VALUES (
	@player_id_int,
	@unix_time_int,
	@player_c_lvl,
	@player_k_lvl,
	@player_m_lvl,
	@game_c_lvl,
	@game_k_lvl,
	@game_m_lvl,
	@won_bool_int,
	@continued_bool_int,
	@price_int,
	@prize_int
);
";
			using (SQLiteCommand cmd = new SQLiteCommand(command, SqLiteConnection))
			{
				cmd.Parameters.AddWithValue("@player_id_int", player_id_int);
				cmd.Parameters.AddWithValue("@unix_time_int", unix_time_int);
				cmd.Parameters.AddWithValue("@player_c_lvl", player_c_lvl);
				cmd.Parameters.AddWithValue("@player_k_lvl", player_k_lvl);
				cmd.Parameters.AddWithValue("@player_m_lvl", player_m_lvl);
				cmd.Parameters.AddWithValue("@game_c_lvl", game_c_lvl);
				cmd.Parameters.AddWithValue("@game_k_lvl", game_k_lvl);
				cmd.Parameters.AddWithValue("@game_m_lvl", game_m_lvl);
				cmd.Parameters.AddWithValue("@won_bool_int", won_bool_int ? 1 : 0);
				cmd.Parameters.AddWithValue("@continued_bool_int", continued_bool_int ? 1 : 0);
				cmd.Parameters.AddWithValue("@price_int", price_int);
				cmd.Parameters.AddWithValue("@prize_int", prize_int);
				cmd.ExecuteNonQuery();
			}
		}
		#endregion MAIN_TABLES

		#region TEMP_TABLES
		private static string createTempPrizesTable = $@"
CREATE TABLE IF NOT EXISTS {TempPrizesTableName} (
	id INTEGER PRIMARY KEY,
	prize_int INTEGER NOT NULL
)
";
		private readonly static Func<long, string> InsertTempPrizesCommand =
			(prizeInt) => $@"
INSERT INTO temp_prizes (prize_int) VALUES ({prizeInt})
";
		public static string SelectAllTempPrizes = $@"
SELECT prize_int from {TempPrizesTableName}
";

		private static string createTempPricesTable = $@"
CREATE TABLE IF NOT EXISTS {TempPricesTableName} (
	id INTEGER PRIMARY KEY,
	price_int INTEGER NOT NULL
)
";
		private readonly static Func<long, string> InsertTempPricesCommand =
			(priceInt) => $@"
INSERT INTO temp_prices (price_int) VALUES ({priceInt})
";
		public static string SelectAllTempPrices = $@"
SELECT price_int from {TempPricesTableName}
";
		#endregion TEMP_TABLES
		private static SQLiteConnection SqLiteConnection { get; set; }
		private static bool IsNotConnected { get => SqLiteConnection == null || SqLiteConnection.IsCanceled(); }
		private static void ExecuteNonQuery(string commandStr)
		{
			if (IsNotConnected)
				return;
			using (SQLiteCommand command = new SQLiteCommand(commandStr, SqLiteConnection))
				command.ExecuteNonQuery();
		}
        private static Sheet GameSheet { get; set; }
        private static Sheet BalanceSheet { get; set; }
		#region PUBLIC VALUES
		public static long Now { get { return DateTimeOffset.Now.ToUnixTimeSeconds(); } }
		public static DateTimeOffset SecToTime(long unixSeconds) => DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
        #endregion PUBLIC VALUES 

        public static void InitSheets(Sheet gameSheet, Sheet balanceSheet)
        {
            GameSheet = gameSheet;
            BalanceSheet = balanceSheet;
        }

        public static void BeginSQL()
		{
			if (!IsNotConnected)
				return;

			if (!File.Exists(dbName))
				SQLiteConnection.CreateFile(dbName);

			SqLiteConnection = new SQLiteConnection(connectionString);
			SqLiteConnection.Open();

			ExecuteNonQuery(createGameRecordsTable);
			ExecuteNonQuery(createPlayersTable);
			ExecuteNonQuery(createTempPrizesTable);
			ExecuteNonQuery(createTempPricesTable);
		}

		public static void EndSQL()
		{
			if (IsNotConnected)
				return;
			SqLiteConnection.Close();
		}

		public static void EraseTableData(string tableName) 
		{
			if (IsNotConnected)
				return;
			ExecuteNonQuery($"DELETE FROM {tableName}"); 
		}

		private static bool PlayerExists(long playerIdInt)
		{
			string query = $"SELECT COUNT(*) FROM {PlayersTableName} WHERE player_id_int = @playerIdInt";

			using (SQLiteCommand cmd = new SQLiteCommand(query, SqLiteConnection))
			{
				cmd.Parameters.AddWithValue("@playerIdInt", playerIdInt);
				return Convert.ToInt64(cmd.ExecuteScalar()) > 0; // == 1
			}
		}

		public static void UpdatePlayerIntData(long playerIdInt, long data, string param)
		{
			string query = $"UPDATE {PlayersTableName} SET {param} = @data WHERE player_id_int = @playerIdInt";

			using (SQLiteCommand cmd = new SQLiteCommand(query, SqLiteConnection))
			{
				cmd.Parameters.AddWithValue("@data", data);
				cmd.Parameters.AddWithValue("@playerIdInt", playerIdInt);
				cmd.ExecuteNonQuery();
			}
		}

		public static void UpdatePlayer(long playerIdInt, long gameCLvl, long gameKLvl, long gameMLvl)
		{
			if (gameCLvl != -1) // -1 is like - or Null or None 
				UpdatePlayerIntData(playerIdInt, gameCLvl, "c_lvl_int");
			if (gameKLvl != -1)
				UpdatePlayerIntData(playerIdInt, gameKLvl, "k_lvl_int");
			if (gameMLvl != -1)
				UpdatePlayerIntData(playerIdInt, gameMLvl, "m_lvl_int");
		}

		public static void InsertInAllTables(
			long playerIdInt, long unixTimeInt,
			long playerCLvl, long playerKLvl, long playerMLvl,
			long gameCLvl, long gameKLvl, long gameMLvl,
			bool wonBoolInt, bool continuedBoolInt,
			long prizeInt, long priceInt,
			long playerUpdCLvl, long playerUpdKLvl, long playerUpdMLvl,
            bool isFirstGame, int[] counters
            )
		{ 
			if (IsNotConnected)
				return;

			InsertGameRecord(
				playerIdInt, unixTimeInt, 
				playerCLvl, playerKLvl, playerMLvl, 
				gameCLvl, gameKLvl, gameMLvl, 
				wonBoolInt, continuedBoolInt, prizeInt, priceInt);
            AppendGameSheet(
                playerIdInt, unixTimeInt,
                playerCLvl, playerKLvl, playerMLvl,
                gameCLvl, gameKLvl, gameMLvl,
                wonBoolInt, continuedBoolInt, prizeInt, priceInt,
                isFirstGame, counters
                );

			if (PlayerExists(playerIdInt))
				UpdatePlayer(playerIdInt, playerUpdCLvl, playerUpdKLvl, playerUpdMLvl);
			else
				InsertPlayer(playerIdInt, playerUpdCLvl, playerUpdKLvl, playerUpdMLvl);

			if (wonBoolInt)
				ExecuteNonQuery(InsertTempPrizesCommand(prizeInt));
			ExecuteNonQuery(InsertTempPricesCommand(priceInt));
		}

        private static string DateOfUnix(long seconds) => 
            DateTimeOffset.FromUnixTimeSeconds(seconds)
            .DateTime.ToString("dd/MM/yy HH:mm:ss");

        private static void AppendGameSheet(
            long player_id_int, long unix_time_int,
            long player_c_lvl, long player_k_lvl, long player_m_lvl,
            long game_c_lvl, long game_k_lvl, long game_m_lvl,
            bool won_bool_int, bool continued_bool_int, long price_int, long prize_int,
            bool isFirstGame, int[] counters
            )
        {
            new Thread(() => {
                try { 
                    string[][] values = new string[][]{ new string[] {
                        player_id_int.ToString(), // ID игрока
                        DateOfUnix(unix_time_int), // Дата
                        (player_c_lvl + player_k_lvl + player_m_lvl).ToString(), // Уровень игрока на момент начала игры
                        player_c_lvl.ToString(), // Уровень игрока на момент начала игры С
                        player_k_lvl.ToString(), // Уровень игрока на момент начала игры К
                        player_m_lvl.ToString(), // Уровень игрока на момент начала игры М
                        $"{DbCurrentRecord.SelectedGameType.View()[0]}{DbCurrentRecord.SelectedLvl}", // Уровень игры
                        "-", // Номер попытки прохождения этого уровня
                        counters[1].ToString(), // кол-во проигрышей до этой игры
                        won_bool_int ? "1" : "0", // Результат
                        continued_bool_int ? "1" : "0", // Продолжил сразу
                        price_int.ToString(), // Стоимость игры
                        prize_int.ToString(), // Сумма возможного выигрыша
                        "-", // Накопительный баланс
                    } };
                    GameSheet.Append(ListAndRange.New("A", "N"), values);
                } catch (Exception e) {
                    File.AppendAllText("GOOGLE_SHEETS_ERRORS.txt", e.Message, encoding: System.Text.Encoding.UTF8);
                }
            }).Start();
        }

        public static void AppendBalanceSheet(long time, bool won, long price, long prize, long balance)
        {
            new Thread(() => {
                try {
                    string[][] values = new string[][] { new string[] {
                        DateOfUnix(time),
                        won ? prize.ToString() : "-",
                        price.ToString(),
                        balance.ToString()
                    } };
                    BalanceSheet.Append(ListAndRange.New("A", "D"), values);
                } catch (Exception e) {
                    File.AppendAllText("GOOGLE_SHEETS_ERRORS.txt", e.Message, encoding: System.Text.Encoding.UTF8);
                }
            }).Start();
        }

        public static long[] SelectIntColumnArray(string commandStr)
		{
			long[] selectItems;

			using (SQLiteCommand command = new SQLiteCommand(commandStr, SqLiteConnection))
			{
				SQLiteDataReader reader = command.ExecuteReader();
				DataTable table = new DataTable();
				table.Load(reader);
				reader.Close();

				selectItems = new long[table.Rows.Count];

				long i = 0;
				foreach (DataRow row in table.Rows)
					selectItems[i++] = Convert.ToInt64(row.ItemArray[0]);
			}
			return selectItems.ToArray();
		}

		public static DbPlayer FindPlayer(long playerIdInt)
		{
			string commandStr = $"SELECT * FROM {PlayersTableName} WHERE player_id_int = @playerIdInt";

			using (SQLiteCommand cmd = new SQLiteCommand(commandStr, SqLiteConnection))
			{
				cmd.Parameters.AddWithValue("@playerIdInt", playerIdInt);

				SQLiteDataReader reader = cmd.ExecuteReader();
				DataTable table = new DataTable();
				table.Load(reader);
				reader.Close();

				DbPlayer[] selectItems = 
					table.Rows
					.Cast<DataRow>()
					.Select(row => DbPlayer.FromArray(row.ItemArray))
					.ToArray();

				if (selectItems.Length > 0)
					return selectItems[0];
			}
			return null;
		}

        public static long AutoincrementCounter(string table)
        {
            string query = $"SELECT seq FROM sqlite_sequence WHERE name = @table";

            /*
             * https://www.sqlite.org/autoinc.html#the_autoincrement_keyword
             *  A row in the sqlite_sequence table corresponding to the table 
             *  with the AUTOINCREMENT column is created the first time 
             *  the AUTOINCREMENT table is written and updated on any subsequent 
             *  writes that increase the maximum rowid.
             */
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(query, SqLiteConnection))
                {
                    command.Parameters.AddWithValue("@table", table);
                    return Convert.ToInt64(command.ExecuteScalar());
                }
            }
            catch
            { 
                return 0;
            }
        }

        public static DbPlayer[] SelectAllPlayers()
        {
            string query = $"SELECT * FROM {PlayersTableName}";

            using (SQLiteCommand cmd = new SQLiteCommand(query, SqLiteConnection))
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                reader.Close();

                DbPlayer[] selectItems =
                    table.Rows
                    .Cast<DataRow>()
                    .Select(row => DbPlayer.FromArray(row.ItemArray))
                    .ToArray();

                return selectItems;
            }
        }

        public static void DeletePlayerWhomIdEquals(long id)
        {
            string query = $"DELETE FROM {PlayersTableName} WHERE id = @id";

            using (SQLiteCommand cmd = new SQLiteCommand(query, SqLiteConnection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// <paramref name="player"/> existing is ensured by user of not being null
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int[] GetCounters(DbPlayer player)
        {
            string query =
                $@"
SELECT won_bool_int FROM {GameRecordsTableName} 
WHERE player_id_int = @playerId
ORDER BY id DESC
LIMIT 3";
            using(SQLiteCommand cmd = new SQLiteCommand(query, SqLiteConnection)) {
                cmd.Parameters.AddWithValue("@playerId", player.PlayerIdInt);

                SQLiteDataReader reader = cmd.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                reader.Close();

                int[] last3 =
                    table.Rows
                    .Cast<DataRow>()
                    .Select(row => Convert.ToInt32(row.ItemArray[0]))
                    .ToArray();

                return
                    last3.Length == 0
                    ? new int[2] { 0, 0 }
                    : last3[0] == 1
                        ? new int[2] { last3.TakeWhile(w => w == 1).Count(), 0 }
                        : new int[2] { 0, last3.TakeWhile(w => w == 0).Count() };
            }
        }
    }
}
