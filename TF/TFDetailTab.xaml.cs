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

namespace TF
{

    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class TFDetailTab : ScreenBase
    {
        public string InvoiceNumber { get; set; }
        public string wfStatus;
        public int TFStatus;
        public string generalCSid;

        public TFDetailTab()
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
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty Detail ");
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "TFFolder";

            //Contract Location Grid
            gTFDetails.MainTableName = "detail";
            gTFDetails.ConfigFileName = "TFDetailGrid";

            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                gTFDetails.ContextMenuAddIsVisible = false;
                gTFDetails.ContextMenuRemoveIsVisible = false;
                gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                //btnAutoCreate.IsEnabled = false;
            }
            else
            {
                gTFDetails.ContextMenuAddIsVisible = true;
                //btnAutoCreate.IsEnabled = true;
            }
            //gTFDetails.ContextMenuAddDelegate = TFDetailsGridAddDelegate;
            //gTFDetails.ContextMenuAddDisplayName = "Add Credit Information Row";
            //gTFDetails.WindowZoomDelegate = InvoiceLookupDoubleClick;
            gTFDetails.ContextMenuGenericDelegate1 = TFDetailGridSelectAll;
            gTFDetails.ContextMenuGenericDisplayName1 = "Select All Invoice Information";
            gTFDetails.ContextMenuGenericIsVisible1 = true;
            gTFDetails.xGrid.FieldSettings.AllowEdit = true;
            //gTFDetails.ContextMenuAddDelegate = TFDetailsGridAddDelegate;
            //gTFDetails.ContextMenuAddDisplayName = "Add Credit Information Row";
            //gBCFDetails.WindowZoomDelegate = InvoiceLookupDoubleClick;
            gTFDetails.SetGridSelectionBehavior(false, true);
            gTFDetails.WindowZoomDelegate = GridDoubleClickDelegate;
            gTFDetails.ContextMenuRemoveDelegate = TFDetailGridDeleteDelegate;
            gTFDetails.ContextMenuRemoveDisplayName = "Delete Invoice Information";
            gTFDetails.FieldLayoutResourceString = "TFDetail";
            GridCollection.Add(gTFDetails);
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty Detail ");
        }

        public void GridDoubleClickDelegate()
        {
            if (gTFDetails.xGrid.ActiveRecord != null)
            {
                //call invoice folder
                if (gTFDetails.DoubleClickFieldName == "invoice_to_be_credited")
                {
                    gTFDetails.ReturnSelectedData("invoice_to_be_credited");
                    cGlobals.ReturnParms.Add("InvoiceZoom");
                    //cGlobals.ReturnParms.Add("GridAgingDetail.CustomerView");
                    RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                    //args.Source = GridAgingDetail.xGrid;
                    args.Source = gTFDetails.xGrid;
                    EventAggregator.GeneratedClickHandler(this, args);
                }
            }
        }

        public void InvoiceLookupDoubleClick()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {

                TFStatus = (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_status_flag"]));
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                //generalCSid = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["cs_id"].ToString();
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
                MessageBox.Show("You must first select and save locations to move.");
                //txtTFDesc.CntrlFocus();
                return;
            }

            if (TFStatus.ToString() == "1" || wfStatus[0] == 'A' || wfStatus[0] == 'I')
                return;

            if (gTFDetails.DoubleClickFieldName == "invoice_to_be_credited")
            {
                RazerBase.Lookups.BCFDetailGridInvoiceLookup TFDetailGridInvoiceLookup = new RazerBase.Lookups.BCFDetailGridInvoiceLookup();
                //this.CurrentBusObj.Parms.ClearParms();

                cGlobals.ReturnParms.Clear();

                if (this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                    cGlobals.ReturnParms.Add(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"]);
                else
                    cGlobals.ReturnParms.Add(0);

                // gets the users response
                TFDetailGridInvoiceLookup.ShowDialog();
                if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0].ToString() != "0")
                {
                    gTFDetails.ActiveRecord.Cells["invoice_to_be_credited"].Value = cGlobals.ReturnParms[0].ToString();

                    // Clear the parms
                    cGlobals.ReturnParms.Clear();
                }
            }
        }

        public void TFDetailGridSelectAll()
        {
                foreach (DataRecord r in gTFDetails.xGrid.Records)
                {
                    if (r != null)
                        r.IsSelected = true;
                }
        }

        private void TFDetailsGridAddDelegate()
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
            gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = true;
            gTFDetails.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = gTFDetails.xGrid.RecordManager.CurrentAddRecord;
            string tf_number = "";
            //Set the default values for the columns
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                tf_number = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
            }
            else
            {
                Messages.ShowWarning("No TF Number set up to add detail to.  TF General tab must first be saved");
                return;
            }
            row.Cells["tf_number"].Value = tf_number;
            ////need to set it to report_id selected in gReports
            row.Cells["invoice_to_be_credited"].Value = "";
            row.Cells["first_billing_period_effected"].Value = "01/01/1900";
            row.Cells["last_billing_period_effected"].Value = "01/01/1900";
            row.Cells["credit_amount"].Value = 0;
            row.Cells["document_id"].Value = "";
            row.Cells["rebill_flag"].Value = 0;
            //Commit the add new record - required to make this record active
            gTFDetails.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            gTFDetails.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            gTFDetails.xGrid.ActiveRecord = gTFDetails.xGrid.Records[0];
            //Set the field as active
            (gTFDetails.xGrid.Records[gTFDetails.ActiveRecord.Index] as DataRecord).Cells["invoice_to_be_credited"].IsActive = true;
            gTFDetails.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        private void txtLocationInvoice_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (this.CurrentBusObj.ObjectData.Tables["locations"] != null && this.CurrentBusObj.ObjectData.Tables["locations"].Rows.Count > 0)
            {
            }
            else
            {
                MessageBox.Show("You must first select and save locations to move.");
                //txtTFDesc.CntrlFocus();
                return;
            }

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            TFInvoiceLookup f = new TFInvoiceLookup();

            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();

            if (this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                cGlobals.ReturnParms.Add(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"]);
            else
                cGlobals.ReturnParms.Add(0);

            //f.Init(new cBaseBusObject("LocationInvoiceLookup"));
            f.Init(new cBaseBusObject("TFInvoiceLookup"));

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

        private void TFDetailGridDeleteDelegate()
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

            if (gTFDetails.xGrid.SelectedItems.Records.Count != 0)
            {
                gTFDetails.xGrid.RecordsDeleting += (sender, e) => e.DisplayPromptMessage = true;
                gTFDetails.xGrid.ExecuteCommand(DataPresenterCommands.DeleteSelectedDataRecords);
                gTFDetails.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
                return;

            }
            else
            {
                Messages.ShowInformation("No records were selected to delete.");
                return;
            }                           
        }

        private void RunAutoCreate(object sender, System.Windows.RoutedEventArgs e)
        {
            string tfNumber = "";
            bool newInvoice;
            bool InvoiceFound = false;
            if (txtLocationInvoice.Text == "" || txtLocationInvoice.Text == null)
            {
                MessageBox.Show("No invoices were selected.");
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
                    //if (!newList.Contains(invoice_number))
                    //    newList.Add(invoice_number);
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
                        cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");
                        //set tf number
                        tfNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                        TFVerification.Parms.ClearParms();
                        TFVerification.Parms.AddParm("@invoice_number", invoice_number);
                        //TFVerification.Parms.AddParm("@cs_id", this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["cs_id"].ToString());
                        TFVerification.Parms.AddParm("@tf_number", tfNumber);
                        TFVerification.LoadTable("invoice");
                        if (TFVerification.ObjectData.Tables["invoice"] == null || TFVerification.ObjectData.Tables["invoice"].Rows.Count > 0)
                            InvoiceFound = true;
                        //if (TFVerification.ObjectData.Tables["invoice"] == null || TFVerification.ObjectData.Tables["invoice"].Rows.Count < 1)
                        //{
                        //    Messages.ShowInformation("Invalid Invoice/Location entered. Location on General tab or Location tab must exist on the invoice selected. Save Location tab and try again.");
                        //    txtLocationInvoice.Text = "";

                        //}
                        //else
                        //{
                        //Need to add rows to the detail table based on the invoice datatable returned
                        foreach (DataRow r in TFVerification.ObjectData.Tables["invoice"].Rows)
                        {
                            gTFDetails.xGrid.FieldLayoutSettings.AllowAddNew = true;
                            DataRecord row = gTFDetails.xGrid.RecordManager.CurrentAddRecord;

                            row.Cells["tf_number"].Value = tfNumber;
                            row.Cells["invoice_to_be_credited"].Value = r["invoice_number"];
                            row.Cells["cs_id"].Value = r["cs_id"];
                            row.Cells["psa_city"].Value = r["psa_city"];
                            row.Cells["psa_state"].Value = r["psa_state"];
                            row.Cells["first_billing_period_effected"].Value = r["service_period_start"];
                            row.Cells["last_billing_period_effected"].Value = r["service_period_end"];
                            row.Cells["credit_amount"].Value = r["extended"];
                            row.Cells["product_code"].Value = r["product_code"];
                            row.Cells["document_id"].Value = "";
                            row.Cells["rebill_flag"].Value = 0;
                            row.Cells["inv_line_id"].Value = r["inv_line_id"];
                            //Commit the add new record - required to make this record active
                            gTFDetails.xGrid.RecordManager.CommitAddRecord();
                            ////Remove the add new record row
                            //gTFDetails.xGrid.FieldLayoutSettings.AllowAddNew = false;
                            ////Set the row just created to the active record
                            //gTFDetails.xGrid.ActiveRecord = gTFDetails.xGrid.Records[0];
                            ////Set the field as active
                            //(gTFDetails.xGrid.Records[gTFDetails.ActiveRecord.Index] as DataRecord).Cells["invoice_to_be_credited"].IsActive = true;
                            //gTFDetails.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                        }
                    }
                    //else
                    //{
                    //    Messages.ShowInformation("Location ID must be entered on the General Tab before credit information can be auto created.");
                    //    return;
                    //}
                }

                    //if (gTFDetails.xGrid.Records.Count > 0)
                    if (InvoiceFound)
                    {
                        //Remove the add new record row
                        gTFDetails.xGrid.FieldLayoutSettings.AllowAddNew = false;
                        //Set the row just created to the active record
                        gTFDetails.xGrid.ActiveRecord = gTFDetails.xGrid.Records[0];
                        //Set the field as active
                        (gTFDetails.xGrid.Records[gTFDetails.ActiveRecord.Index] as DataRecord).Cells["invoice_to_be_credited"].IsActive = true;
                        gTFDetails.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                    }
                    else
                    {
                        MessageBox.Show("Invoice not found.");
                        return;
                    }

            }
        }
    }
}

