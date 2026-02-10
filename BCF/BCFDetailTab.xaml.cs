

#region using statements

using RazerBase;
using RazerInterface;
using RazerBase.Lookups;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;

using System.Windows;

#endregion

namespace BCF
{


    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class BCFDetailTab : ScreenBase
    {

        public string InvoiceNumber { get; set; }
        public string wfStatus;
        public int BCFStatus;
        public string generalCSid;

        public BCFDetailTab()
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
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "BCFFolder";

            //Contract Location Grid
            gBCFDetails.MainTableName = "detail";
            gBCFDetails.ConfigFileName = "BCFDetailGrid";


            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {

                gBCFDetails.ContextMenuAddIsVisible = false;
                gBCFDetails.ContextMenuRemoveIsVisible = false;
                gBCFDetails.xGrid.FieldSettings.AllowEdit = false;
                btnAutoCreate.IsEnabled = false;

            }
            else
            {

                gBCFDetails.ContextMenuAddIsVisible = true;
                btnAutoCreate.IsEnabled = true;
            }
            gBCFDetails.ContextMenuAddDelegate = BCFDetailsGridAddDelegate;
            gBCFDetails.ContextMenuAddDisplayName = "Add Credit Information Row";
            //gBCFDetails.WindowZoomDelegate = InvoiceLookupDoubleClick;
            gBCFDetails.xGrid.FieldSettings.AllowEdit = true;
            //RES 9/24/15 Allow multi-row delete
            //gBCFDetails.SetGridSelectionBehavior(true, false);
            gBCFDetails.SetGridSelectionBehavior(true, true);

            gBCFDetails.WindowZoomDelegate = GridDoubleClickDelegate;
            gBCFDetails.ContextMenuRemoveDelegate = BCFDetailGridDeleteDelegate;
            gBCFDetails.ContextMenuRemoveDisplayName = "Delete Invoice Information";


            gBCFDetails.FieldLayoutResourceString = "BCFDetail";

            GridCollection.Add(gBCFDetails);


        }

        public void GridDoubleClickDelegate()
        {
            if (gBCFDetails.xGrid.ActiveRecord != null)
            {
                //call invoice folder
                if (gBCFDetails.DoubleClickFieldName == "invoice_to_be_credited")
                {
                    gBCFDetails.ReturnSelectedData("invoice_to_be_credited");
                    cGlobals.ReturnParms.Add("InvoiceZoom");
                    //cGlobals.ReturnParms.Add("GridAgingDetail.CustomerView");
                    RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                    //args.Source = GridAgingDetail.xGrid;
                    args.Source = gBCFDetails.xGrid;
                    EventAggregator.GeneratedClickHandler(this, args);
                }
            }
        }

        public void InvoiceLookupDoubleClick()
        {

            if (this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {

                BCFStatus = (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]));
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                generalCSid = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["cs_id"].ToString();
            }
            else
            {
                return;
            }

            if (this.CurrentBusObj.ObjectData.Tables["locations"] != null && this.CurrentBusObj.ObjectData.Tables["locations"].Rows.Count > 0)
            {
            }
            else
            {
                if (generalCSid == "0")
                    return;
            }










            if (BCFStatus.ToString() == "1" || wfStatus[0] == 'A' || wfStatus[0] == 'I')
                return;









            if (gBCFDetails.DoubleClickFieldName == "invoice_to_be_credited")
            {
                RazerBase.Lookups.BCFDetailGridInvoiceLookup BCFDetailGridInvoiceLookup = new RazerBase.Lookups.BCFDetailGridInvoiceLookup();
                //this.CurrentBusObj.Parms.ClearParms();




                cGlobals.ReturnParms.Clear();

                if (this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                    cGlobals.ReturnParms.Add(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"]);
                else
                    cGlobals.ReturnParms.Add(0);



                // gets the users response
                BCFDetailGridInvoiceLookup.ShowDialog();
                if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString() != "0")
                {
                    gBCFDetails.ActiveRecord.Cells["invoice_to_be_credited"].Value = cGlobals.ReturnParms[0].ToString();


                    // Clear the parms
                    cGlobals.ReturnParms.Clear();


                }
            }

        }

