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
using System.Data;

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for SecurityRole.xaml
    /// </summary>
    public partial class SecurityRole : ScreenBase, IScreen
    {

        private int RoleID;

        public SecurityRole() : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load bus obj parms, used in multiple places
        /// </summary>
        private void loadParms()
        {
            this.CurrentBusObj.Parms.AddParm("@role_id", RoleID);
        }


        #region IScreen Implementation
        public void Init(cBaseBusObject businessObject)
        {
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "Role";

            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            //add the tabs
            TabCollection.Add(RoleMembersTab);
            TabCollection.Add(RoleAccessTab);

            // if there are parameters load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                //load Custnum from bus obj
                RoleID = Int32.Parse(this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString());
            }
            else
            {
                RoleID = -1;
            }

            loadParms();

            // load the data
            this.Load();
        }

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }
        #endregion

        private void txtRoleID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CallRoleDialog();
        }

        private void CallRoleDialog()
        {
            cGlobals.ReturnParms.Clear();
            var roleLookup = new RoleLookup();
            var result = roleLookup.ShowDialog();

            if (result != null && result == true)
            {
                if (cGlobals.ReturnParms.Count != 0)
                {
                    txtRoleID.Text = cGlobals.ReturnParms[0].ToString();
                    RoleID = int.Parse(txtRoleID.Text);
                    txtRoleDescription.Text = cGlobals.ReturnParms[1].ToString();
                    //change parms
                    changeParms();
                    // load the data
                    this.Load();
                }
            }
        }

        private void changeParms()
        {
            this.CurrentBusObj.changeParm("@role_id", RoleID.ToString());
        }

        private void txtRoleDescription_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CallRoleDialog();
        }

        private void txtRoleID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRoleID.Text) && Helper.IsNumeric(txtRoleID.Text))
            {
                var test = Helper.IsNumeric("abc");
                RoleID = int.Parse(txtRoleID.Text);

                DataTable dt = this.CurrentBusObj.ObjectData.Tables["Role"];

                if (dt != null)
                {
                    //Get Role Description
                    var results = (from x in dt.AsEnumerable()
                                   where x.Field<int>("role_id") == RoleID
                                   select x).ToArray();

                    if (results.Count() == 1)
                    {
                        txtRoleDescription.Text = results[0].Field<string>("role_description");

                        //change parms
                        changeParms();
                        // load the data
                        this.Load();
                    }
                    else
                    {
                        Messages.ShowInformation(string.Format("Role ID {0} was not found.", RoleID.ToString()));
                    }
                }
                else
                {
                    Messages.ShowInformation(string.Format("Unable to load role information, please contact support."));
                }
            }
        }

    }
}
