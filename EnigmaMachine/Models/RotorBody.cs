using System;
using System.Collections.Generic;
using System.Linq;

namespace EnigmaMachine.Models
{
    public class RotorBody
    {
        #region Rotor Options
        private static char[][] Rotor1 => new[]
        {
            new[] { 'A', 'E' },
            new[] { 'B', 'K' },
            new[] { 'C', 'M' },
            new[] { 'D', 'F' },
            new[] { 'E', 'L' },
            new[] { 'F', 'G' },
            new[] { 'G', 'D' },
            new[] { 'H', 'Q' },
            new[] { 'I', 'V' },
            new[] { 'J', 'Z' },
            new[] { 'K', 'N' },
            new[] { 'L', 'T' },
            new[] { 'M', 'O' },
            new[] { 'N', 'W' },
            new[] { 'O', 'Y' },
            new[] { 'P', 'H' },
            new[] { 'Q', 'X' },
            new[] { 'R', 'U' },
            new[] { 'S', 'S' },
            new[] { 'T', 'P' },
            new[] { 'U', 'A' },
            new[] { 'V', 'I' },
            new[] { 'W', 'B' },
            new[] { 'X', 'R' },
            new[] { 'Y', 'C' },
            new[] { 'Z', 'J' },
        };

        private static char[][] Rotor2 => new[]
        {
            new[] { 'A', 'A' },
            new[] { 'B', 'J' },
            new[] { 'C', 'D' },
            new[] { 'D', 'K' },
            new[] { 'E', 'S' },
            new[] { 'F', 'I' },
            new[] { 'G', 'R' },
            new[] { 'H', 'U' },
            new[] { 'I', 'X' },
            new[] { 'J', 'B' },
            new[] { 'K', 'L' },
            new[] { 'L', 'H' },
            new[] { 'M', 'W' },
            new[] { 'N', 'T' },
            new[] { 'O', 'M' },
            new[] { 'P', 'C' },
            new[] { 'Q', 'Q' },
            new[] { 'R', 'G' },
            new[] { 'S', 'Z' },
            new[] { 'T', 'N' },
            new[] { 'U', 'P' },
            new[] { 'V', 'Y' },
            new[] { 'W', 'F' },
            new[] { 'X', 'V' },
            new[] { 'Y', 'O' },
            new[] { 'Z', 'E' },
        };

        private static char[][] Rotor3 => new[]
        {
            new[] { 'A', 'B' },
            new[] { 'B', 'D' },
            new[] { 'C', 'F' },
            new[] { 'D', 'H' },
            new[] { 'E', 'J' },
            new[] { 'F', 'L' },
            new[] { 'G', 'C' },
            new[] { 'H', 'P' },
            new[] { 'I', 'R' },
            new[] { 'J', 'T' },
            new[] { 'K', 'X' },
            new[] { 'L', 'V' },
            new[] { 'M', 'Z' },
            new[] { 'N', 'N' },
            new[] { 'O', 'Y' },
            new[] { 'P', 'E' },
            new[] { 'Q', 'I' },
            new[] { 'R', 'W' },
            new[] { 'S', 'G' },
            new[] { 'T', 'A' },
            new[] { 'U', 'K' },
            new[] { 'V', 'M' },
            new[] { 'W', 'U' },
            new[] { 'X', 'S' },
            new[] { 'Y', 'Q' },
            new[] { 'Z', 'O' },
        };

        private static char[][] Rotor4 => new[]
        {
            new[] { 'A', 'E' },
            new[] { 'B', 'S' },
            new[] { 'C', 'O' },
            new[] { 'D', 'V' },
            new[] { 'E', 'P' },
            new[] { 'F', 'Z' },
            new[] { 'G', 'J' },
            new[] { 'H', 'A' },
            new[] { 'I', 'Y' },
            new[] { 'J', 'Q' },
            new[] { 'K', 'U' },
            new[] { 'L', 'I' },
            new[] { 'M', 'R' },
            new[] { 'N', 'H' },
            new[] { 'O', 'X' },
            new[] { 'P', 'L' },
            new[] { 'Q', 'N' },
            new[] { 'R', 'F' },
            new[] { 'S', 'T' },
            new[] { 'T', 'G' },
            new[] { 'U', 'K' },
            new[] { 'V', 'D' },
            new[] { 'W', 'C' },
            new[] { 'X', 'M' },
            new[] { 'Y', 'W' },
            new[] { 'Z', 'B' },
        };

