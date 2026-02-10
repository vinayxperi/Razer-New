using RazerBase;
using RazerInterface;
using RazerBase.Lookups;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;

namespace TF
{
    /// <summary>
    /// Interaction logic for TFGeneralTab.xaml
    /// </summary>
    public partial class TFGeneralTab : ScreenBase, IPreBindable
    {
        public string TFNumber = "";
        public bool errorsExist = false;
        public bool NewFlag = false;
        //public ComboBoxItemsProvider cmbServiceStatus { get; set; }
        //public ComboBoxItemsProvider cmbHeadendType { get; set; } 

        public TFGeneralTab()
            : base()
        {
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        public void Init()
        {
            //bool temp;
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty General ");
            this.DoNotSetDataContext = false;
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "general";
            gTFPNI.MainTableName = "general_pni";
            gTFPNI.SetGridSelectionBehavior(false, false);
            gTFPNI.FieldLayoutResourceString = "TFPNI";
            GridCollection.Add(gTFPNI);
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty General ");
        }

        public void PreBind()
        {
            try
            {
             //if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    //cmbOldProductCode.SetBindingExpression("product_code", "product_description", this.CurrentBusObj.ObjectData.Tables["productsold"]);
                    //cmbNewProductCode.SetBindingExpression("product_code", "product_description", this.CurrentBusObj.ObjectData.Tables["productsnew"]);
                    //cmbPSACountry.SetBindingExpression("country_id", "country", this.CurrentBusObj.ObjectData.Tables["country"]);
                    //cmbPSAState.SetBindingExpression("state", "description", this.CurrentBusObj.ObjectData.Tables["state"]);
                    cmbTFStatus.SetBindingExpression("tf_status_flag", "tf_status_description", this.CurrentBusObj.ObjectData.Tables["tfStatus"]);
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }

        private void cmbOldProductCode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //if (cmbOldProductCode.SelectedText != "")
            //{
            //    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            //    {
            //        if (cmbOldProductCode.SelectedValue   == null )  
            //            cmbOldProductCode.SelectedValue = cmbOldProductCode.SelectedText;
            //        this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"] = cmbOldProductCode.SelectedValue.ToString();
            //    }
            //}
        }

        private void cmbNewProductCode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //if (cmbNewProductCode.SelectedText != "")
            //{
            //    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            //    {
            //        if (cmbNewProductCode.SelectedValue == null)
            //            cmbNewProductCode.SelectedValue = cmbNewProductCode.SelectedText;
            //        this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_product_code"] = cmbNewProductCode.SelectedValue.ToString();
            //    }
            //}
        }

        private void txtOldContractID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if approved should not be able to double click to drill down
            //if (cmbBCFType.IsEnabled == false)
            //    return;
            RazerBase.Lookups.ContractLookup ContractLookup = new RazerBase.Lookups.ContractLookup();

            ContractLookup.Init(new cBaseBusObject("ContractLookup"));
            this.CurrentBusObj.Parms.ClearParms();

            //this.CurrentBusObj.Parms.ClearParms(););
            cGlobals.ReturnParms.Clear();

            // gets the users response
            ContractLookup.ShowDialog();

