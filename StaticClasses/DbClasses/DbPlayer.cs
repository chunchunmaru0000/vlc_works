using System;

namespace vlc_works015
{
	public class DbPlayer
	{
        public long Id { get; }
		public long PlayerIdInt { get; set; }
		public long C { get; set; }
		public long K { get; set; }
		public long M { get; set; }

		public DbPlayer(long id, long playerIdInt, long c, long k, long m)
		{
            Id = id;
			PlayerIdInt = playerIdInt;
			C = c;
			K = k;
			M = m;
		}

		public static DbPlayer FromArray(object[] arr) =>
			new DbPlayer(
                Convert.ToInt64(arr[0]),
				Convert.ToInt64(arr[1]),
				Convert.ToInt64(arr[2]),
				Convert.ToInt64(arr[3]),
				Convert.ToInt64(arr[4]));

        public override string ToString() =>
            $"Id:{Id}|PId:{PlayerIdInt}|C:{C}|K:{K}|M:{M}";
    }
}
