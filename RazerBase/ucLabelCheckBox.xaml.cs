using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RazerInterface;

namespace RazerBase
{

    /// <summary>
    /// This class contains a Label and a CheckBox.
    /// </summary>
    public partial class ucLabelCheckBox : UserControl, IFieldValueControl
    {

        private GridLength labelWidth;
        private string labelText;
        private string bindPath;
        private const double DefaultLabelWidth = 84;

        public static readonly RoutedEvent CheckedEvent =
            EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucLabelCheckBox));

        public event RoutedEventHandler Checked
        {
            add { AddHandler(CheckedEvent, value); }
            remove { RemoveHandler(CheckedEvent, value); }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CheckedEvent));
        }

        public static readonly RoutedEvent UnCheckedEvent =
            EventManager.RegisterRoutedEvent("UnChecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucLabelCheckBox));

        public event RoutedEventHandler UnChecked
        {
            add { AddHandler(UnCheckedEvent, value); }
            remove { RemoveHandler(UnCheckedEvent, value); }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(UnCheckedEvent));
        }

        public int IsChecked
        {
            get { return (int)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Checked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(int), typeof(ucLabelCheckBox), 
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        /// <summary>
        /// This property gets or sets the value for 'BindPath'.
        /// </summary>
        public string BindPath
        {
            get { return bindPath; }
            set
            {
                if (value != null)
                {
                    // set the value
                    bindPath = value;

                    // Set the value
                    Binding binding = new Binding(value);

                    binding.Source = DataContext;

                    // Set to two way mode
                    binding.Mode = BindingMode.TwoWay;

                    //Set the update source trigger
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    // set the binding
                    this.SetBinding(IsCheckedProperty, binding);
                }               
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
                this.Label.Width = Convert.ToDouble(value.ToString());
            }
        }

        private bool isEnabled;
        /// <summary>
        /// Sets the readonly value of the textbox in the user control
        /// </summary>
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                this.CheckBox.IsEnabled = value;                
            }
        }

        /// <summary>
        /// Create a new instance of a 'ucLabelTextBox' object.
        /// </summary>
        public ucLabelCheckBox()
        {
            // Create controls
            InitializeComponent();

            // Set the Default Width
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(ucLabelCheckBox_DataContextChanged);
        }

        private void ucLabelCheckBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BindPath = bindPath;

            DataRow dr = DataContext as DataRow;

            if (dr != null)
            {
                IsChecked = Convert.ToInt16(dr[bindPath.TrimStart('[').TrimEnd(']')].ToString());
            }
        }
        private int _tbTabIndex;

        
      

        public int CntrlTabIndex
        {
            get { return _tbTabIndex; }
            set
            {
                _tbTabIndex = value;
                CheckBox.TabIndex = value;
            }
        }
        public void CntrlFocus()
        {
            CheckBox.Focus();
        }

    }
}
