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

namespace Admin
{
   
    /// Interaction logic for ScheduleJob.xaml
 
    public partial class ScheduleJob : ScreenBase, IScreen 
    {
        private static readonly string fieldLayoutResource = "batchjobs";
        private static readonly string mainTableName = "schedule_job";
        private static readonly string dataKey = "description";

       

        public string WindowCaption { get { return string.Empty; } }
        public string jobName = " ";

        public ScheduleJob()
            : base()
        {
            InitializeComponent();
        }

     
        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            string userValueField = "user_id";
            string userDisplayField = "user_name";

            gBatchJobsToRun.WindowZoomDelegate = ReturnSelectedData;
            gBatchJobsToRun.xGrid.FieldLayoutSettings = layouts;
            gBatchJobsToRun.FieldLayoutResourceString = fieldLayoutResource;
            gBatchJobsToRun.MainTableName = mainTableName;
            this.MainTableName = mainTableName;

            this.Load(businessObject);
            this.CurrentBusObj = businessObject;
            if (this.CurrentBusObj.HasObjectData)
            {
            
                //Security User listbox
               
                //this.lkUser.BindingObject = EstablishListObjectBinding(this.CurrentBusObj.GetTable("email_user") as DataTable, true, "user_name",
                //    "user_id", "Select Business User");
                //this.lkUser.SelectedValue = cGlobals.UserName;
                cmbUser.SetBindingExpression(userValueField, userDisplayField, this.CurrentBusObj.ObjectData.Tables["email_user"]);                
                cmbUser.SelectedValue = cGlobals.UserName;
                               
                //cmbUser.SelectedText = 

                //this.lkUser.Text = cGlobals.UserName;
             
            }
            gBatchJobsToRun.SetGridSelectionBehavior(true, true);
            gBatchJobsToRun.LoadGrid(businessObject, gBatchJobsToRun.MainTableName);
              

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);            
          
        }

        
 
       
        /// Handler for double click on grid to return the app selected to run
        public void ReturnSelectedData()
        {

            gBatchJobsToRun.ReturnSelectedData(dataKey);
            if (cGlobals.ReturnParms.Count > 0)
            {
                jobName = cGlobals.ReturnParms[0].ToString();
                txtJobtoRun.Text = jobName;
            }



        }

        // Schedule the job
        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DateTime dt = new DateTime();
                    

          
                dt = ldtDatetoSchedule.SelText;
                if (jobName.ToString() == " ")
                    Messages.ShowWarning("No Job Selected to Run!");
                else
                {

                    MessageBoxResult result = Messages.ShowYesNo("Schedule Job " + jobName.ToString() + " for " + ldtDatetoSchedule.SelText,
                        System.Windows.MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, "", ldtDatetoSchedule.SelText, this.cmbUser.SelectedValue.ToString()) == true)
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

        //private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    base.Close();
        //}


        

    }
}
