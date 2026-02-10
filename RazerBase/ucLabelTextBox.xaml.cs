using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RazerBase
{
    public partial class ucLabelTextBox : UserControl
    {

        private GridLength labelWidth;
        private string labelText;
        private const int DefaultLabelWidth = 84;
        public List<ValidationRule> customRules = new List<ValidationRule>();
        private string bindPath;
        private int maxlength;

        public delegate void TextDoubleClick();
        public TextDoubleClick DoubleClickDelegate { get; set; }

        public delegate void TextPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e);
        public TextPreviewKeyDown PreviewKeyDownDelegate { get; set; }

        public bool FocusSelectsAll { get; set; } //If true (default) selects all data in text box when textbox is entered

        public HorizontalAlignment TextBoxHorizontalContentAlignment
        {
            get 
            {   
                return TextBox.HorizontalContentAlignment;
            }
            set
            {
                TextBox.HorizontalContentAlignment = value;
            }
        }

        public ucLabelTextBox()
        {
            // Create controls
            InitializeComponent();

            // Perform initializations for this object.
            Init();
        }

        public void Init()
        {
            // Set the label width to the default label width
            this.LabelWidth = new GridLength(DefaultLabelWidth);
            EnterActsLikeTab = false;
            FocusSelectsAll = true;
        }

        /// <summary>
        /// This property gets or sets the value for the textbox MaxLength.
        /// </summary>
        public int MaxLength
        {
            get { return maxlength; }
            set
            {
                // set the value
                maxlength = value;

                // Set the Context of the lable 
                this.TextBox.MaxLength = value;
            }
        }

        /// <summary>
        /// This property gets or sets the value for 'LabelText'.
        /// </summary>
        public string LabelText
        {
            get { return labelText; }
            set
            {
                // set the value
                labelText = value;

                // Set the Context of the lable 
                this.Label.Content = value;
            }
        }

        /// <summary>
        /// This property gets or sets the value for 'LabelWidth'.
        /// </summary>
        public GridLength LabelWidth
        {
            get { return labelWidth; }
            set
            {
                // set the value
                labelWidth = value;

                // set the value of the control
                this.GridLabelColumn.Width = value;
                this.Label.Width = System.Convert.ToInt32(value.ToString());
                this.TextBox.Width = this.Width - this.Label.Width - 4;
            }
        }

        /// <summary>
        /// This property gets or sets the value for 'BindPath'.
        /// </summary>
        public string BindPath
        {
            get { return bindPath; }
            set
            {
                // set the value
                bindPath = value;

                // if the bindPath is set
                if (bindPath != null)
                {
                    // Set the value
                    Binding binding = new Binding(bindPath);

                    // Set to two way mode
                    binding.Mode = BindingMode.TwoWay;

                    // Set the update source trigger
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    // set the binding
                    this.SetBinding(TextProperty, binding);
                }
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ucLabelTextBox),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });


        /// <summary>
        /// True - Enter behaves like the tab key - False Enter behaves as normal - False is the default
        /// </summary>
        public bool  EnterActsLikeTab { get; set; }
        
        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Added DWR -10/24/12 - To allow preview key down on text box and not user control
            if (PreviewKeyDownDelegate  != null)
                PreviewKeyDownDelegate(sender,e);

            if (EnterActsLikeTab)
            {
                TextBox tb = e.Source as TextBox;
                if (tb != null)
                {
                    switch (e.Key)
                    {
                        case Key.Enter:
                            tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public void SelectAll()
        {
            TextBox.SelectAll();
        }
        
        private void DoubleClickEventHandler(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClickDelegate != null)
                DoubleClickDelegate();
        }
                
        private bool isReadOnly;
        /// <summary>
        /// Sets the readonly value of the textbox in the user control 
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly  ;}
            set
            {
                isReadOnly = value;
                TextBox.IsReadOnly = value;
                Label.IsTabStop = false;
                TextBox.IsTabStop = false;
            }
        }

        private int _tbTabIndex;

        public int CntrlTabIndex
        {
            get { return _tbTabIndex; }
            set
            {
                _tbTabIndex = value;
                TextBox.TabIndex = value;
            }
        }

        private string _color;

        public string TextColor
        {
            get { return _color; }
            set
            {
                _color = value;
                System.Windows.Media.BrushConverter bc = new System.Windows.Media.BrushConverter();
                TextBox.Foreground = (System.Windows.Media.Brush)bc.ConvertFromString(_color);
            }


        }
        public void CntrlFocus()
        {
            TextBox.Focus();

            
        }

        public virtual void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FocusSelectsAll)
                TextBox.SelectAll();
        }

    }
}