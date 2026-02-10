using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace RazerConverters
{
    public static class cColorMaster
    {
        public static SolidColorBrush RedColor = new SolidColorBrush(Colors.IndianRed);
        public static SolidColorBrush YellowColor = new SolidColorBrush(Colors.PaleGoldenrod);
        public static SolidColorBrush TealColor = new SolidColorBrush(Colors.PaleTurquoise);
        public static SolidColorBrush GreenColor = new SolidColorBrush(Colors.LightGreen);

        static cColorMaster()
        {
            RedColor.Freeze();
            YellowColor.Freeze();
            TealColor.Freeze();
            GreenColor.Freeze();
        }
    }
}
