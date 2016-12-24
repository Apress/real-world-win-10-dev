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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BigMountainX
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandingPage : Page, INotifyPropertyChanged
    {
        DispatcherTimer _timer;
        List<string> _images = new List<string>
        {
            "ms-appx:///media/images/club1.jpg",
                "ms-appx:///media/images/club2.jpg",
                "ms-appx:///media/images/club3.jpg",
                "ms-appx:///media/images/club4.jpg",
                "ms-appx:///media/images/club5.jpg",
                "ms-appx:///media/images/club6.jpg",
        //                      "ms-appx:///media/images/club7.jpg",
        //"https://peach.blender.org/wp-content/uploads/bbb-splash.png",
        //"https://peach.blender.org/wp-content/uploads/rodents.png",
        //"https://peach.blender.org/wp-content/uploads/evil-frank.png",
        //"https://peach.blender.org/wp-content/uploads/bunny-bow.png",
        //"https://peach.blender.org/wp-content/uploads/rinkysplash.jpg",
        //"https://peach.blender.org/wp-content/uploads/its-a-trap.png"
        };
        int _image_index = 0;
        MediaCapture _capture = null;
        StorageFile _target_file = null;
        AudioGraph _graph_record;
        SystemMediaTransportControls _media_control;

        public event PropertyChangedEventHandler PropertyChanged;

        public int NextEventAttendanceCount { get; private set; }
        public LandingPage()
        {
            this.InitializeComponent();
            this.Loaded += LandingPage_Loaded;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += _timer_Tick;
            _media_control = Windows.Media.SystemMediaTransportControls.GetForCurrentView();
            if (ApplicationHostPage.Host != null)
            {
                ApplicationHostPage.Host.GeoLocationChanged += Host_GeoLocationChanged;
                ApplicationHostPage.Host.AttendaceChanged += Host_AttendaceChanged;
            }

            //set the application titlebar look
            var current_view = ApplicationView.GetForCurrentView();
            var titlebar_color = Color.FromArgb(0xFF, 0x6A, 0x6A, 0x6A);
            current_view.TitleBar.BackgroundColor = titlebar_color; //Colors.DarkGray;
            current_view.TitleBar.InactiveBackgroundColor = titlebar_color;
            current_view.TitleBar.ButtonBackgroundColor = titlebar_color;
            current_view.TitleBar.ButtonInactiveBackgroundColor = titlebar_color;



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

        private void _timer_Tick(object sender, object e)
        {
            img_banner.SetImage(_images[_image_index]);
            _image_index++;
            if (_image_index >= _images.Count)
                _image_index = 0;
        }

        async private void LandingPage_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
            img_banner.SetImage(_images[_image_index]);
            _image_index++;

            //initialize the background audio task
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BackgroundMediaPlayer.SendMessageToBackground(new ValueSet());
            });

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

        async private void OnPlayAudioClicked(object sender, RoutedEventArgs e)
        {
            var file = await Windows.ApplicationModel.Package.Current
              .InstalledLocation.GetFileAsync("media\\audio\\tada.wav");
            await PlayAudio(file);
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

        async private void OnShowStreetView(object sender, RoutedEventArgs e)
        {
            if (map.IsStreetsideSupported)
            {
                var street_panorama = await StreetsidePanorama.FindNearbyAsync(map.Center);
                StreetsideExperience street_exp = new StreetsideExperience(street_panorama);
                map.CustomExperience = street_exp;
            }


        }

        private void OnDirectionsClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DirectionsPage), map.Center);
        }
    }
}
