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
    /// Interaction logic for AdjustmentUpload.xaml
    /// </summary>
    public partial class AdjustmentUpload : ScreenBase, IScreen, IPreBindable
    {
        //Property is required for base objects that use IScreen
        public string WindowCaption { get; private set; }
        //List of all of the workbook columns that can be in the spreadsheet to be uploaded
        //These are not the required columns
        List<string> PropertyList = new List<string>(new string[] { "inv_rule_id","cs_id","psa_city","psa_state","name","product_code","item_description","rule_type",
                                                                    "service_period_start","service_period_end","bill_unit_filter","bill_units_current","extended",
                                                                    "net_extended","informational_flag","description","amount_change","invoice_number","prev_adj_amt"});
        bool AreColumnsVerified;
        string ErrorMessage = "";

        private string User = UserName.GetUserName;

        //Variables used to track the current upload / invoice information at a global level
        string CurrentInvoice = ""; //Stores the current invoice number being used
        decimal totalAmount=0;
        int adjustmentType = 0;
        string adjustmentDocumentID = "";
        UploadStep CurrentStep = UploadStep.EmptyScreen ;

        DataTable dtUpload;
        private enum UploadStep { EmptyScreen = 0,RetrieveInvoice = 1, LoadFromSpreadsheet = 2, RetrieveHeader=3, UploadAdjustment = 4 };
        
        public AdjustmentUpload()
            : base()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            WindowCaption = "Adjustment Upload";

            //Setting intial screenstate to empty
            CurrentState = ScreenState.Empty;

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            //Set the maintablename for the folder if it has one
            this.MainTableName = "adjustment_upload";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            //Add any Grid Configuration Information
            //gUpload.MainTableName = "adjustment_upload"; //Should match the ROBJECT table name
            //gUpload.ConfigFileName = "AdjustmentUploadGridConfig"; //This is the file name that will store any user customizations to the grid - must be unique in the app
            gUpload.SetGridSelectionBehavior(true, false); //Sets standard grid behavior for record select and multiselect
            //gUpload.FieldLayoutResourceString = "AdjustmentUploadGrid"; //The name of the FieldLayout in the Field Layouts xaml file - Must be unique
            //Add this code to use a different style for the grid than the default.
            //This particular style is used for color coded grids.
            //Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            //gUpload.GridCellValuePresenterStyle = CellStyle;
            //gUpload.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;



            //Add all grids to the grid collection - This allows grids to automatically load and participate with security
            GridCollection.Add(gUpload);

            //Set dummy parm values for initial load
            RetrieveData("", UploadStep.EmptyScreen );
            //RetrieveData();
            SetWindowStatus();

             //Check Security for the window and hide the load button as appropriate.

            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                btnLoad.IsEnabled = false;
                btnUpload.IsEnabled = false;
            }
            else
                btnLoad.IsEnabled = true; 

        }

        /// <summary>
        /// Method to enable / disable the upload button depending on where you are in the process
        /// </summary>
        private void SetWindowStatus()
        {
            if (CurrentState == ScreenState.Empty || CurrentState==ScreenState.Locked || CurrentState==ScreenState.Normal )
            {
                //btnVerify.IsEnabled = false;
                btnUpload.IsEnabled = false;
            }
            if (CurrentState == ScreenState.Inserting )
            {
                //btnVerify.IsEnabled = false;
                btnUpload.IsEnabled = true;
            }

            return;
        }


