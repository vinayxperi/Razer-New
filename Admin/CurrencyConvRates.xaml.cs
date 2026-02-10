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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RazerBase;
using RazerBase.Interfaces;
using System.Data;
using Infragistics.Documents.Excel;
using Infragistics.Windows.DataPresenter;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

//using RazerInterface;

namespace Admin
{
    /// Interaction logic for CurrencyConvRates

    public partial class CurrencyConvRates : ScreenBase, IScreen //, IPreBindable
    {        
        private static readonly string FromCurrency = "@from_currency";
        private static readonly string ToCurrency = "@to_currency";
        private static readonly string FromDate = "@date_from";
        private static readonly string ToDate = "@date_to";
        
        //Vinay's change
        Workbook dataWorkbook;
        bool AreColumnsVerified;
        string ErrorMessage="";
        //List of all columns that can be on the spreadsheet
        List<string> PropertyList = new List<string>(new string[] { "base_currency", "source_currency", "exchange_rate", "effective_date", "rate_provider", "method"});

        List<string> displayColumnList = new List<string>(new string[] { "base_currency", "source_currency", "effective_date","exchange_rate" });

        Dictionary<string, string> columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "base_currency", "to_currency" },
            { "source_currency", "from_currency" },
            { "effective_date", "conversion_date" },
            { "exchange_rate", "conversion_rate" }
        };
        
        
        //Create a new DataTable with fixed column names
        DataTable loadTable;

        //Vinay's change 
        public string WindowCaption { get { return string.Empty; } }
        
        public CurrencyConvRates()
            : base()
        {
            InitializeComponent();
        }


        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;

            cBaseBusObject CurCodesBusObject = new cBaseBusObject("CurrencyCodes");
            CurCodesBusObject.LoadData();

            DataTable source = CurCodesBusObject.ObjectData.Tables["currencycodes"] as DataTable;
            cmbFromCurrency.SetBindingExpression("currency_code", "description", source);
            cmbToCurrency.SetBindingExpression("currency_code", "description", source);

        }

        //public void PreBind()
        //{
        //    try
        //    {
        //        // if the object data was loaded
        //        if (this.CurrentBusObj.HasObjectData)
        //        {
        //            this.cmbFromCurrency.SetBindingExpression("currency_code", "description", this.CurrentBusObj.ObjectData.Tables["fromcurrency"]);
        //            this.cmbToCurrency.SetBindingExpression("currency_code", "description", this.CurrentBusObj.ObjectData.Tables["tocurrency"]);                    
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        // for debugging only
        //        string err = error.ToString();
        //    }
        //}

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            CurrentBusObj = new cBaseBusObject("CurrencyConvSearch");
            CurrentBusObj.Parms.AddParm(FromCurrency, cmbFromCurrency.SelectedValue);
            CurrentBusObj.Parms.AddParm(ToCurrency, cmbToCurrency.SelectedValue);

            if (ldpRateFrom.SelText != null)
                CurrentBusObj.Parms.AddParm(FromDate, ldpRateFrom.SelText);
            else
                CurrentBusObj.Parms.AddParm(FromDate, new DateTime(1900, 1, 1));

            if ((ldpRateTo.SelText != null) && (ldpRateTo.SelText.ToString() != "1/1/1900 12:00:00 AM"))
                CurrentBusObj.Parms.AddParm(ToDate, ldpRateTo.SelText);
            else
                CurrentBusObj.Parms.AddParm(ToDate, new DateTime(2100, 1, 1));

            this.MainTableName = "cur_conv_search";
            idgCurrencyConvSearch.FieldLayoutResourceString = "CurrencyConvSearch";
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            idgCurrencyConvSearch.xGrid.FieldLayoutSettings = layouts;
            layouts.HighlightAlternateRecords = true;
            idgCurrencyConvSearch.xGrid.FieldSettings.AllowEdit = false;
            idgCurrencyConvSearch.SetGridSelectionBehavior(true, false);

            //System.Windows.Forms.MessageBox.Show(CurrentBusObj.ObjectData.ToString());
            this.Load(CurrentBusObj);

            if (CurrentBusObj.HasObjectData)
                idgCurrencyConvSearch.LoadGrid(CurrentBusObj, this.MainTableName);
                       
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbFromCurrency.SelectedValue = null;
            cmbToCurrency.SelectedValue = null;
            ldpRateFrom.SelText = null;
            ldpRateTo.SelText = null;
            if (CurrentBusObj.BusObjectName == "CurrencyConvSearch")
            {
                this.CurrentBusObj.Parms.ClearParms();
                CurrentBusObj.Parms.AddParm(FromCurrency, "XXX");
                CurrentBusObj.Parms.AddParm(ToCurrency, "XXX");
                CurrentBusObj.Parms.AddParm(FromDate, "1/1/1900 12:00:00 AM");
                CurrentBusObj.Parms.AddParm(ToDate, "1/1/1900 12:00:00 AM");
                if (CurrentBusObj.HasObjectData)
                {
                    this.Load(CurrentBusObj);
                    idgCurrencyConvSearch.LoadGrid(CurrentBusObj, this.MainTableName);
                }
            }
            else
            {
                this.CurrentBusObj.ObjectData.Tables["cur_conv_rate"].Clear();
                idgCurrencyConvSearch.LoadGrid(CurrentBusObj, "cur_conv_rate");
                btnUpload.IsEnabled = false;
                            
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            idgCurrencyConvSearch.xGrid.DataSource = null;

            dataWorkbook = null;
            //Create a Dictionary object for parsing the spreadsheet data
            Dictionary<string, int> ColumnDictionary = new Dictionary<string, int>();

            loadTable = new DataTable();
            loadTable.TableName = "loadTable";
            // from_currency: char(3)
            DataColumn fromCurrencyCol = new DataColumn("from_currency", typeof(string));
            fromCurrencyCol.MaxLength = 3;
            loadTable.Columns.Add(fromCurrencyCol);
            // to_currency: char(3)
            DataColumn toCurrencyCol = new DataColumn("to_currency", typeof(string));
            toCurrencyCol.MaxLength = 3;
            loadTable.Columns.Add(toCurrencyCol);
            // conversion_date: DateTime
            loadTable.Columns.Add("conversion_date", typeof(DateTime));
            loadTable.Columns.Add("conversion_rate", typeof(decimal));
            
            //Create file dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx";

            //Open the Pop-Up Window to select the file 
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileInfo = new FileInfo(dlg.FileName);
                //var IsExcelParsed = true;
                try
                {
                    using (Stream stream = dlg.OpenFile())
                    {
                        dataWorkbook = Workbook.Load(stream);
                    }
                }
                catch { /* do nothing */ }


                if (dataWorkbook == null)
                {
                    btnUpload.IsEnabled = false;
                    Messages.ShowMessage("The application was unable to load the excel document.  Check to see if it is open. Please try again or contact Razer support", System.Windows.MessageBoxImage.Exclamation);
                }
                else
                {
                    string message = "";
                    //Verify column header names and reformat them in the dataworkbook to include "_" and convert to lowercase.
                    VerifyColumnsInWorksheet(dataWorkbook,ref ColumnDictionary, ref message, ref AreColumnsVerified);
                    
                    if (AreColumnsVerified)
                    {
                        Worksheet sheetOne = dataWorkbook.Worksheets[0];
                        foreach (WorksheetRow row in sheetOne.Rows)
                        {
                            if (row.Index != 0) //the first row contains headers
                            {
                                if (!ParseAndAddRow(ColumnDictionary, row))
                                {
                                    Messages.ShowMessage(String.Format("Unable to parse excel document.  Reason: {0}", ErrorMessage), System.Windows.MessageBoxImage.Exclamation);
                                    btnUpload.IsEnabled = false; 
                                    break;
                                }
                            }
                        }
                        //If control comes here, data has been added to loadTable successfully.
                        //Load currency table to filter the data.
                        
                        cBaseBusObject CurrencyBusObject = new cBaseBusObject("currency");
                        CurrencyBusObject.LoadData();
                        
                        DataTable currencyTable = CurrencyBusObject.ObjectData.Tables["currency"] as DataTable;

                        if (currencyTable != null && currencyTable.Rows.Count < 1 )
                        {
                            Messages.ShowInformation("Currency table could not be loaded");
                            btnUpload.IsEnabled = false;
                            return;
                        }

                        // Create a HashSet for fast lookup of currency codes
                        var currencyCodes = new HashSet<string>(
                            currencyTable.AsEnumerable()
                                         .Select(row => row.Field<string>("currency_code")),
                            StringComparer.OrdinalIgnoreCase);

                        var filteredRows = loadTable.AsEnumerable()
                            .Where(row =>
                                ((row.Field<string>("from_currency") == "USD" &&
                                 currencyCodes.Contains(row.Field<string>("to_currency"))) ||
                                (currencyCodes.Contains(row.Field<string>("from_currency")) &&
                                 row.Field<string>("to_currency") == "USD")) && 
                                 (row.Field<string>("from_currency") != "USD" || row.Field<string>("to_currency") != "USD"))
                                 .CopyToDataTable();
                        
                        loadTable = filteredRows as DataTable;

                        //Get the Last day of the previous month and validate the same.
                        DateTime today = DateTime.Today;
                        DateTime endOfLastMonth = new DateTime(today.Year, today.Month, 1).AddDays(-1);

                        bool hasInvalidDate = loadTable.AsEnumerable()
                            .Any(row => row.Field<DateTime>("conversion_date").Date != endOfLastMonth);

                        //Table has filtered data!! Now display it in the grid. 
                        if (hasInvalidDate)
                        {
                            Messages.ShowInformation("Some currency exchange rates do not have conversion_date of " + endOfLastMonth.Date + "\n Check the spreadsheet and Load again");
                            loadTable.Clear();
                            btnUpload.IsEnabled = false;
                            return;
                        }
                        else
                        {
                            //The filtered rows have valid information. Display it on the grid.
                            CurrentBusObj = new cBaseBusObject("CurrencyFxHistory");

                            this.MainTableName = "cur_conv_rate";
                            idgCurrencyConvSearch.FieldLayoutResourceString = "CurrencyConvSearch";
                            FieldLayoutSettings layouts = new FieldLayoutSettings();
                            idgCurrencyConvSearch.xGrid.FieldLayoutSettings = layouts;
                            layouts.HighlightAlternateRecords = true;
                            idgCurrencyConvSearch.xGrid.FieldSettings.AllowEdit = false;
                            idgCurrencyConvSearch.SetGridSelectionBehavior(true, false);

                            this.Load(CurrentBusObj);
                            //Compare the dates and show a pop up!!

                            DateTime latestConversionDate = (DateTime)this.CurrentBusObj.ObjectData.Tables["cur_conv_rate"].Rows[0]["conversion_date"];
                            
                            if (latestConversionDate.AddDays(-1) == endOfLastMonth)
                            {
                                Messages.ShowInformation("The currency conversion rates for " + endOfLastMonth.ToString("MMM").ToUpper() + " have already been uploaded");
                                btnUpload.IsEnabled = false;
                                return;
                            }
                            this.CurrentBusObj.ObjectData.Tables["cur_conv_rate"].Clear();
                            foreach (DataRow dr in loadTable.AsEnumerable())
                            {
                                DataRow newRow = this.CurrentBusObj.ObjectData.Tables["cur_conv_rate"].NewRow();
                                newRow.ItemArray = dr.ItemArray;
                                this.CurrentBusObj.ObjectData.Tables["cur_conv_rate"].Rows.Add(newRow);

                            }
                            
                            idgCurrencyConvSearch.LoadGrid(CurrentBusObj, "cur_conv_rate");
                            //idgCurrencyConvSearch.xGrid.DataSource = loadTable.AsEnumerable();
                            btnUpload.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Messages.ShowMessage(string.Format("Invalid Columns were detected.  Please check the excel document headers and try again. {0}", message).Trim(), System.Windows.MessageBoxImage.Exclamation);
                    }

                }
            }
        }

        //Verify column header names and reformat them in the dataworkbook to include "_" and convert to lowercase.
        private void VerifyColumnsInWorksheet(Workbook dataWorkbook, ref Dictionary<string, int> ColumnDictionary, ref string message, ref bool AreColumnsVerified)
        {
            int colIndex = 0;
            Worksheet sheetOne = dataWorkbook.Worksheets[0];
            WorksheetRow headerRow = sheetOne.Rows[0];
            foreach (WorksheetCell cell in headerRow.Cells)
            {
                if (cell.Value != null)
                {
                    string originalHeader = Convert.ToString(cell.Value);
                    // Clean the header: trim, replace whitespace with "_", and convert to lowercase
                    string cleanedHeader = Regex.Replace(originalHeader.Trim(), @"\s+", "_").ToLower();
                    cell.Value = cleanedHeader;
                }
                else
                {
                    Messages.ShowError("Error -- Column number: " + (colIndex + 1).ToString() + " is not loading properly.  If the column is hidden then unhide it. " +
                                        "Otherwise, try resizing the column and then load again.");
                    AreColumnsVerified = false;
                    return;
                }

                if (colIndex != cell.ColumnIndex)
                {
                    Messages.ShowError("Error -- Column number: " + (colIndex + 1).ToString() + " is not loading properly.  If the column is hidden then unhide it. Otherwise, try resizing the column and then load again.");
                    AreColumnsVerified = false;
                    return;
                }
                else
                    colIndex++;

                if (!PropertyList.Contains(Convert.ToString(cell.Value)))
                {
                    message = string.Format("Check column '{0}' ", cell.Value);
                    AreColumnsVerified = false; 
                    return;
                }
                
                if(displayColumnList.Contains(Convert.ToString(cell.Value)))
                    ColumnDictionary.Add(Convert.ToString(cell.Value), cell.ColumnIndex);
            }
            //Last column.
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

        private bool ParseAndAddRow(Dictionary<string, int> ColumnDictionary,WorksheetRow row)
        {
            int colIndex = 0;
            DataRow newRow = loadTable.NewRow();
            
            foreach (var mapping in columnMapping)
            {
                string spreadsheetColumnName = mapping.Key;
                string dataTableColumnName = mapping.Value;
            
                // Check if the spreadsheet column exists in the dictionary
                if (ColumnDictionary.TryGetValue(spreadsheetColumnName, out colIndex))
                {   
                    WorksheetCell cell = row.Cells[colIndex];

                    // Check if the cell is valid and has a value
                    if (cell != null && cell.Value != null)
                    {
                        if (dataTableColumnName == "conversion_date" && cell.Value is double)
                        {
                            DateTime conversion_date = DateTime.Parse("1-1-1900").AddDays(Convert.ToDouble(cell.Value) - 2);
                            newRow[dataTableColumnName] = conversion_date;
                        }
                        else
                            newRow[dataTableColumnName] = cell.Value;
                    }
                    else
                    {
                        return false;
                    }
                }
                else { }
                    //Do nothing. Additional columns which are not needed. 
            }
            
            loadTable.Rows.Add(newRow);
            return true;
        }
       
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
             this.Save();
            
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Saved Successfully");
            }
            else
                Messages.ShowInformation("Save Failed");

            btnUpload.IsEnabled = false;

        }
    }
}
       
       

       

       



