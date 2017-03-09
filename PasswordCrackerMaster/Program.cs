using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordCrackerMaster
{
    class Program
    {
        private static int _port = 6789;
        private static IPAddress _ip = IPAddress.Parse("127.0.0.1");

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

            Console.WriteLine("\n How many threads are needed? ");
            var threadNum = Convert.ToInt32(Console.ReadLine());

            server = new TCPServer(_ip, _port);

            var socket = server.ServerSocket;

            for (int i = 0; i < threadNum; i++)
            {                
                Thread thread = new Thread(()=> server.StartThreading(socket, $"Thread {i}"));
                thread.Start();
            }
        }
    }
}
