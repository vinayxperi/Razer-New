

#region using statements


using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;

#endregion

namespace Invoice
{

    #region class InvoiceDetailTab
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class InvoiceDetailTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public InvoiceDetailTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

        #region Init()
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "detail";
            //Set up Parent Child Relationship
            //Create the Customer Document object
            CurrentBusObj = new cBaseBusObject("Invoice");
            CurrentBusObj.Parms.ClearParms();
            
            //Establish the Invoice Detail Grid
            gInvoiceDetail.MainTableName = "detail";
            gInvoiceDetail.ConfigFileName = "InvoiceDetailTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gInvoiceDetail.SetGridSelectionBehavior(false, true);
            gInvoiceDetail.FieldLayoutResourceString = "InvoiceDetail";
            gInvoiceDetail.WindowZoomDelegate = GridDoubleClickDelegate;
            // 12/23/20 Add menu item for new revenue recognition window 
            //gInvoiceDetail.ContextMenuAddIsVisible = false;
            //gInvoiceDetail.ContextMenuRemoveIsVisible = false;
            //gInvoiceDetail.ContextMenuAddDelegate = GridInvoiceDetailRevRecDelegate;
            //gInvoiceDetail.ContextMenuAddDisplayName = "Show Revenue Recognition";
            //gInvoiceDetail.ContextMenuAddIsVisible = true;
            gInvoiceDetail.ContextMenuGenericDelegate1 = GridInvoiceDetailRevRecDelegate;
            gInvoiceDetail.ContextMenuGenericDisplayName1 = "Show Revenue Recognition";
            gInvoiceDetail.ContextMenuGenericIsVisible1 = true;
            // 3/28/25 Add menu item for sreen edit sub counts to create a proforma invoice pdf
            gInvoiceDetail.ContextMenuGenericDelegate2 = GridInvoiceDetailEditDelegate;
            gInvoiceDetail.ContextMenuGenericDisplayName2 = "Edit Invoice Lines";
            gInvoiceDetail.ContextMenuGenericIsVisible2 = true;
           
            //GridCustomerDocumentDetail.IsFilterable = true;

            //GridCustomerDocumentDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "apply_to_doc", "seq_code" }, ChildGrids = { GridCustomerDocumentApplied }, ParentFilterOnColumnNames = { "document_id", "seq_code" } });




