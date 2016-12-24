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
	public sealed partial class ManageCustomer : Page
	{
		CameraCaptureUI ccui = new CameraCaptureUI();
		CustomerInfo _customer;
		public ManageCustomer()
		{
			this.InitializeComponent();
			this.Loaded += ManageCustomer_Loaded;
			
		}

		async private void ManageCustomer_Loaded(object sender, RoutedEventArgs e)
		{
			if (_customer.CustomerImage != null)
			{
				BitmapImage image = new BitmapImage();
				control_image.Source = image;
				MemoryStream stream =
					new MemoryStream(_customer.CustomerImage);
				await image.SetSourceAsync(stream.AsRandomAccessStream());
			}
			this.DataContext = _customer;
			control_intensity.Value = _customer.MassageIntensity;
			control_dob.Date = _customer.DOB;
		}

		async private void SaveCustomer(object sender, RoutedEventArgs e)
		{
			_customer.MassageIntensity = control_intensity.Value;
			_customer.DOB = control_dob.Date.Date;
			await App.SaveModelAsync();
			Frame.GoBack();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_customer = e.Parameter as CustomerInfo;
		}

		async private void ReplaceImage(object sender, RoutedEventArgs e)
		{
			BitmapImage image = new BitmapImage();
			control_image.Source = image;

			ccui.PhotoSettings.AllowCropping = true;
			ccui.PhotoSettings.MaxResolution = 
				CameraCaptureUIMaxPhotoResolution.HighestAvailable;
			var result = 
				await ccui.CaptureFileAsync(CameraCaptureUIMode.Photo);
			if (result != null)
			{
				var stream = await result.OpenReadAsync();
				await image.SetSourceAsync(stream);

				//get the image data and store it
				stream.Seek(0);
				BinaryReader reader = 
					new BinaryReader(stream.AsStreamForRead());
				_customer.CustomerImage = new byte[stream.Size];
				reader.Read(_customer.CustomerImage, 
					0, 
					_customer.CustomerImage.Length);
			}
		}
	}
}
