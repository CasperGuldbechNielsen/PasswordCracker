using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordCrackerMaster
{
    class Program
    {
        private static int _port = 6789;
        private static int threadNum = 1;
        private static IPAddress _ip = IPAddress.Parse("127.0.0.1");
        private static Dictionary<string, string> _passwords = new Dictionary<string, string>
        {
            { "anders", "5en6G6MezRroT3XKqkdPOmY / BfQ =" },
            { "peter", "qmz4syDsnnyBP + NQeqczRv / kJP4 =" },
            { "michael", "rIFGj9xqLUA0T0J8xiGCuMlfnvM =" },
            { "vibeke", "EQ91A67FOpjss4uW8kV570lnSa0 =" },
            { "lars", "7qrnVFEkYcp7tQyKQfnwOP1X4SI =" },
            { "poul", "94roVc1d8UZEtbK9LBF3vuo0wkg =" },
            { "susanne", "00kp1 + AcZSNJemn40Yew + bOI5XQ =" },
            { "per", "AXPaVO / 3DmqNsW2uPJw9ZJxf9lc =" }
        };
        private static Dictionary<string, byte[]> _passwordsDecoded = new Dictionary<string, byte[]>();

        private static TCPServer server;

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("+--------------------------------------------------------------------------+");
            Console.WriteLine("| \\                                                                      / |");
            Console.WriteLine("|  ->                ~ Initiating PasswordCracker 1.0 ~                <-  |");
            Console.WriteLine("| /                                                                      \\ |");
            Console.WriteLine("+--------------------------------------------------------------------------+");
            Console.ResetColor();

            foreach (var item in _passwords)
            {
                byte[] pass = Convert.FromBase64String(item.Value);
                _passwordsDecoded.Add(item.Key, pass);
            }


            server = new TCPServer(_ip, _port);

            var socket = server.ServerSocket;

            for (int i = 0; i < threadNum; i++)
            {
                Thread thread = new Thread(() => server.StartThreading(socket, $"Thread {i}", _passwordsDecoded));
                thread.Start();
            }
        }
    }
}
