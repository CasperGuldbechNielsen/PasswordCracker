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
        private static string _machineName = Environment.MachineName;
        private static string _stopMessage;
        private static Socket slaveClient;
        private static bool _closing;
        private static string _msg;
        //private static Dictionary<string,List<string>> Dict;
        private static JObject obj1;
        private static JArray obj2;
        private static readonly Converter<char, byte> Converter = CharToByte;

        //this is a real dictionary put in a list
        private static List<object> list;
        private static HashAlgorithm _hashAlgorithm;

        private static byte[] bits = new byte[10240];
       
        static void Main(string[] args)
        {
           
            // startup
            Console.WriteLine("Slave Node started");

            // aquire current slaves IP Address
            Console.WriteLine("Current Machine Name is: " + _machineName);
            Console.WriteLine("Current IP Address is: " + GetLocalIP());

            // establish TCP connection to master
            slaveClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint end = new IPEndPoint(_masterIp, _masterPort);
            slaveClient.Connect(end);

            // JSON encode SlaveIp & MachineName

            StringBuilder build = new StringBuilder();

            build.Append("Name:");
            build.Append(_machineName);
            build.Append(",");
            build.Append("Ip:");
            build.Append(GetLocalIP());
            // "Name:_machineName,Ip:_slaveIp"

            // serialize
            var json = JsonConvert.SerializeObject(build);

            // Phone home with MachineName & SlaveIp
            byte[] send = Encoding.ASCII.GetBytes(json);
            slaveClient.Send(send);

            while (true)
            {
                // Read work order from master
                var msg = slaveClient.Receive(bits);
                _msg = Encoding.ASCII.GetString(bits, 0, msg);

                if (_msg == null)
                {
                    // do nothing
                }
                else
                {
                    // Deserialize json message
                    JArray newMsg = (JArray)JsonConvert.DeserializeObject(_msg);
                    try
                    {
                        var obj = newMsg.ToObject<List<object>>();
                        try
                        {
                            obj1 = (JObject)obj[0];
                            obj2 = (JArray)obj[1];
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

                List<string> wordList = obj2.ToObject<List<string>>(); //List of words from the dictionary-file
                Dictionary<string, byte[]> userList = obj1.ToObject<Dictionary<string, byte[]>>(); //dictionary usernames and passwords(in byteform)

                //Dictionary attack
                Dictionary<string, string> resultsDictionary = new Dictionary<string, string>();

                List<byte[]> possibleVariant = new List<byte[]>();
                Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();

                foreach (var word in wordList)
                {
                    string oWord = word;

                    //Variants & Mutations
                    possibleVariant.Add(RunHash(oWord));
                    possibleVariant.Add(RunHash(WordVariant.Capitalize(oWord)));
                    possibleVariant.Add(RunHash(WordVariant.Lowercase(oWord)));
                    possibleVariant.Add(RunHash(WordVariant.Reverse(oWord)));
                    possibleVariant.Add(RunHash(WordVariant.Uppercase(oWord)));
                    for (int i = 0; i < 100; i++)
                    {
                        possibleVariant.Add(RunHash(oWord + i));
                    }
                    for (int i = 0; i < 100; i++)
                    {
                        possibleVariant.Add(RunHash(i + oWord));
                    }
                    for (int i = 0; i < 100; i++)
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            possibleVariant.Add(RunHash(i + word + j));
                        }
                    }
                    //TODO: Add to ResultDictionary
                }
                Console.WriteLine("Testing hashes against passwords...");
                foreach (var password in userList.Values)
                {
                    foreach (var item in possibleVariant)
                    {
                        bool test = CompareHashes(password, item);

                        if (test)
                        {
                            foreach (var match in wordList)
                            {
                                if (Equals(match, Convert.ToBase64String(password)))
                                {
                                    result.Add(Convert.ToBase64String(password), item);
                                    Console.WriteLine("Found match!");
                                }
                            }
                        }
                    }
                }
                if(result.Count == 0)
                    Console.WriteLine("No match found.");
                var jsonsend = JsonConvert.SerializeObject(result);

                // Phone home with MachineName & SlaveIp
                byte[] sending = Encoding.ASCII.GetBytes(jsonsend);

                slaveClient.Send(sending);
            }
             
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
        public static byte[] RunHash(string wordToHash)
        {
            _hashAlgorithm = new SHA1CryptoServiceProvider();
            char[] wordAsChar = wordToHash.ToCharArray();
            byte[] wordAsBytes = Array.ConvertAll(wordAsChar, DoConversion());
            byte[] encryptedWord = _hashAlgorithm.ComputeHash(wordAsBytes);

            return encryptedWord;
        }
        public static Converter<char, byte> DoConversion()
        {
            return Converter;
        }
        private static byte CharToByte(char ch)
        {
            return Convert.ToByte(ch);
        }

    }
}
