

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Windows;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentCash' object.
    /// </summary>
    public partial class AdjustmentCash : ScreenBase, IPreBindable
    {
        public cBaseBusObject CashAdjustmentBusObj = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        private bool IsSingleClickOrigin { get; set; }
        public ComboBoxItemsProvider cmbProductGridCombo { get; set; }
        private string DocumentId { get; set; }

        /// <summary>
        /// Create a new instance of a 'AdjustmentCash' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentCash(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = CashAdjustmentBusObj;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentCash";
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
            //setup adj grid
            GridCashSource.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridCashSource.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridCashSource.ContextMenuAddIsVisible = false;
            GridCashSource.ContextMenuRemoveIsVisible = false;
            GridCashSource.SingleClickZoomDelegate = SingleClickZoomDelegateSource;
            GridCashSource.MainTableName = "main";
            GridCashSource.ConfigFileName = "AdjustmentCashSource";
            GridCashSource.SetGridSelectionBehavior(true, false);
            GridCashSource.xGrid.FieldSettings.AllowEdit = false;
            GridCashSource.FieldLayoutResourceString = "GridCashSource";
            GridCollection.Add(GridCashSource);

            GridCashDestination.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(GridDestination_EditModeEnded);
            GridCashDestination.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridCashDestination.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridCashDestination.ContextMenuAddIsVisible = false;
            GridCashDestination.ContextMenuRemoveIsVisible = false;
            GridCashDestination.MainTableName = "destination";
            GridCashDestination.ConfigFileName = "AdjustmentGridCashDestination";
            GridSearch.SetGridSelectionBehavior(true, false);
            GridCashDestination.xGrid.FieldSettings.AllowEdit = false;
            GridCashDestination.FieldLayoutResourceString = "GridCashDestination";
            GridCollection.Add(GridCashDestination);

            GridSearch.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridSearch.DoNotSelectFirstRecordOnLoad = true;
            GridSearch.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridSearch.ContextMenuAddIsVisible = false;
            GridSearch.ContextMenuRemoveIsVisible = false;
            //GridSearch.SingleClickZoomDelegate = SingleClickZoomDelegateSearch;
            GridSearch.WindowZoomDelegate = DoubleClickZoomDelegateSearch;
            GridSearch.MainTableName = "search";
            GridSearch.ConfigFileName = "AdjustmentGridCashSearch";
            GridSearch.SetGridSelectionBehavior(true, false);
            GridSearch.xGrid.FieldSettings.AllowEdit = false;
            GridSearch.FieldLayoutResourceString = "GridCashSearch";
            GridCollection.Add(GridSearch);

            this.Load();
            //set initial screen state
            //txtAdjAmt.IsReadOnly = true;
            setInitScreenState();
            txtDocument.CntrlFocus();
        }

        /// <summary>
        /// works as cellLeave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GridDestination_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = GridCashDestination.ActiveRecord.Cells.IndexOf(e.Cell);

            //int row_index = GridAdjustment.ActiveRecord.Index;

            //WARNING: You may need to change this if the order of the AdjustmentFieldLayouts\GridConversionInvoice changes
            //if (edit_index == 7) //Amount To Adjust Field
            //{
                //commit user entered value to datatable
                GridCashDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                DataRecord GridRecord = null;
                GridRecord = GridCashDestination.ActiveRecord;
                DataRowView dr = GridRecord.DataItem as DataRowView;
                DataView dv = dr.DataView;
                if (GridRecord != null)
                {
                    //clear running total
                    txtRunningOffsetAmt.Text = "";
                    //init AdjTotal w/starting value
                    double AdjTotal = 0.00;
                    if (GridRecord.Cells["amt_to_adjust"].Value.ToString() == "")
                    {
                        GridRecord.Cells["amt_to_adjust"].Value = 0.00;
                    }
                    //loop through each grid record and add adj totals
                    foreach (DataRecord r in GridCashDestination.xGrid.Records)
                    {
                        //sum adj amts
                        AdjTotal += Convert.ToDouble(r.Cells["amt_to_adjust"].Value);
                    }
                    txtRunningOffsetAmt.Text = AdjTotal.ToString("0.00");
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningOffsetAmt.Text) < 0) txtRunningOffsetAmt.TextColor = "Red";
                    else txtRunningOffsetAmt.TextColor = "Black";
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
                    if (AmountsOffset()) btnSave.IsEnabled = true;
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
            //set this to prevent gridDestination single click delegate from firing when GridCashDestination.xGrid.ActiveDataItem = row; runs
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
            GridSearch.ReturnSelectedData(LocationFieldList);
            if (cGlobals.ReturnParms.Count > 0)
            {
                GridCashDestination.SetGridSelectionBehavior(true, false);
                GridSearch.xGrid.FieldSettings.AllowEdit = true;
                DataView dataSource = this.GridCashDestination.xGrid.DataSource as DataView;
                //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
                //Add new grid row and set cursor in first cell of last row
                DataRowView row = dataSource.AddNew();
                GridCashDestination.xGrid.ActiveDataItem = row;
                GridCashDestination.xGrid.ActiveCell = (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
                //document_id
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
                //seq_code
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
                //company_code
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
                //product_code
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[3];
                //receivable_account
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[4];
                //open_amount
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
                //amount
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
                //Amt to Adj
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = 0.00;
                //currency_code
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = cGlobals.ReturnParms[7];
                //exchange_rate
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[8];

                GridCashDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridCashDestination.SetGridSelectionBehavior(false, false);
                GridCashDestination.xGrid.FieldSettings.AllowEdit = true;
                IsSingleClickOrigin = false;
            }
        }
        public void SingleClickZoomDelegateSearch()
        {
            //GridCashDestination.SetGridSelectionBehavior(true, false);
            //GridSearch.xGrid.FieldSettings.AllowEdit = true;
            //DataView dataSource = this.GridCashDestination.xGrid.DataSource as DataView;
            ////GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
            ////Add new grid row and set cursor in first cell of last row
            //DataRowView row = dataSource.AddNew();
            //GridCashDestination.xGrid.ActiveDataItem = row;
            //GridCashDestination.xGrid.ActiveCell = (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
            ////document_id
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
            ////seq_code
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
            ////company_code
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
            ////product_code
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[3];
            ////receivable_account
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[4];
            ////open_amount
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
            ////amount
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
            ////Amt to Adj
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = 0.00;
            ////currency_code
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = cGlobals.ReturnParms[7];
            ////exchange_rate
            //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[8];

            //GridCashDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //GridCashDestination.SetGridSelectionBehavior(false, false);
            //GridCashDestination.xGrid.FieldSettings.AllowEdit = true;
            //IsSingleClickOrigin = false;
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

        public override void Save()
        {
            this.SaveSuccessful = false;
            if (GridCashSource.xGrid.Records.Count == 0)
            {
                Messages.ShowWarning("No cash source document, Save Cancelled");
                txtDocument.CntrlFocus();
                return;
            }
            //make sure offsetting amts are not 0
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            string sRunningOffsetAmt = UnformatTextField(txtRunningOffsetAmt.Text.Trim());
            //if (Convert.ToDouble(txtRunningAdjAmt.Text.Trim()) + Convert.ToDouble(txtAdjAmt.Text.Trim()) + (Convert.ToDouble(txtAdjAmt.Text.Trim()) + Convert.ToDouble(txtRunningOffsetAmt.Text.Trim())) == 0)
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) + Convert.ToDouble(sAdjAmt.Trim()) + (Convert.ToDouble(sAdjAmt.Trim()) + Convert.ToDouble(sRunningOffsetAmt.Trim())) == 0)
            {
                Messages.ShowWarning("Adjustment Amount and Offsetting Amount Cannot Be $0.00. Save Cancelled");
                return;
            }
            if (AmountsOffset())
            {
                string NewDocID = "";
                string SPErrMsg = "";
                //setup array of source values to pass to insert
                string[] strSourceArray = setupSourceArray();
                if (strSourceArray == null)
                {
                    Messages.ShowError("Error Occurred Loading Values from Cash Source Grid. Save Failed");
                    return;
                }
                else
                {
                    //make sure grid source record is selected
                    if (strSourceArray[0].ToString() == "")
                    {
                        Messages.ShowWarning("No Cash Source Record Selected, Save Cancelled");
                        return;
                    }
                }
                //make sure product code is selected if applicable
                foreach (DataRow row in this.CurrentBusObj.ObjectData.Tables["destination"].Rows)
                {
                    if (row["product_code"].ToString() == "")
                    {
                        Messages.ShowWarning("No Product Code Selected for Cash Destination, Save Cancelled");
                        return;
                    }
                    //RES 9/18/17 Do not allow credit amount in the amount to adjust column
                    if (Convert.ToDouble(row["amt_to_adjust"].ToString()) < 0)
                    {
                        Messages.ShowWarning("Amount to Adjust cannot be negative, Save Cancelled");
                        return;
                    }
                }
                //call method to insert a new adj and get new doc Id
                //RES phase 3.0.1 create new ws for adjust cash so apply cash can be different
                bool Success = cGlobals.BillService.InsertNewCashAdjustment(strSourceArray, this.CurrentBusObj.ObjectData, 5, ref NewDocID, ref SPErrMsg);
                //bool Success = cGlobals.BillService.InsertAdjCashAdjustment(strSourceArray, this.CurrentBusObj.ObjectData, 5, ref NewDocID, ref SPErrMsg);
                if (Success)
                {
                    if (NewDocID != "")
                    {
                        //Insert Success
                        Messages.ShowInformation("Save Successful--New Adjustment ID = " + NewDocID);
                        //pop new adjustment in folder
                        AdjFolderScreen.ReturnData(NewDocID, "");
                        //add user name to AdjGeneralTab
                        AdjFolderScreen.AdjustmentGeneralTab.txtCreatedBy.Text = cGlobals.UserName.ToLower();
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
                    else
                    {
                        if (SPErrMsg != "")
                            Messages.ShowError("No New Adj Id and : " + SPErrMsg);
                        else
                            Messages.ShowError("Problem Creating New Adjustment ID");
                        return;
                    }
                }
                else
                {
                    if (SPErrMsg != "")
                        Messages.ShowError(SPErrMsg);
                    else
                        Messages.ShowError("Problem Creating New Adjustment");
                    return;
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
            SourceGridList.Add("seq_code");
            cGlobals.ReturnParms.Clear();
            GridCashSource.ReturnSelectedData(SourceGridList);
            return SourceGridList;
        }

        /// <summary>
        /// searches for selected values and adds results to search grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //double dblAmt = 0;
            if (txtCustNumSearch.Text == null) txtCustNumSearch.Text = "";
            //if (txtCustNameSearch.Text == null) txtCustNameSearch.Text = "";
            if (txtInvNumSearch.Text == null) txtInvNumSearch.Text = "";
            
            this.CurrentBusObj.changeParm("@receivable_account", txtCustNumSearch.Text);
            this.CurrentBusObj.changeParm("@invoice_number", txtInvNumSearch.Text);
            if (txtAmtSearch.Text == null || txtAmtSearch.Text == "")
            {
                txtAmtSearch.Text = "";
                this.CurrentBusObj.changeParm("@amount", "0");
            }
            else
            {
                this.CurrentBusObj.changeParm("@amount", txtAmtSearch.Text);
            }
            
            this.CurrentBusObj.LoadData("search");
            GridSearch.LoadGrid(this.CurrentBusObj, "search");
            if (this.CurrentBusObj.ObjectData.Tables["search"].Rows.Count < 1)
                Messages.ShowInformation("No records found");
        }

        /// <summary>
        /// clears seach fields and grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SearchClear();
        }

        private void SearchClear()
        {
            //clear search criteria
            txtCustNumSearch.Text = "";
            //txtCustNameSearch.Text = "";
            txtInvNumSearch.Text = "";
            txtAmtSearch.Text = "";
            //clear parms
            this.CurrentBusObj.changeParm("@receivable_account", txtCustNumSearch.Text);
            //this.CurrentBusObj.changeParm("@invoice_number", txtCustNameSearch.Text);
            this.CurrentBusObj.changeParm("@invoice_number", "999999999");
            this.CurrentBusObj.changeParm("@amount", "0");
            //clear grid
            this.CurrentBusObj.LoadData("search");
            GridSearch.LoadGrid(this.CurrentBusObj, "search");
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
                    txtAdjAmt.Text = (Convert.ToDouble(txtAdjAmt.Text) * -1).ToString();
                    txtAdjAmt.TextColor = "Red";
                }
                //if amt neg set to red
                else
                {
                    txtAdjAmt.TextColor = "Red";
                }
                //convert the value to a double and format to add trailing zeros if missing
                double formatAmt = Convert.ToDouble(txtAdjAmt.Text);
                txtAdjAmt.Text = formatAmt.ToString("0.00");
            }
            txtRunningAdjAmt.TextColor = "Red";
            txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
            //txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));

        }

        /// <summary>
        /// populates grid and enables adj amt field if grid rows > 0
        /// </summary>
        private void popGrid()
        {
            //load parms 
            loadParms();
            //load the object
            this.Load();
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                //txtAdjAmt.IsReadOnly = false;
                setInitScreenState();
            }
            else
            {
                Messages.ShowInformation("Document Not Found");
                //txtAdjAmt.IsReadOnly = true;
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
            this.CurrentBusObj.Parms.AddParm("@receivable_account", "");
            this.CurrentBusObj.Parms.AddParm("@invoice_number", "");
            this.CurrentBusObj.Parms.AddParm("@amount", 0);
            DocumentId = txtDocument.Text;
        }

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            btnSave.IsEnabled = false;
            GridCashSource.IsEnabled = false;
            GridCashDestination.IsEnabled = false;
            GridSearch.IsEnabled = false;
            chkApplyUACash.IsEnabled = false;
            BorderSearch.IsEnabled = false;
            BorderDestination.IsEnabled = false;
            btnClearDest.IsEnabled = false;
            txtRunningAdjAmt.Text = "0.00";
            chkApplyUACash.IsChecked = 0;
            clearDestGrid();
            SearchClear();
            //clear allocated total
            txtRunningOffsetAmt.Text = "0.00";
        }

        /// <summary>
        /// frees up objects for edit
        /// </summary>
        private void setEditScreenState()
        {
            GridCashSource.IsEnabled = true;
            GridCashDestination.IsEnabled = true;
            GridSearch.IsEnabled = true;
            btnClearDest.IsEnabled = true;
            chkApplyUACash.IsEnabled = true;
            BorderDestination.IsEnabled = true;
            BorderSearch.IsEnabled = true;
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
            if (txtRunningOffsetAmt.Text == null)
                return false;
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            string sRunningOffsetAmt = UnformatTextField(txtRunningOffsetAmt.Text.Trim());
            //if (Convert.ToDouble(txtRunningAdjAmt.Text.Trim()) == Convert.ToDouble(txtAdjAmt.Text.Trim()) && (Convert.ToDouble(txtAdjAmt.Text.Trim()) + Convert.ToDouble(txtRunningOffsetAmt.Text.Trim())) == 0)
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) == Convert.ToDouble(sAdjAmt.Trim()) && (Convert.ToDouble(sAdjAmt.Trim()) + Convert.ToDouble(sRunningOffsetAmt.Trim())) == 0)
            {
                //if (Convert.ToDouble(txtAdjAmt.Text) != 0)
                if (Convert.ToDouble(sAdjAmt) != 0)
                {
                    btnSave.IsEnabled = true;
                }
                return true;
            }
            else
            {
                btnSave.IsEnabled = false;
                return false;
            }
        }

        /// <summary>
        /// calls CustomerLookup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCustNumSearch_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on contract ID field
            RazerBase.Lookups.CustomerLookup custLookup = new RazerBase.Lookups.CustomerLookup();
            custLookup.Init(new cBaseBusObject("CustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            custLookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {                
                //load current parms
                //loadParms("");
                txtCustNumSearch.Text = cGlobals.ReturnParms[0].ToString();
                //txtCustNameSearch.Text = cGlobals.ReturnParms[1].ToString();
                cGlobals.ReturnParms.Clear();
            }
        }

        /// <summary>
        /// calls CustomerLookup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCustNameSearch_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on contract ID field
            RazerBase.Lookups.CustomerLookup custLookup = new RazerBase.Lookups.CustomerLookup();
            custLookup.Init(new cBaseBusObject("CustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            custLookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                txtCustNumSearch.Text = cGlobals.ReturnParms[0].ToString();
                //txtCustNameSearch.Text = cGlobals.ReturnParms[1].ToString();
                cGlobals.ReturnParms.Clear();
            }
        }

        /// <summary>
        /// writes unapplied cash record to destination grid
        /// </summary>
        private void chkApplyUACash_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            string sRunningOffsetAmt = UnformatTextField(txtRunningOffsetAmt.Text.Trim());
            //if (Convert.ToDouble(txtRunningAdjAmt.Text) < Convert.ToDouble(txtRunningOffsetAmt.Text))
            if (Convert.ToDouble(sRunningAdjAmt) < Convert.ToDouble(sRunningOffsetAmt))
            {
                //write rec to dest grid
                writeUACashRec();
            }
            else
            {
                Messages.ShowWarning("No Unapplied Cash Remaining");
            }
        }

        private void chkApplyUACash_UnChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            //remove UACash record?

        }

        /// <summary>
        /// writes unapplied cash record to destination grid
        /// </summary>
        private void writeUACashRec()
        {
            //set this to prevent gridDestination single click delegate from firing when GridCashDestination.xGrid.ActiveDataItem = row; runs
            IsSingleClickOrigin = true;
            System.Collections.Generic.List<string> LocationFieldList = new System.Collections.Generic.List<string>();
            LocationFieldList.Add("company_code");
            LocationFieldList.Add("receivable_account");
            LocationFieldList.Add("open_amount");
            LocationFieldList.Add("amount");
            //LocationFieldList.Add("currency_code");
            //LocationFieldList.Add("exchange_rate");
            cGlobals.ReturnParms.Clear();
            GridCashSource.ReturnSelectedData(LocationFieldList);
            if (cGlobals.ReturnParms.Count > 0)
            {
                GridCashDestination.SetGridSelectionBehavior(true, false);
                GridSearch.xGrid.FieldSettings.AllowEdit = true;
                DataView dataSource = this.GridCashDestination.xGrid.DataSource as DataView;
                //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
                //Add new grid row and set cursor in first cell of last row
                DataRowView row = dataSource.AddNew();
                GridCashDestination.xGrid.ActiveDataItem = row;
                GridCashDestination.xGrid.ActiveCell = (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
                //document_id
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = "";
                //seq_code
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = 0;
                //company_code
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[0];
                //product_code
                //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = "";
                //receivable_account
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[1];
                //open_amount
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = 0.00;
                //amount
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = 0.00;
                //Amt to Adj
                string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
                string sRunningOffsetAmt = UnformatTextField(txtRunningOffsetAmt.Text.Trim());
                //(GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = (Convert.ToDouble(txtRunningAdjAmt.Text) * -1) - Convert.ToDouble(txtRunningOffsetAmt.Text);
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = (Convert.ToDouble(sRunningAdjAmt) * -1) - Convert.ToDouble(sRunningOffsetAmt);
                string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
                txtRunningOffsetAmt.Text = (Convert.ToDouble(sAdjAmt) * -1).ToString();
                if (Convert.ToDouble(txtRunningOffsetAmt.Text) < 0) txtRunningOffsetAmt.TextColor = "Red";
                else txtRunningOffsetAmt.TextColor = "Black";
                /////////////////////////////////////////////////////////////////////////////////////////////
                //if amounts offset then enable save button
                txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
                if (AmountsOffset()) btnSave.IsEnabled = true;
                //currency_code
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = "USD";
                //exchange_rate
                (GridCashDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = 0;

                GridCashDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridCashDestination.SetGridSelectionBehavior(false, false);
                GridCashDestination.xGrid.FieldSettings.AllowEdit = true;
                IsSingleClickOrigin = false;
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
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip.ItemsSource = this.CurrentBusObj.ObjectData.Tables["products"].DefaultView;

                    //set the value and display path
                    ip.ValuePath = "product_code";
                    ip.DisplayMemberPath = "product_description";
                    cmbProductGridCombo = ip;
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
            GridSearch.LoadGrid(this.CurrentBusObj, "destination");
            txtRunningOffsetAmt.Text = "0.00";
            chkApplyUACash.IsChecked = 0;
        }

        /// <summary>
        /// call SelectAll method on textbox portion of ucLabelTextbox, highlights text for user
        /// </summary>
        /// <param name="sender"></param>
        private void txtAdjAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is RazerBase.ucLabelTextBox)
                (sender as RazerBase.ucLabelTextBox).SelectAll();

            if (!string.IsNullOrEmpty(txtAdjAmt.Text))
            {

                txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
                txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
                txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
                txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");
            }
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

    }
}
