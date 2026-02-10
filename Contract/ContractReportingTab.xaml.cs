using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data;
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
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;

namespace Contract
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ContractReportingTab : ScreenBase, IPreBindable
    {
        public ComboBoxItemsProvider cmbTerms { get; set; }
        public ComboBoxItemsProvider cmbPeople { get; set; }
        public ComboBoxItemsProvider cmbLevel { get; set; }
        public ComboBoxItemsProvider cmbSource { get; set; }
        private DataRow rDefault;
        public ContractReportingTab()
        {
            InitializeComponent();
            Init();
        }

        internal ContractFolderMainScreen MainScreen { get; set; }

        private void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "contract_report";

            //Establist the COmpany Contract Grid
           
            gReports.MainTableName = "contract_report";
            gReports.ConfigFileName = "ContractReportingReportsGrid";
            gReports.FieldLayoutResourceString = "ContractReportingReports";
            gReports.xGrid.FieldSettings.AllowEdit = true;
            gReports.SetGridSelectionBehavior(false, false);
            gReports.ContextMenuAddDelegate = ContractReportAddDelegate;
            gReports.ContextMenuAddDisplayName = "Add Report";
            gReports.ContextMenuAddIsVisible = true;
            gReports.ContextMenuGenericDelegate1 = ReportingCommentsDelegate;
            gReports.ContextMenuGenericDisplayName1 = "Reporting Comments & Attachments";
            gReports.ContextMenuGenericIsVisible1 = true;
            gReports.ContextMenuRemoveIsVisible = false;

            gReportDetails.xGrid.FieldSettings.AllowEdit = true;
            gReportDetails.MainTableName = "contract_report_detail";
            gReportDetails.ConfigFileName = "ContractReportingReportDetailsGrid";
            gReportDetails.SetGridSelectionBehavior(false, false);
           
            gReportDetails.FieldLayoutResourceString = "ContractReportingReportDetails";
            gReports.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "report_id" }, ChildGrids = { gReportDetails }, ParentFilterOnColumnNames = { "report_id" } });

            gReportReminderHistory.xGrid.FieldSettings.AllowEdit = false;
            gReportReminderHistory.MainTableName = "contract_reminder_history";
            gReportReminderHistory.ConfigFileName = "ContractReportingReportReminderHistoryGrid";
            gReportReminderHistory.SetGridSelectionBehavior(false, false);
            gReportReminderHistory.FieldLayoutResourceString = "ContractReportingReportReminderHistory";
            gReportDetails.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = {  "report_id" }, ChildGrids = { gReportReminderHistory }, ParentFilterOnColumnNames = {  "report_id" } });

            //gReportAttachments.xGrid.FieldSettings.AllowEdit = false;
            //gReportAttachments.MainTableName = "report_reminder";
            //gReportAttachments.ConfigFileName = "ContractReportingReportReminderHistoryGrid";
            //gReportAttachments.SetGridSelectionBehavior(true, false);
            //gReportAttachments.FieldLayoutResourceString = "ContractReportingReportReminderHistoryGrid";
            //gReports.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "report_id" }, ChildGrids = { gReportDetails }, ParentFilterOnColumnNames = { "report_id" } });

            gReportDistribution.xGrid.FieldSettings.AllowEdit = true;
            gReportDistribution.MainTableName = "contract_report_dist";
            gReportDistribution.ConfigFileName = "ContractReportingReportDistGrid";
            gReportDistribution.SetGridSelectionBehavior(false, false);
            gReportDistribution.FieldLayoutResourceString = "ContractReportingReportDistGrid";
            gReportDistribution.ContextMenuAddDelegate = ContractReportAddDistDelegate;
            gReportDistribution.ContextMenuAddDisplayName = "Add Reminder Distribution";
            gReportDistribution.ContextMenuAddIsVisible = true;
            gReportDistribution.ContextMenuRemoveIsVisible = true;
            gReportDistribution.ContextMenuRemoveDisplayName = "Delete Reminder Distribution";
            gReportDistribution.ContextMenuRemoveDelegate = ContractReportRemoveDistDelegate;

            gReports.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "contract_id" , "report_id"}, ChildGrids = { gReportDistribution }, ParentFilterOnColumnNames = { "contract_id", "report_id" } });
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                gReportDistribution.ContextMenuAddIsVisible = false;
                gReports.ContextMenuAddIsVisible = false;
                gReports.ContextMenuRemoveIsVisible = false;
                gReports.ContextMenuGenericIsVisible1 = false;
            }
            GridCollection.Add(gReports);
            GridCollection.Add(gReportDetails);
            GridCollection.Add(gReportReminderHistory);
            GridCollection.Add(gReportDistribution);
           
        }

        private void ContractReportRemoveDistDelegate()
        {
            //remove contact
            object nothing = null;
            //gInvoiceDistribution.xGrid.FieldSettings.AllowEdit = true;
            DataPresenterCommands.DeleteSelectedDataRecords.Execute(nothing, gReportDistribution);
            //gInvoiceDistribution.xGrid.FieldSettings.AllowEdit = false;
        }

        private void ContractReportAddDelegate()
        {
            DateTime today = DateTime.Today;
            gReports.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = gReports.xGrid.RecordManager.CurrentAddRecord;
            //Set the default values for the columns
            row.Cells["report_id"].Value = 0;
            row.Cells["contract_id"].Value = getContractId();
            row.Cells["report_seq_id"].Value = 0;
            row.Cells["report_description"].Value = "";
            row.Cells["frequency"].Value = 1;
           
            row.Cells["terms_code"].Value = "N30";
            row.Cells["start_date"].Value = today;
            row.Cells["billable_flag"].Value = 1;
            row.Cells["reconciliation_flag"].Value = 1;
            row.Cells["send_reminder_flag"].Value = 1;
            row.Cells["payment_due_flag"].Value = 0;
            row.Cells["report_status"].Value = 0;
            row.Cells["reminder_freq"].Value = 0;
            row.Cells["level0"].Value = 0;
            row.Cells["level1"].Value = 0;
            row.Cells["level2"].Value = 0;
            row.Cells["level3"].Value = 0;
            row.Cells["report_source"].Value = 0;
            //Commit the add new record - required to make this record active
            gReports.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            gReports.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            gReports.xGrid.ActiveRecord = gReports.xGrid.Records[0];
            //Set the field as active
            (gReports.xGrid.Records[gReports.ActiveRecord.Index] as DataRecord).Cells["report_description"].IsActive = true;
            //Moves the cursor into the active cell 
            gReports.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        private void ContractReportAddDistDelegate()
        {
            int reportID = 0; 
            //Need to find the report id in the main grid 
            foreach (Record record in gReports.xGrid.SelectedItems.Records)
            {
                
                DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;

               reportID = Convert.ToInt32(r["report_id"]);
            }
            if (reportID == 0)
            {
                Messages.ShowInformation("The new report must be saved before a distribution list can be added");
                return;
            }
            gReportDistribution.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = gReportDistribution.xGrid.RecordManager.CurrentAddRecord;
            //Set the default values for the columns
            row.Cells["contact_id"].Value = getContractId();
            //need to set it to report_id selected in gReports
            row.Cells["report_id"].Value = reportID;
            row.Cells["level"].Value = 0;
            //Commit the add new record - required to make this record active
            gReportDistribution.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            gReportDistribution.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            gReportDistribution.xGrid.ActiveRecord = gReportDistribution.xGrid.Records[0];
            //Set the field as active
            (gReportDistribution.xGrid.Records[gReportDistribution.ActiveRecord.Index] as DataRecord).Cells["contact_id"].IsActive = true;
            //Moves the cursor into the active cell 
            gReportDistribution.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        private int getContractId()
        {
        //    var localContractId = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
        //                          where x.Field<string>("parmName") == "@contract_id"
        //                          select x.Field<string>("parmValue");

        //    foreach (var id in localContractId)
        //    {
        //        int ContractId = Convert.ToInt32(id);
        //        //return contract id
        //        return ContractId;
        //    }
            return 0;
        }
       
         private void ReportingCommentsDelegate()
        {
             //get contract id and report detail id to pass
             int ReportID = 0;
             int ContractID = 0;
             foreach (Record record in gReports.xGrid.SelectedItems.Records)
            {
                
                DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;

               ReportID = Convert.ToInt32(r["report_id"]);
               ContractID = Convert.ToInt32(r["contract_id"]);
            }


             //ContractLocationMove LocationMoveScreen = new ContractLocationMove(330, this.CurrentBusObj);
             ContractReportingComments ContractCommentsScreen = new ContractReportingComments(ContractID, ReportID, this.CurrentBusObj);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window ContractReportingCommentsWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            ContractReportingCommentsWindow.Title = "Contract Reporting Comments/Attachments Screen";
            ContractReportingCommentsWindow.MaxHeight = 1280;
            ContractReportingCommentsWindow.MaxWidth = 1280;
            /////////////////////////////////////////////////////
            //set screen as content of new window
            ContractReportingCommentsWindow.Content = ContractCommentsScreen;
            //open new window with embedded user control
            ContractReportingCommentsWindow.ShowDialog();
          
            
        }
        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {

                    //Setup Grid Combo Boxes
                    //Product drop down box 
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["terms"].DefaultView;
                    ip.ValuePath = "terms_code";
                    ip.DisplayMemberPath = ("terms_code");
                    cmbTerms = ip;
                    ComboBoxItemsProvider ip2 = new ComboBoxItemsProvider();
                    ip2 = new ComboBoxItemsProvider();
                    ip2.ItemsSource = CurrentBusObj.ObjectData.Tables["cmbPeople"].DefaultView;
                    ip2.ValuePath = "contact_id";
                    ip2.DisplayMemberPath = ("full_name");
                    cmbPeople = ip2;
                    ComboBoxItemsProvider ip3 = new ComboBoxItemsProvider();
                    ip3 = new ComboBoxItemsProvider();
                    ip3.ItemsSource = CurrentBusObj.ObjectData.Tables["cmbLevel"].DefaultView;
                    ip3.ValuePath = "fkey_int";
                    ip3.DisplayMemberPath = ("code_value");
                    cmbLevel = ip3;
                    ComboBoxItemsProvider ip4 = new ComboBoxItemsProvider();
                    ip4 = new ComboBoxItemsProvider();
                    ip4.ItemsSource = CurrentBusObj.ObjectData.Tables["cmbSource"].DefaultView;
                    ip4.ValuePath = "report_source";
                    ip4.DisplayMemberPath = ("code_value");
                    cmbSource = ip4;


                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }
    
    
    }


}
