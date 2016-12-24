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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BigMountainX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PatronCounter : Page
    {
        BMXOccasion _occassion;
        BMXEvent _event;
        public PatronCounter()
        {
            this.InitializeComponent();
            this.Loaded += PatronCounter_Loaded;
            _event = new BMXEvent();
            _occassion = new BMXOccasion(_event);

            btn_add.Click += Btn_add_Click;
        }


        private void PatronCounter_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayAttendants();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void UseProgrammaticallyCreatedTextblocl()
        {
            TextBlock txt_attendants_prog = new TextBlock();
            txt_attendants_prog.HorizontalAlignment = HorizontalAlignment.Center;
            txt_attendants_prog.Text = "0 attendants";
            txt_attendants_prog.FontFamily = new FontFamily("Segoe UI");
            txt_attendants_prog.FontSize = 42;
            txt_attendants_prog.FontWeight = Windows.UI.Text.FontWeights.ExtraLight;
        }

        private void DisplayAttendants()
        {
            txt_attendants.DataContext = null;
            txt_attendants.DataContext = _occassion;
        }

        private void Btn_add_Click(object sender, RoutedEventArgs e)
        {
            _occassion.Enter();
            DisplayAttendants();
        }

        private void RemoveAttendant(object sender, RoutedEventArgs e)
        {
            _occassion.Leave();
            DisplayAttendants();
        }

        private void ResetToZero(object sender, RoutedEventArgs e)
        {
            _occassion = new BMXOccasion(_event);
            DisplayAttendants();
        }
    }
}
