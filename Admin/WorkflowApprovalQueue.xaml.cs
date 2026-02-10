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
   
    /// Interaction logic for WorkflowApprovalQueue
 
    public partial class WorkflowApprovalQueue : ScreenBase, IScreen 
    {
        private static readonly string fieldLayoutResource = "workflowApprovalQueue";
        private static readonly string mainTableName = "WorkflowQueue";



        public string DocumentID = "";
        public string WindowCaption { get { return string.Empty; } }
        

        public WorkflowApprovalQueue()
            : base()
        {
            InitializeComponent();
        }

     
        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            //layouts.HighlightAlternateRecords = true;
            
             
            //Set up generic context menu selections
            gWorkflowApprovalQueue.ContextMenuGenericDelegate1 = ContextMenuApproveDocuments;
            gWorkflowApprovalQueue.ContextMenuGenericDisplayName1 = "Approve Selected Documents";
            gWorkflowApprovalQueue.ContextMenuGenericIsVisible1 = true;
            
            //add delegates to be enabled
            gWorkflowApprovalQueue.ContextMenuAddDelegate = ContextMenuViewMultiCustomers;
            //Set up generic context menu selections
            gWorkflowApprovalQueue.ContextMenuGenericDelegate2 = ContextMenuViewMultiCustomers;
            gWorkflowApprovalQueue.ContextMenuGenericDisplayName2 = "View Multiple Customers";
            gWorkflowApprovalQueue.ContextMenuGenericIsVisible2 = true;
            gWorkflowApprovalQueue.ContextMenuSaveToExcelIsVisible = true;
            gWorkflowApprovalQueue.ContextMenuSaveGridSettingsIsVisible = true;
            gWorkflowApprovalQueue.ContextMenuResetGridSettingsIsVisible = true;
            //add delegates to be enabled
            gWorkflowApprovalQueue.ContextMenuAddDelegate = ContextMenuViewMultiCustomers;
            //disable add Record
            gWorkflowApprovalQueue.ContextMenuAddIsVisible = false;
           
            //gAdjustmentQueue.xGrid.FieldLayoutSettings = layouts;
            
            gWorkflowApprovalQueue.FieldLayoutResourceString = fieldLayoutResource;

            //Add this code to use a different style for the grid than the default.
            //This particular style is used for color coded grids.
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            gWorkflowApprovalQueue.GridCellValuePresenterStyle = CellStyle;
            gWorkflowApprovalQueue.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;

            gWorkflowApprovalQueue.ConfigFileName = "WorkflowQueueConfig";
            gWorkflowApprovalQueue.MainTableName = "workflowApprovalQueue";
            gWorkflowApprovalQueue.WindowZoomDelegate = GridDoubleClickDelegate;
            gWorkflowApprovalQueue.DoNotSelectFirstRecordOnLoad = true;
            this.MainTableName = mainTableName;

            this.Load(businessObject);
            this.CurrentBusObj = businessObject;

            gWorkflowApprovalQueue.SetGridSelectionBehavior(true, true);
            gWorkflowApprovalQueue.LoadGrid(businessObject, gWorkflowApprovalQueue.MainTableName);
            GridCollection.Add(gWorkflowApprovalQueue);  

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);            
          
        }

        private void ContextMenuApproveDocuments()
        {
             
         //move selected rows to datatable to pass.
            if (this.CurrentBusObj.ObjectData.Tables["workflowApprovalQueue"].Rows.Count != 0)
            {
                {
                    DataTable dtToPass = null;
                    
                    foreach (Record record in gWorkflowApprovalQueue.xGrid.SelectedItems.Records)
                    {
                        DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                        if (row["document_id"].ToString().StartsWith("T"))
                            Messages.ShowWarning("TF can only be approved from TF folder!");
                        else
                        {
                            if (dtToPass == null)
                            {
                                dtToPass = row.Table.Clone();
                            }

                            dtToPass.ImportRow(row);
                        }
                    }
                    //DWR Modified 2/1/13-- Included check for null dtToPass as it will be null if now rows are selecetd
                    if (dtToPass!=null && dtToPass.Rows.Count != 0)
                    {

                        if (cGlobals.BillService.WorkflowMultiApproval(dtToPass, cGlobals.UserName.ToString()) == "")
                        {
                            Messages.ShowInformation("Approval Process Completed!");
                            this.Load(CurrentBusObj);
                            

                            gWorkflowApprovalQueue.SetGridSelectionBehavior(true, true);
                            gWorkflowApprovalQueue.LoadGrid(CurrentBusObj, gWorkflowApprovalQueue.MainTableName);
                        }
                        else
                            Messages.ShowWarning("Error Approving Multiple Documents");
                    }
                    else
                        Messages.ShowWarning("No Selected Rows to Approve!");


                    }

            }   
        }
      
        private void ContextMenuViewMultiCustomers()
        {
            //check to see if active row has MULTI in the recv_account
            //if so, based on the document id, bring up a popup window with the customer id and names for that document
            cGlobals.ReturnParms.Clear();
            gWorkflowApprovalQueue.ReturnSelectedData("account_name");
            //check if value is MULTI
            string sAcct = " ";
            if (cGlobals.ReturnParms.Count > 0)
                sAcct = cGlobals.ReturnParms[0].ToString();
            else
            {
                Messages.ShowWarning("No row selected with MULTI to view");
                return;
             }
            if (sAcct.ToString() == "MULTI")
            {
                //clear out the recv account and load the document_id
                cGlobals.ReturnParms.Clear();
                gWorkflowApprovalQueue.ReturnSelectedData("document_id");

                MultiCustomerLookup LookupCustomer = new MultiCustomerLookup();
                LookupCustomer.ShowDialog();

            }
            else
                Messages.ShowWarning("Must be a MULTI document to view Multiple Customer Information");

            
        }
       
        /// Handler for double click on grid to return the app selected to run
        public void ReturnSelectedData()
        {

           //Returns selected data out of the grid
            


        }

        public void GridDoubleClickDelegate()
        {
            ////call adjustment folder - if the document_id begins with ADJ and custom invoice folder if it begins with MINV or MPINV
            gWorkflowApprovalQueue.ReturnSelectedData("document_id");
            
            string documentId = cGlobals.ReturnParms[0].ToString().ToUpper();
            if (documentId.StartsWith("MINV") || documentId.StartsWith("MPINV"))
            {
                cGlobals.ReturnParms.Add("manualinvoice");
            }
            else if (documentId.StartsWith("ADJ"))
            {
                cGlobals.ReturnParms.Add("adjustment");
            }
            else if (documentId.StartsWith("BCF"))
            {
                cGlobals.ReturnParms.Add("BCFFolder");
            }
            else if (documentId.StartsWith("TF"))
            {
                cGlobals.ReturnParms.Add("TFFolder");
            }
            else if (documentId.StartsWith("INV"))
            {
                documentId = documentId.Substring(0, documentId.IndexOf("E"));
                cGlobals.ReturnParms[0] = documentId.ToUpper();
                cGlobals.ReturnParms.Add("btnInvoice");
            }

            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gWorkflowApprovalQueue.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        } 

      
        

    }
}
