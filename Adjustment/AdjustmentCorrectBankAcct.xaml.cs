using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentCorrectBankAcct' object.
    /// </summary>
    /// //THIS IS FUTURE ADJUSTMENT 
    public partial class AdjustmentCorrectBankAcct : ScreenBase, IPreBindable 
    {
        public cBaseBusObject AdjCorrectBankAcctBusObject = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        public int batchID { get; set; }
        public int newBankID;
        public string newCashAcct;
        public string newCurrencyCode;
        

        /// <summary>
        /// Create a new instance of a 'AdjustmentAccountingDocument' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentCorrectBankAcct(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = AdjCorrectBankAcctBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentCorrectBankAcct";
            //set Adj Folder screen obj
            AdjFolderScreen = _AdjFolderScreen;
            // This call is required by the designer.
            //'cmbNewBankID.PropertyChanged += new PropertyChangedEventHandler(cmbNewBankID_PropertyChanged);
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
           
            GridBankAcct.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridBankAcct.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridBankAcct.SkipReadOnlyCellsOnTab = true;
            GridBankAcct.ContextMenuAddIsVisible = false;
            GridBankAcct.ContextMenuRemoveIsVisible = false;
            GridBankAcct.MainTableName = "main";
            GridBankAcct.ConfigFileName = "AdjustmentCorrectBankAcct";
            GridBankAcct.SetGridSelectionBehavior(false, false);
            GridBankAcct.xGrid.FieldSettings.AllowEdit = false;
            GridBankAcct.IsEnabled = true;
            GridBankAcct.FieldLayoutResourceString = "GridBankAcct";
            GridCollection.Add(GridBankAcct);
            loadParms();
            this.Load();
           //set focus when window opens
            txtBatchID.CntrlFocus();
        }

       
      
      

       
        private void txtBatchID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //do this to keep grid from loading and binding re-running clearing user entered values
            if (batchID.ToString() != txtBatchID.Text)
            {
                if (txtBatchID.Text != "")
                {
                    //populate grid
                    popGrid();
                    batchID  = Convert.ToInt32(txtBatchID.Text);
                }
            }
        }

        public void PreBind()
        {
            //check this to keep recursion to this event from occurruing when load is called

            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                this.cmbNewBankID.SetBindingExpression("bank_id", "bank_name", this.CurrentBusObj.ObjectData.Tables["bank"]);

            }
        }

        private void cmbNewBankID_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LostFocus":

                    break;
                case "SelectedValue":
                    if (cmbNewBankID.SelectedValue == null) break;
                    if (cmbNewBankID.SelectedValue.ToString().Trim() != "")
                    {
                        DataTable dcr = this.CurrentBusObj.ObjectData.Tables["bank"];
                        foreach (DataRow item in dcr.Rows)
                        {
                            if (item["bank_id"].ToString() == cmbNewBankID.SelectedValue.ToString())
                            {
                                txtnewBankAcct.Text = item["account_number"].ToString();
                                newBankID = Convert.ToInt32(item["bank_id"]);
                                newCashAcct = item["cash_acct"].ToString();
                                newCurrencyCode = item["currency_code"].ToString();

                            }

                        }
                        btnSave.IsEnabled = true;
                       
                    }
                    break;
                default:
                    break;
            }
        }
       

 

        
       
       
       
        /// populates grid and enables adj amt field if grid rows > 0
     
        private void popGrid()
        {
            loadParms();
            //clear totals
            //txtRunningOffsetAmt.Text = "0.00";
            //load the object
            this.MainTableName = "main";
            this.Load();
           
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                txtAdjAmt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["batch_total"].ToString();

                txtAdjAmt.IsEnabled = false;
                txtAdjAmt.Text = (Convert.ToDouble(txtAdjAmt.Text) * -1).ToString();
                txtAdjAmt.TextColor = "Red";
                txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
                cmbNewBankID.IsEnabled = true;
            }
            else
            {
                Messages.ShowInformation("Batch Not Found");
                setInitScreenState();
            }
        }

  
        /// load parms for data svc
        
        private void loadParms()
        {
            //clear parm            
            this.CurrentBusObj.Parms.ClearParms();
            //add doc id parm to bus obj

            if (txtBatchID.Text == null)
            {
                batchID = -1;
                this.CurrentBusObj.Parms.AddParm("@batch_id", -1);
            }
            else
            {
                batchID = Convert.ToInt32(txtBatchID.Text);
                this.CurrentBusObj.Parms.AddParm("@batch_id", txtBatchID.Text);
            }
            //add parm for error messages
            this.CurrentBusObj.Parms.AddParm("@error_message", "");
        }

        /// <summary>
        /// enables editable fields when adj amt value > 0
        /// </summary>
        private void setEditScreenState()
        {
            //Enable fields
            GridBankAcct.IsEnabled = true;

        }
            

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            btnSave.IsEnabled = false;
            GridBankAcct.IsEnabled = false;
         
            //reset total to 0 when new doc selected
      
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
          
            
                //call method to insert a new conv adj and get new doc Id
                //change to a type = 7 for Correct Bank Account
              
                string NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtBatchID.Text, 7, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));
               
                //insert the new doc id in all existing grid rows for ins sp
                if (NewDocID == "")
                {
                    Messages.ShowError("Problem Creating New Adjustment");
                    return;
                }
                  string sAmtofAdj = UnformatTextField(txtAdjAmt.Text.Trim()); 
                //update values for the insert in the adjust table
            //Putting Batch ID to change in apply to doc just for this adjustment type
                this.CurrentBusObj.ObjectData.Tables["adjust"].Rows[0]["apply_to_doc"] = txtBatchID.Text.ToString();
            //putting new bank id in the Apply to Seq for this adjustment type
                this.CurrentBusObj.ObjectData.Tables["adjust"].Rows[0]["apply_to_seq"] = newBankID;
                this.CurrentBusObj.ObjectData.Tables["adjust"].Rows[0]["adj_document_id"] = NewDocID.ToString();
                this.CurrentBusObj.ObjectData.Tables["adjust"].Rows[0]["amount_to_adjust"] = Convert.ToDouble(sAmtofAdj.Trim());
                this.CurrentBusObj.ObjectData.Tables["adjust"].Rows[0]["adjustment_type"] = 7;
                this.CurrentBusObj.ObjectData.Tables["adjust"].Rows[0]["new_cash_acct"] = newCashAcct.ToString();
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
                {
                    this.CurrentBusObj.ObjectData.Tables["adjust"].Rows[0]["old_cash_acct"] = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["cash_acct"];
                    this.CurrentBusObj.ObjectData.Tables["adjust"].Rows[0]["old_currency_code"] = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["currency_code"];
                }
                  this.CurrentBusObj.ObjectData.Tables["adjust"].Rows[0]["new_currency_code"] = newCurrencyCode.ToString();
                //do this to run the INS stored proc instead of the UPD stored proc//////////
                DataTable dt = this.CurrentBusObj.ObjectData.Tables["adjust"];
                if (dt != null)
                {
                    dt.AcceptChanges();
                    foreach (DataRow row in dt.Rows)
                    {
                        row.SetAdded();
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

   
        /// sets up parms to prepare for delete of orphaned adj rec and calls delete
      
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
     

        private void txtBatchID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
             //call batch lookup
            RazerBase.Lookups.CashBatchDocumentLookup cashBatchDocumentLookup = new RazerBase.Lookups.CashBatchDocumentLookup();
            // gets the users response
            cashBatchDocumentLookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //load current parms
                //loadParms("");
                txtBatchID.Text = cGlobals.ReturnParms[0].ToString();
                txtAdjAmt.Text = cGlobals.ReturnParms[1].ToString();

                // Clear the parms
                cGlobals.ReturnParms.Clear();
                popGrid();
            }
        }

    }

}
