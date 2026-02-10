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

namespace  BCF
{
    /// <summary>
    /// Interaction logic for BCFEmailQueue.xaml
    /// </summary>
    public partial class BCFEmailQueue : ScreenBase, IScreen, IPreBindable
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
            _WindowCaption = "BFC/TF Ops Email Queue";
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "BCFEmailQueue";
            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            BCFEmailQueueGrid.MainTableName = "main";
           BCFEmailQueueGrid.xGrid.FieldLayoutSettings.AllowDelete = false;
            BCFEmailQueueGrid.xGrid.FieldSettings.AllowEdit = false;

            BCFEmailQueueGrid.SetGridSelectionBehavior(true, false);

            BCFEmailQueueGrid.FieldLayoutResourceString = "BCFEmailQueueGrid";
            BCFEmailQueueGrid.ConfigFileName = "BCFEmailQueue";
            BCFEmailQueueGrid.ContextMenuAddIsVisible = false;
            BCFEmailQueueGrid.ContextMenuRemoveIsVisible = false;

            BCFEmailQueueGrid.ContextMenuGenericDelegate1 = SendBCFEmailSingleDelegate;
            BCFEmailQueueGrid.ContextMenuGenericDisplayName1 = "Send BCF/TF";
            BCFEmailQueueGrid.ContextMenuGenericIsVisible1 = true;

            BCFEmailQueueGrid.ContextMenuGenericDelegate2 = ChangeStatusDelegate;
            BCFEmailQueueGrid.ContextMenuGenericDisplayName2 = "Change Status";
            BCFEmailQueueGrid.ContextMenuGenericIsVisible2 = true;

          

            BCFEmailQueueGrid.WindowZoomDelegate = GridDoubleClickDelegate;
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                BCFEmailQueueGrid.ContextMenuGenericIsVisible1 = false;
                BCFEmailQueueGrid.ContextMenuGenericIsVisible2 = false;
                BCFEmailQueueGrid.ContextMenuGenericIsVisible3 = false;
            }





            GridCollection.Add(BCFEmailQueueGrid);

            this.Load();

            cboStatus.SelectedValue = -1;

            FilterInvoiceEmailQueueGrid();
        }

        public void GridDoubleClickDelegate()
        {
            //call contract folder
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
            string BCFNumber = BCFEmailQueueGrid.ActiveRecord.Cells["bcf_number"].Value.ToString();
            BCFEmailQueueGrid.ReturnSelectedData("bcf_number");
            if (BCFNumber.StartsWith("BCF"))
                cGlobals.ReturnParms.Add("BCF.xGrid");
            else
                cGlobals.ReturnParms.Add("TF.xGrid");

            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = BCFEmailQueueGrid.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        } 

        private void ChangeStatusDelegate()
        {
            string Description;
            string FromStatus;
            string ToStatus;
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)
            {
                foreach (Record record in BCFEmailQueueGrid.xGrid.SelectedItems.Records)
                {
                    DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                    Description = row["bcf_number"].ToString();
                    FromStatus = row["status_description"].ToString();
                    if (Convert.ToInt32(row["status_flag"]) == 0)
                    {
                        ToStatus = "Cleared";
                        MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to change the status for this BCF/TF " + Description.Trim() + " to " + ToStatus + "?", MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            row["status_flag"] = "4";
                            Save();
                            Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for this BCF " + Description);
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
                                Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for this BCF " + Description);
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
                                    Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for this BCF " + Description);
                                }
                            }

                            else
                                Messages.ShowWarning("You can only change the status of unsent or cleared Emails!");
                    foreach (DataRecord r in BCFEmailQueueGrid.xGrid.Records)
                    {
                        if (r.Cells["bcf_number"].Value.ToString() == Description)
                        {
                            r.IsActive = true;
                            r.IsSelected = true;
                            r.DataPresenter.BringCellIntoView(r.Cells["bcf_number"]);
                        }
                    }
                }
            }
            else
                Messages.ShowWarning("No Selected Rows to Change Status!");

        }

      

        private void SendBCFEmailSingleDelegate()
        {



            string Description;
            string SStatus;
            int rowID;
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)

                foreach (Record record in BCFEmailQueueGrid.xGrid.SelectedItems.Records)
                {
                    DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                    Description = row["bcf_number"].ToString();
                    //CompanyCode = row["company_code"].ToString();
                    rowID = Convert.ToInt32(row["queue_id"]);

                    if (Convert.ToInt32(row["status_flag"]) == 0)
                    {

                        MessageBoxResult result = Messages.ShowYesNo(string.Format("Are you sure you want to send the email for this BCF/TF  " + Description.ToString() + "?"), MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            foreach (Record Srecord in BCFEmailQueueGrid.xGrid.Records)
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
                            sbParameters.Append(rowID.ToString());

                            


                            if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, "Email BCF", sbParameters.ToString(), DateTime.Now, cGlobals.UserName.ToString()) == true)
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


        private void FilterInvoiceEmailQueueGrid()
        {
            BCFEmailQueueGrid.ClearFilter();

            if (cboStatus.SelectedValue.ToString() != "-1")
            {
                BCFEmailQueueGrid.FilterGrid("status_flag", cboStatus.SelectedValue.ToString());
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

        public BCFEmailQueue()
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
