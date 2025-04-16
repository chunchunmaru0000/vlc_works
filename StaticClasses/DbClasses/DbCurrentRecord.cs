namespace vlc_works015
{
	public static class DbCurrentRecord
	{
		public static long SelectedPrice { get; set; }
		public static long SelectedPrize { get; set; }
		public static long SelectedLvl { get; set; }
		public static GameType SelectedGameType { get; set; } = GameType.Guard;

		public static void SetPricePrizeLvl(long price, long prize, long lvl, GameType gameType)
		{
			SelectedPrice = price;
			SelectedPrize = prize;
			SelectedLvl = lvl;
			SelectedGameType = gameType;
		}
	}
}
