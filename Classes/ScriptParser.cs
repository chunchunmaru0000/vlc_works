using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace vlc_works
{
    public class ScriptParser
    {
        private string ScriptFilePath { get; set; }
        private Encoding Encoding { get; } = Encoding.UTF8;
        #region ERR_MSGS
        private const string firstGameWasnFound = "В СКРИПТЕ ИГРЫ НЕ БЫЛА НАЙДЕНА ПЕРВАЯ ИГРА";
        #endregion ERR_MSGS

        public ScriptParser(string scriptFilePath)
        {
            ScriptFilePath = scriptFilePath.HebrewTrim().Trim();
        }

        /// <summary>
        /// Throws an Exception if any issues with file so there is a need to use try catch
        /// </summary>
        /// <returns></returns>
        public Tuple<GameScript, List<GameScript>> Parse()
        {
            string[] scriptLines = ParseFile();
            string firstGameLine = FindFirstGameLine(scriptLines);
            scriptLines = GetScriptLinesWithoutFirstGameLine(scriptLines);

            GameScript firstGameScript = ParseGameLine(firstGameLine);
            List<GameScript> gameScripts = 
                scriptLines
                .Select(ParseGameLine)
                .ToList();

            return new Tuple<GameScript, List<GameScript>>(firstGameScript, gameScripts);
        }

        private string[] ParseFile() =>
            File
            .ReadAllText(ScriptFilePath, Encoding)
            .HebrewTrim()
            .Replace("\r", "")
            .Split('\n')
            .Where(line => 
                !line.StartsWith("\\"))
            .Select(line => 
                line
                .Trim()
                .ToLower())
            .ToArray();

        private string FindFirstGameLine(string[] scriptLines)
        {
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

        private GameScript ParseGameLine(string gameLine)
        {

        }
    }
}
