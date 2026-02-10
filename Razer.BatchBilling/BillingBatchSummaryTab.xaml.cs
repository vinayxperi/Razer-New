

#region using statements
using RazerBase.Interfaces;
using RazerBase;
using Razer.Common;
using RazerBase.Lookups;
using RazerInterface;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
 
#endregion

namespace Razer.BatchBilling
{


    /// <summary>
    /// This class represents a 'BillingBatchSummaryTab' object.
    /// </summary>
    public partial class BillingBatchSummaryTab : ScreenBase
    {

        int BatchID = 0;
        int ContractID = 0;
        private static readonly string dataKey = "batch_id";
        private static readonly string dataKey2 = "contract_id";
        private static readonly string dataKey3 = "receivable_account";
  

        /// <summary>
        /// Create a new instance of a 'BillingBatchSummaryTab' object and call the ScreenBase's constructor.
        /// </summary>
        public BillingBatchSummaryTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
  

   
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "summary";

            //Create the Summary base object
            CurrentBusObj = new cBaseBusObject("BillingBatchFolder");
           
            CurrentBusObj.Parms.ClearParms();


            //Establish the Billing Batch Summary Grid
            gBillingBatchSummary.MainTableName = "summary";
            gBillingBatchSummary.SetGridSelectionBehavior(false, true);
            gBillingBatchSummary.FieldLayoutResourceString = "BillingBatchSummary";
            gBillingBatchSummary.IsFilterable = true;


            gBillingBatchSummary.ContextMenuAddIsVisible = false;
            gBillingBatchSummary.ContextMenuGenericDelegate1 = ContextMenuDeleteBatch;
            gBillingBatchSummary.ContextMenuGenericDisplayName1 = "Delete Batch";
            gBillingBatchSummary.ContextMenuGenericIsVisible1 = true;
            gBillingBatchSummary.ContextMenuGenericDelegate2 = ContextMenuDeleteContract;
            gBillingBatchSummary.ContextMenuGenericDisplayName2 = "Delete Contract from Batch";
            gBillingBatchSummary.ContextMenuGenericIsVisible2 = true;
            gBillingBatchSummary.ContextMenuGenericDelegate3 = ContextMenuDeleteandResubmitBatch;
            gBillingBatchSummary.ContextMenuGenericDisplayName3 = "Delete Batch and Resubmit";
            gBillingBatchSummary.ContextMenuGenericIsVisible3 = true;
            //add delegates to be enabled
            gBillingBatchSummary.ContextMenuAddDelegate = ContextMenuDeleteBatch;
            gBillingBatchSummary.ContextMenuAddDelegate = ContextMenuDeleteContract;
            gBillingBatchSummary.ContextMenuAddDelegate = ContextMenuDeleteandResubmitBatch;
            gBillingBatchSummary.WindowZoomDelegate = GridDoubleClickDelegate;
            gBillingBatchSummary.ConfigFileName = "BillingBatchSummary";
            //Establish the Billing Batch Total Summary Grid
            gBillingBatchSummaryTotals.MainTableName = "summaryTotal";
            gBillingBatchSummaryTotals.SetGridSelectionBehavior(false, true);
            gBillingBatchSummaryTotals.FieldLayoutResourceString = "BillingBatchSummaryTotal";
            gBillingBatchSummaryTotals.ConfigFileName = "BillingBatchSummaryTotal";
            gBillingBatchSummaryTotals.IsFilterable = true;
            gBillingBatchSummaryTotals.WindowZoomDelegate = GridDoubleClickTotalDelegate;

            //Add grid to Collection
            GridCollection.Add(gBillingBatchSummary);
            GridCollection.Add(gBillingBatchSummaryTotals);

        }

        public void GridDoubleClickDelegate()
        {
            if (gBillingBatchSummary.xGrid.ActiveRecord != null)
            {
                Cell activecell = gBillingBatchSummary.xGrid.ActiveCell;
                if (activecell == null)
                {
                }
                else
                {
                    Record activeRecord = gBillingBatchSummary.xGrid.Records[gBillingBatchSummary.ActiveRecord.Index];

                    if (activecell.Field.Name == "contract_id")
                    {
                        gBillingBatchSummary.ReturnSelectedData(dataKey2);
                        System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
                        cGlobals.ReturnParms.Add("GridContracts.xGrid");
                        RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                        args.Source = gBillingBatchSummary.xGrid;
                        EventAggregator.GeneratedClickHandler(this, args);
                        cGlobals.ReturnParms.Clear();
                    }
                    else
                        if (activecell.Field.Name == "receivable_account")
                        {
                            gBillingBatchSummary.ReturnSelectedData(dataKey3);
                            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
                            cGlobals.ReturnParms.Add("txtReceivableAccount");
                            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                            args.Source = gBillingBatchSummary.xGrid;
                            EventAggregator.GeneratedClickHandler(this, args);
                            cGlobals.ReturnParms.Clear();
                        }
                        else
                        cGlobals.ReturnParms.Clear();

                }


                
            }
            else
            {
                MessageBox.Show("Please select a record in the grid.");
                return;
            }
        }

