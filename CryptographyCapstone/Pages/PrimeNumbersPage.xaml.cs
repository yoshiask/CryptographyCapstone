using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class PrimeNumbersPage : Page
    {
        private string Step1InputA { get; set; } = "0";
        private string Step1InputB { get; set; } = "0";

        public PrimeNumbersPage()
        {
            this.InitializeComponent();
        }

        private void NumberInput_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private async void Step1GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Step1InputA) || String.IsNullOrEmpty(Step1InputB))
                return;

            switch (((ComboBoxItem)OperationSelector.SelectedItem).Content)
            {
                case "+":
                    Step1Output.Text = (Int64.Parse(Step1InputA) + Int64.Parse(Step1InputB)).ToString();
                    break;

                case "*":
                    Step1Output.Text = (Int64.Parse(Step1InputA) * Int64.Parse(Step1InputB)).ToString();
                    break;

                case "%":
                    long denom = Int64.Parse(Step1InputB);
                    if (denom <= 0)
                        break;
                    Step1Output.Text = (Int64.Parse(Step1InputA) % denom).ToString();
                    break;
            }

            return;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                PrimesBox.Items.Clear();
                foreach (int i in Common.GeneratePrimesNaive(100))
                {
                    PrimesBox.Items.Add(i);
                }
            });
        }
    }
}
