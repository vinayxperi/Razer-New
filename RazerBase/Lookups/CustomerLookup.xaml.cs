

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
    /// <summary>
    /// This class represents a 'CustomerLookup' object.
    /// </summary>
    public partial class CustomerLookup : DialogBase, IScreen
    {
        //private static readonly string customerLookupObject = "CustomerLookup";
        cBaseBusObject obj = null;

        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }
       
        /// <summary>
        /// Create a new instance of a 'CustomerLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerLookup()            
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            //Init(new cBaseBusObject(customerLookupObject));
        }

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

            if (cGlobals.ReturnParms.Count > 0)
            {
                txtReceivableAcct.Text = cGlobals.ReturnParms[0].ToString();
                FilterKeyPress(txtReceivableAcct, null);
                txtAcctName.Text = cGlobals.ReturnParms[1].ToString();
                FilterKeyPress(txtAcctName, null);
            }

            cGlobals.ReturnParms.Clear();
        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            obj = businessObject;
            // Set the ScreenBaseType
            //this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            txtAcctName.Focus();
            uBaseLookup.ClearLookup();

            //Define all lookup fields
            uBaseLookup.AddLookup("txtReceivableAcct", "receivable_account");
            uBaseLookup.AddLookup("txtAcctName", "account_name");
            uBaseLookup.AddLookup("txtCountry", "country");
            uBaseLookup.AddLookup("txtAddress1", "address_1");
            uBaseLookup.AddLookup("txtAcctDescription", "description");
            uBaseLookup.AddLookup("txtCity", "city");
            uBaseLookup.AddLookup("txtState", "state");

            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("receivable_account");
            // RES 3/22/12 Added account_name to fix error on lookup when clicking OK
            uBaseLookup.ReturnParmFields.Add("account_name");

            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            //uBaseLookup.uGrid.SelectProcedureIsQuery = true;
            uBaseLookup.uGrid.FieldLayoutResourceString = "CustomerLookupGrid";
            uBaseLookup.uGrid.ConfigFileName = "CustomerLookupGrid";
            uBaseLookup.uGrid.MainTableName = "Main";

            //Set the rows to change color based on account_status field
            Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

            FieldLayoutSettings f = new FieldLayoutSettings();
            f.DataRecordPresenterStyle = s;
            f.HighlightAlternateRecords = true;
            uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")
            GetLookupVals();

            uBaseLookup.uGrid.LoadGrid(obj, uBaseLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);

        }

        private void GetLookupVals()
        {
            obj = new cBaseBusObject();
            obj.BusObjectName = "CustomerLookup";
            //setup and pass chkbox value to stored proc for inactives
            int ShowInactive = 0;
            if (Convert.ToBoolean(chkInactive.IsChecked))
            {
                ShowInactive = 1;
            }
            ///////////////////////////////////////////////////////////
            obj.Parms.ClearParms();
            obj.Parms.AddParm("@show_inactive", ShowInactive.ToString());
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
            //cGlobals.ReturnParms.Add(r.Cells["receivable_account"].Value);
            //cGlobals.ReturnParms.Add(r.Cells["account_name"].Value);
            //this.Close();
            cGlobals.ReturnParms.Clear();
            List<string> dataKeys = new List<string>();
            dataKeys.Add("receivable_account");
            dataKeys.Add("account_name");
            uBaseLookup.uGrid.ReturnSelectedData(dataKeys);


            this.Close();

        }
        
    }
}
