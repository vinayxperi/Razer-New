using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RazerBase;
using RazerBase.Interfaces;
using System.ComponentModel;
using System.Data;
using Infragistics.Windows.Controls;

namespace Razer.CustomInvoice
{
    /// <summary>
    /// Interaction logic for CustomInvoiceFolder.xaml
    /// </summary>
    public partial class CustomInvoiceFolder : ScreenBase, IScreen
    {
        private static readonly string frequencyParameterValue = "CustomFreq";
        private static readonly string frequencyParameterName = "@code_name";
        private static readonly string invoiceField = "invoice_number";
        private static readonly string documentIdParameter = "@document_id";
        private static readonly string attachmentParameter = "@external_char_id";
        private static readonly string attachmentLocID = "@location_id";
        private static readonly string attachmentType = "@attachment_type";
        private static readonly string attachmentExtIntParameter = "@external_int_id";
        private static readonly string commentAttachmentsIdParameter = "@comment_attachments_id";
        private static readonly string commentTypeParameter = "@comment_type";
        private static readonly string recurringInvoiceParameter = "@recurring_original_invoice";
        private static readonly string idField = "sequence_code";
        private static readonly string receivableAccountField = "receivable_account";
        private static readonly string mainTableName = "general";
        private static readonly string detailTableName = "detail";
        private static readonly string acctDetailTableName = "acct_detail";
        private static readonly string viewTableName = "view";
        private static readonly string adjustmentsTableName = "adjustments";
        private static readonly string companyValue = "company_code";
        private static readonly string captionName = "Invoice - ";
        private static readonly string receivableAcctObject = "ManInvRecAcct";
        private static readonly string receivableAcctTableName = "recv_acct";
        private static readonly string receivableAcctParameter = "@receivable_account";
        private static readonly string accountNameField = "account_name";
        private static readonly string address1Field = "address_1";
        private static readonly string address2Field = "address_2";
        private static readonly string cityField = "city";
        private static readonly string stateField = "state";
        private static readonly string postalCodeField = "postal_code";
        private static readonly string countryField = "country_id";
        private static readonly string invoiceTypeField = "invoice_type";
        private static readonly string postedFlagField = "posted_flag";
        private static readonly string printedFlagField = "printed_flag";   
        private static readonly string approvalObjectName = "ManInvApproval";
        private static readonly string invoiceTypeTableName = "invoice_type";
        private static readonly string invoiceTypeValueField = "invoice_type_id";
        private static readonly string dateTypeTableName = "date_types";
        private static readonly string dateTypeField = "date_type";
        private static readonly string dateValueField = "date_value";
        private static readonly string accountingPeriodField = "ACCTPERIOD";
        private static readonly string billingPeriodField = "BILLPERIOD";

        //RES 11/25/19 verify attachments exist
        //private static readonly string ExternalCharId = "@external_char_id";
        //private static readonly string AttachmentType = "@attachment_type";
                

        private string customer;
        private string invoiceNumber;
        private string windowCaption;
        public int invoiceTypeId = 0;
        //private string RealinvoiceNumber;

        private Boolean notFirstNew = false;
        private Boolean differentInvoice = true;
        private Boolean showSaveMessage = true;
        public Boolean okToSave = true;
        public Boolean showNewInvoiceSaveMessage = false;
        public Boolean bypassSave = false;

        public string selectedCompany = string.Empty;

        public int ContractID = 0;

        private cBaseBusObject customerObject;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }
        

