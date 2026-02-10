

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows.Input;
using RazerBase.Lookups;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;

#endregion

namespace Customer
{
    /// <summary>
    /// This class represents a 'CustomerGeneralTab' object.
    /// </summary>
    public partial class CustomerGeneralTab : ScreenBase, IPreBindable
    {

        private string PrevCustName = "";
        Boolean CreditRatingChanged = false;
        public Boolean OrigLoad ;
        public Boolean SaveCancelSelectionChanged = false;
        public Boolean NewSave = false;

        


        /// <summary>
        /// Create a new instance of a 'CustomerGeneralTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerGeneralTab()
            : base()
        {
            
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
            
        }
        
        private void Customer_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on contract ID field
            CustomerLookup f = new CustomerLookup();
            f.Init(new cBaseBusObject("CustomerLookup"));

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                txtParentId.Text = cGlobals.ReturnParms[0].ToString();
                txtParentName.Text = cGlobals.ReturnParms[1].ToString();
                //DWR Added 1/23/13 Verifies that own ID is not picked as the parent.
                if (txtParentId.Text == (findRootScreenBase(this) as CustomerMainScreen).CustomerNum)
                {
                    Messages.ShowInformation("Invalid customer number.  Account cannot be its own parent.  Use 0 if no parent id is needed.");
                    txtParentId.Text = "0";
                    txtParentName.Text = "";
                }
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        //DWR - Added 1/23/13 - New Event Verifies that a valid ID is enetered into the field
        private void txtParentId_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((txtParentId.Text == "0") || (txtParentId.Text == " "))
            {
               if (txtParentName.Text != "")
                   txtParentName.Text = "";
               return;
            }
                    
            cBaseBusObject CustomerVerification = new cBaseBusObject("CustomerVerification");

            CustomerVerification.Parms.ClearParms();
            CustomerVerification.Parms.AddParm("@receivable_account", txtParentId.Text);
            CustomerVerification.LoadTable("cust_name");
            if (CustomerVerification.ObjectData.Tables["cust_name"] == null || CustomerVerification.ObjectData.Tables["cust_name"].Rows.Count < 1)
            {
                Messages.ShowInformation("Invalid customer number.  Please select a valid customer ID for the parent id.  Use 0 if no parent id is needed.");
                txtParentId.Text = "0";
                txtParentName.Text = "";
            }
            else if (txtParentId.Text == (findRootScreenBase(this) as CustomerMainScreen).CustomerNum )
            {
                Messages.ShowInformation("Invalid customer number.  Account cannot be its own parent.  Use 0 if no parent id is needed.");
                txtParentId.Text = "0";
                txtParentName.Text = "";
            }
            else if(txtParentName.Text != CustomerVerification.ObjectData.Tables["cust_name"].Rows[0]["account_name"].ToString())
            {
                txtParentName.Text = CustomerVerification.ObjectData.Tables["cust_name"].Rows[0]["account_name"].ToString();
            }

        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
           
            // Set the ScreenBaseType
            this.DoNotSetDataContext = false;
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "general";
            //this.CurrentBusObj.PropertyChanged += false;


