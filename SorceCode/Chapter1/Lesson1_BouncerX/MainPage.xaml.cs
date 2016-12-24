using System;
using System.Collections.Generic;
using System.ComponentModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Lesson1_BouncerX
{

    public class OccassionConverter : IValueConverter
    {
        /// <summary>
        /// For when converting the databound object into a 
        /// value that can be displayed
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value,
            Type targetType,
            object parameter,
            string language)
        {
            if (value is Occasion)
            {
                var occassion = value as Occasion;
                var ui_command = parameter as string;
                if (ui_command == "number of attendants")
                    return $"{occassion.AttendanceCount} attendants";
            }
            return "Unknown number of attendants";
        }

        /// <summary>
        /// For when saving binding information back to 
        /// the databound object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }

    public class Occasion
    {
        public int AttendanceCount { get; private set; }

        public void Enter()
        {
            AttendanceCount++;
        }

        public void Leave()
        {
            if (AttendanceCount > 0)
                AttendanceCount--;
        }

    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Occasion _occassion;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            btn_add.Click += Btn_add_Click;
            _occassion = new Occasion();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayAttendants();
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
            _occassion = new Occasion();
            DisplayAttendants();
        }
    }
}
