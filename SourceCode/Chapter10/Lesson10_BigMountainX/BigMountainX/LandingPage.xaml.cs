using General.UWP.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Media.Render;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using static Windows.Storage.CreationCollisionOption;
using windowmode = Windows.UI.ViewManagement.ApplicationViewWindowingMode;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BigMountainX
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandingPage : Page, INotifyPropertyChanged
    {

        MediaCapture _capture = null;
        StorageFile _target_file = null;
        AudioGraph _graph_record;
        SystemMediaTransportControls _media_control;

        public event PropertyChangedEventHandler PropertyChanged;

        public int NextEventAttendanceCount { get; private set; }
        BMXFeaturedPerformer Feature { get; set; }
        PopupMenu image_banner_menu;
        public LandingPage()
        {
            this.InitializeComponent();
            this.Loaded += LandingPage_Loaded;


            if (ApplicationHostPage.Host != null)
            {
                ApplicationHostPage.Host.GeoLocationChanged += Host_GeoLocationChanged;
                ApplicationHostPage.Host.AttendaceChanged += Host_AttendaceChanged;
            }



            img_banner.SetImage("ms-appx:///media/images/club1.jpg");
            img_banner.PointerPressed += Img_banner_PointerPressed;
            Feature = App.State.NextEvent.Feature;

            image_banner_menu = new PopupMenu();

            var task_name = "should-have-been-there";
            var previous_task_list = BackgroundTaskRegistration.AllTasks.Values;
            var reg_task = previous_task_list.Where(i => i.Name == task_name).FirstOrDefault();
            image_banner_menu.Commands.Add(new UICommand($"{(reg_task == null ? "Start" : "Stop")} wallpaper switcher", async (ui_command) =>
                {
                    previous_task_list = BackgroundTaskRegistration.AllTasks.Values;
                    var registered = previous_task_list.Where(i => i.Name == task_name).FirstOrDefault();
                    if (registered == null)
                    {
                        var local_folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                        var file = await local_folder.CreateFileAsync("marker.txt", OpenIfExists);
                        await file.WriteTextAsync("1");

                        await ApplicationHostPage.Host.StartBackgroundTaskAsync(() =>
                        {
                            var task = new BackgroundTaskBuilder();

                            task.Name = task_name;
                            task.TaskEntryPoint = typeof(BMX.BackgroundTasks.WallpaperSwitcherTask).ToString();
                            TimeTrigger timer = new TimeTrigger(20, false);
                            task.SetTrigger(timer);
                            registered = task.Register();
                            registered.Progress += async (s, args) =>
                            {
                                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    ApplicationView.GetForCurrentView().Title = $"Showing Image {args.Progress}");
                            };

                            ui_command.Label = "Stop wallpaper switcher";
                        }, "Cant do this now");


                    }
                    else
                    {
                        //unregister if button is clicked again
                        registered.Unregister(true);
                        ui_command.Label = "Start wallpaper switcher";
                    }

                }));

            image_banner_menu.Commands.Add(new UICommand("Next Wallpaper", async (ui_command) =>
                {
                    await UserSettings.ChangeWallpaperAsync();
                }));
            image_banner_menu.Commands.Add(new UICommand("Next Lock Screen", async (ui_command) =>
            {
                await UserSettings.ChangeLockScreenAsync();
            }));

            //initializes the background audio task
            BackgroundMediaPlayer.MessageReceivedFromBackground += BackgroundMessageRecieved;
            _media_control = SystemMediaTransportControls.GetForCurrentView();
            _media_control.ButtonPressed += _media_control_ButtonPressed;

            _media_control.IsPreviousEnabled = true;
            _media_control.IsNextEnabled = true;



        }

        async private void BackgroundMessageRecieved(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                media.Stop();
            });
        }

        private void _media_control_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            //send the a message to the background to handle the action
            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet
    {
        {"action",args.Button.ToString() }
    });
        }

        async private void Img_banner_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            await image_banner_menu.ShowAsync(e.GetCurrentPoint(this).Position);
        }



        async private void Host_AttendaceChanged(int increment)
        {
            //increment this value every time the next event changes
            NextEventAttendanceCount += increment;

            //tell the binding system
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NextEventAttendanceCount"));
            });
        }

        async private void Host_GeoLocationChanged(Geopoint obj)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {

                var point = obj;
                var location = $"LAT:{point.Position.Latitude}, ";
                location += $"LONG:{point.Position.Longitude}";

                txt_location.Text = "searching ... ";
                try
                {
                    var location_results = await MapLocationFinder.FindLocationsAtAsync(point);

                    //since we are only interested in city and state info
                    //we only need the first item from the list
                    var location_result = location_results.Locations.FirstOrDefault();

                    if (location_result != null)
                    {
                        txt_location.Text = $"{location_result.Address.Town}, {location_result.Address.Region}";
                    }
                }
                catch
                {
                    txt_location.Text = "Location unknown";
                }

            });
        }



        private void LandingPage_Loaded(object sender, RoutedEventArgs e)
        {


            //set the starting point of the map
            map.Center = new Geopoint(new BasicGeoposition
            {
                Latitude = App.State.NextEvent.Latitude.Value,
                Longitude = App.State.NextEvent.Longitude.Value,
            });
            map.ZoomLevel = 17.5;
            //MapIcon map_icon = new MapIcon();
            //map_icon.Title = App.State.NextEvent.EventTitle;
            //map_icon.Location = map.Center;
            //map.MapElements.Add(map_icon);

            SymbolIcon symbol = new SymbolIcon(Symbol.Favorite);
            map.Children.Add(symbol);
            MapControl.SetLocation(symbol, map.Center);


            //
            //clear the backstack so that you cannot navigate back from
            //this page
            Frame.BackStack.Clear();
        }



        async private void OnImageViewerLoaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(App.State.UserProfile?.ImageLocation))
            {
                var image_viewer = sender as ImageViewerControl;
                await image_viewer.LoadImageAsync(Windows.Storage.ApplicationData.Current.LocalFolder, App.State.UserProfile?.ImageLocation);
            }
        }

        async private void OnImageSelected(ImageViewerControl sender)
        {
            await sender.ImageFile.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder);
            App.State.UserProfile.ImageLocation = sender.ImageFile.Name;
            await App.State.SaveAsync();
        }

        private void LoadImageForEllipse(object sender, RoutedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            BitmapImage img = new BitmapImage();
            img.UriSource = new Uri("https://peach.blender.org/wp-content/uploads/bbb-splash.png");
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = img;
            ellipse.Fill = brush;
        }

        private static async Task PlayAudio(StorageFile file)
        {
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
            var result = await AudioGraph.CreateAsync(settings);

            if (result.Status == AudioGraphCreationStatus.Success)
            {
                var graph = result.Graph;
                var output = await graph.CreateDeviceOutputNodeAsync();
                var input = await graph.CreateFileInputNodeAsync(file);
                input.FileInputNode.AddOutgoingConnection(output.DeviceOutputNode);
                graph.Start();

            }
            else
                await new MessageDialog("Not created").ShowAsync();
        }

        async private Task RecordAsync(ToggleButton btn_record_audio)
        {

            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
            settings.StreamingCaptureMode = StreamingCaptureMode.Audio;

            _capture = new MediaCapture();
            await _capture.InitializeAsync(settings);
            var profile = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High);
            var feedback_folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("AudioFeedback", CreationCollisionOption.OpenIfExists);

            _target_file = await feedback_folder.CreateFileAsync("audio message.mp3", CreationCollisionOption.GenerateUniqueName);
            await _capture.StartRecordToStorageFileAsync(profile, _target_file);

        }

        async private Task StopRecordingAsync()
        {
            await _capture.StopRecordAsync();
            await PlayAudio(_target_file);
        }

        async private void ToggleRecord(object sender, RoutedEventArgs e)
        {
            var btn_record_audio = sender as ToggleButton;
            if (btn_record_audio.IsChecked == false)
            {
                await StopRecordingAsync();
            }
            else
            {
                media.Stop();  //stop playback since we are recording
                await RecordAsync(btn_record_audio);
            }
        }


        async private void ToggleRecord2(object sender, RoutedEventArgs e)
        {
            var btn_record_audio = sender as ToggleButton;
            if (btn_record_audio.IsChecked == false)
            {
                _graph_record.Stop();
                _graph_record.Dispose();
                await PlayAudio(_target_file);

                //using the media element to play the sound
                //var raf_stream = await _target_file.OpenReadAsync();
                //media.SetSource(raf_stream, "");
                //media.Play();

            }
            else
            {
                //initialize the audio graph for recording and then start recording
                AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
                settings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.LowestLatency;

                CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
                if (result.Status == AudioGraphCreationStatus.Success)
                {
                    _graph_record = result.Graph;

                    //setup the input
                    var input_node = (await _graph_record.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Other)).DeviceInputNode;

                    //setup the output (place where audio will be recorded to)
                    var feedback_folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("AudioFeedback", CreationCollisionOption.OpenIfExists);
                    _target_file = await feedback_folder.CreateFileAsync("audio message.mp3", CreationCollisionOption.GenerateUniqueName);

                    var profile = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High);
                    var file_output_node = (await _graph_record.CreateFileOutputNodeAsync(_target_file, profile)).FileOutputNode;

                    //direct the input to the output
                    input_node.AddOutgoingConnection(file_output_node);
                    media.Stop();  //stop playback since we are recording
                    _graph_record.Start();

                }
                else
                    await new MessageDialog("Could not initialize recorder").ShowAsync();

            }
        }

        async private void ToggleRecordVideo(object sender, RoutedEventArgs e)
        {
            //initialize capture
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
            settings.StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo;
            _capture = new MediaCapture();
            await _capture.InitializeAsync(settings);

            //start preview
            capture_element.Source = _capture;
            await _capture.StartPreviewAsync();

            //start capturing media
            var profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Vga);
            var feedback_folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("VideoFeedback", CreationCollisionOption.OpenIfExists);

            _target_file = await feedback_folder.CreateFileAsync("video message.mp4", CreationCollisionOption.GenerateUniqueName);
            await _capture.StartRecordToStorageFileAsync(profile, _target_file);

            //show the flyout menu
            media_message.Visibility = Visibility.Collapsed;
            media.Stop();  //stop playback since we are recording
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        async private void OnStopVideoCapture(object sender, RoutedEventArgs e)
        {
            await _capture.StopRecordAsync();
            await _capture.StopPreviewAsync();


            media_message.Visibility = Visibility.Visible;
            var raf_stream = await _target_file.OpenReadAsync();
            media_message.SetSource(raf_stream, "video/mp4");

        }


        private void OnVideoMessageRecordingReady(object sender, RoutedEventArgs e)
        {
            media_message.Play();
        }

        private void OnVideoMessagePlaybackCompleted(object sender, RoutedEventArgs e)
        {
            flyout_videomessage.Hide();
        }

        async private void OnClickNextEvent(object sender, RoutedEventArgs e)
        {
            var uri = new Uri($"bingmaps://?where={App.State.NextEvent.Address}");
            await Launcher.LaunchUriAsync(uri);
        }
        CoreApplicationView _core_streetside_view;
        async private void OnShowStreetView(object sender, RoutedEventArgs e)
        {
            var view_id = 0;
            var supported = true;
            var center_point = map.Center;

            if (_core_streetside_view != null)
            {
                await _core_streetside_view.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _core_streetside_view.CoreWindow.Close();
                });
            }

            _core_streetside_view = CoreApplication.CreateNewView();
            await _core_streetside_view.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
             {
                 var app_view = ApplicationView.GetForCurrentView();
                 view_id = app_view.Id;

                 StreetSideViewPage street_view = new StreetSideViewPage();
                 await street_view.InitializeAsync(center_point);

                 street_view.Close += () =>
                 {
                     _core_streetside_view.CoreWindow.Close();
                     _core_streetside_view = null;
                 };
                 street_view.NotSupported += () => supported = false;

                 Window.Current.Content = street_view;
                 Window.Current.Activate();
             });

            if (supported)
            {
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(view_id);
            }
            else
            {
                await new MessageDialog("StreetSide View is not supported.").ShowAsync();
            }
        }


        private void OnDirectionsClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DirectionsPage), map.Center);
        }


    }
}