        public void GridDoubleClickTotalDelegate()
        {
            if (gBillingBatchSummaryTotals.xGrid.ActiveRecord != null)
            {
                Cell activecell = gBillingBatchSummaryTotals.xGrid.ActiveCell;
                if (activecell == null)
                {
                }
                else
                {
                    Record activeRecord = gBillingBatchSummaryTotals.xGrid.Records[gBillingBatchSummaryTotals.ActiveRecord.Index];

                    if (activecell.Field.Name == "contract_id")
                    {
                        gBillingBatchSummaryTotals.ReturnSelectedData(dataKey2);
                        System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
                        cGlobals.ReturnParms.Add("GridContracts.xGrid");
                        RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                        args.Source = gBillingBatchSummaryTotals.xGrid;
                        EventAggregator.GeneratedClickHandler(this, args);
                        cGlobals.ReturnParms.Clear();
                    }
                    else
                        if (activecell.Field.Name == "receivable_account")
                        {
                            gBillingBatchSummaryTotals.ReturnSelectedData(dataKey3);
                            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
                            cGlobals.ReturnParms.Add("txtReceivableAccount");
                            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                            args.Source = gBillingBatchSummaryTotals.xGrid;
                            EventAggregator.GeneratedClickHandler(this, args);
                            cGlobals.ReturnParms.Clear();
                        }
                        else
                            cGlobals.ReturnParms.Clear();

                }



            }
            else
            {
                MessageBox.Show("Please select a record in the grid.");
                return;
            }
        }
         
              
        private void ContextMenuDeleteBatch()
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            //Find the current batch id
            if (this.CurrentBusObj.ObjectData.Tables["summary"].Rows.Count != 0 || this.CurrentBusObj.ObjectData.Tables["exceptions"].Rows.Count != 0)
            {
                //get selected row to selected batch id
                // gBillingBatchSummary.ReturnSelectedData("batch_id");
                int CurrentBatchID = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["ParmTable"].Rows[0]["parmValue"]);
                //Ask if they are sure they want to delete that batch

                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to delete Batch ID " + CurrentBatchID.ToString() + "?", System.Windows.MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    if (cGlobals.BillService.DeleteBatch(CurrentBatchID) == true)
                    {
                        Messages.ShowWarning("Batch ID " + CurrentBatchID.ToString() + " Deleted");
                        this.Load();
                    }
                    else
                        Messages.ShowWarning("Error Deleting Batch");
                }
                else
                {

                    Messages.ShowMessage("Batch not deleted", MessageBoxImage.Information);
                }

            }
        }

         private void ContextMenuDeleteandResubmitBatch()
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            //Find the current batch id
            if (this.CurrentBusObj.ObjectData.Tables["summary"].Rows.Count != 0 || this.CurrentBusObj.ObjectData.Tables["exceptions"].Rows.Count != 0)
            {
                //get selected row to selected batch id
                // gBillingBatchSummary.ReturnSelectedData("batch_id");
                int CurrentBatchID = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["ParmTable"].Rows[0]["parmValue"]);
                //Ask if they are sure they want to delete that batch

                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to delete Batch ID " + CurrentBatchID.ToString() + "?", System.Windows.MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    if (cGlobals.BillService.DeleteBatch(CurrentBatchID) == true)
                    {
                        Messages.ShowWarning("Batch ID " + CurrentBatchID.ToString() + " Deleted");
                        string jobName = "BILBATCH";
                        StringBuilder sbParameters = new StringBuilder(@"/A");
                        sbParameters.Append(" ");
                        sbParameters.Append(CurrentBatchID.ToString());

                        if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, sbParameters.ToString(), DateTime.Now, cGlobals.UserName))
                        {
                            Messages.ShowWarning("Job Scheduled to Run");
                       
                        }
                        else
                        {
                            Messages.ShowWarning("Error Scheduling Job");
                        }
                        this.Load();
                    }
                    else
                        Messages.ShowWarning("Error Deleting Batch");
                }
                else
                {

                    Messages.ShowMessage("Batch not deleted", MessageBoxImage.Information);
                }

            }
        }
         


        private void ContextMenuDeleteContract()
        {
            string sBatchID = "";
            string sContractID = "";
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            //Find the current batch id
            if (this.CurrentBusObj.ObjectData.Tables["summary"].Rows.Count != 0 && gBillingBatchSummary.xGrid.SelectedItems.Count() != 0)
            {
                //get selected row to selected batch id
                gBillingBatchSummary.ReturnSelectedData("batch_id");

                //Ask if they are sure they want to delete that batch
                if (cGlobals.ReturnParms.Count > 0)
                {

                    {

                        sBatchID = cGlobals.ReturnParms[0].ToString();
                        //Get contract id
                        gBillingBatchSummary.ReturnSelectedData("contract_id");
                        if (cGlobals.ReturnParms.Count > 0)
                        {
                            sContractID = cGlobals.ReturnParms[0].ToString();
                            MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to delete contract ID " + sContractID.ToString() + " from Batch ID " + sBatchID.ToString() + "?", System.Windows.MessageBoxImage.Question);


                            if (result == MessageBoxResult.Yes)
                            {
                                BatchID = int.Parse(sBatchID);
                                ContractID = int.Parse(sContractID);

                                if (cGlobals.BillService.DeleteContractfromBatch(BatchID, ContractID) == true)
                                {
                                    Messages.ShowWarning("Contract ID " + ContractID + " Deleted.  Press refresh to update Detail, Exception and Reporting Tabs");
                                
                                    this.Load();

                                  
                                   
                                    
                                   
                                    BillingBatchFolder BillingBatchFolder = UIHelper.FindVisualParent<BillingBatchFolder>(this);
                                    if (BillingBatchFolder != null)
                                    {
                                        
                                    }
                                }
                                else
                                    Messages.ShowWarning("Error Deleting Contract ID");
                            }
                            else
                            {

                                Messages.ShowMessage("Contract not deleted", MessageBoxImage.Information);
                            }
                        }
                        else
                            Messages.ShowWarning("Could not return Contract ID");
                    }
                }
                else
                    Messages.ShowWarning("No contract id to delete");
            }
        }


    }
}

    
