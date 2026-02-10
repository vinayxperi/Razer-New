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
using RazerInterface;
using Infragistics.Windows.Editors;
using System.Data;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.DataPresenter.Events;

namespace Razer.CustomInvoice
{
    /// <summary>
    /// Interaction logic for CustomInvoiceDetail.xaml
    /// </summary>
    public partial class CustomInvoiceDetail : ScreenBase, IPreBindable
    {
        private static readonly string productValueField = "product_code";        
        private static readonly string companyCodeField = "gl_co";
        private static readonly string companyCodeParameter = "company_code";
        private static readonly string itemProductValueField = "item_code";
        private static readonly string itemProductDisplayField = "combo_description";
        private static readonly string revenueRuleValueField = "rule_id";
        private static readonly string revenueRuleDisplayField = "rule_desc";
        private static readonly string revenueTypeValueField = "revenue_type_id";
        private static readonly string revenueTypeDisplayField = "revenue_description";
        private static readonly string revenueStatusDisplayField = "code_value";
        private static readonly string revenueStatusValueField = "fkey_int";
        private static readonly string salesPersonValueField = "contact_id";
        private static readonly string salesPersonDisplayField = "name";
        private static readonly string CLMNumberValueField = "clm_id";
        private static readonly string CLMNumberDisplayField = "clm_number";
        private static readonly string DealTypeValueField = "fkey_int";
        private static readonly string DealTypeDisplayField = "code_value";
        private static readonly string productsTableName = "products_company";
        private static readonly string itemProductsTableName = "item_products";
        private static readonly string itemDateField = "item_date";
        private static readonly string itemDateEndField = "item_date_end";
        private static readonly string extendedAmountField = "extended";
        private static readonly string invoiceDescriptionField = "inv_line_desc";
        private static readonly string descriptionField = "description";        
        private static readonly string rateField = "rate";
        private static readonly string idField = "sequence_code";
        private static readonly string accountCodeField = "account_code";
        private static readonly string arAccountField = "ar_account";
        private static readonly string unitsField = "units";
        private static readonly string invoiceNumberField = "invoice_number";
        private static readonly string percentCompleteFlagField = "percent_complete_flag";        
        private static readonly string accountDetailIdField = "acct_detail_id";
        private static readonly string revenueRuleTableName = "deferred_revenue_rules";
        private static readonly string revenueTypeTableName = "revenue_type";
        private static readonly string revenueStatusTableName = "revenue_status";
        private static readonly string salesPersonTableName = "salesperson";
        private static readonly string CLMNumberTableName = "clm_lookup";
        private static readonly string DealTypeTableName = "dddwdealtype";
        private static readonly string miscInvoiceAcctDetailObject = "MiscInvAcctDetail";
        private static readonly string miscInvoiceAcctDetailTableName = "misc_inv_acct_detail";
        private static readonly string detailTableName = "detail";
        private static readonly string accountDetailTableName = "acct_detail";
        private static readonly string generalTableName = "general";
        private static readonly string detailFieldLayouts = "CustomInvoiceDetailList";
        private static readonly string acctDetailFieldLayouts = "CustomInvoiceRevenueAllocationList";
        private static readonly string refreshOnlyFlagField = "refresh_only_flag";

        public string selectedCompany = string.Empty;
        public string invoiceNumber = string.Empty;

        public int ContractID;
        
        public ComboBoxItemsProvider cmbProductDescription { get; set; }
        public ComboBoxItemsProvider cmbRevenueRule { get; set; }
        public ComboBoxItemsProvider cmbRevenueType { get; set; }
        public ComboBoxItemsProvider cmbRevenueStatus { get; set; }
        public ComboBoxItemsProvider cmbSalesPerson { get; set; }
        public ComboBoxItemsProvider cmbCLMNumber { get; set; }
        public ComboBoxItemsProvider cmbDealType { get; set; }

        decimal totalAmount, discountAmount, netAmount;
        int maxRowId;

        DataTable filteredProducts;
        DataTable filteredProductItems;
        DataTable detailRecords;
        DataTable accountDetailRecords;
        DataTable filteredCLMData;
        DataSet data;
        DataTable filteredProductItemsSorted;

        //string ActiveFlag = "0";
        
        cBaseBusObject accountDetailObject;        

        public DataTable DetailRecords
        {
            get { return detailRecords; }
            set 
            { 
                detailRecords = value;                
                LoadDetailGrid();
            }
        }

        public DataTable AccountDetailRecords
        {
            get { return accountDetailRecords; }
            set 
            { 
                accountDetailRecords = value;
                data.Tables.Add(accountDetailRecords);
            }
        }


