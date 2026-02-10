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

namespace RazerBase.Lookups
{
    /// <summary>
    /// Interaction logic for UnitFiltersLook.xaml
    /// </summary>
    public partial class UnitFiltersLook : DialogBase
    {
        private int _filterid { get; set; }
        public int _seqid { get; set; }
        public int _md_id { get; set; }
        public string _md { get; set; }
        public int _valueid { get; set; }
        public string _value { get; set; }
        public int _construtorid { get; set; }
        public string _constructor { get; set; }
        public int _operatorid { get; set; }
        public string _operator { get; set; }
     
        public bool _hasvars { get; set; }
        public UnitFiltersLook(cBaseBusObject currObject)
        {   

            this.CurrentBusObj = currObject;
            InitializeComponent();
            FillValues();
            Init();
           
        }
        public void Init()
        {
            this.lcbFilter.SetBindingExpression("unit_md_id", "md_name", this.CurrentBusObj.GetTable("unit_md_category") as DataTable);
            this.lcbFilter.ComboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
                  if (cGlobals.ReturnParms.Count > 1)
            {

                lcbFilter.SelectedValue = _md_id;
                cGlobals.ReturnParms.Clear();
                _hasvars = true; 
                ComboBox_SelectionChanged(null, null);

            }
                         
        }
        public void FillValues()
        {
            if (cGlobals.ReturnParms.Count == 0)
            {
                tbSequence.Text = "0";
                _seqid = 0;
            }
            else
            {
                tbSequence.Text = cGlobals.ReturnParms[1].ToString();
                _filterid = Int32.Parse(cGlobals.ReturnParms[0].ToString());
                _seqid = Int32.Parse(cGlobals.ReturnParms[1].ToString());
                _md_id = Int32.Parse(cGlobals.ReturnParms[2].ToString());
                _operatorid = Int32.Parse(cGlobals.ReturnParms[3].ToString());
                _valueid = Int32.Parse(cGlobals.ReturnParms[4].ToString());
                  _construtorid = Int32.Parse(cGlobals.ReturnParms[5].ToString());
               
//                    lcbFilter.SelectedValue = _md_id;
               
            }

        }

        void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataRowView cbx = (DataRowView)lcbFilter.ComboBox.SelectedItem;
      
            if(lcbFilter.SelectedValue != null)
            {
            string TableName = "";
            string ValueField = "";
            string DisplayField = "";
            DataTable dcr = this.CurrentBusObj.ObjectData.Tables["unit_md_category"];
            var category = from x in dcr.AsEnumerable()
                           where x.Field<int>("unit_md_id") == (int)lcbFilter.SelectedValue
                           select new
                           {
                               tablename = x.Field<string>("maint_table_used"),
                               valuefield = x.Field<string>("id_field"),
                               displayfield = x.Field<string>("description_field")

                           };
            foreach (var ad in category)
            {
                TableName = ad.tablename;
                ValueField = ad.valuefield;
                DisplayField = ad.displayfield;
            }

            this.lcbValues.SetBindingExpression(ValueField, DisplayField, this.CurrentBusObj.GetTable(TableName) as DataTable, "");
            DataTable dt = new DataTable();
            dt = this.CurrentBusObj.GetTable("code") as DataTable;
            DataTable constructTable = new DataTable();
            DataTable operatorTable = new DataTable();
            constructTable = dt.Clone();
            operatorTable = dt.Clone();

             foreach (DataRow item in dt.Rows)
            {
                if (item["code_name"].ToString() == "FilterConstruct")
                {
                    constructTable.ImportRow(item);
                }
               if (item["code_name"].ToString() == "FilterOperator")
                {
                    
                            operatorTable.ImportRow(item);
                      
                }

            }
            this.lcbContract.SetBindingExpression("fkey_int", "code_value", constructTable, "");
            this.lcbOperator.SetBindingExpression("fkey_int", "code_value", operatorTable, "");
            if (_hasvars)
            {
               
                    lcbValues.SelectedValue = _valueid;
              
                lcbOperator.SelectedValue = _operatorid;
                lcbContract.SelectedValue = _construtorid;
                btnAdd.Content = "Modify";
                _hasvars = false;
            }

            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
           
            cGlobals.ReturnParms.Clear();
            if (lcbContract.SelectedText == string.Empty || lcbFilter.SelectedText == string.Empty || lcbOperator.SelectedText == string.Empty || lcbValues.SelectedText == string.Empty)
            {
            }
            else
            {
                DataRowView metadata = (DataRowView)lcbFilter.ComboBox.SelectedItem;
                DataRowView Operator = (DataRowView)lcbOperator.ComboBox.SelectedItem;
                DataRowView Construct = (DataRowView)lcbContract.ComboBox.SelectedItem;
                DataRowView Value = (DataRowView)lcbValues.ComboBox.SelectedItem;
                if (_filterid == null)
                {
                    _filterid = 0;
                }
                _seqid = Int32.Parse(tbSequence.Text);
                cGlobals.ReturnParms.Add(_filterid);
                cGlobals.ReturnParms.Add(0);
                cGlobals.ReturnParms.Add(Int32.Parse(lcbFilter.SelectedValue.ToString()));
                cGlobals.ReturnParms.Add(Int32.Parse(lcbOperator.SelectedValue.ToString()));

                cGlobals.ReturnParms.Add(lcbValues.SelectedValue.ToString());

                cGlobals.ReturnParms.Add(Int32.Parse(lcbContract.SelectedValue.ToString()));
                cGlobals.ReturnParms.Add(lcbFilter.SelectedText);
                cGlobals.ReturnParms.Add(lcbOperator.SelectedText);
                cGlobals.ReturnParms.Add(lcbValues.SelectedText);
                cGlobals.ReturnParms.Add(lcbContract.SelectedText);
            }
            this.Close();
        }
           
    }
}
