using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
//using System.Drawing;
using System.Windows.Media;



namespace RazerConverters
{
    /// <summary>
    /// This converter class is used to set the row colors on a grid
    /// The colors set are determined by an integer value passed in
    /// The color values are as follows.
    /// 0 = No Color
    /// 1 = Green
    /// 2 = Yellow
    /// 3 = Red
    /// 4 = Teal
    /// Any other value = No Color
    /// For details on how to add the converter to the XAML for the grid look at the ucAttachmentTab.xaml file in RazerBase
    /// </summary>
    public class cAttachmentHighlightConverter:IValueConverter 
    {
         public object  Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
                int i = 0;

                //Convert the passed object to an integer
                if (value != null)
                {
                    bool result = Int32.TryParse(value.ToString(), out i);
                }
                if (targetType == typeof(System.Windows.Media.Color))
                {
                    //Determine the color to use based on the status value
                    //This branch populates the infragistics grid color type
                    switch (i)
                    {
                        case 1:
                            return Colors.LightGreen;

                        case 2:
                            return Colors.PaleGoldenrod   ;

                        case 3:
                            return Colors.IndianRed  ;

                        case 4:
                            return Colors.PaleTurquoise   ;

                        default:
                            return Binding.DoNothing;

                    }
                }
                else
                {
                    //Determine the color to use based on the status value
                    //this path populates the WPF brush color type
                    switch (i)
                    {
                        //case 1:
                        //    return Brushes.LightGreen;

                        //case 2:
                        //    return Brushes.PaleGoldenrod  ;

                        //case 3:
                        //    return Brushes.IndianRed;

                        //case 4:
                        //    return Brushes.PaleTurquoise  ;

                        //default:
                        //    return Binding.DoNothing;
                        case 1:
                            return cColorMaster.GreenColor;

                        case 2:
                            return cColorMaster.YellowColor ;

                        case 3:
                            return cColorMaster.RedColor ;

                        case 4:
                            return cColorMaster.TealColor ;

                        default:
                            return Binding.DoNothing;

                    }
                }




            //if (i == 0)
            //{
            //   // return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            //    return Binding.DoNothing  ;
            //}
            //else if(i==1)
            //    {
            //        //return Binding.DoNothing;
            //        // return new SolidColorBrush(Color.FromRgb(255, 255, 0));
            //        return Colors.LightGreen ;
            //    }
            //    else if(i


        }

         public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing ;
        }
    }
    public enum StatusColorConverter { Editable, NonEditable };
}

