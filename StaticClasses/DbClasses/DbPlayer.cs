using System;

namespace vlc_works
{
	public class DbPlayer
	{
		public long PlayerIdInt { get; set; }
		public long C { get; set; }
		public long K { get; set; }
		public long M { get; set; }

		public DbPlayer(long playerIdInt, long c, long k, long m)
		{
			PlayerIdInt = playerIdInt;
			C = c;
			K = k;
			M = m;
		}

		public static DbPlayer FromArray(object[] arr) =>
			new DbPlayer(
				Convert.ToInt64(arr[1]), // 0 is id that is not used
				Convert.ToInt64(arr[2]),
				Convert.ToInt64(arr[3]),
				Convert.ToInt64(arr[4]));
	}
}
