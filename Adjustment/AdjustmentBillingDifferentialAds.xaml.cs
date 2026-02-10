

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
    /// This class represents a 'AdjustmentBillingDifferentialAds' object.
    /// </summary>
    public partial class AdjustmentBillingDifferentialAds : ScreenBase
    {
        public cBaseBusObject BillingDifferenceAdsObject = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        cBaseBusObject AccountingAdjustmentBusObj;
        public bool IsDocumentLoaded { get; set; }
        private double AdjMaxAmt { get; set; }
        double FXAmount;
        double AmountToAdjust;
        double USDAmount;
        double BillingDiffAmt;
        public bool Saved = false;
        private string newAdjID { get; set; }
        //string newProforma;


        public AdjustmentBillingDifferentialAds(cBaseBusObject _AccountingAdjustmentBusObj, AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = BillingDifferenceAdsObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentBillingDifferenceAds";
            AccountingAdjustmentBusObj = _AccountingAdjustmentBusObj;
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
            this.CurrentBusObj.Parms.AddParm("@amount_adjusted", "");
            this.CurrentBusObj.Parms.AddParm("@adj_document_id", "");
            //this.CurrentBusObj.Parms.AddParm("@apply_to_doc", "");
            this.Load();
            txtDocument.CntrlFocus();
        }

        private void txtDocument_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Document field
            AdsInvoiceFolderLookup f = new AdsInvoiceFolderLookup();
            //f.Init(new cBaseBusObject("CustomerLookup"));

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
                if (!txtDocument.Text.ToUpper().Contains("RA"))
                {
                    Messages.ShowInformation("Only Ads Invoices are valid");
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
                            if (!IsDocumentLoaded) txtDocument.CntrlFocus();
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
                txtAccountName.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["account_name"].ToString();
                txtCurrency.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["currency_code"].ToString();
                //this.txtAdjAmt.IsReadOnly = false;
                txtCNDAmt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["total_amt"].ToString().Trim();
                txtUSDAmt.Text = "0.00";
                FXAmount = Convert.ToDouble(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["total_amt"]);
                AmountToAdjust = Convert.ToDouble(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["total_amt"]);
                txtCNDAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtCNDAmt.Text));
                txtUSDAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtUSDAmt.Text));
                txtCNDAmt.CntrlFocus();
            }
            else
            {
                Messages.ShowInformation("Document Id Not Found.  Only non-USD invoices are allowed.");
                IsDocumentLoaded = false;
                txtAccountName.Text = "";
                txtCurrency.Text = "";
                txtCNDAmt.Text = "0.00";
                txtUSDAmt.Text = "0.00";
                txtBillingDiffAmt.Text = "0.00";
                btnSave.IsEnabled = false;
                //txtDocument.CntrlFocus();
            }
        }

        private void txtCNDAmt_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            txtCNDAmt.Text = txtCNDAmt.Text.Replace("$", "");
            txtCNDAmt.Text = txtCNDAmt.Text.Replace(",", "");
            txtCNDAmt.Text = txtCNDAmt.Text.Replace("(", "-");
            txtCNDAmt.Text = txtCNDAmt.Text.Replace(")", "");
            FXAmount = Convert.ToDouble(txtCNDAmt.Text);
            BillingDiffAmt = USDAmount - FXAmount;
            txtBillingDiffAmt.Text = BillingDiffAmt.ToString("0.00");
            if (Convert.ToDouble(txtBillingDiffAmt.Text) < 0) txtBillingDiffAmt.TextColor = "Red";
            else txtBillingDiffAmt.TextColor = "Black";
            if (Convert.ToDouble(txtBillingDiffAmt.Text) != 0)
                btnSave.IsEnabled = true;
            txtBillingDiffAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtBillingDiffAmt.Text));
            txtCNDAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtCNDAmt.Text));
            if (!IsDocumentLoaded) btnSave.IsEnabled = false;
        }

        private void txtUSDAmt_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            txtUSDAmt.Text = txtUSDAmt.Text.Replace("$", "");
            txtUSDAmt.Text = txtUSDAmt.Text.Replace(",", "");
            txtUSDAmt.Text = txtUSDAmt.Text.Replace("(", "-");
            txtUSDAmt.Text = txtUSDAmt.Text.Replace(")", "");
            USDAmount = Convert.ToDouble(txtUSDAmt.Text);
            BillingDiffAmt = USDAmount - FXAmount;
            txtBillingDiffAmt.Text = BillingDiffAmt.ToString("0.00");
            if (Convert.ToDouble(txtBillingDiffAmt.Text) < 0) txtBillingDiffAmt.TextColor = "Red";
            else txtBillingDiffAmt.TextColor = "Black";
            if (Convert.ToDouble(txtBillingDiffAmt.Text) != 0)
                btnSave.IsEnabled = true;
            txtBillingDiffAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtBillingDiffAmt.Text));
            txtUSDAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtUSDAmt.Text));
            if (!IsDocumentLoaded) btnSave.IsEnabled = false;
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
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count == 0)
                return;

            if (BillingDiffAmt > 0)
            {
                Messages.ShowError("Only Credits Allowed.  For Debits use Debit Accounting adjustment.");
                btnSave.IsEnabled = false;
                return;
            }

            this.SaveSuccessful = false;
            //string strErr = "";
            string sAdjAmt = UnformatTextField(txtBillingDiffAmt.Text.Trim());
            AmountToAdjust = (USDAmount - FXAmount) * -1;
            string sGLOffsetAmt = AmountToAdjust.ToString();
            string ErrMsg = "";
            string CustomerId = txtDocument.Text.ToUpper().Trim() + this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["receivable_account"].ToString();
            string ProductCode = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["product_code"].ToString();
            //string Currency = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["currency_code"].ToString();
            string GLCo = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["company_code"].ToString();
            if (GLCo == "0") GLCo = "00";
            string GLCostCtr = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["gl_center"].ToString();
            if (GLCostCtr == "0") GLCostCtr = "0000";
            string GLAcct = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["gl_acct"].ToString();
            if (GLAcct == "0") GLAcct = "00000";
            string GLRegion = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["geography"].ToString();
            if (GLRegion == "0") GLRegion = "0000";
            string GLProduct = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["gl_product"].ToString();
            if (GLProduct == "0") GLProduct = "0000";
            string OffsetGLCo = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_co"].ToString();
            if (OffsetGLCo == "0") OffsetGLCo = "00";
            string OffsetGLCostCtr = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_center"].ToString();
            if (OffsetGLCostCtr == "0") OffsetGLCostCtr = "0000";
            string OffsetGLAcct = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_account"].ToString();
            if (OffsetGLAcct == "0") OffsetGLAcct = "00000";
            string OffsetGLRegion = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_region"].ToString();
            if (OffsetGLRegion == "0") OffsetGLRegion = "0000";
            string OffsetGLProduct = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_product"].ToString();
            if (OffsetGLProduct == "0") OffsetGLProduct = "0000";
            string OffsetGLIC = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["offsetting_interco"].ToString();
            if (OffsetGLIC == "0") OffsetGLIC = "00";

            string NewDocID = cGlobals.BillService.InsertNewAdjustmentUACash(CustomerId, sAdjAmt, ProductCode, cGlobals.UserName, "USD", GLCo, GLCostCtr, GLAcct, GLRegion, GLProduct, OffsetGLCo, OffsetGLCostCtr, OffsetGLAcct, OffsetGLProduct, OffsetGLRegion, OffsetGLIC, sGLOffsetAmt, ref ErrMsg);
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
            }
            else
            {
                Messages.ShowInformation("Save Successful--New Adjustment ID = " + NewDocID);
                //pop new adjustment in folder
                this.CurrentBusObj.Parms.UpdateParmValue("@document_id", NewDocID.ToString());
                AdjFolderScreen.ReturnData(NewDocID, "");
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

            
        public override void Close()
        {
            if (Saved != true)
            {
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                if (txtBillingDiffAmt.Text != "" && txtBillingDiffAmt != null) ForceScreenDirty = true;
                if (txtDocument.Text != "" && txtDocument.Text != null) ForceScreenDirty = true;
            }
            else ForceScreenDirty = false;
            base.Close();
        }

        private void txtCNDAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            //newAdjID = txtDocument.Text;
        }

        private void txtUSDAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            //newAdjID = txtDocument.Text;
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
