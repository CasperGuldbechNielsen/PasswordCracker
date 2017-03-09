using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
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
        private static TcpClient slaveClient;
        private static bool _closing;
        private static List<string> recievedData;

        static void Main(string[] args)
        {
            // startup
            Console.WriteLine("Slave Node started");

            // aquire current slaves IP Address
            Console.WriteLine("Current Machine Name is: " + _machineName);
            Console.WriteLine("Current IP Address is: " + GetLocalIP());

            // establish TCP connection to master

            try
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        slaveClient = new TcpClient(_masterIp.ToString(), _masterPort);
                    }
                    catch (Exception)
                    {
                        _closing = true;
                        Console.WriteLine("No connection established");
                        Thread.Sleep(10000);
                    }

                }
            }
            catch (Exception)
            {
                Console.WriteLine("fubar");

            }
            finally
            {
                if (_closing)
                {
                    Console.WriteLine("Closing down in 5 seconds");
                    Thread.Sleep(5000);
                    Environment.Exit(0);
                }
                else
                {
                    //´do nothing
                }
                
            }

            NetworkStream stream = slaveClient.GetStream();
            StreamReader streamReader = new StreamReader(stream);
            StreamWriter streamWriter = new StreamWriter(stream) { AutoFlush = true };

            // JSON encode SlaveIp & MachineName

            StringBuilder build = new StringBuilder();

            build.Append("Name:");
            build.Append(_machineName);
            build.Append(",");
            build.Append("Ip:");
            build.Append(_slaveIp);
            // "Name:_machineName,Ip:_slaveIp"

            // serialize
            var json = JsonConvert.SerializeObject(build);

            // Phone home with MachineName & SlaveIp
            while (true)
            {
                streamWriter.WriteLine(json); //send MachineName
                break;
            }
            
            //Recieve Workload from master
            while (true)
            {
                //
                List<string> deserializedData = JsonConvert.DeserializeObject<List<string>>(streamReader.ReadLine());
                recievedData.AddRange(deserializedData); 
                break;
            }
            
            //TODO: Start Dictionary Attack on workload


            //TODO: Send result back to Master in a dictionary <1,"passPlain">/<0,"">

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
                
            }
            return "no Ip found";
        }
    }
}
