using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace vlc_works
{
    public static class UDPChecker
    {
        private static UdpClient Sender { get; } = new UdpClient();
        private static int SendPort { get; set; }
        private static UdpClient Receiver { get; set; } = null;
        private static IPEndPoint IP = null;
        private static GameDirectory GameDirectory { get; set; }

        public static bool IsActive { get => Receiver != null && IP != null; }

        public static void Constructor(int sendPort, int receivePort, GameDirectory gameDirectory)
        {
            Receiver = new UdpClient(receivePort);
            IP = new IPEndPoint(IPAddress.Any, receivePort);
            SendPort = sendPort;
            GameDirectory = gameDirectory;

            ReceiverThread().Start();
        }

        private static void Print(object obj)
        {
            string str = $"{obj}\n";
            Console.WriteLine(str);

            //const string file = "UDP_REPORT.txt";
            //File.AppendAllText(file, str, encoding: Encoding.UTF8);
        }

        private static List<GameScript> Queue { get; set; } = new List<GameScript>();
        private static bool Received { get; set; } = true;
        private static int PathCounter { get; set; } = 0;

        private static GameVideo GameVideo { get; set; } = new GameVideo(new PathUri("C:\\"), new PathUri("C:\\"));
        private static int Code { get; set; } = 0;

        private static void SetReceived(bool value)
        {
            Received = value;
        }

        public static void Send(GameScript script)
        {
            Queue.Add(script);
            TrySend();
        }

        private static void TrySend()
        {
            if (!Received || Queue.Count == 0)
                return;

            string message = $"{Queue[0].Lvl}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            Sender.Send(data, data.Length, "localhost", SendPort);
            Print($"seneded on {SendPort}: {Encoding.UTF8.GetString(data)}");

            SetReceived(false);
        }

        private static Thread ReceiverThread() => new Thread(() =>
        {
            while (true) {
                Thread.Sleep(250);
                byte[] reveivedData = Receiver.Receive(ref IP);
                string message = 
                    Encoding.UTF8.GetString(reveivedData)
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Replace("\0", "")
                    .HebrewTrim();

                Print($"received: {message}, skip = {message == null || message.Length == 0}");
                if (message == null || message.Length == 0)
                    continue;

                if (message.StartsWith("code")) {
                    Code = Convert.ToInt32(message.Split(':')[1]);
                    Print($"# Code was: {Code}");
                }
                else {
                    Print($"# PathCounter: {++PathCounter}");
                    if (PathCounter == 1)
                        GameVideo.Game = new PathUri(message);
                    else {
                        GameVideo.Stop = new PathUri(message);
                        SetReceived(true);
                        PathCounter = 0;

                        GameScript script = Queue[0];
                        int code = Code;
                        string game = GameVideo.Game.Path;
                        string stop = GameVideo.Stop.Path;

                        new Thread(() => {
                            Thread.Sleep(TimeSpan.FromSeconds(8));
                            DoThingsWithCodeAndPaths(script, code, game, stop);
                        }).Start();

                        Queue.RemoveAt(0);
                        TrySend();
                    }
                }
            }
        });

        private static void DoThingsWithCodeAndPaths(GameScript script, int code, string game, string stop)
        {
            Print($"\t- {code}\n\t- {game}\n\t- {stop}");

            string dir = GameDirectory.GetScriptDirectory(script);
            try {
                Print($"INVALID CHARS: {string.Join("|", Path.GetInvalidFileNameChars().Select(c => (int)c))}");
                Print($"{game}" +
                    $"\n\t{File.Exists(game)}" +
                    $"\n\t{string.Join("|", game.Select(c => (int)c))}" +  
                    $"\n\t{Path.Combine(dir, $"000{code}.mp4")}" +
                    $"\n\t{string.Join("|", Path.Combine(dir, $"000{code}.mp4").Select(c => (int)c))}"
                );
                Print($"{stop}" +
                    $"\n\t{File.Exists(stop)}" +
                    $"\n\t{string.Join("|", stop.Select(c => (int)c))}" +
                    $"\n\t{Path.Combine(dir, $"000{code}_stop.mp4")}" +
                    $"\n\t{string.Join("|", Path.Combine(dir, $"000{code}_stop.mp4").Select(c => (int)c))}"
                );

                File.Move(game, Path.Combine(dir, $"000{code}.mp4"));
                File.Move(stop, Path.Combine(dir, $"000{code}_stop.mp4"));

            } catch (Exception e) {
                File.AppendAllText(
                    "FILE ERRORS.txt",
                    $"{e.GetType().Name}" +
                    $"\n\t{game}" +
                    $"\n\t{stop}" +
                    $"\n\t{e.Message}",
                    encoding: Encoding.UTF8);
            }
        }
    }
}
