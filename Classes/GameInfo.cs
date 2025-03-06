using System;
using System.Collections.Generic;
using System.Linq;

namespace vlc_works
{
    public class GameInfo
    {
        private AccountingForm AccountingForm { get; set; }

        public GameScript FirstGame { get; set; }
        public Dictionary<GameMode, GameScript[]> ModeScripts { get; set; }
        private Dictionary<GameMode, int> ModeStartPoints { get; set; }
        public GameMode GameMode { get; set; }
        private Dictionary<GameMode, int> GameIndices { get; set; }

        public GameScript[] GameModeScripts { get => ModeScripts[GameMode]; }
        public int GameIndex { get => GameIndices[GameMode]; }
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
            if (index < ModeScripts[GameMode].Length)
                GameIndices[GameMode] = index;

            if (Utils.IsFormAlive(AccountingForm) &&
                Utils.IsFormAlive(AccountingForm.scriptEditor)) {

                GameMode mode =
                    //GameMode;
                    AccountingForm.scriptEditor.tableMode;

                AccountingForm.scriptEditor.Invoke(new Action(() =>
                    AccountingForm.scriptEditor.SetGameModeAndScript(mode, ModeScripts[mode])));
            }
        }

        private void Debug()
        {
            if (Utils.DEBUG_FORM && Utils.IsFormAlive(AccountingForm) && Utils.IsFormAlive(AccountingForm.debugForm))
                AccountingForm.debugForm.Invoke(new Action(RefreshDebugForm));
        }

        private void RefreshDebugForm()
        {
            DebugForm df = AccountingForm.debugForm;
            df.w.Text = $"WON = {VideoChecker.won}";
            df.wc.Text = $"WON COUNTER = {WonCounter}";
            df.lc.Text = $"LOST COUNTER = {LostCounter}";
            df.gi.Text = $"GAME INDEX = {GameIndex}";
            df.gm.Text = $"GAME MODE = {GameMode.View()}";
            df.cs.Text = $"CURRENT SCRIPT = {CurrentScript}";
            df.gis.Text = string.Join("\n", GameIndices.Select(p => $"{p.Key.View()} = {p.Value}"));
        }

        public void ClearCounters()
        {
            ClearWonCounter();
            ClearLostCounter();
        }

        public void ClearGameIndicesAndSetFirst(int index)
        {
            GameIndices[GameMode.MEDIUM] = 0;
            GameIndices[GameMode.HARD] = 0;

            GameMode = GameMode.ALL;
            SetGameIndex(index);

            Debug();
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

            if (gameIndex < ModeScripts[GameMode.ALL].Length)
                SetGameIndex(gameIndex);

            Debug();
        }

        public void IncLostCounter()
        {
            LostCounter++;
            ClearWonCounter();
            Debug();

            if (LostCounter < 3)
                return;
            int gameIndex;
            
            if(GameIndex == 0) {
                if (GameMode == GameMode.ALL)
                    return;

                // HARD -> MEDIUM -> ALL
                GameMode = (GameMode)((int)GameMode - 1);
                gameIndex = ModeScripts[GameMode].Length - 1;
            }
            else
                gameIndex = GameIndex - 1;

            SetGameIndex(gameIndex);
            ClearLostCounter();

            Debug();
        }
    }
}