            txtNSInternalID.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(txtNSInternalID_TextChanged));


        }

        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    // set the dataTable for terms
                    //State Lookup
                    this.cmbState.SetBindingExpression("state", "description", this.CurrentBusObj.ObjectData.Tables["customer_state_lookup"]);
                    //Country Lookup
                    this.cmbCountry.SetBindingExpression("country_id", "country", this.CurrentBusObj.ObjectData.Tables["customer_country_lookup"]);
                    //Language Lookup
                    this.cmbLanguage.SetBindingExpression("language_id", "description", this.CurrentBusObj.ObjectData.Tables["language_lookup"]);
                    //Bill Method Lookup
                    this.cmbBillMethod.SetBindingExpression("bill_method_id", "bill_method_name", this.CurrentBusObj.ObjectData.Tables["bill_method_lookup"]);
                    //Credit Coordinator Lookup
                    this.cmbCredCoord.SetBindingExpression("credit_coordinator_id", "credit_coordinator", this.CurrentBusObj.ObjectData.Tables["cred_coord_lookup"]);
                    //Payment Type Lookup
                    this.cmbPaymentType.SetBindingExpression("pmt_type_id", "pmt_type_desc", this.CurrentBusObj.ObjectData.Tables["payment_type_lookup"]);
                    //Finance Charge Lookup
                    this.cmbFinanceCharge.SetBindingExpression("finance_charge_id", "description", this.CurrentBusObj.ObjectData.Tables["finance_charge_lookup"]);
                    this.cmbProvince.SetBindingExpression("province_id", "province", this.CurrentBusObj.ObjectData.Tables["customer_province_lookup"]);
                    //credit rating dddw
                    this.cmbCreditRating.SetBindingExpression("credit_rating", "description", this.CurrentBusObj.ObjectData.Tables["creditdddw"]);
                    //geography
                    this.cmbGeography.SetBindingExpression("geography", "description", this.CurrentBusObj.ObjectData.Tables["dddwgeo"]);
                    //KSH 6.29.12 add payment terms for natn'l ads
                    this.cmbPaymentTerms.SetBindingExpression("terms_code", "description", this.CurrentBusObj.ObjectData.Tables["terms"]);
                    this.cmbWHT.SetBindingExpression("wht_rate_id", "wht_rate", this.CurrentBusObj.ObjectData.Tables["dddwwht"]);
                    
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }

        private void chkComm100Flag_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (txtCommPct.Text != "0.00" && txtCommPct.Text != null )
            //{
            //    System.Windows.MessageBoxResult res = Messages.ShowOkCancel("Checking the 100% Commision Flag Will Override the Commission Percent", System.Windows.MessageBoxImage.Warning);
            //    if (res == System.Windows.MessageBoxResult.OK)
            //        txtCommPct.Text = "0.00";
            //    else
            //        chkComm100Flag.IsChecked = 0;
            //}
        }

        private void chkZeroAgencyCommFlag_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (txtCommPct.Text != "0.00" && txtCommPct.Text != null)
            //{
            //    System.Windows.MessageBoxResult res = Messages.ShowOkCancel("Checking the Zero Agency Commission Flag Will Override the Commission Percent", System.Windows.MessageBoxImage.Warning);
            //    if (res == System.Windows.MessageBoxResult.OK)
            //        txtCommPct.Text = "0.00";
            //    else
            //        chkZeroAgencyCommFlag.IsChecked = 0;
            //}
        }

        private void chkLegalFlag_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (txtCommPct.Text != "0.00" && txtCommPct.Text != null)
            //{
            //    System.Windows.MessageBoxResult res = Messages.ShowOkCancel("Checking the Zero Agency Commission Flag Will Override the Commission Percent", System.Windows.MessageBoxImage.Warning);
            //    if (res == System.Windows.MessageBoxResult.OK)
            //        txtCommPct.Text = "0.00";
            //    else
            //        chkZeroAgencyCommFlag.IsChecked = 0;
            //}
        }

        private void txtCommPct_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key.ToString() != "0" && e.Key.ToString().ToLower() != "numpad0")
                
            ////if (txtCommPct.Text != "0.00" && txtCommPct.Text != null)
            //{
            //    //System.Windows.MessageBoxResult res = Messages.ShowOkCancel("The 100% Commision Flag is Checked and Will Override the Commission Percent", System.Windows.MessageBoxImage.Warning);
            //    if (chkComm100Flag.IsChecked == 1)
            //    {
            //        Messages.ShowWarning("The 100% Commision Flag is Checked and Will Override the Commission Percent. Uncheck the Flag and Try Again");
            //        txtCommPct.Text = "0.00";
            //        return;
            //    }
            //    if (chkZeroAgencyCommFlag.IsChecked == 1)
            //    {
            //        Messages.ShowWarning("The Zero Agency Commission Flag is Checked and Will Override the Commission Percent. Uncheck the Flag and Try Again");
            //        txtCommPct.Text = "0.00";
            //        return;
            //    }
            //}
        }

        public void SetDefaultValues()
        {
            //Method to set default values for inserts
            cmbState.SelectedText = ""; //No State
            cmbCountry.SelectedText = "USA";
            //cmbCountry.SelectedValue = 263;
            //cmbGeography.SelectedText = "UNITED STATES";
            cmbGeography.SelectedText = "1001 - NORTH AMERICA";
            cmbLanguage.SelectedText = "ENGLISH";
            cmbPaymentType.SelectedText = "Check";
            cmbFinanceCharge.SelectedText  = "None";
            cmbProvince.SelectedText = "None";
            cmbProvince.SelectedValue = 0;
            cmbCreditRating.SelectedValue = 0;
            cmbWHT.SelectedValue = 0;

            //txtWhtPct.Text = 0;

        }

        private void ScreenBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void cmbCreditRating_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            CreditRatingChanged = true;
        }


        private void cmbCountry_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            //Need to populate the province table based on the country selected

            //update the wht_rate based on the country selected
            //string debug = cmbCountry.SelectedValue.ToString();
         



            int validProvince = 0;
             int countryID = Convert.ToInt32(cmbCountry.SelectedValue);
             CurrentBusObj.Parms.UpdateParmValue("@country_id_lookup", countryID);
             //if   (SaveCancelSelectionChanged == true) 
             //{
             //    CurrentBusObj.Parms.UpdateParmValue("@country_id_lookup", countryID);
             //    CurrentBusObj.LoadTable("customer_province_lookup");
             //    cmbProvince.SelectedText = "None";
             //    cmbProvince.SelectedValue = 0;

             //    {
             //        if (this.CurrentBusObj.ObjectData.Tables["provincevalidate"].Rows.Count > 0)
             //            if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["provincevalidate"].Rows[0]["provincecount"]) > 0)
             //            {
             //                int provinceID = Convert.ToInt32(cmbProvince.SelectedValue);
             //                foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["customer_province_lookup"].Rows)

             //                    if (Convert.ToInt32(r["province_id"]) == provinceID)
             //                    {
             //                        validProvince = 1;
             //                        return;

             //                    }
             //                if (validProvince == 0)
             //                {
             //                    //Messages.ShowInformation("Please select a valid province for the country from the dropdown");
             //                    provinceID = 0;
             //                    this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["province_id"] = 0;
             //                }


             //            }


             //            else
             //            {
             //                cmbProvince.SelectedValue = 0;
             //                cmbProvince.SelectedText = "None";
             //                this.setWHTRate();
             //            }
             //    }

             //}
             //else
             //{
                 CurrentBusObj.Parms.UpdateParmValue("@country_id_lookup", countryID);
                 CurrentBusObj.LoadTable("customer_province_lookup");
                 CurrentBusObj.LoadTable("provincevalidate");
                 
                 {
                     if (this.CurrentBusObj.ObjectData.Tables["provincevalidate"].Rows.Count > 0)
                         if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["provincevalidate"].Rows[0]["provincecount"]) > 0)
                         {
                             int provinceID = Convert.ToInt32(cmbProvince.SelectedValue);
                             if (cmbProvince.SelectedValue == null)
                             {
                                 if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["province_id"] == null | this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["province_id"].ToString() == "")
                                 {
                                     provinceID = 0;
                                     this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["province_id"] = 0;
                                 }
                                 else
                                 {

                                     provinceID = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["province_id"]);
                                 }
                             }

                             foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["customer_province_lookup"].Rows)

                                 if (Convert.ToInt32(r["province_id"]) == provinceID)
                                 {
                                     validProvince = 1;
                                     return;

                                 }


                             if (validProvince == 0)
                             {
                                 if (NewSave)
                                 {

                                     return;
                                 }
                                 //Messages.ShowInformation("Please select a valid province for the country from the dropdown");
                             }
                             else
                             {
                                 cmbProvince.SelectedText = "None";
                                 cmbProvince.SelectedValue = 0;
                                 //this.setWHTRate();
                             }

                            }


                         else
                         {
                             cmbProvince.SelectedValue = 0;
                             cmbProvince.SelectedText = "None";
                             //this.setWHTRate();
                         }
                 }
                 this.setWHTRate();
             //}
            
            //    //Loop through the country datatable to find the currently selected row
            //    foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["customer_country_lookup"].Rows)
            //        //once the current row is found, set the wht_rad
            //        if (Convert.ToInt32(r["country_id"]) == countryID)
            //        {
            //            txtWhtPct.Text = Convert.ToString(r["wht_rate"]);
                        
            //            return;
            //        }


             
        }

        
        private void txtBillMSO_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on Entity ID field
            EntityLookup f = new EntityLookup();
            cGlobals.ReturnParms.Clear();
            

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {

                txtBillMSO.Text = cGlobals.ReturnParms[0].ToString();

                cGlobals.ReturnParms.Clear();
            }
        }

        private void txtBillMSO_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBillMSO.Text == "")
            {
                //txtContractEntity.Text = "";
                return;
            }

            cBaseBusObject ContractVerification = new cBaseBusObject("ContractVerification");

            ContractVerification.Parms.ClearParms();
            ContractVerification.Parms.AddParm("@mso_id", txtBillMSO.Text);
            ContractVerification.LoadTable("entity_name");
            if (ContractVerification.ObjectData.Tables["entity_name"] == null || ContractVerification.ObjectData.Tables["entity_name"].Rows.Count < 1)
            {
                Messages.ShowInformation("Invalid Entity.  Please select a valid entity ID.");
                txtBillMSO.Text = "0";
                //txtContractEntity.Text = "";
            }            
        }

       

        private void btnPrinted_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string receivableAccount = "";
            receivableAccount = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["receivable_account"].ToString();
               
                    
                    //printed addr screen
                    CustomerPrintedAddr CustomerPrintedAddrScreen = new CustomerPrintedAddr(receivableAccount, this.CurrentBusObj);
                    //////////////////////////////////////////////////////////////
                    //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
                    System.Windows.Window CustomerPrintedAddrWindow = new System.Windows.Window();
                    //set new window properties///////////////////////////


                   CustomerPrintedAddrWindow.Title = "Display Printed Address";
                    CustomerPrintedAddrWindow.MaxHeight = 1280;
                    CustomerPrintedAddrWindow.MaxWidth = 1280;
                    /////////////////////////////////////////////////////
                    //set screen as content of new window
                    CustomerPrintedAddrWindow.Content = CustomerPrintedAddrScreen;
                    //open new window with embedded user control
                    CustomerPrintedAddrWindow.ShowDialog();
                }

        private void chkNoWhtFlag_UnChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            chkWhtMultiFlag.IsChecked = 0;
            //CurrentBusObj.ObjectData.Tables["general"].Rows[0]["no_wht_flag"] = 0;
            this.setWHTRate();
        }

        private void chkNoWhtFlag_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            cmbWHT.SelectedValue = 0;
            chkWhtMultiFlag.IsChecked = 0;
            //CurrentBusObj.ObjectData.Tables["general"].Rows[0]["wht_multi_flag"] = 0;
            //CustomerVerification.ObjectData.Tables["whtrate"].Rows[0]["wht_multi_flag"] = 0;
       }

        private void setWHTRate()
        {
            int countryID = 0;
            int provinceID = 0;
            //If no_wht_flag is checked
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                if (CurrentBusObj.ObjectData.Tables["general"].Rows[0]["no_wht_flag"].ToString() == "1")
                    return;

                if (cmbCountry.SelectedValue == "0")
                    return;

                if (cmbProvince.SelectedValue == "0")
                {
                    provinceID = 0;
                }

                provinceID = Convert.ToInt32(cmbProvince.SelectedValue);
                countryID = Convert.ToInt32(cmbCountry.SelectedValue);

                if (cmbProvince.SelectedValue == null)
                {
                    if (CurrentBusObj.ObjectData.Tables["general"].Rows[0]["province_id"] == null || CurrentBusObj.ObjectData.Tables["general"].Rows[0]["province_id"].ToString() == "")
                      provinceID = 0;
                    else
                      provinceID = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["province_id"]);
                }
                else
                {
                    provinceID = Convert.ToInt32(cmbProvince.SelectedValue);
                }

                if (cmbCountry.SelectedValue == null)
                {
                    if (CurrentBusObj.ObjectData.Tables["general"].Rows[0]["country_id"] == null || CurrentBusObj.ObjectData.Tables["general"].Rows[0]["country_id"].ToString() == "")
                    {
                        countryID = 0;
                    }
                    else
                    {
                        countryID = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["country_id"]);
                    }
                }
                else
                {
                    countryID = Convert.ToInt32(cmbCountry.SelectedValue);
                }



                cBaseBusObject CustomerVerification = new cBaseBusObject("CustomerVerification");

                CustomerVerification.Parms.ClearParms();
                CustomerVerification.Parms.AddParm("@country_id_lookup", countryID);
                CustomerVerification.Parms.AddParm("@province_id_lookup", provinceID);
                CustomerVerification.LoadTable("whtrate");
                if (CustomerVerification.ObjectData.Tables["whtrate"] == null || CustomerVerification.ObjectData.Tables["whtrate"].Rows.Count < 1)
                {
                    //no wht rate for this country/province combo
                    cmbWHT.SelectedValue = 0;
                    if (IsScreenDirty)
                        this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["wht_rate_id"] = 0;
                    chkWhtMultiFlag.IsChecked = 0;
                }
                else
                {
                    if (Convert.ToInt32(CustomerVerification.ObjectData.Tables["whtrate"].Rows[0]["wht_multi_flag"]) == 1)
                        //&& CurrentBusObj.ObjectData.Tables["general"].Rows[0]["no_wht_flag"].ToString() != "1")
                        chkWhtMultiFlag.IsChecked = 1;
                    else
                        chkWhtMultiFlag.IsChecked = 0;
                        cmbWHT.SelectedValue = CustomerVerification.ObjectData.Tables["whtrate"].Rows[0]["wht_rate_id"];
                    if (IsScreenDirty)
                        this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["wht_rate_id"] = CustomerVerification.ObjectData.Tables["whtrate"].Rows[0]["wht_rate_id"];
                }

            }

        }

        private void cmbProvince_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
           
            {
                if (this.CurrentBusObj.ObjectData.Tables["customer_province_lookup"].Rows.Count > 0)
                {

                    setWHTRate();
                }

            }
            
        }

        private void chkWhtMultiFlag_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                //this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["wht_multi_flag"] = 1;
            
        }

        private void chkWhtMultiFlag_UnChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                //this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["wht_multi_flag"] = 0;
        }

        private void txtNSInternalID_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textBox = e.OriginalSource as System.Windows.Controls.TextBox;

            int input;

            if (Int32.TryParse(textBox.Text, out input) && input == 0)
            {
                textBox.Text = "";
                txtNSInternalID.Text = "";
            }

        }

        private void txtNSInternalID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNSInternalID.Text != null && !txtNSInternalID.Text.All(char.IsDigit))
            {
                Messages.ShowInformation("NetSuite Internal ID allows only digits.");
                txtNSInternalID.Text = "";
            }

        }




    }

}
