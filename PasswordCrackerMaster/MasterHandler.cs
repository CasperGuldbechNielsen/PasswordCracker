﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCrackerMaster
{
    static class MasterHandler
    {
        private static int lastLen = 0;

        public static string SendWork(int rowsSent, int listNumsToSend, List<string> slaveListToSend, List<string> slaveList, Dictionary<string, byte[]> password)
        {
            for (int i = (rowsSent + 1); i < listNumsToSend; i++)
            {
                slaveListToSend.Add(slaveList[i]);
            }

            var listLenght = slaveListToSend.Count();

            var result = listLenght - lastLen;
            
            Console.WriteLine($"Sending {result} lines to slave");

            lastLen = listLenght;

            List<object> objectList = new List<object>()
            {
                { password },
                { slaveListToSend }
            };

            return JsonConvert.SerializeObject(objectList);
        }
    }
}
