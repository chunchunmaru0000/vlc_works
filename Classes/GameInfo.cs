using System.Collections.Generic;

namespace vlc_works
{
    public class GameInfo
    {
        public GameScript FirstGame { get; set; }
        public GameScript[] GameScripts { get; set; }
        public Dictionary<GameLabel, GameScript[]> LabelScripts { get; set; }

        public GameInfo(GameScript firstGame, GameScript[] gameScripts, Dictionary<GameLabel, GameScript[]> labelScripts)
        {
            FirstGame = firstGame;
            GameScripts = gameScripts;
            LabelScripts = labelScripts;
        }
    }
}
