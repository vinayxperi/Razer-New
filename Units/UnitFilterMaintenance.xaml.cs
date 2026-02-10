using RazerInterface; //Required for IPreBindable
using RazerBase.Interfaces; //Required for IScreen
using RazerBase;
using RazerBase.Lookups;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DataPresenter;
using System;
using System.Data;
using System.Collections.Generic;
using Infragistics.Windows.Editors;

namespace Units
{
    /// <summary>
    /// Interaction logic for UnitFilterMaintenance.xaml
    /// </summary>
    public partial class UnitFilterMaintenance :ScreenBase ,IScreen, IPreBindable
    {
        //Property is required for base objects that use IScreen
        public string WindowCaption { get; private set; }

        //Screen Level ID
        public int CurrentFilterID { get; set; }

        //This datatable is being added so that the Amount Allocated text box can have a binding
        //Do this for fields that contain informational data but that will not be saved
        //so that you can use converters or other benefits of binding
        DataTable dtMiscInfo = new DataTable("MiscInfo");

        //Setup any grid combobox providers - Variable name should match the Binding Path value in the field layout
        public ComboBoxItemsProvider cmbCurrency { get; set; }
        public ComboBoxItemsProvider cmbProducts { get; set; }
        public ComboBoxItemsProvider cmbSeq { get; set; }

        

        //Setup keys for double click zooms from grids
        private List<string> gFilterDataKeys = new List<string> { "document_id"}; //Used for double click
        
        //Miscellaneous variables and properties
 
    
        //Required constructor
        public UnitFilterMaintenance()
            :base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialization event - this event should setup all tabs and grids as well as be able to handle being passed
        /// information from a zoom from another window.
        /// </summary>
        /// <param name="businessObject"></param>
        public void Init(cBaseBusObject businessObject)
        {
            WindowCaption = "Unit Filter Setup";
            
            //Setting intial screenstate to empty
            CurrentState = ScreenState.Empty;

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder ;

             //Set the maintablename for the folder if it has one
            this.MainTableName = "unit_filter";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            //Add any Grid Configuration Information
            gFilter.MainTableName = "unit_filter_detail"; //Should match the ROBJECT table name
            gFilter.ConfigFileName = "UnitFilterGridConfig"; //This is the file name that will store any user customizations to the grid - must be unique in the app
            gFilter.SetGridSelectionBehavior(true, false); //Sets standard grid behavior for record select and multiselect
            gFilter.WindowZoomDelegate = FilterGridDoubleClick; //The function delegate determines what happens when the user double clicks the grid
            gFilter.FieldLayoutResourceString = "UnitFilterDetailGrid"; //The name of the FieldLayout in the Field Layouts xaml file - Must be unique
            //gFilter.GridGotFocusDelegate = gFilterGrid_GotFocus; //This ties the got focus event of the remit base grid to this method.
            //gFilter.EditModeEndedDelegate = gFilterGrid_EditModeEnded; //This allows for data checks after each cell is exited
            gFilter.SkipReadOnlyCellsOnTab = true; //Sets the grid to skip over non edit fields
            //gFilter.GridPreviewKeyDownDelegate = gFilterGrid_GridPreviewKeyDown;
            //gFilter.RecordActivatedDelegate = gFilter_RecordActivated;
            gFilter.ContextMenuAddDelegate = AddNewFilterDetail;
            gFilter.ContextMenuAddIsVisible = true;
            gFilter.ContextMenuRemoveDelegate = RemoveFilterDetail;
            gFilter.ContextMenuRemoveIsVisible = true;

  
            //Add all grids to the grid collection - This allows grids to automatically load and participate with security
            GridCollection.Add(gFilter);
   
            // add the Tab user controls that are of type screen base - This screen has no detail tabs
            //TabCollection.Add(General);
 

            //Debug code for hardwiring a test parameter set
            //CurrentBusObj.Parms.AddParm("@filter_id", "1" );

            // if there are parameters passed into the window on startup then we need to load the data
            if (CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                txtFilterID.Text = CurrentBusObj.Parms.GetParm("@filter_id");
                RetrieveData();
            }
            else
            {
                SetWindowStatus();
            }
            
        }

