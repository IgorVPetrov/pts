using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Data;



namespace promodz.Widgets.Converters
{
    [ValueConversion(typeof(SolidColorBrush), typeof(double))]
    public class LedRegulatotHorizontaleColorConverter : IMultiValueConverter
    {
        LinearGradientBrush _brush;

        public LinearGradientBrush Brush
        {
            get
            {
                return _brush;
            }

            set
            {
                _brush = value;
            }
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            _brush = (LinearGradientBrush)values[1];
            IEnumerable<GradientStop> low = _brush.GradientStops.TakeWhile(gs => gs.Offset <= (double)values[0]);
            IEnumerable<GradientStop> high = _brush.GradientStops.SkipWhile(gs => gs.Offset <= (double)values[0]);

            GradientStop lowGradient = low.Last();
            GradientStop highGradient = high.First();


            double lowOffset = lowGradient.Offset;
            double highOffset = highGradient.Offset;


            Color lowColor = lowGradient.Color;
            Color highColor = highGradient.Color;

            double ScA = 1;
            double ScB = equality((double)values[0], lowOffset, lowColor.ScB, highOffset, highColor.ScB);
            double ScR = equality((double)values[0], lowOffset, lowColor.ScR, highOffset, highColor.ScR);
            double ScG = equality((double)values[0], lowOffset, lowColor.ScG, highOffset, highColor.ScG);
            Color color = Color.FromScRgb((float)ScA, (float)ScR, (float)ScG, (float)ScB);

            return new SolidColorBrush(color);
        }

        private double equality(double x,double x1,double y1, double x2, double y2)
        {
            return y1 + ((y2 - y1)*(x - x1)) / (x2 - x1);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[2] { Color.FromScRgb(0, 0, 0, 0), Color.FromScRgb(0, 0, 0, 0)};
        }
    }
}