            // Check if a value is returned
            //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
            //then the method should exit without changing previous value
            if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString() != "0")
            {
                //txtOldContractID.Text = cGlobals.ReturnParms[0].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        private void txtOldContractID_LostFocus(object sender, RoutedEventArgs e)
        {
            //Need to load product code based on contract id entered
            //if ((txtOldContractID.Text == "") | (txtOldContractID.Text == "0"))
            //{
            //}
            //else
            //{
            //    this.CurrentBusObj.changeParm("@contract_id", txtOldContractID.Text);
            //    this.CurrentBusObj.Parms.UpdateParmValue("@contract_id", txtOldContractID.Text);
            //    this.CurrentBusObj.LoadTable("productsold");
            //    cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");
            //    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            //        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"].ToString() == "")
            //        {
            //        }
            //        else
            //        {
            //            cmbOldProductCode.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"].ToString();
            //            cmbOldProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"].ToString();
            //        }
            //    TFVerification.Parms.ClearParms();
            //    TFVerification.Parms.AddParm("@contract_id", txtOldContractID.Text);
            //    TFVerification.LoadTable("mso");
            //    if (TFVerification.ObjectData.Tables["mso"].Rows.Count > 0)
            //    {
            //        txtOldOwnerMSOID.Text = TFVerification.ObjectData.Tables["mso"].Rows[0]["mso_id"].ToString();
            //        txtOldOwnerMSOName.Text = TFVerification.ObjectData.Tables["mso"].Rows[0]["name"].ToString();
            //        txtOldCustomerID.Text = TFVerification.ObjectData.Tables["mso"].Rows[0]["receivable_account"].ToString();
            //        txtOldCustomerName.Text = TFVerification.ObjectData.Tables["mso"].Rows[0]["account_name"].ToString();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Invalid Old Contract ID");
            //        txtOldContractID.Focus();
            //    }
            //}
        }
      
        private void txtNewContractID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if approved should not be able to double click to drill down
            //if (cmbBCFType.IsEnabled == false)
            //    return;
            RazerBase.Lookups.ContractLookup ContractLookup = new RazerBase.Lookups.ContractLookup();

            ContractLookup.Init(new cBaseBusObject("ContractLookup"));
            this.CurrentBusObj.Parms.ClearParms();

            //this.CurrentBusObj.Parms.ClearParms(););
            cGlobals.ReturnParms.Clear();

            // gets the users response
            ContractLookup.ShowDialog();

            // Check if a value is returned
            //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
            //then the method should exit without changing previous value
            if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString() != "0")
            {
                //txtNewContractID.Text = cGlobals.ReturnParms[0].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        private void txtNewContractID_LostFocus(object sender, RoutedEventArgs e)
        {
            //Need to load product code based on contract id entered
            //if ((txtNewContractID.Text == "") | (txtNewContractID.Text == "0"))
            //{
            //}
            //else
            //{
            //    this.CurrentBusObj.changeParm("@contract_id", txtNewContractID.Text);
            //    this.CurrentBusObj.Parms.UpdateParmValue("@contract_id", txtNewContractID.Text);
            //    this.CurrentBusObj.LoadTable("productsnew");
            //    cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");
            //    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            //        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_product_code"].ToString() == "")
            //        {
            //        }
            //        else
            //        {
            //            cmbNewProductCode.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_product_code"].ToString();
            //            cmbNewProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_product_code"].ToString();
            //        }
            //    TFVerification.Parms.ClearParms();
            //    TFVerification.Parms.AddParm("@contract_id", txtNewContractID.Text);
            //    TFVerification.LoadTable("mso");
            //    if (TFVerification.ObjectData.Tables["mso"].Rows.Count > 0)
            //    {
            //        txtNewOwnerMSOID.Text = TFVerification.ObjectData.Tables["mso"].Rows[0]["mso_id"].ToString();
            //        txtNewOwnerMSOName.Text = TFVerification.ObjectData.Tables["mso"].Rows[0]["name"].ToString();
            //        txtNewCustomerID.Text = TFVerification.ObjectData.Tables["mso"].Rows[0]["receivable_account"].ToString();
            //        txtNewCustomerName.Text = TFVerification.ObjectData.Tables["mso"].Rows[0]["account_name"].ToString();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Invalid Contract ID");
            //        txtNewContractID.Focus();
            //    }
            //}
        }

        //public void TFpniClearGrid()
        //{
        //    this.CurrentBusObj.ObjectData.Tables["pni"].Rows.Clear();
        //}

        public void LoadDDDWProductsbyContractID()
        {
            //Need to load product code based on contract id entered
            //if ((txtOldContractID.Text == "") | (txtOldContractID.Text == "0"))
            //{
            //}
            //else
            //{

            //    this.CurrentBusObj.changeParm("@contract_id", txtOldContractID.Text);
            //    this.CurrentBusObj.Parms.UpdateParmValue("@contract_id", txtOldContractID.Text);
            //    this.CurrentBusObj.LoadTable("productsold");
            //    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            //        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"].ToString() == "")
            //        {
            //        }
            //        else
            //        {
            //            cmbOldProductCode.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"].ToString().Trim();
            //            cmbOldProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"].ToString().Trim();
            //        }
            //}

            //if ((txtNewContractID.Text == "") | (txtNewContractID.Text == "0"))
            //{
            //}
            //else
            //{

            //    this.CurrentBusObj.changeParm("@contract_id", txtNewContractID.Text);
            //    this.CurrentBusObj.Parms.UpdateParmValue("@contract_id", txtNewContractID.Text);
            //    this.CurrentBusObj.LoadTable("productsnew");
            //    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            //        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_product_code"].ToString() == "")
            //        {
            //        }
            //        else
            //        {
            //            cmbNewProductCode.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_product_code"].ToString().Trim();
            //            cmbNewProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_product_code"].ToString().Trim();
            //        }
            //}
        }

        public void ValidatebeforeSave()
        {
            errorsExist = false;
            //Need to validate columns are populated
           
            //if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_contract_id"].ToString() == "0") || (txtOldContractID.Text == "0"))
            //{
            //    errorsExist = true;
            //    Messages.ShowInformation("Old Contract ID is required ");
            //    return;
            //}

            //if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_contract_id"].ToString() == "0") || (txtNewContractID.Text == "0"))
            //{
            //    errorsExist = true;
            //    Messages.ShowInformation("New Contract ID is required ");
            //    return;
            //}

            txtTFEffectiveDate.CntrlFocus();

            if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_id"].ToString() == "0") || (txtOldOwnerMSOID.Text == "0"))
            {
                errorsExist = true;
                Messages.ShowInformation("Old Owner Entity ID is required ");
                return;
            }

            if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_owner_mso_id"].ToString() == "0") || (txtNewOwnerMSOID.Text == "0"))
            {
                errorsExist = true;
                Messages.ShowInformation("New Owner Entity ID is required ");
                return;
            }

            if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_bill_mso_id"].ToString() == "0") || (txtOldBillMSOID.Text == "0"))
            {
                errorsExist = true;
                Messages.ShowInformation("Old Bill Entity ID is required ");
                return;
            }

            if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_bill_mso_id"].ToString() == "0") || (txtNewBillMSOID.Text == "0"))
            {
                errorsExist = true;
                Messages.ShowInformation("New Bill Entity ID is required ");
                return;
            }

            //if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["TF_effective_date"].ToString() == "1/1/1900 12:00:00 AM") || (txtOldContractID.Text == " "))
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["TF_effective_date"].ToString() == "1/1/1900 12:00:00 AM")
            {
                errorsExist = true;
                Messages.ShowInformation("TF Effective Date is required ");
                return;
            }

            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_description"].ToString() == "")
            {
                errorsExist = true;
                Messages.ShowInformation("TF description is required ");
                return;
            }

        }