        public void BCFdetailClearGrid()
        {

            this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Clear();
        }

        private void BCFDetailsGridAddDelegate()
        {
            string wfStatus = "";
            if (CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                if (wfStatus[0] == 'A')
                    return;
            }
            else
                return;
            gBCFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = true;
            gBCFDetails.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = gBCFDetails.xGrid.RecordManager.CurrentAddRecord;
            string bcf_number = "";
            //Set the default values for the columns
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                bcf_number = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"].ToString();
            }
            else
            {
                Messages.ShowWarning("No BCF Number set up to add detail to.  BCF General tab must first be saved");
                return;
            }
            row.Cells["BCF_number"].Value = bcf_number;
            ////need to set it to report_id selected in gReports
            row.Cells["invoice_to_be_credited"].Value = "";
            row.Cells["first_billing_period_effected"].Value = "01/01/1900";
            row.Cells["last_billing_period_effected"].Value = "01/01/1900";
            row.Cells["credit_amount"].Value = 0;
            row.Cells["document_id"].Value = "";
            row.Cells["rebill_flag"].Value = 0;
            row.Cells["inv_line_id"].Value = 0;
            //Added product item and line desc.
            row.Cells["item_description"].Value = "";
            row.Cells["line_desc"].Value = "";
            //Commit the add new record - required to make this record active
            gBCFDetails.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            gBCFDetails.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            gBCFDetails.xGrid.ActiveRecord = gBCFDetails.xGrid.Records[0];
            //Set the field as active
            (gBCFDetails.xGrid.Records[gBCFDetails.ActiveRecord.Index] as DataRecord).Cells["invoice_to_be_credited"].IsActive = true;

