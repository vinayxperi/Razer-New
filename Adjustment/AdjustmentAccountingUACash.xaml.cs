using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentAccountingUACash' object.
    /// </summary>
    public partial class AdjustmentAccountingUACash : ScreenBase, IPreBindable 
    {

        public cBaseBusObject AdjAcctUACashBusObject = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        public bool IsLoading { get; set; }
        public bool Saved = false;

        /// <summary>
        /// Create a new instance of a 'AdjustmentAccountingUACash' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentAccountingUACash(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = AdjAcctUACashBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentAcctUACash";
            //set Adj Folder screen obj
            AdjFolderScreen = _AdjFolderScreen;
            // This call is required by the designer.
            InitializeComponent();
            cmbProductCode.PropertyChanged += new PropertyChangedEventHandler(cmbProductCode_PropertyChanged);
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
            this.CurrentBusObj.Parms.AddParm("@product_code", "");
            this.Load();
            //////////////////////////////////

        }

        private void cmbProductCode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LostFocus":

                    break;
                case "SelectedValue":
                    if (cmbProductCode.SelectedValue == null) break;
                    if (cmbProductCode.SelectedValue.ToString().Trim() != "")
                    {
                        popFields();
                        txtCustomerId.CntrlFocus();
                        btnNAFXAdjustment.IsEnabled = false;
                    }   
                    break;
                default:
                    break;
            }
        }

        private void txtCustomerId_LostFocus(object sender, RoutedEventArgs e)
        {
            //enable editable fields if cust num and name exists
            if (cmbProductCode.SelectedValue == null) return;
            if (cmbProductCode.SelectedValue.ToString().Trim() != "")
            {
                //if (txtCustomerId.Text != "" && txtCustomerName.Text != "") RSIMS fix issue 927
                if (txtCustomerId.Text != "")
                {
                    //popFields();
                    txtAdjAmt.IsEnabled = true;
                    //get cust name
                    if (txtCustomerId.Text != null)
                    {
                        txtCustomerName.Text = cGlobals.BillService.GetCustomerName(txtCustomerId.Text);
                        if (txtCustomerName.Text == "")
                        {
                            Messages.ShowInformation("Customer Not Found");
                        }
                    }

                }
            }
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
                txtGLOffsetAmt.Text = "0.00";
            }
            ///////////////////////////////////////////////////////////////////////////////////
            //set a default value if user skips
            if (txtAdjAmt.Text == "") txtAdjAmt.Text = "0.00";
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtAdjAmt.Text);
            txtAdjAmt.Text = formatAmt.ToString("0.00");
            txtGLOffsetAmt.Text = formatAmt.ToString("0.00");
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
                txtGLAmt.TextColor = "Red";
                txtRunningAdjAmt.TextColor = "Red";
                txtGLOffsetAmt.TextColor = "Black";
                txtRunningOffsetAmt.TextColor = "Black";
            }
            else
            {
                txtAdjAmt.TextColor = "Black";
                txtGLAmt.TextColor = "Black";
                txtRunningAdjAmt.TextColor = "Black";
                txtGLOffsetAmt.TextColor = "Red";
                txtRunningOffsetAmt.TextColor = "Red";
            }

            //enable editable fields if adj value > 0
            if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            {
                setEditScreenState();
                btnSave.IsEnabled = true;
            }
            txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtAdjAmt.Text));
            txtGLOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtGLOffsetAmt.Text));
            //txtOffsetGLCompany.CntrlFocus();
        }

        /// <summary>
        /// check offsetting amts to see if save button can be enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGLAmt_LostFocus(object sender, RoutedEventArgs e)
        {
            //if txt is not numeric////////////////////////////////////////////////////////////
            Double result = 0;
            if (Double.TryParse(txtGLAmt.Text, out result) == false)
            {
                txtGLAmt.Text = "0.00";
            }
            ///////////////////////////////////////////////////////////////////////////////////
            //set a default value if user blnaks
            if (txtGLAmt.Text == "") txtGLAmt.Text = "0.00";
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtGLAmt.Text);
            txtGLAmt.Text = formatAmt.ToString("0.00");
            ///////////////////////////////////////////////////////////////////////////
            //if less than zero turn red otherwise black/////////////////////////////////////////////////
            if (Convert.ToDouble(txtGLAmt.Text) < 0) txtGLAmt.TextColor = "Red";
            else txtGLAmt.TextColor = "Black";
            if (Convert.ToDouble(txtRunningAdjAmt.Text) < 0) txtRunningAdjAmt.TextColor = "Red";
            else txtRunningAdjAmt.TextColor = "Black";
            //////////////////////////////////////////////////////////////////////////////////////////////
            //if amounts offset then enable save button
            if (AmountsOffset()) btnSave.IsEnabled = true;
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
            //format decimal precision
            double OffsetTotal = Convert.ToDouble(txtGLOffsetAmt.Text);
            txtGLOffsetAmt.Text = OffsetTotal.ToString("0.00");
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
        /// populates fields
        /// </summary>
        private void popFields()
        {
            //change parm to new value
            this.CurrentBusObj.changeParm("@product_code", cmbProductCode.SelectedValue.ToString());
            //this.CurrentBusObj.Parms.ClearParms();
            //this.CurrentBusObj.Parms.AddParm("@product_code", cmbProductCode.SelectedValue.ToString().Trim());
            //this.CurrentBusObj.changeParm("@product_code", cmbProductCode.SelectedValue.ToString());
            ////add parm for error messages
            //this.CurrentBusObj.Parms.AddParm("@error_message", "");
            //load the object and set IsLoading so prebind() doesn't run over & over
            IsLoading = true;
            this.Load();
            IsLoading = false;
            ////////////////////////////////////////////////////////////////////////
            if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count > 0)
            {
                txtCustomerId.IsEnabled = true;
            }
            else
            {
                Messages.ShowInformation("Product Data Not Found");
                setInitScreenState();
            }
        }

        /// <summary>
        /// enables editable fields when adj amt value > 0
        /// </summary>
        private void setEditScreenState()
        {
            //Enable fields
            GLBorder.IsEnabled = true;
            GLOffsetBorder.IsEnabled = true;
            /////////////////////////////////////////////////////
        }

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            GLBorder.IsEnabled = false;
            GLOffsetBorder.IsEnabled = false;
            txtCustomerName.IsEnabled = false;
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
            this.SaveSuccessful = false;
            string strErr = "";
            if (GLDataComplete(ref strErr) == false)
            {
                Messages.ShowError(strErr);
                return;
            }
            if (cmbProductCode.SelectedValue == null)
            {
                 Messages.ShowWarning("Please select product code. Save Cancelled");
                return;
            }
            //make sure all offsetting account fields have been entered
            //if (txtOffsetGLCompany.Text == "" || txtOffsetGLCompany.Text == null || txtOffsetGLCostCtr.Text == "" || txtOffsetGLCostCtr.Text == null ||
            //    txtOffsetGLAcct.Text == "" || txtOffsetGLAcct.Text == null || txtOffsetGLProduct.Text == "" || txtOffsetGLProduct.Text == null ||
            //    txtOffsetGLRegion.Text == "" || txtOffsetGLRegion.Text == null)
            //{
            //    Messages.ShowWarning("Please enter offsetting accounting information. Save Cancelled");
            //    return;
            //}
            //make sure offsetting amts are not 0
            string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            string sRunningAdjAmt = UnformatTextField(txtRunningAdjAmt.Text.Trim());
            string sRunningOffsetAmt = UnformatTextField(txtRunningOffsetAmt.Text.Trim());
            string sGLOffsetAmt = UnformatTextField(txtGLOffsetAmt.Text.Trim());
            string sGLAmt = UnformatTextField(txtGLAmt.Text.Trim());
            //if (Convert.ToDouble(txtRunningAdjAmt.Text.Trim()) + Convert.ToDouble(txtAdjAmt.Text.Trim()) + (Convert.ToDouble(txtAdjAmt.Text.Trim()) + Convert.ToDouble(txtRunningOffsetAmt.Text.Trim())) == 0)
            if (Convert.ToDouble(sRunningAdjAmt.Trim()) + Convert.ToDouble(sAdjAmt.Trim()) + (Convert.ToDouble(sAdjAmt.Trim()) + Convert.ToDouble(sRunningOffsetAmt.Trim())) == 0)
            {
                Messages.ShowWarning("Adjustment Amount and Offsetting Amount Cannot Be $0.00. Save Cancelled");
                return;
            }
            if (AmountsOffset())
            {
                string ErrMsg = "";
                //string NewDocID = cGlobals.BillService.InsertNewAdjustmentUACash(txtCustomerId.Text, txtGLAmt.Text, cmbProductCode.SelectedValue.ToString(), cGlobals.UserName, cmbCurrency.SelectedValue.ToString(), txtGLCompany.Text, txtGLCostCtr.Text, txtGLAcct.Text, txtGLRegion.Text, txtGLProduct.Text, txtOffsetGLCompany.Text, txtOffsetGLCostCtr.Text, txtOffsetGLAcct.Text, txtOffsetGLProduct.Text, txtOffsetGLRegion.Text, "0000", txtGLOffsetAmt.Text, ref ErrMsg);
                string NewDocID = cGlobals.BillService.InsertNewAdjustmentUACash(txtCustomerId.Text, sGLAmt, cmbProductCode.SelectedValue.ToString(), cGlobals.UserName, cmbCurrency.SelectedValue.ToString(), txtGLCompany.Text, txtGLCostCtr.Text, txtGLAcct.Text, txtGLRegion.Text, txtGLProduct.Text, txtOffsetGLCompany.Text, txtOffsetGLCostCtr.Text, txtOffsetGLAcct.Text, txtOffsetGLProduct.Text, txtOffsetGLRegion.Text, txtOffsetGLIC.Text, sGLOffsetAmt, ref ErrMsg);
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
                    AdjFolderScreen.ReturnData(NewDocID, "");
                    //add user name to AdjGeneralTab
                    AdjFolderScreen.AdjustmentGeneralTab.txtCreatedBy.Text = cGlobals.UserName.ToLower();
                    //close me
                    System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

                    this.CurrentBusObj.ObjectData.AcceptChanges();
                    StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
                    this.SaveSuccessful = true;
                    Saved = true;

                    if (!ScreenBaseIsClosing)
                    {
                        AdjParent.Close();
                    }
                }
            }
            else
            {
                Messages.ShowError("Amounts Do Not Offset. Save Cancelled");
            }
        }

        /// <summary>
        /// gets error msgs from SP
        /// </summary>
        /// <returns></returns>
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
        /// gets new adj id
        /// </summary>
        /// <returns></returns>
        private string getNewAdjIdFromStoredProc()
        {
            var SPErrorMsg = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                             where x.Field<string>("parmName") == "@adj_document_id"
                             select new
                             {
                                 parmName = x.Field<string>("parmName"),
                                 parmValue = x.Field<string>("parmValue")
                             };
            foreach (var info in SPErrorMsg)
            {
                if (info.parmName == "@adj_document_id")
                    return info.parmValue;
            }
            return "";
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
            if (txtOffsetGLAcct.Text == null || txtOffsetGLAcct.Text == "")
            {
                strErr = "Offset GL Nat Acct Code Missing";
                return false;
            }
            if (txtOffsetGLRegion.Text == null || txtOffsetGLRegion.Text == "")
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
        /// get pre-bound junk
        /// </summary>
        public void PreBind()
        {
            //check this to keep recursion to this event from occurruing when load is called
            if (IsLoading == true) return;
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                this.cmbProductCode.SetBindingExpression("product_code", "product_description", this.CurrentBusObj.ObjectData.Tables["products"]);
                this.cmbCurrency.SetBindingExpression("currency_code", "description", this.CurrentBusObj.ObjectData.Tables["currency_lookup"]);
                this.cmbCurrency.SelectedValue = "USD";
            }
        }

        /// <summary>
        /// pull up customer lookup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                txtCustomerId.Text = cGlobals.ReturnParms[0].ToString();
                txtCustomerName.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        /// <summary>
        /// highlight textbox text for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAdjAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSelectAll(sender);
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
        /// highlight textbox text for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGLAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSelectAll(sender);
        }

        /// <summary>
        /// highlight textbox text for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCustomerId_GotFocus(object sender, RoutedEventArgs e)
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

        private void txtGLAmt_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public override void Close()
        {
         
            if (Saved != true)
            {
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                if (txtAdjAmt.Text != "" && txtAdjAmt != null && txtAdjAmt.Text != "0.00") ForceScreenDirty = true;
                if (txtCustomerId.Text != "" && txtCustomerId.Text != null) ForceScreenDirty = true;
                //if (txtGLCompany.Text != "" && txtGLCompany.Text != null && txtGLCompany.Text != " ") ForceScreenDirty = true;
                //if (txtGLAcct.Text != "" && txtGLAcct.Text != null && txtGLAcct.Text != "  ") ForceScreenDirty = true;
                //if (AdjustmentBillingDifferentialScreen.txtBillingDiffAmt.Text != "" && txtBillingDiffAmt != null) ForceScreenDirty = true;
                //if (txtDocument.Text != "" && txtDocument.Text != null) ForceScreenDirty = true;
            }
            else ForceScreenDirty = false;
            base.Close();
        }


        //RES 2/14/22 Add button to open Ads FX adjustment window
        private void btnNAFXAdjustment_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["deferred_flag"]) > 0)
            //{
            //    Messages.ShowInformation("Cannot debit and apply cash to a deferred document!");
            //    return;
            //}
            //string documentID = txtDocument.Text;
            //ApplyCashFlag = true;
            //ApplyCashSave();
            //if (!ApplyCashFlag) return;
            //instance location service screen
            AdjustmentBillingDifferentialAds AdjustmentBillingDifferentialScreen = new AdjustmentBillingDifferentialAds(this.CurrentBusObj, AdjFolderScreen);
            //create a new window, show it as a dialog
            System.Windows.Window AdjustmentBillingDifferentialAdsWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            AdjustmentBillingDifferentialAdsWindow.Title = "Ads FX Adjustment";
            AdjustmentBillingDifferentialAdsWindow.MaxHeight = 500;
            AdjustmentBillingDifferentialAdsWindow.MaxWidth = 850;
            //DebitApplyCashWindow.WindowState = WindowState.Maximized;
            //set screen as content of new window
            AdjustmentBillingDifferentialAdsWindow.Content = AdjustmentBillingDifferentialScreen;
            //open new window with embedded user control
            AdjustmentBillingDifferentialAdsWindow.ShowDialog();

            //close me
            Saved = true;
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

}
