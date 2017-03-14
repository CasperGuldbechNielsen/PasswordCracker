using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordCrackerSlave
{
    class WordVariant
    {
        public static String Capitalize(string word)
        {
            if (word != null)
            {
                if (word.Trim().Length == 0)
                {
                    return word;
                }
                String CapitalizedString = word.Substring(0, 1).ToUpper(); 
                String Remainder = word.Substring(1);
                return CapitalizedString + Remainder;
            }
            else
            {
                throw new ArgumentNullException(nameof(word));
            }
        }

        public static string Reverse(string word)
        {
            if (word != null)
            {
                if (word.Trim().Length == 0)
                {
                    return word;
                }
                StringBuilder reverseString = new StringBuilder();
                for (int i = 0; i < word.Length; i++)
                {
                    reverseString.Append(word.ElementAt(word.Length - 1 - i));
                }
                return reverseString.ToString();
            }
            else
            {
                throw new ArgumentNullException(nameof(word));
            }
        }

        public static string Uppercase(string word)
        {
            if (word != null)
            {
                return word.ToUpper();
            }
            else
            {
                throw new ArgumentNullException(nameof(word));
            }
        }
        public static string Lowercase(string word)
        {
            if (word != null)
            {
                return word.ToLower();
            }
            else
            {
                throw new ArgumentNullException(nameof(word));
            }
        }
    }
}
