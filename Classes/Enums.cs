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
	}

	public enum Langs
	{
		RUSSIAN,
		ENGLISH,
		HEBREW,
	}
}
