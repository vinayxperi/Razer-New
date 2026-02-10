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
    /// Interaction logic for ManInvItem.xaml
    /// </summary>
    public partial class ManInvItem : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "ManInvItem";
        private static readonly string mainTableName = "man_inv_item";
        //Needed for the combobox
        private static readonly string RevTypeTableName = "revenue_type";
        private static readonly string RevTypeDisplayPath = "revenue_description";
        private static readonly string RevTypeValuePath = "revenue_type_id";

        private static readonly string RuleHdrIDTableName = "rule_hdr";
        private static readonly string RuleHdrIDDisplayPath = "item_rule_desc";
        private static readonly string RuleHdrIDValuePath = "item_rule_id";
                    
        //Needed for a combobox
        public ComboBoxItemsProvider cmbRevType { get; set; }
        public ComboBoxItemsProvider cmbRuleHdrID { get; set; }
       
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public ManInvItem()
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
            idgManInvItem.xGrid.FieldLayoutSettings = layouts;
            idgManInvItem.FieldLayoutResourceString = fieldLayoutResource;
            idgManInvItem.MainTableName = mainTableName;
            idgManInvItem.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);

            idgManInvItem.ContextMenuGenericDelegate1 = ExportToExcel;
            idgManInvItem.ContextMenuGenericDisplayName1 = "Save to Excel with Descriptions";
            idgManInvItem.ContextMenuGenericIsVisible1 = true;

            //load revenue type combobox
            if (businessObject.HasObjectData)
            {
                cmbRevType = new ComboBoxItemsProvider();
                cmbRevType.ItemsSource = businessObject.ObjectData.Tables[RevTypeTableName].DefaultView;
                cmbRevType.ValuePath = RevTypeValuePath;
                cmbRevType.DisplayMemberPath = RevTypeDisplayPath;


                cmbRuleHdrID = new ComboBoxItemsProvider();
                cmbRuleHdrID.ItemsSource = businessObject.ObjectData.Tables[RuleHdrIDTableName].DefaultView;
                cmbRuleHdrID.ValuePath = RuleHdrIDValuePath;
                cmbRuleHdrID.DisplayMemberPath = RuleHdrIDDisplayPath;
            }
           
            this.Load(businessObject);

            idgManInvItem.LoadGrid(businessObject, idgManInvItem.MainTableName);
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
                idgManInvItem.xGrid.FieldLayouts[0].Fields["revenue_type_id"].Visibility = Visibility.Collapsed;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["revenue_type_desc"].Visibility = Visibility.Visible;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["item_rule_id"].Visibility = Visibility.Collapsed;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["item_rule_desc"].Visibility = Visibility.Visible;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["inactive_flag"].Visibility = Visibility.Collapsed;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["inactive_flag_desc"].Visibility = Visibility.Visible;
                DataPresenterExcelExporter exporter = new DataPresenterExcelExporter();

                exporter.ExportStarted += new EventHandler<ExportStartedEventArgs>(exporter_ExportStarted);

                //Saves file to destination
                if (save.FileName.Contains(".xlsx"))
                {
                    //**DWR Added 4/18/12 - Added Try Catch to stop app from crashing when excel doc was already opened.
                    try
                    {
                        exporter.Export(idgManInvItem.xGrid, save.FileName, WorkbookFormat.Excel2007);
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
                        exporter.Export(idgManInvItem.xGrid, save.FileName, WorkbookFormat.Excel97To2003);
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Spreadsheet file.  Make sure the spreadsheet file is not already open and try again.");
                    }
                }
                idgManInvItem.xGrid.FieldLayouts[0].Fields["revenue_type_id"].Visibility = Visibility.Visible;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["revenue_type_desc"].Visibility = Visibility.Collapsed;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["item_rule_id"].Visibility = Visibility.Visible;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["item_rule_desc"].Visibility = Visibility.Collapsed;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["inactive_flag"].Visibility = Visibility.Visible;
                idgManInvItem.xGrid.FieldLayouts[0].Fields["inactive_flag_desc"].Visibility = Visibility.Collapsed;
            }
        }

        public override void Save()
        {
            // Vinay's changes - Validate account code in GL Chart

            foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["man_inv_item"].Rows)
            {

                if (r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified)
                {
                    cBaseBusObject GLAccountCodeValidationManInvItem = new cBaseBusObject("GLAccountCodeValidationManInvItem");
                    
                    GLAccountCodeValidationManInvItem.Parms.ClearParms();
                    GLAccountCodeValidationManInvItem.Parms.AddParm("@account_code", r["account_code"].ToString());

                    GLAccountCodeValidationManInvItem.LoadTable("gl_account_code_manInvItem");

                    if (GLAccountCodeValidationManInvItem.HasObjectData)
                    {
                        if (GLAccountCodeValidationManInvItem.ObjectData.Tables["gl_account_code_manInvItem"].Rows.Count < 1)
                        {
                            string tempAccCode = r["account_code"].ToString();

                            // Insert hyphens at the specified positions
                            tempAccCode = tempAccCode.Insert(2, "-");
                            tempAccCode = tempAccCode.Insert(6 + 1, "-"); // +1 to account for the previous hyphen
                            tempAccCode = tempAccCode.Insert(11 + 2, "-"); // +2 to account for the previous hyphens
                            tempAccCode = tempAccCode.Insert(15 + 3, "-"); // +3 to account for the previous hyphens
                            tempAccCode = tempAccCode.Insert(19 + 4, "-"); // +4 to account for the previous hyphens


                            MessageBox.Show("Warning - Account code " + tempAccCode + " is missing in GL chart", "GL Chart Validation");
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
