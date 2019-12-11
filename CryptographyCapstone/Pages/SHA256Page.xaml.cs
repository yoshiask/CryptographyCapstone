using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
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
    public sealed partial class SHA256Page : Page
    {
        public SHA256Page()
        {
            this.InitializeComponent();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashValue = sha256.ComputeHash(
                    System.Text.Encoding.Unicode.GetBytes(InputBox.Text)
                );

                HexOutputBox.Text = BytesToString(hashValue, 16);
                DecOutputBox.Text = BytesToString(hashValue, 10);
                BinOutputBox.Text = BytesToString(hashValue, 2);
                ASCIIOutputBox.Text = System.Text.Encoding.Unicode.GetString(hashValue);
            }
        }

        private string BytesToString(byte[] buffer, int Base)
        {
            string output = "";
            foreach (byte b in buffer)
            {
                output += Convert.ToString(b, Base).PadLeft(2, '0');
            }
            return output;
        }
    }
}