        public void SetDefaultValues()
        {
            DateTime today = DateTime.Today;
            //Method to set default values for inserts

            txtRequestedBy.Text = cGlobals.UserName;
            txtDateRequested.SelText = today;
            txtTFEffectiveDate.SelText = Convert.ToDateTime("01/01/1900");
            //cmbPSACountry.SelectedText = "USA";
            //txtPSACity.Text = "";
            //txtPSAState.Text = "";
            //cmbPSAState.SelectedText = "";
            txtApprovedBy.Text = "";
            txtApprovalDate.Text = "01/01/1900";
            txtOldOwnerMSOID.Text = "0";
            txtOldOwnerMSOName.Text = "";
            txtOldBillMSOID.Text = "0";
            txtOldBillMSOName.Text = "";
            //txtOldCustomerID.Text = "0";
            //txtOldCustomerName.Text = "";
            //txtOldContractID.Text = "0";
            //txtOldContractID.Text = "0";
            //cmbOldProductCode.SelectedText = "";
            txtNewOwnerMSOID.Text = "0";
            txtNewOwnerMSOName.Text = "";
            txtNewBillMSOID.Text = "0";
            txtNewBillMSOName.Text = "";
            //txtNewCustomerID.Text = "0";
            //txtNewCustomerName.Text = "";
            //txtNewContractID.Text = "0";
            //txtNewContractID.Text = "0";
            //cmbNewProductCode.SelectedText = "";
            txtNewSystemContact.Text = "";
            txtNewSystemPhone.Text = "";
            txtSpecialComments.Text = "";
      }

      //public void TFpniClearGrid()
      //{
      //    this.CurrentBusObj.ObjectData.Tables["pni"].Rows.Clear();
      //}

