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
    /// Interaction logic for ProductItemXref.xaml
    /// </summary>
    public partial class StateSalesTaxAcct : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "StateSalesTaxAcct";
        private static readonly string mainTableName = "state_sales_tax";
        //Needed for the combobox
        private static readonly string StateCodeTableName = "statedddw";
        private static readonly string StateCodeDisplayPath = "description";
        private static readonly string StateCodeValuePath = "state";
        private static readonly string ProductItemTableName = "productItemdddw";
        private static readonly string ProductItemDisplayPath = "item_description";
        private static readonly string ProductItemValuePath = "item_id";
                    
        //Needed for a combobox
        public ComboBoxItemsProvider cmbStateCode { get; set; }
        public ComboBoxItemsProvider cmbProductItem { get; set; }
       
       
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public StateSalesTaxAcct()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //Adds the insert row at the top
            layouts.AllowAddNew = true;
            layouts.AddNewRecordLocation = AddNewRecordLocation.OnTop;

            this.CurrentBusObj = businessObject;

            this.MainTableName = mainTableName;
            idgStateSalesTaxAcct.xGrid.FieldLayoutSettings = layouts;
            idgStateSalesTaxAcct.FieldLayoutResourceString = fieldLayoutResource;
            idgStateSalesTaxAcct.MainTableName = mainTableName;
            idgStateSalesTaxAcct.xGrid.FieldSettings.AllowEdit = true;
            idgStateSalesTaxAcct.RecordActivatedDelegate = RowBeingClicked;

            this.Load(businessObject);
            //load product item combobox
            if (businessObject.HasObjectData)
            {
                cmbProductItem = new ComboBoxItemsProvider();
                cmbProductItem.ItemsSource = businessObject.ObjectData.Tables[ProductItemTableName].DefaultView;
                cmbProductItem.ValuePath = ProductItemValuePath;
                cmbProductItem.DisplayMemberPath = ProductItemDisplayPath;
            }
            //load state code combobox
            if (businessObject.HasObjectData)
            {
                cmbStateCode = new ComboBoxItemsProvider();
                cmbStateCode.ItemsSource = businessObject.ObjectData.Tables[StateCodeTableName].DefaultView;
                cmbStateCode.ValuePath = StateCodeValuePath;
                cmbStateCode.DisplayMemberPath = StateCodeDisplayPath;
            }
           
            this.Load(businessObject);

            idgStateSalesTaxAcct.LoadGrid(businessObject, idgStateSalesTaxAcct.MainTableName);
         
        }

        public void RowBeingClicked()
        {
            //MessageBox.Show("Index = ", idgStateSalesTaxAcct.xGrid.ActiveRecord.Index.ToString());

            if (idgStateSalesTaxAcct.xGrid.ActiveRecord.Index < 0)
            {
               
                idgStateSalesTaxAcct.xGrid.FieldLayouts[0].Fields["state"].Settings.AllowEdit = true;
                idgStateSalesTaxAcct.xGrid.FieldLayouts[0].Fields["product_item_id"].Settings.AllowEdit = true;
            }
            else
            {
                idgStateSalesTaxAcct.xGrid.FieldLayouts[0].Fields["state"].Settings.AllowEdit = false;
                idgStateSalesTaxAcct.xGrid.FieldLayouts[0].Fields["product_item_id"].Settings.AllowEdit = false;
            }
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