        public CustomInvoiceFolder()
        {
            InitializeComponent();                           
            //this.General.cmbCompany.PropertyChanged += new PropertyChangedEventHandler(cmbCompany_PropertyChanged);
            this.General.txtContractName.MouseDoubleClick += new MouseButtonEventHandler(txtContractName_MouseDoubleClick);
            this.General.cmbCompany.SelectionChanged  += new RoutedEventHandler(cmbCompany_SelectionChanged);
            this.General.cmbInvoiceType.SelectionChanged += new RoutedEventHandler(cmbInvoiceType_SelectionChanged);
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;
            this.MainTableName = mainTableName;
            this.DoNotSetDataContext = true;

            //RES 4/19/16 Add CLM Number
            //this.CurrentBusObj.Parms.AddParm("@contract_number", "0");

            TabCollection.Add(General);
            TabCollection.Add(Details);
            TabCollection.Add(View);
            TabCollection.Add(Attachments);
            TabCollection.Add(Comments);
            TabCollection.Add(Adjustments);
            TabCollection.Add(Deferred);

            Attachments.TabIsEnabled = false;        
            txtCustomerAccount.IsEnabled = false;

            General.ldpAccountingPeriod.IsEnabled = false;
            General.ldpBillingPeriod.IsEnabled = false;         
            
            
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                invoiceNumber = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();
                General.invoiceNumber = invoiceNumber;
                differentInvoice = true;
                LoadData(invoiceNumber);
                GeneralMain.Focus();
                if ((invoiceNumber.StartsWith("MIN")) || (invoiceNumber.StartsWith("min")))                {
                    
                    General.chkConvert.Visibility = Visibility.Collapsed;
                    General.chkClose.Visibility = Visibility.Collapsed;

                }
                else
                {
                    General.chkConvert.Visibility = Visibility.Visible;
                    General.chkClose.Visibility = Visibility.Visible;
                    General.chkConvert.IsEnabled = true;
                    General.chkClose.IsEnabled = true;

                    if (CurrentBusObj.ObjectData.Tables["requestconvert"] != null && CurrentBusObj.ObjectData.Tables["requestconvert"].Rows.Count > 0)
                    {
                        General.FirstTime = true;
                        General.chkConvert.IsChecked = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["requestconvert"].Rows[0]["count"]);
                    }
                    General.FirstTime = false;
                    if (CurrentBusObj.ObjectData.Tables["requestclose"] != null && CurrentBusObj.ObjectData.Tables["requestclose"].Rows.Count > 0)
                    {
                        General.FirstTime2 = true;
                        General.chkClose.IsChecked = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["requestclose"].Rows[0]["count"]);
                    }
                    General.FirstTime2 = false;
                }
            }
            
        }

       private void txtCustomInvoiceNumber_LostFocus(object sender, RoutedEventArgs e)
        {
           if (!string.IsNullOrEmpty(txtCustomInvoiceNumber.Text))
           {
                //if (!string.IsNullOrEmpty(txtCustomInvoiceNumber.Text) && txtCustomInvoiceNumber.Text != invoiceNumber)
                if (txtCustomInvoiceNumber.Text != invoiceNumber)
                {
                    differentInvoice = true;
                    txtCustomerAccount.Text = "";
                    LoadData(txtCustomInvoiceNumber.Text);                
                }
                if ((invoiceNumber.StartsWith("MIN")) || (invoiceNumber.StartsWith("min")))
                {
                    General.chkConvert.IsEnabled = false;
                    General.chkClose.IsEnabled = false;
                }
           }
            //RES 11/25/19 change color to red on missing attachments
            //if (this.CurrentBusObj.ObjectData != null)
            //{
            //    if (this.CurrentBusObj.ObjectData.Tables["attachments"].Rows.Count != 0)
            //        foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["attachments"].Rows)
            //        {
            //            string PathFile = dr["path"].ToString() + dr["prod_filename"].ToString();
            //            if (!System.IO.File.Exists(PathFile))
            //                dr["color_status"] = 3;
            //        }
            //    //    foreach (DataRecord r in Attachments. GridAttachments.xGrid.Records)
            //    //{
            //    //    string PathFile = r.Cells["path"].ToString() + r.Cells["prod_filename"].ToString();
            //    //    if (!System.IO.File.Exists(PathFile))
            //    //        r.Cells["color_status"] = 3;
            //    //}
            //}
        }

        private void txtCustomerAccount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCustomerAccount.Text) && txtCustomerAccount.Text != customer)
            {
                LoadCustomerInformation();
                
            }
        }


        private void txtCustomInvoiceNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            //Check to see if any changes were made, if so ask if they want to save them

            if (this.CurrentBusObj != null && (IsScreenDirty || ForceScreenDirty)
                && (this.SecurityContext == AccessLevel.ViewUpdate || this.SecurityContext == AccessLevel.ViewUpdateDelete))
            {
                var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                {
                    this.Save();
                }
            }



            invoiceNumber = txtCustomInvoiceNumber.Text;
            General.invoiceNumber = invoiceNumber;
            
             
        }


        private void txtCustomInvoiceNumber_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            //Check to see if any changes were made, if so ask if they want to save them

            if (this.CurrentBusObj != null && (IsScreenDirty || ForceScreenDirty)
                && (this.SecurityContext == AccessLevel.ViewUpdate || this.SecurityContext == AccessLevel.ViewUpdateDelete))
            {
                var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                {
                    this.Save();
                }
            }



            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = EventAggregator.GeneratedClickEvent;
            args.Source = txtCustomInvoiceNumber;
            EventAggregator.GeneratedClickHandler(this, args);
            //CustomInvoiceLookup f = new CustomInvoiceLookup();
            //f.Init(new cBaseBusObject("CustomInvoiceLookup"));
            ////this.CurrentBusObj.Parms.ClearParms();
            //cGlobals.ReturnParms.Clear();
            //f.ShowDialog();           
            differentInvoice = true;

            if (cGlobals.ReturnParms.Count > 0)
            {
                
                string id = cGlobals.ReturnParms[0].ToString(); 
                LoadData(id);
                cGlobals.ReturnParms.Clear();

            }
        }

        private void txtCustomerAccount_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.CurrentBusObj.HasObjectData)
            {
                cGlobals.ReturnParms.Clear();
                RoutedEventArgs args = new RoutedEventArgs();                
                args.RoutedEvent = EventAggregator.GeneratedClickEvent;
                args.Source = txtCustomerAccount;
                EventAggregator.GeneratedClickHandler(this, args);

                if (cGlobals.ReturnParms.Count > 0)
                {
                    txtCustomerAccount.Text = cGlobals.ReturnParms[0].ToString();
                    cGlobals.ReturnParms.Clear();
                    LoadCustomerInformation();
                }
            }
            else
            {
                Messages.ShowInformation("Either load an invoice or insert a new one to choose a customer.");
            }

             
        }

        private void txtContractName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.CurrentBusObj.HasObjectData)
            {
                RoutedEventArgs args = new RoutedEventArgs();
                args.RoutedEvent = EventAggregator.GeneratedClickEvent;
                args.Source = General.txtContractName;
                EventAggregator.GeneratedClickHandler(this, args);

                if (cGlobals.ReturnParms.Count > 0)
                {
                    General.txtContractId.Text = cGlobals.ReturnParms[0].ToString();
                    General.txtContractName.Text = cGlobals.ReturnParms[1].ToString();
                    cGlobals.ReturnParms.Clear();
                }
            }
            else
            {
                Messages.ShowInformation("Either load an invoice or insert a new one to choose a contract.");
            }
        }

        private void LoadCustomerInformation()
        {
            customerObject = new cBaseBusObject(receivableAcctObject);
            customerObject.Parms.AddParm(receivableAcctParameter, txtCustomerAccount.Text);

            customerObject.LoadData();

            if (customerObject.ObjectData.Tables[receivableAcctTableName].Rows.Count > 0)
            //if (customerObject.HasObjectData)
            {
                DataRow row = customerObject.ObjectData.Tables[receivableAcctTableName].Rows[0];
                General.txtAccountName.Text = row[accountNameField].ToString();
                General.txtAddress1.Text = row[address1Field].ToString();
                General.txtAddress2.Text = row[address2Field].ToString();
                General.txtCity.Text = row[cityField].ToString();
                General.txtState.Text = row[stateField].ToString();
                General.txtPostalCode.Text = row[postalCodeField].ToString();
                General.txtCountry.Text = row[countryField].ToString();
                this.General.IsEnabled = true;
                txtCustomerAccount.IsEnabled = false;

            }

            else
            {
                Messages.ShowInformation("Customer Account entered is invalid.");
            }

        }


        private void LoadData(string invoiceId)
        {

                       
            //GeneralMain.Focus(); 
            this.CurrentBusObj.Parms.ClearParms();
            txtCustomerAccount.Text = string.Empty;
            txtCustomerAccount.IsEnabled = false;
            string invoiceParameter = string.Format("@{0}", invoiceField);
            this.CurrentBusObj.Parms.AddParm(invoiceParameter, invoiceId);
            this.CurrentBusObj.Parms.AddParm(documentIdParameter, invoiceId);
            this.CurrentBusObj.Parms.AddParm(frequencyParameterName, frequencyParameterValue);
            this.CurrentBusObj.Parms.AddParm(recurringInvoiceParameter, string.Empty);
            addAttachmentParms(invoiceId);
          
            StringBuilder buildCaption = new StringBuilder(captionName);
            buildCaption.Append(": ");
            buildCaption.Append(invoiceId);
            windowCaption = buildCaption.ToString();

            //RES 4/19/16 add CLM Number
            this.CurrentBusObj.Parms.AddParm("@contract_number", "0");


            if (this.CurrentBusObj != null)
            {
                //KSH - 8/24/12 clear comments/attachments grid if applicable bus obj
                clrCommentsAttachmentsObj();
                this.Load();
                if (CurrentBusObj.ObjectData.Tables[mainTableName].Rows.Count > 0)
                {
                    //RES 9/13/19 added below line so that attachments will not be attached to previously proforma documents proforma number
                    //RealinvoiceNumber = CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][invoiceField].ToString();
                    //this.CurrentBusObj.Parms.UpdateParmValue(attachmentParameter, RealinvoiceNumber);
                    //foreach (DataRow r in CurrentBusObj.ObjectData.Tables["ParmTable"].Rows)
                    //{
                    //    if (r["parmName"].ToString() == "@external_char_id")                        
                    //        r["parmValue"] = RealinvoiceNumber;

                    //}

                    //if (this.CurrentBusObj.ObjectData != null)
                    //{
                    //    if (this.CurrentBusObj.ObjectData.Tables["attachments"].Rows.Count != 0)
                    //        foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["attachments"].Rows)
                    //        {
                    //            string PathFile = dr["path"].ToString() + dr["prod_filename"].ToString();
                    //            if (!System.IO.File.Exists(PathFile))
                    //                dr["color_status"] = 3;
                    //        }
                    //}
 
  
                    txtCustomerAccount.Text = CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][receivableAccountField].ToString();
                    customer = txtCustomerAccount.Text;

                    if (invoiceNumber != null)
                    {
                        invoiceId = CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][invoiceField].ToString();
                    }

                    invoiceNumber = invoiceId;
                    General.invoiceNumber = invoiceNumber;
                    txtCustomInvoiceNumber.Text = invoiceNumber;
                    invoiceTypeId = (int)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][invoiceTypeField];
                    if (invoiceTypeId != 1)
                    {
                        General.chkConvert.Visibility = Visibility.Collapsed;
                        General.chkClose.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        General.chkConvert.Visibility = Visibility.Visible;
                        General.chkClose.Visibility = Visibility.Visible;
                        General.chkConvert.IsEnabled = true;
                        General.chkClose.IsEnabled = true;
                        if (CurrentBusObj.ObjectData.Tables["requestconvert"] != null && CurrentBusObj.ObjectData.Tables["requestconvert"].Rows.Count > 0)
                        {
                            General.FirstTime = true;
                            General.chkConvert.IsChecked = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["requestconvert"].Rows[0]["count"]);
                        }
                        General.FirstTime = false;
                        if (CurrentBusObj.ObjectData.Tables["requestclose"] != null && CurrentBusObj.ObjectData.Tables["requestclose"].Rows.Count > 0)
                        {
                            General.FirstTime2 = true;
                            General.chkClose.IsChecked = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["requestclose"].Rows[0]["count"]);
                        }
                        General.FirstTime2 = false;
                    }
                    SetInvoiceTypes(invoiceTypeId);
                    Details.invoiceNumber = invoiceId;
                    Approval.invoiceNumber = invoiceId;
                    Details.DetailRecords = CurrentBusObj.ObjectData.Tables[detailTableName].Copy();
                    Details.AccountDetailRecords = CurrentBusObj.ObjectData.Tables[acctDetailTableName].Copy();
                    Details.UpdateRevenueAllocationRecords(1,1);
                    selectedCompany = CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][companyValue].ToString();
                    Details.LoadCompanyCodeComboFirst(selectedCompany);
                    

                    
                    //Details.idgRevenueAllocation.xGrid.DataSource = String.Empty;
                    //Details.txtDiscountAmount.Text = "0.00";
                    //Details.txtNetAmount.Text = "0.00";
                    //Details.txtTotalAmount.Text = "0.00";

                    
                    int postedFlag = (int)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][postedFlagField];

                    Approval.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                    Approval.ApprovalBusinessObject.Parms.ClearParms();
                    Approval.ApprovalBusinessObject.Parms.AddParm(documentIdParameter, invoiceNumber);
                    //RES 11/25/19 verify attachments exist
                    //Approval.ApprovalBusinessObject.Parms.AddParm(ExternalCharId, invoiceNumber);
                    //Approval.ApprovalBusinessObject.Parms.AddParm(AttachmentType, "MATTACH");
                    Approval.ApprovalBusinessObject.LoadData();
                    Approval.ApprovalBusinessObject.Parms.ClearParms();

                    string wfStatus = (string)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0]["approval_description"];

                    

                    //SetDefaultDates();

                    //if posted or (approved and not a Proforma, need to be able to change the porforma to a prev proforma)
                    //Need to also add logic if is in process status in workflow need to lock down 
                    //if it gets rejected, then we need to unprotect for it to be corrected.
                    //If invoice has been submitted for approval, need to lock it down.
                    //If invoice has been rejected, need to allow for edit.

                    if ((postedFlag == 1) | (postedFlag == 2) | (wfStatus[0] == 'I') )
                    {
                        this.CanExecuteSaveCommand = false;
                        this.CanExecuteDeleteCommand = true;
                        Details.idgDetails.ContextMenuAddIsVisible = false;
                        Details.idgDetails.xGrid.FieldSettings.AllowEdit = false;
                        Details.txtDiscountAmount.IsReadOnly = true;
                        General.cmbCompany.IsEnabled = false;
                        General.cmbInvoiceType.IsEnabled = false;
                        General.txtDescription.IsEnabled = false;
                        General.ldpInvoiceDate.IsEnabled = false;
                        General.ldpDueDate.IsEnabled = false;
                        General.cmbCurrencyCode.IsEnabled = false;
                        General.txtDiscountAccount.IsEnabled = false;
                        General.txtContractName.IsEnabled = false;
                        General.chkRecurring.IsEnabled = true;
                        General.ldpRecurringEndDate.IsEnabled = true;
                        General.cmbRecurringFrequency.IsEnabled = true;
                        General.txtApprovalStatus.IsEnabled = false;
                        General.txtApprovalId.IsEnabled = false;
                        General.txtApprovalDate.IsEnabled = false;
                        okToSave = false;
                        Approval.btnSubmit.IsEnabled = false;
                        Approval.btnAddApprover.IsEnabled = false;
                        Approval.btnApprove.IsEnabled = false;
                        Approval.btnInquiry.IsEnabled = false;
                        Approval.btnReject.IsEnabled = false;
                        Approval.btnReply.IsEnabled = false;


                        //Need to allow workflow buttons to work for in process
                        if (wfStatus[0] == 'I')
                        {
                            Approval.btnSubmit.IsEnabled = true;
                            Approval.btnAddApprover.IsEnabled = true;
                            Approval.btnApprove.IsEnabled = true;
                            Approval.btnInquiry.IsEnabled = true;
                            Approval.btnReject.IsEnabled = true;
                            Approval.btnReply.IsEnabled = true;
                        }

              
                        //The exception is for an approved Proforma (which is never posted).
                        //Need to be able to edit the invoice type to previous proforma and save.
                        //Then be able to submit for approval.
                        if ((invoiceTypeId == 1) & (postedFlag == 2))
                        {
                            General.cmbInvoiceType.IsEnabled = true;
                            this.CanExecuteSaveCommand = true;
                            Approval.btnSubmit.IsEnabled = true;
                        }

                        

                     




                    }
                    else
                    {
                        this.CanExecuteDeleteCommand = true;
                        okToSave = true;
                        //Allow edit of the following just in case they were turned off by someone previously viewing a posted/approved invoice:

                        this.CanExecuteSaveCommand = true;
                        this.CanExecuteDeleteCommand = true;
                        Details.idgDetails.ContextMenuAddIsVisible = true;
                        Details.idgDetails.xGrid.FieldSettings.AllowEdit = true;
                        Details.txtDiscountAmount.IsReadOnly = false;
                        General.cmbCompany.IsEnabled = true;
                        General.cmbInvoiceType.IsEnabled = true;
                        General.txtDescription.IsEnabled = true;
                        General.ldpInvoiceDate.IsEnabled = true;
                        General.ldpDueDate.IsEnabled = true;
                        General.cmbCurrencyCode.IsEnabled = true;
                        General.txtDiscountAccount.IsEnabled = true;
                        General.txtContractName.IsEnabled = true;
                        General.chkRecurring.IsEnabled = true;
                        General.ldpRecurringEndDate.IsEnabled = true;
                        General.cmbRecurringFrequency.IsEnabled = true;
                        General.txtApprovalStatus.IsEnabled = true;
                        General.txtApprovalId.IsEnabled = true;
                        General.txtApprovalDate.IsEnabled = true;
                        okToSave = true;
                        Approval.btnSubmit.IsEnabled = true;
                        Approval.btnAddApprover.IsEnabled = true;
                        Approval.btnApprove.IsEnabled = true;
                        Approval.btnInquiry.IsEnabled = true;
                        Approval.btnReject.IsEnabled = true;
                        Approval.btnReply.IsEnabled = true;

                        //Need to not allow workflow buttons to work except Submit if not submitted or refejected.
                        if (wfStatus[0] == 'N' || wfStatus[0] == 'R')
                        {
                            Approval.btnSubmit.IsEnabled = true;
                            Approval.btnAddApprover.IsEnabled = false;
                            Approval.btnApprove.IsEnabled = false;
                            Approval.btnInquiry.IsEnabled = false;
                            Approval.btnReject.IsEnabled = false;
                            Approval.btnReply.IsEnabled = false;
                        }



                    }


                    if ((int)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][printedFlagField] == 1)
                    {
                        General.btnPrint.IsEnabled = true;

                    }
                    else
                    {
                        General.btnPrint.IsEnabled = false;

                    }

                    notFirstNew = true;
                    Attachments.TabIsEnabled = true;
                    //Attachments.ContextMenu.IsEnabled = true;
                    //GeneralMain.Focus();

                }
                else
                {

                    if (notFirstNew)
                    {
                        txtCustomerAccount.Text = string.Empty;
                        //txtCustomInvoiceNumber.Text = string.Empty;
                        this.CurrentBusObj.Parms.ClearParms();
                        CurrentBusObj.ObjectData.Tables[detailTableName].Clear();
                        CurrentBusObj.ObjectData.Tables[acctDetailTableName].Clear();
                        Details.DetailRecords.Clear();
                        Details.AccountDetailRecords.Clear();
                        Approval.invoiceNumber = invoiceNumber;
                        Approval.ApprovalBusinessObject.Parms.ClearParms();
                        Approval.ApprovalBusinessObject.Parms.AddParm(documentIdParameter, invoiceNumber);
                        Approval.ApprovalBusinessObject.LoadData();
                        Approval.idgAdjustments.LoadGrid(Approval.ApprovalBusinessObject, mainTableName);
                        Approval.ApprovalBusinessObject.Parms.ClearParms();
                        Approval.ApprovalBusinessObject.ObjectData.Clear();
                        
                        //Approval.ApprovalBusinessObject.LoadData();
                        Details.idgRevenueAllocation.xGrid.DataSource = String.Empty;
                    }
                    Messages.ShowInformation("Invoice not found.");
                    this.CanExecuteSaveCommand = false;
                    //GeneralMain.Focus();
                    
                }

            }
        }

        private void addAttachmentParms(string invoiceId)
        {
            //comment tab parms
            this.CurrentBusObj.Parms.AddParm(attachmentParameter, invoiceId);
            this.CurrentBusObj.Parms.AddParm(commentTypeParameter, "MS");
            this.CurrentBusObj.Parms.AddParm(commentAttachmentsIdParameter, "-1");
            //attachment tab parms
            this.CurrentBusObj.Parms.AddParm(attachmentType, "MATTACH");
            this.CurrentBusObj.Parms.AddParm(attachmentExtIntParameter, "0");
            this.CurrentBusObj.Parms.AddParm(attachmentLocID, "-1");
        }

        public override void Save()
        {
            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables[mainTableName].Rows.Count > 0)
            {
                if ((txtCustomerAccount.Text != null)  & (txtCustomerAccount.Text != ""))
                {
                    CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][receivableAccountField] = txtCustomerAccount.Text;
                    Details.idgDetails.ContextMenuAddIsVisible = true; 
                }
                else
                {
                    Messages.ShowInformation("Customer not valid.");
                    Details.idgDetails.ContextMenuAddIsVisible = false; 
                    txtCustomerAccount.CntrlFocus();
                    //GeneralMain.Focus();
                    return;
                }
                //if ((invoiceNumber.StartsWith("MIN")) || (invoiceNumber.StartsWith("min")) || (string.IsNullOrEmpty(txtCustomInvoiceNumber.Text)))  
                //if (invoiceTypeId != 1)
                if (General.cmbInvoiceType.SelectedValue.ToString() == "0")
                {                    
                    General.chkConvert.Visibility = Visibility.Collapsed;
                    General.chkClose.Visibility = Visibility.Collapsed;

                }
                else
                {
                    General.chkConvert.Visibility = Visibility.Visible;
                    General.chkClose.Visibility = Visibility.Visible;
                    General.chkConvert.IsEnabled = true;
                    General.chkClose.IsEnabled = true;
                }
            }


          
            if ((txtCustomerAccount.Text != null) & (txtCustomerAccount.Text != ""))
            {
                //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                    selectedCompany = General.cmbCompany.SelectedValue.ToString();

                if (string.IsNullOrEmpty(selectedCompany))
                {
                    Messages.ShowInformation("Company is required.");
                    Details.idgDetails.ContextMenuAddIsVisible = false;
                    GeneralMain.Focus();
                    return;
                }
                else
                {
                    if (General.chkRecurring.IsChecked == 1)
                    {

                        if (General.ldpRecurringEndDate.SelText.ToString() == "1/1/1900 12:00:00 AM")
                        {
                            Messages.ShowInformation("Recurring End Date is required.");
                            bypassSave = true;
                            GeneralMain.Focus();
                            return;
                        }       
                        
                        
                        if (General.cmbRecurringFrequency.SelectedText.ToString() == "")
                        {
                            Messages.ShowInformation("Recurring Frequency is required.");
                            bypassSave = true;
                            GeneralMain.Focus();
                            return;

                        }

                               
                    }
                }
            }
            else
            {
                return;
            }


            //if (General.chkRecurring.IsChecked == 1)
            //{
            //    if (General.cmbRecurringFrequency.SelectedText.ToString() == "")
            //    {
            //        Messages.ShowInformation("Recurring Frequency is required.");
            //        Details.idgDetails.ContextMenuAddIsVisible = false; 
            //        return;                     
                    
            //    }

            //    //if (General.ldpRecurringEndDate.SelText.ToString() == "")
            //    //{
            //    //    Messages.ShowInformation("Recurring End Date is required.");
            //    //    txtCustomerAccount.CntrlFocus();      
            //    //    return;
            //    //}               
            //}
           
 
            base.Save();

            if (this.SaveSuccessful)
                ////check if attachment tab files need to be copied
                //if (cGlobals.GlobalAttachmentsStorageList.Count > 0)
                //{
                //    //if so pass attachment data table to attachment helper class
                //    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                //    attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables["Attachments"]);

                //}

                ////check if comment attachment files need to be copied
                //if (cGlobals.GlobalCommentAttachmentsStorageList.Count > 0)
                //{
                //    //if so pass attachment data table to attachment helper class
                //    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                //    attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables["Comment_Attachment"]);

                //}


                //If this is the first time a new invoice was saved, display message indicating the new invoice was saved and detail must be added.
                if (showNewInvoiceSaveMessage)
                {
                    if (CurrentBusObj.ObjectData.Tables[mainTableName].Rows.Count > 0)
                    {
                        invoiceNumber = CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][invoiceField].ToString();
                    }

                    Messages.ShowInformation("New Invoice - " + invoiceNumber.ToString() + " Save Successful. Please Add Detail.");
                    showNewInvoiceSaveMessage = false;
                    Attachments.TabIsEnabled = true;
                    //this.CurrentBusObj.Parms.UpdateParmValue("@invoice_number", invoiceNumber);
                }
                else
                {

                    if (GeneralMain.IsSelected.Equals(true))
                    {

                        if (showSaveMessage)
                        {
                            Messages.ShowInformation("Save Successful.");
                            showSaveMessage = true;
                        }

                    }
                }

            //if (string.IsNullOrEmpty(txtCustomInvoiceNumber.Text))
            {
                if (CurrentBusObj.ObjectData.Tables[mainTableName].Rows.Count > 0)
                {
                    invoiceNumber = CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][invoiceField].ToString();
                    General.invoiceNumber = invoiceNumber;
                    txtCustomInvoiceNumber.Text = invoiceNumber;
                    Details.invoiceNumber = invoiceNumber;
                    //If invoice gets changed from proforma to previously proforma, need to clear out workflow as we keep proforma workflow records.

                    Approval.invoiceNumber = invoiceNumber;
                    Approval.ApprovalBusinessObject.Parms.ClearParms();
                    Approval.ApprovalBusinessObject.Parms.AddParm(documentIdParameter, invoiceNumber);
                    //Approval.ApprovalBusinessObject.Parms.AddParm(ExternalCharId, invoiceNumber);
                    //Approval.ApprovalBusinessObject.Parms.AddParm(AttachmentType, "MATTACH");
                    Approval.ApprovalBusinessObject.LoadData();
                    //if (this.CurrentBusObj.ObjectData != null)
                    //{
                    //    if (this.CurrentBusObj.ObjectData.Tables["attachments"].Rows.Count != 0)
                    //        foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["attachments"].Rows)
                    //        {
                    //            string PathFile = dr["path"].ToString() + dr["prod_filename"].ToString();
                    //            if (!System.IO.File.Exists(PathFile))
                    //                dr["color_status"] = 3;
                    //        }
                    //}
                    Approval.idgAdjustments.LoadGrid(Approval.ApprovalBusinessObject, mainTableName);
                    Approval.ApprovalBusinessObject.Parms.ClearParms();

                    this.CurrentBusObj.changeParm(attachmentParameter, invoiceNumber);
                    invoiceTypeId = (int)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][invoiceTypeField];
                    SetInvoiceTypes(invoiceTypeId);
 
                }
            }
            //this.CurrentBusObj.LoadData();
            Details.SaveData();

            

            if ((int)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][postedFlagField] == 1) 
            {
                this.CanExecuteSaveCommand = false;
                this.CanExecuteDeleteCommand = false;
            }
            else
            {
                this.CanExecuteDeleteCommand = true;
            }

        }

        public override void New()
        {
            GeneralMain.Focus();

            showNewInvoiceSaveMessage = true;
            Attachments.TabIsEnabled = false;           
           
            
            invoiceNumber = string.Empty;
            General.invoiceNumber = invoiceNumber;

            txtCustomerAccount.IsEnabled = true;
            txtCustomerAccount.Text = string.Empty;
            txtCustomInvoiceNumber.Text = string.Empty;

            this.CurrentBusObj.Parms.ClearParms();
            string invoiceParameter = string.Format("@{0}", invoiceField);
            this.CurrentBusObj.Parms.AddParm(invoiceParameter, invoiceNumber);
            this.CurrentBusObj.Parms.AddParm(documentIdParameter, string.Empty);

            //Allow edit of the following just in case they were turned off by someone previously viewing a posted/approved invoice:

            this.CanExecuteSaveCommand = true;
            this.CanExecuteDeleteCommand = true;
            Details.idgDetails.ContextMenuAddIsVisible = true;
            Details.idgDetails.xGrid.FieldSettings.AllowEdit = true;
            Details.txtDiscountAmount.IsReadOnly = false;
            General.cmbCompany.IsEnabled = true;
            General.cmbInvoiceType.IsEnabled = true;            
            General.chkConvert.Visibility = Visibility.Collapsed;
            General.chkClose.Visibility = Visibility.Collapsed;

            
            General.chkProforma.IsEnabled = false;
            General.chkProformaEmail.IsEnabled = false;
            General.ldpProformaEmailSentDate.IsEnabled = false;
            General.txtDescription.IsEnabled = true;
            General.ldpInvoiceDate.IsEnabled = true;
            General.ldpDueDate.IsEnabled = true;
            General.cmbCurrencyCode.IsEnabled = true;
            General.txtDiscountAccount.IsEnabled = true;
            General.txtContractName.IsEnabled = true;
            General.chkRecurring.IsEnabled = true;
            General.ldpRecurringEndDate.IsEnabled = true;
            General.cmbRecurringFrequency.IsEnabled = true;
            General.txtApprovalStatus.IsEnabled = true;
            General.txtApprovalId.IsEnabled = true;
            General.txtApprovalDate.IsEnabled = true;
            okToSave = true;
            Approval.btnSubmit.IsEnabled = true;
            Approval.btnAddApprover.IsEnabled = false;
            Approval.btnApprove.IsEnabled = false;
            Approval.btnInquiry.IsEnabled = false;
            Approval.btnReject.IsEnabled = false;
            Approval.btnReply.IsEnabled = false;
           

            if (notFirstNew)  
            {
               //this.CurrentBusObj.Parms.ClearParms();
               CurrentBusObj.ObjectData.Tables[detailTableName].Clear();
               CurrentBusObj.ObjectData.Tables[acctDetailTableName].Clear();
               Details.DetailRecords.Clear();
               Details.AccountDetailRecords.Clear();
               Details.idgRevenueAllocation.xGrid.DataSource = String.Empty;
               Details.invoiceNumber = invoiceNumber;
               Approval.invoiceNumber = invoiceNumber;
               Approval.ApprovalBusinessObject.Parms.ClearParms();
               Approval.ApprovalBusinessObject.Parms.AddParm(documentIdParameter, invoiceNumber);
               Approval.ApprovalBusinessObject.LoadData();
               Approval.idgAdjustments.LoadGrid(Approval.ApprovalBusinessObject, mainTableName);
               Approval.ApprovalBusinessObject.Parms.ClearParms();

               
              

            }

          
            
            //add attachment parms
            addAttachmentParms(invoiceNumber);
            this.CurrentBusObj.Parms.AddParm(frequencyParameterName, frequencyParameterValue);
            this.CurrentBusObj.Parms.AddParm(recurringInvoiceParameter, string.Empty);

            //RES 4/19/16 add CLM Number
            this.CurrentBusObj.Parms.AddParm("@contract_number", "0");

            base.New();

            Details.DetailRecords = CurrentBusObj.ObjectData.Tables[detailTableName].Copy();
            Details.AccountDetailRecords = CurrentBusObj.ObjectData.Tables[acctDetailTableName].Copy();
            
            Approval.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
            

            SetDefaultDates();

            notFirstNew = true;
             

            txtCustomerAccount.CntrlFocus();
            SetInvoiceTypes(3); //Only display Invoice and Proforma in dropdown




            
        }

        public override void Delete()
        {
            if (invoiceNumber == "")
            {

            }
            else
            {

                this.LoadData(invoiceNumber);

                int postedFlag = (int)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][postedFlagField];



                string wfStatus = (string)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0]["approval_description"];

                int DeleteFlag = (int)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0]["delete_flag"];
                string SubmitID = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["creator_id"].ToString();

                  //if ((postedFlag == 1) | (postedFlag == 2) | ((wfStatus[0] == 'I') & DeleteFlag > 0) | (SubmitID != cGlobals.UserName))
                    if ((postedFlag == 1) | (postedFlag == 2) |(DeleteFlag > 0))
                {

                    Messages.ShowInformation("Unable to delete this invoice. It is either Posted, Approved or In Workflow Process.");
                    return;
                }


            }






            if (invoiceTypeId == 2)
            {
                //Messages.ShowInformation("Previously Proforma successfully updated to Proforma.");
            }
            else
            {

                System.Windows.MessageBoxResult result = Messages.ShowYesNo("Would you like to delete this invoice?",
                            System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.No)
                {
                    return;
                }

            }




            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables[mainTableName].Rows.Count > 0)
            {
                invoiceTypeId = (int)CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][invoiceTypeField];
                CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0].Delete();
            }

            CurrentBusObj.ObjectData.Tables[detailTableName].Clear();
            CurrentBusObj.ObjectData.Tables[acctDetailTableName].Clear();

            Details.DetailRecords.Clear();
            Details.AccountDetailRecords.Clear();

            //Approval.ApprovalBusinessObject.ObjectData.Clear();

            txtCustomerAccount.Text = string.Empty;
            txtCustomInvoiceNumber.Text = string.Empty;
            General.invoiceNumber = string.Empty;
            invoiceNumber = string.Empty;
            Details.invoiceNumber = string.Empty;


            base.Save();
            Details.SaveData();
            Details.AccountDetailRecords.Clear();
            Details.idgRevenueAllocation.xGrid.DataSource = String.Empty;
            this.CanExecuteSaveCommand = false;
            this.CanExecuteDeleteCommand = false;
            CurrentBusObj.ObjectData.Tables[mainTableName].Clear();
            CurrentBusObj.ObjectData.Tables[viewTableName].Clear();
            CurrentBusObj.ObjectData.Tables[adjustmentsTableName].Clear();

            if (invoiceTypeId == 2)
            {
                Messages.ShowInformation("Previously Proforma successfully updated to Proforma.");
            }
            else
            {
                Messages.ShowInformation("Delete Successful.");
            }
            windowCaption = "";

            GeneralMain.Focus();





        }

        public void cmbInvoiceType_SelectionChanged(object sender, RoutedEventArgs e)
        {

            if (General.cmbInvoiceType.SelectedValue != null)
            {
                //To make sure the first time the screen is loaded the logic is not executed
                if (General.cmbInvoiceType.SelectedValue.ToString() == "1")
                {
                    General.chkProforma.IsEnabled = true;
                    General.chkProformaEmail.IsEnabled = true;
                    General.ldpProformaEmailSentDate.IsEnabled = true;
                }
                else
                {
                    General.chkProforma.IsEnabled = false;
                    General.chkProforma.IsChecked = 0;
                    General.chkProformaEmail.IsEnabled = false;
                    General.chkProformaEmail.IsChecked = 0;
                    General.ldpProformaEmailSentDate.IsEnabled = false;

                }
            }


        }

        

        public void cmbCompany_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //Company has been selected, okay to let a detail record be added.
            Details.idgDetails.ContextMenuAddIsVisible = true;     

            if (this.CurrentBusObj.HasObjectData && okToSave)
            {
                if (General.cmbCompany.SelectedValue != null)
                {
                    //To make sure the first time the screen is loaded the logic is not executed
                
                    if (differentInvoice)
                    {
                        differentInvoice = false;
                        ////selectedCompany = CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][companyValue].ToString();
                        ////Details.OnCompanyCodeChanged(selectedCompany);
                        //Details.selectedCompany = General.cmbCompany.SelectedValue.ToString();
                        //Details.OnCompanyCodeChanged(General.cmbCompany.SelectedValue.ToString());
                        return;
                    }
                  
                    if (!string.IsNullOrEmpty(Details.selectedCompany) && Details.selectedCompany != General.cmbCompany.SelectedValue.ToString())
                    {
                        if (this.CurrentBusObj.ObjectData.Tables[detailTableName].Rows.Count > 0 || Details.DetailRecords.Rows.Count > 0)
                        {
                            //if (txtCustomInvoiceNumber.Text != invoiceNumber) { return; }
                            //string message = string.Format("Changing the company will cause the detail records {0} to be deleted.  Are you sure you want to continue?", Environment.NewLine);
                            //if (Messages.ShowYesNo(message, MessageBoxImage.Question) == MessageBoxResult.Yes)
                           // {
                                //TODO add the logic to delete the detail records
                                //foreach (DataRow detailRow in CurrentBusObj.ObjectData.Tables[detailTableName].Rows)
                                foreach (DataRow detailRow in Details.DetailRecords.Rows)
                                {
                                    cGlobals.BillService.DeleteCustomInvoiceDetail(invoiceNumber, (int)detailRow[idField]);
                                }

                                CurrentBusObj.ObjectData.Tables[detailTableName].Clear();
                                CurrentBusObj.ObjectData.Tables[acctDetailTableName].Clear();
                                Details.DetailRecords.Clear();
                                Details.AccountDetailRecords.Clear();
                                Details.idgRevenueAllocation.xGrid.DataSource = String.Empty;
                                Details.invoiceNumber = invoiceNumber;
                                CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0]["total_amt"] = 0;
                                CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0]["disc_amt"] = 0;
                                CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0]["net_amt"] = 0;

                                Details.SaveData();
                                Save();
                                LoadData(invoiceNumber);
                                
                                Messages.ShowInformation("Company has changed. Details were deleted successfully.");
                                
                           // }
                            //else
                           // {
                                //return;
                                //this.General.cmbCompany.SelectedValue = this.CurrentBusObj.ObjectData.Tables[mainTableName].Rows[0][companyValue].ToString();
                            //}
                        }
                    }
                    Details.selectedCompany = General.cmbCompany.SelectedValue.ToString();
                    Details.OnCompanyCodeChanged(General.cmbCompany.SelectedValue.ToString());
                }
            }
        }
              

        private void SetInvoiceTypes(int invoiceTypeId)
        {
            switch (invoiceTypeId)
            {
                case 0:
                    var rows = (from r in CurrentBusObj.ObjectData.Tables[invoiceTypeTableName].AsEnumerable()
                                where r.Field<int>(invoiceTypeValueField) > 0
                                select r).ToList();

                    if (rows != null && rows.Count > 0)
                    {
                        rows.ForEach(r => r.Delete());
                        rows.ForEach(r => r.AcceptChanges());
                    }
                    break;
                case 1:
                    var invTypes = (from r in CurrentBusObj.ObjectData.Tables[invoiceTypeTableName].AsEnumerable()
                                    where r.Field<int>(invoiceTypeValueField) == 0
                                    select r).ToList();

                    if (invTypes != null && invTypes.Count > 0)
                    {
                        invTypes[0].Delete();
                        invTypes[0].AcceptChanges();
                    }
                    break;
                case 2:
                    var prevProTypes = (from r in CurrentBusObj.ObjectData.Tables[invoiceTypeTableName].AsEnumerable()
                                        where r.Field<int>(invoiceTypeValueField) < 2
                                        select r).ToList();

                    if (prevProTypes != null && prevProTypes.Count > 0)
                    {
                        prevProTypes.ForEach(r => r.Delete());
                        prevProTypes.ForEach(r => r.AcceptChanges());
                    }
                    break;


                case 3: //only display Invoice and Proforma on new invoice screen
                    var newInvTypes = (from r in

                                           CurrentBusObj.ObjectData.Tables[invoiceTypeTableName].AsEnumerable()
                                       where r.Field<int>(invoiceTypeValueField) > 1
                                       select r).ToList();

                    if (newInvTypes != null && newInvTypes.Count > 0)
                    {
                        newInvTypes.ForEach(r => r.Delete());
                        newInvTypes.ForEach(r => r.AcceptChanges());
                    }
                    break;
            }
        }


        private void SetDefaultDates()
        {  
            var accountingPeriod = (from r in CurrentBusObj.ObjectData.Tables[dateTypeTableName].AsEnumerable()
                                    where r.Field<string>(dateTypeField) == accountingPeriodField
                                    select r).ToList();

            var billingPeriod = (from r in CurrentBusObj.ObjectData.Tables[dateTypeTableName].AsEnumerable()
                                    where r.Field<string>(dateTypeField) == billingPeriodField
                                    select r).ToList();


            General.ldpAccountingPeriod.SelText = (DateTime)accountingPeriod[0][dateValueField];
            General.ldpBillingPeriod.SelText = (DateTime)billingPeriod[0][dateValueField];
            General.ldpAccountingPeriod.IsEnabled = false;
            General.ldpBillingPeriod.IsEnabled = false;
            General.ldpInvoiceDate.SelText = DateTime.Now;
        }
                        

        private void tcMainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object source = e.Source;



            if (e.RemovedItems.Count > 0)
            {
                if (e.RemovedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.RemovedItems[0]).Header.Equals("Details"))
                {
                    //If this is an insert then save the main table to get an invoice id.
                    if (string.IsNullOrEmpty(invoiceNumber))
                    {
                        //Save();
                    }
                    else
                    {
                        if (bypassSave.ToString() == "False")
                        {
                            showSaveMessage = false;
                            Save();
                            
                        }
                        else
                        {
                            bypassSave = false;
                        }

                        //Details.SaveData();
                    }
                }
            }

            if (e.RemovedItems.Count > 0)
            {
                if (e.RemovedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.RemovedItems[0]).Header.Equals("General"))
                {
                    //If this is a new invoce then save the main table to get an invoice id.
                    if (string.IsNullOrEmpty(invoiceNumber))
                    {
                       
                        if (okToSave)
                        {
                            Save();
                        }
                    }
                  
                }
            }


            
            if (e.RemovedItems.Count > 0)
            {
                if (e.RemovedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.RemovedItems[0]).Header.Equals("Approval"))
                {
                    //Only need to Load data if an invoice has already been saved.
                    //This is to display the correct workflow status on the general tab if it has changed since the inoice was
                    //was created or loaded on the general tab.
                    if (string.IsNullOrEmpty(invoiceNumber))
                    {
                        //Save();
                    }
                    else
                    {
                        LoadData(invoiceNumber);
                    }
                }
            }




        }

        //KSH - 8/24/12 clear comments/attachments grid if applicable bus obj
        private void clrCommentsAttachmentsObj()
        {
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();
        }

        private void Adjustments_Loaded(object sender, RoutedEventArgs e)
        {

        }

        



    }
}
