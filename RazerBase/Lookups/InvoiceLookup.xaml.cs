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
    public partial class InvoiceLookup : DialogBase , IScreen
    {

        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }

        cBaseBusObject obj = null;

        public InvoiceLookup()
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
            obj.Parms.AddParm("@to_currency", cGlobals.ReturnParms[0]);
            uBaseLookup.ClearLookup();
            
            cGlobals.ReturnParms.Clear();
           
            //Define all lookup fields
            uBaseLookup.AddLookup("txtDocumentID", "document_id");
            uBaseLookup.AddLookup("txtAcctName", "account_name");
            uBaseLookup.AddLookup("txtCompanyName", "company_description");
            uBaseLookup.AddLookup("txtDetailType", "detail_type");
            uBaseLookup.AddLookup("txtProductCode", "product_code");
            uBaseLookup.AddLookup("txtCity", "city");
            uBaseLookup.AddLookup("txtState", "state");
            uBaseLookup.AddLookup("txtPayorName", "payor_name");

            ////Add the return parameters
            //uBaseLookup.ReturnParmFields.Add("document_id");
            //uBaseLookup.ReturnParmFields.Add("seq_code");        

            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            uBaseLookup.uGrid.FieldLayoutResourceString = "OpenCashLookup";
            uBaseLookup.uGrid.MainTableName = "main";
 
            GridCollection.Add(uBaseLookup.uGrid);
            obj.BusObjectName = "CashOpenLookup";
            Load(obj);
            //GetLookupVals();
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);

        }

        public void BaseGridDoubleClickDelegate()
        {
            //DataRecord r = default(DataRecord);
            //if (uBaseLookup.uGrid.xGrid.SelectedItems.Records.Count > 0)
            //{
            //    r = (Infragistics.Windows.DataPresenter.DataRecord)uBaseLookup.uGrid.xGrid.SelectedItems.Records[0];
            //    cGlobals.ReturnParms.Clear();
            //    cGlobals.ReturnParms.Add(r.Cells["document_id"].Value);
            //    cGlobals.ReturnParms.Add(r.Cells["seq_code"].Value);
            //    this.Close();
            //}

            cGlobals.ReturnParms.Clear();
            List<string> dataKeys = new List<string>();
            dataKeys.Add("document_id");
            dataKeys.Add("seq_code");
            uBaseLookup.uGrid.ReturnSelectedData(dataKeys);
            this.Close();
        }



    }


}