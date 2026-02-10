

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
//using System.Windows.Forms;

namespace Adjustment
{

    /// <summary>
    /// This class represents a 'AdjustmentConversionInvoice' object.
    /// </summary>
    public partial class AdjustmentConversionInvoice : ScreenBase, IPreBindable
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

        /// <summary>
        /// Create a new instance of a 'AdjustmentConversionInvoice' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentConversionInvoice(AdjustmentFolder _AdjFolderScreen)
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
            GridAdjustment.ContextMenuAddIsVisible = false;
            GridAdjustment.ContextMenuRemoveIsVisible = false;
            GridAdjustment.MainTableName = "main";
            GridAdjustment.ConfigFileName = "AdjustmentConversionInvoice";
            GridAdjustment.SetGridSelectionBehavior(false, false);
            GridAdjustment.xGrid.FieldSettings.AllowEdit = true;
            GridAdjustment.IsEnabled = false;
            GridAdjustment.SkipReadOnlyCellsOnTab = true;
            GridAdjustment.FieldLayoutResourceString = "GridConversionInvoice";
            GridCollection.Add(GridAdjustment);
            txtGeo.PreviewKeyDownDelegate = PreviewKeyDownDelegate;
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
        /// used to setup grid for tabbing through editable fields
        /// TODO: Goto first editable field FAIL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GridAdjustment_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Tab || e.Key == Key.Enter) && this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                RazerBase.ucBaseGrid grid = (RazerBase.ucBaseGrid)sender;
                if (grid.xGrid.ActiveCell != null)
                {
                    if (grid.xGrid.ActiveCell.Field.Name == "amt_to_adjust")
                    {
                        //set focus to GLCompany if new adjustment row added to grid on tab off of previewKeyDown
                        if (bNewAdjRowAdded)
                        {
                            txtGLCompany.CntrlFocus();
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// used to setup grid for tabbing through editable fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAdjAmt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Tab || e.Key == Key.Enter) && this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0 )
            {
                this.PrepareFreeformForSave();
                if (Convert.ToDouble(txtAdjAmt.Text) != 0)
                {
                    setEditScreenState();
                    GridAdjustment.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                    e.Handled=true ;
                }
            }
        }

        private void PreviewKeyDownDelegate(object sender, KeyEventArgs e)
        {
            bool chkOffset = AmountsOffset();
            //this is needed in order to see the text in txtGeo before lose focus
            //ucLabelTextBox tGeo = (ucLabelTextBox)sender;
            //strPreviewGeo += Key2AscIIConverter(e.Key.ToString());
            if ((e.Key == Key.Tab || e.Key == Key.Enter) && this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                //validate GL Info & amounts do not offset
                if (IsGLInfoValid(sender) && !chkOffset)
                {
                    //stop tab event from occurring since CreateNewRow() method sets focus
                    e.Handled = true;
                    //create new row in xamdatagrid
                    CreateNewRow();
                }
                else
                {
                    if (chkOffset)
                    {
                        btnSave.Focus();
                    }
                    else
                    {
                        //invalid GL Info
                        Messages.ShowInformation("Invalid Offsetting Account Number");
                        txtGeo.CntrlFocus();
                    }
                }
            }
        }

        //private string Key2AscIIConverter(string key)
        //{
        //    switch (key)
        //    {
        //        case "NumPad0":
        //            return "0";
        //        case "NumPad1" :
        //            return "1";
        //        case "NumPad2":
        //            return "2";
        //        case "NumPad3":
        //            return "3";
        //        case "NumPad4":
        //            return "4";
        //        case "NumPad5":
        //            return "5";
        //        case "NumPad6":
        //            return "6";
        //        case "NumPad7":
        //            return "7";
        //        case "NumPad8":
        //            return "8";
        //        case "NumPad9":
        //            return "9";
        //        case "D0" :
        //            return "0";
        //        case "D1":
        //            return "1";
        //        case "D2":
        //            return "2";
        //        case "D3":
        //            return "3";
        //        case "D4":
        //            return "4";
        //        case "D5":
        //            return "5";
        //        case "D6":
        //            return "6";
        //        case "D7":
        //            return "7";
        //        case "D8":
        //            return "8";
        //        case "D9":
        //            return "9";
        //        default :
        //            return "";
        //    }
        //}

        private bool IsGLInfoValid(object sender)
        {
            TextBox tGeo = (TextBox)sender;
            //call svc and validate GL info
            foreach (DataRow row in this.CurrentBusObj.ObjectData.Tables["gl_validate"].Rows)
            {
                if (row["gl_co"].ToString() == txtGLCompany.Text &&
                    row["gl_center"].ToString() == txtGLCostCtr.Text &&
                    row["gl_account"].ToString() == txtGLAcct.Text &&
                    row["gl_product"].ToString() == txtGLProduct.Text &&
                    row["gl_region"].ToString() == tGeo.Text)
                {
                    strPreviewGeo = "";
                    return true;        
                }
            }
            strPreviewGeo = "";
            return false;
        }

        //public void GridAdjustment_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    //This if tests to see if we are already in the got focus event as infragistics fires the got focus when the active record changes
        //    //******This only appears to work when the grid is empty and receives focus - otherwise if a row or cell receives focus, the below if statement
        //    //will cause the event to be exited
        //    if (!GridAdjustment.xGrid.IsFocused)
        //    {
        //        e.Handled = true;
        //        return;
        //    }

        //    //If we are in insert mode or the item is not posted and the remit total does not match the batch total
        //    //if (CurrentState == ScreenState.Inserting || (tBatchTotal.Text != tRemitTotal.Text && chkPosted.IsChecked == 0 && CurrentState != ScreenState.Empty))
        //    //{
        //        //check for nulls
        //        if (txtRunningAdjAmt.Text == null)
        //            return;
        //        if (txtAdjAmt.Text == null)
        //            return;
        //        string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
        //        string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
        //        if (CurrentState == ScreenState.Inserting || Convert.ToDouble(sRunningAdjAmt.Trim()) < Convert.ToDouble(sAdjAmt.Trim()) && CurrentState != ScreenState.Empty)
        //        {

        //            if (Convert.ToDouble(sAdjAmt) != 0)
        //            {
        //                //GridAdjustment.Focus();
        //                CreateNewRow();
        //            }
        //            return;
        //        }
        //    //}
        //}

         /// <summary>
        /// This method creates a new row in grid and puts the row into edit mode.
        /// </summary>
        private void CreateNewRow()
        {
            int EmptyRow = GridEmptyRowsProcessing(GridAdjustment, false);
            if (EmptyRow != -1)
            {
                GridAdjustment.xGrid.ActiveRecord = GridAdjustment.xGrid.Records[EmptyRow];
            }
            else
            {
                //Sets the grid to allow a new row to be created
                GridAdjustment.xGrid.FieldLayoutSettings.AllowAddNew = true;
                //Establishes a local variable tied to the row we just created
                DataRecord row = GridAdjustment.xGrid.RecordManager.CurrentAddRecord;

                //Set the default values for the columns
                if (CurrentBusObj.ObjectData.Tables["main"].Rows != null && CurrentBusObj.ObjectData.Tables["main"].Rows[0]["apply_to_doc"].ToString() != "0")
                {
                    row.Cells["apply_to_doc"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["apply_to_doc"];
                    row.Cells["apply_to_seq"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["apply_to_seq"];
                    row.Cells["detail_type"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["detail_type"];
                    row.Cells["company_code"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["company_code"];
                    row.Cells["receivable_account"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["receivable_account"];
                    row.Cells["amount"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["amount"];
                    row.Cells["open_amount"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["open_amount"];
                    row.Cells["tax_amount"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["tax_amount"];
                    row.Cells["currency_code"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["currency_code"];
                    row.Cells["date_closed"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["date_closed"];
                    row.Cells["gl_acct"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["gl_acct"];
                    row.Cells["gl_center"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["gl_center"];
                    row.Cells["product_code"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["product_code"];
                    row.Cells["last_activity_date"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["last_activity_date"];
                    row.Cells["geography"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["geography"];
                    row.Cells["interdivision"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["interdivision"];
                    row.Cells["gl_product"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["gl_product"];
                    row.Cells["offsetting_co"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_co"];
                    row.Cells["offsetting_center"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_center"];
                    row.Cells["offsetting_account"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_account"];
                    row.Cells["offsetting_product"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_product"];
                    row.Cells["offsetting_region"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_region"];
                    row.Cells["offsetting_interco"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_interco"];
                    row.Cells["offsetting_amount"].Value = 0.00;
                    row.Cells["amt_to_adjust"].Value = 0.00;
                    row.Cells["adj_document_id"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["adj_document_id"];
                    row.Cells["account_name"].Value = CurrentBusObj.ObjectData.Tables["main"].Rows[0]["account_name"];
                }
                else
                    return;

 
                //Commit the add new record - Required so that we can get rid of the add new record box and make this record active
                GridAdjustment.xGrid.RecordManager.CommitAddRecord();
                //Remove the add new record row
                GridAdjustment.xGrid.FieldLayoutSettings.AllowAddNew = false;
                //Set the row we just created to the active record
                GridAdjustment.xGrid.ActiveRecord = GridAdjustment.xGrid.Records[0];
            }
            //Set the amt_to_adjust field as active, this will not work.  Will not go into edit mode immediately. Have to set to first editable field before it
            //(GridAdjustment.xGrid.Records[GridAdjustment.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsActive = true;
            //(GridAdjustment.xGrid.Records[GridAdjustment.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsSelected = true;
            GridAdjustment.CntrlFocus();
            //set focus on first editable field
            (GridAdjustment.xGrid.Records[0] as DataRecord).Cells["product_code"].IsActive = true;
            (GridAdjustment.xGrid.Records[0] as DataRecord).Cells["product_code"].IsSelected = true;
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
                        IsEmpty = r.Index  ;
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
                if (deleteEmpty && IsEmpty !=-1)
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
                txtGLOffsetAmt2.TextColor = "Black";
                txtRunningOffsetAmt.TextColor = "Black";
            }
            else
            {
                txtAdjAmt.TextColor = "Black";
                txtRunningAdjAmt.TextColor = "Black";
                txtGLOffsetAmt2.TextColor = "Red";
                txtRunningOffsetAmt.TextColor = "Red";
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
        /// works as cellLeave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            e.Handled = true;
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
                    if (GridRecord.Cells["amt_to_adjust"].Value.ToString() == "")
                    {
                        GridRecord.Cells["amt_to_adjust"].Value = 0.00;
                    }
                    double AdjTotal = 0.00;
                    AdjTotal = Convert.ToDouble(GridRecord.Cells["amt_to_adjust"].Value) * -1;
                    txtGLOffsetAmt2.Text = AdjTotal.ToString("0.00");
 
                    //Total Grid Cells
                    totGridRecs();
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0) txtRunningAdjAmt.TextColor = "Red";
                    else txtRunningAdjAmt.TextColor = "Black";
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    if (AmountsOffset()) btnSave.IsEnabled = true;
                    txtGLCompany.CntrlFocus();
                }
            }

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
                //sum adj amts
                AdjTotal += Convert.ToDouble(r.Cells["amt_to_adjust"].Value);
                RunningAdjTotal += Convert.ToDouble(r.Cells["amt_to_adjust"].Value) * -1;
            }
            txtRunningAdjAmt.Text = AdjTotal.ToString("0.00");
            txtRunningOffsetAmt.Text = RunningAdjTotal.ToString("0.00");
            txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
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
        /// populates grid and enables adj amt field if grid rows > 0
        /// </summary>
        private void popGrid()
        {
            //load parms 
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
            //Set the amt_to_adjust number field as active
            (GridAdjustment.xGrid.Records[GridAdjustment.ActiveRecord.Index] as DataRecord).Cells["amt_to_adjust"].IsActive = true;
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
        /// Save button get focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btnSave_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    //check for nulls
        //    if (txtRunningAdjAmt.Text == null)
        //        return;
        //    if (txtAdjAmt.Text == null)
        //        return;
        //    string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
        //    string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
        //    if (Convert.ToDouble(sRunningAdjAmt.Trim()) < Convert.ToDouble(sAdjAmt.Trim()))
        //    {

        //        if (Convert.ToDouble(sAdjAmt) != 0)
        //        {
        //            GridAdjustment.xGrid.Focus();  
        //            CreateNewRow();
        //        }
        //        return;
        //    }
        //}

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
            //make sure all offsetting account fields have been entered
            if (txtGLCompany.Text == "" || txtGLCompany.Text == null || txtGLCostCtr.Text == "" || txtGLCostCtr.Text == null ||
                txtGLAcct.Text == "" || txtGLAcct.Text == null || txtGLProduct.Text == "" || txtGLProduct.Text == null ||
                txtGeo.Text == "" || txtGeo.Text == null)
            {
                Messages.ShowWarning("Please enter offsetting accounting information. Save Cancelled");
                return;
            }
            //make sure offsetting amts are not 0
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            string sRunningOffsetAmt = UnformatTextField(txtRunningOffsetAmt.Text.Trim());
            string sGLOffsetAmt = UnformatTextField(txtGLOffsetAmt.Text.Trim());
            //make sure offsetting amts are not 0
            //if (Convert.ToDouble(txtRunningAdjAmt.Text.Trim()) + Convert.ToDouble(txtAdjAmt.Text.Trim()) + (Convert.ToDouble(txtAdjAmt.Text.Trim()) + Convert.ToDouble(txtRunningOffsetAmt.Text.Trim())) == 0)
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) + Convert.ToDouble(sAdjAmt.Trim()) + (Convert.ToDouble(sAdjAmt.Trim()) + Convert.ToDouble(sRunningOffsetAmt.Trim())) == 0)
            {
                Messages.ShowWarning("Adjustment Amount and Offsetting Amount Cannot Be $0.00. Save Cancelled");
                return;
            }
            if (AmountsOffset())
            {
                //make sure all offset values have been entered for every record in the grid whose amt to adj has a non zero value
                if (!GLOffsetValuesEnteredForEachGridRecGreaterThanZero()) return;
                //call method to insert a new conv adj and get new doc Id
                //string NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 19, cGlobals.UserName.ToLower(), Convert.ToDecimal(txtAdjAmt.Text));
                string NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 19, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));
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

        /// <summary>
        /// blank textbox for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAdjAmt_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            txtSelectAll(sender);
            if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }
            
            txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");
        }

        /// <summary>
        /// blank textbox for user
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

 
    }
}
