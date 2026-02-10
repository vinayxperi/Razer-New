

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;



namespace Contract
{


    /// <summary>
    /// This class represents a 'ContractGeneralTab' object.
    /// </summary>
    public partial class ContractGeneralTab : ScreenBase, IPreBindable 
    {


        /// <summary>
        /// Create a new instance of a 'ucTab1' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractGeneralTab()
            : base()
        {
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
            this.DoNotSetDataContext = false;
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this if your folder has item directly bound to it.
            // In many cases the Tabs have data binding, the Folders
            // do not.
            MainTableName = "general";

            txtCustomerName.DoubleClickDelegate = CustomerDoubleClickHandler;
            txtContractEntity.IsReadOnly = true;
            
        }


        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_archive_flag"].ToString() == "1")
                        this.cmbProductCode.SetBindingExpression("product_code", "product_description", this.CurrentBusObj.ObjectData.Tables["products_with_archive"]);
                    else
                        this.cmbProductCode.SetBindingExpression("product_code", "product_description", this.CurrentBusObj.ObjectData.Tables["products"]);
                    this.cmbRevenueType.SetBindingExpression("revenue_type_id", "revenue_description", this.CurrentBusObj.ObjectData.Tables["revenue_type"]);
                    this.cmbColaID.SetBindingExpression("cola_id", "description", this.CurrentBusObj.ObjectData.Tables["cola"]);
                    this.cmbCurrencyCode.SetBindingExpression("currency_code", "description", this.CurrentBusObj.ObjectData.Tables["currency"]);
                    this.cmbPaymentTerms.SetBindingExpression("terms_code", "description", this.CurrentBusObj.ObjectData.Tables["terms"]);
                    this.cmbSalesperson.SetBindingExpression("contact_id", "name", this.CurrentBusObj.ObjectData.Tables["salesperson"]);
                    this.cmbBillingOwner.SetBindingExpression("billing_owner_id", "billing_owner_name", this.CurrentBusObj.ObjectData.Tables["billing_owner"]);
                    //this.cmbAttribute.SetBindingExpression("attribute_id", "attribute", this.CurrentBusObj.ObjectData.Tables["dddwcontractattributes"]);
                                        
                    //OLD ucLookupTextbox values, get rid of. Left for reference only///////////////////////////////////////////////////////////////////////////////////////////////////////
                    //this.txtProductCode.BindingObject= EstablishListObjectBinding(this.CurrentBusObj.GetTable("products") as DataTable, true, "product_description", 
                    //    "product_code", "Select Product");
                    //this.txtRevenueType.BindingObject=EstablishListObjectBinding(this.CurrentBusObj.GetTable("revenue_type") as DataTable, false, "revenue_description",
                    //    "revenue_type_id", "Select Revenue Type");
                    //this.txtColaID.BindingObject=EstablishListObjectBinding(this.CurrentBusObj.GetTable("cola") as DataTable, false, "description",
                    //    "cola_id", "Select COLA");
                    //this.txtCurrencyCode.BindingObject=EstablishListObjectBinding(this.CurrentBusObj.GetTable("currency") as DataTable, true, "description",
                    //    "currency_code", "Select Currency");
                    //this.txtPaymentTerms.BindingObject=EstablishListObjectBinding(this.CurrentBusObj.GetTable("terms") as DataTable, true, "description",
                    //    "terms_code", "Select Terms");

                    //this.txtSalesperson.BindingObject = EstablishListObjectBinding(this.CurrentBusObj.GetTable("salesperson") as DataTable, false, "Name",
                    //    "contact_id", "Select Salesperson");
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //// if the dataTable exists
                    //if (dtProducts != null)
                    //{
                    //    // ************************************************
                    //    // **********  Setup the Binding For Terms  ************
                    //    // ************************************************

                    //    // Create the BindingObject for the CategoryLookup
                    //    BindingObject bindingObject = new BindingObject();

                    //    // You must set the SourceData. Here we are creating sample a sample DataTable
                    //    bindingObject.SourceData = dtProducts;

                    //    // Set the DisplayField and the ValueField (this can be the same field)
                    //    bindingObject.DisplayField = "product_description";
                    //    bindingObject.ValueField = "product_code";

