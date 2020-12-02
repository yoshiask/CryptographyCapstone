using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Storage;

namespace CryptographyCapstone.Lib
{
    public static class Common
    {
        public static int CharToAlphabetIndex(char ch)
        {
            return Modulo(char.ToUpper(ch) - 65, 26);
        }

        public static char AlphabetIndexToChar(int i)
        {
            return (char)(Modulo(i, 26) + 65);
        }

        public static byte[] ConvertToByteArray(string str, System.Text.Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static String ToBinary(Byte[] data, int padding)
        {
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(padding, '0')));
        }
        public static String ToBinary(Byte[] data)
        {
            return ToBinary(data, 8);
        }

        public static byte[] ToByte(string byteString)
        {
            var numOfBytes = (int)Math.Ceiling(byteString.Length / 8m);
            var bytes = new byte[numOfBytes];
            var chunkSize = 8;

            for (int i = 1; i <= numOfBytes; i++)
            {
                var startIndex = byteString.Length - 8 * i;
                if (startIndex < 0)
                {
                    chunkSize = 8 + startIndex;
                    startIndex = 0;
                }
                bytes[numOfBytes - i] = Convert.ToByte(byteString.Substring(startIndex, chunkSize), 2);
            }
            return bytes;
        }

        public static string IntToPower(int num)
        {
            if (num == 1)
                return "";

            string output = "";
            var digits = IntToDigits(num);

            foreach (int digit in digits)
            {
                string digitChar;
                switch (digit % 10) {
                    case 0:
                        digitChar = "⁰";
                        break;

                    case 1:
                        digitChar = "¹";
                        break;

                    case 2:
                        digitChar = "²";
                        break;

                    case 3:
                        digitChar = "³";
                        break;

                    case 4:
                        digitChar = "⁴";
                        break;

                    case 5:
                        digitChar = "⁵";
                        break;

                    case 6:
                        digitChar = "⁶";
                        break;

                    case 7:
                        digitChar = "⁷";
                        break;

                    case 8:
                        digitChar = "⁸";
                        break;

                    case 9:
                        digitChar = "⁹";
                        break;

                    default:
                        digitChar = "";
                        break;
                }
                output += digitChar;
            }

            return output;
        }

