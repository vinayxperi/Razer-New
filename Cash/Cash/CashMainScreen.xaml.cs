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

namespace Cash
{
    /// <summary>
    /// Interaction logic for CashMainScreen.xaml
    /// IScreen is required by Unity if the screen will be launched from the menu or a zoom
    /// IPrebindable is required if the window uses the ucLabelComboBox
    /// </summary>
    public partial class CashMainScreen :ScreenBase ,IScreen, IPreBindable
    {
        //Property is required for base objects that use IScreen
        public string WindowCaption { get; private set; }

        //Screen Level Batch ID
        public int CurrentBatchID { get; set; }

        //This datatable is being added so that the Amount Allocated text box can have a binding
        //Do this for fields that contain informational data but that will not be saved
        //so that you can use converters or other benefits of binding
        DataTable dtMiscInfo = new DataTable("MiscInfo");

        //Setup any grid combobox providers - Variable name should match the Binding Path value in the field layout
        public ComboBoxItemsProvider cmbCurrency { get; set; }
        public ComboBoxItemsProvider cmbProducts { get; set; }
        public ComboBoxItemsProvider cmbSeq { get; set; }

        

        //Setup keys for double click zooms from grids
        private List<string> gRemitDataKeys = new List<string> { "document_id"}; //Used for double click
        
        //Miscellaneous variables and properties
        //Variable for tracking a temporary remit id
        private Int32 CurrentRemitID = 0;

    
        //Required constructor
        public CashMainScreen()
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
            WindowCaption = "Cash";
            
            //Setting intial screenstate to empty
            CurrentState = ScreenState.Empty;

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder ;

             //Set the maintablename for the folder if it has one
            this.MainTableName = "cash_batch";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            //Add any Grid Configuration Information
            gRemit.MainTableName = "remit"; //Should match the ROBJECT table name
            gRemit.ConfigFileName = "CashRemitGrid"; //This is the file name that will store any user customizations to the grid - must be unique in the app
            gRemit.SetGridSelectionBehavior(true, false); //Sets standard grid behavior for record select and multiselect
            gRemit.WindowZoomDelegate = RemitGridDoubleClick; //The function delegate determines what happens when the user double clicks the grid
            gRemit.FieldLayoutResourceString = "CashRemitLayout"; //The name of the FieldLayout in the Field Layouts xaml file - Must be unique
            gRemit.GridGotFocusDelegate = gRemitGrid_GotFocus; //This ties the got focus event of the remit base grid to this method.
            gRemit.EditModeEndedDelegate = gRemitGrid_EditModeEnded; //This allows for data checks after each cell is exited
            gRemit.SkipReadOnlyCellsOnTab = true; //Sets the grid to skip over non edit fields
            gRemit.GridPreviewKeyDownDelegate = gRemitGrid_GridPreviewKeyDown;
            gRemit.RecordActivatedDelegate = gRemit_RecordActivated;

            //Turn off Grid config and filter abilities as these will mess up the hardwiring of tabbing thorugh grid
            gRemit.ContextMenuToggleFilterIsVisible = false;
            gRemit.ContextMenuResetGridSettingsIsVisible = false;
            gRemit.ContextMenuSaveGridSettingsIsVisible = false;
            gRemit.ContextMenuRemoveIsVisible = false;
            gRemit.ContextMenuAddIsVisible = false;

 
            //Rules Detail Grid
            gAlloc.MainTableName = "remit_alloc";
            gAlloc.ConfigFileName = "CashRemitAllocGrid";
            gAlloc.SetGridSelectionBehavior(true, false);
            gAlloc.WindowZoomDelegate = RemitAllocGridDoubleClick;
            gAlloc.FieldLayoutResourceString = "CashRemitAllocLayout";
            gAlloc.GridGotFocusDelegate = gAllocGrid_GotFocus; //This ties the got focus event of the remit base grid to this method.
            gAlloc.EditModeEndedDelegate = gAllocGrid_EditModeEnded; //This allows for data checks after each cell is exited
            gAlloc.SkipReadOnlyCellsOnTab = true; //Sets the grid to skip over non edit fields
            gAlloc.GridPreviewKeyDownDelegate = gAllocGrid_GridPreviewKeyDown;
            
            //Turn off Grid config and filter abilities as these will mess up the hardwiring of tabbing thorugh grid
            gAlloc.ContextMenuToggleFilterIsVisible  = false;
            gAlloc.ContextMenuResetGridSettingsIsVisible = false;
            gAlloc.ContextMenuSaveGridSettingsIsVisible = false;
            gAlloc.ContextMenuRemoveIsVisible = false;
            gAlloc.ContextMenuAddIsVisible = false;

