using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter;
using RazerBase.Lookups;
using System.Windows.Input;
using System.ComponentModel;
using Infragistics.Windows.DataPresenter.Events;
using System.Windows;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentApplyUACash' object.
    /// </summary>
    public partial class AdjustmentCreditDebit : ScreenBase, IPreBindable
    {
        public cBaseBusObject CreditDebitAdjustmentBusObj = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        public Int32 AdjType;
        private bool IsSingleClickOrigin { get; set; }
        //public ComboBoxItemsProvider cmbProductGridCombo { get; set; }
        private string DocumentId { get; set; }
        //This datatable is being added so that the Amount to adjust text box can have a binding
        //Do this for fields that contain informational data but that will not be saved
        //so that you can use converters or other benefits of binding
        DataTable dtMiscInfo = new DataTable("MiscInfo");
        decimal AdjTotal = 0.00M;
        decimal ProdAdjTotal = 0.00M;
        MessageBoxResult result1;
        MessageBoxResult result2;
        bool MessageDisplayed = false;
        bool ApplyCashFlag = false;
        /// <summary>
        /// Create a new instance of a 'AdjustmentApplyUACash' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentCreditDebit(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = CreditDebitAdjustmentBusObj;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentCreditDebit";
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
            loadParms();
            //set initial screen state
            //txtAdjAmt.IsReadOnly = true;
            setInitScreenState();
            txtDocument.CntrlFocus();

            //Adding fields to the MiscInfo datatable - Fields represent fields that are not bound through
            //the base business object and are not needed for the database
            dtMiscInfo.Columns.Add("amt_to_adjust");
            dtMiscInfo.Columns.Add("amt_adjusted");
            //This statement adds one row that will be used to hold the data.
            dtMiscInfo.Rows.Add("0", "0");
            //Bind the screen field(s) to the datatable
            //txtAdjAmt.DataContext = dtMiscInfo;
            //txtAdjAmt.DataContext = dtMiscInfo;
            //txtRunningAdjAmt.DataContext = dtMiscInfo;
                       
        }
        private void SetParentChildAttributes()
        {
       
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            GridCreditDebitSource.xGrid.FieldLayoutSettings = f;
            GridCreditDebitDestination.xGrid.FieldLayoutSettings = f;
            GridCreditDebitSource.MainTableName = "main";
            GridCreditDebitSource.ConfigFileName = "AdjustmentCreditDebitSource";
            GridCreditDebitSource.FieldLayoutResourceString = "GridCreditDebitSource";
            //Set the grid to allow edits, for readonly columns set the allowedit to false in the field layouts file
            GridCreditDebitSource.xGrid.FieldSettings.AllowEdit = false;
            GridCreditDebitSource.SetGridSelectionBehavior(true, false);
            GridCreditDebitSource.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "product_code" }, ChildGrids = { GridCreditDebitDestination }, ParentFilterOnColumnNames = { "product_code" } });


            //setup attributes for Child
            GridCreditDebitDestination.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            //GridCreditDebitDestination.xGrid.RecordActivated += new EventHandler<Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs>(xGrid_RecordActivated);
            //GridCreditDebitDestination.xGrid.CellActivating += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellActivatingEventArgs>(xGrid_CellActivating);
            //GridCreditDebitDestination.xGrid.CellUpdated += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs>(xGrid_CellUpdated);
            GridCreditDebitDestination.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridCreditDebitDestination.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridCreditDebitDestination.MainTableName = "detail";
            GridCreditDebitDestination.ConfigFileName = "AdjustmentGridCreditDebitDestination";
            GridCreditDebitDestination.FieldLayoutResourceString = "GridCreditDebitDestination";
            GridCreditDebitDestination.SetGridSelectionBehavior(false, false);
            GridCreditDebitDestination.xGrid.FieldSettings.AllowEdit = true;
            GridCreditDebitDestination.IsEnabled = false;
            //RES 6/12/15 Take out next statement to enable filter functionality
            //GridCreditDebitDestination.SkipReadOnlyCellsOnTab = true;
            
            GridCreditDebitDestination.CellUpdatedDelegate = GridDestination_CellUpdated;
            GridCollection.Add(GridCreditDebitSource);
            GridCollection.Add(GridCreditDebitDestination);
        }

        void x_KeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// works as cellLeave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GridDestination_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = GridCreditDebitDestination.ActiveRecord.Cells.IndexOf(e.Cell);

            //int row_index = GridAdjustment.ActiveRecord.Index;

            //WARNING: You may need to change this if the order of the AdjustmentFieldLayouts\GridConversionInvoice changes
            //if (edit_index == 7) //Amount To Adjust Field
            //{
            //commit user entered value to datatable
            GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            DataRecord GridRecord = null;
            GridRecord = GridCreditDebitDestination.ActiveRecord;
            DataRowView dr = GridRecord.DataItem as DataRowView;
            DataView dv = dr.DataView;
            if (GridRecord != null)
            {
                //clear running total
                //txtRunningOffsetAmt.Text = "";
                //init AdjTotal w/starting value
                decimal AdjTotal = 0.00M;
                if (GridRecord.Cells["amount_adjusted"].Value.ToString() == "")
                {
                    GridRecord.Cells["amount_adjusted"].Value = 0.00;
                }
                //if (GridRecord.Cells["rebill"].Value.ToString() == "1")
                //{
                //    GridRecord.Cells["amount_adjusted"].Value = GridRecord.Cells["extended"].Value;
                //}
                //loop through each grid record and add adj totals
                foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
                {
                    //sum adj amts
                    AdjTotal += Convert.ToDecimal(r.Cells["amount_adjusted"].Value);
                }
                //if amounts offset then enable save button
                //txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
                if (AmountsOffset()) 
                {
                    btnSave.IsEnabled = true;
                    if (AdjTotal > 0)
                        btnApplyCash.IsEnabled = true;
                }
                else
                {
                    btnSave.IsEnabled = false;
                    btnApplyCash.IsEnabled = false;
                }
            }
            //}
        }

        private void SingleClickZoomDelegateSource()
        {
            //clear dest grid and reset alloc running total
            clearDestGrid();
        }

        /// <summary>
        /// single click for seach grids adds selected record to destination grid
        /// </summary>
        public void DoubleClickZoomDelegateSearch()
        {
            //set this to prevent gridDestination single click delegate from firing when GridCreditDebitDestination.xGrid.ActiveDataItem = row; runs
            IsSingleClickOrigin = true;
            System.Collections.Generic.List<string> LocationFieldList = new System.Collections.Generic.List<string>();
            LocationFieldList.Add("document_id");
            LocationFieldList.Add("seq_code");
            LocationFieldList.Add("company_code");
            LocationFieldList.Add("product_code");
            LocationFieldList.Add("receivable_account");
            LocationFieldList.Add("open_amount");
            LocationFieldList.Add("amount");
            LocationFieldList.Add("currency_code");
            LocationFieldList.Add("exchange_rate");
            cGlobals.ReturnParms.Clear();
            //GridSearch.ReturnSelectedData(LocationFieldList);
            if (cGlobals.ReturnParms.Count > 0)
            {
                GridCreditDebitDestination.SetGridSelectionBehavior(true, false);
                //GridSearch.xGrid.FieldSettings.AllowEdit = true;
                DataView dataSource = this.GridCreditDebitDestination.xGrid.DataSource as DataView;
                //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
                //Add new grid row and set cursor in first cell of last row
                DataRowView row = dataSource.AddNew();
                GridCreditDebitDestination.xGrid.ActiveDataItem = row;
                GridCreditDebitDestination.xGrid.ActiveCell = (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
                //document_id
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
                //seq_code
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
                //company_code
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
                //product_code
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[3];
                //receivable_account
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[4];
                //open_amount
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
                //amount
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
                //Amt to Adj
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = 0.00;
                //currency_code
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = cGlobals.ReturnParms[7];
                //exchange_rate
                (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[8];

                GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridCreditDebitDestination.SetGridSelectionBehavior(false, false);
                GridCreditDebitDestination.xGrid.FieldSettings.AllowEdit = true;
                IsSingleClickOrigin = false;
            }
        }

        public void SingleClickZoomDelegateSearch()
        {
            ////set this to prevent gridDestination single click delegate from firing when GridCreditDebitDestination.xGrid.ActiveDataItem = row; runs
            //IsSingleClickOrigin = true;
            //System.Collections.Generic.List<string> LocationFieldList = new System.Collections.Generic.List<string>();
            //LocationFieldList.Add("document_id");
            //LocationFieldList.Add("seq_code");
            //LocationFieldList.Add("company_code");
            //LocationFieldList.Add("product_code");
            //LocationFieldList.Add("receivable_account");
            //LocationFieldList.Add("open_amount");
            //LocationFieldList.Add("amount");
            //LocationFieldList.Add("currency_code");
            //LocationFieldList.Add("exchange_rate");
            //cGlobals.ReturnParms.Clear();
            //GridSearch.ReturnSelectedData(LocationFieldList);
            //if (cGlobals.ReturnParms.Count > 0)
            //{
            //    GridCreditDebitDestination.SetGridSelectionBehavior(true, false);
            //    GridSearch.xGrid.FieldSettings.AllowEdit = true;
            //    DataView dataSource = this.GridCreditDebitDestination.xGrid.DataSource as DataView;
            //    //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
            //    //Add new grid row and set cursor in first cell of last row
            //    DataRowView row = dataSource.AddNew();
            //    GridCreditDebitDestination.xGrid.ActiveDataItem = row;
            //    GridCreditDebitDestination.xGrid.ActiveCell = (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
            //    //document_id
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
            //    //seq_code
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
            //    //company_code
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
            //    //product_code
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[3];
            //    //receivable_account
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[4];
            //    //open_amount
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
            //    //amount
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
            //    //Amt to Adj
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = 0.00;
            //    //currency_code
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = cGlobals.ReturnParms[7];
            //    //exchange_rate
            //    (GridCreditDebitDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[8];

            //    GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //    GridCreditDebitDestination.SetGridSelectionBehavior(false, false);
            //    GridCreditDebitDestination.xGrid.FieldSettings.AllowEdit = true;
            //    IsSingleClickOrigin = false;
            //}
        }

        /// <summary>
        ///do save logic 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Save();
        }

        //RES 4/12/19 add apply cash option to debit adjustment
        private void btnApplyCash_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["deferred_flag"]) > 0)
            {
                Messages.ShowInformation("Cannot debit and apply cash to a deferred document!");                
                return;
            }      
                string documentID = txtDocument.Text;
                ApplyCashFlag = true;
                ApplyCashSave();
                if (!ApplyCashFlag) return;
                //instance location service screen
                AdjustmentDebitApplyCash DebitApplyCashScreen = new AdjustmentDebitApplyCash(documentID, this.CurrentBusObj, AdjFolderScreen);
                //create a new window, show it as a dialog
                System.Windows.Window DebitApplyCashWindow = new System.Windows.Window();
                //set new window properties///////////////////////////
                DebitApplyCashWindow.Title = "Debit Apply Unapplied Cash Adjustment Screen";
                DebitApplyCashWindow.MaxHeight = 1000;
                DebitApplyCashWindow.MaxWidth = 1050;
                //DebitApplyCashWindow.WindowState = WindowState.Maximized;
                //set screen as content of new window
                DebitApplyCashWindow.Content = DebitApplyCashScreen;
                //open new window with embedded user control
                DebitApplyCashWindow.ShowDialog();
                
                //close me
                System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);
                this.CurrentBusObj.ObjectData.AcceptChanges();
                StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
                this.SaveSuccessful = true;
                if (!ScreenBaseIsClosing)
                {
                    AdjParent.Close();
                }

        }

        private void ApplyCashSave()
        {
            this.SaveSuccessful = false;
            //make sure offsetting amts are not 0
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());

            if (Convert.ToDouble(sRunningAdjAmt.Trim()) != Convert.ToDouble(sAdjAmt.Trim()))
            {
                Messages.ShowWarning("Amount to Adjust does not equall Adjusted Amount. Unable to Continue");
                ApplyCashFlag = false;
                return;
            }
            //Loop through grid checking for any normalized lines that have been adjusted
            //RES 7/9/12 added allow_crdb indicator for adjustments to normalized lines
            bool NormalizedError = false;
            bool DefPoolError = false;
          
            //RES 8/7/18 do not allow rebill when creating a debit/apply cash adjustment
            MessageDisplayed = false;
            foreach (DataRecord r1 in GridCreditDebitDestination.xGrid.Records)
            {
                if (r1.Cells["rebill_flag"].Value.ToString() == "1")
                {
                    Messages.ShowError("Cannot rebill when applying cash with debit adjustment.");
                    ApplyCashFlag = false;
                    return;
                }  
            }
  
            int apply_to_seq = 0;
            string receivable_account = "";
            int adjustment_type = AdjType;
            string currency_code = "USD";
            string company_code = "";
            string gl_center = "";
            string gl_acct = "";
            string geography = "";
            string interdivision = "";
            string gl_product = "";
            DataRecord r2 = GridCreditDebitSource.ActiveRecord;
            DataRow row = (r2.DataItem as DataRowView).Row;
            if (row != null)
            {
                apply_to_seq = Convert.ToInt32(row["seq_code"]);
                receivable_account = Convert.ToString(row["receivable_account"]);
                //if (Convert.ToDouble(sAdjAmt) > 0) adjustment_type = 1;
                currency_code = Convert.ToString(row["currency_code"]);
                company_code = Convert.ToString(row["company_code"]);
                gl_center = Convert.ToString(row["gl_center"]);
                gl_acct = Convert.ToString(row["gl_acct"]);
                geography = Convert.ToString(row["geography"]);
                interdivision = Convert.ToString(row["interdivision"]);
                gl_product = Convert.ToString(row["gl_product"]);
            }

            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["detail"].Rows)
            {
                //bool rowIsNotModified = (dr.RowState != DataRowState.Modified);
                bool rowIsNotModified = true;
                if (Convert.ToDecimal(dr["amount_adjusted"]) != 0)
                    rowIsNotModified = false;
                //dr["adj_document_id"] = DocID;
                //dr["apply_to_seq"] = apply_to_seq; tas 12.10.13 commented out as now supplied by SP.
                dr["receivable_account"] = receivable_account;
                dr["adjustment_type"] = adjustment_type;
                dr["currency_code"] = currency_code;
                dr["company_code"] = company_code;
                dr["gl_center"] = gl_center;
                dr["gl_acct"] = gl_acct;
                dr["geography"] = geography;
                dr["interdivision"] = interdivision.Trim();
                dr["gl_product"] = gl_product;
                if (rowIsNotModified)
                {
                    dr.AcceptChanges();
                }
                else
                {
                    dr.AcceptChanges();
                    dr.SetAdded();
                }
            }

        }  
 

        public override void Save()
        {
            this.SaveSuccessful = false;
            //make sure offsetting amts are not 0
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            //string sRunningOffsetAmt = UnformatTextField(txtRunningOffsetAmt.Text.Trim());
            //if (Convert.ToDouble(txtRunningAdjAmt.Text.Trim()) + Convert.ToDouble(txtAdjAmt.Text.Trim()) + (Convert.ToDouble(txtAdjAmt.Text.Trim()) + Convert.ToDouble(txtRunningOffsetAmt.Text.Trim())) == 0)
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) != Convert.ToDouble(sAdjAmt.Trim()))
            {
                Messages.ShowWarning("Amount to Adjust does not equall Adjusted Amount. Save Cancelled");
                return;
            }
            //Loop through grid checking for any normalized lines that have been adjusted
            //RES 7/9/12 added allow_crdb indicator for adjustments to normalized lines
            bool NormalizedError = false;
            bool DefPoolError = false;
            bool Normalized = false;
            string NInovoices = "";
            //RES uncomment for Phase 2 to allow normalized adjustments
            foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
            {
                if ((Convert.ToDecimal(r.Cells["amount_adjusted"].Value) != 0) && (r.Cells["normalized_flag"].Value.ToString() == "1") &&
                    (r.Cells["rebill_flag"].Value.ToString() != "1") && (r.Cells["allow_crdb"].Value.ToString() != "1"))
                    NormalizedError = true;
  
                if ((Convert.ToDecimal(r.Cells["amount_adjusted"].Value) != 0) && (r.Cells["normalized_flag"].Value.ToString() == "1") &&
                    (r.Cells["rebill_flag"].Value.ToString() == "1") && (r.Cells["allow_crdb"].Value.ToString() != "1") &&
                    (r.Cells["def_pool_has_run"].Value.ToString() != "1"))
                    DefPoolError = true;

                if ((Convert.ToDecimal(r.Cells["amount_adjusted"].Value) != 0) && (r.Cells["normalized_flag"].Value.ToString() == "1") &&
                    (r.Cells["ninvoices"].Value.ToString() != ""))
                {
                    Normalized = true;
                    NInovoices = r.Cells["ninvoices"].Value.ToString();
                }
            }
            //RES Zero out taxes on the entire invoice 
            //if (Convert.ToBoolean(txtZeroTax.IsChecked))
            //{
            //    //loop through detail data table and update columns
            //    foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["detail"].Rows)
            //    {
            //        if (Convert.ToInt32(dr["tax_amount"]) != 0)
            //        {
            //            //bool rowIsNotModified = (dr.RowState != DataRowState.Modified);
            //            dr["zero_tax_flag"] = 1;
            //        }
            //    }
            //}
            //foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
            //{
            //    if ((Convert.ToDecimal(r.Cells["amount_adjusted"].Value) != 0) && (r.Cells["normalized_flag"].Value.ToString() == "1"))
            //        //(r.Cells["rebill_flag"].Value.ToString() != "1"))
            //        NormalizedError = true;
            //}
            if (NormalizedError)
            {
                //Messages.ShowError("Cannot adjust normalized line.  Only rebill allowed.");
                Messages.ShowError("Cannot adjust a normalized invoice if it is not the last invoice of the closed cycle. Only rebill allowed.");
                return;
            }
            if (DefPoolError)
            {
                Messages.ShowError("Cannot credit normalized line before Def Pool Job has run.  Run Def Pool Job then create adjustment.");
                return;
            }
            MessageBoxResult result1;
            //if (Normalized)
            //{
            //    result1 = Messages.ShowYesNo("Adjusting current invoice will cause the following invoice(s) to be rebilled: " +
            //                                  NInovoices + "  Do you want to continue?", System.Windows.MessageBoxImage.Warning);
            //    if (result1 == MessageBoxResult.No) return;
            //}

            //RES 8/7/18 do not allow amount adjusted to be modified on a rebill line
            MessageDisplayed = false;
            foreach (DataRecord r1 in GridCreditDebitDestination.xGrid.Records)
            {
                if ((r1.Cells["rebill_flag"].Value.ToString() == "1") && (Convert.ToDecimal(r1.Cells["amount_adjusted"].Value) != (Convert.ToDecimal(r1.Cells["extended"].Value) * -1)))
                {
                    Messages.ShowError("Cannot change the amount adjusted on a rebill line.  Amount adjusted should be original amount of the line.");
                    //btnSave.IsEnabled = false;
                    return;
                }
                // RES 8/24/18 Check for WHT adjustments that have not been reversed and show warning message
                if ((r1.Cells["rebill_flag"].Value.ToString() == "1") && (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["wht_amount"]) != 0))
                {
                    if (!MessageDisplayed)
                    {
                        MessageDisplayed = true;
                        //result2 = Messages.ShowYesNo("There are WHT adjustment(s) for this document that have not been reversed. " +
                        //                              "Do you want to continue?", System.Windows.MessageBoxImage.Warning);
                        //if (result2 == MessageBoxResult.No) return;
                        Messages.ShowWarning("There are WHT adjustment(s) for this document that have not been reversed");
                    }
                }
                //RES 5/8/19 Check for SLA adjustments and issue warning
                if ((r1.Cells["rebill_flag"].Value.ToString() == "1") && (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["sla_amount"]) != 0))
                {
                    if (!MessageDisplayed)
                    {
                        MessageDisplayed = true;
                        Messages.ShowWarning("There have been SLA adjustment(s) applied to this document");
                    }
                }
            }

            if (AmountsOffset())
            {
                string NewDocID = "";
                string SPErrMsg = "";

                NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, AdjType, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));
                if (NewDocID != "")
                    updGridRowsDocId(NewDocID);
                else
                {
                    Messages.ShowError("Problem Creating New Adjustment");
                    return;
                }
               
                base.Save();
                if (SaveSuccessful)
                {
                    //error messages from Stored Proc
                    string strSPErrMg = getInfoFromStoredProc();
                    if (strSPErrMg != null && strSPErrMg != "")
                    {
                        Messages.ShowError(strSPErrMg);
                        //do rollback
                        rollbackAdj(NewDocID);
                        //reset running total in case grid was reloaded by base.save
                        totGridRecs();
                    }
                    else
                    {
                        Messages.ShowInformation("Save Successful--New Adjustment ID = " + NewDocID);
                        //pop new adjustment in folder
                        AdjFolderScreen.ReturnData(NewDocID, "");
                        //add user name to AdjGeneralTab
                        AdjFolderScreen.AdjustmentGeneralTab.txtCreatedBy.Text = cGlobals.UserName.ToLower();
                        //close me/////////////////////////////////////////////////////////////////////////////////
                        System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);
                        this.CurrentBusObj.ObjectData.AcceptChanges();
                        StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
                        this.SaveSuccessful = true;
                        if (!ScreenBaseIsClosing)
                        {
                            AdjParent.Close();
                        }
                        ///////////////////////////////////////////////////////////////////////////////////////////
                    }
                }
                else
                {
                    Messages.ShowInformation("Data Error, Save Failed");
                    //do rollback
                    rollbackAdj(NewDocID);
                    //reset running total in case grid was reloaded by base.save
                    totGridRecs();
                }
             }
            else
            {
                Messages.ShowError("Amounts Do Not Offset. Save Cancelled");
            }
        }

        /// <summary>
        /// setup array of values from source grid to go to insert
        /// </summary>
        /// <returns></returns>
        private string[] setupSourceArray()
        {
            string[] strArr = new string[13];

            if (cGlobals.ReturnParms.Count > 0)
            {
                System.Collections.Generic.List<string> SourceGridList = null;
                SourceGridList = getValsFromSourceGrid();
                //adj doc id
                strArr[0] = txtDocument.Text;
                //cust num
                strArr[1] = cGlobals.ReturnParms[0].ToString();
                //adj amt
                strArr[2] = UnformatTextField(txtAdjAmt.Text);
                //prod code
                strArr[3] = cGlobals.ReturnParms[1].ToString();
                //currency
                strArr[4] = cGlobals.ReturnParms[2].ToString();
                //company_code
                strArr[5] = cGlobals.ReturnParms[3].ToString();
                //gl_center
                strArr[6] = cGlobals.ReturnParms[4].ToString();
                //gl_acct
                strArr[7] = cGlobals.ReturnParms[5].ToString();
                //geography
                strArr[8] = cGlobals.ReturnParms[6].ToString();
                //gl_product
                strArr[9] = cGlobals.ReturnParms[7].ToString();
                //user name
                strArr[10] = cGlobals.UserName.ToLower();
                //apply to doc
                strArr[11] = cGlobals.ReturnParms[8].ToString();
                //seq id
                strArr[12] = cGlobals.ReturnParms[9].ToString();
            }
            else
            {
                strArr = null;
            }

            return strArr;
        }

        /// <summary>
        /// set up list of values call base grid returnData() and pass back in cglobal parm list
        /// </summary>
        /// <returns></returns>
        private System.Collections.Generic.List<string> getValsFromSourceGrid()
        {
            System.Collections.Generic.List<string> SourceGridList = new System.Collections.Generic.List<string>();
            SourceGridList.Add("receivable_account");
            SourceGridList.Add("product_code");
            SourceGridList.Add("currency_code");
            SourceGridList.Add("company_code");
            SourceGridList.Add("gl_center");
            SourceGridList.Add("gl_acct");
            SourceGridList.Add("geography");
            SourceGridList.Add("gl_product");
            SourceGridList.Add("document_id");
            SourceGridList.Add("apply_to_seq");
            cGlobals.ReturnParms.Clear();
            GridCreditDebitSource.ReturnSelectedData(SourceGridList);
            return SourceGridList;
        }

        /// <summary>
        /// searches for selected values and adds results to search grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    //double dblAmt = 0;
        //    if (txtCustNumSearch.Text == null) txtCustNumSearch.Text = "";
        //    //if (txtCustNameSearch.Text == null) txtCustNameSearch.Text = "";
        //    if (txtInvNumSearch.Text == null) txtInvNumSearch.Text = "";

        //    this.CurrentBusObj.changeParm("@receivable_account", txtCustNumSearch.Text);
        //    this.CurrentBusObj.changeParm("@invoice_number", txtInvNumSearch.Text);
        //    if (txtAmtSearch.Text == null || txtAmtSearch.Text == "")
        //    {
        //        txtAmtSearch.Text = "";
        //        this.CurrentBusObj.changeParm("@amount", "0");
        //    }
        //    else
        //    {
        //        this.CurrentBusObj.changeParm("@amount", txtAmtSearch.Text);
        //    }

        //    this.CurrentBusObj.LoadData("search");
        //    GridSearch.LoadGrid(this.CurrentBusObj, "search");
        //    if (this.CurrentBusObj.ObjectData.Tables["search"].Rows.Count < 1)
        //        Messages.ShowInformation("No records found");
        //}

        /// <summary>
        /// clears seach fields and grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //SearchClear();
        }

        /// <summary>
        /// enables adj amt and pop cash source grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDocument_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtDocument.Text != "")
            {
                //do this to keep grid from loading and binding re-running clearing user entered values
                if (DocumentId != txtDocument.Text)
                {
                    //populate grid
                    popGrid();
                    //txtRunningAdjAmt.CntrlFocus();
                }
            }
        }

             
        private void txtAdjAmt_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //if txt is not numeric////////////////////////////////////////////////////////////
            Double result = 0;
            if (Double.TryParse(txtAdjAmt.Text, out result) == false)
            {
                txtAdjAmt.Text = "0.00";
            }
            ///////////////////////////////////////////////////////////////////////////////////
            //set a default value if user skips
            if (txtAdjAmt.Text == "") txtAdjAmt.Text = "0.00";
            ///////////////////////////////////////////////////////////////////////////
            //set to edit mode if val != 0
            if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            {
                setEditScreenState();
                //if amt < 0 make neg set to red
                if (Convert.ToDouble(txtAdjAmt.Text) > 0)
                {
                    if (AdjType == 1 || AdjType == 22)
                    {
                        txtAdjAmt.Text = (Convert.ToDouble(txtAdjAmt.Text) * -1).ToString();
                        txtAdjAmt.TextColor = "Red";
                    }
                    else
                    {
                        txtAdjAmt.Text = (Convert.ToDouble(txtAdjAmt.Text)).ToString();
                        //txtAdjAmt.TextColor = "Black";
                    }
                }
                //if amt neg set to red
                else
                {
                    if (AdjType == 2)
                        txtAdjAmt.Text = (Convert.ToDouble(txtAdjAmt.Text) * -1).ToString();
                    else
                        txtAdjAmt.TextColor = "Red";
                }
                //convert the value to a double and format to add trailing zeros if missing
                //double formatAmt = Convert.ToDouble(txtAdjAmt.Text);
                //txtAdjAmt.Text = formatAmt.ToString("0.00");
                if (this.CurrentBusObj.ObjectData != null)
                    if (AmountsOffset())
                    {
                        btnSave.IsEnabled = true;
                        if (AdjTotal > 0)
                            btnApplyCash.IsEnabled = true;
                    }
            }
            txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
        }

        /// <summary>
        /// used to setup grid for tabbing through editable fields
        /// TODO: Goto first editable field FAIL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAdjAmt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (this.CurrentBusObj.ObjectData != null)
                if ((e.Key == Key.Tab || e.Key == Key.Enter) && this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0)
                {
                    this.PrepareFreeformForSave();
                    Double result = 0;
                    if (Double.TryParse(txtAdjAmt.Text, out result) == false)
                    {
                        txtAdjAmt.Text = "0.00";
                    }
                    if (Convert.ToDouble(txtAdjAmt.Text) != 0)
                    {
                        setEditScreenState();
                        GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                        e.Handled = true;
                    }
                }
        }

        /// <summary>
        /// populates grid and enables adj amt field if grid rows > 0
        /// </summary>
        private void popGrid()
        {
           
            //load parms 
            loadParms();
            // set parent/child relationship
            SetParentChildAttributes();
            //load the object
            this.Load();
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                txtAdjAmt.IsReadOnly = true;
                setInitScreenState();
                if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["unposted_adjustments"]) > 0)
                {
                    Messages.ShowInformation("There are unposted adjustment(s) for this document");
                    txtDocument.CntrlFocus();
                    txtZeroTax.IsEnabled = false;
                    txtCurrentTaxRate.IsEnabled = false;
                    //GridCreditDebitSource.IsEnabled = false;
                    GridCreditDebitDestination.IsEnabled = false;
                    return;
                }
                else
                {
                    txtZeroTax.IsEnabled = true;
                    txtCurrentTaxRate.IsEnabled = true;
                    //GridCreditDebitSource.IsEnabled = true;
                    GridCreditDebitDestination.IsEnabled = true;
                    if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0 && AdjType == 22)
                    {
                        GridCreditDebitDestination.xGrid.FieldLayouts[0].Fields["rebill_flag"].Visibility = Visibility.Collapsed;
                        GridCreditDebitDestination.xGrid.FieldLayouts[0].Fields["informational_flag"].Visibility = Visibility.Collapsed;
                        GridCreditDebitDestination.xGrid.FieldLayouts[0].Fields["normalized_flag"].Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        GridCreditDebitDestination.xGrid.FieldLayouts[0].Fields["rebill_flag"].Visibility = Visibility.Visible;
                        GridCreditDebitDestination.xGrid.FieldLayouts[0].Fields["informational_flag"].Visibility = Visibility.Visible;
                        GridCreditDebitDestination.xGrid.FieldLayouts[0].Fields["normalized_flag"].Visibility = Visibility.Visible;
                    }
                }
               
                if (AdjType == 2)
                    txtZeroTax.IsEnabled = false;
                else
                    txtZeroTax.IsEnabled = true;

            }
            else
            {
                Messages.ShowInformation("Document Not Found");
                //txtAdjAmt.IsReadOnly = true;
                setInitScreenState();
                txtDocument.CntrlFocus();
            }
            if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0)
            {
                txtAdjAmt.IsReadOnly = false;
                //setInitScreenState();
            }
            else
            {
                Messages.ShowInformation("Document Detail Not Found");
                txtAdjAmt.IsReadOnly = true;
                setInitScreenState();
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
            this.CurrentBusObj.Parms.AddParm("@document_id", txtDocument.Text);
            DocumentId = txtDocument.Text;
        }

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            //int Index = 0;
            btnSave.IsEnabled = false;
            btnApplyCash.IsEnabled = false;
            GridCreditDebitSource.IsEnabled = true;
            GridCreditDebitDestination.IsEnabled = true;
            BorderDestination.IsEnabled = true;
            txtRunningAdjAmt.Text = "0.00";
            //foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
            //{
            //    if (Convert.ToInt32(r.Cells["normalized_flag"].Value) == 1)
            //        //r.Cells["total_adjusted"].Settings.AllowEdit = false;
            //        GridCreditDebitDestination.xGrid.FieldLayouts[0].Fields["amount_adjusted"].Settings.AllowEdit = false;
            //    //gAlloc.xGrid.FieldLayouts[0].Fields["receivable_account"].Settings.AllowEdit = false;
            //    //Index = Index + 1;
            //}
        }

        /// <summary>
        /// frees up objects for edit
        /// </summary>
        private void setEditScreenState()
        {
            //GridCreditDebitSource.IsEnabled = true;
            GridCreditDebitDestination.IsEnabled = true;
            //btnClearDest.IsEnabled = true;
            BorderDestination.IsEnabled = true;
            //Set the amount_adjusted field as active
            if (this.CurrentBusObj.ObjectData != null)
                //RES 6/12/15 Do not enable active record if it is -1 (filter row)
                if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0 && GridCreditDebitDestination.ActiveRecord.Index > -1)
                //if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0)
                    (GridCreditDebitDestination.xGrid.Records[GridCreditDebitDestination.ActiveRecord.Index] as DataRecord).Cells["amount_adjusted"].IsActive = true;
        }

        /// <summary>
        /// check that amounts are offsetting
        /// </summary>
        /// <returns></returns>
        private bool AmountsOffset()
        {
            //check for nulls
            if (txtRunningAdjAmt.Text == null)
                return false;
            if (txtAdjAmt.Text == null)
                return false;
            //if (txtRunningOffsetAmt.Text == null)
            //    return false;
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            //if (Convert.ToDouble(txtRunningAdjAmt.Text.Trim()) == Convert.ToDouble(txtAdjAmt.Text.Trim()) && (Convert.ToDouble(txtAdjAmt.Text.Trim()) + Convert.ToDouble(txtRunningOffsetAmt.Text.Trim())) == 0)
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) == Convert.ToDouble(sAdjAmt.Trim()))
            {
                //if (Convert.ToDouble(txtAdjAmt.Text) != 0)
                if (Convert.ToDouble(sAdjAmt) != 0)
                {
                    btnSave.IsEnabled = true;
                    if (AdjTotal > 0)
                        btnApplyCash.IsEnabled = true;
                    return true;
                }
                else
                {
                    btnSave.IsEnabled = false;
                    btnApplyCash.IsEnabled = false;
                    return false;
                }
            }
            else
            {
                btnSave.IsEnabled = false;
                btnApplyCash.IsEnabled = false;
                return false;
            }

         
        }

        /// <summary>
        /// handles prebind junk
        /// </summary>
        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    //ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    ////Set the items source to be the databale of the DDDW
                    //ip.ItemsSource = this.CurrentBusObj.ObjectData.Tables["products"].DefaultView;

                    ////set the value and display path
                    //ip.ValuePath = "product_code";
                    //ip.DisplayMemberPath = "product_description";
                    //cmbProductGridCombo = ip;

                    //this.cmbReason.SetBindingExpression("adj_print_reason_code", "adj_print_reason_desc", this.CurrentBusObj.ObjectData.Tables["reason"]);
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }

        /// <summary>
        /// clears destination grid
        /// </summary>
        private void btnClearDest_Click(object sender, System.Windows.RoutedEventArgs e)
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
            //GridSearch.LoadGrid(this.CurrentBusObj, "destination");
            //txtRunningOffsetAmt.Text = "0.00";
            //chkApplyUACash.IsChecked = 0;
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

        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = GridCreditDebitDestination.ActiveRecord.Cells.IndexOf(e.Cell);
            //DataRecord GridParent = GridCreditDebitSource.ActiveRecord;
            //double NewAmountAdjusted = 0;
            
            //int row_index = GridAdjustment.ActiveRecord.Index;

            DataRecord GridRecord = null;
            //if (edit_index == 10) //Amount To Adjust Field
            if (e.Cell.Field.Name == "amount_adjusted")
            {
                //commit user entered value to datatable
                GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                //DataRecord GridRecord2 = null;
                //if (GridCreditDebitDestination.ActiveRecord == FilterRecord) return;
                GridRecord = GridCreditDebitDestination.ActiveRecord;
                if (GridRecord.DataItemIndex < 0)
                    return;
                DataRowView dr = GridRecord.DataItem as DataRowView;
                DataView dv = dr.DataView;
                if (GridRecord != null)
                {
                    //clear running total
                    txtRunningAdjAmt.Text = "";
                    if (GridRecord.Cells["amount_adjusted"].Value.ToString() == "")
                        GridRecord.Cells["amount_adjusted"].Value = 0.00;

                    //Total Grid Cells and update main grid
                    totGridRecs();
                    //tas 12.10.13 modified to use only total for the produt code
                    DataRecord GridParent = GridCreditDebitSource.ActiveRecord;
                    string product = GridParent.Cells["product_code"].Value.ToString();
                    ProdAdjTotal = 0.00M;
                    foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
                    {
                        //sum adj amts
                        if (r.Cells["product_code"].Value.ToString() == product)
                        {
                            ProdAdjTotal += Convert.ToDecimal(r.Cells["amount_adjusted"].Value);
                        }
                           
                    }





                    GridParent.Cells["amount_adjusted"].Value = ProdAdjTotal;
                    GridParent.Cells["apply_to_doc"].Value = GridParent.Cells["document_id"].Value;
                    GridParent.Cells["apply_to_seq"].Value = GridParent.Cells["seq_code"].Value;
                    GridCreditDebitSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    //CurrentBusObj.ObjectData.Tables["main"].AcceptChanges();
   
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0) txtRunningAdjAmt.TextColor = "Red";
                    else txtRunningAdjAmt.TextColor = "Black";
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    if (AmountsOffset())
                    {
                        btnSave.IsEnabled = true;
                        if (AdjTotal > 0)
                            btnApplyCash.IsEnabled = true;
                    }
                 }
            }
        }

        /// <summary>
        /// totals grid cells and adds to running total
        /// </summary>
        private void totGridRecs()
        {
            //init AdjTotal w/starting value
            AdjTotal = 0.00M;
            //loop through each grid record and add adj totals
            foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
            {
                //sum adj amts
                AdjTotal += Convert.ToDecimal(r.Cells["amount_adjusted"].Value);
            }
            txtRunningAdjAmt.Text = AdjTotal.ToString("0.00");

            //foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
            //{
            //    if (Convert.ToDouble(r.Cells["amount_adjusted"].Value) != 0)
            //        r.Cells["total_adjusted"].Value = AdjTotal;
            //}

            //DataRecord GridParent = GridCreditDebitSource.ActiveRecord;
            //GridParent.Cells["amount_adjusted"].Value = AdjTotal;
            GridCreditDebitSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //CurrentBusObj.ObjectData.Tables["main"].AcceptChanges();

        }

        private void updGridRowsDocId(string DocID)
        {
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            int apply_to_seq = 0;
            string receivable_account = "";
            int adjustment_type = AdjType;
            string currency_code = "USD";
            string company_code = "";
            string gl_center = "";
            string gl_acct = "";
            string geography = "";
            string interdivision = "";
            string gl_product = "";
            DataRecord r = GridCreditDebitSource.ActiveRecord;
            DataRow row = (r.DataItem as DataRowView).Row;
            if (row != null)
            {
                apply_to_seq = Convert.ToInt32(row["seq_code"]);
                receivable_account = Convert.ToString(row["receivable_account"]);
                //if (Convert.ToDouble(sAdjAmt) > 0) adjustment_type = 1;
                currency_code = Convert.ToString(row["currency_code"]);
                company_code = Convert.ToString(row["company_code"]);
                gl_center = Convert.ToString(row["gl_center"]);
                gl_acct = Convert.ToString(row["gl_acct"]);
                geography = Convert.ToString(row["geography"]);
                interdivision = Convert.ToString(row["interdivision"]);
                gl_product = Convert.ToString(row["gl_product"]);
            }

            //loop through main data table and update columns
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["main"].Rows)
            {
                bool rowIsNotModified = (dr.RowState != DataRowState.Modified);
                dr["adj_document_id"] = DocID;
                if (rowIsNotModified)
                {
                    dr.AcceptChanges();
                }
                else
                {
                    dr.AcceptChanges();
                    dr.SetAdded();
                }
            }

            //loop through detail data table and update columns
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["detail"].Rows)
            {
                //bool rowIsNotModified = (dr.RowState != DataRowState.Modified);
                bool rowIsNotModified = true;
                if (Convert.ToDecimal(dr["amount_adjusted"]) != 0)
                    rowIsNotModified = false;
                dr["adj_document_id"] = DocID;
                //dr["apply_to_seq"] = apply_to_seq; tas 12.10.13 commented out as now supplied by SP.
                dr["receivable_account"] = receivable_account;
                dr["adjustment_type"] = adjustment_type;
                dr["currency_code"] = currency_code;
                dr["company_code"] = company_code;
                dr["gl_center"] = gl_center;
                dr["gl_acct"] = gl_acct;
                dr["geography"] = geography;
                dr["interdivision"] = interdivision.Trim();
                dr["gl_product"] = gl_product;
                if (rowIsNotModified)
                {
                    dr.AcceptChanges();
                }
                else
                {
                    dr.AcceptChanges();
                    dr.SetAdded();
                }
            }
        }

        private void rollbackAdj(string NewDocID)
        {
            //delete adj header
            bool retVal = cGlobals.BillService.DeleteNewAdjusmentPreamble(NewDocID);
        }

        private string getInfoFromStoredProc()
        {
            var SPErrorMsg = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                             where x.Field<string>("parmName") == "@error_message"
                             select new
                             {
                                 parmName = x.Field<string>("parmName"),
                                 parmValue = x.Field<string>("parmValue")
                             };
            foreach (var info in SPErrorMsg)
            {
                if (info.parmName == "@error_message")
                    return info.parmValue;
            }
            return "";
        }

        private void GridDestination_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            if (e.Cell.Field.Name == "rebill_flag")
            {
                DataRecord GridRecord = null;
                GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridRecord = GridCreditDebitDestination.ActiveRecord;
                if (GridRecord != null)
                {
                    //clear running total
                    txtRunningAdjAmt.Text = "";
                    if (GridRecord.Cells["rebill_flag"].Value.ToString() == "1")
                    {                        
                        GridRecord.Cells["amount_adjusted"].Value = Convert.ToDecimal(GridRecord.Cells["extended"].Value) * -1;
                        GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    }

                    else
                        GridRecord.Cells["amount_adjusted"].Value = 0.00;
                    //Total Grid Cells
                    totGridRecs();
                    DataRecord GridParent = GridCreditDebitSource.ActiveRecord;
                    GridParent.Cells["amount_adjusted"].Value = AdjTotal;
                    GridParent.Cells["apply_to_doc"].Value = GridParent.Cells["document_id"].Value;
                    GridParent.Cells["apply_to_seq"].Value = GridParent.Cells["seq_code"].Value;
                    GridCreditDebitSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0) txtRunningAdjAmt.TextColor = "Red";
                    else txtRunningAdjAmt.TextColor = "Black";
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    if (AmountsOffset())
                    {
                        btnSave.IsEnabled = true;
                        if (AdjTotal > 0)
                            btnApplyCash.IsEnabled = true;
                    }
                }
            }
        }
        //private void GridDestination_BeforeCellActivate(object sender, Infragistics.Windows.UltraWinGrid.CancelableCellEventArgs e)
        //{
        //    //Infragistics.Win.UltraWinGrid.CancelableCellEventArgs e
        //}

        //Create entries to zero out sales tax
        private void txtZeroTax_Checked(object sender, RoutedEventArgs e)
        {
            if (txtCurrentTaxRate.IsChecked == 1)
            {
                Messages.ShowWarning("Cannot select zero tax if use current tax rates is selected!");
                txtZeroTax.IsChecked = 0;
                return;
            }
            foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
            {
                if ((r.Cells["zero_tax_flag"].Value.ToString() == "0") && (Convert.ToDecimal(r.Cells["tax_amount"].Value.ToString()) != 0))
                {
                    txtRunningAdjAmt.Text = "";
                    r.Cells["zero_tax_flag"].Value = 1;
                    r.Cells["amount_adjusted"].Value = Convert.ToDecimal(r.Cells["amount_adjusted"].Value) + (Convert.ToDecimal(r.Cells["adjusted_tax_amount"].Value) * -1);
                    GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    //Total Grid Cells
                    totGridRecs();
                    DataRecord GridParent = GridCreditDebitSource.ActiveRecord;
                    GridParent.Cells["amount_adjusted"].Value = AdjTotal;
                    GridParent.Cells["apply_to_doc"].Value = GridParent.Cells["document_id"].Value;
                    GridParent.Cells["apply_to_seq"].Value = GridParent.Cells["seq_code"].Value;
                    GridCreditDebitSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    txtAdjAmt.Text = AdjTotal.ToString("0.00");
                    //txtAdjAmt.Text = txtRunningAdjAmt.Text;
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0)
                    {
                        txtRunningAdjAmt.TextColor = "Red";
                        txtAdjAmt.TextColor = "Red";
                    }
                    else
                    {
                        txtRunningAdjAmt.TextColor = "Black";
                        txtAdjAmt.TextColor = "Black";
                    }
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    if (AmountsOffset())
                    {
                        btnSave.IsEnabled = true;
                        if (AdjTotal > 0)
                            btnApplyCash.IsEnabled = true;
                    }
                }
            }
        }

        private void txtZeroTax_UnChecked(object sender, RoutedEventArgs e)
        {
            foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
            {

                if (r.Cells["zero_tax_flag"].Value.ToString() == "1")
                {
                    txtRunningAdjAmt.Text = "";
                    r.Cells["zero_tax_flag"].Value = 0;
                    r.Cells["amount_adjusted"].Value = Convert.ToDecimal(r.Cells["amount_adjusted"].Value) - (Convert.ToDecimal(r.Cells["adjusted_tax_amount"].Value) * -1); 
                    GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    //Total Grid Cells
                    totGridRecs();
                    DataRecord GridParent = GridCreditDebitSource.ActiveRecord;
                    GridParent.Cells["amount_adjusted"].Value = AdjTotal;
                    GridParent.Cells["apply_to_doc"].Value = GridParent.Cells["document_id"].Value;
                    GridParent.Cells["apply_to_seq"].Value = GridParent.Cells["seq_code"].Value;
                    GridCreditDebitSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    txtAdjAmt.Text = AdjTotal.ToString("0.00");
                    //txtAdjAmt.Text = txtRunningAdjAmt.Text;
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0)
                    {
                        txtRunningAdjAmt.TextColor = "Red";
                        txtAdjAmt.TextColor = "Red";
                    }
                    else
                    {
                        txtRunningAdjAmt.TextColor = "Black";
                        txtAdjAmt.TextColor = "Black";
                    }
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    if (AmountsOffset())
                    {
                        btnSave.IsEnabled = true;
                        if (AdjTotal > 0)
                            btnApplyCash.IsEnabled = true;
                    }
                }
            }
        }

        //Set flag to use current tax rates when calculating sales tax
        private void txtCurrentTaxRate_Checked(object sender, RoutedEventArgs e)
        {
            if (txtZeroTax.IsChecked == 1)
            {
                Messages.ShowWarning("Cannot select use current tax rates if zero tax is selected!");
                txtCurrentTaxRate.IsChecked = 0;
                return;
            }
            foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
            {
                if ((r.Cells["use_current_tax_rates_flag"].Value.ToString() == "0") && (Convert.ToDecimal(r.Cells["tax_amount"].Value.ToString()) != 0))
                {
                    r.Cells["use_current_tax_rates_flag"].Value = 1;
                    GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                }
            }

        }

        private void txtCurrentTaxRate_UnChecked(object sender, RoutedEventArgs e)
        {
            foreach (DataRecord r in GridCreditDebitDestination.xGrid.Records)
            {

                if (r.Cells["use_current_tax_rates_flag"].Value.ToString() == "1")
                {
                    r.Cells["use_current_tax_rates_flag"].Value = 0;
                    GridCreditDebitDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                }
            }

        }

    }
}

