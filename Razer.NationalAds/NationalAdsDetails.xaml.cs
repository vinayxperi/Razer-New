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
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter.Events;
using System.Data;
using RazerInterface;
using RazerWS;

namespace Razer.NationalAds
{
    /// <summary>
    /// Interaction logic for NationalAdsDetails.xaml
    /// </summary>
    public partial class NationalAdsDetails : ScreenBase, IPreBindable
    {
        private static readonly string fieldLayoutResource = "NationalAdsDetails";
        private static readonly string adjustmentDetailTable = "NationalAdsDetails";
        private static readonly string adjustmentParameterTable = "NationalAdsAdjustmentParms";
        private static readonly string mainTableName = "totals";
        private static readonly string gridTableName = "detail";
        private static readonly string amountField = "ad_amt";
        private static readonly string totalField = "total_amt";
        private static readonly string commissionField = "comm_amt";
        private static readonly string VATField = "vat_tax";
        //private static readonly string previousCommissionField = "prev_comm";
        private static readonly string netTotalField = "net_amt";
        private static readonly string totalAdjustmentField = "total_adjusted";
        private static readonly string invoiceNumberField = "invoice_number";
        private static readonly string adjustmentReasonField = "adjustment_reason";    
        private static readonly string userNameField = "user_name";
        private static readonly string zeroComminsionField = "comm_set_to_zero";
        private static readonly string calcCommissionField = "calc_commission";
        private static readonly string reverseAdjustmentField = "reverse_adjustment";
        private static readonly string zeroVATField = "vat_set_to_zero";
        private static readonly string addVATField = "add_vat";

        //public Razer.NationalAds.NationalAdsFolder nationalAdsFolder = new NationalAdsFolder();

        public bool zeroCommission = false;
        public bool reverseAdjustment = false;
        public bool recalcCommission = false;
        public bool zeroVAT = false;
        public bool addVAT = false;

        public string invoiceNumber = string.Empty;
        public string adjReason = string.Empty;
        public int adjCount = 0;
        public string orderType = string.Empty;

        decimal originalTotal = 0, originalNet = 0, totalAmount = 0, netAmount = 0, adjustment = 0, finalAdjustment = 0, commission = 0,  vat = 0;     

        public NationalAdsDetails()
            : base()
        {    
            InitializeComponent();
            Init();
        }

        public void Init()
        {            
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            layouts.AllowAddNew = false;

            this.DoNotSetDataContext = true;
            this.MainTableName = mainTableName;
            idgAdvertisementsDetail.xGrid.FieldLayoutSettings = layouts;
            idgAdvertisementsDetail.FieldLayoutResourceString = fieldLayoutResource;
            //idgAdvertisementsDetail.MainTableName = mainTableName;
            idgAdvertisementsDetail.xGrid.FieldSettings.AllowEdit = true;            
            idgAdvertisementsDetail.SetGridSelectionBehavior(false, false);            
            idgAdvertisementsDetail.MainTableName = gridTableName;

            idgAdvertisementsDetail.xGrid.CellUpdated += new EventHandler<CellUpdatedEventArgs>(xGrid_CellUpdated);

            idgAdvertisementsDetail.xGrid.UpdateMode = UpdateMode.OnCellChange;
           
            GridCollection.Add(idgAdvertisementsDetail);            
        }

