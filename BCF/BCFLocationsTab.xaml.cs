

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

namespace BCF
{

 
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class BCFLocationsTab : ScreenBase, IPreBindable
    {

        //public string InvoiceNumber { get; set; }
        public string wfStatus;
        public int BCFStatus;
        public ComboBoxItemsProvider cmbServiceStatusPNI { get; set; }
        public ComboBoxItemsProvider cmbHeadendTypePNI { get; set; } 

       
       
        public BCFLocationsTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }

        public void PreBind()
        {

            //Service Status dddw in pni grid
            ComboBoxItemsProvider ip3 = new ComboBoxItemsProvider();
            ip3 = new ComboBoxItemsProvider();
            ip3.ItemsSource = CurrentBusObj.ObjectData.Tables["dddwService"].DefaultView;
            ip3.ValuePath = "service_status";
            ip3.DisplayMemberPath = ("status_desc");
            cmbServiceStatusPNI = ip3;
            //Headend dddw in PNI grid
            ComboBoxItemsProvider ip4 = new ComboBoxItemsProvider();
            ip4 = new ComboBoxItemsProvider();
            ip4.ItemsSource = CurrentBusObj.ObjectData.Tables["dddwHeadend"].DefaultView;
            ip4.ValuePath = "head_end_type";
            ip4.DisplayMemberPath = ("head_end_type_desc");
            cmbHeadendTypePNI = ip4;


            
            




        }

       
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "BCFFolder";
          
            //Contract Location Grid
            gBCFLocations.MainTableName = "locations";
            gBCFLocationsPNI.MainTableName = "locations_pni";
            gBCFLocations.ConfigFileName = "BCFLocationsGrid";
            gBCFLocations.ConfigFileName = "BCFLocationsPNIGrid";
            
          
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
                     
            {

                gBCFLocations.ContextMenuAddIsVisible = false;
                gBCFLocations.ContextMenuRemoveIsVisible = false;
                gBCFLocations.xGrid.FieldSettings.AllowEdit = false;
                gBCFLocationsPNI.ContextMenuAddIsVisible = false;
                gBCFLocationsPNI.ContextMenuRemoveIsVisible = false;
                gBCFLocationsPNI.xGrid.FieldSettings.AllowEdit = false;
                btnAutoCreate.IsEnabled = false;
                 
            }
            else
            {

                //gBCFLocations.ContextMenuAddIsVisible = true;
                btnAutoCreate.IsEnabled = true;
            }
            gBCFLocationsPNI.ContextMenuAddDelegate = BCFLocationsPNIGridAddDelegate;
            gBCFLocationsPNI.ContextMenuAddDisplayName = "Add PNI Location Row";
            gBCFLocationsPNI.ContextMenuRemoveDelegate = BCFLocationsPNIGridDeleteDelegate;
            gBCFLocationsPNI.ContextMenuRemoveDisplayName = "Delete PNI Information";
            
            gBCFLocations.ContextMenuRemoveDelegate = BCFLocationsGridDeleteDelegate;
            gBCFLocations.ContextMenuRemoveDisplayName = "Delete Location Information";

            //gBCFLocations.WindowZoomDelegate = LocationsLookupDoubleClick;
            gBCFLocationsPNI.xGrid.FieldSettings.AllowEdit = true;

            gBCFLocations.SetGridSelectionBehavior(true, false);
            gBCFLocationsPNI.SetGridSelectionBehavior(true, false);


            gBCFLocations.FieldLayoutResourceString = "BCFLocations";
            gBCFLocationsPNI.FieldLayoutResourceString = "BCFLocationsPNI";

