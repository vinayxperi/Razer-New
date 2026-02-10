#region using statements

using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

#endregion

namespace Cash
{


    /// <summary>
    /// This class represents a 'CashBatchEntryTab' object.
    /// </summary>
    public partial class CashBatchEntryTab : ScreenBase, IScreen, IPreBindable
    {
        #region variables
        // new version 1.1
        public string WindowCaption { get; set; }
        private static readonly string fieldLayoutResource = "entry_remitgrid";
        private static readonly string mainTableName = "CashEntry";
        private static readonly string dataKey = "amount";
        private static readonly string gridTableName = "lookup_remit";
        private static readonly string CurrencyDisplayPath = "description";
        private static readonly string CurrencyValuePath = "currency_code";
        private DataRecord _currentAlloc = null;
        private int _remitid = 100;
        private int _batchid = -1;
        private int _allocid = 200;
        private int _remitnumber = 1;
        private string _currencycode = "";
        private decimal _exchangerate = 0.0M;
        private int _tabindex = 8;
        public string compcode;
        private decimal alloc_total = 0.0M;
        private decimal _remittotal = 0.0M;
        public decimal _currenttotal = 0M;

        public decimal _loadtotal = 0M;

        public decimal _rollingalloc = 0M;
        public decimal _currentalloc = 0M;

        public ComboBoxItemsProvider cmbCurrency { get; set; }
        public ComboBoxItemsProvider cmbProducts { get; set; }


        private int _allocindex = 0;
        int _currentindex = 0;
        private bool _CellEdit = true;
        public bool _isnew = false;
        public bool _editmode = false;
        public bool _isnewrow = true;
        public bool _isnewalloc = false;
        #endregion