        public void SaveAdjustment()
        {
            //this.txtTotalAmount.Focus();



            if (CurrentBusObj.ObjectData != null)
            {

                if (CurrentBusObj.HasObjectData)
                {
                    idgAdvertisementsDetail.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndCommitRecord);
                    //Get rows that have changed
                    DataTable rowsModified = CurrentBusObj.ObjectData.Tables[gridTableName].GetChanges(DataRowState.Modified);
                    zeroCommission = chkZeroCommission.IsChecked == 0 ? false : true;
                    recalcCommission = chkCommissionRecalc.IsChecked == 0 ? false : true;
                    reverseAdjustment = chkReverseAdjustment.IsChecked == 0 ? false : true;
                    zeroVAT = chkZeroVAT.IsChecked == 0 ? false : true;
                    addVAT = chkAddVAT.IsChecked == 0 ? false : true;
                    adjReason = txtAdjReason.ToString();

                    if (orderType.ToString() == "P")
                    {
                        //No Adjustments to P invoices allowed.                

                        Messages.ShowInformation("Adjustments to Prepaid Invoices are not allowed. No Changes were made to the invoice and no adjustment was created.");
                        return;

                    }

                    //Check to make sure they want to reverse entire adjustment
                    //If so, loop through and zero out amounts on detail grid.

                    if (reverseAdjustment == true)
                    {
                        var result = MessageBox.Show("Do you really want to adjust entire amount of the invoice?", "Reverse Adjustment", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                        {

                           foreach (DataRecord r in idgAdvertisementsDetail.xGrid.Records)
                            {
                                (r.Cells["ad_amt"].Value) = 0;                               
                            }        
                            
                                     
                            
                            foreach (DataRow row in CurrentBusObj.ObjectData.Tables[gridTableName].Rows)
                            {
                                row.BeginEdit();
                                row["ad_amt"] = 0;
                                row.EndEdit();
                            }

                            idgAdvertisementsDetail.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndCommitRecord);                           
                            rowsModified = CurrentBusObj.ObjectData.Tables[gridTableName].GetChanges(DataRowState.Modified);
                        }
                        else
                        {
                            //KSH - 8/22/12 there is a also a message box they can say no to for the Reverse Adjustment checkbox within the save logic, if no, uncheck the reverse checkbox
                            chkReverseAdjustment.IsChecked = 0;
                            return;
                        }

                    }


                    //If Zero VAT checked, make sure there is a VAT to zero out.
                    if (txtVATAmt.Text.ToString() == "$0.00" && zeroVAT == true)
                    {
                        //No VAT and Zero checked, not allowed.                        

                        Messages.ShowInformation("No VAT to zero out. No Changes were made to the invoice and no adjustment was created.");
                        return;

                    }

                    //If Add VAT checked, make sure there is a VAT to add.
                    if (txtTotalAmount.Text.ToString() == "$0.00" && addVAT == true)
                    {
                        //No VAT and Add checked, not allowed.                        

                        Messages.ShowInformation("No VAT to add. No Changes were made to the invoice and no adjustment was created.");
                        return;

                    }



                    //If Zero VAT checked, make sure there is a VAT to zero out.
                    if (txtVATAmt.Text.ToString() != "$0.00" && addVAT == true)
                    {
                        //VAT exists and Add Vat checked, not allowed.                        

                        Messages.ShowInformation("VAT amount already exists. No Changes were made to the invoice and no adjustment was created.");
                        return;

                    }





                    //If net amount is negative and Do not recalc commission is checked.

                    if (txtTotalAmount.Text.Substring(0,1) == "(" && recalcCommission == true)
                    {
                        //Need to recalc commission to determine if negative net amount is valid                        
                        
                        Messages.ShowInformation("Amount of invoice will be negative. No changes were made to the invoice and no adjustment was created.");
                        return;

                    }



                    if (zeroCommission == true && recalcCommission == true)
                    {
                        Messages.ShowInformation("Only 1 Commission related checkbox allowed. No changes were made to the invoice and no adjustment was created.");
                        return;

                    }

                    if (zeroCommission == true && txtCommission.Text.Substring(0) == "$0.00")
                    {
                        Messages.ShowInformation("No commission exits on this invoice. No changes were made to the invoice and no adjustment was created.");
                        return;

                    }



                    if (rowsModified == null && zeroCommission == false && zeroVAT == false && addVAT == false)
                    {
                        Messages.ShowInformation("No changes were made to the invoice and no adjustment was created.");
                        return;
                    }

                    if (rowsModified != null && (zeroCommission == true || zeroVAT == true || addVAT == true))
                    {
                        Messages.ShowInformation("No changes to detail can be made when Zero Out or Add Vat is checked. No changes were made to the invoice and no adjustment was created.");
                        return;
                    }

                    
                    //if outstanding unposted adjustment can not create new one yet
                    if (adjCount != 0)
                    {
                        Messages.ShowInformation("No changes can be made, Outstanding Adjustment not posted.");
                        return;
                    }



                    //CurrentBusObj.ObjectData.Tables[gridTableName].AcceptChanges();
                    DataTable detail = CurrentBusObj.ObjectData.Tables[gridTableName].Copy();
                    detail.TableName = adjustmentDetailTable;
                    DataTable parameters = BuildParameterTable();

                    DataSet adjustmentData = new DataSet();
                    adjustmentData.Tables.Add(detail);
                    adjustmentData.Tables.Add(parameters);

                    string adjustmentId = cGlobals.BillService.NationalAdsAdjustment(adjustmentData);
                    if (!string.IsNullOrEmpty(adjustmentId))
                    {

                        if (adjustmentId == "Updated")
                        {
                            Messages.ShowInformation(string.Format("Successfully saved changes to invoice."));
                        }
                        else
                        {

                            Messages.ShowInformation(string.Format("Successfully created adjustment {0}", adjustmentId));
                            adjCount = 1;
                        }

                        //Data will be reloaded in the Folder Save method to reflect changes made.

                        //Need to zero out adjustment text field. 

                        txtTotalAdjustment.Text = "$0.00";


                    }
                    else
                        Messages.ShowInformation("The adjustment failed to save.");
                }
            }
            //KSH - 8/22/12 Set all  4 checkboxes to unchecked after a successful or unsuccessful save on the detail tab
            chkCommissionRecalc.IsChecked = 0;
            chkZeroCommission.IsChecked = 0;
            chkReverseAdjustment.IsChecked = 0;
            chkZeroVAT.IsChecked = 0;
            chkAddVAT.IsChecked = 0;
        }

