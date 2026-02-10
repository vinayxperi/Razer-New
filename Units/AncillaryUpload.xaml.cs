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
    /// Interaction logic for AncillaryUpload.xaml
    /// </summary>
    public partial class AncillaryUpload : ScreenBase, IScreen, IPreBindable
    {
        //Property is required for base objects that use IScreen
        public string WindowCaption { get; private set; }
        List<string> PropertyList = new List<string>(new string[] { "mca_address", "anc_code", "service_period", "start_date", "end_date", "comment", "service_id"});
        bool AreColumnsVerified;
        string ErrorMessage="";
        Random random = new Random();
        int RandomStatus;
        private string User = UserName.GetUserName;

        public AncillaryUpload()
            : base()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            WindowCaption = "Ancillary Unit Upload";

            //Setting intial screenstate to empty
            CurrentState = ScreenState.Empty;

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            //Set the maintablename for the folder if it has one
            this.MainTableName = "ancillary_upload";

            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;

            //Add any Grid Configuration Information
            gUpload.MainTableName = "ancillary_upload"; //Should match the ROBJECT table name
            gUpload.ConfigFileName = "AncillaryUploadGridConfig"; //This is the file name that will store any user customizations to the grid - must be unique in the app
            gUpload.SetGridSelectionBehavior(true, false); //Sets standard grid behavior for record select and multiselect
            gUpload.FieldLayoutResourceString = "AncillaryUploadGrid"; //The name of the FieldLayout in the Field Layouts xaml file - Must be unique
            //Add this code to use a different style for the grid than the default.
            //This particular style is used for color coded grids.
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            gUpload.GridCellValuePresenterStyle = CellStyle;
            gUpload.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;

            

            //Add all grids to the grid collection - This allows grids to automatically load and participate with security
            GridCollection.Add(gUpload);

            //Set dummy parm values for initial load
            SetParms(0,0);
            RetrieveData();
            SetWindowStatus();


        }

        private void SetWindowStatus()
        {
            if (CurrentState == ScreenState.Empty)
            {
                //btnVerify.IsEnabled = false;
                btnUpload.IsEnabled = false;
            }
            return;
        }

        private void RetrieveData()
        {
            this.Load();
        }


        public void PreBind()
        {
            return;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {

            RandomStatus=random.Next(10000) * -1;

            Workbook dataWorkbook = null;

            Dictionary<string,int> ColumnDictionary = new Dictionary<string, int>();

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
                            //Upload to the ancillary_upload table
                            SetParms(RandomStatus,0);

                            string retVal = cGlobals.BillService.AncillaryUnitUpload(CurrentBusObj.ObjectData, cGlobals.UserName);
                            //RES Phase 3.1 Display distnct service periods in upload file
                            //RES 4/4/16 Check for service period that is not first day of the month
                            if (retVal.Substring(0, 5) == "Error")
                            {
                                Messages.ShowMessage(retVal, System.Windows.MessageBoxImage.Error);
                                //Messages.ShowYesNo(retVal);
                                //if (Messages.ShowOkCancel(retVal, System.Windows.MessageBoxImage.Stop) == MessageBoxResult.OK)
                                //    retVal = "";
                                //else
                                //{
                                    SetParms(0, 0);
                                    this.Load();
                                    Messages.ShowMessage("Ancillary Upload failed!", System.Windows.MessageBoxImage.Exclamation);
                                    return;
                                //}
                            }
                            else
                                if (retVal.Substring(0,7) == "Service")
                                {
                                    Messages.ShowInformation(retVal);
                                    retVal = "";
                                }
                            if (retVal != "")
                            {
                                Messages.ShowInformation(retVal);
                                //Clear window and data
                                SetParms(0, 0);
                                this.Load();
                                return;
                            }
                            else
                                this.Load();


                            //btnVerify.IsEnabled = true;
                            Messages.ShowMessage(String.Format("File, {0}, loaded.", fileInfo.Name), System.Windows.MessageBoxImage.Information);
                            btnUpload.IsEnabled = true ;
                            btnLoad.IsEnabled = false;
                        }
                    }
                    else
                    {
                        Messages.ShowMessage(string.Format("Invalid Columns were detected.  Please check the excel document headers and try again. {0}", message).Trim(), System.Windows.MessageBoxImage.Exclamation);
                    }
                }
            }

        }

        //private void btnVerify_Click(object sender, RoutedEventArgs e)
        //{
        //    if (CurrentBusObj.ObjectData.Tables["ancillary_upload"] != null)
        //    {
        //        foreach (DataRow r in CurrentBusObj.ObjectData.Tables["ancillary_upload"].Rows)
        //        {
        //            //SetParms(Convert.ToInt32(r["mca_address"]), r["anc_code"].ToString(), r["service_period"].ToString(), 0);
        //            CurrentBusObj.LoadTable("ancillary_verify");
        //            if (CurrentBusObj.ObjectData.Tables["ancillary_verify"] != null && CurrentBusObj.ObjectData.Tables["ancillary_verify"].Rows.Count > 0)
        //            {
        //                DataRow rv = CurrentBusObj.ObjectData.Tables["ancillary_verify"].Rows[0];
        //                r["error_message"] = rv["error_message"];
        //                r["location_id"] = rv["location_id"];
        //                r["location_city_state"] = rv["location_city_state"];
        //                r["can_upload"] = rv["can_upload"];
        //            }
        //            else
        //            {
        //                r["error_message"] = "Error attempting to verifying this record ";
        //            }

        //        }
        //        Messages.ShowInformation("Complete");
        //    }


        //}

        private void SetParms(int tempStatus, int finalStatus)
        {
            CurrentBusObj.Parms.ClearParms();
            CurrentBusObj.Parms.AddParm("@temp_status", tempStatus );
            CurrentBusObj.Parms.AddParm("@final_status_code", finalStatus );

        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentBusObj.ObjectData.Tables["ancillary_upload"] != null)
            {
                SetParms(RandomStatus, RandomStatus);
                CurrentBusObj.LoadTable("upload_ancillary");
                if (CurrentBusObj.ObjectData.Tables["upload_ancillary"] != null && CurrentBusObj.ObjectData.Tables["upload_ancillary"].Rows.Count > 0)
                {
                    DataRow r = CurrentBusObj.ObjectData.Tables["upload_ancillary"].Rows[0];
                    Messages.ShowInformation("Uploaded " + r["units_uploaded"].ToString() + " unit rows. Error Rows: " + r["error_rows"].ToString()
                                                + " " + r["error_text"].ToString());
                    btnUpload.IsEnabled=false;
                    btnLoad.IsEnabled=true;
                    SetParms(0,0);
                    this.Load();
                }
                else
                {
                    Messages.ShowInformation("Upload failed. If error persists contain support.");
                }
            }

        }

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

                string worksheetCellValue = FixUnitUploadColumnNames.UnFixColumnNames(sheetOne.Rows[0].Cells[col.Index].Value.ToString()).ToLower();

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
            DataRow r = CurrentBusObj.ObjectData.Tables["ancillary_upload"].NewRow();

            try
            {
                
                foreach (var vkp in ColumnDictionary)
                {
                    if (!TF) { break; } //check for error
                    CurrentColumn = vkp.Key;
                    switch (vkp.Key)
                    {
                        case "mca_address":
                            r["mca_address"] = GetCellValue<string>(row.Cells[vkp.Value].Value, 0);
                            break;
                        case "anc_code":
                            r["anc_code"] = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "service_period":
                            r["service_period"] = ConvertExcelDateFormatToDateTime(row.Cells[vkp.Value].Value);
                            break;
                        case "start_date":
                            r["start_date"] = ConvertExcelDateFormatToDateTime(row.Cells[vkp.Value].Value);
                            break;
                        case "end_date":
                            r["end_date"] = ConvertExcelDateFormatToDateTime(row.Cells[vkp.Value].Value);
                            break;
                        case "comment":
                            r["comment"]= GetCellValue<string>(row.Cells[vkp.Value].Value, 0);
                            break;
                        case "service_id":
                            r["service_id"] = GetCellValue<string>(row.Cells[vkp.Value].Value, 0);
                            break;
   
                        default:
                            TF = false;
                            ErrorMessage = string.Format("Unable to match column to {0}", vkp.Key);
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
            //Set the can upload value to false on all records on load
            r["can_upload"] = 0;
            r["user_id"] = User;
            r["temp_status"] = RandomStatus;
            if (TF) { CurrentBusObj.ObjectData.Tables["ancillary_upload"].Rows.Add(r); }
            return TF;
        }

    }
}
