

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using RazerBase.Lookups;
using System.Collections.Generic;
#endregion

namespace Cash
{

    #region class ucTab2
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class DetailCashTab : ScreenBase, IPreBindable, IPostBindable
    {

        #region Private Variables
        public string _currencycode;
        public ComboBoxItemsProvider cmbCurrency { get; set; }
        public ComboBoxItemsProvider cmbProducts { get; set; }
        private static readonly string CurrencyDisplayPath = "description";
        private static readonly string CurrencyValuePath = "currency_code";
        public bool _CellEdit = true;
        private int  _remitid = 100;
        private int _currentindex = 0;
        private decimal _loadtotal = 0M;
        private int _batchid = 0;
        private bool _editmode = false;
        private decimal _total_amount = 0M;
        private decimal _currenttotal = 0M;
        private int _remitnumber = 1;
        private int _allocid = 0;
        private int _allocindex = 0;
        private bool _isnewalloc = true;
        private bool _newremit = false;
        public BatchObject _newobject = null;
        private bool _hasbound = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public DetailCashTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();
          
            // Perform initializations for this object
            Init();
          
                  
        }
     
       
        #endregion


        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            MainTableName = "cash_remit";
            CurrentBusObj = new cBaseBusObject("Cash");
            SetChildSettings();
        }

        void gGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {

            if (gRemit.ActiveRecord != null)
            {

                DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_remit"];
                DataRow dr1 = dt.Rows[gRemit.ActiveRecord.Index];
                if (dr1.RowState != DataRowState.Deleted)
                {

                    int edit_index = gRemit.ActiveRecord.Cells.IndexOf(e.Cell);
                    int row_index = gRemit.ActiveRecord.Index;
                    DataRecord _currentRemit = gRemit.ActiveRecord;
                    if (_currentRemit != null)
                    {
                        if (edit_index == 1)
                        {
                            if (_currentRemit.Cells[3].Value.ToString() != "0")
                            {

                            }
                            else
                            {
                                dr1[5] = Int32.Parse(e.Cell.Value.ToString());
                            }
                        }
                        else if (edit_index == 4)
                        {
                            _loadtotal = (decimal)e.Cell.Value;
                            _currenttotal = 0M;
                            if (_currencycode == "USD")
                            {
                                dr1[7] = _loadtotal.ToString();
                            }
                            if ((GetFunctionalRemit()) > _newobject.BatchTotal)
                            {
                                e.Cell.Value = 0;
                                Messages.ShowError("You cannot remit more than the batch total");
                            }
                            else
                            {
                                if (_loadtotal > 0)
                                {

                                    _isnewalloc = true;
                                    if (_newremit)
                                    {
                                        _newremit = false;
                                        int _finalremit;
                                        string newinvoicenumber = cGlobals.BillService.GetNextInvoiceNumber("CASH");
                                        decimal _exchange = GetExchangeRate(dr1["currency_code"].ToString());

                                        decimal bankcharge = ((decimal.Parse(dr1["amount"].ToString()) * _exchange) - decimal.Parse(dr1["amount_functional"].ToString()));
                                        _finalremit = cGlobals.BillService.InsertRemitEntry(_newobject.BatchID, Int32.Parse(dr1["remit_id"].ToString()), dr1["document_id"].ToString(), DateTime.Now, dr1["remit_number"].ToString(), decimal.Parse(dr1["amount"].ToString()), decimal.Parse(dr1["amount_functional"].ToString()), dr1["currency_code"].ToString(), (Double)_exchange, bankcharge);
                                        dr1["remit_id"] = _finalremit;
                                        InsertAllocation();
                                    }

                                }
                            }
                        }
                        else if (edit_index == 2)
                        {
                            dr1[8] = e.Cell.Value;

                        }
                        else if (edit_index == 3)
                        {
                            dr1[7] = decimal.Parse(e.Cell.Value.ToString());
                            _currentRemit.Cells[4].Value = e.Cell.Value;
                        }
                    }
                }
            }
        }
        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
           
            int edit_index = 0;
            DataRecord _currentAlloc;
            try
            {
                edit_index = gAllocation.ActiveRecord.Cells.IndexOf(e.Cell);
                _currentAlloc = gAllocation.ActiveRecord;
            }
            catch
            {
                _currentAlloc = e.Cell.Record;
                edit_index = _currentAlloc.Cells.IndexOf(e.Cell);
            }
            if (e.Cell.Field.Name == "apply_to_doc")
            {

                if (_currentAlloc.Cells["unapplied_flag"].Value.ToString() == "1")
                {
                    Messages.ShowError("You cannot enter an invoice on an unapplied allocation");
                    gAllocation.ActiveRecord.Cells["apply_to_doc"].Value = "";
                }
            }
            else if (e.Cell.Field.Name == "unapplied_flag")
            {
                XamCheckEditor editor = (XamCheckEditor)e.Editor;

                e.Editor.ValueChanged += new RoutedPropertyChangedEventHandler<object>(Editor_ValueChanged);
                editor.EndEditMode(true, true);

                gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellNext);

            }
            else if (e.Cell.Field.Name == "amount")
            {
                if (_currentAlloc != null)
                {
                    DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_alloc"];
                    DataRowView dr = _currentAlloc.DataItem as DataRowView;
                    DataView dv = dr.DataView;
                    DataRow dr1 = dt.Rows[_currentAlloc.Index];
                    decimal dc = decimal.Parse(_currentAlloc.Cells["amount"].Value.ToString().Replace("$", ""));
                    decimal RemitTotal = (AllocationByRemit(Int32.Parse(gRemit.ActiveRecord.Cells["remit_id"].Value.ToString())));
                    decimal AllocationTotal = GetAlloc(Int32.Parse(gRemit.ActiveRecord.Cells["remit_id"].Value.ToString()));
                    if (AllocationTotal > RemitTotal)
                    {
                        Messages.ShowError("Allocation cannot be more than the remit total");
                        return;
                    }
                    if (RemitTotal > AllocationTotal)
                    {
                        SetRemitAllocation(Int32.Parse(gRemit.ActiveRecord.Cells["remit_id"].Value.ToString()));
                        _CellEdit = true;
                        if (gRemit.ActiveRecord.Cells[5].Value != gRemit.ActiveRecord.Cells[4].Value)
                        {
                            _currentindex++;
                            gAllocation.Focus();
                            System.Windows.Input.FocusManager.SetFocusedElement(CashGrid, gAllocation);
                            InsertAllocation();
                        }
                    }
                    else if (GetFunctionalRemit() < _newobject.BatchTotal)
                    {
                        SetRemitAllocation(Int32.Parse(gRemit.ActiveRecord.Cells["remit_id"].Value.ToString()));
                        _remitid++;
                        _currentindex++;
                        _CellEdit = true;
                        if (GetFunctionalRemit() != _newobject.BatchTotal)
                        {
                            System.Windows.Input.FocusManager.SetFocusedElement(CashGrid, gRemit);
                            AddRemittance();
                        }
                    }
                    else
                    {
                        SetRemitAllocation(Int32.Parse(gRemit.ActiveRecord.Cells["remit_id"].Value.ToString()));
                        _currentindex++;
                        _CellEdit = true;
                    }
                }
            }
        }
        void Editor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int x = (int)e.NewValue;
            gAllocation.ActiveRecord.Cells["unapplied_flag"].Value = x;
            if (x == 1)
            {
                decimal _remital = AllocationByRemit(Int32.Parse(gAllocation.ActiveRecord.Cells["remit_id"].Value.ToString()));
                decimal _allocal = GetAlloc(Int32.Parse(gAllocation.ActiveRecord.Cells["remit_id"].Value.ToString()));
                gAllocation.ActiveRecord.Cells["amount"].Value = _remital - _allocal;
                if (gAllocation.ActiveRecord.Cells["apply_to_doc"].Value.ToString().Length > 0)
                {

                    if ((gAllocation.ActiveRecord.Cells["receivable_account"].Value.ToString().Length < 1))
                    {
                        Messages.ShowError("You must choose a customer id if allocation is unapplied");
                        return;
                    }
                    if (gAllocation.ActiveRecord.Cells["product_code"].Value.ToString().Length < 1)
                    {
                        Messages.ShowError("You must choose a product code if allocation is unapplied");
                        return;
                    }
                    if (gAllocation.ActiveRecord.Cells["apply_to_doc"].Value.ToString().Length > 0)
                    {
                        Messages.ShowError("Unapplied cannot be chosen if you have an invoice number.");
                     
                        return;
                    }
                }
            }
            gAllocation.CntrlFocus();
          
        }
        void AllocGrid_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            if (gAllocation.ActiveRecord != null)
            {
                _CellEdit = false;
                DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_alloc"];
                DataRecord _dr = gAllocation.ActiveRecord;
                int g = gAllocation.ActiveRecord.Cells.IndexOf(e.Cell);
                string invoice_number = "";
                if (e.Cell.Field.Name == "apply_to_doc")
                {
                    if (gAllocation.ActiveRecord.Cells["unapplied_flag"].Value.ToString() != "1")
                    {
                        invoice_number = e.Cell.Value.ToString();
                        CurrentBusObj.Parms.ParmList.Clear();
                        this.CurrentBusObj.Parms.AddParm("@batch_id", _newobject.BatchID);
                        CurrentBusObj.Parms.AddParm("@document_id", invoice_number);
                        CurrentBusObj.Parms.AddParm("@date_type", "ACCTPERIOD");
                        this.Load("cash_invoice");
                        DataTable mainalloc = (DataTable)CurrentBusObj.ObjectData.Tables["cash_invoice"];
                        //    gAllocation.ActiveRecord.Cells[3].IsActive = false;
                        if (mainalloc.Rows.Count > 0)
                        {
                            DataRecord Drecord = gRemit.ActiveRecord;
                            if (Int32.Parse(mainalloc.Rows[0]["row_count"].ToString()) > 1)
                            {
                                this.Load("entry_invoicerows");
                                cGlobals.ReturnParms.Clear();
                                CashAllocationLookup pl = new CashAllocationLookup(CurrentBusObj);
                                pl.ShowDialog();
                                if (cGlobals.ReturnParms.Count > 0)
                                {
                                    _dr.Cells["receivable_account"].Value = cGlobals.ReturnParms[3].ToString();
                                    _dr.Cells["company_code"].Value = cGlobals.ReturnParms[4].ToString();
                                    _dr.Cells["apply_to_seq"].Value = Int32.Parse(cGlobals.ReturnParms[2].ToString());
                                    _dr.Cells["product_code"].Value = cGlobals.ReturnParms[0].ToString();
                                    Decimal amt = Decimal.Parse(cGlobals.ReturnParms[1].ToString().Replace("$", ""));
                                    if (amt > 0)
                                    {
                                        _dr.Cells["amount"].Value = amt;
                                    }
                                    cGlobals.ReturnParms.Clear();
                                    gRemit.ActiveRecord = Drecord;
                                }
                                else
                                {
                                    _dr.Cells["receivable_account"].Value = mainalloc.Rows[0]["receivable_account"].ToString();
                                    _dr.Cells["company_code"].Value = mainalloc.Rows[0]["company_code"].ToString();
                                    _dr.Cells["apply_to_seq"].Value = Int32.Parse(mainalloc.Rows[0]["seq_code"].ToString());
                                    _dr.Cells["product_code"].Value = mainalloc.Rows[0]["product_code"].ToString();
                                    Decimal amt = Decimal.Parse(mainalloc.Rows[0]["open_amount"].ToString().ToString().Replace("$", ""));
                                    if (amt > 0)
                                    {
                                        _dr.Cells["amount"].Value = amt;
                                    }
                                    gRemit.ActiveRecord = Drecord;
                                }

                                _CellEdit = true;
                            }
                            else
                            {
                                _dr.Cells["product_code"].Value = mainalloc.Rows[0]["product_code"].ToString();
                                _dr.Cells["receivable_account"].Value = mainalloc.Rows[0]["receivable_account"].ToString();
                                _dr.Cells["company_code"].Value = mainalloc.Rows[0]["company_code"].ToString();
                                _dr.Cells["apply_to_seq"].Value = Int32.Parse(mainalloc.Rows[0]["seq_code"].ToString());
                                Decimal amt = Decimal.Parse(mainalloc.Rows[0]["open_amount"].ToString().ToString().Replace("$", ""));
                                if (amt > 0)
                                {
                                    _dr.Cells["amount"].Value = amt;
                                }
                                _CellEdit = true;
                                gAllocation.LoadGrid(CurrentBusObj, "cash_alloc");

                            }
                        }
                    }
                    else
                    {
                        Messages.ShowError("Invoice Number not found");
                        gAllocation.xGrid.ActiveCell = e.Cell;
                        e.Cell.Value = "";
                        _CellEdit = true;
                    }
                }
                else if (e.Cell.Field.Name == "unapplied_flag")
                {
                    // Check whether to add an alloc or add a remit or do nothing.
                    if (Int32.Parse(_dr.Cells["unapplied_flag"].Value.ToString()) == 1)
                    {
                        if (_dr.Cells["receivable_account"].Value.ToString().Length < 1)
                        {
                            Messages.ShowError("You must choose a customer id if allocation is unapplied");
                            return;
                        }
                        if (_dr.Cells["product_code"].Value.ToString().Length < 1)
                        {
                            Messages.ShowError("You must choose a product code if allocation is unapplied");
                            return;
                        }
                        if (_dr.Cells["apply_to_doc"].Value.ToString().Length > 1)
                        {
                            Messages.ShowError("You can not select unapplied if there is an invoice number");
                            _dr.Cells["unapplied_flag"].Value = 0;
                            return;

                        }
                    }
                }
                else if (e.Cell.Field.Name == "amount")
                {
                    bool haserror = false;
                    if (Int32.Parse(_dr.Cells["unapplied_flag"].Value.ToString()) == 1)
                    {
                        if (_dr.Cells["receivable_account"].Value.ToString().Length < 1)
                        {
                            haserror = true;
                            Messages.ShowError("You must choose a customer id if allocation is unapplied");
                            return;
                        }
                        if (_dr.Cells["product_code"].Value.ToString().Length < 1)
                        {
                            haserror = true;
                            Messages.ShowError("You must choose a product code if allocation is unapplied");
                            return;
                        }
                        if (_dr.Cells["apply_to_doc"].Value.ToString().Length > 1)
                        {
                            haserror = true;
                            Messages.ShowError("You can not select unapplied if there is an invoice number");
                            _dr.Cells["unapplied_flag"].Value = 0;
                            return;

                        }
                    }
                    if(!haserror)
                    {
                        gRemit.CntrlFocus();
                        gRemit.xGrid.ExecuteCommand(DataPresenterCommands.RecordLastDisplayed);
                    }
                }
                else if (e.Cell.Field.Name == "receivable_account")
                {
                    if (Int32.Parse(_dr.Cells["unapplied_flag"].Value.ToString()) == 1)
                    {
                        if (_dr.Cells["receivable_account"].Value.ToString().Length < 1)
                        {
                            Messages.ShowError("You must choose a customer id if allocation is unapplied");
                            return;
                        }
                        if (_dr.Cells["product_code"].Value.ToString().Length < 1)
                        {
                            Messages.ShowError("You must choose a product code if allocation is unapplied");
                            return;
                        }
                        if (_dr.Cells["apply_to_doc"].Value.ToString().Length > 1)
                        {
                            Messages.ShowError("You can not select unapplied if there is an invoice number");
                            _dr.Cells["unapplied_flag"].Value = 0;
                            return;

                        }
                    }
                }
                else if (e.Cell.Field.Name == "product_code")
                {

                        foreach (DataRow itm in this.CurrentBusObj.ObjectData.Tables["cash_products"].Rows)
                        {
                            if (_dr.Cells["product_code"].Value == itm["product_code"])
                            {
                                _dr.Cells["company_code"].Value= itm["gl_co"];
                                break;
                            }
                        }
                }
            }
        }
        ///// <summary>
        /// Override of save method handles save functionality for folder
        /// </summary>
       
        private void SubmitAlloc(int batchid, int remitid, int origremit)
        {
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["cash_alloc"].Rows)
            {
                if (string.IsNullOrEmpty(dr["apply_to_doc"].ToString()) && (string.IsNullOrEmpty(dr["product_code"].ToString())))
                {
                }
                else
                {
                    if (dr["remit_id"].ToString() == origremit.ToString())
                    {
                        if (dr["company_code"].ToString() == "0")
                        {
                            foreach (DataRow itm in this.CurrentBusObj.ObjectData.Tables["entry_products"].Rows)
                            {
                                if (dr["product_code"] == itm["product_code"])
                                    dr["company_code"] = itm["gl_co"];
                            }
                        }
                        cGlobals.BillService.InsertAllocEntry(batchid, remitid, dr["apply_to_doc"].ToString(), Int32.Parse(dr["apply_to_seq"].ToString()), Decimal.Parse(dr["amount"].ToString()), dr["company_code"].ToString(), dr["receivable_account"].ToString(), dr["product_code"].ToString(), Int32.Parse(dr["unapplied_flag"].ToString()));
                    }
                }
            }
        }
        private object ReturnSelectedData()
        {
            throw new NotImplementedException();
        }

        void aGrid_EditModeStarted(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs e)
        {

        }
        private decimal GetExchangeRate(string currency_code)
        {

            decimal _returnexchange = 0.0M;
            if (currency_code != "USD")
            {
                DataTable dcr = this.CurrentBusObj.ObjectData.Tables["cash_currency_history"];
                var currency = from x in dcr.AsEnumerable()
                               where x.Field<string>("from_currency") == currency_code
                               && x.Field<string>("to_currency") == SetCurrencyCode() && x.Field<DateTime>("conversion_date") == _newobject.BatchDate
                               select new
                               {
                                   fexchange = x.Field<Double>("conversion_rate"),

                               };
                foreach (var item in currency)
                {
                    _returnexchange = (decimal)item.fexchange;
                }
            }
            else
            {
                _returnexchange = 1;
            }
            return _returnexchange;
        }
        public string SetCurrencyCode()
        {
            DataTable dcr = this.CurrentBusObj.ObjectData.Tables["cash_bank"];
            foreach (DataRow item in dcr.Rows)
            {
                if (item["bank_id"].ToString() == _newobject.BankID.ToString())
                {
                    _currencycode = item["currency_code"].ToString();

                }

            }
            return _currencycode;
        }
        private void AddRemittance()
        {
            if ((GetTotalRemit() != _newobject.BatchTotal) && (!HasBlankRemit()))
            {

            _allocindex = 0;
            string newinvoicenumber = cGlobals.BillService.GetNextInvoiceNumber("CASH");
            DataRow dr = this.CurrentBusObj.ObjectData.Tables["cash_remit"].NewRow();
            dr["batch_id"] = _newobject.BatchID;
            dr["document_id"] = newinvoicenumber;
            dr["remit_type_id"] = _remitnumber;
            dr["remit_date"] = DateTime.Now;
            dr["remit_number"] = "";
            dr["amount"] = 0.0M;
            dr["amount_functional"] = 0.0M;
            dr["currency_code"] = "USD";
            dr["exchange_rate"] = 0.0M;
            dr["bank_charge_amount"] = 0.0M;
            this.CurrentBusObj.ObjectData.Tables["cash_remit"].Rows.Add(dr);
            _remitnumber++;
            _remitid++;
            gRemit.xGrid.FieldSettings.AllowEdit = true;
           gRemit.LoadGrid(CurrentBusObj, "cash_remit");
           gRemit.xGrid.ExecuteCommand(DataPresenterCommands.RecordLastOverall);
           gRemit.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
           gRemit.xGrid.ExecuteCommand(DataPresenterCommands.CellFirstInRecord);
           gRemit.xGrid.ActiveCell = (gRemit.xGrid.Records[gRemit.ActiveRecord.Index] as DataRecord).Cells[1];
           CellValuePresenter.FromCell((gRemit.xGrid.Records[gRemit.ActiveRecord.Index] as DataRecord).Cells[1]).StartEditMode();
            _newremit = true;
       gRemit.xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);
            }
            else
            {
                Messages.ShowInformation("Cannot remit more than the batch total.");
            }
        }
        public CellValuePresenter GetCellValuePresenter(int rowIndex, int colIndex)
        {
            DataRecord dr = this.gRemit.xGrid.Records[rowIndex] as DataRecord;
            return CellValuePresenter.FromCell(dr.Cells[colIndex]);
        }


        public void PreBind()
        {

            if (this.CurrentBusObj.HasObjectData)
            {
                cmbCurrency = new ComboBoxItemsProvider();
                cmbCurrency.ItemsSource = this.CurrentBusObj.ObjectData.Tables["cash_currency"].DefaultView;
                cmbCurrency.ValuePath = CurrencyValuePath;
                cmbCurrency.DisplayMemberPath = CurrencyValuePath;
                cmbProducts = new ComboBoxItemsProvider();
                cmbProducts.ItemsSource = CurrentBusObj.ObjectData.Tables["cash_products"].DefaultView;
                cmbProducts.ValuePath = "product_code";
                cmbProducts.DisplayMemberPath = "product_code";
            }
        }
        private bool HasBlankAllocation(int _remitid)
        {
            bool hasblanks = false;
            foreach (DataRow item in this.CurrentBusObj.ObjectData.Tables["cash_alloc"].Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {
                    if ((item["apply_to_doc"].ToString().Trim() == "") && (item["receivable_account"].ToString() == "") && (item["product_code"].ToString() == ""))
                    {
                        hasblanks = true;
                    }
                }
            }
            return hasblanks;
        }
        private bool HasBlankRemit()
        {
            bool hasblanks = false;
            foreach (DataRow item in this.CurrentBusObj.ObjectData.Tables["cash_remit"].Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {
                    if ((item["remit_number"].ToString().Trim() == "") && (item["amount_functional"].ToString() == "0.00") && (item["amount"].ToString() == "0.00"))
                    {
                        hasblanks = true;
                    }
                }
            }
            return hasblanks;
        }
        private void InsertAllocation()
        {
            if (GetTotalAllocation() != _newobject.BatchTotal)
            {

           
               
                DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_alloc"];
                int _currentremit = 0;
                _currentremit = Int32.Parse(gRemit.ActiveRecord.Cells["remit_id"].Value.ToString());
                decimal _remittotal = decimal.Parse(gRemit.ActiveRecord.Cells["amount"].Value.ToString());
                decimal _alloctotal = 0M;
                int alloccount = 0;
                
                foreach (DataRow drx in dt.Rows)
                {
                    if (int.Parse(drx["remit_id"].ToString()) == _currentremit)
                    {
                        _alloctotal += (decimal)drx[5];
                        alloccount++;
                    }
                }
                decimal Batch_Total = _newobject.BatchTotal;
                decimal Remit_total = GetFunctionalRemit();
                bool canadd = true;
                if (alloccount == 1)
                {
                    if (_alloctotal == 0)
                    {
                        canadd = false;
                    }
                }
                if ((Remit_total == Batch_Total) && (_alloctotal == _remittotal))
                {

                }

                else
                {

                    if (canadd & (_alloctotal < Remit_total) && (!HasBlankAllocation(_currentremit)))
                    {
                        DataRow dr1 = this.CurrentBusObj.ObjectData.Tables["cash_alloc"].NewRow();
                        dr1["batch_id"] = _newobject.BatchID;
                        dr1["remit_id"] = gRemit.ActiveRecord.Cells["remit_id"].Value;
                        dr1["remit_alloc_id"] = _allocid;
                     
                        dr1["apply_to_doc"] = "";
                        dr1["apply_to_seq"] = 0;
                        dr1["amount"] = 0.0M;
                        dr1["company_code"] = 0;
                        dr1["receivable_account"] = "";
                        dr1["product_code"] = "";
                        dr1["unapplied_flag"] = 0;
                        this.CurrentBusObj.ObjectData.Tables["cash_alloc"].Rows.Add(dr1);
                        _allocindex++;
                        _allocid++;
                        gAllocation.LoadGrid(CurrentBusObj, "cash_alloc");
                    }
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.RecordLastOverall);
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellFirstInRecord);
                
                   //CellValuePresenter.FromCell((gAllocation.xGrid.Records[gAllocation.ActiveRecord.Index] as DataRecord).Cells[0]).StartEditMode();
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);
                }
            }
            else
            {
                Messages.ShowInformation("Cannot allocate more than the batch total.");
            }
        }
           
        public bool movenext = false;
        public decimal AllocationByRemit(int remit_id)
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_remit"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item["remit_id"].ToString() == remit_id.ToString())
                {
                    alloctotal += decimal.Parse(item["amount"].ToString());
                }
            }
            return alloctotal;
        }
        public decimal GetAlloc(int remit_id)
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_alloc"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item["remit_id"].ToString() == remit_id.ToString())
                {
                    alloctotal += decimal.Parse(item["amount"].ToString());
                }
            }
            return alloctotal;
        }

        public void GetDoubleClick()
        {
            Cell _activecell = gAllocation.xGrid.ActiveCell;
            Record _activeRecord = gAllocation.xGrid.Records[gAllocation.ActiveRecord.Index];
            if (_activecell.Field.Name == "apply_to_doc")
            {
                cGlobals.ReturnParms.Clear();
                _CellEdit = true;
                InvoiceLookup il = new InvoiceLookup();
                il.ShowDialog();
                if (cGlobals.ReturnParms.Count > 0)
                {
                    gAllocation.ActiveRecord.Cells["apply_to_doc"].Value = cGlobals.ReturnParms[0].ToString();
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellNext);
                }
            }
            else if (_activecell.Field.Name == "receivable_account")
            {
                cGlobals.ReturnParms.Clear();
                _CellEdit = true;
                CustomerLookup cl = new CustomerLookup();
                cl.Init(new cBaseBusObject("CustomerLookup"));
                cl.ShowDialog();
                if (cGlobals.ReturnParms.Count > 0)
                {
                    gAllocation.ActiveRecord.Cells[5].Value = cGlobals.ReturnParms[0].ToString();
                }
            }
            cGlobals.ReturnParms.Clear();

        }
        public void GridDoubleClickDelegate()
        {
            //call customer document folder
            gRemit.ReturnSelectedData("batch_id");
            cGlobals.ReturnParms.Add("gRemit.xGrid");
            System.Windows.RoutedEventArgs args = new System.Windows.RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gRemit.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);

            // enable_button();
        }
        public void EditRemttance()
        {
            gRemit.xGrid.ExecuteCommand(DataPresenterCommands.CellFirstInRecord);
            gRemit.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            gRemit.xGrid.ActiveCell = (gRemit.xGrid.Records[gAllocation.ActiveRecord.Index] as DataRecord).Cells[1];
            CellValuePresenter.FromCell((gRemit.xGrid.Records[gAllocation.ActiveRecord.Index] as DataRecord).Cells[1]).StartEditMode();
        }
   
   
        public void SetRemitAllocation(int remit_id)
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_alloc"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item["remit_id"].ToString() == remit_id.ToString())
                {
                    alloctotal += decimal.Parse(item["amount"].ToString());
                }
            }
            gRemit.ActiveRecord.Cells["amnt_alloc"].Value = alloctotal;
        }
        public bool HasAllocation(int remit_id)
        {
            bool hasalloc = false;
            int alloctotal = 0;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_alloc"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item["remit_id"].ToString() == remit_id.ToString())
                {
                    alloctotal++;
                }
            }
            if (alloctotal > 0)
            {
                hasalloc = true;
            }
            return hasalloc;

        }
        public bool HasRemittance()
        {
            bool hasalloc = false;
            int alloctotal = 0;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_remit"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {

                    alloctotal++;
                }
            }
            if (alloctotal > 0)
            {
                hasalloc = true;
            }
            return hasalloc;

        }
        public decimal GetTotalRemit()
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_remit"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {
                    alloctotal += decimal.Parse(item["amount_functional"].ToString());

                }

            }
            return alloctotal;
        }
        public decimal GetFunctionalRemit()
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_remit"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {

                    alloctotal += decimal.Parse(item["amount_functional"].ToString());
                }
            }
            return alloctotal;
        }
        public decimal GetTotalAllocation()
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_alloc"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {

                    alloctotal += decimal.Parse(item["amount"].ToString());
                }
            }
            return alloctotal;
        }
        public void PostBind()
        {
            if (CurrentBusObj.ObjectData.Tables["cash_batch"].Rows.Count > 0)
            {
                _hasbound = false;

                _newobject = new BatchObject();
                _newobject.BatchID = Int32.Parse(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_id"].ToString());
                _newobject.AcctPeriod = DateTime.Parse(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["acct_period"].ToString());
                _newobject.BankID = Int32.Parse(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["bank_id"].ToString());
                _newobject.BatchDate = DateTime.Parse(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_date"].ToString());
                _newobject.BatchStatus = Int32.Parse(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_status"].ToString());
                _newobject.BatchTotal = decimal.Parse(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_total"].ToString());
                _newobject.CurrencyCode = CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["currency_code"].ToString();
                _newobject.SourceID = Int32.Parse(CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["source_id"].ToString());
                Change_Visibile();
                _hasbound = true;
              

           
            }
        }
        public bool _hasnoread = false;
        public bool _hasread = false;
        public void Change_Visibile()
        {
            

                 if ((_newobject.BatchStatus == 0))
                {
                    gRemit.xGrid.FieldSettings.AllowEdit = true;
                    gAllocation.xGrid.FieldSettings.AllowEdit = true;
                 
                    gAllocation.ContextMenuAddIsVisible = true;
                    gAllocation.ContextMenuGenericIsVisible1 = false;
                    gAllocation.ContextMenuGenericIsVisible2 = false;
                    gAllocation.ContextMenuGenericIsVisible3 = false;
                    gRemit.ContextMenuRemoveIsVisible = true;
                    gAllocation.ContextMenuRemoveIsVisible = true;
                    gAllocation.ContextMenuResetGridSettingsIsVisible = false;
                    gAllocation.ContextMenuSaveToExcelIsVisible = false;
                    gAllocation.ContextMenuSaveGridSettingsIsVisible = false;

                    gRemit.ContextMenuSaveGridSettingsIsVisible = false;
                    gRemit.ContextMenuAddIsVisible = true;
                    gRemit.ContextMenuGenericIsVisible1 = false;
                    gRemit.ContextMenuGenericIsVisible2 = false;
                    gRemit.ContextMenuGenericIsVisible3 = false;
                    gRemit.ContextMenuResetGridSettingsIsVisible = false;
                    gRemit.ContextMenuSaveToExcelIsVisible = false;


                }
                else if ((_newobject.BatchStatus == 1) )
                {
                    gRemit.xGrid.InitializeRecord += new EventHandler<Infragistics.Windows.DataPresenter.Events.InitializeRecordEventArgs>(xGrid_InitializeRecord);
                    gRemit.xGrid.FieldSettings.AllowEdit = false;
                    gAllocation.xGrid.FieldSettings.AllowEdit = false;
                    gAllocation.ContextMenuAddIsVisible = false;
                    gAllocation.ContextMenuGenericIsVisible1 = false;
                    gAllocation.ContextMenuGenericIsVisible2 = false;
                    gAllocation.ContextMenuGenericIsVisible3 = false;
                    gAllocation.ContextMenuRemoveIsVisible = false;
                    gAllocation.ContextMenuResetGridSettingsIsVisible = false;
                    gAllocation.ContextMenuSaveToExcelIsVisible = false;
                    gAllocation.ContextMenuSaveGridSettingsIsVisible = false;
            
                    gRemit.ContextMenuSaveGridSettingsIsVisible = false;
                    gRemit.ContextMenuAddIsVisible = false;
                    gRemit.ContextMenuGenericIsVisible1 = false;
                    gRemit.ContextMenuGenericIsVisible2 = false;
                    gRemit.ContextMenuGenericIsVisible3 = false;
                    gRemit.ContextMenuRemoveIsVisible = false;
                    gRemit.ContextMenuResetGridSettingsIsVisible = false;
                    gRemit.ContextMenuSaveToExcelIsVisible = false;
                }

            
        }
        public void SetChildSettings()
        {
            gRemit.xGrid.FieldLayouts.Clear();
            gAllocation.xGrid.FieldLayouts.Clear();
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            gRemit.MainTableName = "cash_remit";
            gRemit.ConfigFileName = "GridRemitUnits";
            gRemit.xGrid.FieldLayoutSettings = f;
            gRemit.SetGridSelectionBehavior(false, false);
            gRemit.xGrid.InitializeRecord += new EventHandler<Infragistics.Windows.DataPresenter.Events.InitializeRecordEventArgs>(xGrid_InitializeRecord);
            gAllocation.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            gRemit.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(gGrid_EditModeEnded);
            gAllocation.xGrid.CellUpdated += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs>(AllocGrid_CellUpdated);
            gAllocation.xGrid.EditModeStarted += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs>(aGrid_EditModeStarted);
            gAllocation.ContextMenuRemoveDelegate = RemoveAllocation;
            gRemit.ContextMenuRemoveDelegate = RemoveRemit;

            gAllocation.ContextMenuAddDelegate = InsertAllocation;
            gRemit.ContextMenuAddDelegate = AddRemittance;
            gAllocation.WindowZoomDelegate = GetDoubleClick;
            gRemit.WindowZoomDelegate = ReturnData;
            gRemit.FieldLayoutResourceString = "remits";
            gRemit.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "remit_id" }, ChildGrids = { gAllocation }, ParentFilterOnColumnNames = { "remit_id" } });
            gAllocation.MainTableName = "cash_alloc";
            gAllocation.ConfigFileName = "AllocGrid";
            gAllocation.FieldLayoutResourceString = "cash_allocgrid";
            gAllocation.SetGridSelectionBehavior(false, false);
            GridCollection.Add(gRemit);
            GridCollection.Add(gAllocation);

        }

        private List<string> dataKeys = new List<string> { "document_id","batch_id" };
        private void ReturnData()
        {
            
            gRemit.ReturnSelectedData(dataKeys);
            //cGlobals.ReturnParms.Add("gRemit.xGrid");
            cGlobals.ReturnParms[1] = ("GridCashZoom");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gRemit.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);     
        }
        private void RemoveRemit()
        {
            DataRecord r = gRemit.ActiveRecord;
            if (r != null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;
                int remit_row = Int32.Parse(row["remit_id"].ToString());
                if (row != null)
                {
                    row.Delete();
                    this.Save();
                }
            }
        }
        private void remove_allocation(int remit_id)
        {
            DataTable dt = new DataTable();
            foreach (DataRow dr in CurrentBusObj.ObjectData.Tables["cash_alloc"].Rows)
            {
                if (dr["remit_id"].ToString() != remit_id.ToString())
                {
                    dt.Rows.Add(dr);
                }
            }
            CurrentBusObj.ObjectData.Tables["cash_alloc"].Rows.Clear();
            foreach (DataRow item in dt.Rows)
            {
                CurrentBusObj.ObjectData.Tables["cash_alloc"].Rows.Add(item);
            }
        }
        private void RemoveAllocation()
        {
            DataRecord r = gAllocation.ActiveRecord;
            if (r != null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;

                if (row != null)
                {
                    row.Delete();
                }
            }
        }


        void xGrid_InitializeRecord(object sender, Infragistics.Windows.DataPresenter.Events.InitializeRecordEventArgs e)
        {
            DataRecord dr = (DataRecord)e.Record;
            int id_remit = Int32.Parse(dr.Cells["remit_id"].Value.ToString());
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["cash_alloc"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {

                if (item["remit_id"].ToString() == id_remit.ToString())
                {
                    alloctotal += decimal.Parse(item["amount"].ToString());
                }
                }
            }
            dr.Cells["amnt_alloc"].Value = alloctotal;

        }   



    }
    #endregion
}