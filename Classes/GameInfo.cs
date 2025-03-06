using System;
using System.Collections.Generic;

namespace vlc_works
{
    public class GameInfo
    {
        private AccountingForm AccountingForm { get; set; }

        public GameScript FirstGame { get; set; }
        private GameScript[] GameScripts { get; set; } // the same as ModeScripts[GameMode.ALL]
        public Dictionary<GameMode, GameScript[]> ModeScripts { get; set; }
        private Dictionary<GameMode, int> ModeStartPoints { get; set; }
        public GameMode GameMode { get; set; }
        private Dictionary<GameMode, int> GameIndices { get; set; }

        public GameScript[] GameModeScripts { get => ModeScripts[GameMode]; }
        private int GameIndex { get => GameIndices[GameMode]; }
        public GameScript CurrentScript { get => ModeScripts[GameMode][GameIndex]; }

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

        private void SetGameIndex(int index)
        {
            if (index >= ModeScripts[GameMode].Length)
                index = 0;

            GameIndices[GameMode] = index;

            GameMode mode =
                //GameMode;
                AccountingForm.scriptEditor.tableMode;

            if (Utils.IsFormAlive(AccountingForm) &&
                Utils.IsFormAlive(AccountingForm.scriptEditor)) {
                AccountingForm.scriptEditor.Invoke(new Action(() =>
                    AccountingForm.scriptEditor.SetGameModeAndScript(mode, ModeScripts[mode])));

                if (Utils.DEBUG_FORM) {
                    const bool a = false;
                    if (a) Console.Beep();
                }
            }
        }

        public void ClearCounters()
        {
            ClearWonCounter();
            ClearLostCounter();
        }

        public void ClearGameIndicesAndSetFirst(int index)
        {
            foreach (GameMode key in GameIndices.Keys)
                GameIndices[key] = 0;

            GameIndices[GameMode.ALL] = index;
        }

        public void ClearWonCounter() => WonCounter = 0;

        public void ClearLostCounter() => LostCounter = 0;

        public void IncGameIndex()
        {
            WonCounter++;
            ClearLostCounter();

            int gameIndex;

            if (WonCounter >= 3 && GameMode != GameMode.HARD) {
                ClearWonCounter();

                // ALL -> MEDIUM -> HARD the same as 0 -> 1 -> 2
                GameMode = (GameMode)((int)GameMode + 1);
                gameIndex = 0;
            }
            else
                gameIndex = GameIndex + 1;

            if (gameIndex >= GameScripts.Length)
            SetGameIndex(gameIndex);
        }

        public void IncLostCounter()
        {
            LostCounter++;
            ClearWonCounter();

            if (LostCounter < 3)
                return;
            int gameIndex;
            
            if(GameIndex == 0) {
                if (GameMode == GameMode.ALL)
                    return;

                ClearLostCounter();

                // HARD -> MEDIUM -> ALL
                GameMode = (GameMode)((int)GameMode - 1);
                gameIndex = ModeScripts[GameMode].Length - 1;
            }
            else
                gameIndex = GameIndex - 1;

            SetGameIndex(gameIndex);
        }
    }
}
