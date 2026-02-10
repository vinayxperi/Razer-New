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

using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;

using Infragistics.Windows.DataPresenter.Events;



namespace RazerBase.Lookups
{
    /// <summary>
    /// Interaction logic for BCFLookup.xaml
    /// </summary>
    public partial class TFInvoiceLookup : DialogBase, IScreen
    {
        private string windowCaption;
        DataTable dtLookup;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }

        public TFInvoiceLookup()
        {
            InitializeComponent();
            //Init();
        }

        /// <summary>
        /// Add this event to the text boxes that users can filter on with individual keystrokes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        public List<string> ReturnParmFields = new List<string>();




        public void ClearLookup()
        {
            //Sub to reinitialize the dtlookup table
            dtLookup = new DataTable("lookup");

            //Column to store the name of the text container to be used for filtering
            dtLookup.Columns.Add("text_box_name");
            //Column to store the database field name tied to the related container
            dtLookup.Columns.Add("field_name");

        }

        public void AddLookup(string TextBoxName, string FieldName)
        {
            dtLookup.Rows.Add(TextBoxName, FieldName);

        }

        public void FilterKeyPress(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            //Code to check each change in text lookup box and refilter data.
            dynamic tBox = (TextBox)sender;
            string FieldName = "";

            try
            {
                FieldName = dtLookup.Rows[FindDTRow(ref dtLookup, "text_box_name", tBox.Name.ToString())]["field_name"];
            }
            catch
            {
                Interaction.MsgBox(tBox.Name.ToString() + " textbox not properly configured for filtering.");
                return;
            }

            //Run the base object filter code
            uGrid.FilterGrid(FieldName, tBox.Text);

        }

        public int FindDTRow(ref DataTable DT, string ColName, string sValue)
        {
            //Function to find first row in datatable that matches the criteria of sValue in column ColName
            //Returns -1 if no match is found
            int i = 0;

            if (DT == null)
            {
                return -1;
            }
            for (i = 0; i <= DT.Rows.Count - 1; i++)
            {
                if (DT.Rows[i][ColName].ToString() == sValue)
                {
                    return i;
                }
            }

            return -1;

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
            //uBaseLookup.ClearLookup();

            //Define all lookup fields
            //These fields are textboxes at the top of the screen that will auto filter as the users
            //type into the fields
            //uBaseLookup.AddLookup("txtContractID", "contract_id");
            //uBaseLookup.AddLookup("txtContractDescription", "contract_description");
            //uBaseLookup.AddLookup("txtCustomerNumber", "customer_number");
            //uBaseLookup.AddLookup("txtCustomerName", "customer_name");
            //uBaseLookup.AddLookup("txtLocationID", "location_id");


            //Add the return parameters
            //uBaseLookup.ReturnParmFields.Add("contract_id");

            //Setup base grid information
            //The event to use when the grid is double clicked
            this.uGrid.WindowZoomDelegate = ReturnSelectedData;
            //Sets to row select and single select
            this.uGrid.SetGridSelectionBehavior(true, true);
            //uBaseLookup.uGrid.SelectProcedureIsQuery = true;
            //uBaseLookup.uGrid.AddParm("account_name", "%");

            //Set the grid display string to use
            this.uGrid.FieldLayoutResourceString = "TFInvoiceLookupGrid";
            //Set the grid main table name from the RObject detail table
            this.uGrid.MainTableName = "tf_invoice_lookup";
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            this.uGrid.xGrid.FieldLayoutSettings = f;
            ////Set the rows to change color based on account_status field
            //Style s = (System.Windows.Style)Application.Current.Resources["AccountStatusColorConverter"];

            this.ReturnParmFields.Add("invoice_number");



            //FieldLayoutSettings f = new FieldLayoutSettings();
            //f.DataRecordPresenterStyle = s;
            //uBaseLookup.uGrid.xGrid.FieldLayoutSettings = f;

            //uBaseLookup.uGrid.xGrid.Style = Application.Current.Resources("AccountStatusHighlight")

            //Remove any existing parameters
            CurrentBusObj.Parms.ClearParms();

            //Determine the cs_id parameter
            if (cGlobals.ReturnParms.Count > 0)
            {
                CurrentBusObj.Parms.AddParm("@tf_number", cGlobals.ReturnParms[0]);
            }
            else
            {
                CurrentBusObj.Parms.AddParm("@tf_number", ' ');
            }

            cGlobals.ReturnParms.Clear();

            //Retrieves the base business object
            Load(CurrentBusObj);

            //Load the grid data from the base object
            //?? Why is this not automatically happening with the base?  Look to see if this can be fixed not to run
            this.uGrid.LoadGrid(CurrentBusObj, this.uGrid.MainTableName);

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            //txtContractID.Focus();
            //uBaseLookup.uGrid.IsFilterable = true;

        }

        /// <summary>
        /// Runs when grid is doubleclicked and will normally be used to return parameters to the calling window
        /// </summary>
        public void ReturnSelectedData()
        {
            DataRecord record = default(DataRecord);

            if (uGrid.xGrid.SelectedItems.Records.Count > 1)
            {
                string invoice_number_return;
                invoice_number_return = "";

                foreach (DataRecord selectedrecord in uGrid.xGrid.SelectedItems.Records)
                {
                    //Loop through

                    if (selectedrecord != null)
                    {
                        DataRow row = (selectedrecord.DataItem as DataRowView).Row;
                        invoice_number_return = invoice_number_return + (Convert.ToString(row["invoice_number"])) + ";";

                    }
                }


                cGlobals.ReturnParms.Clear();
                cGlobals.ReturnParms.Add(invoice_number_return);



            }
            else if (uGrid.xGrid.SelectedItems.Records.Count == 1)
            {
                record = (DataRecord)(uGrid.xGrid.SelectedItems.Records[0]);
                if (record != null)
                {
                    cGlobals.ReturnParms.Clear();
                    cGlobals.ReturnParms.Add(record.Cells["invoice_number"].Value.ToString().Trim());
                }
            }
            else if (uGrid.xGrid.ActiveRecord != null)
            {
                record = (DataRecord)(uGrid.xGrid.ActiveRecord);
                if (record != null)
                {
                    cGlobals.ReturnParms.Clear();
                    cGlobals.ReturnParms.Add(record.Cells["invoice_number"].Value.ToString().Trim());
                }
            }
            CloseWindow();




        }

        private void bCancel_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {

            CloseWindow();

        }

        private void bOK_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            ReturnSelectedData();
        }


        private void CloseWindow()
        {
            Window w = Window.GetWindow(this);
            if (w != null)
            {
                w.Close();
            }
        }



        


    }
}
