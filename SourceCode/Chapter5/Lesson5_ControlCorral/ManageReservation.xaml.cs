using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
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
	public sealed partial class ManageReservation : Page
	{
		List<ReservationInfo> _current_list;
		ReservationInfo _selected_reservation;
		

		public ManageReservation()
		{
			this.InitializeComponent();
			this.Loaded += ManageReservation_Loaded;
		}

		private void ManageReservation_Loaded(object sender, RoutedEventArgs e)
		{
			flipview_reservations.DataContext = _current_list;
			flipview_reservations.SelectedItem = _selected_reservation;
		}

		private void MassageDateLoaded(object sender, RoutedEventArgs e)
		{
			var txt_date = sender as TextBlock;
			var reservation_info = txt_date.DataContext as ReservationInfo;

			txt_date.Text = $"{reservation_info.AppointmentDay.Month}/" +
					$"{reservation_info.AppointmentDay.Day}/" +
					$"{reservation_info.AppointmentDay.Year}" +
					$" at {reservation_info.AppointmentTime}";
		}

		async private void CustomerImageLoaded(object sender,
			RoutedEventArgs e)
		{
			var control_image = sender as Image;
			var reservation_info = control_image.DataContext as ReservationInfo;

			if (reservation_info.Customer.CustomerImage != null)
			{
				BitmapImage image = new BitmapImage();
				control_image.Source = image;
				MemoryStream stream =
					new MemoryStream(reservation_info.Customer.CustomerImage);
				await image.SetSourceAsync(stream.AsRandomAccessStream());
			}
		}

		private void MassageIntensityLoaded(object sender,
			RoutedEventArgs e)
		{
			var control_slider = sender as Slider;
			var reservation_info = control_slider.DataContext as ReservationInfo;
			control_slider.Value = reservation_info.MassageIntensity;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			dynamic param = e.Parameter;
			if (param != null)
			{
				_current_list = param.List;
				_selected_reservation = param.Selection;
			}
		}

		async private void CancelReservation(object sender, RoutedEventArgs e)
		{
			App.Model.Reservations.Remove(_selected_reservation);
			await App.SaveModelAsync();
			Frame.GoBack();
		}

		private void LoadDOB(object sender, RoutedEventArgs e)
		{
			var control_dob = sender as DatePicker;
			var reservation_info = control_dob.DataContext as ReservationInfo;
			control_dob.Date = reservation_info.Customer.DOB.Date;
		}
	}
}
