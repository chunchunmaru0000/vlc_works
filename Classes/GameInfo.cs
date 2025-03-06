using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public Dictionary<GameMode, int> GameIndices { get; set; }

        public GameScript[] GameModeScripts { get => ModeScripts[GameMode]; }
        private int GameIndex { get => GameIndices[GameMode]; }

        private int WonCounter { get; set; }
        private int LostCounter { get; set; }

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
            ModeStartPoints = modeStartPoints;

            GameMode = GameMode.ALL;
            GameIndices = new Dictionary<GameMode, int>() {
                { GameMode.ALL, -1 },
                { GameMode.MEDIUM, 0 },
                { GameMode.HARD, 0 },
            };
        }

        public void SetGameIndex(int index)
        {
            GameIndices[GameMode] = index;
            if (Utils.IsFormAlive(AccountingForm) &&
                Utils.IsFormAlive(AccountingForm.scriptEditor))
                AccountingForm.scriptEditor.Invoke(new Action(() =>
                AccountingForm.scriptEditor.SetGameModeAndScript(
                    //GameMode,
                    //GameModeScripts
                    AccountingForm.scriptEditor.tableMode,
                    ModeScripts[AccountingForm.scriptEditor.tableMode]
                    )));
        }

        public void ResetWonCounter() => WonCounter = 0;

        public void ResetLostCounter() => LostCounter = 0;

        public void IncGameIndex(int index)
        {
            WonCounter++;
            ResetLostCounter();

            int gameIndex;

            if (WonCounter >= 3 && GameMode != GameMode.HARD) {
                ResetWonCounter();

                // ALL -> MEDIUM -> HARD the same as 0 -> 1 -> 2
                GameMode = (GameMode)((int)GameMode + 1);
                gameIndex = 0;
            }
            else
                gameIndex = GameIndex + 1;

            if (gameIndex >= GameScripts.Length)
            SetGameIndex(gameIndex);
        }

        public void DecGameIndex(int index)
        {

        }
    }
}
