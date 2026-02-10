using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RazerConverters
{
    public class cCreditStatusConverter : IValueConverter
    {


        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int i = 0;
                bool result = Int32.TryParse(value.ToString(), out i);

                switch (i)
                {
                    case 2:
                        // Bad Debt

                        return System.Drawing.Color.Red;
                    case 3:
                        // Deferred
                        return System.Drawing.Color.Yellow;
                    default:
                        return Binding.DoNothing;
                }
            }
            else
            {
                return Binding.DoNothing;
            }


        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
