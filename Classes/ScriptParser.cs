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
        private string firstGameWasnFound { get; } = "В СКРИПТЕ ИГРЫ НЕ БЫЛА НАЙДЕНА ПЕРВАЯ ИГРА";
        private string errorPartParse { get; } = "ОШИБКА ПРИ ЧТЕНИИ ЧИСЕЛ СКРИПТА";

        private Func<string, string> errorParseFile { get; } = (msg) => $"ОШИБКА ПРИ ПАРСИНГЕ ФАЙЛА:\n{msg}";
        private Func<string, string, string> errorParseGameLine { get; } = 
            (msg, l) => $"ОШИБКА ПРИ ПАРСИНГЕ СКРИПТА [{l}]:\n{msg}";
        private Func<char, string> unknownTypeChar { get; } = (c) => 
            $"НЕИЗВЕСТНЫЙ СИМВОЛ [{c}] [{c.ToString()}]\n" +
            $"ДАННЫЙ СИМВОЛ НЕ ВХОДИТ В СПИСОК ИСПОЛЬЗУЕМЫХ:\n" +
            $"\t[{string.Join("|", CharToGameType.Select(p => p.Key.ToString()))}]";
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
            GameScript firstGameScript = ParseFirstGameLine(firstGameLine);

            Dictionary<GameMode, GameScript[]> modeScripts =
                Utils.EnumValues<GameMode>()
                .Select(gm => new KeyValuePair<GameMode, GameScript[]>( 
                    key: gm,
                    value:
                        GetScriptLinesWithoutFirstGameLine(scriptLines)
                        .Select(l => ParseGameLine(l)[gm])
                        .ToArray()))
                .ToDictionary(p => p.Key, p => p.Value);

            return new GameInfo(firstGameScript, modeScripts, accountingForm);
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

        private static Dictionary<char, GameType> CharToGameType { get; } = 
            new Dictionary<char, GameType>() {
                { 'c', GameType.Guard },
                { 'с', GameType.Guard },
                { 'k', GameType.Painting },
                { 'к', GameType.Painting },
                { 'm', GameType.Mario },
                { 'м', GameType.Mario },
            };

        private GameScript ParseGameParams(GameType gameType, long lvl, string pars)
        {
            long[] prs = 
                pars
                .Split(';')
                .Select(p => Convert.ToInt64(p))
                .ToArray();
            return new GameScript(gameType, lvl, prs[0], prs[1]);
        }

        private Dictionary<GameMode, GameScript> ParseGameLine(string gameLine)
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
    }
}
