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

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for SecurityObjectLookup.xaml
    /// </summary>
    public partial class SecurityObjectLookup : DialogBase
    {
        public SecurityObjectLookup() :
            base()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            // set the businessObject
            this.CurrentBusObj = new cBaseBusObject("SecurityObjectMaintenance");

            this.CurrentBusObj.Parms.AddParm("@object_id", 0);
            this.CurrentBusObj.Parms.AddParm("@code_name", "SecurityObjectType");

            // load the data
            this.Load();

            securityObjectMaintenanceTab1.Load(this.CurrentBusObj);
            
        }
    }
}
