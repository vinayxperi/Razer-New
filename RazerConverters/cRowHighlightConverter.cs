using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;

namespace RazerConverters
{
    public class cRowHighlightConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //int i = (int)value.ToString();
            int i = 0;

            if (value != null)
            {
                bool result = Int32.TryParse(value.ToString(), out i);
            }

            if (i == 1)
            {
                return Color.LightPink;
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
