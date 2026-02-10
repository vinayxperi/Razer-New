using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System;

namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucLabelDatePicker.xaml
    /// </summary>
    public partial class ucLabelDatePicker : UserControl
    {
        private string labelText;
        private GridLength labelWidth;
        private string bindPath;
        private const int DefaultLabelWidth = 84;
        public List<ValidationRule> customRules = new List<ValidationRule>();
        //Delgate used for handling Date Changed event
        public delegate void SelectedDateChanged();

        public ucLabelDatePicker()
        {
            InitializeComponent();
            Init();
            //DataContextChanged += new DependencyPropertyChangedEventHandler(ucLabelDatePicker_DataContextChanged);
        }

        //Delgate used for handling Date Changed event
        public SelectedDateChanged mSelectedDateChangedDelegate;
        public SelectedDateChanged SelectedDateChangedDelegate
        {
            get { return mSelectedDateChangedDelegate; }
            set { mSelectedDateChangedDelegate = value; }
        }

        public void Init()
        {
            // Set the label width to the default label width
            this.LabelWidth = new GridLength(DefaultLabelWidth);
            EnterActsLikeTab = false;
            IsReadOnly = false;
            //DateTime dt = new DateTime();
            //dt = Convert.ToDateTime("01/01/1900");
            SetValue(TextProperty, null);
        }
        public bool EnterActsLikeTab { get; set; }
        public DateTime? SelText
        {
            get { return (DateTime?)GetValue(TextProperty); }
            set { SetValue(TextProperty, value);}
        }
        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("SelText", typeof(DateTime?), typeof(ucLabelDatePicker),
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
                // set the value
                bindPath = value;

                // if the bindPath is set
                if (bindPath != null)
                {
                    // Set the value
                    Binding binding = new Binding(bindPath);
                   //binding.Source = DataContext; 

                    // Set to two way mode
                    //binding.Mode = this.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
                    binding.Mode = BindingMode.TwoWay;

                    // Set the update source trigger
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    // set the binding
                    this.SetBinding(TextProperty, binding);
                }
            }
        }



        private bool isReadOnly;
        /// <summary>
        /// Sets the readonly value of the textbox in the user control
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                if (isReadOnly)
                    DatePicker.IsEnabled = false;
                else
                    DatePicker.IsEnabled = true;
                
            }

        }
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
                this.DatePicker.Width = this.Width - this.Label.Width - 4;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDateChangedDelegate != null)
                SelectedDateChangedDelegate();
        }

        //private void ucLabelDatePicker_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    BindPath = bindPath;
        //}
        private int _dpTabIndex;

        public int CntrlTabIndex 
        {
            get { return _dpTabIndex; }
            set
            {
                _dpTabIndex = value;
                DatePicker.TabIndex = value;
            }
        }
        public void CntrlFocus()
        {
            DatePicker.Focus();
        }

 
    }
}
