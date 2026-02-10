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

namespace RazerBase.Lookups
{
    /// <summary>
    /// Interaction logic for InvoiceLookup.xaml
    /// </summary>
    public partial class AdjustmentWHTLookup : DialogBase , IScreen
    {

        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }

        cBaseBusObject obj = null;

        public AdjustmentWHTLookup()
        {
            InitializeComponent();
                      
        }

  

        private void FilterKeyPress(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            //Filter returned data by individual key strokes
            uBaseLookup.FilterKeyPress(sender, e);
        }


        private void DialogBase_Loaded(object sender, RoutedEventArgs e)
        {
            //Check return parameters and add to filter cells

           
            cGlobals.ReturnParms.Clear();
        }

   

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            obj = businessObject;
            txtAcctName.Focus();
            
            uBaseLookup.ClearLookup();
            
            cGlobals.ReturnParms.Clear();
           
            //Define all lookup fields
            uBaseLookup.AddLookup("txtDocumentID", "document_id");
            uBaseLookup.AddLookup("txtAcctNumber", "receivable_account");
            uBaseLookup.AddLookup("txtAcctName", "account_name");
            uBaseLookup.AddLookup("txtCountry", "country");
            uBaseLookup.AddLookup("txtProvince", "province");
           

           
            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            uBaseLookup.uGrid.FieldLayoutResourceString = "GridAdjustmentWHTLookup";
            uBaseLookup.uGrid.MainTableName = "main";
 
            GridCollection.Add(uBaseLookup.uGrid);
            obj.BusObjectName = "AdjustmentWHTLookup";
            Load(obj);
            //GetLookupVals();
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);

        }

        public void BaseGridDoubleClickDelegate()
        {
            

            cGlobals.ReturnParms.Clear();
            List<string> dataKeys = new List<string>();
            dataKeys.Add("document_id");
           uBaseLookup.uGrid.ReturnSelectedData(dataKeys);
            this.Close();
        }



    }


}