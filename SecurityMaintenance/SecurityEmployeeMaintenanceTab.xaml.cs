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
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter;
using System.Data;
using Infragistics.Windows.Controls;

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for SecurityEmployeeMaintenanceTab.xaml
    /// </summary>
    public partial class SecurityEmployeeMaintenanceTab : ScreenBase
    {
        public ComboBoxItemsProvider RoleCombo { get; set; }


        public bool IsLookup
        {
            get { return (bool)GetValue(IsLookupProperty); }
            set { SetValue(IsLookupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLookup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLookupProperty =
            DependencyProperty.Register("IsLookup", typeof(bool), typeof(SecurityEmployeeMaintenanceTab), new UIPropertyMetadata(false));

        
        public SecurityEmployeeMaintenanceTab()
        {
            InitializeComponent();

            Init();
        }

        public void Init()
        {
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "SecurityUsers";
            EmployeeMaintenance.MainTableName = MainTableName;
            EmployeeMaintenance.xGrid.FieldLayoutSettings.AllowDelete = !IsLookup;

            //assign delegate for displaying inactive users
            //EmployeeMaintenance.ContextMenuAddDelegate = EmployeeMaintenanceGridShowInactive;
            EmployeeMaintenance.ContextMenuGenericDelegate1 = EmployeeMaintenanceGridShowInactive;
            EmployeeMaintenance.ContextMenuGenericDisplayName1 = "Show Inactive Users";
            EmployeeMaintenance.ContextMenuGenericIsVisible1 = true;
            
            //assign delegate for hiding inactive users
            //EmployeeMaintenance.ContextMenuAddDelegate = EmployeeMaintenanceGridHideInactive;
            EmployeeMaintenance.ContextMenuGenericDelegate2 = EmployeeMaintenanceGridHideInactive;
            EmployeeMaintenance.ContextMenuGenericDisplayName2 = "Hide Inactive Users";
            EmployeeMaintenance.ContextMenuGenericIsVisible2 = true;

            EmployeeMaintenance.ContextMenuAddIsVisible = false;
            EmployeeMaintenance.ContextMenuRemoveIsVisible = false;

            EmployeeMaintenance.WindowZoomDelegate = GridDoubleClickDelegate;
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
            }
            else
            {
            if (!IsLookup)
            {
                //assign delegate for adding contacts
                EmployeeMaintenance.ContextMenuAddDelegate = EmployeeMaintenanceGridAddDelegate;
                EmployeeMaintenance.ContextMenuAddDisplayName = "Add User Association";

                //assign delegate for removing contacts
                EmployeeMaintenance.ContextMenuRemoveDelegate = EmployeeMaintenanceGridRemoveDelegate;
                EmployeeMaintenance.ContextMenuRemoveDisplayName = "Remove User Association";

                ////assign delegate for displaying inactive users
                //EmployeeMaintenance.ContextMenuAddDelegate = EmployeeMaintenanceGridShowInactive;
                //EmployeeMaintenance.ContextMenuGenericDelegate1 = EmployeeMaintenanceGridShowInactive;
                //EmployeeMaintenance.ContextMenuGenericDisplayName1 = "Show Inactive Users";
                //EmployeeMaintenance.ContextMenuGenericIsVisible1 = true;

                ////assign delegate for hiding inactive users
                //EmployeeMaintenance.ContextMenuAddDelegate = EmployeeMaintenanceGridHideInactive;
                //EmployeeMaintenance.ContextMenuGenericDelegate2 = EmployeeMaintenanceGridHideInactive;
                //EmployeeMaintenance.ContextMenuGenericDisplayName2 = "Hide Inactive Users";
                //EmployeeMaintenance.ContextMenuGenericIsVisible2 = true;
            }
        }
            EmployeeMaintenance.SetGridSelectionBehavior(IsLookup, false);
            EmployeeMaintenance.FieldLayoutResourceString = "EmployeeMaintenanceGrid";

            GridCollection.Add(EmployeeMaintenance);

            //RES 3/6/14 filter out inactive users when window is opened
            var filter = new RecordFilter();
            filter.FieldName = "role_description";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, "InActive"));
            EmployeeMaintenance.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
        }

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        public void GridDoubleClickDelegate()
        {
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                return;
            }
            cGlobals.ReturnParms.Clear();

            DataRecord r = EmployeeMaintenance.ActiveRecord;

            if (r != null)
            {
                if (IsLookup)
                {
                    var dr = (r.DataItem as DataRowView).Row;
                    cGlobals.ReturnParms.Add(dr);

                    DialogBase myParent = UIHelper.FindVisualParent<DialogBase>(this);

                    if (myParent != null)
                    {
                        myParent.DialogResult = true;
                        myParent.Close();
                    }
                }
                else
                {
                    SecurityUserTab securityUserTab = new SecurityUserTab();
                    SecurityEmployee myParent = UIHelper.FindVisualParent<SecurityEmployee>(this);

                    if (myParent != null)
                    {
                        string user_id = r.Cells["user_id"].Value.ToString();
                        if (!string.IsNullOrEmpty(user_id))
                        {
                            myParent.UserID = user_id;
                            this.CurrentBusObj.changeParm("@user_id", user_id);
                            this.CurrentBusObj.LoadData();

                            securityUserTab.Init(this.CurrentBusObj);

                            var result = securityUserTab.ShowDialog();
                            if (result != null && result == true)
                            {
                                this.Load();
                            }
                        }
                    }
                }
            }
        }

        private void EmployeeMaintenanceGridAddDelegate()
        {
            SecurityUserTab securityUserTab = new SecurityUserTab();
            securityUserTab.IsNewRecord = true;

            SecurityEmployee myParent = UIHelper.FindVisualParent<SecurityEmployee>(this);

            if (myParent != null)
            {
                string user_id = "";
                myParent.UserID = user_id;
                this.CurrentBusObj.changeParm("@user_id", user_id);
                this.CurrentBusObj.LoadData();

                securityUserTab.Init(this.CurrentBusObj);

                var result = securityUserTab.ShowDialog();
                if (result != null && result == true)
                {
                    this.Load();
                }
            }
        }

        private void EmployeeMaintenanceGridRemoveDelegate()
        {
            DataRecord r = EmployeeMaintenance.ActiveRecord;
            if (r != null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;

                if (row != null)
                {
                    row.Delete();
                }
            }
        }

        private void EmployeeMaintenanceGridShowInactive()
        {
            //EmployeeMaintenance.ClearFilter();
            var filter = new RecordFilter();
            filter.FieldName = "role_description";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, "FF"));

            //Apply the filter to the grid
            EmployeeMaintenance.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
        }
        private void EmployeeMaintenanceGridHideInactive()
        {
            var filter = new RecordFilter();
            filter.FieldName = "role_description";
            filter.Conditions.Add(new ComparisonCondition(ComparisonOperator.NotEquals, "InActive"));

            //Apply the filter to the grid
            EmployeeMaintenance.xGrid.FieldLayouts[0].RecordFilters.Add(filter);
        }

        public void Load(cBaseBusObject CurrentBusObj)
        {
            this.CurrentBusObj = CurrentBusObj;
            base.Load(CurrentBusObj);
        }

        public override void New()
        {
            EmployeeMaintenanceGridAddDelegate();
        }

        public override void Delete()
        {
            EmployeeMaintenanceGridRemoveDelegate();
        }
    }
}