      private void txtOldOwnerMSOID_GotFocus(object sender, System.Windows.RoutedEventArgs e)
      {
          
      }

      private void txtOldSystemPhone_GotFocus(object sender, System.Windows.RoutedEventArgs e)
      {
          txtOldSystemPhone.IsReadOnly = false;
          txtOldSystemPhone.IsEnabled = true;
          Messages.ShowInformation("Old System Phone Got Focus");
          txtOldSystemPhone.SelectAll();

      }

      private void txtOldSystemPhone_LostFocus(object sender, System.Windows.RoutedEventArgs e)
      {
          
          Messages.ShowInformation("Old System Phone Lost Focus");


      }

      private void txtOldOwnerMSOID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
      {
          if (txtOldOwnerMSOID.Text == "")
          {
              txtOldOwnerMSOName.Text = "";
              return;
          }
          cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");

          TFVerification.Parms.ClearParms();
          TFVerification.Parms.AddParm("@mso_id", txtOldOwnerMSOID.Text);
          TFVerification.LoadTable("msoname");
          if (TFVerification.ObjectData.Tables["msoname"] == null || TFVerification.ObjectData.Tables["msoname"].Rows.Count < 1)
          {
              Messages.ShowInformation("Invalid Entity entered.  Please select a valid Old Owner Entity ID.");
              txtOldOwnerMSOID.Text = "0";
              txtOldOwnerMSOName.Text = "";
          }
          else
          {
              txtOldOwnerMSOName.Text = TFVerification.ObjectData.Tables["msoname"].Rows[0]["name"].ToString();
          //    TFLocationsTab.txtOldOwnerMSOID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_id"].ToString();
          //    TFLocationsTab.txtOldOwnerMSOName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_name"].ToString();
          }
      }

      private void txtNewOwnerMSOID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
      {
          if (txtNewOwnerMSOID.Text == "")
          {
              txtNewOwnerMSOName.Text = "";
              return;
          }
          cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");

          TFVerification.Parms.ClearParms();
          TFVerification.Parms.AddParm("@mso_id", txtNewOwnerMSOID.Text);
          TFVerification.LoadTable("msoname");
          if (TFVerification.ObjectData.Tables["msoname"] == null || TFVerification.ObjectData.Tables["msoname"].Rows.Count < 1)
          {
              Messages.ShowInformation("Invalid Entity entered.  Please select a valid New Owner Entity ID.");
              txtNewOwnerMSOID.Text = "0";
              txtNewOwnerMSOName.Text = "";
          }
          else
          {
              txtNewOwnerMSOName.Text = TFVerification.ObjectData.Tables["msoname"].Rows[0]["name"].ToString();
          }
      }

      private void txtOldBillMSOID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
      {
          if (txtOldBillMSOID.Text == "")
          {
              txtOldBillMSOName.Text = "";
              return;
          }
          cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");

          TFVerification.Parms.ClearParms();
          TFVerification.Parms.AddParm("@mso_id", txtOldBillMSOID.Text);
          TFVerification.LoadTable("msoname");
          if (TFVerification.ObjectData.Tables["msoname"] == null || TFVerification.ObjectData.Tables["msoname"].Rows.Count < 1)
          {
              Messages.ShowInformation("Invalid Entity entered.  Please select a valid Old Bill Entity ID.");
              txtOldBillMSOID.Text = "0";
              txtOldBillMSOName.Text = "";
          }
          else
          {
              txtOldBillMSOName.Text = TFVerification.ObjectData.Tables["msoname"].Rows[0]["name"].ToString();
          }
          //txtOldSystemPhone.IsTabStop = true;
          txtOldSystemPhone.CntrlFocus();
       
      }

      private void txtNewBillMSOID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
      {
          if (txtNewBillMSOID.Text == "")
          {
              txtNewBillMSOName.Text = "";
              return;
          }
          cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");

          TFVerification.Parms.ClearParms();
          TFVerification.Parms.AddParm("@mso_id", txtNewBillMSOID.Text);
          TFVerification.LoadTable("msoname");
          if (TFVerification.ObjectData.Tables["msoname"] == null || TFVerification.ObjectData.Tables["msoname"].Rows.Count < 1)
          {
              Messages.ShowInformation("Invalid Entity entered.  Please select a valid New Bill Entity ID.");
              txtNewBillMSOID.Text = "0";
              txtNewBillMSOName.Text = "";
          }
          else
          {
              txtNewBillMSOName.Text = TFVerification.ObjectData.Tables["msoname"].Rows[0]["name"].ToString();
          }
      }

