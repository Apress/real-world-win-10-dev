using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Lesson2_ControlCorral
{
	public class GenericCommand : System.Windows.Input.ICommand
	{
		public event EventHandler CanExecuteChanged;
		public event Action<string> DoSomething;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			var command = parameter as string;
			DoSomething?.Invoke(command);
		}
	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		//MediaCapture capture = new MediaCapture();
		List<ReservationInfo> _reservations;
		GenericCommand _command;
		List<string> _usuals = new List<string>();
		CameraCaptureUI ccui = new CameraCaptureUI();
		byte[] _user_image;

		public MainPage()
		{
			this.InitializeComponent();
			this.Loaded += MainPage_Loaded;
			_reservations = new List<ReservationInfo>();
			_command = new GenericCommand();
			_command.DoSomething += _command_DoSomething;
			control_name.QuerySubmitted += Control_name_QuerySubmitted;
			control_name.TextChanged += Control_name_TextChanged;
			control_name.SuggestionChosen += Control_name_SuggestionChosen;

		}

		private void Control_name_SuggestionChosen(AutoSuggestBox sender,
			AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			control_name.Text = args.SelectedItem as string;
		}

		private void Control_name_TextChanged(AutoSuggestBox sender,
			AutoSuggestBoxTextChangedEventArgs args)
		{
			if (args.CheckCurrent())
			{
				var search_term = control_name.Text.ToLower();
				var results = _usuals.Where(i => i.Contains(search_term.ToLower())).ToList();
				control_name.ItemsSource = results;

			}
		}



		private void Control_name_QuerySubmitted(AutoSuggestBox sender,
			AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			var search_term = args.QueryText.ToLower();
			var results = _usuals.Where(i => i.Contains(search_term.ToLower())).ToList();
			control_name.ItemsSource = results;
			control_name.IsSuggestionListOpen = true;
		}

		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			this.DataContext = _command;
			_usuals = new List<string> {"alex",
				"azuka",
				"elizabeth",
				"ahmed","josh","allan", "john","david",
				"chris","jack","bobby","ike","emeka",
				"tobe","chidi","mason","andrew" };
			control_name.ItemsSource = _usuals;
		}

		async void ShowSummary(ReservationInfo new_reservation,
						string reservation_tag = "")
		{
			MessageDialog md =
				new MessageDialog
				($"{_reservations.Count} massages reserved\n" +
					$"Newest is {reservation_tag} on" +
					"{new_reservation.AppointmentDay.Month}/" +
					$"{new_reservation.AppointmentDay.Day}/" +
					$"{new_reservation.AppointmentDay.Year}" +
					$" at {new_reservation.AppointmentTime}\n" +
					$"for {new_reservation.CustomerName}\n" +
					$"born {new_reservation.DOB}\n" +
					$"Massage Type: {new_reservation.Procedure}"
					);
			await md.ShowAsync();
			control_calendar.Date = null;
			//WebView wv = null;
			//HttpRequestMessage req = null;
			//wv.NavigateWithHttpRequestMessage(req);
			//wv.cap
		}



		private ReservationInfo CreateReservation()
		{
			var new_reservation = new ReservationInfo()
			{
				AppointmentDay = control_calendar.Date.Value.Date,
				AppointmentTime = control_time.Time,
				CustomerName = control_name.Text,
				DOB = control_dob.Date.Date,
				Passphrase = txt_passphrase.Password,
				Procedure = (control_procedure.SelectedItem
						as ComboBoxItem).Content as string,
				MassageIntensity = control_intensity.Value,
				CustomerImage = _user_image,
			};
			_reservations.Add(new_reservation);
			return new_reservation;
		}

		async private void _command_DoSomething(string command)
		{
			if (control_calendar.Date == null)
			{
				MessageDialog md =
					new MessageDialog("Select a day first");
				await md.ShowAsync();
				return;
			}

			if (command.ToLower() == "make a reservation")
			{
				var new_reservation = CreateReservation();
				ShowSummary(new_reservation, "confirmed");
			}
			else if (command.ToLower() == "hold my spot")
			{
				var new_reservation = CreateReservation();
				ShowSummary(new_reservation, "**tentatively**");
			}
			control_calendar.Date = null;
		}

		async private void ReplaceImage(object sender, RoutedEventArgs e)
		{
			BitmapImage image = new BitmapImage();
			control_image.Source = image;

			ccui.PhotoSettings.AllowCropping = true;
			ccui.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;
			var result = await ccui.CaptureFileAsync(CameraCaptureUIMode.Photo);
			if (result != null)
			{
				var stream = await result.OpenReadAsync();
				await image.SetSourceAsync(stream);

				//get the image data and store it
				stream.Seek(0);
				BinaryReader reader = new BinaryReader(stream.AsStreamForRead());
				_user_image = new byte[stream.Size];
				reader.Read(_user_image, 0, _user_image.Length);
			}
		}
	}
}