            GridCollection.Add(gBCFLocations);
            GridCollection.Add(gBCFLocationsPNI);

        
            
        }

        public void LocationsLookupDoubleClick()
        {
            
            //BCFStatus = (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]));
            //wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
            //if (BCFStatus.ToString() == "1" || wfStatus[0] == 'A')
            //    return;
            //if (gBCFLocations.DoubleClickFieldName == "locations_to_be_added")
            //{
            //    RazerBase.Lookups.BCFLocationsLookup BCFLocationsLookup = new RazerBase.Lookups.BCFLocationsLookup();
            //    //this.CurrentBusObj.Parms.ClearParms();



            //    //this.CurrentBusObj.Parms.ClearParms();
            //    cGlobals.ReturnParms.Clear();

            //    // gets the users response
            //    BCFLocationsLookup.ShowDialog();
            //    if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString() != "0")
            //    {
            //        gBCFLocations.ActiveRecord.Cells["locations_to_be_added"].Value = cGlobals.ReturnParms[0].ToString();
                   

            //        // Clear the parms
            //        cGlobals.ReturnParms.Clear();


            //    }
            //}
             
        }


        private void BCFLocationsPNIGridDeleteDelegate()
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
                DataRecord r = gBCFLocationsPNI.ActiveRecord;
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

        private void BCFLocationsGridDeleteDelegate()
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
            
            MessageBoxResult result = Messages.ShowYesNo("Location will be deleted from this BCF. Once deleted, to make the changes to the database, you will need to do a Save. Are you sure you want to delete?",
                 System.Windows.MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataRecord r = gBCFLocations.ActiveRecord;
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

        public void BCFlocationsClearGrid()
        {

            this.CurrentBusObj.ObjectData.Tables["locations"].Rows.Clear();
        }
        private void BCFLocationsPNIGridAddDelegate()
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
            
            gBCFLocationsPNI.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = gBCFLocationsPNI.xGrid.RecordManager.CurrentAddRecord;
            string bcf_number = "";
            int mso_id = 0;
            //Set the default values for the columns
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                bcf_number = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"].ToString();
                mso_id =  Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["mso_id"].ToString());

            }


            row.Cells["BCF_number"].Value = bcf_number;
            ////need to set it to report_id selected in gReports
            row.Cells["mso_id"].Value = mso_id;
            row.Cells["cs_id"].Value = 0;
            row.Cells["head_id"].Value = 0;
            row.Cells["head_end_type"].Value = 2;
            row.Cells["service_id"].Value = 0;
            row.Cells["service_status"].Value = 1;
            row.Cells["fs_id"].Value = 0;
            row.Cells["mca_address"].Value = 0;



            //Commit the add new record - required to make this record active
            gBCFLocationsPNI.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            gBCFLocationsPNI.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            gBCFLocationsPNI.xGrid.ActiveRecord = gBCFLocationsPNI.xGrid.Records[0];
            //Set the field as active
            (gBCFLocationsPNI.xGrid.Records[gBCFLocationsPNI.ActiveRecord.Index] as DataRecord).Cells["cs_id"].IsActive = true;

            gBCFLocationsPNI.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }


        private void BCFLocationsGridAddDelegate()
        {
           // gBCFLocations.xGrid.FieldLayoutSettings.AllowAddNew = true;
           // DataRecord row = gBCFLocations.xGrid.RecordManager.CurrentAddRecord;
           // string bcf_number = "";
           // string contract_id = "";
           // //Set the default values for the columns
           //// if the object data was loaded
           //     if (this.CurrentBusObj.HasObjectData)
           //     {
           //        bcf_number = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"].ToString();
           //        contract_id = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"].ToString(); 
           //     }
           //     else
           //     {
           //         Messages.ShowWarning("No BCF Number set up to add locations.  BCF General tab must first be saved");
           //     return;
           //     }
           // row.Cells["BCF_number"].Value = bcf_number;
           // ////need to set it to report_id selected in gReports
           // row.Cells["invoice_to_be_credited"].Value = "";
           // row.Cells["first_billing_period_effected"].Value = "01/01/1900";
           // row.Cells["last_billing_period_effected"].Value = "01/01/1900";
           // row.Cells["credit_amount"].Value = 0;
           // row.Cells["document_id"].Value = "";
           // row.Cells["rebill_flag"].Value = 0;
           // //Commit the add new record - required to make this record active
           // gBCFLocations.xGrid.RecordManager.CommitAddRecord();
           // //Remove the add new record row
           // gBCFLocations.xGrid.FieldLayoutSettings.AllowAddNew = false;
           // //Set the row just created to the active record
           // gBCFLocations.xGrid.ActiveRecord = gBCFLocations.xGrid.Records[0];
           // //Set the field as active
           // (gBCFLocations.xGrid.Records[gBCFLocations.ActiveRecord.Index] as DataRecord).Cells["invoice_to_be_credited"].IsActive = true;

           // gBCFLocations.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        private void txtLocation_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            BCFLocationsLookup f = new BCFLocationsLookup();

            if (CurrentBusObj == null)
            {
                return;
            }


         
            if (this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {
                cGlobals.ReturnParms.Add(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"]);
                cGlobals.ReturnParms.Add(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"]);
            }
            else
            {
                cGlobals.ReturnParms.Add(0);
            }

           

            f.Init(new cBaseBusObject("BCFLocationsLookup"));


            // gets the users response
            f.ShowDialog();

            //// Check if a value is returned
            //if (cGlobals.ReturnParms.Count > 0)
            //{
            //    //DWR-Added 1/15/13 to replace previous retreival code
            //    txtBCFNumber.Text = cGlobals.ReturnParms[0].ToString();
            //    cGlobals.ReturnParms.Clear();
            //    if (txtBCFNumber.Text != BCFNumber)
            //        ReturnData(txtBCFNumber.Text, "@BCF_number");
            //}


            ////Event handles opening of the lookup window upon double click on Entity ID field
            //InvoiceFolderLookup f = new InvoiceFolderLookup();

            //this.CurrentBusObj.Parms.ClearParms();

            //// gets the users response
            //f.ShowDialog();

            // Check if a value is returned
            //DWR - Modifie 3/11/13 - Added 2nd part of if check as screen would crash app if filter and then cancel was clicked in the lookup.
            if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0] != null)
            {

                //load current parms

                txtLocation.Text = cGlobals.ReturnParms[0].ToString();

                //InvoiceNumber = txtLocation.Text.ToString();

                // Clear the parms
                cGlobals.ReturnParms.Clear();
                //General.Focus();

            }
        }



        private void RunAutoCreate(object sender, System.Windows.RoutedEventArgs e)
        {
            string bcfNumber = "";
            bool canConvert;
            int i = 0;
            int bcfType = 0;
            if (txtLocation.Text == "" || (txtLocation.Text == null))
            {
                return;
            }
            else
            {   // To parse your string 
             
                    
                    string location_string = txtLocation.Text.ToString();
                    txtLocation.Text = "";

             
                


                var elements = location_string.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);

                // To Loop through
                foreach (string cs_id in elements)
                {

                    canConvert = int.TryParse(cs_id, out i);
                    if (canConvert == false)
                    {
                        Messages.ShowInformation("Location ID " + cs_id + " is invalid.");
                        continue;
                    }



                    //create lines for location detail - go to invoice 
                    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                    {
                        cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");
                        //set bcf number
                        bcfNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"].ToString();
                        bcfType = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_type"].ToString());

                        BCFVerification.Parms.ClearParms();
                        BCFVerification.Parms.AddParm("@contract_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_id"].ToString());
                        BCFVerification.Parms.AddParm("@product_code", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString());
                        BCFVerification.Parms.AddParm("@cs_id", cs_id);
                        BCFVerification.LoadTable("locations");
                        BCFVerification.LoadTable("getPNI");
                        if (BCFVerification.ObjectData.Tables["locations"] == null || BCFVerification.ObjectData.Tables["locations"].Rows.Count < 1)
                        {
                            Messages.ShowInformation("Invalid location id entered.  Please select a valid contract/product on the General Tab");
                            txtLocation.Text = "";

                        }
                        else
                        {
                            
                            //Need to add rows to the location table based on the location datatable returned
                            foreach (DataRow r in BCFVerification.ObjectData.Tables["locations"].Rows)
                            {



                                gBCFLocations.xGrid.FieldLayoutSettings.AllowAddNew = true;
                                DataRecord row = gBCFLocations.xGrid.RecordManager.CurrentAddRecord;

                                row.Cells["BCF_number"].Value = bcfNumber;
                                row.Cells["cs_id"].Value = r["cs_id"];
                                row.Cells["product_code"].Value = r["product_code"];
                                row.Cells["account_name"].Value = r["account_name"];
                                row.Cells["psa_city"].Value = r["psa_city"];
                                row.Cells["psa_state"].Value = r["psa_state"];
                                row.Cells["psa_country"].Value = r["psa_country"];
                                row.Cells["owner_entity_id"].Value = r["owner_entity_id"];
                                row.Cells["entity_name"].Value = r["entity_name"];
                                row.Cells["receivable_account"].Value = r["receivable_account"];
                                row.Cells["hold_flag"].Value = r["hold_flag"];
                                row.Cells["last_bill_date"].Value = r["last_bill_date"];
                                row.Cells["turn_off_date"].Value = r["turn_off_date"];
                                row.Cells["bill_mso_id"].Value = r["bill_mso_id"];
                                row.Cells["country_id"].Value = r["country_id"];
                                //Commit the add new record - required to make this record active
                                gBCFLocations.xGrid.RecordManager.CommitAddRecord();
                                //Remove the add new record row
                                gBCFLocations.xGrid.FieldLayoutSettings.AllowAddNew = false;
                                //Set the row just created to the active record
                                gBCFLocations.xGrid.ActiveRecord = gBCFLocations.xGrid.Records[0];
                                //Set the field as active
                                (gBCFLocations.xGrid.Records[gBCFLocations.ActiveRecord.Index] as DataRecord).Cells["cs_id"].IsActive = true;

                                gBCFLocations.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);

                            }

                            if (gBCFLocations.xGrid.Records.Count > 1)
                            {
                                BCFFolder c;
                                c = findRootScreenBase(this) as BCFFolder;
                                c.SetMultiLocation();
                            }

                            //Need to add rows to the PNI table based on the PNI codes returned for each location
                            //for Inactivation, Transfer, Reactivation and Consolidation, Auto Load PNI
                            if ((bcfType == 1) || (bcfType == 2) || (bcfType == 5) || (bcfType == 6))
                            {
                                foreach (DataRow r in BCFVerification.ObjectData.Tables["getPNI"].Rows)
                                {



                                    gBCFLocationsPNI.xGrid.FieldLayoutSettings.AllowAddNew = true;
                                    DataRecord row = gBCFLocationsPNI.xGrid.RecordManager.CurrentAddRecord;

                                    row.Cells["BCF_number"].Value = bcfNumber;
                                    row.Cells["cs_id"].Value = cs_id;
                                    row.Cells["mso_id"].Value = r["owner_mso"];
                                    row.Cells["head_id"].Value = r["head_id"];
                                    row.Cells["head_end_type"].Value = r["head_end_type"];
                                    row.Cells["fs_id"].Value = r["file_server_id"];
                                    row.Cells["service_id"].Value = r["service_id"];
                                    row.Cells["service_status"].Value = r["service_status"];
                                    row.Cells["mca_address"].Value = r["mca_address"];

                                    //Commit the add new record - required to make this record active
                                    gBCFLocationsPNI.xGrid.RecordManager.CommitAddRecord();
                                    //Remove the add new record row
                                    gBCFLocationsPNI.xGrid.FieldLayoutSettings.AllowAddNew = false;
                                    //Set the row just created to the active record
                                    gBCFLocationsPNI.xGrid.ActiveRecord = gBCFLocationsPNI.xGrid.Records[0];
                                    //Set the field as active
                                    (gBCFLocationsPNI.xGrid.Records[gBCFLocationsPNI.ActiveRecord.Index] as DataRecord).Cells["cs_id"].IsActive = true;

                                    gBCFLocationsPNI.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);

                                }
                            }
                            



                        }
                    }




                    else
                    {
                        Messages.ShowInformation("Contract ID/Product must be entered on the General Tab before location information can be auto created.");
                        return;
                    }
                }

            }
        }
       

       

    }
  

}
