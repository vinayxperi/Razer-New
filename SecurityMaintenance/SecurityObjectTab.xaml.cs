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
using RazerInterface;
using RazerBase.Interfaces;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for SecurityObjectTab.xaml
    /// </summary>
    public partial class SecurityObjectTab : DialogBase, IScreen, IPreBindable
    {
        public string WindowCaption { get; private set; }

        public bool IsNewRecord { get; set; }

        public bool IsLookup
        {
            get { return (bool)GetValue(IsLookupProperty); }
            set { SetValue(IsLookupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLookup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLookupProperty =
            DependencyProperty.Register("IsLookup", typeof(bool), typeof(SecurityObjectTab), new UIPropertyMetadata(false));

        private bool HasChanges
        {
            get
            {
                bool TF = false;
                if (this.CurrentBusObj != null
                    && (this.CurrentBusObj.ObjectData.Tables[MainTableName].GetChanges() !=null || this.CurrentBusObj.ObjectData.Tables[rObjectMembers.MainTableName].GetChanges() !=null))
                {
                    TF = true;
                }
                return TF;
            }
        }

        public SecurityObjectTab()
        {
            InitializeComponent();
        }

        public void PreBind()
        {
            if (this.CurrentBusObj.HasObjectData)
            {
                cmbParentName.SetBindingExpression("object_id", "name", this.CurrentBusObj.ObjectData.Tables["SecurityObjectMain"]);
                cmbObjectType.SetBindingExpression("code_value", "code_value", this.CurrentBusObj.ObjectData.Tables["SecurityObjectTypeLookup"]);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            txtObjectName.Text = txtObjectName.Text.Trim();
            txtAssociatedMenuItem.Text = txtAssociatedMenuItem.Text.Trim();
            if (HasChanges)
            {
                if (this.CurrentBusObj.Save())
                {
                    DialogResult = true;
                }
                else
                {
                    DialogResult = false;
                    Messages.ShowInformation("Unable to save changes.");
                }
            }
            else
            {
                DialogResult = false;
            }

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

        public void Init(cBaseBusObject businessObject)
        {
            WindowCaption = "Object Detial";
            this.MainTableName = "SecurityObjectDetail";
            this.CurrentBusObj = businessObject;
            PreBind();

            rObjectMembers.MainTableName = "SecurityObjectDetailMembers";
            rObjectMembers.xGrid.FieldLayoutSettings.AllowDelete = !IsLookup;


            if (!IsLookup)
            {
                rObjectMembers.ContextMenuAddDelegate = AddSecurityObjectDetailMember;
                rObjectMembers.ContextMenuAddDisplayName = "Add New Member Association";

                rObjectMembers.ContextMenuRemoveDelegate = RemoveSecurityObjectDetailMember;
                rObjectMembers.ContextMenuRemoveDisplayName = "Remove Member Association"; 
            }
            
            rObjectMembers.SetGridSelectionBehavior(IsLookup, false);
            rObjectMembers.FieldLayoutResourceString = "ObjectDetailMemberGrid";

            GridCollection.Add(rObjectMembers);

            if (IsLookup)
            {
                txtAssociatedMenuItem.IsReadOnly = true;
                txtObjectID.IsReadOnly = true;
                txtObjectName.IsReadOnly = true;
                cmbObjectType.IsEnabled = false;
                cmbParentName.IsEnabled = false;
            }

            this.Load();

            if (IsNewRecord)
            {
                DataTable dt = this.CurrentBusObj.ObjectData.Tables[MainTableName];
                if (dt!=null)
                {
                    dt.Rows.Add(dt.NewRow());
                }
            }
        }

        private void AddSecurityObjectDetailMember()
        {
            SecurityObjectDetailMembers securityObjectDetailMembers = new SecurityObjectDetailMembers();
            securityObjectDetailMembers.Init(this.CurrentBusObj);

            var result = securityObjectDetailMembers.ShowDialog();
            if (result != null && result == true)
            {
                if (cGlobals.ReturnParms.Count != 0)
                {
                    DataRow newRow = (cGlobals.ReturnParms[0] as DataRow);
                    DataTable dt = (rObjectMembers.xGrid.DataSource as DataView).Table;
                    newRow.SetAdded();
                    dt.ImportRow(newRow);
                }
            }
        }

        private void RemoveSecurityObjectDetailMember()
        {
            DataRecord r = rObjectMembers.ActiveRecord;
            if (r != null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;

                if (row != null)
                {
                    row.Delete();
                }
            }
        }

        private void txtAssociatedMenuItem_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}