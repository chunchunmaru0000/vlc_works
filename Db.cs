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
	gameAward        INTEGER NOT NULL,
	gamePrice        INTEGER NOT NULL,
	gameLevel        INTEGER NOT NULL,
	gameStartTime    INTEGER NOT NULL
);";
		public const string selectAll = "select * from games";
		// sql
		static SQLiteConnection SqLiteConnection { get; set; }
		// some
		public static long Now { get { return DateTimeOffset.Now.ToUnixTimeSeconds(); } }
		public static DateTimeOffset SecToTime(long unixSeconds) => DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

		public static void BeginSQL()
		{
			if (!File.Exists(dbName))
				SQLiteConnection.CreateFile(dbName);

			SqLiteConnection = new SQLiteConnection(connectionString);
			SqLiteConnection.Open();

			using (SQLiteCommand command = new SQLiteCommand(createGamesTable, SqLiteConnection))
				command.ExecuteNonQuery();
		}

		public static void EndSQL()
		{
			SqLiteConnection.Close();
		}

		public static void Insert(long gameAward, long gamePrice, long gameLevel, long gameStartTime)
		{
			using (SQLiteCommand command = new SQLiteCommand($@"
insert into games 
( gameAward,   gamePrice,   gameLevel,   gameStartTime)
values
({gameAward}, {gamePrice}, {gameLevel}, {gameStartTime});", SqLiteConnection))
			{
				Console.WriteLine($"INSERTED: {command.ExecuteNonQuery()} ROWS");
			}
		}

		public static DbSelectGamesItem[] SelectAllGames()
		{
			DbSelectGamesItem[] selectItems;

			using (SQLiteCommand command = new SQLiteCommand(selectAll, SqLiteConnection))
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
	}
}