        /// <summary>
        /// This event is used to handle the setup of any ucLabelComboBox controls and is required if the controls are to be used
        /// Since this event runs after the data is retrieved for the business object but before it is bound to the screen,
        /// it can also be used for any other activities that need to run then
        /// </summary>
        public void PreBind()
        {
            //try
            //{
            //    // if the object data was loaded
            //    if (this.CurrentBusObj.HasObjectData)
            //    {
 
            //        //Setup Grid Combo Boxes
            //        //Currency combo box on remit grid
            //        //Add code to populate the grid combobox on the Company Contract Grid
            //        ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
            //        //Set the items source to be the databale of the DDDW
            //        ip.ItemsSource = this.CurrentBusObj.ObjectData.Tables["currency"].DefaultView;
            //        //set the value and display path
            //        ip.ValuePath = "currency_code";
            //        ip.DisplayMemberPath = "currency_code";
            //        //Set the property that the grid combo will bind to
            //        //This value is in the binding in the layout resources file for the grid.
            //        cmbCurrency = ip;

            //        //Product drop down box - alloc grid
            //        ip = new ComboBoxItemsProvider();
            //        ip.ItemsSource = CurrentBusObj.ObjectData.Tables["products"].DefaultView;
            //        ip.ValuePath = "product_code";
            //        ip.DisplayMemberPath = ("product_description");
            //        cmbProducts = ip;

            //        //apply to seq down box - alloc grid
            //        ip = new ComboBoxItemsProvider();
            //        ip.ItemsSource = CurrentBusObj.ObjectData.Tables["open_cash"].DefaultView;
            //        ip.ValuePath = "seq_code";
            //        ip.DisplayMemberPath = ("lookup_description");
            //        cmbSeq = ip;
            //    }
            //}
            //catch (Exception error)
            //{
            //    MessageBox.Show(error.ToString());
            //}
        }

        /// <summary>
        /// This method is needed if the window will allow inserts
        /// This will get called when the user selects new from the application menu
        /// Normally this should only apply to the folder as a whole and not to grids or individual tabs
        /// To use ROBjects with the insert - Make sure to set the select and insert stored procedures for your
        /// main table to have an update_order of 0 in the robjects_detail table.
        /// A retireve will have to be done in order for the objects to be populated with empty data
        /// this allows robjects to know the datatables and fields needed.
        /// In this screen the retrieve is handled in the RetrieveData() method.
        /// </summary>
        public override void New()
        {
            //Set the screen state to inserting
            CurrentState = ScreenState.Inserting;

            if (!RetrieveData())
            {
                MessageBox.Show("Error creating new record.");
                CurrentState = ScreenState.Normal;
                return;
            }
            else //New record created
            {
                //Set any default values for new record here
                txtName.CntrlFocus() ;
                
            }

        }

        /// <summary>
        /// Use the event to retrieve data into the base business object
        /// For Insert make sure to set CurrentState to Inserting
        /// </summary>
        /// <returns></returns>
        private bool RetrieveData()
        {
            //Verify that no save is needed
            if (IsScreenDirty)
            {
                System.Windows.MessageBoxResult result = Messages.ShowYesNo("Would you like to save existing changes?",
                           System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    Save();
                    if (!SaveSuccessful  )
                    {
                        return false;
                    }
                }
            }


            //If inserting to the screen then we will pass a dummy parm so that we can return empty tables
            if (CurrentState == ScreenState.Inserting)
            {
                //Remove any existing parameters
                CurrentBusObj.Parms.ClearParms();
                //Add all parameters back in
                CurrentBusObj.Parms.AddParm("@filter_id", "-1");
                CurrentBusObj.Parms.AddParm("@show_inactive", 0);
                txtFilterID.Text = "-1";
                //Loads a new record in the main table and empty datatbales elsewhere
                base.New();
                            //Reset the current remit ID
                CurrentFilterID = 0;
            }
            //We have a batch ID to retrieve
            else
            {
                //test to make sure The batch is an integer
                int i;
                if (Int32.TryParse(txtFilterID.Text, out i))
                {
                    //Remove existing paramters
                    CurrentBusObj.Parms.ClearParms();
                    //Add all parameters back in
                    CurrentBusObj.Parms.AddParm("@filter_id", txtFilterID.Text);
                    CurrentBusObj.Parms.AddParm("@show_inactive", 0);

                    //Empty the current object if any exists
                    if (CurrentBusObj.HasObjectData)
                        CurrentBusObj.ObjectData.Clear();

                    //Load the base business object and populate the window and tabs
                    this.Load();
 
                }
                else //If non numeric entered
                {
                    MessageBox.Show("Filter ID must be a numeric whole number value");
                    return false;
                }
            }

            

            //Verify that data was returned
            if (CurrentBusObj.ObjectData.Tables["unit_filter"] != null && CurrentBusObj.ObjectData.Tables["unit_filter"].Rows.Count>0)
            {
                CurrentFilterID=Convert.ToInt32(txtFilterID.Text) ;
                SetWindowStatus();
                //Reset the current remit ID - Move to the max rows +1 for any new inserts
                return true;
            }
            else
            {
                MessageBox.Show("No data retrieved for batch id " + txtFilterID.Text);
                //Reset the current remit ID
                CurrentFilterID = 0;
                CurrentState = ScreenState.Empty;
                return false;
            }
        }

