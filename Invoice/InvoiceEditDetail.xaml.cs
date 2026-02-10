

#region using statements

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
using System.Data;
using Infragistics.Windows.DataPresenter;


#endregion

namespace Invoice
{

    public partial class InvoiceEditDetail : ScreenBase, IPreBindable
    {
        public DateTime OldDueDate;
        public DateTime OldInvoiceDate;
        public string OldTerms;
        public int i;
        public string clear_flag = "N";
        //public string NewCumulative = "N";
        //public string CalcCumulative = "N";
        //public int cumulative_rule = 0; 
        //public int cumulative_tier = 0;
        //public decimal cumulative_units = 0;
        //public decimal location_total = 0;
        //public bool IsLoading { get; set; }
        //private static readonly string approvalObjectName = "EditInvoiceapproval";


        public InvoiceEditDetail(cBaseBusObject invEditObj)
            : base()
        {
            // set the businessObject
            this.CurrentBusObj = invEditObj;
            // This call is required by the designer.
            InitializeComponent();
            //cumulative_rule = 0;
            Init();
        }

        public void Init()
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = false;
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;
            //this.MainTableName = "InvoiceEdit";
            this.MainTableName = "invedit";
            //set up grids
            gInvoiceEdit.xGrid.FieldSettings.AllowEdit = false;
            gInvoiceEdit.MainTableName = "invedit";
            gInvoiceEdit.SetGridSelectionBehavior(false, false);
            gInvoiceEdit.FieldLayoutResourceString = "InvoiceEdit";
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            gInvoiceEdit.GridCellValuePresenterStyle = CellStyle;
            gInvoiceEdit.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            gInvoiceEdit.DoNotSelectFirstRecordOnLoad = true;
            //gInvoiceEdit.GridGotFocusDelegate = gUnitEntryGrid_GotFocus; //This ties the got focus event of the  grid to this method.
            gInvoiceEdit.EditModeEndedDelegate = gUnitEntryGrid_EditModeEnded; //This allows for data checks after each cell is exited
            gInvoiceEdit.SkipReadOnlyCellsOnTab = true; //
            GridCollection.Add(gInvoiceEdit);
            this.Load();
            //IsLoading = false;
            ldtInvoiceDate.SelText = Convert.ToDateTime(CurrentBusObj.ObjectData.Tables["invedit"].Rows[0]["invoice_date"]);
            ldtDueDate.SelText = Convert.ToDateTime(CurrentBusObj.ObjectData.Tables["invedit"].Rows[0]["due_date"]);
            OldInvoiceDate = Convert.ToDateTime(CurrentBusObj.ObjectData.Tables["invedit"].Rows[0]["invoice_date"]);
            OldDueDate = Convert.ToDateTime(CurrentBusObj.ObjectData.Tables["invedit"].Rows[0]["due_date"]);
            OldTerms = CurrentBusObj.ObjectData.Tables["invedit"].Rows[0]["terms"].ToString();
            //this.cmbTerms.SelectedValue = CurrentBusObj.ObjectData.Tables["invedit"].Rows[0]["terms"].ToString();
            //tTerms.Text = CurrentBusObj.ObjectData.Tables["invedit"].Rows[0]["terms"].ToString();
            //string sdebug = ""; 
            this.gInvoiceEdit.xGrid.EditModeStarting += xGrid_EditModeStarting;
        }

        private void xGrid_EditModeStarting(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartingEventArgs e)
        {
            var record = e.Cell.Record as DataRecord;

            if (e.Cell.Field.Name == "units" && record != null)
            {

                var calcMethod = Convert.ToInt32(record.Cells["calc_method"].Value);
                var tierNumber = Convert.ToInt32(record.Cells["tier_number"].Value);
                var lineDescription = Convert.ToString(record.Cells["line_description"].Value);
                var ruleType = Convert.ToString(record.Cells["rule_type"].Value);

                bool isMinimumLine = lineDescription.Contains("Minimum") && ruleType == "1";
                bool allowEdit = (calcMethod != 3) || (calcMethod == 3 && tierNumber == 1);

                if (!allowEdit || isMinimumLine)
                {
                    e.Cancel = true; // Prevent editing
                }

            }
        }


