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
using System.Timers;

namespace Admin
{
   
    /// Interaction logic for JobMonitor
 
    public partial class JobMonitor : ScreenBase, IScreen 
    {
        private static readonly string fieldLayoutResource = "jobmonitor";
        private static readonly string mainTableName = "JobMonitor";
        cBaseBusObject obj = null;


        public string DocumentID = "";
        public string WindowCaption { get { return string.Empty; } }
        public int job_id = 0;

        public JobMonitor()
            : base()
        {
            InitializeComponent();
        }

     
        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;

              //Set up generic context menu selections
            gJobSchedToRun.ContextMenuGenericDelegate1 = ContextMenuDeleteJob;
            gJobSchedToRun.ContextMenuGenericDisplayName1 = "Delete Job to Run";
            gJobSchedToRun.ContextMenuGenericIsVisible1 = true;
            
            //add delegates to be enabled
            gJobSchedToRun.ContextMenuAddDelegate = ContextMenuRefresh;
            //Set up generic context menu selections
            gJobSchedToRun.ContextMenuGenericDelegate2 = ContextMenuRefresh;
            gJobSchedToRun.ContextMenuGenericDisplayName2 = "Refresh Monitor";
            gJobSchedToRun.ContextMenuGenericIsVisible2 = true;

            //add delegates to be enabled
            gJobSchedToRun.ContextMenuAddDelegate = ContextMenuDeleteJob;
            //disable add Record
            gJobSchedToRun.ContextMenuAddIsVisible = false;
           
            //gAdjustmentQueue.xGrid.FieldLayoutSettings = layouts;
            gJobSchedToRun.FieldLayoutResourceString = fieldLayoutResource;
            gJobSchedToRun.MainTableName = "jobstorun";

            this.MainTableName = mainTableName;

            //RES 6/26/15 added checkbox to show only current day schedule or entire schedule
            this.CurrentBusObj = businessObject;
            this.CurrentBusObj.Parms.ClearParms();
            this.CurrentBusObj.Parms.AddParm("@show_complete", "0");

            GetLookupVals();

            gJobSchedToRun.SetGridSelectionBehavior(true, true);
            //gJobSchedToRun.LoadGrid(businessObject, gJobSchedToRun.MainTableName);
            //gJobSchedToRun.LoadGrid(obj, gJobSchedToRun.MainTableName);
              
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);            
          
        }

        private void ContextMenuDeleteJob()
        {

            //Find the current batch id
            if (this.CurrentBusObj.ObjectData.Tables["jobstorun"].Rows.Count != 0)
            {
                {
                    gJobSchedToRun.ReturnSelectedData("job_id");
                    job_id = Convert.ToInt32(cGlobals.ReturnParms[0].ToString());
                    if (job_id == 0)
                        Messages.ShowWarning("No job selected to Delete!");
                    else
                    {
                        MessageBoxResult result = Messages.ShowYesNo("Delete Job " + job_id.ToString(),
                        System.Windows.MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            if (cGlobals.BillService.EndJobonMonitor(job_id) == true)
                            {
                                Messages.ShowWarning("Job Deleted Successfully!");
                                this.Load(CurrentBusObj);

                                gJobSchedToRun.SetGridSelectionBehavior(true, true);
                                gJobSchedToRun.LoadGrid(CurrentBusObj, gJobSchedToRun.MainTableName);
                            }


                            else
                                Messages.ShowWarning("Error deleting job");

                        }
                        else


                            Messages.ShowMessage("Job Not Deleted", MessageBoxImage.Information);
                    }



                }

            }
        }
      
        private void ContextMenuRefresh()
        {
            //Refresh the screen
            //cGlobals.ReturnParms.Clear();
            this.Load(CurrentBusObj);
             

            gJobSchedToRun.SetGridSelectionBehavior(true, true);
            gJobSchedToRun.LoadGrid(CurrentBusObj, gJobSchedToRun.MainTableName);

            
        }
       
        /// Handler for double click on grid to return the app selected to run
        public void ReturnSelectedData()
        {

            //gJobSchedToRun.ReturnSelectedData();
            //if (cGlobals.ReturnParms.Count > 0)
            //{
            //    jobName = cGlobals.ReturnParms[0].ToString();

            //}
            


        }

        private void chkComplete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
             GetLookupVals();
        }

        private void GetLookupVals()
        {
            //setup and pass chkbox value to stored proc for all or today's schedule
            int ShowComplete = 0;
            if (Convert.ToBoolean(chkComplete.IsChecked))
            {
                ShowComplete = 1;
            }
  
            this.CurrentBusObj.Parms.UpdateParmValue("@show_complete", ShowComplete.ToString());
            this.CurrentBusObj.LoadTable("jobstorun");
            gJobSchedToRun.LoadGrid(CurrentBusObj, "jobstorun");
    
        }

        //public class DTimer
        //{
        //    private DispatcherTimer timer;
        //    public event Action<int> DoSomething;

        //    private int _timesCalled = 0;

        //    public DTimer()
        //    {
        //        timer = new DispatcherTimer();
        //    }
        //    public void Start(int PeriodInSeconds)
        //    {
        //        timer.Interval = TimeSpan.FromSeconds(PeriodInSeconds);
        //        timer.Tick += timer_Task;
        //        _timesCalled = 0;
        //        timer.Start();
        //    }

        //    public void Stop()
        //    {
        //        timer.Stop();
        //    }
        //    private void timer_Task(object sender, EventArgs e)
        //    {
        //        _timesCalled++;
        //        DoSomething(_timesCalled);

        //    }
        //}

      
        

    }
}
