using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Data;

namespace promodz.Widgets.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class NormalizeConverter : IValueConverter
    {
        double _base;

        public double Base
        {
            get
            {
                return _base;
            }

            set
            {
                _base = value;
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return (double)value / _base;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value * _base;
        }
    }
}
