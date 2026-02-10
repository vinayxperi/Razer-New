using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace RazerConverters
{
    [ValueConversion(typeof(object), typeof(SolidColorBrush))]
    public class NumericValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                          System.Globalization.CultureInfo culture)
        {
            string format = value.ToString();
            if (!string.IsNullOrEmpty(format) && (format.Contains('-') || format.Contains('(')))
            {
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
