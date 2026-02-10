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
using Infragistics.Windows.Editors;

namespace RazerBase.Lookups
{
    /// <summary>
    /// Interaction logic for ProductLookup.xaml
    /// </summary>
    public partial class ProductLookup : DialogBase
    {
        public ProductLookup(cBaseBusObject currObject)
        {
            this.CurrentBusObj = currObject;
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            ucBaseGrid1.MainTableName = "entry_products";
            ucBaseGrid1.ConfigFileName = "GridRemitUnits";
            ucBaseGrid1.xGrid.FieldLayoutSettings = f;
            ucBaseGrid1.SetGridSelectionBehavior(false, true);
            ucBaseGrid1.FieldLayoutResourceString = "cash_products1";
            ucBaseGrid1.xGrid.FieldSettings.AllowEdit = false;
            ucBaseGrid1.WindowZoomDelegate = BaseGridDoubleClickDelegate;

            ucBaseGrid1.LoadGrid(CurrentBusObj, "entry_products");
            GridCollection.Add(ucBaseGrid1);
        }
        public void BaseGridDoubleClickDelegate()
        {
            DataRecord r = default(DataRecord);
            //Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            //If error condition is received when retrieving selected row then the row of the currently active cell is used.
            try
            {
                r = (Infragistics.Windows.DataPresenter.DataRecord)ucBaseGrid1.xGrid.SelectedItems.Records[0];
            }
            catch (Exception ex)
            {
                // for debugging only
                string err = ex.ToString();

                // Set the current record
                r = ucBaseGrid1.xGrid.ActiveCell.Record;
            }
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(r.Cells["product_code"].Value);
            this.Close();

        }

    }
}
