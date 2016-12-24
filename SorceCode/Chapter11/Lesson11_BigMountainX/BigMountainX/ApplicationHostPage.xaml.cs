using BMX.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Windows.Storage.NameCollisionOption;
using windowmode = Windows.UI.ViewManagement.ApplicationViewWindowingMode;

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

            //set the application titlebar look
            var current_view = ApplicationView.GetForCurrentView();
            var titlebar_color = Color.FromArgb(0xFF, 0x6A, 0x6A, 0x6A);
            current_view.TitleBar.BackgroundColor = titlebar_color; //Colors.DarkGray;
            current_view.TitleBar.InactiveBackgroundColor = titlebar_color;
            current_view.TitleBar.ButtonBackgroundColor = titlebar_color;
            current_view.TitleBar.ButtonInactiveBackgroundColor = titlebar_color;
            current_view.TitleBar.InactiveBackgroundColor = titlebar_color;
            current_view.TitleBar.ForegroundColor = Colors.White;
            current_view.TitleBar.InactiveForegroundColor = Colors.White;
            current_view.TitleBar.ButtonForegroundColor = Colors.White;
            current_view.TitleBar.ButtonInactiveForegroundColor = Colors.White;

            ApplicationView.PreferredLaunchWindowingMode = windowmode.Auto;

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(rect_titlebar);
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

                         var root_frame_type = root_frame.Content?.GetType();
                         if (root_frame_type != _last_page_before_nav_lost && root_frame_type != typeof(UserIntroPage))
                             root_frame.Navigate(_last_page_before_nav_lost);
                         CurrentLocation = await _geolocator.GetGeopositionAsync();
                         GeoLocationChanged?.Invoke(CurrentLocation.Coordinate.Point);
                         break;
                     case PositionStatus.Initializing:
                         break;
                     default:
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
                        await InitializeApplication();
                        var root_frame_type = root_frame.Content?.GetType();
                        if (root_frame_type != typeof(LandingPage))
                            root_frame.Navigate(typeof(LandingPage));

                        break;
                    case GeolocationAccessStatus.Denied:
                        root_frame.Navigate(typeof(NoLocationPage));
                        break;
                    case GeolocationAccessStatus.Unspecified:
                        break;
                }

            }
        }

        private async Task InitializeApplication()
        {
            //copy images to appdata
            var local_folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var install_path = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var media_path = await install_path.GetFolderAsync("media\\images");
            var images = await media_path.GetFilesAsync();
            foreach (var image in images)
            {
                try
                {
                    await local_folder.GetFileAsync(image.Name);
                    continue;
                }
                catch { }
                await image.CopyAsync(local_folder, image.Name, ReplaceExisting);
            }

            //initialize contacts watcher
            await StartBackgroundTaskAsync(() =>
            {
                var task_name = "contacts-watcher";
                var previous_task_list = BackgroundTaskRegistration.AllTasks.Values;
                var registered = previous_task_list.Where(i => i.Name == task_name).FirstOrDefault();
                registered?.Unregister(true);
                registered = null;
                if (registered == null)
                {
                    BackgroundTaskBuilder task_builder = new BackgroundTaskBuilder();
                    task_builder.Name = task_name;
                    task_builder.TaskEntryPoint = typeof(ContactsWatcher).ToString();
                    ContactStoreNotificationTrigger contact_trigger = new ContactStoreNotificationTrigger();
                    task_builder.SetTrigger(contact_trigger);
                    task_builder.Register();
                }
            });

            //initialize actionable toast server
            await StartBackgroundTaskAsync(() =>
            {
                var task_name = "contacts-toast-server";
                var previous_task_list = BackgroundTaskRegistration.AllTasks.Values;
                var registered = previous_task_list.Where(i => i.Name == task_name).FirstOrDefault();
                registered?.Unregister(true);
                registered = null;
                if (registered == null)
                {
                    BackgroundTaskBuilder task_builder = new BackgroundTaskBuilder();
                    task_builder.Name = task_name;
                    task_builder.TaskEntryPoint = typeof(ActionableToastServer).ToString();
                    ToastNotificationActionTrigger toast_trigger = new ToastNotificationActionTrigger();
                    task_builder.SetTrigger(toast_trigger);
                    task_builder.Register();
                }
            });

            //initialize location
            await InitializeLocation();
        }



        private async Task InitializeLocation()
        {
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
        }

        public async Task StartBackgroundTaskAsync(Action task_creator, string error_message = "Could not start background task")
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            switch (status)
            {
                case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                    task_creator();
                    break;
                case BackgroundAccessStatus.Denied:
                    await new MessageDialog(error_message).ShowAsync();
                    break;
            }
        }

        private void OnDashboardClicked(object sender, RoutedEventArgs e)
        {
            root_frame.Navigate(typeof(EmployeeSignIn), typeof(PageNotFound));
        }

        private void OnPOS(object sender, RoutedEventArgs e)
        {
            root_frame.Navigate(typeof(EmployeeSignIn), typeof(PageNotFound));
        }

        private void OnManageContactsClicked(object sender, RoutedEventArgs e)
        {
            root_frame.Navigate(typeof(EmployeeSignIn), typeof(ManageCustomer));
        }

        private void OnOpenMicClicked(object sender, RoutedEventArgs e)
        {
            root_frame.Navigate(typeof(EmployeeSignIn), typeof(PageNotFound));
        }

        private void OnManageProfileClicked(object sender, RoutedEventArgs e)
        {
            root_frame.Navigate(typeof(EmployeeSignIn), typeof(PageNotFound));
        }

        private void OnPaneOpened(object sender, RoutedEventArgs e)
        {
            panel_splitter.IsPaneOpen = !panel_splitter.IsPaneOpen;
        }

        private void OnPatronCounterClicked(object sender, RoutedEventArgs e)
        {
            root_frame.Navigate(typeof(EmployeeSignIn), typeof(PatronCounter));
        }
    }
}
