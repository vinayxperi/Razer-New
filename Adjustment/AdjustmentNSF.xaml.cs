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
    /// This class represents a 'AdjustmentNSF' object.  Insufficient Funds
    /// </summary>
    public partial class AdjustmentNSF : ScreenBase
    {
        public cBaseBusObject AdjNSFBusObject = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        
        private string DocumentId { get; set; }

        /// <summary>
        /// Create a new instance of a 'AdjustmentAccountingDocument' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentNSF(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = AdjNSFBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentNSF";
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
            GridNSF.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            GridNSF.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridNSF.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridNSF.SkipReadOnlyCellsOnTab = true;
            GridNSF.ContextMenuAddIsVisible = false;
            GridNSF.ContextMenuRemoveIsVisible = false;
            GridNSF.MainTableName = "main";
            GridNSF.ConfigFileName = "AdjustmentNSFDocument";
            GridNSF.SetGridSelectionBehavior(false, false);
            GridNSF.xGrid.FieldSettings.AllowEdit = true;
            GridNSF.IsEnabled = true;
            GridNSF.FieldLayoutResourceString = "GridNSF";
            GridCollection.Add(GridNSF);
            loadParms();
            this.Load();
            //set focus when window opens
            txtDocument.CntrlFocus();
        }

       
        /// <param name="e"></param>
        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = GridNSF.ActiveRecord.Cells.IndexOf(e.Cell);

          

            //WARNING: You may need to change this if the order of the AdjustmentFieldLayouts\GridConversionInvoice changes
            if (edit_index == 7) //Amount To Adjust Field
            {
                //commit user entered value to datatable
                GridNSF.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                DataRecord GridRecord = null;
                GridRecord = GridNSF.ActiveRecord;
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
                    foreach (DataRecord r in GridNSF.xGrid.Records)
                    {
                        //sum adj amts
                        AdjTotal += Convert.ToDouble(r.Cells["amount_to_adjust"].Value);
                    }
                    txtRunningAdjAmt.Text = AdjTotal.ToString("0.00");
                    //if less than zero turn red otherwise black/////////////////////////////////////////////////
                    if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0) txtRunningAdjAmt.TextColor = "Red";
                    else txtRunningAdjAmt.TextColor = "Black";
                    txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                    //set offsetting amount
                    AdjTotalOffset = AdjTotal ;
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

       
        private void txtDocument_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            Double result = 0;
            if (Double.TryParse(txtAdjAmt.Text, out result) == false)
            {
                txtAdjAmt.Text = "0.00";
            }
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
                        GridNSF.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                        e.Handled = true;
                    }
                }
        }
      

       
        private void txtAdjAmt_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //if txt is not numeric////////////////////////////////////////////////////////////
            //Double result = 0;
            //if (Double.TryParse(txtAdjAmt.Text, out result) == false)
            //{
            //    txtAdjAmt.Text = "0.00";
            //}
            /////////////////////////////////////////////////////////////////////////////////////
            ////set a default value if user skips
            ////if (txtAdjAmt.Text == "") txtAdjAmt.Text = "0.00";

            ////if (Convert.ToDouble(txtAdjAmt.Text) >= 0)
            ////{
            ////    Messages.ShowInformation("Amount must be negative.");
            ////    txtDocument.CntrlFocus();
            ////    return;
            ////}
             
              
            ////convert the value to a double and format to add trailing zeros if missing
            //double formatAmt = Convert.ToDouble(txtAdjAmt.Text);
            //txtAdjAmt.Text = formatAmt.ToString("0.00");
            //if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            //{
            //    setEditScreenState();
            //    //if amt < 0 make neg set to red
            //    if (Convert.ToDouble(txtAdjAmt.Text) > 0)
            //    {
                   
            //            txtAdjAmt.Text = (Convert.ToDouble(txtAdjAmt.Text)  ).ToString();
            //            txtAdjAmt.TextColor = "Black";
            //        }
            //        else
            //            if (Convert.ToDouble(txtAdjAmt.Text) <= 0)
            //            {
            //                txtAdjAmt.Text = (Convert.ToDouble(txtAdjAmt.Text) * -1).ToString();
            //                txtAdjAmt.TextColor = "Black";
            //            }
            //    }
            //    //if amt neg set to red
            //    else
            //    {
            //        txtAdjAmt.TextColor = "Red";
            //    }
            //    GridNSF.Focus();
                
                
          
           




            ////enable editable fields if adj value > 0
            //if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            //{
            //    setEditScreenState();
                
            //}
            //txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
            
        }

        
        private void txtAdjAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            //txtSelectAll(sender);
            //if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }

            //txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
            //txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
            //txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
            //txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");
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
                string PrevNSFAdjustment = this.CurrentBusObj.ObjectData.Tables[0].Rows[0]["prev_adjustment"].ToString();
                if (PrevNSFAdjustment != "          ")
                {
                    Messages.ShowInformation("NSF adjustment " + PrevNSFAdjustment + " already exists for this document");
                    txtDocument.CntrlFocus();
                    btnSave.IsEnabled = false;
                    return;
                }
                txtAdjAmt.IsEnabled = true;
                txtAdjAmt.Text = this.CurrentBusObj.ObjectData.Tables[0].Rows[0]["gross_amount"].ToString();
                txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
                txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
                txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
                txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");
                txtAdjAmt.IsEnabled = false;
                txtRunningAdjAmt.Text = txtAdjAmt.Text;
                //txtRunningAdjAmt.Text = this.CurrentBusObj.ObjectData.Tables[0].Rows[0]["gross_amount"].ToString();
                txtRunningAdjAmt.Text = txtRunningAdjAmt.Text.Replace("$", "");
                txtRunningAdjAmt.Text = txtRunningAdjAmt.Text.Replace(",", "");
                txtRunningAdjAmt.Text = txtRunningAdjAmt.Text.Replace("(", "-");
                txtRunningAdjAmt.Text = txtRunningAdjAmt.Text.Replace(")", "");
                txtRunningOffsetAmt.Text = ((Convert.ToDecimal(txtAdjAmt.Text)) * -1).ToString();
                txtRunningOffsetAmt.Text = txtRunningOffsetAmt.Text.Replace("$", "");
                txtRunningOffsetAmt.Text = txtRunningOffsetAmt.Text.Replace(",", "");
                txtRunningOffsetAmt.Text = txtRunningOffsetAmt.Text.Replace("(", "-");
                txtRunningOffsetAmt.Text = txtRunningOffsetAmt.Text.Replace(")", "");
                txtRunningOffsetAmt.Text = (Convert.ToDouble(txtAdjAmt.Text) * -1).ToString();
                txtRunningOffsetAmt.TextColor = "Red";
                txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
                txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
                txtRunningAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningAdjAmt.Text));
                btnSave.IsEnabled = true;
            }
            else
            {
                Messages.ShowInformation("Document Not Found");
                setInitScreenState();
            }
        }

  
        /// load parms for data svc
        
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
            GridNSF.IsEnabled = true;
           
            //Set the amount_adjusted field as active
            if (this.CurrentBusObj.ObjectData != null)
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
                    (GridNSF.xGrid.Records[GridNSF.ActiveRecord.Index] as DataRecord).Cells["amount_to_adjust"].IsActive = true;
        }
            

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            btnSave.IsEnabled = false;
            GridNSF.IsEnabled = false;
         
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
            if  ((Convert.ToDouble(sRunningAdjAmt.Trim()) + (Convert.ToDouble(sRunningOffsetAmt.Trim())) == 0 )) 
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
            //if  (Convert.ToDouble(sRunningAdjAmt.Trim()) +  
            //    Convert.ToDouble(sRunningOffsetAmt.Trim())  == 0)
            //{
            //}
            //else
            //{
            //    Messages.ShowWarning("Adjustment Amount and Offsetting Amount Must offset to $0.00. Save Cancelled");
            //    return;
            //}
            if (AmountsOffset())
            {
                //call method to insert a new conv adj and get new doc Id
                //change to a type = 6 for insufficient funds
              
                string NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 6, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));
               
                //insert the new doc id in all existing grid rows for ins sp
                if (NewDocID != "")
                    updGridRowsDocId(NewDocID);
                else
                {
                    Messages.ShowError("Problem Creating New Adjustment");
                    return;
                }
                //do this to run the INS stored proc instead of the UPD stored proc on row 1//////////
                DataTable dt = this.CurrentBusObj.ObjectData.Tables["main"];
                if (dt != null)
                {
                    //dt.AcceptChanges();
                    //foreach (DataRow row in dt.Rows)
                    //{
                    //    row.SetAdded();
                    //}
                    DataRow dr = dt.Rows[0];
                    dr["amount_to_adjust"] = dr["gross_amount"];
                    dr.AcceptChanges();
                    dr.SetAdded();
                    //dr.SetAdded();
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
