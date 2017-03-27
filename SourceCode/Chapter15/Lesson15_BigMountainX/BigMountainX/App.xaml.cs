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
using Windows.ApplicationModel.VoiceCommands;
using System.Net;
using Windows.ApplicationModel.Store;


namespace BigMountainX
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static AppState State { get; private set; }
        public static LicenseInformation License { get; set; }
        public static ListingInformation ListingInfo { get; set; }


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
                var query = WebUtility.UrlDecode(protocol_args.Uri.Query.ToLower());
                var reader = QueryReader.Load(query);

                if (reader.Contains("launchcontext"))
                {
                    var context = reader["launchcontext"];
                    if (context == "all_events")
                        Window.Current.Content = new ListEventsPage(App.State.Events);
                    else if (context == "event")
                    {
                        var event_id = Guid.Parse(reader["event_id"]);
                        var event_list = App.State.Events.Where(i => i.EventID == event_id).ToList();
                        Window.Current.Content = new ListEventsPage(event_list);
                    }
                }
                else if (reader.Contains("page"))
                {
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


            }
            else if (args.Kind == ActivationKind.ProtocolForResults)
            {
                var protocol_args = args as ProtocolForResultsActivatedEventArgs;
                Window.Current.Content = new LaunchResponsePage(protocol_args);
            }
            else if (args.Kind == ActivationKind.VoiceCommand)
            {
                var command_args = args as VoiceCommandActivatedEventArgs;
                if (command_args != null)
                {
                    var rule_path = command_args.Result.RulePath[0];
                    var location = command_args.Result.SemanticInterpretation.Properties["location"][0].ToLower();

                    var events = App.State.Events;

                    switch (rule_path)
                    {
                        case "show-events-today":
                            {
                                var today = DateTime.Now.Date;
                                events = (from evt in events
                                          where evt.StartDateTime.Date == today
                                          && evt.Address.ToLower().Contains(location)
                                          select evt).ToList();

                            }
                            break;
                        case "show-events-tomorrow":
                            {
                                var tomorrow = DateTime.Now.Date.Add(TimeSpan.FromDays(1));
                                events = (from evt in events
                                          where evt.StartDateTime.Date == tomorrow
                                          && evt.Address.ToLower().Contains(location)
                                          select evt).ToList();
                            }
                            break;
                        case "show-events-thisweek":
                            {
                                var today = DateTime.Now.Date;
                                var week_starting = today.Subtract(TimeSpan.FromDays((int)today.DayOfWeek));
                                var week = Math.Floor(week_starting.DayOfYear / 7.0);

                                events = (from evt in events
                                          let evt_day = evt.StartDateTime
                                          let evt_dow = (int)evt_day.DayOfWeek
                                          let days_to_subtract = TimeSpan.FromDays(evt_dow)
                                          let evt_week_starting = evt_day.Subtract(days_to_subtract)
                                          let evt_week = Math.Floor(evt_week_starting.DayOfYear / 7.0)
                                          where week == evt_week
                                          && evt.Address.ToLower().Contains(location)
                                          select evt).ToList();
                            }
                            break;
                        case "show-events-thismonth":
                            {
                                var today = DateTime.Now;
                                events = (from evt in events
                                          where evt.StartDateTime.Month == today.Month
                                          && evt.Address.ToLower().Contains(location)
                                          select evt).ToList();
                            }
                            break;
                    }
                    Window.Current.Content = new ListEventsPage(events);
                }
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
            var file_marker = await local_folder.CreateFileAsync("marker.txt", OpenIfExists);
            await file_marker.WriteTextAsync("1");
            var file_marker_lock = await local_folder.CreateFileAsync("marker_lock.txt", OpenIfExists);
            await file_marker_lock.WriteTextAsync("1");

            await RegisterVoiceCommands("ms-appx:///media/audio/voicecommands_1.xml");
            await RegisterVoiceCommands("ms-appx:///media/audio/voicecommands_2.xml");

            await InitializeLicensingAsync();
        }

        public async static Task InitializeLicensingAsync()
        {
            var local = ApplicationData.Current.LocalFolder;
            var install_location = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var proxyFolder = await local.CreateFolderAsync("Microsoft\\Windows Store\\ApiData", CreationCollisionOption.OpenIfExists);
            var installLocation = await install_location.GetFolderAsync("storeproxy");
            // open the proxy file
            StorageFile proxyFile = await installLocation.GetFileAsync("trialmanagement.xml");

            // create proxy file in application data
            var simulator_settings = await proxyFolder.CreateFileAsync("WindowsStoreProxy.xml", CreationCollisionOption.ReplaceExisting);

            // replace the contents
            await proxyFile.CopyAndReplaceAsync(simulator_settings);

            //load the listing information read from the file.  The currentproductsimulator seems
            //to expect a file in the location where i put it.  It reads it and uses it as if it is
            //coming from the marketplace.
            License = CurrentAppSimulator.LicenseInformation;
            ListingInfo = await CurrentAppSimulator.LoadListingInformationAsync();

        }

        private static async Task RegisterVoiceCommands(string file_path)
        {
            //launch cortana integration (foreground)
            var vcd_path = file_path;
            var vcd_uri = new Uri(vcd_path);
            var vcd_file = await StorageFile.GetFileFromApplicationUriAsync(vcd_uri);
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcd_file);

            //modifying phrase list programmatically (for more dynamic interactions)
            VoiceCommandDefinition command_set = VoiceCommandDefinitionManager.InstalledCommandDefinitions["en-us-CommandSet"];

            //for location
            await command_set.SetPhraseListAsync("location", new string[]
                                                                { "London",
                                                                    "Dallas",
                                                                    "Maine",
                                                                    "Phoenix",
                                                                    "Miami",
                                                                    "Boston",
                                                                    "Chicago",
                                                                    "New York",
                                                                    "my regular spot",
                                                                });

        }


    }
}
