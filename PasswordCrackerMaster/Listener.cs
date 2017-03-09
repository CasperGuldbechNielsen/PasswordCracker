using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCrackerMaster
{
    class Listener
    {
        private int _rowsSent;
        private int _listNumsToSend;
        private string _message;
        private string _jsonMessage;
        private string _jsonSendList;
        private string _threadName;
        private List<string> _slaveListToSend;
        private List<string> _slaveList;

        NetworkStream stream;
        StreamReader streamReader;
        StreamWriter streamWriter;

        private MasterHandler _handler;

        public Listener(int rowsSent, int listNumsToSend, List<string> slaveListToSend, List<string> slaveList, string threadName)
        {
            _rowsSent = rowsSent;
            _listNumsToSend = listNumsToSend;
            _slaveListToSend = slaveListToSend;
            _slaveList = slaveList;
            _threadName = threadName;

            _handler = new MasterHandler();
        }

        public void Listen(TcpListener socket)
        {
            while (true)
            {
                Console.WriteLine($"{_threadName}: Waiting for a slave to connect");

                TcpClient serverClient = socket.AcceptTcpClient();

                // Setup streams

                stream = serverClient.GetStream();
                streamReader = new StreamReader(stream);
                streamWriter = new StreamWriter(stream) { AutoFlush = true };

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

                _jsonSendList = _handler.SendWork(_rowsSent, _listNumsToSend, _slaveListToSend, _slaveList);

                // Send data
                streamWriter.Write(_jsonSendList);

                // Wait for slave to respond
                var slaveResponse = streamReader.ReadLine();

                if (!string.IsNullOrEmpty(slaveResponse))
                {
                    // Deserialize the message
                    var response = Convert.ToString(JsonConvert.DeserializeObject(slaveResponse));

                    string[] responseArr = _jsonMessage.ToString().Split(',');

                    string[] slaveNameArr = slaveInfo[0].Split(':');
                    string[] slaveResponseArr = slaveInfo[1].Split(':');

                    Console.WriteLine($"Slave: {slaveNameArr[1]} responded: {slaveResponseArr}");
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

                        if (slaveResponseArr[1] == "Done")
                        {
                            Console.WriteLine($"Slave: {slaveNameArr[1]} is done with his work");

                            if (false /* Some dictionary recieved <key (integer), value (string:plain_text_pass)*/)
                            {
                                if (true /* Some dictionary.key == 1 */)
                                {
                                    Console.WriteLine($"Slave found the password /* password from dictionary */");
                                    Stop();
                                }
                                else
                                {
                                    Console.WriteLine("Slave didn't crack the password");
                                }
                            }
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
            }
        }

        public void Stop()
        {
            Console.ReadKey();

            Environment.Exit(0);
        }
    }
}
