using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace EnigmaMachine.Controls
{
    public sealed partial class RotorOptionsControl : UserControl
    {
        public Models.RotorBody RotorBody {
            get {
                return this.DataContext as Models.RotorBody;
            }
            set {
                DataContext = value;
            }
        }
        public ObservableCollection<string> AvailableRotors = new ObservableCollection<string>();

        public RotorOptionsControl()
        {
            this.InitializeComponent();

            AvailableRotors.Add("1");
            AvailableRotors.Add("2");
            AvailableRotors.Add("3");
            AvailableRotors.Add("4");
            AvailableRotors.Add("5");
        }
    }
}
