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
using System.Data;

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for SecurityUserTab.xaml
    /// </summary>
    public partial class SecurityUserTab : DialogBase, IScreen, IPreBindable
    {
        public string WindowCaption { get; private set; }

        public bool IsNewRecord { get; set; }

        private bool HasChanges
        {
            get
            {
                bool TF = false;
                if (this.CurrentBusObj != null
                    && this.CurrentBusObj.ObjectData.Tables.IndexOf(MainTableName) != -1
                    && this.CurrentBusObj.ObjectData.Tables[MainTableName].GetChanges() != null)
                {
                    TF = true;
                }
                return TF;
            }
        }

        public SecurityUserTab()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            WindowCaption = "User Detail";
            this.MainTableName = "SecurityUserDetail";
            this.CurrentBusObj = businessObject;
            PreBind();

            if (IsNewRecord)
            {
                txtUserID.IsReadOnly = false;
                DataTable dt = this.CurrentBusObj.ObjectData.Tables[MainTableName];
                if (dt != null)
                {
                    dt.Rows.Add(dt.NewRow());
                    this.DataContext = this.CurrentBusObj.ObjectData.Tables[MainTableName].DefaultView;
                }
            }
            else
            {
                this.Load();
            }
        }

        public void PreBind()
        {
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                this.cmbRole.SetBindingExpression("role_id", "role_description", this.CurrentBusObj.ObjectData.Tables["SecurityRolesLookup"]);
                this.cmbRole.SelectedValue = 31;
                this.cmbSupervisor.SetBindingExpression("user_id", "user_name", this.CurrentBusObj.ObjectData.Tables["SecurityUsersdddw"]);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            bool Error = false;
            if (HasChanges)
            {
                if (txtEmail.Text == "" || txtUserID.Text == "" || txtUserName.Text == "" || cmbRole.SelectedText == "") 
                {
                    Messages.ShowInformation("UserID, Email address, User Name and Role are required.");
                    Error = true;
                }
                if (!Error)
                {
                    this.CurrentBusObj.changeParm("@user_id", txtUserID.Text);
                    if (this.CurrentBusObj.SaveTable(MainTableName))
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        DialogResult = false;
                        Messages.ShowInformation("Unable to save changes.");
                    }
                }
            }
            else
            {
                DialogResult = false;
            }
            if (!Error)
                this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (HasChanges)
            {
                this.CurrentBusObj.ObjectData.Tables[MainTableName].RejectChanges();
            }
            DialogResult = false;
            this.Close();
        }
    }
}
