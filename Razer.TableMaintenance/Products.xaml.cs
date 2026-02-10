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


namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ProductsTableMaintenance";
        private static readonly string mainTableName = "products";
        //Needed for the combobox
        private static readonly string productsTableName = "productType";
        private static readonly string productsDisplayPath = "code_value";
        private static readonly string productsValuePath = "fkey_int";
        //RES 2/13/15 co consolidation 
        private static readonly string logotextTableName = "LogoText";
        private static readonly string logotextDisplayPath = "code_value";
        private static readonly string logotextValuePath = "fkey_int";
        
        //Needed for a combobox
        public ComboBoxItemsProvider cmbProductItem { get; set; }
        public ComboBoxItemsProvider cmbLogoText { get; set; }

        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public Products()
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

            idgProducts.ContextMenuGenericDelegate1 = ExportToExcel;
            idgProducts.ContextMenuGenericDisplayName1 = "Save to Excel with Descriptions";
            idgProducts.ContextMenuGenericIsVisible1 = true;

            this.MainTableName = mainTableName;
            idgProducts.xGrid.FieldLayoutSettings = layouts;
            idgProducts.FieldLayoutResourceString = fieldLayoutResource;
            idgProducts.MainTableName = mainTableName;
            idgProducts.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            if (businessObject.HasObjectData)
            {
                cmbProductItem = new ComboBoxItemsProvider();
                cmbProductItem.ItemsSource = businessObject.ObjectData.Tables[productsTableName].DefaultView;
                cmbProductItem.ValuePath = productsValuePath;
                cmbProductItem.DisplayMemberPath = productsDisplayPath;
                //RES 2/13/15 Co Consolidation
                cmbLogoText = new ComboBoxItemsProvider();
                cmbLogoText.ItemsSource = businessObject.ObjectData.Tables[logotextTableName].DefaultView;
                cmbLogoText.ValuePath = logotextValuePath;
                cmbLogoText.DisplayMemberPath = logotextDisplayPath;
            }
            //else
            //    cmbProductItem.SelectedValue = 4;

            idgProducts.LoadGrid(businessObject, idgProducts.MainTableName);
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
                idgProducts.xGrid.FieldLayouts[0].Fields["product_type"].Visibility = Visibility.Collapsed;
                idgProducts.xGrid.FieldLayouts[0].Fields["product_type_desc"].Visibility = Visibility.Visible;
                idgProducts.xGrid.FieldLayouts[0].Fields["logo_text"].Visibility = Visibility.Collapsed;
                idgProducts.xGrid.FieldLayouts[0].Fields["logo_text_desc"].Visibility = Visibility.Visible;
                idgProducts.xGrid.FieldLayouts[0].Fields["archive_flag"].Visibility = Visibility.Collapsed;
                idgProducts.xGrid.FieldLayouts[0].Fields["archive_flag_desc"].Visibility = Visibility.Visible;
                idgProducts.xGrid.FieldLayouts[0].Fields["no_remit_flag"].Visibility = Visibility.Collapsed;
                idgProducts.xGrid.FieldLayouts[0].Fields["no_remit_flag_desc"].Visibility = Visibility.Visible;
                idgProducts.xGrid.FieldLayouts[0].Fields["def_exp_contract_flag"].Visibility = Visibility.Collapsed;
                idgProducts.xGrid.FieldLayouts[0].Fields["def_exp_contract_flag_desc"].Visibility = Visibility.Visible;
                DataPresenterExcelExporter exporter = new DataPresenterExcelExporter();

                exporter.ExportStarted += new EventHandler<ExportStartedEventArgs>(exporter_ExportStarted);

                //Saves file to destination
                if (save.FileName.Contains(".xlsx"))
                {
                    //**DWR Added 4/18/12 - Added Try Catch to stop app from crashing when excel doc was already opened.
                    try
                    {
                        exporter.Export(idgProducts.xGrid, save.FileName, WorkbookFormat.Excel2007);
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
                        exporter.Export(idgProducts.xGrid, save.FileName, WorkbookFormat.Excel97To2003);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }
                }
                idgProducts.xGrid.FieldLayouts[0].Fields["product_type"].Visibility = Visibility.Visible;
                idgProducts.xGrid.FieldLayouts[0].Fields["product_type_desc"].Visibility = Visibility.Collapsed;
                idgProducts.xGrid.FieldLayouts[0].Fields["logo_text"].Visibility = Visibility.Visible;
                idgProducts.xGrid.FieldLayouts[0].Fields["logo_text_desc"].Visibility = Visibility.Collapsed;
                idgProducts.xGrid.FieldLayouts[0].Fields["archive_flag"].Visibility = Visibility.Visible;
                idgProducts.xGrid.FieldLayouts[0].Fields["archive_flag_desc"].Visibility = Visibility.Collapsed;
                idgProducts.xGrid.FieldLayouts[0].Fields["no_remit_flag"].Visibility = Visibility.Visible;
                idgProducts.xGrid.FieldLayouts[0].Fields["no_remit_flag_desc"].Visibility = Visibility.Collapsed;
                idgProducts.xGrid.FieldLayouts[0].Fields["def_exp_contract_flag"].Visibility = Visibility.Visible;
                idgProducts.xGrid.FieldLayouts[0].Fields["def_exp_contract_flag_desc"].Visibility = Visibility.Collapsed;
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
