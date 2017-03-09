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

        private TcpListener _serverSocket;
        private Listener _listen;

        public TCPServer(IPAddress ip, int port)
        {
            _slaveList = new List<string>();
            _slaveListToSend = new List<string>();

            // Initiate server
            Console.WriteLine("Initiating server...");

            Console.Write("How many divisions do you wanna do? ");
            _dictDivide = Convert.ToInt32(Console.ReadLine());

            foreach (var item in Resource.webster_dictionary_reduced)
            {
                _divideNum += 1;
            }

            _listNumsToSend = _divideNum / _dictDivide;

            foreach (var item in Resource.webster_dictionary_reduced)
            {
                _slaveList.Add(Convert.ToString(item));
            }

            _serverSocket = new TcpListener(ip, port);
            _serverSocket.Start();
        }

        public TcpListener ServerSocket
        {
            get { return _serverSocket; }
        }

        public void StartThreading(TcpListener socket, string threadName, Dictionary<string, byte[]> password)
        {
            _listen = new Listener(_rowsSent, _listNumsToSend, _slaveListToSend, _slaveList, threadName, password);
            _listen.Listen(socket);
        }
    }
}
