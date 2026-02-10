#region using statements

using System.Data;
using System;

#endregion


namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucLookupEditorTestScreen.xaml
    /// </summary>
    #region class ucLookupEditorTestScreen
    /// <summary>
    /// Interaction logic for ucLookupEditorTestScreen.xaml
    /// </summary>
    public partial class ucLookupEditorTestScreen : ScreenBase
    {

        #region Private Variables
        private DataRowView customerData;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a LookupEditorTestScreen object
        /// </summary>
        public ucLookupEditorTestScreen()
        {
            // Create Controls
            InitializeComponent();
        }
        #endregion

        #region Events

        #region ScreenBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        /// <summary>
        /// This event is fired after this control is loaded
        /// </summary>
        private void ScreenBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Perform Initializations
            Init();
        }
        #endregion

        #endregion

        #region Methods

        #region CreateCategoryData()
        /// <summary>
        /// This method creates a sample Datatable for testing the lookup control
        /// </summary>
        private DataTable CreateCategoryData()
        {
            // initial value
            DataTable dataTable = new DataTable();

            // add the columns
            dataTable.Columns.Add("CategoryID", typeof(Int32));
            dataTable.Columns.Add("CategoryName", typeof(string));

            // create a data row
            DataRow dataRow = dataTable.NewRow();

            // Add each category
            dataRow["CategoryID"] = 1;
            dataRow["CategoryName"] = "Cable TV";
            dataTable.Rows.Add(dataRow);

            // create a new data row
            dataRow = dataTable.NewRow();

            // Add this category
            dataRow["CategoryID"] = 2;
            dataRow["CategoryName"] = "Electronics";
            dataTable.Rows.Add(dataRow);

            // create a new data row
            dataRow = dataTable.NewRow();

            // Add this category
            dataRow["CategoryID"] = 3;
            dataRow["CategoryName"] = "Media";
            dataTable.Rows.Add(dataRow);

            // return value
            return dataTable;
        }
        #endregion

        #region CreateCustomerData()
        /// <summary>
        /// This method creates a SampleDataTable with one row representing a Customer.
        /// </summary>
        private DataRowView CreateCustomerData()
        {
            // initial value
            DataRowView dataRowView = null;

            // Create a DataTable
            DataTable dataTable = new DataTable();

            // add the columns
            dataTable.Columns.Add("CustomerID", typeof(Int32));
            dataTable.Columns.Add("CustomerName", typeof(string));
            dataTable.Columns.Add("CategoryID", typeof(Int32));
            dataTable.Columns.Add("SecurityRole", typeof(string));

            // Create a DataRow
            DataRow dataRow = null;

            // create a data row
            dataRow = dataTable.NewRow();

            // Now set the values for this DataRow
            dataRow["CustomerID"] = 1;
            dataRow["CustomerName"] = "Joe Customer";
            dataRow["CategoryID"] = 2;
            dataRow["SecurityRole"] = "AdminUser";

            // now add this row to the table
            dataTable.Rows.Add(dataRow);

            // set the return value
            dataRowView = dataTable.DefaultView[0];

            // return value
            return dataRowView;
        }
        #endregion

        #region CreateSecurityRoles()
        /// <summary>
        /// This method creates a sample data table that uses a string key
        /// </summary>
        private DataTable CreateSecurityRoles()
        {
            // initial value
            DataTable securityRoles = new DataTable();

            // add the columns
            securityRoles.Columns.Add("SecurityRole", typeof(string));
            securityRoles.Columns.Add("RoleName", typeof(string));

            // create a data row
            DataRow dataRow = securityRoles.NewRow();

            // Add each Security Role
            dataRow["SecurityRole"] = "InternUser";
            dataRow["RoleName"] = "Intern";
            securityRoles.Rows.Add(dataRow);

            // create a new data row
            dataRow = securityRoles.NewRow();

            // Add each Security Role
            dataRow["SecurityRole"] = "EntryLevel";
            dataRow["RoleName"] = "New Hire";
            securityRoles.Rows.Add(dataRow);

            // create a new data row
            dataRow = securityRoles.NewRow();

            // Add each Security Role
            dataRow["SecurityRole"] = "AdminUser";
            dataRow["RoleName"] = "Administrator";
            securityRoles.Rows.Add(dataRow);

            // create a new data row
            dataRow = securityRoles.NewRow();

            // Add each Security Role
            dataRow["SecurityRole"] = "ExecutiveUser";
            dataRow["RoleName"] = "Management";
            securityRoles.Rows.Add(dataRow);

            // return value
            return securityRoles;
        }
        #endregion

        #region Init()
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // ************************************************
            // **********  Setup the Binding For Category  **********
            // ************************************************

            // The binding must be set for the Lookup before the Binding is set 
            // to display the current record so that the LookupValue and
            // the LookupText can be replaced

            // Create the BindingObject for the CategoryLookup
            BindingObject bindingObject = new BindingObject();

            // You must set the SourceData. Here we are creating sample a sample category DataTable
            bindingObject.SourceData = CreateCategoryData();

            // Set the DisplayField and the ValueField (this can be the same field)
            bindingObject.DisplayField = "CategoryName";
            bindingObject.ValueField = "CategoryID";

            // Set the title for the window
            bindingObject.Title = "Select Category";

            // The CallBackMethod does not need to be set here 
            // because we are using the ucLookupTextBox.
            // Look in the ucLookupTextBox.LookupValueSelected method
            // to see an example of implementing your own CallBackMethod.
            // this.CategoryTextBox.CallBackMethod = [CallBackMethodName];

            // The name is only needed if the CallBackMethod is handling multiple
            // Lookup selections, so you can set the returned values to the 
            // correct controls.
            // bindingObject.Name = "CategorySelector";

            // Set the binding object
            this.CategoryTextBox.BindingObject = bindingObject;

            // ************************************************
            // ********  Setup the Binding For PrimaryRole  **********
            // ************************************************

            // Create the BindingObject for the CategoryLookup
            bindingObject = new BindingObject();

            // You must set the SourceData. Here we are creating sample a sample DataTable
            bindingObject.SourceData = CreateSecurityRoles();

            // Set the DisplayField and the ValueField (this can be the same field)
            bindingObject.DisplayField = "RoleName";
            bindingObject.ValueField = "SecurityRole";

            // Set the title for the window
            bindingObject.Title = "Select Security Role";

            // This bindingObject uses a string key
            bindingObject.LookupAsString = true;

            // The CallBackMethod does not need to be set here 
            // because we are using the ucLookupTextBox.
            // Look in the ucLookupTextBox.LookupValueSelected method
            // to see an example of implementing your own CallBackMethod.
            // this.CategoryTextBox.CallBackMethod = [CallBackMethodName];

            // The name is only needed if the CallBackMethod is handling multiple
            // Lookup selections, so you can set the returned values to the 
            // correct controls.
            // bindingObject.Name = "CategorySelector";

            // Set the binding object
            this.SecurityRoleTextBox.BindingObject = bindingObject;

            // ************************************************
            // *******  Display The Binding For This Control *********
            // ************************************************

            // create the customer data
            this.CustomerData = CreateCustomerData();

            // set the binding
            this.DataContext = this.CustomerData;
        }
        #endregion

        #endregion

        #region Properties

        #region CustomerData
        /// <summary>
        /// This property gets or sets the value for 'CustomerData'.
        /// </summary>
        public DataRowView CustomerData
        {
            get { return customerData; }
            set { customerData = value; }
        }
        #endregion

        #region HasCustomerData
        /// <summary>
        /// This property returns true if this object has a 'CustomerData'.
        /// </summary>
        public bool HasCustomerData
        {
            get
            {
                // initial value
                bool hasCustomerData = (this.CustomerData != null);

                // return value
                return hasCustomerData;
            }
        }
        #endregion

        #endregion

    }
    #endregion
}
