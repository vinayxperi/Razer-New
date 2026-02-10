using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Infragistics.Documents.Excel;
using Infragistics.Windows.DataPresenter;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using RazerBase.Interfaces;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows;

namespace Units
{

    /// <summary>
    /// This class represents a 'UnitUploadTab' object.
    /// </summary>
    public partial class UnitUploadTab : ScreenBase, IPreBindable, IScreen
    {
        public string WindowCaption { get { return string.Empty; } }
        DataTable dtMisc = new DataTable("total"); //Datatable for binding row count and unit totals

        cBaseBusObject UnitUpload = new cBaseBusObject("UnitUpload");
        cBaseBusObject ContractLookup = new cBaseBusObject("ContractLookup");
        cBaseBusObject ContractReportLookup = new cBaseBusObject("ContractReportLookup");

        private string ErrorMessage = "";
        private string mso_id = "0";
        private string cs_id = "0";
        public Boolean deleteUpload = false;

        List<string> PropertyList = new List<string>(new string[] { "mso_name", "cs_name", "product_code", "service_period_start", "service_period_end", "amount", "contract_description", 
                                                                    "report_description", "unit_description", "unit_period_type_description", "contract_id", "report_id", "cs_id", "destination country", 
                                                                    "manufactured country", "model", "technology", "manufactured product", "brand", "subscriber", "ancillary", "data", 
                                                                    //"title","replicator","oem","software","psa_city", "psa_state","estimated_flag" });
                                                                    "title","replicator","oem","software","tivo count description","psa_city", "psa_state","estimated_flag" });
        
        //DWR ADDED 11/5/12 - to provide all required fileds in upload file so that file can be rejected on the load if one of the columns is missing
        List<string> RequiredColumnList = new List<string>(new string[] {"product_code","service_period_start", "service_period_end", "amount",
                                                                          "unit_period_type_description", "contract_id", "report_id", "cs_id"});

        Workbook dataWorkbook;
        ObservableCollection<Unit> Units;

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'UnitUploadTab' object and call the ScreenBase's constructor.
        /// </summary>
        public UnitUploadTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform ase for this object
        }
        #endregion

        #region Init()
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            this.CanExecuteSaveCommand = true;
            bgResults.SetGridSelectionBehavior(true, true);
            bgResults.xGrid.FieldSettings.AllowEdit = false;
            bgResults.ContextMenuAddIsVisible = false;

            bgResults.ContextMenuGenericDelegate1 = bgResultsSaveGrid;
            bgResults.ContextMenuGenericDisplayName1 = "Save Unit Upload Changes";
            bgResults.ContextMenuGenericIsVisible1 = false;

            bgResults.ContextMenuRemoveDelegate = RemoveUnit;
            bgResults.ContextMenuRemoveDisplayName = "Delete Unit";
            bgResults.ContextMenuRemoveIsVisible = false;


            this.DoNotSetDataContext = false;
            this.SetUnitUploadDefaultParms();
            btnVerify.IsEnabled = false;
            btnUpload.IsEnabled = false;

            dtMisc.Columns.Add("unit_total",typeof(Decimal));
            dtMisc.Columns.Add("row_count",typeof(int));
            dtMisc.Rows.Add(0.0M, 0);
            txtUnitTotal.DataContext = dtMisc;
            txtRowCount.DataContext = dtMisc;

