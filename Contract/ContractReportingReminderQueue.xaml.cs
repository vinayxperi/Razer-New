using System;
using System.Collections.Generic;
using System.Linq;
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
using RazerBase;
using RazerBase.Interfaces;
using RazerInterface;
using Infragistics.Windows.Editors;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Contract
{
    /// <summary>
    /// Interaction logic for ContractReportingReminderQueue.xaml
    /// </summary>
    public partial class ContractReportingReminderQueue : ScreenBase, IScreen, IPreBindable
    {
        enum CustomerCreditEmailStatus
        {
            InProgress = 3,
            Error = 2,
            Cleared = 4,
            Sent = 1,
            Unsent = 0,
            All = -1
        }
        private bool IsPreBound = false;
        public void Init(cBaseBusObject businessObject)
        {
            _WindowCaption = "Contract Reporting Reminder Queue";
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "ContractReportingReminderQueue";
            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            ContractReportingReminderQueueGrid.MainTableName = "main";
           ContractReportingReminderQueueGrid.xGrid.FieldLayoutSettings.AllowDelete = false;
            ContractReportingReminderQueueGrid.xGrid.FieldSettings.AllowEdit = false;

            ContractReportingReminderQueueGrid.SetGridSelectionBehavior(true, false);

            ContractReportingReminderQueueGrid.FieldLayoutResourceString = "ContractReportingReminderQueueGrid";
            ContractReportingReminderQueueGrid.ConfigFileName = "ContractReportingReminderQueue";
            ContractReportingReminderQueueGrid.ContextMenuAddIsVisible = false;
            ContractReportingReminderQueueGrid.ContextMenuRemoveIsVisible = false;

            ContractReportingReminderQueueGrid.ContextMenuGenericDelegate1 = SendReportingReminderDelegate;
            //RES 2/18/15 Company Consolidation project
            //ContractReportingReminderQueueGrid.ContextMenuGenericDisplayName1 = "Send Reporting Reminder Email by Company";
            ContractReportingReminderQueueGrid.ContextMenuGenericDisplayName1 = "Send Reporting Reminder Email by Billing Owner";
            ContractReportingReminderQueueGrid.ContextMenuGenericIsVisible1 = true;

            ContractReportingReminderQueueGrid.ContextMenuGenericDelegate2 = ChangeStatusDelegate;
            ContractReportingReminderQueueGrid.ContextMenuGenericDisplayName2 = "Change Status";
            ContractReportingReminderQueueGrid.ContextMenuGenericIsVisible2 = true;

            ContractReportingReminderQueueGrid.ContextMenuGenericDelegate3 = SendReportingReminderSingleDelegate;
            ContractReportingReminderQueueGrid.ContextMenuGenericDisplayName3 = "Send Reporting Reminder Email by Report";
            ContractReportingReminderQueueGrid.ContextMenuGenericIsVisible3 = true;

            ContractReportingReminderQueueGrid.ContextMenuGenericDelegate4 = ScheduleContractReportingDelegate;
            ContractReportingReminderQueueGrid.ContextMenuGenericDisplayName4 = "Schedule Contract Reporting job";
            ContractReportingReminderQueueGrid.ContextMenuGenericIsVisible4 = true;

            ContractReportingReminderQueueGrid.WindowZoomDelegate = GridDoubleClickDelegate;
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                ContractReportingReminderQueueGrid.ContextMenuGenericIsVisible1 = false;
                ContractReportingReminderQueueGrid.ContextMenuGenericIsVisible2 = false;
                ContractReportingReminderQueueGrid.ContextMenuGenericIsVisible3 = false;
                ContractReportingReminderQueueGrid.ContextMenuGenericIsVisible4 = false;
            }





            GridCollection.Add(ContractReportingReminderQueueGrid);

            this.Load();

            cboStatus.SelectedValue = -1;

            FilterInvoiceEmailQueueGrid();
        }

        public void GridDoubleClickDelegate()
        {
            //call contract folder
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
            ContractReportingReminderQueueGrid.ReturnSelectedData("contract_id");
            cGlobals.ReturnParms.Add("GridLocationContracts.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = ContractReportingReminderQueueGrid.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        } 

        private void ChangeStatusDelegate()
        {
            string Description;
            string FromStatus;
            string ToStatus;
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)
            {
                foreach (Record record in ContractReportingReminderQueueGrid.xGrid.SelectedItems.Records)
                {
                    DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                    Description = row["report_description"].ToString();
                    FromStatus = row["status_description"].ToString();
                    if (Convert.ToInt32(row["status_flag"]) == 0)
                    {
                        ToStatus = "Cleared";
                        MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to change the status for this Report " + Description.Trim() + " to " + ToStatus + "?", MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            row["status_flag"] = "4";
                            Save();
                            Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for this Report " + Description);
                        }
                    }
                    else
                        if (Convert.ToInt32(row["status_flag"]) == 4)
                        {
                            ToStatus = "Unsent";
                            MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to change the status of " + Description.Trim() + " to " + ToStatus + "?", MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                row["status_flag"] = "0";
                                Save();
                                Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for this Report " + Description);
                            }
                        }
                        else
                            if (Convert.ToInt32(row["status_flag"]) == 2)
                            {
                                ToStatus = "Unsent";
                                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to change the status of " + Description.Trim() + " to " + ToStatus + "?", MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    row["status_flag"] = "0";
                                    Save();
                                    Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for this Report " + Description);
                                }
                            }

                            else
                                Messages.ShowWarning("You can only change the status of unsent or cleared Emails!");
                    foreach (DataRecord r in ContractReportingReminderQueueGrid.xGrid.Records)
                    {
                        if (r.Cells["report_description"].Value.ToString() == Description)
                        {
                            r.IsActive = true;
                            r.IsSelected = true;
                            r.DataPresenter.BringCellIntoView(r.Cells["report_description"]);
                        }
                    }
                }
            }
            else
                Messages.ShowWarning("No Selected Rows to Change Status!");

        }

        private void SendReportingReminderDelegate()
        {



            string Description;
            string CompanyCode;
            string BillingOwner;
            string SStatus;
            int rowID;
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)

                foreach (Record record in ContractReportingReminderQueueGrid.xGrid.SelectedItems.Records)
                {
                    DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                    Description = row["report_description"].ToString();
                    CompanyCode = row["company_code"].ToString();
                    //RES 2/18/15 Company Consolidation project
                    BillingOwner = row["billing_owner_id"].ToString();
                    rowID = Convert.ToInt32(row["queue_id"]);

                    if (Convert.ToInt32(row["status_flag"]) == 0)
                    {
                        //RES 2/18/15 Company Consolidation project
                        MessageBoxResult result = Messages.ShowYesNo(string.Format("Are you sure you want to send the report tracking reminder emails for all Unsent emails for  " + BillingOwner.ToString() + "?"), MessageBoxImage.Question);
                        //MessageBoxResult result = Messages.ShowYesNo(string.Format("Are you sure you want to send the report tracking reminder emails for all Unsent emails for Company  " + CompanyCode.ToString() + "?"), MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            foreach (Record Srecord in ContractReportingReminderQueueGrid.xGrid.Records)
                            {
                                DataRow Srow = ((Srecord as DataRecord).DataItem as DataRowView).Row;
                                //change status to 3
                                //if ((Srow["company_code"].ToString() == CompanyCode))
                                if ((Srow["billing_owner_id"].ToString() == BillingOwner))
                                {
                                    SStatus = Srow["status_flag"].ToString();
                                    if (SStatus == "0")
                                    {
                                        Srow["status_flag"] = "3";
                                    }
                                }
                            }
                            
                            Save();

                            StringBuilder sbParameters = new StringBuilder(@"/A ");
                            //RES 2/18/15 Company Consolidation project
                            //sbParameters.Append(CompanyCode.ToString());
                            sbParameters.Append(BillingOwner.ToString());
                            sbParameters.Append(" ");

                            //add detail id
                            sbParameters.Append(" /A ");
                            //set detail id to 0 since running by company
                            sbParameters.Append("0");
                            sbParameters.Append(" /A ");
                            sbParameters.Append(cGlobals.UserName.ToString());
                           

                            if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, "Email Contract Reporting", sbParameters.ToString(), DateTime.Now, cGlobals.UserName.ToString()) == true)
                            {
                                Messages.ShowWarning("Job Scheduled to Run");
                                this.Load();
                                cboStatus.SelectedValue = -1;
                                FilterInvoiceEmailQueueGrid();
                            }
                            else
                                Messages.ShowWarning("Error Scheduling Job");
                        }
                        else
                        {
                            Messages.ShowWarning("No Emails scheduled to send");
                        }
                    }
                     else
                    {
                        Messages.ShowWarning("Row must have an Unsent status to be sent");
                    }

                }
        }

        private void SendReportingReminderSingleDelegate()
        {



            string Description;
            string CompanyCode;
            string SStatus;
            int rowID;
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)

                foreach (Record record in ContractReportingReminderQueueGrid.xGrid.SelectedItems.Records)
                {
                    DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                    Description = row["report_description"].ToString();
                    CompanyCode = row["company_code"].ToString();
                    rowID = Convert.ToInt32(row["queue_id"]);

                    if (Convert.ToInt32(row["status_flag"]) == 0)
                    {

                        MessageBoxResult result = Messages.ShowYesNo(string.Format("Are you sure you want to send the report tracking reminder email for this report  " + Description.ToString() + "?"), MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            foreach (Record Srecord in ContractReportingReminderQueueGrid.xGrid.Records)
                            {
                                DataRow Srow = ((Srecord as DataRecord).DataItem as DataRowView).Row;
                                //change status to 3
                                if ((Convert.ToInt32(Srow["queue_id"]) == rowID))
                                {
                                    SStatus = Srow["status_flag"].ToString();
                                    if (SStatus == "0")
                                    {
                                        Srow["status_flag"] = "3";

                                    }
                                }
                            }
                            Save();

                            StringBuilder sbParameters = new StringBuilder(@"/A ");
                            sbParameters.Append("00");
                            sbParameters.Append(" ");
                            //add detail id
                            sbParameters.Append(" /A ");
                            sbParameters.Append(rowID.ToString());

                            sbParameters.Append(" /A ");
                            sbParameters.Append(cGlobals.UserName.ToString());


                            if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, "Email Contract Reporting", sbParameters.ToString(), DateTime.Now, cGlobals.UserName.ToString()) == true)
                            {
                                Messages.ShowWarning("Job Scheduled to Run");
                                this.Load();
                                cboStatus.SelectedValue = -1;
                                FilterInvoiceEmailQueueGrid();
                            }
                            else
                                Messages.ShowWarning("Error Scheduling Job");
                        }
                        else
                        {
                            Messages.ShowWarning("No Emails scheduled to send");
                        }
                    }
                    else
                    {
                        Messages.ShowWarning("Row must have an Unsent status to be sent");
                    }

                }
        }

        private void ScheduleContractReportingDelegate()
        {
            DateTime dt = new DateTime();


            string sParms = "";

            string jobName = "Contract Reporting";

            dt = DateTime.Now;


            MessageBoxResult result = Messages.ShowYesNo("Schedule Job " + jobName.ToString() + " to run",
                System.Windows.MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, sParms, dt, cGlobals.UserName)  == true) 
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

        private void FilterInvoiceEmailQueueGrid()
        {
            ContractReportingReminderQueueGrid.ClearFilter();

            if (cboStatus.SelectedValue.ToString() != "-1")
            {
                ContractReportingReminderQueueGrid.FilterGrid("status_flag", cboStatus.SelectedValue.ToString());
            }


        }

        private string _WindowCaption;
        public string WindowCaption
        {
            get { return _WindowCaption; }
        }
        public void PreBind()
        {
            if (!IsPreBound)
            {
                DataTable dtStatus = new DataTable("Status");
                dtStatus.Columns.Add("status_flag", typeof(int));
                dtStatus.Columns.Add("status_description", typeof(string));

                foreach (CustomerCreditEmailStatus item in Enum.GetValues(typeof(CustomerCreditEmailStatus)))
                {
                    DataRow newRow = dtStatus.NewRow();
                    newRow["status_flag"] = (int)item;
                    newRow["status_description"] = Enum.GetName(typeof(CustomerCreditEmailStatus), item);
                    dtStatus.Rows.Add(newRow);
                }

                cboStatus.SetBindingExpression("status_flag", "status_flag_description", dtStatus);

                IsPreBound = true;
            }
        }

        public ContractReportingReminderQueue()
        {
            InitializeComponent();
        }

        private void cboStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedValue":
                    FilterInvoiceEmailQueueGrid();
                    break;
                default:
                    break;
            }
        }



        private void btnSendInvoice_Click(object sender, RoutedEventArgs e)
        {
            //SendEmailInvoiceDelegate();
        }
    }
}
