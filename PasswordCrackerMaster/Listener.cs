using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    class Listener
    {
        private int _rowsSent;
        private int _listNumsToSend;
        private int _port;
        private string _message;
        private string _jsonMessage;
        private string _jsonSendList;
        private string _threadName;
        private string[] _slaveNameArr;
        private string[] _responseArr;
        private string[] slaveInfo;
        byte[] bits = new byte[10240];
        private bool goOn = false;
        private Dictionary<string, byte[]> _password;
        private List<string> _slaveListToSend;
        private List<string> _slaveList;
        private IPAddress _ip;
        JObject obj1;

        Socket serverClient;
        Socket socket;

        public Listener(IPAddress ip, int port, int rowsSent, int listNumsToSend, List<string> slaveListToSend, List<string> slaveList, string threadName, Dictionary<string, byte[]> password)
        {
            _ip = ip;
            _port = port;
            _rowsSent = rowsSent;
            _listNumsToSend = listNumsToSend;
            _slaveListToSend = slaveListToSend;
            _slaveList = slaveList;
            _threadName = threadName;
            _password = password;
        }

        public void Listen(Socket socket)
        {
            serverClient = socket;

            AwaitSlaveConn();

            while (true)
            {
                SlaveContact();

                Work("first task");
            }
        }

        public void Work(string workType)
        {
            SendWork(workType);

            while (true)
            {
                try
                {
                    goOn = EvaluateSlaveResponse();
                    if (goOn)
                        Stop();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void SendWork(string workType)
        {
            Console.WriteLine($"Sending {workType} to slave...");

            _jsonSendList = MasterHandler.SendWork(_rowsSent, _listNumsToSend, _slaveListToSend, _slaveList, _password);

            // Send data
            byte[] send = Encoding.ASCII.GetBytes(_jsonSendList);
            socket.Send(send);
        }

        public void SlaveContact()
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

        public void AwaitSlaveConn()
        {
            Console.WriteLine($"{_threadName}: Waiting for a slave to connect");

            IPEndPoint localEP = new IPEndPoint(_ip, _port);

            serverClient.Bind(localEP);
            serverClient.Listen(10);


            socket = serverClient.Accept();
            if (socket.Connected)
                Console.WriteLine("Client connected");

            // Setup streams
            //stream = new NetworkStream(socket);
            //streamReader = new StreamReader(stream);
            //streamWriter = new StreamWriter(stream) { AutoFlush = true };
        }

        public bool EvaluateSlaveResponse()
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
                    Work("new task");
                }
               
                else
                {
                    foreach (var item in obj1)
                    {
                        Console.WriteLine($"Slave returned: {item.Key} + {item.Value}");
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