            //DWR - Added 11/2/12
            //Check Security for the tab and hide the load button as appropriate.

            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
                btnLoad.IsEnabled = false;
            else
                btnLoad.IsEnabled = true; 
        }
                
        /// <summary>
        /// Sets up parms on load so we can just 
        /// </summary>
        private void SetUnitUploadDefaultParms()
        {
            UnitUpload.Parms.AddParm("@entity_id", 0);
            UnitUpload.Parms.AddParm("@contract_id", 0);
            UnitUpload.Parms.AddParm("@report_id", 0);
            UnitUpload.Parms.AddParm("@cs_id", 0);
            UnitUpload.Parms.AddParm("@service_period_start", Convert.ToDateTime("1-1-1900"));
            UnitUpload.Parms.AddParm("@service_period_end", Convert.ToDateTime("1-1-1900"));

            UnitUpload.Parms.AddParm("@unit_type", -1);
            UnitUpload.LoadData();

            //for lookups
            ContractLookup.Parms.AddParm("@show_inactive", 0);
            ContractLookup.Parms.AddParm("@show_archived", 0);
            ContractLookup.Parms.AddParm("@mso_id", -1);
            ContractLookup.LoadData();

            ContractReportLookup.Parms.AddParm("@contract_id", 0);
            ContractReportLookup.LoadData();

            this.ltbContract.SetBindingExpression("contract_id", "contract_description", this.ContractLookup.GetTable("contract_lookup") as DataTable);
            this.ltbReport.SetBindingExpression("report_id", "description", this.ContractReportLookup.GetTable("ContractReportLookup") as DataTable);
            ltbReport.IsEnabled = false;

            ltbUnitType.SetBindingExpression("unit_type_id", "unit_description", this.UnitUpload.GetTable("UnitType") as DataTable);

            btnGetData.Focus();
        }

        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    //this.ltbReport.BindingObject = EstablishListObjectBinding(this.CurrentBusObj.GetTable("unit_report") as DataTable, true, "report_description",
                    //"report_id", "Select Location");
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }
        #endregion

        private void bgResultsSaveGrid()
        {
            bgResults.xGrid.ExecuteCommand(Infragistics.Windows.DataPresenter.DataPresenterCommands.EndEditModeAndAcceptChanges);

            Infragistics.Windows.DataPresenter.UpdateMode currentMode = bgResults.xGrid.UpdateMode;

            bgResults.xGrid.UpdateMode = Infragistics.Windows.DataPresenter.UpdateMode.OnUpdate;
            bgResults.xGrid.ExecuteCommand(Infragistics.Windows.DataPresenter.DataPresenterCommands.CommitChangesToAllRecords);

            bgResults.xGrid.UpdateMode = currentMode;

            DataTable unitChanges = UnitUpload.ObjectData.Tables["Unit"].GetChanges(DataRowState.Modified);
            if (unitChanges != null)
            {
                foreach (DataRow row in unitChanges.Rows)
                {
                    int unit_id = Convert.ToInt32(row["unit_id"]);
                    decimal amount = Convert.ToDecimal(row["amount"]);
                    cGlobals.BillService.UpdateUnitAmount(unit_id, amount);
                }
            }
            Messages.ShowInformation("Save Completed.");
        }

        public override void Save()
        {
            
            //base.Save();
            bgResultsSaveGrid();
            //DWR-Added 5/4/12 to total units and rows after save
            CalculateTotals();
        }

        private void RemoveUnit()
        {
            //get unit id and call delete

            //accept changes
            UnitUpload.ObjectData.AcceptChanges();

            if (deleteUpload) //delete only grid rows from uploaded spreadsheet
            {
                if (bgResults.xGrid.SelectedItems.Records.Count != 0)
                {
                    bgResults.xGrid.RecordsDeleting += (sender, e) => e.DisplayPromptMessage = true;
                    bgResults.xGrid.ExecuteCommand(DataPresenterCommands.DeleteSelectedDataRecords);
                    return;

                }
                else
                {
                    Messages.ShowInformation("No records were selected to delete.");
                    return;
                }           


            }
            else
            { //delete rows in database from existing unit records

                int id = 0;

                

                string count = bgResults.xGrid.SelectedItems.Records.Count.ToString();


                if (bgResults.xGrid.SelectedItems.Records.Count > 0)
                {
                    string message = "Do you want to delete the " + count + " selected records?";
                    if (Messages.ShowYesNo(message, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return;
                    }
                    {
                        foreach (DataRecord r in bgResults.xGrid.SelectedItems)
                        {
                            if (r != null)
                            {
                                DataRow row = (r.DataItem as DataRowView).Row;
                                id = Convert.ToInt32(row["unit_id"]);
                                cGlobals.BillService.DeleteUnit(id);

                            }

                        }


                        if (bgResults.xGrid.SelectedItems.Count() != 0)
                        {

                            bgResults.xGrid.RecordsDeleting += (sender, e) => e.DisplayPromptMessage = false;
                            bgResults.xGrid.ExecuteCommand(DataPresenterCommands.DeleteSelectedDataRecords);

                        }
                    }
                }
                else
                {
                    Messages.ShowInformation("No records were selected to delete. On this screen, you must click on left side of grid to select records, not in a cell.");
                    return;
                }                

            }
            //DWR-Added 5/4/12 - Updates totals after delete
            UnitUpload.ObjectData.AcceptChanges();
            CalculateTotals();
        }

        private void btnGetData_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //RES -- Added 4/3/13 to check date fields for 1/1/1900 due to base object change they won't have nulls
            //if (ltbContract.SelectedValue == null && ltbReport.SelectedValue == null && ldpPeriod.SelText == null && ldpPeriodEnd.SelText == null 
            //    && ltbUnitType.SelectedValue == null && (txtEntity.Text== null || txtEntity.Text=="") && (txtLocation.Text == null || txtLocation.Text=="") )
            //{
            //    Messages.ShowInformation("You must enter at least one condition before running a unit query.");
            //    return;
            //}
            if (ltbContract.SelectedValue == null && ltbReport.SelectedValue == null && ldpPeriod.SelText.ToString() == "1/1/1900 12:00:00 AM"
                && ldpPeriodEnd.SelText.ToString() == "1/1/1900 12:00:00 AM"
                && ltbUnitType.SelectedValue == null && (txtEntity.Text == null || txtEntity.Text == "") && (txtLocation.Text == null || txtLocation.Text == ""))
            {
                Messages.ShowInformation("You must enter at least one condition before running a unit query.");
                return;
            }
            //DWR -- Added 5/4/12 to prevent ability to delete units if view only security
            if (this.SecurityContext == AccessLevel.ViewUpdate || this.SecurityContext == AccessLevel.ViewUpdateDelete)
            {
                bgResults.ContextMenuRemoveIsVisible = true;
                bgResults.xGrid.FieldSettings.AllowEdit = true;
                bgResults.ContextMenuGenericIsVisible1 = true;
            }

            
            bgResults.SetGridSelectionBehavior(true, true);
            bgResults.checkForContextMenuHides();

             //LoadResults("1", "39", "1", "5", "1-1-1900", "3");
            LoadResults(mso_id,
                (ltbContract.SelectedValue == null ? "0" : ltbContract.SelectedValue.ToString()),
                (ltbReport.SelectedValue == null ? "0" : ltbReport.SelectedValue.ToString()),
                cs_id,
                (ldpPeriod.SelText == null ? "1-1-1900" : ldpPeriod.SelText.Value.ToShortDateString()),
                (ldpPeriodEnd.SelText == null ? "1-1-1900" : ldpPeriodEnd.SelText.Value.ToShortDateString()),
                (ltbUnitType.SelectedValue == null ? "0" : ltbUnitType.SelectedValue.ToString()));
            //DWR-Added 5/4/12 to total units and rows after load
            CalculateTotals();
        }

        private void LoadResults(string entity_id, string contract_id, string report_id, string cs_id, string service_period_start, string service_period_end, string unit_type, bool IsClearing = false)
        {

            UnitUpload.changeParm("@entity_id", entity_id);
            UnitUpload.changeParm("@contract_id", contract_id);
            UnitUpload.changeParm("@report_id", report_id);
            UnitUpload.changeParm("@cs_id", cs_id);
            UnitUpload.changeParm("@service_period_start", service_period_start);
            UnitUpload.changeParm("@service_period_end", service_period_end);
            UnitUpload.changeParm("@unit_type", unit_type);
            UnitUpload.LoadData();
            bgResults.LoadGrid(UnitUpload, "Unit");

            if (bgResults.xGrid.ViewableRecords.Count == 0 && !IsClearing)
            {
                Messages.ShowInformation("No records matching the set parameters were found.");
            }

            SetColumnEditability(true);
            FixColumnNames();
        }

        private void SetColumnEditability(bool IsEditable)
        {
            foreach (FieldLayout layout in bgResults.xGrid.FieldLayouts)
            {
                if (layout != null)
                {
                    foreach (Field field in layout.Fields)
                    {
                        if (field.Name == "unit_id") { field.Visibility = System.Windows.Visibility.Collapsed; }
                        if (IsEditable && field.Name != "amount") { field.Settings.AllowEdit = false; }
                    }
                }
            }
        }

        private void FixColumnNames()
        {
            foreach (FieldLayout layout in bgResults.xGrid.FieldLayouts)
            {
                if (layout != null && layout.Fields["amount"] != null)
                {
                    layout.Fields["amount"].Settings.EditAsType = typeof(Double);
                }

                FixUnitUploadColumnNames.FixColumnNames(layout);
            }
        }

        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ClearGrid();
        }

        private void ClearGrid()
        {
            bgResults.ContextMenuRemoveIsVisible = false;
            bgResults.xGrid.FieldSettings.AllowEdit = false;
            bgResults.ContextMenuGenericIsVisible1 = false;
            SetColumnEditability(false);
            bgResults.SetGridSelectionBehavior(false, true);
            bgResults.checkForContextMenuHides();


            LoadResults("-1", "-1", "-1", "-1", "1-1-1900", "1-1-1900", "-1", true);
            deleteUpload = false;
            btnVerify.IsEnabled = false;
            btnUpload.IsEnabled = false;
            //DWR-Added 5/4/12 to total units and rows after clear
            CalculateTotals();
        }

        private void btnLoad_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //DWR-Added 5/4/12 - Prevents users with view only from being able to delete rows.
            if (this.SecurityContext == AccessLevel.ViewUpdateDelete || this.SecurityContext == AccessLevel.ViewUpdate)
            {
                bgResults.ContextMenuRemoveIsVisible = true;
            }


            deleteUpload = true;
            bgResults.xGrid.FieldSettings.AllowEdit = false;
            bgResults.ContextMenuGenericIsVisible1 = false;
            SetColumnEditability(false);
            bgResults.SetGridSelectionBehavior(true, true);
            bgResults.checkForContextMenuHides();

            dataWorkbook = null;
            Units = new ObservableCollection<Unit>();

            var AreColumnsVerified = false;
            var ColumnDictionary = new Dictionary<string, int>();

            //Create file dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx";

            //Open the Pop-Up Window to select the file 
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileInfo =  new FileInfo(dlg.FileName);
                var IsExcelParsed = true;
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
                    Messages.ShowMessage("The application was unable to load the excel document.  Check to see if it is open. Please try again or contact Razer support", System.Windows.MessageBoxImage.Exclamation);
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
                            btnVerify.IsEnabled = true;

                            bgResults.xGrid.DataSource = Units;

                            FixColumnNames();
                            //DWR Added 11/5/12 to drop the estimated flag from the upload
                            if (bgResults.xGrid.FieldLayouts[0].Fields["estimated_flag"] != null)
                            {
                                bgResults.xGrid.FieldLayouts[0].Fields["estimated_flag"].Visibility = Visibility.Collapsed;
                            }
                            Messages.ShowMessage(String.Format("File, {0}, loaded.", fileInfo.Name), System.Windows.MessageBoxImage.Information);
                            btnUpload.IsEnabled = false;
                        }
                    }
                    else
                    {
                        Messages.ShowMessage(string.Format("Invalid Columns were detected.  Please check the excel document headers and try again. {0}", message).Trim(), System.Windows.MessageBoxImage.Exclamation);
                    } 
                }
            }


 

            //DWR-Added 5/4/12 to total units and rows after load of spreadsheet
            if (bgResults.ActiveRecord == null && bgResults.xGrid.Records.Count > 0)
            {
                bgResults.ActiveRecord = bgResults.xGrid.Records[0] as DataRecord;
                UnitUpload.ObjectData.Tables["unit"].Rows.Clear();
            }

            CalculateTotals();
        }

        private void btnVerify_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //DWR-Added 5/7/12 - Boolean to track if any records are missing metadata
            bool MissingMetadata = false;

            UnitVerificaiton UnitVerificaiton = new UnitVerificaiton(UnitUpload.ObjectData);
            if (UnitVerificaiton.IsVerificationDataLoaded)
            {
                bool UploadsNeedCorrection = false;
                foreach (Unit Unit in Units)
                {
                    foreach (string propertyName in PropertyList)
                    {
                        //Verify basic data integrity against Unit data annotations
                        string validationMessage = Unit.ValidateProperty(propertyName.Replace(" ", "_"));
                        if (!string.IsNullOrEmpty(validationMessage))
                        {
                            Unit.UploadCorrections.Add(
                                new UploadCorrection
                                {
                                    PropertyName = propertyName,
                                    HasValidationError = true,
                                    OriginalValue = (Unit.GetPropertyValue(propertyName) ?? "").ToString(),
                                    CorrectedValue = validationMessage
                                });
                        }

                    }
                    UnitVerificaiton.Verify(Unit, PropertyList);
                    if (Unit.HasCorrections) 
                    { 
                        UploadsNeedCorrection = true;
                        Unit.HasMetadata = true;
                    }
                    //DWR - Added-5/7/12 - If unit does not have any metadata the verification process will stop and a message will be sent to the user.
                    //All unit rows require at least one metadata item.
                    //If Unit has errors then if statement above will catch it and it will be assumed to have metadata
                    else if (!Unit.HasMetadata)
                    {
                        MissingMetadata = true;
                     }

                }
                //DWR - Added-5/7/12- If even on unit is missing metadata, the entire load will stop.
                if (MissingMetadata)
                {
                    Messages.ShowWarning("Error - All units must have at least one metadata item for units to be able to be verified." +
                     "  Please review HasMetadata column to the right of the upload grid for unchecked boxes to see which units are missing metadata.");
                    return;
                }

                if (UploadsNeedCorrection)
                {
                    UnitUploadVerificationLookup unitVerificationDialog = new UnitUploadVerificationLookup(Units, UnitVerificaiton, bgResults.xGrid.FieldLayouts);
                    unitVerificationDialog.ShowDialog();
                    if (unitVerificationDialog.CanUpload)
                    {
                        bgResults.xGrid.DataSource = null;  //clear the grid so the changes can be seem
                        bgResults.xGrid.DataSource = Units; //repoint the gird to the Units object collection
                        //DWR -- Added 5/4/12 to prevent ability to upload units if view only security
                        if (this.SecurityContext == AccessLevel.ViewUpdate || this.SecurityContext==AccessLevel.ViewUpdateDelete )
                        {
                            btnUpload.IsEnabled = true; 
                        }

                    }
                }
                else
                {
                    //DWR -- Added 5/4/12 to prevent ability to upload units if view only security
                    if (this.SecurityContext == AccessLevel.ViewUpdate || this.SecurityContext == AccessLevel.ViewUpdateDelete)
                    {
                        btnUpload.IsEnabled = true;
                    }
                }
            }
 

            //DWR-Added 5/4/12 to total units and rows after verify
            CalculateTotals();
        }

        private void btnUpload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            List<UnitUpload> Uploads = new List<UnitUpload>();

            foreach (Unit Unit in Units)
            {
                Uploads.Add(Unit.UnitUpload);
            }

            List<UnitUpload> FinalizedList = (from x in Uploads
                                              group x by new
                                              {
                                                  x.contract_id,
                                                  x.report_id,
                                                  x.mso_id,
                                                  x.cs_id,
                                                  x.unit_type_id,
                                                  x.service_period_end,
                                                  x.service_period_start,
                                                  x.unit_period_type,
                                                  x.product_code,
                                                  x.estimated_flag,
                                                  x.ancillary_id,
                                                  x.manu_country_id,
                                                  x.dest_country_id,
                                                  x.model_id,
                                                  x.model,
                                                  x.tech_id,
                                                  x.brand_id,
                                                  x.brand,
                                                  x.data_service_type_id,
                                                  x.subscriber_id,
                                                  x.manufacturer_product_id,
                                                  x.replicator_id,
                                                  x.oem_id,
                                                  x.title_id,
                                                  x.software_id,
                                                  x.tivo_count_id,
                                                  x.tivo_count_description
                                              } into g
                                              select new UnitUpload
                                              {
                                                  contract_id = g.Key.contract_id,
                                                  report_id = g.Key.report_id,
                                                  mso_id = g.Key.mso_id,
                                                  cs_id = g.Key.cs_id,
                                                  unit_type_id = g.Key.unit_type_id,
                                                  service_period_end = g.Key.service_period_end,
                                                  service_period_start = g.Key.service_period_start,
                                                  unit_period_type = g.Key.unit_period_type,
                                                  product_code = g.Key.product_code,
                                                  ancillary_id = g.Key.ancillary_id,
                                                  manu_country_id = g.Key.manu_country_id,
                                                  dest_country_id = g.Key.dest_country_id,
                                                  model_id = g.Key.model_id,
                                                  model = g.Key.model,
                                                  tech_id = g.Key.tech_id,
                                                  brand_id = g.Key.brand_id,
                                                  brand = g.Key.brand,
                                                  data_service_type_id = g.Key.data_service_type_id,
                                                  subscriber_id = g.Key.subscriber_id,
                                                  manufacturer_product_id = g.Key.manufacturer_product_id,
                                                  replicator_id=g.Key.replicator_id,
                                                  title_id=g.Key.title_id,
                                                  software_id=g.Key.software_id,
                                                  oem_id=g.Key.oem_id,
                                                  tivo_count_id = g.Key.tivo_count_id,
                                                  tivo_count_description = g.Key.tivo_count_description,
                                                  amount = g.Sum(s => s.amount)
                                              }).ToList();

            DataSet ds = new DataSet();
            DataTable dt = ToDataTable<UnitUpload>(FinalizedList);
            dt.TableName = "Uploads";
            ds.Tables.Add(dt);

            StartUploadAnimation();

            UploadUnits UploadUnits = new UploadUnits(ds,   new UnitUploadCallback(UploadUnitsResult));
            Thread t = new Thread(new ThreadStart(UploadUnits.Upload));
            t.Start();
        }

        public void StartUploadAnimation()
        {
            btnUpload.IsEnabled = false;

            double time = 2;
            Duration duration = TimeSpan.FromSeconds(time);
            double point = 0;
            StringAnimationUsingKeyFrames sa = new StringAnimationUsingKeyFrames();

            sa.KeyFrames.Add(new DiscreteStringKeyFrame("            ", TimeSpan.FromSeconds(0)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("           U", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("          Up", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("         Upl", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("        Uplo", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("       Uploa", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("      Upload", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("     Uploadi", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("    Uploadin", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("   Uploading", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("  Uploading.", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame(" Uploading..", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("Uploading...", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("ploading... ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("loading...  ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("oading...   ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("ading...    ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("ding...     ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("ing...      ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("ng...       ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("g...        ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("...         ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame("..          ", FigureTime(ref point, time)));
            sa.KeyFrames.Add(new DiscreteStringKeyFrame(".           ", FigureTime(ref point, time)));
            sa.RepeatBehavior = RepeatBehavior.Forever;

            sa.Duration = duration;

            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Duration = duration;
            myStoryboard.Children.Add(sa);

            Storyboard.SetTarget(sa, btnUpload);
            Storyboard.SetTargetProperty(sa, new PropertyPath("Button.Content"));

            btnUpload.BeginAnimation(System.Windows.Controls.Button.ContentProperty, sa);
        }

        private KeyTime FigureTime(ref double point, double duration)
        {
            return TimeSpan.FromSeconds(point += duration / 24);
        }

        public void EndUploadAnimation()
        {
            btnUpload.BeginAnimation(System.Windows.Controls.Button.ContentProperty, null);
        }

        public void UploadUnitsResult(string result)
        {
            if (result == "")
            {
                try
                {
                    Messages.ShowInformation("Upload Completed at : " + DateTime.Now.ToString());
                    btnUpload.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                btnUpload.IsEnabled = false;
                            }));

                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                ClearGrid();
                                EndUploadAnimation();
                            }));
                }
                catch { /* Do Nothing */ }
                finally { /* Do Nothing */ }
            }
            else
            {
                try
                {
                    Messages.ShowError(result);
                    btnUpload.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                btnUpload.IsEnabled = true;
                            }));

                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                EndUploadAnimation();
                            }));
                }
                catch { /* do Nothing */ }
            }
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dt = new DataTable();
            Type tType = typeof(T);
            PropertyInfo[] props = tType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                DataColumn col = new DataColumn(prop.Name, prop.PropertyType);
                dt.Columns.Add(col);
            }

            foreach (T item in items)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo prop in props)
                {
                    dr[prop.Name] = prop.GetValue(item, null);
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private void VerifyColumnsInExcel(Workbook dataWorkbook, ref bool AreColumnsVerified, ref Dictionary<string, int> ColumnDictionary, ref string message)
        {
            int colIndex = 0;
            Worksheet sheetOne = dataWorkbook.Worksheets[0];
          

            foreach (WorksheetColumn col in sheetOne.Columns)
            {
                //DWR - 3/8/12 - Added to throw error if more data exists than was loaded in through the previous step.
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

            //DWR ADDED 11/5/12 - Checks to make sure that spreadsheet contains all required fields
            //If not the upload will generate an error
            foreach (string s in RequiredColumnList)
            {
                if (!ColumnDictionary.ContainsKey(s))
                {

                    if (message == "")
                        message = "Not all required columns are included in upload.  Missing column(s) " + s;
                    else
                        message = message + ", " + s;
                }
            }

            if (message != "")
            {
                AreColumnsVerified = false;
                //Messages.ShowError(message);
                return;
            }
            else
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

        private bool ParseWorksheet(Dictionary<string, int> ColumnDictionary, WorksheetRow row)
        {
            var TF = true;
            Unit Unit = new Unit();
            string CurrentColumn = "";

            try
            {
                foreach (var vkp in ColumnDictionary)
                {
                    if (!TF) { break; } //check for error
                    CurrentColumn = vkp.Key;
                    switch (vkp.Key)
                    {
                        case "amount":
                            Unit.amount = GetCellValue<decimal>(row.Cells[vkp.Value].Value, 0);
                            break;
                        case "ancillary":
                            Unit.ancillary = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "brand":
                            Unit.brand = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "contract_id":
                            Unit.contract_id = GetCellValue<int>(row.Cells[vkp.Value].Value, 0);
                            break;
                        case "contract_description":
                            Unit.contract_description = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "cs_id":
                            if (row.Cells[vkp.Value].Value == null) { throw new Exception("Location ID cannot be blank"); }
                            Unit.cs_id = GetCellValue<int>(row.Cells[vkp.Value].Value, 0);
                            break;
                        //case "mso_id":
                        //    Unit.mso_id = GetCellValue<int>(row.Cells[vkp.Value].Value, 0);
                        //    break;
                        case "report_id":
                            Unit.report_id = GetCellValue<int>(row.Cells[vkp.Value].Value, 0);
                            break;
                        case "cs_name":
                            Unit.cs_name = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "data":
                            Unit.data = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "destination country":
                            Unit.destination_country = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "manufactured country":
                            Unit.manufactured_country = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "manufactured product":
                            Unit.manufactured_product = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "model":
                            Unit.model = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "title":
                            Unit.title = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "replicator":
                            Unit.replicator = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "oem":
                            Unit.oem = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "software":
                            Unit.software = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "mso_name":
                            Unit.mso_name = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "product_code":
                            Unit.product_code = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "report_description":
                            Unit.report_description = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "service_period_end":
                            if (row.Cells[vkp.Value].Value == null) { throw new Exception("Service Period End cannot be blank"); }
                            Unit.service_period_end = ConvertExcelDateFormatToDateTime(row.Cells[vkp.Value].Value);
                            break;
                        case "service_period_start":
                            if (row.Cells[vkp.Value].Value == null) { throw new Exception("Service Period Start cannot be blank"); }
                            Unit.service_period_start = ConvertExcelDateFormatToDateTime(row.Cells[vkp.Value].Value);
                            break;
                        case "subscriber":
                            Unit.subscriber = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "technology":
                            Unit.technology = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "unit_period_type_description":
                            Unit.unit_period_type_description = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "unit_description":
                            if (row.Cells[vkp.Value].Value == null) { throw new Exception("Unit Type cannot be blank"); }
                            Unit.unit_description = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "psa_city":
                            Unit.psa_city = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "psa_state":
                            Unit.psa_state = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        case "tivo count description":
                            Unit.tivo_count_description = GetCellValue<string>(row.Cells[vkp.Value].Value, "").Trim();
                            break;
                        //DWR Added 11/5/12
                        //This keeps estimated flag field from crashing upload, but field is only used to make upload work and is not required. User is not allowed to set the value
                        case "estimated_flag":
                            Unit.estimated_flag = GetCellValue<int>(row.Cells[vkp.Value].Value, 0);
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
            if (TF) { Units.Add(Unit); }
            return TF;
        }

        private DateTime ConvertExcelDateFormatToDateTime(object dateVal)
        {
            //Excel store dates as doubles
            //the left hand side is the number of days 1 is equivalent to 1/1/1900
            //the right hand side is the time stored as decimal numbers between .0 and .99999, where .0 is 00:00:00 and .99999 is 23:59:59.
            return DateTime.Parse("1-1-1900").AddDays(Convert.ToDouble(dateVal) - 2);
        }

        private void ltbContract_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedValue":
                    if (ltbContract.SelectedValue != null)
                    {
                        ContractReportLookup.changeParm("@contract_id", ltbContract.SelectedValue.ToString());
                        ContractReportLookup.LoadData();
                        DataTable dtContractReportLookup = this.ContractReportLookup.GetTable("ContractReportLookup") as DataTable;
                        this.ltbReport.SetBindingExpression("report_id", "description", dtContractReportLookup);
                        if (dtContractReportLookup != null && dtContractReportLookup.Rows.Count != 0)
                        {
                            ltbReport.IsEnabled = true;
                        }
                        else
                        {
                            ltbReport.IsEnabled = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void txtEntity_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //pull up entity lookup 
            EntityLookup entityLookup = new EntityLookup();

            // gets the users response
            entityLookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                mso_id = cGlobals.ReturnParms[0].ToString();
                txtEntity.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }

            filterContract();
        }

        private void filterContract(string mso_id = null)
        {
            ContractLookup.changeParm("@mso_id", (mso_id == null ? this.mso_id : mso_id));
            ContractLookup.LoadData();
            this.ltbContract.SetBindingExpression("contract_id", "contract_description", this.ContractLookup.GetTable("contract_lookup") as DataTable);
        }

        private void txtLocation_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LocationLookup ll = new LocationLookup();
            if (mso_id != "0")
            {
                cGlobals.ReturnParms.Add(mso_id);
            }

            ll.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                cs_id = cGlobals.ReturnParms[0].ToString();
                txtLocation.Text = cGlobals.ReturnParms[1].ToString();
                cGlobals.ReturnParms.Clear();
            }
        }

        private void btnClearSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtEntity.Text = "";
            mso_id = "0";
            ltbContract.SelectedValue = null;
            txtLocation.Text = "";
            cs_id = "0";
            ltbReport.SelectedValue = null;
            ltbReport.IsEnabled = false;
            ldpPeriod.SelText = null;
            ldpPeriodEnd.SelText = null;
            ltbUnitType.SelectedValue = null;
            filterContract("-1");
        }

        private void CalculateTotals()
        {
            int RowCount = 0;
            decimal UnitTotal = 0.0M;

            //DWR Modifed 6/13/12 - Was not getting into if statement when verifying with errors due to brand or model.
            if (bgResults.ActiveRecord != null && UnitUpload.HasObjectData || (bgResults.xGrid.Records != null && bgResults.xGrid.Records.Count>0))
            {
                if (UnitUpload.ObjectData.Tables["unit"].Rows.Count > 0)
                {
                    foreach (DataRow r in UnitUpload.ObjectData.Tables["unit"].Rows)
                    {
                        RowCount += 1;
                        UnitTotal += Convert.ToDecimal(r["amount"]);

                    }
                }
                else
                {
                    foreach (DataRecord r in bgResults.xGrid.Records)
                    {
                        RowCount += 1;
                        UnitTotal += Convert.ToDecimal(r.Cells["amount"].Value);
                    }
                }

            }
            dtMisc.Rows[0]["row_count"] = RowCount;
            dtMisc.Rows[0]["unit_total"] = UnitTotal;
            

        }
        //       private void CalculateTotals()
        //{

        //           if (bgResults.ActiveRecord != null)
        //           {

        //    foreach (DataRow rRemit in CurrentBusObj.ObjectData.Tables["remit"].Rows)
        //    {
        //        decimal remitSum = 0.0M;

        //        EnumerableRowCollection<DataRow> rAlloc = from RemitAlloc in CurrentBusObj.ObjectData.Tables["remit_alloc"].AsEnumerable()
        //                                                 where RemitAlloc.Field<Int32>("remit_id") == Convert.ToInt32(rRemit["remit_id"])
        //                                                 select RemitAlloc ;
        //        //Total the related allocations
        //        foreach (DataRow r in rAlloc)
        //        {
        //            remitSum+=Convert.ToDecimal(r["amount"]);
        //        }
        //        if (remitSum != Convert.ToDecimal(rRemit["amount"]))
        //        {
        //            //Once any error is received record message and exit the loop.
        //            TotalsOutOfBalance += "Remit local amounts and allocation amounts must match. ";
        //            break;
        //        }
        //    }
    }

    public delegate void UnitUploadCallback(string result);

    public class UploadUnits
    {
        private DataSet ds;
        UnitUploadCallback UnitUploadCallback;

        public UploadUnits(DataSet ds, UnitUploadCallback UnitUploadCallback)
        {
            this.ds = ds;
            this.UnitUploadCallback = UnitUploadCallback;
        }

        public void Upload()
        {
            //CLB Added UserName to schedule bill calc if report id is entered
            string retVal = cGlobals.BillService.UnitUpload(ds, cGlobals.UserName);
            if (UnitUploadCallback != null)
            {
                UnitUploadCallback(retVal);
            }
        }
    }

    public static class FixUnitUploadColumnNames
    {
        private static Dictionary<string, string> StaticNamesDict = new Dictionary<string, string>()
        {
            {"Contract ID","contract_id"},
            {"Contract Description", "contract_description"},
            {"Report ID","report_id"},
            {"Report Description","report_description"},
            //{"Entity ID","mso_id"},
            {"Entity Name","mso_name"},
            {"Location ID","cs_id"},
            {"Location Name","cs_name"},
            {"Unit Type","unit_description"},
            {"Service Period Start","service_period_start"},
            {"Service Period End","service_period_end"},
            {"Unit Period Type Description","unit_period_type_description"},
            {"Product Code","product_code"},
            {"Unit Count","amount"},
            {"Psa City", "psa_city"},
            {"Psa State", "psa_state"},
            {"MCA Address","mca_address"},
            {"Anc Code","anc_code"},
            {"Service Period","service_period"},
            {"Start Date","start_date"},
            {"End Date","end_date"},
            {"Comment","comment"},
            {"Service ID","service_id"},
            {"Estimated Flag","estimated_flag"}
        };

        public static void FixColumnNames(FieldLayout layout)
        {
            var SpecialColums = new Dictionary<string, string>();
            //SpecialColums.Add("mso_id", "Entity ID");
            SpecialColums.Add("cs_id", "Location ID");
            SpecialColums.Add("mso_name", "Entity Name");
            SpecialColums.Add("cs_name", "Location Name");
            SpecialColums.Add("amount", "Unit Count");
            SpecialColums.Add("unit_description", "Unit Type");

            if (layout != null)
            {
                foreach (Field field in layout.Fields)
                {
                    if (SpecialColums.ContainsKey(field.Name))
                    {
                        field.Label = SpecialColums[field.Name];
                    }
                    else
                    {
                        List<string> FieldNameMembers = new List<string>();

                        foreach (string FieldNameMember in field.Name.Replace("_", " ").Split(' '))
                        {
                            if (FieldNameMember.ToLower() == "id") { FieldNameMembers.Add(FieldNameMember.ToUpper()); }
                            else
                            {
                                string firstLetter = FieldNameMember.Substring(0, 1).ToUpper();
                                string newFieldNameMember = firstLetter + FieldNameMember.Remove(0, 1);
                                FieldNameMembers.Add(newFieldNameMember);
                            }
                        }
                        string FieldLabel = string.Join(" ", FieldNameMembers.ToArray());

                        field.Label = FieldLabel;
                    }
                }
            }
        }

        public static string UnFixColumnNames(string columnName)
        {
            string retVal = "";
            if (StaticNamesDict.ContainsKey(columnName))
            {
                retVal = StaticNamesDict[columnName];
            }
            else
            {
                retVal = columnName;
            }
            return retVal;
        }
    }

}
