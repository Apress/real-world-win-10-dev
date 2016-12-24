using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BigMountainX
{
    public class BMXOccasionConverter : IValueConverter
    {
        public object Convert(object value,
            Type targetType,
            object parameter,
            string language)
        {
            if (value is BMXOccasion)
            {
                var occasion = value as BMXOccasion;
                var ui_command = parameter as string;
                if (ui_command == "number of attendants")
                    return $"{occasion.AttendanceCount} attendants";
            }
            return "Unknown number of attendants";
        }

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }
}
