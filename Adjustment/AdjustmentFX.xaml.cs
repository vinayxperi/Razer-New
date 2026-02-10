using RazerInterface; //Required for IPreBindable
using RazerBase.Interfaces; //Required for IScreen
using RazerBase;
using RazerBase.Lookups;
using System;
using System.Windows;
using System.Data;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;

using System.Windows.Input;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentFX' object.
    /// </summary>
    public partial class AdjustmentFX : ScreenBase
    {
        public cBaseBusObject FXAdjustmentBusObj = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        private bool IsSingleClickOrigin { get; set; }
        //public ComboBoxItemsProvider cmbProductGridCombo { get; set; }
        //private string DocumentId { get; set; }
        public string CustomerNum { get; set; }
        private cBaseBusObject customerObject;
        private static readonly string receivableAcctObject = "ManInvRecAcct";
        double MaxTotalAdjusted = 0;
        Int32 MaxIndex = 0;
        //private static readonly string receivableAcctTableName = "recv_acct";
        //private static readonly string receivableAcctParameter = "@receivable_account";

        /// <summary>
        /// Create a new instance of a 'AdjustmentFX' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentFX(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = FXAdjustmentBusObj;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentFX";
            //set Adj Folder screen obj
            AdjFolderScreen = _AdjFolderScreen;
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
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;
            this.MainTableName = "main";

            //FieldLayoutSettings f = new FieldLayoutSettings();
            //f.AllowAddNew = false;
            //f.AllowDelete = true;
            ////f.AddNewRecordLocation = AddNewRecordLocation.OnTop;
            //f.SelectionTypeField = SelectionType.Single;
            //f.HighlightAlternateRecords = true;
            //loadParms();
            this.CurrentBusObj.Parms.ClearParms();
            this.CurrentBusObj.Parms.AddParm("@receivable_account", ""); 
            //setup adj grid
            //GridAdjustments.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(GridSource_EditModeEnded);
            //GridAdjustments.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridAdjustments.DoNotSelectFirstRecordOnLoad = true;
            //GridAdjustments.SkipReadOnlyCellsOnTab = true;
            GridAdjustments.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridAdjustments.ContextMenuAddIsVisible = false;
            GridAdjustments.ContextMenuRemoveIsVisible = false;
            GridAdjustments.WindowZoomDelegate = DoubleClickZoomDelegateSearch;
            //GridAdjustments.SingleClickZoomDelegate = SingleClickZoomDelegateSource;
            GridAdjustments.MainTableName = "main";
            GridAdjustments.ConfigFileName = "AdjustmentGridFXSource";
            GridAdjustments.SetGridSelectionBehavior(true, false);
            GridAdjustments.xGrid.FieldSettings.AllowEdit = false;
            GridAdjustments.FieldLayoutResourceString = "GridFXSource";
            GridCollection.Add(GridAdjustments);  
                              
            //GridAdjustmentsApplied.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(GridDestination_EditModeEnded);
            //GridAdjustmentsApplied.WindowZoomDelegate = GridCashDestinationDoubleClick; //The function delegate determines what happens when the user double clicks the grid
  
            //GridAdjustmentsApplied.SkipReadOnlyCellsOnTab = true;
            //GridAdjustmentsApplied.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridAdjustmentsApplied.ContextMenuAddIsVisible = false;
            GridAdjustmentsApplied.ContextMenuRemoveIsVisible = false;
            GridAdjustmentsApplied.MainTableName = "destination";
            GridAdjustmentsApplied.ConfigFileName = "AdjustmentGridFXDestination";
            GridAdjustmentsApplied.SetGridSelectionBehavior(false, true);
            GridAdjustmentsApplied.FieldLayoutResourceString = "GridFXDestination";
            //GridAdjustmentsApplied.xGrid.FieldLayoutSettings = f;
            //GridAdjustmentsApplied.ContextMenuRemoveDelegate = GridAdjustmentsAppliedRemoveDelegate;
            //GridAdjustmentsApplied.ContextMenuRemoveDisplayName = "Remove Adjustment";
            GridAdjustmentsApplied.xGrid.FieldSettings.AllowEdit = false;
            GridAdjustmentsApplied.xGrid.FieldLayoutSettings.AllowDelete = false;
            GridAdjustmentsApplied.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            
            GridCollection.Add(GridAdjustmentsApplied);         

            this.Load();
            //set initial screen state
            //txtAdjAmt.IsReadOnly = true;
            setInitScreenState();
            txtCustomerAccount.CntrlFocus();

        }

        void GridSource_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            //commit user entered value to datatable
            //GridAdjustments.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);

            //change amount to apply to negative if it was entered as positive
            //double USDTotal = 0.00;
            //foreach (DataRecord r in GridAdjustments.xGrid.Records)
            //{
            //   if (Convert.ToDouble(r.Cells["to_currency_amt"].Value.ToString()) > 0)
            //       r.Cells["to_currency_amt"].Value = Convert.ToDouble(r.Cells["to_currency_amt"].Value.ToString()) * -1;
            //   USDTotal = USDTotal + Convert.ToDouble(r.Cells["to_currency_amt"].Value.ToString());    
            //}
            //txtCashConverted.Text = USDTotal.ToString("0.00");
            //txtCashConverted.TextColor = "Black";
            //double FXAmount = 0.00;
            //FXAmount = USDTotal - Convert.ToDouble(txtCashConverted.Text);
            //txtAdjAmt.Text = FXAmount.ToString("0.00");
            ////if less than zero turn red otherwise black/////////////////////////////////////////////////
            //if (Convert.ToDouble(txtAdjAmt.Text) < 0) txtAdjAmt.TextColor = "Red";
            //else txtAdjAmt.TextColor = "Black";
            ///////////////////////////////////////////////////////////////////////////////////////////////
            ////if amounts offset then enable save button
            //txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));

            //if (FXAmount != 0) btnSave.IsEnabled = true;
        }

        /// <summary>
        /// works as cellLeave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GridDestination_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = GridAdjustmentsApplied.ActiveRecord.Cells.IndexOf(e.Cell);           
            //commit user entered value to datatable
            GridAdjustmentsApplied.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            DataRecord GridRecord = null;
            GridRecord = GridAdjustmentsApplied.ActiveRecord;
            DataRowView dr = GridRecord.DataItem as DataRowView;
            DataView dv = dr.DataView;
            if (GridRecord != null)
            {
                //clear running total
                //txtRunningOffsetAmt.Text = "";
                //init AdjTotal w/starting value
                double AdjTotal = 0.00;
                if (GridRecord.Cells["to_currency_amt"].Value.ToString() == "")
                {
                    GridRecord.Cells["to_currency_amt"].Value = 0.00;
                }
                //loop through each grid record and add adj totals
                foreach (DataRecord r in GridAdjustmentsApplied.xGrid.Records)
                {
                    //sum adj amts
                    AdjTotal += Convert.ToDouble(r.Cells["to_currency_amt"].Value);
                }
                txtCashConverted.Text = AdjTotal.ToString("0.00");
                //if less than zero turn red otherwise black/////////////////////////////////////////////////
                if (Convert.ToDouble(txtCashConverted.Text) < 0) txtAdjAmt.TextColor = "Red";
                else txtCashConverted.TextColor = "Black";
                /////////////////////////////////////////////////////////////////////////////////////////////
                //if amounts offset then enable save button
                txtCashConverted.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
                if (AmountsOffset()) btnSave.IsEnabled = true;
                //Populate company based on product_code selected for UA cash               
            }
        }

        //private void SingleClickZoomDelegateSource()
        //{
        //    //clear dest grid and reset alloc running total
        //    //clearDestGrid();
        //}
       
        private void Customer_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            cGlobals.ReturnParms.Clear();
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = EventAggregator.GeneratedClickEvent;
            args.Source = txtCustomerAccount;
            EventAggregator.GeneratedClickHandler(this, args);

            if (cGlobals.ReturnParms.Count > 0)
            {
                txtCustomerAccount.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                LoadCustomerInformation();
                txtCashPaid.CntrlFocus();
            }            
        }

        private void LoadCustomerInformation()
        {
            CustomerNum = txtCustomerAccount.Text;
            customerObject = new cBaseBusObject(receivableAcctObject);
            customerObject.Parms.AddParm("@receivable_account", txtCustomerAccount.Text);
            customerObject.LoadData();

            if (customerObject.ObjectData.Tables["recv_acct"].Rows.Count > 0)
            {
                DataRow row = customerObject.ObjectData.Tables["recv_acct"].Rows[0];
                txtCustomerName.Text = row["account_name"].ToString();
                txtCustomerName.IsEnabled = false;
                popGrid();
                this.PrepareFreeformForSave();
                setEditScreenState();
                GridAdjustments.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                //e.Handled = true;
            }
            else
            {
                Messages.ShowInformation("Customer Number entered is invalid.");
            }            
        }

        /// <summary>
        /// single click for seach grids adds selected record to destination grid
        /// </summary>
        public void DoubleClickZoomDelegateSearch()
        {            
            //set this to prevent gridDestination single click delegate from firing when GridCashDestination.xGrid.ActiveDataItem = row; runs
            IsSingleClickOrigin = true;
            System.Collections.Generic.List<string> LocationFieldList = new System.Collections.Generic.List<string>();
            LocationFieldList.Add("adjustment_number");
            LocationFieldList.Add("invoice_number");
            LocationFieldList.Add("total_adjusted");
            LocationFieldList.Add("acct_period");
            LocationFieldList.Add("product_code");
            LocationFieldList.Add("Razer_FX");
            LocationFieldList.Add("proforma_invoice_number");
            LocationFieldList.Add("proforma_amt");
            LocationFieldList.Add("proforma_currency_code");
            LocationFieldList.Add("conversion_date");
            LocationFieldList.Add("conversion_rate");
            LocationFieldList.Add("to_currency_amt");
            LocationFieldList.Add("amt_to_adjust");
            LocationFieldList.Add("company_code");
            LocationFieldList.Add("gl_center");
            LocationFieldList.Add("gl_acct");
            LocationFieldList.Add("geography");
            LocationFieldList.Add("gl_product");
            LocationFieldList.Add("interdivision");
            LocationFieldList.Add("fx_account");
            cGlobals.ReturnParms.Clear();
            GridAdjustments.ReturnSelectedData(LocationFieldList);
            if (cGlobals.ReturnParms.Count > 0)
            {
                //check to see if document has already been added
                if (this.CurrentBusObj.ObjectData.Tables["destination"].Rows.Count > 0)
                {
                    foreach (DataRecord r in GridAdjustmentsApplied.xGrid.Records)
                    {
                        if (r.Cells["adjustment_number"].Value.ToString() == cGlobals.ReturnParms[0])
                        {
                            Messages.ShowWarning("Adjustment " + r.Cells["adjustment_number"].Value.ToString() + " has already been added");
                            return;
                        }
                    }
                }
                GridAdjustmentsApplied.SetGridSelectionBehavior(true, false);
                GridAdjustments.xGrid.FieldSettings.AllowEdit = true;
                DataView dataSource = this.GridAdjustmentsApplied.xGrid.DataSource as DataView;
                //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
                //Add new grid row and set cursor in first cell of last row
                DataRowView row = dataSource.AddNew();
                GridAdjustmentsApplied.xGrid.ActiveDataItem = row;
                GridAdjustmentsApplied.xGrid.ActiveCell = (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];               
                //adjustment_number
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
                //invoice_number
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
                //total_adjusted
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
                //product_code"
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[4];
                //acct_period
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[3];                
                //Razer_FX
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
                //proforma_invoice_number
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
                //proforma_amount
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = cGlobals.ReturnParms[7];
                //proforma_currency_code
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = cGlobals.ReturnParms[8];
                //conversion_date
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[10];
                //conversion_rate
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[10].Value = cGlobals.ReturnParms[9];
                //to_currency_amt
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[11].Value = cGlobals.ReturnParms[11];
                //amt_to_adjust
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[12].Value = cGlobals.ReturnParms[12];
                //company_code
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[13].Value = cGlobals.ReturnParms[13];
                //gl_center
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[14].Value = cGlobals.ReturnParms[14];
                //gl_account
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[15].Value = cGlobals.ReturnParms[15];
                //geography
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[16].Value = cGlobals.ReturnParms[16];
                //gl_product
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[17].Value = cGlobals.ReturnParms[17];
                //interdivision
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[18].Value = cGlobals.ReturnParms[18];
                //fx_account
                (GridAdjustmentsApplied.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[19].Value = cGlobals.ReturnParms[19];   

                GridAdjustmentsApplied.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridAdjustmentsApplied.SetGridSelectionBehavior(false, false);
                GridAdjustmentsApplied.xGrid.FieldSettings.AllowEdit = false;
                IsSingleClickOrigin = false;
                TotalAmt();
            }
        }

        private void TotalAmt()
        {
            double USDTotal = 0.00;
            double AdjTotal = 0.00;
            double CashPaid = 0.00; 
            if (this.CurrentBusObj.ObjectData.Tables["destination"].Rows.Count > 0)
            {                 
                foreach (DataRecord r in GridAdjustmentsApplied.xGrid.Records)
                {  
                    USDTotal += Convert.ToDouble(r.Cells["to_currency_amt"].Value);
                    if (Convert.ToDouble(r.Cells["total_adjusted"].Value) > MaxTotalAdjusted) 
                    {
                        MaxTotalAdjusted = Convert.ToDouble(r.Cells["total_adjusted"].Value);
                        MaxIndex = r.DataItemIndex;
                    }
                }
                btnClear.IsEnabled = true;         
            }
            txtCashConverted.Text = USDTotal.ToString("0.00");
            //if less than zero turn red otherwise black/////////////////////////////////////////////////
            if (Convert.ToDouble(txtCashConverted.Text) < 0) txtCashConverted.TextColor = "Red";
            else txtCashConverted.TextColor = "Black";
            txtCashConverted.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtCashConverted.Text));

            string sUnformattedTextField = txtCashPaid.Text.ToString().Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");
            string sCashPaid = sUnformattedTextField;
            CashPaid = Convert.ToDouble(sCashPaid.Trim());
            AdjTotal = USDTotal - CashPaid;
            if (AdjTotal != 0) btnSave.IsEnabled = true;
            txtAdjAmt.Text = AdjTotal.ToString("0.00");
            //if less than zero turn red otherwise black/////////////////////////////////////////////////
            if (Convert.ToDouble(txtAdjAmt.Text) < 0) txtAdjAmt.TextColor = "Red";
            else txtAdjAmt.TextColor = "Black";
            txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
        }       

        private void txtCashPaid_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (sender is RazerBase.ucLabelTextBox)
            //    (sender as RazerBase.ucLabelTextBox).SelectAll();

            //if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }
            txtSelectAll(sender);
            txtCashPaid.Text = txtCashPaid.Text.Replace("$", "");
            txtCashPaid.Text = txtCashPaid.Text.Replace(",", "");
            txtCashPaid.Text = txtCashPaid.Text.Replace("(", "-");
            txtCashPaid.Text = txtCashPaid.Text.Replace(")", "");

        }

        private void txtSelectAll(object sender)
        {
            if (sender is RazerBase.ucLabelTextBox)
                (sender as RazerBase.ucLabelTextBox).SelectAll();
        }

        private void txtCashPaid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //if txt is not numeric////////////////////////////////////////////////////////////
            Double result = 0;
            if (Double.TryParse(txtCashPaid.Text, out result) == false)
            {
                txtCashPaid.Text = "0.00";
            }
            ///////////////////////////////////////////////////////////////////////////////////
            //if (AdjMaxAmt != null)
            //{
            //    //if (Convert.ToDouble(txtAdjAmt.Text) > AdjMaxAmt || Convert.ToDouble(txtAdjAmt.Text) < AdjMaxAmt * -1)
            //    if (Convert.ToDouble(txtAdjAmt.Text) > AdjMaxAmt || Convert.ToDouble(txtAdjAmt.Text) < AdjMaxAmt * -1)
            //    {
            //        Messages.ShowWarning("Cannot adjust more than the original invoice amount of " + AdjMaxAmt.ToString());                     
            //        return;
            //    }
            //}
            //set a default value if user skips
            if (txtCashPaid.Text == "") txtCashPaid.Text = "0.00";
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtCashPaid.Text);
            txtCashPaid.Text = formatAmt.ToString("0.00");
            txtCashPaid.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtCashPaid.Text));
            if (this.CurrentBusObj.ObjectData.Tables["destination"].Rows.Count > 0)
            {
                TotalAmt();
            }
            else
            {
                double AdjTotal = 0.00;
                txtAdjAmt.Text = AdjTotal.ToString("0.00");
                //if less than zero turn red otherwise black/////////////////////////////////////////////////
                if (Convert.ToDouble(txtAdjAmt.Text) < 0) txtAdjAmt.TextColor = "Red";
                else txtAdjAmt.TextColor = "Black";
                txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
            }
        }

        //public void SingleClickZoomDelegateSearch()
        //{
            
        //}

        /// <summary>
        ///do save logic 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Save();
        }

        public override void Save()
        {
            this.SaveSuccessful = false;
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
          
            if (Convert.ToDouble(sAdjAmt.Trim()) == 0)
            {
                Messages.ShowWarning("Adjustment Amount Cannot Be $0.00. Save Cancelled");
                return;
            }
            if (MaxTotalAdjusted == 0)
            {
                Messages.ShowWarning("No adjustments selected to be applied. Save Cancelled");
                return;
            }
            if (AmountsOffset())
            {
                string NewDocID = "";
                string NewAdjID = "";
                string SPErrMsg = "";                
                string ApplyToDoc = this.CurrentBusObj.ObjectData.Tables["destination"].Rows[MaxIndex]["invoice_number"].ToString();
                string ProductCode = this.CurrentBusObj.ObjectData.Tables["destination"].Rows[MaxIndex]["product_code"].ToString();
                string GLCo = this.CurrentBusObj.ObjectData.Tables["destination"].Rows[MaxIndex]["company_code"].ToString();
                if (GLCo == "0") GLCo = "00";
                string GLCostCtr = this.CurrentBusObj.ObjectData.Tables["destination"].Rows[MaxIndex]["gl_center"].ToString();
                if (GLCostCtr == "0") GLCostCtr = "0000";
                string GLAcct = this.CurrentBusObj.ObjectData.Tables["destination"].Rows[MaxIndex]["gl_acct"].ToString();
                if (GLAcct == "0") GLAcct = "00000";
                string GLRegion = this.CurrentBusObj.ObjectData.Tables["destination"].Rows[MaxIndex]["geography"].ToString();
                if (GLRegion == "0") GLRegion = "0000";
                string GLProduct = this.CurrentBusObj.ObjectData.Tables["destination"].Rows[MaxIndex]["gl_product"].ToString();
                if (GLProduct == "0") GLProduct = "0000";
                string GLIC = this.CurrentBusObj.ObjectData.Tables["destination"].Rows[MaxIndex]["interdivision"].ToString();
                if (GLIC == "0") GLIC = "00";
                string FXAcct = this.CurrentBusObj.ObjectData.Tables["destination"].Rows[MaxIndex]["fx_account"].ToString();
                string ErrMsg = "";

                if (Convert.ToDouble(sAdjAmt.Trim()) > 0)
                {
                    double GLAmt = Convert.ToDouble(sAdjAmt.Trim()) * -1;
                    string sGLAmt = GLAmt.ToString("0.00");

                    NewAdjID = cGlobals.BillService.InsertNewAdjustmentFXCredit(ApplyToDoc, txtCustomerAccount.Text, sGLAmt, ProductCode, cGlobals.UserName, "USD", GLCo, GLCostCtr, GLAcct, GLRegion, GLProduct, GLCo, GLCostCtr, FXAcct, GLProduct, GLRegion, GLIC, sAdjAmt, ref ErrMsg);
                    if (NewAdjID == "")
                    {
                        //error messages from Stored Proc
                        if (ErrMsg != "")
                        {
                            Messages.ShowError(ErrMsg);
                        }
                        else
                        {
                            //save failed with no error msg
                            Messages.ShowInformation("Data Error, Save Failed");
                        }
                    }
                    else
                    {
                        Messages.ShowInformation("Save Successful--New Adjustment ID = " + NewAdjID);
                        //pop new adjustment in folder
                        AdjFolderScreen.ReturnData(NewAdjID, "");
                        //add user name to AdjGeneralTab
                        AdjFolderScreen.AdjustmentGeneralTab.txtCreatedBy.Text = cGlobals.UserName.ToLower();
                        //close me
                        System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);
                        this.CurrentBusObj.ObjectData.AcceptChanges();
                        StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
                        this.SaveSuccessful = true;
                        //Saved = true;
                        if (!ScreenBaseIsClosing)
                        {
                            AdjParent.Close();
                        }
                    }
                }
                else
                {
                    double GLAmt = Convert.ToDouble(sAdjAmt.Trim()) * -1;
                    string sGLAmt = GLAmt.ToString("0.00");
                    double OffsetAmt = Convert.ToDouble(sAdjAmt.Trim());
                    string sOffsetAmt = OffsetAmt.ToString("0.00");
                    NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(ApplyToDoc, 13, cGlobals.UserName.ToLower(), Convert.ToDecimal(sGLAmt));
                    //insert the new doc id in all existing grid rows for ins sp
                    if (NewDocID == "")
                    {
                        Messages.ShowError("Problem Creating New Adjustment");
                        return;
                    }
                    
                    NewAdjID = cGlobals.BillService.InsertNewAdjustmentFXDebit(ApplyToDoc, "1", NewDocID, txtCustomerAccount.Text, sGLAmt, ProductCode, "USD", GLCo, GLCostCtr, GLAcct, GLRegion, GLIC, GLProduct, GLCo, GLCostCtr, FXAcct, GLProduct, GLRegion, GLIC, sOffsetAmt, ref ErrMsg);
                    if (NewAdjID == "")
                    {
                        //error messages from Stored Proc
                        if (ErrMsg != "")
                        {
                            Messages.ShowError(ErrMsg);
                        }
                        else
                        {
                            //save failed with no error msg
                            Messages.ShowInformation("Data Error, Save Failed");
                        }
                        rollbackAdj(NewDocID);
                    }
                    else
                    {
                        Messages.ShowInformation("Save Successful--New Adjustment ID = " + NewDocID);
                        //pop new adjustment in folder 
                        this.CurrentBusObj.Parms.UpdateParmValue("@document_id", NewDocID.ToString());
                        AdjFolderScreen.ReturnData(NewDocID, " ");
                        //add user name to AdjGeneralTab
                        AdjFolderScreen.AdjustmentGeneralTab.txtCreatedBy.Text = cGlobals.UserName.ToLower();
                        //close me
                        System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);
                        //this.CurrentBusObj.ObjectData.AcceptChanges();
                        StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
                        this.SaveSuccessful = true;
                        //Saved = true;
                        if (!ScreenBaseIsClosing)
                        {
                            AdjParent.Close();
                        }
                    }          
                }
            }
            else
            {
                Messages.ShowError("Amounts Do Not Offset. Save Cancelled");
            }
        }    


        /// <summary>
        /// enables adj amt and pop cash source grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCustomerAccount_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtCustomerAccount.Text != "")
            {
                if (CustomerNum != txtCustomerAccount.Text)
                {
                    LoadCustomerInformation();
                }
                //txtCashPaid.CntrlFocus();
            }
        }
          

         /// <summary>
        /// populates grid and enables adj amt field if grid rows > 0
        /// </summary>
        private void popGrid()
        {
            //load parms 
            //loadParms();
            this.CurrentBusObj.changeParm("@receivable_account", txtCustomerAccount.Text.Trim());
            //load the object
            this.Load();
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                //txtAdjAmt.IsReadOnly = false;
                setInitScreenState();
                GridAdjustments.SetGridSelectionBehavior(false, false);
                GridAdjustments.xGrid.FieldSettings.AllowEdit = true;
                GridAdjustments.IsEnabled = true;
                GridAdjustments.xGrid.Focus();
            }
            else
            {
                Messages.ShowInformation("No Billing Differential adjustments found for customer");
                txtAdjAmt.IsReadOnly = true;
                setInitScreenState();
                GridAdjustments.IsEnabled = false;
            }
        }

        /// <summary>
        /// load parms for data svc
        /// </summary>
        /// <param name="DoingRollback"></param>
        private void loadParms()
        {
            //clear parm            
            this.CurrentBusObj.Parms.ClearParms();
            this.CurrentBusObj.Parms.AddParm("@receivable_account", "");         
        }

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            btnSave.IsEnabled = false;
            GridAdjustments.IsEnabled = true;          
            GridAdjustmentsApplied.IsEnabled = false;     
            BorderDestination.IsEnabled = false;
            BorderSource.IsEnabled = true;        
            txtCashConverted.Text = "0.00";
            txtCashPaid.Text = "0.00";
            txtAdjAmt.Text = "0.00";         
        }

        /// <summary>
        /// frees up objects for edit
        /// </summary>
        private void setEditScreenState()
        {
            GridAdjustments.IsEnabled = true;
            GridAdjustmentsApplied.IsEnabled = true;
            //GridSearch.IsEnabled = true;
            //btnClearDest.IsEnabled = true;
            //chkApplyUACash.IsEnabled = true;
            BorderDestination.IsEnabled = true;
            //BorderSearch.IsEnabled = true;
            BorderSource.IsEnabled = true;
           
            if (this.CurrentBusObj.ObjectData != null)
            {
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
                {
                    //(GridAdjustments.xGrid.Records[GridAdjustments.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsActive = true;
                    //GridAdjustments.xGrid.Records.DataPresenter.BringCellIntoView(GridAdjustments.xGrid.ActiveCell);
                    //GridAdjustments.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);                   
                }
                //txtCashConverted.TextColor = "Black";
                txtCashConverted.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtCashConverted.Text));
                txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
            }

         }

        /// <summary>
        /// check that amounts are offsetting
        /// </summary>
        /// <returns></returns>
        private bool AmountsOffset()
        {
            //check for nulls
            if (txtCashPaid.Text == null)
                return false;
            if (txtAdjAmt.Text == null)
                return false;
            if (txtCashConverted.Text == null)
                return false;
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sCashPaid = UnformatTextField(txtCashPaid.Text.Trim());
            string sCashConverted = UnformatTextField(txtCashConverted.Text.Trim());
            //loop through each grid record and add adj totals
            double ApplyTotal = 0;
            foreach (DataRecord r in GridAdjustmentsApplied.xGrid.Records)
            {
                //sum adj amts
                ApplyTotal += Convert.ToDouble(r.Cells["to_currency_amt"].Value);
            }
            if (Convert.ToDouble(sCashPaid.Trim()) != 0 && Convert.ToDouble(sAdjAmt.Trim()) != 0 && Convert.ToDouble(sCashConverted.Trim()) != 0)
            {                
                btnSave.IsEnabled = true;   
                return true;
            }
            else
            {
                btnSave.IsEnabled = false;
                return false;
            }
        }

         private bool GetLargestDoc()
        {
            //check for nulls
            if (txtCashPaid.Text == null)
                return false;
            if (txtAdjAmt.Text == null)
                return false;
            if (txtCashConverted.Text == null)
                return false;
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sCashPaid = UnformatTextField(txtCashPaid.Text.Trim());
            string sCashConverted = UnformatTextField(txtCashConverted.Text.Trim());
            //loop through each grid record and add adj totals
            double ApplyTotal = 0;
            foreach (DataRecord r in GridAdjustmentsApplied.xGrid.Records)
            {
                //sum adj amts
                ApplyTotal += Convert.ToDouble(r.Cells["to_currency_amt"].Value);
            }
            //if (Convert.ToDouble(txtRunningAdjAmt.Text.Trim()) == Convert.ToDouble(txtAdjAmt.Text.Trim()) && (Convert.ToDouble(txtAdjAmt.Text.Trim()) + Convert.ToDouble(txtRunningOffsetAmt.Text.Trim())) == 0)
            if (Convert.ToDouble(sCashPaid.Trim()) != 0 && Convert.ToDouble(sAdjAmt.Trim()) != 0 && Convert.ToDouble(sCashConverted.Trim()) != 0)
            {                
                btnSave.IsEnabled = true;   
                return true;
            }
            else
            {
                btnSave.IsEnabled = false;
                return false;
            }
        }             

        /// <summary>
        /// clears destination grid
        /// </summary>
        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //clear dest grid & set state
            clearDestGrid();
        }

        /// <summary>
        /// clears destination grid
        /// </summary>
        private void clearDestGrid()
        {
            //clear grid & set state
            this.CurrentBusObj.LoadData("destination");
            GridAdjustmentsApplied.LoadGrid(this.CurrentBusObj, "destination");
            //txtCashConverted.Text = "0.00";
            //chkApplyUACash.IsChecked = 0;
            btnClear.IsEnabled = false;
            btnSave.IsEnabled = false;
            txtCashConverted.Text = "0.00";
            txtAdjAmt.Text = "0.00";
            txtAdjAmt.TextColor = "Black";
            MaxTotalAdjusted = 0;
            MaxIndex = 0;          

        }

        /// <summary>
        /// call SelectAll method on textbox portion of ucLabelTextbox, highlights text for user
        /// </summary>
        /// <param name="sender"></param>
        private void txtAdjAmt_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is RazerBase.ucLabelTextBox)
                (sender as RazerBase.ucLabelTextBox).SelectAll();

            if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }

            txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");
        }

        private string UnformatTextField(string FormattedTextField)
        {
            if (FormattedTextField == null || FormattedTextField == "") return "0";

            string sUnformattedTextField = FormattedTextField.Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");

            return sUnformattedTextField;
        }

        private void rollbackAdj(string NewDocID)
        {
            //delete adj header
            bool retVal = cGlobals.BillService.DeleteNewAdjusmentPreamble(NewDocID);
        }

       
        /// <summary>
        /// This event is the WindowZoomDelegate for the GridCashDestination grid
        /// It will handle any double click lookups or zooms for the grid.
        /// For this grid it will launch the receivable account lookup based
        /// on which field was double clicked
        /// </summary>
        //public void GridCashDestinationDoubleClick()
        //{
           
        //    //Receivable account lookup opens the standard customer lookup screen.
        //    //if (GridCashDestination.xGrid.ActiveCell != null && GridCashDestination.xGrid.ActiveCell.Field.Name == "receivable_account")
        //    //{
        //    //    cGlobals.ReturnParms.Clear();
        //    //    RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
        //    //    args.Source = GridCashDestination.xGrid;
        //    //    EventAggregator.GeneratedClickHandler(this, args);
        //    //    RazerBase.Lookups.CustomerLookup custLookup = new RazerBase.Lookups.CustomerLookup();
        //    //    custLookup.Init(new cBaseBusObject("CustomerLookup"));

        //    //    //this.CurrentBusObj.Parms.ClearParms();

        //    //    // gets the users response
        //    //    custLookup.ShowDialog();

        //    //    if (cGlobals.ReturnParms.Count > 0)
        //    //    {
        //    //        GridCashDestination.ActiveRecord.Cells["receivable_account"].Value = cGlobals.ReturnParms[0].ToString();
        //    //    }
        //    //}
        //    //cGlobals.ReturnParms.Clear();
        //     //Receivable account lookup opens the standard customer lookup screen.
        //    if (GridCashDestination.xGrid.ActiveCell != null && GridCashDestination.xGrid.ActiveCell.Field.Name == "receivable_account")
        //    {
        //        cGlobals.ReturnParms.Clear();
        //        RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
        //        args.Source = txtDocument;
        //        EventAggregator.GeneratedClickHandler(this, args);

        //        if (cGlobals.ReturnParms.Count > 0)
        //        {
        //            GridCashDestination.ActiveRecord.Cells["receivable_account"].Value = cGlobals.ReturnParms[0].ToString();
        //        }
        //    }
        //    cGlobals.ReturnParms.Clear();
        //    System.Collections.Generic.List<string> LocationFieldList = new System.Collections.Generic.List<string>();
        //    LocationFieldList.Add("company_code");
        //    LocationFieldList.Add("receivable_account");
        //    LocationFieldList.Add("open_amount");
        //    LocationFieldList.Add("amount");
        //    GridCashSource.ReturnSelectedData(LocationFieldList);
        //}    
    }
}

