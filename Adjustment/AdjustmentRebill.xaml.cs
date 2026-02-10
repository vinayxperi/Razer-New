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
//using System.Windows.Forms;
using System.Windows;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentApplyUACash' object.
    /// </summary>
    public partial class AdjustmentRebill : ScreenBase, IPreBindable
    {
        public cBaseBusObject RebillAdjustmentBusObj = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        public Int32 AdjType = 16;
        private bool IsSingleClickOrigin { get; set; }
        private string DocumentId { get; set; }
        //This datatable is being added so that the Amount to adjust text box can have a binding
        //Do this for fields that contain informational data but that will not be saved
        //so that you can use converters or other benefits of binding
        DataTable dtMiscInfo = new DataTable("MiscInfo");
        double AdjTotal = 0;
        bool bUncheckLine = false;
        /// <summary>
        /// Create a new instance of a 'AdjustmentApplyUACash' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentRebill(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = RebillAdjustmentBusObj;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentRebill";
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
            GridRebillSource.xGrid.FieldLayoutSettings = f;
            GridRebillDestination.xGrid.FieldLayoutSettings = f;
            GridRebillSource.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            GridRebillSource.MainTableName = "main";
            GridRebillSource.ConfigFileName = "AdjustmentGridRebillSource";
            GridRebillSource.FieldLayoutResourceString = "GridRebillSource";
            //Set the grid to allow edits, for readonly columns set the allowedit to false in the field layouts file
            GridRebillSource.xGrid.FieldSettings.AllowEdit = true;
            GridRebillSource.SetGridSelectionBehavior(true, false);
            //GridRebillSource.IsEnabled = false;
            GridRebillSource.SkipReadOnlyCellsOnTab = true;
            GridRebillSource.CellUpdatedDelegate = GridSource_CellUpdated;
            GridRebillSource.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "product_code" }, ChildGrids = { GridRebillDestination }, ParentFilterOnColumnNames = { "product_code" } });

            //setup attributes for Child
            GridRebillDestination.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridRebillDestination.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridRebillDestination.MainTableName = "detail";
            GridRebillDestination.ConfigFileName = "AdjustmentGridRebillDestination";
            GridRebillDestination.FieldLayoutResourceString = "GridRebillDestination";
            GridRebillDestination.SetGridSelectionBehavior(false, false);
            GridRebillDestination.xGrid.FieldSettings.AllowEdit = false;
            GridRebillDestination.IsEnabled = false;
            GridRebillDestination.SkipReadOnlyCellsOnTab = true;
            GridCollection.Add(GridRebillSource);
            GridCollection.Add(GridRebillDestination);
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
            int edit_index = GridRebillSource.ActiveRecord.Cells.IndexOf(e.Cell);

            //int row_index = GridAdjustment.ActiveRecord.Index;

            //WARNING: You may need to change this if the order of the AdjustmentFieldLayouts\GridConversionInvoice changes
            //if (edit_index == 7) //Amount To Adjust Field
            //{
            //commit user entered value to datatable
            GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            DataRecord GridRecord = null;
            GridRecord = GridRebillSource.ActiveRecord;
            DataRowView dr = GridRecord.DataItem as DataRowView;
            DataView dv = dr.DataView;
            if (GridRecord != null)
            {
                //clear running total
                //init AdjTotal w/starting value
                double AdjTotal = 0.00;
                if (GridRecord.Cells["amount_adjusted"].Value.ToString() == "")
                {
                    GridRecord.Cells["amount_adjusted"].Value = 0.00;
                }
                //loop through each grid record and add adj totals
                foreach (DataRecord r in GridRebillSource.xGrid.Records)
                {
                    //sum adj amts
                    AdjTotal += Convert.ToDouble(r.Cells["amount_adjusted"].Value);
                }
                //if amounts offset then enable save button
                if (AmountsOffset()) 
                    btnSave.IsEnabled = true;
                else
                    btnSave.IsEnabled = false;
            }
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

        //Set all rebill_flags to true
        private void btnRebillAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (DataRecord r in GridRebillSource.xGrid.Records)
            {

                if (r.Cells["rebill_flag"].Value.ToString() == "0")
                {
                    r.Cells["rebill_flag"].Value = 1;
                   
                    //r.Cells["rebill_flag"]. = true;
                    //Cell activeCell = r.Cells["rebill_flag"];
                    //GridRebillSource.xGrid.ActiveCell = activeCell;
                    //GridRebillSource.xGrid.ActiveCell.IsSelected = true;

                    //CellValuePresenter cvp = CellValuePresenter.FromCell(activeCell);
                    //cvp.Value = 1;

                    //if (cvp.Editor is XamCheckEditor)
                    //{
                    //    XamCheckEditor editor = cvp.Editor as XamCheckEditor;
                    //    editor.IsChecked = true;
                    //    editor.EndEditMode(true, true);
                    //}

                    //e.Editor.ValueChanged += new RoutedPropertyChangedEventHandler<object>(Editor_ValueChanged);
                    //editor.EndEditMode(true, true);

                }
                else
                {
                    r.Cells["rebill_flag"].Value = 0;
                    //r.Cells["rebill_flag"].IsSelected = false;
                }
                //GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            }
        }
         
        public override void Save()
        {
            this.SaveSuccessful = false;
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
       
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) == 0)
            {
                Messages.ShowWarning("Adjusted amount is 0. Save Cancelled");
                return;
            }
            string NewDocID = "";
            string SPErrMsg = "";

            //Loop through grid checking for any normalized lines that have been adjusted
            //RES 7/9/12 added allow_crdb indicator for adjustments to normalized lines
            //bool NormalizedError = false;
            bool DefPoolError = false;
            bool Normalized = false;
            string NInovoices = "";
            foreach (DataRecord r in GridRebillSource.xGrid.Records)
            {
                //if ((Convert.ToDecimal(r.Cells["amount_adjusted"].Value) != 0) && (r.Cells["normalized_flag"].Value.ToString() == "1") &&
                //    (r.Cells["rebill_flag"].Value.ToString() != "1") && (r.Cells["allow_crdb"].Value.ToString() != "1"))
                //    NormalizedError = true;

                if ((Convert.ToDecimal(r.Cells["amount_adjusted"].Value) != 0) && (r.Cells["normalized_flag"].Value.ToString() == "1") &&
                    (r.Cells["allow_crdb"].Value.ToString() != "1") && (r.Cells["def_pool_has_run"].Value.ToString() != "1"))
                    DefPoolError = true;

                if ((Convert.ToDecimal(r.Cells["amount_adjusted"].Value) != 0) && (r.Cells["normalized_flag"].Value.ToString() == "1") &&
                    (r.Cells["ninvoices"].Value.ToString() != ""))
                {
                    Normalized = true;
                    NInovoices = r.Cells["ninvoices"].Value.ToString();
                }
            }
            //if (NormalizedError)
            //{
            //    Messages.ShowError("Cannot adjust normalized line.  Only rebill allowed.");
            //    return;
            //}
            if (DefPoolError)
            {
                Messages.ShowError("Cannot rebill normalized line before Def Pool Job has run.  Run Def Pool Job then create adjustment.");
                return;
            }
            MessageBoxResult result1;
            if (Normalized)   
            {
                result1 = Messages.ShowYesNo("Rebilling current invoice will cause the following invoice(s) to be rebilled: " +
                                              NInovoices + "  Do you want to continue?",System.Windows.MessageBoxImage.Warning);
                if (result1 == MessageBoxResult.No) return;
            }

            NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, AdjType, cGlobals.UserName.ToLower(), Convert.ToDecimal(sRunningAdjAmt));
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
                            AdjParent.Close();
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
                ////adj amt
                //strArr[2] = UnformatTextField(txtAdjAmt.Text);
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
            GridRebillSource.ReturnSelectedData(SourceGridList);
            return SourceGridList;
        }

        /// <summary>
        /// clears seach fields and grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    //SearchClear();
        //}

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
                    chkRebillAll.IsChecked = 0;
                    //populate grid
                    popGrid();
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
                setInitScreenState();
                if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["unposted_adjustments"]) > 0)
                {
                    Messages.ShowInformation("There are unposted adjustment(s) for this document");
                    txtDocument.CntrlFocus();
                    chkRebillAll.IsEnabled = false;
                    GridRebillSource.IsEnabled = false;
                    //GridRebillDestination.IsEnabled = false;
                    return;
                }
                // RES 11/14/16 Check for WHT adjustments that have not been reversed and show warning message
                if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["wht_amount"]) != 0)
                {
                    Messages.ShowWarning("There are WHT adjustment(s) for this document that have not been reversed");
                    chkRebillAll.IsEnabled = true;
                    GridRebillSource.IsEnabled = true;
                }
                else
                {
                    chkRebillAll.IsEnabled = true;
                    GridRebillSource.IsEnabled = true;
                }
                //RES 5/8/19 Check for SLA adjustments and issue warning
                if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["sla_amount"]) != 0)
                {
                    Messages.ShowWarning("There have been SLA adjustment(s) applied to this document");                    
                }
                //txtAdjAmt.IsReadOnly = true;
                //setInitScreenState();
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
                //txtAdjAmt.IsReadOnly = false;
                //setInitScreenState();
            }
            else
            {
                Messages.ShowInformation("Document Detail Not Found");
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
            DocumentId = txtDocument.Text;
        }

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            btnSave.IsEnabled = false;
            GridRebillSource.IsEnabled = true;
            BorderSource.IsEnabled = true;
            GridRebillDestination.IsEnabled = true;
            BorderDestination.IsEnabled = true;
            txtRunningAdjAmt.Text = "0.00";
        }

        /// <summary>
        /// frees up objects for edit
        /// </summary>
        private void setEditScreenState()
        {
            GridRebillSource.IsEnabled = true;
            BorderSource.IsEnabled = true;
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
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
          
            if (Convert.ToDouble(sRunningAdjAmt) != 0)
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
            int edit_index = GridRebillSource.ActiveRecord.Cells.IndexOf(e.Cell);
       
            DataRecord GridRecord = null;
            //if (edit_index == 10) //Amount To Adjust Field
            if (e.Cell.Field.Name == "amount_adjusted")
            {
                //commit user entered value to datatable
                GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridRecord = GridRebillSource.ActiveRecord;
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
                    GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
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
        private void totGridRecs()
        {
            //init AdjTotal w/starting value
            AdjTotal = 0.00;
            //loop through each grid record and add adj totals
            foreach (DataRecord r in GridRebillSource.xGrid.Records)
            {
                //sum adj amts
                AdjTotal += Convert.ToDouble(r.Cells["amount_adjusted"].Value);
                //if (r.Cells["rebill_flag"].Value.ToString() == "1")
                //    foreach (DataRecord r2 in GridRebillDestination.xGrid.Records)
                //    {
                //        if (r.Cells["product_code"].Value.ToString() == r2.Cells["product_code"].Value.ToString())
                //            r2.Cells["rebill_flag"].Value = 1; ;
                //    }
            }
            txtRunningAdjAmt.Text = AdjTotal.ToString("0.00");

            //foreach (DataRecord r in GridRebillDestination.xGrid.Records)
            //{
            //    if (Convert.ToDouble(r.Cells["amount_adjusted"].Value) != 0)
            //        r.Cells["total_adjusted"].Value = AdjTotal;
            //}

            GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //CurrentBusObj.ObjectData.Tables["main"].AcceptChanges();

        }

        private void updGridRowsDocId(string DocID)
        {
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

        private void GridSource_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            double temp_amount_adjusted = 0;
            if (e.Cell.Field.Name == "rebill_flag")
            {
                DataRecord GridRecord = null;
                //GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                //RES 12/29/14 fixed issues when using rebill all on invoices with multiple product codes
                //GridRecord = GridRebillSource.ActiveRecord;
                GridRecord = e.Cell.Record;
                if (GridRecord != null)
                {
                    //clear running total
                    txtRunningAdjAmt.Text = "";
                    if (GridRecord.Cells["rebill_flag"].Value.ToString() == "1")
                    {

                        //temp_amount_adjusted = (Convert.ToDouble(GridRecord.Cells["amount"].Value) + Convert.ToDouble(GridRecord.Cells["prior_debit_adjusted"].Value) +
                        //                                             Convert.ToDouble(GridRecord.Cells["prior_adjusted"].Value)) * -1;
                        //temp_amount_adjusted = (Convert.ToDouble(GridRecord.Cells["amount"].Value) + Convert.ToDouble(GridRecord.Cells["prior_debit_adjusted"].Value)) * -1;
                        temp_amount_adjusted = Convert.ToDouble(GridRecord.Cells["amount"].Value) * -1;
                        GridRecord.Cells["amount_adjusted"].Value = temp_amount_adjusted;
                        GridRecord.Cells["apply_to_doc"].Value = GridRecord.Cells["document_id"].Value;
                        //GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                        GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                    }

                    else
                    {
                        GridRecord.Cells["amount_adjusted"].Value = 0.00;
                        bUncheckLine = true;
                        chkRebillAll.IsChecked = 0;
                    }
                    //Total Grid Cells
                    totGridRecs();
                    //GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
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
        private void chkRebillAll_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //Loop through and check all batches to include
            //DataTable dtbatchestocheck = this.CurrentBusObj.ObjectData.Tables["main"];

            //foreach (DataRow dtrow in dtbatchestocheck.Rows)
            //{
            //    //check to include
            //    dtrow["rebill_flag"] = 1;
            //}
            //Loop through and select all rows in the grid
            foreach (DataRecord r in GridRebillSource.xGrid.Records)
            {

                  if (r.Cells["rebill_flag"].Value.ToString() == "0")
                      r.Cells["rebill_flag"].Value = 1;
                
                  //GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                  GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
            }
            bUncheckLine = false;

        }

        private void chkRebillAll_UnChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            //Loop through and uncheck all batches
            //Loop through and check all batches to include
            if (!bUncheckLine)
                 foreach (DataRecord r in GridRebillSource.xGrid.Records)
                {

                    if (r.Cells["rebill_flag"].Value.ToString() == "1")
                        r.Cells["rebill_flag"].Value = 0;

                    //GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                    GridRebillSource.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                }
            bUncheckLine = false;
        }

    }
}