      private void txtOldCustomerID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
      {
          //If no value in the ID field then clear out the text field and return
          //if (txtOldCustomerID.Text == "")
          //{
          //    txtOldCustomerName.Text = "";
          //    return;
          //}

          ////Create a new base business object-Tie it to the RObject that has the verification SPs
          //cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");

          ////Establish the parameter values to use.  Since we are using a load table we only need to worry about the 
          ////parameters fo the load table query we are running
          //TFVerification.Parms.ClearParms();
          //TFVerification.Parms.AddParm("@receivable_account", txtOldCustomerID.Text);
          //TFVerification.LoadTable("cust_name");
          ////Verify that a value was returned.  If it was then populate the description field with the value.
          ////If not return a message and zero out the ID and the text description
          //if (TFVerification.ObjectData.Tables["cust_name"] == null || TFVerification.ObjectData.Tables["cust_name"].Rows.Count < 1)
          //{
          //    Messages.ShowInformation("Invalid Customer ID.  Please select a valid Old Customer ID.");
          //    txtOldCustomerID.Text = "0";
          //    txtOldCustomerName.Text = "";
          //}
          //else
          //{
          //    txtOldCustomerName.Text = TFVerification.ObjectData.Tables["cust_name"].Rows[0]["account_name"].ToString();
          //}
      }

      private void txtNewCustomerID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
      {
          //If no value in the ID field then clear out the text field and return
          //if (txtNewCustomerID.Text == "")
          //{
          //    txtNewCustomerName.Text = "";
          //    return;
          //}

          ////Create a new base business object-Tie it to the RObject that has the verification SPs
          //cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");

          ////Establish the parameter values to use.  Since we are using a load table we only need to worry about the 
          ////parameters fo the load table query we are running
          //TFVerification.Parms.ClearParms();
          //TFVerification.Parms.AddParm("@receivable_account", txtNewCustomerID.Text);
          //TFVerification.LoadTable("cust_name");
          ////Verify that a value was returned.  If it was then populate the description field with the value.
          ////If not return a message and zero out the ID and the text description
          //if (TFVerification.ObjectData.Tables["cust_name"] == null || TFVerification.ObjectData.Tables["cust_name"].Rows.Count < 1)
          //{
          //    Messages.ShowInformation("Invalid Customer ID.  Please select a valid New Customer ID.");
          //    txtNewCustomerID.Text = "0";
          //    txtNewCustomerName.Text = "";
          //}
          //else
          //{
          //    txtNewCustomerName.Text = TFVerification.ObjectData.Tables["cust_name"].Rows[0]["account_name"].ToString();
          //}
      }

      private void txtOldOwnerMSOID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
          //if approved should not be able to double click to drill down
          //if (cmbBCFType.IsEnabled == false)
          //    return;
          //pull up entity lookup 
          RazerBase.Lookups.EntityLookup entityLookup = new RazerBase.Lookups.EntityLookup();

          //this.CurrentBusObj.Parms.ClearParms();
          cGlobals.ReturnParms.Clear();

          // gets the users response
          entityLookup.ShowDialog();

