using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DataPresenter;
using System;
using System.Data;

namespace Contract
{
    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class ContractFolderMainScreen : ScreenBase, IScreen
    {
        public cBaseBusObject ContractBusObject { get; private set; }
        public string WindowCaption { get; private set; }
        
        public string ContractID { get; set; }
        public bool IsScreenInserting { get; set; }

        bool LocalCLMFlag = true;

        //RES 11/18 flag to indicate cumulative rates exist
        //bool LocalCUMFlag = false;

        public int RoleAccess = 0;
        //RES 4/27/15 default to not show inactives
        public int ShowInactive = 0;
        //public string strShowInactive;
        public string sOpsPeriodStart;
        public string sOpsPeriodEnd;

        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBase constructor is also called.
        /// </summary>
        public ContractFolderMainScreen()
            : base()
        {
            // Create Controls
            InitializeComponent();
            DataContextChanged += new System.Windows.DependencyPropertyChangedEventHandler(ContractFolderMainScreen_DataContextChanged);
        }

        protected void ContractFolderMainScreen_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null)
            {
            }
        }

        
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            //set screen inserting flag to false
            IsScreenInserting = false;
            WindowCaption = "Contract";
           

            //if (strShowInactive == null)
            //{
            //    ShowInactive = 1;
            //    //strShowInactive = "1";
            //}

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            // Change this if your folder has item directly bound to it.
            // In many cases the Tabs have data binding, the Folders
            // do not.
            this.DoNotSetDataContext = false;

            //Set the maintablename for the folder if it has one
            this.MainTableName = "contract_product";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            Rates.MainScreen = this;
            Reporting.MainScreen = this;

            // add the Tab user controls that are of type screen base
            TabCollection.Add(General);
            TabCollection.Add(Billing);
            TabCollection.Add(Locations);
            TabCollection.Add(Rates);
            TabCollection.Add(Reporting);
            TabCollection.Add(BillingTotals);
            TabCollection.Add(Comments);
            TabCollection.Add(Units);
            TabCollection.Add(Ops);
            TabCollection.Add(LocRules);
            TabCollection.Add(ContractAttachments);
            TabCollection.Add(ReviewDates);

            //Debug code for hardwiring a test parameter set
            //this.CurrentBusObj.Parms.AddParm("@contract_id", "1" );

            //if there are parameters passed into the window on startup then we need to load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                // load the data
                if (txtContractID.Text != null)
                    WindowCaption = "Contract-" + txtContractID.Text;
                else
                    if (this.CurrentBusObj.Parms.ParmList.Rows[0].ItemArray[1] != null)
                    {
                        WindowCaption = "Contract-" + this.CurrentBusObj.Parms.ParmList.Rows[0].ItemArray[1].ToString();
                        txtContractID.Text = this.CurrentBusObj.Parms.ParmList.Rows[0].ItemArray[1].ToString();
                        loadParms(txtContractID.Text);
                    }
                //ReturnData();
                this.Load();
                //clear any unit totals on Units tab when entity changes
                Units.txtUnitTotal.Text = "";
                //ContractFolderMainScreen c;
                //c = findRootScreenBase(this) as ContractFolderMainScreen;
                chkCOLA();
                //Rates.ContractMainScreen = this;
                //Rates.ContractGeneral = General;
                ChkFieldLevelSecurity();
                //if (IsScreenDirty || ForceScreenDirty)
                //    MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
            }
            else
            {
                setInitScreenState();
            }
            //setInitScreenState();
            CurrentBusObj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentBusObj_PropertyChanged);
            SetHeader();
            //Need to set context menu security for field level security - used on location tab
            if (this.CurrentBusObj.ObjectData != null && this.CurrentBusObj.ObjectData.Tables["role"].Rows != null && this.CurrentBusObj.ObjectData.Tables["role"].Rows.Count > 0)
            {
                RoleAccess = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["role"].Rows[0]["access_level"]);
                if (RoleAccess > 1)
                {
                    Locations.gLocations.ContextMenuGenericIsVisible1 = true;
                    Locations.gLocations.ContextMenuAddIsVisible = true;
                    Locations.gLocations.ContextMenuRemoveIsVisible = true;
                    Reporting.gReportDistribution.ContextMenuAddIsVisible = true;
                    Reporting.gReports.ContextMenuAddIsVisible = true;
                    Reporting.gReports.ContextMenuGenericIsVisible1 = true;

                }
                else
                {
                    Locations.gLocations.ContextMenuGenericIsVisible1 = false;
                    Locations.gLocations.ContextMenuAddIsVisible = false;
                    Locations.gLocations.ContextMenuRemoveIsVisible = false;
                    Reporting.gReportDistribution.ContextMenuAddIsVisible = false;
                    Reporting.gReports.ContextMenuAddIsVisible = false;
                    Reporting.gReports.ContextMenuGenericIsVisible1 = false;

                }

            }
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0) ReturnData();
            
        }

        void CurrentBusObj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (CurrentBusObj.HasObjectData)
            {
                ContractBusObject = this.CurrentBusObj;
            }
            else
            {
                ContractBusObject = null;
            }
        }

        /// <summary>
        /// Load bus obj parms, used in multiple places
        /// </summary>
        private void loadParms(string parmContractId)
        {
            try
            {
                //if (IsScreenDirty || ForceScreenDirty)
                //    MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //RES 4/16/15 default to don't show inactives when data is initially retrieved
                //this.CurrentBusObj.Parms.AddParm("@ShowInactives", "0");
                //RES 4/27/15 default to not show inactives
                ShowInactive = 0;
                this.CurrentBusObj.Parms.AddParm("@ShowInactives", ShowInactive.ToString());
                //if contractId passed load external_char_id and recv_acct with passed contractId
                if (parmContractId != "")
                {
                    this.CurrentBusObj.Parms.AddParm("@external_int_id", parmContractId);
                    this.CurrentBusObj.Parms.AddParm("@contract_id", parmContractId);
                }
                else
                {
                    //if contractId NOT passed load external_char_id and contract with global parm contractId if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.AddParm("@external_int_id", cGlobals.ReturnParms[0].ToString());
                        this.CurrentBusObj.Parms.AddParm("@contract_id", cGlobals.ReturnParms[0].ToString());
                    }
                    //doing an insert setup dummy vals
                    else
                    {
                        this.CurrentBusObj.Parms.AddParm("@external_int_id", "-1");
                        this.CurrentBusObj.Parms.AddParm("@contract_id", "-1");

                        //this.CurrentBusObj.Parms.AddParm("@related_to_char_id", "-1");
                    }
                }
                //constant parms//////////////////////////////////////////
                //comment tab parms
                this.CurrentBusObj.Parms.AddParm("@comment_type", "CT");
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
                //attachment parms
                this.CurrentBusObj.Parms.AddParm("@attachment_type", "CATTACH");
                //Modified DWR 1/27
                this.CurrentBusObj.Parms.AddParm("@external_char_id", "");
                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
                //product rules screen parms
                this.CurrentBusObj.Parms.AddParm("@rate_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@rule_id", "-1");
                //Contract Units parms
                this.CurrentBusObj.Parms.AddParm("@service_period_start", "01/01/1900");
                this.CurrentBusObj.Parms.AddParm("@service_period_end", "01/01/1900");
                //Contract Ops parms
                this.CurrentBusObj.Parms.AddParm("@ops_period_start", "01/01/1900");
                this.CurrentBusObj.Parms.AddParm("@ops_period_end", "01/01/1900");
                if (this.CurrentBusObj.ObjectData != null)
                {
                    this.CurrentBusObj.changeParm("@ops_period_start", "1/1/1900");
                    this.CurrentBusObj.changeParm("@ops_period_end", "1/1/1900");
                    Ops.ClearDates();
                }
                //if (sServicePeriodStart == null || sServicePeriodStart == "")
                //    this.CurrentBusObj.Parms.AddParm("@ops_period_start", "01/01/1900");
                //else
                //    this.CurrentBusObj.Parms.AddParm("@ops_period_start", sServicePeriodStart);
                //if (sServicePeriodEnd == null || sServicePeriodEnd == "")
                //    this.CurrentBusObj.Parms.AddParm("@ops_period_end", "01/01/1900");
                //else
                //    this.CurrentBusObj.Parms.AddParm("@ops_period_end", sServicePeriodEnd);
                //Contract Billing Parms
                this.CurrentBusObj.Parms.AddParm("@contact_id_temp", "-1");
                ////Contract Reporting Parms
                //this.CurrentBusObj.Parms.AddParm("@report_detail_id", "-1");
                //Customer Lookup
                this.CurrentBusObj.Parms.AddParm("@receivable_account_lookup", "");
                //Location role
                this.CurrentBusObj.Parms.AddParm("@user_id", cGlobals.UserName);
                /////////////////////////////////////////////////////////
                //Set the comment code for the comments tab
                Comments.CommentCode = "CT";
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }

        /// <summary>
      
        /// Verifies required fields
        /// </summary>
        /// <param name="MissingField"></param>
        /// <returns></returns>
        private bool RequiredFieldsOK(ref string MissingField)
        {
            //KSH - 10/2/2012 Added to keep empty folder from being saved
            if (General.txtBillMsoId.Text == "0")
            {
                MissingField = "Contract Entity Id";
                return false;
            }
          
            if (General.cmbProductCode.SelectedText == "")
            {
                MissingField = "Product";
                return false;
            }
           

            if (General.txtPrescheduleBillFlag.IsChecked.ToString() == "0")
            {
                General.txtPrescheduleDayofMo.Text  = "0";
              
            }
            if (General.cmbBillingOwner.SelectedText == "")
            {
                MissingField = "Billing Owner";
                return false;
            }
            return true;
        }

        private bool VerifyData()
        {
            LocalCLMFlag = Billing.CLMFlag;
            //LocalCUMFlag = Rates.CUMFlag;
            //need this to save even if they do not tab off the field
            ReviewDates.gReviewDates.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndCommitRecord);
            Billing.gSalesperson.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndCommitRecord);
            //verify the targeted reviewer is not blank on the Review Dates tab
            //Verify that all allocations have either product,account and unapplied flag checked or have document and seq
            if (General.txtPrescheduleBillFlag.IsChecked.ToString() == "1")
            {

                int test;

                if (!Int32.TryParse(General.txtPrescheduleDayofMo.Text.ToString(), out test))
                {
                    General.txtPrescheduleDayofMo.Text = "0";
                    MessageBox.Show("Prescheduled Day of the Month should entered as a number between 1 and 31.");
                    return false;

                }              

                if (Convert.ToInt32(General.txtPrescheduleDayofMo.Text) > 31)
                {
                        MessageBox.Show("Prescheduled Day of the Month cannot be greater than 31 when the Prescheduled Billing is checked.");
                        return false;
                }

                if (Convert.ToInt32(General.txtPrescheduleDayofMo.Text) == 0)
                {
                    MessageBox.Show("Prescheduled Day of the Month be between 1 and 31 when the Prescheduled Billing is checked.");
                    return false;
                }
                
              
            }
            else
            {

                General.txtPrescheduleDayofMo.Text = "0";
            }

            foreach (DataRow r in CurrentBusObj.ObjectData.Tables["contract_review"].Rows)
            {

                if (r["targeted_reviewer"] == null)
                {
                    MessageBox.Show("Targeted Reviewer is required on the Review Dates tab.");
                    return false;

                }
                if (r["targeted_reviewer"].ToString().Trim() == "")
               
                {
                    MessageBox.Show("Targeted Reviewer is required on the Review Dates tab.");
                    return false;

                }
                if (r["targeted_reviewer"].ToString() == " ")
                {
                    MessageBox.Show("Targeted Reviewer is required on the Review Dates tab.");
                    return false;

                }
                 if (r["review_date"] == null)
                {
                    MessageBox.Show("Review Date is required on the Review Dates tab.");
                    return false;

                }
                if (r["review_date"].ToString() == "01/01/1900")
                 {
                    MessageBox.Show("Review Date is required on the Review Dates tab.");
                    return false;

                }
                if (r["review_date"].ToString().Trim() == "1/1/1900 12:00:00 AM")
                {
                    MessageBox.Show("Review Date is required on the Review Dates tab.");
                    return false;

                }
                if (r["review_date"].ToString() == " ")
                {
                    MessageBox.Show("Review Date is required on the Review Dates tab.");
                    return false;

                }
            }
            //Verify that all allocations have either product,account and unapplied flag checked or have document and seq
            foreach (DataRow s in CurrentBusObj.ObjectData.Tables["rates"].Rows)
            {
                if (Convert.ToDateTime(s["start_date"]) > Convert.ToDateTime(s["end_date"]))
                {
                    MessageBox.Show("Rate Start Date Cannot be Saved with a Value Greater Than Rate End Date");
                    return false;

                }
            }
            //Verify that end_date has been entered if it is a turn off
            foreach (DataRow t in CurrentBusObj.ObjectData.Tables["location"].Rows)
            {
                if (t.RowState != DataRowState.Deleted && t.RowState != DataRowState.Detached)
                    if ((t["end_date"].ToString().Trim() == "1/1/1900 12:00:00 AM") && (t["turn_off_flag"].ToString().Trim() == "1"))
                    {
                        MessageBox.Show("End Date is required for Turn Off.");
                        //MessageBox.Show("End Date is required for Turn Off." + t["cs_id"].ToString().Trim() + " " + t["end_date"].ToString().Trim() + " " + t["turn_off_flag"].ToString());
                        return false;
                    }
            }
            //RES 8/24/18 Verify only 1 Ads Revenue Share exists
            //int RevShareAttributes = 0;
            //foreach (DataRow z in CurrentBusObj.ObjectData.Tables["attribute"].Rows)
            //{
            //    if (z["attribute_id"].ToString().Trim() == "5")
            //        RevShareAttributes = RevShareAttributes + 1;
            //}
            //if (RevShareAttributes > 1)
            //{
            //    MessageBox.Show("Only 1 Ads Rev % per contract");
            //    return false;
            //}

            //RES 1/21/21 Verify Ads Revenue Share is entered as decimal
            decimal adsrevshare = 0;
            decimal colamin = 0;
            decimal colamax = 0;
            bool canConvert;
            foreach (DataRow z in CurrentBusObj.ObjectData.Tables["attribute"].Rows)
            {
                if (z["attribute_id"].ToString().Trim() == "5")
                {
                    canConvert = decimal.TryParse(z["description"].ToString().Trim(), out adsrevshare);
                    if (canConvert == false)
                    {
                        MessageBox.Show("Ads Revenue Share not valid.  It must be entered as a decimal (i.e. 15% is entered as .15)");
                        return false;
                    }
                    else
                        if (adsrevshare > 1)
                        {
                            MessageBox.Show("Ads Rev Share cannot be greater than 1");
                            return false;
                        }
                }
                //RES 2/25/22 Verify COLA min/max entered as decimal
                if (z["attribute_id"].ToString().Trim() == "8")
                {
                    canConvert = decimal.TryParse(z["description"].ToString().Trim(), out colamin);
                    if (canConvert == false)
                    {
                        MessageBox.Show("COLA minimum not valid.  It must be numeric");
                        return false;
                    }
                    else
                        if (colamin < 1)
                        {
                            MessageBox.Show("COLA minumum value must be greater than 1 (i.e. 5% is entered as 5)");
                            return false;
                        }
                }
                if (z["attribute_id"].ToString().Trim() == "9")
                {
                    canConvert = decimal.TryParse(z["description"].ToString().Trim(), out colamax);
                    if (canConvert == false)
                    {
                        MessageBox.Show("COLA maximum not valid.  It must be numeric");
                        return false;
                    }
                    else
                        if (colamax < 1)
                        {
                            MessageBox.Show("COLA maximum value must be greater than 1 (i.e. 5% is entered as 5)");
                            return false;
                        }
                }
            }

            //RES 5/10/16 if salesforce account id exists save it so it can be assigned later
            string SFAccountID = "";
            foreach (DataRow x in CurrentBusObj.ObjectData.Tables["contract_salesperson"].Rows)
            {
                if (x.RowState != DataRowState.Deleted && x.RowState != DataRowState.Detached)
                    if (x["sf_account_id"].ToString() != "")
                        SFAccountID = x["sf_account_id"].ToString();
            }

            //Verify product code is not being assigned twice in the Salesperson grid on the Detail tab and product code is not blank
            int rowDeleteCnt = 0;
            foreach (DataRow u in CurrentBusObj.ObjectData.Tables["contract_salesperson"].Rows)
            {

                if (u.RowState != DataRowState.Deleted && u.RowState != DataRowState.Detached)
                {if (u["product_code"].ToString() == "")                    
                    {
                        MessageBox.Show("Product Code is required.");
                        return false;
                    }
                }

                if (u.RowState == DataRowState.Deleted)
                {
                    rowDeleteCnt = rowDeleteCnt + 1;
                }

                //RES 5/10/16 
                if (u.RowState != DataRowState.Deleted && (u.RowState == DataRowState.Added || u.RowState == DataRowState.Modified))
                {
                    if (u["sf_account_id"].ToString() == "")
                    {
                        if (SFAccountID != "")
                            u["sf_account_id"] = SFAccountID;
                        //else 
                        //    Messages.ShowWarning("Warning.  SalesForce Account ID is missing.");
                        //return false;
                    }
                }
            }


            int rowCnt = CurrentBusObj.ObjectData.Tables["contract_salesperson"].Rows.Count;
            rowCnt = rowCnt - rowDeleteCnt;
            DataTable salesperson = CurrentBusObj.ObjectData.Tables["contract_salesperson"];
            DataView view = new DataView(salesperson);
            DataTable distinctValues = view.ToTable(true, "product_code");
            int prodCnt = distinctValues.Rows.Count;

            if (rowCnt > prodCnt && prodCnt != 0)
            {
                MessageBox.Show("Product Code can only be assigned to one salesperson.");
                return false;
            }

            //RES 4/11/17 Require PO number
            foreach (DataRow p in CurrentBusObj.ObjectData.Tables["purchase_order"].Rows)
            {
                if (p.RowState == DataRowState.Added && p["po_number"].ToString() == "")
                {
                    MessageBox.Show("Purchase Order number is required");
                    return false;
                }
            }

            //RES 3/21/17 Inserted PO numbers must have a non 0 PO amount
            foreach (DataRow p in CurrentBusObj.ObjectData.Tables["purchase_order"].Rows)
            {
                if (p.RowState == DataRowState.Added && Convert.ToDecimal(p["amount"].ToString()) == 0)
                {
                    MessageBox.Show("Purchase Order amount is required");
                    return false;
                }
            }
           
            //Dim rowCnt As Integer = MyAudioDataSet.AudioInfo.Rows.Count
            // Dim qry = From AudioInfo In MyAudioDataSet.AudioInfo _
            //      Select AudioInfo.AlbumName Distinct
            // Dim rowCntDistinct As Integer = qry.Count
            // MessageBox.Show("Row Count: " & rowCnt.ToString("n0") & vbCrLf & _
            //            "Unique Album Name Count: " & rowCntDistinct.ToString("n0"))

            //if (IsScreenInserting && clm_id == 0)
            //{
            //    MessageBox.Show("Product Code can only be assigned to one salesperson.");
            //    return false;
            //}


            //RES 8/29/17 Inserted CLM numbers must have a deal type and execution date
            foreach (DataRow p in CurrentBusObj.ObjectData.Tables["legal_agreement"].Rows)
            {
                if (p.RowState == DataRowState.Added && Convert.ToDecimal(p["deal_type"].ToString()) == 0)
                {
                    MessageBox.Show("CLM Deal Type is required");
                    return false;
                }

                if (p.RowState == DataRowState.Added && p["contract_executed_date"].ToString().Trim() == "1/1/1900 12:00:00 AM")
                {
                    MessageBox.Show("Contract Execution Date is required");
                    return false;
                }
            }

            //RES 11/18/19 only allow cumulative digits if a cumulative rate exist
            //if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["cumulative_rates"].Rows[0]["cumulative_flag"]) == 0)
            //    LocalCUMFlag = false;
            //else
            //    LocalCUMFlag = true;
            //if (!LocalCUMFlag && General.txtCumulativeDigits.Text != "0")
            //{
            //    MessageBox.Show("Cannot specify cumulative rate override if no cumulative rates exist");
            //    return false;
            //}
            //RES 12/3/19 only 0 - 6 is allowed for cumulative override digits
            //if (General.txtCumulativeDigits.Text != "0" && General.txtCumulativeDigits.Text != "1" && General.txtCumulativeDigits.Text != "2" && General.txtCumulativeDigits.Text != "3" &&
            //    General.txtCumulativeDigits.Text != "4" && General.txtCumulativeDigits.Text != "5" && General.txtCumulativeDigits.Text != "6")
            //{
            //    MessageBox.Show("Only 0 - 6 is allowed for cumulative rate override");
            //    return false;
            //}


            return true;
        }

        public override void Save()
        {
            //if (ContractBusObject != null && !ContractBusObject.IsValid)
            //{
            //    MessageBox.Show(ContractBusObject.ValidationMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //causes a problem with rates tab. if user enters a date and clicks enter w/out tabbing off date, current date gets put
            //in its place and saved instead
            Prep_ucBaseGridsForSave();
            PrepareFreeformForSave();

            string MissingField = "";
            //chk reqd fields
            if (RequiredFieldsOK(ref MissingField) == false)
            {
                Messages.ShowWarning("Required Field: " + MissingField + " must have a value");
                dirtyBubbleUp();
                SaveSuccessful = false;
                return;
            }
           
            //Verify that the data to be saved is valid
            if (!VerifyData())
            {
                MessageBox.Show("Unable to save data until all errors are corrected.");
                SaveSuccessful = false;
                return;
            }
            int RevShareAttributes = 0;
            foreach (DataRow z in CurrentBusObj.ObjectData.Tables["attribute"].Rows)
            {
                if (z["attribute_id"].ToString().Trim() == "5")
                    RevShareAttributes = RevShareAttributes + 1;
            }
            if (RevShareAttributes > 1)
            {
                MessageBox.Show("Only 1 Ads Rev % per contract");
                Load("attribute");
                return;
            }
            //this.CurrentBusObj.Parms.UpdateParmValue("@ShowInactives", ShowInactive.ToString());
            base.Save();
            if (SaveSuccessful)
                {
                    //RES phase 3.0.1 reload rates table with inactives filtered out
                    //if (ShowInactive != 1)
                    //{
                    ContractRatesTab c;
                    c = findRootScreenBase(this) as ContractRatesTab;
                    //ShowInactive = c.ShowInactive;
                    this.CurrentBusObj.Parms.UpdateParmValue("@ShowInactives", ShowInactive.ToString());
                    this.CurrentBusObj.LoadTable("rates");
                    Rates.gRates.LoadGrid(CurrentBusObj, "rates");
                    //c.gRates.LoadGrid(CurrentBusObj, "rates");
                    //this.CurrentBusObj.LoadTable("rules");
                    //this.CurrentBusObj.LoadTable("rule_detail");
                    //c.gRates.LoadGrid(CurrentBusObj, "rates");
                    //}
                    ////check if attachment tab files need to be copied
                    //if (cGlobals.GlobalAttachmentsStorageList.Count > 0)
                    //{
                    //    //if so pass attachment data table to attachment helper class
                    //    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                    //    attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables["Attachments"]);

                    //}

                    ////check if comment attachment files need to be copied
                    //if (cGlobals.GlobalCommentAttachmentsStorageList.Count > 0)
                    //{
                    //    //if so pass attachment data table to attachment helper class
                    //    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                    //    attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables["Comment_Attachment"]);

                    //}

                    var localContractId = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                          where x.Field<string>("parmName") == "@contract_id"
                                          select x.Field<string>("parmValue");

                    foreach (var id in localContractId)
                    {
                        //pop contract id
                        txtContractID.Text = id;
                        if (IsScreenInserting == false)
                        {
                            //updating, don't reload bus obj
                            ReturnData(false);
                        }
                        else
                        {
                            //Insert has occurred
                            //reset business obj for load of new cust
                            this.CurrentBusObj.ObjectData = null;
                            ReturnData(true);
                            //turn off inserting flag
                            IsScreenInserting = false;
                        }
                    }
                    Messages.ShowInformation("Save Successful");
                    ////RES 3/28/16 Only 1 CLM Number so ask if it should be copied to all active rules. DateTime dtNow = DateTime.Now;
                    if (LocalCLMFlag && CurrentBusObj.ObjectData.Tables["legal_agreement"].Rows.Count > 0 &&
                        CurrentBusObj.ObjectData.Tables["legal_agreement"].Rows.Count < 2 &&
                        CurrentBusObj.ObjectData.Tables["rules"].Rows.Count > 0)
                    {

                        var result = MessageBox.Show("Do you want to assign the CLM Number to all active rules?", "Update Rules", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                        {
                            int RulesUpdated = 0;
                            foreach (DataRow p in CurrentBusObj.ObjectData.Tables["rules"].Rows)
                            {
                                if (p["status_flag"].ToString() != "2" && Convert.ToDateTime(p["end_date"]) >= DateTime.Now)
                                {
                                    p["clm_id"] = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["legal_agreement"].Rows[0]["clm_id"]);
                                    RulesUpdated = RulesUpdated + 1;
                                }
                            }
                            MessageBox.Show(RulesUpdated.ToString() + " active rules updated with CLM Number: " + this.CurrentBusObj.ObjectData.Tables["legal_agreement"].Rows[0]["clm_number"].ToString());
                            if (RulesUpdated > 0)
                            {
                                base.Save();
                                if (!SaveSuccessful)
                                {
                                    Messages.ShowInformation("Save Failed");
                                    return;
                                }
                            }
                        }
                        LocalCLMFlag = false;
                        this.CurrentBusObj.LoadTable("rules");
                        Rates.gRates.LoadGrid(CurrentBusObj, "rules");
                    }
                    //RES 3/31/16 re-load after a save to get a row in the rates grid selected
                    //this.CurrentBusObj.LoadTable("rates");
                    //this.CurrentBusObj.LoadTable("rules");
                    //this.SaveTable("rules");
                    //this.Load();
                    //chkCOLA();                                   
                }
                else
                {
                    Messages.ShowInformation("Save Failed");
                }
             
        }

        public override void New()
        {
            //turn on inserting flag, needed for setting proper screen state
            IsScreenInserting = true;
            ContractID = "-1";
            txtContractID.Text = "New";
            //Add default parameters
            loadParms("");
            setInsertScreenState();
            ContentPane p = (ContentPane)this.Parent;
            p.Header = "Contract-" + txtContractID.Text;
            //RES 4/27/15 default to not show inactives
            ShowInactive = 0;
            base.New();
        }

          /// Check field level on location tab
        private void ChkFieldLevelSecurity() 
    {
        //Need to set context menu security for field level security - used on location tab
        if (this.CurrentBusObj.ObjectData != null && this.CurrentBusObj.ObjectData.Tables["role"].Rows != null && this.CurrentBusObj.ObjectData.Tables["role"].Rows.Count > 0)
        {
            RoleAccess = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["role"].Rows[0]["access_level"]);
            if (RoleAccess > 1)
            {
                Locations.gLocations.ContextMenuGenericIsVisible1 = true;
                Locations.gLocations.ContextMenuAddIsVisible = true;
                Locations.gLocations.ContextMenuRemoveIsVisible = true;

            }
            else
            {
                Locations.gLocations.ContextMenuGenericIsVisible1 = false;
                Locations.gLocations.ContextMenuAddIsVisible = false;
                Locations.gLocations.ContextMenuRemoveIsVisible = false;

            }

        }
    }
        /// <summary>
        /// Checks that data exists before trying to SetHeader()
        /// </summary>
        /// <returns></returns>
        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                return true;
            }
            else
            {
                Messages.ShowWarning("Contract Not Found");
                return false;
            }
        }

        private void ReturnData(bool NeedToGetData = true)
        {
            int iDummy = 0;
            //make sure contract id contains a number
            //RES 12/16/14 fix entry from other screens issue
            //if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            //    if (this.CurrentBusObj.Parms.ParmList.Rows[0].ItemArray[1] != null)
            //    {
            //        txtContractID.Text = this.CurrentBusObj.Parms.ParmList.Rows[0].ItemArray[1].ToString();
            //        this.CurrentBusObj.Parms.ClearParms();
            //    }
            if (Int32.TryParse(txtContractID.Text, out iDummy) && txtContractID.Text != ContractID)
            {
                //DWR--Added 1/15/13 - This section will check to see if changes have been made and if save is desired in the event of a
                //double click lookup or a change of the contract id field.
                //Verify that no save is needed
                Prep_ucBaseGridsForSave();
                PrepareFreeformForSave();
                if (IsScreenDirty)
                {
                    //Establish a temporary contract id for storing the ID the user wanted to go to.  This will be used in the final retrieval in the event of a
                    //Yes or No answer to the question below.
                    int NewContractID = 0;
                    System.Windows.MessageBoxResult result = Messages.ShowYesNoCancel("Would you like to save existing changes?",
                               System.Windows.MessageBoxImage.Question);
                    //Save existing contract information and then load new contract
                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        Int32.TryParse(txtContractID.Text, out NewContractID);
                        Save();
                        //If Save fails then reset contract id to original value and exit retrieve so that changes will not be lost.
                        if (!SaveSuccessful)
                        {
                            txtContractID.Text = ContractID.ToString();
                            return;
                        }
                        else if (NewContractID != 0)
                        {
                            txtContractID.Text = NewContractID.ToString();
                            ReturnData();
                        }
                    }
                    //Returns the user to the current contract window and resets the txtContractID field to original value.
                    else if (result == System.Windows.MessageBoxResult.Cancel)
                    {
                        txtContractID.Text = ContractID.ToString();
                        return;

                    }
                }

                //Clear the current parameters
                //sServicePeriodStart = this.CurrentBusObj.Parms.GetParm("@ops_period_start");
                //sServicePeriodEnd = this.CurrentBusObj.Parms.GetParm("@ops_period_end");
                this.CurrentBusObj.Parms.ClearParms();
               //Add new parameters
                //this.CurrentBusObj.Parms.AddParm("@contract_id", txtContractID.Text);
                loadParms(txtContractID.Text);

                //Set the local contract id

                if (NeedToGetData)
                {
                    //KSH - 8/21/12 clear comments/attachments grid to fix bug
                    clrCommentsAttachmentsObj();
                    //////////////////////////////////////////////////////////
                    //Clear units' dates to fix bug
                    clrUnits();
                    this.Load();
                    //KSH - 10/26/12 set tabs screen state when contract changes
                    setTabScreenStateOnLoad();
                    //make sure data exists for contract and then respond appropriately
                    if (chkForData())
                    {
                        //set folder back to edit mode when data is present
                        chkCOLA();
                        ChkFieldLevelSecurity();
                        setEditScreenState();
                    }
                    //////////////////////////////////////////////////////////////////////
                }
                ContractID = txtContractID.Text;

            }
            WindowCaption = "Contract-" + txtContractID.Text;
            SetHeader();
        }

        /// <summary>
        /// Sets values in tabs when contract changes
        /// </summary>
        private void setTabScreenStateOnLoad()
        {
            //KSH - 10/23/12 call sum unit totals if contract changed and unit recs exist in new contract
            if (Units.GridContractUnits.xGrid.Records.Count > 0)
            {
                //sum unit totals
                Units.sumUnitTotals();
            }
            else
            {
                //clear any unit totals on Units tab when entity changes
                Units.txtUnitTotal.Text = "";
            }
            /////////////////////////////////////////////////////////////////////////////////////////////

            //KSH - 10/1/12 highlights current billing period in the rates grid on the rate tab
            Rates.HighLightCurrentBillPeriod();
            ///////////////////////////////////////////////////////////////////////////////////

            //KSH - 10/26/12 clear batch selections when contract changes
            this.LocRules.clearBatchSelections();
            ////////////////////////////////////////////////////////////
        }

        /// <summary>
        /// Adds the contract description to the tab name
        /// </summary>
        private void AddDescriptionToHeader()
        {
            if (this.CurrentBusObj.ObjectData != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count>0)
                WindowCaption = WindowCaption + "-" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_description"];
        }

        private void SetHeader()
        {
            AddDescriptionToHeader();
            ContentPane p = (ContentPane)this.Parent;

            if(p!=null)
                p.Header = WindowCaption ;
        }

        /// <summary>
        /// sets screen state for init
        /// </summary>
        private void setInitScreenState()
        {
            //don't allow edits on general tab fields
            this.General.IsEnabled = false;
            this.Billing.IsEnabled = false;
            this.Locations.IsEnabled = false;
            this.Rates.IsEnabled = false;
            this.Reporting.IsEnabled = false;
            this.BillingTotals.IsEnabled = false;
            this.ReviewDates.IsEnabled = false;
            this.Comments.IsEnabled = false;
            txtContractID.IsEnabled = true;                   
        }

        /// <summary>
        /// sets screen state for insert
        /// </summary>
        private void setInsertScreenState()
        {
            //go to general tab
            General.Focus();
            //set focus to first field
            this.General.txtBillMsoId.Focus();
            //don't allow edits on general tab fields
            this.General.IsEnabled = true;
            this.Billing.IsEnabled = false;
            this.Locations.IsEnabled = false;
            this.Rates.IsEnabled = false;
            this.Reporting.IsEnabled = false;
            this.BillingTotals.IsEnabled = false;
            this.ReviewDates.IsEnabled = false;
            this.Comments.IsEnabled = false;
            //don't allow header edits
            txtContractID.IsEnabled = false;
        }

        /// <summary>
        /// sets screen state for edit
        /// </summary>
        private void setEditScreenState()
        {
            //allow edits
            this.General.IsEnabled = true;
            this.Billing.IsEnabled = true;
            this.Locations.IsEnabled = true;
            this.Rates.IsEnabled = true;
            this.Reporting.IsEnabled = true;
            this.BillingTotals.IsEnabled = true;
            this.ReviewDates.IsEnabled = true;
            this.Comments.IsEnabled = true;
            txtContractID.IsEnabled = true;
        }

        private void txtContractID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //If the contract ID changed then load a new contract
            //See if Contract changed
            ReturnData();
        }

        private void txtContractID_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //foreach (DataRow r in ObjectData.Tables["changeable_objects"].Rows)
            //{
            //    if (ObjectData.Tables[r["table_name"].ToString()].GetChanges() != null)
            //        MessageBox(ObjectData.Tables[r["table_name"].ToString()]);
            //}

            ContractID = txtContractID.Text;
            //DataTable xDataTable = this.CurrentBusObj.ObjectData.Tables["general"].GetChanges();
            //int changes = 0;
            //if (this.CurrentBusObj.ObjectData.Tables["general"].) changes = 1;
            //this.CurrentBusObj.ObjectData.AcceptChanges();

  

        }

        private void txtContractID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on contract ID field
            ContractLookup f = new ContractLookup();
            f.Init(new cBaseBusObject("ContractLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtContractID.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                ReturnData();

                //DWR - 1/15/13 Removed the below to make the retrieve run through the ReturnData method so that checking for saves would work properly.
                //// now we can load the account
                //this.CurrentBusObj.Parms.ClearParms();
                ////this.CurrentBusObj.Parms.AddParm("@contract_id", cGlobals.ReturnParms[0].ToString());
                //loadParms(cGlobals.ReturnParms[0].ToString());
                //txtContractID.Text = cGlobals.ReturnParms[0].ToString();
                ////KSH - 8/21/12 clear comments/attachments grid to fix bug
                //clrCommentsAttachmentsObj();
                //// Call load 
                //this.Load();
                ////KSH - 10/26/12 set tabs screen state when contract changes
                //setTabScreenStateOnLoad();
                //setEditScreenState();
                ////HeaderName = "Contract-" + txtContractID.Text;
                //WindowCaption = "Contract-" + txtContractID.Text;
                //ContractID = txtContractID.Text;
                //SetHeader();
                //// Clear the parms
                //cGlobals.ReturnParms.Clear();
            }
          
        }

        /// <summary>
        /// KSH - 8/21/12 clear comments/attachments grid to fix bug
        /// </summary>
        private void clrCommentsAttachmentsObj()
        {
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();
        }

        public bool chkCOLA()
        {
            General.txtCOLARuleOverride.IsChecked = 0;
            foreach (DataRow s in CurrentBusObj.ObjectData.Tables["rules"].Rows)
            {
                if (Convert.ToInt32(s["rule_override"]) > 0)
                {
                    General.txtCOLARuleOverride.IsChecked = 1;
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// had to override in order to busobj not to be set to null when save failed
        /// </summary>
        public override void Close()
        {
            dirtyBubbleUp();
            SaveSuccessful = true;
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (this.CurrentBusObj != null && (IsScreenDirty || ForceScreenDirty)
                && (this.SecurityContext == AccessLevel.ViewUpdate || this.SecurityContext == AccessLevel.ViewUpdateDelete))
            {
                var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                {
                    this.Save();
                    //@@Need to add code here to stop the window from closing if save fails
                    StopCloseIfCancelCloseOnSaveConfirmationTrue = true;
                }
            }
            //Set the business object to null so that we do not receive any false trues on future app close checks
            //if (this.CurrentBusObj != null && this.CurrentBusObj.ObjectData != null)
            //    this.CurrentBusObj.ObjectData = null;
        }

        /// <summary>
        /// if ForceScreenDirty is set on the general tab then this bubbles it up to the main screen, otherwise will have no affect on the close logic
        /// </summary>
        private void dirtyBubbleUp()
        {
            if (General.ForceScreenDirty == true)
                this.ForceScreenDirty = true;
        }

        private void clrUnits()
        {
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                {
                    this.Units.txtUnitTotal.Text = "0";
                    this.Units.txtServiceDateStart.SelText = Convert.ToDateTime("01/01/1900");
                    this.Units.txtServiceDateEnd.SelText = Convert.ToDateTime("01/01/1900");

                    this.CurrentBusObj.changeParm("@service_period_start", this.Units.txtServiceDateStart.SelText.ToString());
                    this.CurrentBusObj.changeParm("@service_period_end", this.Units.txtServiceDateStart.SelText.ToString());
                }
        }
    }
}