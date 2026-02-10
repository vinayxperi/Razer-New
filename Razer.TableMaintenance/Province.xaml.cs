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
    /// Interaction logic for Province.xaml
    /// </summary>
    public partial class Province : RazerBase.ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "Province";
        private static readonly string mainTableName = "province";
        public ComboBoxItemsProvider cmbCountry { get; set; }
        public string WindowCaption
        {
            get { return string.Empty; }
        }
        public Province()
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

            idgProvince.ContextMenuGenericDelegate1 = ExportToExcel;
            idgProvince.ContextMenuGenericDisplayName1 = "Save to Excel with Descriptions";
            idgProvince.ContextMenuGenericIsVisible1 = true;

            this.MainTableName = mainTableName;
            idgProvince.xGrid.FieldLayoutSettings = layouts;
            idgProvince.FieldLayoutResourceString = fieldLayoutResource;
            idgProvince.MainTableName = mainTableName;
            idgProvince.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            //load product item combobox
            if (businessObject.HasObjectData)
            {
                cmbCountry = new ComboBoxItemsProvider();
                cmbCountry.ItemsSource = businessObject.ObjectData.Tables["country"].DefaultView;
                cmbCountry.ValuePath = "country_id";
                cmbCountry.DisplayMemberPath = "country";
            }
            idgProvince.LoadGrid(businessObject, idgProvince.MainTableName);

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
                idgProvince.xGrid.FieldLayouts[0].Fields["country_id"].Visibility = Visibility.Collapsed;
                idgProvince.xGrid.FieldLayouts[0].Fields["country_desc"].Visibility = Visibility.Visible;
                DataPresenterExcelExporter exporter = new DataPresenterExcelExporter();

                exporter.ExportStarted += new EventHandler<ExportStartedEventArgs>(exporter_ExportStarted);

                //Saves file to destination
                if (save.FileName.Contains(".xlsx"))
                {
                    //**DWR Added 4/18/12 - Added Try Catch to stop app from crashing when excel doc was already opened.
                    try
                    {
                        exporter.Export(idgProvince.xGrid, save.FileName, WorkbookFormat.Excel2007);
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
                        exporter.Export(idgProvince.xGrid, save.FileName, WorkbookFormat.Excel97To2003);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }
                }
                idgProvince.xGrid.FieldLayouts[0].Fields["country_id"].Visibility = Visibility.Visible;
                idgProvince.xGrid.FieldLayouts[0].Fields["country_desc"].Visibility = Visibility.Collapsed;
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
