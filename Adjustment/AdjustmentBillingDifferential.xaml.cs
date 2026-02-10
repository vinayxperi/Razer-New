

using RazerBase;
using RazerInterface;
using RazerBase.Lookups;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Windows.Input;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentBillingDifferential' object.
    /// </summary>
    public partial class AdjustmentBillingDifferential : ScreenBase
    {
        public cBaseBusObject BillingDifferenceObject = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        //public bool IsLoading { get; set; }
        public bool IsDocumentLoaded { get; set; }
        private double AdjMaxAmt { get; set; }
        double RazerFX;
        double AmountToAdjust;
        double USDAmount;
        double BillingDiffAmt;
        //private bool MaxNumAdjsExceeded { get; set; }
        public bool Saved = false;
        private string newAdjID { get; set; }
        string newProforma;

      
        public AdjustmentBillingDifferential(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = BillingDifferenceObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentBillingDifference";
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
            //do this to pop product dropdown
            this.CurrentBusObj.Parms.ClearParms();
            this.CurrentBusObj.Parms.AddParm("@document_id", "");
            this.CurrentBusObj.Parms.AddParm("@proforma_id", "");
            this.CurrentBusObj.Parms.AddParm("@amount_adjusted", "");
            this.CurrentBusObj.Parms.AddParm("@adj_document_id", "");
            this.CurrentBusObj.Parms.AddParm("@apply_to_doc", "");
            this.Load();
            txtDocument.CntrlFocus();
        }

        private void txtDocument_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Entity ID field
            InvoiceFolderLookup f = new InvoiceFolderLookup();

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            //DWR - Modifie 3/11/13 - Added 2nd part of if check as screen would crash app if filter and then cancel was clicked in the lookup.
            if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0] != null)
            {
                //load current parms
                //loadParms("");
                txtDocument.Text = cGlobals.ReturnParms[0].ToString();
                if (txtDocument.Text != newAdjID)
                {
                    newAdjID = txtDocument.Text;
                    if (txtDocument.Text != "" && txtDocument.Text != null)
                    {
                        //populate fields
                        popFields();
                    }
                }
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }
       
        private void txtDocument_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtDocument.Text != "" && txtDocument.Text != null)
            {
                if (txtDocument.Text.ToUpper().Contains("MPINV"))
                {
                    Messages.ShowInformation("Proforma not a valid document to adjust");
                    btnSave.IsEnabled = false;
                    //txtDocument.CntrlFocus();
                    //return;
                }
                else
                {
                    //cbirney added the newAdjID so it only loads again if the adjustment id has changed
                    if (txtDocument.Text != newAdjID)
                    {
                        newAdjID = txtDocument.Text;
                        if (txtDocument.Text != "" && txtDocument.Text != null)
                        {
                            //populate fields
                            popFields();
                            if (this.CurrentBusObj.ObjectData.Tables["proforma"].Rows.Count > 0)
                            {
                                popProformaFields();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// populates fields
        /// </summary>
        private void popFields()
        {
            //change parm to new value
            this.CurrentBusObj.changeParm("@document_id", txtDocument.Text.Trim());
            //this.Load();
            this.CurrentBusObj.LoadData("main");
            //IsLoading = false;
            //////////////////////////////////////////////////////////////////////////
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                IsDocumentLoaded = true;
                this.txtAdjAmt.IsReadOnly = false;
                txtAdjAmt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["invoice_amount"].ToString().Trim();
                AdjMaxAmt = Convert.ToDouble(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["invoice_amount"]);
                AmountToAdjust = Convert.ToDouble(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["invoice_amount"]);
                txtAdjAmt.CntrlFocus();
            }
            else
            {
                Messages.ShowInformation("Document Id Not Found");
                IsDocumentLoaded = false;
                //setInitScreenState();
            }
        }

        private void txtAdjAmt_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (sender is RazerBase.ucLabelTextBox)
            //    (sender as RazerBase.ucLabelTextBox).SelectAll();

            //if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }
            txtSelectAll(sender);
            txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");

        }

        private void txtSelectAll(object sender)
        {
            if (sender is RazerBase.ucLabelTextBox)
                (sender as RazerBase.ucLabelTextBox).SelectAll();
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
            //if (AdjMaxAmt != null)
            //{
            //    //if (Convert.ToDouble(txtAdjAmt.Text) > AdjMaxAmt || Convert.ToDouble(txtAdjAmt.Text) < AdjMaxAmt * -1)
            //    if (Convert.ToDouble(txtAdjAmt.Text) > AdjMaxAmt || Convert.ToDouble(txtAdjAmt.Text) < AdjMaxAmt * -1)
            //    {
            //        Messages.ShowWarning("Cannot adjust more than the original invoice amount of " + AdjMaxAmt.ToString());                     
            //        return;
            //    }
            //}
            //set a default value if user skips
            if (txtAdjAmt.Text == "") txtAdjAmt.Text = "0.00";
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtAdjAmt.Text);
            txtAdjAmt.Text = formatAmt.ToString("0.00");
            AmountToAdjust = formatAmt;
            txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
            if (this.CurrentBusObj.ObjectData.Tables["proforma"].Rows.Count > 0)
            {
                popProformaFields();
                //RazerFX = AmountToAdjust / Convert.ToDouble(this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["net_amt"]);
                //txtRazerFX.Text = RazerFX.ToString("0.00");
                //txtBillingDiffAmt.Text = BillingDiffAmt.ToString("0.00");
                //if (Convert.ToDouble(txtBillingDiffAmt.Text) < 0) txtBillingDiffAmt.TextColor = "Red";
                //else txtBillingDiffAmt.TextColor = "Black";
                //if (Convert.ToDouble(txtBillingDiffAmt.Text) != 0)
                //    btnSave.IsEnabled = true;
                //else
                //    btnSave.IsEnabled = false;
                // txtBillingDiffAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtBillingDiffAmt.Text));
            }
        }

         private void txtProforma_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AdjustmentBillingDiffLookup w = new AdjustmentBillingDiffLookup();
            w.ShowDialog();
            //if (cGlobals.ReturnParms.Count > 0)
            //{
            //    txtProforma.Text = cGlobals.ReturnParms[0].ToString();
            //    // Clear the Global parms
            //    //This prevents invalid data being passed to other lookups
            //    cGlobals.ReturnParms.Clear();
            //    CurrentState = ScreenState.Normal;
               
            //}
            if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0] != null)
            {
                //load current parms
                //loadParms("");
                txtProforma.Text = cGlobals.ReturnParms[0].ToString();
                if (txtDocument.Text != newProforma)
                {
                    newProforma = txtDocument.Text;
                    if (txtProforma.Text != "" && txtProforma.Text != null)
                    {
                        //populate fields
                        popProformaFields();
                    }
                }
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }
        
        private void txtProforma_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {              
            if (txtProforma.Text != "" && txtProforma.Text != null)
            {
                if (!txtProforma.Text.ToUpper().Contains("MPINV"))
                {
                    Messages.ShowInformation("Invoice not a Proforma");
                    btnSave.IsEnabled = false;
                    return;
                }

                if (txtProforma.Text != newProforma)
                {
                    newProforma = txtDocument.Text;
                    if (txtProforma.Text != "" && txtProforma.Text != null)
                    {
                        //populate fields
                        popProformaFields();
                    }
                }
            }            
        }
     
        private void popProformaFields()
        {
            //change parm to new value
            this.CurrentBusObj.changeParm("@proforma_id", txtProforma.Text.Trim());
            //this.Load();
            this.CurrentBusObj.LoadData("proforma");
            if (this.CurrentBusObj.ObjectData.Tables["proforma"].Rows.Count > 0)
            {
                IsDocumentLoaded = true;
                //this.txtAdjAmt.IsReadOnly = true;
                txtProformaAmt.Text = this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["net_amt"].ToString().Trim();
                txtProformaCurrency.Text = this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["currency_code"].ToString().Trim();
                RazerFX = AmountToAdjust / Convert.ToDouble(this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["net_amt"]);
                txtRazerFX.Text = RazerFX.ToString("0.00");
                txtFromCurrency.Text = this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["from_currency_code"].ToString().Trim();
                txtToCurrency.Text = this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["to_currency_code"].ToString().Trim();
                txtConversionDate.Text = this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["conversion_date"].ToString().Trim();
                txtFXRate.Text = this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["conversion_rate"].ToString().Trim();
                USDAmount = Convert.ToDouble(this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["usd_invoice_amount"]);
                txtConversionAmount.Text = this.CurrentBusObj.ObjectData.Tables["proforma"].Rows[0]["usd_invoice_amount"].ToString().Trim();
                BillingDiffAmt = USDAmount - AmountToAdjust;
                txtBillingDiffAmt.Text = BillingDiffAmt.ToString("0.00");
                if (Convert.ToDouble(txtBillingDiffAmt.Text) < 0) txtBillingDiffAmt.TextColor = "Red";
                else txtBillingDiffAmt.TextColor = "Black";
                //if (txtGLOffsetAmt.Text == "") txtGLOffsetAmt.Text = "0.00";
                if (Convert.ToDouble(txtBillingDiffAmt.Text) != 0)
                    btnSave.IsEnabled = true;
                //txtAdjAmt.CntrlFocus();
                txtBillingDiffAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtBillingDiffAmt.Text));
                txtConversionAmount.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtConversionAmount.Text));
                //txtProformaAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtProformaAmt.Text));
            }
            else
            {
                Messages.ShowInformation("Proforma Invoice Not Found");
                IsDocumentLoaded = false;
                //setInitScreenState();
            }
        }
        

        /// <summary>
        /// enables editable fields when adj amt value > 0
        /// </summary>
        //private void setEditScreenState()
        //{
        //    //Enable fields
        //    txtAdjAmt.IsReadOnly = true;
        //    txtGLOffsetAmt.IsReadOnly = true;
        //    /////////////////////////////////////////////////////
        //}

        ///// <summary>
        ///// occurs when a new document is selected
        ///// </summary>
        //private void setInitScreenState()
        //{
        //    //Disable fields
        //    txtGLAmt.IsReadOnly = true;
        //    txtGLOffsetAmt.IsReadOnly = true;
        //    //////////////////////////////////
        //}

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
            if (txtDocument.Text == "" || txtDocument.Text == null)                
                return;
 
            if (txtDocument.Text.ToUpper().Contains("MPINV"))
            {
                Messages.ShowInformation("Proforma not a valid document to adjust");
                btnSave.IsEnabled = false;
                //txtDocument.CntrlFocus();
                return;
            }
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count == 0)
                return;
            if (this.CurrentBusObj.ObjectData.Tables["proforma"].Rows.Count == 0)
                return;
           
            this.SaveSuccessful = false;

            string sUnformattedTextField = txtBillingDiffAmt.Text.ToString().Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");
            string sAdjAmt = sUnformattedTextField;
            sUnformattedTextField = txtProformaAmt.Text.ToString().Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");
            string sProformaAmt = sUnformattedTextField;
            sUnformattedTextField = txtConversionAmount.Text.ToString().Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");
            string sUSDAmt = sUnformattedTextField;
            
            //if (Convert.ToDouble(txtRunningAdjAmt.Text.Trim()) + Convert.ToDouble(txtAdjAmt.Text.Trim()) + (Convert.ToDouble(txtAdjAmt.Text.Trim()) + Convert.ToDouble(txtRunningOffsetAmt.Text.Trim())) == 0)
            if (Convert.ToDouble(sAdjAmt.Trim())  == 0)
            {
                Messages.ShowWarning("Cannot create adjustment when adjusted amount is 0");
                return;
            }

            string NewDocID = "";
            string NewAdjID = "";
            //string SPErrMsg = "";
            string ErrMsg = "";
            Int32 AdjType = 1;

            //if adjusted amount is positive then the adjustment type is debit else it is credit
            if (Convert.ToDouble(sAdjAmt.Trim()) > 0) AdjType = 2;
            if (txtDocument.Text.ToUpper().Contains("MINV")) AdjType = 3;
          
            NewAdjID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, AdjType, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));
            if (NewAdjID == "")
            {
                Messages.ShowError("Problem Creating New Adjustment");
                return;
            }

            //NewDocID = cGlobals.BillService.InsertNewAdjustmentBillingDiff(txtDocument.Text, NewAdjID, sAdjAmt, txtProforma.Text, sProformaAmt, txtFromCurrency.Text, txtConversionDate.Text, txtFXRate.Text, sUSDAmt, ref ErrMsg);
            NewDocID = cGlobals.BillService.InsertNewAdjustmentBillingDiff(txtDocument.Text, NewAdjID, sAdjAmt, txtProforma.Text, sProformaAmt, txtFromCurrency.Text, txtConversionDate.Text, txtFXRate.Text, sUSDAmt, ref ErrMsg);
            if (NewDocID == "")
            {
                //error messages from Stored Proc
                if (ErrMsg != "")
                {
                    Messages.ShowError(ErrMsg);
                }
                else
                {
                    //save failed with no error msg
                    Messages.ShowInformation("Data Error, Save Failed");
                }
                rollbackAdj(NewDocID);
            }
            else
            {
                Messages.ShowInformation("Save Successful--New Adjustment ID = " + NewDocID);
                //pop new adjustment in folder 
                
                this.CurrentBusObj.Parms.UpdateParmValue("@document_id", NewDocID.ToString());

                AdjFolderScreen.ReturnData(NewDocID, " ");
                //add user name to AdjGeneralTab
                AdjFolderScreen.AdjustmentGeneralTab.txtCreatedBy.Text = cGlobals.UserName.ToLower();
                //close me
                System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

                //this.CurrentBusObj.ObjectData.AcceptChanges();
                StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
                this.SaveSuccessful = true;
                Saved = true;

                if (!ScreenBaseIsClosing)
                {
                    AdjParent.Close();
                }
            }          
        }

        private void rollbackAdj(string NewDocID)
        {
            //delete adj header
            bool retVal = cGlobals.BillService.DeleteNewAdjusmentPreamble(NewDocID);
        }
            

        //private string getInfoFromStoredProc()
        //{
        //    var SPErrorMsg = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
        //                     where x.Field<string>("parmName") == "@error_message"
        //                     select new
        //                     {
        //                         parmName = x.Field<string>("parmName"),
        //                         parmValue = x.Field<string>("parmValue")
        //                     };
        //    foreach (var info in SPErrorMsg)
        //    {
        //        if (info.parmName == "@error_message")
        //            return info.parmValue;
        //    }
        //    return "";
        //}

        /// <summary>
        /// gets new adj id
        /// </summary>
        /// <returns></returns>
        //private string getNewAdjIdFromStoredProc()
        //{
        //    var SPErrorMsg = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
        //                     where x.Field<string>("parmName") == "@adj_document_id"
        //                     select new
        //                     {
        //                         parmName = x.Field<string>("parmName"),
        //                         parmValue = x.Field<string>("parmValue")
        //                     };
        //    foreach (var info in SPErrorMsg)
        //    {
        //        if (info.parmName == "@adj_document_id")
        //            return info.parmValue;
        //    }
        //    return "";
        //}            

        public override void Close()
        {
            if (Saved != true)
            {
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); 
                if (txtAdjAmt.Text != "" && txtAdjAmt != null) ForceScreenDirty = true;
                if (txtDocument.Text != "" && txtDocument.Text != null) ForceScreenDirty = true;
            }
            else ForceScreenDirty = false;
            base.Close();
        }

        private void txtDocument_GotFocus(object sender, RoutedEventArgs e)
        {
            newAdjID = txtDocument.Text;
        }

    }
}