        public static int[] IntToDigits(int num)
        {
            List<int> listOfInts = new List<int>();
            while(num > 0)
            {
                listOfInts.Add(num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }

        public static int GCD(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a == 0 ? b : a;
        }

        public static int LCM(int a, int b)
        {
            return (a * b) / GCD(a, b);
        }

        public static int FindCoprime(int num, int start)
        {
            for (int i = start; i < num; i++)
            {
                if (GCD(num, i) == 1)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Sums the specified function
        /// </summary>
        /// <param name="initial">The index to start with</param>
        /// <param name="final">The last index to compute</param>
        /// <param name="selFunc">selFunc(i, initial, final)</param>
        public static int Sum(int initial, int final, Func<int, int> selFunc)
        {
            int total = 0;
            for (int i = initial; i <= final; i++)
            {
                total += selFunc(i);
            }
            return total;
        }

        public static int Modulo(int a, int m)
        {
            int r = a % m;
            return (r < 0) ? r + m : r;
        }

        public static double IndexOfCoincidence(int length, List<int> freq)
        {
            double sum = Sum(0, freq.Count - 1,
                (i) => freq[i] * (freq[i] - 1)
            );
            double combinations = (double)length * (length - 1) / freq.Count;
            return sum / combinations;
        }
        public static double IndexOfCoincidence(string text)
        {
            return IndexOfCoincidence(text.Length, GetFrequencies(text).Values.ToList());
        }
        public const double IC_TELEGRAPHIC_ENGLISH = 1.73;

        public static Dictionary<char, int> GetFrequencies(string cipherText, bool order = false, bool ignoreCase = false)
        {
            var freq = new Dictionary<char, int>();
            foreach (char ch in cipherText)
            {
                char actualCh = ignoreCase ? ch.ToString().ToUpper()[0] : ch;
                if (freq.ContainsKey(actualCh))
                {
                    freq[actualCh]++;
                }
                else
                {
                    freq.Add(actualCh, 1);
                }
            }

            if (order)
            {
                return freq.OrderBy(pair => pair.Value).ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                return freq;
            }
        }

        public static string RepeatString(string input, int targetLength)
        {
            if (targetLength < input.Length)
                throw new ArgumentException("Target length cannot be shorter than the length of the text");

            string output = "";
            for (int i = 0; i < targetLength; i++)
            {
                output += input[i % input.Length];
            }
            return output;
        }

        /// <summary>
        /// Returns the multiplicative inverse of an integer a mod n
        /// </summary>
        public static int MultiplicativeInverse(int a, int n)
        {
            for (int x = 1; x < n + 1; x++)
            {
                if (Modulo(a * x, n) == 1)
                    return x;
            }
 
            throw new ArgumentException("No multiplicative inverse found");
        }

        public static Dictionary<T2, T1> SwapColumns<T1, T2>(Dictionary<T1, T2> input)
        {
            var output = new Dictionary<T2, T1>();
            foreach (KeyValuePair<T1, T2> pair in input)
            {
                output.Add(pair.Value, pair.Key);
            }
            return output;
        }

        public static IEnumerable<string> SplitIntoNGrams(string input, int n)
        {
            for (int i = 0; i < input.Length; i += n)
                yield return input.Substring(i, Math.Min(n, input.Length - i));
        }

        public static string PrepForCipher(string text)
        {
            string output = "";
            foreach (char ch in text)
                if (Char.IsLetter(ch)) output += Char.ToUpper(ch);
            return output;
        }

        public static string[] SplitWords(string text)
        {
            return text.Split(
                new char[] { ' ', ',', '.', '!', '?' },
                StringSplitOptions.RemoveEmptyEntries);
        }

        private static List<string> EnglishDict;
        public static async Task InitEnglishDict()
        {
            if (EnglishDict != null)
                return;

            EnglishDict = new List<string>();
            StorageFolder installationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await installationFolder.GetFileAsync("Assets\\words_alpha.txt");
            if(File.Exists(file.Path))
            {
                foreach (string word in File.ReadAllLines(file.Path))
                {
                    EnglishDict.Add(word.ToUpper());
                }
            }
        }
        public static async Task<bool> IsEnglishWord(string word)
        {
            if (EnglishDict == null)
            {
                await InitEnglishDict();
            }

            return EnglishDict.Contains(word);
        }
    }

    public struct Fraction
    {
        public Fraction(int n, int d)
        {
            N = n;
            D = d;
        }

        public static Fraction FromRealNumber(double value, double accuracy)
        {
            if (accuracy <= 0.0 || accuracy >= 1.0)
            {
                throw new ArgumentOutOfRangeException("accuracy", "Must be > 0 and < 1.");
            }

            int sign = Math.Sign(value);

            if (sign == -1)
            {
                value = Math.Abs(value);
            }

            // Accuracy is the maximum relative error; convert to absolute maxError
            double maxError = sign == 0 ? accuracy : value * accuracy;

            int n = (int)Math.Floor(value);
            value -= n;

            if (value < maxError)
            {
                return new Fraction(sign * n, 1);
            }

            if (1 - maxError < value)
            {
                return new Fraction(sign * (n + 1), 1);
            }

            double z = value;
            int previousDenominator = 0;
            int denominator = 1;
            int numerator;

            do
            {
                z = 1.0 / (z - (int)z);
                int temp = denominator;
                denominator = denominator * (int)z + previousDenominator;
                previousDenominator = temp;
                numerator = Convert.ToInt32(value * denominator);
            }
            while (Math.Abs(value - (double)numerator / denominator) > maxError && z != (int)z);

            return new Fraction((n * denominator + numerator) * sign, denominator);
        }

        public int N { get; private set; }
        public int D { get; private set; }
    }
}
