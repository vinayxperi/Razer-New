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

namespace Razer.CustomInvoice
{
    /// <summary>
    /// Interaction logic for CustomInvoiceApproval.xaml
    /// </summary>
    public partial class CustomInvoiceApproval : ScreenBase
    {
        private static readonly string mainTableName = "approval";
        private static readonly string fieldLayoutResource = "CustomInvoiceApproval";        
        private static readonly string documentIdParameter = "@document_id";                
        private static readonly string typeParameter = "@wf_type";
        private static readonly string classParameter = "@wf_class";        
        private static readonly string subjectParameter = "@subject";
        private static readonly string errorMessageParameter = "@error_message";
        private static readonly string workflowId = "wf_id";
        //RES 11/25/19 verify attachments exist
        //string BadAttachment = "N";
        //private static readonly string ExternalCharId = "@external_char_id";
        //private static readonly string AttachmentType = "@attachment_type";

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

        public CustomInvoiceApproval()
        {
            InitializeComponent();

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;            

            this.MainTableName = mainTableName;
            idgAdjustments.xGrid.FieldLayoutSettings = layouts;
            idgAdjustments.FieldLayoutResourceString = fieldLayoutResource;
            idgAdjustments.MainTableName = mainTableName;
            idgAdjustments.xGrid.FieldSettings.AllowEdit = false;
            idgAdjustments.xGrid.FieldLayoutSettings.AllowDelete = false;
            idgAdjustments.SetGridSelectionBehavior(true, false);            
        }

        private void LoadApprovalData()
        {
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                approvalBusinessObject.Parms.ClearParms(); 
                approvalBusinessObject.Parms.AddParm(documentIdParameter, invoiceNumber);
                //RES 11/25/19 verify attachments exist
                //approvalBusinessObject.Parms.AddParm(ExternalCharId, invoiceNumber);
                //approvalBusinessObject.Parms.AddParm(AttachmentType, "MATTACH");
                approvalBusinessObject.LoadData();
                idgAdjustments.LoadGrid(approvalBusinessObject, mainTableName);
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
            // RES 9/9/19 Do not allow user to submit if there are bad attachments
            //approvalBusinessObject.Parms.AddParm(documentIdParameter, invoiceNumber);
            //approvalBusinessObject.LoadTable("validate");
            //approvalBusinessObject.Parms.ClearParms();
            //if (approvalBusinessObject.ObjectData.Tables["validate"].Rows[0]["bad_attachments"].ToString() == "Y")
            //{
            //    Messages.ShowError("There are invalid attachments that need to be deleted before the document can be submitted for approval!");
            //    return;
            //}
            //ValidateAttachments();
            //if (BadAttachment == "Y")
            //{
            //    Messages.ShowError("There are invalid attachments that need to be deleted before the document can be submitted for approval!");
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
                    //Messages.ShowInformation("Submit was successful.");
                    LoadApprovalData();
                    btnAddApprover.IsEnabled = true;
                    btnApprove.IsEnabled = true;
                    btnInquiry.IsEnabled = true;
                    btnReject.IsEnabled = true;
                    btnReply.IsEnabled = true;
                   
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
                    //Messages.ShowInformation("Approve was successful.");
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
            btnAddApprover.IsEnabled = false;
            btnApprove.IsEnabled = false;
            btnInquiry.IsEnabled = false;
            btnReject.IsEnabled = false;
            btnReply.IsEnabled = false;
            this.CanExecuteDeleteCommand = true;

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

        //RES 11/25/19 Validate that attachments all exist
        //private void ValidateAttachments()
        //{
        //    //BadAttachment = "N";
        //    approvalBusinessObject.Parms.AddParm(ExternalCharId, invoiceNumber);
        //    approvalBusinessObject.Parms.AddParm(AttachmentType, "MATTACH");
        //    approvalBusinessObject.LoadTable("attachments");
        //    approvalBusinessObject.Parms.ClearParms();

        //    foreach (DataRow dr in approvalBusinessObject.ObjectData.Tables["attachments"].Rows)
        //    {
        //        string PathFile = dr["path"].ToString() + dr["prod_filename"].ToString();
        //        if (!System.IO.File.Exists(PathFile))
        //        {
        //            BadAttachment = "Y";
        //            return;
        //        }
        //    }
        //}

        private void LaunchWorkflowEmail(Button button)
        {
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                //Dictionary<string, object> outputValues = cGlobals.BillService.LaunchWorkflowEmail(invoiceNumber, cGlobals.UserName, status);
                DataTable outputValues = cGlobals.BillService.LaunchWorkflowEmail(invoiceNumber, cGlobals.UserName, status);

                if (!string.IsNullOrEmpty(outputValues.Rows[0][errorMessageParameter].ToString()))
                {
                    Messages.ShowError(outputValues.Rows[0][errorMessageParameter].ToString());
                }
                else
                {
                    if (idgAdjustments.xGrid.SelectedItems.Records != null && idgAdjustments.xGrid.SelectedItems.Records.Count > 0)
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
                Messages.ShowInformation("Invoice number is invalid.");
            }
        }

    }
}
