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
    /// Interaction logic for RazerSFNSCustomerGold.xaml
    /// </summary>
    public partial class RazerSFNSCustomerGold : ScreenBase, IScreen
    {

        private static readonly string fieldLayoutResource = "RazerSFNSCustomerGoldMaintenance";
        private static readonly string mainTableName = "razer_sfns_customer_gold";

        public RazerSFNSCustomerGold()
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
            idgRazerSFNSCustomerGold.xGrid.FieldLayoutSettings = layouts;
            idgRazerSFNSCustomerGold.FieldLayoutResourceString = fieldLayoutResource;
            idgRazerSFNSCustomerGold.MainTableName = mainTableName;
            idgRazerSFNSCustomerGold.xGrid.FieldSettings.AllowEdit = true;

            this.Load(businessObject);
            idgRazerSFNSCustomerGold.LoadGrid(businessObject, mainTableName);
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
