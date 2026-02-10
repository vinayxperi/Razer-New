using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace RazerConverters
{
    public class StringTrimConverter : MarkupExtension, IValueConverter
    {
        //creates a singleton converter that can be used across the app without any resource lookup
        private static StringTrimConverter _converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null)
            {
                _converter = new StringTrimConverter();
            }
            return _converter;
        }

        //Trim a string
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    return value.ToString().Trim();
                }
                else
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Send orig value back
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
