using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

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
		// consts
		const string dbName = "goldInSafe.db";
		const string connectionString = "Data Source=" + dbName + ";Version=3;";
		const string createGamesTable = @"
CREATE TABLE IF NOT EXISTS games (
	id               INTEGER PRIMARY KEY,
	gameLevel        INTEGER NOT NULL,
	gameStartTime    INTEGER NOT NULL
);";
		const string createAwardsTable = @"
CREATE TABLE IF NOT EXISTS awards (
	award_id         INTEGER PRIMARY KEY,
	game_id          INTEGER NOT NULL,
	award            INTEGER NOT NULL,

	FOREIGN KEY (game_id) REFERENCES games(id)
)
";
		const string createPricesTable = @"
CREATE TABLE IF NOT EXISTS prices (
	price_id         INTEGER PRIMARY KEY,
	game_id          INTEGER NOT NULL,
	price            INTEGER NOT NULL,

	FOREIGN KEY (game_id) REFERENCES games(id)
)
";
		public const string selectAllGames = "select * from games";
		public const string selectLastRowId = "select last_insert_rowid()";
		public const string selectAllAwards = "select award from awards";
		public const string selectAllPrices = "select price from prices";
		// sql
		static SQLiteConnection SqLiteConnection { get; set; }
		// some
		public static long Now { get { return DateTimeOffset.Now.ToUnixTimeSeconds(); } }
		public static DateTimeOffset SecToTime(long unixSeconds) => DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

		public static void BeginSQL()
		{
			if (!File.Exists(dbName))
				SQLiteConnection.CreateFile(dbName);

			if (SqLiteConnection == null)
			{
				SqLiteConnection = new SQLiteConnection(connectionString);
				SqLiteConnection.Open();
			}

			using (SQLiteCommand command = new SQLiteCommand(createGamesTable, SqLiteConnection))
				command.ExecuteNonQuery();
			using (SQLiteCommand command = new SQLiteCommand(createAwardsTable, SqLiteConnection))
				command.ExecuteNonQuery();
			using (SQLiteCommand command = new SQLiteCommand(createPricesTable, SqLiteConnection))
				command.ExecuteNonQuery();
		}

		public static void EndSQL()
		{
			SqLiteConnection.Close();
		}

		public static void Insert(long gameAward, long gamePrice, long gameLevel, long gameStartTime)
		{
			using (SQLiteCommand command = new SQLiteCommand($@"
insert into games (gameLevel, gameStartTime) values({gameLevel}, {gameStartTime});", SqLiteConnection))
				command.ExecuteNonQuery();

			long game_id;
			using (SQLiteCommand command = new SQLiteCommand(selectLastRowId, SqLiteConnection))
				game_id = (long)command.ExecuteScalar();

			using (SQLiteCommand command = new SQLiteCommand($@"
insert into awards (game_id, award) values({game_id}, {gameAward});", SqLiteConnection))
				command.ExecuteNonQuery();

			using (SQLiteCommand command = new SQLiteCommand($@"
insert into prices (game_id, price) values({game_id}, {gamePrice});", SqLiteConnection))
				command.ExecuteNonQuery();
		}

		public static DbSelectGamesItem[] SelectAllGames()
		{
			DbSelectGamesItem[] selectItems;

			using (SQLiteCommand command = new SQLiteCommand(selectAllGames, SqLiteConnection))
			{
				SQLiteDataReader reader = command.ExecuteReader();
				DataTable table = new DataTable();
				table.Load(reader);
				reader.Close();

				selectItems = new DbSelectGamesItem[table.Rows.Count];

				long i = 0;
				foreach(DataRow row in table.Rows)
					selectItems[i++] = DbSelectGamesItem.Arr2GamesItem(row.ItemArray.Select(r => (long)r).ToArray());
			}
			return selectItems.ToArray();
		}

		public static long[] SelectAllLong(string commandString)
		{
			long[] selectItems;

			using (SQLiteCommand command = new SQLiteCommand(commandString, SqLiteConnection))
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

		public static void DropTable(string tableName)
		{
			using (SQLiteCommand command = new SQLiteCommand($"DROP TABLE {tableName}", SqLiteConnection))
				command.ExecuteNonQuery();
		}
	}
}
