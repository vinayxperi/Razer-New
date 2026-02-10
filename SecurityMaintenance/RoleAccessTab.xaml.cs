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
using RazerInterface;
using Infragistics.Windows.Editors;

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for RoleAccessTab.xaml
    /// </summary>
    public partial class RoleAccessTab : ScreenBase, IPreBindable
    {
        public ComboBoxItemsProvider AccessLevelCombo { get; set; }

        public RoleAccessTab()
            : base()
        {
            InitializeComponent();

            Init();
        }

        public void Init()
        {
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "RoleAccess";
            RoleAccess.MainTableName = MainTableName;
            RoleAccess.xGrid.FieldLayoutSettings.AllowDelete = true;
            RoleAccess.xGrid.FieldSettings.AllowEdit = true;

            //RoleAccess.WindowZoomDelegate = GridDoubleClickDelegate;
            
            //assign delegate for adding
            RoleAccess.ContextMenuAddDelegate = RoleAccessGridAddDelegate;
            RoleAccess.ContextMenuAddDisplayName = "Add New Object Association";

            //assign delegate for removing contacts
            RoleAccess.ContextMenuRemoveDelegate = RoleAccessGridRemoveDelegate;
            RoleAccess.ContextMenuRemoveDisplayName = "Remove Object Association";

            RoleAccess.SetGridSelectionBehavior(false, false);
            RoleAccess.FieldLayoutResourceString = "RoleAccessGrid";

            GridCollection.Add(RoleAccess);
        }

        private void GridDoubleClickDelegate()
        {
            //DataRecord r = RoleAccess.xGrid.ActiveCell.Record;

            //cGlobals.ReturnParms.Clear();
            //cGlobals.ReturnParms.Add(r.Cells["object_id"].Value);

            //if (cGlobals.ReturnParms.Count != 0)
            //{
            //    if (Convert.ToBoolean(cGlobals.ReturnParms[0]))
            //    {
            //        this.Load();
            //    }
            //}
            //cGlobals.ReturnParms.Clear();
        }

        public void RoleAccessGridAddDelegate()
        {
            cGlobals.ReturnParms.Clear();
            var SecurityObjectLookup = new SecurityObjectLookup();
            var result = SecurityObjectLookup.ShowDialog();

            //DataTable dt1 = (RoleAccess.xGrid.DataSource as DataView).Table;
            //dt1.AcceptChanges();
            //this.Save();

            int role_id = GetRoleID();

            if (result != null && result == true && cGlobals.ReturnParms.Count != 0)
            {
                DataRow row = cGlobals.ReturnParms[0] as DataRow;
                if (row != null)
                {
                    DataTable dt = (RoleAccess.xGrid.DataSource as DataView).Table;
                    if (dt != null)
                    {

                        int object_id = Convert.ToInt32(row["object_id"]);
                        if (IsObjectRolePermissionNew(object_id, role_id))
                        {
                            DataRow newRow = dt.NewRow();

                            newRow["object_id"] = object_id;
                            newRow["object_type"] = row["object_type"];
                            newRow["object_name"] = row["object_name"];
                            newRow["parent_id"] = row["parent_id"];
                            //newRow["robject_detail_id"] = row["robject_detail_id"];
                            newRow["role_id"] = role_id;
                            newRow["access_level"] = 0;

                            dt.Rows.Add(newRow);
                        }
                    }
                }
            }
            cGlobals.ReturnParms.Clear();
        }

        private int GetRoleID()
        {
            int role_id = 0;

            SecurityRole SecurityRole = UIHelper.FindVisualParent<SecurityRole>(this);

            if (SecurityRole != null)
            {
                role_id = Convert.ToInt32(SecurityRole.txtRoleID.Text);
            }

            return role_id;
        }

        private bool IsObjectRolePermissionNew(int object_id, int role_id)
        {
            DataTable dt = (RoleAccess.xGrid.DataSource as DataView).Table;
            //dt.AcceptChanges();

            int results = (from x in dt.AsEnumerable()
                           where x.Field<int>("object_id") == object_id
                           && x.Field<int>("role_id") == role_id
                           select x).Count();

            return (results == 0 ? true : false);
        }

        public void RoleAccessGridRemoveDelegate()
        {
            DataRecord r = RoleAccess.ActiveRecord;
            if (r!=null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;
                if (row != null)
                {
                    row.Delete();
                    //row.AcceptChanges();
                    //this.Save();
                }
            }
            //RoleAccess.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //DataTable dt = (RoleAccess.xGrid.DataSource as DataView).Table;
           //dt.AcceptChanges();
            //this.CurrentBusObj.LoadData("RoleAccess");
            //RoleAccess.LoadGrid(this.CurrentBusObj, "RoleAccess");
            //this.Save();
            //RES 9/19/14 phase 3.1
            //base.Save();
            //this.Save();
            //RoleAccess.LoadGrid(this.CurrentBusObj, "search");

            //if (RoleAccess.xGrid.SelectedItems.Count() != 0)
            //{
            //    DataPresenterCommands.DeleteSelectedDataRecords.Execute(null, RoleAccess);
            //    //RoleAccess.xGrid.ExecuteCommand(DataPresenterCommands.DeleteSelectedDataRecords);
            //}
        }

        public void PreBind()
        {
            ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
            ip.ItemsSource = Enum.GetValues(typeof(AccessLevel));
            AccessLevelCombo = ip;
        }

        public void Init(cBaseBusObject businessObject)
        {
            throw new NotImplementedException();
        }

        public string WindowCaption
        {
            get { throw new NotImplementedException(); }
        }
    }
}
