

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using RazerBase;
using RazerBase.Interfaces;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Linq;

namespace Contract
{
    /// <summary>
    /// This class represents a 'ContractLocationServiceLookup' object.
    /// </summary>
    public partial class ContractOpsAdd : ScreenBase
    {
        public cBaseBusObject ContractOpsAddBusObject = new cBaseBusObject();
        public int ContractId = 0;
        public bool errorsExist = false;
        //used to calculate scheduled bill date
        int offset = 0;
        int billFreq = 0;



        //contract object from caller
        cBaseBusObject ContractObj;
        /// <summary>
        /// Create a new instance of a 'ContractLocationServiceLookup' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractOpsAdd(int _ContractId, cBaseBusObject _ContractObj)
            : base()
        {
            //set obj
            this.CurrentBusObj = ContractOpsAddBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "ContractOpsAdd";
            ContractId = _ContractId;
            //get handle to contract obj
            ContractObj = _ContractObj;
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
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            this.MainTableName = "baseOps";
            //dtpStartDate.SelectedDateChangedDelegate = SelectedStartDateChanged;
            //dtpEndDate.SelectedDateChangedDelegate = SelectedEndDateChanged;
            this.CurrentBusObj.Parms.AddParm("@contract_id", ContractId);
            this.CurrentBusObj.Parms.AddParm("@rate_id", 0);
            this.CurrentBusObj.Parms.AddParm("@billing_location_id", 0);


            //load bus obj and inserting a blank row using the Select statement and also the Insert from robject
            base.New();

            //Establish default values
            if (CurrentBusObj.ObjectData.Tables["baseOps"].Rows != null)
            {

                chkNeverBillFlag.IsChecked = 0;
                txtContractId.Text = ContractId.ToString();
                this.cmbRuleID.IsEnabled = false;


                //set up new row and datatable to populate
                DataRow drOps = CurrentBusObj.ObjectData.Tables["baseOps"].Rows[0];
                drOps["contract_id"] = ContractId;
                drOps["rate_id"] = 0;
                drOps["rule_id"] = 0;
                drOps["cs_id"] = 0;
                drOps["service_period_start"] = "01/01/1900";
                drOps["service_period_end"] = "01/01/1900";
                //drOps["scheduled_bill_period"] = "";
                drOps["billing_type"] = 0;
                drOps["never_bill_flag"] = 0;
                drOps["user_override_flag"] = 1;
                drOps["billing_status_flag"] = 0;
                //load combobox for rates
                this.cmbRateID.SetBindingExpression("rate_id", "rate_id", this.CurrentBusObj.ObjectData.Tables["dddwrates"]);
                //load combobox for locations
                this.cmbLocation.SetBindingExpression("contract_location_id", "contract_location_desc", this.CurrentBusObj.ObjectData.Tables["dddwlocations"]);
                //load combobox for rules
                this.cmbRuleID.SetBindingExpression("rule_id", "rule_id", this.CurrentBusObj.ObjectData.Tables["dddwrules"]);

            }
        }

        public void cmbRateID_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ContractOpsAddBusObject.Parms.ClearParms();
            ContractOpsAddBusObject.Parms.AddParm("@contract_id", ContractId);
            ContractOpsAddBusObject.Parms.AddParm("@rate_id", cmbRateID.SelectedValue);
            this.cmbRuleID.IsEnabled = true;
            ContractOpsAddBusObject.LoadTable("dddwrules");

        }


