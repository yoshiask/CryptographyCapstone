using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Extreme.Mathematics;
using Extreme.Mathematics.Curves;

namespace CryptographyCapstone.Lib
{
    public static class Cryptoanalyzer
    {
        public enum EncryptionMethods
        {
            Shift, Substitution, Polyalphabetic, Vigenere, Bifid, Atbash, MorseCode, Ascii, OneTimePad, Affine, Multiplicative, Polynomial, PolynomialBlock
        }

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
        public static char DecryptShiftCipher(char cipherText, int key)
        {
            return EncryptShiftCipher(cipherText, -key);
        }

        public static Dictionary<int, string> GuessShiftCipher(string cipherText)
        {
            var ICList = new Dictionary<Tuple<int, string>, double>();
            for (int i = 1; i <= 26; i++)
            {
                string plainTextGuess = DecryptShiftCipher(cipherText, i);
                ICList.Add(new Tuple<int, string>(i, plainTextGuess),
                    Common.IndexOfCoincidence(plainTextGuess)
                );
            }

            var guessedKeys = new Dictionary<int, string>();
            foreach (var ic in ICList.OrderByDescending(d => Math.Abs(Common.IC_TELEGRAPHIC_ENGLISH - d.Value)))
            {
                guessedKeys.Add(ic.Key.Item1, ic.Key.Item2);
            }
            return guessedKeys;
        }
        #endregion

        #region Simple Substitution Cipher
        public static string EncryptSubstitutionCipher(string plainText, Dictionary<string, string> key)
        {
            string cipherText = "";

            foreach (var ch in plainText)
            {
                if (key.ContainsKey(ch.ToString().ToUpper()))
                    cipherText += key[ch.ToString().ToUpper()];
                else
                    cipherText += ch;
            }

            return cipherText;
        }

        public static string DecryptSubstitutionCipher(string cipherText, Dictionary<string, string> key)
        {
            return EncryptSubstitutionCipher(cipherText, Common.SwapColumns(key));
        }
        #endregion

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

        public static string EncryptVigenereCipher(string plainText, string key)
        {
            // https://en.wikipedia.org/wiki/Vigen%C3%A8re_cipher
            string cipherText = "";
            for (int i = 0; i < plainText.Length; i++)
            {
                int shiftKey = Common.CharToAlphabetIndex(key[i % key.Length]);
                cipherText += EncryptShiftCipher(plainText[i], shiftKey);
            }
            return cipherText;
        }

