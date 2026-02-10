



using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;



namespace Contract
{

  
    /// <summary>
    /// This class represents a 'ContractLocationsTab' object.
    /// </summary>
    public partial class ContractLocationsTab : ScreenBase
    {
        private string CurrentAccount = "";
        /// <summary>
        /// Create a new instance of a 'ContractLocationsTab' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractLocationsTab()
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
            MainTableName = "location";

            //Contract Location Grid
            gLocations.MainTableName = "location";
            gLocations.ConfigFileName = "ContractLocationGrid";
            gLocations.SetGridSelectionBehavior(false, true);
          
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                gLocations.ContextMenuGenericIsVisible1 = false;
                gLocations.ContextMenuAddIsVisible = false;
                gLocations.ContextMenuRemoveIsVisible = false;
            }
            else
            {
                gLocations.ContextMenuGenericIsVisible1 = true;
                gLocations.ContextMenuAddIsVisible = true;
                gLocations.ContextMenuRemoveIsVisible = true;
            }
            gLocations.ContextMenuAddDelegate = LocationsGridAddDelegate;
            gLocations.ContextMenuAddDisplayName = "Add Location";
            
            gLocations.ContextMenuGenericDelegate1 = LocationsGridMoveLocationDelegate;
            gLocations.ContextMenuGenericDisplayName1 = "Move Location";
            
            gLocations.ContextMenuRemoveDelegate = LocationsGridRemoveDelegate;
            gLocations.ContextMenuRemoveDisplayName = "Remove Location";
            
