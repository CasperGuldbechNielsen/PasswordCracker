using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    class Listener
    {
        private int _port;
        private string _message;
        private string _jsonMessage;
        private string _jsonSendList;
        private string _threadName;
        private string _slaveName;
        private string[] slaveInfo;
        byte[] bits = new byte[10240];
        private bool goOn = false;
        private Dictionary<string, byte[]> _password;
        private List<string> _slaveList;
        private IPAddress _ip;

        public Listener(IPAddress ip, int port, List<string> slaveList, string threadName, Dictionary<string, byte[]> password)
        {
            _ip = ip;
            _port = port;
            _slaveList = slaveList;
            _threadName = threadName;
            _password = password;
        }

        public void Listen(Socket socket)
        {
            Socket serverClient = socket;

            AwaitSlaveConn(serverClient);
        }

        public void HandleClients(object obj)
        {
            Socket socket = (Socket)obj;

            while (true)
            {
                SlaveContact(socket);

                Work(socket);
            }
        }

        public void Work(Socket socket)
        {
            SendWork(socket);

            while (true)
            {
                try
                {
                    goOn = EvaluateSlaveResponse(socket);
                    if (goOn)
                        Stop();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void SendWork(Socket socket)
        {
            _jsonSendList = MasterHandler.SendWork(_slaveList, _password, _slaveName);

            // Send data
            byte[] send = Encoding.ASCII.GetBytes(_jsonSendList);
            socket.Send(send);
        }

        public void SlaveContact(Socket socket)
        {
            // Read input
            var msg = socket.Receive(bits);
            _message = Encoding.ASCII.GetString(bits, 0, msg);
        
            // Do stuff with the message
            if (!string.IsNullOrEmpty(_message))
            {
                // Deserialize the message
                _jsonMessage = Convert.ToString(JsonConvert.DeserializeObject(_message));

                slaveInfo = _jsonMessage.ToString().Split(',');
                string[] slaveMaxCap = slaveInfo[0].Split(':');
                string[] slaveCap = slaveInfo[1].Split(':');
                string[] slaveName = slaveInfo[2].Split(':');
                _slaveName = slaveName[2];
                string[] slaveIp = slaveInfo[3].Split(':');

                string[] resultIP = slaveIp[1].Split('.');
                string[] finalIP = resultIP[3].Split('\"');

                string IP = resultIP[0] + "." + resultIP[1] + "." + resultIP[2] + "." + finalIP[0];

                // Get slave IP and such..
                Console.WriteLine($"Slave connected with information: \nName: {slaveName[2]} \nIP: {IP} \nMax_Capacity: {slaveMaxCap[1]} \nCapacity: {slaveCap[1]}\n");
            }
            else
            {
                // do something
                Console.WriteLine("Something went wrong. Closing thread.");
                Stop();
            }
        }

        public void AwaitSlaveConn(Socket serverClient)
        {
            Console.WriteLine($"{_threadName}: Waiting for a slave to connect");

            IPEndPoint localEP = new IPEndPoint(_ip, _port);

            serverClient.Bind(localEP);
            serverClient.Listen(10);

            while (true)
            {
                Socket socket = serverClient.Accept();

                if (socket.Connected)
                {
                    Console.WriteLine("Client connected");
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClients));
                    t.Start(socket);
                }
            }
        }

        public bool EvaluateSlaveResponse(Socket socket)
        {
            // Wait for slave to finish
            var msg = socket.Receive(bits);
            var slaveNewResponse = Encoding.ASCII.GetString(bits, 0, msg);

            if (!string.IsNullOrEmpty(slaveNewResponse))
            {
                // Deserialize the message
                var response = Convert.ToString(JsonConvert.DeserializeObject(slaveNewResponse));

                JObject newMsg = (JObject)JsonConvert.DeserializeObject(response);

                if (newMsg.Count == 0)
                {
                    Console.WriteLine($"<= Slave {_slaveName} returned: No Match..\n");
                    Work(socket);
                }
               
                else
                {
                    foreach (var item in newMsg)
                    {
                        Console.WriteLine($"<= Slave {_slaveName} returned: {item.Key} + {item.Value}");
                    }
                }
            }
            return true;
        }

        public void Stop()
        {
            Console.ReadKey();

            Environment.Exit(0);
        }
    }
}
