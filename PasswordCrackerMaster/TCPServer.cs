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
    class TCPServer
    {
        private int _dictDivide;
        private int _divideNum;
        private int _listNumsToSend;

        private List<string> _slaveList;

        private Socket _serverSocket;
        private Listener _listen;

        public TCPServer()
        {
            _slaveList = new List<string>();

            // Initiate server
            Console.WriteLine("Initiating server...");

            _dictDivide = 500;
            Console.WriteLine($"Dividing dictionary in {_dictDivide} equally sized pieces..");

            foreach (var item in Resource.webster_dictionary_reduced)
            {
                _divideNum += 1;
            }

            _listNumsToSend = _divideNum / _dictDivide;
            MasterHandler.listNumsToSend = _listNumsToSend;

            Console.WriteLine($"\nTotal amount of lines to send to slaves: {_divideNum} \nWith {_dictDivide} divisions \nAmount to {_listNumsToSend} in block size\n");

            string[] dict = Resource.webster_dictionary_reduced.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string item in dict)
            {
                _slaveList.Add(Convert.ToString(item));
            }

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartThreading(IPAddress ip, int port, string threadName, Dictionary<string, byte[]> password)
        {
            Listener _listener = new Listener(ip, port, _slaveList, threadName, password);
            _listener.Listen(_serverSocket);
        }
    }
}
