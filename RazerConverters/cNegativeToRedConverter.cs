using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
//using System.Drawing;
using System.Windows.Media;
using System.Windows.Markup;

namespace RazerConverters
{
    public class cNegativeToRedConverter:IValueConverter 
    //public class cNegativeToRedConverter : MarkupExtension, IValueConverter 
    {
        //creates a singleton converter that can be used across the app without any resource lookup
        //private static BitToBoolConverter _converter = null;
        //public override object ProvideValue(IServiceProvider serviceProvider)
        //{
        //    if (_converter == null)
        //    {
        //        _converter = new BitToBoolConverter();
        //    }
        //    return _converter;
        //}

        public object  Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
                float f = 0;

                //Convert the passed object to an integer
                if (value == null)
                {
                    return Binding.DoNothing ;
                }

                bool result = float.TryParse(value.ToString(), out f);

                if (f == null)
                    return Binding.DoNothing;

                if (targetType == typeof(System.Windows.Media.Color))
                {
                    //Determine the color to use based on the status value
                    //This branch populates the infragistics grid color type
                    if (f < 0)
                        return Colors.Red;
                    else
                        return Colors.Black;

                }
                else
                {
                    //Determine the color to use based on the status value
                    //this path populates the WPF brush color type
                         if(f<0)
                            return Brushes.Red;
                         else

                            return Colors.Black ;

 
                }




        }

         public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing ;
        }

    }
}
