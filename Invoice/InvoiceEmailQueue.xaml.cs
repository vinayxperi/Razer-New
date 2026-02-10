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
using Infragistics.Windows.Controls;

namespace Invoice
{
    /// <summary>
    /// Interaction logic for InvoiceEmailQueue.xaml
    /// </summary>
    public partial class InvoiceEmailQueue : ScreenBase, IScreen, IPreBindable
    {
        enum InvoiceEmailStatus
        {
            InProgress = 3,
            Error = 2,
            Cleared = 4,
            Sent = 1,
            Unsent = 0,
            All = -1
        }

        private string CompanyCode { get; set; }

        private bool IsPreBound = false;

        #region IScreen Implementation
        public void Init(cBaseBusObject businessObject)
        {
            _WindowCaption = "Invoice Email Queue";
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "InvoiceEmailQueue";
            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            InvoiceEmailQueueGrid.MainTableName = MainTableName;
            InvoiceEmailQueueGrid.xGrid.FieldLayoutSettings.AllowDelete = false;
            InvoiceEmailQueueGrid.xGrid.FieldSettings.AllowEdit = false;

            InvoiceEmailQueueGrid.SetGridSelectionBehavior(true, false);
            InvoiceEmailQueueGrid.FieldLayoutResourceString = "InvoiceEmailQueueGrid";
            InvoiceEmailQueueGrid.ConfigFileName = "InvoiceEmailQueueConfig";
            InvoiceEmailQueueGrid.ContextMenuAddIsVisible = false;
            InvoiceEmailQueueGrid.ContextMenuRemoveIsVisible = false;
            
            InvoiceEmailQueueGrid.ContextMenuGenericDelegate1 = SendEmailInvoiceDelegate;
            InvoiceEmailQueueGrid.ContextMenuGenericDisplayName1 = "Email Invoices";
            InvoiceEmailQueueGrid.ContextMenuGenericIsVisible1 = true;

            InvoiceEmailQueueGrid.ContextMenuGenericDelegate2 = ChangeStatusDelegate;
            InvoiceEmailQueueGrid.ContextMenuGenericDisplayName2 = "Change Status";
            InvoiceEmailQueueGrid.ContextMenuGenericIsVisible2 = true;

            //RES 4/28/15 add option to display/hide sent invoices
            InvoiceEmailQueueGrid.ContextMenuGenericDelegate3 = ShowSentDelegate;
            InvoiceEmailQueueGrid.ContextMenuGenericDisplayName3 = "Show Sent Invoices";
            InvoiceEmailQueueGrid.ContextMenuGenericIsVisible3 = true;

            //RES 4/28/15 add option to display/hide sent invoices
            InvoiceEmailQueueGrid.ContextMenuGenericDelegate4 = HideSentDelegate;
            InvoiceEmailQueueGrid.ContextMenuGenericDisplayName4 = "Hide Sent Invoices";
            InvoiceEmailQueueGrid.ContextMenuGenericIsVisible4 = true;

            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                InvoiceEmailQueueGrid.ContextMenuGenericIsVisible1 = false;
                InvoiceEmailQueueGrid.ContextMenuGenericIsVisible2 = false;
            }


            GridCollection.Add(InvoiceEmailQueueGrid);

            this.Load();

