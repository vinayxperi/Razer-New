

using RazerBase;
using RazerInterface;
using System;
using System.Windows;
using System.Data;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;

namespace Contract
{
    /// <summary>
    /// This class represents a 'ContractLocationMove' object.
    /// </summary>
    public partial class ContractRatesCopy : ScreenBase, IPreBindable
    {
        public cBaseBusObject ContractRateCopyBusObject = new cBaseBusObject();
        public int ContractId = 0;
        public int rateID = 0;
        public int oldrateID = 0;
        public bool IsSingleClickOrigin { get; set; }
        public string NewEndDate = "";
        public int NewStatus = 1;
        //for rates status flag
        public ComboBoxItemsProvider cmbRatesStatusFlagCombo { get; set; }
      
        private DataRow rDefault;
        //contract object from caller
        cBaseBusObject ContractObj;

        /// Create a new instance of a 'ContractLocationServiceLookup' object and call the ScreenBase's constructor.

        public ContractRatesCopy(int _ContractId,  cBaseBusObject _ContractObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = ContractRateCopyBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "ContractRateCopy";
            ContractId = _ContractId;
            //get handle to contract obj
            ContractObj = _ContractObj;
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }

        
        /// This method performs initializations for this object.
       
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            this.MainTableName = "main";
            
            
            this.CurrentBusObj.Parms.AddParm("@contract_id", ContractId);

            this.CurrentBusObj.Parms.AddParm("@rate_id", 0);
            //setup locations grid
            GridRatesToCopy.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(GridRatesToCopy_EditModeEnded);
            GridRatesToCopy.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            GridRatesToCopy.MainTableName = "main";
            GridRatesToCopy.ConfigFileName = "ContractRateCopy";
            GridRatesToCopy.SetGridSelectionBehavior(false, false);
            GridRatesToCopy.IsFilterable = false;
            GridRatesToCopy.DoNotSelectFirstRecordOnLoad = true;
            GridRatesToCopy.FieldLayoutResourceString = "GridAvailRatesToCopy";
            GridRatesToCopy.SingleClickZoomDelegate = RatesSingleClickDelegate;
            GridRatesToCopy.IsEnabled = true;
            GridCollection.Add(GridRatesToCopy);
            GridRulesToCopy.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            GridRulesToCopy.MainTableName = "rulesAvail";
            GridRulesToCopy.ConfigFileName = "ContractRuleCopy";
            GridRulesToCopy.SetGridSelectionBehavior(true, true);
            GridRulesToCopy.IsFilterable = false;
            GridRulesToCopy.DoNotSelectFirstRecordOnLoad = true;
            GridRulesToCopy.FieldLayoutResourceString = "GridAvailRulesToCopyforRate";  
            GridCollection.Add(GridRulesToCopy);
     
          
            //load bus obj
            this.Load();
            TemplateBorder.IsEnabled = true;
           //set chkCopyAllRules to false
            chkCopyAllRules.IsChecked = 0;
            if (this.CurrentBusObj.ObjectData != null)
            {
                //if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
                //    (GridRatesToCopy.xGrid.Records[GridRatesToCopy.ActiveRecord.Index] as DataRecord).Cells["new_end_date"].IsActive = true;
                GridRatesToCopy.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
            }
          
            
        }


        private void RatesSingleClickDelegate()
        {

           //load Rules for selected rate
            if (GridRatesToCopy.xGrid.ActiveRecord != null)
            {
                GridRatesToCopy.ReturnSelectedData("rate_id");
                rateID = Convert.ToInt32(cGlobals.ReturnParms[0]);
                //save to reset later
                //oldrateID = rateID;
                CurrentBusObj.Parms.UpdateParmValue("@rate_id", rateID);
                //Flag rate selected to copy
                int ctr = 0;

                foreach (Record record in GridRatesToCopy.xGrid.SelectedItems.Records)
                {
                    ctr++;
                    DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                    r["copy_rate"] = 1;
                    //if (r["new_end_date"].ToString() != "1/1/1900") NewEndDate = r["new_end_date"].ToString();
                }
                if (oldrateID != rateID)
                   CurrentBusObj.LoadTable("rulesAvail");
                oldrateID = rateID;
                if (CurrentBusObj.ObjectData.Tables["rulesAvail"] != null && CurrentBusObj.ObjectData.Tables["rulesAvail"].Rows.Count > 0)
                {

                }

                else
                {
                    Messages.ShowInformation("No Rules exist.  Click on a rate to display available rules to copy.");
                }
            }
            else
            {
                Messages.ShowInformation("No Rate selected.  Click on a rate to select the rate to copy.");
            }

        }

        void GridRatesToCopy_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            //commit user entered value to datatable
            GridRatesToCopy.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            DateTime DEndDate;
            DateTime DNewStartDate;
            DateTime DNewEndDate;

