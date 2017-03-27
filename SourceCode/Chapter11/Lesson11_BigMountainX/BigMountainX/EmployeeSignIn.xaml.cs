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
    public sealed partial class EmployeeSignIn : Page
    {
        Type _target_type;
        public EmployeeSignIn()
        {
            this.InitializeComponent();
        }

        private void OnSignInRequested(object sender, RoutedEventArgs e)
        {
            if (txt_employeecode.Password == "employee")
                Frame.Navigate(_target_type);

            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _target_type = e.Parameter as Type;
            base.OnNavigatedTo(e);
        }
    }
}
