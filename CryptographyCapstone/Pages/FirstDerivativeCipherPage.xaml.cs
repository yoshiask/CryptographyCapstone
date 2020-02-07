using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CryptographyCapstone.Lib;
using Extreme.Mathematics.Curves;
using Microsoft.UI.Xaml.Controls;
using RefreshStateChangedEventArgs = Microsoft.UI.Xaml.Controls.RefreshStateChangedEventArgs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptographyCapstone.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstDerivativeCipherPage : Page
    {
        public FirstDerivativeCipherPage()
        {
            this.InitializeComponent();
        }

        private void UpdateCipherText(bool decrypt)
        {
            if (!IsLoaded)
                return;

            PolynomialOutputBox.Text = "";

            if (decrypt)
            {
                // Parse the polynomials and decode them
                List<List<double>> coeffs = new List<List<double>>();
                string[] inputPolynomials = InputBox.Text.Split('\r', '\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (string inputPoly in inputPolynomials)
                {
                    List<double> polyCoeffs = new List<double>();
                    foreach (string inputCoeff in inputPoly.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        polyCoeffs.Add(Double.Parse(inputCoeff));
                    }

                    polyCoeffs.Reverse();
                    
                    // Send the coefficients to be decoded
                    PolynomialOutputBox.Text += FirstDerivativeCipher.DecodePolynomial(polyCoeffs, (int)KeyBox.Value) + "\r\n";
                }
            }
            else
            {
                var polynomials = FirstDerivativeCipher.EncodeMessage(InputBox.Text, (int)KeyBox.Value);
                foreach (string output in polynomials)
                {
                    PolynomialOutputBox.Text += output + "\r\n";
                }
            }

            return;
            // Don't do this:
            // Generate one gigantic polynomial for the entire message
            Debug.WriteLine("Encryption started: " + DateTime.Now);
            PolynomialOutputBox.Text +=
                FirstDerivativeCipher.CoeffsToPolynomial(FirstDerivativeCipher.EncryptString(InputBox.Text, 1));
            Debug.WriteLine("Encryption Finished: " + DateTime.Now);
        }

        private void DecryptButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UpdateCipherText(true);
        }

        private void EncryptButton_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateCipherText(false);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await new ContentDialog()
            {
                Content = "DO NOT USE THIS!!! It is effectively only a shift cipher.",
                Title = "Warning",
                PrimaryButtonText = "OK"
            }.ShowAsync();
        }
    }

    public class FirstDerivativeCipher
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
