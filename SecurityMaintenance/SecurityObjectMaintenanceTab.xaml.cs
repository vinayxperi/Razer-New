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
    /// Interaction logic for SecurityObjectMaintenanceTab.xaml
    /// </summary>
    public partial class SecurityObjectMaintenanceTab : ScreenBase
    {
        
        public bool IsLookup
        {
            get { return (bool)GetValue(IsLookupProperty); }
            set { SetValue(IsLookupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLookup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLookupProperty =
            DependencyProperty.Register("IsLookup", typeof(bool), typeof(SecurityObjectMaintenanceTab),
            new PropertyMetadata()
            {
                DefaultValue = false
            });

        public SecurityObjectMaintenanceTab()
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
            MainTableName = "SecurityObjects";
            ObjectMaintenance.MainTableName = MainTableName;
            ObjectMaintenance.xGrid.FieldLayoutSettings.AllowDelete = !IsLookup;

            ObjectMaintenance.WindowZoomDelegate = GridDoubleClickDelegate;

            ObjectMaintenance.ContextMenuAddDelegate = AddSecurityObject;
            ObjectMaintenance.ContextMenuAddDisplayName = "Add New Object Association";

            ObjectMaintenance.ContextMenuRemoveDelegate = RemoveSecurityObject;
            ObjectMaintenance.ContextMenuRemoveDisplayName = "Remove Object Association";

            ObjectMaintenance.SetGridSelectionBehavior(IsLookup, false);
            ObjectMaintenance.FieldLayoutResourceString = "ObjectMaintenanceGrid";

            GridCollection.Add(ObjectMaintenance);
        }

        private void GridDoubleClickDelegate()
        {
            DataRecord r = ObjectMaintenance.ActiveRecord;
            
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
                SecurityObjectTab objectDetail = new SecurityObjectTab();
                SecurityObject myParent = UIHelper.FindVisualParent<SecurityObject>(this);

                if (myParent!=null)
                {
                    int object_id =0;
                    if (int.TryParse(r.Cells["object_id"].Value.ToString(), out object_id))
                    {
                        myParent.ObjectID = object_id;
                        this.CurrentBusObj.changeParm("@object_id", object_id.ToString());
                        this.CurrentBusObj.LoadData();
                        objectDetail.Init(this.CurrentBusObj);

                        var result = objectDetail.ShowDialog();
                        if (result != null && result == true)
                        {
                            this.Load();
                        }
                    }
                }
            }            
        }

        private void AddSecurityObject()
        {
            SecurityObjectTab objectDetail = new SecurityObjectTab();
            objectDetail.IsNewRecord = true;

            SecurityObject myParent = UIHelper.FindVisualParent<SecurityObject>(this);

            if (myParent != null)
            {
                int object_id = 0;
                myParent.ObjectID = object_id;
                this.CurrentBusObj.changeParm("@object_id", object_id.ToString());
                this.CurrentBusObj.LoadData();
                objectDetail.Init(this.CurrentBusObj);

                var result = objectDetail.ShowDialog();
                if (result != null && result == true)
                {
                    this.Load();
                }
            }
        }

        private void RemoveSecurityObject()
        {
            DataRecord r = ObjectMaintenance.ActiveRecord;
            if (r != null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;

                if (row != null)
                {
                    row.Delete();
                }
            }
        }

        public void Load(cBaseBusObject CurrentBusObj)
        {
            this.CurrentBusObj = CurrentBusObj;
            base.Load(CurrentBusObj);
        }

        public override void New()
        {
            AddSecurityObject();
        }

        public override void Delete()
        {
            RemoveSecurityObject();
        }
    }
}
