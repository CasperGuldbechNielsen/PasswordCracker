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
        private string jsonSendList;

        public string SendWork(int rowsSent, int listNumsToSend, List<string> slaveListToSend, List<string> slaveList)
        {
            for (int i = (rowsSent + 1); rowsSent < listNumsToSend; i++)
            {
                slaveListToSend.Add(slaveList[i]);
            }

            var listLenght = slaveListToSend.Count();

            Console.WriteLine($"Sending {listLenght} lines to slave");

            return jsonSendList = JsonConvert.SerializeObject(slaveListToSend);
        }
    }
}
