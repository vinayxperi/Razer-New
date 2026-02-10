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
    /// This class represents a 'ucTab1' object.
    /// </summary>
    public partial class UnitsEntryTab : ScreenBase, IScreen, IPreBindable
    {

        //Property is required for base objects that use IScreen
        public string WindowCaption { get; private set; }
        private Int32 CurrentUnitID = 0;
        private Int32 CurrentLocID = 0;
        private Int32 CurrentUnitType = 0;
        private Int32 CurrentSubscriberType = 0;
        private Int32 CurrentDataType = 0;
        private Int32 CurrentTivoType = 0;
        int UpdateIndicator = 0;
        DateTime defaultbillperiod;
        public ComboBoxItemsProvider cmbProducts { get; set; }
        public ComboBoxItemsProvider cmbContract { get; set; }
        public ComboBoxItemsProvider cmbUnits { get; set; }
        public ComboBoxItemsProvider cmbSubscriber { get; set; }
        //RES 11/1/24 Add data type selection
        public ComboBoxItemsProvider cmbData { get; set; }
        public ComboBoxItemsProvider cmbTivo { get; set; }

        ////Setup keys for double click zooms from grids
        //private List<string> gUnitEntryDataKeys = new List<string> { "cs_id" }; //Used for double click

        public UnitsEntryTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Perform initializations for this object
        }

        public void Init(cBaseBusObject businessObject)
        {
            WindowCaption = "Unit Entry";

            //Setting intial screenstate to empty
            CurrentState = ScreenState.Empty;
           

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            //Set the maintablename for the folder if it has one
            this.MainTableName = "main";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;
            
            //Disable File New/Delete
            this.CanExecuteNewCommand = false;
            this.CanExecuteDeleteCommand = false;
            //Add any Grid Configuration Information
            gUnitEntry.MainTableName = "main"; //Should match the ROBJECT table name
            gUnitEntry.ConfigFileName = "UnitEntryGrid"; //This is the file name that will store any user customizations to the grid - must be unique in the app
            gUnitEntry.SetGridSelectionBehavior(true, false); //Sets standard grid behavior for record select and multiselect
            gUnitEntry.FieldLayoutResourceString = "UnitEntryGrid"; //The name of the FieldLayout in the Field Layouts xaml file - Must be unique
            gUnitEntry.GridGotFocusDelegate = gUnitEntryGrid_GotFocus; //This ties the got focus event of the  grid to this method.
            gUnitEntry.EditModeEndedDelegate = gUnitEntryGrid_EditModeEnded; //This allows for data checks after each cell is exited
            gUnitEntry.SkipReadOnlyCellsOnTab = true; //Sets the grid to skip over non edit fields
            gUnitEntry.GridPreviewKeyDownDelegate = gUnitEntry_GridPreviewKeyDown;
    
            ////Turn off Grid config and filter abilities as these will mess up the hardwiring of tabbing thorugh grid
            gUnitEntry.ContextMenuToggleFilterIsVisible = false;
            gUnitEntry.ContextMenuResetGridSettingsIsVisible = false;
            gUnitEntry.ContextMenuSaveGridSettingsIsVisible = false;
            gUnitEntry.ContextMenuRemoveIsVisible = false;
            gUnitEntry.ContextMenuAddIsVisible = false;
            gUnitEntry.GridLoadedDelegate = gUnitEntry_GridLoaded;
            gUnitEntry.WindowZoomDelegate = gUnitEntry_DoubleClick;


            //Add all grids to the grid collection - This allows grids to automatically load and participate with security
            GridCollection.Add(gUnitEntry);

            ////Debug code for hardwiring a test parameter set
            CurrentBusObj.Parms.AddParm("@cs_id_parm", "0");
            CurrentBusObj.Parms.AddParm("@product_code_parm", "");
            CurrentBusObj.Parms.AddParm("@contract_id_parm", 0);
            //RES 11/1/24 Add unit type selection
            //CurrentBusObj.Parms.AddParm("@unit_type_id_parm", 1);
            CurrentBusObj.Parms.AddParm("@unit_type_id_parm", 0);
            //CurrentBusObj.Parms.AddParm("@unit_id", 0);
            CurrentBusObj.Parms.AddParm("@subscriber_id_parm", 0);
            CurrentBusObj.Parms.AddParm("@data_service_id_parm", 0);
            CurrentBusObj.Parms.AddParm("@tivo_count_id_parm", 0);
            CurrentBusObj.Parms.AddParm("@service_period_start_parm", "01/01/1900");
            CurrentBusObj.Parms.AddParm("@service_period_end_parm", "01/01/1900");
           

            CurrentState = ScreenState.Inserting;
            // if there are parameters passed into the window on startup then we need to load the data
            if (CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                CurrentLocID = Convert.ToInt32(CurrentBusObj.Parms.GetParm("@cs_id_parm"));
                RetrieveData();
            }
            else
            {
                SetWindowStatus();
                
            }
        }

        //Use the event to retrieve data into the base business object
        // For Insert make sure to set CurrentState to Inserting

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
                    if (!SaveSuccessful)
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
                CurrentBusObj.Parms.AddParm("@cs_id_parm", "0");
                CurrentBusObj.Parms.AddParm("@product_code_parm", "");
                CurrentBusObj.Parms.AddParm("@contract_id_parm", 0);
                //RES 11/1/24 Add unit type selection
                //CurrentBusObj.Parms.AddParm("@unit_type_id_parm", 1);
                CurrentBusObj.Parms.AddParm("@unit_type_id_parm", 0);
                //CurrentBusObj.Parms.AddParm("@unit_id", 0);
                CurrentBusObj.Parms.AddParm("@subscriber_id_parm", 0);
                CurrentBusObj.Parms.AddParm("@data_service_id_parm", 0);
                CurrentBusObj.Parms.AddParm("@tivo_count_id_parm", 0);
                CurrentBusObj.Parms.AddParm("@service_period_start_parm", "01/01/1900");
                CurrentBusObj.Parms.AddParm("@service_period_end_parm", "01/01/1900");
               
                CurrentLocID = 0;
                //Loads a new record in the main table and empty datatbales elsewhere
                this.Load();
                if (CurrentBusObj.HasObjectData)
                    defaultbillperiod = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["billperioddefault"].Rows[0][0]);
                this.CreateNewUnitRow();

            }
            //We have a location ID to retrieve
            else
            {
                //test to make sure The location id is an integer
                int i;
                if (Int32.TryParse(CurrentLocID.ToString(), out i))
                {
                    //Remove existing paramters
                    CurrentBusObj.Parms.ClearParms();
                    //Add all parameters back in
                    CurrentBusObj.Parms.AddParm("@cs_id_parm", CurrentLocID.ToString());
                    //Empty the current object if any exists
                    if (CurrentBusObj.HasObjectData)
                        CurrentBusObj.ObjectData.Clear();

                    //Load the base business object and populate the window and tabs
                   

                    this.Load();

                }
                else //If non numeric entered
                {
                    MessageBox.Show("Location ID must be a numeric whole number value");
                    return false;
                }


            }
            //Verify that data was returned
            if (CurrentBusObj.ObjectData.Tables["main"] != null && CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                //Reset the current unit ID - Move to the max rows +1 for any new inserts
                //CurrentUnitID = CurrentBusObj.ObjectData.Tables["main"].Rows.Count + 1;
                
                SetWindowStatus();
                
                
                return true;
            }
            else
            {
                MessageBox.Show("No data retrieved for Location id " + CurrentLocID);
                //Reset the current remit ID
                CurrentUnitID = 0;
                CurrentState = ScreenState.Empty;
                return false;
            }
        }

        private void gUnitEntry_GridLoaded(object sender, RoutedEventArgs e)
        {
            (gUnitEntry.xGrid.Records[gUnitEntry.ActiveRecord.Index] as DataRecord).Cells["cs_id"].IsActive = true;


            //Moves the cursor into the active cell - This code may be required to get the cell in edit mode without clicking.
            gUnitEntry.xGrid.Records.DataPresenter.BringCellIntoView(gUnitEntry.xGrid.ActiveCell);
            //gUnitEntry.xGrid.ActiveDataItem = 1;

            //Puts the cell into edit mode
            gUnitEntry.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            CurrentState = ScreenState.Normal;
        }


        private void SetWindowStatus()
        {

            if (CurrentState == ScreenState.Empty)
            {

                gUnitEntry.xGrid.FieldSettings.AllowEdit = false;
                gUnitEntry.SetGridSelectionBehavior(true, false);


            }
            else //Not an empty screen and not posted
            {

                gUnitEntry.Focus();
                gUnitEntry.xGrid.FieldSettings.AllowEdit = true;
                gUnitEntry.SetGridSelectionBehavior(false, false);
                //Set delete to enable on menu
                CanExecuteDeleteCommand = true;
                gUnitEntry.xGrid.FieldLayouts[0].Fields["cs_id"].Settings.AllowEdit = true;
                (gUnitEntry.xGrid.Records[gUnitEntry.ActiveRecord.Index] as DataRecord).Cells["cs_id"].IsActive = true;
                //Moves the cursor into the active cell - This code may be required to get the cell in edit mode without clicking.
                //gUnitEntry.xGrid.Records.DataPresenter.BringCellIntoView(gUnitEntry.xGrid.ActiveCell);
                //gUnitEntry.xGrid.ActiveDataItem = 1;
                //Puts the cell into edit mode
                //gUnitEntry.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                //CurrentState = ScreenState.Normal;
            }

            }
             

   
        public void gUnitEntryGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            //This if tests to see if we are already in the got focus event as infragistics fires the got focus when the active record changes
            if (!gUnitEntry.xGrid.IsFocused)
            {
                e.Handled = true;
                return;
            }

            ////If we are in insert mode
          
            //if (CurrentState == ScreenState.Inserting )
            //{
            //    CreateNewUnitRow();
            //}
        }

        /// <summary>
        /// Creates new unit row and puts cursor in first cell in edit mode
        /// </summary>
        public void CreateNewUnitRow()
        {
            //KSH - 8/27/12 made some general changes to make the cursor focus in edit mode in the first cell of the newly added record
            if (CurrentState == ScreenState.Deleting || CurrentState == ScreenState.Empty)
            {
                CurrentState = ScreenState.Normal;
                return;
            }
           
            gUnitEntry.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = gUnitEntry.xGrid.RecordManager.CurrentAddRecord;

            //Set the default values for the columns
            row.Cells["unit_id"].Value = 0;
            row.Cells["contract_id"].Value = 0;
            row.Cells["cs_id"].Value = 0;
            row.Cells["name"].Value = "";
            row.Cells["mso_name"].Value = "";
            row.Cells["psa_city"].Value = "";
            row.Cells["psa_state"].Value = "";           
            //row.Cells["unit_description"].Value = "Subscribers";
            row.Cells["product_code"].Value = "";
            row.Cells["service_period_start"].Value = defaultbillperiod;
            //row.Cells["service_period_end"].Value = "01/01/1900";
            row.Cells["amount"].Value = 0;
            row.Cells["last_update_date"].Value = "01/01/1900";
            if (CurrentUnitType != 0)
            {
                if (CurrentUnitType == 1)
                {
                    row.Cells["unit_type_id"].Value = 1;
                    row.Cells["subscriber_id"].Value = 0;
                    row.Cells["data_service_type_id"].Value = 0;
                    row.Cells["tivo_count_id"].Value = 0;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Visible;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Collapsed;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Collapsed;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Settings.AllowEdit = true;
                }
                if (CurrentUnitType == 5)
                {
                    row.Cells["unit_type_id"].Value = 5;
                    row.Cells["subscriber_id"].Value = 0;
                    row.Cells["data_service_type_id"].Value = 0;
                    row.Cells["tivo_count_id"].Value = 0;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Collapsed;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Collapsed;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Visible;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Settings.AllowEdit = true;
                }
                if (CurrentUnitType == 7)
                {
                    row.Cells["unit_type_id"].Value = 7;
                    row.Cells["subscriber_id"].Value = 0;
                    row.Cells["data_service_type_id"].Value = 0;
                    row.Cells["tivo_count_id"].Value = 0;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Collapsed;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Visible;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Collapsed;
                    gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Settings.AllowEdit = true;
                }
            }
            else
            {
                row.Cells["unit_type_id"].Value = 1;
                row.Cells["subscriber_id"].Value = 0;
                row.Cells["data_service_type_id"].Value = 0;
                row.Cells["tivo_count_id"].Value = 0;
                gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Visible;
                gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Collapsed;
                gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Collapsed;
                gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Settings.AllowEdit = true;
            }
            
         
                    
            //Commit the add new record - Required so that we can get rid of the add new record box and make this record active
            gUnitEntry.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            //Set the row created to the active record
            gUnitEntry.xGrid.ActiveRecord = gUnitEntry.xGrid.Records[0];
            //Set the cs_id field as active
            (gUnitEntry.xGrid.Records[0] as DataRecord).Cells["cs_id"].IsActive = true;
            (gUnitEntry.xGrid.Records[0] as DataRecord).Cells["cs_id"].IsSelected = true;    
            //Moves the cursor into the active cell - This code may be required to get the cell in edit mode without clicking.
            gUnitEntry.xGrid.Records.DataPresenter.BringCellIntoView(gUnitEntry.xGrid.ActiveCell);
            //Puts the cell into edit mode
            CurrentState = ScreenState.Normal;
            gUnitEntry.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            //ADDING THIS BACK IN WILL NOT ALLOW YOUR CURSOR TO SET FOCUS IN THE CELL IN EDIT MODE -- THAT WOULD BE VERY BAD!!!
            //gUnitEntry.xGrid.FieldLayoutSettings.AllowAddNew = false;
        }

         public void gUnitEntryGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
         {
             //DWR Modified 3/26/13 -- to use the row the cell is on and not the active record.  
             //Active Record was causing the changes to be made to the wrong row if the user clicked on a different row.
             //DWR -- Also changed every instance of ActiveRecord in this event to use the row instead.
             DataRecord row = e.Cell.Record; //gUnitEntry.ActiveRecord;

             if (e.Cell.Field.Name == "cs_id")
             {
                 //DWR ADDED 2/14/13 - Prevents crash if cs_id is deleted
                 if (DBNull.Value.Equals(e.Cell.Value))
                     e.Cell.Value = 0;

                    //Activate company, product and c   CurrentBusObj.Parms.ClearParms();
                 //Add all parameters back in
                 CurrentLocID = Convert.ToInt32(e.Cell.Value);
                 if (CurrentLocID == 0)
                     return;

                 CurrentBusObj.Parms.UpdateParmValue ("@cs_id_parm", Convert.ToInt32(e.Cell.Value));
                 CurrentBusObj.LoadTable("location");
                 if (CurrentBusObj.ObjectData.Tables["location"].Rows.Count > 0)
                 {
                         CurrentBusObj.LoadTable("products");
                         CurrentBusObj.LoadTable("unittype");
                         //if (CurrentUnitType == 0) CurrentBusObj.LoadTable("unittypeloc");
                         //else
                         if (CurrentBusObj.ObjectData.Tables["main"].Rows.Count == 1)
                         {
                             CurrentBusObj.LoadTable("unittypeloc");
                             CurrentUnitType = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["unit_type_id"]);
                             row.Cells["unit_type_id"].Value = CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["unit_type_id"];
                         }
                         
                         //set the default unit type
                         //gUnitEntry.ActiveRecord.Cells["unit_description"].Value = "Subscribers";
                         gUnitEntry.xGrid.FieldLayouts[0].Fields["product_code"].Settings.AllowEdit = true;
                         gUnitEntry.xGrid.FieldLayouts[0].Fields["contract_id"].Settings.AllowEdit = false;
                         gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_start"].Settings.AllowEdit = false;
                         gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_end"].Settings.AllowEdit = false;
                         gUnitEntry.xGrid.FieldLayouts[0].Fields["unit_type_id"].Settings.AllowEdit = true;
                         gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Settings.AllowEdit = false;
                         row.Cells["name"].Value = CurrentBusObj.ObjectData.Tables["location"].Rows[0]["name"];
                         //gUnitEntry.ActiveRecord.Cells["mso_id"].Value = CurrentBusObj.ObjectData.Tables["location"].Rows[0]["mso_id"];
                         //gUnitEntry.ActiveRecord.Cells["mso_name"].Value = CurrentBusObj.ObjectData.Tables["location"].Rows[0]["mso_name"];
                         row.Cells["psa_city"].Value = CurrentBusObj.ObjectData.Tables["location"].Rows[0]["psa_city"];
                         row.Cells["psa_state"].Value = CurrentBusObj.ObjectData.Tables["location"].Rows[0]["psa_state"];


                         if (CurrentBusObj.ObjectData.Tables["products"].Rows.Count == 1)
                         {
                             row.Cells["product_code"].Value = CurrentBusObj.ObjectData.Tables["products"].Rows[0]["product_code"];
                             CurrentBusObj.Parms.UpdateParmValue("@product_code_parm", row.Cells["product_code"].Value);
                             CurrentBusObj.LoadTable("contracts");

                             gUnitEntry.xGrid.FieldLayouts[0].Fields["contract_id"].Settings.AllowEdit = true;
                             if (CurrentBusObj.ObjectData.Tables["contracts"].Rows.Count == 1)
                             {
                                 row.Cells["contract_id"].Value = CurrentBusObj.ObjectData.Tables["contracts"].Rows[0]["contract_id"];                                 
                                 gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Settings.AllowEdit = true;
                                 CurrentBusObj.LoadTable("subtype");
                                 CurrentBusObj.Parms.UpdateParmValue("@contract_id_parm", Convert.ToInt32(row.Cells["contract_id"].Value));
                                 CurrentBusObj.LoadTable("contract_entity");
                                 if (CurrentBusObj.ObjectData.Tables["contract_entity"] != null && CurrentBusObj.ObjectData.Tables["contract_entity"].Rows.Count > 0)
                                 {
                                     row.Cells["mso_id"].Value = CurrentBusObj.ObjectData.Tables["contract_entity"].Rows[0]["bill_mso_id"];
                                     row.Cells["mso_name"].Value = CurrentBusObj.ObjectData.Tables["contract_entity"].Rows[0]["name"];
                                 }
                             }
                             //RES 11/1/24 Add unit type selection
                             if (CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows.Count == 1)
                             {
                                 if (CurrentUnitType == 0)
                                 {
                                     CurrentUnitType = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["unit_type_id"]);
                                     row.Cells["unit_type_id"].Value = CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["unit_type_id"];
                                     //}
                                     //else
                                     //    row.Cells["unit_type_id"].Value = CurrentUnitType;
                                     //{
                                     //    MessageBox.Show("Only 1 unit type at a time is allowed, unit type will be changed to current unit type");
                                     //    row.Cells["unit_type_id"].Value = CurrentUnitType;
                                     //    //CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["data_service_type_id"].ToString()
                                     //}
                                     gUnitEntry.xGrid.FieldLayouts[0].Fields["unit_type_id"].Settings.AllowEdit = true;
                                     if (CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["data_service_type_id"].ToString() != "0")
                                     //if (row.Cells["unit_type_id"].Value.ToString() == "5")
                                     {
                                         row.Cells["data_service_type_id"].Value = CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["data_service_type_id"];
                                         //row.Cells["data_service_type_id"].Value = "0"
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Visible;
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Collapsed;
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Collapsed;
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Settings.AllowEdit = true;
                                         row.Cells["subscriber_id"].Value = 0;
                                         row.Cells["tivo_count_id"].Value = 0;
                                         CurrentBusObj.LoadTable("datatype");
                                     }
                                     if (CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["tivo_count_id"].ToString() != "0")
                                     //if (row.Cells["unit_type_id"].Value.ToString() == "7")
                                     {
                                         row.Cells["tivo_count_id"].Value = CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["tivo_count_id"];
                                         //row.Cells["tivo_count_id"].Value = "0";
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Visible;
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Collapsed;
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Collapsed;
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Settings.AllowEdit = true;
                                         row.Cells["subscriber_id"].Value = 0;
                                         row.Cells["data_service_type_id"].Value = 0;
                                         CurrentBusObj.LoadTable("tivocount");
                                     }
                                     if (CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["subscriber_id"].ToString() != "0")
                                     {
                                         row.Cells["subscriber_id"].Value = CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["subscriber_id"];
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Visible;
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Collapsed;
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Collapsed;
                                         gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Settings.AllowEdit = true;
                                         row.Cells["tivo_count_id"].Value = 0;
                                         row.Cells["data_service_type_id"].Value = 0;
                                         CurrentBusObj.LoadTable("subtype");
                                     }
                                 }
                                 //CurrentBusObj.Parms.UpdateParmValue("@unit_id", row.Cells["unit_id"].Value);
                                 //CurrentBusObj.LoadTable("unittypeloc");
                                 //row.Cells["unit_type_id"].Value = CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["unit_description"];
                             }
                         }

                     //DWR Modified 3/26/13 -- Removed setting of value to 0 as this was happening automatically and would reset to 0 after user entered value
                     //if (row.Cells["amount"].Value) ==null )
                     //    row.Cells["amount"].Value = 0;
                     }
                     else
                         MessageBox.Show("No data retrieved for Location id " + CurrentLocID);
                   
                 
             }

             //This code runs a query to verify that the document id is valid when attempting to leave the apply to doc field
             if (e.Cell.Field.Name == "product_code")
             {
                 CurrentBusObj.Parms.UpdateParmValue("@product_code_parm", e.Cell.Value ); 
                 CurrentBusObj.LoadTable("contracts");
            
                 gUnitEntry.xGrid.FieldLayouts[0].Fields["contract_id"].Settings.AllowEdit = true;
                 if (CurrentBusObj.ObjectData.Tables["contracts"].Rows.Count == 1)
                 {
                     row.Cells["contract_id"].Value = CurrentBusObj.ObjectData.Tables["contracts"].Rows[0]["contract_id"];
                     //RES 11/1/24 Add unit type selection
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["unit_type_id"].Settings.AllowEdit = true;
                     CurrentBusObj.LoadTable("unittype");
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Settings.AllowEdit = true;
                     CurrentBusObj.LoadTable("subtype");
                     CurrentBusObj.Parms.UpdateParmValue("@contract_id_parm", Convert.ToInt32(row.Cells["contract_id"].Value));
                     CurrentBusObj.LoadTable("contract_entity");
                     if (CurrentBusObj.ObjectData.Tables["contract_entity"] != null && CurrentBusObj.ObjectData.Tables["contract_entity"].Rows.Count > 0)
                     {
                         row.Cells["mso_id"].Value = CurrentBusObj.ObjectData.Tables["contract_entity"].Rows[0]["bill_mso_id"];
                         row.Cells["mso_name"].Value = CurrentBusObj.ObjectData.Tables["contract_entity"].Rows[0]["name"];
                     }
                 }
             }


             if (e.Cell.Field.Name == "contract_id")
             {
                 //RES 11/1/24 Add unit type selection
                 gUnitEntry.xGrid.FieldLayouts[0].Fields["unit_type_id"].Settings.AllowEdit = true;
                 CurrentBusObj.LoadTable("unittype");
                 gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Settings.AllowEdit = true;
                 CurrentBusObj.LoadTable("subtype");

                 CurrentBusObj.Parms.UpdateParmValue("@contract_id_parm", Convert.ToInt32(e.Cell.Value));
                 CurrentBusObj.LoadTable("contract_entity");
                 if (CurrentBusObj.ObjectData.Tables["contract_entity"] != null && CurrentBusObj.ObjectData.Tables["contract_entity"].Rows.Count > 0)
                 {
                     row.Cells["mso_id"].Value = CurrentBusObj.ObjectData.Tables["contract_entity"].Rows[0]["bill_mso_id"];
                     row.Cells["mso_name"].Value = CurrentBusObj.ObjectData.Tables["contract_entity"].Rows[0]["name"];
                 }
 
             }
             //RES 11/1/24 Add unit type selection
             if (e.Cell.Field.Name == "unit_type_id")
             {
                 //if (CurrentUnitType == 0)
                 //   CurrentUnitType = Convert.ToInt32(row.Cells["unit_type_id"].Value);
                 if (CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 1)
                 {
                     if (row.Cells["unit_type_id"].Value.ToString() != CurrentUnitType.ToString())
                     {
                         MessageBox.Show("Only 1 unit type at a time is allowed, unit type will be changed to current unit type");
                         row.Cells["unit_type_id"].Value = CurrentUnitType;
                         //return;
                     }
                 }
                 else
                    CurrentUnitType = Convert.ToInt32(row.Cells["unit_type_id"].Value);

                 gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_start"].Settings.AllowEdit = true;
                 if (row.Cells["unit_type_id"].Value.ToString() == "5")
                 {
                     //row.Cells["data_service_type_id"].Value = CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["data_service_type_id"];
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Visible;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Collapsed;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Collapsed;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Settings.AllowEdit = true;
                     //row.Cells["data_service_type_id"].Value = 0;
                     row.Cells["subscriber_id"].Value = 0;
                     row.Cells["tivo_count_id"].Value = 0;
                     CurrentBusObj.LoadTable("datatype");
                 }
                 if (row.Cells["unit_type_id"].Value.ToString() == "7")
                 {
                     //row.Cells["data_service_type_id"].Value = CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["data_service_type_id"];
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Visible;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Collapsed;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Collapsed;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Settings.AllowEdit = true;
                     //row.Cells["tivo_count_id"].Value = 0;
                     row.Cells["subscriber_id"].Value = 0;
                     row.Cells["data_service_type_id"].Value = 0;
                     CurrentBusObj.LoadTable("tivocount");
                 }
                 if (row.Cells["unit_type_id"].Value.ToString() == "1")
                 {
                     //row.Cells["data_service_type_id"].Value = CurrentBusObj.ObjectData.Tables["unittypeloc"].Rows[0]["data_service_type_id"];
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Visibility = Visibility.Visible;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["tivo_count_id"].Visibility = Visibility.Collapsed;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["data_service_type_id"].Visibility = Visibility.Collapsed;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["subscriber_id"].Settings.AllowEdit = true;
                     //row.Cells["subscriber_id"].Value = 0;
                     row.Cells["tivo_count_id"].Value = 0;
                     row.Cells["data_service_type_id"].Value = 0;
                     CurrentBusObj.LoadTable("subtype");
                 }
                 gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_start"].Settings.AllowEdit = true;
             }
             if (e.Cell.Field.Name == "data_service_type_id")
             {
                 if (CurrentDataType == 0)
                     CurrentDataType = Convert.ToInt32(row.Cells["data_service_type_id"].Value);
                 gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_start"].Settings.AllowEdit = true;
                 //gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_start"].Visibility = Visibility.Hidden;
             }
             if (e.Cell.Field.Name == "tivo_count_id")
             {
                 if (CurrentTivoType == 0)
                     CurrentTivoType = Convert.ToInt32(row.Cells["tivo_count_id"].Value);
                 gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_start"].Settings.AllowEdit = true;
                 //gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_start"].Visibility = Visibility.Hidden;
             }
             if (e.Cell.Field.Name == "subscriber_id")
             {
                 if (CurrentSubscriberType == 0)
                     CurrentSubscriberType = Convert.ToInt32(row.Cells["subscriber_id"].Value);
                 gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_start"].Settings.AllowEdit = true;
             }
             if (e.Cell.Field.Name == "service_period_start" && row.Cells["service_period_start"].Value != null)
             {
                 //RES 1/8/16 validate date
                 DateTime temp;
                 temp = Convert.ToDateTime(row.Cells["service_period_start"].Value.ToString());

                 if ((temp.Year < 1990) || (temp.Year > 2050))
                 {
                     MessageBox.Show("Invalid Year in Service Period Start.");
                     return;
                 }
                 if ((temp.Day > 31) || (temp.Day < 1))
                 {
                     MessageBox.Show("Invalid Day in Service Period Start.");
                     return;
                 }
                 if ((temp.Month > 12) || (temp.Month < 1))
                 {
                     MessageBox.Show("Invalid Month in Service Period Start.");
                     return;
                 }
                 //Calculate end date using datetime.daysinmonth method
                 
                 DateTime dtConvert = new DateTime();
                
                 string sschedDate;
                 string sschedEndDate;
                 string startmonth;
                 string startday;
                 string startyear;
                 string endmonth;
                 string endday;
                 string endyear;

                 //DWR ADDED 2/14/13 - Prevents crash if date is deleted
                 if(DBNull.Value.Equals(e.Cell.Value))
                     e.Cell.Value=defaultbillperiod;

                 sschedDate = Convert.ToString(e.Cell.Value); 
                 dtConvert = Convert.ToDateTime(sschedDate);
                 startmonth = dtConvert.Month.ToString();
                 startday = dtConvert.Day.ToString();
                 startyear = dtConvert.Year.ToString();
                 if  ((startday != "01") & (startday != "1"))
                 {
                     MessageBox.Show("Scheduled Start Date should be the first of the month. "  );
                     return;
                 }
                 else
                 {
                     endmonth = startmonth;
                     endyear = startyear;
                     endday = Convert.ToString(System.DateTime.DaysInMonth(Convert.ToInt32(startyear), Convert.ToInt32(startmonth))) ;
                     sschedEndDate = endmonth + "/" + endday + "/" + endyear;
                     row.Cells["service_period_end"].Value = sschedEndDate;
                     gUnitEntry.xGrid.FieldLayouts[0].Fields["service_period_end"].Settings.AllowEdit = true;
                 }
                 
              

             }
             if (e.Cell.Field.Name == "service_period_end" && row.Cells["service_period_end"].Value != null)
             {
                 //RES 1/8/16 validate date
                 DateTime temp;
                 temp = Convert.ToDateTime(row.Cells["service_period_end"].Value.ToString());
    
                 if ((temp.Year < 1990) || (temp.Year > 2050))
                 {
                     MessageBox.Show("Invalid Year in Service Period End.");
                     return;
                 }
                 if ((temp.Day > 31) || (temp.Day < 1))
                 {
                     MessageBox.Show("Invalid Day in Service Period End.");
                     return;
                 }
                 if ((temp.Month > 12) || (temp.Month < 1))
                 {
                     MessageBox.Show("Invalid Month in Service Period End.");
                     return;
                 }
                
                 //DWR ADDED 2/14/13 - Prevents crash if date is deleted
                 if (DBNull.Value.Equals(e.Cell.Value))
                     e.Cell.Value = Convert.ToDateTime(row.Cells["service_period_start"].Value).AddMonths(1).AddDays(-1);

                 //check if all required fields are populated and if so, reload the table to see if an amount exists
                 if (Convert.ToInt32(row.Cells["cs_id"].Value) != 0)

                     CurrentBusObj.Parms.UpdateParmValue("@cs_id_parm", row.Cells["cs_id"].Value);
                 else
                 {
                     MessageBox.Show("Location is required. ");
                     return;
                 }
                 if  ( row.Cells["product_code"].Value.ToString() != "")

                     CurrentBusObj.Parms.UpdateParmValue("@product_code_parm", row.Cells["product_code"].Value);
                 else
                 {
                     MessageBox.Show("Product Code is required. ");
                     return;
                 }
                 if (Convert.ToInt32(row.Cells["contract_id"].Value) != 0)

                     CurrentBusObj.Parms.UpdateParmValue("@contract_id_parm", row.Cells["contract_id"].Value);
                 else
                 {
                     MessageBox.Show("Contract ID is required. ");
                     return;
                 }
                 //RES 11/1/24 Add unit type selection
                 //if (Convert.ToInt32(gUnitEntry.ActiveRecord.Cells["unit_type_id"].Value) != 0)
                 //    CurrentBusObj.Parms.UpdateParmValue("@unit_type_id", gUnitEntry.ActiveRecord.Cells["unit_type_id"].Value);
                 if (Convert.ToInt32(row.Cells["unit_type_id"].Value) != 0)
                     CurrentBusObj.Parms.UpdateParmValue("@unit_type_id_parm", row.Cells["unit_type_id"].Value);
                 else
                 {
                     MessageBox.Show("Unit Type is required. ");
                     return;
                 }
                 if (Convert.ToInt32(row.Cells["unit_type_id"].Value) == 1)
                 {
                     if (Convert.ToInt32(row.Cells["subscriber_id"].Value) != 0)
                     {
                         CurrentBusObj.Parms.UpdateParmValue("@subscriber_id_parm", row.Cells["subscriber_id"].Value);
                     }
                     else
                     {
                         MessageBox.Show("Subscriber Type is required. ");
                         return;
                     }
                 }
                 if (Convert.ToInt32(row.Cells["unit_type_id"].Value) == 7)
                 {
                     if (Convert.ToInt32(row.Cells["tivo_count_id"].Value) != 0)
                     {
                         CurrentBusObj.Parms.UpdateParmValue("@tivo_count_id_parm", row.Cells["tivo_count_id"].Value);
                     }
                     else
                     {
                         MessageBox.Show("Tivo Type is required. ");
                         return;
                     }
                 }
                 if (Convert.ToInt32(row.Cells["unit_type_id"].Value) == 5)
                 {
                     if (Convert.ToInt32(row.Cells["data_service_type_id"].Value) != 0)
                     {
                         CurrentBusObj.Parms.UpdateParmValue("@data_service_id_parm", row.Cells["data_service_type_id"].Value);
                     }
                     else
                     {
                         MessageBox.Show("Data Service Type is required. ");
                         return;
                     }
                 }
                 //if (Convert.ToInt32(row.Cells["tivo_count_id"].Value) != 0 && Convert.ToInt32(row.Cells["unit_type_id"].Value) == 7)
                 //    CurrentBusObj.Parms.UpdateParmValue("@tivo_count_id_parm", row.Cells["tivo_count_id"].Value);
                 //else
                 //{
                 //    MessageBox.Show("Tivo Type is required. ");
                 //    return;
                 //}
                 //if (Convert.ToInt32(row.Cells["data_service_type_id"].Value) != 0 && Convert.ToInt32(row.Cells["unit_type_id"].Value) == 1)
                 //    CurrentBusObj.Parms.UpdateParmValue("@data_service_id_parm", row.Cells["data_service_type_id"].Value);
                 //else
                 //{
                 //    MessageBox.Show("Data Service Type is required. ");
                 //    return;
                 //}
                 //if (Convert.ToInt32(row.Cells["tivo_count_id"].Value) != 0)
                 //    CurrentBusObj.Parms.UpdateParmValue("@tivo_count_id_parm", row.Cells["tivo_count_id"].Value);
                 //else
                 //{
                 //    MessageBox.Show("Tivo Type is required. ");
                 //    return;
                 //}
                 if ((row.Cells["service_period_start"].Value.ToString() != "01/01/1900" )  ||
                     (row.Cells["service_period_start"].Value != "1/1/1900" ))
                     CurrentBusObj.Parms.UpdateParmValue("@service_period_start_parm", row.Cells["service_period_start"].Value);
                 else
                 {
                     MessageBox.Show("Service Period Start is required. ");
                     return;
                 }
                 if ((row.Cells["service_period_end"].Value.ToString() != "01/01/1900") ||
                     (row.Cells["service_period_end"].Value.ToString() != "1/1/1900"))
                     CurrentBusObj.Parms.UpdateParmValue("@service_period_end_parm", row.Cells["service_period_end"].Value);
                 else
                 {
                     MessageBox.Show("Service Period End is required. ");
                     return;
                 }
                 CurrentBusObj.LoadTable("unitamt");
                 if (CurrentBusObj.ObjectData.Tables["unitamt"].Rows.Count > 0)
                 {
                     UpdateIndicator = 1; //Update existing row
                     row.Cells["amount"].Value = CurrentBusObj.ObjectData.Tables["unitamt"].Rows[0]["amount"];
                     //row.Cells["unit_id"].Value = CurrentBusObj.ObjectData.Tables["unitamt"].Rows[0]["unit_id"];
                     row.Cells["last_update_date"].Value = CurrentBusObj.ObjectData.Tables["unitamt"].Rows[0]["last_update_date"];
                 }
                 else
                     UpdateIndicator = 1; //New row
                     
             }
             //AMOUNT is handled in PreviewKeyDown
         
         }

         /// The grid preview key down event is used to alter the behavior of keyboard activity based on which cell the event was received on
         /// The purpose of this event is to intercept the behavior of the tab and enter keys on the unit_entry grid when used in the amount  
         /// field so that the default tab behavior will not be used.
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         public void gUnitEntry_GridPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
         {
             if (gUnitEntry.xGrid.ActiveCell != null)
             {
                 if (gUnitEntry.xGrid.ActiveCell.Field.Name == "amount" && (e.Key == Key.Tab || e.Key == Key.Enter))
                 {
                     gUnitEntry.ActiveRecord.Cells["last_update_date"].Value = DateTime.Today;

                     gUnitEntry.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                     this.CreateNewUnitRow();
                     e.Handled = true;
                 }                 
             }

         }
       
       
    
   
        
        
        private bool chkForData()
        {
            bool hasdata = true;
            if (this.CurrentBusObj.ObjectData != null)
            {
                if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count != 0)
                {
                    
                    hasdata = true;
                }
                else
                {
                    Messages.ShowWarning("Unit Not Found");
                    hasdata = false;
                }
            }
            return hasdata;
        }
        
        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                   
                    //Setup Grid Combo Boxes
                    //Product drop down box 
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["products"].DefaultView;
                    ip.ValuePath = "product_code";
                    ip.DisplayMemberPath = ("product_description");
                    cmbProducts = ip;

                    //Setup Grid Combo Boxes
                    //Contract drop down box 
                  
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["contracts"].DefaultView;
                    ip.ValuePath = "contract_id";
                    ip.DisplayMemberPath = ("contract_description");
                    cmbContract = ip;

                    //Setup Grid Combo Boxes
                    //Subscriber Type drop down box 
                    //RES 11/1/24 Add unit type selection
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["unittype"].DefaultView;
                    ip.ValuePath = "unit_type_id";
                    ip.DisplayMemberPath = ("unit_description");
                    cmbUnits = ip;

                    //RES 11/1/24 Add data type selection
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["datatype"].DefaultView;
                    ip.ValuePath = "data_service_type_id";
                    ip.DisplayMemberPath = ("data_service_description");
                    cmbData = ip;

                    //RES 11/1/24 Add tivo count selection
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["tivocount"].DefaultView;
                    ip.ValuePath = "tivo_count_id";
                    ip.DisplayMemberPath = ("tivo_count_description");
                    cmbTivo = ip;

                    //Setup Grid Combo Boxes
                    //Subscriber Type drop down box 

                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["subtype"].DefaultView;
                    ip.ValuePath = "subscriber_id";
                    ip.DisplayMemberPath = ("subscriber_name");                    
                    cmbSubscriber = ip;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }


        private int GridEmptyRowsProcessing(ucBaseGrid gridToCheck, bool deleteEmpty)
        {
            int IsEmpty = -1;
            List<DataRecord> rDelete = new List<DataRecord>();

            if (gridToCheck.xGrid.Records != null && gridToCheck.xGrid.Records.Count > 0)
            {
                //Loop through the grid records looking for empty records
                foreach (DataRecord r in gridToCheck.xGrid.Records)
                {

                    //Check for empty records
                    if (Convert.ToDecimal(r.Cells["cs_id"].Value) == 0)
                    {
                        //Record the record index
                        IsEmpty = r.Index;
                        //If not deleting then return the index and exit the method
                        if (!deleteEmpty)
                            return IsEmpty;
                        //Otherwise add the record to the delete list
                        rDelete.Add(r);
                    }
                    else
                        continue;

                }
                //Delete records
                if (deleteEmpty && IsEmpty != -1)
                {
                    foreach (DataRecord r in rDelete)
                    {
                        DataRow row = (r.DataItem as DataRowView).Row;
                        if (row != null)
                        {
                            row.Delete();
                            CurrentState = ScreenState.Deleting;

                        }
                    }

                }

            }
            return IsEmpty;


        }



        public override void Save()
        {
            

            //Need to check grid for empty rows and if found,delete them from the grid
            GridEmptyRowsProcessing(gUnitEntry, true);

            //DWR Added 2/19/13 - To make sure and update grid values before checking for missing info.
            Prep_ucBaseGridsForSave();

            //Validate all required fields are populated
            bool RequiredError = false;
            foreach (DataRecord r in gUnitEntry.xGrid.Records)
            {
                if ((Convert.ToInt32(r.Cells["cs_id"].Value) == 0))
                {
                    MessageBox.Show("Location is required. ");
                    RequiredError = true;
                    return;
                }
                if ((Convert.ToString(r.Cells["product_code"].Value) == ""))
                {
                    MessageBox.Show("Product is required. ");
                    RequiredError = true;
                    return;
                }
                if ((Convert.ToInt32(r.Cells["contract_id"].Value) == 0))
                {
                    MessageBox.Show("Contract is required. ");
                    RequiredError = true;
                    return;
                }
                if ((Convert.ToInt32(r.Cells["unit_type_id"].Value) == 0))
                {
                    MessageBox.Show("Unit type is required. ");
                    RequiredError = true;
                    return;
                }
                if ((Convert.ToInt32(r.Cells["unit_type_id"].Value) == 5))
                {
                    if ((Convert.ToInt32(r.Cells["data_service_type_id"].Value) == 0))
                    {
                        MessageBox.Show("Data Type is required. ");
                        RequiredError = true;
                        return;
                    }
                }
                if ((Convert.ToInt32(r.Cells["unit_type_id"].Value) == 7))
                {
                    if ((Convert.ToInt32(r.Cells["tivo_count_id"].Value) == 0))
                    {
                        MessageBox.Show("Tivo Count Type is required. ");                
                        RequiredError = true;
                        return;
                    }
                }
                if ((Convert.ToInt32(r.Cells["unit_type_id"].Value) == 1))
                {
                    if ((Convert.ToInt32(r.Cells["subscriber_id"].Value) == 0))
                    {
                        MessageBox.Show("Subscriber Type is required. ");
                        RequiredError = true;
                        return;
                    }
                }
                //if ((Convert.ToString(r.Cells["report_id"].Value) == ""))
                //{
                //    r.Cells["report_id"].Value = 0;
                //}
                // if ((Convert.ToInt32(r.Cells["subscriber_type"].Value) == 0))
                //{
                //    MessageBox.Show("Subscriber Type is required. ");
                //    RequiredError = true;
                //    return;
                //}
                if ((Convert.ToString(r.Cells["service_period_start"].Value) == "01/01/1900 12:00:00 AM" )  ||
                     (Convert.ToString(r.Cells["service_period_start"].Value) == "1/1/1900 12:00:00 AM" ))
                {
                     MessageBox.Show("Service Period Start is required. ");
                     RequiredError = true;
                     return;
                 }
                string stest = "";
                stest = Convert.ToString(r.Cells["service_period_start"].Value);
                 if ((Convert.ToString(r.Cells["service_period_end"].Value)  == "01/01/1900 12:00:00 AM") ||
                     (Convert.ToString(r.Cells["service_period_end"].Value) == "1/1/1900  12:00:00 AM"))
                    
                 {
                     MessageBox.Show("Service Period End is required. ");
                     RequiredError = true;
                     return;
                 }
            }
            if (RequiredError == false)
            {
                base.Save();
                if (SaveSuccessful)
                {
                    Messages.ShowInformation("Unit Entry Saved!");
                    CurrentState = ScreenState.Inserting;
                    //Remove any existing parameters
                    CurrentBusObj.Parms.ClearParms();
                    //Add all parameters back in
                    CurrentBusObj.Parms.AddParm("@cs_id_parm", "0");
                    CurrentBusObj.Parms.AddParm("@product_code_parm", "");
                    CurrentBusObj.Parms.AddParm("@contract_id_parm", 0);
                    //RES 11/1/24 Add unit type selection
                    //CurrentBusObj.Parms.AddParm("@unit_type_id_parm", 1);
                    CurrentBusObj.Parms.AddParm("@unit_type_id_parm", 0);
                    CurrentBusObj.Parms.AddParm("@data_service_id_parm", 0);
                    CurrentBusObj.Parms.AddParm("@tivo_count_id_parm", 0);
                    //CurrentBusObj.Parms.AddParm("@unit_id_parm", 0);
                    CurrentBusObj.Parms.AddParm("@subscriber_id_parm", 0);
                    CurrentBusObj.Parms.AddParm("@service_period_start_parm", "01/01/1900");
                    CurrentBusObj.Parms.AddParm("@service_period_end_parm", "01/01/1900");
                    CurrentLocID = 0;
                    //Loads a new record in the main table and empty datatbales elsewhere
                    this.Load();
                    this.CreateNewUnitRow();

                }

            }
            else
                MessageBox.Show("Save Failed");

        }

        public void gUnitEntry_DoubleClick()
        {
            if (gUnitEntry.xGrid.ActiveRecord != null)
            {
                if (gUnitEntry.xGrid.ActiveCell.Field.Name == "cs_id")
                {
                    //Event handles opening of the lookup window upon double click on Location ID field
                    LocationLookup f = new LocationLookup();

                    cGlobals.ReturnParms.Clear();

                    // gets the users response
                    f.ShowDialog();

                    // Check if a value is returned
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        gUnitEntry.xGrid.ActiveCell.Value = cGlobals.ReturnParms[0].ToString();

                        // Clear the parms
                        cGlobals.ReturnParms.Clear();

                    }
                }
            }
        }
        
    }
}
