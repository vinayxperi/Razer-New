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

namespace Admin
{
   
    /// Interaction logic for ScheduleJob.xaml
 
    public partial class ScheduleCashPosting : ScreenBase, IScreen 
    {
        private static readonly string fieldLayoutResource = "CashBatchesToPost";
        private static readonly string mainTableName = "scheduleCashPosting";
        public static readonly string jobName = "CASHPOST";
        

       

        public string WindowCaption { get { return string.Empty; } }
    
    
        public ScheduleCashPosting()
            : base()
        {
            InitializeComponent();
        }

     
        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //set isscreendirty = false to prevent save message
        //this.IsScreenDirty = false;
           
            this.CanExecuteSaveCommand = false;
            string userValueField = "user_id";
            string userDisplayField = "user_name";
            gCashBatchesToRun.WindowZoomDelegate = ReturnSelectedData;
            gCashBatchesToRun.xGrid.FieldLayoutSettings = layouts;
            gCashBatchesToRun.FieldLayoutResourceString = fieldLayoutResource;
            gCashBatchesToRun.MainTableName = "cashBatchesAvail";
            this.MainTableName = mainTableName;

            this.Load(businessObject);
            this.CurrentBusObj = businessObject;
            if (this.CurrentBusObj.HasObjectData)
            {
            
                //Security User listbox
               
                //this.lkUser.BindingObject = EstablishListObjectBinding(this.CurrentBusObj.GetTable("email_user") as DataTable, true, "user_name",
                //    "user_id", "Select Business User");
                //this.lkUser.SelectedValue = cGlobals.UserName;
                cmbUser.SetBindingExpression(userValueField, userDisplayField, this.CurrentBusObj.ObjectData.Tables["emailUser"]);                
                cmbUser.SelectedValue = cGlobals.UserName;
                               
                //cmbUser.SelectedText = 

                //this.lkUser.Text = cGlobals.UserName;
             
            }
            gCashBatchesToRun.SetGridSelectionBehavior(false, true);
            gCashBatchesToRun.LoadGrid(businessObject, gCashBatchesToRun.MainTableName);
              

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

          
                dt = ldtDatetoSchedule.SelText;
            
                MessageBoxResult result = Messages.ShowYesNo("Schedule Job " + jobName.ToString() + " for " + ldtDatetoSchedule.SelText,
                    System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //Use Linq to see what batches will be included.  String together batch ids together in parms
                   
                   

                    DataTable dtbatches = this.CurrentBusObj.ObjectData.Tables["cashBatchesAvail"];
                   
                    var testResults = (from parms in dtbatches.AsEnumerable()
                                       where parms.Field<int>("batch_status") == 1
                                       select parms.Field<int>("batch_id")).ToArray();

                    if (testResults.Count() != 0)
                        sParms = "/A " + string.Join(",", testResults);


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

        private void chkAllBatches_Checked(object sender, RoutedEventArgs e)
        {
            //Loop through and check all batches to include
            DataTable dtbatchestocheck = this.CurrentBusObj.ObjectData.Tables["cashBatchesAvail"];
           
            foreach (DataRow dtrow in dtbatchestocheck.Rows)
            {
                //check to include
                 dtrow["batch_status"] = 1;
            }
            //Loop through and select all rows in the grid

        }

        private void chkAllBatches_UnChecked(object sender, RoutedEventArgs e)
        {
            //Loop through and uncheck all batches
            //Loop through and check all batches to include
            DataTable dtbatchestouncheck = this.CurrentBusObj.ObjectData.Tables["cashBatchesAvail"];
           
            foreach (DataRow dtrow in dtbatchestouncheck.Rows)
            {
                //uncheck 
                dtrow["batch_status"] = 0;
            }
        }

        public override void Close()
        {
            
                    
               
        }
        //private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    base.Close();
        //}


        

    }
}
