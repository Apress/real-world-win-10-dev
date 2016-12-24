using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel;
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
    public sealed partial class ManageMailingList : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        List<string> MailingList { get; set; }
        public ManageMailingList()
        {
            this.InitializeComponent();
        }

        private void OnImportAllClicked(object sender, RoutedEventArgs e)
        {
            NetTcpBinding tcp_binding_client = new NetTcpBinding(SecurityMode.None);
            EndpointAddress address = new EndpointAddress("net.tcp://localhost:9999/profile-generator");
            ChannelFactory<IProfileGenerator> channel = new ChannelFactory<IProfileGenerator>(tcp_binding_client, address);
            var profile_gnerator = channel.CreateChannel();
            var list = profile_gnerator.GetMailingList("live");
            MailingList = list;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MailingList"));
        }
    }
}
