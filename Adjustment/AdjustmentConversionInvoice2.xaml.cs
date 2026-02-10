

#region using statements
using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows.Input;
using System.Collections.Generic;
using Infragistics.Windows.Editors;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Linq;
#endregion

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentConversionInvoice2' object.
    /// </summary>
    public partial class AdjustmentConversionInvoice2 : ScreenBase, IPreBindable
    {
        public cBaseBusObject ConversionInvoiceBusObject = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        private string DocumentId { get; set; }
        public ComboBoxItemsProvider cmbProducts { get; set; }
        //This datatable is being added so that the Amount to adjust text box can have a binding
        //Do this for fields that contain informational data but that will not be saved
        //so that you can use converters or other benefits of binding
        DataTable dtMiscInfo = new DataTable("MiscInfo");
        private bool bNewAdjRowAdded { get; set; }
        private string strPreviewGeo { get; set; }
        DataTable dtRollupTemp = new DataTable();
        string NewDocID = "";

        /// <summary>
        /// Create a new instance of a 'AdjustmentConversionInvoice2' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentConversionInvoice2(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = ConversionInvoiceBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentConversionInvoice";
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

            //setup adj grid
            //GridAdjustment.xGrid.PreviewKeyDown += new EventHandler<Infragistics.Windows.DataPresenter.cellpr.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            GridAdjustment.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            GridAdjustment.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridAdjustment.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridAdjustment.ContextMenuRemoveIsVisible = false;
            GridAdjustment.MainTableName = "main";
            GridAdjustment.ConfigFileName = "AdjustmentConversionInvoice";
            GridAdjustment.SetGridSelectionBehavior(false, false);
            GridAdjustment.xGrid.FieldSettings.AllowEdit = true;
            GridAdjustment.IsEnabled = false;
            GridAdjustment.SkipReadOnlyCellsOnTab = true;
            GridAdjustment.FieldLayoutResourceString = "GridConversionInvoice";
            GridAdjustment.ContextMenuAddDelegate = GridAdjustmentContextMenuAdd;
            GridCollection.Add(GridAdjustment);
            //Adding fields to the MiscInfo datatable - Fields represent fields that are not bound through
            //the base business object and are not needed for the database
            dtMiscInfo.Columns.Add("amt_adjusted");
            //Bind the screen field(s) to the datatable
            txtAdjAmt.DataContext = dtMiscInfo;

            loadParms();
            this.Load();
            //set focus when window opens
            txtDocument.CntrlFocus();
        }

        /// <summary>
        /// populates grid and enables adj amt field if grid rows > 0
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

        /// <summary>
        /// populates grid and enables adj amt field if grid rows > 0
        /// </summary>
        private void popGrid()
        {
            //load parms 
            loadParms();
            //clear totals
            //txtRunningOffsetAmt.Text = "0.00";
            //load the object
            this.Load();
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
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
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            btnSave.IsEnabled = false;
            GridAdjustment.IsEnabled = false;
            //GLBorder.IsEnabled = false;
            //reset total to 0 when new doc selected
            txtRunningAdjAmt.Text = "0.00";
        }

        /// <summary>
        /// works as cellLeave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            e.Handled = true;
            int edit_index = GridAdjustment.ActiveRecord.Cells.IndexOf(e.Cell);
            DataRecord GR = GridAdjustment.ActiveRecord;
            if (GR.Cells["amt_to_adjust"].Value.ToString() == "")
            {
                GR.Cells["amt_to_adjust"].Value = 0.00;
                GR.Cells["offsetting_amount"].Value = 0.00;
            }
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
                    if (GridRecord.Cells["amt_to_adjust"].Value.ToString() == "")
                    {
                        GridRecord.Cells["amt_to_adjust"].Value = 0.00;
                        GridRecord.Cells["offsetting_amount"].Value = 0.00;
                    }
                    //Total Grid Cells
                    totGridRecs();
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0) txtRunningAdjAmt.TextColor = "Red";
                    else txtRunningAdjAmt.TextColor = "Black";
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    if (AmountsOffset()) btnSave.IsEnabled = true;
                }
            }
            if (edit_index == -1 || edit_index == 19) //last cell in record
            {
                //validate GL info
                DataRecord row = (DataRecord)GridAdjustment.xGrid.ActiveRecord;
                //change parms for GL Validation
                this.CurrentBusObj.changeParm("@company", row.Cells["offsetting_co"].Value.ToString());
                this.CurrentBusObj.changeParm("@center", row.Cells["offsetting_center"].Value.ToString());
                this.CurrentBusObj.changeParm("@account", row.Cells["offsetting_account"].Value.ToString());
                this.CurrentBusObj.changeParm("@product", row.Cells["offsetting_product"].Value.ToString());
                this.CurrentBusObj.changeParm("@region", row.Cells["offsetting_region"].Value.ToString());
                this.CurrentBusObj.changeParm("@interco", row.Cells["offsetting_interco"].Value.ToString());
                this.CurrentBusObj.LoadTable("gl_validate");
                if (this.CurrentBusObj.ObjectData.Tables["gl_validate"].Rows.Count < 1)
                {
                    Messages.ShowInformation("The Offsetting GL Account is Invalid.");
                } 
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
            this.CurrentBusObj.Parms.AddParm("@company", "");
            this.CurrentBusObj.Parms.AddParm("@center", "");
            this.CurrentBusObj.Parms.AddParm("@account", "");
            this.CurrentBusObj.Parms.AddParm("@product", "");
            this.CurrentBusObj.Parms.AddParm("@region", "");
            this.CurrentBusObj.Parms.AddParm("@interco", "");
        }

        /// <summary>
        /// totals grid cells and adds to running total
        /// </summary>
        private void totGridRecs()
        {
            //init AdjTotal w/starting value
            double AdjTotal = 0.00;
            double RunningAdjTotal = 0.00;
            //loop through each grid record and add adj totals
            foreach (DataRecord r in GridAdjustment.xGrid.Records)
            {
                if (r.Cells["amt_to_adjust"].Value.ToString() == "")
                    r.Cells["amt_to_adjust"].Value = 0.00;
                //sum adj amts
                AdjTotal += Convert.ToDouble(r.Cells["amt_to_adjust"].Value);
                RunningAdjTotal += Convert.ToDouble(r.Cells["amt_to_adjust"].Value) * -1;
            }
            txtRunningAdjAmt.Text = AdjTotal.ToString("0.00");
            //txtRunningOffsetAmt.Text = RunningAdjTotal.ToString("0.00");
            //txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
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
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) == Convert.ToDouble(sAdjAmt))
            {
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
        /// get pre-bound junk
        /// </summary>
        public void PreBind()
        {
            //Product drop down box - alloc grid
            ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
            ip = new ComboBoxItemsProvider();
            ip.ItemsSource = CurrentBusObj.ObjectData.Tables["products"].DefaultView;
            ip.ValuePath = "product_code";
            ip.DisplayMemberPath = ("product_description");
            cmbProducts = ip;
        }

        //Remove format characters from string so that it can be converted to numeric
        private string UnformatTextField(string FormattedTextField)
        {
            if (FormattedTextField == null || FormattedTextField == "") return "0";

            string sUnformattedTextField = FormattedTextField.Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");

            return sUnformattedTextField;
        }

        private void txtAdjAmt_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key == Key.Tab || e.Key == Key.Enter) && this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                this.PrepareFreeformForSave();
                if (Convert.ToDouble(UnformatTextField(txtAdjAmt.Text)) != 0)
                {
                    setEditScreenState();
                    GridAdjustment.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// enables editable fields when adj amt value > 0
        /// </summary>
        private void setEditScreenState()
        {
            //Enable fields
            GridAdjustment.IsEnabled = true;
            //GLBorder.IsEnabled = true;
            //Set the amt_to_adjust number field as active
            (GridAdjustment.xGrid.Records[GridAdjustment.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsActive = true;
            (GridAdjustment.xGrid.Records[GridAdjustment.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsSelected = true;
            /////////////////////////////////////////////////////
        }

        private void txtAdjAmt_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {

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
            /////////////////////////////////////////////////////////////////////////////////
            //set a default value if user skips
            if (txtAdjAmt.Text == "") txtAdjAmt.Text = "0.00";
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtAdjAmt.Text);
            txtAdjAmt.Text = formatAmt.ToString("0.00");
            /////////////////////////////////////////////////////////////////////////
            //if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            //{
            //    txtGLOffsetAmt2.Text = Convert.ToString(Convert.ToDouble(txtAdjAmt.Text) * -1);
            //    txtRunningOffsetAmt.Text = txtGLOffsetAmt2.Text;
            //}
            //if less than zero turn red
            if (Convert.ToDouble(txtAdjAmt.Text) < 0)
            {
                txtAdjAmt.TextColor = "Red";
                txtRunningAdjAmt.TextColor = "Red";
                //txtGLOffsetAmt2.TextColor = "Black";
                //txtRunningOffsetAmt.TextColor = "Black";
            }
            else
            {
                txtAdjAmt.TextColor = "Black";
                txtRunningAdjAmt.TextColor = "Black";
                //txtGLOffsetAmt2.TextColor = "Red";
                //txtRunningOffsetAmt.TextColor = "Red";
            }

            ////enable editable fields if adj value > 0
            //if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            //{
            //    setEditScreenState();
            //    //btnSave.IsEnabled = true;
            //}
            txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
            //txtGLOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtGLOffsetAmt.Text));
            //txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
            //txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
        }

        /// <summary>
        /// Creates new row with context menu add
        /// </summary>
        private void GridAdjustmentContextMenuAdd()
        {
            CreateNewRow();
        }

        /// <summary>
        /// This method creates a new row in grid and puts the row into edit mode.
        /// </summary>
        private void CreateNewRow()
        {
            //GridAdjustment.SetGridSelectionBehavior(true, false);
            int EmptyRow = GridEmptyRowsProcessing(GridAdjustment, false);
            if (EmptyRow != -1)
            {
                GridAdjustment.xGrid.ActiveRecord = GridAdjustment.xGrid.Records[EmptyRow];
            }
            else
            {
                //this.Load(this.CurrentBusObj, false, "main");
                //GridAdjustment.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                Prep_ucBaseGridsForSave();
                //Sets the grid to allow a new row to be created
                GridAdjustment.xGrid.FieldLayoutSettings.AllowAddNew = true;
                //Establishes a local variable tied to the row we just created
                //this.CurrentBusObj.ObjectData.Tables["main"].AcceptChanges();
                DataRecord row = GridAdjustment.xGrid.RecordManager.CurrentAddRecord;
                DataRecord selRow = (DataRecord)GridAdjustment.xGrid.ActiveRecord;
                //Set the default values for the columns
                if (CurrentBusObj.ObjectData.Tables["main"].Rows != null && CurrentBusObj.ObjectData.Tables["main"].Rows[0]["apply_to_doc"].ToString() != "0")
                {
                    row.Cells["apply_to_doc"].Value = selRow.Cells["apply_to_doc"].Value;
                    row.Cells["apply_to_seq"].Value = selRow.Cells["apply_to_seq"].Value;
                    row.Cells["detail_type"].Value = selRow.Cells["detail_type"].Value;
                    row.Cells["company_code"].Value = selRow.Cells["company_code"].Value;
                    row.Cells["receivable_account"].Value = selRow.Cells["receivable_account"].Value;
                    row.Cells["amount"].Value = selRow.Cells["amount"].Value;
                    row.Cells["open_amount"].Value = selRow.Cells["open_amount"].Value;
                    row.Cells["tax_amount"].Value = selRow.Cells["tax_amount"].Value;
                    row.Cells["currency_code"].Value = selRow.Cells["currency_code"].Value;
                    row.Cells["date_closed"].Value = selRow.Cells["date_closed"].Value;
                    row.Cells["gl_acct"].Value = selRow.Cells["gl_acct"].Value;
                    row.Cells["gl_center"].Value = selRow.Cells["gl_center"].Value;
                    row.Cells["product_code"].Value = selRow.Cells["product_code"].Value;
                    //row.Cells["apply_to_doc"].Value = selRow.Cells["product_code"];
                    row.Cells["last_activity_date"].Value = selRow.Cells["last_activity_date"].Value;
                    row.Cells["geography"].Value = selRow.Cells["geography"].Value;
                    row.Cells["interdivision"].Value = selRow.Cells["interdivision"].Value;
                    row.Cells["gl_product"].Value = selRow.Cells["gl_product"].Value;
                    row.Cells["offsetting_co"].Value = selRow.Cells["offsetting_co"].Value;
                    row.Cells["offsetting_center"].Value = selRow.Cells["offsetting_center"].Value;
                    row.Cells["offsetting_account"].Value = selRow.Cells["offsetting_account"].Value;
                    row.Cells["offsetting_product"].Value = selRow.Cells["offsetting_product"].Value;
                    row.Cells["offsetting_region"].Value = selRow.Cells["offsetting_region"].Value;
                    row.Cells["offsetting_interco"].Value = selRow.Cells["offsetting_interco"].Value;
                    row.Cells["offsetting_amount"].Value = 0.00;
                    row.Cells["amt_to_adjust"].Value = 0.00;
                    row.Cells["adj_document_id"].Value = selRow.Cells["adj_document_id"].Value;
                    row.Cells["account_name"].Value = selRow.Cells["account_name"].Value;
                }
                else
                    return;
                //Commit the add new record - Required so that we can get rid of the add new record box and make this record active
                GridAdjustment.xGrid.RecordManager.CommitAddRecord();
                //Remove the add new record row
                GridAdjustment.xGrid.FieldLayoutSettings.AllowAddNew = false;

                //GridAdjustment.SetGridSelectionBehavior(false, false);

                //Set the row we just created to the active record
                GridAdjustment.xGrid.ActiveRecord = GridAdjustment.xGrid.Records[0];
            }
            //Set the amt_to_adjust field as active, this will not work.  Will not go into edit mode immediately. Have to set to first editable field before it
            //(GridAdjustment.xGrid.Records[GridAdjustment.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsActive = true;
            //(GridAdjustment.xGrid.Records[GridAdjustment.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsSelected = true;
            GridAdjustment.CntrlFocus();
            //set focus on first editable field
            //(GridAdjustment.xGrid.Records[GridAdjustment.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsActive = true;
            //(GridAdjustment.xGrid.Records[GridAdjustment.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsSelected = true;
            (GridAdjustment.xGrid.Records[0] as DataRecord).Cells["receivable_account"].IsActive = true;
            (GridAdjustment.xGrid.Records[0] as DataRecord).Cells["receivable_account"].IsSelected = true;
            CurrentState = ScreenState.Normal;
            bNewAdjRowAdded = true;
            
        }

        /// <summary>
        /// Method checks to see if the grid has empty records
        /// The method can be told to delete empty records
        /// </summary>
        /// <param name="gridToCheck">The ucBaseGrid to check for empty records</param>
        /// <param name="deleteEmpty">True if the empty records should be deleted</param>
        /// <returns></returns>
        private int GridEmptyRowsProcessing(ucBaseGrid gridToCheck, bool deleteEmpty)
        {
            int IsEmpty = -1;
            List<DataRecord> rDelete = new List<DataRecord>();

            if (gridToCheck.xGrid.Records != null && gridToCheck.xGrid.Records.Count > 0)
            {
                //Loop through the grid records looking for empty records
                foreach (DataRecord r in gridToCheck.xGrid.Records)
                {

                    //Check for empty records
                    if (Convert.ToDecimal(r.Cells["amount"].Value) == 0)
                    {
                        //Record the record index
                        IsEmpty = r.Index;
                        //If not deleting then return the index and exit the method
                        if (!deleteEmpty)
                            return IsEmpty;
                        //Otherwise add the record to the delete list
                        rDelete.Add(r);
                    }
                    else
                        continue;

                }
                //Delete records
                if (deleteEmpty && IsEmpty != -1)
                {
                    foreach (DataRecord r in rDelete)
                    {
                        DataRow row = (r.DataItem as DataRowView).Row;
                        if (row != null)
                        {
                            row.Delete();
                        }
                    }

                }

            }
            return IsEmpty;
        }

        private void GridAdjustment_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

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
            //commit fields to data tables.
            Prep_ucBaseGridsForSave();
            PrepareFreeformForSave();

            this.SaveSuccessful = false;
            //tot recs make sure all adj amts in grid have a valid value
            totGridRecs();
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            //make sure offsetting amts are not 0
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) + Convert.ToDouble(sAdjAmt.Trim()) == 0)
            {
                Messages.ShowWarning("Adjustment Amount and Offsetting Amounts Cannot Sum to $0.00. Save Cancelled");
                return;
            }
            if (AmountsOffset())
            {
                //make sure all offset values have been entered for every record in the grid whose amt to adj has a non zero value
                if (!GLOffsetValuesEnteredForEachGridRecGreaterThanZero()) return;
                //set offsetting amts in grid
                setOffsettingAmtsGrid();
                //call method to insert a new conv adj and get new doc Id
                if (NewDocID == "")
                    NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 19, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));
                //insert the new doc id in all existing grid rows for ins sp
                if (NewDocID != "")
                    updGridRowsDocId(NewDocID);
                else
                {
                    Messages.ShowError("Problem Creating New Adjustment");
                    return;
                }
                if (!rollupDetailRecords())
                {
                    Messages.ShowError("Error Writing Detail Records to Summary Table");
                    return;
                }
                //do this to run the INS stored proc instead of the UPD stored proc//////////
                DataTable dt = this.CurrentBusObj.ObjectData.Tables["rollup"];
                if (dt != null)
                {
                    dt.AcceptChanges();
                    foreach (DataRow row in dt.Rows)
                    {
                        row.SetAdded();
                    }
                }
                DataTable dt2 = this.CurrentBusObj.ObjectData.Tables["main"];
                if (dt2 != null)
                {
                    dt2.AcceptChanges();
                    foreach (DataRow row2 in dt2.Rows)
                    {
                        row2.SetAdded();
                    }
                }

                /////////////////////////////////////////////////////////////////////////////
                //clear error message
                this.CurrentBusObj.changeParm("@error_message", "");
                //validate GL info
                foreach (DataRecord rowgl in GridAdjustment.xGrid.Records)
                {
                    //DataRecord rowgl = (DataRecord)GridAdjustment.xGrid.ActiveRecord;
                    //change parms for GL Validation
                    this.CurrentBusObj.changeParm("@company", rowgl.Cells["offsetting_co"].Value.ToString());
                    this.CurrentBusObj.changeParm("@center", rowgl.Cells["offsetting_center"].Value.ToString());
                    this.CurrentBusObj.changeParm("@account", rowgl.Cells["offsetting_account"].Value.ToString());
                    this.CurrentBusObj.changeParm("@product", rowgl.Cells["offsetting_product"].Value.ToString());
                    this.CurrentBusObj.changeParm("@region", rowgl.Cells["offsetting_region"].Value.ToString());
                    this.CurrentBusObj.changeParm("@interco", rowgl.Cells["offsetting_interco"].Value.ToString());
                    this.CurrentBusObj.LoadTable("gl_validate");
                    if (this.CurrentBusObj.ObjectData.Tables["gl_validate"].Rows.Count < 1)
                    {
                        Messages.ShowInformation("The Offsetting GL Account is Invalid.");
                        GridAdjustment.xGrid.FieldLayoutSettings.AllowAddNew = false;
                        rowgl.Cells["offsetting_co"].IsActive = true;
                        rowgl.Cells["offsetting_co"].IsSelected = true;
                        return;
                    }
                }
                //else
                //{
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
                //}
            }
            else
            {
                Messages.ShowError("Amounts Do Not Offset. Save Cancelled");
            }
        }

        private void setOffsettingAmtsGrid()
        {
            //use this loop to add offsetting values to the grid
            foreach (DataRecord dr in this.GridAdjustment.xGrid.Records)
            {
                double AdjTotal = 0.00;
                AdjTotal = Convert.ToDouble(dr.Cells["amt_to_adjust"].Value) * -1;
                //set offsetting amt to adj amt inverse
                dr.Cells["offsetting_amount"].Value = AdjTotal;
            }

        }

        /// <summary>
        /// used to sum up the detail record for each product. If product is diff an new record is created
        /// if product is the same it adj amt is summed in the existing product record
        /// </summary>
        /// <returns></returns>
        private bool rollupDetailRecords()
        {
            bool ProductFound = false;
            //clear rollup table
            this.CurrentBusObj.ObjectData.Tables["rollup"].Clear();
            decimal sumAdjAmt = 0;

            //loop through GridAdjustment records and add a new record when product changes, otherwise sum adj_amts for same product
            for(int i = 0; i < GridAdjustment.xGrid.Records.Count; i++)
            {
                DataRecord row = GridAdjustment.xGrid.Records[i] as DataRecord;
                if (i == 0)
                {
                    CreateNewRollupRow(row);
                    sumAdjAmt += (decimal)row.Cells["amt_to_adjust"].Value;
                }
                else
                {
                    ProductFound = false;
                    //use the linq query to access the rollup datatable without locking the records in the datatable
                    //this is necessary to add values to the datatable while it is being looped through
                    var RollupDtRows = from x in this.CurrentBusObj.ObjectData.Tables["rollup"].AsEnumerable()
                                     //where x.Field<string>("product_code") == "product_code"
                                     select new
                                     {
                                         //get the product code from the rollup datatable to compare to the product code in the grid
                                         productCodeValue = x.Field<string>("product_code"),
                                         
                                     };
                    foreach (var rec in RollupDtRows)
                    {
                        //check to see if prod code in the rollup datatable exists
                        if (rec.productCodeValue == row.Cells["product_code"].Value.ToString())
                        {
                            ProductFound = true;
                        }
                    }
                    //use this loop to add the values to the datatable
                    foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["rollup"].Rows)
                    {
                        if (ProductFound)
                        {
                            //determine if datatable already contains record for this product
                            if (dr.ItemArray[12].ToString() == row.Cells["product_code"].Value.ToString())
                            {
                                //row exists for this product add to its total
                                sumAdjAmt += (decimal)row.Cells["amt_to_adjust"].Value;
                                //offsetting amount needs to be the the inverse of the amt_to_adjust 
                                decimal inverseAdjAmt = (decimal)row.Cells["amt_to_adjust"].Value * -1;
                                decimal cellInverseAmt = Convert.ToDecimal(dr[23]);
                                decimal InverseRunningTotal = cellInverseAmt + inverseAdjAmt;
                                dr[23] = InverseRunningTotal;
                                //dr[23] = sumAdjAmt * -1;
                                //add sum to datatable rec
                                decimal AdjAmt = (decimal)row.Cells["amt_to_adjust"].Value;
                                decimal cellAdjAmt = Convert.ToDecimal(dr[24]);
                                decimal RunningTotal = cellAdjAmt + AdjAmt;
                                dr[24] = RunningTotal;
                                //dr[24] = sumAdjAmt;
                                this.CurrentBusObj.ObjectData.Tables["rollup"].AcceptChanges();
                                break;
                            }
                        }
                        else
                        {
                            //create new product record and add adj amt
                            if (CreateNewRollupRow(row))
                            {
                                //break out of inner loop when new row created
                                break;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Adds new record to the rollup datatable
        /// </summary>
        private bool CreateNewRollupRow(DataRecord row)
        {
            GridAdjustment.xGrid.FieldLayoutSettings.AllowAddNew = true;
            //Set the default values for the columns
            if (CurrentBusObj.ObjectData.Tables["main"].Rows != null && CurrentBusObj.ObjectData.Tables["main"].Rows[0]["apply_to_doc"].ToString() != "0")
            {
                DataRow dr = this.CurrentBusObj.ObjectData.Tables["rollup"].NewRow();
                dr[0] = row.Cells["apply_to_doc"].Value;
                dr[1] = row.Cells["apply_to_seq"].Value;
                dr[2] = row.Cells["detail_type"].Value;
                dr[3] = row.Cells["company_code"].Value;
                dr[4] = row.Cells["receivable_account"].Value;
                dr[5] = row.Cells["amount"].Value;
                dr[6] = row.Cells["open_amount"].Value;
                dr[7] = row.Cells["tax_amount"].Value;
                dr[8] = row.Cells["currency_code"].Value;
                dr[9] = row.Cells["date_closed"].Value;
                dr[10] = row.Cells["gl_acct"].Value;
                dr[11] = row.Cells["gl_center"].Value;
                dr[12] = row.Cells["product_code"].Value;
                dr[13] = row.Cells["last_activity_date"].Value;
                dr[14] = row.Cells["geography"].Value;
                dr[15] = row.Cells["interdivision"].Value;
                dr[16] = row.Cells["gl_product"].Value;
                dr[17] = row.Cells["offsetting_co"].Value;
                dr[18] = row.Cells["offsetting_center"].Value;
                dr[19] = row.Cells["offsetting_account"].Value;
                dr[20] = row.Cells["offsetting_product"].Value;
                dr[21] = row.Cells["offsetting_region"].Value;
                dr[22] = row.Cells["offsetting_interco"].Value;
                //dr[23] = row.Cells["offsetting_amount"].Value;
                //offsetting amount needs to be the the inverse of the amt_to_adjust
                dr[23] = Convert.ToDecimal(row.Cells["amt_to_adjust"].Value) * -1;
                dr[24] = row.Cells["amt_to_adjust"].Value;
                dr[25] = row.Cells["adj_document_id"].Value;
                dr[26] = row.Cells["account_name"].Value;

                this.CurrentBusObj.ObjectData.Tables["rollup"].Rows.Add(dr);
                this.CurrentBusObj.ObjectData.Tables["rollup"].AcceptChanges();
            }
            else
            {
                return false;
            }
            return true;
        }

        private bool GLOffsetValuesEnteredForEachGridRecGreaterThanZero()
        {
            //loop through each grid record and add adj totals
            foreach (DataRecord r in GridAdjustment.xGrid.Records)
            {
                //if adj amt > 0 make sure offsetting values exist
                if (Convert.ToDouble(r.Cells["amt_to_adjust"].Value) != 0)
                {
                    //chk offsetting vals
                    if (r.Cells["offsetting_co"].Value.ToString() == "" ||
                        r.Cells["offsetting_center"].Value.ToString() == "" ||
                        r.Cells["offsetting_account"].Value.ToString() == "" ||
                        r.Cells["offsetting_product"].Value.ToString() == "" ||
                        r.Cells["offsetting_region"].Value.ToString() == "" ||
                        r.Cells["offsetting_amount"].Value.ToString() == "")
                    {
                        Messages.ShowWarning("G/L Offsetting Value(s) Missing for an Adjusted Document in the Grid. Save Cancelled");
                        return false;
                    }
                }
            }
            return true;
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
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["rollup"].Rows)
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

    }
}
