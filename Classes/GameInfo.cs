using System.Collections.Generic;

namespace vlc_works
{
    public class GameInfo
    {
        public GameScript FirstGame { get; set; }
        public GameScript[] GameScripts { get; set; } // the same as ModeScripts[GameMode.ALL]
        public Dictionary<GameMode, GameScript[]> ModeScripts { get; set; }
        public GameMode GameMode { get; set; } = GameMode.ALL;

        public GameInfo(GameScript firstGame, GameScript[] gameScripts, Dictionary<GameMode, GameScript[]> labelScripts)
        {
            FirstGame = firstGame;
            GameScripts = gameScripts;
            ModeScripts = labelScripts;
        }
    }
}
