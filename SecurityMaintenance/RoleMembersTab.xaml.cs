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
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for RoleMembersTab.xaml
    /// </summary>
    public partial class RoleMembersTab : ScreenBase
    {
        public RoleMembersTab()
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
            MainTableName = "RoleMembers";
            RoleMembers.MainTableName = MainTableName;
            RoleMembers.xGrid.FieldLayoutSettings.AllowDelete = false;
            RoleMembers.xGrid.FieldSettings.AllowEdit = false;
            RoleMembers.WindowZoomDelegate = GridDoubleClickDelegate;
            RoleMembers.SetGridSelectionBehavior(true, false);
            RoleMembers.FieldLayoutResourceString = "RoleMemberGrid";
            GridCollection.Add(RoleMembers);
        }

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        public void GridDoubleClickDelegate()
        {
            cGlobals.ReturnParms.Clear();

            DataRecord r = RoleMembers.ActiveRecord;

            if (r != null)
            {
                SecurityUserTab securityUserTab = new SecurityUserTab();
                cBaseBusObject security = new cBaseBusObject("SecurityEmployee");

                string user_id = r.Cells["user_id"].Value.ToString();
                if (!string.IsNullOrEmpty(user_id))
                {
                    security.Parms.AddParm("@user_id", user_id);
                    security.LoadData();

                    securityUserTab.Init(security);

                    var result = securityUserTab.ShowDialog();
                    if (result != null && result == true)
                    {
                        this.Load();
                    }
                }
            }
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

        private bool IsObjectMemberNew(int object_id, int role_id)
        {
            throw new NotImplementedException();
        }
    }
}