            GridCollection.Add(gInvoiceDetail);

        }

        public void GridInvoiceDetailRevRecDelegate()
        {
            //call invoice revenue recognition screen
            this.GetInvoiceRevRecInfoToPass();


        }
        public void GetInvoiceRevRecInfoToPass()
        {

            cBaseBusObject InvoiceRevRecObj = new cBaseBusObject();

            InvoiceRevRecObj.BusObjectName = "InvoiceRevRec";
            //gInvoiceDetail.ReturnSelectedData("invoice_number");
            string invoice_number_to_pass = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();
            InvoiceRevRecObj.Parms.AddParm("@invoice_number", invoice_number_to_pass);
            //if (cGlobals.ReturnParms.Count > 0)
            //{
                //get invoice_number
                //string invoice_number_to_pass = cGlobals.ReturnParms[0].ToString();
                //InvoiceRevRecObj.Parms.AddParm("@invoice_number", invoice_number_to_pass);
            //}
            //else
            //{
            //    Messages.ShowInformation("No Parms Returned");

            //}
           
            if (InvoiceRevRecObj != null)
            {
                //show the product rules screen
                callInvoiceRevRecScreen(InvoiceRevRecObj);
            }
            else
            {
                Messages.ShowInformation("Problem Opening Invoice Revenue Recognition Screen");
            }

        }
        private void callInvoiceRevRecScreen(cBaseBusObject InvoiceRevRecObj)
        {

            //tell the rules screen it is inserting if adding new record
            InvoiceRevRecDetail invoiceAcctScreen = new InvoiceRevRecDetail(InvoiceRevRecObj);
            System.Windows.Window invoiceScreenWindow = new System.Windows.Window();
            invoiceScreenWindow.Title = "Invoice Revenue Recognition Detail ";              
            //set rules screen as content of new window
            invoiceScreenWindow.Content = invoiceAcctScreen;
            //open new window with embedded user control
            invoiceScreenWindow.ShowDialog();
        }

        public void GridInvoiceDetailEditDelegate()
        {
            //call invoice revenue recognition screen
            this.GetInvoiceEditInfoToPass();


        }
        public void GetInvoiceEditInfoToPass()
        {

            cBaseBusObject InvoiceEditObj = new cBaseBusObject();

            InvoiceEditObj.BusObjectName = "InvoiceEdit";
            //gInvoiceDetail.ReturnSelectedData("invoice_number");
            string invoice_number_to_pass = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();
            InvoiceEditObj.Parms.AddParm("@invoice_number", invoice_number_to_pass);
            
            if (InvoiceEditObj != null)
            {
                //validate the invoice can be edited (no discount or different tier by and bill by)
                InvoiceEditObj.LoadTable("validate");
                if (InvoiceEditObj.ObjectData.Tables["validate"] == null || InvoiceEditObj.ObjectData.Tables["validate"].Rows.Count < 1)
                    Messages.ShowInformation("Problem Validating Invoice for Edit");
                else
                {
                    if (Convert.ToInt32(InvoiceEditObj.ObjectData.Tables["validate"].Rows[0]["isvalid"]) > 0)
                        Messages.ShowInformation("Invoice not eligible to edit due having a discount or tier by different than bill by");
                    else
                        //show the product rules screen
                        callInvoiceEditScreen(InvoiceEditObj);
                }
                ////show the product rules screen
                //callInvoiceEditScreen(InvoiceEditObj);
            }
            else
            {
                Messages.ShowInformation("Problem Opening Invoice Edit Screen");
            }

        }
        private void callInvoiceEditScreen(cBaseBusObject InvoiceEditObj)
        {

            //tell the rules screen it is inserting if adding new record
            InvoiceEditDetail invoiceAcctScreen = new InvoiceEditDetail(InvoiceEditObj);
            System.Windows.Window invoiceScreenWindow = new System.Windows.Window();
            invoiceScreenWindow.Title = "Invoice Line Detail To Edit ";
            //set rules screen as content of new window
            invoiceScreenWindow.Content = invoiceAcctScreen;
            //open new window with embedded user control
            invoiceScreenWindow.ShowDialog();
        }

        public void GridDoubleClickDelegate()
        {
            //call invoiceactruletier detail screen
            this.GetInvoiceLineInfoToPass();         
            


        } 
        public void GetInvoiceLineInfoToPass()
        {
            
            cBaseBusObject InvoiceAcctRuleTierObj = new cBaseBusObject();
            //add invoice number and inv_line_id to bus obj parm table
            
             
            InvoiceAcctRuleTierObj.BusObjectName = "InvoiceAcctRuleTier";
            gInvoiceDetail.ReturnSelectedData("invoice_number");
            if (cGlobals.ReturnParms.Count > 0)
            {
                //get invoice_number
                string invoice_number_to_pass = cGlobals.ReturnParms[0].ToString();
                InvoiceAcctRuleTierObj.Parms.AddParm("@invoice_number", invoice_number_to_pass);
            }
            else
            {
                Messages.ShowInformation("No Invoice Line Selected");
                 
            }
            gInvoiceDetail.ReturnSelectedData("inv_line_id");
            if (cGlobals.ReturnParms.Count > 0)
            {
                
                {
                    //get inv_line_id
                    string inv_line_id_to_pass = cGlobals.ReturnParms[0].ToString();
                    InvoiceAcctRuleTierObj.Parms.AddParm("@inv_line_id", inv_line_id_to_pass);
                }
            }
            else
            {
                Messages.ShowInformation("No Invoice Line Selected");
                 
            }

            if (InvoiceAcctRuleTierObj != null)
            {
                //show the product rules screen
                callInvoiceAcctRuleTierScreen(InvoiceAcctRuleTierObj);
            }
            else
            {
                Messages.ShowInformation("Problem Opening Invoice Accounting Tier Rule Detail Screen");
            }

        }

        #endregion
        private void callInvoiceAcctRuleTierScreen(cBaseBusObject InvoiceAcctRuleTierObj)
        {

            //tell the rules screen it is inserting if adding new record
            InvoiceAcctRuleTierDetail invoiceAcctScreen = new InvoiceAcctRuleTierDetail(InvoiceAcctRuleTierObj);
            System.Windows.Window invoiceScreenWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            invoiceScreenWindow.Title = "Invoice Accounting Tier Rule Detail ";
            //invoiceScreenWindow.MaxHeight = 1280;
            //invoiceScreenWindow.MaxWidth = 1024;
            /////////////////////////////////////////////////////
            //set rules screen as content of new window
            invoiceScreenWindow.Content = invoiceAcctScreen;
            //open new window with embedded user control
            invoiceScreenWindow.ShowDialog();
        }
     
        #endregion

    }
    #endregion

}
