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
        private Func<string, string> errorPartParseLvl { get; } = (lvl) => 
            $"ОШИБКА ПРИ ЧТЕНИИ ЧИСЕЛ СКРИПТА\n\tlvl = {lvl}";
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
                    .Split('|')
                    .Select(part => part.Trim())
                    .ToArray();

                string[] firstPart = parts[0].Split(';');
                parts[0] = string.Join(";", firstPart.Skip(1));
                
                char typeChar = firstPart[0][0];
                if (!CharToGameType.ContainsKey(typeChar))
                    throw new Exception(unknownTypeChar(typeChar));
                GameType gameType = CharToGameType[typeChar];

                if (!long.TryParse(firstPart[0].Substring(1), out long lvl))
                    throw new Exception(errorPartParseLvl(firstPart[0].Substring(1)));

                return new Dictionary<GameMode, GameScript>() {
                    { GameMode.LOW, ParseGameParams(gameType, lvl, parts[0]) },
                    { GameMode.MID, ParseGameParams(gameType, lvl, parts[1]) },
                    { GameMode.HIGH, ParseGameParams(gameType, lvl, parts[2]) },
                };
            } catch (Exception e) {
                throw new Exception(errorParseGameLine(e.Message, gameLine));
            }
        }

        private GameScript ParseFirstGameLine(string line)
        {
            string[] parts =
                line
                .Split(';')
                .Select(part => part.Trim())
                .ToArray();

            char typeChar = parts[0][0];
            if (!CharToGameType.ContainsKey(typeChar))
                throw new Exception(unknownTypeChar(typeChar));
            GameType gameType = CharToGameType[typeChar];

            if (!long.TryParse(parts[0].Substring(1), out long lvl))
                throw new Exception(errorPartParseLvl(parts[0].Substring(1)));

            return ParseGameParams(gameType, lvl, string.Join(";", parts.Skip(1)));
        }
    }
}
