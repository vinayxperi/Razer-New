

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

    #region class ContactLookup
    /// <summary>
    /// This class represents a 'ContactLookup' object.
    /// </summary>
    public partial class ContactLookup : DialogBase
    {

        #region Private Variables

        cBaseBusObject obj = null;

        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ContactLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public ContactLookup()
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

            if (cGlobals.ReturnParms.Count > 0)
            {
                txtContactId.Text = cGlobals.ReturnParms[0].ToString();
                FilterKeyPress(txtContactId, null);
                txtContactId.Text = cGlobals.ReturnParms[1].ToString();
                FilterKeyPress(txtContactId, null);
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
            cGlobals.ReturnParms.Clear();
            uBaseLookup.ClearLookup();

            //Define all lookup fields
            uBaseLookup.AddLookup("txtContactId", "contact_id");
            uBaseLookup.AddLookup("txtContactName", "full_name");
            uBaseLookup.AddLookup("txtAddress", "address_1");
            uBaseLookup.AddLookup("txtCity", "city");
            uBaseLookup.AddLookup("txtState", "state");
            uBaseLookup.AddLookup("txtDesc", "description");

            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("contact_id");

            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            //uBaseLookup.uGrid.SelectProcedureIsQuery = true;
            uBaseLookup.uGrid.FieldLayoutResourceString = "ContactLookupGrid";
            uBaseLookup.uGrid.MainTableName = "contact_lookup";
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;
            //Set the rows to change color based on account_status field
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
            obj = new cBaseBusObject("ContactLookup");
            obj.BusObjectName = "ContactLookup";
            //setup and pass chkbox value to stored proc for inactives
            int ShowInactive = 0;
            if (Convert.ToBoolean(chkInactive.IsChecked))
            {
                ShowInactive = 1;
            }
            ///////////////////////////////////////////////////////////
            obj.Parms.ClearParms();
            //obj.Parms.AddParm("@ShowInactives", ShowInactive.ToString());
            obj.Parms.AddParm("@ShowInactives", "-1");
            Load(obj);
        }

        public void BaseGridDoubleClickDelegate()
        {
            DataRecord r = default(DataRecord);
            if (uBaseLookup.uGrid.xGrid.SelectedItems.Records.Count > 0)
            {
                r = (Infragistics.Windows.DataPresenter.DataRecord)uBaseLookup.uGrid.xGrid.SelectedItems.Records[0];
                cGlobals.ReturnParms.Clear();
                cGlobals.ReturnParms.Add(r.Cells["contact_id"].Value); 
                this.Close();
            }
        }


        #endregion

    }
    #endregion

}