        public override void Save()
        {
            //If deleting then a verify is not needed
            if(CurrentState!=ScreenState.Deleting)
            {
                //Verify that the data to be saved is valid
                if (!VerifyData())
                {
                    MessageBox.Show("Unable to save data until all errors are corrected.");
                    SaveSuccessful = false;
                    return;
                }
            }
            
            //If verified or deleting the save the data
            base.Save();
            if (SaveSuccessful)
            {
                if (CurrentState == ScreenState.Deleting)
                {
                    Messages.ShowInformation("Filter Deleted");
                }
                else
                {
                    Messages.ShowInformation("Save Successful");
                    txtFilterID.CntrlFocus();
                    txtFilterID.Text = CurrentBusObj.ObjectData.Tables["unit_filter"].Rows[0]["filter_id"].ToString();
                }

            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
           
           
        }

        /// <summary>
        /// This event tracks the primary data key field to see if the value changes once the field is exited.  
        /// The field will be exited either by a tab or an enter.  At that point if the ID has changed then
        /// the database is requeried by the base business object. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFilterID_LostFocus(object sender, RoutedEventArgs e)
        {
            int i;
            Int32.TryParse(txtFilterID.Text, out i);
            if (i != CurrentFilterID)
            {
                CurrentState = ScreenState.Normal;
                RetrieveData();
            }
        }

  
 
        /// <summary>
        /// Method to establish field and grid status and editability
        /// </summary>
        private void SetWindowStatus()
        {

                if (CurrentState == ScreenState.Empty )
                {
                    gFilter.xGrid.FieldSettings.AllowEdit = false;
                    gFilter.xGrid.IsEnabled = false;
                    chkFilterEqualsOne.IsEnabled = false;
                    chkInactive.IsEnabled = false;
                    txtDescription.IsEnabled = false;
                    txtName.IsEnabled = true;
                    gFilter.SetGridSelectionBehavior(true, false);
                 }
                else if (chkInactive.IsChecked == 1)
                {
                    gFilter.xGrid.FieldSettings.AllowEdit = false;
                    gFilter.xGrid.IsEnabled = false;
                    chkFilterEqualsOne.IsEnabled = false;
                    chkInactive.IsEnabled = true;
                    txtDescription.IsEnabled = true;
                    txtName.IsEnabled = true;
                    gFilter.SetGridSelectionBehavior(true, false);
                }
                else if (chkFilterEqualsOne.IsChecked == 1)
                {
                    gFilter.xGrid.FieldSettings.AllowEdit = false;
                    gFilter.xGrid.IsEnabled = false;
                    chkFilterEqualsOne.IsEnabled = true;
                    chkInactive.IsEnabled = true;
                    txtDescription.IsEnabled = true;
                    txtName.IsEnabled = true;
                }
                else //Not an empty, inactive or filetr equals 1 screen
                {
                    gFilter.xGrid.FieldSettings.AllowEdit = true;
                    gFilter.xGrid.IsEnabled = true;
                    chkFilterEqualsOne.IsEnabled = true;
                    chkInactive.IsEnabled = true;
                    txtDescription.IsEnabled = true;
                    txtName.IsEnabled = true;
                    gFilter.SetGridSelectionBehavior(true, false);
                    //Set delete to enable on menu
                    //CanExecuteDeleteCommand = true;
                }

        }

        private bool VerifyData()
        {
            return true;
        }

        /// <summary>
        /// This event handles the double click launching of a lookup window.  If a value is returned from the lookup window
        /// then the data for the window is requeried.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFilterID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            cGlobals.ReturnParms.Clear();
            UnitFilterLookup w = new UnitFilterLookup();
            w.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                txtFilterID.Text = cGlobals.ReturnParms[0].ToString();
                // Clear the Global parms
                //This prevents invalid data being passed to other lookups
                cGlobals.ReturnParms.Clear();
                CurrentState = ScreenState.Normal;
                RetrieveData();
            }
        }

