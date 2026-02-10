using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;

namespace RazerBase.Lookups
{
    /// <summary>
    /// Interaction logic for AccountingClassLookup.xaml
    /// </summary>
    public partial class AccountingClassLookup : RazerBase.DialogBase
    {
        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }

        public AccountingClassLookup()
        {
    
            InitializeComponent();
            Init();
         
        }
       

        public void Init()
        {
            // Set the ScreenBaseType
            //this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //Instantiate the base business object
            //Set the business object name - the name should correspond to the name on the RObject table
            CurrentBusObj = new cBaseBusObject("AccountClassLookup");

            //Clear any lookup values
            uBaseLookup.ClearLookup();

            //Define all lookup fields
            //These fields are textboxes at the top of the screen that will auto filter as the users
            //type into the fields
            uBaseLookup.AddLookup("txtAcct", "acct_class");
            uBaseLookup.AddLookup("txtDescription", "description");
            
            
            //Add the return parameters
            uBaseLookup.ReturnParmFields.Add("acct_class_id");

            //Setup base grid information
            //The event to use when the grid is double clicked
            uBaseLookup.uGrid.WindowZoomDelegate = ReturnSelectedData;
            //Sets to row select and single select
            uBaseLookup.uGrid.SetGridSelectionBehavior(true, false);
            //uBaseLookup.uGrid.SelectProcedureIsQuery = true;
            //uBaseLookup.uGrid.AddParm("account_name", "%");

            //Set the grid display string to use
            uBaseLookup.uGrid.FieldLayoutResourceString = "AcctClassLookup";
            //Set the grid main table name from the RObject detail table
            uBaseLookup.uGrid.MainTableName = "acct_lookup";
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;
            ////Set the rows to change color based on account_status field
            //Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];


            //FieldLayoutSettings f = new FieldLayoutSettings();
            //f.DataRecordPresenterStyle = s;
            //uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")

            //Setup base parameters - No inactive / No Archived

            //Retrieves the base business object
            Load(CurrentBusObj);

            //Load the grid data from the base object
            //?? Why is this not automatically happening with the base?  Look to see if this can be fixed not to run
            uBaseLookup.uGrid.LoadGrid(CurrentBusObj, uBaseLookup.uGrid.MainTableName);
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
  //          txtContractEntity.Focus();


        }
        public void ReturnSelectedData()
        {
            //Establish a datarecord variable for retrieving the grid data
            DataRecord r = default(DataRecord);
            ////Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            ////If error condition is received when retrieving selected row then the row of the currently active cell is used.
            try
            {
                
                r = (Infragistics.Windows.DataPresenter.DataRecord)uBaseLookup.uGrid.xGrid.SelectedItems.Records[0];
            }
            catch
            {
                //// for debugging only
                //string err = ex.ToString();

                // Set the current record
                r = uBaseLookup.uGrid.xGrid.ActiveCell.Record;
            }
            ////Clear the return parms in case they have data
            cGlobals.ReturnParms.Clear();
            ////Add any return parms here pulling from the grid columns as needed
            cGlobals.ReturnParms.Add(r.Cells["acct_class_id"].Value);
            //cGlobals.ReturnParms.Add(r.Cells["contract_description"].Value);
       
            //Close the lookup
            this.Close();

        }
        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void FilterKeyPress(object sender, TextChangedEventArgs e)
        {
            uBaseLookup.FilterKeyPress(sender, e);
        }

        private void cbRatio_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void cbRatio_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private void txtAcct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
         
        }

    }
}
