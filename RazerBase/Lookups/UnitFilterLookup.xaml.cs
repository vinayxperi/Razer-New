



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
    /// This class represents a 'EntityLookup' object.
    /// </summary>
    public partial class UnitFilterLookup : DialogBase
    {
        cBaseBusObject obj = null;
        private int _md_id = 0;
 
        /// <summary>
        /// Create a new instance of a 'EntityLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public UnitFilterLookup()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
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

                txtFilterName.Text = cGlobals.ReturnParms[1].ToString();
                FilterKeyPress(txtFilterName, null);
                txtFilterDescription.Text = cGlobals.ReturnParms[2].ToString();
                FilterKeyPress(txtFilterDescription, null);
            }

            cGlobals.ReturnParms.Clear();
         
        }
        





        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            //this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            uBaseLookup.ClearLookup();

            //Define all lookup fields
            uBaseLookup.AddLookup("txtFilterName", "filter_name");
            uBaseLookup.AddLookup("txtFilterDescription", "filter_description");
        
           
            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("filter_id");

            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            uBaseLookup.uGrid.FieldLayoutResourceString = "unit_filter_lookup";
            uBaseLookup.uGrid.MainTableName = "unit_filter_lookup";

            //Set the rows to change color based on account_status field
           //Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

            FieldLayoutSettings f = new FieldLayoutSettings();
            //f.DataRecordPresenterStyle = s;
            f.HighlightAlternateRecords = true;
            uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")
            GetLookupVals();

            uBaseLookup.uGrid.LoadGrid(obj, uBaseLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            txtFilterName.Focus();

        }

        private void GetLookupVals()
        {
            obj = new cBaseBusObject();
            obj.BusObjectName = "UnitFilter";
            //setup and pass chkbox value to stored proc for inactives
            int ShowInactive = 0;
            if (Convert.ToBoolean(chkInactive.IsChecked))
            {
                ShowInactive = 1;
            }
            ///////////////////////////////////////////////////////////
            obj.Parms.ClearParms();
            obj.Parms.AddParm("@show_inactive", ShowInactive.ToString());
            obj.Parms.AddParm("@filter_id", 0);
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
            cGlobals.ReturnParms.Add(r.Cells["filter_id"].Value);
            //cGlobals.ReturnParms.Add(r.Cells["name"].Value);
            this.Close();

        }



    }


}
