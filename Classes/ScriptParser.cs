using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace vlc_works
{
    public class ScriptParser
    {
        private string ScriptFilePath { get; set; }
        private Encoding Encoding { get; } = Encoding.UTF8;
        #region ERR_MSGS
        private const string firstGameWasnFound = "В СКРИПТЕ ИГРЫ НЕ БЫЛА НАЙДЕНА ПЕРВАЯ ИГРА";
        private const string errorPartParse = "ОШИБКА ПРИ ЧТЕНИИ ЧИСЕЛ СКРИПТА";

        private Func<string, string> errorParseFile = (msg) => $"ОШИБКА ПРИ ПАРСИНГЕ ФАЙЛА:\n{msg}";
        private Func<string, string, string> errorParseGameLine = 
            (msg, l) => $"ОШИБКА ПРИ ПАРСИНГЕ СКРИПТА [{l}]:\n{msg}";
        private Func<char, string> unknownTypeChar = (c) => 
            $"НЕИЗВЕСТНЫЙ СИМВОЛ [{c}] [{c.ToString()}]\n" +
            $"ДАННЫЙ СИМВОЛ НЕ ВХОДИТ В СПИСОК ИСПОЛЬЗУЕМЫХ:\n" +
            $"\t[{string.Join("|", CharToGameType.Select(p => p.Key.ToString()))}]";
        private Func<GameMode, string> labelError = (label) => 
            $"ПРОБЛЕМА С МЕТКОЙ, ВОЗМОЖНА БЫЛА ОШИБКА\n" +
            $"ОЖИДАЛСЯ ОДИН ИЗ ВАРИАНТОВ:\n\t{string.Join("\n\t", label.Views())}";
        #endregion ERR_MSGS

        public ScriptParser(string scriptFilePath)
        {
            ScriptFilePath = scriptFilePath.HebrewTrim().Trim();
        }

        /// <summary>
        /// Throws an Exception if any issues with file so there is a need to use try catch
        /// </summary>
        /// <returns></returns>
        public GameInfo Parse(AccountingForm accountingForm)
        {
            string[] scriptLines = ParseFile();
            string firstGameLine = FindFirstGameLine(scriptLines);
            scriptLines = GetScriptLinesWithoutFirstGameLine(scriptLines);

            GameScript firstGameScript = ParseGameLine(firstGameLine);
            GameScript[] gameScripts = 
                scriptLines
                .Where(line => !line.EndsWith(":")) // not labels
                .Select(ParseGameLine)
                .ToArray();

            int mediumModeCount = GetLabelGameIndex(GameMode.MEDIUM, scriptLines);
            int hardModeCount = 
                GetLabelGameIndex(
                    GameMode.HARD, 
                    scriptLines.Where((l, i) => i != mediumModeCount).ToArray() // skip MEDIUM: label line
                    );

            Dictionary<GameMode, GameScript[]> modeScripts = new Dictionary<GameMode, GameScript[]>() {
                { GameMode.ALL, gameScripts },
                { GameMode.MEDIUM,
                    gameScripts
                    .Skip(mediumModeCount)
                    .Take(hardModeCount - mediumModeCount)
                    .Select(s => s.Clone())
                    .ToArray() },
                { GameMode.HARD,
                    gameScripts
                    .Skip(hardModeCount)
                    .Take(gameScripts.Length - hardModeCount)
                    .Select(s => s.Clone())
                    .ToArray() },
            };

            Dictionary<GameMode, int> modeStartPoints = new Dictionary<GameMode, int>() {
                { GameMode.ALL, 0 },
                { GameMode.MEDIUM, mediumModeCount },
                { GameMode.HARD, hardModeCount },
            };

            return new GameInfo(firstGameScript, modeScripts, modeStartPoints, accountingForm);
        }

        private string[] ParseFile()
        {
            try {
                return
                    File
                    .ReadAllText(ScriptFilePath, Encoding)
                    .HebrewTrim()
                    .Replace("\r", "")
                    .Split('\n')
                    .Where(line => 
                        !line.StartsWith("//") &&
                        !string.IsNullOrEmpty(line) && 
                        !string.IsNullOrWhiteSpace(line))
                    .Select(line => 
                        line
                        .Trim()
                        .ToLower())
                    .ToArray();
            } catch (Exception e) {
                throw new Exception(errorParseFile(e.Message));
            }
        }

        private string FindFirstGameLine(string[] scriptLines)
        {
            //Console.WriteLine(string.Join("\n", scriptLines));
            string firstGameLine =
                scriptLines
                .FirstOrDefault(line => line.StartsWith("первая"));

            if (firstGameLine == null) // default
                throw new Exception(firstGameWasnFound);

            return
                firstGameLine
                .Split('=')
                [1]
                .Trim();
        }

        private string[] GetScriptLinesWithoutFirstGameLine(string[] scriptLines) =>
            scriptLines
                .Where(line =>
                    !line.Contains("первая"))
                .ToArray();

        private static readonly Dictionary<char, GameType> CharToGameType = 
            new Dictionary<char, GameType>() {
                { 'c', GameType.Guard },
                { 'с', GameType.Guard },
                { 'k', GameType.Painting },
                { 'к', GameType.Painting },
                { 'm', GameType.Mario },
                { 'м', GameType.Mario },
            };

        private GameScript ParseGameLine(string gameLine)
        {
            try {
                string[] parts =
                    gameLine
                    .Split(';')
                    .Select(part => part.Trim())
                    .ToArray();

                char typeChar = parts[0][0];
                if (!CharToGameType.ContainsKey(typeChar))
                    throw new Exception(unknownTypeChar(typeChar));

                GameType gameType = CharToGameType[typeChar];

                long lvl = 0, prize = 0, price = 0;
                bool parseResult =
                    long.TryParse(parts[0].Substring(1), out lvl) &&
                    long.TryParse(parts[1], out prize) &&
                    long.TryParse(parts[2], out price);
                if (!parseResult)
                    throw new Exception(errorPartParse);

                return new GameScript(gameType, lvl, prize, price);
            } catch (Exception e) {
                throw new Exception(errorParseGameLine(e.Message, gameLine));
            }
        }

        private int GetLabelGameIndex(GameMode label, string[] scriptLines)
        {
            try {
                return
                    scriptLines
                    .Select((l, i) => new KeyValuePair<int, string>(i, l.TrimEnd(':').ToLower()))
                    .First(p => label.Views().Contains(p.Value))
                    .Key;
            } catch {
                throw new Exception(labelError(label));
            }
        }
    }
}
