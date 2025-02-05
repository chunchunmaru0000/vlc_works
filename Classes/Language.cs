using System;

namespace vlc_works
{
	public class PathUri
	{
		public string Path { get; set; }
		public Uri Uri { get; set; }

		public PathUri(string path)
		{
			Path = path;
			Uri = new Uri(path);
		}
	}

	public class Language
	{
	    public Langs Lang { get; set; }
		#region WONT_ADDS_OR_BE_CHANGED
		public PathUri Victory { get; set; }
		public PathUri Rules { get; set; }
		public PathUri Params { get; set; }
		#endregion
		#region CHANGEABLE
		public PathUri PlayAgain { get; set; }
		public PathUri HowToPay { get; set; }
		public PathUri GamePayed { get; set; }
		#endregion

		public Language(Langs lang, string victoryPath, string rulesPath, string paramsPath, string playAgainPath, string howToPayPath, string gamePayedPath)
		{
			Lang = lang;

			Victory = new PathUri(victoryPath);
			Rules = new PathUri(rulesPath);
			Params = new PathUri(paramsPath);
			PlayAgain = new PathUri(playAgainPath);
			HowToPay = new PathUri(howToPayPath);
			GamePayed = new PathUri(gamePayedPath);
		}

		public static Language Get(Langs lang, string[] lines, int offset) =>
			new Language(lang, 
				lines[0 + offset], lines[3 + offset], lines[6 + offset], 
				lines[9 + offset], lines[12 + offset], lines[15 + offset]
				);
	}
}