        private void xGrid_CellUpdated(object sender, CellUpdatedEventArgs e)
        {
            if (e.Cell.Field.Name == amountField)
            {   
                var dataContext = idgAdvertisementsDetail.xGrid.DataContext;
                DataTable dt;

                if (dataContext.GetType() == typeof(DataView))
                {
                    dt = ((DataView)dataContext).Table;
                }
                else if (dataContext.GetType() == typeof(DataTable))
                {
                    dt = (DataTable)dataContext;
                }
                else
                    return;

                foreach (DataRow row in dt.Rows)
                {
                    decimal rowAmount = 0;
                    if (decimal.TryParse(row[amountField].ToString(), out rowAmount))
                    {
                        totalAmount += rowAmount;
                    }
                }
                adjustment = totalAmount - originalTotal;
                netAmount = originalNet + adjustment;
                txtTotalAmount.Text = totalAmount.ToString("C2");
                txtNetAmount.Text = netAmount.ToString("C2");
                txtTotalAdjustment.Text = adjustment.ToString("C2");
                totalAmount = 0;
                
               
            }
            #region KSH - 8/22/12 Logic to make field red if negative num


            if (adjustment < 0)
            {
                //if less than zero turn red
                txtTotalAdjustment.TextColor = "Red";
            }
            else
            {
                //if greater than zero turn black
                txtTotalAdjustment.TextColor = "Black";
            }


            if (netAmount < 0)
            {
                //if less than zero turn red
                txtNetAmount.TextColor = "Red";
            }
            else
            {
                //if greater than zero turn black
                txtNetAmount.TextColor = "Black";
            }

            adjustment = 0;
            netAmount = 0;
            
            #endregion
        }

