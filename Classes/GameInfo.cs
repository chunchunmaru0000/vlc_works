using System.Collections.Generic;

namespace vlc_works
{
    public class GameInfo
    {
        public GameScript FirstGame { get; set; }
        public GameScript[] GameScripts { get; set; }
        public Dictionary<GameMode, GameScript[]> LabelScripts { get; set; }
        public GameMode GameMode { get; set; } = GameMode.ALL;

        public GameInfo(GameScript firstGame, GameScript[] gameScripts, Dictionary<GameMode, GameScript[]> labelScripts)
        {
            FirstGame = firstGame;
            GameScripts = gameScripts;
            LabelScripts = labelScripts;
        }
    }
}
