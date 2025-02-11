namespace vlc_works
{
	public static class StringExtensions
	{
		public static string HebrewTrim(this string str)
		{
			return 
				string.IsNullOrEmpty(str) 
					? str
					: str
						.Replace("\u202A", "")
						.Replace("\u202B", "")
						.Replace("\u202C", "")
						.Replace("\u200F", "")
						;
		}
	}
}
