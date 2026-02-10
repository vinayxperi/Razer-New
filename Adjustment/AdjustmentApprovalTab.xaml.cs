

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Adjustment
{
   
    /// <summary>
    /// This class represents a 'AdjustmentApprovalTab' object.
    /// </summary>
    public partial class AdjustmentApprovalTab : ScreenBase
    {
        private static readonly string mainTableName = "approval";
        private static readonly string fieldLayoutResource = "AdjustmentApproval";
        private static readonly string documentIdParameter = "@document_id";
        private static readonly string typeParameter = "@wf_type";
        private static readonly string classParameter = "@wf_class";
        private static readonly string subjectParameter = "@subject";
        private static readonly string errorMessageParameter = "@error_message";
        private static readonly string workflowId = "wf_id";

        int status = -1;
        cBaseBusObject approvalBusinessObject;

        public string invoiceNumber;

        public cBaseBusObject ApprovalBusinessObject
        {
            get { return approvalBusinessObject; }
            set
            {
                approvalBusinessObject = value;
                LoadApprovalData();
            }
        }

        public AdjustmentApprovalTab()       
        {            
            InitializeComponent();

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;

            this.MainTableName = mainTableName;
            gridAdjApproval.xGrid.FieldLayoutSettings = layouts;
            gridAdjApproval.FieldLayoutResourceString = fieldLayoutResource;
            gridAdjApproval.MainTableName = mainTableName;
            gridAdjApproval.xGrid.FieldSettings.AllowEdit = false;
            gridAdjApproval.xGrid.FieldLayoutSettings.AllowDelete = false;
            gridAdjApproval.SetGridSelectionBehavior(true, false); 
        }

        private void LoadApprovalData()
        {
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                approvalBusinessObject.Parms.AddParm(documentIdParameter, invoiceNumber);
                approvalBusinessObject.LoadData();
                gridAdjApproval.LoadGrid(approvalBusinessObject, mainTableName);
                approvalBusinessObject.Parms.ClearParms();
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
       // RES 9/20/18 Do not allow user tosubmit if there are WHT adjustments that have not been reversed for credit/rebill, debit/rebill, rebill, reverse adjustments
            //if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["approval"].Rows[0]["wht_amount"]) != 0)
            //{
            //    Messages.ShowError("There are WHT adjustment(s) for this document that have not been reversed");
            //    return;
            //}
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                string errorMessage = cGlobals.BillService.WorkflowSubmit(invoiceNumber, cGlobals.UserName);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Messages.ShowError(errorMessage);
                }
                else
                {
                    Messages.ShowInformation("Submit was successful.");
                    btnAddApprover.IsEnabled = true;
                    btnApprove.IsEnabled = true;
                    btnInquiry.IsEnabled = true;
                    btnReject.IsEnabled = true;
                    btnReply.IsEnabled = true;
                    LoadApprovalData();
                }
            }
            else
            {
                Messages.ShowInformation("Invoice number is invalid.");
            }
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                string errorMessage = cGlobals.BillService.WorkflowApprove(invoiceNumber, cGlobals.UserName);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Messages.ShowError(errorMessage);
                }
                else
                {
                    Messages.ShowInformation("Approve was successful.");
                    LoadApprovalData();
                }
            }
            else
            {
                Messages.ShowInformation("Invoice number is invalid.");
            }
        }

        private void btnInquiry_Click(object sender, RoutedEventArgs e)
        {
            status = 4;
            LaunchWorkflowEmail((Button)sender);
            LoadApprovalData();
        }

        private void btnReply_Click(object sender, RoutedEventArgs e)
        {
            status = 9;
            LaunchWorkflowEmail((Button)sender);
            LoadApprovalData();
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            status = 3;
            LaunchWorkflowEmail((Button)sender);
            LoadApprovalData();
        }

        private void btnAddApprover_Click(object sender, RoutedEventArgs e)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                cGlobals.ReturnParms.Clear();
                cGlobals.ReturnParms.Add(invoiceNumber);
                cGlobals.ReturnParms.Add(btnAddApprover.Name);

                RoutedEventArgs args = new RoutedEventArgs();
                args.RoutedEvent = EventAggregator.GeneratedClickEvent;
                args.Source = sender;
                EventAggregator.GeneratedClickHandler(this, args);
                LoadApprovalData();
            }
            else
            {
                Messages.ShowInformation("Invoice number is invalid.");
            }
        }

        private void LaunchWorkflowEmail(Button button)
        {
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                DataTable outputValues = cGlobals.BillService.LaunchWorkflowEmail(invoiceNumber, cGlobals.UserName, status);

                if (!string.IsNullOrEmpty(outputValues.Rows[0][errorMessageParameter].ToString()))
                {
                    Messages.ShowError(outputValues.Rows[0][errorMessageParameter].ToString());
                }
                else
                {
                    if (gridAdjApproval.xGrid.SelectedItems.Records != null && gridAdjApproval.xGrid.SelectedItems.Records.Count > 0)
                    {
                        cGlobals.ReturnParms.Clear();                        
                        cGlobals.ReturnParms.Add(invoiceNumber);
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

                        DataRecord record = (DataRecord)gridAdjApproval.xGrid.SelectedItems.Records[0];
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
                Messages.ShowInformation("Invoice number is invalid.");
            }
        }
    }
}
