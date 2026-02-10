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
using System.Data;

namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for ProductItemAuto.xaml
    /// </summary>
    public partial class ProductItemAuto : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ProductItemAutoCreate";
        private static readonly string mainTableName = "product_item_auto_create";

        //Needed for the combobox
        private static readonly string ItemIDsTableName = "product_item";
        private static readonly string ItemIDsDisplayPath = "item_description";
        private static readonly string ItemIDsValuePath = "item_id";
        private static readonly string AncTypeTableName = "ancillary_type";
        private static readonly string AncTypeDisplayPath = "ancillary_name";
        private static readonly string AncTypeValuePath = "ancillary_id";

        //Needed for a combobox
        public ComboBoxItemsProvider cmbItemIDs { get; set; }
        public ComboBoxItemsProvider cmbAncType { get; set; }

        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public ProductItemAuto()
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
            idgProductItemAuto.xGrid.FieldLayoutSettings = layouts;
            idgProductItemAuto.FieldLayoutResourceString = fieldLayoutResource;
            idgProductItemAuto.MainTableName = mainTableName;
            idgProductItemAuto.xGrid.FieldSettings.AllowEdit = true;
            this.Load(businessObject);

            //load comboboxes
            if (businessObject.HasObjectData)
            {
                cmbItemIDs = new ComboBoxItemsProvider();
                cmbItemIDs.ItemsSource = businessObject.ObjectData.Tables[ItemIDsTableName].DefaultView;
                cmbItemIDs.ValuePath = ItemIDsValuePath;
                cmbItemIDs.DisplayMemberPath = ItemIDsDisplayPath;
                cmbAncType = new ComboBoxItemsProvider();
                cmbAncType.ItemsSource = businessObject.ObjectData.Tables[AncTypeTableName].DefaultView;
                cmbAncType.ValuePath = AncTypeValuePath;
                cmbAncType.DisplayMemberPath = AncTypeDisplayPath;
            }

            this.Load(businessObject);

            idgProductItemAuto.LoadGrid(businessObject, idgProductItemAuto.MainTableName);
            idgProductItemAuto.RecordActivatedDelegate = RowBeingClicked;
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
        public void RowBeingClicked()
        {
            //MessageBox.Show("Index = ", idgStateSalesTaxAcct.xGrid.ActiveRecord.Index.ToString());

            if (idgProductItemAuto.xGrid.ActiveRecord.Index < 0)
            {

                //idgProductItemAuto.xGrid.FieldLayouts[0].Fields["fkey_id"].Settings.AllowEdit = true;
                idgProductItemAuto.xGrid.FieldLayouts[0].Fields["product_item_id"].Settings.AllowEdit = true;
            }
            else
            {
                //idgProductItemAuto.xGrid.FieldLayouts[0].Fields["fkey_id"].Settings.AllowEdit = false;
                idgProductItemAuto.xGrid.FieldLayouts[0].Fields["product_item_id"].Settings.AllowEdit = false;
            }
        }
 
     }
}
