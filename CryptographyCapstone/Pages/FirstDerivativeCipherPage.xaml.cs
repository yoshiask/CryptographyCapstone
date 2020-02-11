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
                    PolynomialOutputBox.Text += PolynomialCipher.DecodePolynomial(polyCoeffs, (int)KeyBox.Value) + "\r\n";
                }
            }
            else
            {
                var polynomials = PolynomialCipher.EncodeMessage(InputBox.Text, (int)KeyBox.Value);
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
                PolynomialCipher.CoeffsToPolynomial(PolynomialCipher.EncryptString(InputBox.Text, 1));
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
}
