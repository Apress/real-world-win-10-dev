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
    public sealed partial class ViewEventPage : Page
    {
        BMXEvent CurrentEvent { get; set; }
        List<BMXEvent> Events { get; set; }

        public ViewEventPage(List<BMXEvent> events, BMXEvent current_event)
        {
            this.InitializeComponent();

            Events = events;
            CurrentEvent = current_event;
            txt_eventtitle.Text = CurrentEvent.EventTitle;
            txt_eventdescription.Text = CurrentEvent.Description;
            txt_address.Text = CurrentEvent.Address;
            control_calendar.Date = CurrentEvent.StartDateTime;
            slider_duration.Value = CurrentEvent.Duration.Hours;
        }

        private void CompletedEvent(object sender, RoutedEventArgs e)
        {
            Window.Current.Content = new ListEventsPage(Events);
        }
    }
}