        public CustomInvoiceDetail()
        {
            InitializeComponent();

            this.MainTableName = generalTableName;

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            layouts.AddNewRecordLocation = AddNewRecordLocation.OnTop;
            idgDetails.xGrid.FieldLayoutSettings = layouts;
            idgDetails.FieldLayoutResourceString = detailFieldLayouts;                     
            idgDetails.SetGridSelectionBehavior(false, false);
            idgDetails.xGrid.FieldSettings.AllowEdit = true;
            idgDetails.xGrid.FieldLayoutSettings.AllowDelete = false;

            idgDetails.xGrid.DataContext = detailRecords;

            idgDetails.xGrid.CellUpdated += new EventHandler<CellUpdatedEventArgs>(xGrid_CellUpdated);                        
            idgDetails.xGrid.SelectedItemsChanged += new EventHandler<SelectedItemsChangedEventArgs>(xGrid_SelectedItemsChanged);
            idgDetails.xGrid.UpdateMode = UpdateMode.OnCellChange;

            if (SecurityContext == AccessLevel.ViewOnly)
                idgDetails.ContextMenuAddIsVisible = false;
            else
            {
                idgDetails.ContextMenuAddDelegate = AddDetailRecord;
                idgDetails.ContextMenuAddDisplayName = "Add Detail Record";
                idgDetails.ContextMenuAddIsVisible = true;
            }

            /* RES 1/29/21 add menu option to display revenue recognition schedule  */
            idgDetails.ContextMenuGenericDelegate1 = GridInvoiceDetailRevRecDelegate;
            idgDetails.ContextMenuGenericDisplayName1 = "Show Revenue Recognition";
            idgDetails.ContextMenuGenericIsVisible1 = true;

            FieldLayoutSettings revAllocationLayouts = new FieldLayoutSettings();
            revAllocationLayouts.HighlightAlternateRecords = true;



            idgRevenueAllocation.xGrid.FieldLayoutSettings = revAllocationLayouts;
            idgRevenueAllocation.FieldLayoutResourceString = acctDetailFieldLayouts;
            idgRevenueAllocation.xGrid.FieldLayoutSettings.AllowDelete = false;

            /* RES 1/29/21 add menu option to display revenue recognition schedule  */
            idgRevenueAllocation.ContextMenuGenericDelegate1 = GridInvoiceDetailRevRecDelegate;
            idgRevenueAllocation.ContextMenuGenericDisplayName1 = "Show Revenue Recognition";
            idgRevenueAllocation.ContextMenuGenericIsVisible1 = true;
            
            txtDiscountAmount.LostFocus += new RoutedEventHandler(txtDiscountAmount_LostFocus);
            
        }

        public void OnCompanyCodeChanged(string companyCode)
        {
            if (!string.IsNullOrEmpty(companyCode))
            {
                if (CurrentBusObj != null && CurrentBusObj.HasObjectData)
                {
                    var rows = (from r in CurrentBusObj.ObjectData.Tables[productsTableName].AsEnumerable()
                                where r.Field<string>(companyCodeField) == companyCode
                                select r).ToList();

                    if (rows != null && rows.Count > 0)
                    {
                        filteredProducts = CurrentBusObj.ObjectData.Tables[productsTableName].Clone();
                        foreach (DataRow row in rows)
                        {
                            DataRow filteredRow = filteredProducts.NewRow();
                            foreach (DataColumn column in row.Table.Columns)
                            {
                                filteredRow[column.ColumnName] = row[column.ColumnName];
                            }
                            filteredProducts.Rows.Add(filteredRow);
                        }                        
                    }

                    filteredProductItems = CurrentBusObj.ObjectData.Tables[itemProductsTableName].Clone();
                    filteredProductItemsSorted = CurrentBusObj.ObjectData.Tables[itemProductsTableName].Clone();

                    if (filteredProducts != null && filteredProducts.Rows.Count > 0)
                    {
                        foreach (DataRow product in filteredProducts.Rows)
                        {
                            var items = (from r in CurrentBusObj.ObjectData.Tables[itemProductsTableName].AsEnumerable()
                                         where r.Field<string>(productValueField) == product[productValueField].ToString()
                                         select r).ToList();

                            if (items != null && items.Count > 0)
                            {
                                foreach (DataRow item in items)
                                {
                                    DataRow filteredRow = filteredProductItems.NewRow();
                                    foreach (DataColumn column in item.Table.Columns)
                                    {
                                        filteredRow[column.ColumnName] = item[column.ColumnName];
                                    }
                                    filteredProductItems.Rows.Add(filteredRow);
                                }
                            }                            
                        }
                        /* RES 5/29/20 sort inactive custom invoice items to the bottom of the dropdown  */
                        if (filteredProductItems != null && filteredProductItems.Rows.Count > 0)
                        {
                            var rows2 = (from r in filteredProductItems.AsEnumerable()
                                         where r.Field<Int32>("inactive_flag") == 0
                                         select r).ToList();

                            if (rows2 != null && rows2.Count > 0)
                            {       
                                foreach (DataRow row in rows2)
                                {
                                    DataRow filteredRow = filteredProductItemsSorted.NewRow();
                                    foreach (DataColumn column in row.Table.Columns)
                                    {
                                        filteredRow[column.ColumnName] = row[column.ColumnName];
                                    }
                                    filteredProductItemsSorted.Rows.Add(filteredRow);
                                }
                            }
                            var rows3 = (from r in filteredProductItems.AsEnumerable()
                                         where r.Field<Int32>("inactive_flag") == 1
                                         select r).ToList();

                            if (rows3 != null && rows2.Count > 0)
                            {
                                //filteredProducts = CurrentBusObj.ObjectData.Tables[productsTableName].Clone();
                                foreach (DataRow row in rows3)
                                {
                                    DataRow filteredRow = filteredProductItemsSorted.NewRow();
                                    foreach (DataColumn column in row.Table.Columns)
                                    {
                                        filteredRow[column.ColumnName] = row[column.ColumnName];
                                    }
                                    filteredProductItemsSorted.Rows.Add(filteredRow);
                                }
                            }
                        }                       
                    }



                    cmbProductDescription = new ComboBoxItemsProvider();
                    cmbProductDescription.ItemsSource = filteredProductItemsSorted.DefaultView;
                    cmbProductDescription.ValuePath = itemProductValueField;
                    cmbProductDescription.DisplayMemberPath = itemProductDisplayField;

                    if (filteredProductItems.Rows.Count == 0)
                    {
                        Messages.ShowInformation(string.Format("No product item detail exists for company code {0}", companyCode));
                    }
                }
                
                selectedCompany = companyCode;
                discountAmount = 0;
                totalAmount = 0;
                netAmount = 0;
                txtNetAmount.Text = netAmount.ToString("C2");
                txtTotalAmount.Text = totalAmount.ToString("C2");
                txtDiscountAmount.Text = discountAmount.ToString("C2");


            }
        }

