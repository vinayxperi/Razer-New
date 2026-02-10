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

namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for AdjustmentBaseClass.xaml
    /// </summary>
    public partial class AdjustmentBaseClass : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "AdjustmentBaseClass";
        private static readonly string mainTableName = "adjustment_base_class";
        public string WindowCaption { get { return string.Empty; } }

        public AdjustmentBaseClass()
        {
            InitializeComponent();
        }


        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            layouts.AllowAddNew = true;
            layouts.AddNewRecordLocation = AddNewRecordLocation.OnTop;

            this.CurrentBusObj = businessObject;

            this.MainTableName = mainTableName;
            idgAdjustmentBaseClass.xGrid.FieldLayoutSettings = layouts;
            idgAdjustmentBaseClass.FieldLayoutResourceString = fieldLayoutResource;
            idgAdjustmentBaseClass.MainTableName = mainTableName;
            idgAdjustmentBaseClass.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgAdjustmentBaseClass.LoadGrid(businessObject, idgAdjustmentBaseClass.MainTableName);
        }

        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {

                Messages.ShowInformation("Save Successful");
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }
    }
}
