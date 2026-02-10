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
    /// Interaction logic for CountryMaint.xaml
    /// </summary>
    public partial class CountryMaint : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "Country";
        private static readonly string mainTableName = "country";
        //Needed for the combobox
        private static readonly string regionTableName = "region";
        private static readonly string regionDisplayPath = "region";
        private static readonly string regionValuePath = "region_id";
        private static readonly string georegionTableName = "geographic_region";
        private static readonly string georegionDisplayPath = "description";
        private static readonly string georegionValuePath = "geographic_region_id";
        
                
        //Needed for a combobox
        public ComboBoxItemsProvider cmbRegionID { get; set; }
        public ComboBoxItemsProvider cmbGeoRegionID { get; set; }
        


        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public CountryMaint()
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

            idgCountry.ContextMenuGenericDelegate1 = ExportToExcel;
            idgCountry.ContextMenuGenericDisplayName1 = "Save to Excel with Descriptions";
            idgCountry.ContextMenuGenericIsVisible1 = true;

            this.MainTableName = mainTableName;
            idgCountry.xGrid.FieldLayoutSettings = layouts;
            idgCountry.FieldLayoutResourceString = fieldLayoutResource;
            idgCountry.MainTableName = mainTableName;
            idgCountry.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load region combobox
            if (businessObject.HasObjectData)
            {
                cmbRegionID = new ComboBoxItemsProvider();
                cmbRegionID.ItemsSource = businessObject.ObjectData.Tables[regionTableName].DefaultView;
                cmbRegionID.ValuePath = regionValuePath;
                cmbRegionID.DisplayMemberPath = regionDisplayPath;
            }
            //load geographic region combobox
            if (businessObject.HasObjectData)
            {
                cmbGeoRegionID = new ComboBoxItemsProvider();
                cmbGeoRegionID.ItemsSource = businessObject.ObjectData.Tables[georegionTableName].DefaultView;
                cmbGeoRegionID.ValuePath = georegionValuePath;
                cmbGeoRegionID.DisplayMemberPath = georegionDisplayPath;
            }

            this.Load(businessObject);

            idgCountry.LoadGrid(businessObject, idgCountry.MainTableName);
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
                idgCountry.xGrid.FieldLayouts[0].Fields["region_id"].Visibility = Visibility.Collapsed;
                idgCountry.xGrid.FieldLayouts[0].Fields["region_desc"].Visibility = Visibility.Visible;
                idgCountry.xGrid.FieldLayouts[0].Fields["geographic_region_id"].Visibility = Visibility.Collapsed;
                idgCountry.xGrid.FieldLayouts[0].Fields["geographic_region_desc"].Visibility = Visibility.Visible;
                idgCountry.xGrid.FieldLayouts[0].Fields["region_flag"].Visibility = Visibility.Collapsed;
                idgCountry.xGrid.FieldLayouts[0].Fields["region_flag_desc"].Visibility = Visibility.Visible;
                idgCountry.xGrid.FieldLayouts[0].Fields["use_flag"].Visibility = Visibility.Collapsed;
                idgCountry.xGrid.FieldLayouts[0].Fields["use_flag_desc"].Visibility = Visibility.Visible;
                DataPresenterExcelExporter exporter = new DataPresenterExcelExporter();

                exporter.ExportStarted += new EventHandler<ExportStartedEventArgs>(exporter_ExportStarted);

                //Saves file to destination
                if (save.FileName.Contains(".xlsx"))
                {
                    //**DWR Added 4/18/12 - Added Try Catch to stop app from crashing when excel doc was already opened.
                    try
                    {
                        exporter.Export(idgCountry.xGrid, save.FileName, WorkbookFormat.Excel2007);
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
                        exporter.Export(idgCountry.xGrid, save.FileName, WorkbookFormat.Excel97To2003);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }
                }
                idgCountry.xGrid.FieldLayouts[0].Fields["region_id"].Visibility = Visibility.Visible;
                idgCountry.xGrid.FieldLayouts[0].Fields["region_desc"].Visibility = Visibility.Collapsed;
                idgCountry.xGrid.FieldLayouts[0].Fields["geographic_region_id"].Visibility = Visibility.Visible;
                idgCountry.xGrid.FieldLayouts[0].Fields["geographic_region_desc"].Visibility = Visibility.Collapsed;
                idgCountry.xGrid.FieldLayouts[0].Fields["region_flag"].Visibility = Visibility.Visible;
                idgCountry.xGrid.FieldLayouts[0].Fields["region_flag_desc"].Visibility = Visibility.Collapsed;
                idgCountry.xGrid.FieldLayouts[0].Fields["use_flag"].Visibility = Visibility.Visible;
                idgCountry.xGrid.FieldLayouts[0].Fields["use_flag_desc"].Visibility = Visibility.Collapsed;
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
