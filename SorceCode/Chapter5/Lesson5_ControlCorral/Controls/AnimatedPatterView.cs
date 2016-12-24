using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Lesson2_ControlCorral.Controls
{
	public class AnimatedPatternView : Control
	{
		Canvas Part_root;

		public static DependencyProperty ShowFilledCirclesProperty = DependencyProperty.Register("ShowFillCircles", typeof(bool), typeof(AnimatedPatternView), null);
		public static DependencyProperty IsMonochromeProperty = DependencyProperty.Register("IsMonochrome", typeof(bool), typeof(AnimatedPatternView), null);
		public static DependencyProperty MinimumDurationProperty = DependencyProperty.Register("MinimumDuration", typeof(int), typeof(AnimatedPatternView), null);
		public static DependencyProperty MaximumDurationProperty = DependencyProperty.Register("MaximumDuration", typeof(int), typeof(AnimatedPatternView), null);

		public AnimatedPatternView()
		{
			this.DefaultStyleKey = typeof(AnimatedPatternView);
			this.Loaded += AnimatedPatternView_Loaded;

		}

		#region
		/// <summary>
		/// Minimum lenght of time for an individual animation to run
		/// </summary>
		public int MinimumDuration
		{
			get
			{
				return (int)this.GetValue(MinimumDurationProperty);
			}
			set
			{
				this.SetValue(MinimumDurationProperty, value);
			}
		}

		/// <summary>
		/// Maximum amount of time for an individual animation to run
		/// </summary>
		public int MaximumDuration
		{
			get
			{
				return (int)this.GetValue(MaximumDurationProperty);
			}
			set
			{

				this.SetValue(MaximumDurationProperty, value);
			}
		}

		public bool ShowFilledCircles
		{
			get
			{
				return (bool)this.GetValue(ShowFilledCirclesProperty);
			}
			set
			{
				this.SetValue(ShowFilledCirclesProperty, value);
			}
		}

		public bool IsMonochrome
		{
			get
			{
				return (bool)this.GetValue(IsMonochromeProperty);
			}
			set
			{
				this.SetValue(IsMonochromeProperty, value);
			}
		}
		#endregion

		protected override void OnApplyTemplate()
		{
			Part_root = this.GetTemplateChild("PART_root") as Canvas;
			this.IsEnabled = false;
		}

		void AnimatedPatternView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{


			Part_root.Children.Clear();

			//Random size_random = new Random();
			Random position_random = new Random();
			Color color = Colors.Gray;

			Storyboard sb = new Storyboard();
			foreach (var i in Enumerable.Range(1, 50))
			{
				#region code
				int size = position_random.Next(10, 500);
				int left = position_random.Next((int)this.ActualWidth);
				int top = position_random.Next((int)this.ActualHeight);
				int range_x = position_random.Next((int)this.ActualWidth);
				int range_y = position_random.Next((int)this.ActualHeight);

				int min_duration = 5, max_duration = 90;
				if (MinimumDuration > 5)
					min_duration = MinimumDuration;

				if (MaximumDuration < 90)
					max_duration = MaximumDuration;

				int duration = position_random.Next(min_duration, max_duration);

				byte a = (byte)position_random.Next(0, 255);
				byte r = (byte)position_random.Next(0, 255);
				byte g = (byte)position_random.Next(0, 255);
				byte b = (byte)position_random.Next(0, 255);

				if (!IsMonochrome)
				{
					color = Color.FromArgb(a, r, g, b);
				}

				DoubleAnimation motion_x = new DoubleAnimation();
				Storyboard.SetTargetProperty(motion_x, "(Canvas.Left)");

				DoubleAnimation motion_y = new DoubleAnimation();
				Storyboard.SetTargetProperty(motion_y, "(Canvas.Top)");

				DoubleAnimationUsingKeyFrames opacity = new DoubleAnimationUsingKeyFrames();
				LinearDoubleKeyFrame ld = new LinearDoubleKeyFrame();
				ld.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration / 2.0));
				ld.Value = 1;
				LinearDoubleKeyFrame ld2 = new LinearDoubleKeyFrame();
				ld2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration));
				ld2.Value = 0;
				opacity.KeyFrames.Add(ld);
				opacity.KeyFrames.Add(ld2);

				DisplayCircle(sb, motion_x, motion_y, opacity, left, top, size, size, color, ShowFilledCircles, duration, range_x, range_y);

				sb.Children.Add(motion_x);
				sb.Children.Add(motion_y);
				sb.Children.Add(opacity);
				#endregion
			}

			sb.Begin();
		}

		#region background_animation
		private void DisplayCircle(Storyboard sb, DoubleAnimation motion_x, DoubleAnimation motion_y, DoubleAnimationUsingKeyFrames opacity, int x, int y, int height, int width, Color color, bool fill, double duration, int range_x, int range_y, Ellipse circle = null)
		{
			if (circle == null)
			{
				circle = new Ellipse();
				Part_root.Children.Add(circle);
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

			opacity.Completed += (a, b) =>
			{
				#region body
				//remove the completed circle from the tree
				//Part_root.Children.Remove(circle);

				//if this item circle is transient then when it dies dont create another one


				Random size_random = new Random();
				Random left_random = new Random();
				var bounds = Window.Current.Bounds;

				int size = size_random.Next(10, 500);
				int left = left_random.Next((int)bounds.Width);
				int top = left_random.Next((int)bounds.Height);
				int temp_range_x = left_random.Next((int)bounds.Width);
				int temp_range_y = left_random.Next((int)bounds.Height);
				int temp_duration = left_random.Next(3, 60);

				byte a2 = (byte)left_random.Next(0, 255);
				byte r2 = (byte)left_random.Next(0, 255);
				byte g2 = (byte)left_random.Next(0, 255);
				byte b2 = (byte)left_random.Next(0, 255);
				Color color2 = Color.FromArgb(a2, r2, g2, b2);

				DisplayCircle(sb, motion_x, motion_y, opacity, left, top, size, size, color2, fill, temp_duration, temp_range_x, temp_range_y, circle);

				#endregion
			};
			Storyboard.SetTarget(opacity, circle);
			Storyboard.SetTargetName(opacity, circle.Name);
			Storyboard.SetTargetProperty(opacity, "Opacity");
		}


		#endregion
	}
}
