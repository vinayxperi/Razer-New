using RazerInterface; //Required for IPreBindable
using RazerBase.Interfaces; //Required for IScreen
using RazerBase;
using RazerBase.Lookups;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DataPresenter;
using System;
using System.Data;
using System.Collections.Generic;
using Infragistics.Windows.Editors;
using Infragistics.Documents.Excel;
using System.Windows.Forms;
using System.IO;

namespace Units
{
    /// <summary>
    /// Interaction logic for UnitTypeSetup.xaml
    /// </summary>
    public partial class UnitTypeSetup :ScreenBase, IScreen, IPreBindable
    {
                //Property is required for base objects that use IScreen
        public string WindowCaption { get; private set; }

        //Screen Level Unit ID
        public int CurrentUnitTypeID { get; set; }

        //Setup any grid combobox providers - Variable name should match the Binding Path value in the field layout
        public ComboBoxItemsProvider cmbUnitMD { get; set; }

        ////Setup keys for double click zooms from grids
        //private List<string> gRemitDataKeys = new List<string> { "document_id"}; //Used for double click
        
    
        //Required constructor
        public UnitTypeSetup()
            :base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialization event - this event should setup all tabs and grids as well as be able to handle being passed
        /// information from a zoom from another window.
        /// </summary>
        /// <param name="businessObject"></param>
        public void Init(cBaseBusObject businessObject)
        {
            WindowCaption = "Unit Type Setup";
            
            //Setting intial screenstate to empty
            CurrentState = ScreenState.Empty;

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder ;

             //Set the maintablename for the folder if it has one
            this.MainTableName = "unit_type";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            //Add any Grid Configuration Information
            gXref.MainTableName = "unit_type_categories"; //Should match the ROBJECT table name
            gXref.ConfigFileName = "UnitTypeSetupGrid"; //This is the file name that will store any user customizations to the grid - must be unique in the app
            gXref.SetGridSelectionBehavior(true, false); //Sets standard grid behavior for record select and multiselect
            //gRemit.WindowZoomDelegate = RemitGridDoubleClick; //The function delegate determines what happens when the user double clicks the grid
            gXref.FieldLayoutResourceString = "UnitTypeSetupLayout"; //The name of the FieldLayout in the Field Layouts xaml file - Must be unique
            //gRemit.GridGotFocusDelegate = gRemitGrid_GotFocus; //This ties the got focus event of the remit base grid to this method.
            //gRemit.EditModeEndedDelegate = gRemitGrid_EditModeEnded; //This allows for data checks after each cell is exited
            //gRemit.SkipReadOnlyCellsOnTab = true; //Sets the grid to skip over non edit fields
            //gRemit.GridPreviewKeyDownDelegate = gRemitGrid_GridPreviewKeyDown;
            //gRemit.RecordActivatedDelegate = gRemit_RecordActivated;

            ////Turn off Grid config and filter abilities as these will mess up the hardwiring of tabbing thorugh grid
            //gRemit.ContextMenuToggleFilterIsVisible = false;
            //gRemit.ContextMenuResetGridSettingsIsVisible = false;
            //gRemit.ContextMenuSaveGridSettingsIsVisible = false;
            gXref.ContextMenuRemoveIsVisible = true;
            gXref.ContextMenuRemoveDelegate = DeleteRow;
            //gRemit.ContextMenuAddIsVisible = false;

 
   
            //Add all grids to the grid collection - This allows grids to automatically load and participate with security
            GridCollection.Add(gXref);

   
            // add the Tab user controls that are of type screen base - This screen has no detail tabs
            //TabCollection.Add(General);
 
 

            //Debug code for hardwiring a test parameter set
            //CurrentBusObj.Parms.AddParm("@batch_id", "1" );

            // if there are parameters passed into the window on startup then we need to load the data
            if (CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                txtUnitTypeID.Text = CurrentBusObj.Parms.GetParm("@unit_type_id");
                RetrieveData();
            }
            else
            {
                SetWindowStatus();
            }
            
        }

        private bool RetrieveData()
        {
            //Verify that no save is needed
            if (IsScreenDirty)
            {
                System.Windows.MessageBoxResult result = Messages.ShowYesNo("Would you like to save existing changes?",
                           System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    Save();
                    if (!SaveSuccessful)
                    {
                        return false;
                    }
                }
            }


            //If inserting to the screen then we will pass a dummy parm so that we can return empty tables
            if (CurrentState == ScreenState.Inserting)
            {
                //Remove any existing parameters
                CurrentBusObj.Parms.ClearParms();
                //Add all parameters back in
                CurrentBusObj.Parms.AddParm("@unit_type_id", "-1");
                CurrentBusObj.Parms.AddParm("@code_name", "UnitType");

                txtUnitTypeID.Text = "-1";
                //Loads a new record in the main table and empty datatbales elsewhere
                base.New();


            }
            //We have a batch ID to retrieve
            else
            {
                //test to make sure The batch is an integer
                int i;
                if (Int32.TryParse(txtUnitTypeID.Text, out i))
                {
                    //Remove existing paramters
                    CurrentBusObj.Parms.ClearParms();
                    //Add all parameters back in
                    CurrentBusObj.Parms.AddParm("@unit_type_id", txtUnitTypeID.Text);
                    CurrentBusObj.Parms.AddParm("@code_name", "UnitType");

                    //Empty the current object if any exists
                    if (CurrentBusObj.HasObjectData)
                        CurrentBusObj.ObjectData.Clear();

                    //Load the base business object and populate the window and tabs
                    this.Load();

                }
                else //If non numeric entered
                {
                    Messages.ShowError("Batch ID must be a numeric whole number value");
                    return false;
                }
            }



            //Verify that data was returned
            if (CurrentBusObj.ObjectData.Tables["unit_type"] != null && CurrentBusObj.ObjectData.Tables["unit_type"].Rows.Count > 0)
            {
                CurrentUnitTypeID = Convert.ToInt32(txtUnitTypeID.Text);

                SetWindowStatus();
                EnableGridEdit();

                return true;
            }
            else
            {
                Messages.ShowError("No data retrieved for batch id " + txtUnitTypeID.Text);
                //Reset the current ID
                CurrentUnitTypeID = 0;
                CurrentState = ScreenState.Empty;
                return false;
            }
        }

