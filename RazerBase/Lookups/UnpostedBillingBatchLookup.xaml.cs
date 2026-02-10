

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using RazerBase.Interfaces;

#endregion

namespace RazerBase.Lookups
{

    #region class UnpostedBillingBatachLookup
     
    public partial class UnpostedBillingBatchLookup : DialogBase, IScreen
    {
        private static readonly string billingBatchResourceString = "BillingBatchLookup";
        private static readonly string billingBatchMainTable = "unpostedbillingbatchlookup";
        private static readonly string batchIdField = "batch_id";
        private static readonly string batchNameField = "batch_name";
        cBaseBusObject obj = null;
        cBaseBusObject businessObject;

        public string WindowCaption { get { return string.Empty; } }
        
        #region Constructor
        /// <summary>
        /// Create a new instance of a 'EntityLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public UnpostedBillingBatchLookup()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            obj = new cBaseBusObject();
            obj.BusObjectName = "UnpostedBillBatchLookup";
            // Perform initializations for this object
            Init(obj);
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


            cGlobals.ReturnParms.Clear();
        }

        


        #endregion

        #region Methods

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject busObject)
        {
            // Set the ScreenBaseType
            //this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            uBaseLookup.ClearLookup();

            //Define all lookup fields
            uBaseLookup.AddLookup("txtBatchID", batchIdField);
            uBaseLookup.AddLookup("txtBatchName", batchNameField);
             
           
            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add(batchIdField );
            uBaseLookup.ReturnParmFields.Add(batchNameField );

            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            uBaseLookup.uGrid.FieldLayoutResourceString = billingBatchResourceString;
            uBaseLookup.uGrid.MainTableName = billingBatchMainTable;

            FieldLayoutSettings f = new FieldLayoutSettings();
            //f.DataRecordPresenterStyle = s;
            uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")
            GetLookupVals();

            uBaseLookup.uGrid.LoadGrid(obj, uBaseLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
           
        }

        private void GetLookupVals()
        {
            Load(obj);
        }
        public void BaseGridDoubleClickDelegate()
        {
            //DataRecord r = default(DataRecord);
            ////Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            ////If error condition is received when retrieving selected row then the row of the currently active cell is used.
            //try
            //{
            //    r = (Infragistics.Windows.DataPresenter.DataRecord)uBaseLookup.uGrid.xGrid.SelectedItems.Records[0];
            //}
            //catch (Exception ex)
            //{
            //    // for debugging only
            //    string err = ex.ToString();

            //    // Set the current record
            //    r = uBaseLookup.uGrid.xGrid.ActiveCell.Record;
            //}
            //cGlobals.ReturnParms.Clear();
            //cGlobals.ReturnParms.Add(r.Cells["batch_id"].Value);
            //cGlobals.ReturnParms.Add(r.Cells["batch_name"].Value);

            List<string> dataKeys = new List<string>();
            dataKeys.Add(batchIdField);
            dataKeys.Add(batchNameField);
            uBaseLookup.uGrid.ReturnSelectedData(dataKeys);

            this.Close();            
        }


        #endregion

       
    }
    #endregion

}
