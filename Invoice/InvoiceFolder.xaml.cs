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
using RazerBase.Lookups;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;

namespace Invoice
{
    
    //Interaction logic for InvoiceFolder.xaml
    public partial class InvoiceFolder : ScreenBase, IScreen
    {
        private static readonly string approvalObjectName = "EditInvoiceapproval";

        #region class vars
        private string InvoiceNumber { get; set; }
        private string CustomerID { get; set; }

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }

        #endregion

        #region Constructor Stuff
        public InvoiceFolder()
            : base()
        {
            // Create Controls
            InitializeComponent();

            // performs initializations for this object.
            //Init();
        }

        public void Init(cBaseBusObject businessObject)
        {

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "Invoice";
            this.DoNotSetDataContext = false;
            // set the businessObject
            this.CurrentBusObj = businessObject;

            // add the Tabs
            TabCollection.Add(General);
            TabCollection.Add(Detail);
            TabCollection.Add(Adjustments);
            TabCollection.Add(uAttachments);
            TabCollection.Add(View);
            TabCollection.Add(uComments);
            TabCollection.Add(Deferred);

            // if there are parameters than we need to load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                string invoiceNumber = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();
                
                //set document_id for View tab
                this.loadParms(invoiceNumber);
                // load the data
                this.Load();
                // Set the Header
                //Need to chack 
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
                {
                    windowCaption = "Invoice Number -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["inv_nbr"].ToString();
                    txtInvoiceNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["inv_nbr"].ToString();
                    txtCustomerID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["acct_name"].ToString();
                    txtCompany.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["company_description"].ToString();
                    //RES 5/1/25 Edit Invioce                    
                    EditInvoiceApprovalTab.invoiceNumber = txtInvoiceNumber.Text;
                    EditInvoiceApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                    EditInvoiceApprovalTab.ApprovalBusinessObject.Parms.AddParm("@document_id", txtInvoiceNumber.Text);
                    EditInvoiceApprovalTab.ApprovalBusinessObject.LoadData();
                    EditInvoiceApprovalTab.idgInvApproval.LoadGrid(EditInvoiceApprovalTab.ApprovalBusinessObject, "approval");
                    EditInvoiceApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
                    if (this.CurrentBusObj.ObjectData.Tables["edit"].Rows[0]["approval_flag"].ToString() == "Y")
                    {
                        EditInvoiceApprovalTab.btnSubmit.IsEnabled = true;
                        EditInvoiceApprovalTab.btnAddApprover.IsEnabled = true;
                        EditInvoiceApprovalTab.btnApprove.IsEnabled = true;
                        EditInvoiceApprovalTab.btnInquiry.IsEnabled = true;
                        EditInvoiceApprovalTab.btnReject.IsEnabled = true;
                        EditInvoiceApprovalTab.btnReply.IsEnabled = true;
                    }
                    else
                    {
                        EditInvoiceApprovalTab.btnSubmit.IsEnabled = false;
                        EditInvoiceApprovalTab.btnAddApprover.IsEnabled = false;
                        EditInvoiceApprovalTab.btnApprove.IsEnabled = false;
                        EditInvoiceApprovalTab.btnInquiry.IsEnabled = false;
                        EditInvoiceApprovalTab.btnReject.IsEnabled = false;
                        EditInvoiceApprovalTab.btnReply.IsEnabled = false;
                    }
                }
                else
                    Messages.ShowWarning("Invoice not found!");
                 
            }
            this.HasPrintReport = true;
        }
        #endregion

        private void loadParms(string invoiceNumber)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //if invoiceNumber passed load invoice number and document id
                if (invoiceNumber != "")
                {
                    this.CurrentBusObj.Parms.AddParm("@invoice_number", invoiceNumber);
                    this.CurrentBusObj.Parms.AddParm("@document_id", invoiceNumber);
                    this.CurrentBusObj.Parms.AddParm("@external_char_id", invoiceNumber);
                      
                }
                else
                {
                    //if invoiceNumber NOT passed load   with global parm invoiceNumber if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.AddParm("@invoice_number", cGlobals.ReturnParms[0].ToString());
                        this.CurrentBusObj.Parms.AddParm("@document_id", cGlobals.ReturnParms[0].ToString());
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", cGlobals.ReturnParms[0].ToString());
                    }
                    //set dummy vals
                    else
                    {
                        this.CurrentBusObj.Parms.AddParm("@invoice_number", "-1");
                        this.CurrentBusObj.Parms.AddParm("@document_id", "-1");
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");
                        //this.CurrentBusObj.Parms.AddParm("@related_to_char_id", "-1");
                    }
                }
                //comment tab parms
                this.CurrentBusObj.Parms.AddParm("@comment_type", "IV");
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
                //attachment tab parms
                this.CurrentBusObj.Parms.AddParm("@attachment_type", "IATTACH");
                this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }
        
        private void txtInvoiceNumber_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            InvoiceNumber = txtInvoiceNumber.Text;
        }

        private void txtInvoiceNumber_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //If the Document ID changed then load a new document
            if (txtInvoiceNumber.Text != InvoiceNumber)
                ReturnData(txtInvoiceNumber.Text, "@invoice_number");
        }
       
        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                 
                return true;
            }
            else
            {
                Messages.ShowWarning("Invoice Number Not Found");
                return false;
            }
        }

        private void ReturnData(string SearchValue, string DbParm)
        {
            //if no value do nothing
            if (SearchValue == "") return;
            //Add new parameters
            loadParms(SearchValue);
            //KSH - 8/21/12 clear comments/attachments grid to fix bug
            clrCommentsAttachmentsObj();
            //load data
            //if coming from save do not do this...
            this.Load();
            //if invoiceNumber found then set header and pop otherwise send message
            if (chkForData())
            {
                SetHeaderName();
                EditInvoiceApprovalTab.invoiceNumber = txtInvoiceNumber.Text;
                EditInvoiceApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                EditInvoiceApprovalTab.ApprovalBusinessObject.Parms.AddParm("@document_id", txtInvoiceNumber.Text);
                EditInvoiceApprovalTab.ApprovalBusinessObject.LoadData();
                EditInvoiceApprovalTab.idgInvApproval.LoadGrid(EditInvoiceApprovalTab.ApprovalBusinessObject, "approval");
                EditInvoiceApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
                if (this.CurrentBusObj.ObjectData.Tables["edit"].Rows[0]["approval_flag"].ToString() == "Y")
                {
                    EditInvoiceApprovalTab.btnSubmit.IsEnabled = true;
                    EditInvoiceApprovalTab.btnAddApprover.IsEnabled = true;
                    EditInvoiceApprovalTab.btnApprove.IsEnabled = true;
                    EditInvoiceApprovalTab.btnInquiry.IsEnabled = true;
                    EditInvoiceApprovalTab.btnReject.IsEnabled = true;
                    EditInvoiceApprovalTab.btnReply.IsEnabled = true;
                }
                else
                {
                    EditInvoiceApprovalTab.btnSubmit.IsEnabled = false;
                    EditInvoiceApprovalTab.btnAddApprover.IsEnabled = false;
                    EditInvoiceApprovalTab.btnApprove.IsEnabled = false;
                    EditInvoiceApprovalTab.btnInquiry.IsEnabled = false;
                    EditInvoiceApprovalTab.btnReject.IsEnabled = false;
                    EditInvoiceApprovalTab.btnReply.IsEnabled = false;
                }
            }
        }

        /// Sets HeaderName based on value entered into  textbox
        /// </summary>
        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (txtInvoiceNumber.Text == null)
            {
                windowCaption = "Invoice Number -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["inv_nbr"].ToString();
                txtInvoiceNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["inv_nbr"].ToString();
                txtCustomerID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["acct_name"].ToString();
                txtCompany.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["company_description"].ToString();
                //InvoiceNumber = txtInvoiceNumber.Text;


            }
            //Sets the header name from within same folder
            else
            {
                ContentPane p = (ContentPane)this.Parent;
                p.Header = "Invoice Number -" + txtInvoiceNumber.Text;
                txtInvoiceNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["inv_nbr"].ToString();
                txtCustomerID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["acct_name"].ToString();
                txtCompany.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["company_description"].ToString();
            }
        }

        private void txtInvoiceNumber_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on Entity ID field
            //InvoiceFolderLookup f = new InvoiceFolderLookup();

            InvoiceLookup f = new InvoiceLookup();
            f.Init(new cBaseBusObject("InvoiceFolderLookup"));
            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();
            f.Title = "Invoice Search";


            this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            //DWR - Modifie 3/11/13 - Added 2nd part of if check as screen would crash app if filter and then cancel was clicked in the lookup.
            if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0]!= null)
            {

                //load current parms
                loadParms("");
                txtInvoiceNumber.Text = cGlobals.ReturnParms[0].ToString();
                //KSH - 8/21/12 clear comments/attachments grid to fix bug
                clrCommentsAttachmentsObj();
                // Call load 
                this.Load();

                windowCaption = "Invoice -" + txtInvoiceNumber.Text;
                InvoiceNumber = txtInvoiceNumber.Text.ToString();
                SetHeaderName();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
                //General.Focus();

            }
        }

        /// <summary>
        /// KSH - 8/21/12 clear comments/attachments grid to fix bug
        /// </summary>
        private void clrCommentsAttachmentsObj()
        {
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();
        }
        

    }
}