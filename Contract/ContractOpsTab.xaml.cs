



using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Infragistics.Windows.DataPresenter;




namespace Contract
{

  
    /// <summary>
    /// This class represents a 'ContractLocationsTab' object.
    /// </summary>
    public partial class ContractOpsTab : ScreenBase
    {
        string StartDate;
        string EndDate;
        /// <summary>
        /// Create a new instance of a 'ContractLocationsTab' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractOpsTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "ops";

            //Contract Location Grid
            gOps.MainTableName = "ops";
          
            gOps.SetGridSelectionBehavior(false, false);
            gOps.ContextMenuAddDelegate = LocationsGridAddDelegate;
            gOps.ContextMenuAddDisplayName = "Add Billing Location";
            
            gOps.ContextMenuRemoveIsVisible = false;
            gOps.ContextMenuGenericDisplayName1 = "Unpost Ops Data";
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                gOps.ContextMenuGenericIsVisible1 = false;
                gOps.ContextMenuAddIsVisible = false;
                gOps.ContextMenuGenericIsVisible2 = false;
                gOps.ContextMenuGenericIsVisible3 = false;
                gOps.ContextMenuGenericIsVisible4 = false;
            }
            else
            {
                gOps.ContextMenuGenericIsVisible1 = true;
                gOps.ContextMenuAddIsVisible = true;
                gOps.ContextMenuGenericIsVisible2 = true;
                gOps.ContextMenuGenericIsVisible3 = true;
                gOps.ContextMenuGenericIsVisible4 = true;
            }
            gOps.ContextMenuGenericDelegate1 = UnpostOpsDataDelegate;
            gOps.ContextMenuGenericDisplayName2 = "Post Ops Data";
            
            gOps.ContextMenuGenericDelegate2 = PostOpsDataDelegate;

            /* RES 1/24/18 add option to menu to check/uncheck never bill for all rows displayed  */
            gOps.ContextMenuGenericDisplayName3 = "Check Never Bill for all rows";
            gOps.ContextMenuGenericDelegate3 = CheckNeverBillDelegate;
            gOps.ContextMenuGenericDisplayName4 = "UnCheck Never Bill for all rows";
            gOps.ContextMenuGenericDelegate4 = UnCheckNeverBillDelegate;

            gOps.ConfigFileName = "ContractOps";
            gOps.FieldLayoutResourceString = "ContractOps";
            //gOps.mWindowZoomDelegate = LocationZoomDelegate;

            //Add this code to use a different style for the grid than the default.
            //This particular style is used for color coded grids.
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            gOps.GridCellValuePresenterStyle = CellStyle;
            gOps.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;


            GridCollection.Add(gOps);

            //DWR - Added 8/28/12
            //Check Security for the tab and hide the run prebill button as appropriate.
            //@@If more granular security than the tab is needed in the future a specific security context can be tied to the robject
            //@@that is used to run the prebill
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
                btnPrebill.Visibility = Visibility.Hidden;
            else
                btnPrebill.Visibility = Visibility.Visible;

