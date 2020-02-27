using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
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
            GuessEncryptionMethodBox.ItemsSource = _enumval.ToList();
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

                case Cryptoanalyzer.EncryptionMethods.PolynomialBlock:
                    cipherText = Cryptoanalyzer.EncryptPolynomialBlockCipher(InputBox.Text, Int32.Parse(rawKey));
                    break;

                case Cryptoanalyzer.EncryptionMethods.Bifid:
                    cipherText = Cryptoanalyzer.EncryptBifidCipher(plainText, rawKey);
                    break;

                case Cryptoanalyzer.EncryptionMethods.Atbash:
                    cipherText = Cryptoanalyzer.DecryptAtbashCipher(plainText);
                    break;

                case Cryptoanalyzer.EncryptionMethods.MorseCode:
                    string[] morseCodeChars = rawKey.Split(',', ';', ':', ' ');
                    if (morseCodeChars.Length != 3)
                        cipherText = Cryptoanalyzer.EncryptMorseCode(plainText);
                    else
                        cipherText = Cryptoanalyzer.EncryptMorseCode(plainText, morseCodeChars[0], morseCodeChars[1], morseCodeChars[2]);
                    break;

                case Cryptoanalyzer.EncryptionMethods.Ascii:
                    cipherText = Cryptoanalyzer.EncryptAsciiEncoding(plainText);
                    break;
            }
            OutputBox.Text = cipherText;
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string cipherText = InputBox.Text;
            string rawKey = KeyBox.Text;
            string plainText = "";

            List<List<double>> coeffs;
            string[] inputPolynomials;

            switch ((Cryptoanalyzer.EncryptionMethods)EncryptionMethodBox.SelectedIndex)
            {
                #region Shift
                case Cryptoanalyzer.EncryptionMethods.Shift:
                    plainText = Cryptoanalyzer.DecryptShiftCipher(cipherText, Int32.Parse(rawKey));
                    break;
                #endregion

                #region OTP
                case Cryptoanalyzer.EncryptionMethods.OneTimePad:
                    plainText = Cryptoanalyzer.DecryptOneTimePad(cipherText, rawKey);
                    break;
                #endregion

                #region Affine
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
                #endregion

                #region Polynomial
                case Cryptoanalyzer.EncryptionMethods.Polynomial:
                    // Parse the polynomials and decode them
                    coeffs = new List<List<double>>();
                    inputPolynomials = InputBox.Text.Split('\r', '\n', StringSplitOptions.RemoveEmptyEntries);
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
                #endregion

                #region Multiplicative
                case Cryptoanalyzer.EncryptionMethods.Multiplicative:
                    plainText = Cryptoanalyzer.DecryptMultiplicativeCipher(cipherText, Int32.Parse(rawKey));
                    break;
                #endregion

                #region Atbash
                case Cryptoanalyzer.EncryptionMethods.Atbash:
                    plainText = Cryptoanalyzer.DecryptAtbashCipher(cipherText);
                    break;
                #endregion

                #region Morse Code
                case Cryptoanalyzer.EncryptionMethods.MorseCode:
                    string[] morseCodeChars = rawKey.Split(',', ';', ':', ' ');
                    if (morseCodeChars.Length != 2)
                        plainText = Cryptoanalyzer.DecryptMorseCode(cipherText);
                    else
                        plainText = Cryptoanalyzer.DecryptMorseCode(cipherText, morseCodeChars[0], morseCodeChars[1], morseCodeChars[2]);
                    break;
                #endregion

                #region Polynomial Block
                case Cryptoanalyzer.EncryptionMethods.PolynomialBlock:
                    // Parse the polynomials and decode them
                    List<List<double>> blockCoeffs = new List<List<double>>();
                    string[] blockInputPolynomials = InputBox.Text.Split('\r', '\n', StringSplitOptions.RemoveEmptyEntries);
                    foreach (string inputPoly in blockInputPolynomials)
                    {
                        List<double> polyCoeffs = new List<double>();
                        foreach (string inputCoeff in inputPoly.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        {
                            polyCoeffs.Add(Double.Parse(inputCoeff));
                        }
                        blockCoeffs.Add(polyCoeffs);
                    }
                    plainText = Cryptoanalyzer.DecryptPolynomialBlockCipher(blockCoeffs, Int32.Parse(rawKey));
                    break;
                    #endregion

                #region ASCII Encoding
                case Cryptoanalyzer.EncryptionMethods.Ascii:
                    plainText = Cryptoanalyzer.DecryptAsciiEncoding(cipherText);
                    break;
                #endregion
            }
            OutputBox.Text = plainText;
        }

        private async void GuessButton_Click(object sender, RoutedEventArgs e)
        {
            string cipherText = CiphertextInputBox.Text;
            string plainText = "";
            GuessOutputBox.Items.Clear();
            switch ((Cryptoanalyzer.EncryptionMethods)GuessEncryptionMethodBox.SelectedIndex)
            {
                case Cryptoanalyzer.EncryptionMethods.Shift:
                    var guesses = Cryptoanalyzer.GuessShiftCipher(cipherText);
                    foreach (var guess in await Cryptoanalyzer.RankCandidatePlaintext(guesses.Values))
                    {
                        GuessOutputBox.Items.Add(guess.Key);
                    }
                    break;

                case Cryptoanalyzer.EncryptionMethods.OneTimePad:

                    break;

                case Cryptoanalyzer.EncryptionMethods.Affine:
                    foreach (var guess in Cryptoanalyzer.GuessAffineCipher(cipherText))
                    {
                        GuessOutputBox.Items.Add(guess.Value);
                    }
                    break;

                case Cryptoanalyzer.EncryptionMethods.Multiplicative:
                    foreach (var guess in Cryptoanalyzer.GuessMultiplicativeCipher(cipherText))
                    {
                        GuessOutputBox.Items.Add(guess.Value);
                    }
                    break;

                case Cryptoanalyzer.EncryptionMethods.Atbash:
                    GuessOutputBox.Items.Add(Cryptoanalyzer.DecryptAtbashCipher(cipherText));
                    break;
            }
            //GuessOutputBox.Text = plainText;
        }

        private void EncryptionSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            InputView.IsPaneOpen = EncryptionSettingsButton.IsChecked.Value;
        }

        private void GuessSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            GuessInputView.IsPaneOpen = GuessSettingsButton.IsChecked.Value;
        }

        private void GuessOutputBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                dataPackage.SetText(GuessOutputBox.SelectedValue.ToString());
                Clipboard.SetContent(dataPackage);
            }
            catch {}
        }
    }
}