        public static string DecryptVigenereCipher(string cipherText, string key)
        {
            // https://en.wikipedia.org/wiki/Vigen%C3%A8re_cipher
            string plainText = "";
            for (int i = 0; i < cipherText.Length; i++)
            {
                int shiftKey = Common.CharToAlphabetIndex(key[i % key.Length]);
                plainText += DecryptShiftCipher(cipherText[i], shiftKey);
            }
            return plainText;
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
        public static string EncryptAffineCipher(string plainText, int a, int b)
        {
            string cipherText = "";
            
            // Compute e(x) = (ax + b)(mod m) for every character in the Plain Text
            foreach (char c in plainText)
            {
                if (Char.IsLetter(c))
                {
                    int x = Common.CharToAlphabetIndex(c);
                    cipherText += Common.AlphabetIndexToChar(a * x + b);
                }
                else
                {
                    cipherText += c;
                }
            }
 
            return cipherText;
        }
        
        public static string DecryptAffineCipher(string cipherText, int a, int b)
        {
            string plainText = "";
 
            // Get Multiplicative Inverse of a
            int aInverse = Common.MultiplicativeInverse(a, 26);
 
            // Put Cipher Text (all capitals) into Character Array
            char[] chars = cipherText.ToUpper().ToCharArray();
 
            // Computer d(x) = aInverse * (e(x)  b)(mod m)
            foreach (char c in chars)
            {
                if (char.IsLetter(c))
                {
                    int x = Common.CharToAlphabetIndex(c);
                    plainText += Common.AlphabetIndexToChar(aInverse * (x - b));
                }
                else
                {
                    plainText += c;
                }
            }
 
            return plainText;
        }

        public static Dictionary<Tuple<int, int>, string> GuessAffineCipher(string cipherText)
        {
            Debug.WriteLine("Affine brute-force started: " + DateTime.Now);
            var ICList = new ConcurrentDictionary<Tuple<int, int, string>, double>();
            Parallel.For(1, 26, (i, state) =>
            {
                if (i % 2 == 0)
                    return;

                for (int j = 1; j < 26; j += 2)
                {
                    // 13 is a special case, because 13 * 2 = 26.
                    if (i == 13) continue;

                    string plainTextGuess = DecryptAffineCipher(cipherText, i, j);
                    ICList.TryAdd(new Tuple<int, int, string>(i, j, plainTextGuess),
                        Common.IndexOfCoincidence(plainTextGuess)
                    );
                }
            });

            var guessedKeys = new Dictionary<Tuple<int, int>, string>();
            foreach (var ic in ICList.OrderByDescending(d => Math.Abs(Common.IC_TELEGRAPHIC_ENGLISH - d.Value)))
            {
                guessedKeys.Add(new Tuple<int, int>(ic.Key.Item1, ic.Key.Item2), ic.Key.Item3);
            }
            Debug.WriteLine("Affine brute-force completed: " + DateTime.Now);
            return guessedKeys;
        }

        #endregion

        #region Multiplicative Cipher
        public static string EncryptMultiplicativeCipher(string plainText, int key)
        {
            string cipherText = "";
            
            // Compute e(x) = (ax)(mod m) for every character in the Plain Text
            foreach (char c in plainText)
            {
                if (char.IsLetter(c))
                {
                    int x = Common.CharToAlphabetIndex(c);
                    cipherText += Common.AlphabetIndexToChar(key * x);
                }
                else
                {
                    cipherText += c;
                }
            }
 
            return cipherText;
        }
        
        public static string DecryptMultiplicativeCipher(string cipherText, int key)
        {
            string plainText = "";
 
            // Get Multiplicative Inverse of a
            int aInverse = Common.MultiplicativeInverse(key, 26);
 
            // Put Cipher Text (all capitals) into Character Array
            char[] chars = cipherText.ToUpper().ToCharArray();
 
            // Compute d(x) = aInverse * e(x)(mod m)
            foreach (char c in chars)
            {
                if (char.IsLetter(c))
                {
                    int x = Common.CharToAlphabetIndex(c);
                    plainText += Common.AlphabetIndexToChar(aInverse * x);
                }
                else
                {
                    plainText += c;
                }
            }
 
            return plainText;
        }

        public static Dictionary<int, string> GuessMultiplicativeCipher(string cipherText)
        {
            var ICList = new Dictionary<Tuple<int, string>, double>();
            Parallel.For(1, 27, i =>
            {
                try
                {
                    string plainTextGuess = DecryptMultiplicativeCipher(cipherText, i);
                    ICList.Add(new Tuple<int, string>(i, plainTextGuess),
                        Common.IndexOfCoincidence(plainTextGuess)
                    );
                }
                catch (ArgumentException)
                {
                }
            });

            var guessedKeys = new Dictionary<int, string>();
            foreach (var ic in ICList.OrderByDescending((d => 1 - d.Value)))
            {
                guessedKeys.Add(ic.Key.Item1, ic.Key.Item2);
            }
            return guessedKeys;
        }
        #endregion

        #region Polynomial Block Cipher
        public static string EncryptPolynomialBlockCipher(string plainText, int key, int blockSize = 2)
        {
            string cipherText = "";

            // Even keys prevent the characters from being decrypted in the proper order
            if (key.IsEven()) key++;

            foreach (var raw in Common.SplitIntoNGrams(plainText, blockSize))
            {
                string ngram = raw.Replace(" ", "Z").Replace("-", "X").Replace(".", "Q");
                cipherText += PolynomialCipher.CoeffsToPolynomial(PolynomialCipher.EncryptString(ngram, key)) + "\r\n";
            }

            return cipherText;
        }

        public static string DecryptPolynomialBlockCipher(List<List<double>> coeffs, int key, int blockSize = 2)
        {
            string plainText = "";
            if (key.IsEven()) key++;

            foreach (var coeff in coeffs)
            {
                string cipherPart = PolynomialCipher.DecodePolynomial(coeff, key);
                plainText += cipherPart.Replace("Z", " ").Replace("X", "-").Replace("Q", ".");
            }
            return plainText;
        }
        #endregion

        #region Bifid Cipher
        public static string EncryptBifidCipher(string pT, string key)
        {
            string plainText = Common.PrepForCipher(pT);
            string cipherText = "";
            var tableauChar = new Table<char>(5, 5, '\0');

            // Generate the tableau
            int currentDefaultKey = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    // i is row, j is column
                    int singleIndex = (j * 5) + i;
                    if (singleIndex >= key.Length)
                    {
                        char defaultKeyChar = Common.AlphabetIndexToChar(currentDefaultKey);
                        if (!tableauChar.IsInTable(defaultKeyChar) && defaultKeyChar != 'Q')
                        {
                            tableauChar[i, j] = defaultKeyChar;
                        }
                        currentDefaultKey++;
                    }
                    else
                    {
                        char keyChar = key[singleIndex];
                        if (!tableauChar.IsInTable(keyChar))
                        {
                            tableauChar[i, j] = keyChar;
                        }
                    }

                    Debug.Write(tableauChar[i, j] + " ");
                }
                Debug.WriteLine("");
            }

