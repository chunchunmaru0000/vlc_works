using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vlc_works
{
    public class GameDirectory
    {
        private string GameDirectoryPath { get; set; }
        private Random rnd = new Random();

        public GameDirectory(string gameDirectoryPath)
        {
            GameDirectoryPath = gameDirectoryPath;
        }

        public string GetScriptDirectory(GameScript gameScript)
        {
            string gameFoldersDirectory = Path.Combine(new string[] {
                GameDirectoryPath,
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
            int[] levels = Enumerable.Range(0, 10).ToArray();
            string[] games = new string[] {
                GameType.Guard.View(),
                GameType.Painting.View(),
                GameType.Mario.View(),
            };

            List<string> errs = new List<string>();

            foreach (int level in levels) {
                string path = Path.Combine(new string[] {
                    GameDirectoryPath,
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
                AssertGameScript(firstScript),

                gameScripts.Select(AssertGameScript)
                .SelectMany(s => s)
                .ToArray()
            }
            .SelectMany(a => a)
            .ToArray();

        private string[] AssertGameScript(GameScript gameScript)
        {
            string gameFoldersDirectory = Path.Combine(new string[] {
                GameDirectoryPath,
                $"уровень {gameScript.Lvl}",
            });
            if (!Directory.Exists(gameFoldersDirectory))
                return new string[1] { $"НЕСУЩЕСТВУЮЩАЯ ПАПКА {gameFoldersDirectory}" };

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

        public GameVideo GetRandomGame(GameScript gameScript)
        {
            string directory = GetScriptDirectory(gameScript);

            // game file starts with 8 nums e.g. 10512345 and ends on .mp4
            string[] files = 
                Directory.EnumerateFiles(directory)
                .Where(f => 
                    Path.GetFileName(f).Length > 8 &&
                    Path.GetExtension(f) == ".mp4" && 
                    Path.GetFileName(f)
                        .Substring(0, 8)
                        .All(fc => char.IsNumber(fc))
                    )
                .ToArray();
            
            if (files.Length < 1)
                throw new InvalidOperationException($"НЕТ ФАЙЛОВ ВИДЕО В ПАПКЕ: \n{directory}");

            string gameFile = files[rnd.Next(files.Length)];
            return new GameVideo(
                new PathUri(gameFile), 
                new PathUri(Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(gameFile)}_stop.mp3")));
             

            return new GameVideo(
                new PathUri("C:\\Users\\cho22\\OneDrive\\Desktop\\vlcvideos\\example\\51055555.mp4"),
                new PathUri("C:\\Users\\cho22\\OneDrive\\Desktop\\vlcvideos\\example\\51055555_stop.mp4"));
        }
    }
}
