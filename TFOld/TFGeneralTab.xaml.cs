

#region using statements
using RazerBase;
using RazerInterface;
using RazerBase.Lookups;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;

#endregion

namespace TF
{

    #region class BCFGeneralTab
    /// <summary>
    /// This class represents a 'ucTab1' object.
    /// </summary>
    public partial class BCFGeneralTab : ScreenBase, IPreBindable
    {


        public string BCFNumber = "";
        public bool errorsExist = false;
        public bool NewFlag = false;
        public ComboBoxItemsProvider cmbServiceStatus { get; set; } 
          public ComboBoxItemsProvider cmbHeadendType { get; set; } 
       

        /// Create a new instance of a 'ucTab1' object and call the ScreenBase's constructor.
        /// </summary>
        public BCFGeneralTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }


        #region Methods

        /// </summary>
        public void Init()
        {
            this.DoNotSetDataContext = false;
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "general";
            gPNI.MainTableName = "pni";
            gPNI.ConfigFileName = "BCFPNIHistory";
            gPNI.SetGridSelectionBehavior(false, false);
            gPNI.FieldLayoutResourceString = "BCFPNI";
            gPNI.EditModeEndedDelegate = gPNI_EditModeEnded; //This allows for data checks after each cell is exited
            //gPNI.EditModeStartedDelegate = gPNI_EditModeStarting;
            gPNI.ContextMenuAddDelegate = PNIGridAddDelegate;
            gPNI.ContextMenuAddDisplayName = "Add PNI Information";
            gPNI.ContextMenuRemoveDelegate = PNIGridDeleteDelegate;
            gPNI.ContextMenuRemoveDisplayName = "Delete PNI Information";
            GridCollection.Add(gPNI);
        }


        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {

                    cmbProductCode.SetBindingExpression("product_code", "product_description", this.CurrentBusObj.ObjectData.Tables["products"]);
                    cmbBCFType.SetBindingExpression("BCF_type", "BCF_type_description", this.CurrentBusObj.ObjectData.Tables["bcftype"]);
                    cmbPSACountry.SetBindingExpression("country_id", "country", this.CurrentBusObj.ObjectData.Tables["country"]);
                    cmbServiceType.SetBindingExpression("head_end_type", "head_end_type_desc", this.CurrentBusObj.ObjectData.Tables["dddwHeadend"]);
                    cmbBCFStatus.SetBindingExpression("bcf_status_flag", "bcf_status_description", this.CurrentBusObj.ObjectData.Tables["bcfStatus"]);
                    cmbToPSACountry.SetBindingExpression("country_id", "country", this.CurrentBusObj.ObjectData.Tables["country"]);
                    //Service Status dddw in pni grid
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    ip = new ComboBoxItemsProvider();
                    ip.ItemsSource = CurrentBusObj.ObjectData.Tables["dddwService"].DefaultView;
                    ip.ValuePath = "service_status";
                    ip.DisplayMemberPath = ("status_desc");
                    cmbServiceStatus = ip;
                    //Headend dddw in PNI grid
                    ComboBoxItemsProvider ip2 = new ComboBoxItemsProvider();
                    ip2 = new ComboBoxItemsProvider();
                    ip2.ItemsSource = CurrentBusObj.ObjectData.Tables["dddwHeadend"].DefaultView;
                    ip2.ValuePath = "head_end_type";
                    ip2.DisplayMemberPath = ("head_end_type_desc");
                    cmbHeadendType = ip2;
                    

                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }


        private void PNIGridAddDelegate()
        {
            string wfStatus = "";
            if (CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                if (wfStatus[0] == 'I' || wfStatus[0] == 'A')
                    return;
            }
            else
                return;
            gPNI.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = gPNI.xGrid.RecordManager.CurrentAddRecord;
            string bcf_number = "";
            //Set the default values for the columns
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                bcf_number = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"].ToString();
            }


            row.Cells["BCF_number"].Value = bcf_number;
            ////need to set it to report_id selected in gReports
            row.Cells["mso_id"].Value = txtMSOID.Text;
            row.Cells["cs_id"].Value = txtcsID.Text;
            row.Cells["head_id"].Value = 0;
            row.Cells["head_end_type"].Value = 0;
            row.Cells["service_id"].Value = 0;
            row.Cells["service_status"].Value = 0;
            row.Cells["fs_id"].Value = 0;
            row.Cells["mca_address"].Value = 0;
          
           

