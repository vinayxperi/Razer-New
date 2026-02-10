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

namespace Razer.CustomInvoice
{
   
    /// Interaction logic for Razer.CustomInvoice
 
    public partial class UnpostedCustomInvoices : ScreenBase, IScreen 
    {
        private static readonly string fieldLayoutResource = "UnpostedCustomInvoices";
        private static readonly string mainTableName = "unpostedCustom";
        private static readonly string dataKey = "description";

       

        public string WindowCaption { get { return string.Empty; } }
        public string jobName = "Custom Invoice Posting";

        public UnpostedCustomInvoices()
            : base()
        {
            InitializeComponent();
        }

     
        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            
             
            //Set up generic context menu selections
            gUnpostedCustomInvoices.ContextMenuGenericDelegate1 = ContextMenuScheduleJob;
            gUnpostedCustomInvoices.ContextMenuGenericDisplayName1 = "Schedule Custom Invoice Posting";
            gUnpostedCustomInvoices.ContextMenuGenericIsVisible1 = true;


            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {

                gUnpostedCustomInvoices.ContextMenuGenericIsVisible1 = false;

            }
            
            //add delegates to be enabled
            gUnpostedCustomInvoices.ContextMenuAddDelegate = ContextMenuScheduleJob;
            //disable add Record
            gUnpostedCustomInvoices.ContextMenuAddIsVisible = false;
           
            //gAdjustmentQueue.xGrid.FieldLayoutSettings = layouts;
            gUnpostedCustomInvoices.FieldLayoutResourceString = fieldLayoutResource;
            gUnpostedCustomInvoices.MainTableName = mainTableName;
            gUnpostedCustomInvoices.WindowZoomDelegate = GridDoubleClickDelegate;
            this.MainTableName = mainTableName;

            this.Load(businessObject);
            this.CurrentBusObj = businessObject;

            gUnpostedCustomInvoices.SetGridSelectionBehavior(true, false);
            gUnpostedCustomInvoices.LoadGrid(businessObject, gUnpostedCustomInvoices.MainTableName);
              

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);            
          
        }

        private void ContextMenuScheduleJob()
        {
             
           
            if (this.CurrentBusObj.ObjectData.Tables["unpostedCustom"].Rows.Count != 0)
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
 
       
        /// Handler for double click on grid to return the app selected to run
        public void ReturnSelectedData()
        {

            gUnpostedCustomInvoices.ReturnSelectedData(dataKey);
            if (cGlobals.ReturnParms.Count > 0)
            {
                
            }



        }

        public void GridDoubleClickDelegate()
        {
            //call custom invoice folder
            gUnpostedCustomInvoices.ReturnSelectedData("invoice_number");
            cGlobals.ReturnParms.Add("gUnpostedCustomInvoices.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gUnpostedCustomInvoices.xGrid; 
            EventAggregator.GeneratedClickHandler(this, args);


        } 

         

        

    }
}
