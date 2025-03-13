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
        public GameScript CurrentScript { get => 
                GameIndex == -1 
                ? FirstGame
                : GameModeScripts[GameIndex]; }

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

        private void SetGameIndex(int index, bool SAME_LEVEL = false)
        {
            if (index < ModeScripts[GameMode].Length) {
                GameIndices[GameMode] = index;

                for (int i = 0; i < 10; i++)
                Console.WriteLine($"SET BOXES TO {index} INDEX");

                if (index >= 0)
                    SetBoxesToPlayScript(CurrentScript, SAME_LEVEL);
            }

            if (Utils.IsFormAlive(AccountingForm) && Utils.IsFormAlive(AccountingForm.scriptEditor)) {

                GameMode mode =
                    GameMode;
                    //AccountingForm.scriptEditor.tableMode;

                AccountingForm.scriptEditor.Invoke(new Action(() =>
                    AccountingForm.scriptEditor.SetGameModeAndScript(mode, ModeScripts[mode])));
            }
        }

        public void SetBoxesToPlayScript(GameScript script, bool SAME_LEVEL)
        {
            Console.WriteLine($"[[[ SetBoxesUntilScript {script} ]]]");

            int tmpIndex =
                ModeScripts[GameMode.ALL].Length -
                GameModeScripts.Length + GameIndex;
            int scriptIndex = tmpIndex + 1;

            if (scriptIndex >= GameModeScripts.Length
                ||
                GameModeScripts[tmpIndex].GameType == GameModeScripts[scriptIndex].GameType
                )
                scriptIndex = tmpIndex;

            Console.WriteLine($"{scriptIndex} -> {ModeScripts[GameMode.ALL][scriptIndex]}");
            /*
            ALSO BUG IN DEC GAME INDEX
            if (SAME_LEVEL) {
                long scriptLvl = script.Lvl;

                foreach(GameType gameType in Enum.GetValues(typeof(GameType))) {
                    if (GameModeScripts.Any(s => s.Lvl == script.Lvl && s.GameType == gameType)) {
                        AccountingForm.clientForm.SetBox(gameType, scriptLvl);
                    } else {
                        GameScript[] thisTypeScripts =
                            ModeScripts[GameMode.ALL]
                            .Where(s => s.GameType == gameType)
                            .ToArray();

                        AccountingForm.clientForm.SetBox(gameType,
                            thisTypeScripts.Length == 0
                            ? 0
                            : thisTypeScripts.Where(s => s.Lvl <= scriptLvl).Count() == 0
                                ? 0
                                : thisTypeScripts.Where(s => s.Lvl <= scriptLvl).Max(s => s.Lvl)
                        );
                    }
                }
            } else
                   */ {
                List<GameType> added = new List<GameType>();

                for (int i = scriptIndex; i >= 0; i--) {
                    GameScript iScript = ModeScripts[GameMode.ALL][i];
                    GameType scriptType = iScript.GameType;

                    Console.Write($"\t{i} -> {iScript}, BEFORE [{string.Join(", ", added.Select(t => t.View()))}]");

                    if (!added.Contains(scriptType)) {
                        added.Add(scriptType);
                        AccountingForm.clientForm.SetBox(scriptType, iScript.Lvl);
                    }

                    Console.WriteLine($" --->>> _AFTER [{string.Join(", ", added.Select(t => t.View()))}]");
                }
            }


            Console.WriteLine($"[[[ SetBoxesUntilScript {script} ]]]");
        }

        #region DEBUG

        private void Debug()
        {
            if (Utils.DEBUG_FORM && Utils.IsFormAlive(AccountingForm.debugForm))
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

        #endregion DEBUG

        public void ClearCounters()
        {
            ClearWonCounter();
            ClearLostCounter();
        }

        public int[] GetCounters() => new int[2] { WonCounter, LostCounter };

        public void ClearGameIndicesAndSetFirst(int index)
        {
            GameIndices[GameMode.MEDIUM] = 0;
            GameIndices[GameMode.HARD] = 0;

            GameMode = GameMode.ALL;
            SetGameIndex(index);

            Debug();
        }

        public void ClearWonCounter() => WonCounter = 0;

        public void SetWonCounter(int count) => WonCounter = count;

        public void SetLostCounter(int count) => LostCounter = count;

        public void ClearLostCounter() => LostCounter = 0;

        public void IncGameIndex()
        {
            WonCounter++;
            ClearLostCounter();

            int gameIndex;
            bool SAME_LEVEL = false;

            if (WonCounter >= 3 && GameMode != GameMode.HARD) {
                ClearWonCounter();
                SAME_LEVEL = true;

                int lastIndex = GameIndex;
                GameMode lastMode = GameMode;

                // ALL -> MEDIUM -> HARD the same as 0 -> 1 -> 2
                GameMode = (GameMode)((int)GameMode + 1);

                // A5 -> M6 | M6 -> H7 | A9 -> M9 -> H9
                gameIndex = 0;
                /*
                    ModeScripts[lastMode].Length - GameModeScripts.Length 
                    - 
                    ;
                if (gameIndex + )
                    gameIndex = 0;
                else if (gameIndex >= GameModeScripts.Length) // if 10 should become 9 as it's max lvl
                    gameIndex = GameModeScripts.Length - 1;
                 */
            }
            else
                gameIndex = GameIndex + 1;

            SetGameIndex(gameIndex, SAME_LEVEL);

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

                GameMode lastMode = GameMode;
                // HARD -> MEDIUM -> ALL
                GameMode = (GameMode)((int)GameMode - 1);
                gameIndex = ModeScripts[GameMode].Length - ModeScripts[lastMode].Length - 1;
            }
            else
                gameIndex = GameIndex - 1;

            SetGameIndex(gameIndex);
            ClearLostCounter();

            Debug();
        }
    }
}
