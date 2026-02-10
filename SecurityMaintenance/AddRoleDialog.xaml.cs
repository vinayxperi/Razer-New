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
using System.Data;

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for AddRoleDialog.xaml
    /// </summary>
    public partial class AddRoleDialog : DialogBase
    {

        public AddRoleDialog(cBaseBusObject CurrentBusObj)
        {
            InitializeComponent();

            this.CurrentBusObj = CurrentBusObj;
            this.MainTableName = "Role";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRoleName.Text))
            {
                Messages.ShowInformation("You must provide a Role name to continue.");
            }
            else
            {
                int results = (from x in this.CurrentBusObj.ObjectData.Tables[this.MainTableName].AsEnumerable()
                               where x.Field<string>("role_description").ToLower() == txtRoleName.Text.ToLower()
                               select x).Count();

                if (results == 0)
                {
                    DataTable roles = this.CurrentBusObj.ObjectData.Tables[this.MainTableName];
                    DataRow newRole = roles.NewRow();
                    newRole["role_description"] = txtRoleName.Text;
                    roles.Rows.Add(newRole);
                    this.CurrentBusObj.Save();
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    Messages.ShowInformation(string.Format("The role, {0}, already exists.", txtRoleName.Text));
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentBusObj.ObjectData.Tables[this.MainTableName].RejectChanges();
            this.DialogResult = false;
            this.Close();
        }
    }
}
