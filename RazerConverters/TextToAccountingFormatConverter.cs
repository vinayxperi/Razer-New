using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace RazerConverters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class TextToAccountingFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                          System.Globalization.CultureInfo culture)
        {
            double numericValue;
            if (Double.TryParse(value.ToString(), out numericValue))
            {
                return String.Format("{0:$###,##0.00;($###,##0.00)}", numericValue);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
        {
            return value.ToString().Replace("$", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
        }
    }
}
