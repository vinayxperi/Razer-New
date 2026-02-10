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
using System.Data;

namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for OracleNSAccountGold.xaml
    /// </summary>
    public partial class OracleNSAccountGold : ScreenBase, IScreen
    {

        private static readonly string fieldLayoutResource = "OracleNSAccountGoldMaintenance";
        private static readonly string mainTableName = "oracle_ns_account_gold";

        public OracleNSAccountGold()
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
            idgOracleNSAccountGold.xGrid.FieldLayoutSettings = layouts;
            idgOracleNSAccountGold.FieldLayoutResourceString = fieldLayoutResource;
            idgOracleNSAccountGold.MainTableName = mainTableName;
            idgOracleNSAccountGold.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            idgOracleNSAccountGold.LoadGrid(businessObject, mainTableName);

            var acctMapping_FieldLayout = idgOracleNSAccountGold.xGrid.FieldLayouts[mainTableName];
            var acctMapping_field = acctMapping_FieldLayout.Fields.FirstOrDefault(f => f.Name == "OracleAcct");
            if (acctMapping_field != null)
            {
                acctMapping_field.Settings.EditorStyle = new Style(typeof(XamTextEditor));
                acctMapping_field.Settings.EditorStyle.Setters.Add(new EventSetter(UIElement.LostFocusEvent, new RoutedEventHandler(OracleAcct_LostFocus)));
            }
        }

        private void OracleAcct_LostFocus(object sender, RoutedEventArgs e)
        {
            var editor = sender as XamTextEditor;

            if (editor.DisplayText !=null && editor.DisplayText != "" && editor.DisplayText != "10010") //If it is not the cash account, default Entity to *.  
            {
                DataRecord record = idgOracleNSAccountGold.xGrid.ActiveRecord as DataRecord;
                if (record != null)
                    record.SetCellValue(record.FieldLayout.Fields["Entity"], "*");
            }
        }

        public string WindowCaption
        {
            get { throw new NotImplementedException(); }
        }

        public override void Save()
        {
            Dictionary<string, string> fieldLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "OracleAcct", "Oracle Account" },
                { "NSAcct", "Netsuite Account" },
                { "OracleDept", "Oracle Department" },
                { "NSDept", "Netsuite Department" },
                { "NSClass", "NetSuite Class" },
                { "name", "Customer Name" }
            };

            //Validate all mandatory fields.
            foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables[mainTableName].Rows)
            {

                if (r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified)
                {
                    foreach (DataColumn col in r.Table.Columns)
                    {
                        var value = r[col];
                        string friendlyName;
                        string label = fieldLabels.TryGetValue(col.ColumnName, out friendlyName)
                            ? friendlyName
                            : col.ColumnName;

                        if (value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            Messages.ShowInformation(label.ToUpper() + " must to be populated before saving.");
                            return; // Optional: stop checking further fields in this row
                        }

                        // Check for int type and value == 0
                        if (col.DataType == typeof(int) && Convert.ToInt32(value) == 0)
                        {
                            Messages.ShowInformation(label.ToUpper() + " must have a value greater than 0.");
                            return;
                        }

                        if (col.ColumnName == "Entity")
                        {
                            //Check if valid company code is added!!
                            bool matchFound = false;
                            if (r["OracleAcct"].ToString() != "10010" && value.ToString() == "*")
                            {
                                matchFound = true;
                            }
                            else if(r["OracleAcct"].ToString() == "10010")
                            {
                                cBaseBusObject region = new cBaseBusObject("GetActiveCompanyCodes");
                                region.LoadTable("company");

                                if (region.HasObjectData)
                                {
                                    if (region.ObjectData.Tables["company"].Rows.Count > 0)//Active companies have been fetched!!
                                    {
                                        foreach (DataRow row in region.ObjectData.Tables["company"].Rows)
                                        {
                                            if (value.ToString() == row["company_code"].ToString())
                                            {
                                                matchFound = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (!matchFound)
                            {
                                Messages.ShowInformation("Please enter a valid Entity");
                                return;
                            }

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
