namespace vlc_works
{
	public enum Stage
	{
		IDLE,

		SELECT_LANG,
		RULES,
		COST_AND_PRIZE,
		GAME,

		ERROR,
		GAME_CANT_INPUT,
		VICTORY,

		PLAY_AGAIN,
		HOW_PO_PAY,
		GAME_PAYED,
	}

	public enum Langs
	{
		RUSSIAN,
		ENGLISH,
		HEBREW,
	}

	public enum GameType
	{
		Guard,
		Painting,
		Mario
	}
}
