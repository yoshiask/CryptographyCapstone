using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extreme.Mathematics.Curves;

namespace CryptographyCapstone.Lib
{
    public static class Cryptoanalyzer
    {
        #region Shift Cipher
        public static string EncryptShiftCipher(string plainText, int key)
        {
            string cipherText = "";

            foreach (var ch in plainText)
            {
                cipherText += EncryptShiftCipher(ch, key);
            }

            return cipherText;
        }
        public static char EncryptShiftCipher(char plainText, int key)
        {
            return char.IsLetter(plainText) ? Common.AlphabetIndexToChar(Common.CharToAlphabetIndex(plainText) + key) : plainText;
        }

        public static string DecryptShiftCipher(string cipherText, int key)
        {
            return EncryptShiftCipher(cipherText, -key);
        }

        public static List<int> GuessShiftCipher(string cipherText)
        {
            Dictionary<int, double> ICList = new Dictionary<int, double>();
            for (int i = 1; i <= 26; i++)
            {
                ICList.Add(i, Common.IndexOfCoincidence(
                    DecryptShiftCipher(cipherText, i)
                ));
            }

            var guessedKeys = new List<int>();
            foreach (var ic in ICList.OrderByDescending((d => 1 - d.Value)))
            {
                guessedKeys.Add(ic.Key);
            }
            return guessedKeys;
        }
        #endregion

        public static string EncryptSubstitutionCipher(string plainText, Dictionary<string, string> key)
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

        public static string DecryptSubstitutionCipher(string cipherText, Dictionary<string, string> key)
        {
            return EncryptSubstitutionCipher(cipherText, Common.SwapColumns(key));
        }

        public static string EncryptPolyalphabeticSubstitutionCipher(string plainText, Dictionary<string, List<string>> key)
        {
            string cipherText = "";

            foreach (var ch in plainText)
            {
                if (key.ContainsKey(ch.ToString()))
                {
                    List<string> candidates = key[ch.ToString()];
                    cipherText += candidates[new Random().Next(0, candidates.Count - 1)];
                }
                else
                    cipherText += ch;
            }

            return cipherText;
        }

        public static string EncryptVigenereCipher(string plainText, int key)
        {
            string cipherText = "";
            foreach (char ch in plainText)
            {
                cipherText += EncryptShiftCipher(ch, key);
                key++;
            }
            return cipherText;
        }

        #region One-Time Pad
        public static string EncryptOneTimePad(string plainText, string key)
        {
            string expandedKey = Common.RepeatString(key, plainText.Length);
            string cipherText = "";
            for (int i = 0; i < plainText.Length; i++)
            {
                char ch = plainText[i];
                if (char.IsLetter(ch))
                {
                    cipherText += EncryptShiftCipher(ch, Common.CharToAlphabetIndex(expandedKey[i]));
                }
                else
                {
                    cipherText += ch;
                }
            }
            return cipherText;
        }

        public static string DecryptOneTimePad(string cipherText, string key)
        {
            string expandedKey = Common.RepeatString(key, cipherText.Length);
            string plainText = "";
            for (int i = 0; i < cipherText.Length; i++)
            {
                char ch = cipherText[i];
                if (char.IsLetter(ch))
                {
                    plainText += EncryptShiftCipher(ch, -Common.CharToAlphabetIndex(expandedKey[i]));
                }
                else
                {
                    plainText += ch;
                }
            }
            return plainText;
        }
        #endregion

        #region Affine Cipher
        public static string EncryptAffine(string plainText, int a, int b)
        {
            string cipherText = "";
            
            // Compute e(x) = (ax + b)(mod m) for every character in the Plain Text
            foreach (char c in plainText)
            {
                int x = Common.CharToAlphabetIndex(c);
                cipherText += Common.AlphabetIndexToChar(a * x + b);
            }
 
            return cipherText;
        }
        
        public static string DecryptAffine(string cipherText, int a, int b)
        {
            string plainText = "";
 
            // Get Multiplicative Inverse of a
            int aInverse = Common.MultiplicativeInverse(a, 26);
 
            // Put Cipher Text (all capitals) into Character Array
            char[] chars = cipherText.ToUpper().ToCharArray();
 
            // Computer d(x) = aInverse * (e(x)  b)(mod m)
            foreach (char c in chars)
            {
                int x = Convert.ToInt32(c - 65);
                if (x - b < 0) x = Convert.ToInt32(x) + 26;
                plainText += Convert.ToChar(((aInverse * (x - b)) % 26) + 65);
            }
 
            return plainText;
        }
        #endregion
    }

