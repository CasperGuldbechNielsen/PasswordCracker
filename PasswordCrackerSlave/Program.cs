using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
        //private static Dictionary<string,List<string>> Dict;
        private static JArray obj1;
        private static JObject obj2;

        //this is a real dictionary put in a list
        private static List<object> list;
        private static HashAlgorithm _hashAlgorithm;
       
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
            
            // Read work order from master
            var msg = streamReader.ReadLine();
            if (msg == null)
            {
                // do nothing
            }
            else
            {
                // Deserialize json message
                JArray newMsg = (JArray)JsonConvert.DeserializeObject(msg);
                try
                {
                    var obj = newMsg.ToObject<List<object>>();
                    try
                    {
                        obj1 = (JArray)obj[0];
                        obj2 = (JObject)obj[1];
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            List<string> WordList = obj1.ToObject<List<string>>(); //List of words from the dictionary-file
            Dictionary<string, byte[]> UserList = obj2.ToObject<Dictionary<string, byte[]>>(); //dictionary usernames and passwords(in byteform)

            //Dictionary attack
            foreach (var item in WordList)
            {
                
            }

            #region AttemptNr1

            //string match = "";

            //foreach (var item in Dict)
            //{
            //    foreach (var dictValue in Dict.Values)
            //    {
            //        match = dictValue.ToString();
            //        var hasheditem = _hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(dictValue.ToString()));
            //        if (CompareHashes(Encoding.ASCII.GetBytes(item.Key), hasheditem))
            //            break;
            //        }

            //    if (CompareHashes(Encoding.ASCII.GetBytes(item.Key), _hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(match))))

            //    // success!!!
            //}
            #endregion


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

        public static void ListWork(object item)
            {
            
                
            }

        public static void DictWork()
        {
            
        }
        public static bool CompareHashes(byte[] pwdHash, byte[] compareHash)
            {
            var pwdLenght = pwdHash.Length;
            var compareLenght = compareHash.Length;

            if (pwdLenght != compareLenght)
                return false;

            bool match = false;

            for (int i = 0; i < pwdLenght; i++)
            {
                match = Equals(pwdHash[i], compareHash[i]);
            }

            return match;
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
        // take in the string dictionary item , hash it and return it as a byte
        //public static byte[] RunHash(string wordToHash)
        //{
        //    _hashAlgorithm = new SHA1CryptoServiceProvider();
        //    char[] wordAsChar = wordToHash.ToCharArray();
        //    byte[] wordAsBytes = Array.ConvertAll(wordAsChar, DoConversion());
        //    byte[] encryptedPassword = _hashAlgorithm.ComputeHash(wordAsBytes);

        //    return encryptedPassword;
        //}
        //private static Converter<char, byte> DoConversion()
        //    {
        //    return Converter;
        //    }
        //private static byte CharToByte(char ch)
        //    {
        //    return Convert.ToByte(ch);
        //    }

        }
}
