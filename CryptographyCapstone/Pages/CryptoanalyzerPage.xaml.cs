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
            var encryptionMethods = _enumval.ToList();
            EncryptionMethodBox.ItemsSource = encryptionMethods;
            GuessEncryptionMethodBox.ItemsSource = encryptionMethods;
        }

        private void LetterFreqAnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            string cipherText = LetterFreqInputBox.Text;
            OutputBox.Text = "";
            LetterFreqOutputBox.Items.Clear();
            foreach (var (c, freq) in Lib.Common.GetFrequencies(cipherText, true, true).Reverse())
            {
                string letter = c.ToString();
                switch (letter)
                {
                    case "\n":
                    case "\r":
                        letter = "↩";
                        break;
                    case " ":
                        letter = "⎵";
                        break;
                    case "\t":
                        letter = "↦";
                        break;
                }

                LetterFreqOutputBox.Items.Add(new ListViewItem
                {
                    Content = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = letter,
                                FontWeight = FontWeights.Bold
                            },
                            new TextBlock
                            {
                                Text = " : " + freq
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
                    cipherText = Cryptoanalyzer.EncryptShiftCipher(plainText, int.Parse(rawKey));
                    break;

                case Cryptoanalyzer.EncryptionMethods.OneTimePad:
                    cipherText = Cryptoanalyzer.EncryptOneTimePad(plainText, rawKey);
                    break;

                case Cryptoanalyzer.EncryptionMethods.Affine:
                    string[] keys = rawKey.Split(',', ';', ':', ' ');
                    cipherText = Cryptoanalyzer.EncryptAffineCipher(plainText,
                        int.Parse(keys[0]), int.Parse(keys[1]));
                    break;

                case Cryptoanalyzer.EncryptionMethods.Polynomial:
                    var polynomials = PolynomialCipher.EncodeMessage(InputBox.Text, int.Parse(rawKey));
                    cipherText = string.Join("\r\n", polynomials);
                    break;

                case Cryptoanalyzer.EncryptionMethods.Multiplicative:
                    cipherText = Cryptoanalyzer.EncryptMultiplicativeCipher(plainText, int.Parse(rawKey));
                    break;

                case Cryptoanalyzer.EncryptionMethods.PolynomialBlock:
                    cipherText = Cryptoanalyzer.EncryptPolynomialBlockCipher(InputBox.Text, int.Parse(rawKey));
                    break;

                case Cryptoanalyzer.EncryptionMethods.Bifid:
                    cipherText = Cryptoanalyzer.EncryptBifidCipher(plainText, rawKey);
                    break;

                case Cryptoanalyzer.EncryptionMethods.Atbash:
                    cipherText = Cryptoanalyzer.DecryptAtbashCipher(plainText);
                    break;

                case Cryptoanalyzer.EncryptionMethods.MorseCode:
                    string[] morseCodeChars = rawKey.Split(',', ';', ':', ' ');
                    cipherText = morseCodeChars.Length != 3
                        ? Cryptoanalyzer.EncryptMorseCode(plainText)
                        : Cryptoanalyzer.EncryptMorseCode(plainText, morseCodeChars[0], morseCodeChars[1], morseCodeChars[2]);
                    break;

                case Cryptoanalyzer.EncryptionMethods.Ascii:
                    cipherText = Cryptoanalyzer.EncryptAsciiEncoding(plainText, int.Parse(rawKey));
                    break;

                case Cryptoanalyzer.EncryptionMethods.Vigenere:
                    cipherText = Cryptoanalyzer.EncryptVigenereCipher(plainText, rawKey);
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
                    plainText = Cryptoanalyzer.DecryptShiftCipher(cipherText, int.Parse(rawKey));
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
                            int.Parse(keys[0]), int.Parse(keys[1]));
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
                            polyCoeffs.Add(double.Parse(inputCoeff));
                        }

                        polyCoeffs.Reverse();
                    
                        // Send the coefficients to be decoded
                        plainText += PolynomialCipher.DecodePolynomial(polyCoeffs, int.Parse(rawKey)) + "\r\n";
                    }
                    break;
                #endregion

                #region Multiplicative
                case Cryptoanalyzer.EncryptionMethods.Multiplicative:
                    plainText = Cryptoanalyzer.DecryptMultiplicativeCipher(cipherText, int.Parse(rawKey));
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
                    plainText = morseCodeChars.Length != 2
                        ? Cryptoanalyzer.DecryptMorseCode(cipherText)
                        : Cryptoanalyzer.DecryptMorseCode(cipherText, morseCodeChars[0], morseCodeChars[1], morseCodeChars[2]);
                    break;
                #endregion

                #region Polynomial Block
                case Cryptoanalyzer.EncryptionMethods.PolynomialBlock:
                    // Parse the polynomials and decode them
                    string[] blockInputPolynomials = InputBox.Text.Split('\r', '\n', StringSplitOptions.RemoveEmptyEntries);
                    List<List<double>> blockCoeffs = blockInputPolynomials.Select(PolynomialCipher.ParsePolynomial).ToList();
                    plainText = Cryptoanalyzer.DecryptPolynomialBlockCipher(blockCoeffs, int.Parse(rawKey));
                    break;
                    #endregion

                #region ASCII Encoding
                case Cryptoanalyzer.EncryptionMethods.Ascii:
                    plainText = Cryptoanalyzer.DecryptAsciiEncoding(cipherText, int.Parse(rawKey));
                    break;
                #endregion

                #region Vigenere
                case Cryptoanalyzer.EncryptionMethods.Vigenere:
                    plainText = Cryptoanalyzer.DecryptVigenereCipher(cipherText, rawKey);
                    break;
                #endregion
            }
            OutputBox.Text = plainText;
        }

        private async void GuessButton_Click(object sender, RoutedEventArgs e)
        {
            string cipherText = CiphertextInputBox.Text;
            string plainText = "";
            GuessOutputBox.Items?.Clear();
            switch ((Cryptoanalyzer.EncryptionMethods)GuessEncryptionMethodBox.SelectedIndex)
            {
                case Cryptoanalyzer.EncryptionMethods.Shift:
                    var guesses = Cryptoanalyzer.GuessShiftCipher(cipherText);
                    foreach (var guess in await Cryptoanalyzer.RankCandidatePlaintext(guesses.Values))
                    {
                        GuessOutputBox.Items?.Add(guess.Key);
                    }
                    break;

                case Cryptoanalyzer.EncryptionMethods.OneTimePad:

                    break;

                case Cryptoanalyzer.EncryptionMethods.Affine:
                    foreach (var guess in Cryptoanalyzer.GuessAffineCipher(cipherText))
                    {
                        GuessOutputBox.Items?.Add(guess.Value);
                    }
                    break;

                case Cryptoanalyzer.EncryptionMethods.Multiplicative:
                    foreach (var guess in Cryptoanalyzer.GuessMultiplicativeCipher(cipherText))
                    {
                        GuessOutputBox.Items?.Add(guess.Value);
                    }
                    break;

                case Cryptoanalyzer.EncryptionMethods.Atbash:
                    GuessOutputBox.Items?.Add(Cryptoanalyzer.DecryptAtbashCipher(cipherText));
                    break;
            }
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
                Clipboard.Flush();
            }
            catch {}
        }
    }
}
