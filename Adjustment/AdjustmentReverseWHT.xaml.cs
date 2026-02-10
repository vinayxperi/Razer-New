using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter;
using RazerBase.Lookups;
using System.Windows.Input;
using System.ComponentModel;
using Infragistics.Windows.DataPresenter.Events;
using System.Windows;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentWHT' object.
    /// </summary>
    public partial class AdjustmentReverseWHT : ScreenBase, IPreBindable
    {
        public cBaseBusObject WHTAdjustmentBusObj = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        public Int32 AdjType;
        public ComboBoxItemsProvider cmbProductItemCombo { get; set; }
        private string DocumentId { get; set; }
        ////This datatable is being added so that the Amount to adjust text box can have a binding
        ////Do this for fields that contain informational data but that will not be saved
        ////so that you can use converters or other benefits of binding
        //DataTable dtMiscInfo = new DataTable("MiscInfo");
        decimal AdjTotal = 0.00M;
        private bool IsSingleClickOrigin { get; set; }

        public AdjustmentReverseWHT(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
            //    //set obj
            this.CurrentBusObj = WHTAdjustmentBusObj;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentReverseWHT";
            //    //set Adj Folder screen obj
            AdjFolderScreen = _AdjFolderScreen;
            //    // This call is required by the designer.
            InitializeComponent();
            //    // Perform initializations for this object
            Init();
        }


        public void Init()
        {
            //    // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;
            this.MainTableName = "main";

            txtDocument.CntrlFocus();



        }

        private void txtDocument_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtDocument.Text != "")
            {
                //do this to keep grid from loading and binding re-running clearing user entered values
                if (DocumentId != txtDocument.Text)
                {

                    //populate grid
                    popScreen();
                    //txtRunningAdjAmt.CntrlFocus();
                }
            }
        }

        private void popScreen()
        {
            //load parms 
            loadParms(DocumentId);
            //load the object
            ReturnData(txtDocument.Text, DocumentId);


            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {

                setInitScreenState();


            }
            else
            {
                Messages.ShowInformation("Document Not Found");

                setInitScreenState();
                txtDocument.CntrlFocus();
            }

        }


        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Save();
        }

        public override void Save()
        {
            this.SaveSuccessful = false;
           
            decimal adjustamt = 0;
           
          
            if (this.CurrentBusObj.ObjectData != null)
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
                {
                    string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
                    adjustamt = Convert.ToDecimal(sAdjAmt);
                    string NewDocID = "";
                    string SPErrMsg = "";
                    sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
                    //NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 9, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt) * -1);
                    NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 9, cGlobals.UserName.ToLower(), Convert.ToDecimal(sAdjAmt));

                    //insert the new doc id in all existing grid rows for ins sp
                    if (NewDocID != "")
                    {
                        //set adjustment number on remit table to NewDocID
                        this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["adj_document_id"] = NewDocID;
                    }

                    else
                    {
                        Messages.ShowError("Problem Creating New Adjustment");
                        return;
                    }

                    this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["amount_to_adjust"] = sAdjAmt;
                    

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

                    }
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




        private void rollbackAdj(string NewDocID)
        {
            //delete adj header
            bool retVal = cGlobals.BillService.DeleteNewAdjusmentPreamble(NewDocID);
        }




        private void loadParms(string DocumentID)
        {
            //    //clear parm     

            try
            {

                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.AddParm("@document_id", txtDocument.Text);
                this.CurrentBusObj.Parms.AddParm("@country_id", 0);
                this.CurrentBusObj.Parms.AddParm("@province_id", 0);
                this.CurrentBusObj.Parms.AddParm("@amount", 0);
                this.CurrentBusObj.Parms.AddParm("@receivable_account", "");
                this.CurrentBusObj.Parms.AddParm("@searchind", 0);
                this.CurrentBusObj.Parms.AddParm("@cash_document_id", "");
                this.CurrentBusObj.Parms.AddParm("@seq_code", 0);
                DocumentId = txtDocument.Text;

            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }


        private void setInitScreenState()
        {

            btnSave.IsEnabled = false;

            //txtIncomeWHT.Text = "";
            //txtWHTAmount.Text = "";
            //txtWHTRate.Text = "0.00";


        }

        ///// <summary>
        ///// frees up objects for edit
        ///// </summary>



        public void PreBind()
        {

            if (this.CurrentBusObj.HasObjectData)
            {
                this.cmbProvince.SetBindingExpression("province_id", "province", this.CurrentBusObj.ObjectData.Tables["province"]);
                this.cmbCountry.SetBindingExpression("country_id", "country", this.CurrentBusObj.ObjectData.Tables["country"]);

            }


        }

        private void txtDocument_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            string documentID = "";
            if (txtDocument.Text != "")
            {
                DocumentId = txtDocument.Text;
                //Need to clear both grids and all screens
                if (this.CurrentBusObj.HasObjectData)
                {
                    txtAdjAmt.Text = "";
                    txtIncomeWHT.Text = "";
                    txtWHTAmount.Text = "";
                    txtWHTRate.Text = "0.00";

                }

            }

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            AdjustmentWHTLookup f = new AdjustmentWHTLookup();
            f.Init(new cBaseBusObject("AdjustmentWHTLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtDocument.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                if (txtDocument.Text != DocumentId)
                {
                    ReturnData(txtDocument.Text, "@document_id");
                    btnSave.IsEnabled = true;
                }
            }
        }


        public void ReturnData(string SearchValue, string DbParm)
        {

            Decimal whtAmount = 0;
            decimal whtReverse =0;
            decimal whtIncomeSubject = 0;
            //if no value do nothing
            if (SearchValue == "") return;
            //Add new parameters
            loadParms(SearchValue);
            //load data
            //if coming from save do not do this...
            this.Load();
            //if BCFNumber found then set header and pop otherwise send message
            //if customer number found then set header and pop otherwise send message
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)
            {

                //txtWHTAmount.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["wht_amount"].ToString();
                //txtIncomeWHT.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["income_subject_to_wht"].ToString();
                whtAmount = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["wht_amount"]);
                whtIncomeSubject = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["income_subject_to_wht"]);
                //whtReverse = whtAmount * -1;
                whtReverse = whtAmount;
                //txtAdjAmt.Text = whtReverse.ToString();

                txtWHTAmount.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", whtAmount);
                txtIncomeWHT.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", whtIncomeSubject);
                txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", whtReverse);
              
                DocumentId = txtDocument.Text;
                txtCustomer.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["receivable_account"].ToString().Trim();

                cmbProvince.SelectedValue = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["province_id"];
                cmbCountry.SelectedValue = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["country_id"];

                if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }

                //txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
                //txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
                //txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
                //txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");

                if (string.IsNullOrEmpty(txtWHTAmount.Text)) { return; }

                //txtWHTAmount.Text = txtWHTAmount.Text.Replace("$", "");
                //txtWHTAmount.Text = txtWHTAmount.Text.Replace(",", "");
                //txtWHTAmount.Text = txtWHTAmount.Text.Replace("(", "-");
                //txtWHTAmount.Text = txtWHTAmount.Text.Replace(")", "");

                if (string.IsNullOrEmpty(txtIncomeWHT.Text)) { return; }

                //txtWHTAmount.Text = txtIncomeWHT.Text.Replace("$", "");
                //txtWHTAmount.Text = txtIncomeWHT.Text.Replace(",", "");
                //txtWHTAmount.Text = txtIncomeWHT.Text.Replace("(", "-");
                //txtWHTAmount.Text = txtIncomeWHT.Text.Replace(")", "");





            }

        }
        private void txtAdjAmt_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is RazerBase.ucLabelTextBox)
                (sender as RazerBase.ucLabelTextBox).SelectAll();

            if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }

            //txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
            //txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
            //txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
            //txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");
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