            gLocations.FieldLayoutResourceString = "ContractLocation";
            //uses the move icon 
            gLocations.ContextMenuGenericImageSwap1 = Icons.Toggle;
            gLocations.mWindowZoomDelegate = LocationZoomDelegate;
            gLocations.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = {"cs_id", "product_code" }, ChildGrids = { gHistory }, ParentFilterOnColumnNames = { "cs_id", "product_code" } });

            gHistory.MainTableName = "locationhistory";
            gHistory.ConfigFileName = "ContractLoactionHistory";
            gHistory.SetGridSelectionBehavior(false, true);
            gHistory.FieldLayoutResourceString = "ContractLocationHistory";

            GridCollection.Add(gLocations);
            GridCollection.Add(gHistory);
            
           
        }

        public void LocationZoomDelegate()
        {
            gLocations.ReturnSelectedData("cs_id");
            cGlobals.ReturnParms.Add("btnLocation");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gLocations.xGrid ;
            EventAggregator.GeneratedClickHandler(this, args); 
        }


        /// <summary>
        /// Locations grid add delegate, calls Location Service Lookup to add new locations to contract
        /// </summary>
        private void LocationsGridAddDelegate()
        {
            //clear parms
            cGlobals.ReturnParms.Clear();
            // gets the users response
            callLocationLookupServiceScreen(this.CurrentBusObj);
            
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
            CurrentBusObj.LoadTable("location_rules");
            //gRuleDetail.LoadGrid(CurrentBusObj, "location_rules");
        }

        /// <summary>
        /// Do everything necessary to show the prod rules screen
        /// </summary>
        /// <param name="RulesObj"></param>
        private void callLocationLookupServiceScreen(cBaseBusObject RulesObj)
        {
            //instance location service screen
            ContractLocationServiceLookup LocationSvcScreen = new ContractLocationServiceLookup(getContractId(), this.CurrentBusObj);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window LocationSvcWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            LocationSvcWindow.Title = "Location Service Screen";
            LocationSvcWindow.MaxHeight = 1280;
            LocationSvcWindow.MaxWidth = 1024;
            /////////////////////////////////////////////////////
            //set rules screen as content of new window
            LocationSvcWindow.Content = LocationSvcScreen;
            //open new window with embedded user control
            LocationSvcWindow.ShowDialog();
        }

        public void LocationsGridRemoveDelegate()
        {
            MessageBoxResult result = Messages.ShowYesNo("Location will be deleted from this contract. Once deleted, to make the changes to the database, you will need to do a Save. Are you sure you want to delete?",
                    System.Windows.MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataRecord r = gLocations.ActiveRecord;
                if (r != null)
                {
                    DataRow row = (r.DataItem as DataRowView).Row;
                    if (row != null)
                    {
                        if (Convert.ToInt32(row["already_billed_flag"]) == 1)
                            MessageBox.Show("Cannot delete a location that has already billed for this contract.");
                        else
                        {
                            //need to set turn off and hold flags to 0
                            chkHoldFlag.IsChecked = 0;
                            chkTurnOffFlag.IsChecked = 0;
                            row.Delete();
                            // Clear the parms
                            cGlobals.ReturnParms.Clear();
                        }
                    }
                
                    //stop here for debug
                    string sdebug = "";
                    sdebug = "1";
                }
            }
           
        }


        private void LocationsGridMoveLocationDelegate()
        {
            //instance location service screen
            ContractLocationMove LocationMoveScreen = new ContractLocationMove(getContractId(), this.CurrentBusObj);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window LocationMoveWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            LocationMoveWindow.Title = "Location Move Screen";
            LocationMoveWindow.MaxHeight = 1280;
            LocationMoveWindow.MaxWidth = 1280;
            /////////////////////////////////////////////////////
            //set screen as content of new window
            LocationMoveWindow.Content = LocationMoveScreen;
            //open new window with embedded user control
            LocationMoveWindow.ShowDialog();
        }

        /// <summary>
        /// event to handle Hold Flag when unchecked
        /// DWR Modified 4/19/13 - Fixed problem with dates being updated improperly. For more info see comment in chkTurnOffFlag_Checked event for 4/19/13
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkHoldFlag_UnChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (gLocations.ActiveRecord != null)
                if (Convert.ToDateTime(gLocations.ActiveRecord.Cells["hold_date"].Value).Date != Convert.ToDateTime("1/1/1900").Date)
                    gLocations.ActiveRecord.Cells["hold_date"].Value = Convert.ToDateTime("01/01/1900");
        }

        /// <summary>
        /// event to handle Hold Flag when checked
        /// DWR Modified 4/19/13 - Fixed problem with dates being updated improperly. For more info see comment in chkTurnOffFlag_Checked event for 4/19/13
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkHoldFlag_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (gLocations.ActiveRecord != null)
                if (Convert.ToDateTime(gLocations.ActiveRecord.Cells["hold_date"].Value).Date == Convert.ToDateTime("1/1/1900").Date)
                    gLocations.ActiveRecord.Cells["hold_date"].Value = DateTime.Now;
        }
        
        /// <summary>
        /// get Contract from parms in busObj
        /// </summary>
        /// <returns></returns>
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

        private void chkTurnOffFlag_Checked(object sender, RoutedEventArgs e)
        {
            //DWR - 4/19/13 - Modified event to change information on the grid instead of the freeform as the data being updated on the
            //freeform was being applied to the incorrect row.  This was due to the checked flag being processed before the dates on a record change.  
            //The turn off flag would process for the new record but the date bindings hadn't been processed and were still attached to the old record.
            if (gLocations.ActiveRecord != null)
            {
                if (Convert.ToBoolean(chkTurnOffFlag.IsChecked))
                {
                    //txtTurnOffDate.IsReadOnly = false;
                    if (Convert.ToDateTime(gLocations.ActiveRecord.Cells["termination_date"].Value).Date == Convert.ToDateTime("1/1/1900").Date)
                        //txtTurnOffDate.SelText = DateTime.Now;
                        gLocations.ActiveRecord.Cells["termination_date"].Value = DateTime.Now;

                    if (Convert.ToDateTime(gLocations.ActiveRecord.Cells["termination_acct_date"].Value).Date == Convert.ToDateTime("1/1/1900").Date)
                    {
                        //txtTurnOffAcctDate.SelText = DateTime.Now;
                        if (this.CurrentBusObj.ObjectData.Tables["date_type"] != null && this.CurrentBusObj.ObjectData.Tables["date_type"].Rows != null)
                        {
                            foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["date_type"].Rows)
                            {
                                if (r["date_type"].ToString() == "ACCTPERIOD")
                                {
                                    //txtTurnOffAcctDate.SelText = Convert.ToDateTime(r["date_value"]);
                                    gLocations.ActiveRecord.Cells["termination_acct_date"].Value = Convert.ToDateTime(r["date_value"]);
                                    break;
                                }
                            }

                        }
                    }
                }
                else if (!(Convert.ToBoolean(chkTurnOffFlag.IsChecked)))
                {
                    //txtTurnOffDate.IsReadOnly = true;
                    //txtTurnOffDate.SelText = Convert.ToDateTime("1/1/1900");
                    //txtTurnOffAcctDate.SelText = Convert.ToDateTime("1/1/1900");
                    if (Convert.ToDateTime(gLocations.ActiveRecord.Cells["termination_acct_date"].Value).Date != Convert.ToDateTime("1/1/1900").Date)
                        gLocations.ActiveRecord.Cells["termination_acct_date"].Value = "1/1/1900";

                    if (Convert.ToDateTime(gLocations.ActiveRecord.Cells["termination_date"].Value).Date != Convert.ToDateTime("1/1/1900").Date)
                        gLocations.ActiveRecord.Cells["termination_date"].Value = "1/1/1900";
                }
            }
        }

        /// <summary>
        /// Event that verifies the customer name is correct based on the customer number and that the customer number is correct
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCustomerID_LostFocus(object sender, RoutedEventArgs e)
        {
            CurrentBusObj.Parms.UpdateParmValue("@receivable_account_lookup", txtCustomerID.Text ?? string.Empty);
            CurrentBusObj.LoadTable("customer");
            if (CurrentBusObj.ObjectData.Tables["customer"] != null && CurrentBusObj.ObjectData.Tables["customer"].Rows.Count > 0)
            {
                //Nothing to do here for now.  May need to return the customer name to the window in the future.
                txtCustomerName.Text = CurrentBusObj.ObjectData.Tables["customer"].Rows[0]["account_name"].ToString();
            }
            else //Customer not found
            {
                MessageBox.Show("Invalid customer ID.  Please choose a valid customer.");
                txtCustomerName.Text = "INVALID CUSTOMER";
                
                //txtCustomerID.CntrlFocus(); 
                //e.Cell.Value = "";
            }
        }

        /// <summary>
        /// Event to allow lookup of customer
        /// ***DWR-Added 5/25/12
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCustomerID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
    

            //call customer lookup
            RazerBase.Lookups.CustomerLookup customerLookup = new RazerBase.Lookups.CustomerLookup();
            customerLookup.Init(new cBaseBusObject("CustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();

            // gets the users response
            customerLookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //load current parms
                //loadParms("");
                txtCustomerID.Text = cGlobals.ReturnParms[0].ToString();
                txtCustomerName.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }

        }

        private void txtCustomerID_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            
        }


    }


}