        private DataTable BuildParameterTable()
        {
            DataTable parameterTable = new DataTable(adjustmentParameterTable);
            parameterTable.Columns.Add(invoiceNumberField, typeof(string));
            parameterTable.Columns.Add(userNameField, typeof(string));
            parameterTable.Columns.Add(zeroComminsionField, typeof(bool));
            parameterTable.Columns.Add(calcCommissionField, typeof(bool));
            parameterTable.Columns.Add(reverseAdjustmentField, typeof(bool));
            parameterTable.Columns.Add(zeroVATField, typeof(bool));
            parameterTable.Columns.Add(addVATField, typeof(bool));
            parameterTable.Columns.Add(commissionField, typeof(decimal));
            parameterTable.Columns.Add(VATField, typeof(decimal));
            parameterTable.Columns.Add(netTotalField, typeof(decimal));
            parameterTable.Columns.Add(totalField, typeof(decimal));            
            parameterTable.Columns.Add(totalAdjustmentField, typeof(decimal));
            parameterTable.Columns.Add(adjustmentReasonField, typeof(string));

                      
            
            totalAmount = ParseToDecimal(UnformatTextField(txtTotalAmount.Text.Substring(0)));
            netAmount = ParseToDecimal(UnformatTextField(txtNetAmount.Text.Substring(0)));
            adjustment = ParseToDecimal(UnformatTextField(txtTotalAdjustment.Text.Substring(0)));
            commission = ParseToDecimal(UnformatTextField(txtCommission.Text.Substring(0)));
            vat = ParseToDecimal(UnformatTextField(txtVATAmt.Text.Substring(0)));
            adjReason = txtAdjReason.Text;

            if (adjReason == null || adjReason == "")
            //if (adjReason == null)
            {
                adjReason = "Ads Adjustment";
            }

            if (adjReason.Length > 80)
            {
                adjReason = adjReason.Substring(0, 80);
            }




            parameterTable.Rows.Add(invoiceNumber, cGlobals.UserName, zeroCommission, recalcCommission, reverseAdjustment, zeroVAT, addVAT,
                commission, vat, netAmount, totalAmount, adjustment, adjReason);

            totalAmount = 0;
            netAmount = 0;
            adjustment = 0;
            commission = 0;
            vat = 0;
            adjReason = " ";
            txtAdjReason.Text = adjReason;

            return parameterTable;
        }

        private decimal ParseToDecimal(string charValue)
        {
            decimal amount = 0;
            decimal.TryParse(charValue, out amount);

            return amount;
        }

        private string UnformatTextField(string FormattedTextField)
        {
            if (FormattedTextField == null || FormattedTextField == "") return "0";

            string sUnformattedTextField = FormattedTextField.Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");

            return sUnformattedTextField;
        }

        public void PreBind()
        {
            if (CurrentBusObj.HasObjectData)
            {
                if (CurrentBusObj.ObjectData.Tables[mainTableName].Rows.Count > 0)
                {
                    DataTable totals = CurrentBusObj.ObjectData.Tables[mainTableName];
                    DataTable detail = CurrentBusObj.ObjectData.Tables[gridTableName];

                    this.DataContext = totals;
                    idgAdvertisementsDetail.DataContext = detail;
                    idgAdvertisementsDetail.xGrid.DataSource = detail.DefaultView;

                    originalTotal = ParseToDecimal(totals.Rows[0][totalField].ToString());
                    originalNet = ParseToDecimal(totals.Rows[0][netTotalField].ToString());                  



                }
            }
        }

        #region KSH - 8/22/12 Logic that only allows one checkbox to be set at a time
        private void chkCommissionRecalc_Checked(object sender, RoutedEventArgs e)
        {
            chkZeroCommission.IsChecked = 0;
            chkReverseAdjustment.IsChecked = 0;
            chkZeroVAT.IsChecked = 0;
            chkAddVAT.IsChecked = 0;
        }

        private void chkZeroCommission_Checked(object sender, RoutedEventArgs e)
        {
            chkCommissionRecalc.IsChecked = 0;
            chkReverseAdjustment.IsChecked = 0;
            chkZeroVAT.IsChecked = 0;
            chkAddVAT.IsChecked = 0;
        }

        private void chkReverseAdjustment_Checked(object sender, RoutedEventArgs e)
        {
            chkCommissionRecalc.IsChecked = 0;
            chkZeroCommission.IsChecked = 0;
            chkZeroVAT.IsChecked = 0;
            chkAddVAT.IsChecked = 0;
        }

        private void chkZeroVAT_Checked(object sender, RoutedEventArgs e)
        {
            chkCommissionRecalc.IsChecked = 0;
            chkZeroCommission.IsChecked = 0;
            chkReverseAdjustment.IsChecked = 0;
            chkAddVAT.IsChecked = 0;
        }

        private void chkAddVAT_Checked(object sender, RoutedEventArgs e)
        {
            chkCommissionRecalc.IsChecked = 0;
            chkZeroCommission.IsChecked = 0;
            chkReverseAdjustment.IsChecked = 0;
            chkZeroVAT.IsChecked = 0;
        }
        #endregion
    }
}
