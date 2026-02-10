using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Linq;
using Microsoft.VisualBasic;

namespace Customer
{
   
    /// Interaction logic for CustomerEmailInvoice.xaml
 
    public partial class CustomerEmailInvoice : ScreenBase 
    {
        
        private static readonly string mainTableName = "EmailInvoices";
        public string CustomerID = "";
        public cBaseBusObject CustomerEmailInvoiceBusObject = new cBaseBusObject();
       
        //customer object from caller
        cBaseBusObject CustomerObj;
        

       

        public string WindowCaption { get { return string.Empty; } }


        public CustomerEmailInvoice(string _CustomerId, cBaseBusObject _CustObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = CustomerEmailInvoiceBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "EmailInvoices";
            CustomerID = _CustomerId;
            //get handle to contract obj
            CustomerObj = _CustObj;
            InitializeComponent();
            Init();
        }

     
        public void Init()
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //set isscreendirty = false to prevent save message
        //this.IsScreenDirty = false;
            FieldLayoutSettings Insertlayout = new FieldLayoutSettings();
            Insertlayout.AllowAddNew = true;
            Insertlayout.AddNewRecordLocation = AddNewRecordLocation.OnTop;
            
            this.CanExecuteSaveCommand = false;
              this.CurrentBusObj.Parms.ClearParms();
                //if custId passed load external_char_id and recv_acct with passed customerId
           
            this.CurrentBusObj.Parms.AddParm("@receivable_account", CustomerID);
            this.CurrentBusObj.Parms.AddParm("@subject", "");
            this.CurrentBusObj.Parms.AddParm("@body", "");
            this.CurrentBusObj.Parms.AddParm("@addresses", "");
            this.CurrentBusObj.Parms.AddParm("@attachments", "");
            this.CurrentBusObj.Parms.AddParm("@username", "");
            gInvoicesAvailToEmail.WindowZoomDelegate = ReturnSelectedData;
            gInvoicesAvailToEmail.SingleClickZoomDelegate = InvoicestoAttachSingleClickDelegate;
            gInvoicesAvailToEmail.xGrid.FieldLayoutSettings = layouts;
            gInvoicesAvailToEmail.FieldLayoutResourceString = "CustomerEmailInvoices";
            gInvoicesAvailToEmail.SetGridSelectionBehavior(true, true);
            gInvoicesAvailToEmail.DoNotSelectFirstRecordOnLoad = true;
            gInvoicesAvailToEmail.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            gInvoicesAvailToEmail.MainTableName = "emailInvoices";

            gContactsforEmail.WindowZoomDelegate = ReturnSelectedData;
            gContactsforEmail.xGrid.FieldLayoutSettings = Insertlayout;
            gContactsforEmail.xGrid.FieldSettings.AllowEdit = true;
            gContactsforEmail.FieldLayoutResourceString = "CustomerContactstoReceiveEmail";
            gContactsforEmail.WindowZoomDelegate = GridDoubleClickDelegate;
            gContactsforEmail.DoNotSelectFirstRecordOnLoad = true;
            gContactsforEmail.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            gContactsforEmail.MainTableName = "emailContacts";
            