            foreach (char ch in plainText)
            {
                
            }
            return cipherText;
        }
        #endregion

        #region Atbash Cipher
        private static Dictionary<string, string> AtbashKey = new Dictionary<string, string>()
        {
            { "A", "Z" },
            { "B", "Y" },
            { "C", "X" },
            { "D", "W" },
            { "E", "V" },
            { "F", "U" },
            { "G", "T" },
            { "H", "S" },
            { "I", "R" },
            { "J", "Q" },
            { "K", "P" },
            { "L", "O" },
            { "M", "N" },
            { "N", "M" },
            { "O", "L" },
            { "P", "K" },
            { "Q", "J" },
            { "R", "I" },
            { "S", "H" },
            { "T", "G" },
            { "U", "F" },
            { "V", "E" },
            { "W", "D" },
            { "X", "C" },
            { "Y", "B" },
            { "Z", "A" },
        };
        public static string EncryptAtbashCipher(string plainText)
        {
            return EncryptSubstitutionCipher(plainText, AtbashKey);
        }

        public static string DecryptAtbashCipher(string cipherText)
        {
            return EncryptAtbashCipher(cipherText);
        }
        #endregion

        #region Morse Code
        private static Dictionary<string, string> MorseCodeKey = new Dictionary<string, string>()
        {
            { "A", ".-" },
            { "B", "-..." },
            { "C", "-.-." },
            { "D", "-.." },
            { "E", "." },
            { "F", "..-." },
            { "G", "--." },
            { "H", "...." },
            { "I", ".." },
            { "J", ".---" },
            { "K", "-.-" },
            { "L", ".-.." },
            { "M", "--" },
            { "N", "-." },
            { "O", "---" },
            { "P", ".--." },
            { "Q", "--.-" },
            { "R", ".-." },
            { "S", "..." },
            { "T", "-" },
            { "U", "..-" },
            { "V", "...-" },
            { "W", ".--" },
            { "X", "-..-" },
            { "Y", "-.--" },
            { "Z", "--.." },
            { "1", ".----" },
            { "2", "..---" },
            { "3", "...--" },
            { "4", "....-" },
            { "5", "....." },
            { "6", "-...." },
            { "7", "--..." },
            { "8", "---.." },
            { "9", "----." },
            { "0", "-----" },
        };
        
        public static string EncryptMorseCode(string plainText)
        {
            return EncryptMorseCode(plainText, ".", "-", " ");
        }
        public static string EncryptMorseCode(string plainText, string dot, string dash, string space)
        {
            string cipherText = "";
            var key = GenerateMorseCodeKey(dot, dash);

            foreach (var word in Common.SplitWords(plainText))
            {
                foreach (var ch in word)
                {
                    if (key.ContainsKey(ch.ToString().ToUpper()))
                        cipherText += key[ch.ToString().ToUpper()];
                    else
                        cipherText += ch;
                    cipherText += " ";
                }
                cipherText += space;
            }

            return cipherText;
        }

        public static string DecryptMorseCode(string cipherText)
        {
            return DecryptMorseCode(cipherText, ".", "-", " ");
        }
        public static string DecryptMorseCode(string cipherText, string dot, string dash, string space)
        {
            string plainText = "";
            var key = Common.SwapColumns(GenerateMorseCodeKey(dot, dash));

            foreach (var letter in cipherText.Split(space))
            {
                if (key.ContainsKey(letter))
                    plainText += key[letter];
                else
                    plainText += letter;
            }

            return plainText;
        }

        private static Dictionary<string, string> GenerateMorseCodeKey(string dot, string dash)
        {
            var key = new Dictionary<string, string>();
            foreach (var pair in MorseCodeKey)
            {
                key[pair.Key] = pair.Value.Replace(".", dot).Replace("-", dash);
            }
            return key;
        }
        #endregion

        #region Index Encoding
        public static string EncryptIndexEncoding(string plainText)
        {
            string output = "";
            foreach (char ch in plainText)
            {
                if (char.IsLetter(ch))
                    output += Common.CharToAlphabetIndex(ch) + " ";
                else
                    output += ch;
            }
            return output;
        }

