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
        public string[] AssertAllGameDirectoryFolders()
        {
            Langs[] langs = new Langs[] { Langs.RUSSIAN, Langs.ENGLISH, Langs.HEBREW };
            int[] levels = Enumerable.Range(0, 10).ToArray();
            string[] games = new string[] {
                GameType.Guard.View(),
                GameType.Painting.View(),
                GameType.Mario.View(),
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

        /// <summary>
        /// Returns an array of unexisting directores
        /// </summary>
        public string[] AssertScriptDirectoryFolders(GameScript firstScript, GameScript[] gameScripts) =>
            new List<string[]> {
                AssertGameScriptForAllLangs(firstScript),

                gameScripts.Select(AssertGameScriptForAllLangs)
                .SelectMany(s => s)
                .ToArray()
            }
            .SelectMany(a => a)
            .ToArray();

        public string[] AssertGameScriptForAllLangs(GameScript gameScript) =>
            new List<string[]> {
                TryGameScriptForLang(gameScript, Langs.RUSSIAN),
                TryGameScriptForLang(gameScript, Langs.ENGLISH),
                TryGameScriptForLang(gameScript, Langs.HEBREW),
            }
            .SelectMany(a => a)
            .ToArray();

        private string[] TryGameScriptForLang(GameScript gameScript, Langs language)
        {
            string gameFoldersDirectory = Path.Combine(new string[] {
                GameDirectoryPath,
                language.View(),
                $"уровень {gameScript.Lvl}",
            });

            string game = gameScript.GameType.View();

            string[] gameScriptFolders =
                Directory
                .GetDirectories(gameFoldersDirectory)
                .Where(f =>
                    Path
                    .GetFileName(f)
                    .StartsWith(game))
                .ToArray();

            return gameScriptFolders.Length == 0
                ? new string[1] { $"НЕСУЩЕСТВУЮЩАЯ ПАПКА {gameFoldersDirectory} + \\{game}..." }
                : new string[0];
        }

        public PathUri GetRandomGame(GameScript gameScript, Langs language)
        {
            string directory = GetScriptDirectory(gameScript, language);

            // game file starts with 8 nums e.g. 10512345 and ends on .mp4
            string[] files = 
                Directory.GetFiles(directory)
                .Where(f => 
                    Path.GetExtension(f) == "mp4" && 
                    f.Substring(0, 8).All(fc => char.IsNumber(fc))
                    )
                .ToArray();

            return new PathUri(path);
        }
    }
}
