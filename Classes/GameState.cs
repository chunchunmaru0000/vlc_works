using System.Collections.Generic;

namespace vlc_works
{
    public class GameState
    {
        public GameScript Script { get; set; }
        public Dictionary<GameType, GameScript> TypeScripts { get; set; }
        public GameState Won { get; set; } // nullable
        public GameState Lost { get; set; } // nullable

        public GameState(GameScript script, Dictionary<GameType, GameScript> typeScripts, GameState won, GameState lost)
        {
            Script = script;
            TypeScripts = typeScripts;
            Won = won;
            Lost = lost;
        }
    }
}