            //This command sets up the allocation grid to be a child of the remit grid
            //This will have the alloc grid automatically filter by the key fields provided in the command
            gRemit.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "remit_id"}, ChildGrids = { gAlloc }, ParentFilterOnColumnNames = { "remit_id"} });


            //Add all grids to the grid collection - This allows grids to automatically load and participate with security
            GridCollection.Add(gRemit);
            GridCollection.Add(gAlloc);
   
            // add the Tab user controls that are of type screen base - This screen has no detail tabs
            //TabCollection.Add(General);
 
            //Adding fields to the MiscInfo datatable - Fields represent fields that are not bound through
            //the base business object and are not needed for the database
            dtMiscInfo.Columns.Add("total_allocated");
            dtMiscInfo.Columns.Add("total_remit");
            dtMiscInfo.Columns.Add("remit_line_total");
            //This statement adds one row that will be used to hold the data.
            dtMiscInfo.Rows.Add("0","0","0");

            //Bind the screen field(s) to the datatable
            tRemitTotal.DataContext = dtMiscInfo;
            tAllocTotal.DataContext = dtMiscInfo;
            tRemitLineTotal.DataContext = dtMiscInfo;

            //Debug code for hardwiring a test parameter set
            //CurrentBusObj.Parms.AddParm("@batch_id", "1" );

            // if there are parameters passed into the window on startup then we need to load the data
            if (CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                tBatchID.Text = CurrentBusObj.Parms.GetParm("@batch_id");
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
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    this.cbBank.SetBindingExpression("bank_id", "account_description", this.CurrentBusObj.ObjectData.Tables["bank"]);
                    this.cbSource.SetBindingExpression("source_id", "description", this.CurrentBusObj.ObjectData.Tables["batch_source"]);

                    //Setup Grid Combo Boxes
                    //Currency combo box on remit grid
                    //Add code to populate the grid combobox on the Company Contract Grid
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip.ItemsSource = this.CurrentBusObj.ObjectData.Tables["currency"].DefaultView;
                    //set the value and display path
                    ip.ValuePath = "currency_code";
                    ip.DisplayMemberPath = "currency_code";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbCurrency = ip;

                    //Product drop down box - alloc grid
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["products"].DefaultView;
                    ip.ValuePath = "product_code";
                    ip.DisplayMemberPath=("product_description");
                    cmbProducts = ip;

                    //apply to seq down box - alloc grid
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["open_cash"].DefaultView;
                    ip.ValuePath = "seq_code";
                    ip.DisplayMemberPath = ("lookup_description");
                    cmbSeq = ip;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
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
                dpAcctPeriod.SelText = Convert.ToDateTime("1/1/1900");
                dpBatchDate.SelText = DateTime.Now.AddDays(-1);
 
                cbSource.CntrlFocus() ;
                
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
                CurrentBusObj.Parms.AddParm("@batch_id", "-1");
                CurrentBusObj.Parms.AddParm("@doc_id_lookup", "");
                CurrentBusObj.Parms.AddParm("@conversion_date", "");
                CurrentBusObj.Parms.AddParm("@from_currency", "");
                CurrentBusObj.Parms.AddParm("@to_currency", "");
                CurrentBusObj.Parms.AddParm("@receivable_account_lookup", "");
                tBatchID.Text = "-1";
                //Loads a new record in the main table and empty datatbales elsewhere
                base.New();
                            //Reset the current remit ID
                CurrentRemitID = 0;
            }
            //We have a batch ID to retrieve
            else
            {
                //test to make sure The batch is an integer
                int i;
                if (Int32.TryParse(tBatchID.Text, out i))
                {
                    //Remove existing paramters
                    CurrentBusObj.Parms.ClearParms();
                    //Add all parameters back in
                    CurrentBusObj.Parms.AddParm("@batch_id", tBatchID.Text);
                    CurrentBusObj.Parms.AddParm("@doc_id_lookup", "");
                    CurrentBusObj.Parms.AddParm("@conversion_date", "");
                    CurrentBusObj.Parms.AddParm("@from_currency", "");
                    CurrentBusObj.Parms.AddParm("@to_currency", "");
                    CurrentBusObj.Parms.AddParm("@receivable_account_lookup", "");
                    //Empty the current object if any exists
                    if (CurrentBusObj.HasObjectData)
                        CurrentBusObj.ObjectData.Clear();

                    //Load the base business object and populate the window and tabs
                    this.Load();
 
                }
                else //If non numeric entered
                {
                    MessageBox.Show("Batch ID must be a numeric whole number value");
                    return false;
                }
            }

            

            //Verify that data was returned
            if (CurrentBusObj.ObjectData.Tables["cash_batch"] != null && CurrentBusObj.ObjectData.Tables["cash_batch"].Rows.Count>0)
            {
                CurrentBatchID=Convert.ToInt32(tBatchID.Text) ;
                CalculateAmountAllocated();
                SetWindowStatus();
                //Reset the current remit ID - Move to the max rows +1 for any new inserts
                CurrentRemitID = CurrentBusObj.ObjectData.Tables["remit"].Rows.Count +1;
                return true;
            }
            else
            {
                MessageBox.Show("No data retrieved for batch id " + tBatchID.Text);
                //Reset the current remit ID
                CurrentRemitID = 0;
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
                    Messages.ShowInformation("Batch Deleted");
                    //Clear out the totals table
                    dtMiscInfo.Rows.Clear();
                    dtMiscInfo.Rows.Add("0", "0", "0");
                    tBatchID.CntrlFocus();
                    tBatchID.Text = "";
                }
                else
                {
                    tBatchID.CntrlFocus();
                    tBatchID.Text = CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_id"].ToString();
                    Messages.ShowInformation("Save Successful");

                    //If the save was successful then recalc amounts
                    CalculateAmountAllocated();
                }

            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
           
           
        }

        /// <summary>
        /// Base method override for deletion of the object
        /// The delete works by all of the rows in the appropriate tables being deleted from the dataset
        /// Then doing a Save()
        /// </summary>
        public override void Delete()
        {
            //Verify the batch is not posted - If posted then the user cannot delete
            //This should be handled at the command level as well to prevent the menu option from even being selected
            if (chkPosted.IsChecked == 1)
            {
                MessageBox.Show("Posted batches cannot be deleted.");
                return;
            }
            else //Not a posted batch
            {
                //Verify the delete
                System.Windows.MessageBoxResult result = Messages.ShowYesNo("Are you sure you would like to delete batch " + tBatchID.Text + "?",
                           System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    //base.Delete();
                    CurrentState = ScreenState.Deleting;
                    //Delete from all of the datatables using the base business object DaleteAllTableRows method
                    if (CurrentBusObj.ObjectData.Tables["remit_alloc"] != null && CurrentBusObj.ObjectData.Tables["remit_alloc"].Rows.Count > 0)
                        CurrentBusObj.DeleteAllTableRows("remit_alloc");

                    if (CurrentBusObj.ObjectData.Tables["remit"] != null && CurrentBusObj.ObjectData.Tables["remit"].Rows.Count > 0)
                        CurrentBusObj.DeleteAllTableRows("remit");

                    if (CurrentBusObj.ObjectData.Tables["cash_batch"] != null && CurrentBusObj.ObjectData.Tables["cash_batch"].Rows.Count > 0)
                        CurrentBusObj.DeleteAllTableRows("cash_batch");
                    
                    //Run the save to delete all rows from database
                    Save();
                    if (SaveSuccessful)
                    {
                        CurrentState = ScreenState.Empty;
                        SetWindowStatus();
                    }
                    else
                    {
                        CurrentState = ScreenState.Normal;
                    }
                }
            }
        }

        /// <summary>
        /// This method verifies that all of the totals match before allowing a save
        /// If all totals match this method also deletes any $0 rows from the remit and alloc grids
        /// Then all other data verifications are made before the save is attempted
        /// All error messages are stored in the temporary string s.  If an empty string is in s then the data will be considered verified.
        /// Otherwise the contents of s will be returned to the user through a message window.
        /// </summary>
        /// <returns></returns>
        private bool VerifyData()
        {
            //Recalculate bank charge totals
            //Appears that this sub is not needed as the calculation is performed in the remit grid when the amount field is
            //exited so it will commented out for now.
            //CalculateBankCharges();

            //Compare the totals to see if the transaction is not balanced
            string s = CompareTotals();

            //Return any errors to the user and exit
            if (s  != "")
            {
                MessageBox.Show(s);
                return false;
            }

            //Delete empty rows in grids - Empty rows in this case are rows with $0 in the amount fields
            GridEmptyRowsProcessing(gAlloc, true);
            GridEmptyRowsProcessing(gRemit, true);
            
            //Verify that all allocations have either product,account and unapplied flag checked or have document and seq
            foreach (DataRow r in CurrentBusObj.ObjectData.Tables["remit_alloc"].Rows)
            {
                //If unapplied flag is checked verify that:
                //1) amount is greater than 0 
                //2) product code has a value
                //3) Receivable account has a value - This may need to be changed to check that the value is valid
                if (Convert.ToInt32(r["unapplied_flag"]) == 1)
                {
                    if (Convert.ToDecimal(r["amount"]) < 0)
                    {
                        s += "Allocated amount must be greater than 0. ";
                    }

                    if (r["product_code"].ToString() == "")
                    {
                        s += "Unapplied Cash must have a product code assigned. ";
                    }

                    if (r["receivable_account"].ToString() == "")
                    {
                        s += "You must enter a valid customer number. ";
                    }

                }
                else //Applied amount
                {
                    //If unapplied flag is not checked verify that:
                    //1) amount is greater than 0 
                    //2) apply to doc has a value
                    //3) apply to seq has a non zero non blank value
                    if (Convert.ToDecimal(r["amount"]) < 0)
                        s += "Allocated amount must be greater than 0. ";
                 
                    if (r["apply_to_doc"].ToString() == "")
                       s += "Must have valid apply to document. ";
                    
                    if (r["apply_to_seq"].ToString()  == "0" || r["apply_to_seq"].ToString()  == "")
                       s += "Must have valid apply to seq. ";
                }

                //Verify Currency codes match between remit and remit alloc document selected
                 EnumerableRowCollection<DataRow> RemitRow = from RemitTable in CurrentBusObj.ObjectData.Tables["remit"].AsEnumerable()
                                                         where RemitTable.Field<Int32>("remit_id") == Convert.ToInt32(r["remit_id"]) 
                                                         select RemitTable;

                 foreach (DataRow remit in RemitRow)
                 {
                     if (r["currency_code"].ToString() != remit["currency_code"].ToString())
                         s += "Remit " + remit["remit_number"].ToString() + " currency code of " + remit["currency_code"] +
                            " does not equal allocation currency code of " + r["currency_code"].ToString();
                 }

            }
            //Verify remit info
            //1) Must have a remit number
            //2) Amount must be greater than zero
            //3) Exchange rate should not equal 1.0 if cash batch currency does not match remit currency
            foreach(DataRow r in CurrentBusObj.ObjectData.Tables["remit"].Rows)
            {
                if (r["remit_number"].ToString() == "")
                    s += "All remit lines must a have remit number. ";

                if(Convert.ToDecimal(r["amount"]) <0 || Convert.ToDecimal(r["amount_functional"]) <0 )
                    s += "Remit amounts must be greater than 0. ";

                if (r["currency_code"].ToString() != CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["currency_code"].ToString() &&
                        Convert.ToDecimal(r["exchange_rate"]) == 1.0M)
                    s += "Invalid exchange rate for transaction.";
            }

            if (s != "")
            {
                MessageBox.Show(s);
                return false;
            }
            else
                return true;

        }

        /// <summary>
        /// This event tracks the primary data key field to see if the value changes once the field is exited.  
        /// The field will be exited either by a tab or an enter.  At that point if the ID has changed then
        /// the database is requeried by the base business object. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tBatchID_LostFocus(object sender, RoutedEventArgs e)
        {
            int i;
            Int32.TryParse(tBatchID.Text, out i);
            if (i != CurrentBatchID)
            {
                CurrentState = ScreenState.Normal;
                RetrieveData();
            }
        }

        /// <summary>
        /// This event handles the double click launching of a lookup window.  If a value is returned from the lookup window
        /// then the data for the window is requeried.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tBatchID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CashLookup w = new CashLookup();
            w.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                tBatchID.Text = cGlobals.ReturnParms[0].ToString();
                // Clear the Global parms
                //This prevents invalid data being passed to other lookups
                cGlobals.ReturnParms.Clear();
                CurrentState = ScreenState.Normal;
                RetrieveData();
            }
        }

        /// <summary>
        /// This event is the WindowZoomDelegate for the remit grid
        /// It will handle any double click lookups or zooms for the grid.
        /// For this grid it will allow a zoom to the view cash screen if the screen
        /// has a value in the document id field
        /// </summary>
        public void RemitGridDoubleClick()
        {
            if (gRemit.DoubleClickFieldName == "document_id")
                //if (gAlloc.xGrid.ActiveCell != null && gAlloc.xGrid.ActiveCell.Field.Name == "document_id")
            {
                //Get the document_id field from the grid
                gRemit.ReturnSelectedData(gRemitDataKeys);
                //If the document id is not null or blank then zoom to window
                if (cGlobals.ReturnParms[0] != null && cGlobals.ReturnParms[0].ToString() != "")
                {
                    //Set appropriate double click destination
                    cGlobals.ReturnParms.Add("GridCashZoom");
                    RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                    args.Source = gRemit.xGrid;
                    EventAggregator.GeneratedClickHandler(this, args);
                    cGlobals.ReturnParms.Clear();
                }
            }
        }
        
        
        /// <summary>
        /// This event is the WindowZoomDelegate for the remit grid
        /// It will handle any double click lookups or zooms for the grid.
        /// For this grid it will launch the invoice or receivable account lookup based
        /// on which field was double clicked
        /// </summary>
        public void RemitAllocGridDoubleClick()
        {
            //Determine the document type and then set appropriate double click destination
            //Apply to doc lookup shows all open cash documents that match the currency selected in the remit row
            if (gAlloc.xGrid.ActiveCell!=null && gAlloc.xGrid.ActiveCell.Field.Name == "apply_to_doc")
            {
                cGlobals.ReturnParms.Clear();
                cGlobals.ReturnParms.Add(gRemit.ActiveRecord.Cells["currency_code"].Value.ToString()??string.Empty );
                cGlobals.ReturnParms.Add("tBatchTotal");
                RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                args.Source = tBatchTotal;
                EventAggregator.GeneratedClickHandler(this, args);

                if (cGlobals.ReturnParms.Count > 0)
                {
                    gAlloc.ActiveRecord.Cells["apply_to_doc"].Value = cGlobals.ReturnParms[0].ToString();
                    gAlloc.ActiveRecord.Cells["apply_to_seq"].Value = cGlobals.ReturnParms[1].ToString();
                }
            }
            //Receivable account lookup opens the standard customer lookup screen.
            else if (gAlloc.xGrid.ActiveCell != null && gAlloc.xGrid.ActiveCell.Field.Name == "receivable_account")
            {
                cGlobals.ReturnParms.Clear();
                RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                args.Source = tBatchID;
                EventAggregator.GeneratedClickHandler(this, args);

                if (cGlobals.ReturnParms.Count > 0)
                {
                    gAlloc.ActiveRecord.Cells["receivable_account"].Value = cGlobals.ReturnParms[0].ToString();
                }
            }
            cGlobals.ReturnParms.Clear();
        }

        /// <summary>
        /// Method to establish field and grid status and editability
        /// </summary>
        private void SetWindowStatus()
        {
            //See if posted or not
            if (chkPosted.IsChecked == 0)
            {
                if (CurrentState == ScreenState.Empty)
                {
                    gRemit.xGrid.FieldSettings.AllowEdit = false;
                    gAlloc.xGrid.FieldSettings.AllowEdit = false;
                    tBatchTotal.IsEnabled = false;
                    cbSource.IsEnabled = false;
                    cbBank.IsEnabled = false;
                    dpBatchDate.IsEnabled = false;
                    gRemit.SetGridSelectionBehavior(true, false);
                    gAlloc.SetGridSelectionBehavior(true, false);


                }
                else //Not an empty screen and not posted
                {

                    gRemit.xGrid.FieldSettings.AllowEdit = true;
                    gAlloc.xGrid.FieldSettings.AllowEdit = true;
                    tBatchTotal.IsEnabled = true;
                    cbSource.IsEnabled = true;
                    cbBank.IsEnabled = true;
                    dpBatchDate.IsEnabled = true;
                   gRemit.SetGridSelectionBehavior(false, false);
                    gAlloc.SetGridSelectionBehavior(false, false);
                    //Set delete to enable on menu
                    CanExecuteDeleteCommand = true;
                    gRemit.xGrid.FieldLayouts[0].Fields["remit_number"].Settings.AllowEdit = true;
                 }

            }
            else //Posted
            {
                CurrentState = ScreenState.Locked;
                gRemit.xGrid.FieldSettings.AllowEdit = false;
                gAlloc.xGrid.FieldSettings.AllowEdit = false;
                tBatchTotal.IsEnabled = false;
                cbSource.IsEnabled = false;
                cbBank.IsEnabled = false;
                dpBatchDate.IsEnabled = false;
                gRemit.SetGridSelectionBehavior(true, false);
                gAlloc.SetGridSelectionBehavior(true, false);
                //Set delete to disabled on menu
                CanExecuteDeleteCommand = false;
                gRemit.xGrid.FieldLayouts[0].Fields["remit_number"].Settings.AllowEdit = false;
            }

        }

        //***Methods below this point are specific to the cash window

        /// <summary>
        /// This method calculates the current amount of the total remittance and the total allocation
        /// and places the amounts in the dtMiscInfo datatable used for binding
        /// It is fired with every data load and every change to a remit or allocation amount
        /// </summary>
        private void CalculateAmountAllocated()
        {
            if (CurrentState == ScreenState.Deleting || CurrentState == ScreenState.Empty)
            {
                return;
            }

            decimal dAmount = 0.0M;
            decimal dAmountRow = 0.0M;
            
            //Calculate the current amount allocated for the remit currently selected.  This will be in local currency and place in the total_allocated field
            if (CurrentBusObj.ObjectData.Tables["remit_alloc"] != null && CurrentBusObj.ObjectData.Tables["remit_alloc"].Rows.Count > 0 && gRemit.ActiveRecord!=null)
            {
                int RowRemitID = Convert.ToInt32(gRemit.ActiveRecord.Cells["remit_id"].Value);
                foreach (DataRecord r in gAlloc.xGrid.Records )
                {
                    if (Convert.ToInt32(r.Cells["remit_id"].Value) == RowRemitID)
                    {
                        dAmountRow = 0.0M;
                        decimal.TryParse(r.Cells["amount"].Value.ToString(), out dAmountRow);
                        dAmount += dAmountRow;
                    }
                }
            }
            dtMiscInfo.Rows[0]["total_allocated"] = dAmount;

            dAmount = 0.0M;
            dAmountRow = 0.0M;
            //Calculate the total of all remittances in the functional currency and put in the total_remit field
            if (CurrentBusObj.ObjectData.Tables["remit"] != null && CurrentBusObj.ObjectData.Tables["remit"].Rows.Count > 0)
            {
                foreach (DataRow r in CurrentBusObj.ObjectData.Tables["remit"].Rows)
                {
                    dAmountRow = 0.0M;
                    decimal.TryParse(r["amount_functional"].ToString(), out dAmountRow);
                    dAmount += dAmountRow;
                }
            }
            dtMiscInfo.Rows[0]["total_remit"] = dAmount;

            //Determine the remit line total by getting the local amount from the amount field on the selected remit
            if (gRemit.ActiveRecord != null)
                dtMiscInfo.Rows[0]["remit_line_total"] = gRemit.ActiveRecord.Cells["amount"].Value ;
            else
                dtMiscInfo.Rows[0]["remit_line_total"] = 0;
        }

        /// <summary>
        /// This method compares the totals of the cash being entered
        /// It checks to see if batch total equals all of the remit line functional totals
        /// and if all individual remit line local amounts match the amount allocated in the remit_alloc grid
        /// This method returns an empty string if the totals match and a message if they do not.
        /// </summary>
        /// <returns></returns>
        private string CompareTotals()
        {
            string TotalsOutOfBalance = "";
            CalculateAmountAllocated();

            //Compare the batch total to the remit total
            if (Convert.ToDecimal(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_total"]) != Convert.ToDecimal(dtMiscInfo.Rows[0]["total_remit"]))
            {
                TotalsOutOfBalance="Batch total must match total of all remittances. ";
            }
            //Compare each individual remit total to the sum of the related allocations
            foreach (DataRow rRemit in CurrentBusObj.ObjectData.Tables["remit"].Rows)
            {
                decimal remitSum = 0.0M;

                EnumerableRowCollection<DataRow> rAlloc = from RemitAlloc in CurrentBusObj.ObjectData.Tables["remit_alloc"].AsEnumerable()
                                                         where RemitAlloc.Field<Int32>("remit_id") == Convert.ToInt32(rRemit["remit_id"])
                                                         select RemitAlloc ;
                //Total the related allocations
                foreach (DataRow r in rAlloc)
                {
                    remitSum+=Convert.ToDecimal(r["amount"]);
                }
                if (remitSum != Convert.ToDecimal(rRemit["amount"]))
                {
                    //Once any error is received record message and exit the loop.
                    TotalsOutOfBalance += "Remit local amounts and allocation amounts must match. ";
                    break;
                }
            }

            return TotalsOutOfBalance;
        }


        //This method is not currently needed
        //private void CalculateBankCharges()
        //{
        //}

        /// <summary>
        /// This method creates a new empty remit row and puts the cursor in the first box in edit mode
        /// </summary>
        private void CreateNewRemitRow()
        {
            //If a previously empty row exists, jump the cursor to that row and do not create a new row
            int EmptyRow = GridEmptyRowsProcessing(gRemit, false);
            if (EmptyRow != -1)
            {
                gRemit.xGrid.ActiveRecord = gRemit.xGrid.Records[EmptyRow];
            }
            else
            {
                //Increment temp remit ID
                CurrentRemitID += 1;
                //Sets the grid to allow a new row to be created
                gRemit.xGrid.FieldLayoutSettings.AllowAddNew = true;
                //Establishes a local variable tied to the row we just created
                DataRecord row = gRemit.xGrid.RecordManager.CurrentAddRecord;
                //Set the default values for the columns
                row.Cells["batch_id"].Value = 0;
                row.Cells["remit_id"].Value = CurrentRemitID;
                row.Cells["document_id"].Value = "";
                row.Cells["remit_type_id"].Value = 1;
                row.Cells["remit_date"].Value = CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_date"];
                row.Cells["remit_number"].Value = "";
                row.Cells["amount"].Value = 0.0;
                row.Cells["amount_functional"].Value = 0.0;
                row.Cells["currency_code"].Value = CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["currency_code"];
                row.Cells["exchange_rate"].Value = 1.0;
                row.Cells["bank_charge_amount"].Value = 0.0;
                row.Cells["amnt_alloc"].Value = 0.0;

                //Commit the add new record - Required so that we can get rid of the add new record box and make this record active
                gRemit.xGrid.RecordManager.CommitAddRecord();
                //Remove the add new record row
                gRemit.xGrid.FieldLayoutSettings.AllowAddNew = false;
                //Set the row we just created to the active record
                gRemit.xGrid.ActiveRecord = gRemit.xGrid.Records[0];
            }
            //Set the remit number field as active
            (gRemit.xGrid.Records[gRemit.ActiveRecord.Index] as DataRecord).Cells["remit_number"].IsActive = true;
            //Moves the cursor into the active cell - This code may be required to get the cell in edit mode without clicking.
            //gRemit.xGrid.Records.DataPresenter.BringCellIntoView(gRemit.xGrid.ActiveCell);
            //Puts the cell into edit mode
            gRemit.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        /// <summary>
        /// This method creates a new alloc row and puts the row into edit mode.
        /// </summary>
        private void CreateNewAllocRow()
        {
            int EmptyRow = GridEmptyRowsProcessing(gAlloc, false);
            if (EmptyRow != -1)
            {
                gAlloc.xGrid.ActiveRecord = gAlloc.xGrid.Records[EmptyRow];
            }
            else
            {
                //Sets the grid to allow a new row to be created
                gAlloc.xGrid.FieldLayoutSettings.AllowAddNew = true;
                //Establishes a local variable tied to the row we just created
                DataRecord row = gAlloc.xGrid.RecordManager.CurrentAddRecord;

                //Set the default values for the columns
                if (CurrentBusObj.ObjectData.Tables["cash_batch"].Rows != null && CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_id"].ToString() != "0")
                    row.Cells["batch_id"].Value = CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_id"];
                else
                    row.Cells["batch_id"].Value = 0;

                row.Cells["remit_id"].Value = gRemit.ActiveRecord.Cells["remit_id"].Value.ToString();

                //Check to see if the remit ID is a temporalily assigned ID
                //If it is then flag the alloc row that it will need a new remit id on save
                //Otherwise use the remit ID passed on save
                if (Convert.ToInt32(gRemit.ActiveRecord.Cells["remit_id"].Value) == CurrentRemitID)
                    row.Cells["need_remit_id"].Value = 1;
                else
                    row.Cells["need_remit_id"].Value = 0;
                
                //else
                //    row.Cells["remit_id"].Value = 0;

                row.Cells["remit_alloc_id"].Value = 0;
                row.Cells["apply_to_doc"].Value = "";
                row.Cells["apply_to_seq"].Value = 0;
                row.Cells["amount"].Value = 0.0;
                row.Cells["company_code"].Value = "";
                row.Cells["receivable_account"].Value = "";
                row.Cells["product_code"].Value = "";
                row.Cells["unapplied_flag"].Value = 0;
                row.Cells["currency_code"].Value = gRemit.ActiveRecord.Cells["currency_code"].Value.ToString() ?? string.Empty;


                //Commit the add new record - Required so that we can get rid of the add new record box and make this record active
                gAlloc.xGrid.RecordManager.CommitAddRecord();
                //Remove the add new record row
                gAlloc.xGrid.FieldLayoutSettings.AllowAddNew = false;
                //Set the row we just created to the active record
                gAlloc.xGrid.ActiveRecord = gAlloc.xGrid.Records[0];
            }
            //Set the remit number field as active
            (gAlloc.xGrid.Records[gAlloc.ActiveRecord.Index] as DataRecord).Cells["unapplied_flag"].IsActive = true;
            //Moves the cursor into the active cell - This code may be required to get the cell in edit mode without clicking.
            gAlloc.xGrid.Records.DataPresenter.BringCellIntoView(gAlloc.xGrid.ActiveCell);
            //Puts the cell into edit mode
            gAlloc.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }


        /// <summary>
        /// Event fires anytime the bank combo box is changed and screen is in insert mode
        /// and sets the value of the currency code at the batch level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbBank_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentState == ScreenState.Inserting || CurrentState==ScreenState.Editable )
            {
                EnumerableRowCollection<DataRow> Banks = from BankTable in CurrentBusObj.ObjectData.Tables["bank"].AsEnumerable()
                                                         where BankTable.Field<Int32>("bank_id") == Convert.ToInt32(cbBank.SelectedValue)
                                                         select BankTable;

                foreach (DataRow r in Banks)
                {
                    tCurrencyCode.Text = r["currency_code"].ToString();
                }
            }
        }


        //*** Grid events
        /// <summary>
        /// Captures when the grid receives focus and if screen is in insert mode and batch total does not equal remit total
        /// will automatically insert rows.
        /// 
        /// </summary>
        public void gRemitGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            //This if tests to see if we are already in the got focus event as infragistics fires the got focus when the active record changes
            //******This only appears to work when the grid is empty and receives focus - otherwise if a row or cell receives focus, the below if statement
            //will cause the event to be exited
            if (!gRemit.xGrid.IsFocused)
            {
                e.Handled = true;
                return;
            }

            //If we are in insert mode or the item is not posted and the remit total does not match the batch total
            if (CurrentState == ScreenState.Inserting || (tBatchTotal.Text != tRemitTotal.Text && chkPosted.IsChecked == 0 && CurrentState != ScreenState.Empty))
            {
                CreateNewRemitRow();
            }
        }

        public void gAllocGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            //This if tests to see if we are already in the got focus event as infragistics fires the got focus when the active record changes
            if (!gAlloc.xGrid.IsFocused)
            {
                e.Handled = true;
                return;
            }

            //If we are in insert mode or the item is not posted and the remit total does not match the batch total
            if (CurrentState == ScreenState.Inserting || (tBatchTotal.Text != tRemitTotal.Text && chkPosted.IsChecked == 0 && CurrentState!= ScreenState.Empty ))
            {
                CreateNewAllocRow();
            }
        }

        /// <summary>
        /// Event causes any row change on the grid to recalculate all amounts
        /// </summary>
        public void gRemit_RecordActivated()
        {
            CalculateAmountAllocated();
        }


        /// <summary>
        /// This event is used to determine what behaviors to perform when a field is exited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gRemitGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            DataRecord row = gRemit.ActiveRecord;

            //If the amount_functional field was changed then update the functional amount to match if USD and
            //Recalculate the totals
            if (e.Cell.Field.Name == "amount_functional")
            {
                //If currencies match between the remit and the batch then set the bank charge and exchange rates to default values
                if (row.Cells["currency_code"].Value.ToString() == tCurrencyCode.Text)
                {
                    row.Cells["amount"].Value = e.Cell.Value;
                    row.Cells["exchange_rate"].Value = 1;
                    row.Cells["bank_charge_amount"].Value = 0;
                    CalculateAmountAllocated();
                    //Set the amount field to not be editable if the currencies are matched
                    gRemit.xGrid.FieldLayouts[0].Fields["amount"].Settings.AllowEdit = false;
                    //Verify amounts
                    if (Convert.ToDecimal(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_total"]) < Convert.ToDecimal(dtMiscInfo.Rows[0]["total_remit"])
                            || Convert.ToDecimal(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_total"]) < Convert.ToDecimal(e.Cell.Value))
                    {
                        MessageBox.Show("Remit functional amount for the line(s) must be less than the batch total.");
                    }

                    //If we have not fully allocated the remit amount then create a new alloc row
                    if (Convert.ToDecimal(dtMiscInfo.Rows[0]["total_allocated"]) < Convert.ToDecimal(dtMiscInfo.Rows[0]["remit_line_total"]))
                    {
                        gAlloc.Focus();
                        CreateNewAllocRow();
                    }
                    //If fully allocated on the current remit row but total of remits does not match the cash batch total then create new remit row
                    else if (Convert.ToDecimal(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_total"]) > Convert.ToDecimal(dtMiscInfo.Rows[0]["total_remit"]))
                    {
                        gRemit.Focus();
                        CreateNewRemitRow();
                    }
                    else //If fully allocated then return to the top of the screen
                    {
                        tBatchID.CntrlFocus();
                    }
                }
 
                else //Currencies don't match - Must enter a local currency before performing the steps to setup empty records
                {
                    gRemit.xGrid.FieldLayouts[0].Fields["amount"].Settings.AllowEdit = true;
                 }

            }
            //If a different currency code is used the exchange rate and bank charges are now calculated
            else if (e.Cell.Field.Name == "amount" && row.Cells["currency_code"].Value.ToString() != tCurrencyCode.Text)
            {
                CalculateAmountAllocated();
                CurrentBusObj.Parms.UpdateParmValue("@conversion_date", dpBatchDate.SelText.ToString() ?? string.Empty);
                CurrentBusObj.Parms.UpdateParmValue("@from_currency", tCurrencyCode.Text ?? string.Empty);
                CurrentBusObj.Parms.UpdateParmValue("@to_currency", row.Cells["currency_code"].Value.ToString() ?? string.Empty);
                //Get the exchange rate for the from and to currency for the batch date
                CurrentBusObj.LoadTable("exchange_rates");
                if (CurrentBusObj.ObjectData.Tables["exchange_rates"] != null && CurrentBusObj.ObjectData.Tables["exchange_rates"].Rows.Count > 0)
                {
                    //Convert all base data to local variables to make the code easier to read
                    decimal conversionRate = Convert.ToDecimal(CurrentBusObj.ObjectData.Tables["exchange_rates"].Rows[0]["conversion_rate"]);
                    decimal functionalAmount = Convert.ToDecimal(row.Cells["amount_functional"].Value);
                    decimal localAmount = Convert.ToDecimal(row.Cells["amount"].Value);
                    //This calculation determines the bank charge by taking the localcurrency amount and the current exchange rate
                    //to determine what amount we should have received from the bank and then subtracting what we actually received from the bank
                    //The remiander is the bank charge
                    decimal bankAmount = (localAmount / conversionRate) - functionalAmount;

                    row.Cells["exchange_rate"].Value = conversionRate;
                    row.Cells["bank_charge_amount"].Value = bankAmount ;
                }
                else
                    MessageBox.Show("Unable to find exchange rate for Conversion Date:" + dpBatchDate.SelText.ToString() ?? string.Empty + " - From Currency:" +
                                    tCurrencyCode.Text ?? string.Empty + " - To Currency:" + row.Cells["currency_code"].Value.ToString() ?? string.Empty);
            }
        }

        /// <summary>
        /// The grid preview key down event is used to alter the behavior of keyboard activity based on which cell the event was received on
        /// The purpose of this event is to intercept the behavior of the tab and enter keys on the remit grid when used in the amount or amount 
        /// functional field so that the default tab behavior will not be used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gRemitGrid_GridPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (gRemit.xGrid.ActiveCell != null)
            {
                
                if (gRemit.xGrid.ActiveCell.Field.Name == "amount_functional" && (e.Key == Key.Tab || e.Key == Key.Enter) && (Keyboard.Modifiers != ModifierKeys.Shift))
                {
                    gRemit.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

                    if (gRemit.xGrid.FieldLayouts[0].Fields["amount"].Settings.AllowEdit == false)
                    {
                        e.Handled = true;
                        //gAlloc.Focus();
                    }

                }

                //This if is not currently being used as the tab automatically goes to the amount functional field next.  The logic for jumping between grids
                //is handled above in the amount functional field
                if (gRemit.xGrid.ActiveCell.Field.Name == "amount" && (e.Key == Key.Tab || e.Key == Key.Enter) && (Keyboard.Modifiers != ModifierKeys.Shift))
                {
                    //CalculateAmountAllocated();
                    //gAlloc.Focus();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gAllocGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            DataRecord row = gAlloc.ActiveRecord;

            //If the amount_functional field was changed then update the functional amount to match if USD and
            //Recalculate the totals
            if (e.Cell.Field.Name == "amount")
            {
                CalculateAmountAllocated();


                //Verify amounts
                if (Convert.ToDecimal(dtMiscInfo.Rows[0]["remit_line_total"]) < Convert.ToDecimal(dtMiscInfo.Rows[0]["total_allocated"])
                       || Convert.ToDecimal(dtMiscInfo.Rows[0]["remit_line_total"]) < Convert.ToDecimal(gAlloc.xGrid.ActiveCell.Value))
                {
                    MessageBox.Show("Remit allocation amount(s) must be less than the amount total for the remittance.");
                }

                //If the remit is not fully allocated then create a new remit_alloc row
                if (Convert.ToDecimal(dtMiscInfo.Rows[0]["total_allocated"]) < Convert.ToDecimal(dtMiscInfo.Rows[0]["remit_line_total"]))
                {
                    gAlloc.Focus();
                    CreateNewAllocRow();
                }
                //If this remit is fully allocated but the total of all remits does not match the batch total then a new remit row will be created
                else if (Convert.ToDecimal(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_total"]) > Convert.ToDecimal(dtMiscInfo.Rows[0]["total_remit"]))
                {
                    gRemit.Focus();
                    CreateNewRemitRow();
                }
                else
                {
                    tBatchID.CntrlFocus(); 
                }

  
            }

            if (e.Cell.Field.Name == "unapplied_flag")
            {
                if (Convert.ToInt32(e.Cell.Value) == 0) //Unapplied not checked
                {
                    //Activate document id and seq field
                    gAlloc.xGrid.FieldLayouts[0].Fields["apply_to_doc"].Settings.AllowEdit = true;
                    gAlloc.xGrid.FieldLayouts[0].Fields["apply_to_seq"].Settings.AllowEdit = true;

                    //Deactivate product, company, customer and blank values
                    gAlloc.xGrid.FieldLayouts[0].Fields["product_code"].Settings.AllowEdit = false;
                    gAlloc.xGrid.FieldLayouts[0].Fields["receivable_account"].Settings.AllowEdit = false;
                    gAlloc.ActiveRecord.Cells["product_code"].Value  = string.Empty ;
                    gAlloc.ActiveRecord.Cells["receivable_account"].Value = string.Empty;
                    gAlloc.ActiveRecord.Cells["company_code"].Value = string.Empty;

                   

                }
                else //Unapplied is checked - User must enter product and customer - Amount defaults to remianing open
                {
                    //Activate company, product and customer
                    gAlloc.xGrid.FieldLayouts[0].Fields["product_code"].Settings.AllowEdit = true;
                    gAlloc.xGrid.FieldLayouts[0].Fields["receivable_account"].Settings.AllowEdit = true;

                    //deactivate document_id and seq fields
                    gAlloc.xGrid.FieldLayouts[0].Fields["apply_to_doc"].Settings.AllowEdit = false;
                    gAlloc.xGrid.FieldLayouts[0].Fields["apply_to_seq"].Settings.AllowEdit = false;
                    gAlloc.ActiveRecord.Cells["apply_to_doc"].Value = string.Empty;
                    gAlloc.ActiveRecord.Cells["apply_to_seq"].Value = 0;

                    //Autocalc the amount remaining and default to amount field
                    gAlloc.ActiveRecord.Cells["amount"].Value = Convert.ToDecimal(dtMiscInfo.Rows[0]["remit_line_total"]) - Convert.ToDecimal(dtMiscInfo.Rows[0]["total_allocated"]);
                    //If less than zero then set to zero
                    if (Convert.ToDecimal(gAlloc.ActiveRecord.Cells["amount"].Value) < 0)
                        gAlloc.ActiveRecord.Cells["amount"].Value = 0;
                }
            }

            //This code runs a query to verify that the document id is valid when attempting to leave the apply to doc field
            if (e.Cell.Field.Name == "apply_to_doc")
            {
                CurrentBusObj.Parms.UpdateParmValue("@doc_id_lookup",e.Cell.Value.ToString() ?? string.Empty );
                CurrentBusObj.Parms.UpdateParmValue("@to_currency", gRemit.ActiveRecord.Cells["currency_code"].Value.ToString() ?? string.Empty);
                CurrentBusObj.LoadTable("open_cash");
                if (CurrentBusObj.ObjectData.Tables["open_cash"] != null && CurrentBusObj.ObjectData.Tables["open_cash"].Rows.Count > 0)
                {
                    //@@Need to complete this code
                    if (CurrentBusObj.ObjectData.Tables["open_cash"].Rows.Count > 1) //More than one row that can be applied to
                    {
                        gAlloc.xGrid.FieldLayouts[0].Fields["apply_to_seq"].Settings.AllowEdit = true;
                    }
                    else  //Single apply to row
                    {
                        gAlloc.xGrid.FieldLayouts[0].Fields["apply_to_seq"].Settings.AllowEdit = false;
                        gAlloc.ActiveRecord.Cells["apply_to_seq"].Value = CurrentBusObj.ObjectData.Tables["open_cash"].Rows[0]["seq_code"];
                        gAlloc.ActiveRecord.Cells["product_code"].Value = CurrentBusObj.ObjectData.Tables["open_cash"].Rows[0]["product_code"];
                        gAlloc.ActiveRecord.Cells["company_code"].Value = CurrentBusObj.ObjectData.Tables["open_cash"].Rows[0]["company_code"];
                        gAlloc.ActiveRecord.Cells["receivable_account"].Value = CurrentBusObj.ObjectData.Tables["open_cash"].Rows[0]["receivable_account"];
                        gAlloc.ActiveRecord.Cells["amount"].Value = CurrentBusObj.ObjectData.Tables["open_cash"].Rows[0]["open_amount"];
                        gAlloc.ActiveRecord.Cells["currency_code"].Value = CurrentBusObj.ObjectData.Tables["open_cash"].Rows[0]["currency_code"]; ;
                    }
                }
                else  //No data returned
                {
                    MessageBox.Show("Document: " + gAlloc.ActiveRecord.Cells["apply_to_doc"].Value.ToString() + " does not exist or has no open cash.  Please select another document.");
                    e.Handled = true;
                    return;
                }
            }

            //This query verifies that the receivable account selected is valid when the field is exited
            if (e.Cell.Field.Name == "receivable_account")
            {
                CurrentBusObj.Parms.UpdateParmValue("@receivable_account_lookup",e.Cell.Value.ToString() ?? string.Empty );
                CurrentBusObj.LoadTable("customer");
                if (CurrentBusObj.ObjectData.Tables["customer"] != null && CurrentBusObj.ObjectData.Tables["customer"].Rows.Count > 0)
                {
                    //Nothing to do here for now.  May need to return the customer name to the window in the future.
                }
                else //Customer not found
                {
                    MessageBox.Show("Invalid customer ID.  Please choose a valid customer.");
                    e.Cell.Value = "";
                }
            }

            //This field updates all remit alloc values based on which apply to seq is selected
            if (e.Cell.Field.Name == "apply_to_seq")
            {
            
                EnumerableRowCollection<DataRow> DocRow = from OpenCash in CurrentBusObj.ObjectData.Tables["open_cash"].AsEnumerable()
                                                         where OpenCash.Field<Int32>("seq_code") == Convert.ToInt32(e.Cell.Value) 
                                                         select OpenCash;

                foreach (DataRow r in DocRow)
                {
                    //gAlloc.ActiveRecord.Cells["apply_to_seq"].Value = r["seq_code"];
                    gAlloc.ActiveRecord.Cells["product_code"].Value = r["product_code"];
                    gAlloc.ActiveRecord.Cells["company_code"].Value = r["company_code"];
                    gAlloc.ActiveRecord.Cells["receivable_account"].Value = r["receivable_account"];
                    gAlloc.ActiveRecord.Cells["amount"].Value = r["open_amount"];
                    gAlloc.ActiveRecord.Cells["currency_code"].Value = r["currency_code"]; ;

                }
               
            }


        }


        /// <summary>
        /// The grid preview key down event is used to alter the behavior of keyboard activity based on which cell the event was received on
        /// The purpose of this event is to intercept the behavior of the tab and enter keys on the remit_alloc grid when used in the amount  
        /// field so that the default tab behavior will not be used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gAllocGrid_GridPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (gAlloc.xGrid.ActiveCell != null)
            {
                if (gAlloc.xGrid.ActiveCell.Field.Name == "amount" && (e.Key == Key.Tab || e.Key == Key.Enter) && (Keyboard.Modifiers != ModifierKeys.Shift))
                {
                    gAlloc.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    e.Handled = true;
                }
            }

        }

        /// <summary>
        /// Method checks to see if the remit or remit_alloc grids have empty records
        /// The method can be told to delete empty records
        /// </summary>
        /// <param name="gridToCheck">The ucBaseGrid to check for empty records</param>
        /// <param name="deleteEmpty">True if the empty records should be deleted</param>
        /// <returns></returns>
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
                    if (Convert.ToDecimal(r.Cells["amount"].Value) == 0)
                    {
                        //Record the record index
                        IsEmpty = r.Index  ;
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
                if (deleteEmpty && IsEmpty !=-1)
                {
                    foreach (DataRecord r in rDelete)
                    {
                        DataRow row = (r.DataItem as DataRowView).Row;
                        if (row != null)
                        {
                            row.Delete();
                        }
                    }

                }
 
            }
            return IsEmpty;
        }
   
    }
}