        public static string DecryptIndexEncoding(string cipherText)
        {
            string output = "";
            foreach (char ch in cipherText)
            {
                if (char.IsDigit(ch))
                    output += Common.AlphabetIndexToChar(int.Parse(ch.ToString())) + " ";
                else
                    output += ch;
            }
            return output;
        }
        #endregion

        #region ASCII Encoding
        public static string EncryptAsciiEncoding(string plainText, int @base = 2)
        {
            return Common.ToBinary(Common.ConvertToByteArray(plainText, Encoding.ASCII));
        }

        public static string DecryptAsciiEncoding(string cipherText, int @base = 2)
        {
            List<byte> bytes = new List<byte>();
            foreach (string byteString in Common.SplitWords(cipherText))
            {
                bytes.Add(Convert.ToByte(byteString, @base));
            }
            return Encoding.ASCII.GetString(bytes.ToArray());
        }
        #endregion

        #region Cryptanalysis
        public static async Task<IDictionary<string, double>> RankCandidatePlaintext(IEnumerable<string> guesses)
        {
            var unsortedList = new Dictionary<string, double>();
            foreach (string guess in guesses)
            {
                unsortedList.Add(guess, await RankCandidatePlaintext(guess));
            }
            return unsortedList.OrderByDescending(pair => pair.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public static async Task<double> RankCandidatePlaintext(string guess)
        {
            // Set up the dictionary we're going to use
            await Common.InitEnglishDict();

            int realWordCount = 0;
            var words = Common.SplitWords(guess);
            foreach (var word in words)
            {
                if (await Common.IsEnglishWord(word))
                    realWordCount++;
            }
            return (double)realWordCount / words.Length;
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
                if (Math.Abs(coeffs[i] - 1.0) > 0.00001)
                    output += Math.Abs(coeffs[i]);

                if (i == 0) continue;
                output += "x" + Common.IntToPower(i);
                if (coeffs[i-1] >= 0)
                    output += " + ";
                else
                    output += " - ";
            }

            return output;
        }

        public static List<double> ParsePolynomial(string polynomial)
        {
            // Insert 1 where x has no coefficient
            polynomial = Regex.Replace(polynomial, @"(?<!\d+)x", "1x");

            // Extract the coefficients
            var matches = Regex.Matches(polynomial, @"(?<sign>-?)(?:\s?)(?<value>[\d.]+)+");
            var coeffs = new List<double>(matches.Count);
            foreach (Match match in matches)
            {
                coeffs.Add(double.Parse(match.Groups["sign"].Value + match.Groups["value"].Value));
            }

            return coeffs;
        }

        public static List<string> EncodeMessage(string message, int key)
        {
            var output = new List<string>();
            // Encode each word
            foreach (string word in Common.SplitWords(message))
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
                int c = Common.CharToAlphabetIndex(ch) + 1;
                if (c >= 0 && c < 65)
                    plaintext.Add(c);
            }

            if (plaintext.Count < 1)
                return encoded;

            // Convert the plaintext numbers to a usable list of roots,
            // where the multiplicity of the root is the position of the character,
            // and the value of *a* where x-a=0 is the character itself.
            var roots = new List<double>();
            int[] lastCoprime = new int[27];
            for (int i = 0; i < lastCoprime.Length; i++)
            {
                lastCoprime[i] = 1;
            }
            for (int i = 0; i < plaintext.Count; i++)
            {
                int currentChar = plaintext[i];
                // First, determine which appearance of the character this is
                int coprime = Common.FindCoprime(currentChar + key, lastCoprime[currentChar]);
                double root = (double)(currentChar + key) / coprime;
                lastCoprime[currentChar] = coprime+1;

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
            var polynomial = new Polynomial(coeffs);
            var roots = polynomial.FindRoots();
            var decodedChars = new Dictionary<double, Tuple<int, char>>();

            foreach (var root in roots)
            {
                // Considering ax-b=0, where a and b are the same value,
                // the roots listed are only x. We need a*x, so to do that
                // we convert the decimal to a fraction and take the denominator
                double realValue = Fraction.FromRealNumber(root, 0.01).D;

                // Shift the value back to its character by subtracting the key
                realValue -= key;

                // Track this character to determine the character's position
                double identifier = Math.Round(root, 2);
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
                        Common.AlphabetIndexToChar((int)Math.Round(realValue) - 1)
                    ));
                }
            }

            // Now that the characters have been extracted, we need to put them
            // in the correct order
            var orderedPairs = decodedChars.OrderBy(
                data => data.Value.Item1);

            return orderedPairs.Aggregate(string.Empty, (current, pair) => current + pair.Value.Item2);
        }
    }
}
