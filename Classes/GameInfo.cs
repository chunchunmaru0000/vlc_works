﻿using System;
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
            Console.WriteLine($"ModeStartPoints: \n\t{string.Join("\n\t", ModeStartPoints.Select(m => $"{m.Key}:{m.Value}"))}");

            GameMode = GameMode.ALL;
            GameIndices = new Dictionary<GameMode, int>() {
                { GameMode.ALL, -1 },
                { GameMode.MEDIUM, 0 },
                { GameMode.HARD, 0 },
            };
        }

        private void SetGameIndex(int index)
        {
            if (index < ModeScripts[GameMode].Length) {
                GameIndices[GameMode] = index;

                for (int i = 0; i < 10; i++)
                Console.WriteLine($"SET BOXES TO {index} INDEX");

                if (index >= 0)
                    SetBoxesToPlayScript(index, CurrentScript);
            }

            if (Utils.IsFormAlive(AccountingForm) && Utils.IsFormAlive(AccountingForm.scriptEditor)) {

                GameMode mode =
                    GameMode;
                    //AccountingForm.scriptEditor.tableMode;

                AccountingForm.scriptEditor.Invoke(new Action(() =>
                    AccountingForm.scriptEditor.SetGameModeAndScript(mode, ModeScripts[mode])));
            }
        }

        public void SetBoxesToPlayScript(int scriptIndex, GameScript script)
        {
            Console.WriteLine($"[[[ SetBoxesUntilScript {script} ]]]");

            GameType[] allTypes = Utils.EnumValues<GameType>();
            GameMode[] allModes = Utils.EnumValues<GameMode>();

            int scriptType = (int)script.GameType;
            Dictionary<GameType, long> typeLvls =
                allTypes
                .ToDictionary(k => k, v => 
                    ((scriptType - (int)v) > 0 ? 1 : 0) + 
                    script.Lvl
                );

            foreach(GameType type in allTypes) {
                long lvl = FindLvlOfType(GameMode, type, typeLvls[type]);
                AccountingForm.clientForm.SetBox(type, lvl);
            }

            Console.WriteLine($"[[[ SetBoxesUntilScript {script} ]]]");
        }

        private long FindLvlOfType(GameMode startMode, GameType findType, long maxLvl)
        {
            GameScript[] findScriptsOfMode =
                ModeScripts[startMode]
                .Where(s => s.GameType == findType && s.Lvl <= maxLvl)
                .ToArray();

            return
                findScriptsOfMode.Length > 0
                ? findScriptsOfMode.Last().Lvl
                : (int)startMode > 0
                    ? FindLvlOfType((GameMode)((int)startMode - 1), findType, maxLvl)
                    : 0;
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

        public void SetWonCounter(int count) => WonCounter = count;

        public void ClearWonCounter() => WonCounter = 0;

        public void SetLostCounter(int count) => LostCounter = count;

        public void ClearLostCounter() => LostCounter = 0;

        private static GameType LESS_TYPE { get; } = Utils.EnumValues<GameType>().First(); // supposedly C - Guard = 0
        private static int MAX_LVL { get; } = 9;
        private static int MIN_LVL { get; } = 0;

        private int GetNextGameIndex(int maxOffset, int defValue, bool dec, Func<int, bool> exitCondition)
        {
            int currentLessTypeLvl =
                (int)
                GameModeScripts
                .Take(GameIndex + 1)
                .Last(s => s.GameType == LESS_TYPE).Lvl
                + Convert.ToInt32(CurrentScript.GameType != LESS_TYPE);

            int nextLvl = 
                dec
                ? Math.Max(MIN_LVL, currentLessTypeLvl + maxOffset)
                : Math.Min(MAX_LVL, currentLessTypeLvl + maxOffset);

            if (exitCondition(nextLvl))
                return defValue;

            int index = Array.FindIndex(GameModeScripts, s => s.GameType == LESS_TYPE && s.Lvl == nextLvl);
            return index == -1 ? defValue : index;
        }

        public void IncGameIndex()
        {
            WonCounter++;
            ClearLostCounter();

            int gameIndex;

            if (WonCounter >= 3) {
                ClearWonCounter();
                gameIndex = GetNextGameIndex(2, GameIndex + 1, false, (nl) => nl >= MAX_LVL);
                if (gameIndex >= GameModeScripts.Length)
                    gameIndex = GameModeScripts.Length - 1;
            }
            else
                gameIndex = GameIndex + 1;

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

            int gameIndex = Math.Max(0, GetNextGameIndex(-1, GameIndex - 1, true, (nl) => nl < MIN_LVL));

            SetGameIndex(gameIndex);
            ClearLostCounter();

            Debug();
        }
    }
}
