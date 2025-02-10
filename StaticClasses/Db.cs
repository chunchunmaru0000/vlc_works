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

		public const string GameRecordsTableName = "game_records";
		public const string TempPrizesTableName = "temp_prizes";
		public const string TempPricesTableName = "temp_prices";
		#endregion CONSTANTS
		#region SQL COMMANDS
		private static string createGameRecordsTable = $@"
CREATE TABLE IF NOT EXISTS {GameRecordsTableName} (
	id INTEGER PRIMARY KEY,
	unix_time_int INTEGER NOT NULL,
	prize_int INTEGER NOT NULL,
	price_int INTEGER NOT NULL,
	win_bool_int INTEGER NOT NULL,
	game_type_str TEXT NOT NULL,
	player_id_int INTEGER NOT NULL,
	game_level_int INTEGER NOL NULL
);
";
		private static Func<long, long, long, bool, GameType, long, long, string> InsertGameRecordCommand = 
			(unixTimeInt, prizeInt, priceInt, winBoolInt, gameType, playerIdInt, gameLevelInt) => $@"
INSERT INTO game_records (
	unix_time_int, 
	prize_int, 
	price_int, 
	win_bool_int, 
	game_type_str,
	player_id_int,
	game_level_int
)
VALUES (
	{unixTimeInt},
	{prizeInt},
	{priceInt},
	{(winBoolInt ? 1 : 0)},
	{gameType.View()},
	{playerIdInt},
	{gameLevelInt}
)
";

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

		public static void InsertGameRecordsRecord(
			long unixTimeInt, long prizeInt, long priceInt, bool winBoolInt, 
			GameType gameType, long playerIdInt, long gameLevelInt
			)
		{ 
			if (IsNotConnected)
				return;

			ExecuteNonQuery(
				InsertGameRecordCommand(
					unixTimeInt, prizeInt, priceInt, winBoolInt, gameType, playerIdInt, gameLevelInt));
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