        public override void New()
        {
            //Set the screen state to inserting
            CurrentState = ScreenState.Inserting;

            if (!RetrieveData())
            {
                Messages.ShowError("Error creating new record.");
                CurrentState = ScreenState.Normal;
                return;
            }
            else //New record created
            {
                txtDescription.CntrlFocus();

            }

        }

        void EnableGridEdit()
        {
            if (CurrentState != ScreenState.Empty && SecurityContext != AccessLevel.NoAccess && SecurityContext != AccessLevel.ViewOnly)
            {
                gXref.xGrid.FieldSettings.AllowEdit = true;
                gXref.SetGridSelectionBehavior(false, false);
                gXref.xGrid.FieldLayoutSettings.AllowAddNew = true;
            }
 
        }
        void SetWindowStatus()
        {

        }

        void DeleteRow()
        {
            gXref.xGrid.ExecuteCommand(DataPresenterCommands.DeleteSelectedDataRecords);
        }


        public override void Save()
        {
            Prep_ucBaseGridsForSave();
            //Check that no duplicate metadata IDs exist on current row - If any exists then error and exit save
            if (CurrentBusObj.ObjectData.Tables["unit_type_categories"] != null &&
                CurrentBusObj.ObjectData.Tables["unit_type_categories"].Rows.Count > 1)
            {
                DataTable dtMD;
                dtMD = CurrentBusObj.ObjectData.Tables["unit_type_categories"];
                //Cycle through all rows and runquery to see how many IDs exist that are the same
                foreach (DataRow r in dtMD.Rows)
                {
                    if (r.RowState != DataRowState.Deleted)
                    {
                        //Had to remove the below as it would crash if any records had been deleted.
                        //EnumerableRowCollection<DataRow> MDRow = from a in dtMD.AsEnumerable()
                        //                                         where (a.Field<Int32>("unit_md_id") == Convert.ToInt32(r["unit_md_id"])
                        //                                            && a.RowState != DataRowState.Deleted)
                        //                                         select a;
                        int iCount = 0;
                        foreach (DataRow r1 in dtMD.Rows)
                        {
                            if (r1.RowState != DataRowState.Deleted && r1["unit_md_id"] == r["unit_md_id"])
                            {
                                iCount++;
                            }

                        }
                        //Any duplicate IDs will cause this branch to fire
                        if (iCount > 1)
                        {
                            Messages.ShowError("Cannot save with duplicate metadata.  Please verify that no duplicates exist and then try to save again.");
                            return;
                        }
                    }
                }
            }
            //If verified or deleting the save the data
            base.Save();
            if (SaveSuccessful)
            {

                //txtUnitTypeID.CntrlFocus();
                txtUnitTypeID.Text = CurrentBusObj.ObjectData.Tables["unit_type"].Rows[0]["unit_type_id"].ToString();
                Messages.ShowInformation("Save Successful");
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }

        }


        /// <summary>
        /// This event is used to handle the setup of any ucLabelComboBox controls and is required if the controls are to be used
        /// Since this event runs after the data is retrieved for the business object but before it is bound to the screen,
        /// it can also be used for any other activities that need to run then
        /// </summary>
        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    this.cbUnitType.SetBindingExpression("fkey_int", "code_value", this.CurrentBusObj.ObjectData.Tables["unit_type_list"]);

                    //Setup Grid Combo Boxes
                    //Currency combo box on remit grid
                    //Add code to populate the grid combobox on the Company Contract Grid
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip.ItemsSource = this.CurrentBusObj.ObjectData.Tables["md_categories"].DefaultView;
                    //set the value and display path
                    ip.ValuePath = "unit_md_id";
                    ip.DisplayMemberPath = "md_name";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbUnitMD = ip;

                }
            }
            catch (Exception error)
            {
                Messages.ShowError(error.ToString());
            }
        }

        private void txtUnitTypeID_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UnitTypeLookup w = new UnitTypeLookup();
            w.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                txtUnitTypeID.Text = cGlobals.ReturnParms[0].ToString();
                // Clear the Global parms
                //This prevents invalid data being passed to other lookups
                cGlobals.ReturnParms.Clear();
                CurrentState = ScreenState.Normal;
                RetrieveData();
            }
        }

        private void txtUnitTypeID_LostFocus(object sender, RoutedEventArgs e)
        {
             
            int i;
            Int32.TryParse(txtUnitTypeID.Text, out i);
            if (i != CurrentUnitTypeID)
            {
                CurrentState = ScreenState.Normal;
                RetrieveData();
            }

        }
    }
}