            //capture new end date if it exists
            foreach (DataRecord r in GridRatesToCopy.xGrid.Records)
            {
                DNewEndDate = Convert.ToDateTime(r.Cells["new_end_date"].Value.ToString());
                //if ((r.Cells["new_end_date"].Value.ToString() != "1/1/1900") && (Convert.ToInt32(r.Cells["copy_rate"].Value) == 1))
                if ((DNewEndDate.Date.ToString() != "1/1/1900") && (DNewEndDate.Date.ToString() != "1/1/1900 12:00:00 AM") && (Convert.ToInt32(r.Cells["copy_rate"].Value) == 1))
                {
                    //NewEndDate = r.Cells["new_end_date"].Value.ToString();
                    DEndDate = Convert.ToDateTime(r.Cells["end_date"].Value.ToString());
                    DNewStartDate = DEndDate.AddDays(1);
                    //DNewEndDate = Convert.ToDateTime(NewEndDate);
                    if ((DNewEndDate < DNewStartDate) || (DNewEndDate == DNewStartDate))
                    {
                        MessageBox.Show("New End Date is Invalid");
                        r.Cells["new_end_date"].Value = "1/1/1900";
                    }
                    NewEndDate = r.Cells["new_end_date"].Value.ToString();
                }
                if ((r.Cells["new_status"].Value.ToString() != "1") && (Convert.ToInt32(r.Cells["copy_rate"].Value) == 1))
                    NewStatus = Convert.ToInt32(r.Cells["new_status"].Value.ToString());
                //if (chkCopyAllRules.IsChecked == 1) chkCopyAllRules_Checked(sender,);

            }
            
        }


        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {

            //Loop through the grid and if it is highlighted, set the copy indicator on 
            // SP will first insert the new rules for the new rate
  
            int ctr = 0;

            foreach (Record record in GridRulesToCopy.xGrid.SelectedItems.Records)
            {
                ctr++;
                DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                r["copy_rule"] = 1;
            }
    
            if (ctr == 0)
            {
                MessageBox.Show("No rules selected to copy");
                return;
            }
            Save();
        }

        private void updGridRuleswithRateId(int RateID)
        {
             
           

            //loop through main Rules data table and update columns
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["rulesAvail"].Rows)
            {
                bool rowIsNotModified = (dr.RowState != DataRowState.Modified);
                dr["new_rate_id"] = RateID;
               
                
            }

            
        }

        public override void Save()
        {
            int newRateID = 0;
            if (NewEndDate == "") NewEndDate = "1/1/1900";
            newRateID = cGlobals.BillService.InsertNewRatefromRateCopy(rateID, NewEndDate, NewStatus, cGlobals.UserName.ToLower());
            if (newRateID != 0)
                updGridRuleswithRateId(newRateID);
            else
            {
                Messages.ShowError("Problem Copying Rate");
                return;
            }
            ////reset the parm
            //CurrentBusObj.Parms.ClearParms();
            //CurrentBusObj.Parms.AddParm("@contract_id", ContractId);
            //CurrentBusObj.Parms.AddParm ("@rate_id", oldrateID);
           
            base.Save();
            if (SaveSuccessful)
            {
                //error messages from Stored Proc
                string strSPErrMg = getInfoFromStoredProc();
                if (strSPErrMg != null && strSPErrMg != "")
                {
                    Messages.ShowError(strSPErrMg);
                    //do rollback
                    rollbackCopy(newRateID);
                    
                }
                else
                {
                    Messages.ShowInformation("Save Successful--New Rate ID = " + newRateID);
                     
                    //close me/////////////////////////////////////////////////////////////////////////////////
                    System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);
                    this.CurrentBusObj.ObjectData.AcceptChanges();
                    StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
                    this.SaveSuccessful = true;
                    Messages.ShowInformation("Copy Successful");
                    //add contract_id parm for refresh of location grid on rates tab
                     ContractObj.Parms.UpdateParmValue ("@contract_id", ContractId);
                     ContractObj.LoadTable("rates");
                     CloseScreen();
                }
            }
            else
            {
                Messages.ShowInformation("Data Error, Save Failed");
                //do rollback
                rollbackCopy(newRateID);
                 
           
               
            }
            
        }


        private string getInfoFromStoredProc()
        {
            var SPErrorMsg = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                             where x.Field<string>("parmName") == "@error_message"
                             select new
                             {
                                 parmName = x.Field<string>("parmName"),
                                 parmValue = x.Field<string>("parmValue")
                             };
            foreach (var info in SPErrorMsg)
            {
                if (info.parmName == "@error_message")
                    return info.parmValue;
            }
            return "";
        }

        private void rollbackCopy(int newRateID)
        {
            //delete rate
            bool retVal = cGlobals.BillService.DeleteRatefromRateCopy(newRateID);
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

        private void chkCopyAllRules_Checked(object sender, RoutedEventArgs e)
        {
            if (chkCopyAllRules.IsChecked == 1)
            {
                //loop through and uncheck copy_rule flags

                int ctr = 0;
                foreach (Record record in GridRulesToCopy.xGrid.Records)
                {
                    ctr++;
                    //Select (highlight) rows to copy
                    GridRulesToCopy.xGrid.ActiveRecord = record;
                    record.IsActive = true;
                    record.IsSelected = true;

                    DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                                   
                    r["copy_rule"] = 1;
                }
                 

                
            }
           
            }


        private void chkCopyAllRules_UnChecked(object sender, RoutedEventArgs e)
        {
            if (chkCopyAllRules.IsChecked == 0)
            {
                //loop through and check copy_rule_flags
                int ctr = 0;
                foreach (Record record in GridRulesToCopy.xGrid.Records)
                {
                    ctr++;
                    //unselect if they are selected
                    record.IsActive = false;
                    record.IsSelected = false;
                    DataRow r = ((record as DataRecord).DataItem as DataRowView).Row;
                            r["copy_rule"] = 0;
                }
            }
        }

        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    //Add code to populate the new status combobox
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    //this.cmbStatus.SetBindingExpression("fkey_int", "code_value", this.CurrentBusObj.ObjectData.Tables["rates_status_lookup"]);
                    ip.ItemsSource = this.CurrentBusObj.ObjectData.Tables["rate_status_lookup"].DefaultView;

                    //set the value and display path
                    ip.ValuePath = "fkey_int";
                    ip.DisplayMemberPath = "code_value";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbRatesStatusFlagCombo = ip;
                 }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }

      

        

      
    }
}
