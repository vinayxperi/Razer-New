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
using System.Windows.Shapes;
using System.Data;
using Infragistics.Windows.DataPresenter;
namespace RazerBase.Lookups
{
    /// <summary>
    /// Interaction logic for UnitEntryLookup.xaml
    /// </summary>
    public partial class UnitEntryLookup : DialogBase
    {
        private int _mso_id = 0;
        private int _cs_id = 0;
        private string _locname;
        private int _contract_id = 0;
        private int _report_id = 0;
        private int _unit_type_id = 0;
        private string _contract_name;
        public string WindowCaption { get; set; }

        public UnitEntryLookup(cBaseBusObject currObject)
        {
            this.CurrentBusObj = currObject;
            InitializeComponent();

            Init();

        }
        public void Init()
        {
            //this.lcbFilter.SetBindingExpression("unit_md_id", "md_name", this.CurrentBusObj.GetTable("unit_md_category") as DataTable);
            //this.lcbFilter.ComboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            //if (cGlobals.ReturnParms.Count > 1)
            //{

            //    lcbFilter.SelectedValue = _md_id;
            //    cGlobals.ReturnParms.Clear();
            //    ComboBox_SelectionChanged(null, null);

            //}
            DateTime dt = DateTime.Now.AddDays(-90);
            int month = dt.Month;
            int year = dt.Year;
            DateTime startDate = Convert.ToDateTime(month.ToString() + "/01/" + year.ToString());
            DateTime endDate1 = DateTime.Now.AddMonths(1);
            month = endDate1.Month;
            year = endDate1.Year;
           
            if (CurrentBusObj.HasObjectData)
            {
                this.cbProduct.SetBindingExpression("product_code", "product_description", this.CurrentBusObj.GetTable("unit_product") as DataTable, "");
                this.cbUnits.SetBindingExpression("unit_type_id", "unit_description", this.CurrentBusObj.GetTable("unit_type") as DataTable, "");
                this.cbReport.SetBindingExpression("report_id", "description", this.CurrentBusObj.GetTable("contract_report") as DataTable, "");
            }
            FieldLayoutSettings f = new FieldLayoutSettings();
            gFilterGrid.MainTableName = "unit_entry_sel";
            gFilterGrid.ConfigFileName = "filtergrid1";
            gFilterGrid.xGrid.FieldLayoutSettings = f;
            gFilterGrid.SetGridSelectionBehavior(false, true);
            gFilterGrid.FieldLayoutResourceString = "unit_entryGrid";
            gFilterGrid.xGrid.FieldSettings.AllowEdit = false;
            gFilterGrid.WindowZoomDelegate = ClickGrid;

           
        }
        public void ClickGrid()
        {
            //call customer document folder
            DataRecord r = default(DataRecord);
            //Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            //If error condition is received when retrieving selected row then the row of the currently active cell is used.
            try
            {
                r = (Infragistics.Windows.DataPresenter.DataRecord)gFilterGrid.xGrid.SelectedItems.Records[0];
            }
            catch
            {
                //// for debugging only
                //string err = ex.ToString();

                // Set the current record
                r = gFilterGrid.xGrid.ActiveCell.Record;
            }
            //Clear the return parms in case they have data
            cGlobals.ReturnParms.Clear();
            //Add any return parms here pulling from the grid columns as needed
            cGlobals.ReturnParms.Add(r.Cells["unit_id"].Value);
             cGlobals.ReturnParms.Add(r.Cells["mso_id"].Value);
             cGlobals.ReturnParms.Add(r.Cells["cs_id"].Value);
             cGlobals.ReturnParms.Add(r.Cells["report_id"].Value);
             cGlobals.ReturnParms.Add(r.Cells["contract_id"].Value);
             cGlobals.ReturnParms.Add(r.Cells["unit_type_id"].Value);
             cGlobals.ReturnParms.Add(r.Cells["product_code"].Value);

            //Close the lookup
            this.Close();
        }
        private void tbEntity_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EntityLookup el = new EntityLookup();
            el.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                _mso_id =Int32.Parse(cGlobals.ReturnParms[0].ToString());
                tbEntity.Text = cGlobals.ReturnParms[1].ToString();
                cGlobals.ReturnParms.Clear();
            }
        }

        private void tbLocationMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(_mso_id.ToString());
            LocationLookup ll = new LocationLookup();
            ll.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                _cs_id = Int32.Parse(cGlobals.ReturnParms[0].ToString());
                _locname = cGlobals.ReturnParms[1].ToString();
                tbLocation.Text = _locname;
                cGlobals.ReturnParms.Clear();
            }
        }
        private void tbContractMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContractLookup cl = new ContractLookup();
            cl.Init(new cBaseBusObject("ContractLookup"));
            cl.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                _contract_id = Int32.Parse(cGlobals.ReturnParms[0].ToString());
                _contract_name = cGlobals.ReturnParms[1].ToString();
                tbContract.Text = _contract_name;
                CurrentBusObj.Parms.ClearParms();
                CurrentBusObj.Parms.AddParm("@unit_id", 0);
                CurrentBusObj.Parms.AddParm("@contract_id", _contract_id);
                this.Load();
                DataTable reports = (DataTable)CurrentBusObj.GetTable("contract_report");
                if ((reports.Rows.Count > 0) || (reports != null))
                {
                    cbReport.IsEnabled = true;
                }
                else
                {
                    cbReport.IsEnabled = false;
                }
                cGlobals.ReturnParms.Clear();
            }
        }
       
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
                CurrentBusObj.Parms.ClearParms();
                if (cbReport.SelectedValue != null)
                {
                    _report_id = Int32.Parse(cbReport.SelectedValue.ToString());
                    CurrentBusObj.Parms.AddParm("@report_id", _report_id);
                }
                if (cbUnits.SelectedValue != null)
                {
                    CurrentBusObj.Parms.AddParm("@unit_type", Int32.Parse(cbUnits.SelectedValue.ToString()));
                }
                if (cbProduct.SelectedValue != null)
                {
                    CurrentBusObj.Parms.AddParm("@product", cbProduct.SelectedValue.ToString());
                }
                CurrentBusObj.Parms.AddParm("@unit_id", 0);
                CurrentBusObj.Parms.AddParm("@contract_id", _contract_id);
                    CurrentBusObj.Parms.AddParm("@entity_id", _mso_id);
                    CurrentBusObj.Parms.AddParm("@cs_id", _cs_id);
                    CurrentBusObj.Parms.AddParm("@location_id", 0);
                if (_contract_id != 0)
                {
                    CurrentBusObj.Parms.AddParm("@contract_id_s", _contract_id);
                }
                if (dpDateStart.SelText.HasValue)
                {
                    CurrentBusObj.Parms.AddParm("@service_period_start", Convert.ToDateTime(dpDateStart.SelText));
                }
                if (dpDateEnd.SelText.HasValue)
                {
                    CurrentBusObj.Parms.AddParm("@service_period_end", Convert.ToDateTime(dpDateEnd.SelText));
                }
                this.Load();
                gFilterGrid.LoadGrid(CurrentBusObj, "unit_entry_sel");
        }
    }
}
