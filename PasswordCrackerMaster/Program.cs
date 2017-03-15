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
        private static int[] _port = { 6789, 6790, 6791, 6792, 6793, 6794, 6795, 6796, 6797, 6798, 6799 };
        private static int threadNum = 2;
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

            server = new TCPServer();

            Console.WriteLine("Starting listener on port: 6789");
            Thread thread = new Thread(() => server.StartThreading(_ip, 6789, "Thread 1", _passwordsDecoded));
            thread.Start();
        }
    }
}
