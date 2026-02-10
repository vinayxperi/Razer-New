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
using Infragistics.Windows.DataPresenter;
using System.Data;

namespace TF
{
    /// <summary>
    /// Interaction logic for TFApprovalTab.xaml
    /// </summary>
    public partial class TFApprovalTab : ScreenBase
    {

        private static readonly string mainTableName = "approval";
        private static readonly string fieldLayoutResource = "TFApproval";
        private static readonly string documentIdParameter = "@document_id";
        private static readonly string typeParameter = "@wf_type";
        private static readonly string classParameter = "@wf_class";
        private static readonly string subjectParameter = "@subject";
        private static readonly string errorMessageParameter = "@error_message";
        private static readonly string workflowId = "wf_id";

        int status = -1;
        cBaseBusObject approvalBusinessObject;

        public string TFNumber;
        public Boolean approval_btn_clicked = false;

        public cBaseBusObject ApprovalBusinessObject
        {
            get { return approvalBusinessObject; }
            set
            {
                approvalBusinessObject = value;
                LoadApprovalData();
            }
        }
        public TFApprovalTab()
        {
            InitializeComponent();

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;

            this.MainTableName = mainTableName;
            idgAdjustments.xGrid.FieldLayoutSettings = layouts;
            idgAdjustments.FieldLayoutResourceString = fieldLayoutResource;
            idgAdjustments.MainTableName = mainTableName;
            idgAdjustments.xGrid.FieldSettings.AllowEdit = false;
            idgAdjustments.SetGridSelectionBehavior(true, false);  
        }

        private void LoadApprovalData()
        {
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty Approval ");
            if (this.CurrentBusObj.HasObjectData)
            {
                TFNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
            }

            if (!string.IsNullOrEmpty(TFNumber))
            {
                approvalBusinessObject.Parms.ClearParms();
                approvalBusinessObject.Parms.AddParm(documentIdParameter, TFNumber);
                approvalBusinessObject.LoadData();
                idgAdjustments.LoadGrid(approvalBusinessObject, mainTableName);
                approvalBusinessObject.Parms.ClearParms();
            }
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty Approval ");
        }

