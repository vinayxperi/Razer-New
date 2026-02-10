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
    /// Interaction logic for UnitMetaDataLookup.xaml
    /// </summary>
    public partial class UnitMetaDataLookup : DialogBase
    {
          private int _md_id = 0;
        private int _fkey_id = 0;
        private int _unit_id = 0;
        public UnitMetaDataLookup(cBaseBusObject currObject)
        {
            this.CurrentBusObj = currObject;
            InitializeComponent();
            FillValues();
            Init();
           
        }
        public void Init()
        {
            this.lcbFilter.SetBindingExpression("unit_md_id", "md_name", this.CurrentBusObj.GetTable("unit_md_lookup") as DataTable);
            this.lcbFilter.ComboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            if (cGlobals.ReturnParms.Count > 1)
            {

                lcbFilter.SelectedValue = _md_id;
                cGlobals.ReturnParms.Clear();
                ComboBox_SelectionChanged(null, null);

            }

        }
        public void FillValues()
        {
            if (cGlobals.ReturnParms.Count> 0)
            {
                _unit_id = Int32.Parse(cGlobals.ReturnParms[0].ToString());
                if (cGlobals.ReturnParms.Count > 1)
                {
                    _fkey_id = Int32.Parse(cGlobals.ReturnParms[2].ToString());
                    _md_id = Int32.Parse(cGlobals.ReturnParms[1].ToString());
                    btnAdd.Content = "Modify";
                }
            }
           
        }
        void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataRowView cbx = (DataRowView)lcbFilter.ComboBox.SelectedItem;

            if (lcbFilter.SelectedValue != null)
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
                if (_fkey_id != 0)
                {
                    lcbValues.SelectedValue = _fkey_id;
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(Int32.Parse(lcbFilter.SelectedValue.ToString()));
            cGlobals.ReturnParms.Add(Int32.Parse(lcbValues.SelectedValue.ToString()));
            cGlobals.ReturnParms.Add(_unit_id);
            cGlobals.ReturnParms.Add(lcbFilter.SelectedText);
            cGlobals.ReturnParms.Add(lcbValues.SelectedText);
            this.Close();

        }
    }
}