                    //    // Set the title for the window
                    //    bindingObject.Title = "Select Product";

                    //    // This bindingObject uses a string key
                    //    bindingObject.LookupAsString = true;

                    //    // The CallBackMethod does not need to be set here 
                    //    // because we are using the ucLookupTextBox.
                    //    // Look in the ucLookupTextBox.LookupValueSelected method
                    //    // to see an example of implementing your own CallBackMethod.
                    //    // this.CategoryTextBox.CallBackMethod = [CallBackMethodName];

                    //    // The name is only needed if the CallBackMethod is handling multiple
                    //    // Lookup selections, so you can set the returned values to the 
                    //    // correct controls.
                    //    bindingObject.Title = "Products";

                    //    // Set the bindingObject
                    //    this.txtProductCode.BindingObject = bindingObject;
                    //}
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }

        /// <summary>
        /// Event to handle the doubleclick of a field within the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Screen_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            //Create switch statement based on fields that can be double clicked on.
            //switch (switch_on)
            //{
            //    default:
            //}

            //@@Add Code -- Double click code to base label text box so that it knows if it is a double click item
            //and will highlight the box accordingly -- can also store the screen name and the parm values to send
            //Create a RegisterDoubleClick method in the label text base

        }

        private void CustomerDoubleClickHandler()
        {
            //don't try to go to an empty folder
            if (txtReceivableAccount.Text == "") return;
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(txtReceivableAccount.Text);
            cGlobals.ReturnParms.Add(txtReceivableAccount.Name);
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = txtReceivableAccount;
            EventAggregator.GeneratedClickHandler(this, args);            
        }

        //If no cola flag is checked then the ID is set to 0; cola source date and cola effective date set to null.
        private void txtNoColaFlag_Checked(object sender, RoutedEventArgs e)
        {
            //if (Convert.ToBoolean(txtNoColaFlag.IsChecked))
            //{
            //    ldtColaSourceDate.SelText = Convert.ToDateTime("1/1/1900");
            //    cmbColaID.SelectedValue = 0;
            //    ldtEffColaDate.SelText = Convert.ToDateTime("1/1/1900");
            //}

        }

        //Populate COLA min and max when user selects a colaid from dropdown
        private void cmbColaID_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EnumerableRowCollection<DataRow> MinMax = from COLATable in CurrentBusObj.ObjectData.Tables["cola"].AsEnumerable()
                                                      where COLATable.Field<Int32>("cola_id") == Convert.ToInt32(cmbColaID.SelectedValue)
                                                      select COLATable;

                foreach (DataRow r in MinMax)
                {
                    txtCOLAMin.Text = r["cola_min"].ToString();
                    txtCOLAMax.Text = r["cola_max"].ToString();
                }
        }


        //If Evergreen is checked then the renewal date and expiration are set to null
        //terms are all set to 0
        private void txtEvergreen_Checked(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(txtEvergreen.IsChecked))
            {
                txtRenewalDate.SelText = Convert.ToDateTime("1/1/1900");
                txtExpirationDate.SelText = Convert.ToDateTime("1/1/1900");
                txtTerm.Text = "0";
                txtRenewalTerms.Text = "0";
                txtRenewalTerm.Text = "0";
                txtTerm.IsReadOnly = true;
                txtRenewalTerm.IsReadOnly = true;
                txtRenewalTerms.IsReadOnly = true;
            }
            else
            {
                txtTerm.IsReadOnly = false;
                txtRenewalTerm.IsReadOnly = false;
                txtRenewalTerms.IsReadOnly = false;
                CalcTerm(sender, e);
            }
        }

        //Fires when the effective date, term, renewal terms or renewal term fields change
        //Recalculates the expiration date to equal
        //tas 12.18.13 - modified to use new expiration and renewal expiration logic:
        //Renewal Expiration date = effective date + terms(months) + (renewal terms * renewal term)
        //Expioration date is set to execution date + renewal term(months)
        private void CalcTerm(object sender, RoutedEventArgs e)
        {
            //If the evergreen flag is checked, do not recalculate dates
            if (txtEvergreen.IsChecked == 1)
            {
                DateTime ExpirationDate = Convert.ToDateTime("1/1/1900");
                DateTime RenewalDate = Convert.ToDateTime("1/1/1900");
                txtExpirationDate.SelText = Convert.ToDateTime(ExpirationDate);
                txtRenewalDate.SelText = Convert.ToDateTime(RenewalDate);
                return;
            }

          
            //Verify that the effective date has a value
            if (ldtEffectiveDate.SelText.ToString() != "" && ldtEffectiveDate.SelText != null)
            {
                DateTime EffectiveDate;
                //If the value is not a date then this error should fire and exit the method
                try
                {
                     EffectiveDate = Convert.ToDateTime(ldtEffectiveDate.SelText);
                }
                catch
                {
                    return;
                }

                //If no error on getting efective date then proceed with the calculation
               
                
                int Term = 0;
                int RenewalTerms = 0;
                int RenewalTerm = 0;

                Int32.TryParse(txtTerm.Text, out Term);
                Int32.TryParse(txtRenewalTerm.Text, out RenewalTerm);
                Int32.TryParse(txtRenewalTerms.Text, out RenewalTerms);

                //Verify that Term or renewal term and renewal terms have a value
                if (Term == 0 )
                {
                    txtExpirationDate.SelText = Convert.ToDateTime(EffectiveDate );
                    //txtRenewalDate.SelText = Convert.ToDateTime(EffectiveDate );
                    DateTime RenewalDate2 = Convert.ToDateTime("1/1/1900");
                    txtRenewalDate.SelText = Convert.ToDateTime(RenewalDate2);
                    return;
                }
                DateTime ExpirationDate = Convert.ToDateTime("1/1/1900");
                DateTime RenewalDate3 = Convert.ToDateTime("1/1/1900");
                //Calcuate the new renewal date
                ExpirationDate = EffectiveDate.AddMonths(Term).AddDays(-1);
                RenewalDate3 = EffectiveDate.AddMonths(Term + (RenewalTerms * RenewalTerm)).AddDays(-1); 
                txtExpirationDate.SelText = Convert.ToDateTime(ExpirationDate );
                txtRenewalDate.SelText = Convert.ToDateTime(RenewalDate3);
            }
            
            
        }

        //Event to warn the user when attempting to archive when the inactive flag is not checked or contracted flag is checked.
        private void txtArchiveFlag_Checked(object sender, RoutedEventArgs e)
        {
            string s = "";
            if (Convert.ToBoolean(txtArchiveFlag.IsChecked))
            {
                if (Convert.ToBoolean(txtContractedFlag.IsChecked))
                {
                   s="The contracted flag must be off before a contract can be archived. ";
                }

                if (!Convert.ToBoolean(txtContractStatus.IsChecked))
                {
                    s= s+ " The inactive flag must be set to inactive status before a contract can be archived.";
                }

                if (s != "")
                {
                    MessageBox.Show(s);
                    txtArchiveFlag.IsChecked = 0;
                }

            }

        }

        //If contracted flag is checked, then the inacytive flag will be uncheked
        private void txtContractedFlag_Checked(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(txtContractedFlag.IsChecked))
            {
                txtContractStatus.IsChecked = 0;
            }
        }

        //Contract cannot be flagged as inactive while contracted
        private void txtContractStatus_Checked(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(txtContractedFlag.IsChecked) && Convert.ToBoolean(txtContractStatus.IsChecked))
            {
                MessageBox.Show("A contract with the Contracted Flag checked cannot be set to inactive status.");
                txtContractStatus.IsChecked = 0;
            }
        }

        //When hold flag is checked auto populate todays date in the hold date field
        private void txtHoldFlag_Checked(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(txtHoldFlag.IsChecked))
            {
                ldtHoldDate.SelText = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
            }
        }

        private void txtContractEntity_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //don't try to go to an empty folder
            if (txtBillMsoId.Text == "") return;
            cGlobals.ReturnParms.Clear();
            //cGlobals.ReturnParms.Add(txtReceivableAccount.Text);
            //cGlobals.ReturnParms.Add(txtReceivableAccount.Name);
            //RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            //args.Source = txtReceivableAccount;
            //EventAggregator.GeneratedClickHandler(this, args);            
            //call entity folder
            //GridContacts.ReturnSelectedData("contact_id");
            cGlobals.ReturnParms.Add(txtBillMsoId.Text);
            cGlobals.ReturnParms.Add("txtContractEntity");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = txtContractEntity;
            EventAggregator.GeneratedClickHandler(this, args);            
        }

        private void txtBillMsoId_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //pull up entity lookup 
            RazerBase.Lookups.EntityLookup entityLookup = new RazerBase.Lookups.EntityLookup();

            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();

            // gets the users response
            entityLookup.ShowDialog();

            // Check if a value is returned
            //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
            //then the method should exit without changing previous value
            if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString()!="0")
            {
                txtBillMsoId.Text = cGlobals.ReturnParms[0].ToString();
                txtContractEntity.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        
        //DWR Added 1/23/13- Verifies that Contract Entity is valid and allows user to key in ID.
        private void txtBillMsoId_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBillMsoId.Text == "")
            {
                txtContractEntity.Text = "";
                return;
            }

            cBaseBusObject ContractVerification = new cBaseBusObject("ContractVerification");

            ContractVerification.Parms.ClearParms();
            ContractVerification.Parms.AddParm("@mso_id", txtBillMsoId.Text);
            ContractVerification.LoadTable("entity_name");
            if (ContractVerification.ObjectData.Tables["entity_name"] == null || ContractVerification.ObjectData.Tables["entity_name"].Rows.Count < 1)
            {
                Messages.ShowInformation("Invalid contract Entity.  Please select a valid entity ID.");
                txtBillMsoId.Text = "0";
                txtContractEntity.Text = "";
            }
            else
            {
                txtContractEntity.Text = ContractVerification.ObjectData.Tables["entity_name"].Rows[0]["name"].ToString();
            }
        }

        private void txtReceivableAccount_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //call customer lookup
            RazerBase.Lookups.CustomerLookup customerLookup = new RazerBase.Lookups.CustomerLookup();
            customerLookup.Init(new cBaseBusObject("CustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();

            // gets the users response
            customerLookup.ShowDialog();

            // Check if a value is returned
            //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
            //then the method should exit without changing previous value
            if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString()!="0")
            {
                //load current parms
                //loadParms("");
                txtReceivableAccount.Text = cGlobals.ReturnParms[0].ToString();
                txtCustomerName.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        //DWR Added 1/23/13- Verifies that Customer is valid and allows user to key in ID.
        private void txtReceivableAccount_LostFocus(object sender, RoutedEventArgs e)
        {
            //If no value in the ID field then clear out the text field and return
            if (txtReceivableAccount.Text == "")
            {
                txtCustomerName.Text = "";
                return;
            }

            //Create a new base business object-Tie it to the RObject that has the verification SPs
            cBaseBusObject ContractVerification = new cBaseBusObject("ContractVerification");

            //Establish the parameter values to use.  Since we are using a load table we only need to worry about the 
            //parameters fo the load table query we are running
            ContractVerification.Parms.ClearParms();
            ContractVerification.Parms.AddParm("@receivable_account", txtReceivableAccount.Text);
            ContractVerification.LoadTable("cust_name");
            //Verify that a value was returned.  If it was then populate the description field with the value.
            //If not return a message and zero out the ID and the text description
            if (ContractVerification.ObjectData.Tables["cust_name"] == null || ContractVerification.ObjectData.Tables["cust_name"].Rows.Count < 1)
            {
                Messages.ShowInformation("Invalid Customer ID.  Please select a valid Customer ID.");
                txtReceivableAccount.Text = "0";
                txtCustomerName.Text = "";
            }
            else
            {
                txtCustomerName.Text = ContractVerification.ObjectData.Tables["cust_name"].Rows[0]["account_name"].ToString();
            }
        }


        private void txtHoldFlag_UnChecked(object sender, RoutedEventArgs e)
        {
            ldtHoldDate.SelText = Convert.ToDateTime("01/01/1900");
            txtHoldReason.Text = "";
        }

        private void cmbProductCode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //DWR -- Removed code as it was causing for false positives on save checks
            //only force dirty when actual value changed, prevent from setting on initital load
            //ForceScreenDirty = true;
        }




       

    }


}
