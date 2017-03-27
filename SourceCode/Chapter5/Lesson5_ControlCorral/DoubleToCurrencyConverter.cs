using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Lesson2_ControlCorral
{
	public class DoubleToCurrencyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
			object parameter, string language)
		{
			return $"{value:C}";
		}

		public object ConvertBack(object value, Type targetType,
			object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