        public void ValidatebeforeSave()
        {
            if (ldtServicePeriodStart.SelText.ToString() != "" && ldtServicePeriodStart.SelText != null)
            {
                string sDateportion = ldtServicePeriodStart.SelText.ToString().Substring(0, 9).Trim() ;
                if (sDateportion == "1/1/1900")
                {
                    errorsExist = true;
                    Messages.ShowInformation("Service Period Start Date is Required");
                    ldtServicePeriodStart.Focus();
                    return;
                }
            }
            else
            {
                errorsExist = true;
                Messages.ShowInformation("Service Period Start Date is Required");
                ldtServicePeriodStart.Focus();
                return;

            }
            if (ldtServicePeriodEnd.SelText.ToString() != "" && ldtServicePeriodEnd.SelText != null)
            {
                string sDateportionEnd = ldtServicePeriodEnd.SelText.ToString().Substring(0, 9).Trim();
                if (sDateportionEnd == "1/1/1900")
                {
                    errorsExist = true;
                    Messages.ShowInformation("Service Period End Date is Required");
                    ldtServicePeriodStart.Focus();
                    return;
                }
            }
            else
            {
                errorsExist = true;
                Messages.ShowInformation("Service Period End Date is Required");
                ldtServicePeriodStart.Focus();
                return;

            }

            if (ldtServicePeriodStart.SelText >= ldtServicePeriodEnd.SelText)
            {
                errorsExist = true;
                Messages.ShowInformation("Service Period Start Date cannnot be greater than Service Period End Date");
                ldtServicePeriodStart.Focus();
                return;

            }

            if (cmbLocation.SelectedValue == null)
            {

                errorsExist = true;
                Messages.ShowInformation("Location is Required");
                cmbLocation.Focus();
                return;
            }

            //if (Convert.ToInt32(cmbRateID.SelectedValue) = 0)
            //{
            //}
            if (cmbRateID.SelectedValue.ToString() == "0")
            {
                errorsExist = true;
                Messages.ShowInformation("Rate is Required");
                cmbRateID.Focus();
                return;
            }
            if (cmbRuleID.SelectedValue.ToString() == "0")
            {
                errorsExist = true;
                Messages.ShowInformation("Rule is Required");
                cmbRuleID.Focus();
                return;
            }
            //Validate there is not already a billing location for this location, rate_id, rule_id, service_period_start, Service_period_end
            int contractLocation = Convert.ToInt32(cmbLocation.SelectedValue);
            int contractRateID = Convert.ToInt32(cmbRateID.SelectedValue);
            int contractRuleID = Convert.ToInt32(cmbRuleID.SelectedValue);
            //DateTime dateStartdt = DateTime.Parse(ldtServicePeriodStart.SelText.ToString());
            string dateStart = ldtServicePeriodStart.SelText.ToString().Substring(0, 9).Trim();
            string dateEnd = ldtServicePeriodEnd.SelText.ToString().Substring(0, 9).Trim();

            foreach (DataRow dr in ContractObj.ObjectData.Tables["ops"].Rows)
            {
                if ((Convert.ToInt32(dr["contract_location_id"]) == contractLocation) && (Convert.ToInt32(dr["rate_id"]) == contractRateID)  &&
                   (Convert.ToInt32(dr["rule_id"]) == contractRuleID) &&
                   ((Convert.ToString(dr["service_period_start"]).Substring(0,9).Trim()) == dateStart) &&
                   ((Convert.ToString(dr["service_period_end"]).Substring(0,9).Trim()) == dateEnd))
                {
                    errorsExist = true;
                    Messages.ShowInformation("Billing Location already exists for this criteria");
                    return;
                }


            }
           
            if (errorsExist == false)
            //reset parms before Save
            {
                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.AddParm("@contract_id", ContractId);
                this.CurrentBusObj.Parms.AddParm("@rate_id", cmbRateID.SelectedValue);
      

                var RateIDRow = (from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                 where x.Field<string>("parmName").ToLower() == "@rate_id"
                                 select x).Single();

                if (RateIDRow != null)
                    RateIDRow["parmValue"] = cmbRateID.SelectedValue.ToString();

            }


        }





      
       

        private void cmbLocation_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            //Need to set the cs_id based on the location selected
            if (cmbLocation.SelectedValue.ToString() != "0")
            {
                int contractLocation = Convert.ToInt32(cmbLocation.SelectedValue);
                //Loop through the contract location datatable to find the currently selected row
                foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["dddwlocations"].Rows)
                    //once the current row is found, set the cs_id
                    if (Convert.ToInt32(r["contract_location_id"]) == contractLocation)
                    {
                        int csID = Convert.ToInt32(r["cs_id"]);
                        txtLocationID.Text = csID.ToString();
                        return;
                    } 
            

            }



        }

        private void cmbRuleID_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            //Need to set the cs_id based on the location selected
            if (cmbRuleID.SelectedValue.ToString() != "0")
            {
                int contractrule = Convert.ToInt32(cmbRuleID.SelectedValue);
                //Loop through the contract location datatable to find the currently selected row
                foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["dddwrules"].Rows)
                    //once the current row is found, set the cs_id
                    if (Convert.ToInt32(r["rule_id"]) == contractrule)
                    {
                        offset = Convert.ToInt32(r["service_period_offset"]);
                        billFreq = Convert.ToInt32(r["bill_frequency"]);
                      
                        return;
                    }


            }

        }

        private void ldtServicePeriodStart_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ldtServicePeriodStart.SelText.ToString() != "" && ldtServicePeriodStart.SelText != null)
            {
                string sDateportion =  ldtServicePeriodStart.SelText.ToString().Substring(0, 8);
                if (sDateportion != "1/1/1900")
                {
                    //get the offset to calculate the scheduled bill period
                    DateTime scheduleDate;
                
                   
                     scheduleDate = DateTime.Parse(ldtServicePeriodStart.SelText.ToString()).AddMonths(offset * -1);
                     txtSchedBillPeriod.Text = scheduleDate.ToString();
                    //Calculate the service Period End date
                    DateTime endDate;
                    endDate = DateTime.Parse(ldtServicePeriodStart.SelText.ToString()).AddMonths(billFreq);
                    ldtServicePeriodEnd.SelText = DateTime.Parse(endDate.ToString()).AddDays(-1);

                }
            }
        }

      

        private void cmbRateID_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            this.CurrentBusObj.Parms.ClearParms();
            this.CurrentBusObj.Parms.AddParm("@contract_id", ContractId);
            this.CurrentBusObj.Parms.AddParm("@rate_id", cmbRateID.SelectedValue);
            this.cmbRuleID.IsEnabled = true;
            this.CurrentBusObj.LoadTable("dddwrules");
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ValidatebeforeSave();
            if (errorsExist)
            {
                errorsExist = false;
                return;
            }
            else
                Save();
        }


        public override void Save()
        {
            //we do not need to bind after saving
            BypassBindafterSave = true;

            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Save Successful");
                //add contract_id parm for refresh of location grid on location tab
                //ContractObj.Parms.AddParm("@contract_id", ContractId);
                ContractObj.LoadTable("ops");
                CloseScreen();
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }

        private void CloseScreen()
        {
            System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            //this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                AdjParent.Close();
            }
        }

        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            //this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                AdjParent.Close();
            }
        }
         
    }
}
