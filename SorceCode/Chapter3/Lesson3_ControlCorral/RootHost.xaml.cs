using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class RootHost : Page
	{
		public RootHost()
		{
			this.InitializeComponent();
			SystemNavigationManager
				.GetForCurrentView()
				.AppViewBackButtonVisibility 
				= AppViewBackButtonVisibility.Visible;
			SystemNavigationManager
				.GetForCurrentView()
				.BackRequested += RootHost_BackRequested;

			this.Loaded += RootHost_Loaded;
		}

		private void RootHost_Loaded(object sender, RoutedEventArgs e)
		{
			rootframe.Navigate(typeof(Dashboard));
		}

		private void RootHost_BackRequested(object sender, 
			BackRequestedEventArgs e)
		{
			if (rootframe.CanGoBack)
			{
				e.Handled = true;
				rootframe.GoBack();
			}
			else
				e.Handled = false;
		}
	}
}
