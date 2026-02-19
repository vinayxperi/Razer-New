using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Admin
{

    /// Interaction logic for Razer.CustomInvoice

    public partial class ReportManager : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "rptsAvailtoRun";
        private static readonly string fieldLayoutResource2 = "rptsAvailtoView";
        private static readonly string mainTableName = "rptstorun";
        private static readonly string dataKey = "robject_member_name";
        private static readonly string dataKey2 = "app_id";
        private static readonly string dataKey3 = "description";
        private static readonly string server = "server_loc";
        private static readonly string file = "file_name";
        private static readonly string directory = "directory";
        public Boolean NoParmsInd = false;
        public string WindowCaption { get { return string.Empty; } }
        public string jobName = "";
        public string jobParms = "";
        public string rptParms = "";
        public int rptAppID = 0;
        public int totalParmsExpected = 0;
        public int totalParmsProvided = 0;
        public ComboBoxItemsProvider cmbProduct { get; set; }
        private static readonly string productDisplayPath = "product_description";
        private static readonly string productValuePath = "product_code";

        public ComboBoxItemsProvider cmbProductsWithContracts { get; set; }
        private static readonly string productsWithContractsDisplayPath = "product_description";
        private static readonly string productsWithContractsValuePath = "product_code";

        public ComboBoxItemsProvider cmbUnitMetaData { get; set; }
        private static readonly string metadataDisplayPath = "md_name";
        private static readonly string metadataValuePath = "unit_md_id";
        public ComboBoxItemsProvider cmbProductAll { get; set; }
        private static readonly string productAllDisplayPath = "product_description";
        private static readonly string productAllValuePath = "product_code";
        public ComboBoxItemsProvider cmbProductActive { get; set; }
        private static readonly string productActiveDisplayPath = "product_description";
        private static readonly string productActiveValuePath = "product_code";
        public ComboBoxItemsProvider cmbProductTypeAll { get; set; }
        private static readonly string productTypeAllDisplayPath = "product_type_description";
        private static readonly string productTypeAllValuePath = "product_type";
        public ComboBoxItemsProvider cmbProductType { get; set; }
        private static readonly string productTypeDisplayPath = "product_type_description";
        private static readonly string productTypeValuePath = "product_type";
        public ComboBoxItemsProvider cmbModelDescAll { get; set; }
        private static readonly string modelAllDisplayPath = "model_description";
        private static readonly string modelAllValuePath = "model_value";
        public ComboBoxItemsProvider cmbCompany { get; set; }
        private static readonly string companyDisplayPath = "company_description";
        private static readonly string companyValuePath = "company_code";
        public ComboBoxItemsProvider cmbCompanyAll { get; set; }
        private static readonly string companyAllDisplayPath = "company_description";
        private static readonly string companyAllValuePath = "company_code";
        public ComboBoxItemsProvider cmbMSO { get; set; }
        private static readonly string MSODisplayPath = "name";
        private static readonly string MSOValuePath = "mso_id";
        public ComboBoxItemsProvider cmbMSOType { get; set; }
        private static readonly string MSOTypeDisplayPath = "code_value";
        private static readonly string MSOTypeValuePath = "code_value";
        public ComboBoxItemsProvider cmbCreditHist1 { get; set; }
        private static readonly string CreditHist1DisplayPath = "credit_history_title";
        private static readonly string CreditHist1ValuePath = "credit_hist_id";
        public ComboBoxItemsProvider cmbCreditHist2 { get; set; }
        private static readonly string CreditHist2DisplayPath = "credit_history_title";
        private static readonly string CreditHist2ValuePath = "credit_hist_id";
        public ComboBoxItemsProvider cmbCreditHist3 { get; set; }
        private static readonly string CreditHist3DisplayPath = "credit_history_title";
        private static readonly string CreditHist3ValuePath = "credit_hist_id";
        public ComboBoxItemsProvider cmbProductNoAds { get; set; }
        private static readonly string productNoAdsDisplayPath = "product_description";
        private static readonly string productNoAdsValuePath = "product_code";
        public ComboBoxItemsProvider cmbProdItemEbif { get; set; }
        private static readonly string productItemEbifDisplayPath = "display";
        private static readonly string productItemEbifValuePath = "product_item_id";
        public ComboBoxItemsProvider cmbSubEbif { get; set; }
        private static readonly string subscriberTypeEbifDisplayPath = "display";
        private static readonly string subscriberTypeEbifValuePath = "subscriber_id";
        public ComboBoxItemsProvider cmbProductGroup { get; set; }
        private static readonly string productGroupDisplayPath = "product_group_description";
        private static readonly string productGroupValuePath = "product_group_description";
        private bool DonotSave = false;

        public ReportManager()
            : base()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.MainTableName = mainTableName;
            this.CurrentBusObj = businessObject;
            SetupReportsAvailtoRunGrid();
            SetupReportParmsGrid();
            SetupReportsAvailtoViewGrid();
            
            this.CurrentBusObj.Parms.AddParm("@app_id", 0);
            //this.CurrentBusObj.Parms.AddParm("@report_image_id", 0);
            this.Load(CurrentBusObj);

            if (businessObject.HasObjectData)
            {
                gReportsAvailtoRun.LoadGrid(businessObject, gReportsAvailtoRun.MainTableName);
            }
            else
            {
                Messages.ShowInformation("Unable to load available reports.");
            }

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
        }

        private void SetupReportsAvailtoRunGrid()
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //gReportsAvailtoRun.WindowZoomDelegate = GridDoubleClickDelegate;
            gReportsAvailtoRun.SingleClickZoomDelegate = GridRptsAvailSingleClickDelegate;
            gReportsAvailtoRun.xGrid.FieldLayoutSettings = layouts;
            gReportsAvailtoRun.FieldLayoutResourceString = fieldLayoutResource;
            gReportsAvailtoRun.MainTableName = mainTableName;
            gReportsAvailtoRun.DoNotSelectFirstRecordOnLoad = true;
            gReportsAvailtoRun.SetGridSelectionBehavior(true, false);  
        }

        private void SetupReportsAvailtoViewGrid()
        {
            
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            gRptstoView.WindowZoomDelegate = GridDoubleClickDelegateView;
            gRptstoView.xGrid.FieldLayoutSettings = layouts;
            gRptstoView.FieldLayoutResourceString = fieldLayoutResource2;
            gRptstoView.xGrid.FieldSettings.AllowEdit = true;
            gRptstoView.xGrid.UpdateMode = UpdateMode.OnCellChange;
            gRptstoView.SetGridSelectionBehavior(false, false);

            gRptstoView.ContextMenuGenericDelegate1 = RefreshDelegate;
            gRptstoView.ContextMenuGenericDisplayName1 = "Refresh";
            gRptstoView.ContextMenuGenericIsVisible1 = true;
            
        }

        private void RefreshDelegate()
        {
            this.Prep_ucBaseGridsForSave();
            if (IsScreenDirty)
            {
                System.Windows.MessageBoxResult result = Messages.ShowYesNo("Screen changes have not been saved. Do you want to save and refresh (Yes) or just refresh and lose changes (No)?",
                               System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    Save();
                }
            }
            this.Load(CurrentBusObj);
            gRptstoView.LoadGrid(CurrentBusObj, "rptView");
        }
      
        private void SetupReportParmsGrid()
        {
            //Set up generic context menu selections
            gReportParms.ContextMenuGenericDelegate1 = ContextMenuScheduleJob;
            gReportParms.ContextMenuGenericDisplayName1 = "Schedule Report to Run";
            gReportParms.ContextMenuGenericIsVisible1 = true;
            //gReportParms.ContextMenuGenericDelegate2 = ParmMulitSelect;
            //gReportParms.ContextMenuGenericDisplayName2 = "Select Multiple Parms";
            //gReportParms.ContextMenuGenericIsVisible2 = true;

            //Remove Context Menu Options
            gReportParms.ContextMenuAddIsVisible = false;
            gReportParms.ContextMenuRemoveIsVisible = false;
            gReportParms.ContextMenuResetGridSettingsIsVisible = false;
            gReportParms.ContextMenuSaveGridSettingsIsVisible = false;
            gReportParms.ContextMenuSaveToExcelIsVisible = false;

            gReportParms.xGrid.FieldSettings.AllowEdit = true;
            //gReportParms.SetGridSelectionBehavior(false, false);
            gReportParms.DoNotSelectFirstRecordOnLoad = true;
            gReportParms.SetGridSelectionBehavior(true, true);
        }

        //private void ParmMulitSelect()
        //{
        //}

        private void ContextMenuScheduleJob()
        {
            //Look to see if rptgrid has a row in it
            //pull values out of it and schedule job
            DateTime dtConvert = new DateTime();
            string sDate = "";
            jobParms = "";
            string sParms = "";
            string ToAcctPeriod = "Y";
            string FromAcctPeriod = "Y";
            string ToDueDate = "Y";
            string FromDueDate = "Y";
            
            if (gReportParms.xGrid.FieldLayouts.Count != 0)
            {
                 

                if (NoParmsInd == false)
                {
                    totalParmsProvided = 0;
                    this.Prep_ucBaseGridsForSave();
                    //RES 4/28/15 allow multi select for select reports
                    //RES 6/27/25 Added multi select for Bill Period Variance Report
                    if ((jobName == "Contract Rate Detail By Product") || (jobName == "Bill Period Variance Report"))
                    //if (jobName == "Contract Rate Detail By Product")
                    {
                        if (gReportParms.xGrid.SelectedItems.Records.Count > 0)
                            totalParmsProvided = totalParmsProvided + 1;
                        foreach (Record record in gReportParms.xGrid.SelectedItems.Records)
                        //foreach (Cell cell2 in (gReportParms.xGrid.SelectedItems.Records as DataRecord).Cells)
                        {
                            DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                            if (sParms == "")
                                sParms = row["product_code"].ToString().TrimEnd();
                            else
                                sParms += "," + row["product_code"].ToString().TrimEnd();
                        }
                        if (sParms.Length > 250)
                        {
                            Messages.ShowWarning("Product codes exceed parameter length limit of 250 characters");
                            return;
                        }
                        if (jobName == "Bill Period Variance Report")
                            jobParms += "/A \"" + sParms + "\" ";
                        else
                            jobParms += "/A " + sParms + " ";
                    }               
                    
                    else
                    foreach (Cell cell in (gReportParms.xGrid.Records[0] as DataRecord).Cells)
                    {
                        if ((cell.Value.ToString() != null) && (cell.Value.ToString() != ""))
                        {
                            if (cell.Value is DateTime)
                            {                                
                                dtConvert = Convert.ToDateTime(cell.Value.ToString());
                                if (jobName == "Deferral Reclass Report" && dtConvert.Day.ToString() != "1")
                                {
                                    Messages.ShowWarning("Recog Period must be first day of the month");
                                    return;
                                }
                                sDate = dtConvert.Month.ToString() + "-" + dtConvert.Day.ToString() + "-" + dtConvert.Year.ToString();
                                jobParms += "/A " + sDate.ToString() + " ";
                                totalParmsProvided = totalParmsProvided + 1;
                            }
                            else
                            {
                                jobParms += "/A " + cell.Value.ToString().TrimEnd() + " ";
                                totalParmsProvided = totalParmsProvided + 1;
                            }
                        }
                        else
                            if (jobName == "Aging with History")
                            {
                                jobParms += "/A 0 ";
                                totalParmsProvided = totalParmsProvided + 1;
                            }
                            else
                                if (jobName == "Billing Invoice Register")
                                {
                                    if (cell.Field.Name.ToString() == "from_due_date")
                                    {
                                        jobParms += "/A 1-1-1900 ";
                                        totalParmsProvided = totalParmsProvided + 1;
                                        FromDueDate = "N";
                                    }
                                    else
                                        if (cell.Field.Name.ToString() == "to_due_date")
                                        {
                                            jobParms += "/A 12-31-2099 ";
                                            totalParmsProvided = totalParmsProvided + 1;
                                            ToDueDate = "N";
                                        }
                                        else
                                            if (cell.Field.Name.ToString() == "from_date")
                                            {
                                                jobParms += "/A 1-1-1900 ";
                                                totalParmsProvided = totalParmsProvided + 1;
                                                FromAcctPeriod = "N";
                                            }
                                            else
                                                if (cell.Field.Name.ToString() == "to_date")
                                                {
                                                    jobParms += "/A 12-31-2099 ";
                                                    totalParmsProvided = totalParmsProvided + 1;
                                                    ToAcctPeriod = "N";
                                                }
                                }
                                else
                                {
                                    Messages.ShowWarning("All report parameters are required to be entered before submitting the report.");
                                    return;
                                }
                    }
                        

                        if (totalParmsProvided < totalParmsExpected)
                        {
                            Messages.ShowWarning("Please enter all parameters required.");
                            return;
                        }
                        if (jobName == "Billing Invoice Register")
                        {
                            if (((FromAcctPeriod == "Y") && (ToAcctPeriod == "Y")) || ((FromDueDate == "Y") && (ToDueDate == "Y")))
                            {
                                //Messages.ShowWarning("You must enter either Account Period Date range or Due Date range.");
                                //return;
                            }
                            else
                            {
                                Messages.ShowWarning("You must enter To and From Account Period Dates or To and From Due Dates.");
                                return;
                            }
                                
                        }
                    }
                 
            }
            

            DateTime dt = DateTime.Now;

            if (jobName.ToString() == " ")
                Messages.ShowWarning("No Report Selected to Run!");
            else
            {
                MessageBoxResult result = Messages.ShowYesNo("Schedule Report " + jobName.ToString() + " for " + dt,
                    System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, jobParms, dt, cGlobals.UserName.ToString()) == true)
                    {
                        Messages.ShowWarning("Report Scheduled to Run");
                        //need to clear the grid in gReportParms
                    }

                    else
                        Messages.ShowWarning("Error Scheduling Job");
                }
                else
                {

                    Messages.ShowMessage("Report Not Scheduled", MessageBoxImage.Information);
                }
            }



        }

        /// Handler for double click on grid to return the app selected to run
        public void ReturnSelectedData()
        {


        }

        public void PopRptParameters()
        {
             

            bool IsReportParmsLoaded = false;

            //Get report app id
            if (rptAppID != 0)
            {
               
                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.AddParm("@app_id", rptAppID);
                
                this.CurrentBusObj.DeleteAllTableRows("rptParms");
                this.Load(CurrentBusObj);
                //this.CurrentBusObj.LoadTable("rptParms");

                //if (rptParmBusObject.HasObjectData)
                //Reset totalParmsExpected
                totalParmsExpected = 0;
               if (this.CurrentBusObj.ObjectData.Tables["rptParms"].Rows.Count >= 0)
                 {

                    //add record for entry
                   
                    DataTable dtRptParms = this.CurrentBusObj.ObjectData.Tables["rptParms"];
                    string fieldName = this.CurrentBusObj.ObjectData.Tables["rptParms"].Columns[0].ToString();
                    //clear report parms grid
                    gReportParms.xGrid.DataSource = null;
                    gReportParms.xGrid.FieldLayouts.Clear();
                    //load the reports to view
                    gRptstoView.LoadGrid(this.CurrentBusObj, "rptView");
                    //if no columns then there will be a column called id               
                    if (fieldName == "id")
                    {
                        NoParmsInd = true;
                        Messages.ShowMessage("Report does not have any parameters.  Right mouse click to schedule the report", MessageBoxImage.Information);
                        IsReportParmsLoaded = true;
                        totalParmsExpected = 0;
                    }
                    else
                    {
                        FieldLayout rptParmsFieldLayout = new FieldLayout();
                        NoParmsInd = false;
                        DataRow newRow = dtRptParms.NewRow();
                        dtRptParms.Rows.Add(newRow);
                        //dtRptParms.AcceptChanges();
                        //clear the field layouts if there are any
                        if (gReportParms.xGrid.FieldLayouts.Count != 0)
                        {
                            gReportParms.xGrid.FieldLayouts.Clear();
                        }
                        int counter = 0;
                        //cbirney changed rptParmBusObject to this.CurrentBusObj
                        this.CurrentBusObj.ObjectData.Tables["rptParms"].Columns[counter].ToString();
                        totalParmsExpected = this.CurrentBusObj.ObjectData.Tables["rptParms"].Columns.Count;
                        //SET UP FIELD LAYOUTS FOR REPORT SELECTED
                        //ADJUSTMENT DETAIL BY MONTH REPORT = APP ID = 71
                        //ADJUSTMENT DETAIL BY PRODUCT REPORT = APP ID = 100394
                        //ADS INVOICE REGISTER REPORT = APP ID = 100499
                        //ADS PREBILL REPORT = APP ID = 100491
                        //AGING COMPANIES WITH HISTORY = APP ID = 100482
                        //AGING TOTALS BY PRODUCT = APP ID = 46
                        //AR CASH BY ACCT PERIODS = APP ID = 100453
                        //BILLING BATCH REPORT = APP ID = 50169
                        //BILLING VERIFICATION DOLLARS REPORT = APP ID = 100501
                        //BILLING VERIFICATION UNITS REPORT = APP ID = 100506
                        //CASH POSTED REPORT = APPID = 122
                        //CONTRACT EXPIRED REPORT = APPID = 100454
                        //CUSTOM INVOICE REGISTER = APPID = 100400
                        //CUSTOMER ACCT HISTORY TAB PRINT = APPID = 100484
                        //CUSTOMER AGING TAB PRINT = APPID = 100485
                        //DAILY CASH REPORT = APPID = 100447
                        //DEFERRED POOL AUDIT REPORT = APPID = 173
                        //DEFERRAL RECLASS REPORT = APPID = 100523
                        //FP & A REPORT = APPID = 100442
                        //GL TRANSACTION REPORT = APPID = 100483
                        //HOLD REPORT = APPID = 100500
                        //INVOICE OUT OF CONTRACT REPORT = APPID = 100462
                        //INVOICE REGISTER = APPID = 25
                        //LAUNCH DATES BY CUSTOMER = APPID = 100423
                        //MISSING RATES REPORT = APPID = 100475
                        //PRODUCT STATUS SUMMARY BY SYSTEM REPORT = 100231
                        //REVENUE BY STATE = APPID = 50168
                        //REVENUE PER UNIT = APPID = 100487
                        //SALES TAX BY INVOICE DATE = APPID = 100451
                        //SHIPMENT DATA DETAIL = APPID = 100486
                        //STATE SUMMARY REPORT = APPID = 170
                        //UNPOSTED BILLING REPORT = APPID = 100498
                        //UNPOSTED DETAIL BY MSO = APPID = 100450
                        //UNPOSTED SUMMARY BY MSO = APPID = 100449


                        //Revenue by Country Report
                        if (jobName == "Accounts With No Cash")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrAccountsNoCashRpt");

                        }


                         //ADJUSTMENT DETAIL BY MONTH REPORT
                        if (jobName == "Adjustment Detail By Month")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrAdjDetailbyMo");
                            //load combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyAllValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                            //load company combobox
                            //cmbCompany = new ComboBoxItemsProvider();
                            //cmbCompany.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            //cmbCompany.ValuePath = companyValuePath;
                            //cmbCompany.DisplayMemberPath = companyDisplayPath;
                        }

                        //ADJUSTMENT DETAIL BY PRODUCT REPORT
                        if (jobName == "Adjustment Detail By Product")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrAdjDetailbyProduct");
                            //load product combobox
                            cmbProductActive = new ComboBoxItemsProvider();
                            //cmbProductActive.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsActive"].DefaultView;
                            cmbProductActive.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductActive.ValuePath = productValuePath;
                            cmbProductActive.DisplayMemberPath = productDisplayPath;
                        }

                        //ADS INVOICE REGISTER
                        if (jobName == "Ads Invoice Register")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrAdsInvoiceRpt");
                        }

                        //ADS PREBILL REPORT
                        if (jobName == "Ads Prebill Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrAdsPrebillRpt");
                        }

                        //ADS REVENUE SHARE
                        if (jobName == "Ads Revenue Share Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrAdsRevShareRpt");
                        }

                        //AGING COMPANIES WITH HISTORY
                        if ((jobName == "Aging with History") || (jobName == "Aging with History - OLD"))
                        {

                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrAgingwithHist");
                            //load combobox
                            cmbCreditHist1 = new ComboBoxItemsProvider();
                            cmbCreditHist1.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCreditHist1"].DefaultView;
                            cmbCreditHist1.ValuePath = CreditHist1ValuePath;
                            cmbCreditHist1.DisplayMemberPath = CreditHist1DisplayPath;
                            //load combobox
                            cmbCreditHist2 = new ComboBoxItemsProvider();
                            cmbCreditHist2.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCreditHist2"].DefaultView;
                            cmbCreditHist2.ValuePath = CreditHist2ValuePath;
                            cmbCreditHist2.DisplayMemberPath = CreditHist2DisplayPath;
                            //load combobox
                            cmbCreditHist3 = new ComboBoxItemsProvider();
                            cmbCreditHist3.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCreditHist3"].DefaultView;
                            cmbCreditHist3.ValuePath = CreditHist3ValuePath;
                            cmbCreditHist3.DisplayMemberPath = CreditHist3DisplayPath;
                        }

                        //AGING BY PRODUCT REPORT
                        if (jobName == "Aging Totals by Product")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrAgingbyProductRpt");
                            //load combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyAllValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                            //cmbProductTypeAll = new ComboBoxItemsProvider();
                            //cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            //cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            //cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;
                        }

                        //ANCILLARY SUMMARY BY ACCT PERIOD REPORT
                        if (jobName == "Ancillary Summary Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrAncillarySummarybyAcctPeriodRpt");
                        }


                        //AR CASH BY ACCT PERIODS REPORT
                        if (jobName == "AR Cash by Acct Periods")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrARCashbyAcctPeriodRpt");
                        }

                        //Bad Debt REPORT
                        if (jobName == "Bad Debt Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrBadDebtRpt");
                        }

                        //Bad Debt REPORT
                        if (jobName == "Bad Debt Report All")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrBadDebtRpt");
                        }

                        //BILLING BATCH REPORT
                        if (jobName == "Billing Batch Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrBillingBatchRpt");
                        }

                           //BILLING BATCH REPORT
                        if (jobName == "Billing Reports Received Not Billed")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrBillingReportsRecievedNotBilled");

                            //load product combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;


                        }

                        //BILLING BATCH REPORT
                        if (jobName == "Billing Contract Review Dates")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptBillingContractReviewDates");

                            //load product combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;


                        }


                        //BILL PERIOD VARIANCE REPORT
                        if (jobName == "Bill Period Variance Report")
                        {
                            //RES 6/26/25 load products into grid instead of combobox so multiple rows can be selected     
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrBillPeriodVarRpt");
                            gReportParms.xGrid.FieldLayouts.Add(rptParmsFieldLayout);
                            gReportParms.LoadGrid(this.CurrentBusObj, "cmbProductsAll");

                            //rptParmsFieldLayout = (FieldLayout)FindResource("RptContractRateDetailByProduct");
                            //gReportParms.xGrid.FieldLayouts.Add(rptParmsFieldLayout);
                            //gReportParms.LoadGrid(this.CurrentBusObj, "cmbProductsWithContracts");

                            //load product combobox
                            //cmbProductAll = new ComboBoxItemsProvider();
                            //cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            //cmbProductAll.ValuePath = productValuePath;
                            //cmbProductAll.DisplayMemberPath = productDisplayPath;
                            ////load product type combobox                                                 
                            //cmbProductTypeAll = new ComboBoxItemsProvider();
                            //cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            //cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            //cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;
                            ////load company combobox
                            //cmbCompanyAll = new ComboBoxItemsProvider();
                            //cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            //cmbCompanyAll.ValuePath = companyValuePath;
                            //cmbCompanyAll.DisplayMemberPath = companyDisplayPath;
                        }

                        //BILLING INVOICE REGISTER
                        if (jobName == "Billing Invoice Register")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrBillingInvoiceRegisterRpt");
                            //load company combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyDisplayPath;
                            //load product combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;
                        }

                        //BILLING INVOICE REGISTER for Tax 10/10/18
                        if (jobName == "Billing Invoice Register for Tax")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrBillingInvoiceRegisterTaxRpt");
                            //load company combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyDisplayPath;
                            //load product combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;
                        }

                        
                        //BILLING VERIFICATION DOLLARS REPORT
                        if (jobName == "Billing Verification Dollars Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrBillingVerRpt");
                            //load combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyAllValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                        }

                        //BILLING VERIFICATION UNITS REPORT
                        if (jobName == "Billing Verification Units Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrBillingVerUnitRpt");
                            //load product type combobox
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;
                            //load combobox
                            cmbUnitMetaData = new ComboBoxItemsProvider();
                            cmbUnitMetaData.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbUnitMetaData"].DefaultView;
                            cmbUnitMetaData.ValuePath = metadataValuePath;
                            cmbUnitMetaData.DisplayMemberPath = metadataDisplayPath;
                        }

                        //Cash Posted by Product Item
                        if (jobName == "Cash Posted by Product Item Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrCashPostedByItemRpt");

                        }

                        //CASH POSTED REPORT
                        if (jobName == "Cash Posted Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrCashPostedRpt");
                            //RES 9/30/19 change company dropdown to dropdown with ALL option
                            //load company combobox
                            //cmbCompany = new ComboBoxItemsProvider();
                            //cmbCompany.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompany"].DefaultView;
                            //cmbCompany.ValuePath = companyValuePath;
                            //cmbCompany.DisplayMemberPath = companyDisplayPath;
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyAllValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                        }

                        //CONTRACT EXPIRED REPORT
                        if (jobName == "Contract Expired Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrContractExpiredRpt");
                            //load combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyAllValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                        }

                        //CONTRACT LOCATION REPORT
                        if (jobName == "Contract Location By Product Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrContractLocationRpt");
                            //load product combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;

                        }

                        //CONTRACT missing Customer Report 
                        if (jobName == "Contracts missing Customers Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrContractsmissingCustomersRpt");
                           

                        }

                        //CONTRACT Locations missing Customer Report 
                        if (jobName == "Contract Locations missing Customers Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrContractLocationsmissingCustomersRpt");


                        }



                        //CONTRACT RATE DETAIL BY PRODUCT
                        if (jobName == "Contract Rate Detail By Product")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptContractRateDetailByProduct");

                            ////load product combobox
                            //cmbProductAll = new ComboBoxItemsProvider();
                            //cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            //cmbProductAll.ValuePath = productValuePath;
                            //cmbProductAll.DisplayMemberPath = productDisplayPath;

                            //RES 4/28/15 load products into grid instead of combobox so multiple rows can be selected
                            gReportParms.xGrid.FieldLayouts.Add(rptParmsFieldLayout);
                            //gReportParms.LoadGrid(this.CurrentBusObj, "cmbProducts");
                            gReportParms.LoadGrid(this.CurrentBusObj, "cmbProductsWithContracts");


                        }

                        //CONTRACT prescheduled billing report 
                        if (jobName == "Contract Prescheduled Billing Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrContractPrescheduledBillingRpt");


                        }

                        //CONTRACT unfulfilled milestons report 
                        if (jobName == "Contracts Unfulfilled Milestones Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrContractUnfulfilledMilestonesRpt");


                        }

                        //CUSTOMER ACCT HISTORY TAB PRINT
                        if (jobName == "Customer Acct History Tab Print")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrCustHistoryTabRpt");
                        }

                        //CUSTOMER AGING TAB PRINT
                        if (jobName == "Customer Aging Tab Print")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrCustAgingTabRpt");
                            //load product combobox
                            cmbProduct = new ComboBoxItemsProvider();
                            cmbProduct.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProducts"].DefaultView;
                            cmbProduct.ValuePath = productValuePath;
                            cmbProduct.DisplayMemberPath = productDisplayPath;
                        }

                        //CUSTOMER HISTORY REPORT
                        if (jobName == "Customer History Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrCustHistRpt");                          
                        }

                        //CUSTOMER HISTORY WITH SUBS REPORT
                        if (jobName == "Customer History With Subs Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrCustHistSubsRpt");
                        }

                        //CUSTOMER RECONCILIATION REPORT
                        if (jobName == "Customer Reconciliation Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrCustReconRpt");
                            //load product combobox
                            cmbProductNoAds = new ComboBoxItemsProvider();
                            cmbProductNoAds.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsNoAds"].DefaultView;
                            cmbProductNoAds.ValuePath = productValuePath;
                            cmbProductNoAds.DisplayMemberPath = productDisplayPath;
                        }

                        //CUSTOM INVOICE REGISTER REPORT
                        if (jobName == "Custom Invoice Register")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrCustomInvRegRpt");
                            //load company combobox
                            //cmbCompany = new ComboBoxItemsProvider();
                            //cmbCompany.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompany"].DefaultView;
                            //cmbCompany.ValuePath = companyValuePath;
                            //cmbCompany.DisplayMemberPath = companyDisplayPath;
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;
                        }
                         //DAILY CASH REPORT
                        if (jobName == "Daily Cash Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrDailyCashRpt");
                        }

                        //DEFERRAL RECLASS REPORT
                        if (jobName == "Deferral Reclass Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrDefReclassRpt");
                        }

                        //DEFERRED POOL AUDIT REPORT
                        if (jobName == "Deferred Pool Audit Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrDefPoolRpt");
                            //load company combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyDisplayPath;
                        }

                        //EBIF Subscriber Royalty Detail
                        if (jobName == "EBIF Subscriber Royalty Detail Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrEbifSubDetailRpt");
                            //load combobox
                            cmbProdItemEbif = new ComboBoxItemsProvider();
                            cmbProdItemEbif.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProdItemEbif"].DefaultView;
                            cmbProdItemEbif.ValuePath = productItemEbifValuePath;
                            cmbProdItemEbif.DisplayMemberPath = productItemEbifDisplayPath;

                            //load combobox
                            cmbSubEbif = new ComboBoxItemsProvider();
                            cmbSubEbif.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbSubEbif"].DefaultView;
                            cmbSubEbif.ValuePath = subscriberTypeEbifValuePath;
                            cmbSubEbif.DisplayMemberPath = subscriberTypeEbifDisplayPath;

                          
                        }

                        //EBIF Subscriber Royalty Detail
                        if (jobName == "EBIF Subscriber Royalty Summary Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrEbifSubSummaryRpt");
                            //load combobox
                            cmbProdItemEbif = new ComboBoxItemsProvider();
                            cmbProdItemEbif.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProdItemEbif"].DefaultView;
                            cmbProdItemEbif.ValuePath = productItemEbifValuePath;
                            cmbProdItemEbif.DisplayMemberPath = productItemEbifDisplayPath;

                            //load combobox
                            cmbSubEbif = new ComboBoxItemsProvider();
                            cmbSubEbif.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbSubEbif"].DefaultView;
                            cmbSubEbif.ValuePath = subscriberTypeEbifValuePath;
                            cmbSubEbif.DisplayMemberPath = subscriberTypeEbifDisplayPath;


                        }


                        //ENTITY MCA ADDRESS REPORT
                        if (jobName == "Entity MCA Address Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrEntityMCAAddrRpt");
                            //load product combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;

                        }

                      
                        //FP&A REPORT
                        if (jobName == "FP&A Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrFPandARpt");
                            //load product type combobox
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;
                          
                        }

                        //RES 8/3/16 FP&A REPORT with Ads Prod Description
                        if (jobName == "FP&A Report with Prod Desc")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrFPandARpt");
                            //load product type combobox
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;

                        }

                        //RES 6/21/18 FP&A REPORT with Service Period
                        if (jobName == "FP&A Report with Service Period")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrFPandARpt");
                            //load product type combobox
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;

                        }

                        //Revenue per FP&A Units Report
                        if (jobName == "FP&A Units Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrFPandAUnitRpt");
                            //load product combobox
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;
                                                      
                        }



                        //GL Transaction REPORT
                        if (jobName == "GL Transaction Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrGLTransactionRpt");
                            //load combobox
                            //cmbCompanyAll = new ComboBoxItemsProvider();
                            //cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            //cmbCompanyAll.ValuePath = companyAllValuePath;
                            //cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;

                        }

                        //RES 4/7/22 GL Deferred Transaction REPORT
                        if (jobName == "GL Deferred Transaction Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrGLDefTransactionRpt");
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;

                        }

                        //HOLD REPORT
                        if (jobName == "Hold Report")
                        {
                            //setup cbo layout
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrHoldRpt");
                           
                            
                            //load combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;
                            //load combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyAllValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                        }

                        //Invoice Out of Contract Report
                        if (jobName == "Invoice Out of Contract Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrOutofContractRpt");
                            //load product type combobox
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;
                        }
                       
                        //Invoice Register
                        if (jobName == "Transaction Register")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrInvoiceRegisterRpt");
                           
                           
                        }

                        //Last Invoice by Location 
                        if (jobName == "Last Invoice by Location Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrLastInvoicebyLocation");
                            //load product combobox
                            cmbProduct = new ComboBoxItemsProvider();
                            cmbProduct.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProducts"].DefaultView;
                            cmbProduct.ValuePath = productValuePath;
                            cmbProduct.DisplayMemberPath = productDisplayPath;
                        }

                         //Launch Dates by Customer
                        if (jobName == "Launch Dates by Customer")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrLaunchbyCustRpt");
                        }

                        //Location Rule Override
                        if (jobName == "Location Rule Override Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrLocRuleOverrideRpt");
                        }

                        //Missing Rates Report
                        if (jobName == "Missing Rates Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrMissingRatesRpt");
                        }



                        //Revenue per UnitReport
                        if (jobName == "Models Billed Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrModelsBilledRpt");

                            //load combobox

                            
                            cmbProduct = new ComboBoxItemsProvider();
                            cmbProduct.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProducts"].DefaultView;
                            cmbProduct.ValuePath = productValuePath;
                            cmbProduct.DisplayMemberPath = productDisplayPath;

                            //load combobox
                            cmbModelDescAll = new ComboBoxItemsProvider();
                            cmbModelDescAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbModelDescAll"].DefaultView;
                            cmbModelDescAll.ValuePath = modelAllValuePath;
                            cmbModelDescAll.DisplayMemberPath = modelAllDisplayPath;

                          
                        }

                        //MULTIPLE RENEWAL REPORT
                        if (jobName == "Multiperiod Renewal Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrMultiRenewalRpt");
                            //load combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyAllValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                            //load product combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;

                        }


                        //OPS GLOBAL RECON REPORT
                        if (jobName == "Ops Global Recon Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrOpsGlobalRpt");
                            //load combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyAllValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                            //load product combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;

                        }

                        if (jobName == "Qtrly Product Items by Company Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrItemsByCoRpt");
                            //load company combobox
                            //cmbCompany = new ComboBoxItemsProvider();
                            //cmbCompany.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompany"].DefaultView;
                            //cmbCompany.ValuePath = companyValuePath;
                            //cmbCompany.DisplayMemberPath = companyDisplayPath;
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductType"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;
                        }
                        //Outbound Royalty Report
                       
                     

                        //Product Status Summary by System
                        if (jobName == "Product Status Summary by Location")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrProductStatRpt");
                           
                            //load MSO Typecombobox
                            cmbMSOType = new ComboBoxItemsProvider();
                            cmbMSOType.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbMSOType"].DefaultView;
                            cmbMSOType.ValuePath = MSOTypeValuePath;
                            cmbMSOType.DisplayMemberPath = MSOTypeDisplayPath;
                            //load MSO combobox
                            cmbMSO = new ComboBoxItemsProvider();
                            cmbMSO.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbMSO"].DefaultView;
                            cmbMSO.ValuePath = MSOValuePath;
                            cmbMSO.DisplayMemberPath = MSODisplayPath;
                        }

                        //RECOGNIZED DEFERRED REVENUE REPORT
                        if (jobName == "Recognized Deferred Revenue Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrRecDefRevRpt");
                        }

                        //Rejected Adjustments Report
                        if (jobName == "Rejected Adjustments")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrRejectedAdjustments");

                        }
                        //Research Report
                        if (jobName == "Research Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrResearchRpt");

                            //load product combobox
                            cmbProductAll = new ComboBoxItemsProvider();
                            cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            cmbProductAll.ValuePath = productValuePath;
                            cmbProductAll.DisplayMemberPath = productDisplayPath;
                            //load company combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyAllValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                        }


                         //Revenue by Country Report
                        if (jobName == "Revenue By Country (Tax report)")

                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrRevByCountryRpt");
                           
                        }

                        //Revenue by State Report
                        if (jobName == "Revenue By State (Year End Tax Reporting)")

                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrRevbyStateRpt");
                            //load company combobox
                            //cmbCompany = new ComboBoxItemsProvider();
                            //cmbCompany.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            //cmbCompany.ValuePath = companyValuePath;
                            //cmbCompany.DisplayMemberPath = companyDisplayPath;
                            cmbProductType = new ComboBoxItemsProvider();
                            cmbProductType.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductType"].DefaultView;
                            cmbProductType.ValuePath = productTypeAllValuePath;
                            cmbProductType.DisplayMemberPath = productTypeAllDisplayPath;
                        }


                        

                        //Revenue per UnitReport
                        if (jobName == "Revenue Per Unit")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrRevperUnitRpt");
                            //load company combobox
                            cmbCompanyAll = new ComboBoxItemsProvider();
                            cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            cmbCompanyAll.ValuePath = companyValuePath;
                            cmbCompanyAll.DisplayMemberPath = companyDisplayPath;
                        }


                        //Sales Billing Rates
                        if (jobName == "Sales Billing Rates")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrSalesBillingRates");
                        }

                        //Sales Billing Rates
                        if (jobName == "PTMOBI Revenue Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrPTMOBIRevenue");
                        }


                        //Sales Tax by Invoice Date
                        if (jobName == "Sales Tax by Invoice Date")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrSalesTaxbyInvDtRpt");
                        }

                        //Shipment Data Detail
                        if (jobName == "Shipment Data Detail")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrShipmentDataRpt");
                        }

                        //State Summary Report
                        if (jobName == "State Summary Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrStateSmryRpt");
                            //load product combobox
                            cmbProduct = new ComboBoxItemsProvider();
                            cmbProduct.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProducts"].DefaultView;
                            cmbProduct.ValuePath = productValuePath;
                            cmbProduct.DisplayMemberPath = productDisplayPath;
                        }

                        //Subscriber Last Updated Report REPORT
                        if (jobName == "Subscriber Last Updated Report")
                        {
                            //setup cbo layout
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrSubLastUpdatedRpt");


                            //load combobox
                            //cmbProductAll = new ComboBoxItemsProvider();
                            //cmbProductAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductsAll"].DefaultView;
                            //cmbProductAll.ValuePath = productValuePath;
                            //cmbProductAll.DisplayMemberPath = productDisplayPath;
                            //load combobox
                            //cmbCompanyAll = new ComboBoxItemsProvider();
                            //cmbCompanyAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbCompanyAll"].DefaultView;
                            //cmbCompanyAll.ValuePath = companyAllValuePath;
                            //cmbCompanyAll.DisplayMemberPath = companyAllDisplayPath;
                            //load combobox
                            cmbProductTypeAll = new ComboBoxItemsProvider();
                            cmbProductTypeAll.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductTypeAll"].DefaultView;
                            cmbProductTypeAll.ValuePath = productTypeAllValuePath;
                            cmbProductTypeAll.DisplayMemberPath = productTypeAllDisplayPath;
                        }

                        //Subscriber By Product Group REPORT
                        if (jobName == "Subscriber Report By Product Group")
                        {
                            //setup cbo layout
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrSubsByProductGrpRpt");
                                                    
                            //load combobox
                            cmbProductGroup = new ComboBoxItemsProvider();
                            cmbProductGroup.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbProductGroup"].DefaultView;
                            cmbProductGroup.ValuePath = productGroupValuePath;
                            cmbProductGroup.DisplayMemberPath = productGroupDisplayPath;
                        }

                        //Unposted Billing Report
                        if (jobName == "Unposted Billing Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrUnpostedBillingRpt");
                        }

                        //Unposted Detail by MSO
                        if (jobName == "Unposted Detail by MSO")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrUnpostedDetailbyMSORpt");
                            //load MSO combobox
                            cmbMSO = new ComboBoxItemsProvider();
                            cmbMSO.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbMSO"].DefaultView;
                            cmbMSO.ValuePath = MSOValuePath;
                            cmbMSO.DisplayMemberPath = MSODisplayPath;
                        }

                        //Unposted Summary by MSO
                        if (jobName == "Unposted Summary by MSO")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrUnpostedSmrybyMSORpt");
                            //load MSO combobox
                            cmbMSO = new ComboBoxItemsProvider();
                            cmbMSO.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cmbMSO"].DefaultView;
                            cmbMSO.ValuePath = MSOValuePath;
                            cmbMSO.DisplayMemberPath = MSODisplayPath;
                        }

                        //Withholding Tax Report
                        if (jobName == "WHT Adjustment Report")
                        {
                            rptParmsFieldLayout = (FieldLayout)FindResource("RptMgrWHTAdjustmentRpt");

                        }
                        if ((jobName != "Contract Rate Detail By Product") && (jobName != "Bill Period Variance Report"))
                        //if (jobName != "Contract Rate Detail By Product")
                        {
                            gReportParms.xGrid.FieldLayouts.Add(rptParmsFieldLayout);
                            //cbirney changed rptParmBusObject to this.CurrentBusObj
                            gReportParms.LoadGrid(this.CurrentBusObj, "rptParms");
                            //IsReportParmsLoaded = true;
                        }
                        IsReportParmsLoaded = true;
                    }

                    if (!IsReportParmsLoaded)
                    {
                        Messages.ShowInformation("Unable to load report parms.");
                    }
                }
            }
        }

        

        private void GridRptsAvailSingleClickDelegate()
        {
            gReportsAvailtoRun.ReturnSelectedData(dataKey2);
            rptAppID = Convert.ToInt32(cGlobals.ReturnParms[0]);

            if (rptParms == null)
                Messages.ShowWarning("Invalid report selected to run!  Validate the parameter table is setup.");
            else
            {
                //gReportsAvailtoRun.ReturnSelectedData(dataKey2);
                //rptAppID = Convert.ToInt32(cGlobals.ReturnParms[0]);
                cGlobals.ReturnParms.Clear();
                gReportsAvailtoRun.ReturnSelectedData(dataKey3);
                jobName = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                PopRptParameters();
            }
        }

        private void GridDoubleClickDelegateView()
        {
            //View the reports
            DataRecord record = gRptstoView.xGrid.ActiveRecord as DataRecord;

            if (record != null)
            {
                string serverLocation = record.Cells[server].Value.ToString();
                string fileName = record.Cells[file].Value.ToString();
                string folder = record.Cells[directory].Value.ToString();

                StringBuilder pathBuilder = new StringBuilder(serverLocation);
                pathBuilder.Append(folder);
                pathBuilder.Append(@"\");
                pathBuilder.Append(fileName);
                if (File.Exists(pathBuilder.ToString()))

                    Process.Start(pathBuilder.ToString());
                else
                    MessageBox.Show("File does not exist");
            }

        }

        
        public override void Save()
        {
            //need to see if gRptstoView was modified
            this.Prep_ucBaseGridsForSave();
            if (IsScreenDirty)
            {
    
                base.Save();
                if (SaveSuccessful)
                {
                    Messages.ShowInformation("Save Successful");
                    //this.Load(CurrentBusObj);
                    //if (this.CurrentBusObj.HasObjectData)
                    //{
                    //    gReportsAvailtoRun.LoadGrid(this.CurrentBusObj, gReportsAvailtoRun.MainTableName);
                    //}
                    //else
                    //{
                    //    Messages.ShowInformation("Unable to load available reports.");
                    //}

                    //System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
                }
                else
                {
                    Messages.ShowInformation("Save Failed");
                }
            }
        }
        public void GridDoubleClickDelegate()
        {

        }



        public void gReportConfigs_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void gReportParms_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void gRptstoView_LostFocus(object sender, RoutedEventArgs e)
        {
            //Check to see if the grid was modified and if so, ask if they want to save
            if (IsScreenDirty)
            {

                var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                {
                    this.Save();
                    //this.CurrentBusObj.LoadTable("rptView");
                }
                else
                {
                    this.CurrentBusObj.ObjectData.Tables["rptView"].Clear();
                    this.CurrentBusObj.LoadTable("rptView");
                    return;
                }
            }
          }
            
         

        

    }
}