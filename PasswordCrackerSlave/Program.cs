using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCrackerSlave
{
    class Program
    {
        private static IPAddress _ip = IPAddress.Parse("127.0.0.1");
        private static int _port = 6789;
        private static int _dictDivide;
        private static string _message;
        private static string _stopMessage;

        static void Main(string[] args)
        {
            // Initiate server
            Console.WriteLine("Initiating server...");

            Console.Write("How many divisions do you wanna do? ");
            _dictDivide = Convert.ToInt32(Console.ReadLine());

            TcpListener serverSocket = new TcpListener(_ip, _port);
            serverSocket.Start();

            while (true)
            {
                Console.WriteLine("Waiting for a slave to connect");

                TcpClient serverClient = serverSocket.AcceptTcpClient();

                // Setup streams

                NetworkStream stream = serverClient.GetStream();
                StreamReader streamReader = new StreamReader(stream);
                StreamWriter streamWriter = new StreamWriter(stream) { AutoFlush = true };

                // Read input

                _message = streamReader.ReadLine();

                // Deserialize the message

                var jsonMessage = JsonConvert.DeserializeObject(_message);

                // Do stuff with the message

                if (!string.IsNullOrEmpty(_message))
                {
                    // Get slave IP and such..
                    Console.WriteLine("Client wrote: " + _message);
                }

                Console.WriteLine("Sending work to slave...");

                //
                // Send work
                //

                // Stopping the server

                Console.WriteLine("Type 'EXIT', 'Exit' or 'exit' to stop the server");

                _stopMessage = Console.ReadLine();

                if (_stopMessage == "EXIT" || _stopMessage == "Exit" || _stopMessage == "exit")
                {
                    // Send exit to slaves...
                    Environment.Exit(0);
                }
            }
        }
    }
}
