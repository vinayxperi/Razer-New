
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

  #region class CashBatchDocumentLookup
    /// <summary>
    /// This class represents CashDocumentLookup object.
    /// </summary>
    public partial class CashBatchDocumentLookup : DialogBase, IScreen
    {
        cBaseBusObject obj = null;
        private static readonly string batchID = "batch_id";
        private static readonly string batchTotal = "batch_total";
      
        public string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }
       

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'EntityLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public CashBatchDocumentLookup()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            obj = new cBaseBusObject();
            obj.BusObjectName = "CashBatchDocumentLookup";
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
        public void Init(cBaseBusObject obj)
        {
            // Set the ScreenBaseType
            //this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //txtCustomerName.Focus();
            uBaseLookup.ClearLookup();

            //Define all lookup fields
            uBaseLookup.AddLookup("txtBatchID", "batch_id");
            uBaseLookup.AddLookup("txtBank", "bank_id");
            uBaseLookup.AddLookup("txtBatchDate", "batch_date");
            uBaseLookup.AddLookup("txtCheckNumber", "remit_id");


            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("batch_id");
            uBaseLookup.ReturnParmFields.Add("batch_total");
            //Setup base grid information
            uBaseLookup.uGrid.IsFilterable = true;
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            uBaseLookup.uGrid.FieldLayoutResourceString = "viewBatchforCorrectBankAcctLookup";
            uBaseLookup.uGrid.MainTableName = "lookup";

            //Set the rows to change color based on account_status field
            //Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

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
            DataRecord r = default(DataRecord);
            ////Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            ////If error condition is received when retrieving selected row then the row of the currently active cell is used.
            //try
            //{
            //r = (Infragistics.Windows.DataPresenter.DataRecord)uBaseLookup.uGrid.xGrid.SelectedItems.Records[0];
            //}
            //catch (Exception ex)
            //{

            //r = uBaseLookup.uGrid.xGrid.ActiveCell.Record;
            ////}
            //cGlobals.ReturnParms.Clear();
            //cGlobals.ReturnParms.Add(r.Cells["document_id"].Value);
            //cGlobals.ReturnParms.Add(r.Cells["batch_id"].Value);
            //cGlobals.ReturnParms.Add(r.Cells["remit_number"].Value);
            //this.Close();

            List<string> dataKeys = new List<string>();
          
            dataKeys.Add(batchID);
            dataKeys.Add(batchTotal);
            uBaseLookup.uGrid.ReturnSelectedData(dataKeys);

            this.Close();   

        }


        #endregion

        private void FilterKeyPress(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }




        //public void Init(cBaseBusObject businessObject)
        //{
        //    throw new NotImplementedException();
        //}

       
    }
    #endregion

}
