using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace promodz.Widgets.Converters
{
    [ValueConversion(typeof(double), typeof(String))]
    public class DoubleToIntStrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return System.Convert.ToString( System.Convert.ToInt32(value) );
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble((String)value);
        }
    }
}
