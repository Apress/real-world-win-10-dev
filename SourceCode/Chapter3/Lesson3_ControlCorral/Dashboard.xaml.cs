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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Lesson2_ControlCorral
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Dashboard : Page
	{
		public Dashboard()
		{
			this.InitializeComponent();
			this.Loaded += Dashboard_Loaded;
		}

		private void Dashboard_Loaded
			(object sender, RoutedEventArgs e)
		{
			gridview_customers.DataContext = App.Model.Customers;
			listview_reservations.DataContext =
				App.Model.Reservations
				.Where(i => i.AppointmentDay.Date == DateTime.Now.Date)
				.ToList();
		}

		private void OnReservation(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(MainPage));
		}

		private void ListReservations(object sender,
			RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(ListReservations),
				App.Model.Reservations);
		}

		async private void CustomerImageLoaded
			(object sender, RoutedEventArgs e)
		{
			var control_image = sender as Image;
			var customer_info = control_image
				.DataContext as CustomerInfo;

			if (customer_info.CustomerImage != null)
			{
				BitmapImage image = new BitmapImage();
				control_image.Source = image;
				MemoryStream stream =
					new MemoryStream(customer_info.CustomerImage);
				await image.SetSourceAsync(stream.AsRandomAccessStream());
			}
		}

		private void CustomerSelected(object sender, ItemClickEventArgs e)
		{
			var selected_customer = e.ClickedItem as CustomerInfo;
			Frame.Navigate(typeof(ManageCustomer), selected_customer);
		}

		private void ReservationSelected(object sender, ItemClickEventArgs e)
		{
			var selected_reservation = e.ClickedItem as ReservationInfo;
			this.Frame.Navigate(typeof(ManageReservation),
				new
				{
					List = listview_reservations.DataContext,
					Selection = selected_reservation,
				});
		}
	}
}
