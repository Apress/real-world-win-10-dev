using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Lesson2_ControlCorral
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class RootHost : Page
	{
		DispatcherTimer _security_timer;
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
			_security_timer = new DispatcherTimer();
			_security_timer.Interval = TimeSpan.FromSeconds(30);
			_security_timer.Tick += (a, b) =>
			{
				_security_timer.Stop();
				StartScreenHider();
				_security_timer.Start();
			};
			//	Window.Current.SetTitleBar(border_titlebar);
			//	CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
			ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Gainsboro;
			ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Colors.Gainsboro;

		}



		void StartScreenHider()
		{
			animation_root.Visibility = Visibility.Visible;
			animation_root.Children.Clear();

			Random position_random = new Random();
			Color color = Colors.Gray;



			foreach (var i in Enumerable.Range(1, 50))
			{
				Storyboard sb = new Storyboard();

				#region code
				int size = position_random.Next(10, 500);
				int left = position_random.Next((int)this.ActualWidth);
				int top = position_random.Next((int)this.ActualHeight);
				int range_x = position_random.Next((int)this.ActualWidth);
				int range_y = position_random.Next((int)this.ActualHeight);

				int min_duration = 5, max_duration = 10;


				int duration = position_random.Next(min_duration, max_duration);

				byte a = (byte)position_random.Next(0, 255);
				byte r = (byte)position_random.Next(0, 255);
				byte g = (byte)position_random.Next(0, 255);
				byte b = (byte)position_random.Next(0, 255);
				color = Color.FromArgb(a, r, g, b);


				DoubleAnimation motion_x = new DoubleAnimation();
				Storyboard.SetTargetProperty(motion_x, "(Canvas.Left)");

				DoubleAnimation motion_y = new DoubleAnimation();
				Storyboard.SetTargetProperty(motion_y, "(Canvas.Top)");

				DoubleAnimationUsingKeyFrames opacity =
							new DoubleAnimationUsingKeyFrames();
				LinearDoubleKeyFrame ld = new LinearDoubleKeyFrame();
				ld.KeyTime = KeyTime
							.FromTimeSpan(TimeSpan.FromSeconds(duration / 2.0));
				ld.Value = 1;
				LinearDoubleKeyFrame ld2 = new LinearDoubleKeyFrame();
				ld2.KeyTime = KeyTime
							.FromTimeSpan(TimeSpan.FromSeconds(duration));
				ld2.Value = 0;
				opacity.KeyFrames.Add(ld);
				opacity.KeyFrames.Add(ld2);

				DisplayCircle(sb, motion_x, motion_y, opacity,
					left, top, size, size, color, true,
					duration, range_x, range_y);

				sb.Children.Add(motion_x);
				sb.Children.Add(motion_y);
				sb.Children.Add(opacity);

				#endregion

				sb.Begin();
			}


		}


		private void DisplayCircle(Storyboard sb, DoubleAnimation motion_x, DoubleAnimation motion_y, DoubleAnimationUsingKeyFrames opacity, int x, int y, int height, int width, Color color, bool fill, double duration, int range_x, int range_y, Ellipse circle = null)
		{
			if (circle == null)
			{
				circle = new Ellipse();
				animation_root.Children.Add(circle);
			}

			circle.Width = width;
			circle.Height = height;
			circle.SetValue(Canvas.LeftProperty, x);
			circle.SetValue(Canvas.TopProperty, y);
			circle.Stroke = new SolidColorBrush(color);
			if (fill)
				circle.Fill = new SolidColorBrush(color);
			circle.StrokeThickness = 1;
			circle.Opacity = 0;


			Storyboard.SetTarget(motion_x, circle);
			Storyboard.SetTarget(motion_y, circle);

			motion_x.To = range_x;
			motion_y.To = range_y;
			motion_x.Duration = new Duration(TimeSpan.FromSeconds(duration));
			motion_y.Duration = new Duration(TimeSpan.FromSeconds(duration));


			Storyboard.SetTarget(opacity, circle);
			Storyboard.SetTargetName(opacity, circle.Name);
			Storyboard.SetTargetProperty(opacity, "Opacity");

			opacity.Completed += (a, b) =>
			{
				#region body
				Storyboard sb2 = new Storyboard();
				Random size_random = new Random();
				Random left_random = new Random();
				var bounds = Window.Current.Bounds;

				int size = size_random.Next(10, 500);
				int left = left_random.Next((int)bounds.Width);
				int top = left_random.Next((int)bounds.Height);
				int temp_range_x = left_random.Next((int)bounds.Width);
				int temp_range_y = left_random.Next((int)bounds.Height);
				int temp_duration = left_random.Next(5, 10);

				byte a2 = (byte)left_random.Next(0, 255);
				byte r2 = (byte)left_random.Next(0, 255);
				byte g2 = (byte)left_random.Next(0, 255);
				byte b2 = (byte)left_random.Next(0, 255);
				Color color2 = Color.FromArgb(a2, r2, g2, b2);

				DoubleAnimation motion_x2 = new DoubleAnimation();
				Storyboard.SetTargetProperty(motion_x2, "(Canvas.Left)");

				DoubleAnimation motion_y2 = new DoubleAnimation();
				Storyboard.SetTargetProperty(motion_y2, "(Canvas.Top)");

				DoubleAnimationUsingKeyFrames opacity2 =
					new DoubleAnimationUsingKeyFrames();
				LinearDoubleKeyFrame ld = new LinearDoubleKeyFrame();
				ld.KeyTime = KeyTime
					.FromTimeSpan(TimeSpan.FromSeconds(duration / 2.0));
				ld.Value = 1;
				LinearDoubleKeyFrame ld2 = new LinearDoubleKeyFrame();
				ld2.KeyTime = KeyTime
					.FromTimeSpan(TimeSpan.FromSeconds(duration));
				ld2.Value = 0;
				opacity2.KeyFrames.Add(ld);
				opacity2.KeyFrames.Add(ld2);


				DisplayCircle(sb2, motion_x2, motion_y2, opacity2, left, top,
				size, size, color2, fill, temp_duration,
				temp_range_x, temp_range_y, circle);
				sb2.Children.Add(motion_x2);
				sb2.Children.Add(motion_y2);
				sb2.Children.Add(opacity2);
				sb2.Begin();
				#endregion
			};
		}

		private void RootHost_Loaded(object sender, RoutedEventArgs e)
		{
			rootframe.Navigate(typeof(Dashboard));
			_security_timer.Start();
		}

		private void ResetTimer()
		{
			_security_timer.Stop();
			_security_timer.Start();
			animation_root.Visibility = Visibility.Collapsed;
		}

		protected override void OnKeyDown(KeyRoutedEventArgs e)
		{
			base.OnKeyDown(e);
			ResetTimer();
		}

		protected override void OnPointerMoved(PointerRoutedEventArgs e)
		{
			base.OnPointerMoved(e);
			ResetTimer();
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
