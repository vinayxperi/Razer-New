

#region using statements

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
#endregion

namespace BCF
{
    /// <summary>
    /// Interaction logic for BCFApprovalTab.xaml
    /// </summary>   
    public partial class BCFApprovalTab : ScreenBase
    {

        
        private static readonly string mainTableName = "approval";
        private static readonly string fieldLayoutResource = "BCFApproval";
        private static readonly string documentIdParameter = "@document_id";
        private static readonly string typeParameter = "@wf_type";
        private static readonly string classParameter = "@wf_class";
        private static readonly string subjectParameter = "@subject";
        private static readonly string errorMessageParameter = "@error_message";
        private static readonly string workflowId = "wf_id";

        int status = -1;
        cBaseBusObject approvalBusinessObject;

        public string BCFNumber;

        public cBaseBusObject ApprovalBusinessObject
        {
            get { return approvalBusinessObject; }
            set
            {
                approvalBusinessObject = value;
                LoadApprovalData();
            }
        }



        public BCFApprovalTab()
        {
            // This call is required by the designer.
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
            if (this.CurrentBusObj.HasObjectData)
            {
                BCFNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"].ToString();
            }
            
            
            if (!string.IsNullOrEmpty(BCFNumber))
            {
                approvalBusinessObject.Parms.ClearParms();
                approvalBusinessObject.Parms.AddParm(documentIdParameter, BCFNumber);
                approvalBusinessObject.LoadData();
                idgAdjustments.LoadGrid(approvalBusinessObject, mainTableName);
                approvalBusinessObject.Parms.ClearParms();
            }
        }


        private void btnSubmit_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (!string.IsNullOrEmpty(BCFNumber))
            {
                string errorMessage = cGlobals.BillService.WorkflowSubmit(BCFNumber, cGlobals.UserName);

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
                Messages.ShowInformation("BCF number is invalid.");
            }


        }

       

        private void btnApprove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (!string.IsNullOrEmpty(BCFNumber))
            {
                string errorMessage = cGlobals.BillService.WorkflowApprove(BCFNumber, cGlobals.UserName);

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
                Messages.ShowInformation("BCF number is invalid.");
            }


        }

        private void btnInquiry_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            status = 4;
            LaunchWorkflowEmail((Button)sender);
            LoadApprovalData();
        }

        private void btnReply_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            status = 9;
            LaunchWorkflowEmail((Button)sender);
            LoadApprovalData();

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


        }

        private void btnAddApprover_Click(object sender, System.Windows.RoutedEventArgs e)
        {


            if (SecurityContext == AccessLevel.ViewOnly)
            {
                MessageBox.Show("You do not have permission for this action.");
                return;
            }
            if (!string.IsNullOrEmpty(BCFNumber))
            {
                cGlobals.ReturnParms.Clear();
                cGlobals.ReturnParms.Add(BCFNumber);
                cGlobals.ReturnParms.Add(btnAddApprover.Name);

                RoutedEventArgs args = new RoutedEventArgs();
                args.RoutedEvent = EventAggregator.GeneratedClickEvent;
                args.Source = sender;
                EventAggregator.GeneratedClickHandler(this, args);
                LoadApprovalData();
            }
            else
            {
                Messages.ShowInformation("BCF number is invalid.");
            }

        }

        private void LaunchWorkflowEmail(Button button)
        {
            if (!string.IsNullOrEmpty(BCFNumber))
            {
                //Dictionary<string, object> outputValues = cGlobals.BillService.LaunchWorkflowEmail(BCFNumber, cGlobals.UserName, status);
                DataTable outputValues = cGlobals.BillService.LaunchWorkflowEmail(BCFNumber, cGlobals.UserName, status);

                if (!string.IsNullOrEmpty(outputValues.Rows[0][errorMessageParameter].ToString()))
                {
                    Messages.ShowError(outputValues.Rows[0][errorMessageParameter].ToString());
                }
                else
                {
                    if (idgAdjustments.xGrid.SelectedItems.Records != null && idgAdjustments.xGrid.SelectedItems.Records.Count > 0)
                    {
                        cGlobals.ReturnParms.Clear();
                        cGlobals.ReturnParms.Add(BCFNumber);
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
                Messages.ShowInformation("BCF number is invalid.");
            }
        }

    }


}