        public void LoadCompanyCodeComboFirst(string companyCode)
        {
            if (!string.IsNullOrEmpty(companyCode))
            {
                if (CurrentBusObj != null && CurrentBusObj.HasObjectData)
                {
                    var rows = (from r in CurrentBusObj.ObjectData.Tables[productsTableName].AsEnumerable()
                                where r.Field<string>(companyCodeField) == companyCode
                                select r).ToList();

                    if (rows != null && rows.Count > 0)
                    {
                        filteredProducts = CurrentBusObj.ObjectData.Tables[productsTableName].Clone();
                        foreach (DataRow row in rows)
                        {
                            DataRow filteredRow = filteredProducts.NewRow();
                            foreach (DataColumn column in row.Table.Columns)
                            {
                                filteredRow[column.ColumnName] = row[column.ColumnName];
                            }
                            filteredProducts.Rows.Add(filteredRow);
                        }
                    }

                    filteredProductItems = CurrentBusObj.ObjectData.Tables[itemProductsTableName].Clone();
                    filteredProductItemsSorted = CurrentBusObj.ObjectData.Tables[itemProductsTableName].Clone();

                    if (filteredProducts != null && filteredProducts.Rows.Count > 0)
                    {
                        foreach (DataRow product in filteredProducts.Rows)
                        {
                            var items = (from r in CurrentBusObj.ObjectData.Tables[itemProductsTableName].AsEnumerable()
                                         where r.Field<string>(productValueField) == product[productValueField].ToString()
                                         select r).ToList();

                            if (items != null && items.Count > 0)
                            {
                                foreach (DataRow item in items)
                                {
                                    DataRow filteredRow = filteredProductItems.NewRow();
                                    foreach (DataColumn column in item.Table.Columns)
                                    {
                                        filteredRow[column.ColumnName] = item[column.ColumnName];
                                    }
                                    filteredProductItems.Rows.Add(filteredRow);
                                }
                            }
                        }
                        /* RES 5/29/20 sort inactive custom invoice items to the bottom of the dropdown  */
                        if (filteredProductItems != null && filteredProductItems.Rows.Count > 0)
                        {
                            var rows2 = (from r in filteredProductItems.AsEnumerable()
                                         where r.Field<Int32>("inactive_flag") == 0
                                         select r).ToList();

                            if (rows2 != null && rows2.Count > 0)
                            {
                                foreach (DataRow row in rows2)
                                {
                                    DataRow filteredRow = filteredProductItemsSorted.NewRow();
                                    foreach (DataColumn column in row.Table.Columns)
                                    {
                                        filteredRow[column.ColumnName] = row[column.ColumnName];
                                    }
                                    filteredProductItemsSorted.Rows.Add(filteredRow);
                                }
                            }
                            var rows3 = (from r in filteredProductItems.AsEnumerable()
                                         where r.Field<Int32>("inactive_flag") == 1
                                         select r).ToList();

                            if (rows3 != null && rows2.Count > 0)
                            {
                                //filteredProducts = CurrentBusObj.ObjectData.Tables[productsTableName].Clone();
                                foreach (DataRow row in rows3)
                                {
                                    DataRow filteredRow = filteredProductItemsSorted.NewRow();
                                    foreach (DataColumn column in row.Table.Columns)
                                    {
                                        filteredRow[column.ColumnName] = row[column.ColumnName];
                                    }
                                    filteredProductItemsSorted.Rows.Add(filteredRow);
                                }
                            }
                        }              
                    }



                    cmbProductDescription = new ComboBoxItemsProvider();
                    cmbProductDescription.ItemsSource = filteredProductItemsSorted.DefaultView;
                    cmbProductDescription.ValuePath = itemProductValueField;
                    cmbProductDescription.DisplayMemberPath = itemProductDisplayField;

                    if (filteredProductItems.Rows.Count == 0)
                    {
                        Messages.ShowInformation(string.Format("No product item detail exists for company code {0}", companyCode));
                    }
                }

                selectedCompany = companyCode;
                //discountAmount = 0;
                //totalAmount = 0;
                //netAmount = 0;
                //txtNetAmount.Text = netAmount.ToString("C2");
                //txtTotalAmount.Text = totalAmount.ToString("C2");
                //txtDiscountAmount.Text = discountAmount.ToString("C2");


            }
        }



        private void LoadDetailGrid()
        {            
            data = new DataSet();
            data.Tables.Add(detailRecords);                        
            idgDetails.xGrid.DataSource = detailRecords.DefaultView;

            //if (Convert.ToInt32(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"]) != 0)
            //{
            //    //this.CurrentBusObj.Parms.AddParm("@external_int_id",CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"].ToString());
            //    this.CurrentBusObj.Parms.AddParm("@contract_id", CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"].ToString());
            //}
            //else
            //{
            //    //this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
            //    this.CurrentBusObj.Parms.AddParm("@contract_id", "0");
            //}
        }

