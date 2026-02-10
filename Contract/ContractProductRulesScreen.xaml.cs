using RazerBase;
using RazerInterface;
using System;
using System.Windows;
using System.Data;
using Infragistics.Windows.Editors;
using System.Windows.Input;
using Infragistics.Windows.DataPresenter;
using System.Collections.Generic;

namespace Contract
{
    /// <summary>
    /// Used for correctly calculating month diff btwn two dates
    /// </summary>
    public struct DateTimeSpan
    {
        private readonly int years;
        private readonly int months;
        private readonly int days;
        private readonly int hours;
        private readonly int minutes;
        private readonly int seconds;
        private readonly int milliseconds;

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            this.years = years;
            this.months = months;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.milliseconds = milliseconds;
        }

        public int Years { get { return years; } }
        public int Months { get { return months; } }
        public int Days { get { return days; } }
        public int Hours { get { return hours; } }
        public int Minutes { get { return minutes; } }
        public int Seconds { get { return seconds; } }
        public int Milliseconds { get { return milliseconds; } }

        enum Phase { Years, Months, Days, Done }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {

            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date2;
            int years = 0;
            int months = 0;
            int days = 0;

            //Phase phase = Phase.Years;
            Phase phase = Phase.Months;
            DateTimeSpan span = new DateTimeSpan();

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.Year == 1 || current.AddYears(-1) < date1)
                        {
                            phase = Phase.Months;
                        }
                        else
                        {
                            current = current.AddYears(-1);
                            years++;
                        }
                        break;
                    case Phase.Months:
                        if (current.AddMonths(-1) < date1)
                        {
                            phase = Phase.Days;
                        }
                        else
                        {
                            current = current.AddMonths(-1);
                            months++;
                        }
                        break;
                    case Phase.Days:
                        if (current.AddDays(-1) < date1)
                        {
                            var timespan = current - date1;
                            span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                            phase = Phase.Done;
                        }
                        else
                        {
                            current = current.AddDays(-1);
                            days++;
                        }
                        break;
                }
            }

            return span;

        }
    }

    /// <summary>
    /// This class represents a 'ContractProductRulesScreen' object.
    /// </summary>
    public partial class ContractProductRulesScreen : ScreenBase, IPreBindable
    {
        #region class vars
        private string prodDesc = "";

        //RES 11/18/19 Flag to indicate if a cumulative rate exists
        //public bool CUMFlag = true;

        public DateTime currentStartDate { get; set; }
        public DateTime currentEndDate { get; set; }
        //flag for determining if screen in insert mode
        public bool IsInserting { get; set; }
        //used for tier_start_date binding
        private bool mTierPeriodBool = false;
        private DataRow rDefault; //Default data for rule
        private DataRow rCombine; //Combined invoice line rule
        public bool TierPeriodBool
        {
            get { return mTierPeriodBool; }
            set { mTierPeriodBool = value; }
        }
        
        private bool mIsStartDateLoaded = false;
        public bool IsStartDateLoaded
        {
            get
            {
                return mIsStartDateLoaded;
            }
            set
            {
                mIsStartDateLoaded = value;
            }
        }

        private bool mIsEndDateLoaded = false;
        public bool IsEndDateLoaded
        {
            get
            {
                return mIsEndDateLoaded;
            }
            set
            {
                mIsEndDateLoaded = value;
            }
        }

        //for rules group grid
        public ComboBoxItemsProvider cmbRulesGroupCombo { get; set; }

        //contract object from caller
        cBaseBusObject ContractObj;

        //used for determining last record in Tiers grid
        private int TiersActiveRecord { get; set; }

        //property for to chk if last cell in row is active
        private bool IsOnLastCellInRow { get; set; }

        private ucBaseGrid gRates;
        private ucBaseGrid gRules;

        #endregion

        /// <summary>
        /// Create a new instance of a 'ContractProductRulesScreen' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractProductRulesScreen(cBaseBusObject rulesObj, bool _IsInserting, cBaseBusObject _ContractObj, ucBaseGrid _gRates, ucBaseGrid _gRules)
            : base()
        {
            //set curr bus obj
            this.CurrentBusObj = rulesObj;
            //get handle to contract obj
            ContractObj = _ContractObj;
            //get handle to ContractRatesTab
            gRates = _gRates;
            //get handle to ContractRulesTab
            gRules = _gRules;
            //do this so flag will be set from caller in proper sequence, cannot set property in caller
            this.IsInserting = _IsInserting;
            //this.DataContext = rulesObj;
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }


        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            //InputBindings.Add(new KeyBinding(btnSave, new KeyGesture(Key.S, ModifierKeys.Control));
            MainTableName = "product_rule";
            
            //setup RuleGroup grid
            GridRuleGroup.xGrid.FieldLayoutSettings.AllowAddNew = true;
            GridRuleGroup.xGrid.FieldLayoutSettings.AllowDelete = true;
            GridRuleGroup.xGrid.FieldLayoutSettings.AddNewRecordLocation = Infragistics.Windows.DataPresenter.AddNewRecordLocation.OnTop;
            GridRuleGroup.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridRuleGroup.ContextMenuAddIsVisible = false;
            GridRuleGroup.ContextMenuRemoveIsVisible = false;
            GridRuleGroup.xGrid.FieldSettings.AllowEdit = true;
            GridRuleGroup.MainTableName = "rule_group";
            GridRuleGroup.ConfigFileName = "RuleGroup";
            GridRuleGroup.SetGridSelectionBehavior(false, true);
            GridRuleGroup.FieldLayoutResourceString = "GridRuleGroups";
            GridCollection.Add(GridRuleGroup);
            //setup tiers grid
            GridTiers.xGrid.FieldLayoutSettings.AllowDelete = true;
            GridTiers.xGrid.FieldLayoutSettings.AddNewRecordLocation = Infragistics.Windows.DataPresenter.AddNewRecordLocation.OnBottom;
            GridTiers.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridTiers.ContextMenuAddIsVisible = true;

            GridTiers.ContextMenuAddDelegate = TiersContextMenuAdd;

            GridTiers.ContextMenuRemoveIsVisible = true;
            GridTiers.ContextMenuRemoveDisplayName = "Delete Tier";
            GridTiers.ContextMenuRemoveDelegate = TiersGridRemovingDelegate;
            
            GridTiers.EditModeEndedDelegate = GridTiers_EditModeEnded; //This allows for data checks after each cell is exited
            GridTiers.RecordActivatedDelegate = GridTiers_RecordActivated;
            GridTiers.xGrid.FieldSettings.AllowEdit = true;
            GridTiers.MainTableName = "rule_detail";
            GridTiers.ConfigFileName = "TierGrid";
            GridTiers.SetGridSelectionBehavior(true, false);
            GridTiers.FieldLayoutResourceString = "ContractRuleDetail";
            GridCollection.Add(GridTiers);
            CurrentBusObj.Parms.AddParm("@rule_start_date", "01/01/1900");
            CurrentBusObj.Parms.AddParm("@rule_end_date", "01/01/1900"); 
            if (this.IsInserting == true)
                this.New();
            else
            {
                //CurrentBusObj.Parms.AddParm("@start_date", "01/01/1900");
                //CurrentBusObj.Parms.AddParm("@end_date", "01/01/1900");               
                //load obj data
                this.Load();
            }
             // if the object data was loaded, set the rate to recognize to disabled unless the flag is checked
            if (this.CurrentBusObj.HasObjectData)
            {
                if (this.CurrentBusObj.ObjectData.Tables["product_rule"].Rows[0]["completed_contract_flag"].ToString() == "1")
                {
                    dtpStartDate.IsEnabled = false;
                    dtpEndDate.IsEnabled = false;
                }
                //else
                txtratetorecognize.IsEnabled = false;
            }

        }

        private void TiersContextMenuAdd()
        {
            CreateNewTierRow();
        }

        #region KSH 9/7/12 Added for data entry effeciency. Adds a new row, sets focus in first cell of new row, calculates new lower_limit value 

        private void GridTiers_RecordActivated()
        {
            //InsertNewTier();
            if (GridTiers.xGrid.Records.Count > 0)
            {
                //make sure on the last record
                if (GridTiers.ActiveRecord.Index == - 1)
                {
                    DataRecord row = (DataRecord)GridTiers.xGrid.Records[GridTiers.xGrid.Records.Count - 1];
                    if (row.Cells["upper_limit"].Value.ToString() != "999999999")
                    {
                        InsertNewTier();
                    }
                    else
                    {
                        GridTiers.xGrid.FieldLayoutSettings.AllowAddNew = false;
                        btnSave.Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Creates new unit row and puts cursor in first cell in edit mode
        /// </summary>
        public void CreateNewTierRow()
        {
            //if gridtiers has no records then allow insert regardless of calc type
            if (GridTiers.xGrid.Records.Count < 1)
            {
                //run insert logic
                InsertNewTier();
                return;
            }
            //if calc type is flat only one tier record can exist
            if (cmbCalcMethod.SelectedText != "Flat")
            {
                //run insert logic
                InsertNewTier();
            }
            else
            {
                Messages.ShowWarning("Flat Calculation Type Can Only Have One Tier Record");
            }
        }

        /// <summary>
        /// Logic to insert a new tier record
        /// </summary>
        private void InsertNewTier()
        {
            if (CurrentState == ScreenState.Deleting || CurrentState == ScreenState.Empty)
            {
                CurrentState = ScreenState.Normal;
                return;
            }
            //setNewRecDefaultVals();
            List<string> parmsList = new List<string>();
            //get rate_id and rule_id parms
            parmsList = getParms();
            GridTiers.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = GridTiers.xGrid.RecordManager.CurrentAddRecord;
            //Set the default values for the columns
            row.Cells["contract_id"].Value = txtContractId.Text;
            row.Cells["apply_to_group"].Value = 0;
            row.Cells["rate_id"].Value = parmsList[0];
            row.Cells["rule_id"].Value = parmsList[1];
            row.Cells["amount"].Value = 0.00;
            if (GridTiers.xGrid.Records.Count > 0)
            {
                row.Cells["lower_limit"].Value = GetNewLowerLimit();
            }
            else
            {
                row.Cells["lower_limit"].Value = 1;
            }
            row.Cells["upper_limit"].Value = "999999999";
            //Commit the add new record
            GridTiers.xGrid.RecordManager.CommitAddRecord();

            //Set the last row created to the active record
            GridTiers.xGrid.ActiveRecord = GridTiers.xGrid.Records[GridTiers.xGrid.Records.Count - 1];
            //Set the lower_limit field as active on the last row created
            (GridTiers.xGrid.Records[GridTiers.xGrid.Records.Count - 1] as DataRecord).Cells["lower_limit"].IsActive = true;
            (GridTiers.xGrid.Records[GridTiers.xGrid.Records.Count - 1] as DataRecord).Cells["lower_limit"].IsSelected = true;
            //Moves the cursor into the active cell
            GridTiers.xGrid.Records.DataPresenter.BringCellIntoView(GridTiers.xGrid.ActiveCell);
            //Puts the cell into edit mode
            CurrentState = ScreenState.Normal;
            GridTiers.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            //cancel add new mode if flat tier. can only have one rec
            if (cmbCalcMethod.SelectedText == "Flat")
            {
                GridTiers.xGrid.FieldLayoutSettings.AllowAddNew = false;
            }
        }

        /// <summary>
        /// gets new lower limit value by adding 1 to the value of the previous upper_limit
        /// </summary>
        /// <returns></returns>
        private int GetNewLowerLimit()
        {
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["rule_detail"];
            DataRecord row = (DataRecord)GridTiers.xGrid.Records[GridTiers.xGrid.Records.Count - 1];
            return Convert.ToInt32(row.Cells[2].Value) + 1;
        }   

        /// <summary>
        /// used for setting focus and going into edit mode on tiers grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridTiers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataRecord CurrentRow = (DataRecord)GridTiers.xGrid.Records[GridTiers.ActiveRecord.Index];
            //set property IsOnLastCellInRow if the last cell of the row is active. This is needed in the PreviewKeyUp event later.
            if (CurrentRow.Cells["apply_to_group"].IsActive)
            {
                IsOnLastCellInRow = true;
            }
            else
            {
                IsOnLastCellInRow = false;
            }
            switch (e.Key)
            {
                case Key.Insert:
                    CreateNewTierRow();
                    break;
                //need to capture the active record before the key down event fires or the ActiveRecord.Index will = -1 and be meaningless
                case Key.Tab:
                    //if no rows return
                    if (GridTiers.xGrid.Records.Count < 1) return;
                    if (GridTiers.ActiveRecord.Index == -1) return;

                    //get active record here because by the time the preivew key up event fires the active record will be the next row
                    TiersActiveRecord = GridTiers.ActiveRecord.Index;
                    break;
            }
        }

        /// <summary>
        /// used for setting focus and going into edit mode on tiers grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridTiers_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            //if no rows return
            if (GridTiers.xGrid.Records.Count < 1) return;
            if (GridTiers.ActiveRecord.Index == -1) return;
            //can't be on last row on last row
            if (GridTiers.xGrid.Records.Count - 1 == GridTiers.ActiveRecord.Index)
            {
                DataRecord CurrentRow = (DataRecord)GridTiers.xGrid.Records[GridTiers.ActiveRecord.Index];
                //Not on last row so check to see if on last cell of row
                if (IsOnLastCellInRow)
                {
                    //try to set focus to the lower limit cell only if NOT currently on the last row, and last column of row
                    //Moves the cursor into the active cell
                    (GridTiers.xGrid.Records[GridTiers.xGrid.Records.Count - 1] as DataRecord).Cells["lower_limit"].IsActive = true;
                    (GridTiers.xGrid.Records[GridTiers.xGrid.Records.Count - 1] as DataRecord).Cells["lower_limit"].IsSelected = true;
                    GridTiers.xGrid.Records.DataPresenter.BringCellIntoView(CurrentRow.Cells["lower_limit"]);
                    //Puts the cell into edit mode
                    CurrentState = ScreenState.Normal;
                    GridTiers.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// used for setting focus and going into edit mode on tiers grid and creating new record on tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GridTiers_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            //Don't allow empty strings
            if (e.Cell.Value.ToString() == "")
            {
                e.Cell.Value = 0;
            }
            int row_index = GridTiers.ActiveRecord.Index;
            //make sure on last row and last cell in row
            if (GridTiers.xGrid.Records.Count - 1 == TiersActiveRecord && IsOnLastCellInRow)
            {
                DataRecord row = (DataRecord)GridTiers.xGrid.ActiveRecord;
                if (row.Cells["upper_limit"].Value.ToString() != "999999999")
                    CreateNewTierRow();
                else
                {
                    GridTiers.xGrid.FieldLayoutSettings.AllowAddNew = false;
                    btnSave.Focus();
                }
            }
        }

        /// <summary>
        /// make sure upper limit value in last row is 999999999
        /// </summary>
        /// <returns></returns>
        private bool LastRowUpperLimitCorrect()
        {
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["rule_detail"];
            //this will prevent error if no tier recs are entered on save
            if (GridTiers.xGrid.Records.Count < 1) return false;
            DataRecord row = (DataRecord)GridTiers.xGrid.Records[GridTiers.xGrid.Records.Count - 1];
            //make sure upper limit value in last row is 999999999
            if (Convert.ToInt32(row.Cells[2].Value) == 999999999)
                return true;
            else
                return false;
        }

        /// <summary>
        /// if current row lower limit is not previous row upper limit + 1 return false
        /// </summary>
        /// <returns></returns>
        private bool CheckStaggeredLimits()
        {
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["rule_detail"];
            //only run check if more than one row in grid
            if (GridTiers.xGrid.Records.Count > 1)
            {
                //loop through grid rows
                for (int i = 0; i < GridTiers.xGrid.Records.Count; i++)
                {
                    //start check on 2nd row
                    if (i > 0)
                    {
                        //check current upper limit
                        DataRecord CurrentRow = (DataRecord)GridTiers.xGrid.Records[i];
                        DataRecord PrevRow = (DataRecord)GridTiers.xGrid.Records[i - 1];
                        //if current row lower limit is not previous row upper limit + 1 return false
                        if (Convert.ToInt32(PrevRow.Cells[2].Value) + 1 != Convert.ToInt32(CurrentRow.Cells[1].Value))
                            return false;
                    }
                }
            }
            //no problems found with limits return true
            return true;
        }

        private bool CheckLowerLimitsLessThanUpperLimits()
        {
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["rule_detail"];
            //only run check if more than one row in grid
            if (GridTiers.xGrid.Records.Count > 1)
            {
                //loop through grid rows
                for (int i = 0; i < GridTiers.xGrid.Records.Count; i++)
                {
                    //check current upper limit
                    DataRecord CurrentRow = (DataRecord)GridTiers.xGrid.Records[i];
                    //if current row lower limit is >= current row upper limit return false
                    //DWR Modified 1/23/13 - Modified to make equal to a valid condition
                    if (Convert.ToInt32(CurrentRow.Cells[1].Value) > Convert.ToInt32(CurrentRow.Cells[2].Value))
                        return false;
                }
            }
            //no problems found with limits return true
            return true;
        }

        //make sure the terminator only exists one record
        private bool CheckTerminator()
        {
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["rule_detail"];
            int termFound = 0;
            //only run check if more than one row in grid
            if (GridTiers.xGrid.Records.Count > 1)
            {
                //loop through grid rows
                for (int i = 0; i < GridTiers.xGrid.Records.Count; i++)
                {
                    //check current upper limit
                    DataRecord CurrentRow = (DataRecord)GridTiers.xGrid.Records[i];
                    //if current row lower limit is >= current row upper limit return false
                    if (Convert.ToInt32(CurrentRow.Cells[2].Value) == 999999999)
                        termFound++;
                }
                if (termFound > 1)
                    return false;
            }
            //no problems found with limits return true
            return true;
        }

        /// <summary>
        /// never allow the first tier row lower limit to be > 1
        /// </summary>
        /// <returns></returns>
        private bool FirstRowLowerLimitCorrect()
        {
            DataTable dt = this.CurrentBusObj.ObjectData.Tables["rule_detail"];
            //this will prevent error if no tier recs are entered on save
            if (GridTiers.xGrid.Records.Count < 1) return false;
            DataRecord row = (DataRecord)GridTiers.xGrid.Records[0];
            //keep lower limit <= 1 on first tier record
            if (Convert.ToInt32(row.Cells["lower_limit"].Value) <= 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Validates going from a multi-tiered calc type back to a flat type.  Can only have one tier for flat type 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCalcMethod_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedValue" && cmbCalcMethod.SelectedValue != null && cmbCalcMethod.SelectedValue != "")
            {
                //Test to see if it is flat calc type and check if > 1 tier
                if (cmbCalcMethod.SelectedValue.ToString() == "0" && GridTiers.xGrid.Records.Count > 1)
                {
                    MessageBoxResult MsgResult = Messages.ShowYesNo("Calculation Type of Flat Can Only Have one Tier. Rule Will Not Save. Delete All but the First Tier?", MessageBoxImage.Question);
                    if (MsgResult == MessageBoxResult.Yes)
                    {
                        //delete all but first tier
                        for (int i = 0; i < CurrentBusObj.ObjectData.Tables["rule_detail"].Rows.Count; i++)
                        {
                            if (i > 0)
                            {
                                CurrentBusObj.ObjectData.Tables["rule_detail"].Rows[i].Delete();
                            }
                        }
                    }
                    else
                    {
                        //do nothing.  ucLabelCombobox does not have the items collection exposed so no way to iterate through collection and set value back
                        //to what it was initially
                    }
                }
            }
        }

        /// <summary>
        /// set focus on tiers grid when tabbing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    //stop tab event from occurring
                    e.Handled = true;
                    if (GridTiers.xGrid.Records.Count < 1)
                    {
                        //insert default tier row of no rows in grid
                        InsertNewTier();
                    }
                    else
                    {
                        //set focus to first row first, first editable field in grid
                        GridTiers.CntrlFocus();
                        (GridTiers.xGrid.Records[0] as DataRecord).Cells["lower_limit"].IsActive = true;
                        (GridTiers.xGrid.Records[0] as DataRecord).Cells["lower_limit"].IsSelected = true;
                        GridTiers.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                    }
                    break;
            }
        }

        /// <summary>
        /// Prevents row with missing defaults from occurring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void GridTiers_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (GridTiers.xGrid.Records.Count < 1)
        //    {
        //        //insert default tier row of no rows in grid
        //        InsertNewTier();
        //    }
        //}

        #endregion

        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    //setBindingExpresion on combo box
                    //RES 4/2/15 assign sp to combo box based on archived status of product item
                    if (this.CurrentBusObj.ObjectData.Tables["product_rule"].Rows[0]["item_archive_flag"].ToString() == "1")
                       this.cmbProdItem.SetBindingExpression("item_id", "item_description", this.CurrentBusObj.ObjectData.Tables["product_lookup_with_archive"]);
                    else
                        this.cmbProdItem.SetBindingExpression("item_id", "item_description", this.CurrentBusObj.ObjectData.Tables["product_lookup"]);
                    this.cmbStatus.SetBindingExpression("fkey_int","code_value",this.CurrentBusObj.ObjectData.Tables["status_lookup"]);
                    this.cmbRuleType.SetBindingExpression("fkey_int", "code_value", this.CurrentBusObj.ObjectData.Tables["type_lookup"]);
                    this.cmbCalcMethod.SetBindingExpression("fkey_int", "code_value", this.CurrentBusObj.ObjectData.Tables["calc_method_lookup"]);
                    this.cmbTierBy.SetBindingExpression("filter_id", "filter_name" , this.CurrentBusObj.ObjectData.Tables["unit_filter_lookup"]);
                    this.cmbDiscountType.SetBindingExpression("discount_id", "discount_description", this.CurrentBusObj.ObjectData.Tables["discount_lookup"]);
                    this.cmbBillBy.SetBindingExpression("filter_id", "filter_name", this.CurrentBusObj.ObjectData.Tables["count_lookup"]);
                    this.cmbNormalizedBase.SetBindingExpression("filter_id", "filter_name", this.CurrentBusObj.ObjectData.Tables["count_lookup"]);
                    this.cmbCola.SetBindingExpression("cola_id", "description", this.CurrentBusObj.ObjectData.Tables["cola_lookup"]);
                    this.cmbAcctClass.SetBindingExpression("acct_class_id", "acct_class", this.CurrentBusObj.ObjectData.Tables["acct_class_lookup"]);
                    if (this.CurrentBusObj.ObjectData.Tables["product_rule"].Rows[0]["acct_class_status"].ToString() == "0"  &&
                        this.CurrentBusObj.ObjectData.Tables["product_rule"].Rows[0]["rate_id"].ToString() != "0")
                        this.cmbAcctClass.SetBindingExpression("acct_class_id", "acct_class", this.CurrentBusObj.ObjectData.Tables["acct_class_lookup_with_archive"]);
                    //else
                    //    this.cmbAcctClass.SetBindingExpression("acct_class_id", "acct_class", this.CurrentBusObj.ObjectData.Tables["acct_class_lookup"]);
                    this.cmbCurrency.SetBindingExpression("currency_code", "description", this.CurrentBusObj.ObjectData.Tables["currency_lookup"]);
                    this.cmbAcctRule.SetBindingExpression("rule_id", "rule_desc", this.CurrentBusObj.ObjectData.Tables["acct_rule_lookup"]);
                    this.cmbLineDescOptions.SetBindingExpression("fkey_int", "code_value", this.CurrentBusObj.ObjectData.Tables["line_desc_options"]);
                    //RES 3/28/16 Add CLM Number
                    this.cmbCLMNumber.SetBindingExpression("clm_id", "clm_number", this.CurrentBusObj.ObjectData.Tables["clm_lookup"]);
                    //Add code to populate the grid combobox on the Company Contract Grid
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip.ItemsSource = this.CurrentBusObj.ObjectData.Tables["rule_group_lookup"].DefaultView;
                    //set the value and display path
                    ip.ValuePath = "rule_group_id";
                    ip.DisplayMemberPath = "rule_group_description";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbRulesGroupCombo = ip;
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }

        /// <summary>
        /// Override of New command
        /// </summary>
        public override void New()
        {
            //TODO: kill the grids for now during an insert, because of the robjects problem, may change later
            //populate read only default values
            //GridRuleGroup.IsEnabled = false;
            //GridTiers.IsEnabled = false;
            //GridTiers.xGrid.FieldLayoutSettings.AllowAddNew = true;
            //////////////////////////////////////////////////////////////////////////////////////////////////
            base.New();
        }

        public override void Save()
        {
            //Make contract product data verifications
            if (!VerifyData())
                return;
            base.Save();
            if (SaveSuccessful)
            {
                //Messages.ShowInformation("Save Successful");
                ContractObj.LoadTable("rules");
                ContractObj.LoadTable("rules_detail");
                //ContractObj.LoadTable("cumulative_rates");
                //refresh all grids
                gRates.FilterCall();
                selectNewRule();
                //ContractObj.Parms.UpdateParmValue("@contract_id", Convert.ToInt32(txtContractId.Text));
                //ContractObj.LoadTable("general");
                CloseScreen();
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }

        /// <summary>
        /// use this when you need to record count of a child base grid. Have to do this becuase of an Infragistics API bug
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private int CountFilteredRecords(RecordCollectionBase records)
        {
            int count = 0;

            //Loop through the list of records.
            foreach(Record rec in records)
            {
                switch (rec.GetType().FullName)
                {
                    case "Infragistics.Windows.DataPresenter.GroupByRecord":
                        //For GroupByRecords, do a recursive call for the child records
                        GroupByRecord castRec = (GroupByRecord)rec;
                        count += this.CountFilteredRecords(castRec.ChildRecords);
                        break;
                    case "Infragistics.Windows.DataPresenter.DataRecord":
                        DataRecord record =  (DataRecord)rec;
                        if (record.IsFilteredOut == true && record.IsFilteredOut != null)
                        {
                            count += 1;
                        }
                        break;
                    default:
                        throw new NotImplementedException("Record type not implemented.");
                }
            }
            //return the parent count - filtered record count to get filtered record count.
            return gRules.xGrid.Records.Count - count;
        }

        private void selectNewRule()
        {
            int count = 0; int filteredCount = 0; int largestRuleId = 0;
            //get number of records in Rules grid
            int RulesCount = CountFilteredRecords(gRules.xGrid.Records);
            //exit if 1 or fewer recs exit in rules grid, logic already handles 1 & 0 recs
            if (RulesCount < 2) return;
            //Loop through the list of records.
            foreach (Record rec in gRules.xGrid.Records)
            {
                DataRecord currDR = gRules.xGrid.Records[count] as DataRecord;
                switch (rec.GetType().FullName)
                {
                    case "Infragistics.Windows.DataPresenter.DataRecord":
                        DataRecord record = (DataRecord)rec;
                        if (record.IsFilteredOut == false || record.IsFilteredOut == null)
                        {
                            if (filteredCount == 0)
                            {
                                //load first rec start date
                                largestRuleId = (int)currDR.Cells["rule_id"].Value;
                            }
                            if (Convert.ToInt32(currDR.Cells["rule_id"].Value) >= largestRuleId)
                            {
                                //set active record//////////////////////
                                gRules.SetGridSelectionBehavior(false, false);
                                gRules.xGrid.ActiveRecord = currDR;
                                currDR.IsActive = true;
                                currDR.IsSelected = true;
                                gRules.SetGridSelectionBehavior(false, true);
                                largestRuleId = (int)currDR.Cells["rule_id"].Value;
                                /////////////////////////////////////////////
                            }
                            filteredCount++;
                        }
                        break;
                }
                count++;
            }
            ////select record in grid that matches current bill period
            //for (int i = 0; i < RulesCount; i++)
            //{
            //    DataRecord currDR = gRules.xGrid.Records[i] as DataRecord;
            //    if (i == 0)
            //    {
            //        //load first rec start date
            //        largestRuleId = (int)currDR.Cells["rule_id"].Value;
            //    }
            //    //if (RulesCount == 1)
            //    //{
            //    //    //set 1st & only row as new active record
            //    //    gRules.SetGridSelectionBehavior(false, false);
            //    //    gRules.xGrid.ActiveRecord = currDR;
            //    //    currDR.IsActive = true;
            //    //    currDR.IsSelected = true;
            //    //    gRules.SetGridSelectionBehavior(false, true);
            //    //    largestRuleId = (int)currDR.Cells["rule_id"].Value;
            //    //    /////////////////////////////////////////////
            //    //    return;
            //    //}
            //    if (Convert.ToInt32(currDR.Cells["rule_id"].Value) >= largestRuleId)
            //    {
            //        //set active record//////////////////////
            //        gRules.SetGridSelectionBehavior(false, false);
            //        gRules.xGrid.ActiveRecord = currDR;
            //        currDR.IsActive = true;
            //        currDR.IsSelected = true;
            //        gRules.SetGridSelectionBehavior(false, true);
            //        largestRuleId = (int)currDR.Cells["rule_id"].Value;
            //        /////////////////////////////////////////////
            //    }
            //}
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Save();
            this.CurrentBusObj.LoadTable("location_rules");
        }

        private bool VerifyData()
        {
            //used for tryParse
            int iParseResult = 0;

            if (CurrentBusObj.ObjectData.Tables["rule_default"].Rows != null)
            {
                //Force dates to first or last day of month
                DateTime  dtStart = Convert.ToDateTime(dtpStartDate.SelText) ;
                DateTime dtEnd = Convert.ToDateTime(dtpEndDate.SelText);
                DateTime  dtTempEnd = dtEnd.AddMonths(1);
                dtTempEnd = Convert.ToDateTime(Convert.ToString(dtEnd.Month) + "/" + Convert.ToString(dtEnd.Year)).AddMonths(-1);

                //Set start date to the first of the month.
                if (dtStart.Day != 1)
                    dtpStartDate.SelText =Convert.ToDateTime( Convert.ToString(dtStart.Month) + "/1/" + Convert.ToString(dtStart.Year));
                
                //If not the end of the month - Change value to be the end of the month
                //Determined by taking first of next month and subtracting a day
                if (dtEnd.AddDays(1).Day  != 1)
                    dtpEndDate.SelText = dtTempEnd ;

                rDefault = CurrentBusObj.ObjectData.Tables["rule_default"].Rows[0];
                //Verify Rule start date not before rate start date
                if (Convert.ToDateTime(rDefault["start_date"]) > Convert.ToDateTime(dtpStartDate.SelText))
                {
                    Messages.ShowError("Rule start date must be greater than or equal to the rate start date of: " + Convert.ToString(rDefault["start_date"]));
                    return false;
                }
                //Verify rule end date not after rate end date
                if (Convert.ToDateTime(rDefault["end_date"]) < Convert.ToDateTime(dtpEndDate.SelText))
                {
                    Messages.ShowError("Rule end date must be less than or equal to the rate end date of: " + Convert.ToString(rDefault["end_date"]));
                    return false;
                } 

                //Verify rule end date greater than rule start date
                if (Convert.ToDateTime(dtpStartDate.SelText) > Convert.ToDateTime(dtpEndDate.SelText))
                {
                    Messages.ShowError("Rule start date must be less than the rule end date.");
                    return false;
                }

                //Verify bill frequency start date between rule start and end dates
                if (Convert.ToDateTime(dtpBillFreqStartDate.SelText) > Convert.ToDateTime(dtpEndDate.SelText) ||
                    Convert.ToDateTime(dtpBillFreqStartDate.SelText) < Convert.ToDateTime(dtpStartDate.SelText))
                {
                    Messages.ShowError("Bill Frequency Start Date must be between Rule start and end dates");
                    return false;
                }

                //Verify tier period less than or equal to start date
                if (Convert.ToDateTime(dtpTierStartDate.SelText) > Convert.ToDateTime(dtpStartDate.SelText))
                {
                    Messages.ShowError("Tier period start date must be less than or equal to the rule start date");
                    return false;
                }

                //Tier Period >= 1
                if (Convert.ToInt32(txtTierPeriod.Text) <= 0)
                {
                    Messages.ShowError("Tier period must be greater than 0");
                    return false;
                }
                
                //Bill frequency >=1 
                if(Convert.ToInt32(txtFrequency.Text) <= 0 )
                {
                    Messages.ShowError("Bill frequency must be greater than 0");
                    return false;
                }

                //Must have a valid product item
                if (Convert.ToInt32(cmbProdItem.SelectedValue) == 0)
                {
                    Messages.ShowError("Please select a valid product item");
                    return false;
                }

                if (Convert.ToString(cmbCurrency.SelectedText) =="")
                {
                    Messages.ShowError("Please select a valid currency");
                    return false;
                }

                ////New rules must have a CLM Number assigned
                //if (Convert.ToString(cmbCLMNumber.SelectedText) == "" && IsInserting)
                //{
                //    Messages.ShowError("Please enter CLM Number");
                //    return false;
                //}

                //check that flat calc type does not have more than one tier
                if (cmbCalcMethod.SelectedText == "Flat")
                {
                    //tiers > 1 throw error
                    if (GridTiers.xGrid.Records.Count > 1)
                    {
                        Messages.ShowError("Calc Type of Flat Can Only Have One Tier Record");
                        return false;
                    }
                }

                if (!CheckTerminator())
                {
                    Messages.ShowError("Max Upper Limit, 999999999, can only exist once and on the last tier record");
                    return false;
                }

                //check that multiple tiers last record upper limit end with 999999999
                if (GridTiers.xGrid.Records.Count > 0)
                {
                    if (!LastRowUpperLimitCorrect())
                    {
                        Messages.ShowError("Last Tier Record Upper Limit Value Must Equal 999999999");
                        return false;
                    }
                }
                
                //check that multiple tiers lower limits are all + 1 from the prev record's upper limit
                if (GridTiers.xGrid.Records.Count > 0)
                {
                    if (!CheckStaggeredLimits())
                    {
                        Messages.ShowError("A lower limit violation exists. All lower limits values must be one greater than their respective previous row's upper limit. Also Lower Limit Values Must be Less than or Equal to their Respective Upper Limit Values.");
                        return false;
                    }
                }

                if (!CheckLowerLimitsLessThanUpperLimits())
                {
                    Messages.ShowError("Lower Limit Values Cannot Be Greater Than Their Respective Upper Limit Values.");
                    return false;
                }

                //If the rule is a discount and uses a percentage discount, then the bill by has to be a value of 0
                if (cmbRuleType.SelectedValue.ToString() == "3" && chkPercentDiscoType(cmbDiscountType) == true)
                {
                    if ((int)cmbBillBy.SelectedValue != 0)
                    {
                        Messages.ShowError("The Rule is a Discount and Uses a Percentage Discount Type.  The Bill By Must Be 0.");
                        return false;
                    }
                }

                //Don’t allow save if bill frequency 0
                if (txtFrequency.Text == "0")
                {
                    Messages.ShowError("Bill Frequency Cannot Be 0");
                    return false;
                }

                //Don’t allow save if tier period equal 0.
                if (txtTierPeriod.Text == "0")
                {
                    Messages.ShowError("Tier Period Cannot Be 0");
                    return false;
                }

                //multiple checks on tier period and bill freq here
                if (txtTierPeriod.Text != null && txtFrequency.Text != null)
                {
                    //make sure int convert will work
                    if (Int32.TryParse(txtTierPeriod.Text, out iParseResult) && Int32.TryParse(txtFrequency.Text, out iParseResult))
                    {
                        if (Convert.ToInt32(txtTierPeriod.Text) < Convert.ToInt32(txtFrequency.Text))
                        {
                            //Bill frequency start date must equal rule start date if tier period is less than bill frequency
                            if (dtpBillFreqStartDate.SelText != dtpStartDate.SelText)
                            {
                                Messages.ShowError("Bill Frequency Start Date Must Equal Rule Start Date if Tier Period is Less than Bill Frequency");
                                return false;
                            }
                            //If the tier period is less than the bill frequency, then the bill frequency must be divisible by the tier period with no remainder
                            if (Convert.ToInt32(txtFrequency.Text) % Convert.ToInt32(txtTierPeriod.Text) != 0)
                            {
                                Messages.ShowError("Bill Frequency must be divisible by the Tier Period with no Remainder, When Tier Period is Less than Bill Frequency");
                                return false;
                            }
                        }
                        //Don’t allow normalized flag to be true if tier period <= bill frequency
                        if (chkNormalized.IsChecked == 1)
                        {
                            if (Convert.ToInt32(txtTierPeriod.Text) <= Convert.ToInt32(txtFrequency.Text))
                            {
                                Messages.ShowError("Normalized Flag Cannot be Set if Tier Period Less Than Or Equal To Bill Frequency");
                                return false;
                            }
                        }

                        //If calc method = cumulative or Group Tiered then the line display option cannot be Overwrite Description
                        if ((int)cmbCalcMethod.SelectedValue == 2 || (int)cmbCalcMethod.SelectedValue == 3)
                        {
                            if ((int)cmbLineDescOptions.SelectedValue == 1)
                            {
                                Messages.ShowError("Calc Method of " + cmbCalcMethod.SelectedText + " and Line Desc Display Option of Overwrite Description Are Not Compatible");
                                return false;
                            }
                        }
                    }
                }

                //Don’t allow save if bill frequency is greater than the number of months between the rule start and end date
                if (txtTierPeriod.Text != null && txtFrequency.Text != null)
                {
                    iParseResult = 0;
                    if (Int32.TryParse(txtFrequency.Text, out iParseResult))
                    {
                        //calculate diff in months btwn rule start and end dates
                        DateTime dt;
                        var dateSpan = DateTimeSpan.CompareDates(Convert.ToDateTime(dtpEndDate.SelText), Convert.ToDateTime(dtpStartDate.SelText) );
                        if (Convert.ToInt32(txtFrequency.Text) > dateSpan.Months + 1)
                        {
                            Messages.ShowError("Bill Frequency is " + txtFrequency.Text + " it Cannot be Larger than the Number of Months Between the Rule Start and End Dates which is " + dateSpan.Months);
                            return false;
                        }
                    }
                }

                //Don’t allow change of tier period to less than bill frequency if any data in billing_location_posted for the rule with a status of 1
                if (CurrentBusObj.ObjectData.Tables["billing_location_posted"].Rows.Count > 0)
                {
                    if (Convert.ToInt32(txtTierPeriod.Text) < Convert.ToInt32(txtFrequency.Text))
                    {
                        Messages.ShowError("Tier Period Cannot Be Less Than Bill Frequency if Billing Location Posted Records Exist for the Rule with a Status of 1");
                        return false;
                    }
                }

                //Don't allow Lower Limit of First Tier Record Cannot Be Greater Than 1                
                if (!FirstRowLowerLimitCorrect())
                {
                    Messages.ShowError("Lower Limit of First Tier Record Cannot Be Greater Than 1");
                    return false;
                }

            }
            ////New rules must have a CLM Number assigned
            if (Convert.ToString(cmbCLMNumber.SelectedText) == "")
            {
                Messages.ShowError("Please enter CLM Number");
                return false;
            }
            //Warning if existing rules don't have CLM Number
            //if (Convert.ToString(cmbCLMNumber.SelectedText) == "" && !IsInserting)
            //{
            //    var result = MessageBoxResult.OK;
            //    result = MessageBox.Show("CLM Number has not been added.  Do you want to proceed?", "Warning", MessageBoxButton.YesNo);
            //    if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
            //    {
            //    }
            //    else
            //        return false;
            //}

            //9/9/24 verify lines to combine on invoice have the same data in group by columns
            if (chkCompletedContract.IsChecked == 1)
            {
                if ((txtLineDesc.Text == "")  || (txtLineDesc.Text == " "))
                {
                    Messages.ShowError("Line description cannot be blank for combined rules");
                    return false;
                }
                //DateTime dtStart = Convert.ToDateTime(dtpStartDate.SelText);
                //DateTime dtEnd = Convert.ToDateTime(dtpEndDate.SelText);
                //dtpStartDate.SelText = Convert.ToDateTime(Convert.ToString(dtStart.Month) + "/1/" + Convert.ToString(dtStart.Year));
                //dtpEndDate.SelText = Convert.ToDateTime(Convert.ToString(dtEnd.Month) + "/1/" + Convert.ToString(dtEnd.Year));
                CurrentBusObj.Parms.UpdateParmValue("@rule_start_date", dtpStartDate.SelText);
                CurrentBusObj.Parms.UpdateParmValue("@rule_end_date", dtpEndDate.SelText);
                this.CurrentBusObj.LoadTable("combine");
                
                if (CurrentBusObj.ObjectData.Tables["combine"].Rows.Count > 0)
                //if (CurrentBusObj.ObjectData.Tables["combinestart"].Rows.Count > 0)
                {
                    rCombine = CurrentBusObj.ObjectData.Tables["combine"].Rows[0];
                    //rCombine = CurrentBusObj.ObjectData.Tables["combinestart"].Rows[0];
                    
                    //if (txtRuleId.Text != Convert.ToString(rCombine["rule_id"]))
                    //{
                        if (txtLineDesc.Text != Convert.ToString(rCombine["line_desc"]))
                        {
                            Messages.ShowError("Line description must be the same as the line description on combined rule (" + Convert.ToString(rCombine["line_desc"]) + ")");
                            return false;
                        }
                        if (Convert.ToDateTime(dtpStartDate.SelText) != Convert.ToDateTime(rCombine["start_date"]))
                        {
                            Messages.ShowError("Start date must be the same as start date on combined rule (" + Convert.ToDateTime(rCombine["start_date"]) + ")");
                            return false;
                        }
                        if (Convert.ToDateTime(dtpEndDate.SelText) != Convert.ToDateTime(rCombine["end_date"]))
                        {
                            Messages.ShowError("End date must be the same as end date on combined rule (" + Convert.ToDateTime(rCombine["end_date"]) + ")");
                            return false;
                        }
                        if (Convert.ToInt32(txtFrequency.Text) != Convert.ToInt32(rCombine["bill_frequency"]))
                        {
                            Messages.ShowError("Bill frequency must be the same as bill frequency on combined rule (" + Convert.ToString(rCombine["bill_frequency"]) + ")");
                            return false;
                        }
                        if ((int)cmbBillBy.SelectedValue != Convert.ToInt32(rCombine["bill_by"]))
                        {
                            Messages.ShowError("Bill by must be the same as bill by on combined rule (" + Convert.ToString(rCombine["filter_name"]) + ")");
                            return false;
                        }
                        if (txtProductDesc.Text != Convert.ToString(rCombine["product_code"]))
                        {
                            Messages.ShowError("Product code must be the same as product_code on combined rule (" + Convert.ToString(rCombine["product_code"]) + ")");
                            return false;
                        }
                        if ((int)cmbCalcMethod.SelectedValue != Convert.ToInt32(rCombine["calc_method"]))
                        {
                            Messages.ShowError("Calc method must be the same as calc method on combined rule (" + Convert.ToString(rCombine["code_value"]) + ")");
                            return false;
                        }
                        if (Convert.ToInt32(txtServicePeriodOffset.Text) != Convert.ToInt32(rCombine["service_period_offset"]))
                        {
                            Messages.ShowError("Service period offset must be the same as service period offset on combined rule (" + Convert.ToString(rCombine["service_period_offset"]) + ")");
                            return false;
                        }
                    //}
                }
            }

            return true;
        }

        private void TiersGridRemovingDelegate()
        {
            int tiers = GridTiers.xGrid.SelectedItems.Records.Count;
            //int tierId = -1, tierUpper = -1, tierLower = -1;
            //string tierNumberField = "tier_number";
            //string tierLowerField = "lower_limit";
            //string tierUpperField = "upper_limit";
            string ruleDetailTableName = "rule_detail";

            if (tiers > 0)
            {
                int intTest = 0;
                DataRecord record = (DataRecord)GridTiers.xGrid.SelectedItems.Records[0];
                DataRow row = ((DataRowView)record.DataItem).Row;

                //if (Int32.TryParse(row[tierNumberField].ToString(), out intTest))
                //    tierId = (int)row[tierNumberField];
                //else
                //    tierId = 0;
                //intTest = 0;
                //if (Int32.TryParse(row[tierLowerField].ToString(), out intTest))
                //    tierLower = (int)row[tierLowerField];
                //else
                //    tierLower = 0;
                //intTest = 0;
                //if (Int32.TryParse(row[tierUpperField].ToString(), out intTest))
                //    tierUpper = (int)row[tierUpperField];
                //else
                //    tierUpper = 0;

                row.Delete();
                
            //    int rowId = -1;

            //    foreach (DataRow detailRow in CurrentBusObj.ObjectData.Tables[ruleDetailTableName].Rows)
            //    {
            //        if (detailRow.RowState == DataRowState.Deleted) 
            //        {                        
            //            continue; 
            //        }

            //        rowId = (int)detailRow[tierNumberField];

            //        if (rowId < tierId)
            //        {                       
            //            if (rowId == tierId - 1)
            //            {
            //                detailRow[tierUpperField] = tierUpper;
            //            }
            //        }
            //        else if (rowId > tierId)
            //        {
            //            detailRow[tierNumberField] = rowId - 1;
            //        }                   
            //    }                
            }

            GridTiers.LoadGrid(CurrentBusObj, ruleDetailTableName);
        }

        /// <summary>
        /// used to find rate_id and rule_id in objectData ParmsTable
        /// </summary>
        /// <returns></returns>
        private List<string> getParms()
        {
            try
            {
                List<string> parmsList = new List<string>();
                var ParmIDs = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                    where x.Field<string>("parmName") == "@rate_id" ||
                                        x.Field<string>("parmName") == "@rule_id"
                                    select new
                                    {
                                        parmName = x.Field<string>("parmName"),
                                        parmValue = x.Field<string>("parmValue")
                                    };

                foreach (var info in ParmIDs)
                {
                    if (info.parmName == "@rate_id") parmsList.Add(info.parmValue);
                    if (info.parmName == "@rule_id") parmsList.Add(info.parmValue);
                }
                return parmsList;
            }
            catch (Exception ex)
            {
                Messages.ShowError("Problem finding parameters. Error : " + ex.Message);
                return null;
            }
        }

        private void txtContractId_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsInserting == true)
            {
                var Info = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                        where x.Field<string>("parmName") == "@contract_id"
                                        select new
                                        {
                                            parmName = x.Field<string>("parmName"),
                                            parmValue = x.Field<string>("parmValue")
                                        };

                foreach (var info in Info)
                {
                    if (info.parmName == "@contract_id")
                        if (info.parmValue!= "")
                            txtContractId.Text = info.parmValue;
                        else
                            txtContractId.Text = "0";
                }
            }
        }

        private void txtBillMsoId_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsInserting == true)
            {
                var Info = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                        where x.Field<string>("parmName") == "@contract_entity_id"
                                        select new
                                        {
                                            parmName = x.Field<string>("parmName"),
                                            parmValue = x.Field<string>("parmValue")
                                        };

                foreach (var info in Info)
                {
                    if (info.parmName == "@contract_entity_id")
                        if (info.parmValue != "")
                            txtBillMsoId.Text = info.parmValue;
                        else
                            txtBillMsoId.Text = "0";
                }
            }
        }

        private void txtContractEntity_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsInserting == true)
            {
                var Info = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                        where x.Field<string>("parmName") == "@contract_entity"
                                        select new
                                        {
                                            parmName = x.Field<string>("parmName"),
                                            parmValue = x.Field<string>("parmValue")
                                        };

                foreach (var info in Info)
                {
                    if (info.parmName == "@contract_entity")
                        if (info.parmValue != "")
                            txtContractEntity.Text = info.parmValue;
                        else
                            txtContractEntity.Text = "0";
                }
            }
        }

        private void txtContractDescription_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (IsInserting == true)
            {
                var Info = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                           where x.Field<string>("parmName") == "@contract_description"
                           select new
                           {
                               parmName = x.Field<string>("parmName"),
                               parmValue = x.Field<string>("parmValue")
                           };

                foreach (var info in Info)
                {
                    if (info.parmName == "@contract_description")
                        if (info.parmValue != "")
                            txtContractDescription.Text = info.parmValue;
                        else
                            txtContractDescription.Text = "0";
                }
            }
        }

        private void ContractProductRuleScreen_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Establish default values
            //Only use defaults if for a new rule
            if (CurrentBusObj.ObjectData.Tables["rule_default"].Rows != null && this.IsInserting==true )
            {
                rDefault = CurrentBusObj.ObjectData.Tables["rule_default"].Rows[0];
                txtFrequency.Text = Convert.ToString(rDefault["bill_frequency"]);
                dtpStartDate.SelText = Convert.ToDateTime(rDefault["start_date"]); 
                dtpEndDate.SelText = Convert.ToDateTime(rDefault["end_date"]);
                cmbCurrency.SelectedValue = Convert.ToString(rDefault["currency_code"]);
                dtpTierStartDate.SelText = Convert.ToDateTime(rDefault["start_date"]);
                dtpBillFreqStartDate.SelText = Convert.ToDateTime(rDefault["start_date"]);
                txtTierPeriod.Text = Convert.ToString(rDefault["bill_frequency"]);
                txtServicePeriodOffset.Text = "1";
                cmbCola.SelectedValue = Convert.ToInt32(rDefault["cola_id"]);
            }
 
            //Set the verification values for the screen
            if (CurrentBusObj.ObjectData.Tables["rule_default"].Rows != null)
            {
                rDefault = CurrentBusObj.ObjectData.Tables["rule_default"].Rows[0];
                currentStartDate = Convert.ToDateTime(rDefault["start_date"]);
                currentEndDate = Convert.ToDateTime(rDefault["end_date"]);
            }

            else
            {
                currentStartDate = Convert.ToDateTime("1/1/1900");
                currentEndDate = Convert.ToDateTime("1/1/1900");
            }
            //add new mode if not flat tier calc type
            //if (cmbCalcMethod.SelectedText != "Flat")
            //{
            //    GridTiers.xGrid.FieldLayoutSettings.AllowAddNew = true;
            //}

            if (CurrentBusObj.ObjectData.Tables["billing_location_posted"].Rows.Count > 0)
            {
                //Don’t allow change of bill_frequency if any data in billing_location_posted for the rule with a status of 1
                txtFrequency.IsEnabled = false;
            }

            //enable/disable line desc options combo as necessary
            setLineDescOptionsCbo();
        }

        private void cmbRuleType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //If the rule is changed to a discount then enable the discount type box, otherwise disable and set the value to 0
            if (e.PropertyName == "SelectedValue" && cmbRuleType.SelectedValue != null)
            {
                //Test to see if it is a discount rule type
                if (cmbRuleType.SelectedValue.ToString() == "3")
                    cmbDiscountType.IsEnabled = true;
                else
                {
                    cmbDiscountType.IsEnabled = false;
                    cmbDiscountType.SelectedValue = 0;
                }
            }
        }

        private void CloseScreen()
        {
            System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                AdjParent.Close();
            }

        }

        /// <summary>
        /// Modifies the style of the amounts on the tier grid based on the discount type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbDiscountType_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            ucLabelComboBox db = (ucLabelComboBox)sender;
            Field f = GridTiers.xGrid.FieldLayouts[0].Fields["amount"];
            if (chkPercentDiscoType(db))
            {
                //percent disco type found
                f.Settings.EditorStyle = (Style)TryFindResource("RateStylePercent");
                cmbBillBy.SelectedValue = 0;
            }
            else
            {
                //currency disco type found
                f.Settings.EditorStyle = (Style)TryFindResource("RateStyleDecimal6");
            }

            //ucLabelComboBox db = (ucLabelComboBox)sender;
            //if (db.SelectedValue != null)
            //{
            //    int DiscountType = Convert.ToInt32(db.SelectedValue);
            //    //Loop through the discount type datatable and find the currently selected row
            //    foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["discount_lookup"].Rows)
            //    {
            //        //Once the correct record is found get the fixed rate flag and determine how to display
            //        //1 = Currency / 0 = percentage
            //        if (Convert.ToInt32(r["discount_id"]) == DiscountType)
            //        {
            //            Field f = GridTiers.xGrid.FieldLayouts[0].Fields["amount"];
            //            if (Convert.ToInt32(r["fixed_rate_flag"]) == 1)
            //            {
            //                f.Settings.EditorStyle = (Style)TryFindResource("RateStyleDecimal6");
            //            }
            //            else
            //            {
            //                //f.Settings.EditorStyle = (Style)TryFindResource("RateStylePercent");
            //                f.Settings.EditorStyle = (Style)TryFindResource("RateStylePercent");
            //                cmbBillBy.SelectedValue = 0;
            //            }
            //            break;
            //        }
            //    }
            //}

        }

        //Populate COLA min and max when user selects a colaid from dropdown
        private void cmbColaID_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EnumerableRowCollection<DataRow> MinMax = from COLATable in CurrentBusObj.ObjectData.Tables["cola_lookup"].AsEnumerable()
                                                      where COLATable.Field<Int32>("cola_id") == Convert.ToInt32(cmbCola.SelectedValue)
                                                      select COLATable;

            foreach (DataRow r in MinMax)
            {
                txtCOLAMin.Text = r["cola_min"].ToString();
                txtCOLAMax.Text = r["cola_max"].ToString();
            }
            
        }

        private bool chkPercentDiscoType(object sender)
        {
            ucLabelComboBox db = (ucLabelComboBox)sender;
            if (db.SelectedValue != null)
            {
                int DiscountType = Convert.ToInt32(db.SelectedValue);
                //Loop through the discount type datatable and find the currently selected row
                foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["discount_lookup"].Rows)
                {
                    //Once the correct record is found get the fixed rate flag and determine how to display
                    //1 = Currency / 0 = percentage
                    if (Convert.ToInt32(r["discount_id"]) == DiscountType)
                    {
                        if (Convert.ToInt32(r["fixed_rate_flag"]) == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// enable/disable line desc options combo as necessary
        /// </summary>
        private void setLineDescOptionsCbo()
        {
            if (txtLineDesc.Text != "" && txtLineDesc.Text != " ")
            {
                cmbLineDescOptions.IsEnabled = true;
                cmbLineDescOptions.CntrlFocus();
            }
            else
            {
                cmbLineDescOptions.SelectedValue = 0;
                cmbLineDescOptions.IsEnabled = false;
            }
        }

        private void txtLineDesc_LostFocus(object sender, RoutedEventArgs e)
        {
            //enable/disable line desc options combo as necessary
            setLineDescOptionsCbo();
        }

        private void cmbProdItem_SelectionChanged(object sender, RoutedEventArgs e)
        {
            prodDesc = "";
            this.txtProductDesc.Text = "";
            this.chkPrimaryProduct.IsChecked = 0;

            if (this.CurrentBusObj.ObjectData != null)
            {
                if (this.CurrentBusObj.ObjectData.Tables["product_lookup"].Rows.Count > 0)
                {
                    EnumerableRowCollection<DataRow> rows = from ProdLookup in CurrentBusObj.ObjectData.Tables["product_lookup"].AsEnumerable()
                                                              where ProdLookup.Field<Int32>("item_id") == Convert.ToInt32(cmbProdItem.SelectedValue)
                                                              select ProdLookup;

                    foreach (DataRow r in rows)
                    {
                        if (r["item_type"].ToString() != "")
                        {
                            this.chkPrimaryProduct.IsChecked = Convert.ToInt32(r["item_type"]);
                        }
                        prodDesc = r["product_code"].ToString();
                        this.txtProductDesc.Text = r["product_code"].ToString();

                    }
                }
            }
        }

        private void cmbCLMNumber_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //prodDesc = "";
            //this.txtProductDesc.Text = "";
            //this.chkPrimaryProduct.IsChecked = 0;

            //if (this.CurrentBusObj.ObjectData != null)
            //{
            //    if (this.CurrentBusObj.ObjectData.Tables["product_lookup"].Rows.Count > 0)
            //    {
            //        EnumerableRowCollection<DataRow> rows = from ProdLookup in CurrentBusObj.ObjectData.Tables["product_lookup"].AsEnumerable()
            //                                                where ProdLookup.Field<Int32>("item_id") == Convert.ToInt32(cmbProdItem.SelectedValue)
            //                                                select ProdLookup;

            //        foreach (DataRow r in rows)
            //        {
            //            if (r["item_type"].ToString() != "")
            //            {
            //                this.chkPrimaryProduct.IsChecked = Convert.ToInt32(r["item_type"]);
            //            }
            //            prodDesc = r["product_code"].ToString();
            //            this.txtProductDesc.Text = r["product_code"].ToString();

            //        }
            //    }
            //}
        }

        private void cmbCLMNumber_Loaded(object sender, RoutedEventArgs e)
        {
            //this.txtProductDesc.Text = prodDesc;
        }

        private void cmbProdItem_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtProductDesc.Text = prodDesc;
        }

        private void chkRateDispute_Checked(object sender, RoutedEventArgs e)
        {
            dtpStartDate.IsEnabled = false;
            dtpEndDate.IsEnabled = false;
            
            if (Convert.ToBoolean(chkRateDispute.IsChecked))
            {
                //txtratetorecognize.IsEnabled = true;
                //this.CurrentBusObj.ObjectData.Tables["product_rule"].Rows[0]["rate_dispute_flag"] = 2;
            }
        }

        private void chkRateDispute_UnChecked(object sender, RoutedEventArgs e)
        {
            dtpStartDate.IsEnabled = true;
            dtpEndDate.IsEnabled = true;
            //this.CurrentBusObj.ObjectData.Tables["product_rule"].Rows[0]["rate_dispute_flag"] = 0;
            //txtratetorecognize.IsEnabled = false;
            //txtratetorecognize.Text = "0";
        }

        private void chkPercentComplete_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void chkPercentComplete_Checked(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(chkPercentComplete.IsChecked))
            {
                chkCompletedContract.IsChecked = 0;
            }
        }

        private void chkPercentComplete_UnChecked(object sender, RoutedEventArgs e)
        {

        }

        private void chkCompletedContract_Checked(object sender, RoutedEventArgs e)
        {
            dtpStartDate.IsEnabled = false;
            dtpEndDate.IsEnabled = false;
            //if (Convert.ToBoolean(chkCompletedContract.IsChecked))
            //{
            //    chkPercentComplete.IsChecked = 0;
            //}
            //else

        }

        private void chkCompletedContract_UnChecked(object sender, RoutedEventArgs e)
        {
            dtpStartDate.IsEnabled = true;
            dtpEndDate.IsEnabled = true;
            //if (Convert.ToBoolean(chkCompletedContract.IsChecked))
            //{
            //    chkPercentComplete.IsChecked = 0;
            //}
        }
    }

}

