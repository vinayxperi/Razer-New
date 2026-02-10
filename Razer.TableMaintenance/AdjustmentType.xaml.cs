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
using Infragistics.Windows.Editors;

namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for AdjustmentType.xaml
    /// </summary>
    public partial class AdjustmentType : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "AdjustmentType";
        private static readonly string mainTableName = "adjustment_type";
        //Needed for the combobox
        private static readonly string AdjBaseTableName = "adjustment_base_class";
        private static readonly string AdjBaseDisplayPath = "base_class_desc";
        private static readonly string AdjBaseValuePath = "base_class_id";
        //Needed for a combobox
        public ComboBoxItemsProvider cmbBaseClassID { get; set; }

        public string WindowCaption { get { return string.Empty; } }

        public AdjustmentType()
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
            idgAdjustmentType.xGrid.FieldLayoutSettings = layouts;
            idgAdjustmentType.FieldLayoutResourceString = fieldLayoutResource;
            idgAdjustmentType.MainTableName = mainTableName;
            idgAdjustmentType.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load attribute combobox
            if (businessObject.HasObjectData)
            {
                cmbBaseClassID = new ComboBoxItemsProvider();
                cmbBaseClassID.ItemsSource = businessObject.ObjectData.Tables[AdjBaseTableName].DefaultView;
                cmbBaseClassID.ValuePath = AdjBaseValuePath;
                cmbBaseClassID.DisplayMemberPath = AdjBaseDisplayPath;
            }

            idgAdjustmentType.LoadGrid(businessObject, idgAdjustmentType.MainTableName);
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