        public void PreBind()
        {
            if (this.CurrentBusObj.HasObjectData)
            {
                OnCompanyCodeChanged(selectedCompany);                

                cmbRevenueRule = new ComboBoxItemsProvider();
                cmbRevenueRule.ItemsSource = this.CurrentBusObj.ObjectData.Tables[revenueRuleTableName].DefaultView;
                cmbRevenueRule.ValuePath = revenueRuleValueField;
                cmbRevenueRule.DisplayMemberPath = revenueRuleDisplayField;

                cmbRevenueStatus = new ComboBoxItemsProvider();
                cmbRevenueStatus.ItemsSource = this.CurrentBusObj.ObjectData.Tables[revenueStatusTableName].DefaultView;
                cmbRevenueStatus.ValuePath = revenueStatusValueField;
                cmbRevenueStatus.DisplayMemberPath = revenueStatusDisplayField;

                cmbRevenueType = new ComboBoxItemsProvider();
                cmbRevenueType.ItemsSource = this.CurrentBusObj.ObjectData.Tables[revenueTypeTableName].DefaultView;
                cmbRevenueType.ValuePath = revenueTypeValueField;
                cmbRevenueType.DisplayMemberPath = revenueTypeDisplayField;

                cmbSalesPerson = new ComboBoxItemsProvider();
                cmbSalesPerson.ItemsSource = this.CurrentBusObj.ObjectData.Tables[salesPersonTableName].DefaultView;
                cmbSalesPerson.ValuePath = salesPersonValueField;
                cmbSalesPerson.DisplayMemberPath = salesPersonDisplayField;

                //RES 4/19/16 Add CLM Number
                if (CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                    if (Convert.ToInt32(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"]) != 0)
                    {
                        this.CurrentBusObj.Parms.UpdateParmValue("@external_int_id", CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"].ToString());
                        this.CurrentBusObj.Parms.UpdateParmValue("@contract_number", CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"].ToString());
                        idgDetails.xGrid.FieldLayouts[0].Fields["clm_id"].Visibility = Visibility.Visible;
                        idgDetails.xGrid.FieldLayouts[0].Fields["clm_freeform"].Visibility = Visibility.Collapsed;
                        idgDetails.xGrid.FieldLayouts[0].Fields["deal_type"].Settings.AllowEdit = false;
                        idgDetails.xGrid.FieldLayouts[0].Fields["contract_executed_date"].Settings.AllowEdit = false;
                    }
                    else
                    {
                        if (this.CurrentBusObj.Parms.GetParm("@contract_number") == null)
                        {
                            this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
                            this.CurrentBusObj.Parms.AddParm("@contract_number", "0");
                        }
                        idgDetails.xGrid.FieldLayouts[0].Fields["clm_id"].Visibility = Visibility.Collapsed;
                        idgDetails.xGrid.FieldLayouts[0].Fields["clm_freeform"].Visibility = Visibility.Visible;
                        //string wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                        if ((Convert.ToInt32(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["posted_flag"]) == 0) &&
                            (CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"].ToString() == "N"))
                        {
                            idgDetails.xGrid.FieldLayouts[0].Fields["deal_type"].Settings.AllowEdit = true;
                            idgDetails.xGrid.FieldLayouts[0].Fields["contract_executed_date"].Settings.AllowEdit = true;
                        }
                    }
                this.CurrentBusObj.UpdateParms();
                this.CurrentBusObj.LoadTable("clm_lookup");
                cmbCLMNumber = new ComboBoxItemsProvider();
                cmbCLMNumber.ItemsSource = this.CurrentBusObj.ObjectData.Tables[CLMNumberTableName].DefaultView;
                cmbCLMNumber.ValuePath = CLMNumberValueField;
                cmbCLMNumber.DisplayMemberPath = CLMNumberDisplayField;
                this.CurrentBusObj.LoadTable("dddwdealtype");
                cmbDealType = new ComboBoxItemsProvider();
                cmbDealType.ItemsSource = this.CurrentBusObj.ObjectData.Tables[DealTypeTableName].DefaultView;
                cmbDealType.ValuePath = DealTypeValueField;
                cmbDealType.DisplayMemberPath = DealTypeDisplayField;
                  
                DataTable totals = CurrentBusObj.ObjectData.Tables[generalTableName];
                this.DataContext = totals;                
            }
        }

        void txtDiscountAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            discountAmount = ParseToDecimal(txtDiscountAmount.Text.Remove(0,1));
            totalAmount = ParseToDecimal(txtTotalAmount.Text.Remove(0,1));
            netAmount = totalAmount - discountAmount;
            txtNetAmount.Text = netAmount.ToString("C2");

           
        }


        void AddDetailRecord()
        {
            
            if (string.IsNullOrEmpty(selectedCompany))
            {
                Messages.ShowInformation("You must select a company prior to adding records.");
                
                return;
            }

            if (detailRecords.Rows.Count > 0)
            {
                string maxQuery = string.Format("MAX({0})", idField);
                var maxRow = detailRecords.Compute(maxQuery, string.Empty);
                maxRowId = (int)maxRow + 1;
            }
            else
            {
                maxRowId = 1;
            }

            DataRow row = detailRecords.NewRow();
            row[idField] = maxRowId;
            row[invoiceNumberField] = invoiceNumber;

            if (CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"].ToString() == "0")
            {
                idgDetails.xGrid.FieldLayouts[0].Fields["clm_id"].Visibility = Visibility.Collapsed;
                idgDetails.xGrid.FieldLayouts[0].Fields["clm_freeform"].Visibility = Visibility.Visible;
                //idgDetails.xGrid.FieldLayoutSettings.AllowDelete = false;
            }
            else
            {
                idgDetails.xGrid.FieldLayouts[0].Fields["clm_id"].Visibility = Visibility.Visible;
                idgDetails.xGrid.FieldLayouts[0].Fields["clm_freeform"].Visibility = Visibility.Collapsed;
               //RES 4/19/16 auto populate CLM Number if there is only 1 assigned to the contract
                if (this.CurrentBusObj.ObjectData.Tables["clm_lookup"].Rows.Count == 1)
                {
                    row["clm_id"] = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["clm_lookup"].Rows[0]["clm_id"]);
                    row["deal_type"] = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["clm_lookup"].Rows[0]["deal_type"]);
                    row["contract_executed_date"] = this.CurrentBusObj.ObjectData.Tables["clm_lookup"].Rows[0]["contract_executed_date"];
                }
            }

            detailRecords.Rows.Add(row);
            idgDetails.xGrid.DataSource = detailRecords.DefaultView;            
        }

       
        private void xGrid_CellUpdated(object sender, CellUpdatedEventArgs e)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
                return;
            int id = (int)e.Cell.Record.Cells[idField].Value;
            decimal rate = 0, extended = 0, units = 0;
            discountAmount = ParseToDecimal(txtDiscountAmount.Text.Remove(0, 1));

            //RES 9/1/17 populate deal_type and contract_executed_date if a new clm # was selected
            if (e.Cell.Field.Name == "clm_id") 
                if (Convert.ToInt16(e.Cell.Value) > 0)
                {
                    var recordsCLM = (from r1 in detailRecords.AsEnumerable()
                                    where r1.Field<int>(idField) == id
                                    select r1).ToList();

                    if (recordsCLM != null && recordsCLM.Count > 0)
                    {
                        DataRow detailRowCLM = recordsCLM[0] as DataRow;
                        var recordsCLMData = (from r2 in CurrentBusObj.ObjectData.Tables["clm_lookup"].AsEnumerable()
                                            where r2.Field<int>("clm_id") == Convert.ToInt16(e.Cell.Value)
                                            select r2).ToList();

                        if (recordsCLMData != null && recordsCLMData.Count > 0)
                        {
                            DataRow CLMData = recordsCLMData[0] as DataRow;
                            detailRowCLM["deal_type"] = CLMData["deal_type"];
                            detailRowCLM["contract_executed_date"] = CLMData["contract_executed_date"];
                        }
                    }
                }


            //If Item Date changes then change the Item End date to last day of the itemDateField

            if (e.Cell.Field.Name == itemDateField)
            {
                var recordsDate = (from r in detailRecords.AsEnumerable()
                               where r.Field<int>(idField) == id
                               select r).ToList();

                if (recordsDate != null && recordsDate.Count > 0)
                {
                    DataRow detailRowDate = recordsDate[0] as DataRow;

                    DateTime endDate = (DateTime)e.Cell.Value;
                                                          
                    detailRowDate[itemDateEndField] = GetLastDayOfMonth(endDate);
                }

                if (idgDetails.ActiveRecord != null)
                {
                    idgDetails.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                }
                               

            }
           

            if (e.Cell.Field.Name == itemDateField || e.Cell.Field.Name == revenueRuleValueField)
            {  
                UpdateRevenueAllocationRecords(id,0);
                
            }
            else if (e.Cell.Field.Name == rateField || e.Cell.Field.Name == unitsField || e.Cell.Field.Name == itemProductValueField)
            {
                var records = (from r in detailRecords.AsEnumerable()
                               where r.Field<int>(idField) == id
                               select r).ToList();

                if (e.Cell.Field.Name == itemProductValueField)
                {
                    int itemCode = (int)e.Cell.Value;


                    var items = (from r in CurrentBusObj.ObjectData.Tables[itemProductsTableName].AsEnumerable()
                                 where r.Field<int>(itemProductValueField) == itemCode
                                 select r).ToList();
                    

                    string query = string.Format("{0} = {1}", idField, id);
                    var details = detailRecords.Select(query);

                    if (items != null && items.Count > 0)
                    {   
                        DataRow row = items[0] as DataRow;

                        if (details != null && details.Length > 0)
                        {
                            DataRow detail = details[0] as DataRow;

                            detail[productValueField] = row[productValueField];
                            detail[accountCodeField] = row[accountCodeField];
                            detail[arAccountField] = row[arAccountField];
                            detail[invoiceDescriptionField] = row[descriptionField];
                            detail[rateField] = row[rateField];

                            rate = ParseToDecimal(row[rateField].ToString());
                            units = ParseToDecimal(detail[unitsField].ToString());

                            extended = Math.Round((rate * units), 2);
                            detail[extendedAmountField] = extended;
                            detail[itemDateField] = GetFirstDayOfCurrentMonth();
                            detail[itemDateEndField] = GetLastDayOfCurrentMonth();
                        }
                    }
                }

                if (records != null && records.Count > 0)
                {
                    DataRow detailRow = records[0] as DataRow;
                    rate = ParseToDecimal(detailRow[rateField].ToString());
                    units = ParseToDecimal(detailRow[unitsField].ToString());

                    extended = Math.Round((rate * units), 2);
                    detailRow[extendedAmountField] = extended;
                }

                totalAmount = 0;

                foreach (DataRow row in detailRecords.Rows)
                {
                    decimal rowAmount = 0;
                  
                   
                    if (decimal.TryParse(row[extendedAmountField].ToString(), out rowAmount))
                    {
                        totalAmount += rowAmount;
                    }
                     


                }

                if (idgDetails.ActiveRecord != null)
                {
                    idgDetails.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                }

                netAmount = totalAmount - discountAmount;
                txtTotalAmount.Text = totalAmount.ToString("C2");
                txtNetAmount.Text = netAmount.ToString("C2");                
                
                UpdateRevenueAllocationRecords(id,0);
            }
        }

        void xGrid_SelectedItemsChanged(object sender, SelectedItemsChangedEventArgs e)
        {
            if (accountDetailRecords == null) { return; }

            if (e.Type == typeof(Record))
            {                
                DataRecord record = default(DataRecord);

                if (idgDetails.xGrid.ActiveRecord != null)
                {
                    record = (DataRecord)idgDetails.xGrid.ActiveRecord;
                }
                else if (idgDetails.xGrid.SelectedItems.Records.Count > 0)
                {
                    record = (DataRecord)idgDetails.xGrid.SelectedItems.Records[0];
                }

                if (record != null)
                {
                    if (record.Cells[idField].Value != null)
                    {
                        int rowId = (int)record.Cells[idField].Value;

                        var records = (from r in accountDetailRecords.AsEnumerable()
                                       where r.Field<int>(idField) == rowId
                                       select r).ToList();

                        cBaseBusObject businessObject = new cBaseBusObject();
                        DataSet data = new DataSet();
                        DataTable newRecords = accountDetailRecords.Clone();

                        if (records != null && records.Count > 0)
                        {
                            List<DataRow> recordList = records.ToList<DataRow>();
                            recordList.ForEach(r => newRecords.ImportRow(r));
                        }

                        data.Tables.Add(newRecords);                        
                        idgRevenueAllocation.xGrid.DataSource = newRecords.DefaultView;
                    }
                }
            }
        }
        

        public void UpdateRevenueAllocationRecords(int id, int refreshOnlyFlag)
        {
            if (SecurityContext == AccessLevel.ViewOnly)
                return;
            var records = (from r in detailRecords.AsEnumerable()
                               where r.Field<int>(idField) == id
                               select r).ToList();

            idgRevenueAllocation.xGrid.DataSource = String.Empty;

            if (records != null && records.Count > 0)
            {
                DataRow detailRow = records[0] as DataRow;

                if (string.IsNullOrEmpty(detailRow[itemProductValueField].ToString())) { return; }

                accountDetailObject = new cBaseBusObject(miscInvoiceAcctDetailObject);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", invoiceNumberField), detailRow[invoiceNumberField]);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", idField), detailRow[idField]);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", itemProductValueField), detailRow[itemProductValueField]);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", itemDateField), detailRow[itemDateField]);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", extendedAmountField), detailRow[extendedAmountField]);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", productValueField), detailRow[productValueField]);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", accountCodeField), detailRow[accountCodeField]);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", revenueRuleValueField), detailRow[revenueRuleValueField]);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", percentCompleteFlagField), detailRow[percentCompleteFlagField]);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", companyCodeParameter), selectedCompany);
                accountDetailObject.Parms.AddParm(string.Format("@{0}", refreshOnlyFlagField), refreshOnlyFlag);

                accountDetailObject.LoadData();

                //string query = string.Format("{0} = {1}", idField, detailRow[idField]);
                //var rows = accountDetailRecords.Select(query);

                //foreach (var row in rows)
                //    row.Delete();   

                accountDetailRecords.Clear();
                
               

                DataTable replacements = accountDetailObject.ObjectData.Tables[miscInvoiceAcctDetailTableName];

                

                //if (rows != null && rows.Length > 0)
                //{                    
                //    for (int i = 0; i < rows.Length; i++)
                //    {
                //        foreach (DataColumn column in rows[i].Table.Columns)
                //        {
                //            if (!(column.ColumnName == accountDetailIdField))
                //            {
                //                rows[i][column.ColumnName] = replacements.Rows[i][column.ColumnName];
                //            }
                //        }
                       
                //    }
                //}
                //else
                //{
                //    accountDetailRecords.Merge(replacements);
                //}                              

               
            
          
                accountDetailRecords.Merge(replacements);

                accountDetailRecords.AcceptChanges();

                idgRevenueAllocation.xGrid.DataSource = accountDetailRecords.DefaultView;
            }
        }

        private decimal ParseToDecimal(string charValue)
        {
            decimal amount = 0;
            decimal.TryParse(charValue, out amount);

            return amount;
        }

        private DateTime GetFirstDayOfCurrentMonth()
        {
            DateTime today = DateTime.Now;
            DateTime firstDay = new DateTime(today.Year, today.Month, 1);
            return firstDay;
        }

        private DateTime GetLastDayOfCurrentMonth()
        {
            DateTime today = DateTime.Today;
            DateTime endOfMonth = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
            return endOfMonth;
        }

        private DateTime GetFirstDayOfMonth(DateTime dateValue)
        {
            
            DateTime firstDay = new DateTime(dateValue.Year, dateValue.Month, 1);
            return firstDay;
        }

        private DateTime GetLastDayOfMonth(DateTime dateValue)
        {
            
            DateTime endOfMonth = new DateTime(dateValue.Year, dateValue.Month, 1).AddMonths(1).AddDays(-1);
            return endOfMonth;
        }

        public void GridInvoiceDetailRevRecDelegate()
        {
            //call invoice revenue recognition screen
            this.GetInvoiceRevRecInfoToPass();
        }

        public void GetInvoiceRevRecInfoToPass()
        {

            cBaseBusObject CustomInvoiceRevRecObj = new cBaseBusObject();

            CustomInvoiceRevRecObj.BusObjectName = "CustomInvoiceRevRec";
            //gInvoiceDetail.ReturnSelectedData("invoice_number");
            string invoice_number_to_pass = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();
            CustomInvoiceRevRecObj.Parms.AddParm("@invoice_number", invoice_number_to_pass);            
            
            if (CustomInvoiceRevRecObj != null)
            {
                //check for posted invoice
                if (Convert.ToInt32(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["posted_flag"]) != 1) 
                    Messages.ShowInformation("Cannot disply revenue recognition until invoice is posted");
                else
                //show the revenue recognition screen
                    callInvoiceRevRecScreen(CustomInvoiceRevRecObj);
            }
            else
            {
                Messages.ShowInformation("Problem Opening Custom Invoice Revenue Recognition Screen");
            }

        }
        private void callInvoiceRevRecScreen(cBaseBusObject CustomInvoiceRevRecObj)
        {

            CustomInvoiceRevRecDetail invoiceAcctScreen = new CustomInvoiceRevRecDetail(CustomInvoiceRevRecObj);
            System.Windows.Window invoiceScreenWindow = new System.Windows.Window();
            invoiceScreenWindow.Title = "Custom Invoice Revenue Recognition Detail ";
            //set rules screen as content of new window
            invoiceScreenWindow.Content = invoiceAcctScreen;
            //open new window with embedded user control
            invoiceScreenWindow.ShowDialog();
        }

        public bool SaveData()
        {

            bool hasDataChanged = false;
            bool returnFalse = false;
            bool CLMFlag = true;
            bool AddFlag = false;
            bool UpdateFlag = false;
            //String PONumber = "";
            //Decimal POAmount = 0;
            //Int32 POOverrideFlag = 0;
            //Decimal POCharges = 0;
            if (SecurityContext == AccessLevel.ViewOnly)
                return false;
            
            if (detailRecords.Rows.Count > 0 )
            {                
                //tas 10.24.13 - added logic to ensure that both Percent Complete and Contract Completed are not checked.

                foreach (DataRow r in detailRecords.Rows)
                {
                    //If Percent Complete is checked, ensure Completed Contract is not checked.
                    //If Both Percent Complete, send message to user before saving.
                    if ((Convert.ToInt32(r["percent_complete_flag"]) == 1) && (Convert.ToInt32(r["completed_contract_flag"]) == 1))
                    {
                        Messages.ShowInformation("Both Percent Complete and Contract Completed cannot be checked.");
                        returnFalse = true;
                        break;
                    }
                }

                if (returnFalse)
                {
                    return false;
                }

                
                
                DataTable rowsModifed = detailRecords.GetChanges(DataRowState.Modified);
                DataTable rowsAdded = detailRecords.GetChanges(DataRowState.Added);

                AddFlag = false;
                UpdateFlag = false;
                if (rowsModifed != null && rowsModifed.Rows.Count > 0)
                {
                    hasDataChanged = true;
                    UpdateFlag = true;

                }
                //else if (rowsAdded != null && rowsAdded.Rows.Count > 0)
                if (rowsAdded != null && rowsAdded.Rows.Count > 0)
                {
                    hasDataChanged = true;
                    AddFlag = true;
                }

                //RES 4/19/16 Check for CLM Number in new or modified detail rows
                //RES 8/31/16 Check that new CLM columns (deal type, execution date) are populated when a CLM # is added
                if (hasDataChanged)
                {
                    int validateitem = 0;
                    if (AddFlag)
                        foreach (DataRow r2 in rowsAdded.Rows)
                        {
                            validateitem = (Convert.ToInt32(r2["item_code"]));
                            cBaseBusObject ItemInactive = new cBaseBusObject("ManItemXREFInactiveEdit");
                            ItemInactive.Parms.ClearParms();
                            ItemInactive.Parms.AddParm("@item_code", validateitem);
                            ItemInactive.LoadTable("inactive");
                            if (Convert.ToInt32(ItemInactive.ObjectData.Tables["inactive"].Rows[0]["inactive_flag"]) > 0)
                            {
                                MessageBox.Show("Item code '" + ItemInactive.ObjectData.Tables["inactive"].Rows[0]["description"].ToString() + "' selected on new detail record is inactive.  Save cancelled.");
                                return false; 
                            }                           
                            //RES 4/19/16 Check that CLM Number has been added if not give warning but allow to continue
                            if ((Convert.ToInt32(r2["clm_id"]) == 0) && r2["clm_freeform"].ToString() == "")
                            {
                                CLMFlag = false;
                                if (r2["deal_type"].ToString() != "" && r2["deal_type"].ToString() != "0")
                                {
                                    MessageBox.Show("CLM Deal Type entered without a CLM number");
                                    return false;
                                }
                                if (r2["contract_executed_date"].ToString() != "" && r2["contract_executed_date"].ToString() != "1/1/1900 12:00:00 AM")
                                {
                                    MessageBox.Show("CLM Contract Executed Date entered without a CLM number");
                                    return false;
                                }
                            }
                            else
                            {
                                //if (Convert.ToInt32(r2["deal_type"]) == null)
                                if (Convert.ToInt32(r2["clm_id"]) == 0 && r2["deal_type"].ToString() == "")
                                {
                                    MessageBox.Show("CLM Deal Type is required");
                                    return false;
                                }

                                if (Convert.ToInt32(r2["clm_id"]) == 0 && r2["contract_executed_date"].ToString() == "")
                                {
                                    MessageBox.Show("Contract Executed Date is required");
                                    return false;
                                }
                            }
                        }
                    if (UpdateFlag)
                        foreach (DataRow r2 in rowsModifed.Rows)
                        {
                            validateitem = (Convert.ToInt32(r2["item_code"]));
                            cBaseBusObject ItemInactive = new cBaseBusObject("ManItemXREFInactiveEdit");
                            ItemInactive.Parms.ClearParms();
                            ItemInactive.Parms.AddParm("@item_code", validateitem);
                            ItemInactive.LoadTable("inactive");
                            if (Convert.ToInt32(ItemInactive.ObjectData.Tables["inactive"].Rows[0]["inactive_flag"]) > 0)
                            {
                                MessageBox.Show("Item code '" + ItemInactive.ObjectData.Tables["inactive"].Rows[0]["description"].ToString() + "' selected on updated detail record is inactive.  Save cancelled.");
                                return false;
                            }            
                            //RES 4/19/16 Check that CLM Number has been added if not give warning but allow to continue
                            if ((Convert.ToInt32(r2["clm_id"]) == 0) && r2["clm_freeform"].ToString() == "")
                            {
                                CLMFlag = false;
                                if (r2["deal_type"].ToString() != "" && r2["deal_type"].ToString() != "0")
                                {
                                    MessageBox.Show("CLM Deal Type entered without a CLM number");
                                    return false;
                                }
                                if (r2["contract_executed_date"].ToString() != "" && r2["contract_executed_date"].ToString() != "1/1/1900 12:00:00 AM")
                                {
                                    MessageBox.Show("CLM Contract Executed Date entered without a CLM number");
                                    return false;
                                }
                            }
                            else
                            {
                                //if (Convert.ToInt32(r2["deal_type"]) == null)
                                if (Convert.ToInt32(r2["clm_id"]) == 0 && r2["deal_type"].ToString() == "")
                                {
                                    MessageBox.Show("CLM Deal Type is required");
                                    return false;
                                }

                                if (Convert.ToInt32(r2["clm_id"]) == 0 && r2["contract_executed_date"].ToString() == "")
                                {
                                    MessageBox.Show("Contract Executed Date is required");
                                    return false;
                                }
                            }
                        }
                    //if (!CLMFlag)
                    //    Messages.ShowInformation("Missing CLM Number in new or updated detail row.");

                    //RES 3/22/17 Check for PO number amount exceeded
                    //if (AddFlag)
                    //    POAmount = 0;
                    //    PONumber = "";
                    //    foreach (DataRow r2 in rowsAdded.Rows)
                    //    {
                    //        if (r2["po_number"].ToString() != "")
                    //            if (PONumber == r2["po_number"].ToString())
                    //                POCharges = POCharges + Convert.ToDecimal(r2["extended"]);
                    //            else
                    //                if (PONumber == "")
                    //                {
                    //                    PONumber = r2["po_number"].ToString();
                    //                    POCharges = Convert.ToDecimal(r2["extended"]);
                    //                }
                    //                else
                    //                {
                    //                    //customerObject = new cBaseBusObject(receivableAcctObject);
                    //                    //customerObject.Parms.AddParm(receivableAcctParameter, PONumber);
                    //                    //customerObject.LoadData();

                    //                    //if (customerObject.ObjectData.Tables[receivableAcctTableName].Rows.Count > 0)
                    //                    //{
                    //                    //    DataRow row = customerObject.ObjectData.Tables[receivableAcctTableName].Rows[0];
                    //                    //    General.txtAccountName.Text = row[accountNameField].ToString();
                    //                    //    General.txtAddress1.Text = row[address1Field].ToString();
                    //                    //    General.txtAddress2.Text = row[address2Field].ToString();
                
                    //                    //}
                    //                }

                    //    }
                    if (UpdateFlag)
                        foreach (DataRow r2 in rowsModifed.Rows)
                        {
                            //RES 4/19/16 Check that CLM Number has been added if not give warning but allow to continue
                            if ((Convert.ToInt32(r2["clm_id"]) == 0) && r2["clm_freeform"].ToString() == "")
                                CLMFlag = false;

                        }
                    if (!CLMFlag)
                        Messages.ShowInformation("Missing CLM Number in new or updated detail row.");
 
                }


                //CBirney 4.20.12 added the accessLevel and security context to this statement
                if  (hasDataChanged) 
                {
                    //if (SecurityContext == AccessLevel.ViewOnly)
                    //{
                    //    MessageBox.Show("You do not have permission for this action.");
                    //    hasDataChanged = false;
                    //    return false;
                    //}
                 
                    //string message = "Do you want to save the data from the Details tab?";
                    //if (Messages.ShowYesNo(message, MessageBoxImage.Question) == MessageBoxResult.Yes)

                    if (accountDetailRecords.Rows.Count > 0)
                    {
                        //if this is an insert invoice number was not set when these records were created                            
                        foreach (DataRow row in detailRecords.Rows)
                        { //Set item date to first of month

                            DateTime itemDate;
                            itemDate = Convert.ToDateTime(row[itemDateField]);

                            row[itemDateField] = GetFirstDayOfMonth(itemDate);


                            if (row.RowState == DataRowState.Added)
                            {
                                if (string.IsNullOrEmpty(row[invoiceNumberField].ToString()))
                                {
                                    row[invoiceNumberField] = this.invoiceNumber;
                                    string query = string.Format("{0} = {1}", idField, row[idField]);
                                    var children = accountDetailRecords.Select(query);

                                    if (children != null && children.Length > 0)
                                    {
                                        for (int i = 0; i < children.Length; i++)
                                        {
                                            children[i][invoiceNumberField] = this.invoiceNumber;
                                        }
                                    }
                                }
                            }
                        }


                        if (cGlobals.BillService.UpdateCustomInvoiceDetail(data))
                        {
                            //Loop through and update item dates to first of month


                            detailRecords.AcceptChanges();
                            accountDetailRecords.AcceptChanges();

                            Messages.ShowInformation("Save successful.");
                            //this.CurrentBusObj.LoadTable("clm_lookup");
                            //this.CurrentBusObj.LoadTable("detail");
                            //idgDetails.LoadGrid(CurrentBusObj, "detail");
                            return true;
                        }
                        else
                        {
                            Messages.ShowInformation("Save was not successful.");
                            return false;
                        }
                    }

                   
                }
                
            }
            return hasDataChanged;
        }
    }
}
