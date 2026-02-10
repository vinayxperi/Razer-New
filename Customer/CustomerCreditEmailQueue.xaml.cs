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

namespace Customer
{
    /// <summary>
    /// Interaction logic for CustomerCreditEmailQueue.xaml
    /// </summary>
    public partial class CustomerCreditEmailQueue : ScreenBase, IScreen, IPreBindable
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
            _WindowCaption = "Customer Credit Email Queue";
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "customerCreditEmailQueue";
            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            CustomerCreditEmailQueueGrid.MainTableName = "main";
            CustomerCreditEmailQueueGrid.xGrid.FieldLayoutSettings.AllowDelete = false;
            CustomerCreditEmailQueueGrid.xGrid.FieldSettings.AllowEdit = false;

            CustomerCreditEmailQueueGrid.SetGridSelectionBehavior(true, false);
     
            CustomerCreditEmailQueueGrid.FieldLayoutResourceString = "CustomerCreditEmailQueueGrid";
            CustomerCreditEmailQueueGrid.ConfigFileName = "CustomerCreditEmailQueueConfig";
            CustomerCreditEmailQueueGrid.ContextMenuAddIsVisible = false;
            CustomerCreditEmailQueueGrid.ContextMenuRemoveIsVisible = false;

            CustomerCreditEmailQueueGrid.ContextMenuGenericDelegate1 = SendEmailCreditStatusDelegate;
            CustomerCreditEmailQueueGrid.ContextMenuGenericDisplayName1 = "Send Credit Email";
            CustomerCreditEmailQueueGrid.ContextMenuGenericIsVisible1 = true;

            CustomerCreditEmailQueueGrid.ContextMenuGenericDelegate2 = ChangeStatusDelegate;
            CustomerCreditEmailQueueGrid.ContextMenuGenericDisplayName2 = "Change Status";
            CustomerCreditEmailQueueGrid.ContextMenuGenericIsVisible2 = true;

            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                CustomerCreditEmailQueueGrid.ContextMenuGenericIsVisible1 = false;
                CustomerCreditEmailQueueGrid.ContextMenuGenericIsVisible2 = false;
            }
            else
            {
                CustomerCreditEmailQueueGrid.ContextMenuGenericIsVisible1 = true;
                CustomerCreditEmailQueueGrid.ContextMenuGenericIsVisible2 = true;
            }




            GridCollection.Add(CustomerCreditEmailQueueGrid);

            this.Load();

            cboStatus.SelectedValue = -1;

            FilterInvoiceEmailQueueGrid();
        }

        private void ChangeStatusDelegate()
        {
            string Customer;
            string FromStatus;
            string ToStatus;
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)
            {
                foreach (Record record in CustomerCreditEmailQueueGrid.xGrid.SelectedItems.Records)
                {
                   DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                   Customer = row["receivable_account"].ToString();
                   FromStatus = row["status_flag_description"].ToString(); 
                   if (Convert.ToInt32(row["status_flag"]) == 0)
                   {
                       ToStatus = "Cleared";
                       MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to change the status for Customer " + Customer.Trim() + " to " + ToStatus + "?", MessageBoxImage.Question);
                       if (result == MessageBoxResult.Yes)
                       {
                           row["status_flag"] = "4";
                           Save();
                           Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for Customer " + Customer);
                       }
                   }
                   else
                       if (Convert.ToInt32(row["status_flag"]) == 4)
                       {
                           ToStatus = "Unsent";
                           MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to change the status of " + Customer.Trim() + " to " + ToStatus + "?", MessageBoxImage.Question);
                           if (result == MessageBoxResult.Yes)
                           {
                               row["status_flag"] = "0";
                               Save();
                               Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for Customer " + Customer);
                           }
                       }
                       else
                           Messages.ShowWarning("You can only change the status of unsent or cleared Emails!");
                   foreach (DataRecord r in CustomerCreditEmailQueueGrid.xGrid.Records)
                   {
                       if (r.Cells["receivable_account"].Value.ToString() == Customer)
                       {
                           r.IsActive = true;
                           r.IsSelected = true;
                           r.DataPresenter.BringCellIntoView(r.Cells["receivable_account"]);
                       }
                   }
                }
            } 
            else
                Messages.ShowWarning("No Selected Rows to Change Status!");

        }

        private void SendEmailCreditStatusDelegate()
        {



            string Customer;
            int rowID;
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)

                foreach (Record record in CustomerCreditEmailQueueGrid.xGrid.SelectedItems.Records)
                {
                    DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                    Customer = row["receivable_account"].ToString();
                    rowID = Convert.ToInt32(row["credit_rating_queue_id"]);

                    if (Convert.ToInt32(row["status_flag"]) == 0)
                    {

                        MessageBoxResult result = Messages.ShowYesNo(string.Format("Are you sure you want to send the credit change emails to the salespersons for Customer " + Customer + "?"), MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            row["status_flag"] = "3";
                            Save();


                            StringBuilder sbParameters = new StringBuilder(@"/A");
                            sbParameters.Append(" ");

                            sbParameters.Append(cGlobals.UserName.ToString());
                            sbParameters.Append(" /A ");
                            sbParameters.Append(rowID.ToString());

                            if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, "Email Credit Status Change", sbParameters.ToString(), DateTime.Now, cGlobals.UserName.ToString()) == true)
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
                            Messages.ShowWarning("No Emails will be sent");
                        }
                    }

                }
        }
            

        private void FilterInvoiceEmailQueueGrid()
        {
            CustomerCreditEmailQueueGrid.ClearFilter();

            if (cboStatus.SelectedValue.ToString() != "-1")
            {
                CustomerCreditEmailQueueGrid.FilterGrid("status_flag", cboStatus.SelectedValue.ToString());
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
                dtStatus.Columns.Add("status_flag_description", typeof(string));

                foreach (CustomerCreditEmailStatus item in Enum.GetValues(typeof(CustomerCreditEmailStatus)))
                {
                    DataRow newRow = dtStatus.NewRow();
                    newRow["status_flag"] = (int)item;
                    newRow["status_flag_description"] = Enum.GetName(typeof(CustomerCreditEmailStatus), item);
                    dtStatus.Rows.Add(newRow);
                }

                cboStatus.SetBindingExpression("status_flag", "status_flag_description", dtStatus);

                IsPreBound = true;
            }
        }

        public CustomerCreditEmailQueue()
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
