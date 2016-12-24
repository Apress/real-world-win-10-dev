using General.UWP.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
	public sealed partial class UserIntroPage : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		BMXProfile Profile { get; set; }
		List<string> Genders { get; set; }

		public UserIntroPage()
		{
			this.InitializeComponent();
			Profile = new BMXProfile();
			this.Loaded += UserIntroPage_Loaded;
		}


		private void UserIntroPage_Loaded(object sender, RoutedEventArgs e)
		{
			Genders = Enum.GetValues(typeof(GenderCode))
				.Cast<GenderCode>()
				.Select(i => i.ToString()).ToList();
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Genders"));

		}

		async private void OnContinue(object sender, RoutedEventArgs e)
		{
			if (Profile.Gender != null
				&& Profile.DOB != null
				&& !string.IsNullOrWhiteSpace(Profile.FirstName)
				&& !string.IsNullOrWhiteSpace(Profile.LastName)
				&& !string.IsNullOrWhiteSpace(Profile.ContactNumber)
				&& !string.IsNullOrWhiteSpace(Profile.Email)
				&& !string.IsNullOrWhiteSpace(Profile.ImageLocation))
			{
				App.State.UserProfile = Profile;
				await App.State.SaveAsync();
				Frame.Navigate(typeof(LandingPage));
			}
			else
				await new MessageDialog("Please enter all fields").ShowAsync();
		}

		private void OnGenderSelected(object sender, SelectionChangedEventArgs e)
		{
			ComboBox combo = sender as ComboBox;
			var selection = combo.SelectedItem as string;
			var gender_code = (GenderCode)Enum.Parse(typeof(GenderCode), selection);
			Profile.Gender = gender_code;
		}

		private void OnDOBSelected(object sender, DatePickerValueChangedEventArgs e)
		{
			DatePicker date = sender as DatePicker;
			var selection = date.Date.Date;
			Profile.DOB = selection;
		}

		async private void OnImageSelected(ImageViewerControl sender)
		{
			await sender.ImageFile.CopyAsync(ApplicationData.Current.LocalFolder);
			Profile.ImageLocation = sender.ImageFile.Name;
		}

	}
}
