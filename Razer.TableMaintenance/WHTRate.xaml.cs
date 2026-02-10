using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
    /// Interaction logic for WHTRate.xaml
    /// </summary>
    public partial class WHTRate : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "WHTRateX";
        private static readonly string mainTableName = "wht_rate";
        //Needed for the country combobox
        private static readonly string countryTableName = "country";
        private static readonly string countryDisplayPath = "country";
        private static readonly string countryValuePath = "country_id";
        //Needed for the province combobox
        private static readonly string provinceTableName = "province";
        private static readonly string provinceDisplayPath = "province";
        private static readonly string provinceValuePath = "province_id";
        //Needed for item certificate issued combobox
        private static readonly string certIndTableName = "certInd";
        private static readonly string certIndDisplayPath = "code_value";
        private static readonly string certIndValuePath = "fkey_char";
        //Needed for item product item combobox
        private static readonly string ProductItemTableName = "dddwproductitem";
        private static readonly string ProductItemDisplayPath = "item_description";
        private static readonly string ProductItemValuePath = "item_id";
        //Needed for a combobox
        public ComboBoxItemsProvider cmbCountry { get; set; }
        public ComboBoxItemsProvider cmbProvince { get; set; }
        public ComboBoxItemsProvider cmbCertInd { get; set; }
        public ComboBoxItemsProvider cmbProductItem { get; set; }



        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public WHTRate()
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

            idgWHTRate.ContextMenuGenericDelegate1 = ExportToExcel;
            idgWHTRate.ContextMenuGenericDisplayName1 = "Save to Excel with Descriptions";
            idgWHTRate.ContextMenuGenericIsVisible1 = true;

            this.MainTableName = mainTableName;
            idgWHTRate.xGrid.FieldLayoutSettings = layouts;
            idgWHTRate.FieldLayoutResourceString = fieldLayoutResource;
            idgWHTRate.MainTableName = mainTableName;
            idgWHTRate.xGrid.FieldSettings.AllowEdit = true;
            CurrentBusObj.Parms.AddParm("@country", "0");
            CurrentBusObj.Parms.AddParm("@province", "0");
            //CurrentBusObj.Parms.AddParm("@province", " ");
            this.Load(businessObject);
            //load country item type combobox
            if (businessObject.HasObjectData)
            {
                cmbCountry = new ComboBoxItemsProvider();
                cmbCountry.ItemsSource = businessObject.ObjectData.Tables[countryTableName].DefaultView;
                cmbCountry.ValuePath = countryValuePath;
                cmbCountry.DisplayMemberPath = countryDisplayPath;
            }
            //load province item type combobox
            if (businessObject.HasObjectData)
            {
                cmbProvince = new ComboBoxItemsProvider();
                cmbProvince.ItemsSource = businessObject.ObjectData.Tables[provinceTableName].DefaultView;
                cmbProvince.ValuePath = provinceValuePath;
                cmbProvince.DisplayMemberPath = provinceDisplayPath;
            }
            //load cert ind combobox
            if (businessObject.HasObjectData)
            {
                cmbCertInd = new ComboBoxItemsProvider();
                cmbCertInd.ItemsSource = businessObject.ObjectData.Tables[certIndTableName].DefaultView;
                cmbCertInd.ValuePath = certIndValuePath;
                cmbCertInd.DisplayMemberPath = certIndDisplayPath;
            }
            //load product item combobox
            if (businessObject.HasObjectData)
            {
                cmbProductItem = new ComboBoxItemsProvider();
                cmbProductItem.ItemsSource = businessObject.ObjectData.Tables[ProductItemTableName].DefaultView;
                cmbProductItem.ValuePath = ProductItemValuePath;
                cmbProductItem.DisplayMemberPath = ProductItemDisplayPath;
            }
            idgWHTRate.LoadGrid(businessObject, idgWHTRate.MainTableName);
            Field f = idgWHTRate.xGrid.FieldLayouts[0].Fields["wht_rate"];
            f.Settings.EditorStyle = (Style)TryFindResource("RateStylePercentCalc");
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
                idgWHTRate.xGrid.FieldLayouts[0].Fields["country_id"].Visibility = Visibility.Collapsed;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["country_desc"].Visibility = Visibility.Visible;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["province_id"].Visibility = Visibility.Collapsed;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["province_desc"].Visibility = Visibility.Visible;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["item_id"].Visibility = Visibility.Collapsed;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["product_item_desc"].Visibility = Visibility.Visible;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["cert_ind"].Visibility = Visibility.Collapsed;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["cert_ind_desc"].Visibility = Visibility.Visible;
                DataPresenterExcelExporter exporter = new DataPresenterExcelExporter();

                exporter.ExportStarted += new EventHandler<ExportStartedEventArgs>(exporter_ExportStarted);

                //Saves file to destination
                if (save.FileName.Contains(".xlsx"))
                {
                    //**DWR Added 4/18/12 - Added Try Catch to stop app from crashing when excel doc was already opened.
                    try
                    {
                        exporter.Export(idgWHTRate.xGrid, save.FileName, WorkbookFormat.Excel2007);
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
                        exporter.Export(idgWHTRate.xGrid, save.FileName, WorkbookFormat.Excel97To2003);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }
                }
                idgWHTRate.xGrid.FieldLayouts[0].Fields["country_id"].Visibility = Visibility.Visible;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["country_desc"].Visibility = Visibility.Collapsed;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["province_id"].Visibility = Visibility.Visible;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["province_desc"].Visibility = Visibility.Collapsed;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["item_id"].Visibility = Visibility.Visible;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["product_item_desc"].Visibility = Visibility.Collapsed;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["cert_ind"].Visibility = Visibility.Visible;
                idgWHTRate.xGrid.FieldLayouts[0].Fields["cert_ind_desc"].Visibility = Visibility.Collapsed;
            }
        }

        public override void Save()
        {
            int validatecountry = 0;
            int validateprovince = 0;
            
            if (this.CurrentBusObj.HasObjectData)
            {
               // this.CurrentBusObj.ObjectData.Tables["wht_rate"].AcceptChanges();
                DataTable dtwhtrate = this.CurrentBusObj.ObjectData.Tables["wht_rate"];

                foreach (DataRow dtrow in dtwhtrate.Rows)
                {
                    ////validate province matches country
                    if (Convert.ToInt32(dtrow["country_id"]) == 0) 

                    {
                        if (Convert.ToInt32(dtrow["province_id"]) > 0)
                        {
                            MessageBox.Show("Country is a required column");
                            return;
                        }
                        
                    }
                    else
                    {
                        validatecountry = (Convert.ToInt32(dtrow["country_id"]));
                        if (Convert.ToInt32(dtrow["province_id"]) == 0)
                        {
                        }
                        else
                        {
                            validateprovince = (Convert.ToInt32(dtrow["province_id"]));
                            cBaseBusObject WHTVerification = new cBaseBusObject("WHTVerification");

                            WHTVerification.Parms.ClearParms();
                            WHTVerification.Parms.AddParm("@country_id", validatecountry);
                            WHTVerification.Parms.AddParm("@province_id", validateprovince);
                            WHTVerification.LoadTable("validate");
                            if (WHTVerification.ObjectData.Tables["validate"] == null || WHTVerification.ObjectData.Tables["validate"].Rows.Count < 1)
                             
                            
                            {
                               
                                //need to set focus on that row
                                  DataRecord currDR ;
                                  for (int i = 0; i < idgWHTRate.xGrid.Records.Count; i++)
                                  {
                                      currDR = idgWHTRate.xGrid.Records[i] as DataRecord;
                                      if (currDR.Cells["province_id"].Value.ToString() == validateprovince.ToString())
                                      {
                                          if (currDR.Cells["country_id"].Value.ToString() == validatecountry.ToString())
                                          {
                                              idgWHTRate.SetGridSelectionBehavior(true, false);
                                              idgWHTRate.ActiveRecord = currDR;
                                              currDR.IsSelected = true;
                                              MessageBox.Show("Province and Country do not match in the Province table");
                                              return;
                                          }
                                      }
                                  }


                            }
                            //else found a match

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
