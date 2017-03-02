using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCrackerSlave
{
    class Program
    {
        private static readonly IPAddress _masterIp = IPAddress.Parse("127.0.0.1");
        private static readonly int _masterPort = 6789;
        private static IPAddress _slaveIp;
        private static string _machineName = Environment.MachineName;
        private static string _stopMessage;

        static void Main(string[] args)
        {
            // startup
            Console.WriteLine("Slave Node started");

            // aquire current slaves IP Address
            Console.WriteLine("Current Machine Name is: " + _machineName);
            Console.WriteLine("Current IP Address is: " + GetLocalIP());

            // establish TCP connection to master
            TcpClient slaveClient = new TcpClient(_masterIp.ToString(), _masterPort);

            NetworkStream stream = slaveClient.GetStream();
            StreamReader streamReader = new StreamReader(stream);
            StreamWriter streamWriter = new StreamWriter(stream) { AutoFlush = true };

            // JSON encode SlaveIp & MachineName
            var jsonIp = JsonConvert.SerializeObject(_slaveIp);
            var jsonMn = JsonConvert.SerializeObject(_machineName);

            // Phone home with MachineName & SlaveIp
            while (true)
            {
                streamWriter.WriteLine(jsonIp); //send Slave IP
                streamWriter.WriteLine(jsonMn); //send MachineName
            }

            #region Stop functionality
            // Temporary stop functionality on slave end
            Console.WriteLine("Type 'EXIT', 'Exit' or 'exit' to stop the server");

            _stopMessage = Console.ReadLine();

            if (_stopMessage == "EXIT" || _stopMessage == "Exit" || _stopMessage == "exit")
            {
                // Send exit to slaves...
                Environment.Exit(0);
            } 
            #endregion
        }

        public static string GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
                throw new Exception("Local IP-Address was not found");
            }
            
        }
    }
}