            this.MainTableName = mainTableName;


            
            this.GridCollection.Add(gInvoicesAvailToEmail);
            this.GridCollection.Add(gContactsforEmail);
            this.Load();
            if (this.CurrentBusObj.HasObjectData)
            {
            
                //Security User listbox
               
                //this.lkUser.BindingObject = EstablishListObjectBinding(this.CurrentBusObj.GetTable("email_user") as DataTable, true, "user_name",
                //    "user_id", "Select Business User");
                //this.lkUser.SelectedValue = cGlobals.UserName;
               
                               
                //cmbUser.SelectedText = 

                //this.lkUser.Text = cGlobals.UserName;
             
            }
           
              

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);            
          
        }
        private void InvoicestoAttachSingleClickDelegate()
        {

            //load Rules for selected rate
            if (gInvoicesAvailToEmail.xGrid.ActiveRecord != null)
            {
                
                int ctr = 0;

                foreach (Record record in gInvoicesAvailToEmail.xGrid.SelectedItems.Records)
                {
                    ctr++;
                    DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                    if (Convert.ToInt16(r["email_ind"]) == 0)
                       r["email_ind"] = 1;
                    else
                        if (Convert.ToInt16(r["email_ind"]) == 1)
                            r["email_ind"] = 0;
                }
               
            }

        }

        public void GridDoubleClickDelegate()
        {
            //move the email they double-click on into the email to use column
            string emailToUse = "";
           gContactsforEmail.ReturnSelectedData("email");
           if (cGlobals.ReturnParms.Count > 0)
           {
               emailToUse = cGlobals.ReturnParms[0].ToString();
               this.CurrentBusObj.ObjectData.Tables["emailContacts"].Rows[gContactsforEmail.xGrid.ActiveRecord.Index]["email_to_use"] = emailToUse.ToString();


           }
             


        }

       
        /// Handler for double click  
        public void ReturnSelectedData()
        {

          


        }

        
        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentBusObj.Parms.ClearParms();
           
            
            if (txtSubject.Text == "")
            {
                Messages.ShowWarning("Email Subject is required!");
                txtSubject.Focus();
                return;
            }
            else
            {
                 //CurrentBusObj.ObjectData.Tables["ParmTable"].Rows[1]["parmValue"] = txtSubject.Text;
                CurrentBusObj.Parms.AddParm("@subject", txtSubject.Text);

                  }
            if (txtBody.Text == "")
            {
                Messages.ShowWarning("Email Body is required!");
                txtBody.Focus();
                return;
            }
            else
            {

                CurrentBusObj.Parms.AddParm("@body", txtBody.Text);
            }
            
           //loop through and concatenate email addresses
          
            DataTable dtemailaddresses = this.CurrentBusObj.ObjectData.Tables["emailContacts"];
            string addresses = "";
            foreach (DataRow dtrow in dtemailaddresses.Rows)
            {
                //check to include
                if (dtrow["email_to_use"].ToString() == "")
                {
                }
                else
                {
                    if (addresses.ToString() =="")
                        addresses = dtrow["email_to_use"].ToString();
                    else

                        addresses = addresses + ";" + dtrow["email_to_use"].ToString();
                }

            }
            if (addresses.ToString() == "")
            {
                Messages.ShowWarning("Email Address to Use is required!");
                return;
            }
            else
            {
               
               
               CurrentBusObj.Parms.AddParm("@addresses",   addresses.ToString());
            }


            //get list of attachments
            string attachments = "";
            //First loop through and make sure the email_ind is set correctly
            foreach (Record record in gInvoicesAvailToEmail.xGrid.Records)
            {
                 
                DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                 
                    r["email_ind"] = 0 ;
               
            }
            //Now reset indicator if selected row
            foreach (Record record in gInvoicesAvailToEmail.xGrid.SelectedItems.Records)
            {
                 
                DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                r["email_ind"] = 1;
                
            }
            DataTable dtinvoices = this.CurrentBusObj.ObjectData.Tables["emailInvoices"];
            foreach (DataRow dtrow in dtinvoices.Rows)
            
            {

                //check to include
                if (Convert.ToInt32(dtrow["email_ind"]) == 0)
                {
                }
                else
                {
                    string ls_temp_attach = "";
                    string slash = "\\";
                        if (dtrow["server_loc"].ToString().Substring(0,2) == "\\\\")
                          ls_temp_attach = dtrow["server_loc"].ToString() + dtrow["directory"].ToString().Trim() + slash.ToString() + dtrow["filename"].ToString().Trim();
                        else
                            ls_temp_attach = "\\\\" + dtrow["server_loc"].ToString() + dtrow["directory"].ToString().Trim() + slash.ToString() +  dtrow["filename"].ToString().Trim();
                        if (attachments.ToString() == "")
                        {
                            attachments = ls_temp_attach.ToString();
                        }
                        else
                        {

                            attachments = attachments + ";" + ls_temp_attach.ToString();
                        }
                }

            }
            if (attachments.ToString() == "")
            {
                Messages.ShowWarning("No invoices selected to include as attachments");
                return;
            }
            else
            {
                
                CurrentBusObj.Parms.AddParm("@attachments",  attachments.ToString());
            }
            CurrentBusObj.Parms.AddParm("@username", cGlobals.UserName);
           
           CurrentBusObj.LoadTable("sendEmail");
           CloseScreen();
          
           



        }

        private void CloseScreen()
        {

            System.Windows.Window CustomerParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                CustomerParent.Close();
            }
        }
        private void chkAllInvoices_UnChecked(object sender, RoutedEventArgs e)
        {
            //Loop through and uncheck all batches
            //Loop through and check all batches to include
            DataTable dtinvoicestouncheck = this.CurrentBusObj.ObjectData.Tables["emailInvoices"];
           
            foreach (DataRow dtrow in dtinvoicestouncheck.Rows)
            {
                //uncheck 
                dtrow["email_ind"] = 0;
            }
        }

        public override void Close()
        {
            
                    
               
        }

        


        

    }
}
