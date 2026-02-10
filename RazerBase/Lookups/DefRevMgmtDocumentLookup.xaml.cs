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
    /// Interaction logic for DefRevMgmtDocumentLookup.xaml
    /// </summary>
    public partial class DefRevMgmtDocumentLookup : DialogBase, IScreen
    {
        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }

        public DefRevMgmtDocumentLookup()
        {
            InitializeComponent();
            //Init();
        }

        /// <summary>
        /// Add this event to the text boxes that users can filter on with individual keystrokes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterKeyPress(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            //Filter returned data by individual key strokes
            uBaseLookup.FilterKeyPress(sender, e);
        }



        private void DialogBase_Loaded(object sender, RoutedEventArgs e)
        {
            ////Check return parameters and add to filter cells

            //if (cGlobals.ReturnParms.Count > 0)
            //{
            //    //If data is passed to the lookup window then set the values here
            //    //so that the filtering will automatically be populated
            //    txtContractID.Text = cGlobals.ReturnParms[0].ToString();
            //    FilterKeyPress(txtContractID, null);
            //}

            //Clear the return parameters so that they will be ready for the user selection
            cGlobals.ReturnParms.Clear();
        }



        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            //this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //Instantiate the base business object
            //Set the business object name - the name should correspond to the name on the RObject table
            //CurrentBusObj = new cBaseBusObject("ContractLookup");
            CurrentBusObj = businessObject;

            //Instansiate lookup datatable and Clear any lookup values
            uBaseLookup.ClearLookup();

            //Define all lookup fields
            //These fields are textboxes at the top of the screen that will auto filter as the users
            //type into the fields
            uBaseLookup.AddLookup("txtDocumentID", "Document");
            uBaseLookup.AddLookup("txtCustomerName", "account_name");


            //Add the return parameters
            //uBaseLookup.ReturnParmFields.Add("contract_id");

            //Setup base grid information
            //The event to use when the grid is double clicked
            uBaseLookup.uGrid.WindowZoomDelegate = ReturnSelectedData;
            //Sets to row select and single select
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            //uBaseLookup.uGrid.SelectProcedureIsQuery = true;
            //uBaseLookup.uGrid.AddParm("account_name", "%");

            //Set the grid display string to use
            uBaseLookup.uGrid.FieldLayoutResourceString = "DefRevMgmtDocumentLookupGrid";
            //Set the grid main table name from the RObject detail table
            uBaseLookup.uGrid.MainTableName = "document_lookup";
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;
            ////Set the rows to change color based on account_status field
            //Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

            uBaseLookup.ReturnParmFields.Add("Document");

            //FieldLayoutSettings f = new FieldLayoutSettings();
            //f.DataRecordPresenterStyle = s;
            //uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")

            //Retrieves the base business object
            Load(CurrentBusObj);

            //Load the grid data from the base object
            //?? Why is this not automatically happening with the base?  Look to see if this can be fixed not to run
            uBaseLookup.uGrid.LoadGrid(CurrentBusObj, uBaseLookup.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            //txtContractID.Focus();
            //uBaseLookup.uGrid.IsFilterable = true;

        }

        /// <summary>
        /// Runs when grid is doubleclicked and will normally be used to return parameters to the calling window
        /// </summary>
        public void ReturnSelectedData()
        {
            //Establish a datarecord variable for retrieving the grid data
            //DataRecord r = default(DataRecord);
            ////Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            ////If error condition is received when retrieving selected row then the row of the currently active cell is used.
            //try
            //{
            //    if (uBaseLookup.uGrid.xGrid.ActiveCell == null) return;
            //    r = (Infragistics.Windows.DataPresenter.DataRecord)uBaseLookup.uGrid.xGrid.SelectedItems.Records[0];
            //}
            //catch 
            //{
            //    //// for debugging only
            //    //string err = ex.ToString();

            //    // Set the current record
            //    r = uBaseLookup.uGrid.xGrid.ActiveCell.Record;
            //}
            ////Clear the return parms in case they have data
            //cGlobals.ReturnParms.Clear();
            //Add any return parms here pulling from the grid columns as needed
            //cGlobals.ReturnParms.Add(r.Cells["contract_id"].Value);
            //cGlobals.ReturnParms.Add(r.Cells["contract_description"].Value);
            List<string> dataKeys = new List<string>();
            dataKeys.Add("Document");

            uBaseLookup.uGrid.ReturnSelectedData(dataKeys);
            //Close the lookup
            this.Close();

        }

        ///// <summary>
        ///// Use this event if you need true / false filtering for data flags
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void chkBox_Click(object sender, RoutedEventArgs e)
        //{
        //    //Event modifies the lookup parms and reretrives lookup data if the archived or inactive flags are checked.
        //    //If a row is checked as archived then this routine will automatically check inactive as well
        //    //since a contract cannot be archived unless it is inactive.
        //    int ShowArchived = 0;
        //    int ShowInactive = 0;
        //    CurrentBusObj.Parms.ClearParms();
        //    if (Convert.ToBoolean(chkArchive.IsChecked))
        //    {
        //        ShowArchived = 1;
        //        chkInactive.IsChecked = true;
        //    }
        //    if (Convert.ToBoolean(chkInactive.IsChecked))
        //    {
        //        ShowInactive = 1;
        //    }

        //    //Add the business object parms
        //    CurrentBusObj.Parms.ClearParms();
        //    CurrentBusObj.Parms.AddParm("@show_inactive", Convert.ToString(ShowInactive));
        //    CurrentBusObj.Parms.AddParm("@show_archived", Convert.ToString(ShowArchived));
        //    //Load the object data
        //    Load(CurrentBusObj);

        //    //Load the grid data from the base object
        //    //?? Why is this not automatically happening with the base?  Look to see if this can be fixed not to run
        //    uBaseLookup.uGrid.LoadGrid(CurrentBusObj, uBaseLookup.uGrid.MainTableName);
        //}



    }
}
