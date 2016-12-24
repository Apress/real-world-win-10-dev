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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BigMountainX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageEventsPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        List<BMXEvent> Events { get; set; }

        public ManageEventsPage()
        {
            this.InitializeComponent();
            this.Loaded += ManageEventsPage_Loaded;
        }

        private void ManageEventsPage_Loaded(object sender, RoutedEventArgs e)
        {
            Events = App.State.Events;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Events"));
        }

        private void OnNewEventClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateEventPage), null);
        }

        private void EventSelected(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(CreateEventPage), e.ClickedItem);
        }
    }
}
