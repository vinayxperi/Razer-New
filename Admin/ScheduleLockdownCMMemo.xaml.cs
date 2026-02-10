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

    /// Interaction logic for ScheduleLockdownCMMemo.xaml

    public partial class ScheduleLockdownCMMemo : ScreenBase, IScreen 
    {
 
        private static readonly string mainTableName = "ScheduleLockdownCMMemo";  
        public static readonly string jobName = "Lockdown Credit Memo";
        

       

        public string WindowCaption { get { return string.Empty; } }
    
    
        public ScheduleLockdownCMMemo()
            : base()
        {
            InitializeComponent();
        }

     
        public void Init(cBaseBusObject businessObject)
        {
           
           
            this.CanExecuteSaveCommand = false;
            string userValueField = "user_id";
            string userDisplayField = "user_name";
            
            this.MainTableName = mainTableName;
            this.Load(businessObject);
            this.CurrentBusObj = businessObject;
            if (this.CurrentBusObj.HasObjectData)
            {
            
             
                cmbUser.SetBindingExpression(userValueField, userDisplayField, this.CurrentBusObj.ObjectData.Tables["emailUser"]);                
                cmbUser.SelectedValue = cGlobals.UserName;

              
             
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


           if (txtCreditTitle.Text == "")
           {
               Messages.ShowError("Credit History Title is required");
               return;
           }
           if (txtCreditDesc.Text == "")
           {
               Messages.ShowError("Credit History Description is required");
               return;
           }
            sParms = "/A " + '"' + txtCreditTitle.Text.ToString() + '"' +" /A " + '"' + txtCreditDesc.Text.ToString() + '"';
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
        


