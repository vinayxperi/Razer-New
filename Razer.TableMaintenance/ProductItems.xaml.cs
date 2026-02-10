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
using Infragistics.Windows.DataPresenter.ExcelExporter;
using Microsoft.Win32;
using Infragistics.Documents.Excel;
using System.Data;



namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for ProductItems.xaml
    /// </summary>
    public partial class ProductItems : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ProductItemMaintenance";
        private static readonly string mainTableName = "product_item";
        //Needed for the combobox
        private static readonly string productsTableName = "productType";
        private static readonly string productsDisplayPath = "code_value";
        private static readonly string productsValuePath = "fkey_int";
        //Needed for item category combobox
        private static readonly string categoryTableName = "productCategory";
        private static readonly string categoryDisplayPath = "code_value";
        private static readonly string categoryValuePath = "fkey_int";
        //Needed for revenue type combobox
        private static readonly string RevTypeTableName = "revenue_type";
        private static readonly string RevTypeDisplayPath = "revenue_description";
        private static readonly string RevTypeValuePath = "revenue_type_id";
        //Needed for product code combobox
        private static readonly string ProductCodeTableName = "products";
        private static readonly string ProductCodeDisplayPath = "product_description";
        private static readonly string ProductCodeValuePath = "product_code";
        //Needed for a combobox
        public ComboBoxItemsProvider cmbProductItem { get; set; }
        public ComboBoxItemsProvider cmbProductCat { get; set; }
        public ComboBoxItemsProvider cmbRevType { get; set; }
        public ComboBoxItemsProvider cmbProductCode { get; set; }



        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public ProductItems()
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

            idgProductItems.ContextMenuGenericDelegate1 = ExportToExcel;
            idgProductItems.ContextMenuGenericDisplayName1 = "Save to Excel with Descriptions";
            idgProductItems.ContextMenuGenericIsVisible1 = true;

            this.MainTableName = mainTableName;
            idgProductItems.xGrid.FieldLayoutSettings = layouts;
            idgProductItems.FieldLayoutResourceString = fieldLayoutResource;
            idgProductItems.MainTableName = mainTableName;
            idgProductItems.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load product item type combobox
            if (businessObject.HasObjectData)
            {
                cmbProductItem = new ComboBoxItemsProvider();
                cmbProductItem.ItemsSource = businessObject.ObjectData.Tables[productsTableName].DefaultView;
                cmbProductItem.ValuePath = productsValuePath;
                cmbProductItem.DisplayMemberPath = productsDisplayPath;


                cmbRevType = new ComboBoxItemsProvider();
                cmbRevType.ItemsSource = businessObject.ObjectData.Tables[RevTypeTableName].DefaultView;
                cmbRevType.ValuePath = RevTypeValuePath;
                cmbRevType.DisplayMemberPath = RevTypeDisplayPath;

                cmbProductCode = new ComboBoxItemsProvider();
                cmbProductCode.ItemsSource = businessObject.ObjectData.Tables[ProductCodeTableName].DefaultView;
                cmbProductCode.ValuePath = ProductCodeValuePath;
                cmbProductCode.DisplayMemberPath = ProductCodeDisplayPath;

            }
           
            //load product item category combobox
            if (businessObject.HasObjectData)
            {
                cmbProductCat = new ComboBoxItemsProvider();
                cmbProductCat.ItemsSource = businessObject.ObjectData.Tables[categoryTableName].DefaultView;
                cmbProductCat.ValuePath = categoryValuePath;
                cmbProductCat.DisplayMemberPath = categoryDisplayPath;
            }
            idgProductItems.LoadGrid(businessObject, idgProductItems.MainTableName);
        }

        void exporter_ExportStarted(object sender, ExportStartedEventArgs e)
        {
            //Excel Exporter details
            e.CurrentWorksheet.DisplayOptions.PanesAreFrozen = true;
            e.CurrentWorksheet.DisplayOptions.FrozenPaneSettings.FrozenRows = 1;
            e.CurrentWorksheet.DisplayOptions.ShowGridlines = true;
        }

        public void ExportToExcel()
        {
            SaveFileDialog save = new SaveFileDialog();
            //Can save as Excle 2007 or 2003
            save.Filter = " Office 2007 Excel File(*.xlsx)|*.xlsx|Office 2003 Excel File (*.xls)|*.xls";
            System.Nullable<bool> dialogResult = save.ShowDialog();
            if (dialogResult == true)
            {
                idgProductItems.xGrid.FieldLayouts[0].Fields["item_type"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["item_type_desc"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["revenue_type_id"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["revenue_type_desc"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["item_category"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["item_category_desc"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["archive_flag"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["archive_flag_desc"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["revenue_share_flag"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["revenue_share_flag_desc"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["use_customer_geography"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["use_customer_geography_desc"].Visibility = Visibility.Visible;
                DataPresenterExcelExporter exporter = new DataPresenterExcelExporter();

                exporter.ExportStarted += new EventHandler<ExportStartedEventArgs>(exporter_ExportStarted);

                //Saves file to destination
                if (save.FileName.Contains(".xlsx"))
                {
                    //**DWR Added 4/18/12 - Added Try Catch to stop app from crashing when excel doc was already opened.
                    try
                    {
                        exporter.Export(idgProductItems.xGrid, save.FileName, WorkbookFormat.Excel2007);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }

                }
                else
                {
                    try
                    {
                        exporter.Export(idgProductItems.xGrid, save.FileName, WorkbookFormat.Excel97To2003);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }
                }
                idgProductItems.xGrid.FieldLayouts[0].Fields["item_type"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["item_type_desc"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["revenue_type_id"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["revenue_type_desc"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["item_category"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["item_category_desc"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["archive_flag"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["archive_flag_desc"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["revenue_share_flag"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["revenue_share_flag_desc"].Visibility = Visibility.Collapsed;
                idgProductItems.xGrid.FieldLayouts[0].Fields["use_customer_geography"].Visibility = Visibility.Visible;
                idgProductItems.xGrid.FieldLayouts[0].Fields["use_customer_geography_desc"].Visibility = Visibility.Collapsed;
            }
        }

        public override void Save()
        {
            // Vinay's changes - Validate account code in GL Chart.
            foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["product_item"].Rows)
            {

                if (r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified)
                {

                    cBaseBusObject GLAccountCodeValidation = new cBaseBusObject("GLAccountCodeValidation");
                    GLAccountCodeValidation.Parms.ClearParms();
                    GLAccountCodeValidation.Parms.AddParm("@gl_co", r["gl_co"].ToString());
                    GLAccountCodeValidation.Parms.AddParm("@gl_center", r["gl_center"].ToString());
                    GLAccountCodeValidation.Parms.AddParm("@gl_account", r["revenue_acct"].ToString());
                    GLAccountCodeValidation.Parms.AddParm("@gl_product", r["gl_product"].ToString());
                    GLAccountCodeValidation.Parms.AddParm("@gl_region", r["geography"].ToString());
                    GLAccountCodeValidation.Parms.AddParm("@gl_intercompany", r["interdivision"].ToString());

                    //GLAccountCodeValidation.Parms.AddParm("@account_code", null);

                    GLAccountCodeValidation.LoadTable("gl_account_code");

                    if (GLAccountCodeValidation.HasObjectData)
                    {
                        if (GLAccountCodeValidation.ObjectData.Tables["gl_account_code"].Rows.Count < 1)
                        {
                            string curAccountCode = r["gl_co"].ToString() + "-"
                                                    + r["gl_center"].ToString() + "-"
                                                    + r["revenue_acct"].ToString() + "-"
                                                    + r["gl_product"].ToString() + "-"
                                                    + r["geography"].ToString() + "-"
                                                    + r["interdivision"].ToString();
                            MessageBox.Show("Warning - Account code " + curAccountCode + " is missing in GL Chart", "GL Chart Validation");
                        }
                    }
                }

            }
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
