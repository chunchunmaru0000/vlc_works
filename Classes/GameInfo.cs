using System;
using System.Collections.Generic;

namespace vlc_works
{
    public class GameInfo
    {
        private AccountingForm AccountingForm { get; set; }

        public GameScript FirstGame { get; set; }
        public GameScript[] GameScripts { get; set; } // the same as ModeScripts[GameMode.ALL]
        public Dictionary<GameMode, GameScript[]> ModeScripts { get; set; }
        public Dictionary<GameMode, int> ModeStartPoints { get; set; }
        public GameMode GameMode { get; set; }
        public int GameIndex { get; set; }
        public GameScript[] GameModeScripts { get => ModeScripts[GameMode]; }

        private int WonCounter { get; set; }

        public GameInfo(
            GameScript firstGame, 
            Dictionary<GameMode, GameScript[]> modeScripts, 
            Dictionary<GameMode, int> modeStartPoints, 
            AccountingForm accountingForm)
        {
            AccountingForm = accountingForm;

            FirstGame = firstGame;
            GameScripts = modeScripts[GameMode.ALL];
            ModeScripts = modeScripts;

            GameMode = GameMode.ALL;
            GameIndex = -1;
        }

        public void SetGameIndex(int index)
        {
            GameIndex = index;
            if (Utils.IsFormAlive(AccountingForm) &&
                Utils.IsFormAlive(AccountingForm.scriptEditor))
                AccountingForm.scriptEditor.Invoke(new Action(() =>
                AccountingForm.scriptEditor.SetGameModeAndScript(
                    AccountingForm.scriptEditor.tableMode,
                    GameModeScripts
                    )));
        }

        public void ResetWonCounter() => WonCounter = 0;

        public void IncGameIndex(int index)
        {
            WonCounter++;
            
            if (WonCounter >= 3 && GameMode != GameMode.HARD) {
                ResetWonCounter();

                // ALL -> MEDIUM -> HARD == 0 -> 1 -> 2
                GameMode = (GameMode)((int)GameMode + 1);

                switch (GameMode) {
                    // case GameMode.ALL: break; // can't be
                    case GameMode.MEDIUM:
                        int mediumGameIndex = ;
                        SetGameIndex(mediumGameIndex); break;
                    case GameMode.HARD:
                        int hardGameIndex = ;
                        SetGameIndex(hardGameIndex); break;
                }
            } else {

            }
        }

        public void DecGameIndex(int index)
        {

        }
    }
}
