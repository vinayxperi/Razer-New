

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Windows.Input;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentMiscCreditDebit' object.
    /// </summary>
    public partial class AdjustmentMiscCreditDebit : ScreenBase
    {
        public cBaseBusObject MiscCrDbBusObject = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        //public bool IsLoading { get; set; }
        public bool IsDocumentLoaded { get; set; }
        private double AdjMaxAmt { get; set; }
        private bool MaxNumAdjsExceeded { get; set; }
        public bool Saved = false;
        private string newAdjID { get; set; }

        /// <summary>
        /// Create a new instance of a 'AdjustmentMiscCreditDebit' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentMiscCreditDebit(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //set obj
            this.CurrentBusObj = MiscCrDbBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentMiscCreditDebit";
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
            this.CurrentBusObj.Parms.AddParm("@apply_to_doc", "");
            this.CurrentBusObj.Parms.AddParm("@adj_crdb", "");
            this.CurrentBusObj.Parms.AddParm("@max_crdb", "");
            this.Load();
            txtDocument.CntrlFocus();
            //////////////////////////////////

        }

        /// <summary>
        /// populates grid and enables adj amt field if grid rows > 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDocument_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //cbirney added the newAdjID so it only loads again if the adjustment id has changed
            if (txtDocument.Text != newAdjID)
            {
                newAdjID = txtDocument.Text;
                if (txtDocument.Text != "" && txtDocument.Text != null)
                {
                    //populate fields
                    popFields();
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
            }
            ///////////////////////////////////////////////////////////////////////////////////
            if (AdjMaxAmt != null)
            {
                if (Convert.ToDouble(txtAdjAmt.Text) > AdjMaxAmt || Convert.ToDouble(txtAdjAmt.Text) < AdjMaxAmt * -1)
                {
                    Messages.ShowWarning("Adjustment Amount does not fall in the range of -" + AdjMaxAmt + " to " + AdjMaxAmt);
                    //e.Handled = true;
                    
                    return;
                }
            }
            //set a default value if user skips
            if (txtAdjAmt.Text == "") txtAdjAmt.Text = "0.00";
            //convert the value to a double and format to add trailing zeros if missing
            double formatAmt = Convert.ToDouble(txtAdjAmt.Text);
            txtAdjAmt.Text = formatAmt.ToString("0.00");
            ///////////////////////////////////////////////////////////////////////////
            //if less than zero turn red
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

            //if (Convert.ToDouble(txtAdjAmt.Text) < 0) txtAdjAmt.TextColor = "Red";
            ////otherwise black
            //else txtAdjAmt.TextColor = "Black";

            ////enable editable fields if adj value > 0
            //if (Convert.ToDouble(txtAdjAmt.Text) != 0)
            //{
            //    setEditScreenState();
            //    //set inverse of AdjAmt in offset amt txtbox
            //    this.txtGLOffsetAmt.Text = (Convert.ToDouble(txtAdjAmt.Text) * -1).ToString();
            //    //set text to red if < 0
            //    if (Convert.ToDouble(txtGLOffsetAmt.Text) < 0) txtGLOffsetAmt.TextColor = "Red";
            //    this.txtGLOffsetAmt.CntrlFocus();
            //}
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
            if (AdjMaxAmt != null)
            {
                if (Convert.ToDouble(txtGLAmt.Text) > AdjMaxAmt)
                {
                    Messages.ShowWarning("Maximum Adjustment Amount of $" + AdjMaxAmt + " Will Be Exceeded, Reduce Adjustment Amount");
                    //e.Handled = true;
                    return;
                }
            }
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
            if (AmountsOffset())
            {
                txtGLOffsetAmt.CntrlFocus();
                btnSave.IsEnabled = true;
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
            if (AdjMaxAmt != null)
            {
                if (Convert.ToDouble(txtGLOffsetAmt.Text) > AdjMaxAmt)
                {
                    Messages.ShowWarning("Maximum Adjustment Amount of $" + AdjMaxAmt + " Will Be Exceeded, Reduce Adjustment Amount");
                    //e.Handled = true;
                    return;
                }
            }
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
            this.CurrentBusObj.changeParm("@document_id", txtDocument.Text.Trim());
            this.CurrentBusObj.changeParm("@apply_to_doc", txtDocument.Text.Trim());
            ////load the object and set IsLoading so prebind() doesn't run over & over
            //IsLoading = true;
            this.Load();
            //IsLoading = false;
            //////////////////////////////////////////////////////////////////////////
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                //assess adj maximums
                AssessAdjMaximums();
                if (MaxNumAdjsExceeded == true)
                {
                    Messages.ShowWarning("This Document Cannot Be Adjusted. The Maximum Number of Adjustments for this Adjustment Type will be Exceeded");
                    return;
                }
                IsDocumentLoaded = true;
                this.txtAdjAmt.IsReadOnly = false;
                //set focus to amt to adj

            }
            else
            {
                Messages.ShowInformation("Document Id Not Found");
                IsDocumentLoaded = false;
                setInitScreenState();
            }
        }

        /// <summary>
        /// checks to see if maximums will be exceeded max adj amt and max num of adjs on document
        /// </summary>
        private void AssessAdjMaximums()
        {
            string MaxNumAdjs = "";
            string strAdjMaxAmt = "";
            bool RetVal = cGlobals.BillService.GetMiscCreditDebitMaximums(ref MaxNumAdjs, ref strAdjMaxAmt);
            //if max amt not empty then set val to property
            if (strAdjMaxAmt != "" && strAdjMaxAmt != null)
            {
                AdjMaxAmt = Convert.ToDouble(strAdjMaxAmt);
            }
            //if is empty set property to 0
            else
            {
                AdjMaxAmt = 0.00;
            }
            //do max num adj stff
            if (MaxNumAdjs != "" && MaxNumAdjs != null)
            {
                //check to see if max num adjs will be exceeded
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count < Convert.ToInt32(MaxNumAdjs))
                {
                    MaxNumAdjsExceeded = false;
                }
                else
                {
                    //max num of adjs will be exceeded
                    MaxNumAdjsExceeded = true;
                }
            }
        }

        /// <summary>
        /// enables editable fields when adj amt value > 0
        /// </summary>
        private void setEditScreenState()
        {
            //Enable fields
            txtGLAmt.IsReadOnly = true;
            txtGLOffsetAmt.IsReadOnly = true;
            /////////////////////////////////////////////////////
        }

        /// <summary>
        /// occurs when a new document is selected
        /// </summary>
        private void setInitScreenState()
        {
            //Disable fields
            txtGLAmt.IsReadOnly = true;
            txtGLOffsetAmt.IsReadOnly = true;
            //////////////////////////////////
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
            //Verify amount is valid
            //string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
            //if (Convert.ToDouble(sAdjAmt) > AdjMaxAmt || Convert.ToDouble(sAdjAmt) * -1 < AdjMaxAmt * -1)
            //{
            //    Messages.ShowWarning("Adjustment Amount does not fall in the range of " + Convert.ToChar((Convert.ToDouble(sAdjAmt) * -1)) + " to " + AdjMaxAmt);
            //    //e.Handled = true;
            //    return;
            //}
            //if (txtOffsetGLAcct.Text == null || txtOffsetGLCompany.Text == null || txtOffsetGLCostCtr.Text == null || txtOffsetGLProduct.Text == null || txtOffsetGLRegion.Text == null)
            //{
            //    Messages.ShowWarning("Data Error, a G/L Offset Entry Value is Missing.  Save Failed");
            //    return;
            //}
            //make sure all offsetting account fields have been entered
            if (txtOffsetGLCompany.Text == "" || txtOffsetGLCompany.Text == null || txtOffsetGLCostCtr.Text == "" || txtOffsetGLCostCtr.Text == null ||
                txtOffsetGLAcct.Text == "" || txtOffsetGLAcct.Text == null || txtOffsetGLProduct.Text == "" || txtOffsetGLProduct.Text == null ||
                txtOffsetGLRegion.Text == "" || txtOffsetGLRegion.Text == null)
            {
                Messages.ShowWarning("Please enter offsetting accounting information. Save Cancelled");
                return;
            }
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
                //Messages.ShowInformation("Amounts Offset, Run Save Logic");
                string ErrMsg = "";
                //string NewDocID = cGlobals.BillService.InsertNewAdjustmentMiscCreditDebit(txtCustomerId.Text, txtGLAmt.Text, txtProduct.Text, cGlobals.UserName, txtCurrency.Text, txtGLCompany.Text, txtGLCostCtr.Text, txtGLAcct.Text, txtGLRegion.Text, txtGLProduct.Text, txtOffsetGLCompany.Text, txtOffsetGLCostCtr.Text, txtOffsetGLAcct.Text, txtOffsetGLProduct.Text, txtOffsetGLRegion.Text, "0000", txtGLOffsetAmt.Text, txtDocument.Text, txtSeq.Text, ref ErrMsg);
                string NewDocID = cGlobals.BillService.InsertNewAdjustmentMiscCreditDebit(txtCustomerId.Text, sGLAmt, txtProduct.Text, cGlobals.UserName, txtCurrency.Text, txtGLCompany.Text, txtGLCostCtr.Text, txtGLAcct.Text, txtGLRegion.Text, txtGLProduct.Text, txtOffsetGLCompany.Text, txtOffsetGLCostCtr.Text, txtOffsetGLAcct.Text, txtOffsetGLProduct.Text, txtOffsetGLRegion.Text, "0000", sGLOffsetAmt, txtDocument.Text, txtSeq.Text, ref ErrMsg);
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
                    //changed cbirney - to get to work correctly
                 
                    this.CurrentBusObj.Parms.UpdateParmValue ("@document_id", NewDocID.ToString());
            
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
        private void txtGLAmt_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSelectAll(sender);
        }

        /// <summary>
        /// highlight textbox text for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGLOffsetAmt_GotFocus(object sender, RoutedEventArgs e)
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

        private string UnformatTextField(string FormattedTextField)
        {
            if (FormattedTextField == null || FormattedTextField == "") return "0";

            string sUnformattedTextField = FormattedTextField.Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");

            return sUnformattedTextField;
        }

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
