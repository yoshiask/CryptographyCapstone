using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptographyCapstone.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CryptoanalyzerPage : Page
    {
        private enum EncryptionMethods
        {
            Shift, Substitution, Polyalphabetic, Vigenere, OneTimePad, Affine, Polynomial
        }

        public CryptoanalyzerPage()
        {
            this.InitializeComponent();

            var _enumval = Enum.GetValues(typeof(EncryptionMethods)).Cast<EncryptionMethods>();
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
            switch ((EncryptionMethods)EncryptionMethodBox.SelectedIndex)
            {
                case EncryptionMethods.Shift:
                    cipherText = Lib.Cryptoanalyzer.EncryptShiftCipher(plainText, Int32.Parse(rawKey));
                    break;

                case EncryptionMethods.OneTimePad:
                    cipherText = Lib.Cryptoanalyzer.EncryptOneTimePad(plainText, rawKey);
                    break;

                case EncryptionMethods.Affine:
                    string[] keys = rawKey.Split(',', ';', ':', ' ');
                    cipherText = Lib.Cryptoanalyzer.EncryptAffine(plainText,
                        Int32.Parse(keys[0]), Int32.Parse(keys[1]));
                    break;

                case EncryptionMethods.Polynomial:
                    var polynomials = Lib.PolynomialCipher.EncodeMessage(InputBox.Text, Int32.Parse(rawKey));
                    foreach (string output in polynomials)
                    {
                        cipherText += output + "\r\n";
                    }
                    break;
            }
            OutputBox.Text = cipherText;
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string cipherText = InputBox.Text;
            string rawKey = KeyBox.Text;
            string plainText = "";
            switch ((EncryptionMethods)EncryptionMethodBox.SelectedIndex)
            {
                case EncryptionMethods.Shift:
                    plainText = Lib.Cryptoanalyzer.DecryptShiftCipher(cipherText, Int32.Parse(rawKey));
                    break;

                case EncryptionMethods.OneTimePad:
                    plainText = Lib.Cryptoanalyzer.DecryptOneTimePad(cipherText, rawKey);
                    break;

                case EncryptionMethods.Affine:
                    string[] keys = rawKey.Split(',', ';', ':', ' ');
                    try
                    {
                        plainText = Lib.Cryptoanalyzer.DecryptAffine(cipherText,
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

                case EncryptionMethods.Polynomial:
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
                        plainText += Lib.PolynomialCipher.DecodePolynomial(polyCoeffs, Int32.Parse(rawKey)) + "\r\n";
                    }
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