        public void gUnitEntryGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            //DWR Modified 3/26/13 -- to use the row the cell is on and not the active record.  
            //Active Record was causing the changes to be made to the wrong row if the user clicked on a different row.
            //DWR -- Also changed every instance of ActiveRecord in this event to use the row instead.
            DataRecord row = e.Cell.Record; //gUnitEntry.ActiveRecord;           
        }

        public void PreBind()
        {
            //check this to keep recursion to this event from occurruing when load is called
            //if (IsLoading == true) return;
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                this.cmbTerms.SetBindingExpression("terms_code", "description", this.CurrentBusObj.ObjectData.Tables["terms"]);
                this.cmbTerms.SelectedValue = CurrentBusObj.ObjectData.Tables["invedit"].Rows[0]["terms"].ToString();
            }
        }

        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //foreach (DataRecord x in gInvoiceEdit.xGrid.Records)
            //{
            //CurrentBusObj.Parms.AddParm("@terms", this.cmbTerms.SelectedValue);
            clear_flag = "Y";
            //cumulative_rule = 0;
            //cumulative_units = 0;
            //cumulative_tier = 0;
            //CalcCumulative = "N";
            this.Load();
            //}
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            i = 0;
            string invoice_number = "";

            cBaseBusObject PopEditInvoice = new cBaseBusObject("PopEditInvoice");
            foreach (DataRecord x in gInvoiceEdit.xGrid.Records)
            {
                invoice_number = x.Cells["invoice_number"].Value.ToString();
                //cBaseBusObject PopEditInvoice = new cBaseBusObject("PopEditInvoice");
                PopEditInvoice.Parms.ClearParms();
                PopEditInvoice.Parms.AddParm("@invoice_number", x.Cells["invoice_number"].Value.ToString());
                PopEditInvoice.Parms.AddParm("@inv_line_id", Convert.ToInt32(x.Cells["inv_line_id"].Value));
                PopEditInvoice.Parms.AddParm("@cs_id", Convert.ToInt32(x.Cells["cs_id"].Value));
                PopEditInvoice.Parms.AddParm("@service_period_start", Convert.ToDateTime(x.Cells["service_period_start"].Value));
                PopEditInvoice.Parms.AddParm("@service_period_end", Convert.ToDateTime(x.Cells["service_period_end"].Value));
                PopEditInvoice.Parms.AddParm("@item_description", x.Cells["item_description"].Value.ToString());
                PopEditInvoice.Parms.AddParm("@contract_id", Convert.ToInt32(x.Cells["contract_id"].Value));
                PopEditInvoice.Parms.AddParm("@line_description", x.Cells["line_description"].Value.ToString());
                PopEditInvoice.Parms.AddParm("@units", Convert.ToDecimal(x.Cells["units"].Value));
                PopEditInvoice.Parms.AddParm("@rate", Convert.ToDecimal(x.Cells["rate"].Value));
                PopEditInvoice.Parms.AddParm("@extended", Convert.ToDecimal(x.Cells["extended"].Value));
                PopEditInvoice.Parms.AddParm("@psa_cityst", x.Cells["psa_cityst"].Value.ToString());
                //PopEditInvoice.Parms.AddParm("@edit_flag", x.Cells["edit_flag"].Value.ToString());
                PopEditInvoice.Parms.AddParm("@calc_method", Convert.ToInt32(x.Cells["calc_method"].Value));
                PopEditInvoice.Parms.AddParm("@rate_id", Convert.ToInt32(x.Cells["rate_id"].Value));
                PopEditInvoice.Parms.AddParm("@rule_id", Convert.ToInt32(x.Cells["rule_id"].Value));

                if (ldtDueDate.SelText == OldDueDate)
                    PopEditInvoice.Parms.AddParm("@due_date", "1/1/1900");
                else
                    PopEditInvoice.Parms.AddParm("@due_date", ldtDueDate.SelText);
                if (ldtInvoiceDate.SelText == OldInvoiceDate)
                    PopEditInvoice.Parms.AddParm("@invoice_date", "1/1/1900");
                else
                    PopEditInvoice.Parms.AddParm("@invoice_date", ldtInvoiceDate.SelText);
                if (cmbTerms.SelectedText == OldTerms)
                    PopEditInvoice.Parms.AddParm("@terms", "");
                else
                    PopEditInvoice.Parms.AddParm("@terms", cmbTerms.SelectedValue);
                //CurrentBusObj.Parms.AddParm("@user_id", cGlobals.UserName.ToLower());
                //CurrentBusObj.Parms.AddParm("@amount", 0);
                i = i + 1;
                if (gInvoiceEdit.xGrid.Records.Count == i)
                    PopEditInvoice.Parms.AddParm("@edit_flag", "S");
                else
                    PopEditInvoice.Parms.AddParm("@edit_flag", x.Cells["edit_flag"].Value.ToString());

                if (x.Cells["line_description"].Value.ToString() == "****************No longer using this tier***************")
                    if (gInvoiceEdit.xGrid.Records.Count == i)
                        PopEditInvoice.Parms.UpdateParmValue("@edit_flag", "X");
                    else
                        PopEditInvoice.Parms.UpdateParmValue("@edit_flag", "D");

                if (x.Cells["line_description"].Value.ToString() == "****************No longer using minimum***************")
                    if (gInvoiceEdit.xGrid.Records.Count == i)
                        PopEditInvoice.Parms.UpdateParmValue("@edit_flag", "X");
                    else
                        PopEditInvoice.Parms.UpdateParmValue("@edit_flag", "D");

                gInvoiceEdit.MainTableName = "invedit";
                PopEditInvoice.LoadTable("InsInvoice");
                //this.Load();
                //this.Save();
            }
            gInvoiceEdit.MainTableName = "detail_acct";
            Messages.ShowInformation("Invoice Reprint job scheduled");

            //Clear Workflow
            cBaseBusObject Approvalobj = new cBaseBusObject("EditInvoiceapproval");
            Approvalobj.Parms.ClearParms();
            Approvalobj.Parms.AddParm("@document_id", "");
            Approvalobj.LoadData();
            Invoice.EditInvoiceApprovalTab.ExposeGrid.LoadGrid(Approvalobj, "approval");

            CloseScreen();

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

        private void ldtInvoiceDate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ldtInvoiceDate.SelText.ToString() != "")
            {
                cBaseBusObject GetTerms = new cBaseBusObject("TermsLookup");
                GetTerms.Parms.AddParm("@terms_code", this.cmbTerms.SelectedValue);
                GetTerms.Parms.AddParm("@invoice_date", ldtInvoiceDate.SelText);
                GetTerms.LoadTable("termdays");
                if (GetTerms.ObjectData.Tables["termdays"] == null || GetTerms.ObjectData.Tables["termdays"].Rows.Count < 1)
                {
                    Messages.ShowInformation("Term code not found.");
                }
                else
                {
                    ldtDueDate.SelText = Convert.ToDateTime(GetTerms.ObjectData.Tables["termdays"].Rows[0]["due_date"]);
                }

            }
        }

        private void ldtDueDate_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        //private void cmbTerms_LostFocus(object sender, RoutedEventArgs e)
        private void cmbTerms_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (ldtInvoiceDate.SelText.ToString() != "" && clear_flag != "Y")
            {
                cBaseBusObject GetTerms = new cBaseBusObject("TermsLookup");
                GetTerms.Parms.AddParm("@terms_code", this.cmbTerms.SelectedValue);
                GetTerms.Parms.AddParm("@invoice_date", ldtInvoiceDate.SelText);
                GetTerms.LoadTable("termdays");
                if (GetTerms.ObjectData.Tables["termdays"] == null || GetTerms.ObjectData.Tables["termdays"].Rows.Count < 1)
                {
                    Messages.ShowInformation("Term code not found.");
                }
                else
                {
                    ldtDueDate.SelText = Convert.ToDateTime(GetTerms.ObjectData.Tables["termdays"].Rows[0]["due_date"]);
                }

            }
            clear_flag = "N";
        }

        private bool CalcMinimum(IEnumerable<DataRecord> Records)
        {
            bool minAmountFound = false;
            decimal totalAmount = 0;
            decimal minAmount = 0;
            bool calcOnlyAmount = false;
            bool minLineExists = false;
            bool minRuleGroupExists = false;
            DateTime servicePeriodEnd = DateTime.MinValue;
            cBaseBusObject GetFlat = null;
            int minCount = 0;
            //In some scenario's only the minimum line will be sent! If it's just the minimum, return; 

            foreach (DataRecord r in Records)
            {
                if (!(r.Cells["line_description"].Value.ToString().Contains("Minimum") && Convert.ToString(r.Cells["rule_type"].Value) == "1"))
                {
                    break;
                }
                else
                {
                    minCount++;
                }
            }

            if (Records.Count() == minCount)
            {
                //Only minimum lines; return;
                return false;
            }
            foreach (DataRecord r in Records)
            {
                GetFlat = new cBaseBusObject("GetTier");
                GetFlat.Parms.AddParm("@rate_id", Convert.ToInt32(r.Cells["rate_id"].Value));

                // *****   create new sp to lookup min for rate_id and put in GetTier
                GetFlat.Parms.AddParm("@rule_id", Convert.ToInt32(r.Cells["rule_id"].Value));
                GetFlat.Parms.AddParm("@units", Convert.ToDecimal(r.Cells["units"].Value));
                GetFlat.LoadTable("tieramount");
                if (GetFlat.ObjectData.Tables["tieramount"] == null || GetFlat.ObjectData.Tables["tieramount"].Rows.Count < 1)
                {
                    Messages.ShowInformation("Minimum not found");
                    return false;
                }
                minAmount = Convert.ToDecimal(GetFlat.ObjectData.Tables["tieramount"].Rows[0]["minimum"]);

                if (minAmount > 0)
                {
                    minAmountFound = true;
                    calcOnlyAmount = true;
                    break;
                }

            }
            //Find if Minimum rule group exists.
            foreach (DataRecord r in Records)
            {
                try
                {
                    if (Convert.ToString(r.Cells["rule_group_id"].Value) == "3")
                    {
                        minRuleGroupExists = true;
                        break;
                    }
                }
                catch
                {
                    continue;
                }
            }

            foreach (DataRecord r in Records)
            {
                if ((Convert.ToString(r.Cells["item_type"].Value) == "1" || minRuleGroupExists) && r.Cells["line_description"].Value.ToString().Contains("Minimum") && Convert.ToString(r.Cells["rule_type"].Value) == "1")
                {
                    minLineExists = true;
                }
                else //Not the minimum line
                {
                    if (minAmountFound && minRuleGroupExists)   //Rule group is minimum. Include only minimum rule_types. 
                    {
                        //if(Convert.ToString(r.Cells["rule_type"].Value) == "1")   //Rule_type is minimum for this record. 
                        //product item does not need to be of primary if minRuleGroupExists. 
                        if (Convert.ToString(r.Cells["rule_group_id"].Value) == "3")
                        //It's a primary product type And, rule is not cumulative. Include it in the minimum calcualtion.
                        {
                            if (calcOnlyAmount)
                            {
                                if (servicePeriodEnd == DateTime.MinValue)
                                    servicePeriodEnd = Convert.ToDateTime(r.Cells["service_period_end"].Value);
                                //Call cumulative only for the tier 1. 
                                if (r.Cells["calc_method"].Value.ToString() == "3" && Convert.ToString(r.Cells["tier_number"].Value) == "1")
                                    totalAmount += calcCumulative(Records, false, calcOnlyAmount);
                                //Calculate the bill amount for this invoice line and add it to totalAmount.
                                totalAmount += updateInvoiceLine(r, calcOnlyAmount);
                            }

                            //MessageBox.Show(r.Cells["line_description"].ToString() + "is included");
                        }
                        //}
                    }
                    else if (minAmountFound)//Minimum exists but rule_group is not minimum. Include all lines except for cumulative and sub products. 
                    {
                        //RUle group is not minimu type. Include all invoice lines that are primary product type in the minimum calculation.
                        if (Convert.ToString(r.Cells["item_type"].Value) == "1")   //It's a primary product type. Include it in the minimum calcualtion.
                        {
                            if (servicePeriodEnd == DateTime.MinValue)
                                servicePeriodEnd = Convert.ToDateTime(r.Cells["service_period_end"].Value);
                            //Call cumulative only for the tier 1. 
                            if (r.Cells["calc_method"].Value.ToString() == "3" && Convert.ToString(r.Cells["tier_number"].Value) == "1")
                                totalAmount += calcCumulative(Records, false, calcOnlyAmount);
                            //Calculate the bill amount for this invoice line and add it to totalAmount.
                            totalAmount += updateInvoiceLine(r, calcOnlyAmount);
                        }
                    }
                }
            }

            calcOnlyAmount = false;

            if (totalAmount < minAmount)
            {
                //Add the minimum line. If it does not already exist.
                if (minLineExists)
                {
                    //Do nothing. Just update the other lines and drop minimum charge. 
                    foreach (DataRecord r in Records)
                    {
                        if ((Convert.ToString(r.Cells["item_type"].Value) == "1" || minRuleGroupExists) && r.Cells["line_description"].Value.ToString().Contains("Minimum") && Convert.ToString(r.Cells["rule_type"].Value) == "1")
                        {
                            //Minimum line// Leave it as it is!!
                            if (servicePeriodEnd != DateTime.MinValue && servicePeriodEnd != Convert.ToDateTime(r.Cells["service_period_end"].Value))
                            {//Not the current service period
                                r.Cells["extended"].Value = 0;
                                r.Cells["units"].Value = r.Cells["old_units"].Value;
                            }
                            else
                            {
                                r.Cells["extended"].Value = minAmount;
                                r.Cells["units"].Value = r.Cells["old_units"].Value;
                            }
                        }
                        else //Not the minimum line
                        {
                            if (Convert.ToString(r.Cells["units"].Value) != Convert.ToString(r.Cells["old_units"].Value))//Value has changed.
                            {
                                r.Cells["color_status"].Value = 1;
                                updateInvoiceLine(r);//Update all lines except for the minimum line. 
                            }
                            else
                                r.Cells["color_status"].Value = 0;

                            if (minRuleGroupExists)
                            {
                                if (Convert.ToString(r.Cells["rule_group_id"].Value) == "3" /*&& Convert.ToString(r.Cells["item_type"].Value) == "1" */ && r.Cells["calc_method"].Value.ToString() != "3")
                                    r.Cells["extended"].Value = 0;  //Make the value 0 as minimum charge will be dropped. Only if it's a sub product or not cumulative rule type.                                                   
                            }
                            else
                            {
                                if (Convert.ToString(r.Cells["item_type"].Value) == "1" && r.Cells["calc_method"].Value.ToString() != "3")
                                    r.Cells["extended"].Value = 0;  //Make the value 0 as minimum charge will be dropped. Only if it's a sub product or not cumulative rule type.                                                   
                            }
                        }
                    }
                }
                return true;
                /* else
                 {
                     foreach (DataRecord r in Records)
                     {

                         if (r.Cells["units"].Value.ToString() != r.Cells["old_units"].Value.ToString())//Value has changed.
                         {
                             updateInvoiceLine(r);//Update all lines except for the minimum line. 
                             r.Cells["color_status"].Value = 1;
                         }
                         else
                             r.Cells["color_status"].Value = 0;


                         if (minRuleGroupExists)   //Rule group is minimum. Include only minimum rule_types. 
                         {
                             if (r.Cells["rule_type"].Value.ToString() == "1")   //Rule_type is minimum for this record. 
                             {
                                 if (r.Cells["item_type"].Value.ToString() == "1" && r.Cells["rule_id"].Value.ToString() != cumulative_rule.ToString())
                                     r.Cells["extended"].Value = 0;  //Make the value 0 as minimum charge will be dropped. Only if it's a sub product or not cumulative rule type. 
                             }
                         }
                         else
                         {
                             if (r.Cells["item_type"].Value.ToString() == "1" && r.Cells["rule_id"].Value.ToString() != cumulative_rule.ToString())
                                 r.Cells["extended"].Value = 0;
                         }

                     }
                     MessageBox.Show("Add the minimum line");
                 }
                 */

            }
            else if (totalAmount >= minAmount) // Calcualte each line and display accordingly.
            {
                foreach (DataRecord r in Records)
                {
                    if ((Convert.ToString(r.Cells["item_type"].Value) == "1" || minRuleGroupExists) && r.Cells["line_description"].Value.ToString().Contains("Minimum") && Convert.ToString(r.Cells["rule_type"].Value) == "1") //Minimum line 
                    {//Minimum line
                        r.Cells["extended"].Value = 0;
                        r.Cells["units"].Value = r.Cells["old_units"].Value;
                        continue;
                    }
                    if (r.Cells["units"].Value.ToString() != r.Cells["old_units"].Value.ToString())//Value has changed.
                    {
                        r.Cells["color_status"].Value = 1;
                        updateInvoiceLine(r);//Update all lines except for the minimum line. 
                    }
                    else
                    {
                        updateInvoiceLine(r);
                        r.Cells["color_status"].Value = 0;
                    }
                }
            }
            return false;
        }

        private decimal calcCumulative(IEnumerable<DataRecord> Records, bool minDropped = false, bool calcOnlyAmount = false)
        {
            string units = null;
            string rate_id = null;
            bool cumulativeUnitsChanged = false;
            int cumulative_rule = 0;
            DataRow tierOneRow = null;
            cBaseBusObject GetTier;
            decimal totalCumulativeAmount = 0;
            try
            {
                foreach (DataRecord r in Records)
                {
                    if (r.Cells["calc_method"].Value.ToString() == "3" && Convert.ToString(r.Cells["tier_number"].Value) == "1")//Cumulative tier.
                    {
                        if (r.Cells["units"].Value.ToString() != r.Cells["old_units"].Value.ToString())
                        {
                            cumulativeUnitsChanged = true;
                            cumulative_rule = Convert.ToInt32(r.Cells["rule_id"].Value);//Cumulative rule.
                            units = r.Cells["units"].Value.ToString(); //Updated units. 
                            rate_id = Convert.ToString(r.Cells["rate_id"].Value);
                            break;
                        }
                    }
                }

                if (cumulativeUnitsChanged)
                {//User updated the units; Delete all the cumulative rows except for the first tier;
                    foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["invedit"].Rows)
                    {
                        if (dr.RowState != DataRowState.Deleted && !calcOnlyAmount)
                        {
                            if (Convert.ToInt32(dr["rule_id"]) == cumulative_rule && Convert.ToInt32(dr["tier_number"]) == 1)
                            {
                                tierOneRow = dr;
                            }
                            if (Convert.ToInt32(dr["rule_id"]) == cumulative_rule && Convert.ToInt32(dr["tier_number"]) != 1)
                                dr.Delete();
                        }
                    }
                    //Call getTier sp to get the new tier values as per the new units
                    GetTier = new cBaseBusObject("GetTier");
                    GetTier.Parms.AddParm("@rate_id", Convert.ToInt32(rate_id));
                    GetTier.Parms.AddParm("@rule_id", Convert.ToInt32(cumulative_rule));
                    //GetTier.Parms.AddParm("@units", Convert.ToDecimal(r.Cells["units"].Value));
                    GetTier.Parms.AddParm("@units", Convert.ToDecimal(units));
                    GetTier.LoadTable("cumulative");
                    if (GetTier.ObjectData.Tables["cumulative"] == null || GetTier.ObjectData.Tables["cumulative"].Rows.Count < 1)
                    {
                        Messages.ShowInformation("Cumulative Tier amount not found.");
                    }

                    if (calcOnlyAmount)
                    {
                        foreach (DataRow dr in GetTier.ObjectData.Tables["cumulative"].Rows)
                        {
                            totalCumulativeAmount += Convert.ToDecimal(dr["units"]) * Convert.ToDecimal(dr["rate"]);
                        }
                        return totalCumulativeAmount;
                    }
                    //We fetched the tier values and we know that the user has updated the invoice line for the cumulative tier; 
                    //Iterate through the grid and get the first tier for our cumulative rule; 
                    foreach (DataRecord r in Records)
                    {
                        if (r.Cells["calc_method"].Value.ToString() == "3" &&
                            Convert.ToInt32(r.Cells["rule_id"].Value) == cumulative_rule &&
                            Convert.ToInt32(r.Cells["tier_number"].Value) == 1)
                        {//Tier 1 cumuative row
                            //Iterate through the tiers and update the grid; 
                            foreach (DataRow dr in GetTier.ObjectData.Tables["cumulative"].Rows)
                            {//Tier 1 will be the first!!
                                if (Convert.ToInt32(dr["tier_number"]) == 1)
                                {
                                    string extended;
                                    //Update extended
                                    r.Cells["extended"].Value = Convert.ToDecimal(dr["units"]) * Convert.ToDecimal(dr["rate"]);
                                    extended = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(r.Cells["extended"].Value));
                                    //Update units
                                    r.Cells["units"].Value = Convert.ToInt32(dr["units"]);
                                    //Resetting old units as well.
                                    r.Cells["old_units"].Value = Convert.ToInt32(dr["units"]);
                                    //Update line description
                                    if (Convert.ToDecimal(dr["units"]) == Convert.ToInt32(dr["units"]))
                                        units = String.Format("{0:###,##0;($###,##0)}", Convert.ToInt32(dr["units"]));
                                    else
                                        units = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(dr["units"]));
                                    //units = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(r.Cells["units"].Value));
                                    if (r.Cells["rule_line_desc"].Value.ToString() != " ")
                                        r.Cells["line_description"].Value = units + " " + r.Cells["rule_line_desc"].Value.ToString() +
                                                " * $ " + dr["rate"].ToString() + " = $ " + extended;
                                    else
                                        r.Cells["line_description"].Value = units + " " + r.Cells["bill_by"].Value.ToString() +
                                                " * $ " + dr["rate"].ToString() + " = $ " + extended;
                                    //Update rate
                                    r.Cells["rate"].Value = Convert.ToDecimal(dr["rate"]);
                                    r.Cells["color_status"].Value = 1;
                                    if (minDropped)
                                        r.Cells["extended"].Value = 0;

                                }
                                else if (Convert.ToInt32(dr["tier_number"]) > 1)
                                {//Add a row to the grid and update it's values from cumulative sp.
                                    DataTable invoiceEditDataTable = this.CurrentBusObj.ObjectData.Tables["invedit"];
                                    //New row added. 
                                    DataRow newRow = invoiceEditDataTable.NewRow();
                                    foreach (DataColumn column in invoiceEditDataTable.Columns)
                                    {
                                        newRow[column.ColumnName] = tierOneRow[column.ColumnName];
                                    }//New row has the updated data.
                                    //Add the updated information from sp.
                                    string extended;
                                    //Update extended
                                    newRow["extended"] = Convert.ToDecimal(dr["units"]) * Convert.ToDecimal(dr["rate"]);
                                    extended = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(newRow["extended"]));
                                    //Update units
                                    newRow["units"] = Convert.ToInt32(dr["units"]);

                                    //Update line description
                                    if (Convert.ToDecimal(dr["units"]) == Convert.ToInt32(dr["units"]))
                                        units = String.Format("{0:###,##0;($###,##0)}", Convert.ToInt32(dr["units"]));
                                    else
                                        units = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(dr["units"]));
                                    //units = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(r.Cells["units"].Value));
                                    if (newRow["rule_line_desc"].ToString() != " ")
                                        newRow["line_description"] = units + " " + newRow["rule_line_desc"].ToString() +
                                                " * $ " + dr["rate"].ToString() + " = $ " + extended;
                                    else
                                        newRow["line_description"] = units + " " + newRow["bill_by"].ToString() +
                                                " * $ " + dr["rate"].ToString() + " = $ " + extended;
                                    //Update rate
                                    newRow["rate"] = Convert.ToDecimal(dr["rate"]);
                                    //Update tier_number
                                    newRow["tier_number"] = Convert.ToInt32(dr["tier_number"]);
                                    //Add the data row to the invoice edit table.
                                    newRow["color_status"] = 1;
                                    if (minDropped)
                                        newRow["extended"] = 0;
                                    invoiceEditDataTable.Rows.Add(newRow);
                                }
                            }
                            break;
                        }
                    }

                }
                else if (!cumulativeUnitsChanged && calcOnlyAmount)
                {
                    foreach (DataRecord r in Records)
                    {
                        if (r.Cells["calc_method"].Value.ToString() == "3")
                        {
                            totalCumulativeAmount += Convert.ToDecimal(r.Cells["extended"].Value);
                        }
                    }
                    return totalCumulativeAmount;
                }
            }
            catch
            {
                Messages.ShowInformation("Clear the rows before they can be calculated again");
                return 0;
            }
            return 0;
        }

        private decimal updateInvoiceLine(DataRecord r, bool calcOnlyAmount = false)
        {

            string extended;
            string units;
            //int max_tier=0;
            string rate;

            if (r.Cells["calc_method"].Value.ToString() == "0") //flat
            {
                if (calcOnlyAmount)
                    return Convert.ToDecimal(r.Cells["units"].Value) * Convert.ToDecimal(r.Cells["rate"].Value);

                r.Cells["extended"].Value = Convert.ToDecimal(r.Cells["units"].Value) * Convert.ToDecimal(r.Cells["rate"].Value);
                extended = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(r.Cells["extended"].Value));
                if (Convert.ToDecimal(r.Cells["units"].Value) == Convert.ToInt32(r.Cells["units"].Value))
                    units = String.Format("{0:###,##0;($###,##0)}", Convert.ToInt32(r.Cells["units"].Value));
                else
                    units = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(r.Cells["units"].Value));
                //units = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(r.Cells["units"].Value));
                if (r.Cells["rule_line_desc"].Value.ToString() != " ")
                    r.Cells["line_description"].Value = units + " " + r.Cells["rule_line_desc"].Value.ToString() +
                            " * $ " + r.Cells["rate"].Value.ToString() + " = $ " + extended;
                else
                    r.Cells["line_description"].Value = units + " " + r.Cells["bill_by"].Value.ToString() +
                            " * $ " + r.Cells["rate"].Value.ToString() + " = $ " + extended;
            }
            else if ((r.Cells["calc_method"].Value.ToString() == "1") && (r.Cells["bill_by"].Value.ToString() != "0")) //tiered
            {
                cBaseBusObject GetTier = new cBaseBusObject("GetTier");
                GetTier.Parms.AddParm("@rate_id", Convert.ToInt32(r.Cells["rate_id"].Value));
                GetTier.Parms.AddParm("@rule_id", Convert.ToInt32(r.Cells["rule_id"].Value));
                GetTier.Parms.AddParm("@units", Convert.ToDecimal(r.Cells["units"].Value));
                GetTier.LoadTable("tieramount");
                if (GetTier.ObjectData.Tables["tieramount"] == null || GetTier.ObjectData.Tables["tieramount"].Rows.Count < 1)
                {
                    Messages.ShowInformation("Tier amount not found.");
                }
                else
                {
                    //r.Cells["rate"].Value = 
                    if (calcOnlyAmount)
                        return Convert.ToDecimal(r.Cells["units"].Value) * Convert.ToDecimal(GetTier.ObjectData.Tables["tieramount"].Rows[0]["amount"]);

                    r.Cells["extended"].Value = Convert.ToDecimal(r.Cells["units"].Value) *
                                                Convert.ToDecimal(GetTier.ObjectData.Tables["tieramount"].Rows[0]["amount"]);
                }
                extended = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(r.Cells["extended"].Value));
                if (Convert.ToDecimal(r.Cells["units"].Value) == Convert.ToInt32(r.Cells["units"].Value))
                    units = String.Format("{0:###,##0;($###,##0)}", Convert.ToInt32(r.Cells["units"].Value));
                else
                    units = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(r.Cells["units"].Value));
                rate = String.Format("{0:###,##0.0000;($###,##0.0000)}", Convert.ToDecimal(GetTier.ObjectData.Tables["tieramount"].Rows[0]["amount"]));
                if (r.Cells["rule_line_desc"].Value.ToString() != " ")
                    r.Cells["line_description"].Value = units + " " + r.Cells["rule_line_desc"].Value.ToString() +
                            " * $ " + rate + " = $ " + extended;
                else
                    r.Cells["line_description"].Value = units + " " + r.Cells["bill_by"].Value.ToString() +
                            " * $ " + rate + " = $ " + extended;
                r.Cells["rate"].Value = rate;
            }
            //cumulative calculation

            /*
              if ((CalcCumulative == "Y") && (r.Cells["units"].Value.ToString() != r.Cells["old_units"].Value.ToString()))
              {
                  Messages.ShowInformation("Enter total cumulative units on first cumulative line and system will calculate tier values.");
                  return 0;
              }

              r.Cells["color_status"].Value = 3;
              //r.Cells["edit_flag"].Value = "C";
              if (cumulative_rule == 0)
              {
                  cumulative_units = Convert.ToDecimal(r.Cells["units"].Value);
                  r.Cells["edit_flag"].Value = "C";
                  CalcCumulative = "Y";
              }
              //else
              //    Messages.ShowInformation("Enter total cumulative units on first cumulative line and system will calculate tier values.");
              if (cumulative_rule == Convert.ToInt32(r.Cells["rule_id"].Value))
                      r.Cells["units"].Value = cumulative_units;
              cumulative_tier = Convert.ToInt32(r.Cells["tier_number"].Value);
              cumulative_rule = Convert.ToInt32(r.Cells["rule_id"].Value);
              max_tier = Convert.ToInt32(r.Cells["max_tier_number"].Value);
              //cumulative_units = Convert.ToDecimal(r.Cells["units"].Value);
              cBaseBusObject GetTier = new cBaseBusObject("GetTier");
              GetTier.Parms.AddParm("@rate_id", Convert.ToInt32(r.Cells["rate_id"].Value));
              GetTier.Parms.AddParm("@rule_id", Convert.ToInt32(r.Cells["rule_id"].Value));
              //GetTier.Parms.AddParm("@units", Convert.ToDecimal(r.Cells["units"].Value));
              GetTier.Parms.AddParm("@units", cumulative_units);
              GetTier.LoadTable("cumulative");
              if (GetTier.ObjectData.Tables["cumulative"] == null || GetTier.ObjectData.Tables["cumulative"].Rows.Count < 1)
              {
                  Messages.ShowInformation("Cumulative Tier amount not found.");
              }
              else
              {
                  foreach (DataRow dr in GetTier.ObjectData.Tables["cumulative"].Rows)
                  {
                      if (Convert.ToInt32(dr["tier_number"]) > max_tier)  //new cumulative tier
                      {
                          NewCumulative = "Y";                                   
                      }
                      else
                          if (cumulative_tier == Convert.ToInt32(dr["tier_number"]))
                          {
                              r.Cells["color_status"].Value = 1;
                              r.Cells["extended"].Value = Convert.ToDecimal(dr["units"]) * Convert.ToDecimal(dr["rate"]);
                              extended = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(r.Cells["extended"].Value));
                              if (Convert.ToDecimal(dr["units"]) == Convert.ToInt32(dr["units"]))
                                  units = String.Format("{0:###,##0;($###,##0)}", Convert.ToInt32(dr["units"]));
                              else
                                  units = String.Format("{0:###,##0.00;($###,##0.00)}", Convert.ToDecimal(dr["units"]));
                              rate = String.Format("{0:###,##0.0000;($###,##0.0000)}", Convert.ToDecimal(dr["rate"]));
                              if (r.Cells["rule_line_desc"].Value.ToString() != " ")
                                  r.Cells["line_description"].Value = units + " " + r.Cells["rule_line_desc"].Value.ToString() +
                                          " * $ " + rate + " = $ " + extended;
                              else
                                  r.Cells["line_description"].Value = units + " " + r.Cells["bill_by"].Value.ToString() +
                                          " * $ " + rate + " = $ " + extended;
                              r.Cells["rate"].Value = rate;
                              r.Cells["units"].Value = units;
                              CalcCumulative = "Y";
                          }                                
                  }
                  if (Convert.ToInt32(r.Cells["color_status"].Value) == 3)
                  {
                      r.Cells["extended"].Value = 0;
                      r.Cells["rate"].Value = 0;
                      r.Cells["units"].Value = 0; 
                      r.Cells["line_description"].Value = "****************No longer using this tier***************";
                  }
                  //r.Cells["extended"].Value = cumulative_units *
                                              //Convert.ToDecimal(GetTier.ObjectData.Tables["tieramount"].Rows[0]["amount"]);
              }    
             */
            /*
            if (NewCumulative == "Y")
            {
                CalcNewCumulativeRow();
                NewCumulative = "N";
            }
             * */
            return 0;


        }
        private void btnCalc_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            btnCalc.Focus();

            var uniqueDates = gInvoiceEdit.xGrid.Records
                .Cast<DataRecord>()
                .Select(r => r.Cells["service_period_end"].Value)
                .Distinct();

            foreach (var date in uniqueDates)
            {
                var filteredRecords = gInvoiceEdit.xGrid.Records
                    .Cast<DataRecord>()
                    .Where(r => r.Cells["service_period_end"].Value.Equals(date))
                    .ToList();

                // Now you can use filteredRecords as needed
                bool minDropped = CalcMinimum(filteredRecords); // If calcMinimum accepts IEnumerable<DataRecord>
                calcCumulative(filteredRecords, minDropped);
            }
        }

    }
}


