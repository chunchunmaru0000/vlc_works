using System;
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

        public static bool IsActive { get => Receiver != null && IP != null; }

        public static void Constructor(int sendPort, int receivePort)
        {
            Receiver = new UdpClient(receivePort);
            IP = new IPEndPoint(IPAddress.Any, receivePort);
            SendPort = sendPort;

            ReceiverThread().Start();
        }

        public static void Send(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes($"{message}\r\n");
            Sender.Send(data, data.Length, "localhost", SendPort);
        }

        private static Thread ReceiverThread() => new Thread(() =>
        {
            while (true) {
                Thread.Sleep(200);
                byte[] reveivedData = Receiver.Receive(ref IP);
                string message = Encoding.UTF8.GetString(reveivedData);

                if (message != null && message.Length > 0)
                    ;
            }
        });

    }
}
