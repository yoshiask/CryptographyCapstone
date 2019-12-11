using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PasswordExperiment
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Please specify a password to attack.");
                Environment.Exit(1);
            }
            password = args[0];
            passwordHash = GetHashString(password);
            initBruteForce();
        }

        #region Private variables

        // the secret password which we will try to find via brute force
        private static string password;
        private static string passwordHash;

        private static bool isMatched = false;

        /* The length of the charactersToTest Array is stored in a
         * additional variable to increase performance  */
        private static int charactersToTestLength = 0;
        private static long computedKeys = 0;

        /* An array containing the characters which will be used to create the brute force keys,
         * if less characters are used (e.g. only lower case chars) the faster the password is matched  */
        private static char[] charactersToTest =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
            'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8',
            '9', '0', '!', '$', '#', '@', '-', '>', '^', '_'
        };

        private static DateTime timeStarted;

        #endregion

        #region Brute Force methods
        /// <summary>
        /// Starts a dictionary and brute force attack against the password hash
        /// </summary>
        public static void initBruteForce()
        {
            timeStarted = DateTime.Now;
            Console.WriteLine("Start BruteForce - {0}", timeStarted.ToString());

            // The length of the array is stored permanently during runtime
            charactersToTestLength = charactersToTest.Length;

            // The length of the password is unknown, so we have to run trough the full search space
            var estimatedPasswordLength = 0;

            testFromDictionary(0, System.IO.File.ReadAllLines(
                @"C:\Users\jjask\source\repos\CryptographyCapstone\PasswordExperiment\PasswordDictionary.txt"
            ));

            while (!isMatched)
            {
                /* The estimated length of the password will be increased and every possible key for this
                 * key length will be created and compared against the password */
                estimatedPasswordLength++;
                startBruteForce(estimatedPasswordLength);
            }
        }

        /// <summary>
        /// Starts the recursive method which will create the keys via brute force
        /// </summary>
        /// <param name="keyLength">The length of the key</param>
        private static void startBruteForce(int keyLength)
        {
            var keyChars = createCharArray(keyLength, charactersToTest[0]);
            // The index of the last character will be stored for slight perfomance improvement
            var indexOfLastChar = keyLength - 1;
            createNewKey(0, keyChars, keyLength, indexOfLastChar);
        }

        /// <summary>
        /// Creates a new char array of a specific length filled with the defaultChar
        /// </summary>
        /// <param name="length">The length of the array</param>
        /// <param name="defaultChar">The char with whom the array will be filled</param>
        /// <returns></returns>
        private static char[] createCharArray(int length, char defaultChar)
        {
            return (from c in new char[length] select defaultChar).ToArray();
        }

        /// <summary>
        /// This is the main workhorse, it creates new keys and compares them to the password until the password
        /// is matched or all keys of the current key length have been checked
        /// </summary>
        /// <param name="currentCharPosition">The position of the char which is replaced by new characters currently</param>
        /// <param name="keyChars">The current key represented as char array</param>
        /// <param name="keyLength">The length of the key</param>
        /// <param name="indexOfLastChar">The index of the last character of the key</param>
        private static void createNewKey(int currentCharPosition, char[] keyChars, int keyLength, int indexOfLastChar)
        {
            var nextCharPosition = currentCharPosition + 1;
            // We are looping trough the full length of our charactersToTest array
            for (int i = 0; i < charactersToTestLength; i++)
            {
                /* The character at the currentCharPosition will be replaced by a
                 * new character from the charactersToTest array => a new key combination will be created */
                keyChars[currentCharPosition] = charactersToTest[i];

                // The method calls itself recursively until all positions of the key char array have been replaced
                if (currentCharPosition < indexOfLastChar)
                {
                    createNewKey(nextCharPosition, keyChars, keyLength, indexOfLastChar);
                }
                else
                {
                    // A new key has been created, remove this counter to improve performance
                    computedKeys++;

                    /* The char array will be converted to a string and compared to the password. If the password
                     * is matched the loop breaks and the password is stored as result. */
                    string guess = new String(keyChars);
                    Console.WriteLine("Guessing: " + guess);
                    if (checkGuess(guess))
                    {
                        if (!isMatched)
                        {
                            isMatched = true;
                            resultFound(guess);
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Tests every item in the specified dictionary against the password hash
        /// </summary>
        /// <param name="index"></param>
        /// <param name="dictionary"></param>
        private static void testFromDictionary(int index, string[] dictionary)
        {
            if (index < dictionary.Length)
            {
                string guess = dictionary[index];
                if (guess.StartsWith("#"))
                {
                    testFromDictionary(index + 1, dictionary);
                }
                else
                {
                    Console.WriteLine("Guessing: " + guess);
                    // Check if this hash matches the hash of the password we're trying to crack
                    if (checkGuess(guess))
                    {
                        if (!isMatched)
                        {
                            isMatched = true;
                            resultFound(guess);
                        }
                        return;
                    }
                    else
                    {
                        testFromDictionary(index + 1, dictionary);
                    }
                }
            }
        }

        /// <summary>
        /// Prints out some info about the attack and exits the application on user input
        /// </summary>
        /// <param name="result"></param>
        private static void resultFound(string result)
        {
            Console.WriteLine("Password matched. - {0}", DateTime.Now.ToString());
            Console.WriteLine("Time passed: {0}s", DateTime.Now.Subtract(timeStarted).TotalSeconds);
            Console.WriteLine("Resolved password: {0}", result);
            Console.WriteLine("Computed keys: {0}", computedKeys);

            Console.ReadLine();

            Environment.Exit(0);
        }

        #endregion

        /// <summary>
        /// Converts a string into a list of bytes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] toBytes(string input)
        {
            var chars = input.ToCharArray();
            byte[] output = new byte[input.Length];
            
            for (int i = 0; i < chars.Length; i++)
            {
                output[i] = Convert.ToByte(chars[i]);
            }

            return output;
        }

        /// <summary>
        /// Checks a plain-text guess against a password hash
        /// </summary>
        /// <param name="guess"></param>
        /// <returns></returns>
        private static bool checkGuess(string guess)
        {
            if (guess == password)
                return GetHashString(guess).Equals(passwordHash);
            else
                return false;
        }

        /// <summary>
        /// Hashes a string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        /// <summary>
        /// Hashes a string and returns a string (for easy comparison)
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