/// <summary>
/// Method handles what data objects to retrieve based upon what step in the upload process is being performed.
/// </summary>
/// <param name="invoiceNumber"></param>
/// <param name="Step"></param>
        private void RetrieveData(string invoiceNumber, UploadStep Step)
        {
            CurrentStep = Step;

            switch (Step)
            {
                case UploadStep.EmptyScreen:
                    CurrentBusObj.Parms.ClearParms();
                    CurrentBusObj.Parms.AddParm("@invoice_number", "");
                    CurrentBusObj.Parms.AddParm("@document_id", "");
                    CurrentBusObj.Parms.AddParm("@doc_to_adj_id", "");
                    CurrentBusObj.Parms.AddParm("@adjustment_type_id", 0);
                    CurrentBusObj.Parms.AddParm("@user_id", "");
                    CurrentBusObj.Parms.AddParm("@amount", 0);
                    CurrentState = ScreenState.Empty;
                    adjustmentDocumentID = "";
                    adjustmentType = 0;
                    totalAmount = 0;
                    CurrentInvoice = "";
                    gUpload.MainTableName = "adjustment_upload";
                    this.Load();
                    SetWindowStatus();
                    break;

                case UploadStep.RetrieveInvoice:
                    CurrentBusObj.Parms.ClearParms();
                    CurrentBusObj.Parms.AddParm("@invoice_number", invoiceNumber);
                    CurrentBusObj.Parms.AddParm("@document_id", invoiceNumber);
                    CurrentBusObj.Parms.AddParm("@doc_to_adj_id", "");
                    CurrentBusObj.Parms.AddParm("@adjustment_type_id", 0);
                    CurrentBusObj.Parms.AddParm("@user_id", "");
                    CurrentBusObj.Parms.AddParm("@amount", 0);
                    gUpload.MainTableName = "adjustment_upload";
                    CurrentState = ScreenState.Normal;
                    this.Load();
                    SetWindowStatus();
                    break;
                case UploadStep.LoadFromSpreadsheet:
                    CurrentBusObj.Parms.ClearParms();
                    CurrentBusObj.Parms.AddParm("@invoice_number", "");
                    CurrentBusObj.Parms.AddParm("@document_id", invoiceNumber);
                    CurrentBusObj.Parms.AddParm("@doc_to_adj_id", "");
                    CurrentBusObj.Parms.AddParm("@adjustment_type_id", 0);
                    CurrentBusObj.Parms.AddParm("@user_id", "");
                    CurrentBusObj.Parms.AddParm("@amount", 0);
                    gUpload.MainTableName = "detail_acct";
                    this.Load();
                    CurrentState = ScreenState.Inserting;
                    SetWindowStatus();
                    break;

                case UploadStep.RetrieveHeader :
                    CurrentBusObj.Parms.ClearParms();
                    CurrentBusObj.Parms.AddParm("@invoice_number", "");
                    CurrentBusObj.Parms.AddParm("@document_id", invoiceNumber);
                    CurrentBusObj.Parms.AddParm("@doc_to_adj_id", invoiceNumber);
                    CurrentBusObj.Parms.AddParm("@adjustment_type_id", adjustmentType );
                    CurrentBusObj.Parms.AddParm("@user_id", cGlobals.UserName.ToLower());
                    CurrentBusObj.Parms.AddParm("@amount", totalAmount );
                    gUpload.MainTableName = "detail_acct";
                    CurrentBusObj.LoadTable("header");
                    CurrentState = ScreenState.Locked;
                    break;

               case UploadStep.UploadAdjustment :
                    CurrentBusObj.Parms.ClearParms();
                    CurrentBusObj.Parms.AddParm("@invoice_number", "");
                    CurrentBusObj.Parms.AddParm("@document_id", invoiceNumber);
                    CurrentBusObj.Parms.AddParm("@doc_to_adj_id", invoiceNumber);
                    CurrentBusObj.Parms.AddParm("@adjustment_type_id", adjustmentType  );
                    CurrentBusObj.Parms.AddParm("@user_id", cGlobals.UserName.ToLower());
                    CurrentBusObj.Parms.AddParm("@amount", 0 );
                    gUpload.MainTableName = "detail_acct";
                    this.Save();
                    if (SaveSuccessful)
                    {
                        Messages.ShowInformation("Saved adjustment ID: " + adjustmentDocumentID);
                        CurrentState = ScreenState.Empty;
                        SetWindowStatus();
                    }
                    else
                    {
                        //cGlobals.BillService.DeleteNewAdjusmentPreamble(adjustmentDocumentID);
                        Messages.ShowError("Error Saving Adjustment ");
                     }

                    break;

            }


        }


        public void PreBind()
        {
            return;
        }

        /// <summary>
        /// Method to load spreadsheet data into datatable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            ////Click event is firing on enter key.  This stops that from opening the dialog
            //if (CurrentStep == UploadStep.RetrieveInvoice)
            //{
            //    CurrentStep = UploadStep.EmptyScreen;
            //    return;
            //}
            //Create the base workbook object
            Workbook dataWorkbook = null;

            //Create a Dictionary object for parsing the spreadsheet data
            Dictionary<string, int> ColumnDictionary = new Dictionary<string, int>();

            //Create file dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx";

            //Open the Pop-Up Window to select the file 
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(dlg.FileName);
                bool IsExcelParsed = true;
                try
                {
                    using (Stream stream = dlg.OpenFile())
                    {
                        dataWorkbook = Workbook.Load(stream);
                    }
                }
                catch { /* do nothing */ }

                //verify workbook file is not locked
                if (dataWorkbook == null)
                {
                    Messages.ShowMessage("The application was unable to load the excel document.  " +
                        "Check to see if it is open. Please try again or contact Razer support", System.Windows.MessageBoxImage.Exclamation);
                }
                else
                {
                    string message = "";
                    VerifyColumnsInExcel(dataWorkbook, ref AreColumnsVerified, ref ColumnDictionary, ref message);

                    if (AreColumnsVerified)
                    {
                        Worksheet sheetOne = dataWorkbook.Worksheets[0];
                        CreateUploadTable();
                        CurrentInvoice = "";
                        foreach (WorksheetRow row in sheetOne.Rows)
                        {
                            if (row.Index != 0) //the first row contains headers
                            {
                                if (!ParseWorksheet(ColumnDictionary, row))
                                {
                                    IsExcelParsed = false;
                                    Messages.ShowMessage(String.Format("Unable to parse excel document.  Reason: {0}", ErrorMessage), System.Windows.MessageBoxImage.Exclamation);
                                    break;
                                }

                            }
                        }

                        if (IsExcelParsed)
                        {


                            RetrieveData(CurrentInvoice, UploadStep.LoadFromSpreadsheet);

                            //Check for previous adjustments for current document being adjusted
                            if (CurrentBusObj.ObjectData.Tables["prev_adj"] != null && CurrentBusObj.ObjectData.Tables["prev_adj"].Rows.Count > 0)
                            {
                                string sMessage = "Unable to process.  Current unposted adjustments exist for this document.  Unposted adjustment document ids: ";
                                foreach (DataRow r in CurrentBusObj.ObjectData.Tables["prev_adj"].Rows)
                                {
                                    sMessage += r["document_id"].ToString() + "  ";
                                }
                                Messages.ShowError(sMessage);
                                RetrieveData("", UploadStep.EmptyScreen);
                                return;
                            }
                                                            
                            //Load detail adjustment grids
                            tInvoice.Text = CurrentInvoice;

                            bool UploadHasDollars = false;
                            //Populate line values
                            foreach (DataRow r in dtUpload.Rows)
                            {
                                if (Convert.ToDecimal(r["amount_change"]) != 0)
                                {
                                    UploadHasDollars = true;
                                    EnumerableRowCollection DetailListRows = from row in this.CurrentBusObj.ObjectData.Tables["detail_acct"].AsEnumerable()
                                                                             where row.Field<decimal>("acct_detail_id") == Convert.ToDecimal(r["inv_rule_id"].ToString())
                                                                             select row;

                                    foreach (DataRow r1 in DetailListRows)
                                    {
                                        if (Convert.ToInt32(r1["normalized_flag"]) == 1)
                                        {
                                            Messages.ShowError("Cannot adjust lines that are normalized.");
                                            RetrieveData("", UploadStep.EmptyScreen);
                                            return;
                                        }
                                        r1["amount_adjusted"] = r["amount_change"];
                                    }
                                }
                                
                            }
                            if (!UploadHasDollars)
                            {
                                Messages.ShowError("Upload has no amount_change values.");
                                RetrieveData("", UploadStep.EmptyScreen);
                                return;
                            }

                        }
                        else
                        {
                            Messages.ShowMessage(string.Format("Invalid Columns were detected.  Please check the excel document headers and try again. {0}", message).Trim(), System.Windows.MessageBoxImage.Exclamation);
                        }
                    }
                    else
                    {
                        Messages.ShowMessage(string.Format("Invalid Columns were detected.  Please check the excel document headers and try again. {0}", message).Trim(), System.Windows.MessageBoxImage.Exclamation);
                    }
                }

            }
        }

  


        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentBusObj.ObjectData.Tables["detail_acct"] != null)
            {
                //Get Total amount 
                totalAmount=0;
                foreach(DataRow r in CurrentBusObj.ObjectData.Tables["detail_acct"].Rows)
                {
                    totalAmount += Convert.ToDecimal(r["amount_adjusted"]);
                }
                //Set adjustment type 1 = credit, 2 = debit
                if (totalAmount>0)
                    adjustmentType=2;
                else
                    adjustmentType=1;

                //Retrieve the header table to insert the adjustment header and to get the adjustment ID
                RetrieveData(CurrentInvoice,UploadStep.RetrieveHeader );

                if (CurrentBusObj.ObjectData.Tables["header"] != null && CurrentBusObj.ObjectData.Tables["header"].Rows.Count>0)
                    adjustmentDocumentID=CurrentBusObj.ObjectData.Tables["header"].Rows[0]["adj_document_id"].ToString();
                else
                {
                    Messages.ShowError("Error getting adjustment document header.");
                    return;
                }

                //Finish prep work on detail and acct tables
                PopulateDetailAdjValues();

                //Save adjustment
                RetrieveData(CurrentInvoice, UploadStep.UploadAdjustment);

                //Clear Screen
                RetrieveData("", UploadStep.EmptyScreen);

  
            }

        }

        /// <summary>
        /// Method to populate accounting and detail tables with all necessary information
        /// </summary>
        private void PopulateDetailAdjValues()
        {
            decimal seqTotal = 0;
            bool HasRows = false;
            foreach (DataRow r in CurrentBusObj.ObjectData.Tables["detail"].Rows)
            {


                seqTotal = 0;
                //Get all records from acct table that match the product code from the detail table
                EnumerableRowCollection DetailListRows = from row in this.CurrentBusObj.ObjectData.Tables["detail_acct"].AsEnumerable()
                                                         where row.Field<string>("product_code") == r["product_code"].ToString()
                                                         && row.Field<decimal>("amount_adjusted") != 0
                                                         select row;

                HasRows = false;

                foreach (DataRow r1 in DetailListRows)
                {
                    HasRows = true;
                    //Create running total of amount for current seq_code
                    seqTotal += Convert.ToDecimal(r1["amount_adjusted"]);
                    //Update needed values to acct detail while in this loop
                    r1["adj_document_id"] = adjustmentDocumentID;
                    r1["apply_to_seq"] = Convert.ToInt32(r["seq_code"]);
                    r1["adjustment_type"] = adjustmentType;
                    r1["receivable_account"] = r["receivable_account"].ToString();
                    r1["currency_code"] = r["currency_code"].ToString();
                    r1["company_code"] = r["company_code"].ToString();
                    r1["gl_center"] = r["gl_center"].ToString();
                    r1["gl_acct"] = r["gl_acct"].ToString();
                    r1["geography"] = r["geography"].ToString();
                    r1["interdivision"] = r["interdivision"].ToString();
                    r1["gl_product"] = r["gl_product"].ToString();
                }

                if (HasRows) //Only update the detail table if that particular product code has data being updated
                {
                    //Update total and other needed values for this loop.
                    r["amount_adjusted"] = seqTotal;
                    r["apply_to_doc"] = CurrentInvoice;
                    r["adj_document_id"] = adjustmentDocumentID;
                    r["apply_to_seq"] = r["seq_code"];
                }
            }    
            

        }

        /// <summary>
        /// Method to make sure that all column headers in spreadsheet match expected headers
        /// </summary>
        /// <param name="dataWorkbook"></param>
        /// <param name="AreColumnsVerified"></param>
        /// <param name="ColumnDictionary"></param>
        /// <param name="message"></param>
        private void VerifyColumnsInExcel(Workbook dataWorkbook, ref bool AreColumnsVerified, ref Dictionary<string, int> ColumnDictionary, ref string message)
        {
            int colIndex = 0;
            Worksheet sheetOne = dataWorkbook.Worksheets[0];
            foreach (WorksheetColumn col in sheetOne.Columns)
            {
                //Added to throw error if more data exists than was loaded in through the previous step.
                //For some unknown reason, the column collection will sometimes drop a column.  This code catches those columns dropped from
                //the middle of the column collection.
                if (colIndex != col.Index)
                {
                    Messages.ShowError("Error -- Column number: " + (colIndex + 1).ToString() + " is not loading properly.  If the column is hidden then unhide it. Otherwise, try resizing the column and then load again.");
                    AreColumnsVerified = false;
                    return;
                }
                else
                    colIndex++;

                if (sheetOne.Rows[0].Cells[col.Index].Value == null) { break; }

                //string worksheetCellValue = FixUnitUploadColumnNames.UnFixColumnNames(sheetOne.Rows[0].Cells[col.Index].Value.ToString()).ToLower();
                string worksheetCellValue = sheetOne.Rows[0].Cells[col.Index].Value.ToString().ToLower();

                if (string.IsNullOrEmpty(worksheetCellValue)) { break; }

                if (!PropertyList.Contains(worksheetCellValue)) { message = string.Format("Check column '{0}' ", worksheetCellValue); return; }

                ColumnDictionary.Add(worksheetCellValue, col.Index);
            }

            //DWR - 3/8/12 - Added to throw error if more data exists than was loaded in through the previous step.
            //For some unknown reason, the column collection will sometimes drop a column.  This code catches those columns dropped from
            //the end.
            if (sheetOne.Rows[0].Cells[colIndex].Value != null)
            {
                Messages.ShowError("Error -- Column number: " + (colIndex + 1).ToString() + " is not loading properly.  If the column is hidden then unhide it. " +
                                    "Otherwise, try resizing the column and then load again.");
                AreColumnsVerified = false;
                return;
            }


            message = "";
            AreColumnsVerified = true;
        }

        /// <summary>
        /// Generic method to return infragistics grid cell values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private T GetCellValue<T>(object value, object defaultValue)
        {
            if (value == null)
            {
                return (T)(Convert.ChangeType(defaultValue, typeof(T)));
            }
            else
            {
                return (T)(Convert.ChangeType(value, typeof(T)));
            }
        }

        private DateTime ConvertExcelDateFormatToDateTime(object dateVal)
        {
            //Excel store dates as doubles
            //the left hand side is the number of days 1 is equivalent to 1/1/1900
            //the right hand side is the time stored as decimal numbers between .0 and .99999, where .0 is 00:00:00 and .99999 is 23:59:59.
            return DateTime.Parse("1-1-1900").AddDays(Convert.ToDouble(dateVal) - 2);
        }

        private bool ParseWorksheet(Dictionary<string, int> ColumnDictionary, WorksheetRow row)
        {
            var TF = true;
            string CurrentColumn = "";

            DataRow r = dtUpload.NewRow();

            try
            {

                foreach (var vkp in ColumnDictionary)
                {
                    if (!TF) { break; } //check for error
                    CurrentColumn = vkp.Key;
                    switch (vkp.Key)
                    {
                        case "inv_rule_id":
                            r["inv_rule_id"] = GetCellValue<int>(row.Cells[vkp.Value].Value, 0);
                            break;
                        case "amount_change":
                            r["amount_change"] = GetCellValue<decimal>(row.Cells[vkp.Value].Value, 0);
                            break;
                        case "invoice_number":
                            r["invoice_number"] = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            
                            if (CurrentInvoice == "") //Establish the current invoice number the first time through
                                CurrentInvoice = r["invoice_number"].ToString();
                            else
                            {
                                //On future passes make sure the invoice number hasn't changed
                                //If it has signify an error and stop the upload.
                                if (CurrentInvoice != r["invoice_number"].ToString())
                                {
                                    TF = false;
                                    ErrorMessage = "Multiple invoice numbers uploaded for adjustment. Only one invoice adjustment can be upload at a time.";
                                }
                            }
                            break;

                        default:
                            //TF = false;
                            //ErrorMessage = string.Format("Unable to match column to {0}", vkp.Key);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                TF = false;
                ErrorMessage = string.Format("Unable to convert a column, {1}, to the proper type: {0}",
                    ex.Message,
                    (string.IsNullOrEmpty(CurrentColumn) ? "unknown" : CurrentColumn));
            }
            ////Set the can upload value to false on all records on load
            //r["can_upload"] = 0;
            //r["user_id"] = User;
            //r["temp_status"] = RandomStatus;
            if (TF) { dtUpload.Rows.Add(r); }
            return TF;
        }

        private void tInvoice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tInvoice.Text != "" && tInvoice.Text != CurrentInvoice)
            {
                RetrieveData("", UploadStep.EmptyScreen);
                RetrieveData(tInvoice.Text, UploadStep.RetrieveInvoice );
                PopulatePrevAdjAmount();
                //If invoice exists then retrieve the appropriate invoice data to the screen for download.
                if (CurrentBusObj.ObjectData.Tables["adjustment_upload"] != null && CurrentBusObj.ObjectData.Tables["adjustment_upload"].Rows.Count > 0)
                {
                    CurrentInvoice = tInvoice.Text;
                    SetWindowStatus();
                }
                else //Invoice doesn't exist - Give error message and reset screen
                {
                    Messages.ShowError("Invoice does not exist.  Please enter a valid invoice number.");
                    CurrentInvoice = "";
                    tInvoice.Text = "";
                    RetrieveData("", UploadStep.EmptyScreen);
                }


                //e.Handled=true;
            }
        }


        /// <summary>
        /// Method to populate the previously adjusted amounts on each detail line item so that the download file will include the information.
        /// </summary>
        private void PopulatePrevAdjAmount()
        {
            if (CurrentBusObj.ObjectData.Tables["adjustment_upload"] != null && CurrentBusObj.ObjectData.Tables["detail_acct"] != null)
            {
                //Populate line values
                foreach (DataRow r in CurrentBusObj.ObjectData.Tables["adjustment_upload"].Rows)
                {

                    EnumerableRowCollection DetailListRows = from row in this.CurrentBusObj.ObjectData.Tables["detail_acct"].AsEnumerable()
                                                             where row.Field<decimal>("acct_detail_id") == Convert.ToDecimal(r["inv_rule_id"].ToString())
                                                             select row;

                    foreach (DataRow r1 in DetailListRows)
                    {
                        r["prev_adj_amt"] = Convert.ToDecimal(r["prev_adj_amt"]) + Convert.ToDecimal(r1["amount_change"]);
                    }

                }

                
            }
        }

        /// <summary>
        /// Method used to clear and rebuild temporary table for storing adjustment uploads.
        /// </summary>
        private void CreateUploadTable()
        {
            dtUpload = new DataTable("upload_adjustment");
            dtUpload.Columns.Add("invoice_number");
            dtUpload.Columns.Add("inv_rule_id");
            dtUpload.Columns.Add("amount_change");
        }

        private void btnDummy_Click(object sender, RoutedEventArgs e)
        {
            return;
        }

    }
}