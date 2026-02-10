

#region using statements

using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Data;
using System;
using System.Windows;

#endregion

namespace RazerBase
{
    
    #region class ListSelector : UserControl
    /// <summary>
    /// Interaction logic for ListSelector.xaml
    /// </summary>
    public partial class ucListSelector : ScreenBase
    {

        #region Private Variables
        private BindingObject binding;
        private bool closeWindowOnSelection;
        private DataRowView selectedDataRowView;
        #endregion

        #region Constructors

            #region Constructor
            /// <summary>
            /// This constructor creates a new instance of a ucListSelector.
            /// </summary>
            public ucListSelector()
            {
                // Create controls
                InitializeComponent();

                // Perform Initializations
                Init();
            }
            #endregion

            #region Parameterized Constructor
            /// <summary>
            /// Create a new instance of a ucListSelector object.
            /// </summary>
            /// <param name="binding">The current binding for this project</param>
            public ucListSelector(BindingObject binding)
            {
                // Create Controls
                InitializeComponent();

                // Perform initializations for this object
                Init();

                // verify the binding exists
                if (binding != null)
                {
                    // Set the data context
                    this.SelectionListBox.ItemsSource = binding.DisplayField;
                    this.SelectionListBox.DataContext = binding.SourceData.DefaultView;
                }
            }
            #endregion

        #endregion

        #region Events

            #region FilterTextBox_KeyDown(object sender, KeyEventArgs e)
            /// <summary>
            /// This event is here so the Arrow Key Down can send the user to the SelectionListBox.
            /// </summary>
            protected void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
            {
                // if the down arrow key is pressed
                if ((e.Key == Key.Tab) && (this.HasOneOrMoreSelections))
                {
                    // Select the first item
                    this.SelectionListBox.SelectedItem = this.SelectionListBox.Items[0];

                    // Set Focus to the list box so future up down arrow events are handled by the 
                    // SelectionListBox
                    this.SelectionListBox.Focus();
                }
            }
            #endregion
            
            #region FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
            /// <summary>
            /// This event [enter description here].
            /// </summary>
            private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
            {
                // filter the current data
                FilterData();
            }
            #endregion

            #region SelectionListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            /// <summary>
            /// This event is used to call the delegate that an entry was selected
            /// </summary>
            private void SelectionListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                // if the Binding.CallBackMethod exists
                if ((this.HasBinding) && (this.Binding.HasCallBackMethod))
                {
                    // get the selected item
                    DataRowView selectedRowView = this.SelectionListBox.SelectedItem as DataRowView;

                    // if the selectedRowView exists
                    if (selectedRowView != null)
                    {
                        // Call the call back method
                        this.Binding.CallBackMethod(selectedRowView);
                    }

                    // if the ParentWindow should be closed when a selection is made
                    if ((this.HasParentWindow) && (this.CloseWindowOnSelection))
                    {
                        // Close the parent window
                        this.ParentWindow.Close();
                    }
                }
            }
            #endregion

