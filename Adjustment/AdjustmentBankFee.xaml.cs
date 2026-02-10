using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter;
using RazerBase.Lookups;
using System.Windows.Input;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentBankFee' object.
    /// </summary>
    public partial class AdjustmentBankFee : ScreenBase, IPreBindable
    {
        public cBaseBusObject BankFeeAdjustmentBusObj = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        private bool IsSingleClickOrigin { get; set; }
        public ComboBoxItemsProvider cmbProductGridCombo { get; set; }
        private string DocumentId { get; set; }

        /// <summary>
        /// Create a new instance of a 'AdjustmentApplyUACash' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentBankFee(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = BankFeeAdjustmentBusObj;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentBankFee";
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
            //GridCashSource.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            //GridCashSource.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            //GridCashSource.ContextMenuAddIsVisible = false;
            //GridCashSource.ContextMenuRemoveIsVisible = false;
            //GridCashSource.SingleClickZoomDelegate = SingleClickZoomDelegateSource;
            //GridCashSource.MainTableName = "main";
            //GridCashSource.ConfigFileName = "AdjustmentCashSource";
            //GridCashSource.SetGridSelectionBehavior(true, false);
            //GridCashSource.xGrid.FieldSettings.AllowEdit = false;
            //GridCashSource.FieldLayoutResourceString = "GridApplyUACashSource";
            //GridCollection.Add(GridCashSource);

            GridBankFeeDestination.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(GridDestination_EditModeEnded);
            GridBankFeeDestination.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridBankFeeDestination.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridBankFeeDestination.ContextMenuAddIsVisible = false;
            GridBankFeeDestination.ContextMenuRemoveIsVisible = false;
            GridBankFeeDestination.MainTableName = "destination";
            GridBankFeeDestination.ConfigFileName = "AdjustmentGridBankFeeDestination";
            GridSearch.SetGridSelectionBehavior(true, false);
            GridBankFeeDestination.xGrid.FieldSettings.AllowEdit = true;
            GridBankFeeDestination.FieldLayoutResourceString = "GridBankFeeDestination";
            GridCollection.Add(GridBankFeeDestination);

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

            txtAdjAmt.CntrlFocus();

        }

        /// <summary>
        /// works as cellLeave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GridDestination_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = GridBankFeeDestination.ActiveRecord.Cells.IndexOf(e.Cell);

            //int row_index = GridAdjustment.ActiveRecord.Index;

            //WARNING: You may need to change this if the order of the AdjustmentFieldLayouts\GridConversionInvoice changes
            //if (edit_index == 7) //Amount To Adjust Field
            //{
            //commit user entered value to datatable
            GridBankFeeDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            DataRecord GridRecord = null;
            GridRecord = GridBankFeeDestination.ActiveRecord;
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
                foreach (DataRecord r in GridBankFeeDestination.xGrid.Records)
                {
                    //sum adj amts
                    AdjTotal += Convert.ToDouble(r.Cells["amt_to_adjust"].Value);
                }
                AdjTotal = AdjTotal * -1;
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
            //set this to prevent gridDestination single click delegate from firing when GridBankFeeDestination.xGrid.ActiveDataItem = row; runs
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
            //LocationFieldList.Add("adj_doc_id");
            cGlobals.ReturnParms.Clear();
            GridSearch.ReturnSelectedData(LocationFieldList);
            if (cGlobals.ReturnParms.Count > 0)
            {
                GridBankFeeDestination.SetGridSelectionBehavior(true, false);
                GridSearch.xGrid.FieldSettings.AllowEdit = true;
                DataView dataSource = this.GridBankFeeDestination.xGrid.DataSource as DataView;
                //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
                //Add new grid row and set cursor in first cell of last row
                DataRowView row = dataSource.AddNew();
                GridBankFeeDestination.xGrid.ActiveDataItem = row;
                GridBankFeeDestination.xGrid.ActiveCell = (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
                //document_id
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
                //seq_code
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
                //company_code
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
                //product_code
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[3];
                //receivable_account
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[4];
                //open_amount
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
                //amount
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
                //Amt to Adj
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = 0.00;
                //currency_code
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = cGlobals.ReturnParms[7];
                //exchange_rate
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[8];
                //adj_doc_id
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[8];

                GridBankFeeDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridBankFeeDestination.SetGridSelectionBehavior(false, false);
                GridBankFeeDestination.xGrid.FieldSettings.AllowEdit = true;
                IsSingleClickOrigin = false;
            }
        }

        public void SingleClickZoomDelegateSearch()
        {
            ////set this to prevent gridDestination single click delegate from firing when GridBankFeeDestination.xGrid.ActiveDataItem = row; runs
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
            //    GridBankFeeDestination.SetGridSelectionBehavior(true, false);
            //    GridSearch.xGrid.FieldSettings.AllowEdit = true;
            //    DataView dataSource = this.GridBankFeeDestination.xGrid.DataSource as DataView;
            //    //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
            //    //Add new grid row and set cursor in first cell of last row
            //    DataRowView row = dataSource.AddNew();
            //    GridBankFeeDestination.xGrid.ActiveDataItem = row;
            //    GridBankFeeDestination.xGrid.ActiveCell = (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
            //    //document_id
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
            //    //seq_code
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
            //    //company_code
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
            //    //product_code
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[3];
            //    //receivable_account
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[4];
            //    //open_amount
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
            //    //amount
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
            //    //Amt to Adj
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = 0.00;
            //    //currency_code
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = cGlobals.ReturnParms[7];
            //    //exchange_rate
            //    (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[8];

            //    GridBankFeeDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //    GridBankFeeDestination.SetGridSelectionBehavior(false, false);
            //    GridBankFeeDestination.xGrid.FieldSettings.AllowEdit = true;
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

        public override void Save()
        {
            this.SaveSuccessful = false;
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
             
                foreach (DataRow row in this.CurrentBusObj.ObjectData.Tables["destination"].Rows)
                {
                    if (row["product_code"].ToString() == "")
                    {
                        Messages.ShowWarning("No Product Code Selected for Bank Fee Destination, Save Cancelled");
                        return;
                    }
                }



                //call method to insert a new adj and get new doc Id
                
                NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(" ", 11, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt) * -1);
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
                        //totGridRecs("PRODUCT");
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
                    //totGridRecs("PRODUCT");
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
                //strArr[0] = txtDocument.Text;
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
            //GridCashSource.ReturnSelectedData(SourceGridList);
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
            //clear dest grid & set state
            clearDestGrid();
            if (txtCustNumSearch.Text == null || txtCustNumSearch.Text == "")
            {
                Messages.ShowError("Please enter a Customer number.");
                txtCustNumSearch.CntrlFocus();
                return;

            };
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
        /// This event handles the double click launching of a lookup window.  If a value is returned from the lookup window
        /// then the data for the window is requiered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void txtDocument_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    AdjustmentUACashLookup w = new AdjustmentUACashLookup();
        //    w.ShowDialog();
        //    if (cGlobals.ReturnParms.Count > 0)
        //    {
        //        txtDocument.Text = cGlobals.ReturnParms[0].ToString();
        //        // Clear the Global parms
        //        //This prevents invalid data being passed to other lookups
        //        cGlobals.ReturnParms.Clear();
        //        CurrentState = ScreenState.Normal;
        //        //loadParms();
        //        //this.Load();
        //    }
        //}

        /// <summary>
        /// enables adj amt and pop cash source grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void txtDocument_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    if (txtDocument.Text != "")
        //    {
        //        //do this to keep grid from loading and binding re-running clearing user entered values
        //        if (DocumentId != txtDocument.Text)
        //        {
        //            //populate grid
        //            popGrid();
        //        }
        //    }
        //}

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
            if (Convert.ToDouble(txtAdjAmt.Text) >= 0)
            {
                Messages.ShowInformation("Amount must be negative");
                Double negResult = (Convert.ToDouble(txtAdjAmt.Text) * -1.0);
                txtAdjAmt.Text = negResult.ToString();
                //txtAdjAmt.CntrlFocus();
                
            }
            ///////////////////////////////////////////////////////////////////////////
            //set to edit mode if val != 0
            if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            {
                setEditScreenState();
                //if amt < 0 make neg set to red
                if (Convert.ToDouble(txtAdjAmt.Text) > 0)
                {
                    //txtAdjAmt.Text = (Convert.ToDouble(txtAdjAmt.Text) * -1).ToString();
                    //txtAdjAmt.TextColor = "Red";
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
            //txtRunningAdjAmt.TextColor = "Red";
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
                txtAdjAmt.IsReadOnly = false;
                setInitScreenState();
            }
            else
            {
                Messages.ShowInformation("Document Not Found");
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
            //this.CurrentBusObj.Parms.AddParm("@document_id", txtDocument.Text);
            this.CurrentBusObj.Parms.AddParm("@receivable_account", "");
            this.CurrentBusObj.Parms.AddParm("@invoice_number", "");
            this.CurrentBusObj.Parms.AddParm("@amount", 0);
            //this.CurrentBusObj.Parms.AddParm("@seq_code", 0);
            //DocumentId = txtDocument.Text;
        }

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            btnSave.IsEnabled = false;
            //GridCashSource.IsEnabled = false;
            GridBankFeeDestination.IsEnabled = false;
            GridSearch.IsEnabled = false;
            //chkApplyUACash.IsEnabled = false;
            BorderSearch.IsEnabled = true;
            BorderDestination.IsEnabled = false;
            btnClearDest.IsEnabled = false;
            txtRunningAdjAmt.Text = "0.00";
            //chkApplyUACash.IsChecked = 0;
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
            //GridCashSource.IsEnabled = true;
            GridBankFeeDestination.IsEnabled = true;
            GridSearch.IsEnabled = true;
            btnClearDest.IsEnabled = true;
            //chkApplyUACash.IsEnabled = true;
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
            if (Convert.ToDouble(sRunningAdjAmt) * -1 > Convert.ToDouble(sRunningOffsetAmt))
            {
                //write rec to dest grid
                writeUACashRec();
            }
            else
            {
                Messages.ShowWarning("No Unapplied Cash Remaining");
                //chkApplyUACash.IsChecked = 0;
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
            //set this to prevent gridDestination single click delegate from firing when GridBankFeeDestination.xGrid.ActiveDataItem = row; runs
            IsSingleClickOrigin = true;
            System.Collections.Generic.List<string> LocationFieldList = new System.Collections.Generic.List<string>();
            LocationFieldList.Add("company_code");
            LocationFieldList.Add("receivable_account");
            LocationFieldList.Add("open_amount");
            LocationFieldList.Add("amount");
            //LocationFieldList.Add("currency_code");
            //LocationFieldList.Add("exchange_rate");
            cGlobals.ReturnParms.Clear();
            //GridCashSource.ReturnSelectedData(LocationFieldList);
            if (cGlobals.ReturnParms.Count > 0)
            {
                GridBankFeeDestination.SetGridSelectionBehavior(true, false);
                GridSearch.xGrid.FieldSettings.AllowEdit = true;
                DataView dataSource = this.GridBankFeeDestination.xGrid.DataSource as DataView;
                //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
                //Add new grid row and set cursor in first cell of last row
                DataRowView row = dataSource.AddNew();
                GridBankFeeDestination.xGrid.ActiveDataItem = row;
                GridBankFeeDestination.xGrid.ActiveCell = (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
                //document_id
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = "";
                //seq_code
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = 0;
                //company_code
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[0];
                //product_code
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = "";
                //receivable_account
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[1];
                //open_amount
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = 0.00;
                //amount
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = 0.00;
                //Amt to Adj
                string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
                string sRunningOffsetAmt = UnformatTextField(txtRunningOffsetAmt.Text.Trim());
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = (Convert.ToDouble(sRunningAdjAmt) * -1) - Convert.ToDouble(sRunningOffsetAmt);
                string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
                txtRunningOffsetAmt.Text = (Convert.ToDouble(sAdjAmt) * -1).ToString();
                if (Convert.ToDouble(txtRunningOffsetAmt.Text) < 0) txtRunningOffsetAmt.TextColor = "Red";
                else txtRunningOffsetAmt.TextColor = "Black";
                /////////////////////////////////////////////////////////////////////////////////////////////
                //if amounts offset then enable save button
                txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
                if (AmountsOffset()) btnSave.IsEnabled = true;
                //currency_code
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = "USD";
                //exchange_rate
                (GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = 0;
                
                //apply_to_seq
                //(GridBankFeeDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[10].Value = 0;

                GridBankFeeDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridBankFeeDestination.SetGridSelectionBehavior(false, false);
                GridBankFeeDestination.xGrid.FieldSettings.AllowEdit = true;
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

        private void updGridRowsDocId(string DocID)
        {
            //string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            //int apply_to_seq = 0;
            //string receivable_account = "";
            //int adjustment_type = 11;
            //string currency_code = "USD";
            //string company_code = "";
            //string gl_center = "";
            //string gl_acct = "";
            //string geography = "";
            //string interdivision = "";
            //string gl_product = "";
            //DataRecord r =   .ActiveRecord;
            //DataRow row = (r.DataItem as DataRowView).Row;
            //if (row != null)
            //{
            //    apply_to_seq = Convert.ToInt32(row["seq_code"]);
            //    receivable_account = Convert.ToString(row["receivable_account"]);
            //    //if (Convert.ToDouble(sAdjAmt) > 0) adjustment_type = 1;
            //    currency_code = Convert.ToString(row["currency_code"]);
            //    company_code = Convert.ToString(row["company_code"]);
            //    gl_center = Convert.ToString(row["gl_center"]);
            //    gl_acct = Convert.ToString(row["gl_acct"]);
            //    geography = Convert.ToString(row["geography"]);
            //    interdivision = Convert.ToString(row["interdivision"]);
            //    gl_product = Convert.ToString(row["gl_product"]);
            //}

            //loop through main data table and update columns
            //foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["main"].Rows)
            //{
            //    bool rowIsNotModified = (dr.RowState != DataRowState.Modified);
            //    dr["adj_document_id"] = DocID;
            //    if (rowIsNotModified)
            //    {
            //        dr.AcceptChanges();
            //    }
            //    else
            //    {
            //        dr.AcceptChanges();
            //        dr.SetAdded();
            //    }
            //}

            //loop through detail data table and update columns
          
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["destination"].Rows)
            {
                bool rowIsNotModified = (dr.RowState != DataRowState.Modified);
                dr["adj_document_id"] = DocID;
                //dr["apply_to_seq"] = apply_to_seq;
                //dr["receivable_account"] = receivable_account;
                //dr["adjustment_type"] = adjustment_type;
                //dr["currency_code"] = currency_code;
                //dr["company_code"] = company_code;
                //dr["gl_center"] = gl_center;
                //dr["gl_acct"] = gl_acct;
                //dr["geography"] = geography;
                //dr["interdivision"] = interdivision.Trim();
                //dr["gl_product"] = gl_product;
                if (rowIsNotModified)
                {
                    dr.AcceptChanges();
                    dr.SetAdded();
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



        /// <summary>
        /// Use the event to retrieve data into the base business object
        /// For Insert make sure to set CurrentState to Inserting
        /// </summary>
        /// <returns></returns>
      

    }
}

