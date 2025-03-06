using System;
using System.Collections.Generic;

namespace vlc_works
{
    public class GameInfo
    {
        public GameScript FirstGame { get; set; }
        public GameScript[] GameScripts { get; set; } // the same as ModeScripts[GameMode.ALL]
        public Dictionary<GameMode, GameScript[]> ModeScripts { get; set; }
        public GameMode GameMode { get; set; }
        public int GameIndex { get; set; }
        public GameScript[] GameModeScripts { get => ModeScripts[GameMode]; }

        public GameInfo(GameScript firstGame, Dictionary<GameMode, GameScript[]> modeScripts)
        {
            FirstGame = firstGame;
            GameScripts = modeScripts[GameMode.ALL];
            ModeScripts = modeScripts;

            GameMode = GameMode.ALL;
            GameIndex = -1;
        }

        public void SetGameIndex(int index, AccountingForm accountingForm)
        {
            GameIndex = index;
            if (Utils.IsFormAlive(accountingForm) &&
                Utils.IsFormAlive(accountingForm.scriptEditor)
                )
                accountingForm.scriptEditor.Invoke(new Action(() =>
                accountingForm.scriptEditor.SetGameModeAndScript(
                    accountingForm.scriptEditor.tableMode,
                    //GameMode,
                    GameModeScripts
                    )));
        }

        public void IncGameIndex(int index)
        {

        }

        public void DecGameIndex(int index)
        {

        }
    }
}
