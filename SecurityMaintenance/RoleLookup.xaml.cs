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
using Infragistics.Windows.DataPresenter;

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for RoleLookup.xaml
    /// </summary>
    public partial class RoleLookup : DialogBase
    {
        public int RoleID { get; set; }

        public RoleLookup()
        {
            InitializeComponent();

            Init();
        }

        public void Init()
        {
            // set the businessObject
            this.CurrentBusObj = new cBaseBusObject("RoleMaintenance");

            RoleID = 0;

            this.CurrentBusObj.Parms.AddParm("@role_id", 0);

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "Role";
            RolesGrid.MainTableName = this.MainTableName;
            windowCaption = this.MainTableName;
            RolesGrid.xGrid.FieldLayoutSettings.AllowDelete = false;
            RolesGrid.xGrid.FieldSettings.AllowEdit = false;

            RolesGrid.WindowZoomDelegate = GridDoubleClickDelegate;

            //assign delegate for adding roles
            RolesGrid.ContextMenuAddDelegate = RolesGridAddDelegate;
            RolesGrid.ContextMenuAddDisplayName = "Add New Role";

            //assign delegate for removing roles
            //RolesGrid.ContextMenuRemoveDelegate = RolesGridRemoveDelegate;
            //RolesGrid.ContextMenuRemoveDisplayName = "Remove Role";

            RolesGrid.SetGridSelectionBehavior(true, false);
            RolesGrid.FieldLayoutResourceString = "RoleGrid";

            GridCollection.Add(RolesGrid);

            // load the data
            this.Load();
        }

        private void RolesGridAddDelegate()
        {
            AddRoleDialog addRoleDialog = new AddRoleDialog(this.CurrentBusObj);
            bool? dr = addRoleDialog.ShowDialog();

            if (dr!=null && dr==true)
            {
                this.Load();
            }
        }

        public void GridDoubleClickDelegate()
        {
            DataRecord r = RolesGrid.ActiveRecord;
            //Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            //If error condition is received when retrieving selected row then the row of the currently active cell is used.

            if (r!=null)
            {
                try
                {
                    r = (Infragistics.Windows.DataPresenter.DataRecord)RolesGrid.xGrid.SelectedItems.Records[0];
                }
                catch { return; /*Do Nothing*/ }
            }
            
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(r.Cells["role_id"].Value);
            cGlobals.ReturnParms.Add(r.Cells["role_description"].Value);

            this.DialogResult = true;

            this.Close();
        }

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }

        private void GetLookupVals()
        {
            //obj = new cBaseBusObject("RoleMaintenance");
            if (cGlobals.ReturnParms.Count > 0)
            {
                object ParmValue = cGlobals.ReturnParms[0].ToString();
                if (Helper.IsNumeric(ParmValue))
                {

                }
                cGlobals.ReturnParms.Clear();
            }

        }
    }
}
