using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCrackerMaster
{
    class Program
    {
        private static IPAddress _ip = IPAddress.Parse("127.0.0.1");
        private static int _port = 6789;
        private static int _dictDivide;
        private static string _message;
        private static string _stopMessage;
        private static List<string> _slaveList;
        private static List<string> _slaveListToSend;
        private static int _listNum;
        private static string _jsonMessage;
        private static Dictionary<int, Dictionary<string, string>> _mainDict;
        private static Dictionary<string, string> _nestedDict;


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

                _slaveList = new List<string>();
                _slaveListToSend = new List<string>();

                // Read input

                _message = streamReader.ReadLine();

                // Do stuff with the message

                if (!string.IsNullOrEmpty(_message))
                {
                    // Deserialize the message
                    _jsonMessage = Convert.ToString(JsonConvert.DeserializeObject(_message));
                }
                else
                {
                    // do something
                }

                string[] slaveInfo = _jsonMessage.ToString().Split(',');

                string[] slaveName = slaveInfo[0].Split(':');
                string[] slaveIp = slaveInfo[1].Split(':');

                // Get slave IP and such..
                Console.WriteLine($"Slave connected with name {slaveName[1]} nd IP {slaveIp[1]}.");

                Console.WriteLine("Sending work to slave...");

                foreach(var item in Resource.webster_dictionary_reduced)
                {
                    _slaveList.Add(Convert.ToString(item));
                }

                var listLenght = _slaveList.Count();
                listLenght = listLenght / _dictDivide;

                Console.WriteLine($"Sending {listLenght} lines to slave");

                for(int i = 0; i <= listLenght; i++)
                {
                    _slaveListToSend.Add(_slaveList[i]);
                }

                var jsonSendList = JsonConvert.SerializeObject(_slaveListToSend);

                // Send data
                streamWriter.Write(jsonSendList);

                // Add work to dict
                _nestedDict.Add(slaveName[1], "sent");
                _mainDict.Add(_listNum, _nestedDict);

                // Wait for slave to respond.

                var slaveResponse = streamReader.ReadLine();

                if (!string.IsNullOrEmpty(slaveResponse))
                {
                    // Deserialize the message
                    var response = Convert.ToString(JsonConvert.DeserializeObject(slaveResponse));

                    string[] responseArr = _jsonMessage.ToString().Split(',');

                    string[] slaveNameArr = slaveInfo[0].Split(':');
                    string[] slaveResponseArr = slaveInfo[1].Split(':');

                    Console.WriteLine($"Slave: {slaveNameArr[1]} responded: {slaveResponseArr}");

                    _nestedDict[slaveNameArr[1]] = slaveResponseArr[1];
                }
                else
                {
                    // do something
                }

                while (true)
                {
                    // Wait for slave to finish

                    var slaveNewResponse = streamReader.ReadLine();

                    if (!string.IsNullOrEmpty(slaveNewResponse))
                    {
                        // Deserialize the message
                        var response = Convert.ToString(JsonConvert.DeserializeObject(slaveNewResponse));

                        string[] responseArr = _jsonMessage.ToString().Split(',');

                        string[] slaveNameArr = slaveInfo[0].Split(':');
                        string[] slaveResponseArr = slaveInfo[1].Split(':');

                        Console.WriteLine($"Slave: {slaveNameArr[1]} responded: {slaveResponseArr}");

                        _nestedDict[slaveNameArr[1]] = slaveResponseArr[1];

                        if (slaveResponseArr[1] == "Done")
                        {
                            Console.WriteLine($"Slave: {slaveNameArr[1]} is done with his work");
                            break;
                        }
                        else
                        {
                            // Just continue..
                        }
                    }
                    else
                    {
                        // do something
                    }              
                }

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
