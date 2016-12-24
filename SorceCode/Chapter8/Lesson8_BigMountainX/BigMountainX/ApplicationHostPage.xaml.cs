using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
    public sealed partial class ApplicationHostPage : Page
    {
        Geolocator _geolocator;
        public Geoposition CurrentLocation;

        public event Action<Geopoint> GeoLocationChanged;
        public static ApplicationHostPage Host { get; private set; }
        Type _last_page_before_nav_lost = typeof(LandingPage);
        public event Action<int> AttendaceChanged;

        public ApplicationHostPage()
        {
            this.InitializeComponent();
            this.Loaded += ApplicationHostPage_Loaded;
            Host = this;
            _geolocator = new Geolocator();
            _geolocator.StatusChanged += _geolocator_StatusChanged;
            _geolocator.PositionChanged += _geolocator_PositionChanged;
            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (root_frame.CanGoBack)
            {
                e.Handled = true;
                root_frame.GoBack();
            }
            else
                e.Handled = false;
        }

        private void _geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            CurrentLocation = args.Position;
            GeoLocationChanged?.Invoke(CurrentLocation.Coordinate.Point);
        }

        private void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var change_reports = sender.ReadReports();
            foreach (var change_report in change_reports)
            {
                var new_state = change_report.NewState;
                if (new_state == GeofenceState.Entered)
                {
                    AttendaceChanged?.Invoke(1);
                }
                else if (new_state == GeofenceState.Exited)
                {
                    AttendaceChanged?.Invoke(-1);
                }
            }
        }

        async private void _geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
             {
                 var current_view = ApplicationView.GetForCurrentView();
                 var status = args.Status;
                 switch (status)
                 {
                     case PositionStatus.Disabled:
                         _last_page_before_nav_lost = root_frame.CurrentSourcePageType;
                         root_frame.Navigate(typeof(NoLocationPage));
                         break;
                     case PositionStatus.Ready:
                         current_view.TitleBar.BackgroundColor = Colors.Transparent;
                         root_frame.Navigate(_last_page_before_nav_lost);
                         CurrentLocation = await _geolocator.GetGeopositionAsync();
                         GeoLocationChanged?.Invoke(CurrentLocation.Coordinate.Point);
                         break;
                     case PositionStatus.Initializing:
                         current_view.TitleBar.BackgroundColor = Colors.Yellow;
                         break;
                     default:
                         current_view.TitleBar.BackgroundColor = Colors.Red;
                         break;
                 }
             });

        }

        async private void ApplicationHostPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.State.UserProfile == null)
                root_frame.Navigate(typeof(UserIntroPage));
            else
            {
                var access_status = await Geolocator.RequestAccessAsync();
                switch (access_status)
                {
                    case GeolocationAccessStatus.Allowed:
                        root_frame.Navigate(typeof(LandingPage));
                        CurrentLocation = await _geolocator.GetGeopositionAsync();
                        GeoLocationChanged?.Invoke(CurrentLocation.Coordinate.Point);
                        var fence_id = App.State.NextEvent.EventID.ToString();
                        var fences = GeofenceMonitor.Current.Geofences;
                        var fence = fences.Where(i => i.Id == fence_id).FirstOrDefault();
                        if (fence == null)
                        {
                            Geocircle event_radius = new Geocircle(new BasicGeoposition
                            {
                                Latitude = App.State.NextEvent.Latitude.Value,
                                Longitude = App.State.NextEvent.Longitude.Value,
                            }, 100);

                            fence = new Geofence(fence_id, event_radius);

                            //add a geofence 
                            GeofenceMonitor.Current.Geofences.Add(fence);

                        }

                        break;
                    case GeolocationAccessStatus.Denied:
                        root_frame.Navigate(typeof(NoLocationPage));
                        break;
                    case GeolocationAccessStatus.Unspecified:
                        break;
                }

            }
        }
    }
}
