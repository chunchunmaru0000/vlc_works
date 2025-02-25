using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vlc_works
{
    public class GameDirectory
    {
        private string GameDirectoryPath { get; set; }
        private Dictionary<GameType, string> GameTypeToFolderName { get; set; }

        public GameDirectory(string gameDirectoryPath, Dictionary<GameType, string> gameTypeToFolderName)
        {
            GameDirectoryPath = gameDirectoryPath;
            GameTypeToFolderName = gameTypeToFolderName;
        }

        public string GetScriptDirectory(GameScript gameScript, Langs language)
        {
            return Path.Combine(new string[] {
                GameDirectoryPath,
                language.View(),
                $"уровень {gameScript.Lvl}",
                GameTypeToFolderName[gameScript.GameType],
            });
        }

        /// <summary>
        /// Returns an array of unexisting directores
        /// </summary>
        public string[] AssertGameDirectoryFolders()
        {
            Langs[] langs = new Langs[] { Langs.RUSSIAN, Langs.ENGLISH, Langs.ENGLISH };
            int[] levels = Enumerable.Range(0, 10).ToArray();
            string[] games = GameTypeToFolderName.Values.ToArray();

            List<string> errs = new List<string>();

            foreach (Langs lang in langs)
                foreach (int level in levels)
                    foreach (string game in games) {
                        string path = Path.Combine(new string[] {
                            GameDirectoryPath,
                            lang.View(),
                            $"уровень {level}",
                            game,
                        });

                        if (!Directory.Exists(path))
                            errs.Add($"НЕСУЩЕСТВУЮЩАЯ ПАПКА {path}");
                    }
            return errs.ToArray();
        }
    }
}
