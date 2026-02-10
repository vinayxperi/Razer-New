

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

#endregion

namespace RazerBase.Lookups
{

    #region class InvoiceFolderLookup
    /// <summary>
    /// This class represents a 'EntityLookup' object.
    /// </summary>
    public partial class InvoiceFolderLookup : DialogBase
    {
        cBaseBusObject obj = null;
        bool IsColsing = false;

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'EntityLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public InvoiceFolderLookup()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
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
            GetLookupVals();

            //Load the grid data from the base object
            //?? Why is this not automatically happening with the base?  Look to see if this can be fixed not to run
            uBaseLookup.uGrid.LoadGrid(obj, uBaseLookup.uGrid.MainTableName);
        }

        private void DialogBase_Loaded(object sender, RoutedEventArgs e)
        {
            //Check return parameters and add to filter cells
            txtInvoiceNbr.Focus();
            if (cGlobals.ReturnParms.Count > 0)
            {
                txtInvoiceNbr.Text = cGlobals.ReturnParms[0].ToString();
                FilterKeyPress(txtInvoiceNbr, null);
              //terKeyPress(txt  txtEntityName.Text = cGlobals.ReturnParms[1].ToString();
              //  FilEntityName, null);
            }

            cGlobals.ReturnParms.Clear();
        }


        #endregion

        #region Methods

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            //this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            uBaseLookup.ClearLookup();

            //Define all lookup fields
            uBaseLookup.AddLookup("txtInvoiceNbr", "invoice_number");
            uBaseLookup.AddLookup("txtAccountName", "account_name");
            
           
            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("invoice_number");

            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            uBaseLookup.uGrid.FieldLayoutResourceString = "InvoiceFolderLookup";
            uBaseLookup.uGrid.MainTableName = "invoicefolderlookup";

            //Set the rows to change color based on account_status field
           //Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
             uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")
            GetLookupVals();

            uBaseLookup.uGrid.LoadGrid(obj, uBaseLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);

            this.Closing += new System.ComponentModel.CancelEventHandler(InvoiceFolderLookup_Closing);
        }

        void InvoiceFolderLookup_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsColsing = true;
            //BaseGridDoubleClickDelegate();
        }

        private void GetLookupVals()
        {
            obj = new cBaseBusObject();
            obj.BusObjectName = "InvoiceFolderLookup";
            
           
            Load(obj);
            
        }

        public void BaseGridDoubleClickDelegate()
        {
            DataRecord r = default(DataRecord);
            //if (r == null)
            //{
            //}
            //else
            //{
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
                cGlobals.ReturnParms.Add(r.Cells["invoice_number"].Value);
            //}
            
            if (!IsColsing) { this.Close(); }
        }


        #endregion

    }
    #endregion

}
