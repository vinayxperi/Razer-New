using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Infragistics.Windows.Editors;
using System.Windows;

namespace RazerConverters
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateTimeToNullableDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (!(value.GetType() == typeof(DateTime))) { return value; }
            
            DateTime minDate = DateTime.Parse("1/2/1900");
            if ((DateTime)value <= minDate)
            {
                return string.Empty;
            }
            else
            {
                return ((DateTime)value).ToShortDateString() ;
            }
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(DateTime)) { return value; }

            string strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
