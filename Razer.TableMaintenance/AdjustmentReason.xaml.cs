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
    /// Interaction logic for AdjustmentReason.xaml
    /// </summary>
    public partial class AdjustmentReason : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "AdjustmentReason";
        private static readonly string mainTableName = "adjustment_reason_code";

        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public AdjustmentReason()
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
            idgAdjustmentReason.xGrid.FieldLayoutSettings = layouts;
            idgAdjustmentReason.FieldLayoutResourceString = fieldLayoutResource;
            idgAdjustmentReason.MainTableName = mainTableName;
            idgAdjustmentReason.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgAdjustmentReason.LoadGrid(businessObject, idgAdjustmentReason.MainTableName);  
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
