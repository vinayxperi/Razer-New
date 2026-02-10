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
    /// Interaction logic for CashLookup.xaml
    /// </summary>
    public partial class CashLookup : DialogBase
    {
          #region Private Variables

        cBaseBusObject obj = null;

        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ContactLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public CashLookup()
            : base()
        {
            InitializeComponent();
            Init();
        }
        #endregion

        #region Events

        private void FilterKeyPress(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            //Filter returned data by individual key strokes
            uBaseLookup.FilterKeyPress(sender, e);
        }


        private void DialogBase_Loaded(object sender, RoutedEventArgs e)
        {
            //Check return parameters and add to filter cells

            if (cGlobals.ReturnParms.Count > 0)
            {
                cGlobals.ReturnParms.Clear();
            }

        }

        #endregion

        #region Methods

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
          
        public void Init()
        {
            uBaseLookup.ClearLookup();
            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("batch_id");
            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            //uBaseLookup.uGrid.SelectProcedureIsQuery = true;
            uBaseLookup.uGrid.FieldLayoutResourceString = "cash_lookupgrid";
            uBaseLookup.uGrid.MainTableName = "cashlookup";
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;
            //This is slowing down the load
            //Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

            //FieldLayoutSettings f = new FieldLayoutSettings();
            //f.DataRecordPresenterStyle = s;
            //uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")
            GetLookupVals();

            uBaseLookup.uGrid.LoadGrid(obj, uBaseLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
        }

        private void GetLookupVals()
        {
            obj = new cBaseBusObject("Cashlookup");
            obj.BusObjectName = "Cashlookup";

            obj.Parms.ClearParms();
            Load(obj);
        }

        public void BaseGridDoubleClickDelegate()
        {
            DataRecord r = default(DataRecord);
            //Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            //If error condition is received when retrieving selected row then the row of the currently active cell is used.
            try
            {
                r = (Infragistics.Windows.DataPresenter.DataRecord)uBaseLookup.uGrid.xGrid.SelectedItems.Records[0];
            }
            catch (Exception ex)
            {
                // for debugging only
                string err = ex.ToString();

                // Set the current record
                r = uBaseLookup.uGrid.xGrid.ActiveCell.Record;
            }
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(r.Cells["batch_id"].Value);
            this.Close();

        }


        #endregion

    }
}

