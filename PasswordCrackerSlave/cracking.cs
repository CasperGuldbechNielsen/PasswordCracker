using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCrackerSlave
{
    class Cracking
    {
        private readonly HashAlgorithm _msgDigest;

        public Cracking()
        {
            _msgDigest = new SHA1CryptoServiceProvider();
        }

        public void RunCracking()
        {
            
        }
    }
}
