using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace General.UWP.Library
{
	public sealed partial class ImageViewerControl : UserControl
	{
		MediaCapture _capture;
		CameraCaptureUI _capture_ui;
		public event Action<ImageViewerControl> ImageSelected;
		public StorageFile ImageFile { get; set; }
		public ImageViewerControl()
		{
			this.InitializeComponent();
			_capture = new MediaCapture();
			_capture_ui = new CameraCaptureUI();
			this.Loaded += ImageViewerControl_Loaded;
		}

		private void ImageViewerControl_Loaded(object sender, RoutedEventArgs e)
		{
			
			
		}

		async public Task LoadImageAsync(StorageFolder folder, string image_file_path)
		{
			try
			{
				var image_file = await folder.GetFileAsync(image_file_path);
				await InternalLoadImageAsync(image_file);
			}
			catch { }
		}

		async private void TakePictureClicked(object sender, RoutedEventArgs e)
		{
			//_capture_ui.PhotoSettings.AllowCropping = true;
			//_capture_ui.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;
			//var captured_image = await _capture_ui.CaptureFileAsync(CameraCaptureUIMode.Photo);
			//if (captured_image != null)
			//{
			//	await InternalLoadImageAsync(captured_image);
			//	ImageSelected?.Invoke(this);
			//}
			symbol_camera.Visibility = Visibility.Visible;
			img_control.Visibility = Visibility.Collapsed;

			
			//initialize capture
			await _capture.InitializeAsync();

			//start previewing
			capture_element.Source = _capture;
			await _capture.StartPreviewAsync();
		}

		async private void BrowseImageClicked(object sender, RoutedEventArgs e)
		{
			FileOpenPicker opener = new FileOpenPicker();
			opener.ViewMode = PickerViewMode.Thumbnail;
			opener.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			opener.CommitButtonText = "Select Picture";
			opener.FileTypeFilter.Add(".png");
			opener.FileTypeFilter.Add(".jpg");
			var selected_file = await opener.PickSingleFileAsync();
			if (selected_file != null)
			{
				await InternalLoadImageAsync(selected_file);
				ImageSelected?.Invoke(this);
			}
		}

		private async Task InternalLoadImageAsync(StorageFile selected_file)
		{
			ImageFile = selected_file;

			//display the image
			BitmapImage image = new BitmapImage();
			img_control.Source = image;
			var stream = await selected_file.OpenReadAsync();
			await image.SetSourceAsync(stream);
		}

		private void PointerPressedHandler(object sender, PointerRoutedEventArgs e)
		{
			FlyoutBase.ShowAttachedFlyout(img_control);
		}

		private void OnImageOpened(object sender, RoutedEventArgs e)
		{
			symbol_camera.Visibility = Visibility.Collapsed;
		}

		async private void OnTakeMediaCapturePicture(object sender, RoutedEventArgs e)
		{
			//set properties of image
			ImageEncodingProperties image_props = new ImageEncodingProperties();
			image_props.Height = (uint)this.ActualHeight;
			image_props.Width = (uint)this.ActualWidth;
			image_props.Subtype = "PNG";

			//specify where image will be stored
			var captured_image = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync("temp.png",
				CreationCollisionOption.GenerateUniqueName);

			//capture to file
			await _capture.CapturePhotoToStorageFileAsync(image_props, captured_image);
			img_control.Visibility = Visibility.Visible;
			await _capture.StopPreviewAsync();


			await InternalLoadImageAsync(captured_image);
			ImageSelected?.Invoke(this);


		}
	}
}
