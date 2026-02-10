using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace RazerConverters
{
    public class BitToBoolConverter : MarkupExtension, IValueConverter
    {
        //creates a singleton converter that can be used across the app without any resource lookup
        private static BitToBoolConverter _converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null)
            {
                _converter = new BitToBoolConverter();
            }
            return _converter;
        }

        //Convert bit (0,1) to bool
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    switch (value.ToString())
                    {
                        case "0":
                            return false;
                        case "1":
                            return true;
                        default:
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Convert bool back to bit (0,1)
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    switch ((bool)value)
                    {
                        case false:
                            return 0;
                        case true:
                            return 1;
                        default:
                            return 0;
                    }
                }
                else
                {
                    return 0;
                } 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
