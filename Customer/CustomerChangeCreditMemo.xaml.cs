using RazerBase.Interfaces;
using RazerBase;
using RazerInterface;
using RazerBase.Lookups;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Linq;
using Microsoft.VisualBasic;
using System.ComponentModel;

namespace Customer
{

    /// Interaction logic for CustomerChangeCreditMemo.xaml

    public partial class CustomerChangeCreditMemo : ScreenBase  
    {

        private static readonly string mainTableName = "creditMemo";
        public cBaseBusObject CreditMemoUpdate = new cBaseBusObject();
        public cBaseBusObject CustomerBusObj = new cBaseBusObject();
        string DocumentID = "";
        int SeqID;
        Boolean NewCMFlag = false;
        String sOverrideBadDebt;
        String sOverrideDeferred;
        String sQtrFirstDetermined;
        int ichkOverride;

        public string WindowCaption { get { return string.Empty; } }


        public CustomerChangeCreditMemo(string documentID, int seqID, cBaseBusObject _CustomerBusObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = CreditMemoUpdate;
            //name obj
            this.CurrentBusObj.BusObjectName = "CreditMemoUpdate";
            //Save customer business object for reload of aging tab
            CustomerBusObj = _CustomerBusObj;
            DocumentID = documentID;
            SeqID = seqID;
             
            InitializeComponent();

            // Perform initializations for this object
            Init();
           
        }

     
        public void Init()
        {
           
           
            this.CanExecuteSaveCommand = true;
            //this.CanExecuteCloseCommand = false;
            //this.ForceScreenDirty = true;
            txtdocumentID.Text = DocumentID;
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            this.MainTableName = "main";
            this.CurrentBusObj.Parms.AddParm("@document_id", DocumentID);

            this.CurrentBusObj.Parms.AddParm("@recv_doc_line", SeqID);
            this.Load();

            //RES 10/13/17 check for situation that will cause bad GL entries
            if (this.CurrentBusObj.ObjectData.Tables["defpool"].Rows[0]["bad_gl_flag"].ToString() == "1")
                //MessageBox.Show("Changing or adding an override on this document will result in bad General Ledger entries!","Warning");
                Messages.ShowWarning("Changing or adding an override on this document will result in bad General Ledger entries!");

                                                               
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["main"] != null && CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                txtProduct.Text = this.CurrentBusObj.ObjectData.Tables["product"].Rows[0]["product_code"].ToString();
                    //txtBadDebt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["bad_debt_amount"].ToString();
                    txtBadDebt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["current_bad_debt_amount"].ToString();
                    txtBadDebt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtBadDebt.Text));
                    //txtDeferred.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["credit_memo_amount"].ToString();
                    txtDeferred.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["current_credit_memo_amount"].ToString();
                    txtDeferred.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtDeferred.Text));
                    //txtOverrideBadDebt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_bad_debt_amount"].ToString();
                    //txtOverrideBadDebt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideBadDebt.Text));
                    //txtOverrideDeferred.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_credit_memo_amount"].ToString();
                    //txtOverrideDeferred.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideDeferred.Text));
                    txtQtrFirstDetermined.SelText = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["qtr_first_determined"]);
                    if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"].ToString() == "1")
                    {
                        chkOverride.IsChecked = 1;
                        txtOverrideBadDebt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_bad_debt_amount"].ToString();
                        txtOverrideBadDebt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideBadDebt.Text));
                        txtOverrideDeferred.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_credit_memo_amount"].ToString();
                        txtOverrideDeferred.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideDeferred.Text));
                    }
                    else
                    {
                        chkOverride.IsChecked = 0;
                        txtOverrideBadDebt.Text = "0";
                        txtOverrideBadDebt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideBadDebt.Text));
                        txtOverrideDeferred.Text = "0";
                        txtOverrideDeferred.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideDeferred.Text));
                    }
                    //RES 1/15/15 save initial values to compare when exiting to be able to issue save prompt if anything has changed
                    sOverrideBadDebt = txtOverrideBadDebt.Text;
                    sOverrideDeferred = txtOverrideDeferred.Text;
                    sQtrFirstDetermined = txtQtrFirstDetermined.SelText.ToString();
                    ichkOverride = chkOverride.IsChecked;
                }
            else
                this.New();
        }

        public override void New()
        {
            //Need this to insert a blank row into the datatable on a row that doesnt exist in the table yet
            this.CurrentBusObj.ObjectData.Tables["main"].Rows.Add();
            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["document_id"] = DocumentID;
            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["recv_doc_line"] = SeqID;
            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["qtr_first_determined"] = "01/01/1900";
            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"] = "0";
            chkOverride.IsChecked = 0;

            txtdocumentID.Text = DocumentID;
            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["product_code"] = this.CurrentBusObj.ObjectData.Tables["product"].Rows[0]["product_code"].ToString();
            txtProduct.Text = this.CurrentBusObj.ObjectData.Tables["product"].Rows[0]["product_code"].ToString();
            txtBadDebt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["bad_debt_amount"].ToString();
            txtBadDebt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtBadDebt.Text));
            txtDeferred.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["credit_memo_amount"].ToString();
            txtDeferred.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtDeferred.Text));
            txtOverrideBadDebt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_bad_debt_amount"].ToString();
            txtOverrideBadDebt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideBadDebt.Text));
            txtOverrideDeferred.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_credit_memo_amount"].ToString();
            txtOverrideDeferred.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideDeferred.Text));
            txtQtrFirstDetermined.SelText = Convert.ToDateTime("01/01/1900");

            //RES 1/15/15 save initial values to compare when exiting to be able to issue save prompt if anything has changed
            sOverrideBadDebt = txtOverrideBadDebt.Text;
            sOverrideDeferred = txtOverrideDeferred.Text;
            sQtrFirstDetermined = txtQtrFirstDetermined.SelText.ToString();
            ichkOverride = chkOverride.IsChecked;
             
            
        }


        private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.CanExecuteCloseCommand = true;
            //this.CurrentBusObj.ObjectData.AcceptChanges();
            //RES 1/15/15 check if values have changed without a save. Prompt for save
           
            //Messages.ShowWarning("sQtrFirstDetermined " + sQtrFirstDetermined + "txtQtrFirstDetermined " + txtQtrFirstDetermined.SelText.ToString());
            if ((sOverrideBadDebt != txtOverrideBadDebt.Text) || (sOverrideDeferred != txtOverrideDeferred.Text) || (sQtrFirstDetermined != txtQtrFirstDetermined.SelText.ToString()) ||
                (ichkOverride != chkOverride.IsChecked))
            {
                var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                {
                    if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["document_id"].ToString() == "")
                    {//Need to set status_code to either 0 (None) or 3 Deferred
                        this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["document_id"] = DocumentID.ToString();
                        this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["recv_doc_line"] = SeqID.ToString();
                        this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["posted_flag"] = 0;
                    }
                    else
                    {
                        string sDeferred = UnformatTextField(txtOverrideDeferred.Text.Trim());
                        string sBadDebt = UnformatTextField(txtOverrideBadDebt.Text.Trim());
                        double totAmount = 0;
                        double totOpen = 0;
                        double InvAmount = 0;
                        if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["open"] != null && CurrentBusObj.ObjectData.Tables["open"].Rows.Count > 0)
                        {
                            totOpen = Convert.ToDouble(CurrentBusObj.ObjectData.Tables["open"].Rows[0]["open_amount"]);
                            InvAmount = Convert.ToDouble(CurrentBusObj.ObjectData.Tables["open"].Rows[0]["amount"]);
                        }
                        else
                            totOpen = 0;
                        //update the status and move in columns as entered - validate the amounts summed are not greater than the open amount
                        totAmount = Convert.ToDouble(sDeferred.Trim()) + Convert.ToDouble(sBadDebt.Trim());
                        if (totAmount > totOpen)
                        {
                            Messages.ShowWarning("Amounts Summed cannot exceed the open amount of the document");
                            return;
                        }
                        //RES 5/6/16 check if override values have decreased without going to 0 and cash has been applied
                        if (InvAmount > totOpen)
                        {
                            double OverrideBD = 0;
                            double OverrideDef = 0;
                            double NewOverrideBD = 0;
                            double NewOverrideDef = 0;
                            NewOverrideBD = Convert.ToDouble(UnformatTextField(txtOverrideBadDebt.Text.Trim()));
                            NewOverrideDef = Convert.ToDouble(UnformatTextField(txtOverrideDeferred.Text.Trim()));
                            OverrideBD = Convert.ToDouble(UnformatTextField(sOverrideBadDebt.Trim()));
                            OverrideDef = Convert.ToDouble(UnformatTextField(sOverrideDeferred.Trim())); 
                            if (((OverrideBD > NewOverrideBD) && NewOverrideBD != 0) || ((OverrideDef > NewOverrideDef) && NewOverrideDef != 0))
                            {
                                Messages.ShowWarning("Reset override to $0 and run deferred.  Then change override back to your amount.");
                                return;
                            }
                        }
                    }
                    this.Save();
                }
            }
            CloseScreen();
        }

        // Save the changes
        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {


            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["document_id"].ToString() == "")
            {//Need to set status_code to either 0 (None) or 3 Deferred
                this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["document_id"] = DocumentID.ToString();
                this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["recv_doc_line"] = SeqID.ToString();
                this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["posted_flag"] = 0;
            }
            else
            {
                string sDeferred = UnformatTextField(txtOverrideDeferred.Text.Trim());
                string sBadDebt = UnformatTextField(txtOverrideBadDebt.Text.Trim());
                double totAmount = 0;
                double totOpen = 0;
                if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["open"] != null && CurrentBusObj.ObjectData.Tables["open"].Rows.Count > 0)
                    totOpen = Convert.ToDouble(CurrentBusObj.ObjectData.Tables["open"].Rows[0]["open_amount"]);
                else
                    totOpen = 0;
                //update the status and move in columns as entered - validate the amounts summed are not greater than the open amount
                totAmount = Convert.ToDouble(sDeferred.Trim()) + Convert.ToDouble(sBadDebt.Trim());
                if (totAmount > totOpen)
                {
                    Messages.ShowWarning("Amounts Summed cannot exceed the open amount of the document");
                    return;
                }
            }
            this.Save();
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




        public override void Save()
        {
            //Move values to table since it is not bound

            //RES 6/6/19 Check for unappied debit apply cash adjustments since this document will now be deferred
            if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["defpool"].Rows[0]["debitapplycash"].ToString()) > 0)
            {
                Messages.ShowWarning("There are unposted debit apply cash adjustments for this document!  It cannot be overridden until they are posted.");
                return;
            }

            //RES 10/15/20 Check for unrecognized time based deferrals if adding a bad debt override amount
            if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["defpool"].Rows[0]["unrecognized"].ToString()) > 0 &&
                Convert.ToDecimal(UnformatTextField(txtOverrideBadDebt.Text.Trim())) > 0)
            {
                var result = MessageBox.Show("There are unrecognized service periods for this document!  Do you want to continue?", "Check",MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;               
            }
           
            if (chkOverride.IsChecked == 1)
               this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"] = "1";
            else
                this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"] = "0";
            
            string sCreditMemoAmt = UnformatTextField(txtDeferred.Text.Trim());
            decimal test;
            if (!decimal.TryParse(sCreditMemoAmt, out test))
            {
                Messages.ShowInformation("Credit Memo Amount is not valid.");
                return;
            }
            //this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["credit_memo_amount"] = Convert.ToDecimal(sCreditMemoAmt);
            //this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["credit_memo_amount"] = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["credit_memo_amount"];
            string sBadDebtAmt = UnformatTextField(txtBadDebt.Text.Trim());
            if (!decimal.TryParse(sBadDebtAmt, out test))
            {
                Messages.ShowInformation("Bad Debt Amount is not valid.");
                return;
            }
            //this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["bad_debt_amount"] = Convert.ToDecimal(sBadDebtAmt);
            //this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["bad_debt_amount"] = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["bad_debt_amount"];
            string sOverrideCreditMemoAmt = UnformatTextField(txtOverrideDeferred.Text.Trim());
            if (!decimal.TryParse(sOverrideCreditMemoAmt, out test))
            {
                Messages.ShowInformation("Override Credit Memo Amount is not valid.");
                return;
            }
            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_credit_memo_amount"] = Convert.ToDecimal(sOverrideCreditMemoAmt);
            string sOverrideBadDebtAmt = UnformatTextField(txtOverrideBadDebt.Text.Trim());
            if (!decimal.TryParse(sOverrideBadDebtAmt, out test))
            {
                Messages.ShowInformation("Override Bad Debt Amount is not valid.");
                return;
            }

            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_bad_debt_amount"] = Convert.ToDecimal(sOverrideBadDebtAmt);
            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["qtr_first_determined"] = txtQtrFirstDetermined.SelText;
            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Save Successful");
                //No need to refresh  
                //add customer_id parm for refresh of aging detail grid on aging tab

                //RES 1/15/15 save initial values to compare when exiting to be able to issue save prompt if anything has changed
                sOverrideBadDebt = txtOverrideBadDebt.Text;
                sOverrideDeferred = txtOverrideDeferred.Text;
                sQtrFirstDetermined = txtQtrFirstDetermined.SelText.ToString();
                ichkOverride = chkOverride.IsChecked;
                 
               CustomerBusObj.LoadTable("aging_detail");
               CloseScreen();
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }

        //public new void Close()
        //{
        //    this.CurrentBusObj.ObjectData.AcceptChanges();
        //    //RES 1/15/15 check if values have changed without a save. Prompt for save
        //    if ((sOverrideBadDebt != txtOverrideBadDebt.Text) || (sOverrideDeferred != txtOverrideDeferred.Text) || (sQtrFirstDetermined != txtQtrFirstDetermined.ToString()) ||
        //        (ichkOverride != chkOverride.IsChecked))
        //    {
        //        var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
        //        if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
        //        {
        //            this.Save();
        //            //@@Need to add code here to stop the window from closing if save fails
        //            StopCloseIfCancelCloseOnSaveConfirmationTrue = true;
        //        }
        //    }
        //}
        private void CloseScreen()
        {

            System.Windows.Window CustomerParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            //RES 1/15/15 check if values have changed without a save. Prompt for save
            //if ((sOverrideBadDebt != txtOverrideBadDebt.Text) || (sOverrideDeferred != txtOverrideDeferred.Text) || (sQtrFirstDetermined != txtQtrFirstDetermined.ToString()) ||
            //    (ichkOverride != chkOverride.IsChecked))
            //{
            //    var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
            //    if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
            //    {
            //        this.Save();
            //        //@@Need to add code here to stop the window from closing if save fails
            //        StopCloseIfCancelCloseOnSaveConfirmationTrue = true;
            //    }
            //}

            if (!ScreenBaseIsClosing)
            {
                CustomerParent.Close();
            }
        }

        private void txtOverrideBadDebt_LostFocus(object sender, RoutedEventArgs e)
        {
            string sOverrideBadDebt = UnformatTextField(txtOverrideBadDebt.Text.Trim());
            string origamount = "0.00";
            Double result = 0;
            if (Double.TryParse(sOverrideBadDebt.Trim(), out result) == false)
            {
                Messages.ShowInformation("Bad Debt Amount format is not valid.");
                return;
            }
     
            if (Convert.ToDouble(sOverrideBadDebt.Trim()) > 0 ) 
            {
                //if txt is not numeric////////////////////////////////////////////////////////////
               
                




                txtOverrideBadDebt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(sOverrideBadDebt.Trim()));

                if (txtOverrideBadDebt.Text == "0.00")
                {
                    origamount = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_bad_debt_amount"].ToString();
                    {
                        if (txtOverrideBadDebt.Text != origamount.ToString())
                        {
                            chkOverride.IsChecked = 1;
                            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"] = "1";
                        }
                    }
                }
                else
                {
                    chkOverride.IsChecked = 1;
                    this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"] = "1";
                }

            }
              
        }

        private void txtOverrideDeferred_LostFocus(object sender, RoutedEventArgs e)
        {
            string sOverrideDeferred = UnformatTextField(txtOverrideDeferred.Text.Trim());
             string origamount = "0.00";
            //if txt is not numeric////////////////////////////////////////////////////////////
            Double result = 0;
            if (Double.TryParse(sOverrideDeferred.Trim(), out result) == false)
            {
                Messages.ShowInformation("Deferred Amount format is not valid.");
                return;

                
            }



            if (Convert.ToDouble(sOverrideDeferred.Trim()) > 0)
            {
               



                txtOverrideDeferred.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(sOverrideDeferred));

                if (txtOverrideDeferred.Text == "0.00")
                {
                    if (txtOverrideDeferred.Text == "0.00")

                        origamount = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_credit_memo_amount"].ToString();
                    {
                        if (txtOverrideDeferred.Text != origamount.ToString())
                        {
                        chkOverride.IsChecked = 1;
                        this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"] = "1";
                        }
                    }

                }
                else
                {
                    chkOverride.IsChecked = 1;
                    this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"] = "1";
                }

            }
        }

        private void chkOverride_Checked(object sender, RoutedEventArgs e)
        {
            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"] = "1";
        }

        private void chkOverride_UnChecked(object sender, RoutedEventArgs e)
        {
            this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["override_flag"] = "0";
            txtOverrideBadDebt.Text = "0.00";
            txtOverrideBadDebt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideBadDebt.Text));
            txtOverrideDeferred.Text = "0.00";
            txtOverrideDeferred.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtOverrideDeferred.Text));

        }

      

        

    }
}
