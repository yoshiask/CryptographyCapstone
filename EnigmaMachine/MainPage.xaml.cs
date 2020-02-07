using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EnigmaMachine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Models.RotorBody RotorBody;
        private Models.Rotor RotorA;
        private Models.Rotor RotorB;
        private Models.Rotor RotorC;
        private Models.Reflector Reflector;
        string textCache;

        public MainPage()
        {
            this.InitializeComponent();

            ResetMachine();
        }

        private void PlainInputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textCache = PlainInputBox.Text;
        }
        
        private void PlainInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            ResetLights();

            if (e.Key == Windows.System.VirtualKey.Back)
            {
                // The user cannot delete text, so undo it for them
                int ogIndex = PlainInputBox.SelectionStart;
                string deletedText = textCache.Substring(ogIndex, 1);
                PlainInputBox.Text = PlainInputBox.Text.Insert(ogIndex, deletedText);
                PlainInputBox.SelectionStart = PlainInputBox.Text.Length;
                return;
            }

            // Set the caret to the end of the string
            PlainInputBox.SelectionStart = PlainInputBox.Text.Length;            

            // Alt-light the plaintext letter for clarity
            switch (e.Key)
            {
                #region Letter Keys
                case Windows.System.VirtualKey.A:
                    SetLightStatus(LightA, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.B:
                    SetLightStatus(LightB, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.C:
                    SetLightStatus(LightC, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.D:
                    SetLightStatus(LightD, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.E:
                    SetLightStatus(LightE, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.F:
                    SetLightStatus(LightF, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.G:
                    SetLightStatus(LightG, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.H:
                    SetLightStatus(LightH, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.I:
                    SetLightStatus(LightI, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.J:
                    SetLightStatus(LightJ, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.K:
                    SetLightStatus(LightK, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.L:
                    SetLightStatus(LightL, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.M:
                    SetLightStatus(LightM, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.N:
                    SetLightStatus(LightN, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.O:
                    SetLightStatus(LightO, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.P:
                    SetLightStatus(LightP, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.Q:
                    SetLightStatus(LightQ, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.R:
                    SetLightStatus(LightR, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.S:
                    SetLightStatus(LightS, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.T:
                    SetLightStatus(LightT, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.U:
                    SetLightStatus(LightU, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.V:
                    SetLightStatus(LightV, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.W:
                    SetLightStatus(LightW, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.X:
                    SetLightStatus(LightX, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.Y:
                    SetLightStatus(LightY, LightStatus.LitAlt);
                    break;

                case Windows.System.VirtualKey.Z:
                    SetLightStatus(LightZ, LightStatus.LitAlt);
                    break;

                #endregion

                default:
                    return;
            }

            // Encrypt the new letter
            char currentChar = e.Key.ToString()[0];
            //currentNo = plugBoard.runThrough(currentNo);
            currentChar = (char)RotorC.RunThrough(currentChar, true);
            currentChar = (char)RotorB.RunThrough(currentChar, true);
            currentChar = (char)RotorA.RunThrough(currentChar, true);
            currentChar = (char)Reflector.RunThrough(currentChar, true);
            currentChar = (char)RotorA.RunThrough(currentChar, false);
            currentChar = (char)RotorB.RunThrough(currentChar, false);
            currentChar = (char)RotorC.RunThrough(currentChar, false);
            /*currentChar = (char)SendThroughRotor(currentChar, RotorCPos, RotorCWiring, true);
            currentChar = (char)SendThroughRotor(currentChar, RotorBPos, RotorBWiring, true);
            currentChar = (char)SendThroughRotor(currentChar, RotorAPos, RotorAWiring, true);
            currentChar = (char)SendThroughReflector(currentChar, RotorUKWb, true);
            currentChar = (char)SendThroughRotor(currentChar, RotorAPos, RotorAWiring, false);
            currentChar = (char)SendThroughRotor(currentChar, RotorBPos, RotorBWiring, false);
            currentChar = (char)SendThroughRotor(currentChar, RotorCPos, RotorCWiring, false);
            char encChar = RotorBody.Encrypt(e.Key.ToString());*/
            CipherOutputBox.Text += currentChar;

            // Light the encrypted letter
            switch (currentChar)
            {
                #region Letters
                case 'A':
                    SetLightStatus(LightA, LightStatus.Lit);
                    break;

                case 'B':
                    SetLightStatus(LightB, LightStatus.Lit);
                    break;

                case 'C':
                    SetLightStatus(LightC, LightStatus.Lit);
                    break;

                case 'D':
                    SetLightStatus(LightD, LightStatus.Lit);
                    break;

                case 'E':
                    SetLightStatus(LightE, LightStatus.Lit);
                    break;

                case 'F':
                    SetLightStatus(LightF, LightStatus.Lit);
                    break;

                case 'G':
                    SetLightStatus(LightG, LightStatus.Lit);
                    break;

                case 'H':
                    SetLightStatus(LightH, LightStatus.Lit);
                    break;

                case 'I':
                    SetLightStatus(LightI, LightStatus.Lit);
                    break;

                case 'J':
                    SetLightStatus(LightJ, LightStatus.Lit);
                    break;

                case 'K':
                    SetLightStatus(LightK, LightStatus.Lit);
                    break;

                case 'L':
                    SetLightStatus(LightL, LightStatus.Lit);
                    break;

                case 'M':
                    SetLightStatus(LightM, LightStatus.Lit);
                    break;

                case 'N':
                    SetLightStatus(LightN, LightStatus.Lit);
                    break;

                case 'O':
                    SetLightStatus(LightO, LightStatus.Lit);
                    break;

                case 'P':
                    SetLightStatus(LightP, LightStatus.Lit);
                    break;

                case 'Q':
                    SetLightStatus(LightQ, LightStatus.Lit);
                    break;

                case 'R':
                    SetLightStatus(LightR, LightStatus.Lit);
                    break;

                case 'S':
                    SetLightStatus(LightS, LightStatus.Lit);
                    break;

                case 'T':
                    SetLightStatus(LightT, LightStatus.Lit);
                    break;

                case 'U':
                    SetLightStatus(LightU, LightStatus.Lit);
                    break;

                case 'V':
                    SetLightStatus(LightV, LightStatus.Lit);
                    break;

                case 'W':
                    SetLightStatus(LightW, LightStatus.Lit);
                    break;

                case 'X':
                    SetLightStatus(LightX, LightStatus.Lit);
                    break;

                case 'Y':
                    SetLightStatus(LightY, LightStatus.Lit);
                    break;

                case 'Z':
                    SetLightStatus(LightZ, LightStatus.Lit);
                    break;

                #endregion

                default:
                    return;
            }
        }

        public SolidColorBrush GetThemeResource(string name)
        {
            return Application.Current.Resources[name] as SolidColorBrush;
        }

        private void SetLightStatus(Grid light, LightStatus status)
        {
            var unlitBrush = "UnlitBackgroundColor";
            var unlitForeBrush = "UnlitForegroundColor";

            var litBrush = "LitBackgroundColor";
            var litForeBrush = "LitForegroundColor";

            var litAltBrush = "LitAltBackgroundColor";


            switch (status)
            {
                case LightStatus.Unlit:
                    light.Background = GetThemeResource(unlitBrush);
                    (light.Children[0] as TextBlock).Foreground = GetThemeResource(unlitForeBrush);
                    break;

                case LightStatus.Lit:
                    light.Background = GetThemeResource(litBrush);
                    (light.Children[0] as TextBlock).Foreground = GetThemeResource(litForeBrush);
                    break;

                case LightStatus.LitAlt:
                    light.Background = GetThemeResource(litAltBrush);
                    (light.Children[0] as TextBlock).Foreground = GetThemeResource(unlitForeBrush);
                    break;
            }
        }

        private void ResetLights()
        {
            // Turn off all lights
            foreach (StackPanel row in LightboardLayout.Children)
            {
                foreach (Grid light in row.Children)
                {
                    SetLightStatus(light, LightStatus.Unlit);
                }
            }
        }

        enum LightStatus
        {
            Unlit,
            Lit,
            LitAlt
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetMachine();
        }

        private void ResetMachine()
        {
            try
            {
                //RotorBody.RotorsChanged -= RotorBody_RotorsChanged;
            }
            catch { }

            //RotorBody = new Models.RotorBody();
            //RotorBody.RotorsChanged += RotorBody_RotorsChanged;
            RotorA = Models.Rotor
            PlainInputBox.Text = "";
            CipherOutputBox.Text = "";
            textCache = "";
            ResetLights();
        }

        private void RotorBody_RotorsChanged(Tuple<int, int, int> newRotorPositions)
        {
            RotaryA.Value = newRotorPositions.Item1;
            RotaryB.Value = newRotorPositions.Item2;
            RotaryC.Value = newRotorPositions.Item3;
        }
    }
}
