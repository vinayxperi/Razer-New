using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Infragistics.Windows.DataPresenter;
using System.Windows;


namespace RazerBase.Lookups
{
    /// <summary>
    /// Interaction logic for MultiCustomerLookup.xaml
    /// </summary>
    public partial class MultiCustomerLookup : DialogBase
    {
        cBaseBusObject obj = null;
        public string company_code;
        public MultiCustomerLookup()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            uMultiCustomerLookup.ClearLookup();
           
            //Setup base grid information
            uMultiCustomerLookup.uGrid.SetGridSelectionBehavior(true, false);
            //uBaseLookup.uGrid.SelectProcedureIsQuery = true;
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            uMultiCustomerLookup.uGrid.xGrid.FieldLayoutSettings = f;
            uMultiCustomerLookup.uGrid.FieldLayoutResourceString = "multiCustomerLookup";
            uMultiCustomerLookup.uGrid.MainTableName = "multiCustomer";

            //This is slowing down the load
            //Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

            //FieldLayoutSettings f = new FieldLayoutSettings();
            //f.DataRecordPresenterStyle = s;
            //uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")
            GetLookupVals();

            uMultiCustomerLookup.uGrid.LoadGrid(obj, uMultiCustomerLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
        }

        private void GetLookupVals()
        {
            obj = new cBaseBusObject("MultiCustomerLookup");
            obj.BusObjectName = "MultiCustomerLookup";
            if (cGlobals.ReturnParms.Count > 0)
            {
                obj.Parms.ClearParms();
                obj.Parms.AddParm("@document_id", cGlobals.ReturnParms[0].ToString());
                //cGlobals.ReturnParms.Clear();
                Load(obj);
            }
           
        }
        public void BaseGridDoubleClickDelegate()
        {
            DataRecord r = default(DataRecord);
            //Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            //If error condition is received when retrieving selected row then the row of the currently active cell is used.
            try
            {
                r = (Infragistics.Windows.DataPresenter.DataRecord)uMultiCustomerLookup.uGrid.xGrid.SelectedItems.Records[0];
            }
            catch (Exception ex)
            {
                // for debugging only
                string err = ex.ToString();

                // Set the current record
                r = uMultiCustomerLookup.uGrid.xGrid.ActiveCell.Record;
            }
            cGlobals.ReturnParms.Clear();
            
            this.Close();

        }

    }
}
