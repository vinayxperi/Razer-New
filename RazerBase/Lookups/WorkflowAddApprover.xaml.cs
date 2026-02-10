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
using RazerBase.Interfaces;
using Infragistics.Windows.DataPresenter;
using System.Data;

namespace RazerBase.Lookups
{
    /// <summary>
    /// Interaction logic for WorkflowAddApprover.xaml
    /// </summary>
    public partial class WorkflowAddApprover : DialogBase, IScreen
    {
        private static readonly string approverLayout = "ApprovalUsers";
        private static readonly string approvalRecordLayout = "ApprovalRecords";
        private static readonly string approverTableName = "approver";
        private static readonly string approverQueTableName = "approver_que";
        private static readonly string userIdField = "user_Id";        
        private static readonly string workflowSequenceField = "workflow_seq";
        

        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public WorkflowAddApprover()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;
            this.Load();

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            
            idgApprovers.xGrid.FieldLayoutSettings = layouts;
            idgApprovers.FieldLayoutResourceString = approverLayout;
            idgApprovers.MainTableName = approverTableName;
            idgApprovers.xGrid.FieldSettings.AllowEdit = false;
            idgApprovers.SetGridSelectionBehavior(true, false);

            idgApprovers.LoadGrid(CurrentBusObj, approverTableName);

            FieldLayoutSettings queLayouts = new FieldLayoutSettings();
            queLayouts.HighlightAlternateRecords = true;

            idgApprovalRecords.xGrid.FieldLayoutSettings = queLayouts;
            idgApprovalRecords.FieldLayoutResourceString = approvalRecordLayout;
            idgApprovalRecords.MainTableName = approverQueTableName;
            idgApprovalRecords.xGrid.FieldSettings.AllowEdit = false;
            idgApprovalRecords.SetGridSelectionBehavior(true, false);

            idgApprovalRecords.LoadGrid(CurrentBusObj, approverQueTableName);
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            cGlobals.ReturnParms.Clear();
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (idgApprovers.xGrid.SelectedItems.Records != null && idgApprovers.xGrid.SelectedItems.Records.Count > 0
                && idgApprovalRecords.xGrid.SelectedItems.Records != null && idgApprovalRecords.xGrid.SelectedItems.Records.Count > 0)
            {

                DataRecord record = (DataRecord)idgApprovers.xGrid.SelectedItems.Records[0];
                DataRow row = ((DataRowView)record.DataItem).Row;

                DataRecord approvalRecord = (DataRecord)idgApprovalRecords.xGrid.SelectedItems.Records[0];
                DataRow approvalRow = ((DataRowView)approvalRecord.DataItem).Row;

                string userId = row[userIdField].ToString();
                int sequence = (int)approvalRow[workflowSequenceField];

                int position = 1;               

                if ((bool)rdbAfter.IsChecked) { position += sequence; }

                if (cGlobals.BillService.AddWorkflowApprover(cGlobals.ReturnParms[0].ToString(), userId, sequence, position,cGlobals.UserName))
                {
                    Messages.ShowInformation("Approver was added to the approval queue.");
                }
                else
                {
                    Messages.ShowInformation("The attempt to add the approver failed.");
                }

                cGlobals.ReturnParms.Clear();
                this.Close();
            }
            else
            {
                Messages.ShowInformation("You must select and approver and a record from the approval queue.");
            }
        }
    }
}
