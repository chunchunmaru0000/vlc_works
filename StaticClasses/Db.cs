using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace vlc_works
{
	public struct DbSelectGamesItem
	{
		public long Id { get; set; }
		public long GameAward { get; set; }
		public long GamePrice { get; set; }
		public long GameLevel { get; set; }
		public long GameStartTime { get; set; }

		public DbSelectGamesItem(long id, long gameAward, long gamePrice, long gameLevel, long gameStartTime)
		{
			Id = id;
			GameAward = gameAward;
			GamePrice = gamePrice;
			GameLevel = gameLevel;
			GameStartTime = gameStartTime;
		}

		public static DbSelectGamesItem Arr2GamesItem(long[] arr) => 
			new DbSelectGamesItem(arr[0], arr[1], arr[2], arr[3], arr[4]);

		public override string ToString() => $"{Id} {GameAward} {GamePrice} {GameLevel} {Db.SecToTime(GameStartTime)}";
	}

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
		#region SQL COMMANDS
		private static string createPlayersTable = $@"
CREATE TABLE IF NOT EXISTS {PlayersTableName} (
	id INTEGER NOT NULL,

	player_id_str TEXT NOT NULL,
	c_lvl_int INTEGER NOT NULL,
	k_lvl_int INTEGER NOT NULL,
	m_lvl_int INTEGER NOT NULL
);
";
		public static void InsertPlayer(string playerIdStr, int cLvlInt, int kLvlInt, int mLvlInt)
			{ 
				string command = $@"
INSERT INTO {PlayersTableName} (player_id_str, c_lvl_int, k_lvl_int, m_lvl_int)
VALUES (@playerIdStr, @cLvlInt, @kLvlInt, @mLvlInt);
";
				using (SQLiteCommand cmd = new SQLiteCommand(command, SqLiteConnection))
				{
					cmd.Parameters.AddWithValue("@playerIdStr", playerIdStr);
					cmd.Parameters.AddWithValue("@cLvlInt", cLvlInt);
					cmd.Parameters.AddWithValue("@kLvlInt", kLvlInt);
					cmd.Parameters.AddWithValue("@mLvlInt", mLvlInt);
					cmd.ExecuteNonQuery();
				}
			}

		private static string createGameRecordsTable = $@"
CREATE TABLE IF NOT EXISTS {GameRecordsTableName} (
	id INTEGER PRIMARY KEY,

	player_id_str TEXT NOT NULL,
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

	FOREIGN KEY (player_id_str) REFERENCES {PlayersTableName}(player_id_str)
);
";
		public static void InsertGameRecord(
			string player_id_str, long unix_time_int,
			int player_c_lvl, int player_k_lvl, int player_m_lvl,
			int game_c_lvl,   int game_k_lvl,   int game_m_lvl,
			bool won_bool_int, bool continued_bool_int, long price_int, long prize_int
			)
		{
			string command = $@"
INSERT INTO {GameRecordsTableName} (
	player_id_str,
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
	@player_id_str,
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
				cmd.Parameters.AddWithValue("@player_id_str", player_id_str);
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

		private static string createTempPrizesTable = $@"
CREATE TABLE IF NOT EXISTS {TempPrizesTableName} (
	id INTEGER PRIMARY KEY,
	prize_int INTEGER NOT NULL
)
";
		private static Func<long, string> InsertTempPrizesCommand =
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
		private static Func<long, string> InsertTempPricesCommand =
			(priceInt) => $@"
INSERT INTO temp_prices (price_int) VALUES ({priceInt})
";
		public static string SelectAllTempPrices = $@"
SELECT price_int from {TempPricesTableName}
";
		#endregion SQL COMMANDS
		private static SQLiteConnection SqLiteConnection { get; set; }
		private static bool IsNotConnected { get => SqLiteConnection == null || SqLiteConnection.IsCanceled(); }
		private static void ExecuteNonQuery(string commandStr)
		{
			if (IsNotConnected)
				return;
			using (SQLiteCommand command = new SQLiteCommand(commandStr, SqLiteConnection))
				command.ExecuteNonQuery();
		}
		#region PUBLIC VALUES
		public static long Now { get { return DateTimeOffset.Now.ToUnixTimeSeconds(); } }
		public static DateTimeOffset SecToTime(long unixSeconds) => DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
		#endregion PUBLIC VALUES 

		public static void BeginSQL()
		{
			if (!File.Exists(dbName))
				SQLiteConnection.CreateFile(dbName);

			if (SqLiteConnection == null)
			{
				SqLiteConnection = new SQLiteConnection(connectionString);
				SqLiteConnection.Open();
			}

			ExecuteNonQuery(createGameRecordsTable);
			ExecuteNonQuery(createTempPrizesTable);
			ExecuteNonQuery(createTempPricesTable);
		}

		public static void EndSQL()
		{
			if (IsNotConnected)
				return;
			SqLiteConnection.Close();
		}

		public static void DropTable(string tableName) 
		{
			if (IsNotConnected)
				return;
			ExecuteNonQuery($"DROP TABLE {tableName}"); 
		}

		public static void InsertInAllTables(
			string playerIdStr, long unixTimeInt, 
			int playerCLvl, int playerKLvl, int playerMLvl,
			int gameCLvl,   int gameKLvl,   int gameMLvl,
			bool wonBoolInt, bool continuedBoolInt,
			long prizeInt, long priceInt
			)
		{ 
			if (IsNotConnected)
				return;

			if (Contains player where player_id = playerIdStr)
				update table players where player_id = playerIdStr 
				set playerCLvl = ..., playerKLvl = ..., playerMLvl = ...
			else
				InsertPlayer(playerIdStr, 0, 0, 0);
			InsertGameRecord(
				playerIdStr, unixTimeInt, playerCLvl, playerKLvl, playerMLvl, 
				gameCLvl, gameKLvl, gameMLvl, wonBoolInt, continuedBoolInt, prizeInt, priceInt);
			ExecuteNonQuery(InsertTempPrizesCommand(prizeInt));
			ExecuteNonQuery(InsertTempPricesCommand(priceInt));
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
	}
}
