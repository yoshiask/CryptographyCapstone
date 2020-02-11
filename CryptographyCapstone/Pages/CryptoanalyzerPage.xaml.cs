using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CryptographyCapstone.Lib;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptographyCapstone.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CryptoanalyzerPage : Page
    {

        public CryptoanalyzerPage()
        {
            this.InitializeComponent();

            var _enumval = Enum.GetValues(typeof(Cryptoanalyzer.EncryptionMethods)).Cast<Cryptoanalyzer.EncryptionMethods>();
            EncryptionMethodBox.ItemsSource = _enumval.ToList();
        }

        private void LetterFreqAnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            string cipherText = LetterFreqInputBox.Text;
            OutputBox.Text = "";
            LetterFreqOutputBox.Items.Clear();
            foreach (KeyValuePair<char, int> freq in Lib.Common.GetFrequencies(cipherText, true, true).Reverse())
            {
                string letter = freq.Key.ToString();
                if (letter == "\n" || letter == "\r")
                {
                    letter = "↩";
                }
                else if (letter == " ")
                {
                    letter = "⎵";
                }
                else if (letter == "\t")
                {
                    letter = "↦";
                }

                LetterFreqOutputBox.Items.Add(new ListViewItem()
                {
                    Content = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new TextBlock()
                            {
                                Text = letter,
                                FontWeight = FontWeights.Bold
                            },
                            new TextBlock()
                            {
                                Text = " : " + freq.Value.ToString()
                            },
                        }
                    }
                });
            }
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            string plainText = InputBox.Text;
            string rawKey = KeyBox.Text;
            string cipherText = "";
            switch ((Cryptoanalyzer.EncryptionMethods)EncryptionMethodBox.SelectedIndex)
            {
                case Cryptoanalyzer.EncryptionMethods.Shift:
                    cipherText = Cryptoanalyzer.EncryptShiftCipher(plainText, Int32.Parse(rawKey));
                    break;

                case Cryptoanalyzer.EncryptionMethods.OneTimePad:
                    cipherText = Cryptoanalyzer.EncryptOneTimePad(plainText, rawKey);
                    break;

                case Cryptoanalyzer.EncryptionMethods.Affine:
                    string[] keys = rawKey.Split(',', ';', ':', ' ');
                    cipherText = Cryptoanalyzer.EncryptAffineCipher(plainText,
                        Int32.Parse(keys[0]), Int32.Parse(keys[1]));
                    break;

                case Cryptoanalyzer.EncryptionMethods.Polynomial:
                    var polynomials = PolynomialCipher.EncodeMessage(InputBox.Text, Int32.Parse(rawKey));
                    foreach (string output in polynomials)
                    {
                        cipherText += output + "\r\n";
                    }
                    break;

                case Cryptoanalyzer.EncryptionMethods.Multiplicative:
                    cipherText = Cryptoanalyzer.EncryptMultiplicativeCipher(plainText, Int32.Parse(rawKey));
                    break;
            }
            OutputBox.Text = cipherText;
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string cipherText = InputBox.Text;
            string rawKey = KeyBox.Text;
            string plainText = "";
            switch ((Cryptoanalyzer.EncryptionMethods)EncryptionMethodBox.SelectedIndex)
            {
                case Cryptoanalyzer.EncryptionMethods.Shift:
                    plainText = Cryptoanalyzer.DecryptShiftCipher(cipherText, Int32.Parse(rawKey));
                    break;

                case Cryptoanalyzer.EncryptionMethods.OneTimePad:
                    plainText = Cryptoanalyzer.DecryptOneTimePad(cipherText, rawKey);
                    break;

                case Cryptoanalyzer.EncryptionMethods.Affine:
                    string[] keys = rawKey.Split(',', ';', ':', ' ');
                    try
                    {
                        plainText = Cryptoanalyzer.DecryptAffineCipher(cipherText,
                            Int32.Parse(keys[0]), Int32.Parse(keys[1]));
                    }
                    catch (ArgumentException ex)
                    {
                        await new ContentDialog()
                        {
                            Content = "Failed to decrypt message, check key: " + ex.Message,
                            Title = "Error",
                            PrimaryButtonText = "OK"
                        }.ShowAsync();
                    }
                    break;

                case Cryptoanalyzer.EncryptionMethods.Polynomial:
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
                        plainText += PolynomialCipher.DecodePolynomial(polyCoeffs, Int32.Parse(rawKey)) + "\r\n";
                    }
                    break;

                case Cryptoanalyzer.EncryptionMethods.Multiplicative:
                    plainText = Cryptoanalyzer.DecryptMultiplicativeCipher(cipherText, Int32.Parse(rawKey));
                    break;
            }
            OutputBox.Text = plainText;
        }

        private void EncryptionSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            InputView.IsPaneOpen = EncryptionSettingsButton.IsChecked.Value;
        }
    }
}
