namespace vlc_works
{
	public class DbPlayer
	{
		public string PlayerIdStr { get; set; }
		public long C { get; set; }
		public long K { get; set; }
		public long M { get; set; }

		public DbPlayer(string playerIdStr, long c, long k, long m)
		{
			PlayerIdStr = playerIdStr;
			C = c;
			K = k;
			M = m;
		}
	}
}
