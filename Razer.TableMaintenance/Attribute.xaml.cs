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
    /// Interaction logic for Attribute.xaml
    /// </summary>
    public partial class Attribute : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "Attribute";
        private static readonly string mainTableName = "attribute";
        //Needed for the combobox
        private static readonly string attrtypeTableName = "attribute_type";
        private static readonly string attrtypeDisplayPath = "attribute_type";
        private static readonly string attrtypeValuePath = "attribute_type_id";

        //Needed for a combobox
        public ComboBoxItemsProvider cmbAttrType { get; set; }

        public string WindowCaption { get { return string.Empty; } }

        public Attribute()
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
            idgAttribute.xGrid.FieldLayoutSettings = layouts;
            idgAttribute.FieldLayoutResourceString = fieldLayoutResource;
            idgAttribute.MainTableName = mainTableName;
            idgAttribute.xGrid.FieldSettings.AllowEdit = true;
            this.Load(businessObject);

            //load remit to combobox
            if (businessObject.HasObjectData)
            {
                cmbAttrType = new ComboBoxItemsProvider();
                cmbAttrType.ItemsSource = businessObject.ObjectData.Tables[attrtypeTableName].DefaultView;
                cmbAttrType.ValuePath = attrtypeValuePath;
                cmbAttrType.DisplayMemberPath = attrtypeDisplayPath;
            }

            this.Load(businessObject);

            idgAttribute.LoadGrid(businessObject, idgAttribute.MainTableName);
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
