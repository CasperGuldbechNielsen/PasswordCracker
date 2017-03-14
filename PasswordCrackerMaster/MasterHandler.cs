using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCrackerMaster
{
    class MasterHandler
    {
        private string _jsonSendList;

        public string SendWork(int rowsSent, int listNumsToSend, List<string> slaveListToSend, List<string> slaveList, Dictionary<string, byte[]> password)
        {
            for (int i = (rowsSent + 1); i <= listNumsToSend; i++)
            {
                slaveListToSend.Add(slaveList[i]);
            }

            var listLenght = slaveListToSend.Count();

            Console.WriteLine($"Sending {listLenght} lines to slave");

            // TODO: Also send password
            List<object> objectList = new List<object>()
            {
                { password },
                { slaveListToSend }
            };

            return _jsonSendList = JsonConvert.SerializeObject(objectList);
        }
    }
}
