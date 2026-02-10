

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Windows;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Windows.Forms;

#endregion

namespace Contract
{

  
    /// <summary>
    /// This class represents a 'ContractRatesTab' object.
    /// </summary>
    public partial class ContractRatesTab : ScreenBase, IPreBindable
    {
        //RES 11/18/19 Flag to indicate if a cumulative rate exists
        //public bool CUMFlag = true;
        //setting to true keeps the single click delegate from firing when context menu add record is selected
        public bool ContextAddClicked { get; set; }
        public ContractFolderMainScreen ContractMainScreen { get; set; }
        public ContractGeneralTab ContractGeneral { get; set; }
        //for rates status flag
        public ComboBoxItemsProvider cmbRatesStatusFlagCombo { get; set; }
        //RES 3/29/16 for rules clm_id
        //public ComboBoxItemsProvider cmbRulesCLMNumber { get; set; }
        //Needed for a combobox
        //for rules group grid
        public ComboBoxItemsProvider cmbRulesGroupCombo { get; set; }
        public int EndDateRateID = 0;
        //RES 4/17/15 added to keep inactive flag
        public int ShowInactive;
        //cBaseBusObject obj = null;
        /// <summary>
        /// Create a new instance of a 'ContractRatesTab' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractRatesTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Perform initializations for this object
            Init();
        }

        internal ContractFolderMainScreen MainScreen { get; set; }
      
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            ContextAddClicked = false;
            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "rates";
            //Rates Grid
            gRates.xGrid.FieldLayoutSettings.AllowAddNew = true;
            gRates.xGrid.FieldSettings.AllowEdit = true;
            gRates.MainTableName = "rates";
            gRates.ConfigFileName = "ContractProductRatesGrid";
            gRates.SetGridSelectionBehavior(false, false);
            gRates.ContextMenuAddIsVisible = false;
            gRates.ContextMenuRemoveIsVisible = false;
            gRates.ContextMenuAddDelegate = RatesGridAddDelegate;
            gRates.ContextMenuGenericDelegate1 = RatesGridCopyDelegate;
            gRates.ContextMenuGenericDisplayName1 = "Copy Rate/Rules";
            gRates.EditModeStartedDelegate = RatesGridEditModeDelegate;
            gRates.ContextMenuGenericIsVisible1 = true;
            //TODO: Take out RecordAdding event no longer in use
            //gRates.RecordAddingEventDelegate = RatesGridRecordAddingEvent;
            gRates.FieldLayoutResourceString = "ContractProductRates";
            gRates.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(gRates_EditModeEnded);
            //gRates.xGrid.SelectedItemsChanged += new EventHandler<Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs>(gRates_SelectedItemsChanged);

            //RES 4/16/15 highlight inactive rates in red
            //Add this code to use a different style for the grid than the default.
            //This particular style is used for color coded grids.
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            gRates.GridCellValuePresenterStyle = CellStyle;
            gRates.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;

            ////This grid is editable and has a dropdown within the grid
            ////A field layoutsettings file will establish that the grid can be added to and deleted from
            //FieldLayoutSettings f = new FieldLayoutSettings();
            //f.AllowAddNew = true;
            //f.AllowDelete = true;
            //f.AddNewRecordLocation = AddNewRecordLocation.OnBottom;
            //gCompany.xGrid.FieldLayoutSettings = f;
            ////Make the grid editable
            //gCompany.xGrid.FieldSettings.AllowEdit = true;

