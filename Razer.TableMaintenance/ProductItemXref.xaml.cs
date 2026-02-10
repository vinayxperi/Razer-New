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
    /// Interaction logic for ProductItemXref.xaml
    /// </summary>
    public partial class ProductItemXref : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ProductItemXref";
        private static readonly string mainTableName = "product_item_xref";
        //Needed for the combobox
        private static readonly string ItemCodeTableName = "man_inv_item";
        private static readonly string ItemCodeDisplayPath = "description";
        private static readonly string ItemCodeValuePath = "item_code";
        private static readonly string ProductCodeTableName = "products";
        private static readonly string ProductCodeDisplayPath = "product_description";
        private static readonly string ProductCodeValuePath = "product_code";
                    
        //Needed for a combobox
        public ComboBoxItemsProvider cmbItemCode { get; set; }
        public ComboBoxItemsProvider cmbProductCode { get; set; }
       
       
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public ProductItemXref()
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

            idgProductItemXref.ContextMenuGenericDelegate1 = ExportToExcel;
            idgProductItemXref.ContextMenuGenericDisplayName1 = "Save to Excel with Descriptions";
            idgProductItemXref.ContextMenuGenericIsVisible1 = true;

            this.MainTableName = mainTableName;
            idgProductItemXref.xGrid.FieldLayoutSettings = layouts;
            idgProductItemXref.FieldLayoutResourceString = fieldLayoutResource;
            idgProductItemXref.MainTableName = mainTableName;
            idgProductItemXref.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load product code combobox
            if (businessObject.HasObjectData)
            {
                cmbProductCode = new ComboBoxItemsProvider();
                cmbProductCode.ItemsSource = businessObject.ObjectData.Tables[ProductCodeTableName].DefaultView;
                cmbProductCode.ValuePath = ProductCodeValuePath;
                cmbProductCode.DisplayMemberPath = ProductCodeDisplayPath;
            }
            //load item code combobox
            if (businessObject.HasObjectData)
            {
                cmbItemCode = new ComboBoxItemsProvider();
                cmbItemCode.ItemsSource = businessObject.ObjectData.Tables[ItemCodeTableName].DefaultView;
                cmbItemCode.ValuePath = ItemCodeValuePath;
                cmbItemCode.DisplayMemberPath = ItemCodeDisplayPath;
            }
           
            this.Load(businessObject);

            idgProductItemXref.LoadGrid(businessObject, idgProductItemXref.MainTableName);
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
                idgProductItemXref.xGrid.FieldLayouts[0].Fields["product_code"].Visibility = Visibility.Collapsed;
                idgProductItemXref.xGrid.FieldLayouts[0].Fields["product_code_desc"].Visibility = Visibility.Visible;
                idgProductItemXref.xGrid.FieldLayouts[0].Fields["item_code"].Visibility = Visibility.Collapsed;
                idgProductItemXref.xGrid.FieldLayouts[0].Fields["item_code_desc"].Visibility = Visibility.Visible;
                DataPresenterExcelExporter exporter = new DataPresenterExcelExporter();

                exporter.ExportStarted += new EventHandler<ExportStartedEventArgs>(exporter_ExportStarted);

                //Saves file to destination
                if (save.FileName.Contains(".xlsx"))
                {
                    //**DWR Added 4/18/12 - Added Try Catch to stop app from crashing when excel doc was already opened.
                    try
                    {
                        exporter.Export(idgProductItemXref.xGrid, save.FileName, WorkbookFormat.Excel2007);
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
                        exporter.Export(idgProductItemXref.xGrid, save.FileName, WorkbookFormat.Excel97To2003);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }
                }
                idgProductItemXref.xGrid.FieldLayouts[0].Fields["product_code"].Visibility = Visibility.Visible;
                idgProductItemXref.xGrid.FieldLayouts[0].Fields["product_code_desc"].Visibility = Visibility.Collapsed;
                idgProductItemXref.xGrid.FieldLayouts[0].Fields["item_code"].Visibility = Visibility.Visible;
                idgProductItemXref.xGrid.FieldLayouts[0].Fields["item_code_desc"].Visibility = Visibility.Collapsed;
            }
        }

        public override void Save()
        {
            int validateitem = 0;            
            if (this.CurrentBusObj.HasObjectData)
            {
                foreach (DataRow r in CurrentBusObj.ObjectData.Tables["product_item_xref"].Rows)
                {
                    if (r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified)
                    {
                        validateitem = (Convert.ToInt32(r["item_code"]));
                        cBaseBusObject ItemVerification = new cBaseBusObject("ManItemXREFDupEdit");         
                        ItemVerification.Parms.ClearParms();
                        ItemVerification.Parms.AddParm("@item_code", validateitem);
                        ItemVerification.LoadTable("duplicate");
                        if (Convert.ToInt32(ItemVerification.ObjectData.Tables["duplicate"].Rows[0]["xref_id"]) > 0)
                        {
                            MessageBox.Show("Item code has been used in another XREF entry");
                            return;
                        }

                        cBaseBusObject ItemInactive = new cBaseBusObject("ManItemXREFInactiveEdit");         
                        ItemInactive.Parms.ClearParms();
                        ItemInactive.Parms.AddParm("@item_code", validateitem);
                        ItemInactive.LoadTable("inactive");
                        if (Convert.ToInt32(ItemInactive.ObjectData.Tables["inactive"].Rows[0]["inactive_flag"]) > 0)
                        {
                            MessageBox.Show("Item code is inactive");
                            return;
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
}
