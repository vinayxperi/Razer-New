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
    /// This class represents a 'AdjustmentWriteOff' object.
    /// </summary>
    public partial class AdjustmentWriteoff : ScreenBase
    {
        public cBaseBusObject AdjWriteOffBusObject = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        public int writeoffType ;
        public string writeoffTypeValue;
        public string DocumentID { get; set; }
        private string CustomerId { get; set; }

        /// <summary>
        /// Create a new instance of a 'AdjustmentAccountingDocument' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentWriteoff(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = AdjWriteOffBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentWriteoff";
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
            GridWriteoff.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            GridWriteoff.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridWriteoff.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridWriteoff.SkipReadOnlyCellsOnTab = true;
            GridWriteoff.ContextMenuAddIsVisible = false;
            GridWriteoff.ContextMenuRemoveIsVisible = false;
            GridWriteoff.MainTableName = "main";
            GridWriteoff.ConfigFileName = "AdjustmentWriteoff";
            GridWriteoff.SetGridSelectionBehavior(false, false);
            GridWriteoff.xGrid.FieldSettings.AllowEdit = true;
            GridWriteoff.IsEnabled = false;
            GridWriteoff.FieldLayoutResourceString = "GridWriteoff";
            GridCollection.Add(GridWriteoff);
            loadParms();
            this.Load();
            //set focus when window opens
            txtCustomer.CntrlFocus();
        }

        private void txtCustomerId_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //call customer lookup
            RazerBase.Lookups.CustomerLookup customerLookup = new RazerBase.Lookups.CustomerLookup();
            customerLookup.Init(new cBaseBusObject("CustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            customerLookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //load current parms
                //loadParms("");
                txtCustomer.Text = cGlobals.ReturnParms[0].ToString();
                txtCustomerName.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
                popGrid();
            }
        }
        /// <param name="e"></param>
        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = GridWriteoff.ActiveRecord.Cells.IndexOf(e.Cell);

          

            //WARNING: You may need to change this if the order of the AdjustmentFieldLayouts\GridConversionInvoice changes
            if (edit_index == 7) //Amount To Adjust Field
            {
                //commit user entered value to datatable
                GridWriteoff.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                DataRecord GridRecord = null;
                GridRecord = GridWriteoff.ActiveRecord;
                DataRowView dr = GridRecord.DataItem as DataRowView;
                DataView dv = dr.DataView;
                if (GridRecord != null)
                {
                    //clear running total
                    txtRunningAdjAmt.Text = "";
                    //init AdjTotal w/starting value
                    double AdjTotal = 0.00;
                    double AdjTotalOffset = 0.00;
                    if (GridRecord.Cells["amount_to_adjust"].Value.ToString() == "")
                    {
                        GridRecord.Cells["amount_to_adjust"].Value = 0.00;
                    }
                    //loop through each grid record and add adj totals
                    foreach (DataRecord r in GridWriteoff.xGrid.Records)
                    {
                        //sum adj amts
                        DocumentID = Convert.ToString(r.Cells["apply_to_doc"].Value);
                        AdjTotal += Convert.ToDouble(r.Cells["amount_to_adjust"].Value);
                    }
                    txtRunningAdjAmt.Text = AdjTotal.ToString("0.00");
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0) txtRunningAdjAmt.TextColor = "Red";
                    else txtRunningAdjAmt.TextColor = "Black";
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    //set offsetting amount
                    AdjTotalOffset = AdjTotal * -1;
                    txtRunningOffsetAmt.Text = AdjTotalOffset.ToString("0.00");
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningOffsetAmt.Text) < 0) txtRunningOffsetAmt.TextColor = "Red";
                    else txtRunningOffsetAmt.TextColor = "Black";
                    txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
                    /////////////////////////////////////////////////////////////////////////////////////////////
                    //if amounts offset then enable save button
                    if (AmountsOffset()) btnSave.IsEnabled = true;
                }
            }
        }

       
        private void txtCustomer_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //do this to keep grid from loading and binding re-running clearing user entered values
            if (CustomerId != txtCustomer.Text)
            {
                txtCustomerName.Text = "";
                if (txtCustomer.Text != "")
                {
                    //populate grid
                    popGrid();
                }
            }
        }

        private void txtAdjAmt_PreviewKeyDown(object sender, KeyEventArgs e)
      
        {
            if (this.CurrentBusObj.ObjectData != null)
                if ((e.Key == Key.Tab || e.Key == Key.Enter) && this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
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
                        GridWriteoff.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                        e.Handled = true;
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
            if (Convert.ToDouble(txtAdjAmt.Text) > 0)
                {
                    Messages.ShowInformation("Writeoff Amount must be negative");
                    txtCustomer.CntrlFocus();
                    return;
                }
            
              
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtAdjAmt.Text);
            txtAdjAmt.Text = formatAmt.ToString("0.00");
            if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            {
                setEditScreenState();
                txtAdjAmt.TextColor = "Red";
                //if amt < 0 make neg set to red
                
                GridWriteoff.Focus();
                
                
            }
           




            //enable editable fields if adj value > 0
            if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            {
                setEditScreenState();
                //txtRunningAdjAmt.Text 
                
            }
            txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
            
        }

        
        private void txtAdjAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSelectAll(sender);
            if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }

            txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");
        }

      
        private void txtGLOffsetAmt_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            txtSelectAll(sender);
        }

       
        private void txtSelectAll(object sender)
        {
            if (sender is RazerBase.ucLabelTextBox)
                (sender as RazerBase.ucLabelTextBox).SelectAll();
        }

       
        /// populates grid and enables adj amt field if grid rows > 0
     
        private void popGrid()
        {
            loadParms();
            //clear totals
             //txtRunningOffsetAmt.Text = "0.00";
            //load the object
            this.Load();
           
            if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count > 0)
            {
                txtAdjAmt.IsEnabled = true;
                if (txtCustomerName.Text == "")
                    txtCustomerName.Text = this.CurrentBusObj.ObjectData.Tables[0].Rows[0]["account_name"].ToString();
            }
            else
            {
                Messages.ShowInformation("No documents Found");
                setInitScreenState();
            }
        }

  
        /// load parms for data svc
        
        private void loadParms()
        {
            //clear parm            
            this.CurrentBusObj.Parms.ClearParms();
            //add doc id parm to bus obj
            this.CurrentBusObj.Parms.AddParm("@receivable_account", txtCustomer.Text);
            CustomerId = txtCustomer.Text;
            //add parm for error messages
            this.CurrentBusObj.Parms.AddParm("@error_message", "");
        }

        /// <summary>
        /// enables editable fields when adj amt value > 0
        /// </summary>
        private void setEditScreenState()
        {
            //Enable fields
              GridWriteoff.IsEnabled = true;
           
            //Set the amount_adjusted field as active
            if (this.CurrentBusObj.ObjectData != null)
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
                    (GridWriteoff.xGrid.Records[GridWriteoff.ActiveRecord.Index] as DataRecord).Cells["amount_to_adjust"].IsActive = true;
        }
            

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            btnSave.IsEnabled = false;
            GridWriteoff.IsEnabled = false;
         
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

       

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Save();
        }

        public override void Save()
        {
            this.SaveSuccessful = false;
            string strErr = "";
            
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
                //change to a type = 10 for writeoff adjustment

                string NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(DocumentID.ToString(), 10, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));
               
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
                    }
                    else
                    {
                        //Messages.ShowInformation("Save Successful--New Adjustment ID = " + NewDocID);
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
                dr["adj_doc_id"] = DocID;
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