        private void btnSubmit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (!string.IsNullOrEmpty(TFNumber))
            {
                string errorMessage = cGlobals.BillService.WorkflowSubmit(TFNumber, cGlobals.UserName);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Messages.ShowError(errorMessage);
                }
                else
                {
                    //Messages.ShowInformation("Submit was successful.");
                    LoadApprovalData();
                    btnAddApprover.IsEnabled = true;
                    btnApprove.IsEnabled = true;
                    btnInquiry.IsEnabled = true;
                    btnReject.IsEnabled = true;
                    btnReply.IsEnabled = true;
                    approval_btn_clicked = true;
                }
            }
            else
            {
                Messages.ShowInformation("TF number is invalid.");
            }
        }



        private void btnApprove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (!string.IsNullOrEmpty(TFNumber))
            {
                //this.CurrentBusObj.LoadTable("general");
                string Approver = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approver"].ToString();

                if (Approver == "TFApprover")
                {
                    foreach (DataRow r in CurrentBusObj.ObjectData.Tables["credit"].Rows)
                    {
                        if (r["new_contract_id"].ToString() == "0")
                        {
                            Messages.ShowInformation("You cannot approve until all of the New Contract ID's have been filled out on the Credit tab");
                            return;
                        }
                        //if (r["end_date"].ToString() == "1/1/1900 12:00:00 AM" || r["termination_date"].ToString() == "1/1/1900 12:00:00 AM")
                        //{
                        //    Messages.ShowInformation("You cannot approve until locations on old contract have been turned off in the contract folder location tab");
                        //    return;
                        //    //Messages.ShowInformation("Contract/Location " + r["old_contract_id"].ToString() + "/" + r["cs_id"].ToString() + 
                        //    //                         " has not been turned off");
                        //    //return;
                        //}
                        //if (r["termination_reason"].ToString() == "rrr" )
                        //{
                        //    Messages.ShowInformation("You cannot approve until turn off reason has been added to old contract locations that have been turned off");
                        //    return;
                        //}
                    }
                }

                if (Approver == "USCredit")
                {
                    foreach (DataRow r in CurrentBusObj.ObjectData.Tables["credit"].Rows)
                    {
                        if (r["new_receivable_account"].ToString() == "" || r["new_receivable_account"].ToString().StartsWith(" "))
                        //if (r["new_receivable_account"].ToString() <= "0")
                        {
                            Messages.ShowInformation("You cannot approve until all of the New Customer ID's have been filled out on the Credit tab");
                            return;
                        }
                    }
                    foreach (DataRow r in CurrentBusObj.ObjectData.Tables["credit"].Rows)
                    {
                        if (r["date_owner_paid_through"].ToString() == "1/1/1900 12:00:00 AM")
                        {
                            Messages.ShowInformation("You cannot approve until all of the Last Service Period have been filled out on the Credit tab");
                            return;
                        }
                    }
                }

                string errorMessage = cGlobals.BillService.WorkflowApprove(TFNumber, cGlobals.UserName);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Messages.ShowError(errorMessage);
                }
                else
                {
                    //Messages.ShowInformation("Approve was successful.");
                    LoadApprovalData();
                    approval_btn_clicked = true;
                }
            }
            else
            {
                Messages.ShowInformation("TF number is invalid.");
            }
        }

        private void btnInquiry_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            status = 4;
            LaunchWorkflowEmail((Button)sender);
            LoadApprovalData();
            approval_btn_clicked = true;
        }

        private void btnReply_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            status = 9;
            LaunchWorkflowEmail((Button)sender);
            LoadApprovalData();
            approval_btn_clicked = true;
        }

        private void btnReject_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            status = 3;
            LaunchWorkflowEmail((Button)sender);
            LoadApprovalData();
            btnAddApprover.IsEnabled = false;
            btnApprove.IsEnabled = false;
            btnInquiry.IsEnabled = false;
            btnReject.IsEnabled = false;
            btnReply.IsEnabled = false;
            approval_btn_clicked = true;
        }

        private void btnAddApprover_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (!string.IsNullOrEmpty(TFNumber))
            {
                cGlobals.ReturnParms.Clear();
                cGlobals.ReturnParms.Add(TFNumber);
                cGlobals.ReturnParms.Add(btnAddApprover.Name);

                RoutedEventArgs args = new RoutedEventArgs();
                args.RoutedEvent = EventAggregator.GeneratedClickEvent;
                args.Source = sender;
                EventAggregator.GeneratedClickHandler(this, args);
                LoadApprovalData();
            }
            else
            {
                Messages.ShowInformation("TF number is invalid.");
            }
        }

        private void LaunchWorkflowEmail(Button button)
        {
            if (!string.IsNullOrEmpty(TFNumber))
            {
                //Dictionary<string, object> outputValues = cGlobals.BillService.LaunchWorkflowEmail(TFNumber, cGlobals.UserName, status);
                DataTable outputValues = cGlobals.BillService.LaunchWorkflowEmail(TFNumber, cGlobals.UserName, status);

                if (!string.IsNullOrEmpty(outputValues.Rows[0][errorMessageParameter].ToString()))
                {
                    Messages.ShowError(outputValues.Rows[0][errorMessageParameter].ToString());
                }
                else
                {
                    if (idgAdjustments.xGrid.SelectedItems.Records != null && idgAdjustments.xGrid.SelectedItems.Records.Count > 0)
                    {
                        cGlobals.ReturnParms.Clear();
                        cGlobals.ReturnParms.Add(TFNumber);
                        cGlobals.ReturnParms.Add(button.Name);
                        cGlobals.ReturnParms.Add(outputValues.Rows[0][typeParameter]);
                        cGlobals.ReturnParms.Add(outputValues.Rows[0][classParameter]);

                        //These parameters will not be added to the business object however they are needed 
                        //for the popup subject text so it has to be at a greater index than the 
                        //parameters passed into the business object.  The EventAggregator 
                        //will take the first parameter values and match them with they keys
                        //listed in the parameters element in the mappings section of the app.config 
                        cGlobals.ReturnParms.Add(status);
                        cGlobals.ReturnParms.Add(outputValues.Rows[0][subjectParameter]);

                        DataRecord record = (DataRecord)idgAdjustments.xGrid.SelectedItems.Records[0];
                        DataRow row = ((DataRowView)record.DataItem).Row;
                        cGlobals.ReturnParms.Add(row[workflowId]);

                        RoutedEventArgs args = new RoutedEventArgs();
                        args.RoutedEvent = EventAggregator.GeneratedClickEvent;
                        args.Source = button;
                        EventAggregator.GeneratedClickHandler(this, args);
                    }
                    else
                    {
                        Messages.ShowInformation("Please select a record from the grid.");
                    }
                }
            }
            else
            {
                Messages.ShowInformation("TF number is invalid.");
            }
        }
    }
}
