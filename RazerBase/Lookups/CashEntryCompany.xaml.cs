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
    /// Interaction logic for CashEntryCompany.xaml
    /// </summary>
    public partial class CashEntryCompany : DialogBase
    {
        cBaseBusObject obj = null;
        public string company_code;
        public CashEntryCompany()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            uEntryLookup.ClearLookup();
            //Add the return parameters
            uEntryLookup.ReturnParmFields.Add("@document_id");
            uEntryLookup.ReturnParmFields.Add("@seq_code");
            uEntryLookup.ReturnParmFields.Add("@company_code");
            uEntryLookup.ReturnParmFields.Add("@product_code");
            //Setup base grid information
            uEntryLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uEntryLookup.uGrid.SetGridSelectionBehavior(true, false);
            //uBaseLookup.uGrid.SelectProcedureIsQuery = true;
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            uEntryLookup.uGrid.xGrid.FieldLayoutSettings = f;
            uEntryLookup.uGrid.FieldLayoutResourceString = "cash_invoice";
            uEntryLookup.uGrid.MainTableName = "company_lookup";

            //This is slowing down the load
            //Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

            //FieldLayoutSettings f = new FieldLayoutSettings();
            //f.DataRecordPresenterStyle = s;
            //uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")
            GetLookupVals();

            uEntryLookup.uGrid.LoadGrid(obj, uEntryLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
        }

        private void GetLookupVals()
        {
            obj = new cBaseBusObject("CashEntryLookup");
            obj.BusObjectName = "CashEntryLookup";
            if (cGlobals.ReturnParms.Count > 0)
            {
                obj.Parms.ClearParms();
                obj.Parms.AddParm("@receivable_account", cGlobals.ReturnParms[0].ToString());
                cGlobals.ReturnParms.Clear();
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
                r = (Infragistics.Windows.DataPresenter.DataRecord) uEntryLookup.uGrid.xGrid.SelectedItems.Records[0];
            }
            catch (Exception ex)
            {
                // for debugging only
                string err = ex.ToString();

                // Set the current record
                r = uEntryLookup.uGrid.xGrid.ActiveCell.Record;
            }
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(r.Cells["document_id"].Value);
            cGlobals.ReturnParms.Add(r.Cells["seq_code"].Value);
            cGlobals.ReturnParms.Add(r.Cells["company_code"].Value);
            cGlobals.ReturnParms.Add(r.Cells["product_code"].Value);
            cGlobals.ReturnParms.Add(r.Cells["amount"].Value); 
           
            this.Close();

        }

    }
}
