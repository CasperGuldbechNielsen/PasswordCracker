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
        private int _rowsSent = -1;

        private List<string> _slaveList;
        private List<string> _slaveListToSend;

        private Socket _serverSocket;
        private Listener _listen;

        public TCPServer()
        {
            _slaveList = new List<string>();
            _slaveListToSend = new List<string>();

            // Initiate server
            Console.WriteLine("Initiating server...");

            Console.WriteLine("Dividing dictionary in 100 equally sized pieces..");
            _dictDivide = 100;

            foreach (var item in Resource.webster_dictionary_reduced)
            {
                _divideNum += 1;
            }

            _listNumsToSend = _divideNum / _dictDivide;

            foreach (var item in Resource.webster_dictionary_reduced)
            {
                _slaveList.Add(Convert.ToString(item));
            }

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Socket ServerSocket
        {
            get { return _serverSocket; }
        }

        public void StartThreading(IPAddress ip, int port, Socket socket, string threadName, Dictionary<string, byte[]> password)
        {
            _listen = new Listener(ip, port, _rowsSent, _listNumsToSend, _slaveListToSend, _slaveList, threadName, password);
            _listen.Listen(socket);
        }
    }
}