            //Rules Grid
            gRules.MainTableName = "rules";
            gRules.ConfigFileName = "ContractProductRulesGrid";
            gRules.SetGridSelectionBehavior(true, false);
            gRules.ContextMenuAddDelegate = RulesGridAddDelegate;
            gRules.WindowZoomDelegate = RulesGridDoubleClickDelegate;
            gRules.ContextMenuAddDisplayName = "Add Rule";
            gRules.ContextMenuRemoveDisplayName = "Delete Rule";
            gRules.FieldLayoutResourceString = "ContractProductRules";
            gRates.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "rate_id" }, ChildGrids = { gRules }, ParentFilterOnColumnNames = { "rate_id" } });
            gRules.RecordActivatedDelegate = RulesGridRecordActivatedDelegate;
            //RES 4/16/15 highlight inactive rates in red
            //Add this code to use a different style for the grid than the default.
            //This particular style is used for color coded grids.
            gRules.GridCellValuePresenterStyle = CellStyle;
            gRules.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;

            //Rules Detail Grid
            gRuleDetail.MainTableName = "rule_detail";
            gRuleDetail.ConfigFileName = "ContractRuleDetailGrid";
            gRuleDetail.SetGridSelectionBehavior(true, false);
            gRuleDetail.ContextMenuAddIsVisible = false;
            gRuleDetail.ContextMenuRemoveIsVisible = false;
            gRuleDetail.WindowZoomDelegate = RulesGridDoubleClickDelegate;
            gRuleDetail.FieldLayoutResourceString = "ContractRuleDetail";
            gRules.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "rate_id", "rule_id" }, ChildGrids = { gRuleDetail }, ParentFilterOnColumnNames = { "rate_id", "rule_id" } });

            GridCollection.Add(gRates);
            GridCollection.Add(gRules);
            GridCollection.Add(gRuleDetail);

            //RES 4/16/15 default to don't show inactives
            //chkInactive.IsChecked = true;
            //chkInactive.IsChecked = false;
            //this.CurrentBusObj.Parms.UpdateParmValue("@ShowInactives", "0");
            //this.CurrentBusObj.LoadTable("rates");
            //gRates.LoadGrid(CurrentBusObj, "rates");

            //Setup base parameters - Show inactive
            //CurrentBusObj.Parms.AddParm("@show_inactive", "0");
            //Load the object data
            //Load(CurrentBusObj);
       }

        //This kind of works but was implemented contractmainscreen and was unreliable
        //public void Rates_NewRow(object sender, DataTableNewRowEventArgs e)
        //{
        //    //DataRow dr = this.CurrentBusObj.ObjectData.Tables["rates"].NewRow();
        //    e.Row["contract_id"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[0].ToString();
        //    e.Row["rate_id"] = 0;
        //    e.Row["contract_entity_id"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[1].ToString();
        //    e.Row["contract_entity"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[44].ToString();
        //    e.Row["contract_description"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[4].ToString();
        //    //this.CurrentBusObj.ObjectData.Tables["rates"].Rows.Add(dr);
        //}

        //TODO: Take out no longer in use
        //public void RatesGridRecordAddingEvent()
        //{
        //    DataRow dr = this.CurrentBusObj.ObjectData.Tables["rates"].NewRow();
        //    dr["contract_id"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[0].ToString();
        //    dr["rate_id"] = 0;
        //    dr["contract_entity_id"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[1].ToString();
        //    dr["contract_entity"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[44].ToString();
        //    dr["contract_description"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[4].ToString();
        //    this.CurrentBusObj.ObjectData.Tables["rates"].Rows.Add(dr);
        //}

        private void RatesGridAddDelegate()
        {
            DataRow dr = this.CurrentBusObj.ObjectData.Tables["rates"].NewRow();
            //setRatesDefaults();
            //dr["contract_id"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[0].ToString();
            //dr["rate_id"] = 0;
            //dr["contract_entity_id"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[1].ToString();
            //dr["contract_entity"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[44].ToString();
            //dr["contract_description"] = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0].ItemArray[4].ToString();
            this.CurrentBusObj.ObjectData.Tables["rates"].Rows.Add(dr);
        }

        /// <summary>
        /// Double Click on Rules Grid
        /// pull up and populate product rules screen based off of rate and rules grid selections
        /// </summary>
        /// 
        private void RatesGridCopyDelegate()
        {
            //instance location service screen
            ContractRatesCopy RatesCopyScreen = new ContractRatesCopy(getContractId(), this.CurrentBusObj);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window RatesCopyWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            RatesCopyWindow.Title = "Rate/Rules Copy Screen";
            RatesCopyWindow.MaxHeight = 1280;
            RatesCopyWindow.MaxWidth = 1280;
            /////////////////////////////////////////////////////
            //set screen as content of new window
            RatesCopyWindow.Content = RatesCopyScreen;
            //open new window with embedded user control
            RatesCopyWindow.ShowDialog();
            this.findRootScreenBase(this).Refresh();
        }

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

        private void RulesGridDoubleClickDelegate()
        {
            bool rateNotSaved = false;
            ContextAddClicked = false;
            //create bus obj for rules screen
            cBaseBusObject RulesObj = new cBaseBusObject();
           
            //add rate id and rule id to bus obj parm table
            bool checkBool = getRatesAndRules(ref RulesObj, ref rateNotSaved);
            if (RulesObj != null)
            {
                //show the product rules screen
                callRulesScreen(RulesObj);
                ContractFolderMainScreen c ;
                c = findRootScreenBase(this) as ContractFolderMainScreen;
                c.chkCOLA();
            }
            else
            {
                Messages.ShowInformation("Problem Opening Product Rules Screen");
            }

        }

        /// <summary>
        /// Right Click and Add Record on Rules Grid
        /// </summary>
        private void RulesGridAddDelegate()
        {
            string OpSys = cGlobals.getOSInfo();
            if (OpSys == "XP")
            {
                //if OpSys is Windows XP do try/catch to ignore XP bug
                try
                {
                    doRulesAddDelegate();
                }
                catch (Exception ex)
                {
                    //ignore
                }
            }
            else
            {
                //if not Windows XP then do normal error handling
                doRulesAddDelegate();
            }

        }

        private void doRulesAddDelegate()
        {
            bool rateNotSaved = false;
            bool checkBool = false;
            //add a new rule 
            ContextAddClicked = true;
            //create bus obj for rules screen
            cBaseBusObject RulesObj = new cBaseBusObject();
            //add rate id and rule id to bus obj parm table
            checkBool = getRatesAndRules(ref RulesObj, ref rateNotSaved);
            if (checkBool == true)
            {
                refactorRulesScreenCall(RulesObj);
            }
            else
            {
                if (rateNotSaved)
                {
                    //prompt user to save rule
                    Messages.ShowWarning("Rate Must be Saved before a Rule can be Assigned");
                    //TODO: Come back to this when more time, autosaves and then goes to prod rules screen -- can't call mainscreen save because it is an override
                    //      Will have to copy it all or do something totally different
                    //MessageBoxResult res = Messages.ShowYesNo("Rate Must be Saved before a Rule can be Assigned", MessageBoxImage.Stop);
                    //if (res == MessageBoxResult.Yes)
                    //{
                    //    //save rule
                    //    Save();
                    //    if (SaveSuccessful)
                    //    {
                    //        this.CurrentBusObj.LoadTable("rates");
                    //        //run this again to open prod rules screen now that rate is saved
                    //        cBaseBusObject RulesObjRedux = new cBaseBusObject();
                    //        rateNotSaved = false;
                    //        checkBool = getRatesAndRules(ref RulesObj, ref rateNotSaved);
                    //        refactorRulesScreenCall(RulesObjRedux);
                    //    }
                    //}
                }
                else
                {
                    Messages.ShowInformation("Problem Opening Product Rules Screen");
                }
            }
            ContextAddClicked = false;
        }

        /// <summary>
        /// called in multiple places used to elim dupe code
        /// </summary>
        /// <param name="RulesObj"></param>
        private void refactorRulesScreenCall(cBaseBusObject RulesObj)
        {
            if (RulesObj != null)
            {
                //show the product rules screen
                callRulesScreen(RulesObj);
            }
        }

        /// <summary>
        /// add selected rate and rule id to bus obj parms for prod rules screen
        /// </summary>
        /// <param name="RulesObj"></param>
        /// <returns></returns>
        private bool getRatesAndRules(ref cBaseBusObject RulesObj, ref bool rateNotSaved)
        {
            try
            {
                string contract_id = "";
                string entity_id = "";
                string entity_name = "";
                string contract_desc = "";
                //cBaseBusObject RulesObj = new cBaseBusObject();
                RulesObj.BusObjectName = "ProductRules";

                gRates.ReturnSelectedData("rate_id");
                if (cGlobals.ReturnParms.Count > 0)
                {
                    //get rate_id 
                    string rateID = cGlobals.ReturnParms[0].ToString();
                    RulesObj.Parms.AddParm("@rate_id", rateID);
                    //KSH -- 10/31/12 Added to prevent rule from being saved with unsaved rate
                    if (Convert.ToInt32(rateID) < 1)
                    {
                        rateNotSaved = true;
                        return false;
                    }
                }
                else
                {
                    Messages.ShowInformation("No Rate Record Selected");
                    return false;
                }
                gRules.ReturnSelectedData("rule_id");
                if (cGlobals.ReturnParms.Count > 0)
                {
                    if (ContextAddClicked == true)
                    {
                        //there will be no rule if adding a rule, set rule_id to 0
                        RulesObj.Parms.AddParm("@rule_id", 0);
                    }
                    else
                    {
                        //get rate_id 
                        string ruleID = cGlobals.ReturnParms[0].ToString();
                        RulesObj.Parms.AddParm("@rule_id", ruleID);
                    }
                }
                gRates.ReturnSelectedData("contract_id");
                if (cGlobals.ReturnParms.Count > 0)
                {
                        //get contract_id 
                        string contractID = cGlobals.ReturnParms[0].ToString();
                        RulesObj.Parms.AddParm("@contract_id", contractID);
                        contract_id = contractID;
                }
                else
                {
                    Messages.ShowInformation("Missing Contract Id from Rates Grid");
                    return false;
                }
                //grab display data for inserting new rule
                if (ContextAddClicked == true)
                {
                    //gRates.ReturnSelectedData("contract_id");
                    //if (cGlobals.ReturnParms.Count > 0)
                    //{
                    //    //get contract_id 
                    //    contract_id = cGlobals.ReturnParms[0].ToString();
                    //}
                    //else
                    //{
                    //    Messages.ShowInformation("Missing Contract Id from Rates Grid");
                    //    return false;
                    //}
                    gRates.ReturnSelectedData("contract_entity_id");
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        //get entity_id 
                        entity_id = cGlobals.ReturnParms[0].ToString();
                    }
                    else
                    {
                        Messages.ShowInformation("Missing Contract Entity Id from Rates Grid");
                        return false;
                    }
                    gRates.ReturnSelectedData("contract_entity");
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        //get entity name 
                        entity_name = cGlobals.ReturnParms[0].ToString();
                    }
                    else
                    {
                        Messages.ShowInformation("Missing Contract Entity Name from Rates Grid");
                        return false;
                    }
                    gRates.ReturnSelectedData("contract_description");
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        //get entity name 
                        contract_desc = cGlobals.ReturnParms[0].ToString();
                    }
                    else
                    {
                        Messages.ShowInformation("Missing Contract Entity Name from Rates Grid");
                        return false;
                    }
                    //add info to globals for rules screen display
                    //RulesObj.Parms.AddParm("@contract_id", contract_id);
                    RulesObj.Parms.AddParm("@contract_entity_id", entity_id);
                    RulesObj.Parms.AddParm("@contract_entity", entity_name);
                    RulesObj.Parms.AddParm("@contract_description", contract_desc);
                    //cGlobals.ReturnParms.Clear();
                    //cGlobals.ReturnParms.Add(contract_id);
                    //cGlobals.ReturnParms.Add(entity_id);
                    //cGlobals.ReturnParms.Add(entity_name);
                    //cGlobals.ReturnParms.Add(contract_desc);
                }
                //add parm for inactive flag in unit_filter (Tier_By lookup)
                RulesObj.Parms.AddParm("@show_inactive", "0");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Do everything necessary to show the prod rules screen
        /// </summary>
        /// <param name="RulesObj"></param>
        private void callRulesScreen(cBaseBusObject RulesObj)
        {

            //tell the rules screen it is inserting if adding new record
            bool IsInserting = false;
            if (ContextAddClicked == true) IsInserting = true;
            //instance rules screen
            ContractProductRulesScreen rulesScreen = new ContractProductRulesScreen(RulesObj, IsInserting, this.CurrentBusObj, gRates, gRules);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window rulesScreenWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            rulesScreenWindow.Title = "Product Rules Screen";
            //DWR modified 1/23/13 to maximize window on launch
            //rulesScreenWindow.MaxHeight = 1480;
            //rulesScreenWindow.Width = 1480;
            rulesScreenWindow.WindowState = WindowState.Maximized;
            /////////////////////////////////////////////////////
            //set rules screen as content of new window
            rulesScreenWindow.Content = rulesScreen;
            //open new window with embedded user control
            rulesScreenWindow.ShowDialog( );

            CurrentBusObj.LoadTable("rule_detail");
            gRuleDetail.LoadGrid(CurrentBusObj, "rule_detail");
            CurrentBusObj.LoadTable("location_rules");
            //gRuleDetail.LoadGrid(CurrentBusObj, "location_rules");
        }

        public void PreBind()
        {
            //RES 4/16/15 default to don't show inactives
            //chkInactive.IsChecked = true;
            chkInactive.IsChecked = false;
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    //if (EndDateRateID > 0)
                    //{
                    //    //System.Windows.MessageBox.Show(EndDateRateID.ToString());
                    //    foreach (Record record in gRates.xGrid.Records)
                    //    {
                    //        DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                    //        if (r["rate_id"].ToString() == EndDateRateID.ToString())
                    //        {
                    //            record.IsActive = true;
                    //            record.IsSelected = true;
                    //            gRates.ActiveRecord.IsSelected = true;
                    //            gRates.xGrid.BringRecordIntoView(record);
                    //            //ScrollCellIntoView(gRates .SelectionSettings.SelectedRows[0].Cells[0]);
                    //            //gRates.ActiveRecord() = true;
                    //        }
                    //    }
                    //}
                    //Add code to populate the grid combobox on the Company Contract Grid
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    //this.cmbStatus.SetBindingExpression("fkey_int", "code_value", this.CurrentBusObj.ObjectData.Tables["rates_status_lookup"]);
                    ip.ItemsSource = this.CurrentBusObj.ObjectData.Tables["rate_status_lookup"].DefaultView;

                    //set the value and display path
                    ip.ValuePath = "fkey_int";
                    ip.DisplayMemberPath = "code_value";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbRatesStatusFlagCombo = ip;
                    //set up combobox for rule groups
                    ComboBoxItemsProvider ip2 = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip2.ItemsSource = this.CurrentBusObj.ObjectData.Tables["rule_group_lookup"].DefaultView;
                    //set the value and display path
                    ip2.ValuePath = "rule_group_id";
                    ip2.DisplayMemberPath = "rule_group_description";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbRulesGroupCombo = ip2;
                    //RES 3/29/16 CLM Number
                    //set up combobox for rule groups
                    //ComboBoxItemsProvider ip3 = new ComboBoxItemsProvider();
                    ////Set the items source to be the databale of the DDDW
                    //ip3.ItemsSource = this.CurrentBusObj.ObjectData.Tables["clm_lookup"].DefaultView;
                    ////set the value and display path
                    //ip3.ValuePath = "clm_id";
                    //ip3.DisplayMemberPath = "clm_number";
                    ////Set the property that the grid combo will bind to
                    ////This value is in the binding in the layout resources file for the grid.
                    //cmbRulesCLMNumber = ip3;
                    //if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["cumulative_rates"].Rows[0]["cumulative_flag"]) == 0)
                    //    CUMFlag = false;
                    //else
                    //    CUMFlag = true;
             
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }

        /// <summary>
        /// Sets the style of the detail grid / rate column to be percentage or currency
        /// </summary>
        public void RulesGridRecordActivatedDelegate()
        {
            if (gRules.ActiveRecord != null)
            {
                Field f = gRuleDetail.xGrid.FieldLayouts[0].Fields["amount"];

                //If the rate should show the currency sign
                if (Convert.ToInt32(gRules.ActiveRecord.Cells["rate_sign_type"].Value ) == 1)
                {
                      f.Settings.EditorStyle=(Style)TryFindResource("RateStyleCurrency");
                }
                else //rate should show percentage style
                {
                      f.Settings.EditorStyle=(Style)TryFindResource("RateStylePercent");
                }

            }
        }
     
        //public void gRates_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        //{
        //    var dataTable = MainScreen.ContractBusObject.DataTables.Where(dt => dt.GetType() == typeof(RateDataTable)).FirstOrDefault();

        //    if (dataTable != null)
        //    {
        //        RateDataTable rateDataTable = dataTable as RateDataTable;
        //        rateDataTable.SelectedRecord = gRates.xGrid.ActiveRecord;
        //    }
        //}

        public void RatesGridEditModeDelegate(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs e)
        {
            if (e.Cell.Record.IsAddRecord && e.Cell.Field.Name == "start_date")
            {
                //If other rates in the rates table
                if (this.CurrentBusObj.ObjectData.Tables["rates"].Rows.Count > 0)
                {
                    //Currently just picks last row
                    e.Cell.Value = Convert.ToString(Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["rates"].Rows[this.CurrentBusObj.ObjectData.Tables["rates"].Rows.Count - 1]["end_date"]).AddDays(1));
                    e.Cell.Record.Cells["end_date"].Value = Convert.ToString(Convert.ToDateTime(e.Cell.Value).AddYears(1).AddDays(-1));
                }
                else //If no other rates then just pick a default value
                {
                    DateTime dtNow = DateTime.Now;
                    e.Cell.Value = Convert.ToString(dtNow.Month.ToString() + "/1/" + dtNow.Year.ToString());
                    e.Cell.Record.Cells["end_date"].Value = Convert.ToString(Convert.ToDateTime(e.Cell.Value).AddYears(1).AddDays(-1));
                    //e.Cell.Record.Cells["end_date"].Value = 0;

                }
            }
            gRates.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
        }

        //If Show Inactive is checked display all rates and rules else only active
        //private void txtShowInactiveFlag_Checked(object sender, RoutedEventArgs e)
        //{
        //    //if (Convert.ToBoolean(txtShowInactiveFlag.IsChecked))
        //    //{
        //    //    txtRenewalDate.SelText = Convert.ToDateTime("1/1/1900");
        //    //    txtExpirationDate.SelText = Convert.ToDateTime("1/1/1900");
        //    //    txtTerm.Text = "0";
        //    //    txtRenewalTerms.Text = "0";
        //    //    txtRenewalTerm.Text = "0";
        //    //    txtTerm.IsReadOnly = true;
        //    //    txtRenewalTerm.IsReadOnly = true;
        //    //    txtRenewalTerms.IsReadOnly = true;
        //    //}
        //    //else
        //    //{
        //    //    txtTerm.IsReadOnly = false;
        //    //    txtRenewalTerm.IsReadOnly = false;
        //    //    txtRenewalTerms.IsReadOnly = false;
        //    //    CalcTerm(sender, e);
        //    //}
        //}

        void gRates_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            //DateTime DEndDate;
            //DateTime DCurrentDate = DateTime.Today;
            DateTime DOldEndDate;
            Int32 RulesChanged = 0;
            Int32 RateID = 0;
            string message;

            if (e.Cell.Record.IsDataChanged && e.Cell.Field.Name == "end_date")
            {
                DOldEndDate = Convert.ToDateTime(e.Cell.Record.Cells["old_end_date"].Value);
                RateID = Convert.ToInt32(e.Cell.Record.Cells["rate_id"].Value);
                EndDateRateID = RateID;
                if (this.CurrentBusObj.ObjectData.Tables["rules"].Rows.Count > 0)
                {
                    RulesChanged = 0;
                    foreach (DataRecord r in gRules.xGrid.Records)
                    {
                        if ((Convert.ToDateTime(r.Cells["end_date"].Value.ToString()) == DOldEndDate) && (Convert.ToInt32(r.Cells["rate_id"].Value.ToString()) == RateID))
                        {
                            //r.Cells["end_date"].Value = DNewEndDate;
                            RulesChanged = RulesChanged + 1;
                        }
                    }
                    if (RulesChanged > 0)
                    {
                        message = "Do you want " + RulesChanged.ToString() + " rule end dates updated with the new rate end date?";
                        if (Messages.ShowYesNo(message, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            e.Cell.Record.Cells["update_rules_flag"].Value = 1;
                        else
                            e.Cell.Record.Cells["update_rules_flag"].Value = 0;
                    }
                }
                //commit user entered value to datatable
                gRates.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            }
  
        }

        private void chkBox_Click(object sender, RoutedEventArgs e)
        {
            //setup and pass chkbox value to stored proc for inactives
            ContractFolderMainScreen c;
            c = findRootScreenBase(this) as ContractFolderMainScreen;
            //c.txtMultipleLocations.IsChecked = 1;

            //int ShowInactive = 0;
            ShowInactive = 0;
            if (Convert.ToBoolean(chkInactive.IsChecked))
                ShowInactive = 1;
            //else
            //    ShowInactive = 0;
            ///////////////////////////////////////////////////////////

            c.ShowInactive = ShowInactive;
            this.CurrentBusObj.Parms.UpdateParmValue("@ShowInactives", ShowInactive.ToString());
            this.CurrentBusObj.LoadTable("rates");
            gRates.LoadGrid(CurrentBusObj, "rates");
           
        }

        //public void ReloadRates()
        //{
        //    gRates.LoadGrid(CurrentBusObj, "rates");
        //}

        public void HighLightCurrentBillPeriod()
        {
            //do nothing if no rows
            if (gRates.xGrid.Records.Count < 1) return;
            //determine current bill period
            DateTime BillPeriodDate = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["rates"].Rows[0]["billing_period"]);
            System.TimeSpan diffClosestSpan = BillPeriodDate - System.DateTime.Now;
            bool MatchFound = false;
            DataRecord currDR;
            DateTime currentStartDate;
            DataRecord ClosestDR = null;
            int j = 0;
            //select record in grid that matches current bill period
            for (int i = 0; i < gRates.xGrid.Records.Count; i++)
            {
                currDR = gRates.xGrid.Records[i] as DataRecord;
                //only evaluate where status_flag is active (0)
                if (currDR.Cells["status_flag"].Value.ToString() == "0")
                {
                    currentStartDate = Convert.ToDateTime(currDR.Cells["start_date"].Value);
                    if (j == 0)
                    {
                        //set closest record on first iteration as current start date
                        ClosestDR = currDR;
                        //set first record
                        diffClosestSpan = BillPeriodDate - currentStartDate;
                    }
                    //get the difference btwn the BillPeriod date and currentStartDate
                    System.TimeSpan diffcurrentStartDate = BillPeriodDate - currentStartDate;
                    //if date is greater than bill date make sure duration is always a positive number for evaluation
                    if (currentStartDate > BillPeriodDate) diffcurrentStartDate.Negate();
                    //compare absolute differences
                    if (diffcurrentStartDate.Duration().Days <= diffClosestSpan.Duration().Days)
                    {
                        diffClosestSpan = diffcurrentStartDate;
                        ClosestDR = currDR;
                        //used to set active record if match found
                        MatchFound = true;
                    }
                    j++;
                }
            }
            //set the closest record to billdate if record found
            if (MatchFound)
            {
                //set new active record//////////////////////
                gRates.SetGridSelectionBehavior(true, false);
                gRates.xGrid.ActiveRecord = ClosestDR;
                ClosestDR.IsActive = true;
                ClosestDR.IsSelected = true;
                gRates.FilterCall();
            }
        }



   

    }
}
