using System;

namespace vlc_works
{
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
		public PathUri GameRules { get; set; }
		public PathUri GameLeftSeconds { get; set; }
		public PathUri GameStopSound { get; set; }
		#endregion

		public Language(Langs lang, 
            string victoryPath, string rulesPath, string paramsPath, string playAgainPath, 
            string howToPayPath, string gamePayedPath, string gameRulesPath, string gameLeftSeconds,
            string gameStopSound)
		{
			Lang = lang;

			Victory = new PathUri(victoryPath);
			Rules = new PathUri(rulesPath);
			Params = new PathUri(paramsPath);
			PlayAgain = new PathUri(playAgainPath);
			HowToPay = new PathUri(howToPayPath);
			GamePayed = new PathUri(gamePayedPath);
            GameRules = new PathUri(gameRulesPath);
            GameLeftSeconds = new PathUri(gameLeftSeconds);
            GameStopSound = new PathUri(gameStopSound);
		}

		public static Language Get(Langs lang, string[] lines, int offset) => new Language(lang, 
			lines[0 + offset], lines[3 + offset], lines[6 + offset], 
			lines[9 + offset], lines[12 + offset], lines[15 + offset],
            lines[27 + offset], lines[30 + offset], lines[33 + offset]
		);
	}
}
