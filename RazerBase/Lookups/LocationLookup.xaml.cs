

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

    #region class LocationLookup
    /// <summary>
    /// This class represents a 'EntityLookup' object.
    /// </summary>
    public partial class LocationLookup : DialogBase
    {
        cBaseBusObject obj = null;
        private static readonly string csID = "cs_id";
        private static readonly string csName = "name";

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'EntityLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public LocationLookup()
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
            //if (cGlobals.ReturnParms.Count == 1)
            //{
            //    _mso_id = cGlobals.ReturnParms[0].ToString();
            //    txtOwnerMSO.Text = _mso_id;
            //    txtOwnerMSO.IsEnabled = false;
            //    cGlobals.ReturnParms.Clear();
            //    FilterKeyPress(txtOwnerMSO, null);
            //}
            //if (cGlobals.ReturnParms.Count > 0)
            //{
            //    txtLocationID.Text = cGlobals.ReturnParms[0].ToString();
            //    FilterKeyPress(txtLocationID, null);
            //    txtLocationName.Text = cGlobals.ReturnParms[1].ToString();
            //    FilterKeyPress(txtLocationName, null);
            //}
             
            cGlobals.ReturnParms.Clear();
        }
        private string _mso_id; 

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
            uBaseLookup.AddLookup("txtLocationID", "cs_id");
            uBaseLookup.AddLookup("txtLocationName", "name");
            uBaseLookup.AddLookup("txtOwnerMSO", "owner_mso");
            uBaseLookup.AddLookup("txtEntityName", "mso_name");
            uBaseLookup.AddLookup("txtPSACity", "psa_city");
            uBaseLookup.AddLookup("txtPSAState", "psa_state");

          
            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("cs_id");
            uBaseLookup.ReturnParmFields.Add("name");
           
            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, true);
            uBaseLookup.uGrid.FieldLayoutResourceString = "locationlookup";
            uBaseLookup.uGrid.MainTableName = "locationlookup";

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
            obj = new cBaseBusObject();
            obj.BusObjectName = "LocationLookup";
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
            //cGlobals.ReturnParms.Add(r.Cells["cs_id"].Value);
            //cGlobals.ReturnParms.Add(r.Cells["name"].Value);
            //this.Close();

            List<string> dataKeys = new List<string>();
            dataKeys.Add(csID);
            dataKeys.Add(csName);
            uBaseLookup.uGrid.ReturnSelectedData(dataKeys);

            this.Close();   

        }


        #endregion

    }
    #endregion

}
