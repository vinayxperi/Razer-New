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

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for SecurityEmployee.xaml
    /// </summary>
    public partial class SecurityEmployee : ScreenBase, IScreen, IPreBindable
    {

        public string UserID { get; set; }

        public SecurityEmployee()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load bus obj parms, used in multiple places
        /// </summary>
        private void loadParms()
        {
            this.CurrentBusObj.Parms.AddParm("@user_id", UserID);
        }

        #region IScreen Implementation
        public void Init(cBaseBusObject businessObject)
        {
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "SecurityUsers";

            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            //add the tabs
            TabCollection.Add(securityEmployeeMaintenanceTab1);

            UserID = "";

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

        public void PreBind()
        {

        }

        public override void New()
        {
            securityEmployeeMaintenanceTab1.New();
        }

        public override void Delete()
        {
            securityEmployeeMaintenanceTab1.Delete();
        }
    }
}
