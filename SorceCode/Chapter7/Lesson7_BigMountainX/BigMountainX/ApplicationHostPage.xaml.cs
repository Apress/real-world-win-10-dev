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

namespace BigMountainX
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ApplicationHostPage : Page
	{
		public ApplicationHostPage()
		{
			this.InitializeComponent();
			this.Loaded += ApplicationHostPage_Loaded;
		}

		private void ApplicationHostPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (App.State.UserProfile == null)
				root_frame.Navigate(typeof(UserIntroPage));
			else
				root_frame.Navigate(typeof(LandingPage));
		}
	}
}
