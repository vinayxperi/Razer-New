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
   
    /// Interaction logic for ScheduleBillPosting.xaml
 
    public partial class ScheduleBillPosting : ScreenBase, IScreen 
    {
        private static readonly string fieldLayoutResource = "BillBatchesToPost";
        private static readonly string mainTableName = "scheduleBillPosting";
        public static readonly string jobName = "POSTING";
        

       

        public string WindowCaption { get { return string.Empty; } }
    
    
        public ScheduleBillPosting()
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
            gBillBatchesToRun.WindowZoomDelegate = ReturnSelectedData;
            gBillBatchesToRun.xGrid.FieldLayoutSettings = layouts;
            gBillBatchesToRun.FieldLayoutResourceString = fieldLayoutResource;
            gBillBatchesToRun.MainTableName = "billBatchesAvail";
            this.MainTableName = mainTableName;


            gBillBatchesOverride.FieldLayoutResourceString = "BillBatchesOverride";
            gBillBatchesOverride.ConfigFileName = "billbatchesoverridedate";
            gBillBatchesOverride.MainTableName = "duedate";
            this.GridCollection.Add(gBillBatchesOverride);
            this.CurrentBusObj = businessObject;
            this.Load(businessObject);
            if (this.CurrentBusObj.HasObjectData)
            {

                //Security User listbox

                //this.lkUser.BindingObject = EstablishListObjectBinding(this.CurrentBusObj.GetTable("email_user") as DataTable, true, "user_name",
                //    "user_id", "Select Business User");
                //this.lkUser.SelectedValue = cGlobals.UserName;
                cmbUser.SetBindingExpression(userValueField, userDisplayField, this.CurrentBusObj.ObjectData.Tables["emailUser"]);
                cmbUser.SelectedValue = cGlobals.UserName;

                //cmbUser.SelectedText = 


            }
            else
            {
                Messages.ShowWarning("No Batches available to Post.");
                return;
            }
 
             
             
            gBillBatchesToRun.SetGridSelectionBehavior(false, true);
            gBillBatchesToRun.LoadGrid(businessObject, gBillBatchesToRun.MainTableName);
              

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
            DateTime dtConvert = new DateTime();
            DateTime dtInv = new DateTime();
            string sinvDate;
             

            sinvDate = ldInvoiceDate.SelText.ToString();
            dtConvert = Convert.ToDateTime(ldInvoiceDate.SelText);
            sinvDate = dtConvert.Month.ToString() + "-" + dtConvert.Day.ToString() + "-" + dtConvert.Year.ToString();
           
            string sParms = "";

            if (sinvDate == "" || sinvDate == "01/01/2001")  
            {
                Messages.ShowWarning("Invoice Date to Print on Invoice is Required!");
                ldInvoiceDate.Focus();
            }
            else
            {
                dtInv = Convert.ToDateTime(ldInvoiceDate.SelText);
                     dt = ldtDatetoSchedule.SelText;

                MessageBoxResult resultdt = Messages.ShowYesNo("The date to print on invoices is " + sinvDate + ".  Is this the correct date?",
                    System.Windows.MessageBoxImage.Question);
                if (resultdt == MessageBoxResult.Yes)
                {
                }

                else
                {
                    ldInvoiceDate.Focus();
                    return;
                }
                MessageBoxResult result = Messages.ShowYesNo("Schedule Job " + jobName.ToString() + " for " + ldtDatetoSchedule.SelText,
                    System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //Use Linq to see what batches will be included.  String together batch ids together in parms



                    DataTable dtbatches = this.CurrentBusObj.ObjectData.Tables["billBatchesAvail"];

                    var testResults = (from parms in dtbatches.AsEnumerable()
                                       where parms.Field<int>("include_in_batch") == 1
                                       select parms.Field<int>("batch_id")).ToArray();

                    if (testResults.Count() != 0)
                        sParms = "/A " + string.Join(",", testResults);

                    sParms += " /A " +   sinvDate.ToString();
                    DataRow dorow;

                    if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, sParms, ldtDatetoSchedule.SelText, this.cmbUser.SelectedValue.ToString()) == true)
                    {
                        Messages.ShowWarning("Job Scheduled to Run");
                        //Need to loop through and update the due_date_override table so it can be inserted using robjects INS
                        DataTable dtbatchestopost = this.CurrentBusObj.ObjectData.Tables["billBatchesAvail"];
                        DataTable dtbatchestooverride = this.CurrentBusObj.ObjectData.Tables["duedate"];
                        foreach (DataRow dtrow in dtbatchestopost.Rows)
                        {
                            //check to include
                            if (Convert.ToInt32(dtrow["include_in_batch"]) == 1)
                            {

                                dorow = dtbatchestooverride.NewRow();
                                int columnCount = dtbatchestooverride.Columns.Count;
                                bool rowIsNotModified = (dorow.RowState != DataRowState.Modified);
                                dorow["batch_id"] = dtrow["batch_id"];
                                dorow["immediate_flag"] = dtrow["immediate_flag"];




                                this.CurrentBusObj.ObjectData.Tables["duedate"].Rows.Add(dorow);
                                
                               
                            }
                           
                                
                           
                        }
                        //reject changes in dtbatchestopost so it won't try to save
                        dtbatchestopost.AcceptChanges();

                        this.Save();
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

        private void chkAllBatches_Checked(object sender, RoutedEventArgs e)
        {
            //Loop through and check all batches to include
            DataTable dtbatchestocheck = this.CurrentBusObj.ObjectData.Tables["billBatchesAvail"];
              
            foreach (DataRow dtrow in dtbatchestocheck.Rows)
            {
                //check to include
                dtrow["include_in_batch"] = 1;
            }
            //Loop through and select all rows in the grid
             

        }

        private void chkAllBatches_UnChecked(object sender, RoutedEventArgs e)
        {
            //Loop through and uncheck all batches
            //Loop through and check all batches to include
            DataTable dtbatchestouncheck = this.CurrentBusObj.ObjectData.Tables["billBatchesAvail"];
           
            foreach (DataRow dtrow in dtbatchestouncheck.Rows)
            {
                //uncheck 
                dtrow["include_in_batch"] = 0;
            }
        }

        public override void Close()
        {
            
                    
               
        }

        private void ldInvoiceDate_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void cmbUser_Loaded(object sender, RoutedEventArgs e)
        {

        }
        //private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    base.Close();
        //}


        

    }
}
