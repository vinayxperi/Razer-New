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

namespace Adjustment
{
   
    /// Interaction logic for AdjustmentQueue
 
    public partial class AdjustmentQueue : ScreenBase, IScreen 
    {
        private static readonly string fieldLayoutResource = "adjustmentQueue";
        private static readonly string mainTableName = "adjustmentQueue";
        private static readonly string dataKey = "description";

       

        public string WindowCaption { get { return string.Empty; } }
        public string jobName = "ADJUSTMENT POSTING";

        public AdjustmentQueue()
            : base()
        {
            InitializeComponent();
        }

     
        public void Init(cBaseBusObject businessObject)
        {
            //this.canExecuteRefreshCommand = true;

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = false;
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;
             
            //Set up generic context menu selections
            gAdjustmentQueue.ContextMenuGenericDelegate1 = ContextMenuScheduleJob;
            gAdjustmentQueue.ContextMenuGenericDisplayName1 = "Schedule Adjustment Posting";
            gAdjustmentQueue.ContextMenuGenericIsVisible1 = true;
            //add delegates to be enabled
            gAdjustmentQueue.ContextMenuAddDelegate = ContextMenuRefresh;
            //Set up generic context menu selections
            gAdjustmentQueue.ContextMenuGenericDelegate2 = ContextMenuRefresh;
            gAdjustmentQueue.ContextMenuGenericDisplayName2 = "Refresh Queue";
            gAdjustmentQueue.ContextMenuGenericIsVisible2 = true;
            
            //add delegates to be enabled
            gAdjustmentQueue.ContextMenuAddDelegate = ContextMenuScheduleJob;
            //disable add Record
            gAdjustmentQueue.ContextMenuAddIsVisible = false;
           
            //gAdjustmentQueue.xGrid.FieldLayoutSettings = layouts;
            gAdjustmentQueue.ConfigFileName = "AdjustmentQueueConfig";
            gAdjustmentQueue.FieldLayoutResourceString = fieldLayoutResource;
            //Add this code to use a different style for the grid than the default.
            //This particular style is used for color coded grids.
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            gAdjustmentQueue.GridCellValuePresenterStyle = CellStyle;
            gAdjustmentQueue.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            gAdjustmentQueue.DoNotSelectFirstRecordOnLoad = true;
            gAdjustmentQueue.MainTableName = mainTableName;
            gAdjustmentQueue.WindowZoomDelegate = GridDoubleClickDelegate;
            this.MainTableName = mainTableName;

            this.Load(businessObject);
            this.CurrentBusObj = businessObject;
            
            gAdjustmentQueue.SetGridSelectionBehavior(true, false);
            gAdjustmentQueue.LoadGrid(businessObject, gAdjustmentQueue.MainTableName);

            GridCollection.Add(gAdjustmentQueue);


            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);            
          
        }

        private void ContextMenuScheduleJob()
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            //Find the current batch id
            if (this.CurrentBusObj.ObjectData.Tables["adjustmentQueue"].Rows.Count != 0)
            {
                {
                    DateTime dt = new DateTime();
                    dt = DateTime.Now;

                                      
                    if (jobName.ToString() == " ")
                        Messages.ShowWarning("No Job Selected to Run!");
                    else
                    {

                        MessageBoxResult result = Messages.ShowYesNo("Schedule Job " + jobName.ToString() + " for " + dt, 
                            System.Windows.MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            if (cGlobals.BillService.AdjustmentStatusUpdate())
                            {
                            }
                            else
                             Messages.ShowWarning("Error Setting Status Codes.  May not be any adjustments to post!");
                        

                            if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, "", dt, cGlobals.UserName.ToString()) == true)
                                Messages.ShowWarning("Job Scheduled to Run");
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
        }

        private void ContextMenuRefresh()
        {
            //Refresh the screen
            //cGlobals.ReturnParms.Clear();
            this.Load(CurrentBusObj);


            gAdjustmentQueue.SetGridSelectionBehavior(true, false);
            gAdjustmentQueue.LoadGrid(CurrentBusObj, gAdjustmentQueue.MainTableName);


        }
        /// Handler for double click on grid to return the app selected to run
        public void ReturnSelectedData()
        {

            gAdjustmentQueue.ReturnSelectedData(dataKey);
            if (cGlobals.ReturnParms.Count > 0)
            {
                
            }



        }

        public void GridDoubleClickDelegate()
        {
            //call adjustment folder
            gAdjustmentQueue.ReturnSelectedData("document_id");
            cGlobals.ReturnParms.Add("gAdjustmentQueue.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gAdjustmentQueue.xGrid; 
            EventAggregator.GeneratedClickHandler(this, args);


        } 

        // Schedule the job
        //private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    DateTime dt = new DateTime();
                    

          
        //        dt = ldtDatetoSchedule.SelText;
        //        if (jobName.ToString() == " ")
        //            Messages.ShowWarning("No Job Selected to Run!");
        //        else
        //        {

        //            MessageBoxResult result = Messages.ShowYesNo("Schedule Job " + jobName.ToString() + " for " + ldtDatetoSchedule.SelText,
        //                System.Windows.MessageBoxImage.Question);
        //            if (result == MessageBoxResult.Yes)
        //            {
        //                if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, "", ldtDatetoSchedule.SelText, this.cmbUser.SelectedValue.ToString()) == true)
        //                    Messages.ShowWarning("Job Scheduled to Run");
        //                else
        //                    Messages.ShowWarning("Error Scheduling Job");
        //            }
        //            else
        //            {

        //                Messages.ShowMessage("Job Not Scheduled", MessageBoxImage.Information);
        //            }



        //        }
            
                    
        //}

        //private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    base.Close();
        //}


        

    }
}