            ClearDates();
            //txtServiceDateStart.SelText = null;
            //txtServiceDateEnd.SelText = null;
        }

        public void LocationZoomDelegate()
        {
            gOps.ReturnSelectedData("contract_id");
            cGlobals.ReturnParms.Add("btnBillLocation");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gOps.xGrid ;
            EventAggregator.GeneratedClickHandler(this, args); 
        }


        /// <summary>
        /// Locations grid add delegate, calls Location Service Lookup to add new locations to contract
        /// </summary>
        private void LocationsGridAddDelegate()
        {
            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            callAddBillLocationScreen(this.CurrentBusObj);

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //load current parms
                //loadParms("");
                //txtReceivableAccount.Text = cGlobals.ReturnParms[0].ToString();
                //txtCustomerName.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        /// <summary>
        /// Do everything necessary to show the prod rules screen
        /// </summary>
        /// <param name="RulesObj"></param>
        private void callAddBillLocationScreen(cBaseBusObject RulesObj)
        {
            //instance location service screen
            ContractOpsAdd ContractOpsScreen = new ContractOpsAdd(getContractId(), this.CurrentBusObj);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window ContractOpsWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            ContractOpsWindow.Title = "Add Billing Location";
            ContractOpsWindow.MaxHeight = 1280;
            ContractOpsWindow.MaxWidth = 1024;
            /////////////////////////////////////////////////////
            //set rules screen as content of new window
            ContractOpsWindow.Content = ContractOpsScreen;
            //open new window with embedded user control
            ContractOpsWindow.ShowDialog();
        }

        private void UnpostOpsDataDelegate()
        {
            //opens ContractOpsUnpostData Window
            ContractOpsUnpostData ContractUnpostScreen = new ContractOpsUnpostData(getContractId(), this.CurrentBusObj);
            //////////////////////////////////////////////////////////////
            //create a new window , show it as a dialog
            System.Windows.Window ContractUnpostWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            ContractUnpostWindow.Title = "Contract Unpost Ops Data ";
            ContractUnpostWindow.MaxHeight = 1880;
            ContractUnpostWindow.MaxWidth = 2280;
            /////////////////////////////////////////////////////
            //set screen as content of new window
            ContractUnpostWindow.Content = ContractUnpostScreen;
            //open new window with embedded user control
            ContractUnpostWindow.ShowDialog();
        }

        private void PostOpsDataDelegate()
        {
            //opens instance of ContractPostOpsData screen
            ContractPostOpsData PostOpsScreen = new ContractPostOpsData(getContractId(), this.CurrentBusObj);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window PostOpsWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            PostOpsWindow.Title = "Post Ops Data";
            PostOpsWindow.MaxHeight = 1880;
            PostOpsWindow.MaxWidth = 2280;
            /////////////////////////////////////////////////////
            //set screen as content of new window
            PostOpsWindow.Content = PostOpsScreen;
            //open new window with embedded user control
            PostOpsWindow.ShowDialog();
        }

        private void CheckNeverBillDelegate()
        {
            var result = MessageBox.Show("You have chosen to check Never Bill for ops data between " + StartDate + " and " + EndDate + ".  Do you want to continue?", "Check", 
                    MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            //Loop through and select all rows in the grid
            foreach (DataRecord r in gOps.xGrid.Records)
            {

                if (r.Cells["never_bill_flag"].Value.ToString() == "0")
                    r.Cells["never_bill_flag"].Value = 1;

                gOps.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
            }
        }

        private void UnCheckNeverBillDelegate()
        {
            var result = MessageBox.Show("You have chosen to uncheck Never Bill for ops data between " + StartDate + " and " + EndDate + ".  Do you want to continue?", "UnCheck",
                    MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            //Loop through and unselect all rows in the grid
            foreach (DataRecord r in gOps.xGrid.Records)
            {

                if (r.Cells["never_bill_flag"].Value.ToString() == "1")
                    r.Cells["never_bill_flag"].Value = 0;

                if (r.Cells["user_override_flag"].Value.ToString() == "1")
                    r.Cells["user_override_flag"].Value = 0;

                gOps.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
            }
        }
        
        private void LocationsGridMoveLocationDelegate()
        {
            
        }

        /// <summary>
        /// event to handle Hold Flag when unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      
        private int getContractId()
        {
                var localContractId = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                            where x.Field<string>("parmName") == "@contract_id"
                            select x.Field<string>("parmValue");

                foreach (var id in localContractId)
                {
                    int ContractId = Convert.ToInt32(id);
                    //return contract id
                    return ContractId;
                }
                return 0;
        }

        /// <summary>
        /// Method to run prebill from contract ops tab prebill button.
        /// DWR - Added 8/28/12 for Phase 2 billing enhancements
        /// Use this method as an example of how to use an robject to run a procedure without using a web service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunPrebill(object sender, RoutedEventArgs e)
        {
            
            try
            {
                //Create a new base business object to run the prebill from.
                //The prebill is run from the usp_batch_prebill procedure when has been added to robjects
                //The new base business object is created / parameters added and then a load data command is issued
                //which will launch the prebill procedure.  There is no return data in the base business object so 
                //it is local to this method.
                cBaseBusObject PrebillObject = new cBaseBusObject();
                string LogEntries = "";
                PrebillObject.BusObjectName = "PrebillProcess";
                PrebillObject.Parms.AddParm("@prebill_type", 1);
                PrebillObject.Parms.AddParm("@prebill_parm", getContractId());
                PrebillObject.Parms.AddParm("@log_entries", LogEntries);
                PrebillObject.LoadData();
            }
            finally 
            {
            }
            //Refresh the screen Data - Which will show any changes to the ops data.
            this.findRootScreenBase(this).Refresh();

            MessageBox.Show("Prebill Process Complete.");

        }

     

        /// <summary>
        /// Retrieve Button pressed - retrieves data to populate grid based on search criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRetrieve_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //KSH - 8/29/12 keep button click from blowing up app when no rec selected
            if (this.CurrentBusObj == null)
                return;

            //if (IsScreenDirty == true)
            ////if (this.CurrentBusObj.DataChanged())
            //{
            //    var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
            //    if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
            //    {
            //        this.Save();
            //        if (SaveSuccessful == false)
            //            return;
            //        //StopCloseIfCancelCloseOnSaveConfirmationTrue = true;
            //    }
            //}

            
            //If no start input then select all previous
            if (txtServiceDateStart.SelText.ToString() == "" || txtServiceDateStart.SelText == null || txtServiceDateStart.SelText.ToString() == "1/1/1900 12:00:00 AM")
            {
                this.CurrentBusObj.changeParm("@ops_period_start", "1/1/1900");
                StartDate = "1/1/1900";
            }
            else
            {
                this.CurrentBusObj.changeParm("@ops_period_start", txtServiceDateStart.SelText.ToString());
                int BlankStart = txtServiceDateStart.SelText.ToString().IndexOf(" ");
                StartDate = txtServiceDateStart.SelText.ToString().Substring(0, BlankStart);
            }

            //if no end date then select all future dates
            if (txtServiceDateEnd.SelText.ToString() == "1/1/1900 12:00:00 AM" || txtServiceDateEnd.SelText == null)
            {
                this.CurrentBusObj.changeParm("@ops_period_end", "12/31/2100");
                EndDate = "12/31/2100";
            }
            else
            {
                this.CurrentBusObj.changeParm("@ops_period_end", txtServiceDateEnd.SelText.ToString());
                int BlankEnd = txtServiceDateEnd.SelText.ToString().IndexOf(" ");
                EndDate = txtServiceDateEnd.SelText.ToString().Substring(0, BlankEnd);
            }

            //this.CurrentBusObj.LoadTable("ops");
            this.CurrentBusObj.LoadData("ops");
            //this.findRootScreenBase(this).Refresh();
            //approvalBusinessObject.LoadData();
            //idgAdjustments.LoadGrid(approvalBusinessObject, mainTableName);
            if (CurrentBusObj.ObjectData.Tables["ops"].Rows.Count == 0)
            {
                Messages.ShowWarning("No Ops for Dates Specified");
            }
            else
            {
                //gOps.LoadGrid(this.CurrentBusObj, MainTableName);
            }
        }

        public void ClearDates()
        {
            txtServiceDateStart.SelText = null;
            txtServiceDateEnd.SelText = null;
        }


    }


}
