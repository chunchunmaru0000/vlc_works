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
        private string HEADER { get; } = @"
// Любая строка, которая начинается с //, считается комментарием и не влияет на скрипт.
// Символы // не могут быть где-то в строке так как это уже не будет комментарием.

// Схема записи игры:
//	    t - тип игры type
//	    l - уровень level
//	    a - награда award
//	    p - цена price
//	    tl|  a;  p |  a;  p |  a;  p

// Тип и уровень должны писаться вместе.
// Пробелов до типа, после уровня и перед и после символов ; может быть сколько угодно.
// Напрмиер:
// с1    |                123;123| 123;  10 | 123;  10
//         к3|123;123            | 123;  10 | 123;  10
//м0|     123            ;      123             |            123;  10 | 123;  10
// Все вырианты выше являются верными, конечно самые правильно выглядящие варианты ниже:
//      с1| 123;  10 | 123;  10 | 123;  10
//      к1| 321; 100 | 123;  10 | 123;  10
//      м2|  10;  10 | 123;  10 | 123;  10

// Буквы могут быть как английские так и русские, как большие так и маленькие.
// На данный момент доступны только буквы:
//      с - С с C c
//      к - К к K k
//      м - М м M m

// Ниже представлен скрипт, соответствующий текущему тз, в незакомментированном виде:

// Скрипт для первой игры имеет отдельный синтаксис, 
// где сначала пишется слово ПЕРВАЯ с последующим символом = и далее обычным скриптом игры.
// !! Если первая игра проиграна, то дальше будет запущена вторая в скрипте, 
// !! то есть в данном случае: c0;  20;  10
// !! Если первая игры выиграна,  то дальше будет запущена третья в скрипте,
// !! то есть в данном случае: k0;  30;  10
// Любая игра кроме первой будет повторяться до победы игрока в ней.
";
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

        #region READ

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

        private IEnumerable<string> GetParts(string line) =>
            line
            .Split('|')
            .Select(part => part.Trim());

        private KeyValuePair<GameType, long> GetTypeLvl(string firstPart)
        {
            char typeChar = firstPart[0];
            if (!CharToGameType.ContainsKey(typeChar))
                throw new Exception(unknownTypeChar(typeChar));
            GameType gameType = CharToGameType[typeChar];

            if (!long.TryParse(firstPart.Substring(1), out long lvl))
                throw new Exception(errorPartParseLvl(firstPart.Substring(1)));

            return new KeyValuePair<GameType, long>(gameType, lvl);
        }

        private Dictionary<GameMode, GameScript> ParseGameLine(string gameLine)
        {
            try {
                IEnumerable<string> parts = GetParts(gameLine);

                KeyValuePair<GameType, long> typeLvl = GetTypeLvl(parts.ElementAt(0));
                GameType gameType = typeLvl.Key; 
                long lvl = typeLvl.Value;

                return new Dictionary<GameMode, GameScript>() {
                    { GameMode.LOW, ParseGameParams(gameType, lvl, parts.ElementAt(1)) },
                    { GameMode.MID, ParseGameParams(gameType, lvl, parts.ElementAt(2)) },
                    { GameMode.HIGH, ParseGameParams(gameType, lvl, parts.ElementAt(3)) },
                };
            } catch (Exception e) {
                throw new Exception(errorParseGameLine(e.Message, gameLine));
            }
        }

        private GameScript ParseFirstGameLine(string line)
        {
            IEnumerable<string> parts = GetParts(line);
            KeyValuePair<GameType, long> typeLvl = GetTypeLvl(parts.ElementAt(0));

            return ParseGameParams(typeLvl.Key, typeLvl.Value, parts.ElementAt(1));
        }

        #endregion READ

        #region WRITE

        public void SaveGameInfo(GameInfo gameInfo)
        {

        }

        #endregion WRITE
    }
}