            //Commit the add new record - required to make this record active
            gPNI.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            gPNI.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            gPNI.xGrid.ActiveRecord = gPNI.xGrid.Records[0];
            //Set the field as active
            (gPNI.xGrid.Records[gPNI.ActiveRecord.Index] as DataRecord).Cells["head_id"].IsActive = true;

            gPNI.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        private void PNIGridDeleteDelegate()
        {
            string wfStatus = "";
            if (CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                if (wfStatus[0] == 'I' || wfStatus[0] == 'A')
                    return;
            }
            else
                return;
            MessageBoxResult result = Messages.ShowYesNo("PNI will be deleted from this BCF. Once deleted, to make the changes to the database, you will need to do a Save. Are you sure you want to delete?",
                 System.Windows.MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataRecord r = gPNI.ActiveRecord;
                if (r != null)
                {
                    DataRow row = (r.DataItem as DataRowView).Row;
                    if (row != null)
                    {

                        row.Delete();
                        // Clear the parms
                        cGlobals.ReturnParms.Clear();
                    }
                }
            }
        }
        public void gPNI_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            

            int ServiceID = 0;
            DataRecord row = e.Cell.Record; //gPNI.ActiveRecord;

            if (e.Cell.Field.Name == "service_id")
            {
                //DWR ADDED 2/14/13 - Prevents crash if service_id is deleted
                if (DBNull.Value.Equals(e.Cell.Value))
                    e.Cell.Value = 0;


                //Add all parameters back in
                ServiceID = Convert.ToInt32(e.Cell.Value);
                if (ServiceID == 0)
                    return;

                cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");

                BCFVerification.Parms.ClearParms();
                BCFVerification.Parms.AddParm("@service_id", ServiceID);
                BCFVerification.LoadTable("service");
                if (BCFVerification.ObjectData.Tables["service"] == null || BCFVerification.ObjectData.Tables["service"].Rows.Count < 1)
                {
                    Messages.ShowInformation("Invalid Service ID entered.  Please select a valid Service ID.");
                    ServiceID = 0;

                }
                else
                {
                    row.Cells["head_id"].Value = BCFVerification.ObjectData.Tables["service"].Rows[0]["head_id"];
                    row.Cells["fs_id"].Value = BCFVerification.ObjectData.Tables["service"].Rows[0]["file_server_id"];
                    row.Cells["mca_address"].Value = 0;



                }
            }

        }

        public void gPNI_EditModeStarting(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs e)
        {
            //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            //{
            //    int BCFStatus = 0;
            //    string wfStatus = "";
            //    BCFStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]);
            //    wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];

            //    if (BCFStatus.ToString() == "1" || wfStatus[0] == 'A')
            //    {
            //        gPNI.xGrid.FieldSettings.AllowEdit = false;
            //        gPNI.ContextMenuAddIsVisible = false;
            //        return;
            //    }
            //    else
            //    {
            //        gPNI.xGrid.FieldSettings.AllowEdit = true;
            //        gPNI.ContextMenuAddIsVisible = true;
            //    }
            //}
        }

        public void SetDefaultValues()
        {
            DateTime today = DateTime.Today;
            //Method to set default values for inserts
           
            txtRequestedBy.Text = cGlobals.UserName;
            txtDateRequested.SelText = today;
            txtBCFEffectiveDate.SelText = Convert.ToDateTime("01/01/1900");
            txtContractID.Text = "0";
            cmbProductCode.SelectedText = "";
            txtcsID.Text = "0";
            cmbPSACountry.SelectedText = "USA";
           
            txtPSACity.Text = "";
            txtMSOID.Text = "0";
            txtBCFEffectiveDate.SelText = Convert.ToDateTime("01/01/1900");
            txtApprovedBy.Text = "";
            txtApprovalDate.Text = "01/01/1900";
           
          
            txtCustomerID.Text = "";
            txtLastValidInvoicedMonth.SelText = Convert.ToDateTime("01/01/1900");
            txtBCFDescription.Text = "";
            txtToCSID.Text = "0";
            txtToPSACity.Text = "";
            txtToPSAState.Text = "";
            txtToService.Text = "0";
            txtToMCA.Text = "0";
            txtToHeadEndID.Text = "0";







        }

        public void BCFpniClearGrid()
        {
            this.CurrentBusObj.ObjectData.Tables["pni"].Rows.Clear();
        }
        public void ValidatebeforeSave()
        {



            errorsExist = false;
            cmbBCFType.CntrlFocus();
            //Need to validate columns are populated
            if (cmbBCFType.SelectedText == "")
            {
                errorsExist = true;
                Messages.ShowInformation("BCF type is required");
                return;
            }

            if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"].ToString() == "0") || (txtContractID.Text == "0"))
            {
                errorsExist = true;
                Messages.ShowInformation("Contract ID is required ");
                return;
            }

            if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_effective_date"].ToString() == "1/1/1900 12:00:00 AM") || (txtContractID.Text == " "))
            {
                errorsExist = true;
                Messages.ShowInformation("BCF Effective Date is required ");
                return;
            }
            //Per Rhonda and Oliver on 10/4/2013 no longer a requirement
            //if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["last_valid_invoiced_month"].ToString() == "1/1/1900 12:00:00 AM") || (txtContractID.Text == " ")) 
            //{
            //    errorsExist = true;
            //    Messages.ShowInformation("Last Valid Invoice Month is required ");
            //    return;
            //}

            if ((cmbBCFType.SelectedText[0] == 'C') || (cmbBCFType.SelectedText[0] == 'T'))
            {

                if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["cs_id"].ToString() == "") || (txtcsID.Text == "0"))
                {
                    if (Convert.ToBoolean(txtMultipleLocations.IsChecked))
                    {
                    }
                    else
                    {


                        errorsExist = true;
                        Messages.ShowInformation("Location is required for a Consolidation or Transfer");
                        return;
                    }
                }
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["multiple_locations"].ToString() == "1")
                {
                    txtcsID.Text = "0";
                    txtcsName.Text = "";
                    txtPSACity.Text = "";
                    txtPSAState.Text = "";
                  
                    cmbPSACountry.SelectedValue = 0;
                    this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["cs_id"] = 0;
                }
                if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["to_cs_id"].ToString() == "") || (txtToCSID.Text == "0"))
                {
                    errorsExist = true;
                    Messages.ShowInformation("To Location is required for a Consolidation or Transfer");
                    return;
                }
                //if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["to_service_id"].ToString() == "") || (txtToService.Text == "0"))
                //{
                //    errorsExist = true;
                //    Messages.ShowInformation("To Service ID is required for a Consolidation or Transfer");
                //    return;
                //}
                if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["nbrofsubs"].ToString() == "") || (txtNbrofSubs.Text == "0"))
                {
                    errorsExist = true;
                    Messages.ShowInformation("Number of Subs is required for a Consolidation or Transfer");
                    return;
                }


            }

            if   ((cmbBCFType.SelectedValue.ToString() == "1") || (cmbBCFType.SelectedValue.ToString() == "2") || (cmbBCFType.SelectedValue.ToString() == "5") || (cmbBCFType.SelectedValue.ToString() == "6")) 
            {
                if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contact_name"].ToString() == "") || (txtContactName.Text == ""))
                {
                    errorsExist = true;
                    Messages.ShowInformation("Contact Name is required for a Transfer, Inactivation, Reactivation and Consolidation");
                    return;
                }
            }
            if (cmbBCFType.SelectedText[0] == 'T')
            {
                //if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["service_type"].ToString() == "") || (cmbServiceType.SelectedText == ""))
                //{
                //    errorsExist = true;
                //    Messages.ShowInformation("Headend Type is required for a Transfer");
                //    return;
                //}
                //if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["to_head_id"].ToString() == "") || (txtToHeadEndID.Text == "0"))
                //{
                //    errorsExist = true;
                //    Messages.ShowInformation("To HeadEnd ID is required for a Transfer");
                //    return;
                //}
            }
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString() == "")
            {
                if ((cmbProductCode.SelectedValue == null) || (cmbProductCode.SelectedValue.ToString() == ""))
                {
                    errorsExist = true;
                    Messages.ShowInformation("Product Code is required");
                    return;
                }

                //if (cmbProductCode.SelectedValue.ToString() == "")    
                //  {
                //       errorsExist = true;
                //          Messages.ShowInformation("Product Code is required");
                //         return;
                //    }

                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"] = cmbProductCode.SelectedValue.ToString();
            }
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["receivable_account"].ToString() == "")
            {
                errorsExist = true;
                Messages.ShowInformation("Customer ID is required");
                return;
            }
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_Description"].ToString() == "")
            {
                if (txtBCFDescription.Text == "")
                {
                    errorsExist = true;
                    Messages.ShowInformation("BCF Description is required");
                    txtBCFDescription.CntrlFocus();
                    return;
                }
                else
                    this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_Description"] = txtBCFDescription.Text;
            }
            //for Inactivation, Transfer, Reactivation and Consolidation, PNI is required
            if ((cmbBCFType.SelectedValue.ToString() == "1") || (cmbBCFType.SelectedValue.ToString() == "2") || (cmbBCFType.SelectedValue.ToString() == "5") || (cmbBCFType.SelectedValue.ToString() == "6"))
            {

            }

        }

        private void txtContractDescription_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        #endregion

        private void txtMSOID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if approved should not be able to double click to drill down
            if (cmbBCFType.IsEnabled == false)
                return;
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
                txtMSOID.Text = cGlobals.ReturnParms[0].ToString();
                txtMSOName.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        private void txtMSOID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtMSOID.Text == "")
            {
                txtMSOName.Text = "";
                return;
            }
            cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");

            BCFVerification.Parms.ClearParms();
            BCFVerification.Parms.AddParm("@mso_id", txtMSOID.Text);
            BCFVerification.LoadTable("msoname");
            if (BCFVerification.ObjectData.Tables["msoname"] == null || BCFVerification.ObjectData.Tables["msoname"].Rows.Count < 1)
            {
                Messages.ShowInformation("Invalid Entity entered.  Please select a valid Entity ID.");
                txtMSOID.Text = "0";
                txtMSOName.Text = "";
            }
            else
            {
                txtMSOName.Text = BCFVerification.ObjectData.Tables["msoname"].Rows[0]["name"].ToString();
            }
        }

        private void txtCustomerID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if approved should not be able to double click to drill down
            if (cmbBCFType.IsEnabled == false)
                return;
            //call location lookup
            RazerBase.Lookups.CustomerLookup customerLookup = new RazerBase.Lookups.CustomerLookup();
            customerLookup.Init(new cBaseBusObject("CustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();

            // gets the users response
            customerLookup.ShowDialog();

            // Check if a value is returned
            //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
            //then the method should exit without changing previous value
            if (cGlobals.ReturnParms.Count > 0)
            {
                //load current parms
                //loadParms("");
                txtCustomerID.Text = cGlobals.ReturnParms[0].ToString();
                txtCustomerName.Text = cGlobals.ReturnParms[1].ToString();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }


        private void txtCustomerID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //If no value in the ID field then clear out the text field and return
            if (txtCustomerID.Text == "")
            {
                txtCustomerName.Text = "";
                return;
            }

            //Create a new base business object-Tie it to the RObject that has the verification SPs
            cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");

            //Establish the parameter values to use.  Since we are using a load table we only need to worry about the 
            //parameters fo the load table query we are running
            BCFVerification.Parms.ClearParms();
            BCFVerification.Parms.AddParm("@receivable_account", txtCustomerID.Text);
            BCFVerification.LoadTable("cust_name");
            //Verify that a value was returned.  If it was then populate the description field with the value.
            //If not return a message and zero out the ID and the text description
            if (BCFVerification.ObjectData.Tables["cust_name"] == null || BCFVerification.ObjectData.Tables["cust_name"].Rows.Count < 1)
            {
                Messages.ShowInformation("Invalid Customer ID.  Please select a valid Customer ID.");
                txtCustomerID.Text = "0";
                txtCustomerName.Text = "";
            }
            else
            {
                txtCustomerName.Text = BCFVerification.ObjectData.Tables["cust_name"].Rows[0]["account_name"].ToString();
            }
        }

        private void txtcsID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            //if approved should not be able to double click to drill down
            if (cmbBCFType.IsEnabled == false)
                return;
            //new code
            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            BCFLocationsLookup f = new BCFLocationsLookup();
            if (txtContractID.Text == "")
            {
                MessageBox.Show("Contract ID is required for Location Lookup");
                return;
            }
            else
                cGlobals.ReturnParms.Add(txtContractID.Text.ToString());

            if (cmbProductCode.SelectedValue == null || cmbProductCode.SelectedValue.ToString() == "" )
            {
                MessageBox.Show("Product Code is required for Location Lookup");
                return;
            }
            else
                cGlobals.ReturnParms.Add(cmbProductCode.SelectedValue.ToString());
 



            f.Init(new cBaseBusObject("BCFLocationsLookup"));


            // gets the users response
            f.ShowDialog();
            //end of new code
            //RazerBase.Lookups.LocationLookup LocationLookup = new RazerBase.Lookups.LocationLookup();


            //this.CurrentBusObj.Parms.ClearParms();



            ////this.CurrentBusObj.Parms.ClearParms();
            //cGlobals.ReturnParms.Clear();

            //// gets the users response
            //LocationLookup.ShowDialog();

            // Check if a value is returned
            //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
            //then the method should exit without changing previous value
            if (cGlobals.ReturnParms.Count > 0)
            {
                //load current parms
                //loadParms("");
                txtcsID.Text = cGlobals.ReturnParms[0].ToString();
                //txtcsName.Text = cGlobals.ReturnParms[1].ToString();
                txtCSId_Verify();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        public void txtCSId_Load_PNI()
        {
            //for Inactivation, Transfer, Reactivation and Consolidation, Auto Load PNI
            if ((cmbBCFType.SelectedValue.ToString() == "1") || (cmbBCFType.SelectedValue.ToString() == "2") || (cmbBCFType.SelectedValue.ToString() == "5") || (cmbBCFType.SelectedValue.ToString() == "6"))
            {
                if (gPNI.xGrid.Records.Count > 0)
                {
                    BCFpniClearGrid();

                }
                cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");
                BCFVerification.Parms.ClearParms();
                BCFVerification.Parms.AddParm("@cs_id", txtcsID.Text);
                BCFVerification.Parms.AddParm("@product_code", cmbProductCode.SelectedValue.ToString());
                BCFVerification.LoadTable("getPNI");
                 if (BCFVerification != null && BCFVerification.HasObjectData)
                 {
                     if (BCFVerification.ObjectData.Tables["getPNI"].Rows.Count > 0)
                     {
                         
                          

                         foreach (DataRow r in BCFVerification.ObjectData.Tables["getPNI"].Rows)
                         {



                             gPNI.xGrid.FieldLayoutSettings.AllowAddNew = true;
                             DataRecord row = gPNI.xGrid.RecordManager.CurrentAddRecord;
                             if (BCFNumber.ToString() == "")
                                 BCFNumber = "NEW";
                             row.Cells["BCF_number"].Value = BCFNumber.ToString();
                             row.Cells["cs_id"].Value = txtcsID.Text;
                             row.Cells["head_id"].Value = r["head_id"];
                             row.Cells["fs_id"].Value = r["file_server_id"];
                             row.Cells["service_id"].Value = r["service_id"];
                             row.Cells["mca_address"].Value = r["mca_address"];
                             row.Cells["head_end_type"].Value = r["head_end_type"];
                             row.Cells["mso_id"].Value = txtMSOID.Text;
                             row.Cells["service_status"].Value = r["service_status"];
                             //Commit the add new record - required to make this record active
                             gPNI.xGrid.RecordManager.CommitAddRecord();
                             //Remove the add new record row
                             gPNI.xGrid.FieldLayoutSettings.AllowAddNew = false;
                             //Set the row just created to the active record
                             gPNI.xGrid.ActiveRecord = gPNI.xGrid.Records[0];
                             //Set the field as active
                             (gPNI.xGrid.Records[gPNI.ActiveRecord.Index] as DataRecord).Cells["head_id"].IsActive = true;

                             gPNI.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);

                         }
                     
                        }
                    }

                   
            

                gPNI.LoadGrid(CurrentBusObj, "pni");
                }

            }
  


        public void txtCSId_Verify()
        {
            if (txtcsID.Text == "")
            {
                txtcsName.Text = "";
                return;
            }
            cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");

            BCFVerification.Parms.ClearParms();
            BCFVerification.Parms.AddParm("@cs_id", txtcsID.Text);
            BCFVerification.LoadTable("location");

            if (txtcsID.Text == "0")
                return;
            else
            {
                if (BCFVerification.ObjectData.Tables["location"] == null || BCFVerification.ObjectData.Tables["location"].Rows.Count < 1)
                {
                    Messages.ShowInformation("Invalid location entered.  Please select a valid Location ID.");
                    txtcsID.Text = "0";
                    txtcsName.Text = "";
                    txtPSACity.Text = "";
                    txtPSAState.Text = "";
                    txtMSOID.Text = "0";
                    txtMSOName.Text = "";
                    cmbPSACountry.SelectedValue = 0;
                }
                else
                {
                    //validate the location belongs to the entity id
                    txtcsName.Text = BCFVerification.ObjectData.Tables["location"].Rows[0]["name"].ToString();
                    txtPSACity.Text = BCFVerification.ObjectData.Tables["location"].Rows[0]["psa_city"].ToString();
                    txtPSAState.Text = BCFVerification.ObjectData.Tables["location"].Rows[0]["psa_state"].ToString();
                    cmbPSACountry.SelectedValue = BCFVerification.ObjectData.Tables["location"].Rows[0]["psa_country"];

                    this.CurrentBusObj.changeParm("@cs_id", txtcsID.Text);
                    this.CurrentBusObj.Parms.UpdateParmValue("@cs_id", txtcsID.Text);
                }

            }

        }

        private void txtcsID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtcsID.Text == "")
            {
                txtcsName.Text = "";
                return;
            }
            if (cmbProductCode.SelectedText == "")
            {
                Messages.ShowInformation("A product code is required");
                return;
            }
            txtCSId_Verify();
            txtCSId_Load_PNI();
            string debug = "";
        }



        private void txtContractID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if approved should not be able to double click to drill down
            if (cmbBCFType.IsEnabled == false)
                return;
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
                //load current parms
                //loadParms("");
                txtContractID.Text = cGlobals.ReturnParms[0].ToString();

                // Clear the parms
                cGlobals.ReturnParms.Clear();


            }



        }

        private void txtHEActive_Checked(object sender, RoutedEventArgs e)
        {
            txtRemServiceID1.IsEnabled = true;
            txtRemServiceID2.IsEnabled = true;
            txtRemServiceID3.IsEnabled = true;


        }

        private void txtHEActive_UnChecked(object sender, RoutedEventArgs e)
        {
            txtRemServiceID1.IsEnabled = false;
            txtRemServiceID2.IsEnabled = false;
            txtRemServiceID3.IsEnabled = false;

        }

        private void cmbProductCode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (cmbProductCode.SelectedText != "")
            {
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                {
                    if (cmbProductCode.SelectedValue   == null )  

                        cmbProductCode.SelectedValue = cmbProductCode.SelectedText;

                    this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"] = cmbProductCode.SelectedValue.ToString();
                }
            }

        }

        private void txtMultipleLocations_UnChecked(object sender, RoutedEventArgs e)
        {
            cmbPSACountry.SelectedValue = 0;
            cmbPSACountry.SelectedText = "None";
            //Blank out all location rows
            if (this.CurrentBusObj.HasObjectData)
            {
                this.CurrentBusObj.ObjectData.Tables["locations"].AcceptChanges();
                foreach (DataRow row in this.CurrentBusObj.ObjectData.Tables["locations"].Rows)
                {

                    if (row != null)
                    {
                        row.Delete();
                    }
                }

                //this.CurrentBusObj.ObjectData.Tables["locations_pni"].AcceptChanges();

                this.CurrentBusObj.ObjectData.Tables["locations_pni"].AcceptChanges();
                foreach (DataRow row in this.CurrentBusObj.ObjectData.Tables["locations_pni"].Rows)
                {

                    if (row != null)
                    {
                        row.Delete();
                    }
                }

                //this.CurrentBusObj.ObjectData.Tables["locations_pni"].AcceptChanges();

                //this.CurrentBusObj.ObjectData.Tables["pni"].AcceptChanges();

                this.CurrentBusObj.ObjectData.Tables["pni"].AcceptChanges();
                foreach (DataRow row in this.CurrentBusObj.ObjectData.Tables["pni"].Rows)
                {

                    if (row != null)
                    {
                        row.Delete();
                    }
                }

                //his.CurrentBusObj.ObjectData.Tables["pni"].AcceptChanges();
                txtcsID.IsEnabled = true;






            }
        }

        private void txtMultipleLocations_Checked(object sender, RoutedEventArgs e)
        {

            if (Convert.ToBoolean(txtMultipleLocations.IsChecked))
            {
                //Blank out location information

                txtcsID.Text = "0";
                txtPSACity.Text = "";
                txtPSAState.Text = "";
                txtcsName.Text = "";
                cmbPSACountry.SelectedValue = 0;
                cmbPSACountry.SelectedText = "None";
                txtcsID.IsEnabled = false;
                //Blank out all PNI rows
                if (this.CurrentBusObj.HasObjectData)
                {
                    this.CurrentBusObj.ObjectData.Tables["pni"].AcceptChanges();
                    foreach (DataRow row in this.CurrentBusObj.ObjectData.Tables["pni"].Rows)
                    {

                        if (row != null)
                        {
                            row.Delete();
                        }
                    }

                    this.CurrentBusObj.ObjectData.Tables["pni"].AcceptChanges();
                }


            }


        }

        public void LoadDDDWProductsbyContractID()
        {
            //Need to load product code based on contract id entered
            if ((txtContractID.Text == "") | (txtContractID.Text == "0"))
            {
            }
            else
            {

                this.CurrentBusObj.changeParm("@contract_id", txtContractID.Text);
                this.CurrentBusObj.Parms.UpdateParmValue("@contract_id", txtContractID.Text);
                this.CurrentBusObj.LoadTable("products");
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString() == "")
                    {
                    }
                    else
                    {
                        cmbProductCode.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString().Trim();
                        cmbProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString().Trim() ;
                    }
            }

        }

        private void txtContractID_LostFocus(object sender, RoutedEventArgs e)
        {
            //Need to load product code based on contract id entered
            if ((txtContractID.Text == "") | (txtContractID.Text == "0"))
            {
            }
            else
            {

                this.CurrentBusObj.changeParm("@contract_id", txtContractID.Text);
                this.CurrentBusObj.Parms.UpdateParmValue("@contract_id", txtContractID.Text);
                this.CurrentBusObj.LoadTable("products");
                cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString() == "")
                    {
                    }
                    else
                    {
                        cmbProductCode.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString();
                        cmbProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString();
                    }
                BCFVerification.Parms.ClearParms();
                BCFVerification.Parms.AddParm("@contract_id", txtContractID.Text);
                BCFVerification.LoadTable("mso");
                if (BCFVerification.ObjectData.Tables["mso"].Rows.Count > 0)
                {
                    txtMSOID.Text = BCFVerification.ObjectData.Tables["mso"].Rows[0]["mso_id"].ToString();
                    txtMSOName.Text = BCFVerification.ObjectData.Tables["mso"].Rows[0]["name"].ToString();
                    txtCustomerID.Text = BCFVerification.ObjectData.Tables["mso"].Rows[0]["receivable_account"].ToString();
                    txtCustomerName.Text = BCFVerification.ObjectData.Tables["mso"].Rows[0]["account_name"].ToString();
                }
                else
                {
                    MessageBox.Show("Invalid Contract ID");
                    txtContractID.Focus();
                }

            }
        }

        private void txtToCSID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if approved should not be able to double click to drill down
            if (cmbBCFType.IsEnabled == false)
                return;
            RazerBase.Lookups.LocationLookup LocationLookup = new RazerBase.Lookups.LocationLookup();


            this.CurrentBusObj.Parms.ClearParms();



            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();

            // gets the users response
            LocationLookup.ShowDialog();

            // Check if a value is returned
            //DWR Modified 1/23/13- Add check for 0 return parm.  If 1st parm equals 0 
            //then the method should exit without changing previous value
            if (cGlobals.ReturnParms.Count > 0)
            {
                //load current parms
                //loadParms("");
                txtToCSID.Text = cGlobals.ReturnParms[0].ToString();

                txtToCSId_Verify();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        public void txtToCSId_Verify()
        {
            if (txtToCSID.Text == "")
            {

                return;
            }
            cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");

            BCFVerification.Parms.ClearParms();
            BCFVerification.Parms.AddParm("@cs_id", txtToCSID.Text);
            BCFVerification.LoadTable("location");

            if (txtToCSID.Text == "0")
                return;
            else
            {
                if (BCFVerification.ObjectData.Tables["location"] == null || BCFVerification.ObjectData.Tables["location"].Rows.Count < 1)
                {
                    Messages.ShowInformation("Invalid location entered.  Please select a valid Location ID.");
                    txtToCSID.Text = "0";

                    txtToPSACity.Text = "";
                    txtToPSAState.Text = "";
                    cmbToPSACountry.SelectedValue = 0;



                }
                else
                {

                    txtToPSACity.Text = BCFVerification.ObjectData.Tables["location"].Rows[0]["psa_city"].ToString();
                    txtToPSAState.Text = BCFVerification.ObjectData.Tables["location"].Rows[0]["psa_state"].ToString();

                    cmbToPSACountry.SelectedValue = BCFVerification.ObjectData.Tables["location"].Rows[0]["psa_country"];


                    this.CurrentBusObj.changeParm("@cs_id", txtToCSID.Text);
                    this.CurrentBusObj.Parms.UpdateParmValue("@cs_id", txtToCSID.Text);
                }

            }

        }

        private void txtToService_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if approved should not be able to double click to drill down
            if (cmbBCFType.IsEnabled == false)
                return;
            if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["to_cs_id"].ToString() == "") || (txtToCSID.Text == "0"))
            {
                errorsExist = true;
                Messages.ShowInformation("To Location is required to do a lookup on Service ID.  Please select a valid To Location ID.");
                return;
            }
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString() == "")
            {
                if ((cmbProductCode.SelectedValue == null) || (cmbProductCode.SelectedValue.ToString() == ""))
                {
                    errorsExist = true;
                    Messages.ShowInformation("Product Code is required to do a lookup on Service ID.  Please select a valid product code.");
                    return;
                }
            }

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on To Service ID field
            BCFServiceLookup f = new BCFServiceLookup();



            cGlobals.ReturnParms.Add(txtToCSID.Text.ToString());
            cGlobals.ReturnParms.Add(cmbProductCode.SelectedValue.ToString());




            f.Init(new cBaseBusObject("BCFServiceLookup"));


            // gets the users response
            f.ShowDialog();


            //DWR - Modifie 3/11/13 - Added 2nd part of if check as screen would crash app if filter and then cancel was clicked in the lookup.
            if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0] != null)
            {

                //load current parms
                txtToService.Text = cGlobals.ReturnParms[0].ToString();



                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }


        }
    #endregion

        private void txtToCSID_LostFocus(object sender, RoutedEventArgs e)
        {
            txtToCSId_Verify();
        }

        private void txtToService_LostFocus(object sender, RoutedEventArgs e)
        {
             if (cmbBCFType.IsEnabled == false)
                return;



             if ((txtToService.Text == "") || (txtToService.Text == "0"))
                 return;
             else
             {
                 cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");
                 
                 BCFVerification.Parms.ClearParms();
                 BCFVerification.Parms.AddParm("@service_id",  txtToService.Text);
                 BCFVerification.LoadTable("getmca");
                 if (BCFVerification.ObjectData.Tables["getmca"].Rows.Count > 0)
                 {
                    txtToMCA.Text = BCFVerification.ObjectData.Tables["getmca"].Rows[0]["mca_address"].ToString();
                      
                 }
                 else
                 {
                     txtToMCA.Text = "0";
                 }
             }
        }
    }
}