        /// <summary>
        /// Create a new instance of a 'CashBatchEntryTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CashBatchEntryTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Perform initializations for this object
        }


        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            this.gRemittance.PreviewKeyDown += new KeyEventHandler(gRemittance_PreviewKeyDown);
            gRemittance.GridGotFocusDelegate = gRemittanceGotFocus;
            this.ltbBatchTotal.PreviewKeyDown += new KeyEventHandler(ltbBatchTotal_PreviewKeyDown);
            this.CancelCloseOnSaveConfirmation = true;
            this.CurrentBusObj = businessObject;
            businessObject.Parms.AddParm("@batch_id", _batchid);
            businessObject.Parms.AddParm("@document_id", -1);
            businessObject.Parms.AddParm("@date_type", "ACCTPERIOD");
            this.MainTableName = "entry_cash";
            SetParentChildAttributes();
            this.Load(businessObject);
            gRemittance.LoadGrid(businessObject, mainTableName);
            gAllocation.LoadGrid(businessObject, "lookup_alloc");
            DataRow dbr = this.CurrentBusObj.ObjectData.Tables["entry_cash"].NewRow();
            this.CurrentBusObj.ObjectData.Tables["entry_cash"].Rows.Add(dbr);
            lktbBankID.Focusable = true;
            lktbBankID.Focus();
            lktbBankID.CntrlFocus();

        }

        /// <summary>
        /// used to setup grid for tabbing through editable fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gRemittance_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Tab)
            //{
            //    RazerBase.ucBaseGrid grid = (RazerBase.ucBaseGrid)sender;
            //    if (grid.xGrid.ActiveCell != null)
            //    {
            //        if (grid.xGrid.ActiveCell.Field.Name == "remit_id")
            //        {
            //            //grid.ExecuteCommand(DataPresenterCommands.CellLastInRecord);
            //            grid.xGrid.ExecuteCommand(DataPresenterCommands.CellNext);
            //            grid.xGrid.ActiveCell.IsActive = true;
            //            grid.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            //            e.Handled = true;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// used to setup grid for tabbing through editable fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ltbBatchTotal_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (ltbBatchTotal.Text != "")
            //{
            //    if (e.Key == Key.Tab)
            //    {
            //        gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
            //        gRemittance.xGrid.ActiveCell.IsActive = true;
            //        gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            //        //e.Handled = true;
            //    }
            //}
        }

        void gGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_remit"];
            DataRow dr1 = dt.Rows[gRemittance.ActiveRecord.Index];

            int edit_index = gRemittance.ActiveRecord.Cells.IndexOf(e.Cell);
            int row_index = gRemittance.ActiveRecord.Index;
            DataRecord _currentRemit = gRemittance.ActiveRecord;

            if (edit_index == 1)
            {
                if (_currentRemit.Cells[3].Value.ToString() != "0")
                {
                    _isnewrow = false;
                }
                else
                {
                    dr1[5] = Int32.Parse(e.Cell.Value.ToString());
                }
            }
            else if (edit_index == 4)
            {

                _loadtotal = (decimal)e.Cell.Value;
                decimal _batchtotal = decimal.Parse(ltbBatchTotal.Text.Replace("$", ""));
                _currenttotal = 0M;

                if (_currencycode == "USD")
                {
                    dr1[7] = _loadtotal.ToString();
                }
                foreach (DataRow dr in dt.Rows)
                {
                    _currenttotal += (decimal)dr["amount_functional"];
                }
                if ((_currenttotal) > _batchtotal)
                {
                    e.Cell.Value = 0;
                    Messages.ShowError("You cannot remit more than the batch total");
                }
                else
                {
                    if (_loadtotal > 0)
                    {
                        ltbRemitTotal.Text = "$" + (_currenttotal).ToString();
                        _isnewalloc = true;
                        InsertAllocation();
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

        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            int edit_index = 0;
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
                if (gAllocation.ActiveRecord.Cells["unapplied_flag"].Value.ToString() == "1")
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
                    DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_alloc"];
                    DataRowView dr = _currentAlloc.DataItem as DataRowView;
                    DataView dv = dr.DataView;
                    DataRow dr1 = dt.Rows[_currentAlloc.Index];


                    decimal dc = decimal.Parse(_currentAlloc.Cells["amount"].Value.ToString().Replace("$", ""));
                    decimal RemitTotal = (AllocationByRemit(Int32.Parse(gRemittance.ActiveRecord.Cells["remit_id"].Value.ToString())));
                    decimal AllocationTotal = GetAlloc(Int32.Parse(gRemittance.ActiveRecord.Cells["remit_id"].Value.ToString()));

                    if (AllocationTotal > RemitTotal)
                    {
                        Messages.ShowError("Allocation cannot be more than the remit total");
                        return;
                    }
                    else
                    {
                        _rollingalloc += dc;
                        alloc_total += dc;
                        _currentalloc += dc;
                    }


                    if (RemitTotal > AllocationTotal)
                    {
                        SetRemitAllocation(Int32.Parse(gRemittance.ActiveRecord.Cells["remit_id"].Value.ToString()));
                        ltbAllocatedTotal.Text = alloc_total.ToString();
                        _CellEdit = true;
                        if (gRemittance.ActiveRecord.Cells[5].Value != gRemittance.ActiveRecord.Cells[4].Value)
                        {
                            _currentindex++;
                            gAllocation.Focus();
                            System.Windows.Input.FocusManager.SetFocusedElement(CashGrid, gAllocation);
                            InsertAllocation();
                        }


                    }
                    else if (GetFunctionalRemit() < decimal.Parse(ltbBatchTotal.Text.Replace("$", "")))
                    {
                        SetRemitAllocation(Int32.Parse(gRemittance.ActiveRecord.Cells["remit_id"].Value.ToString()));
                        ltbAllocatedTotal.Text = alloc_total.ToString();
                        _remitid++;
                        _currentindex++;
                        _currentalloc = 0;
                        _CellEdit = true;
                        if (GetFunctionalRemit() != decimal.Parse(ltbBatchTotal.Text.Replace("$", "")))
                        {
                            System.Windows.Input.FocusManager.SetFocusedElement(CashGrid, gRemittance);

                            AddRemittance();
                        }
                    }
                    else
                    {
                        SetRemitAllocation(Int32.Parse(gRemittance.ActiveRecord.Cells["remit_id"].Value.ToString()));
                        ltbAllocatedTotal.Text = alloc_total.ToString();
                        _currentindex++;
                        _CellEdit = true;
                    }
                }
            }
        }

        private bool _haveshown = false;
        void Editor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int x = (int)e.NewValue;

            gAllocation.ActiveRecord.Cells["unapplied_flag"].Value = x;
            if ((x == 1) && (!_haveshown))
            {
                decimal _remital = AllocationByRemit(Int32.Parse(gAllocation.ActiveRecord.Cells["remit_id"].Value.ToString()));
                decimal _allocal = GetAlloc(Int32.Parse(gAllocation.ActiveRecord.Cells["remit_id"].Value.ToString()));
                gAllocation.ActiveRecord.Cells["amount"].Value = _remital - _allocal;

                if (gAllocation.ActiveRecord.Cells["product_code"].Value.ToString().Length < 1)
                {
                    Messages.ShowError("You must choose a product code if allocation is unapplied");
                    gAllocation.ActiveRecord.Cells["unapplied_flag"].Value = 0;
                    _haveshown = true;
                    return;
                }
                if ((gAllocation.ActiveRecord.Cells["receivable_account"].Value.ToString().Length < 1))
                {
                    Messages.ShowError("You must choose a customer id if allocation is unapplied");
                    gAllocation.ActiveRecord.Cells["unapplied_flag"].Value = 0;
                    _haveshown = true;
                    return;
                }
                if (gAllocation.ActiveRecord.Cells["apply_to_doc"].Value.ToString().Length > 0)
                {
                    Messages.ShowError("Unapplied cannot be chosen if you have an invoice number.");
                    _currentAlloc.Cells["unapplied_flag"].Value = 0;
                    _haveshown = true;
                    return;


                }
            }
            gAllocation.Focus();

        }

        public override void New()
        {
            _batchid = -1;
            this.CurrentBusObj.Parms.ClearParms();
            this.CurrentBusObj.Parms.AddParm("@batch_id", _batchid);
            CurrentBusObj.Parms.AddParm("@document_id", "");
            CurrentBusObj.Parms.AddParm("@date_type", "ACCTPERIOD");
            _remittotal = 0;
            _allocindex = 0;
            alloc_total = 0;
            _currenttotal = 0;
            _currentindex = 0;
            _currencycode = "";
            _currentalloc = 0;
            _remitid = 100;
            _remitnumber = 1;
            _remittotal = 0M;
            _rollingalloc = 0;
            ltbAllocatedTotal.Text = "";
            ltbRemitTotal.Text = "";
            this.Load();
        }

        void AllocGrid_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            if ((_CellEdit) && (gAllocation.ActiveRecord != null))
            {
                _CellEdit = false;
                DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_alloc"];
                //  DataRow dr1 = dt.Rows[gAllocation.ActiveRecord.Index];
                DataRecord _dr = gAllocation.ActiveRecord;
                int g = gAllocation.ActiveRecord.Cells.IndexOf(e.Cell);
                string invoice_number = "";
                if (g == 0)
                {
                    invoice_number = e.Cell.Value.ToString();
                    CurrentBusObj.Parms.ParmList.Clear();
                    this.CurrentBusObj.Parms.AddParm("@batch_id", _batchid);
                    CurrentBusObj.Parms.AddParm("@document_id", invoice_number);
                    CurrentBusObj.Parms.AddParm("@date_type", "ACCTPERIOD");
                    this.Load("entry_invoice");

                    DataTable mainalloc = (DataTable)CurrentBusObj.ObjectData.Tables["entry_invoice"];
                    //    gAllocation.ActiveRecord.Cells[3].IsActive = false;
                    if (mainalloc.Rows.Count > 0)
                    {
                        DataRecord Drecord = gRemittance.ActiveRecord;
                        if (Int32.Parse(mainalloc.Rows[0]["row_count"].ToString()) > 1)
                        {
                            this.Load("entry_invoicerows");
                            //   gAllocation.ActiveRecord.Cells[3].EditorType.is = false;
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
                                gRemittance.ActiveRecord = Drecord;
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
                                gRemittance.ActiveRecord = Drecord;
                            }
                            // string Messages = "";



                            gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.RecordLastDisplayed);

                            _CellEdit = true;


                        }
                        else
                        {


                            _dr.Cells[2].Value = mainalloc.Rows[0]["product_code"].ToString();
                            _dr.Cells[5].Value = mainalloc.Rows[0]["receivable_account"].ToString();
                            _dr.Cells[4].Value = mainalloc.Rows[0]["company_code"].ToString();
                            _dr.Cells[1].Value = Int32.Parse(mainalloc.Rows[0]["seq_code"].ToString());
                            Decimal amt = Decimal.Parse(mainalloc.Rows[0]["open_amount"].ToString().ToString().Replace("$", ""));
                            if (amt > 0)
                            {
                                _dr.Cells[6].Value = amt;
                            }
                            _CellEdit = true;
                            gAllocation.LoadGrid(CurrentBusObj, "lookup_alloc");
                            gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.RecordLastDisplayed);
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
                else if (g == 3)
                {
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);

                }
                else if (g == 5)
                {



                }
                else if (g == 6)
                {
                    // Check whether to add an alloc or add a remit or do nothing.
                    if (Int32.Parse(_dr.Cells["unapplied_flag"].Value.ToString()) == 1)
                    {
                        if (_dr.Cells[5].Value.ToString().Length < 1)
                        {
                            Messages.ShowError("You must choose a receivable account if allocation is unapplied");
                            return;
                        }
                        if (_dr.Cells[2].Value.ToString().Length < 1)
                        {
                            Messages.ShowError("You must choose a product code if allocation is unapplied");
                            return;
                        }

                    }




                }

            }

        }

        public string SetCurrencyCode()
        {
            DataTable dcr = this.CurrentBusObj.ObjectData.Tables["lookup_bank"];
            foreach (DataRow item in dcr.Rows)
            {
                if (item["bank_id"].ToString() == lktbBankID.SelectedValue.ToString())
                {
                    _currencycode = item["currency_code"].ToString();

                }

            }
            return _currencycode;
        }

        ///// <summary>
        /// Override of save method handles save functionality for folder
        /// </summary>
        public override void Save()
        {
            decimal dTemp = 0.0M;
            decimal.TryParse(ltbBatchTotal.Text, out dTemp);
            if (dTemp == null || dTemp <= 0.0M)
            {
                MessageBox.Show("Batch must have a valid non zero value in the batch total field.");
                return;
            }

            if (ltbBatchDate.SelText == null || Convert.ToString(ltbBatchDate.SelText) == "1/1/1900")
            {
                MessageBox.Show("Batch must have a valid batch date.");
                return;
            }

            decimal final_total = decimal.Parse(ltbBatchTotal.Text.Replace("$", ""));


            decimal final_remit = 0M;
            if (string.IsNullOrEmpty(ltbRemitTotal.Text))
            {
                final_remit = 0M;
            }
            else
            {
                final_remit = decimal.Parse(ltbRemitTotal.Text.Replace("$", ""));
            }
            decimal final_alloc = 0M;
            final_alloc = GetTotalAllocation();
            bool isvalid = true;
            string error_message = "";
            if (GetFunctionalRemit() != final_total)
            {
                isvalid = false;
                error_message = "Remittance must equal batch total";
            }
            if (GetTotalAllocation() != GetTotalRemit())
            {
                isvalid = false;
                error_message = "Allocation must equal total Remit";
            }
            foreach (DataRow item in this.CurrentBusObj.ObjectData.Tables["lookup_alloc"].Rows)
            {
                if (item["unapplied_flag"].ToString() == "1")
                {
                    if (item["apply_to_doc"].ToString().Length > 0)
                    {
                        isvalid = false;
                        error_message = "Cannot allocate an unapplied with an invoice number";
                    }
                    if (item["product_code"].ToString().Length < 1)
                    {
                        isvalid = false;
                        error_message = "You must choose a product for an unapplied allocation";
                    }
                    if (item["receivable_account"].ToString().Length < 1)
                    {
                        isvalid = false;
                        error_message = "You must select a customer id for an unapplied allocation";
                    }
                }
            }
            if (isvalid)
            {
                CanExecuteCloseCommand = true;
                DateTime dt = new DateTime();
                DataTable datTime = this.CurrentBusObj.ObjectData.Tables["cash_date"];
                dt = Convert.ToDateTime(datTime.Rows[0]["date_value"].ToString());
                int _finalbatch;
                _finalbatch = cGlobals.BillService.InsertCashEntry((int)ltbSourceID.SelectedValue, (DateTime)ltbBatchDate.SelText, (int)lktbBankID.SelectedValue, decimal.Parse(ltbBatchTotal.Text.Replace("$", "")), SetCurrencyCode(), dt);

                foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["lookup_remit"].Rows)
                {
                    if (!string.IsNullOrEmpty(dr["remit_number"].ToString()))
                    {
                        int _finalremit;
                        string newinvoicenumber = cGlobals.BillService.GetNextInvoiceNumber("CASH");
                        decimal _exchange = GetExchangeRate(dr["currency_code"].ToString());

                        decimal bankcharge = ((decimal.Parse(dr["amount"].ToString()) * _exchange) - decimal.Parse(dr["amount_functional"].ToString()));
                        _finalremit = cGlobals.BillService.InsertRemitEntry(_finalbatch, Int32.Parse(dr["remit_id"].ToString()), newinvoicenumber, DateTime.Now, dr["remit_number"].ToString(), decimal.Parse(dr["amount"].ToString()), decimal.Parse(dr["amount_functional"].ToString()), dr["currency_code"].ToString(), (Double)_exchange, bankcharge);
                        SubmitAlloc(_finalbatch, _finalremit, Int32.Parse(dr["remit_id"].ToString()));
                    }
                    //Insert Batch.
                    //Insert Alloc
                }
                Messages.ShowInformation("Batch Entry # - " + _finalbatch.ToString() + " - has been created.");
                this.New();
            }
            else
            {
                Messages.ShowError(error_message);


            }
        }

        private void SubmitAlloc(int batchid, int remitid, int origremit)
        {
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["lookup_alloc"].Rows)
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

        private void ltbBatchTotal_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((ltbBatchTotal.Text == "0") || (string.IsNullOrEmpty(ltbBatchTotal.Text)))
            {
                ltbBatchTotal.Text = "";
                _isnew = true;
            }
        }

        private void ltbBatchTotal_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {


        }

        private decimal GetExchangeRate(string currency_code)
        {

            decimal _returnexchange = 0.0M;
            if (currency_code != "USD")
            {
                DataTable dcr = this.CurrentBusObj.ObjectData.Tables["lookup_currency_history"];
                var currency = from x in dcr.AsEnumerable()
                               where x.Field<string>("from_currency") == currency_code
                               && x.Field<string>("to_currency") == SetCurrencyCode() && x.Field<DateTime>("conversion_date") == ltbBatchDate.SelText
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
        private void SetParentChildAttributes()
        {
            //Establish the WHT Tracking Grid
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            gRemittance.MainTableName = "lookup_remit";
            gRemittance.ConfigFileName = "GridRemitUnits";
            gRemittance.xGrid.FieldLayoutSettings = f;
            gRemittance.SetGridSelectionBehavior(false, true);
            gRemittance.FieldLayoutResourceString = "remits";
            gRemittance.xGrid.FieldSettings.AllowEdit = true;

            gAllocation.ContextMenuAddIsVisible = false;
            gAllocation.ContextMenuGenericIsVisible1 = false;
            gAllocation.ContextMenuGenericIsVisible2 = false;
            gAllocation.ContextMenuGenericIsVisible3 = false;
            gAllocation.ContextMenuRemoveIsVisible = false;
            gAllocation.ContextMenuResetGridSettingsIsVisible = false;
            gAllocation.ContextMenuSaveToExcelIsVisible = false;
            gAllocation.ContextMenuSaveGridSettingsIsVisible = false;

            gRemittance.ContextMenuSaveGridSettingsIsVisible = false;
            gRemittance.ContextMenuAddIsVisible = false;
            gRemittance.ContextMenuGenericIsVisible1 = false;
            gRemittance.ContextMenuGenericIsVisible2 = false;
            gRemittance.ContextMenuGenericIsVisible3 = false;
            gRemittance.ContextMenuRemoveIsVisible = false;
            gRemittance.ContextMenuResetGridSettingsIsVisible = false;
            gRemittance.ContextMenuSaveToExcelIsVisible = false;
            gAllocation.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);
            gRemittance.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(gGrid_EditModeEnded);
            //gRemittance.xGrid.EditModeStarted += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs>(xGrid_EditModeStarted);            
            gRemittance.xGrid.CellActivated += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellActivatedEventArgs>(xGrid_CellActivated);
            gRemittance.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "remit_id" }, ChildGrids = { gAllocation }, ParentFilterOnColumnNames = { "remit_id" } });
            gAllocation.MainTableName = "lookup_alloc";
            gAllocation.ConfigFileName = "AllocGrid";
            gAllocation.FieldLayoutResourceString = "cash_allocgrid";
            gAllocation.xGrid.CellUpdated += new EventHandler<Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs>(AllocGrid_CellUpdated);
            gAllocation.xGrid.EditModeStarted += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs>(aGrid_EditModeStarted);
            // gAllocation.ContextMenuAddDelegate = AllocGridAddDelegate;
            gAllocation.SetGridSelectionBehavior(false, false);
            gAllocation.xGrid.FieldSettings.AllowEdit = true;
            gAllocation.WindowZoomDelegate = GetDoubleClick;

            GridCollection.Add(gRemittance);
            GridCollection.Add(gAllocation);
        }

        private object ReturnSelectedData()
        {
            throw new NotImplementedException();
        }

        void xGrid_CellActivated(object sender, Infragistics.Windows.DataPresenter.Events.CellActivatedEventArgs e)
        {
            if (e.Cell.Field.Name == "remit_number")
            {
                //System.Windows.Forms.SendKeys.SendWait("{F2}");
            }
        }

        void xGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        //void xGrid_EditModeStarted(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs e)
        //{
        //    if (e.Cell.Field.Name == "remit_number")
        //    {
        //        //((XamNumericEditor)e.Editor).StartEditMode();
        //        //System.Windows.Forms.SendKeys.SendWait("{F2}");
        //    }
        //}
        void aGrid_EditModeStarted(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs e)
        {
            if (e.Cell.Field.Name == "apply_to_doc")
            {
                //clb 01.26.12
                XamTextEditor editor = (XamTextEditor)e.Editor;
                editor.StartEditMode();
            }
            if (e.Cell.Field.Name == "product_code")
            {
                if (movenext)
                {
                    movenext = false;
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellPreviousByTab);
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellPreviousByTab);


                }
            }


        }

        private void AddRemittance()
        {

            _allocindex = 0;

            DataRow dr = this.CurrentBusObj.ObjectData.Tables["lookup_remit"].NewRow();
            //add contact_id
            dr["batch_id"] = _batchid;
            dr["remit_id"] = _remitid;
            dr["document_id"] = "";
            dr["remit_type_id"] = _remitnumber;
            dr["remit_date"] = DateTime.Now;
            dr["remit_number"] = "";
            dr["amount"] = 0.0M;
            dr["amount_functional"] = 0.0M;
            dr["currency_code"] = "USD";
            dr["exchange_rate"] = 0.0M;
            dr["bank_charge_amount"] = 0.0M;
            this.CurrentBusObj.ObjectData.Tables["lookup_remit"].Rows.Add(dr);
            _remitnumber++;
            _remitid++;
            gRemittance.xGrid.FieldSettings.AllowEdit = true;
            gRemittance.LoadGrid(CurrentBusObj, "lookup_remit");
            //gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.RecordLastOverall);                
            //gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.CellFirstInRecord);
            (gRemittance.xGrid.Records[gRemittance.ActiveRecord.Index] as DataRecord).Cells["remit_number"].IsActive = true;
            //gRemittance.xGrid.ActiveCell.IsInEditMode = true;
            gRemittance.xGrid.Records.DataPresenter.BringCellIntoView(gRemittance.xGrid.ActiveCell);

            //gRemittance.xGrid.ActiveCell = (gRemittance.xGrid.Records[gRemittance.ActiveRecord.Index] as DataRecord).Cells["remit_number"];
            gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            //CellValuePresenter.FromCell((gRemittance.xGrid.Records[gRemittance.ActiveRecord.Index] as DataRecord).Cells[1]).StartEditMode();
            //gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);
            //gRemittance.xGrid.ActiveCell = (gRemittance.xGrid.Records[dataSource.Count - 1] as DataRecord).Cells[1];
            //gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.RecordNext);
        }
        public CellValuePresenter GetCellValuePresenter(int rowIndex, int colIndex)
        {

            DataRecord dr = this.gRemittance.xGrid.Records[rowIndex] as DataRecord;
            return CellValuePresenter.FromCell(dr.Cells[colIndex]);
        }


        public void PreBind()
        {

            if (this.CurrentBusObj.HasObjectData)
            {
                cmbCurrency = new ComboBoxItemsProvider();
                cmbCurrency.ItemsSource = this.CurrentBusObj.ObjectData.Tables["lookup_currency"].DefaultView;
                cmbCurrency.ValuePath = CurrencyValuePath;
                cmbCurrency.DisplayMemberPath = CurrencyValuePath;
                cmbProducts = new ComboBoxItemsProvider();
                cmbProducts.ItemsSource = CurrentBusObj.ObjectData.Tables["entry_products"].DefaultView;
                cmbProducts.ValuePath = "product_code";
                cmbProducts.DisplayMemberPath = "product_code";
                this.lktbBankID.SetBindingExpression("bank_id", "bank_name", this.CurrentBusObj.GetTable("lookup_bank") as DataTable, "");
                ltbSourceID.SetBindingExpression("source_id", "description", this.CurrentBusObj.GetTable("lookup_source") as DataTable, "");
            }
        }

        private void InsertAllocation()
        {

            if (!_editmode)
            {
                if (string.IsNullOrEmpty(ltbAllocatedTotal.Text))
                {
                    ltbAllocatedTotal.Text = "0";
                }
                DataRow dr = this.CurrentBusObj.ObjectData.Tables["lookup_remit"].Rows[gRemittance.ActiveRecord.Index];
                DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_alloc"];
                int _currentremit = 0;
                _currentremit = Int32.Parse(dr["remit_id"].ToString());
                decimal _remittotal = decimal.Parse(dr["amount"].ToString());
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
                decimal Batch_Total = Decimal.Parse(ltbBatchTotal.Text.Replace("$", ""));
                decimal Remit_total = Decimal.Parse(ltbRemitTotal.Text.Replace("$", ""));
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

                    if (canadd & (_alloctotal < Remit_total))
                    {
                        DataRow dr1 = this.CurrentBusObj.ObjectData.Tables["lookup_alloc"].NewRow();
                        dr1["batch_id"] = _batchid;
                        dr1["remit_id"] = gRemittance.ActiveRecord.Cells["remit_id"].Value;
                        dr1["remit_alloc_id"] = _allocid;
                        dr1["apply_to_doc"] = "";
                        dr1["apply_to_seq"] = 0;
                        dr1["amount"] = 0.0M;
                        dr1["company_code"] = 0;
                        dr1["receivable_account"] = "";
                        dr1["product_code"] = "";
                        dr1["unapplied_flag"] = 0;
                        dr1["datarowindex"] = _allocindex;
                        this.CurrentBusObj.ObjectData.Tables["lookup_alloc"].Rows.Add(dr1);
                        _allocindex++;
                        _allocid++;
                        gAllocation.LoadGrid(CurrentBusObj, "lookup_alloc");
                    }
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.RecordLastOverall);
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellFirstInRecord);
                    movenext = true;
                    CellValuePresenter.FromCell((gAllocation.xGrid.Records[gAllocation.ActiveRecord.Index] as DataRecord).Cells[0]).StartEditMode();
                    gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellNextByTab);
                }
            }
            else
            {
                gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.RecordFirstOverall);
                gAllocation.xGrid.ExecuteCommand(DataPresenterCommands.CellFirstInRecord);
                int remitid = Int32.Parse((gAllocation.xGrid.Records[gAllocation.ActiveRecord.Index] as DataRecord).Cells["remit_id"].Value.ToString());
                decimal remittotal = Int32.Parse((gRemittance.xGrid.Records[gRemittance.ActiveRecord.Index] as DataRecord).Cells["amount"].Value.ToString());
                if (AllocationByRemit(remitid) == remittotal)
                {
                    CellValuePresenter.FromCell((gAllocation.xGrid.Records[gAllocation.ActiveRecord.Index] as DataRecord).Cells[0]).StartEditMode();
                }
                else
                {
                    _editmode = false;
                }
            }
        }
        public bool movenext = false;
        public decimal AllocationByRemit(int remit_id)
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_remit"] as DataTable;
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
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_alloc"] as DataTable;
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
                InvoiceLookup il = new InvoiceLookup();
                il.ShowDialog();
                if (cGlobals.ReturnParms.Count > 0)
                {
                    gAllocation.ActiveRecord.Cells["apply_to_doc"].Value = cGlobals.ReturnParms[0].ToString();
                }
            }
            else if (_activecell.Field.Name == "receivable_account")
            {
                cGlobals.ReturnParms.Clear();
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
            gRemittance.ReturnSelectedData("batch_id");
            cGlobals.ReturnParms.Add("gRemittance.xGrid");
            System.Windows.RoutedEventArgs args = new System.Windows.RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gRemittance.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);

            // enable_button();
        }
        public void EditRemttance()
        {
            gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.CellFirstInRecord);
            gRemittance.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            gRemittance.xGrid.ActiveCell = (gRemittance.xGrid.Records[gAllocation.ActiveRecord.Index] as DataRecord).Cells[1];
            CellValuePresenter.FromCell((gRemittance.xGrid.Records[gAllocation.ActiveRecord.Index] as DataRecord).Cells[1]).StartEditMode();
        }
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }

        private void ltbSourceID_GotFocus(object sender, RoutedEventArgs e)
        {
            DataTable dr = this.CurrentBusObj.ObjectData.Tables["lookup_remit"];
            if (dr.Rows.Count > 0)
            {
                ucBaseGrid bg = new ucBaseGrid();
                bg = FindChild<ucBaseGrid>(Application.Current.MainWindow, "gRemittance");
                bg.Focus();
            }
        }
        public void SetRemitAllocation(int remit_id)
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_alloc"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {
                if (item["remit_id"].ToString() == remit_id.ToString())
                {
                    alloctotal += decimal.Parse(item["amount"].ToString());
                }
            }
            gRemittance.ActiveRecord.Cells["amnt_alloc"].Value = alloctotal;
        }
        public bool HasAllocation(int remit_id)
        {
            bool hasalloc = false;
            int alloctotal = 0;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_alloc"] as DataTable;
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
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_remit"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {

                alloctotal++;

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
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_remit"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {

                alloctotal += decimal.Parse(item["amount"].ToString());

            }
            return alloctotal;
        }
        public decimal GetFunctionalRemit()
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_remit"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {

                alloctotal += decimal.Parse(item["amount_functional"].ToString());

            }
            return alloctotal;
        }
        public decimal GetTotalAllocation()
        {
            decimal alloctotal = 0M;
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["lookup_alloc"] as DataTable;
            foreach (DataRow item in dt.Rows)
            {

                alloctotal += decimal.Parse(item["amount"].ToString());

            }
            return alloctotal;
        }

        public void gRemittanceGotFocus(object sender, RoutedEventArgs e)
        {
            if (!HasRemittance())
            {
                AddRemittance();
            }
        }




    }
}