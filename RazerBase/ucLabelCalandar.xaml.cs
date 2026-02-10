

#region using statements

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RazerInterface;

#endregion

namespace RazerBase
{

    #region class ucLabelCalendar
    /// <summary>
    /// Interaction logic for ucLabelCalendar.xaml
    /// </summary>
    public partial class ucLabelCalendar : UserControl, IFieldValueControl
    {
        
        #region Private Variables
        private GridLength labelWidth;
        private string labelText;
        private string bindPath;
        private string selectedDate;
        private double DefaultLabelWidth = 84;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucLabelCalendar' object.
        /// </summary>
        public ucLabelCalendar()
        {
            // Create controls
            InitializeComponent();

            // Perform initializations for this object.
            Init();
        }
        #endregion

        #region Methods

            #region Init()
            /// <summary>
            /// This method performs initializations for this object.
            /// </summary>
            public void Init()
            {
              // Set the label width to the default label width
               this.LabelWidth = new GridLength(DefaultLabelWidth);
            }
            #endregion
        #endregion

        #region Events

            #region OnPropertyChanged(string propertyName)
            /// <summary>
            /// This event is fired when a bound property has changes on this object
            /// </summary>
            protected void OnPropertyChanged(string propertyName)
            {
                // if the PropertyChanged event handler is set
                if (PropertyChanged != null)
                {
                    // Raise the PropertyChanged Event
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            #endregion

            #region OnDatePropertyChanged(DependencyPropertyChangedEventArgs e)
            /// <summary>
            /// This event is fired when the Date changes
            /// </summary>
            private void OnDatePropertyChanged(DependencyPropertyChangedEventArgs e)
            {
                // Set the Datet of this object to the new value 
                // this.Calendar.Date = e.NewValue.ToString();
            }
            #endregion

            #region OnDatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            /// <summary>
            /// This event is fired when the bound object changes
            /// </summary>
            private static void OnDatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            {
                // Cast the depency object as a labelCalendar control
                ucLabelCalendar labelCalendar = dependencyObject as ucLabelCalendar;

                // if the cast was successful
                if (labelCalendar != null)
                {
                    // Now notify the events on the control
                    labelCalendar.OnPropertyChanged("SelectedDate");
                    labelCalendar.OnDatePropertyChanged(e);
                }
            }
            #endregion

            #region PropertyChanged
            /// <summary>
            /// This event is raised when the SelectedValue changes
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
            
            #region Calendar_DateChanged(object sender, DateChangedEventArgs e)
            /// <summary>
            /// This event is fired when the Date changes in the Calendar
            /// </summary>
            private void Calendar_DateChanged(object sender, EventArgs e)
            {
                //// Set the text
                this.SelectedDate = DatePicker.SelectedDate.Value.ToShortDateString();
            }
            #endregion
            
        #endregion

        #region Properties
    
            #region BindPath
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

                    // Set the value
                    Binding binding = new Binding(value);

                    // Set to two way mode
                    binding.Mode = BindingMode.TwoWay;

               
                    // set the binding
                    this.SetBinding(DateProperty, binding);
                }
            }
            #endregion


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
            public string SelectedDate
            {
                get { return (string)GetValue(DateProperty); }
                set
                {
                    if (value == "1/1/1900")
                    {
                        selectedDate = null;
                      
                    }
                    else 
                    {
                        selectedDate = value;
                      
                       
                    } 
                    SetValue(DateProperty, selectedDate);
                }
            }
            public static readonly DependencyProperty DateProperty = DependencyProperty.Register("SelectedDate", typeof(string), typeof(ucLabelCalendar));

   
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
                //   this.Calendar.Width = this.Width - this.Label.Width - 4;
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
                  
                }

            }

         
            /// <summary>
            /// This is a DepencyProperty so that this object can be bound
            /// </summary>
                   
        #endregion
        
    }
    #endregion

}
