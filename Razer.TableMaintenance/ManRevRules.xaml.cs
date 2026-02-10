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
    /// Interaction logic for ManRevRules.xaml
    /// </summary>
    public partial class ManRevRules : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ManRevRules";
        private static readonly string mainTableName = "man_inv_def_rev_rules";

        //Needed for the combobox
        private static readonly string RuleHdrIDTableName = "rule_hdr";
        private static readonly string RuleHdrIDDisplayPath = "rule_desc";
        private static readonly string RuleHdrIDValuePath = "rule_id";

        //Needed for a combobox
        public ComboBoxItemsProvider cmbRuleHdrID { get; set; }
               
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public ManRevRules()
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

            idgManRevRules.ContextMenuGenericDelegate1 = ExportToExcel;
            idgManRevRules.ContextMenuGenericDisplayName1 = "Save to Excel with Descriptions";
            idgManRevRules.ContextMenuGenericIsVisible1 = true;

            this.MainTableName = mainTableName;
            idgManRevRules.xGrid.FieldLayoutSettings = layouts;
            idgManRevRules.FieldLayoutResourceString = fieldLayoutResource;
            idgManRevRules.MainTableName = mainTableName;
            idgManRevRules.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);


            //load Rule Header combobox
            if (businessObject.HasObjectData)
            {
                cmbRuleHdrID = new ComboBoxItemsProvider();
                cmbRuleHdrID.ItemsSource = businessObject.ObjectData.Tables[RuleHdrIDTableName].DefaultView;
                cmbRuleHdrID.ValuePath = RuleHdrIDValuePath;
                cmbRuleHdrID.DisplayMemberPath = RuleHdrIDDisplayPath;
            }

            idgManRevRules.LoadGrid(businessObject, idgManRevRules.MainTableName);
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
                idgManRevRules.xGrid.FieldLayouts[0].Fields["rule_id"].Visibility = Visibility.Collapsed;
                idgManRevRules.xGrid.FieldLayouts[0].Fields["rule_desc"].Visibility = Visibility.Visible;
                DataPresenterExcelExporter exporter = new DataPresenterExcelExporter();

                exporter.ExportStarted += new EventHandler<ExportStartedEventArgs>(exporter_ExportStarted);

                //Saves file to destination
                if (save.FileName.Contains(".xlsx"))
                {
                    //**DWR Added 4/18/12 - Added Try Catch to stop app from crashing when excel doc was already opened.
                    try
                    {
                        exporter.Export(idgManRevRules.xGrid, save.FileName, WorkbookFormat.Excel2007);
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
                        exporter.Export(idgManRevRules.xGrid, save.FileName, WorkbookFormat.Excel97To2003);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }
                }
                idgManRevRules.xGrid.FieldLayouts[0].Fields["rule_id"].Visibility = Visibility.Visible;
                idgManRevRules.xGrid.FieldLayouts[0].Fields["rule_desc"].Visibility = Visibility.Collapsed;
            }
        }

        public override void Save()
        {
            bool OverrideSaveTableListing = UIHelper.FindVisualParent<TableListing>(this).OverrideSave;

            UIHelper.FindVisualParent<TableListing>(this).OverrideSave = true;


            Boolean bError = false;
            //this.CurrentBusObj.LoadTable("validatePct");
            Int32 ictr = 1;
           
            decimal pctTotD = 0;
            decimal pctSumD = 0;
            
            int ruleID = 0;
            int prevruleID = 0;

            DataView view = new DataView(this.CurrentBusObj.ObjectData.Tables["man_inv_def_rev_rules"]);
            DataTable distinctValues = view.ToTable(true, "rule_id");
            if (distinctValues.Rows.Count > 0)
                ictr = distinctValues.Rows.Count;
            
           
            foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["man_inv_def_rev_rules"].Rows)
            {
                ruleID = Convert.ToInt32(r["rule_id"]);
                
            
    

                
                //validate each row is = 100, otherwise error!
               
                pctTotD = pctTotD + Convert.ToDecimal(r["alloc_percent"]);
                 
                                  
                }
           
            pctSumD = pctTotD / ictr ;

            //pctSumD = Convert.ToDecimal(Math.Round(pctSumD, 2));
            if  (pctSumD != 100)
               
            {
                bError = true;
                Messages.ShowError("Rules must total 100%");
            }

            if (bError == false)
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
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }


       
    }
}
