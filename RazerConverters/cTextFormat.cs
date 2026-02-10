using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Microsoft.VisualBasic;

namespace RazerConverters
{
    public class cTextFormat : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (parameter != null)
            {
                return Strings.Format(value, parameter.ToString());
            }

            return value;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (object.ReferenceEquals(targetType, typeof(System.DateTime)) || object.ReferenceEquals(targetType, typeof(Nullable<System.DateTime>)))
            {
                if (Information.IsDate(value))
                {
                    return (DateTime)(value);
                }
                else if (string.IsNullOrEmpty(value.ToString()))
                {
                    return null;
                }
                else
                {
                    Interaction.MsgBox("Please enter a valid date in the form of X/X/XX");
                    return DateAndTime.Now;
                    //invalid type was entered so just give a default.
                }
            }
            else if (object.ReferenceEquals(targetType, typeof(decimal)))
            {
                if (Information.IsNumeric(value))
                {
                    return (decimal)(value);
                }
                else
                {
                    return 0;
                }
            }

            return value;
        }

    }
}
