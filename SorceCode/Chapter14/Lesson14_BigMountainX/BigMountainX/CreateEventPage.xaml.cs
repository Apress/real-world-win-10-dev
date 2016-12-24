using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
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
    public sealed partial class CreateEventPage : Page
    {
        BMXEvent CurrentEvent;



        public CreateEventPage()
        {
            this.InitializeComponent();
        }

        async private void SaveEvent(object sender, RoutedEventArgs e)
        {
            var default_position = new BasicGeoposition
            {
                Latitude = 40.7484,
                Longitude = -73.9857,
            };
            Geopoint point = new Geopoint(default_position);
            var lat = default_position.Latitude;
            var lon = default_position.Longitude;

            if (CurrentEvent == null)
            {
                CurrentEvent = new BMXEvent
                {
                    EventID = Guid.NewGuid(),
                    EventTitle = txt_eventtitle.Text,
                    Description = txt_eventdescription.Text,
                    Address = txt_address.Text,
                    Longitude = lon,
                    Latitude = lat,
                    CreateDate = DateTime.Now,
                    Duration = TimeSpan.FromHours(slider_duration.Value),
                    StartDateTime = control_calendar.Date.Value.Date.Add(control_time.Time),
                };
            }
            else
            {
                CurrentEvent.EventTitle = txt_eventtitle.Text;
                CurrentEvent.Description = txt_eventdescription.Text;
                CurrentEvent.Address = txt_address.Text;
                CurrentEvent.Longitude = lon;
                CurrentEvent.Latitude = lat;
                CurrentEvent.CreateDate = DateTime.Now;
                CurrentEvent.Duration = TimeSpan.FromHours(slider_duration.Value);
                CurrentEvent.StartDateTime = control_calendar.Date.Value.Date.Add(control_time.Time);
            }

            App.State.Events.Add(CurrentEvent);
            await App.State.SaveAsync();
            Frame.GoBack();
        }

        private void CancelEvent(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                CurrentEvent = e.Parameter as BMXEvent;

                //brute force populate fields
                txt_eventtitle.Text = CurrentEvent.EventTitle;
                txt_eventdescription.Text = CurrentEvent.Description;
                txt_address.Text = CurrentEvent.Address;
                control_calendar.Date = CurrentEvent.StartDateTime;
                slider_duration.Value = CurrentEvent.Duration.Hours;
            }

        }
    }
}
