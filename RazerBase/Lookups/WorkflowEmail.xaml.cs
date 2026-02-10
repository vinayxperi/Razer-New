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

namespace RazerBase.Lookups
{
    /// <summary>
    /// Interaction logic for WorkflowEmail.xaml
    /// </summary>
    public partial class WorkflowEmail : DialogBase, IScreen
    {
        private static readonly string mainTableName = "wf_user_email";
        private static readonly string userDisplayField = "user_name";
        private static readonly string userValueField = "email_address";
        private static readonly string workflowIdParameter = "@wf_id";
        private static readonly string documentIdParameter = "@document_id";
        private static readonly string userIdParameter = "@user_Id";        
        private static readonly string subjectParameter = "@email_subject";
        private static readonly string bodyParameter = "@email_body";
        private static readonly string emailToParameter = "@email_to";
        private static readonly string errorMessageParameter = "@error_message";
        private static readonly string typeParameter = "@wf_type";
        

        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public WorkflowEmail()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;
            this.Load();

            if ((int)cGlobals.ReturnParms[4] != 3)
            {
                cmbUser.SetBindingExpression(userValueField, userDisplayField, CurrentBusObj.ObjectData.Tables[mainTableName]);
            }
            else
            {
                cmbUser.Visibility = System.Windows.Visibility.Collapsed;
            }

            txtSubject.Text = cGlobals.ReturnParms[5].ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            cGlobals.ReturnParms.Clear();
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int workflowId = 0, workflowStatus = 0;
            int.TryParse(cGlobals.ReturnParms[6].ToString(), out workflowId);
            int.TryParse(cGlobals.ReturnParms[4].ToString(), out workflowStatus);
            
            string subject = txtSubject.Text ?? string.Empty;
            string message = txtMessage.Text ?? string.Empty;
            string emailTo = cmbUser.SelectedValue == null ? string.Empty : cmbUser.SelectedValue.ToString();


            message = message.Replace("'", "''");

            subject = subject.Replace("'", "''");
            

            string errorMessage = cGlobals.BillService.SendWorkflowEmail(cGlobals.ReturnParms[0].ToString(), cGlobals.UserName, workflowId, workflowStatus, subject, message, emailTo);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Messages.ShowError(errorMessage);
            }
            else
            {
                Messages.ShowInformation("Email was sent.");
            }

            cGlobals.ReturnParms.Clear();
            this.Close();
        }
    }
}
