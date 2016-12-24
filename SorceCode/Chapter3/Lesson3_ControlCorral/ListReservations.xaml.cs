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

namespace Lesson2_ControlCorral
{

	public sealed partial class ListReservations : Page
	{
		List<ReservationInfo> _current_list;
		public ListReservations()
		{
			this.InitializeComponent();
			this.Loaded += ListReservations_Loaded;
		}

		private void ListReservations_Loaded(object sender,
			RoutedEventArgs e)
		{
			list_reservations.DataContext = _current_list;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			var list = e.Parameter as List<ReservationInfo>;
			if (list != null)
				_current_list = list;
		}

		private void ViewReservation(object sender, RoutedEventArgs e)
		{
			var control = sender as FrameworkElement;
			var reservation_info = control.DataContext as ReservationInfo;

			Frame.Navigate(typeof(ManageReservation), new
			{
				Selection = reservation_info,
				List = _current_list,
			});
		}
	}
}
