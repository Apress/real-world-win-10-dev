using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
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
    public sealed partial class LaunchResponsePage : Page
    {
        ProtocolForResultsActivatedEventArgs _args;

        public LaunchResponsePage(ProtocolForResultsActivatedEventArgs args)
        {
            this.InitializeComponent();
            _args = args;
            var request = _args.Data["request"] as string;
            txt_message.Text = $"request is \"{ request}\"";
        }

        private void ResponseClicked(object sender, RoutedEventArgs e)
        {
            ValueSet response_data = new ValueSet();
            response_data.Add("response", txt_page.Text);
            _args.ProtocolForResultsOperation.ReportCompleted(response_data);
        }
    }
}
