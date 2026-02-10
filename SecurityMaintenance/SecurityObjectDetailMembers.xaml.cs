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

namespace SecurityMaintenance
{
    /// <summary>
    /// Interaction logic for SecurityObjectDetailMembers.xaml
    /// </summary>
    public partial class SecurityObjectDetailMembers : DialogBase
    {
        public SecurityObjectDetailMembers()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.MainTableName = "SecurityObjectDetailMembersAll";
            this.CurrentBusObj = businessObject;

            DetailMembers.MainTableName = MainTableName;
            DetailMembers.xGrid.FieldLayoutSettings.AllowDelete = false;

            DetailMembers.SetGridSelectionBehavior(true, false);
            DetailMembers.FieldLayoutResourceString = "ObjectDetailMemberGrid";

            DetailMembers.WindowZoomDelegate = GridDoubleClickDelegate;

            GridCollection.Add(DetailMembers);

            this.LoadExistingData();
        }

        private void GridDoubleClickDelegate()
        {
            cGlobals.ReturnParms.Clear();

            DataRecord r = DetailMembers.ActiveRecord;

            if (r != null)
            {
                var dr = (r.DataItem as DataRowView).Row;
                cGlobals.ReturnParms.Add(dr);

                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