    public class PolynomialCipher
    {
        public static string CoeffsToPolynomial(List<double> coeffs)
        {
            string output = "";

            for (int i = coeffs.Count - 1; i >= 0; i--)
            {
                if (coeffs[i] != 1.0)
                    output += Math.Abs(coeffs[i]);

                if (i != 0)
                {
                    output += "x" + Lib.Common.IntToPower(i);
                    if (coeffs[i-1] >= 0)
                        output += " + ";
                    else
                        output += " - ";
                }
            }

            return output;
        }

        public static List<string> EncodeMessage(string message, int key)
        {
            var output = new List<string>();
            // Split the message into words
            var words = message.Split(
                new char[] { ' ', ',', '.', '!', '?' },
                StringSplitOptions.RemoveEmptyEntries);

            // Encode each word
            foreach (string word in words)
            {
                output.Add(CoeffsToPolynomial(EncryptString(word, key)));
            }

            return output;
        }

        /// <summary>
        /// Returns a list of coefficients where the index is the power
        /// </summary>
        public static List<double> EncryptString(string message, int key)
        {
            var encoded = new List<double>();
            var plaintext = new List<int>();

            // Convert the message to non-zero indexed list
            foreach (char ch in message)
            {
                int c = Lib.Common.CharToAlphabetIndex(ch) + 1;
                if (c >= 0 && c < 65)
                    plaintext.Add(c);
            }

            if (plaintext.Count < 1)
                return encoded;

            // Convert the plaintext numbers to a usable list of roots,
            // where the multiplicity of the root is the position of the character,
            // and the value of *a* where x-a=0 is the character itself.
            var roots = new List<double>();
            int[] lastCoprime = new int[26];
            for (int i = 0; i < plaintext.Count; i++)
            {
                int currentChar = plaintext[i];
                // First, determine which appearance of the character this is
                int coprime = Lib.Common.FindCoprime(currentChar + key, lastCoprime[currentChar]);
                double root = (double)(currentChar + key) / coprime;
                lastCoprime[currentChar] = coprime;

                // Finally, make it a multiplicity
                for (int k = 0; k < i + 1; k++)
                {
                    roots.Add(root);
                }
            }

            // Create the polynomial from roots
            var polynomial = Polynomial.FromRoots(roots.ToArray());
            try
            {
                // Pull the coefficients from the polynomial
                int i = 0;
                while (true)
                {
                    encoded.Add(polynomial[i]);
                    i++;
                }
            }
            catch {}

            return encoded;
        }

        /// <summary>
        /// Decodes a polynomial using the given key and coefficients
        /// </summary>
        /// <param name="coeffs">A list of the coefficients, with the least significant first</param>
        public static string DecodePolynomial(List<double> coeffs, int key)
        {
            string output = "";

            var polynomial = new Polynomial(coeffs);
            var roots = polynomial.FindRoots();
            var decodedChars = new Dictionary<double, Tuple<int, char>>();

            for (int i = 0; i < roots.Length; i++)
            {
                // Considering ax-b=0, where a and b are the same value,
                // the roots listed are only x. We need a*x, so to do that
                // we convert the decimal to a fraction and take the denominator
                double realValue = roots[i] * Fraction.FromRealNumber(roots[i], 0.01).D;

                // Shift the value back to its character by subtracting the key
                realValue -= key;

                // Track this character to determine the character's position
                double identifier = Math.Round(roots[i], 2);
                if (decodedChars.ContainsKey(identifier))
                {
                    decodedChars[identifier] = new Tuple<int, char>(
                        decodedChars[identifier].Item1 + 1,
                        decodedChars[identifier].Item2
                    );
                }
                else
                {
                    decodedChars.Add(identifier, new Tuple<int, char>(
                        1,
                        Lib.Common.AlphabetIndexToChar((int)Math.Round(realValue) - 1)
                    ));
                }
            }

            // Now that the characters have been extracted, we need to put them
            // in the correct order
            var orderedPairs = decodedChars.OrderBy(
                data => data.Value.Item1);
            foreach (var pair in orderedPairs)
            {
                output += pair.Value.Item2;
            }

            return output;
        }
    }
}
