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
    /// Interaction logic for RazerGlobalProductXref.xaml
    /// </summary>
    public partial class RazerGlobalProductXref : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "RazerGlobalXref";
        private static readonly string mainTableName = "main";
        //Needed for product code combobox
        private static readonly string ProductCodeTableName = "products";
        private static readonly string ProductCodeDisplayPath = "product_description";
        private static readonly string ProductCodeValuePath = "product_code";
        //Needed for a combobox
        public ComboBoxItemsProvider cmbProductCode { get; set; }
        //Needed for global product code combobox
        //private static readonly string GlobalProductCodeTableName = "globalproducts";
        //private static readonly string GlobalProductCodeDisplayPath = "global_product_code";
        //private static readonly string GlobalProductCodeValuePath = "global_product_code";
        ////Needed for a combobox
        public ComboBoxItemsProvider cmbGlobalProductCode { get; set; }
        public string WindowCaption
        {
            get { return string.Empty; }
        }


        public RazerGlobalProductXref()
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
            idgXref.xGrid.FieldLayoutSettings = layouts;
            idgXref.FieldLayoutResourceString = fieldLayoutResource;
            idgXref.MainTableName = mainTableName;
            idgXref.xGrid.FieldSettings.AllowEdit = true;
            idgXref.RecordActivatedDelegate = RowBeingClicked;
           

            this.Load(businessObject);
            //load product code combobox
            if (businessObject.HasObjectData)
            {
                

                cmbProductCode = new ComboBoxItemsProvider();
                cmbProductCode.ItemsSource = businessObject.ObjectData.Tables[ProductCodeTableName].DefaultView;
                cmbProductCode.ValuePath = ProductCodeValuePath;
                cmbProductCode.DisplayMemberPath = ProductCodeDisplayPath;

            }
            ////load global product code combobox
            //if (businessObject.HasObjectData)
            //{


            //    cmbGlobalProductCode = new ComboBoxItemsProvider();
            //    cmbGlobalProductCode.ItemsSource = businessObject.ObjectData.Tables[GlobalProductCodeTableName].DefaultView;
            //    cmbGlobalProductCode.ValuePath = GlobalProductCodeValuePath;
            //    cmbGlobalProductCode.DisplayMemberPath = GlobalProductCodeDisplayPath;

            //}
            idgXref.LoadGrid(businessObject, idgXref.MainTableName);
        }

        public void RowBeingClicked()
        {

            if (idgXref.xGrid.ActiveRecord.Index < 0)
            {

                idgXref.xGrid.FieldLayouts[0].Fields["razer_product_code"].Settings.AllowEdit = true;
                idgXref.xGrid.FieldLayouts[0].Fields["global_product_code"].Settings.AllowEdit = true;
            }
            else
            {
                idgXref.xGrid.FieldLayouts[0].Fields["razer_product_code"].Settings.AllowEdit = false;
                 
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
