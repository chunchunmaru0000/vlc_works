using System;
using System.Collections.Generic;
using System.IO;
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
            Console.WriteLine($"seneded on {SendPort}: {Encoding.UTF8.GetString(data)}");

            SetReceived(false);
        }

        private static Thread ReceiverThread() => new Thread(() =>
        {
            while (true) {
                Thread.Sleep(250);
                byte[] reveivedData = Receiver.Receive(ref IP);
                string message = Encoding.UTF8.GetString(reveivedData);

                Console.WriteLine($"received: {message}, skip = {message == null || message.Length == 0}");
                if (message == null || message.Length == 0)
                    continue;

                if (message.StartsWith("code")) {
                    Code = Convert.ToInt32(message.Split(':')[1]);
                    Console.WriteLine($"# Code was: {Code}");
                }
                else {
                    Console.WriteLine($"# PathCounter: {++PathCounter}");
                    if (PathCounter == 1)
                        GameVideo.Game = new PathUri(message);
                    else {
                        GameVideo.Stop = new PathUri(message);
                        SetReceived(true);
                        PathCounter = 0;
                        DoThingsWithCodeAndPaths();
                        Queue.RemoveAt(0);
                        TrySend();
                    }
                }
            }
        });

        private static void DoThingsWithCodeAndPaths()
        {
            Console.WriteLine($"\t- {Code}\n\t- {GameVideo.Game.Path}\n\t- {GameVideo.Stop.Path}");

            string dir = GameDirectory.GetScriptDirectory(Queue[0]);
            try {
                Console.WriteLine($"{GameVideo.Game.Path} {File.Exists(GameVideo.Game.Path)} {Path.Combine(dir, $"000{Code}.mp4")}");
                Console.WriteLine($"{GameVideo.Stop.Path} {File.Exists(GameVideo.Stop.Path)} {Path.Combine(dir, $"000{Code}_stop.mp4")}");

                File.Move(GameVideo.Game.Path, Path.Combine(dir, $"000{Code}.mp4"));
                File.Move(GameVideo.Stop.Path, Path.Combine(dir, $"000{Code}_stop.mp4"));

            } catch (FileNotFoundException e) {
                File.AppendAllText(
                    "FILE ERRORS.txt", 
                    $"вот ошибка может изза тире в пути в папке гдето в {GameVideo.Game.Path} или " +
                    $"{GameVideo.Stop.Path}\n{e.Message}\n");
            }
        }
    }
}
