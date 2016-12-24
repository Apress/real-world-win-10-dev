using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
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
    public sealed partial class DirectionsPage : Page
    {
        Geopoint _center;
        public DirectionsPage()
        {
            this.InitializeComponent();
            this.Loaded += DirectionsPage_Loaded;
        }

        async private void DirectionsPage_Loaded(object sender, RoutedEventArgs e)
        {
            var start = ApplicationHostPage.Host.CurrentLocation.Coordinate;
            var result = await MapRouteFinder.GetDrivingRouteAsync(start.Point, _center);


            progress.IsActive = false;
            map.Center = start.Point;
            SymbolIcon symbol_start = new SymbolIcon(Symbol.Favorite);
            map.Children.Add(symbol_start);
            MapControl.SetLocation(symbol_start, start.Point);

            SymbolIcon symbol_end = new SymbolIcon(Symbol.Favorite);
            map.Children.Add(symbol_end);
            MapControl.SetLocation(symbol_end, _center);

            MapRouteView route_view = new MapRouteView(result.Route);
            map.Routes.Add(route_view);
            //route_view.Route.BoundingBox

            await map.TrySetViewBoundsAsync(route_view.Route.BoundingBox,null,MapAnimationKind.Default);

            progress.Visibility = Visibility.Collapsed;
            foreach (var leg in result.Route.Legs)
            {
                SymbolIcon leg_symbol = new SymbolIcon(Symbol.Refresh);
                leg_symbol.HorizontalAlignment = HorizontalAlignment.Center;
                leg_symbol.Margin = new Thickness(5);
                StackPanel stack_leg = new StackPanel();
                stack_leg.HorizontalAlignment = HorizontalAlignment.Center;
                foreach (var manuever in leg.Maneuvers)
                {
                    TextBlock txt_instruction = new TextBlock();
                    txt_instruction.Margin = new Thickness(5);
                    txt_instruction.Text = manuever.InstructionText;
                    stack_leg.Children.Add(txt_instruction);

                }

                spanel_directions.Children.Add(leg_symbol);
                spanel_directions.Children.Add(stack_leg);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _center = e.Parameter as Geopoint;

            base.OnNavigatedTo(e);
        }
    }
}
