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
        [StringValue("русский")]
		RUSSIAN,
        [StringValue("английский")]
		ENGLISH,
        [StringValue("иврит")]
		HEBREW,
	}

	public enum GameType
	{
		[StringValue("сторож")]
		Guard,
		[StringValue("картины")]
		Painting,
		[StringValue("марио")]
		Mario,
	}

    public enum Channel
    {
        CAMERA_UP = 1,
        CAMERA_DOWN = 2,
        COINS_LIGHT = 3,
        APPARAT_LIGHT = 4,
    }

    public enum GameMode
    {
        [StringValue("НИЗКИЙ")]
        LOW,
        [StringValue("СРЕДНИЙ")]
        MID,
        [StringValue("ВЫСОКИЙ")]
        HIGH,
    }

    public enum BackupNum
    {
        Finger0 = 0,
        Finger1 = 1,
        Finger2 = 2,
        Finger3 = 3,
        Finger4 = 4,
        Finger5 = 5,
        Finger6 = 6,
        Finger7 = 7,
        Finger8 = 8,
        Finger9 = 9,

        PasswordData = 10,
        CardData = 11,
        AllFingersPasswordsCards = 12,
        AllFingers = 13,

        Face20 = 20,
        Face21 = 21,
        Face22 = 22,
        Face23 = 23,
        Face24 = 24,
        Face25 = 25,
        Face26 = 26,
        Face27 = 27,

        AIFace = 50,
    }
}
