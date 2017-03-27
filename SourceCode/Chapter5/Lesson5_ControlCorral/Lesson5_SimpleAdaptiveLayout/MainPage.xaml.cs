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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Lesson5_SimpleAdaptiveLayout
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		double _threshold = 0;
		public MainPage()
		{
			this.InitializeComponent();
			this.SizeChanged += WindowSizeChanged;
		}

		private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (stack.ActualWidth >= e.NewSize.Width)
			{
				stack.Orientation = Orientation.Vertical;
				_threshold = e.NewSize.Width;
			}
			else
			{
				if (e.NewSize.Width > _threshold)
					stack.Orientation = Orientation.Horizontal;
			}
		}


	}
}
