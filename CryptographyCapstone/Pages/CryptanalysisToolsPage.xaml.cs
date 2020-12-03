using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class CryptanalysisToolsPage : Page
    {
        public CryptanalysisToolsPage()
        {
            this.InitializeComponent();
        }

        private void SubtractButton_Click(object sender, RoutedEventArgs e)
        {
            string cipherText = SubtractCiphertextBox.Text;
            string plainText = SubtractPlaintextBox.Text;
            string key = "";
            for (int i = 0; i < cipherText.Length; i++)
            {
                int newChar = Lib.Common.CharToAlphabetIndex(cipherText[i]) - Lib.Common.CharToAlphabetIndex(plainText[i % plainText.Length]);
                key += Lib.Common.AlphabetIndexToChar(newChar);
            }
            SubtractKeyBox.Text = key;
        }
    }
}
