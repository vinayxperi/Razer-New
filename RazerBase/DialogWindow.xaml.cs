#region using statements

using System.Windows;
using System.Data;

#endregion


namespace RazerBase
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    #region class DialogWindow
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// This constructor [enter description here].
        /// </summary>
        public DialogWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events

            #region Window_Activated(object sender, System.EventArgs e)
            /// <summary>
            /// This event is fired when this control is activated
            /// </summary>
            private void Window_Activated(object sender, System.EventArgs e)
            {
                // Set the focus to the TextBox
                this.ListSelector.SetFocusToTextBox();
            }
            #endregion
            
            #region Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
            /// <summary>
            /// This event is fired when the user enters text
            /// </summary>
            private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
            {
                // get the key that was just typed in
                string key = e.Key.ToString();

                // if the Escape Key was pressed
                if (key == "Escape")
                {
                    // Close this window without a selection being made
                    this.Close();
                }
                else if (key == "Return")
                {
                    // if there is not a selected item, and there is only one item to select, select that item
                    if ((!this.ListSelector.HasSelectedDataRowView) && (this.ListSelector.HasOnlyOneSelection))
                    {
                        // Set the SelectedDataRowView
                        this.ListSelector.SelectedDataRowView = this.ListSelector.SelectionListBox.Items[0] as DataRowView;
                    }
                    
                    // if there is a SelectedDataRowView
                    if (this.ListSelector.HasSelectedDataRowView)
                    {
                        // if the ListSelector has all the required information
                        if (this.ListSelector.CanSelectItem)
                        {
                            // Select the item
                            this.ListSelector.SelectItem();
                        }
                    }
                }
            }
            #endregion
            
        #endregion

        #region Methods

            #region SetBinding(BindingObject binding)
            /// <summary>
            /// This method sets the binding object on the ucListSelector control
            /// </summary>
            public void SetBinding(BindingObject binding)
            {
                // verify the binding exists
                if (binding != null)
                {
                    // display the title
                    this.Title = binding.Title;

                    // Set the binding 
                    this.ListSelector.Binding = binding;
                }
            }
            #endregion

            #region SetBinding(BindingObject binding, string filterText)
            /// <summary>
            /// This method sets the binding object on the ucListSelector control
            /// </summary>
            /// <param name="binding"></param>
            /// <param name="filterText"></param>
            public void SetBinding(BindingObject binding, string filterText)
            {
                // verify the binding exists
                if (binding != null)
                {
                    // display the title
                    this.Title = binding.Title;

                    // Set the binding 
                    this.ListSelector.Binding = binding;

                    // Set the FilterText on the ListSelector control
                    this.ListSelector.SetFilterText(filterText);
                }
            }
            #endregion

        #endregion

    }
    #endregion
}