        public void FilterGridDoubleClick()
        {

            if (gFilter.xGrid.ActiveRecord != null)
            {
                DataRecord r = (DataRecord)gFilter.xGrid.ActiveRecord;
                cGlobals.ReturnParms.Clear();
                cGlobals.ReturnParms.Add(r.Cells["filter_id"].Value);
                cGlobals.ReturnParms.Add(r.Cells["seq_id"].Value);
                cGlobals.ReturnParms.Add(r.Cells["unit_md_id"].Value);
                cGlobals.ReturnParms.Add(r.Cells["operator"].Value);
                cGlobals.ReturnParms.Add(r.Cells["value"].Value);
                cGlobals.ReturnParms.Add(r.Cells["construct"].Value);
                UnitFiltersLook fl = new UnitFiltersLook(CurrentBusObj);
                fl.ShowDialog();

                if (cGlobals.ReturnParms.Count > 0)
                {
                    //_filterid = Int32.Parse(cGlobals.ReturnParms[0].ToString());
                    //_seqid = Int32.Parse(cGlobals.ReturnParms[1].ToString());
                    r.Cells["unit_md_id"].Value = Int32.Parse(cGlobals.ReturnParms[2].ToString());
                    r.Cells["operator"].Value= Int32.Parse(cGlobals.ReturnParms[3].ToString());
                    r.Cells["value"].Value = cGlobals.ReturnParms[4].ToString();
                    r.Cells["construct"].Value = Int32.Parse(cGlobals.ReturnParms[5].ToString());
                    r.Cells["md_name"].Value = cGlobals.ReturnParms[6].ToString();
                    r.Cells["operator_text"].Value = cGlobals.ReturnParms[7].ToString();
                    r.Cells["value_text"].Value = cGlobals.ReturnParms[8].ToString();
                    r.Cells["construct_text"].Value = cGlobals.ReturnParms[9].ToString();
                }
                cGlobals.ReturnParms.Clear();
            }
        }

        public void AddNewFilterDetail()
        {
            cGlobals.ReturnParms.Clear();
            UnitFiltersLook ufl = new UnitFiltersLook(this.CurrentBusObj);
            ufl.ShowDialog();

            if (cGlobals.ReturnParms.Count > 0)
            {
                //Sets the grid to allow a new row to be created
                gFilter.xGrid.FieldLayoutSettings.AllowAddNew = true;
                //Establishes a local variable tied to the row we just created
                DataRecord row = gFilter.xGrid.RecordManager.CurrentAddRecord;
                //Set the default values for the columns
                row.Cells["filter_id"].Value = Convert.ToInt32((txtFilterID.Text));
                row.Cells["seq_id"].Value = 0;
                row.Cells["unit_md_id"].Value = Int32.Parse(cGlobals.ReturnParms[2].ToString());
                row.Cells["operator"].Value = Int32.Parse(cGlobals.ReturnParms[3].ToString());
                row.Cells["value"].Value = cGlobals.ReturnParms[4].ToString();
                row.Cells["construct"].Value = Int32.Parse(cGlobals.ReturnParms[5].ToString());
                row.Cells["md_name"].Value = cGlobals.ReturnParms[6].ToString();
                row.Cells["operator_text"].Value = cGlobals.ReturnParms[7].ToString();
                row.Cells["value_text"].Value = cGlobals.ReturnParms[8].ToString();
                row.Cells["construct_text"].Value = cGlobals.ReturnParms[9].ToString();

                //Commit the add new record - Required so that we can get rid of the add new record box and make this record active
                gFilter.xGrid.RecordManager.CommitAddRecord();
                //Remove the add new record row
                gFilter.xGrid.FieldLayoutSettings.AllowAddNew = false;
                //Set the row we just created to the active record
                gFilter.xGrid.ActiveRecord = gFilter.xGrid.Records[0];
            }
        }

        public void RemoveFilterDetail()
        {
            if (gFilter.ActiveRecord!=null)
            {
               // gFilter.xGrid.DataItems.RemoveAt(gFilter.xGrid.ActiveRecord.Index);
                gFilter.xGrid.ExecuteCommand(DataPresenterCommands.DeleteSelectedDataRecords); 

            }
        }

        private void chkInactive_Checked(object sender, RoutedEventArgs e)
        {
            SetWindowStatus();
        }

        private void chkFilterEqualsOne_Checked(object sender, RoutedEventArgs e)
        {
            if (chkFilterEqualsOne.IsChecked == 1 && CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["unit_filter_detail"] != null &&
               CurrentBusObj.ObjectData.Tables["unit_filter_detail"].Rows.Count > 0)
            {
                System.Windows.MessageBoxResult result = Messages.ShowYesNo("All detail rows will be deleted for this filter. Continue?",
                          System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    CurrentBusObj.DeleteAllTableRows("unit_filter_detail");
                }
                else
                {
                    chkFilterEqualsOne.IsChecked = 0;
                }
            }
            SetWindowStatus();

        }
 
    }
}
