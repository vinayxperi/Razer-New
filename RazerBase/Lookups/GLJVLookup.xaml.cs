

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

    #region classGLJVLookup
     
    public partial class GLJVLookup : DialogBase
    {
        cBaseBusObject obj = null;

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'EntityLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public GLJVLookup()
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

       
        private void DialogBase_Loaded(object sender, RoutedEventArgs e)
        {
            //Check return parameters and add to filter cells
            
            if (cGlobals.ReturnParms.Count > 0)
            {
                txtJournalID.Text = cGlobals.ReturnParms[0].ToString();
                FilterKeyPress(txtJournalID, null);
                txtDescription.Text = cGlobals.ReturnParms[1].ToString();
                FilterKeyPress(txtDescription, null);
            }

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
            uBaseLookup.AddLookup("txtJournalID", "journal_ctrl_num");
            uBaseLookup.AddLookup("txtDescription", "description");
            uBaseLookup.AddLookup("txtControlGroup", "control_group");
            uBaseLookup.AddLookup("txtDatePosted", "date_posted");
          

          
            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("journal_ctrl_num");
           
            //Setup base grid information
            uBaseLookup.uGrid.WindowZoomDelegate = BaseGridDoubleClickDelegate;
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            uBaseLookup.uGrid.FieldLayoutResourceString = "glVchrLookup";
            uBaseLookup.uGrid.MainTableName = "glJVlookup";

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
            obj.BusObjectName = "GLJVLookup";
            //setup and pass chkbox value to stored proc for inactives
             
            ///////////////////////////////////////////////////////////
            obj.Parms.ClearParms();
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
            cGlobals.ReturnParms.Add(r.Cells["journal_ctrl_num"].Value);
            cGlobals.ReturnParms.Add(r.Cells["description"].Value);
            this.Close();

        }


        #endregion

    }
    #endregion

}