            gBCFDetails.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);

        }

        private void txtLocationInvoice_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            LocationInvoiceLookup f = new LocationInvoiceLookup();

            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();

            if (this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                cGlobals.ReturnParms.Add(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"]);
            else
                cGlobals.ReturnParms.Add(0);

            f.Init(new cBaseBusObject("LocationInvoiceLookup"));


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

                txtLocationInvoice.Text = cGlobals.ReturnParms[0].ToString();

                InvoiceNumber = txtLocationInvoice.Text.ToString();

                // Clear the parms
                cGlobals.ReturnParms.Clear();
                //General.Focus();

            }
        }

        private void BCFDetailGridDeleteDelegate()
        {
            string wfStatus = "";
            if (CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                if (wfStatus[0] == 'A')
                    return;
            }
            else
                return;

            //RES 9/24/15 Allow multi-row delete
            if (gBCFDetails.xGrid.SelectedItems.Records.Count != 0)
            {
                gBCFDetails.xGrid.RecordsDeleting += (sender, e) => e.DisplayPromptMessage = true;
                gBCFDetails.xGrid.ExecuteCommand(DataPresenterCommands.DeleteSelectedDataRecords);
                gBCFDetails.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                return;

            }
            else
            {
                Messages.ShowInformation("No records were selected to delete.");
                return;
            }

            //MessageBoxResult result = Messages.ShowYesNo("Detail record will be deleted from this BCF. Once deleted, to make the changes to the database, you will need to do a Save. Are you sure you want to delete?",
            //     System.Windows.MessageBoxImage.Question);
            //if (result == MessageBoxResult.Yes)
            //{
            //    DataRecord r = gBCFDetails.ActiveRecord;
            //    if (r != null)
            //    {
            //        DataRow row = (r.DataItem as DataRowView).Row;
            //        if (row != null)
            //        {

            //            row.Delete();
            //            // Clear the parms
            //            cGlobals.ReturnParms.Clear();
            //        }
            //    }
            //}
        }



        private void RunAutoCreate(object sender, System.Windows.RoutedEventArgs e)
        {
            string bcfNumber = "";
            bool newInvoice;
            if (txtLocationInvoice.Text == "")
            {
                return;
            }
            else
            {   // To parse your string 
                string invoice_string = txtLocationInvoice.Text.ToString();
                var elements = invoice_string.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);

                //9/24/15 RES remove duplicate invoice numbers
                System.Collections.ArrayList newList = new System.Collections.ArrayList();
                foreach (string invoice_number in elements)
                {
                    if (!newList.Contains(invoice_number))
                    {
                        newInvoice = true;
                        foreach (DataRow r in CurrentBusObj.ObjectData.Tables["detail"].Rows)
                        {
                            if (r["invoice_to_be_credited"].ToString() == invoice_number)
                            {
                                newInvoice = false;
                                break;
                            }
                        }
                        if (newInvoice) newList.Add(invoice_number);
                    }
                }

                // To Loop through
                //foreach (string invoice_number in elements)
                foreach (string invoice_number in newList)
                {


                    //create lines for invoice detail - go to invoice 
                    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                    {
                        cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");
                        //set bcf number
                        bcfNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"].ToString();
                        BCFVerification.Parms.ClearParms();
                        BCFVerification.Parms.AddParm("@invoice_number", invoice_number);
                        BCFVerification.Parms.AddParm("@cs_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["cs_id"].ToString());
                        BCFVerification.Parms.AddParm("@BCF_number", bcfNumber);
                        BCFVerification.LoadTable("invoice");
                        //if (BCFVerification.ObjectData.Tables["invoice"] == null || BCFVerification.ObjectData.Tables["invoice"].Rows.Count < 1)
                        //{
                        //    Messages.ShowInformation("Invalid Invoice/Location entered. Location on General tab or Location tab must exist on the invoice selected. Save Location tab and try again.");
                        //    txtLocationInvoice.Text = "";

                        //}
                        //else
                        //{
                        //Need to add rows to the detail table based on the invoice datatable returned
                        foreach (DataRow r in BCFVerification.ObjectData.Tables["invoice"].Rows)
                        {



                            gBCFDetails.xGrid.FieldLayoutSettings.AllowAddNew = true;
                            DataRecord row = gBCFDetails.xGrid.RecordManager.CurrentAddRecord;

                            row.Cells["BCF_number"].Value = bcfNumber;
                            row.Cells["invoice_to_be_credited"].Value = r["invoice_number"];
                            row.Cells["first_billing_period_effected"].Value = r["service_period_start"];
                            row.Cells["last_billing_period_effected"].Value = r["service_period_end"];
                            row.Cells["credit_amount"].Value = r["extended"];
                            row.Cells["document_id"].Value = "";
                            row.Cells["rebill_flag"].Value = 0;
                            //Added product item and line description.
                            row.Cells["inv_line_id"].Value = r["inv_line_id"];
                            row.Cells["item_description"].Value = r["item_description"];
                            row.Cells["line_desc"].Value = r["line_desc"];
                            //Commit the add new record - required to make this record active
                            gBCFDetails.xGrid.RecordManager.CommitAddRecord();
                            ////Remove the add new record row
                            //gBCFDetails.xGrid.FieldLayoutSettings.AllowAddNew = false;
                            ////Set the row just created to the active record
                            //gBCFDetails.xGrid.ActiveRecord = gBCFDetails.xGrid.Records[0];
                            ////Set the field as active
                            //(gBCFDetails.xGrid.Records[gBCFDetails.ActiveRecord.Index] as DataRecord).Cells["invoice_to_be_credited"].IsActive = true;

                            //gBCFDetails.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);

                        }

                    }




                    else
                    {
                        Messages.ShowInformation("Location ID must be entered on the General Tab before credit information can be auto created.");
                        return;
                    }
                }

                //Remove the add new record row
                gBCFDetails.xGrid.FieldLayoutSettings.AllowAddNew = false;
                //Set the row just created to the active record
                gBCFDetails.xGrid.ActiveRecord = gBCFDetails.xGrid.Records[0];
                //Set the field as active
                (gBCFDetails.xGrid.Records[gBCFDetails.ActiveRecord.Index] as DataRecord).Cells["invoice_to_be_credited"].IsActive = true;

                gBCFDetails.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);

            }
        }


    }


}
