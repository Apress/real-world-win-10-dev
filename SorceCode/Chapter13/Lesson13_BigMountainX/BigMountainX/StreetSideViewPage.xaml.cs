using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class StreetSideViewPage : Page
    {
        public event Action Close, NotSupported;
        public Geopoint Center { get; private set; }
        ApplicationView _appview;
        CoreApplicationView _core_appview;
        Color TitlebarColor { get; } = Color.FromArgb(0xFF, 0x6A, 0x6A, 0x6A);
        public StreetSideViewPage()
        {
            this.InitializeComponent();
            _appview = ApplicationView.GetForCurrentView();
            _core_appview = CoreApplication.GetCurrentView();

            this.Loaded += StreetSideViewPage_Loaded;
            streetview_map.CustomExperienceChanged += (s, args) =>
            {
                if (streetview_map.CustomExperience == null)
                {
                    Close?.Invoke();
                }
            };


        }

        async public Task InitializeAsync(Geopoint center)
        {
            Center = center;
            streetview_map.Center = Center;
            if (streetview_map.IsStreetsideSupported)
            {
                var street_panorama = await StreetsidePanorama.FindNearbyAsync(streetview_map.Center);
                var street_exp = new StreetsideExperience(street_panorama);
                street_exp.OverviewMapVisible = true;
                streetview_map.CustomExperience = street_exp;
            }
            else
            {
                NotSupported?.Invoke();
            }
            _core_appview.TitleBar.ExtendViewIntoTitleBar = true;
            

            //size the window
            _appview.SetPreferredMinSize(new Size(500, 400));

            //set background
            _appview.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            _appview.TitleBar.ButtonForegroundColor = Colors.White;
            _appview.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            _appview.TitleBar.ButtonInactiveForegroundColor = Colors.White;

            rect_titlebar.Fill = new SolidColorBrush(TitlebarColor);
            Window.Current.SetTitleBar(rect_titlebar);
        }

        private void StreetSideViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Window.Current.SetTitleBar(rect_titlebar);
        }
    }
}
