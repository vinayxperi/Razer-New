using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Data;

namespace RazerBase
{

    /// <summary>
    /// This class is used to place a label and a Multiline TextBox in one control.
    /// </summary>
    public partial class ucLabelMultilineTextBox : UserControl
    {

        private GridLength labelWidth;
        private GridLength textWidth;
        private string labelText;
        public List<ValidationRule> customRules = new List<ValidationRule>();
        private string bindPath;
        private int maxlength;

        /// <summary>
        /// Create a new instance of a 'ucLabelTextBox' object.
        /// </summary>
        public ucLabelMultilineTextBox()
        {
            // Create controls
            InitializeComponent();

            // Perform initializations for this object.
            Init();
        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {

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
        /// This event [enter description here].
        /// </summary>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Set the height
            this.TextBox.Height = this.Height;
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


        public GridLength TextWidth
        {
            get { return textWidth; }
            set 
            { 
                textWidth = value;
                this.TextBox.Width = System.Convert.ToInt32(value.ToString());
            }
        }

        public void CntrlFocus()
        {
            TextBox.Focus();
        }
        private int _dpTabIndex;

        public int CntrlTabIndex
        {
            get { return _dpTabIndex; }
            set
            {
                _dpTabIndex = value;
                TextBox.TabIndex = value;
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

        /// <summary>
        /// This property gets or sets the value for 'Text'.
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// This is a DepencyProperty so that this object can be bound
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ucLabelMultilineTextBox),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
    }

}
