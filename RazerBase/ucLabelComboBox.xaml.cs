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
using System.Collections;
using System.Data;
using System.ComponentModel;

namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucLabelComboBox.xaml
    /// </summary>
    public partial class ucLabelComboBox : UserControl
    {
        private GridLength labelWidth;
        private string labelText;        
        private static readonly string SelectedValuePropertyName = "SelectedValue";        
        
        public ucLabelComboBox()
        {
            InitializeComponent();
            //this.ComboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            UseAutoComplete = true;
        }

        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ucLabelComboBox));

        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
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
                this.Label.Width = System.Convert.ToInt32(value.ToString());                
                this.ComboBox.Width = this.Width - this.Label.Width - 4;                
            }
        }

        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }            
            set
            {
                SetSelectedItem(value);
                SetValue(SelectedValueProperty, value);
            }
        }        

        public string SelectedText
        {
            get { return this.ComboBox.Text; }
            set { this.ComboBox.Text = value; }
        }

        public string DisplayPath
        {
            get { return this.ComboBox.DisplayMemberPath; }
            set { this.ComboBox.DisplayMemberPath = value; }
        }

        public string ValuePath
        {
            get { return this.ComboBox.SelectedValuePath; }
            set { this.ComboBox.SelectedValuePath = value; }
        }

        //This allows the editable abehavior of the combo box and the key up event with autocomplete to work
        private bool mUseAutoComplete;
        public bool UseAutoComplete
        {
            get { return mUseAutoComplete; }
            set
            {
                mUseAutoComplete = value;
                this.ComboBox.IsEditable = mUseAutoComplete;
            }
        }

        public void SetBindingExpression(string valueField, string displayField, DataTable dataSource, string bindPath = "", BindingMode bindingMode = BindingMode.OneWayToSource)
        {
            //string path = string.IsNullOrEmpty(bindPath) ? valueField : bindPath;

            //Since the binding sets the default to the first row in data table.
            //var column = dataSource.Columns[valueField];            
            //DataRow row = dataSource.NewRow();            

            //switch (column.DataType.FullName)
            //{
            //    case "System.String":
            //        row[valueField] = string.Empty;
            //        row[displayField] = string.Empty;
            //        dataSource.Rows.InsertAt(row, 0);
            //        break;
            //    default:
            //        row[valueField] = 0;
            //        row[displayField] = string.Empty;
            //        dataSource.Rows.InsertAt(row, 0);
            //        break;
            //}    

            Binding binding = new Binding();
            if (!string.IsNullOrEmpty(bindPath))
            {
                binding.Path = new PropertyPath(bindPath);
                binding.Mode = bindingMode;
                binding.Source = dataSource;
                this.SetBinding(ucLabelComboBox.SelectedValueProperty, binding);
            }

            this.ComboBox.ItemsSource = dataSource.DefaultView;
            this.ItemSource = dataSource.DefaultView;
            this.DisplayPath = displayField;
            this.ValuePath = valueField;

            //this.ComboBox.SelectedValue = "";
            //this.ComboBox.Text = "";
        }        

        public event PropertyChangedEventHandler PropertyChanged;

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
            this.SelectedValue = e.NewValue;
            //SetValue(SelectedValueProperty, e.NewValue);
        }

        private static void OnSelectedValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ucLabelComboBox labelComboBox = dependencyObject as ucLabelComboBox;
            labelComboBox.OnPropertyChanged(SelectedValuePropertyName);
            labelComboBox.OnSelectedValuePropertyChanged(e);           
        }

        private void SetSelectedItem(object selectedValue)
        {
            if (selectedValue == null) return;
            IEnumerable<DataRowView> rows;
            if (selectedValue.GetType() == typeof(string))
            {
                if ((string)selectedValue == string.Empty) return;
 
                rows = from DataRowView rowView in this.ComboBox.ItemsSource
                       where rowView.Row.Field<string>(ValuePath).ToLower() == ((string)selectedValue).ToLower()
                       select rowView;
            }
            else
            {
                if ((int)selectedValue <= 0) return;

                rows = from DataRowView rowView in this.ComboBox.ItemsSource
                       where rowView.Row.Field<int>(ValuePath) == (int)selectedValue
                       select rowView;
            }

            if (rows != null && rows.Count() > 0)
            {
                this.ComboBox.SelectedItem = rows.ElementAt(0);
                //SetValue(SelectedValueProperty, this.ComboBox.SelectedValue);
            }
        }        

        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty = ComboBox.ItemsSourceProperty.AddOwner(typeof(ucLabelComboBox));

        public static readonly DependencyProperty SelectedValueProperty = ComboBox.SelectedValueProperty.AddOwner(typeof(ucLabelComboBox),
            new FrameworkPropertyMetadata()
            {
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,                
                PropertyChangedCallback = OnSelectedValuePropertyChanged,
                Inherits = true,
            });

        private int _cbTabIndex;

        public int CntrlTabIndex 
        {
            get { return _cbTabIndex; }
            set
            {
                _cbTabIndex = value;
                ComboBox.TabIndex = value;
            }
        }
        public void CntrlFocus()
        {
            ComboBox.Focus();
        }

  
        /// <summary>
        /// DWR - Added 2/29/12
        /// This event will check to see if the entered value still has matches in the combo box list.
        /// If it doesn't then the last keyed item will be removed and the previously highlighted item will reappear in the combo box
        /// This only works if a datatable is bound to the combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            //If turned off then exit
            if (!UseAutoComplete)
                return;

            //Get the combobox
            try
            {
                ComboBox cb = sender as ComboBox;
                //Get the bound datatable
                DataTable dt = (cb.ItemsSource as DataView).Table;
                //// Get the textbox part of the combobox         
                TextBox textBox = cb.Template.FindName("PART_EditableTextBox", cb) as TextBox;

                if (textBox == null || textBox.Text == null)
                    return;

                //Query the items in the datatable to see if the text input so far still has a valid value with a starts with
                EnumerableRowCollection<DataRow> ValidItems = from cbItems in dt.AsEnumerable()
                                                              where cbItems.Field<String>(DisplayPath).StartsWith(textBox.Text)
                                                              select cbItems;

                //If any one value is returned, then the text is valid so exit this event
                foreach (DataRow r in ValidItems)
                {
                    return;
                }

                //If no valid values match the text entered remove the last character typed
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                if (textBox.Text.Length == 0)
                    return;
    
                textBox.CaretIndex = textBox.Text.Length;

                //Query the datatable again to get the last valid value to put back into the combo text box
                ValidItems = from cbItems in dt.AsEnumerable()
                             where cbItems.Field<String>(DisplayPath).StartsWith(textBox.Text)
                             select cbItems;
                
                //Grab the first value retrieved
                foreach (DataRow r1 in ValidItems)
                {
                    //This command highlights the text to the right of the last character entered - this is the standard autocomplete behavior of
                    //the combobox
                    textBox.SelectedText = r1[DisplayPath].ToString().Substring(textBox.CaretIndex, r1[DisplayPath].ToString().Length - textBox.CaretIndex);
                    return;
                }
            }
                //If any error then exit the event
            catch
            {
                return;
            }


  
        } 

    }
}
