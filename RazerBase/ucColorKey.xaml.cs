using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucColorKey.xaml
    /// This control is used to add a color key when using the cAttachmentHighlightColor converter
    /// It is automaticallly tied to the colors in the converter
    /// Turn on and off the colors that you want to show by using the Visible functions (i.e. GreenVisible/RedVisible/etc.)
    /// Use the colorname / label properties to set the name of the key
    /// Set the width of all columns with the BaseWidth property - Default is 150
    /// </summary>
    public partial class ucColorKey : UserControl
    {

        private GridLength zeroWidthColumn = new GridLength(0.0);
        private GridLength baseWidth = new GridLength();
        public GridLength BaseWidth
        {
            get { return baseWidth; }
            set
            {
                baseWidth = value;
                foreach (ColumnDefinition cd in gColorKeyDisplay.ColumnDefinitions)
                {
                    cd.Width = baseWidth;
                }
            }
        }

        public ucColorKey()
        {
            InitializeComponent();
            BaseWidth = new GridLength(150.0);
            
        }

   
        private bool greenVisible;
        public bool GreenVisible
        {
            get { return greenVisible; }
            set
            {
                greenVisible = value;
                if (greenVisible)
                {
                    gColorKeyDisplay.ColumnDefinitions[0].Width = baseWidth;
                }
                else
                {
                    gColorKeyDisplay.ColumnDefinitions[0].Width = zeroWidthColumn;
                }
            }
        }

        private bool redVisible;
        public bool RedVisible
        {
            get { return redVisible; }
            set
            {
                redVisible = value;
                if (redVisible)
                {
                    gColorKeyDisplay.ColumnDefinitions[2].Width = baseWidth;
                }
                else
                {
                    gColorKeyDisplay.ColumnDefinitions[2].Width = zeroWidthColumn;
                }
            }
        }

        private bool yellowVisible;
        public bool YellowVisible
        {
            get { return yellowVisible; }
            set
            {
                yellowVisible = value;
                if (yellowVisible)
                {
                    gColorKeyDisplay.ColumnDefinitions[1].Width = baseWidth;
                }
                else
                {
                    gColorKeyDisplay.ColumnDefinitions[1].Width = zeroWidthColumn;
                }
            }
        }

        private bool tealVisible;
        public bool TealVisible
        {
            get { return tealVisible; }
            set
            {
                tealVisible = value;
                if (tealVisible)
                {
                    gColorKeyDisplay.ColumnDefinitions[3].Width = baseWidth;
                }
                else
                {
                    gColorKeyDisplay.ColumnDefinitions[3].Width = zeroWidthColumn;
                }
            }
        }

        private string greenLabel;
        public string GreenLabel
        {
            get { return greenLabel; }
            set
            {
                greenLabel = value;
                ColorKeyLabel1.Content = greenLabel;
            }
        }

        private string redLabel;
        public string RedLabel
        {
            get { return redLabel; }
            set
            {
                redLabel = value;
                ColorKeyLabel3.Content = redLabel;
            }
        }

        private string yellowLabel;
        public string YellowLabel
        {
            get { return yellowLabel; }
            set
            {
                yellowLabel = value;
                ColorKeyLabel2.Content = yellowLabel;
            }
        }

        private string tealLabel;
        public string TealLabel
        {
            get { return tealLabel; }
            set
            {
                tealLabel = value;
                ColorKeyLabel4.Content = tealLabel;
            }
        }
    }
}
