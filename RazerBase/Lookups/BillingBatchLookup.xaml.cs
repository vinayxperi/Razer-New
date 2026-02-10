

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

    #region class BillingBatachLookup
     
    public partial class BillingBatchLookup : DialogBase, IScreen
    {
        private static readonly string billingBatchResourceString = "BillingBatchLookup";
        private static readonly string billingBatchMainTable = "billingbatchlookup";
        private static readonly string batchIdField = "batch_id";
        private static readonly string batchNameField = "batch_name";

        cBaseBusObject businessObject;

        public string WindowCaption { get { return string.Empty; } }

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'EntityLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public BillingBatchLookup()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            //Init();
        }
        #endregion

        #region Events
        private void FilterKeyPress(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            //Filter returned data by individual key strokes
            uBaseLookup.FilterKeyPress(sender, e);
        }

        private void chkInactive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //load bus obj
            //GetLookupVals();

            Load(businessObject); 

            //Load the grid data from the base object
            //?? Why is this not automatically happening with the base?  Look to see if this can be fixed not to run
            uBaseLookup.uGrid.LoadGrid(businessObject, uBaseLookup.uGrid.MainTableName);
        }

        private void DialogBase_Loaded(object sender, RoutedEventArgs e)
        {
            //Check return parameters and add to filter cells

            if (cGlobals.ReturnParms.Count > 0)
            {
                txtBatchID.Text = cGlobals.ReturnParms[0].ToString();
                FilterKeyPress(txtBatchID, null);
                txtBatchName.Text = cGlobals.ReturnParms[1].ToString();
                FilterKeyPress(txtBatchName, null);
            }

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
            uBaseLookup.ReturnParmFields.Add("batch_id");

            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            uBaseLookup.uGrid.FieldLayoutResourceString = billingBatchResourceString;
            uBaseLookup.uGrid.MainTableName = billingBatchMainTable;            

            FieldLayoutSettings f = new FieldLayoutSettings();            
            uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            businessObject = busObject;
            Load(businessObject);            

            uBaseLookup.uGrid.LoadGrid(businessObject, uBaseLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
        }

        //private void GetLookupVals()
        //{
        //    obj = new cBaseBusObject();
        //    obj.BusObjectName = "BillingBatchLookup";
            
        //    Load(obj);
            
        //}

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
