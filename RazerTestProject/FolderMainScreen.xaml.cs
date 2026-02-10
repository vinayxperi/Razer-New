

#region using statements

using System.Windows.Controls;
using Infragistics.Windows.DockManager;
using RazerBase;
using Infragistics.Windows.Controls;
using System.Data;
using System.Windows;

#endregion

namespace RazerTestProject
{

    #region class FolderMainScreen
    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class FolderMainScreen : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBase constructor is also called.
        /// </summary>
        public FolderMainScreen(cBaseBusObject currentBusinessObject) : base()
        {
            // Create Controls
            InitializeComponent();

            // save the parameter
            this.CurrentBusObj = currentBusinessObject;

            // Perform Initializations 
            Init();
        }
        #endregion

        #region Methods

            #region CreateTestData()
            /// <summary>
            /// This method creates a test DataSet so I can test the binding of the LabelTextBox control
            /// without using the Web Service.
            /// </summary>
            public DataSet CreateTestData()
            {
                // initial value
                DataSet returnDataSet = new DataSet();

                // Now create a data table
                DataTable customerInfo = new DataTable("Customers");

                // Create the columns
                DataColumn firstNameColumn = new DataColumn("FirstName", typeof(string));
                firstNameColumn.MaxLength = 30;
                firstNameColumn.AllowDBNull = false;
                DataColumn lastNameColumn = new DataColumn("LastName", typeof(string));
                lastNameColumn.MaxLength = 30;
                lastNameColumn.AllowDBNull = false;
                DataColumn phoneColumn = new DataColumn("Phone", typeof(string));
                phoneColumn.MaxLength = 20;
                phoneColumn.AllowDBNull = true;
                DataColumn aboutColumn = new DataColumn("About", typeof(string));
                aboutColumn.AllowDBNull = true;

                // Add the columns
                customerInfo.Columns.Add(firstNameColumn);
                customerInfo.Columns.Add(lastNameColumn);
                customerInfo.Columns.Add(phoneColumn);
                customerInfo.Columns.Add(aboutColumn);

                // Now create a test row
                DataRow customerRow = customerInfo.NewRow();

                // set the values for this row
                customerRow[firstNameColumn] = "Tommy";
                customerRow[lastNameColumn] = "TuTone";
                customerRow[phoneColumn] = "8675309";
                customerRow[aboutColumn] = "This blue looks a little too dark to read the text. I think in Multiline textbox the blue gradient should be a little lighter.";

                // Now add the row to the table
                customerInfo.Rows.Add(customerRow);

                // now add the table to the DataSet
                returnDataSet.Tables.Add(customerInfo);
                
                // return value
                return returnDataSet;
            }
            #endregion
            
            #region Init()
            /// <summary>
            /// This method performs initializations for this object.
            /// </summary>
            public void Init()
            {
                // This is a folder
                this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

                // This folder does not have any direct data; All of the data binding is on the tabs
                this.DoNotSetDataContext = true;

                // Add the Tabs
                this.TabCollection.Add(this.ContactInfoTab);
                this.TabCollection.Add(this.Tab2);

                // Call Simulated Load
                this.Load();
            }
            #endregion

            #region Load()
            /// <summary>
            /// This method is here so Load can be faked and simulated data returned.
            /// </summary>
            public new void Load()
            {
                // if the current business object exists
                if (this.HasCurrentBusObj) 
                {
                    // here we are bypassing the webservice and just setting the ojbect data so I can test
                    // the binding for the LabelTextBoxControl without affecting anything.
                    this.CurrentBusObj.ObjectData = CreateTestData();

                    // By default the DataContext is set, unless you turn it off.
                    if ((!this.DoNotSetDataContext) && (this.HasMainTableName))
                    {
                        // Set the DataContext
                        this.DataContext = this.CurrentBusObj.ObjectData.Tables[MainTableName];
                    }
                    
                    // if there are one or more tabs
                    if (this.HasOneOrMoreTabs)
                    {
                        // iterate the tabs
                        foreach (ScreenBase tab in this.TabCollection)
                        {
                            // By default a Tab will set its DataContext if the tab has a MainTableName set
                            if ((!tab.DoNotSetDataContext) && (tab.HasMainTableName))
                            {
                                // Set the DataContext for this tab
                                tab.DataContext = this.CurrentBusObj.ObjectData.Tables[tab.MainTableName];
                            }
                        }
                    }
                }
            }
            #endregion
            
            #region Save()
            /// <summary>
            /// This method is only here to demonstrate validation
            /// </summary>
            public new void Save()
            {
                
            }
            #endregion
            
        #endregion

    }
    #endregion

}
