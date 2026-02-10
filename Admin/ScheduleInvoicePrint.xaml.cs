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

namespace Admin
{

    /// Interaction logic for ScheduleInvoicePrint.xaml

    public partial class ScheduleInvoicePrint : ScreenBase, IScreen 
    {
 
        private static readonly string mainTableName = "ScheduleInvoicePrint";  
        public static readonly string jobName = "INVGNRTR";
        

       

        public string WindowCaption { get { return string.Empty; } }
    
    
        public ScheduleInvoicePrint()
            : base()
        {
            InitializeComponent();
        }

     
        public void Init(cBaseBusObject businessObject)
        {
           
            this.CanExecuteSaveCommand = false;
            string userValueField = "user_id";
            string userDisplayField = "user_name";
            //RES 2/12/15 Company Consolidation Project select invoices to print by billing owner instead of company
            string billingValueField = "billing_owner_id";
            string billingDisplayField = "billing_owner_name";
            //string companyValueField = "company_code";
            //string companyDisplayField = "company_description";
            string printerValueField = "server_location";
            string printerDisplayField = "printer_name";

            this.MainTableName = mainTableName;
            this.Load(businessObject);
            this.CurrentBusObj = businessObject;
            if (this.CurrentBusObj.HasObjectData)
            {
            
      
                cmbUser.SetBindingExpression(userValueField, userDisplayField, this.CurrentBusObj.ObjectData.Tables["emailUser"]);                
                cmbUser.SelectedValue = cGlobals.UserName;
                //RES 2/12/15 Company Consolidation Project select invoices to print by billing owner instead of company
                //cmbCompany.SetBindingExpression(companyValueField, companyDisplayField, this.CurrentBusObj.ObjectData.Tables["cbCompany"]);
                cmbBillingOwner.SetBindingExpression(billingValueField, billingDisplayField, this.CurrentBusObj.ObjectData.Tables["billing_owner"]);
                cmbBillingOwner.SelectedValue = cGlobals.UserName;
                cmbPrinters.SetBindingExpression(printerValueField, printerDisplayField, this.CurrentBusObj.ObjectData.Tables["cbPrinters"]);   
             
            }
            
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);            
          
        }

        
 
       
        /// Handler for double click on grid to return the app selected to run
        public void ReturnSelectedData()
        {

            //gCashBatchesToRun.ReturnSelectedData();
            //if (cGlobals.ReturnParms.Count > 0)
            //{
            //    jobName = cGlobals.ReturnParms[0].ToString();
                 
            //}



        }

        // Schedule the job
        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DateTime dt = new DateTime();
           

           string sParms = "";

           //RES 2/12/15 Company Consolidation Project select invoices to print by billing owner instead of company
           if (cmbBillingOwner.SelectedText == "")
               Messages.ShowError("Billing owner must be Selected!");
           //if (cmbCompany.SelectedText == "")
           //    Messages.ShowError("Company must be Selected!");
           else
           {
               sParms = "/A " + cmbBillingOwner.SelectedValue.ToString();
               //sParms = "/A " + cmbCompany.SelectedValue.ToString();

               if (cmbPrinters.SelectedText == "")
                   Messages.ShowError("Printer must be Selected!");
               else
               {
                   sParms = sParms + " /A " + cmbPrinters.SelectedValue.ToString();

                   dt = ldtDatetoSchedule.SelText;


                   MessageBoxResult result = Messages.ShowYesNo("Schedule Job " + jobName.ToString() + " for " + ldtDatetoSchedule.SelText,
                       System.Windows.MessageBoxImage.Question);
                   if (result == MessageBoxResult.Yes)
                   {
                       if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, sParms, ldtDatetoSchedule.SelText, this.cmbUser.SelectedValue.ToString()) == true)
                       {
                           Messages.ShowWarning("Job Scheduled to Run");
                           this.CallScreenClose();
                       }
                       else
                           Messages.ShowWarning("Error Scheduling Job");
                   }

                   else
                   {

                       Messages.ShowMessage("Job Not Scheduled", MessageBoxImage.Information);
                   }


               }
           }
                    
        }

       
        public override void Close()
        {
            
                    
               
        }

       

        

    }
}
