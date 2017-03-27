using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BigMountainX
{
	
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class TestPage : Page
	{
		MediaCapture _capture;
		public TestPage()
		{
			this.InitializeComponent();
			this.Loaded += MainPage_Loaded;
			_capture = new MediaCapture();
		}

		async private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			await _capture.InitializeAsync();
		}

		private void OnImageCaptureInvoked(object sender, RoutedEventArgs e)
		{
			
			
			//_capture.PrepareLowLagPhotoCaptureAsync(
		}
	}
}
