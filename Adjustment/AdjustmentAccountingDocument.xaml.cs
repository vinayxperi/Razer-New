using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentAccountingDocument' object.
    /// </summary>
    public partial class AdjustmentAccountingDocument : ScreenBase
    {
        public cBaseBusObject AdjAcctDocBusObject = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        private string DocumentId { get; set; }

        /// <summary>
        /// Create a new instance of a 'AdjustmentAccountingDocument' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentAccountingDocument(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = AdjAcctDocBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentAcctDocument";
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
            //setup events for tabbing through grid
            this.GridAdjustment.PreviewKeyDown += new KeyEventHandler(GridAdjustment_PreviewKeyDown);
            this.txtAdjAmt.PreviewKeyDown += new KeyEventHandler(txtAdjAmt_PreviewKeyDown);
            //setup adj grid
            GridAdjustment.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            GridAdjustment.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridAdjustment.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridAdjustment.ContextMenuAddIsVisible = false;
            GridAdjustment.ContextMenuRemoveIsVisible = false;
            GridAdjustment.MainTableName = "main";
            GridAdjustment.ConfigFileName = "AdjustmentDocument";
            GridAdjustment.SetGridSelectionBehavior(false, false);
            GridAdjustment.xGrid.FieldSettings.AllowEdit = true;
            GridAdjustment.IsEnabled = false;
            GridAdjustment.FieldLayoutResourceString = "GridConversionInvoice";
            GridCollection.Add(GridAdjustment);
            loadParms();
            this.Load();
        }

        /// <summary>
        /// used to setup grid for tabbing through editable fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GridAdjustment_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            //{
            //    if (e.Key == Key.Tab)
            //    {
            //        RazerBase.ucBaseGrid grid = (RazerBase.ucBaseGrid)sender;
            //        if (grid.xGrid.ActiveCell != null)
            //        {
            //            if (grid.xGrid.ActiveCell.Field.Name == "amt_to_adjust")
            //            {
            //                //grid.ExecuteCommand(DataPresenterCommands.CellLastInRecord);
            //                grid.xGrid.ExecuteCommand(DataPresenterCommands.CellNext);
            //                grid.xGrid.ActiveCell.IsActive = true;
            //                grid.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            //                e.Handled = true;
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// used to setup grid for tabbing through editable fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAdjAmt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            //{
            //    //enable editable fields if adj value > 0
            //    if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            //    {
            //        setEditScreenState();
            //        if (e.Key == Key.Tab)
            //        {
            //            GridAdjustment.xGrid.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
            //            GridAdjustment.xGrid.ActiveCell.IsActive = true;
            //            GridAdjustment.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            //            //e.Handled = true;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// works as cellLeave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = GridAdjustment.ActiveRecord.Cells.IndexOf(e.Cell);

            //int row_index = GridAdjustment.ActiveRecord.Index;

            //WARNING: You may need to change this if the order of the AdjustmentFieldLayouts\GridConversionInvoice changes
            if (edit_index == 7) //Amount To Adjust Field
            {
                //commit user entered value to datatable
                GridAdjustment.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                DataRecord GridRecord = null;
                GridRecord = GridAdjustment.ActiveRecord;
                DataRowView dr = GridRecord.DataItem as DataRowView;
                DataView dv = dr.DataView;
                if (GridRecord != null)
                {
                    //clear running total
                    txtRunningAdjAmt.Text = "";
                    //init AdjTotal w/starting value
                    double AdjTotal = 0.00;
                    if (GridRecord.Cells["amt_to_adjust"].Value.ToString() == "")
                    {
                        GridRecord.Cells["amt_to_adjust"].Value = 0.00;
                    }
                    GridRecord.Cells["offsetting_amount"].Value = Convert.ToDouble(GridRecord.Cells["amt_to_adjust"].Value) * -1;
                    //loop through each grid record and add adj totals
                    foreach (DataRecord r in GridAdjustment.xGrid.Records)
                    {
                        //sum adj amts
                        AdjTotal += Convert.ToDouble(r.Cells["amt_to_adjust"].Value);
                    }
                    txtRunningAdjAmt.Text = AdjTotal.ToString("0.00");
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
        /// populates grid and enables adj amt field if grid rows > 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDocument_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //do this to keep grid from loading and binding re-running clearing user entered values
            if (DocumentId != txtDocument.Text)
            {
                if (txtDocument.Text != "")
                {
                    //populate grid
                    popGrid();
                }
            }
        }

        /// <summary>
        /// check offsetting amts to see if save button can be enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGLOffsetAmt_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //if txt is not numeric////////////////////////////////////////////////////////////
            Double result = 0;
            if (Double.TryParse(txtGLOffsetAmt.Text, out result) == false)
            {
                txtGLOffsetAmt.Text = "0.00";
            }
            ///////////////////////////////////////////////////////////////////////////////////
            //set a default value if user blnaks
            if (txtGLOffsetAmt.Text == "") txtGLOffsetAmt.Text = "0.00";
            double OffsetTotal = 0.00;
            //loop through each grid record and add adj totals
            foreach (DataRecord r in GridAdjustment.xGrid.Records)
            {
                //sum adj amts
                OffsetTotal += Convert.ToDouble(r.Cells["offsetting_amount"].Value);
            }
            txtRunningOffsetAmt.Text = OffsetTotal.ToString("0.00");
            //convert the value to a double and format to add trailing zeros if missing
            decimal formatAmt = Convert.ToDecimal(txtGLOffsetAmt.Text);
            txtGLOffsetAmt.Text = formatAmt.ToString("0.00");
            //if less than zero turn red otherwise black/////////////////////////////////////////////////
            if (Convert.ToDouble(txtGLOffsetAmt.Text) < 0) txtGLOffsetAmt.TextColor = "Red";
            else txtGLOffsetAmt.TextColor = "Black";
            if (Convert.ToDouble(txtRunningOffsetAmt.Text) < 0) txtRunningOffsetAmt.TextColor = "Red";
            else txtRunningOffsetAmt.TextColor = "Black";
            //////////////////////////////////////////////////////////////////////////////////////////////
            //if amounts offset then enable save button
            if (AmountsOffset()) btnSave.IsEnabled = true;

        }

        /// <summary>
        /// AdjAmt Lost focus enables editable fields when value > 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtAdjAmt.Text);
            txtAdjAmt.Text = formatAmt.ToString("0.00");
            ///////////////////////////////////////////////////////////////////////////
            if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            {
                txtGLOffsetAmt.Text = Convert.ToString(Convert.ToDouble(txtAdjAmt.Text) * -1);
                txtRunningOffsetAmt.Text = txtGLOffsetAmt.Text;
            }
            //if less than zero turn red
            if (Convert.ToDouble(txtAdjAmt.Text) < 0)
            {
                txtAdjAmt.TextColor = "Red";
                txtRunningAdjAmt.TextColor = "Red";
                txtGLOffsetAmt.TextColor = "Black";
                txtRunningOffsetAmt.TextColor = "Black";
            }
            //otherwise black
            else
            {
                txtAdjAmt.TextColor = "Black";
                txtRunningAdjAmt.TextColor = "Black";
                txtGLOffsetAmt.TextColor = "Red";
                txtRunningOffsetAmt.TextColor = "Red";
            }

            //enable editable fields if adj value > 0
            if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            {
                setEditScreenState();
                //TODO: Goto first editable field FAIL
                //GridAdjustment.DisableBaseGridTab = true;
                //GridAdjustment.SetRowCellFocus(GridAdjustment.xGrid.Records[0] as DataRecord, "amt_to_adjust");
                //e.Handled = true;
            }
            txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
            //txtGLOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtGLOffsetAmt.Text));
            //txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
            //txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
        }

        /// <summary>
        /// highlight textbox text for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAdjAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSelectAll(sender);
            if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }

            txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");
        }

        /// <summary>
        /// highlight textbox text for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGLOffsetAmt_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            txtSelectAll(sender);
        }

        /// <summary>
        /// call SelectAll method on textbox portion of ucLabelTextbox
        /// </summary>
        /// <param name="sender"></param>
        private void txtSelectAll(object sender)
        {
            if (sender is RazerBase.ucLabelTextBox)
                (sender as RazerBase.ucLabelTextBox).SelectAll();
        }

        /// <summary>
        /// populates grid and enables adj amt field if grid rows > 0
        /// </summary>
        private void popGrid()
        {
            loadParms();
            //clear totals
            txtRunningOffsetAmt.Text = "0.00";
            //load the object
            this.Load();
            if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count > 0)
            {
                txtAdjAmt.IsEnabled = true;
            }
            else
            {
                Messages.ShowInformation("Document Not Found");
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
            //add doc id parm to bus obj
            this.CurrentBusObj.Parms.AddParm("@apply_to_doc", txtDocument.Text);
            DocumentId = txtDocument.Text;
            //add parm for error messages
            this.CurrentBusObj.Parms.AddParm("@error_message", "");
        }

        /// <summary>
        /// enables editable fields when adj amt value > 0
        /// </summary>
        private void setEditScreenState()
        {
            //Enable fields
            GridAdjustment.IsEnabled = true;
            GLBorder.IsEnabled = true;
            /////////////////////////////////////////////////////
        }

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            btnSave.IsEnabled = false;
            GridAdjustment.IsEnabled = false;
            GLBorder.IsEnabled = false;
            //reset total to 0 when new doc selected
            txtRunningAdjAmt.Text = "0.00";
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
        /// check that all GL fields are entered
        /// </summary>
        /// <param name="strErr"></param>
        /// <returns></returns>
        private bool GLDataComplete(ref string strErr)
        {
            if (txtGLCompany.Text == null || txtGLCompany.Text == "")
            {
                strErr = "GL Company Missing";
                return false;
            }
            if (txtGLCostCtr.Text == null || txtGLCostCtr.Text == "")
            {
                strErr = "GL Cost Center Missing";
                return false;
            }
            if (txtGLAcct.Text == null || txtGLAcct.Text == "")
            {
                strErr = "GL Nat Acct Code Missing";
                return false;
            }
            if (txtGeo.Text == null || txtGeo.Text == "")
            {
                strErr = "GL GEO Missing";
                return false;
            }
            if (txtGLProduct.Text == null || txtGLProduct.Text == "")
            {
                strErr = "GL Product Mising";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Do Save logic
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
            string strErr = "";
            if (GLDataComplete(ref strErr) == false)
            {
                Messages.ShowError(strErr);
                return;
            }
            //make sure offsetting amts are not 0
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            string sRunningOffsetAmt = UnformatTextField(txtRunningOffsetAmt.Text.Trim());
            //string sGLOffsetAmt = UnformatTextField(txtGLOffsetAmt.Text.Trim());
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) + Convert.ToDouble(sAdjAmt.Trim()) + (Convert.ToDouble(sAdjAmt.Trim()) + 
                Convert.ToDouble(sRunningOffsetAmt.Trim())) == 0)
            {
                Messages.ShowWarning("Adjustment Amount and Offsetting Amount Cannot Be $0.00. Save Cancelled");
                return;
            }
            if (AmountsOffset())
            {
                //call method to insert a new conv adj and get new doc Id
                //string NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 13, cGlobals.UserName.ToLower(), Convert.ToDecimal(txtAdjAmt.Text));
                string NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 13, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));
                //insert the new doc id in all existing grid rows for ins sp
                if (NewDocID != "")
                    updGridRowsDocId(NewDocID);
                else
                {
                    Messages.ShowError("Problem Creating New Adjustment");
                    return;
                }
                //do this to run the INS stored proc instead of the UPD stored proc//////////
                DataTable dt = this.CurrentBusObj.ObjectData.Tables["main"];
                if (dt != null)
                {
                    dt.AcceptChanges();
                    foreach (DataRow row in dt.Rows)
                    {
                        row.SetAdded();
                    }
                }
                /////////////////////////////////////////////////////////////////////////////
                //Clear error message
                this.CurrentBusObj.changeParm("@error_message", "");
                double AdjustedAmount = Convert.ToDouble(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["amt_to_adjust"].ToString());
                double OffAdjustedAmount = Convert.ToDouble(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_amount"].ToString());
                base.Save();
                this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["amt_to_adjust"] = AdjustedAmount;
                this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_amount"] = OffAdjustedAmount;
                if (SaveSuccessful)
                {
                    //error messages from Stored Proc
                    string strSPErrMg = getInfoFromStoredProc();
                    if (strSPErrMg != null && strSPErrMg != "") 
                    {
                        Messages.ShowError(strSPErrMg);
                        //do rollback
                        rollbackAdj(NewDocID);
                        //btnSave.IsEnabled = false;
                    }
                    else
                    {
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
                }
                else
                {
                    Messages.ShowInformation("Save Failed");
                    //do rollback
                    rollbackAdj(NewDocID);
                }
            }
            else
            {
                Messages.ShowError("Amounts Do Not Offset. Save Cancelled");
            }
        }

        public override void Close()
        {
            base.Close();
        }

        /// <summary>
        /// sets up parms to prepare for delete of orphaned adj rec and calls delete
        /// </summary>
        /// <param name="NewDocID"></param>
        private void rollbackAdj(string NewDocID)
        {
            //delete adj header
            bool retVal = cGlobals.BillService.DeleteNewAdjusmentPreamble(NewDocID);
        }

        private void updGridRowsDocId(string DocID)
        {
            //loop through grid and upd with new Doc Id
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["main"].Rows)
            {
                dr["adj_document_id"] = DocID;
                dr.AcceptChanges();

            }
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