        private static char[][] Rotor5 => new[]
        {
            new[] { 'A', 'V' },
            new[] { 'B', 'Z' },
            new[] { 'C', 'B' },
            new[] { 'D', 'R' },
            new[] { 'E', 'G' },
            new[] { 'F', 'I' },
            new[] { 'G', 'T' },
            new[] { 'H', 'Y' },
            new[] { 'I', 'U' },
            new[] { 'J', 'P' },
            new[] { 'K', 'S' },
            new[] { 'L', 'D' },
            new[] { 'M', 'N' },
            new[] { 'N', 'H' },
            new[] { 'O', 'L' },
            new[] { 'P', 'X' },
            new[] { 'Q', 'A' },
            new[] { 'R', 'W' },
            new[] { 'S', 'M' },
            new[] { 'T', 'J' },
            new[] { 'U', 'Q' },
            new[] { 'V', 'O' },
            new[] { 'W', 'F' },
            new[] { 'X', 'E' },
            new[] { 'Y', 'C' },
            new[] { 'Z', 'K' },
        };

        private static char[][] RotorUKWa => new[]
        {
            new[] { 'A', 'E' },
            new[] { 'B', 'J' },
            new[] { 'C', 'M' },
            new[] { 'D', 'Z' },
            new[] { 'E', 'A' },
            new[] { 'F', 'L' },
            new[] { 'G', 'Y' },
            new[] { 'H', 'X' },
            new[] { 'I', 'V' },
            new[] { 'J', 'B' },
            new[] { 'K', 'W' },
            new[] { 'L', 'F' },
            new[] { 'M', 'C' },
            new[] { 'N', 'R' },
            new[] { 'O', 'Q' },
            new[] { 'P', 'U' },
            new[] { 'Q', 'O' },
            new[] { 'R', 'N' },
            new[] { 'S', 'T' },
            new[] { 'T', 'S' },
            new[] { 'U', 'P' },
            new[] { 'V', 'I' },
            new[] { 'W', 'K' },
            new[] { 'X', 'H' },
            new[] { 'Y', 'G' },
            new[] { 'Z', 'D' },
        };

        private static char[][] RotorUKWb => new[]
        {
            new[] { 'A', 'Y' },
            new[] { 'B', 'R' },
            new[] { 'C', 'U' },
            new[] { 'D', 'H' },
            new[] { 'E', 'Q' },
            new[] { 'F', 'S' },
            new[] { 'G', 'L' },
            new[] { 'H', 'D' },
            new[] { 'I', 'P' },
            new[] { 'J', 'X' },
            new[] { 'K', 'N' },
            new[] { 'L', 'G' },
            new[] { 'M', 'O' },
            new[] { 'N', 'K' },
            new[] { 'O', 'M' },
            new[] { 'P', 'I' },
            new[] { 'Q', 'E' },
            new[] { 'R', 'B' },
            new[] { 'S', 'F' },
            new[] { 'T', 'Z' },
            new[] { 'U', 'C' },
            new[] { 'V', 'W' },
            new[] { 'W', 'V' },
            new[] { 'X', 'J' },
            new[] { 'Y', 'A' },
            new[] { 'Z', 'T' },
        };

        private static char[][] RotorUkWc => new[]
        {
            new[] { 'A', 'F' },
            new[] { 'B', 'V' },
            new[] { 'C', 'P' },
            new[] { 'D', 'J' },
            new[] { 'E', 'I' },
            new[] { 'F', 'A' },
            new[] { 'G', 'O' },
            new[] { 'H', 'Y' },
            new[] { 'I', 'E' },
            new[] { 'J', 'D' },
            new[] { 'K', 'R' },
            new[] { 'L', 'Z' },
            new[] { 'M', 'X' },
            new[] { 'N', 'W' },
            new[] { 'O', 'G' },
            new[] { 'P', 'C' },
            new[] { 'Q', 'T' },
            new[] { 'R', 'K' },
            new[] { 'S', 'U' },
            new[] { 'T', 'Q' },
            new[] { 'U', 'S' },
            new[] { 'V', 'B' },
            new[] { 'W', 'N' },
            new[] { 'X', 'M' },
            new[] { 'Y', 'H' },
            new[] { 'Z', 'L' },
        };
        #endregion

        char[][] RotorAWiring, RotorBWiring, RotorCWiring;
        public int RotorAPos {
            get;
            set;
        }
        public int RotorBPos {
            get;
            set;
        }
        public int RotorCPos {
            get;
            set;
        }
        int RotorAInitPos, RotorBInitPos, RotorCInitPos;

        public RotorBody(int rotorA, int rotorB, int rotorC, int initA, int initB, int initC)
        {
            Setup(rotorA, rotorB, rotorC, initA, initB, initC);
        }
        public RotorBody(int rotorA, int rotorB, int rotorC)
        {
            Setup(rotorA, rotorB, rotorC, 0, 0, 0);
        }
        public RotorBody()
        {
            Setup(1, 2, 3, 0, 0, 0);
        }

