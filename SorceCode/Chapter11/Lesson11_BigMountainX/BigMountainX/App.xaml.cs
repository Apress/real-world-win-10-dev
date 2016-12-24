using General.UWP.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
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
using Windows.Storage;
using static Windows.Storage.CreationCollisionOption;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;

namespace BigMountainX
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static AppState State { get; private set; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

        }

        async protected override void OnActivated(IActivatedEventArgs args)
        {
            await InitializeAppAsync();
            if (args.Kind == ActivationKind.Protocol)
            {
                var protocol_args = args as ProtocolActivatedEventArgs;
                var query = protocol_args.Uri.Query.ToLower();
                var reader = QueryReader.Load(query);



                var target_page = reader["page"];



                if (target_page == "street")
                {
                    StreetSideViewPage street_view = new StreetSideViewPage();
                    var center_point = new Geopoint(new BasicGeoposition
                    {
                        Latitude = App.State.NextEvent.Latitude.Value,
                        Longitude = App.State.NextEvent.Longitude.Value,
                    });
                    await street_view.InitializeAsync(center_point);
                    Window.Current.Content = street_view;
                }
                else if (target_page == "main")
                {
                    Window.Current.Content = new ApplicationHostPage();
                }
                else if (target_page == "counter")
                {
                    Window.Current.Content = new PatronCounter();
                }
                else if (target_page == "test")
                {
                    Window.Current.Content = new TestPage();
                }
                else
                {
                    Window.Current.Content = new PageNotFound();
                }

            }
            else if (args.Kind == ActivationKind.ProtocolForResults)
            {
                var protocol_args = args as ProtocolForResultsActivatedEventArgs;
                Window.Current.Content = new LaunchResponsePage(protocol_args);
            }
            Window.Current.Activate();
        }

        async protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            var target_file = args.Files.FirstOrDefault() as StorageFile;
            var query = await target_file.ReadTextAsync();
            var reader = QueryReader.Load(query);
            var target_page = reader["page"];
            if (target_page == "street")
            {
                StreetSideViewPage street_view = new StreetSideViewPage();
                var center_point = new Geopoint(new BasicGeoposition
                {
                    Latitude = App.State.NextEvent.Latitude.Value,
                    Longitude = App.State.NextEvent.Longitude.Value,
                });
                await street_view.InitializeAsync(center_point);
                Window.Current.Content = street_view;
            }
            else if (target_page == "main")
            {
                Window.Current.Content = new ApplicationHostPage();
            }
            else if (target_page == "counter")
            {
                Window.Current.Content = new PatronCounter();
            }
            else if (target_page == "test")
            {
                Window.Current.Content = new TestPage();
            }
            else
            {
                Window.Current.Content = new PageNotFound();
            }
            Window.Current.Activate();
        }

        async protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            await InitializeAppAsync();
            ManageCustomer cust = new ManageCustomer();
            await cust.Activated(args);
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        async protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            await InitializeAppAsync();
            // Place the frame in the current Window
            Window.Current.Content = new ApplicationHostPage();

            Window.Current.Activate();
        }

        private static async Task InitializeAppAsync()
        {
            var bio_path = "media\\text\\artistbio.txt";
            var bio = await Package.Current.InstalledLocation.GetFileAsync(bio_path);
            var roaming_folder = ApplicationData.Current.LocalFolder;
            //await ApplicationData.Current.ClearAsync();
            State = await AppState.FromStorageFileAsync(roaming_folder, "state.xml");
            // State.UserProfile = null;
            if (State.NextEvent == null)
            {
                State.NextEvent = new BMXEvent
                {
                    EventID = Guid.NewGuid(),
                    Address = "350 5th Ave, New York, NY 10118",
                    Latitude = 40.7484,
                    Longitude = -73.9857,
                    CreateDate = DateTime.Now,
                    Description = "A night of wine and comedy",
                    EventTitle = "Comedy Night at the Empire State",
                    Duration = TimeSpan.FromHours(4),
                    StartDateTime = new DateTime(2015, 12, 1, 20, 0, 0),
                    Feature = new BMXFeaturedPerformer
                    {
                        BIO = await bio.ReadTextAsync(),
                    },
                };
                await State.SaveAsync();
            }


            //set the value of marker and marker lock
            var local_folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file_marker = await local_folder.CreateFileAsync("marker_.txt", OpenIfExists);
            await file_marker.WriteTextAsync("1");
            var file_marker_lock = await local_folder.CreateFileAsync("marker_lock.txt", OpenIfExists);
            await file_marker_lock.WriteTextAsync("1");
        }
    }
}
