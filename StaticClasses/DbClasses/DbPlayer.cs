using System;

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

		public static DbPlayer FromArray(object[] arr) =>
			new DbPlayer(
				arr[1].ToString(), // 0 is id thas is not used
				Convert.ToInt64(arr[2]),
				Convert.ToInt64(arr[3]),
				Convert.ToInt64(arr[4]));
	}
}
