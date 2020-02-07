using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyCapstone.Lib
{
    public static class Cryptoanalyzer
    {
        public static string ApplyShiftCipher(string plainText, int key)
        {
            string cipherText = "";

            foreach (var ch in plainText)
            {
                cipherText += (Common.CharToAlphabetIndex(ch) + key) % 26;
            }

            return cipherText;
        }

        public static string ApplySubstitutionCipher(string plainText, Dictionary<string, string> key)
        {
            string cipherText = "";

            foreach (var ch in plainText)
            {
                if (key.ContainsKey(ch.ToString()))
                    cipherText += key[ch.ToString()];
                else
                    cipherText += ch;
            }

            return cipherText;
        }

        public static List<int> CreateShiftCipherGuess(string cipherText)
        {
            Dictionary<int, double> ICList = new Dictionary<int, double>();
            for (int i = 1; i <= 26; i++)
            {
                ICList.Add(i, Common.IndexOfCoincidence(
                    ApplyShiftCipher(cipherText, -i)
                ));
            }

            var temp = ICList.OrderByDescending((d => 1 - d.Value));
            var guessedKeys = new List<int>();
            foreach (var ic in ICList.OrderByDescending((d => 1 - d.Value)))
            {
                guessedKeys.Add(ic.Key);
            }
            return guessedKeys;
        }
    }
}
