using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vlc_works
{
    public class GameDirectory
    {
        private string GameDirectoryPath { get; set; }

        public GameDirectory(string gameDirectoryPath)
        {
            GameDirectoryPath = gameDirectoryPath;
        }

        public string GetScriptDirectory(GameScript gameScript, Langs language)
        {
            string gameFoldersDirectory = Path.Combine(new string[] {
                GameDirectoryPath,
                language.View(),
                $"уровень {gameScript.Lvl}",
                //GameTypeToFolderName[gameScript.GameType],
            });

            string[] gameScriptFolders =
                Directory
                .GetDirectories(gameFoldersDirectory)
                .Where(f =>
                    Path
                    .GetFileName(f)
                    .StartsWith(gameScript.GameType.View()))
                .ToArray();

            return gameScriptFolders[0];
        }

        /// <summary>
        /// Returns an array of unexisting directores
        /// </summary>
        public string[] AssertGameDirectoryFolders()
        {
            Langs[] langs = new Langs[] { Langs.RUSSIAN, Langs.ENGLISH, Langs.HEBREW };
            int[] levels = Enumerable.Range(0, 10).ToArray();
            string[] games = new string[] {
                GameType.Guard.View(),
                GameType.Painting.View(),
                //GameType.Mario.View(),
            };

            List<string> errs = new List<string>();

            foreach (Langs lang in langs)
                foreach (int level in levels) {
                    string path = Path.Combine(new string[] {
                        GameDirectoryPath,
                        lang.View(),
                        $"уровень {level}",
                    });
                    if (!Directory.Exists(path)) {
                        errs.Add($"НЕСУЩЕСТВУЮЩАЯ ПАПКА {path}");
                        continue;
                    }

                    foreach (string game in games) {
                        string[] gameFolders = 
                            Directory
                            .GetDirectories(path)
                            .Where(f => 
                                Path
                                .GetFileName(f)
                                .StartsWith(game))
                            .ToArray();
                        if (gameFolders.Length == 0)
                            errs.Add($"НЕСУЩЕСТВУЮЩАЯ ПАПКА {path} + \\{game}...");
                    }
                }
            return errs.ToArray();
        }
    }
}
