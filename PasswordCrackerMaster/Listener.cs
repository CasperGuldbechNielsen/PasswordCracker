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
        private string[] _slaveNameArr;
        private string[] _responseArr;
        private string[] slaveInfo;
        private bool goOn = false;
        private Dictionary<string, byte[]> _password;
        private List<string> _slaveListToSend;
        private List<string> _slaveList;

        NetworkStream stream;
        StreamReader streamReader;
        StreamWriter streamWriter;

        private MasterHandler _handler;

        public Listener(int rowsSent, int listNumsToSend, List<string> slaveListToSend, List<string> slaveList, string threadName, Dictionary<string, byte[]> password)
        {
            _rowsSent = rowsSent;
            _listNumsToSend = listNumsToSend;
            _slaveListToSend = slaveListToSend;
            _slaveList = slaveList;
            _threadName = threadName;
            _password = password;

            _handler = new MasterHandler();
        }

        public void Listen(TcpListener socket)
        {
            AwaitSlaveConn(socket);

            while (true)
            {
                SlaveContact();

                Work("first task");
            }
        }

        public void Work(string workType)
        {
            SendWork(workType);

            while (goOn)
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

            _jsonSendList = _handler.SendWork(_rowsSent, _listNumsToSend, _slaveListToSend, _slaveList, _password);

            // Send data
            streamWriter.Write(_jsonSendList);

            // Wait for slave to respond
            var slaveResponse = streamReader.ReadLine();

            if (!string.IsNullOrEmpty(slaveResponse))
            {
                // Deserialize the message
                var response = Convert.ToString(JsonConvert.DeserializeObject(slaveResponse));

                _responseArr = _jsonMessage.ToString().Split(',');
                _slaveNameArr = slaveInfo[0].Split(':');
                _responseArr = slaveInfo[1].Split(':');

                Console.WriteLine($"Slave: {_slaveNameArr[1]} responded: {_responseArr}");
            }
            else
            {
                // do something
                Console.WriteLine("Something went wrong. Closing thread.");
                Stop();
            }
        }

        public void SlaveContact()
        {
            using(StreamReader streamReader = new StreamReader(stream))
            {
                while(!streamReader.EndOfStream)
                {
                    if (stream.DataAvailable)
                    {
                        // Read input
                        _message = streamReader.ReadLine();
                    }
                }
            }

            // Do stuff with the message
            if (!string.IsNullOrEmpty(_message))
            {
                // Deserialize the message
                _jsonMessage = Convert.ToString(JsonConvert.DeserializeObject(_message));

                slaveInfo = _jsonMessage.ToString().Split(',');
                string[] slaveName = slaveInfo[0].Split(':');
                string[] slaveIp = slaveInfo[1].Split(':');

                // Get slave IP and such..
                Console.WriteLine($"Slave connected with name {slaveName[1]} nd IP {slaveIp[1]}.");
            }
            else
            {
                // do something
                Console.WriteLine("Something went wrong. Closing thread.");
                Stop();
            }
        }

        public void AwaitSlaveConn(TcpListener socket)
        {
            Console.WriteLine($"{_threadName}: Waiting for a slave to connect");

            TcpClient serverClient = socket.AcceptTcpClient();

            // Setup streams

            stream = serverClient.GetStream();
            streamReader = new StreamReader(stream);
            streamWriter = new StreamWriter(stream) { AutoFlush = true };
        }

        public bool EvaluateSlaveResponse()
        {
            // Wait for slave to finish
            var slaveNewResponse = streamReader.ReadLine();

            if (!string.IsNullOrEmpty(slaveNewResponse))
            {
                // Deserialize the message
                var response = Convert.ToString(JsonConvert.DeserializeObject(slaveNewResponse));

                string[] responseArr = _jsonMessage.ToString().Split(',');

                string[] slaveNameArr = responseArr[0].Split(':');
                string[] slaveResponseArr = responseArr[1].Split(':');

                Console.WriteLine($"Slave: {slaveNameArr[1]} responded: {slaveResponseArr}");

                if (slaveResponseArr[1] == "Done")
                {
                    Console.WriteLine($"Slave: {slaveNameArr[1]} is done with his work");

                    /*
                     * 
                     * We need to evaluate slave response here. Get the result back and so on. 
                     * Digest response and handle a failed crack attempt with a new send work.
                     * 
                     * 
                     */

                    if (false /* Some dictionary recieved <key (integer), value (string:plain_text_pass)*/)
                    {
                        if (true /* Some dictionary.key == 1 */)
                        {
                            Console.WriteLine($"Slave found the password /* password from dictionary */");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Slave didn't crack the password");
                            Work("new task");
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        public void Stop()
        {
            Console.ReadKey();

            Environment.Exit(0);
        }
    }
}