        private void Setup(int rotorA, int rotorB, int rotorC, int initA, int initB, int initC)
        {
            RotorAWiring = GetWiringFromNumber(rotorA);
            RotorBWiring = GetWiringFromNumber(rotorB);
            RotorCWiring = GetWiringFromNumber(rotorC);

            initA %= RotorAWiring.Length;
            initB %= RotorBWiring.Length;
            initC %= RotorCWiring.Length;

            RotorAInitPos = initA;
            RotorBInitPos = initB;
            RotorCInitPos = initC;

            RotorAPos = initA;
            RotorBPos = initB;
            RotorCPos = initC;

            RotorsChanged?.Invoke(new Tuple<int, int, int>(RotorAPos, RotorBPos, RotorCPos));
        }

        public void Advance()
        {
            RotorCPos +=1;
            if (RotorCPos == 26) {
                RotorCPos = 0;
                RotorBPos+=1;
                if (RotorBPos == 26) {
                    RotorBPos = 0;
                    RotorAPos+=1;
                    if (RotorAPos == 26) {
                        RotorAPos = 0;
                    }
                }
            }
            RotorsChanged?.Invoke(new Tuple<int, int, int>(RotorAPos, RotorBPos, RotorCPos));
        }

        public int SendThroughRotor(char character, int position, char[][] wiring, bool forward)
        {
            int input = CharToAlphabetIndex(character);

            if (forward)
            {
                input = (input + position) % 26;

                return wiring[input][1];
            }
            else
            {
                var other = wiring.ToList().Find((x) => AlphabetIndexToChar(input) == x[1]);
                if (other != null && other.Length > 0)
                {
                    int output = (other[0] - position);
                    while (output < 0)
                    {
                        output = 26 + output;
                    }

                    output = output % 26;

                    return AlphabetIndexToChar(output);
                }

                return -1;
            }
        }

        public int SendThroughReflector(char character, char[][] wiring, bool forward)
        {
            int input = CharToAlphabetIndex(character);
            if (forward)
            {
                return wiring[input][1];
            }

            return wiring[input][0];
        }

        public char Encrypt(char plain)
        {

            char currentChar = plain;
            //currentNo = plugBoard.runThrough(currentNo);
            currentChar = (char)SendThroughRotor(currentChar, RotorCPos, RotorCWiring, true);
            currentChar = (char)SendThroughRotor(currentChar, RotorBPos, RotorBWiring, true);
            currentChar = (char)SendThroughRotor(currentChar, RotorAPos, RotorAWiring, true);
            currentChar = (char)SendThroughReflector(currentChar, RotorUKWb, true);
            currentChar = (char)SendThroughRotor(currentChar, RotorAPos, RotorAWiring, false);
            currentChar = (char)SendThroughRotor(currentChar, RotorBPos, RotorBWiring, false);
            currentChar = (char)SendThroughRotor(currentChar, RotorCPos, RotorCWiring, false);
            //currentNo = plugBoard.runThrough(currentNo);

            // Push the char through the right rotor
            /*char cChar = RotorCWiring.ElementAt(CharToAlphabetIndex(plain));

            // Push the char through the middle rotor
            char bcChar = RotorBWiring.ElementAt(CharToAlphabetIndex(cChar));

            // Push the char through the left rotor
            char abcChar = RotorAWiring.ElementAt(CharToAlphabetIndex(bcChar));

            // Push the char through the reflector
            char rflChar = RotorUKWb.ElementAt(CharToAlphabetIndex(abcChar));

            // Push the char back through each rotor
            char aChar = RotorAWiring.ElementAt(CharToAlphabetIndex(rflChar));
            char baChar = RotorBWiring.ElementAt(CharToAlphabetIndex(aChar));
            char cbaChar = RotorCWiring.ElementAt(CharToAlphabetIndex(baChar));*/

            Advance();

            //return cbaChar;
            return currentChar;
        }
        public char Encrypt(string plain)
        {
            return Encrypt(plain.ToCharArray()[0]);
        }

        public string EncryptString(string plain)
        {
            string encStr = "";
            char[] chars = plain.ToCharArray();
            foreach (char plChar in chars)
            {
                encStr += Encrypt(plChar);
            }
            return encStr;
        }

        private char[][] GetWiringFromNumber(int rotorNum)
        {
            switch (rotorNum)
            {
                case 1:
                    return Rotor1;

                case 2:
                    return Rotor2;

                case 3:
                    return Rotor3;

                case 4:
                    return Rotor4;

                case 5:
                    return Rotor5;

                default:
                    throw new ArgumentOutOfRangeException("The specified rotor number is invalid");
            }
        }

        private int CharToAlphabetIndex(char ch)
        {
            return char.ToUpper(ch) - 65;
        }
        private char AlphabetIndexToChar(int i)
        {
            return (char)(i + 65);
        }

        public delegate void RotorsChangedHandler(Tuple<int, int, int> newRotorPositions);
        public event RotorsChangedHandler RotorsChanged;
    }
}
