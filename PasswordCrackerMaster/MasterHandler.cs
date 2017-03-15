using Newtonsoft.Json;
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
        private static List<string> _slaveListToSend = new List<string>();
        private static int _loopVar = -1;
        private static int _listNumsToSend;
        private static int _rowsSent = 0;

        public static int listNumsToSend
        {
            get { return _listNumsToSend; }
            set { _listNumsToSend = value; }
        }

        public static string SendWork(List<string> slaveList, Dictionary<string, byte[]> password, string name)
        {
            _slaveListToSend.Clear();

            for (int i = (_loopVar + 1); i < _listNumsToSend; i++)
            {
                _slaveListToSend.Add(slaveList[i]);
            }

            var listLenght = _slaveListToSend.Count();
            
            Console.WriteLine($"=> Sending {listLenght} lines to slave {name}. Already sent {_rowsSent} to slaves.");

            _rowsSent += listLenght;

            List<object> objectList = new List<object>()
            {
                { password },
                { _slaveListToSend }
            };

            return JsonConvert.SerializeObject(objectList);
        }
    }
}
