using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
namespace RazerBase
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            DateTime date = (DateTime)value;
            if (date == DateTime.Parse("01/01/1900"))
            {
                return "";
            }
            else if (date == DateTime.Parse("01/01/0001"))
            {
                return "";
            }
            else
            {
                return date.ToShortDateString();
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(DateTime)) { return value; }

                string strValue = value as string;
                DateTime resultDateTime;
                if (DateTime.TryParse(strValue, out resultDateTime))
                {
                    return resultDateTime;
                }

                //DWR Modified 3/26/13 -- If error condition then 1/1/1900 will be returned - Commented out the return UnsetValue
                return "1/1/1900";
                //return DependencyProperty.UnsetValue;
            }
            else
            {
                return null;
            }
        }


    }
}
