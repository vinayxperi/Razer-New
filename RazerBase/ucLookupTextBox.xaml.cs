using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RazerBase
{

    /// <summary>
    /// Interaction logic for ucLookupTextBox.xaml
    /// </summary>
    public partial class ucLookupTextBox : UserControl
    {
        
        private BindingObject bindingObject;
        public const int DefaultValue = 0;
        private string returnedTextValue;
        private GridLength labelWidth;
        private string labelText;
        
        /// <summary>
        /// Create a new instance of a ucLookupTextBox class
        /// </summary>
        public ucLookupTextBox()
        {
            // Create controls
            InitializeComponent();
            // Perform Initializations for this object
            Init();
        }

        #region Events

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

            /// <summary>
            /// This event [enter description here].
            /// </summary>
            private void OnSelectedValuePropertyChanged(DependencyPropertyChangedEventArgs e)
            {
                // Get the new value of this property
                this.SelectedValue = e.NewValue;

                // if the binding object exists
                if (this.HasBindingObject)
                {
                    // Here we need to replace out the Text with the DisplayField from the data
                    this.Text = this.BindingObject.GetTextDisplay(this.SelectedValue);
                }
            }

            /// <summary>
            /// This event is fired when the bound object changes
            /// </summary>
            private static void OnSelectedValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
            {
                ucLookupTextBox lookupTextBox = dependencyObject as ucLookupTextBox;
                lookupTextBox.OnPropertyChanged("SelectedValue");
                lookupTextBox.OnSelectedValuePropertyChanged(e);
            }

            /// <summary>
            /// This event is raised when the SelectedValue changes
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// This event is fired when the TextBox loses focus
            /// </summary>
            private void TextBox_LostFocus(object sender, RoutedEventArgs e)
            {
                // Get the text that was entered
                string filterText = this.Text;

                // if the TextValue has changed (this is used if a LookupValue was selected fromt the lookup control
                // there is no reason to go back and check if the value entered is valid
                if ((this.HasTextValueChanged) && (this.HasBindingObject))
                {
                    // local variable
                    int itemCount = 0;

                    // if we are doing binding lookup to a String Key
                    if (this.BindingObject.LookupAsString)
                    {
                        // Test if only one object is found
                        string key = this.BindingObject.GetKeyValue(filterText, ref itemCount);

                        // if only one record was found
                        if ((itemCount == 1) && (key != "Error"))
                        {
                            // if the SelectedValueString is different the value that was just found
                            if (this.SelectedValueString != key)
                            {
                                // Set the selected value which will change the text
                                this.SelectedValue = key;
                            }
                            else
                            {
                                // The SelectedValue did not change, but we must make sure the text is correct
                                string displayText = this.BindingObject.GetTextDisplay(key);

                                // Change the text displayed
                                this.Text = displayText;
                            }
                        }
                        else if (itemCount == 0)
                        {
                            // There were not any records found, so the ListSelector 
                            // must be shown without any FilterText set
                            DialogWindow dialogWindow = new DialogWindow();
                            dialogWindow.SetBinding(this.BindingObject);
                            dialogWindow.ShowDialog();
                        }
                        else
                        {
                            // Show the control to find the category
                            DialogWindow dialogWindow = new DialogWindow();
                            dialogWindow.SetBinding(this.BindingObject, filterText);
                            dialogWindow.ShowDialog();
                        }
                    }
                    else
                    {
                        // lookup as integer

                        // Test if only one object is found
                        int selectedValue = this.BindingObject.GetValue(filterText, ref itemCount);

                        // if only one record was found
                        if ((itemCount == 1) && (selectedValue > 0))
                        {
                            // if the SelectedValue is different the value that was just found
                            if (this.SelectedValueInt != selectedValue)
                            {
                                // Set the selected value which will change the text
                                this.SelectedValue = selectedValue;
                            }
                            else
                            {
                                // The SelectedValue did not change, but we must make sure the text is correct
                                string displayText = this.BindingObject.GetTextDisplay(selectedValue);

                                // Change the text displayed
                                this.Text = displayText;
                            }
                        }
                        else if (itemCount == 0)
                        {
                            // There were not any records found, so the ListSelector 
                            // must be shown without any FilterText set
                            DialogWindow dialogWindow = new DialogWindow();
                            dialogWindow.SetBinding(this.BindingObject);
                            dialogWindow.ShowDialog();
                        }
                        else
                        {
                            // Show the control to find the category
                            DialogWindow dialogWindow = new DialogWindow();
                            dialogWindow.SetBinding(this.BindingObject, filterText);
                            dialogWindow.ShowDialog();
                        }
                    }
                }
            }
            
            /// <summary>
            /// This event is used to launch the ListSelector
            /// </summary>
            public void ucLookupTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                // if the binding object does not exist raise an error
                if (!this.HasBindingObject)
                {
                    //don't raise an exception ksh 8/18/11
                    return;
                    // raise an exception for debugging purposes
                    //throw new Exception("The binding object must be set to handle the lookup functionality");
                }

                // Show the control to find the category
                DialogWindow dialogWindow = new DialogWindow();
                dialogWindow.SetBinding(this.BindingObject);
                dialogWindow.ShowDialog();
            }
            private void Button_Click(object sender, RoutedEventArgs e)
            {
                if (!this.HasBindingObject)
                {
                    //don't raise an exception ksh 8/18/11
                    return;
                    // raise an exception for debugging purposes
                    //throw new Exception("The binding object must be set to handle the lookup functionality");
                }

                // Show the control to find the category
                DialogWindow dialogWindow = new DialogWindow();
                dialogWindow.SetBinding(this.BindingObject);
                dialogWindow.ShowDialog();
            }

        #endregion

        #region Methods

            /// <summary>
            /// This method performs initializations for this object.
            /// </summary>
            public void Init()
            {
                // Register Events
                this.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(ucLookupTextBox_MouseDoubleClick);
            }

            /// <summary>
            /// This method [enter description here].
            /// </summary>
            public void SelectionCallBack(DataRowView selectedItem)
            {
                // if the selectedItem existse
                if ((selectedItem != null) && (this.HasBindingObject))
                {
                    // if this BindingObject has a string key
                    if (this.BindingObject.LookupAsString)
                    {
                        // Set the SelectedValue
                        this.SelectedValue = selectedItem.Row[bindingObject.ValueField].ToString();

                        // set the returned text value
                        this.ReturnedTextValue = selectedItem.Row[bindingObject.DisplayField].ToString();
                    }
                    else
                    {
                        object value = selectedItem.Row[bindingObject.ValueField];

                        switch (value.GetType().FullName)
                        {
                            case"System.Byte":
                                this.SelectedValue = ParseNumericForGetTextDisplay<System.Byte>(value);
                                break;
                            case "System.Int32":
                                this.SelectedValue = ParseNumericForGetTextDisplay<System.Int32>(value);
                                break;
                            case "System.Decimal":
                                this.SelectedValue = ParseNumericForGetTextDisplay<System.Decimal>(value);
                                break;
                            default:
                                break;
                        }
                        //this.SelectedValue = Convert.ToInt32(selectedItem.Row[bindingObject.ValueField]);


                        // set the returned text value
                        this.ReturnedTextValue = selectedItem.Row[bindingObject.DisplayField].ToString();
                    }

                    // Set the text
                    this.Text = ReturnedTextValue;
                }
            }

            private T ParseNumericForGetTextDisplay<T>(object value)
            {
                //Convert object value to type T so when cast below the code will not error
                var TValue = Convert.ChangeType(value, typeof(T));
                return (T)TValue;
            }


            /// <summary>
            /// This method attempts to set the display value since we have a SelectedValue
            /// </summary>
            private void SetDisplayValue()
            {
                // if the BindingObject exists
                if (this.HasBindingObject)
                {
                    // get the displaytext
                    string displayText = this.BindingObject.GetTextDisplay(this.SelectedValue);

                    // Set the displayText
                    this.Text = displayText;
                }
            }

        #endregion

        #region Properties

            /// <summary>
            /// This property gets or sets the value for 'BindingObject'.
            /// </summary>
            public BindingObject BindingObject
            {
                get { return bindingObject; }
                set
                {
                    // set the bindingObject
                    bindingObject = value;

                    // if the binding object does not have a CallBackMethod
                    if (!bindingObject.HasCallBackMethod)
                    {
                        // Set the call back method
                        this.BindingObject.CallBackMethod = this.SelectionCallBack;
                    }
                }
            }

            /// <summary>
            /// This property returns true if this object has a 'BindingObject'.
            /// </summary>
            public bool HasBindingObject
            {
                get
                {
                    // initial value
                    bool hasBindingObject = (this.BindingObject != null);

                    // return value
                    return hasBindingObject;
                }
            }

            /// <summary>
            /// This property returns true if the 'ReturnedTextValue' exists.
            /// </summary>
            public bool HasReturnedTextValue
            {
                get
                {
                    // initial value
                    bool hasReturnedTextValue = (!String.IsNullOrEmpty(this.ReturnedTextValue));

                    // return value
                    return hasReturnedTextValue;
                }
            }

            /// <summary>
            /// This property returns true if this object has a 'SelectedValue'.
            /// </summary>
            public bool HasSelectedValue
            {
                get
                {
                    // initial value
                    bool hasSelectedValue = (this.SelectedValue != null);

                    // return value
                    return hasSelectedValue;
                }
            }

            /// <summary>
            /// This property returns true if the 'Text' exists.
            /// </summary>
            public bool HasText
            {
                get
                {
                    // initial value
                    bool hasText = (!String.IsNullOrEmpty(this.Text));

                    // return value
                    return hasText;
                }
            }

            /// <summary>
            /// This property gets or sets the value for 'HasTextValueChanged'.
            /// </summary>
            public bool HasTextValueChanged
            {
                get
                {
                    // initial value (Default to true so a lookup takes place
                    bool hasTextValueChanged = true;

                    // verify the Text and ReturnedTextValue both exist
                    if ((this.HasText) && (this.HasReturnedTextValue))
                    {
                        // set the return value
                        hasTextValueChanged = (this.Text != this.ReturnedTextValue);
                    }

                    // return value
                    return hasTextValueChanged;
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
            /// This property gets or sets the value for 'ReturnedTextValue'.
            /// </summary>
            public string ReturnedTextValue
            {
                get { return returnedTextValue; }
                set { returnedTextValue = value; }
            }

            /// <summary>
            /// This property gets or sets the value for 'SelectedValue'.
            /// </summary>
            public object SelectedValue
            {
                get
                {
                    return GetValue(SelectedValueProperty);
                }
                set
                {
                    SetValue(SelectedValueProperty, value);
                }
            }

            /// <summary>
            /// This read only property returns the Integer value of the 'SelectedValue'.
            /// </summary>
            public int SelectedValueInt
            {
                get
                {
                    // initial value
                    int selectedValueInt = 0;

                    try
                    {
                        // if the SelectedValue exists
                        if (this.HasSelectedValue)
                        {
                            // set the return value
                            selectedValueInt = Convert.ToInt32(this.SelectedValue);
                        }
                    }
                    catch (Exception error)
                    {
                        // for debugging only
                        string err = error.ToString();

                        // set the return value to -1 to indicate an error
                        selectedValueInt = -1;
                    }

                    // return value
                    return selectedValueInt;
                }
            }

            /// <summary>
            /// This is a DepencyProperty so that this object can be bound
            /// </summary>
            public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(object), typeof(ucLookupTextBox), new PropertyMetadata(DefaultValue, OnSelectedValuePropertyChanged));

            /// <summary>
            /// This read only property returns the String value of the 'SelectedValue'.
            /// </summary>
            public string SelectedValueString
            {
                get
                {
                    // initial value
                    string selectedValueString = "";

                    try
                    {
                        // if the SelectedValue exists
                        if (this.HasSelectedValue)
                        {
                            // set the return value
                            selectedValueString = this.SelectedValue.ToString();
                        }
                    }
                    catch (Exception error)
                    {
                        // for debugging only
                        string err = error.ToString();

                        // set the return value to -1 to indicate an error
                        selectedValueString = "Error";
                    }

                    // return value
                    return selectedValueString;
                }
            }

            /// <summary>
            /// This property gets or sets the value for 'Text'.
            /// </summary>
            public string Text
            {
                get { return this.TextBox.Text; }
                set { this.TextBox.Text = value; }
            }

        #endregion

        
    }
}