            cboStatus.SelectedValue = -1;
            //RES 4/28/15 initialize filter above
            //FilterInvoiceEmailQueueGrid();
            //RES 4/28/15 filter out sent invoices when window is opened
            var filter = new RecordFilter();
            filter.FieldName = "sent_flag";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 1));
            InvoiceEmailQueueGrid.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
        }

        private void ChangeStatusDelegate()
        {
            string Invoice;
            string FromStatus;
            string ToStatus;
            if (this.CurrentBusObj.ObjectData.Tables["InvoiceEmailQueue"].Rows.Count != 0)
            {
                foreach (Record record in InvoiceEmailQueueGrid.xGrid.SelectedItems.Records)
                {
                   DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                   Invoice = row["invoice_number"].ToString();
                   FromStatus = row["sent_flag_description"].ToString(); 
                   if (Convert.ToInt32(row["sent_flag"]) == 0)
                   {
                       ToStatus = "Cleared";
                       MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to change the status of " + Invoice.Trim() + " to " + ToStatus + "?", MessageBoxImage.Question);
                       if (result == MessageBoxResult.Yes)
                       {
                           row["sent_flag"] = "4";
                           Save();
                           Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for invoice " + Invoice);
                       }
                   }
                   else
                       if (Convert.ToInt32(row["sent_flag"]) == 4)
                       {
                           ToStatus = "Unsent";
                           MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to change the status of " + Invoice.Trim() + " to " + ToStatus + "?", MessageBoxImage.Question);
                           if (result == MessageBoxResult.Yes)
                           {
                               row["sent_flag"] = "0";
                               Save();
                               Messages.ShowInformation("Status changed from " + FromStatus + " to " + ToStatus + " for invoice " + Invoice);
                           }
                       }
                       else
                           Messages.ShowWarning("You can only change the status of unsent or cleared invoices!");
                   foreach (DataRecord r in InvoiceEmailQueueGrid.xGrid.Records)
                   {
                       if (r.Cells["invoice_number"].Value.ToString() == Invoice)
                       {
                           r.IsActive = true;
                           r.IsSelected = true;
                           r.DataPresenter.BringCellIntoView(r.Cells["invoice_number"]);
                       }
                   }
                }
            } 
            else
                Messages.ShowWarning("No Selected Rows to Change Status!");

        }

        //RES 4/28/15 Show sent invoices
        private void ShowSentDelegate()
        {
            var filter = new RecordFilter();
            filter.FieldName = "sent_flag";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, -1));

            //Apply the filter to the grid
            InvoiceEmailQueueGrid.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
        }

        //RES 4/28/15 Hide sent invoices
        private void HideSentDelegate()
        {
            var filter = new RecordFilter();
            filter.FieldName = "sent_flag";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, 1));

            //Apply the filter to the grid
            InvoiceEmailQueueGrid.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
        }

        private void SendEmailInvoiceDelegate()
        {
                   
            InvoiceEmailCountriesLookup lookup = new InvoiceEmailCountriesLookup(this.CurrentBusObj);
            lookup.HideShowAllCompaniesButton();

            var DialogResult = lookup.ShowDialog();
            //RES 2/19/15 Company Consolidation
            if (DialogResult != null && DialogResult == true && !string.IsNullOrEmpty(lookup.BillingOwnerID))
            //if (DialogResult != null && DialogResult == true && !string.IsNullOrEmpty(lookup.CompanyCode))
            {
                //RES 2/19/15 Company Consolidation
                MessageBoxResult result = Messages.ShowYesNo(string.Format("Are you sure you want to email the invoices for {0}?", lookup.BillingOwnerName), MessageBoxImage.Question);
                //MessageBoxResult result = Messages.ShowYesNo(string.Format("Are you sure you want to email the invoices for {0}?", lookup.CompanyDescription), MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //RES 2/19/15 Company Consolidation
                    if (cGlobals.BillService.SetInvoiceEmailQueue(lookup.BillingOwnerID))
                    //if (cGlobals.BillService.SetInvoiceEmailQueue(lookup.CompanyCode))
                    {
                        StringBuilder sbParameters = new StringBuilder(@"/A");
                        sbParameters.Append(" ");
                        //RES 2/19/15 Company consolidation
                        sbParameters.Append(lookup.BillingOwnerID.ToString());
                        //sbParameters.Append(lookup.CompanyCode.ToString());
                        //RES 4/18/13 Razer Ver 3.0
                        sbParameters.Append(" /A ");
                        sbParameters.Append(cGlobals.UserName.ToString());

                        if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, "Email Invoices", sbParameters.ToString(), DateTime.Now, cGlobals.UserName.ToString()) == true)
                        {
                            Messages.ShowWarning("Job Scheduled to Run");
                            this.Load();
                            cboStatus.SelectedValue = -1;
                            //RES 4/28/15 Filter using options on dropdown menu
                            //FilterInvoiceEmailQueueGrid();
                        }
                        else
                            Messages.ShowWarning("Error Scheduling Job");
                    }
                    else
                    {
                        Messages.ShowWarning("Error Setting Invoices to 'In Progress'");
                    }
                }
            }
        }

        private void FilterInvoiceEmailQueueGrid()
        {
            InvoiceEmailQueueGrid.ClearFilter();

            if (cboStatus.SelectedValue.ToString() != "-1")
            {
                InvoiceEmailQueueGrid.FilterGrid("sent_flag", cboStatus.SelectedValue.ToString());
            }

            if (CompanyCode != "")
            {
                InvoiceEmailQueueGrid.FilterGrid("primary_co", CompanyCode);
            }
        }

        private string _WindowCaption;
        public string WindowCaption
        {
            get { return _WindowCaption; }
        }
        #endregion

        public void PreBind()
        {
            if (!IsPreBound)
            {
                DataTable dtStatus = new DataTable("Status");
                dtStatus.Columns.Add("status", typeof(int));
                dtStatus.Columns.Add("description", typeof(string));

                foreach (InvoiceEmailStatus item in Enum.GetValues(typeof(InvoiceEmailStatus)))
                {
                    DataRow newRow = dtStatus.NewRow();
                    newRow["status"] = (int)item;
                    newRow["description"] = Enum.GetName(typeof(InvoiceEmailStatus), item);
                    dtStatus.Rows.Add(newRow);
                }

                cboStatus.SetBindingExpression("status", "description", dtStatus);

                IsPreBound = true;
            }
        }

        public InvoiceEmailQueue()
        {
            InitializeComponent();
        }

        private void cboStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedValue":
                    //RES 4/28/15 Filter using options on dropdown menu
                    //FilterInvoiceEmailQueueGrid();
                    break;
                default:
                    break;
            }
        }

        private void cboCompany_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            InvoiceEmailCountriesLookup lookup = new InvoiceEmailCountriesLookup(this.CurrentBusObj);

            var result = lookup.ShowDialog();

            if (result != null && result == true)
            {
                //RES 2/19/15 Company consolidation
                CompanyCode = lookup.BillingOwnerID;
                cboCompany.Text = lookup.BillingOwnerName;
                //CompanyCode = lookup.CompanyCode;
                //cboCompany.Text = lookup.CompanyDescription;
                //RES 4/28/15 Filter using options on dropdown menu
                //FilterInvoiceEmailQueueGrid();
            }
        }

        private void btnSendInvoice_Click(object sender, RoutedEventArgs e)
        {
            SendEmailInvoiceDelegate();
        }
    }
}