            #region SelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            /// <summary>
            /// This event is fired when a selection is made in the SelectionsListBox.
            /// </summary>
            private void SelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                // if the SelectionsListBox does not have any selections
                if ((this.SelectionListBox.SelectedItems.Count == 0) || (this.SelectionListBox.SelectedItems.Count > 1))
                {
                    // We must have exactly one record selected to have a SelectedDataRowView
                    this.SelectedDataRowView = null;
                }
                else
                {
                    // if there is a selected item
                    if (this.SelectionListBox.SelectedItem != null)
                    {
                        // Set the SelectedDataRowView
                        this.SelectedDataRowView = this.SelectionListBox.SelectedItem as DataRowView;    
                    }
                }
            }
            #endregion
            
        #endregion

        #region Methods

            #region ApplyBinding(DataTable filteredTable)
            /// <summary>
            /// This method [enter description here].
            /// </summary>
            private void ApplyBinding(DataTable filteredTable)
            {
                // if the filterTable
                if ((filteredTable != null) && (this.HasBinding))
                {
                    // Set the DataContext
                    this.SelectionListBox.ItemsSource = filteredTable.DefaultView;
                    this.SelectionListBox.DisplayMemberPath = binding.DisplayField;
                }
            }
            #endregion

            #region ApplyBinding()
            /// <summary>
            /// This method binds the data in the SourceTable to the list box
            /// </summary>
            private void ApplyBinding()
            {
                // if the binding is set
                if (this.HasBinding)
                {
                    // Set the DataContext
                    this.SelectionListBox.ItemsSource = binding.SourceData.DefaultView;
                    this.SelectionListBox.DisplayMemberPath = binding.DisplayField;
                }
            }
            #endregion

            #region FilterData()
            /// <summary>
            /// This method filters the current data
            /// </summary>
            private void FilterData()
            {
                try
                {
                    // verify the binding object has been set
                if (this.HasBinding)
                {
                    // get the filterText
                    string filterText = this.FilterTextBox.Text;

                    // check if we have FilterText
                    bool hasFilterText = (!String.IsNullOrEmpty(filterText));

                    // if FilterText is entered
                    if (hasFilterText)
                    {
                        // create the filteredTable
                        DataTable filteredTable = null;

                        var query = from p in binding.SourceData.AsEnumerable()
                                    where p.Field<string>(binding.DisplayField).IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0
                                    select p;

                        // if there are one or more records
                        if (query.AsDataView().Count > 0)
                        {
                            // create the filteredTable
                            filteredTable = query.CopyToDataTable();
                        }

                        // if the filteredTable exists
                        if (filteredTable != null)
                        {
                            // bind to the SourceData
                            this.ApplyBinding(filteredTable);
                        }
                        else
                        {
                            // Clear all items from the list bxo
                            this.SelectionListBox.ItemsSource = null;
                        }
                    }
                    else
                    {
                        // bind to the SourceData
                        this.ApplyBinding();
                    }
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();

                // show the error for now
                MessageBox.Show("An error occurred filtering your data", "Binding Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

            #region Init()
            /// <summary>
            /// This method performs initializations for this object
            /// </summary>
            public void Init()
            {
                // Default to true
                this.CloseWindowOnSelection = true;

                // Setup the FilterTextBox KeyDown event so the down arrow key can send the 
                // user into the SelectionsListBox
                this.FilterTextBox.KeyDown += new KeyEventHandler(FilterTextBox_KeyDown);
            }
            #endregion

            #region SelectItem()
            /// <summary>
            /// This method is used to select the SelectedDataRowView object.
            /// You should check the property CanSelectItem before calling this method to 
            /// ensure that the item can be selected.
            /// </summary>
            public void SelectItem()
            {
                // if there is a selected data row view object
                if (this.CanSelectItem)
                {
                    // Call the call back method of the Binding object
                    this.Binding.CallBackMethod(this.SelectedDataRowView);

                    // if the ParentWindow should be closed when a selection is made
                    if ((this.HasParentWindow) && (this.CloseWindowOnSelection))
                    {
                        // Close the parent window
                        this.ParentWindow.Close();
                    }
                }
            }
            #endregion
            
            #region SetFilterText(string filterText)
            /// <summary>
            /// This method sets the text for the FilterTextBox
            /// </summary>
            public void SetFilterText(string filterText)
            {
                // Set the text on the FilterTextBox
                this.FilterTextBox.Text = filterText;
            }
            #endregion

            #region SetFocusToTextBox()
            /// <summary>
            /// This method sets the focus to the FilterTextBox.
            /// </summary>
            internal void SetFocusToTextBox()
            {
                // Set the focus to the FilterTextBox
                this.FilterTextBox.Focus();
            }
            #endregion
            
        #endregion

        #region Properties

            #region Binding
            /// <summary>
            /// This property gets or sets the value for 'Binding'.
            /// </summary>
            public BindingObject Binding
            {
                get { return binding; }
                set
                {
                    // set the binding data
                    binding = value;

                    // Bind the data to the list box
                    ApplyBinding();
                }
            }
            #endregion

            #region CanSelectItem
            /// <summary>
            /// This read only property returns true if there is exactly one item selected.
            /// </summary>
            public bool CanSelectItem
            {
                get
                {
                    // initial value
                    bool canSelectItem = false;

                    // if there is a selected data row view object & the binding call back method has been set.
                    if ((this.HasSelectedDataRowView) && (this.HasBinding) && (this.Binding.HasCallBackMethod))
                    {
                        // Set the return value
                        canSelectItem = true;
                    }

                    // return value
                    return canSelectItem;
                }
            }
            #endregion
            
            #region CloseWindowOnSelection
            /// <summary>
            /// This property gets or sets the value for 'CloseWindowOnSelection'.
            /// </summary>
            public bool CloseWindowOnSelection
            {
                get { return closeWindowOnSelection; }
                set { closeWindowOnSelection = value; }
            }
            #endregion

            #region HasBinding
            /// <summary>
            /// This property returns true if this object has a 'Binding'.
            /// </summary>
            public bool HasBinding
            {
                get
                {
                    // initial value
                    bool hasBinding = (this.Binding != null);

                    // return value
                    return hasBinding;
                }
            }
            #endregion

            #region HasOnlyOneSelection
            /// <summary>
            /// This read only property returns true if there is only one item to select.
            /// </summary>
            public bool HasOnlyOneSelection
            {
                get
                {
                    // initial value
                    bool hasOnlyOneSelection = (this.SelectionsCount == 1);

                    // return value
                    return hasOnlyOneSelection;
                }
            }
            #endregion
            
            #region HasParentWindow
            /// <summary>
            /// This property returns true if this object has a 'ParentWindow'.
            /// </summary>
            public bool HasParentWindow
            {
                get
                {
                    // initial value
                    bool hasParentWindow = (this.ParentWindow != null);

                    // return value
                    return hasParentWindow;
                }
            }
            #endregion

            #region HasSelectedDataRowView
            /// <summary>
            /// This property returns true if this object has a 'SelectedDataRowView'.
            /// </summary>
            public bool HasSelectedDataRowView
            {
                get
                {
                    // initial value
                    bool hasSelectedDataRowView = (this.SelectedDataRowView != null);
                    
                    // return value
                    return hasSelectedDataRowView;
                }
            }
            #endregion

            #region HasOneOrMoreSelections
            /// <summary>
            /// This property returns true if the 'SelectionsCount' is greater than 0.
            /// </summary>
            public bool HasOneOrMoreSelections
            {
                get
                {
                    // initial value
                    bool hasOneOrMoreSelections = (this.SelectionsCount > 0);
                    
                    // return value
                    return hasOneOrMoreSelections;
                }
            }
            #endregion
            
            #region ParentWindow
            /// <summary>
            /// This read only property returns the 'ParentWindow'.
            /// </summary>
            public Window ParentWindow
            {
                get
                {
                    // initial value
                    Window parentWindow = Window.GetWindow(this);

                    // return value
                    return parentWindow;
                }
            }
            #endregion

            #region SelectedDataRowView
            /// <summary>
            /// This property gets or sets the value for 'SelectedDataRowView'.
            /// </summary>
            public DataRowView SelectedDataRowView
            {
                get { return selectedDataRowView; }
                set { selectedDataRowView = value; }
            }
            #endregion
            
            #region SelectionsCount
            /// <summary>
            /// This readonly property returns the number of choices in the SelectionsListBox.
            /// </summary>
            public int SelectionsCount
            {
                get
                {
                    // iniitial value
                    int selectionsCount = 0;

                    // if the SelectionsListBox exists
                    if (this.SelectionListBox != null)
                    {
                        // set the return value
                        selectionsCount = this.SelectionListBox.Items.Count;
                    }
                    // return value
                    return selectionsCount;
                }
            }
            #endregion
            
        #endregion

    }
    #endregion
}
