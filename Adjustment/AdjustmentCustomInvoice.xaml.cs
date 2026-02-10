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

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentApplyUACash' object.
    /// </summary>
    public partial class AdjustmentCustomInvoice : ScreenBase, IPreBindable
    {
        public cBaseBusObject CustomInvoiceAdjustmentBusObj = new cBaseBusObject();
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
        decimal AdjTotalProduct = 0.00M;
        /// <summary> 
        /// Create a new instance of a 'AdjustmentApplyUACash' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentCustomInvoice(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = CustomInvoiceAdjustmentBusObj;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentCustomInvoice";
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
            GridCustomInvoiceSource.xGrid.FieldLayoutSettings = f;
            GridCustomInvoiceDestination.xGrid.FieldLayoutSettings = f;
            GridCustomInvoiceSource.MainTableName = "main";
            GridCustomInvoiceSource.ConfigFileName = "AdjustmentCustomInvoiceSource";
            GridCustomInvoiceSource.FieldLayoutResourceString = "GridCustomInvoiceSource";
            //Set the grid to allow edits, for readonly columns set the allowedit to false in the field layouts file
            GridCustomInvoiceSource.xGrid.FieldSettings.AllowEdit = false;
            GridCustomInvoiceSource.SetGridSelectionBehavior(true, false);
            GridCustomInvoiceSource.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "product_code" }, ChildGrids = { GridCustomInvoiceDestination }, ParentFilterOnColumnNames = { "product_code" } });


            //setup attributes for Child
            GridCustomInvoiceDestination.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            //GridCustomInvoiceDestination.xGrid.RecordActivated += new EventHandler<Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs>(xGrid_RecordActivated);
            //GridCustomInvoiceDestination.xGrid.CellActivating += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellActivatingEventArgs>(xGrid_CellActivating);
            //GridCustomInvoiceDestination.xGrid.CellUpdated += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs>(xGrid_CellUpdated);
            GridCustomInvoiceDestination.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridCustomInvoiceDestination.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridCustomInvoiceDestination.MainTableName = "detail";
            GridCustomInvoiceDestination.ConfigFileName = "AdjustmentGridCustomInvoiceDestination";
            GridCustomInvoiceDestination.FieldLayoutResourceString = "GridCustomInvoiceDestination";
            GridCustomInvoiceDestination.SetGridSelectionBehavior(false, false);
            GridCustomInvoiceDestination.xGrid.FieldSettings.AllowEdit = true;
            GridCustomInvoiceDestination.IsEnabled = false;
            GridCustomInvoiceDestination.SkipReadOnlyCellsOnTab = true;
            GridCustomInvoiceDestination.CellUpdatedDelegate = GridDestination_CellUpdated;
            GridCollection.Add(GridCustomInvoiceSource);
            GridCollection.Add(GridCustomInvoiceDestination);
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
            int edit_index = GridCustomInvoiceDestination.ActiveRecord.Cells.IndexOf(e.Cell);

            //int row_index = GridAdjustment.ActiveRecord.Index;

            //WARNING: You may need to change this if the order of the AdjustmentFieldLayouts\GridConversionInvoice changes
            //if (edit_index == 7) //Amount To Adjust Field
            //{
            //commit user entered value to datatable
            GridCustomInvoiceDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            DataRecord GridRecord = null;
            GridRecord = GridCustomInvoiceDestination.ActiveRecord;
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
                foreach (DataRecord r in GridCustomInvoiceDestination.xGrid.Records)
                {
                    //sum adj amts
                    AdjTotal += Convert.ToDecimal(r.Cells["amount_adjusted"].Value);
                }
                //if amounts offset then enable save button
                //txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
                if (AmountsOffset()) 
                    btnSave.IsEnabled = true;
                else
                    btnSave.IsEnabled = false;
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
            //set this to prevent gridDestination single click delegate from firing when GridCustomInvoiceDestination.xGrid.ActiveDataItem = row; runs
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
                GridCustomInvoiceDestination.SetGridSelectionBehavior(true, false);
                //GridSearch.xGrid.FieldSettings.AllowEdit = true;
                DataView dataSource = this.GridCustomInvoiceDestination.xGrid.DataSource as DataView;
                //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
                //Add new grid row and set cursor in first cell of last row
                DataRowView row = dataSource.AddNew();
                GridCustomInvoiceDestination.xGrid.ActiveDataItem = row;
                GridCustomInvoiceDestination.xGrid.ActiveCell = (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
                //document_id
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
                //seq_code
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
                //company_code
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
                //product_code
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[3];
                //receivable_account
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[4];
                //open_amount
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
                //amount
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
                //Amt to Adj
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = 0.00;
                //currency_code
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = cGlobals.ReturnParms[7];
                //exchange_rate
                (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[8];

                GridCustomInvoiceDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridCustomInvoiceDestination.SetGridSelectionBehavior(false, false);
                GridCustomInvoiceDestination.xGrid.FieldSettings.AllowEdit = true;
                IsSingleClickOrigin = false;
            }
        }

        public void SingleClickZoomDelegateSearch()
        {
            ////set this to prevent gridDestination single click delegate from firing when GridCustomInvoiceDestination.xGrid.ActiveDataItem = row; runs
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
            //    GridCustomInvoiceDestination.SetGridSelectionBehavior(true, false);
            //    GridSearch.xGrid.FieldSettings.AllowEdit = true;
            //    DataView dataSource = this.GridCustomInvoiceDestination.xGrid.DataSource as DataView;
            //    //GridLocationsFinal.xGrid.FieldSettings.AllowEdit = true;
            //    //Add new grid row and set cursor in first cell of last row
            //    DataRowView row = dataSource.AddNew();
            //    GridCustomInvoiceDestination.xGrid.ActiveDataItem = row;
            //    GridCustomInvoiceDestination.xGrid.ActiveCell = (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0];
            //    //document_id
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[0].Value = cGlobals.ReturnParms[0];
            //    //seq_code
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1].Value = cGlobals.ReturnParms[1];
            //    //company_code
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[2].Value = cGlobals.ReturnParms[2];
            //    //product_code
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[3].Value = cGlobals.ReturnParms[3];
            //    //receivable_account
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[4].Value = cGlobals.ReturnParms[4];
            //    //open_amount
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[5].Value = cGlobals.ReturnParms[5];
            //    //amount
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[6].Value = cGlobals.ReturnParms[6];
            //    //Amt to Adj
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[7].Value = 0.00;
            //    //currency_code
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[8].Value = cGlobals.ReturnParms[7];
            //    //exchange_rate
            //    (GridCustomInvoiceDestination.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[9].Value = cGlobals.ReturnParms[8];

            //    GridCustomInvoiceDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //    GridCustomInvoiceDestination.SetGridSelectionBehavior(false, false);
            //    GridCustomInvoiceDestination.xGrid.FieldSettings.AllowEdit = true;
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
            //int loopCnt = 0;
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
           // bool NormalizedError = false;
            //foreach (DataRecord r in GridCustomInvoiceDestination.xGrid.Records)
              //  loopCnt = loopCnt + 1;


                    
           // {
           //     if ((Convert.ToDecimal(r.Cells["amount_adjusted"].Value) != 0) && (r.Cells["normalized_flag"].Value.ToString() == "1") &&
           //         (r.Cells["rebill_flag"].Value.ToString() != "1"))
           //         NormalizedError = true;
           // }
           // if (NormalizedError)
           // {
           //     Messages.ShowError("Cannot adjust normalized line.  Only rebill allowed");
           //     return;
           // }
            if (AmountsOffset())
            {
                string NewDocID = "";
                string SPErrMsg = "";

                NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 3, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));
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
                        totGridRecs("PRODUCT");
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
                    totGridRecs("PRODUCT");
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
            GridCustomInvoiceSource.ReturnSelectedData(SourceGridList);
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
                    if (AdjType == 1)
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
                    if (AmountsOffset()) btnSave.IsEnabled = true;
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
                        GridCustomInvoiceDestination.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
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
            GridCustomInvoiceSource.IsEnabled = true;
            GridCustomInvoiceDestination.IsEnabled = true;
            BorderDestination.IsEnabled = true;
            txtRunningAdjAmt.Text = "0.00";
            //foreach (DataRecord r in GridCustomInvoiceDestination.xGrid.Records)
            //{
            //    if (Convert.ToInt32(r.Cells["normalized_flag"].Value) == 1)
            //        //r.Cells["total_adjusted"].Settings.AllowEdit = false;
            //        GridCustomInvoiceDestination.xGrid.FieldLayouts[0].Fields["amount_adjusted"].Settings.AllowEdit = false;
            //    //gAlloc.xGrid.FieldLayouts[0].Fields["receivable_account"].Settings.AllowEdit = false;
            //    //Index = Index + 1;
            //}
        }

        /// <summary>
        /// frees up objects for edit
        /// </summary>
        private void setEditScreenState()
        {
            //GridCustomInvoiceSource.IsEnabled = true;
            GridCustomInvoiceDestination.IsEnabled = true;
            //btnClearDest.IsEnabled = true;
            BorderDestination.IsEnabled = true;
            //Set the amount_adjusted field as active
            if (this.CurrentBusObj.ObjectData != null)
                if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0)
                    (GridCustomInvoiceDestination.xGrid.Records[GridCustomInvoiceDestination.ActiveRecord.Index] as DataRecord).Cells["amount_adjusted"].IsActive = true;
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

        //void xGrid_RecordActivated(object sender, Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs e)
        //{
        //    //DataRecord GridRecord = null;
        //    //Cell GridCell = null;

        //    //GridRecord = GridCustomInvoiceDestination.ActiveRecord;
        //    //GridCell = GridCustomInvoiceDestination.CurrentActiveEditCell;

        //    //if (GridCustomInvoiceDestination.CurrentActiveEditCell != null) 
        //    //if ((GridCell.Field.Name == "amount_adjusted") && (GridRecord.Cells["normalized_flag"].Value.ToString() == "1"))
        //    //{
        //    //    Messages.ShowError("Cannot adjust normalized line.  Only rebill allowed");
        //    //    GridRecord.Cells["amount_adjusted"].Value = 0.00;
        //    //}

        //}

        //void xGrid_CellActivating(object sender, Infragistics.Windows.DataPresenter.Events.CellActivatingEventArgs e)
        //{
        //    DataRecord GridRecord = null;
        //    //Cell GridCell = null;

        //    GridRecord = GridCustomInvoiceDestination.ActiveRecord;
        //    //GridCell = GridCustomInvoiceDestination.CurrentActiveEditCell;

        //    if ((e.Cell.Field.Name == "amount_adjusted") && (GridRecord.Cells["normalized_flag"].Value.ToString() == "1"))
        //        {
        //            Messages.ShowError("Cannot adjust normalized line.  Only rebill allowed");
        //            GridRecord.Cells["amount_adjusted"].Value = 0.00;
        //        }

        //}

        //void xGrid_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        //{
        //    DataRecord GridRecord = null;
        //    //Cell GridCell = null;

        //    GridRecord = GridCustomInvoiceDestination.ActiveRecord;
        //    //GridCell = GridCustomInvoiceDestination.CurrentActiveEditCell;

        //    if ((e.Cell.Field.Name == "amount_adjusted") && (GridRecord.Cells["normalized_flag"].Value.ToString() == "1"))
        //    {
        //        Messages.ShowError("Cannot adjust normalized line.  Only rebill allowed");
        //        GridRecord.Cells["amount_adjusted"].Value = 0.00;
        //    }

        //}

        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = GridCustomInvoiceDestination.ActiveRecord.Cells.IndexOf(e.Cell);
            //DataRecord GridParent = GridCustomInvoiceSource.ActiveRecord;
            //double NewAmountAdjusted = 0;
            
            //int row_index = GridAdjustment.ActiveRecord.Index;

            DataRecord GridRecord = null;
            //if (edit_index == 10) //Amount To Adjust Field
            if (e.Cell.Field.Name == "amount_adjusted")
            {
                //commit user entered value to datatable
                GridCustomInvoiceDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                //DataRecord GridRecord2 = null;
                GridRecord = GridCustomInvoiceDestination.ActiveRecord;
                DataRowView dr = GridRecord.DataItem as DataRowView;
                DataView dv = dr.DataView;
                if (GridRecord != null)
                {
                    //clear running total
                    txtRunningAdjAmt.Text = "";
                    if (GridRecord.Cells["amount_adjusted"].Value.ToString() == "")
                        GridRecord.Cells["amount_adjusted"].Value = 0.00;

                    //Total Grid Cells and update main grid
                    totGridRecs(GridRecord.Cells["product_code"].Value.ToString());
                    DataRecord GridParent = GridCustomInvoiceSource.ActiveRecord;
                    GridParent.Cells["amount_adjusted"].Value = AdjTotalProduct;
                    GridParent.Cells["apply_to_doc"].Value = GridParent.Cells["document_id"].Value;
                    GridParent.Cells["apply_to_seq"].Value = GridParent.Cells["seq_code"].Value;
                    GridCustomInvoiceSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    //CurrentBusObj.ObjectData.Tables["main"].AcceptChanges();
   
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0) txtRunningAdjAmt.TextColor = "Red";
                    else txtRunningAdjAmt.TextColor = "Black";
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    if (AmountsOffset()) btnSave.IsEnabled = true;
                 }
            }
        }

        /// <summary>
        /// totals grid cells and adds to running total
        /// </summary>
        private void totGridRecs(string product_code)
        {
            //init AdjTotal w/starting value
            AdjTotal = 0.00M;
            AdjTotalProduct = 0.00M;
            //loop through each grid record and add adj totals
            foreach (DataRecord r in GridCustomInvoiceDestination.xGrid.Records)
            {
                //sum adj amts
                AdjTotal += Convert.ToDecimal(r.Cells["amount_adjusted"].Value);

                if (r.Cells["product_code"].Value.ToString() == product_code)
                {
                    AdjTotalProduct += Convert.ToDecimal(r.Cells["amount_adjusted"].Value);
                }
            }
            txtRunningAdjAmt.Text = AdjTotal.ToString("0.00");

            //foreach (DataRecord r in GridCustomInvoiceDestination.xGrid.Records)
            //{
            //    if (Convert.ToDouble(r.Cells["amount_adjusted"].Value) != 0)
            //        r.Cells["total_adjusted"].Value = AdjTotal;
            //}

            //DataRecord GridParent = GridCustomInvoiceSource.ActiveRecord;
            //GridParent.Cells["amount_adjusted"].Value = AdjTotal;
            GridCustomInvoiceSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //CurrentBusObj.ObjectData.Tables["main"].AcceptChanges();

        }

        private void updGridRowsDocId(string DocID)
        {
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            int apply_to_seq = 0;
            string receivable_account = "";
            int adjustment_type = 3;
            string currency_code = "USD";
            string company_code = "";
            string gl_center = "";
            string gl_acct = "";
            string geography = "";
            string interdivision = "";
            string gl_product = "";
            DataRecord r = GridCustomInvoiceSource.ActiveRecord;
            DataRow row = (r.DataItem as DataRowView).Row;
            if (row != null)
            {
                //apply_to_seq = Convert.ToInt32(row["seq_code"]);
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
                bool rowIsNotModified = (dr.RowState != DataRowState.Modified);
                dr["adj_document_id"] = DocID;
                //dr["apply_to_seq"] = apply_to_seq;
                dr["receivable_account"] = receivable_account;
                dr["adjustment_type"] = adjustment_type;
                dr["currency_code"] = currency_code;
                //dr["company_code"] = company_code;
                //dr["gl_center"] = gl_center;
                //dr["gl_acct"] = gl_acct;
                //dr["geography"] = geography;
                //dr["interdivision"] = interdivision.Trim();
                //dr["gl_product"] = gl_product;
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
                GridCustomInvoiceDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridRecord = GridCustomInvoiceDestination.ActiveRecord;
                if (GridRecord != null)
                {
                    //clear running total
                    txtRunningAdjAmt.Text = "";
                    if (GridRecord.Cells["rebill_flag"].Value.ToString() == "1")
                    {
                        GridRecord.Cells["amount_adjusted"].Value = (Convert.ToDecimal(GridRecord.Cells["extended"].Value) +
                                                                     Convert.ToDecimal(GridRecord.Cells["amount_change"].Value)) * -1;
                        GridCustomInvoiceDestination.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    }

                    else
                        GridRecord.Cells["amount_adjusted"].Value = 0.00;
                    //Total Grid Cells
                    totGridRecs("PRODUCT");
                    DataRecord GridParent = GridCustomInvoiceSource.ActiveRecord;
                    GridParent.Cells["amount_adjusted"].Value = AdjTotal;
                    GridParent.Cells["apply_to_doc"].Value = GridParent.Cells["document_id"].Value;
                    GridParent.Cells["apply_to_seq"].Value = GridParent.Cells["seq_code"].Value;
                    GridCustomInvoiceSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0) txtRunningAdjAmt.TextColor = "Red";
                    else txtRunningAdjAmt.TextColor = "Black";
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    if (AmountsOffset()) btnSave.IsEnabled = true;
                }
            }
        }
        //private void GridDestination_BeforeCellActivate(object sender, Infragistics.Windows.UltraWinGrid.CancelableCellEventArgs e)
        //{
        //    //Infragistics.Win.UltraWinGrid.CancelableCellEventArgs e
        //}

    }
}