          // Check if a value is returned
          //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
          //then the method should exit without changing previous value
          //if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString() != "0")
          if (cGlobals.ReturnParms.Count > 0)
          {
              txtOldOwnerMSOID.Text = cGlobals.ReturnParms[0].ToString();
              txtOldOwnerMSOName.Text = cGlobals.ReturnParms[1].ToString();
              // Clear the parms
              cGlobals.ReturnParms.Clear();
          }
      }

      private void txtNewOwnerMSOID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
          //if approved should not be able to double click to drill down
          //if (cmbBCFType.IsEnabled == false)
          //    return;
          //pull up entity lookup 
          RazerBase.Lookups.EntityLookup entityLookup = new RazerBase.Lookups.EntityLookup();

          //this.CurrentBusObj.Parms.ClearParms();
          cGlobals.ReturnParms.Clear();

          // gets the users response
          entityLookup.ShowDialog();

          // Check if a value is returned
          //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
          //then the method should exit without changing previous value
          //if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString() != "0")
          if (cGlobals.ReturnParms.Count > 0)
          {
              txtNewOwnerMSOID.Text = cGlobals.ReturnParms[0].ToString();
              txtNewOwnerMSOName.Text = cGlobals.ReturnParms[1].ToString();
              // Clear the parms
              cGlobals.ReturnParms.Clear();
          }
      }

      private void txtOldBillMSOID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
          //if approved should not be able to double click to drill down
          //if (cmbBCFType.IsEnabled == false)
          //    return;
          //pull up entity lookup 
          RazerBase.Lookups.EntityLookup entityLookup = new RazerBase.Lookups.EntityLookup();

          //this.CurrentBusObj.Parms.ClearParms();
          cGlobals.ReturnParms.Clear();

          // gets the users response
          entityLookup.ShowDialog();

          // Check if a value is returned
          //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
          //then the method should exit without changing previous value
          //if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString() != "0")
          if (cGlobals.ReturnParms.Count > 0)
          {
              txtOldBillMSOID.Text = cGlobals.ReturnParms[0].ToString();
              txtOldBillMSOName.Text = cGlobals.ReturnParms[1].ToString();
              // Clear the parms
              cGlobals.ReturnParms.Clear();
          }
      }

      private void txtNewBillMSOID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
          //if approved should not be able to double click to drill down
          //if (cmbBCFType.IsEnabled == false)
          //    return;
          //pull up entity lookup 
          RazerBase.Lookups.EntityLookup entityLookup = new RazerBase.Lookups.EntityLookup();

          //this.CurrentBusObj.Parms.ClearParms();
          cGlobals.ReturnParms.Clear();

          // gets the users response
          entityLookup.ShowDialog();

          // Check if a value is returned
          //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
          //then the method should exit without changing previous value
          //if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString() != "0")
          if (cGlobals.ReturnParms.Count > 0)
          {
              txtNewBillMSOID.Text = cGlobals.ReturnParms[0].ToString();
              txtNewBillMSOName.Text = cGlobals.ReturnParms[1].ToString();
              // Clear the parms
              cGlobals.ReturnParms.Clear();
          }
      }

      private void txtOldCustomerID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
          //if approved should not be able to double click to drill down
          //if (cmbBCFType.IsEnabled == false)
          //    return;
          //call location lookup
          //RazerBase.Lookups.CustomerLookup customerLookup = new RazerBase.Lookups.CustomerLookup();
          //customerLookup.Init(new cBaseBusObject("CustomerLookup"));

          ////this.CurrentBusObj.Parms.ClearParms();
          //cGlobals.ReturnParms.Clear();

          //// gets the users response
          //customerLookup.ShowDialog();

          //// Check if a value is returned
          ////DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
          ////then the method should exit without changing previous value
          //if (cGlobals.ReturnParms.Count > 0)
          //{
          //    //load current parms
          //    //loadParms("");
          //    txtOldCustomerID.Text = cGlobals.ReturnParms[0].ToString();
          //    txtOldCustomerName.Text = cGlobals.ReturnParms[1].ToString();
          //    // Clear the parms
          //    cGlobals.ReturnParms.Clear();
          //}
      }

      private void txtNewCustomerID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
          ////if approved should not be able to double click to drill down
          ////if (cmbBCFType.IsEnabled == false)
          ////    return;
          ////call location lookup
          //RazerBase.Lookups.CustomerLookup customerLookup = new RazerBase.Lookups.CustomerLookup();
          //customerLookup.Init(new cBaseBusObject("CustomerLookup"));

          ////this.CurrentBusObj.Parms.ClearParms();
          //cGlobals.ReturnParms.Clear();

          //// gets the users response
          //customerLookup.ShowDialog();

          //// Check if a value is returned
          ////DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
          ////then the method should exit without changing previous value
          //if (cGlobals.ReturnParms.Count > 0)
          //{
          //    //load current parms
          //    //loadParms("");
          //    txtNewCustomerID.Text = cGlobals.ReturnParms[0].ToString();
          //    txtNewCustomerName.Text = cGlobals.ReturnParms[1].ToString();
          //    // Clear the parms
          //    cGlobals.ReturnParms.Clear();
          //}
      }

      private void ScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
      {

      }

    }
}
