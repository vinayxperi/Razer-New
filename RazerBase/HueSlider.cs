using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

namespace RazerBase
{
    public class HueSlider : Slider
    {
        //Class for a hue slider control used by the RAZER application to set color schemes
        //One of 9 schemes can be selected
        //The control can be expanded to allow an unlimited number of colors by removing the 
        //slider stops

       // private BitmapSource ColorGradient;
        private object updateLock = new object();
        //private bool isValueUpdating = false;
        //private bool isFirstTime = true;

        public enum HueColors { Black = 0, Blue = 1, Teal = 2, Green = 3, Yellow = 4, Orange = 5, Red = 6, Purple = 7, Gray = 8 };


        static HueSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HueSlider), new FrameworkPropertyMetadata(typeof(HueSlider)));
        }

        //public static readonly DependencyProperty SelectedColorsProperty =
        //    DependencyProperty.Register("SelectedColor", typeof(Color), typeof(HueSlider),
        //    new UIPropertyMetadata(Colors.Blue, new PropertyChangedCallback(SelectedColorChangedCallBack)));

        public HueSlider()
        {
            //This is used to select a large number of colors
            this.Minimum = 0;
            this.Maximum = 1000;
            this.LargeChange = 50;
            this.SmallChange = 5;

            //This statement sets the 9 colors that are currently being used
            this.Background = new LinearGradientBrush(new GradientStopCollection() 
            { 
                new GradientStop(Colors.Black, 0.0),
                new GradientStop(Colors.Blue, 0.125),
                new GradientStop(Colors.Teal, 0.25),
                new GradientStop(Colors.Green, 0.375),
                new GradientStop(Colors.Yellow, 0.50),
                new GradientStop(Colors.Orange, 0.625),
                new GradientStop(Colors.Red, 0.75),
                new GradientStop(Colors.Purple , .875),
                new GradientStop(Colors.LightGray , 1.0)
            });
        }

        /// <summary>
        /// Returns the actual color based on the position in the gradient - Not currently in use for RAZER
        /// </summary>
        //public Color SelectedColor
        //{
        //    get { return (Color)this.GetValue(SelectedColorsProperty); }
        //    set { this.SetValue(SelectedColorsProperty, value); }
        //}

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);

        //    if (this.isFirstTime)
        //    {
        //        this.CacheBitmap();
        //        this.SetColor(this.SelectedColor);
        //        this.isFirstTime = false;
        //    }
        //}

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            ResourceDictionary d = new ResourceDictionary();
            string SourceString = this.GetRAZERColorSchemeName();

            d.Source = new Uri(SourceString, UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(d);
        }

        //protected override void OnValueChanged(double oldValue, double newValue)
        //{
        //    // prevent value change occurring when we set the Color
        //    if (Monitor.TryEnter(this.updateLock, 0) && !this.isValueUpdating)
        //    {
        //        try
        //        {
        //            this.isValueUpdating = true;

        //            double width = this.VisualBounds.Width;
        //            if (width != double.NegativeInfinity)
        //            {
        //                // work out the track position based on the control's width
        //                int position = (int)(((newValue - base.Minimum) / (base.Maximum - base.Minimum)) * width);

        //                this.SelectedColor = GetColor(this.ColorGradient, position);
        //            }
        //        }
        //        finally
        //        {
        //            isValueUpdating = false;
        //            Monitor.Exit(this.updateLock);
        //        }
        //    }

        //    base.OnValueChanged(oldValue, newValue);
        //}

        //protected Rect VisualBounds
        //{
        //    get { return VisualTreeHelper.GetDescendantBounds(this); }
        //}

        #region Private Methods

        /// <summary>
        /// Establishes the starting point on the color selector
        /// </summary>
        /// <param name="Color"></param>
        //private void SetColor(Color Color)
        //{
        //    if (Monitor.TryEnter(this.updateLock, 0) && !this.isValueUpdating)
        //    {
        //        try
        //        {
        //            Rect bounds = this.VisualBounds;
        //            double currentDistance = int.MaxValue;
        //            int currentPosition = -1;

        //            for (int i = 0; i < bounds.Width; i++)
        //            {
        //                Color c = this.GetColor(this.ColorGradient, i);
        //                double distance = c.Distance(Color);

        //                if (distance == 0.0)
        //                {
        //                    //we cannot get a better match, break now
        //                    currentPosition = i;
        //                    break;
        //                }

        //                if (distance < currentDistance)
        //                {
        //                    currentDistance = distance;
        //                    currentPosition = i;
        //                }
        //            }

        //            base.Value = (currentPosition / bounds.Width) * (base.Maximum - base.Minimum);
        //        }
        //        finally
        //        {
        //            Monitor.Exit(updateLock);
        //        }
        //    }
        //}

        //private Color GetColor(BitmapSource bitmap, int position)
        //{
        //    if (position >= bitmap.Width - 1)
        //    {
        //        position = (int)bitmap.Width - 2;
        //    }

        //    CroppedBitmap cb = new CroppedBitmap(bitmap, new Int32Rect(position, (int)this.VisualBounds.Height / 2, 1, 1));
        //    byte[] triColor = new byte[4];

        //    cb.CopyPixels(triColor, 4, 0);
        //    Color c = Color.FromRgb(triColor[2], triColor[1], triColor[0]);

        //    return c;
        //}

        //private void CacheBitmap()
        //{
        //    Rect bounds = this.VisualBounds;
        //    RenderTargetBitmap source = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);
        //    //RenderTargetBitmap source = new RenderTargetBitmap(250, 15, 96, 96, PixelFormats.Pbgra32);
        //    DrawingVisual dv = new DrawingVisual();

        //    using (DrawingContext dc = dv.RenderOpen())
        //    {
        //        VisualBrush vb = new VisualBrush(this);
        //        dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
        //    }

        //    source.Render(dv);
        //    this.ColorGradient = source;
        //}

        //private static void SelectedColorChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        //{
        //    HueSlider ColorSlider = (HueSlider)property;
        //    Color Color = (Color)args.NewValue;

        //    ColorSlider.SetColor(Color);
        //}


        /// <summary>
        /// Method returns the rovi style name of the color selected by a HueSlider
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public string GetRAZERColorSchemeName()
        {
            string SourceString = "";
            switch (Convert.ToInt16(this.Value))
            {
                case (int)HueSlider.HueColors.Black:
                    {
                        SourceString = "Black";
                        break;
                    }
                case (int)HueSlider.HueColors.Blue:
                    {
                        SourceString = "Blue";
                        break;
                    }

                case (int)HueSlider.HueColors.Teal:
                    {
                        SourceString = "Teal";
                        break;
                    }

                case (int)HueSlider.HueColors.Green:
                    {
                        SourceString = "Green";
                        break;
                    }
                case (int)HueSlider.HueColors.Yellow:
                    {
                        SourceString = "Yellow";
                        break;
                    }
                case (int)HueSlider.HueColors.Orange:
                    {
                        SourceString = "Orange";
                        break;
                    }
                case (int)HueSlider.HueColors.Red:
                    {
                        SourceString = "Red";
                        break;
                    }
                case (int)HueSlider.HueColors.Purple:
                    {
                        SourceString = "Purple";
                        break;
                    }
                case (int)HueSlider.HueColors.Gray:
                    {
                        SourceString = "Gray";
                        break;
                    }
            }
            SourceString = "Resources\\Styles\\Style" + SourceString + ".xaml";
            return SourceString;

        }

        #endregion

    }
}
