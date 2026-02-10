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
    public partial class TFCreditTab : ScreenBase
    {
        public string InvoiceNumber { get; set; }
        public string wfStatus;
        public int TFStatus;
        public string generalCSid;
        public bool CreditDirty;
        //public ComboBoxItemsProvider cmbCustomer { get; set; }

        public TFCreditTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }

        public void PreBind()
        {
            //Customer dddw in credit grid
            //ComboBoxItemsProvider ip3 = new ComboBoxItemsProvider();
            //ip3 = new ComboBoxItemsProvider();
            //ip3.ItemsSource = CurrentBusObj.ObjectData.Tables["dddwCustomer"].DefaultView;
            //ip3.ValuePath = "receivable_account";
            //ip3.DisplayMemberPath = ("account_name");
            //cmbCustomer = ip3;
          
        }

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty Credit ");
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "TFFolder";

            //Contract Location Grid
            gTFCredit.MainTableName = "credit";
            gTFCredit.ConfigFileName = "TFCreditGrid";
            CreditDirty = false;

            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                gTFCredit.ContextMenuAddIsVisible = false;
                gTFCredit.ContextMenuRemoveIsVisible = false;
                gTFCredit.xGrid.FieldSettings.AllowEdit = false;
                //btnAutoCreate.IsEnabled = false;
            }
            else
            {
                gTFCredit.ContextMenuAddIsVisible = true;
                //btnAutoCreate.IsEnabled = true;
            }
            gTFCredit.xGrid.FieldSettings.AllowEdit = true;
            //gTFCredit.set
            gTFCredit.SetGridSelectionBehavior(false, true);
            //gTFCredit.SkipReadOnlyCellsOnTab = true;
            //gTFLocations.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;
            gTFCredit.CellUpdatedDelegate = gTFCredit_CellUpdated;
            gTFCredit.WindowZoomDelegate = GridDoubleClickDelegate;
            //gTFCredit.RecordActivatedDelegate = gTFLocations_RecordActivated;
            //gTFLocations.EditModeEndedDelegate = gTFLocations_EditModeEnded;
            gTFCredit.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(gTFCredit_EditModeEnded);
            //gTFCredit.ContextMenuGenericDelegate1 = TFCopyCellDelegate;
            //gTFCredit.ContextMenuGenericDisplayName1 = "Copy Cell Value to All Rows";
            //gTFCredit.ContextMenuGenericIsVisible1 = true;
            gTFCredit.FieldLayoutResourceString = "TFCredit";
            GridCollection.Add(gTFCredit);
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty Credit ");
        }

        public void GridDoubleClickDelegate()
        {
            if (gTFCredit.xGrid.ActiveRecord != null)
            {
                if (gTFCredit.DoubleClickFieldName == "old_contract_id")
                {
                    //call contract folder
                    System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
                    gTFCredit.ReturnSelectedData("old_contract_id");
                    cGlobals.ReturnParms.Add("GridLocationContracts.xGrid");
                    RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                    args.Source = gTFCredit.xGrid;
                    EventAggregator.GeneratedClickHandler(this, args);
                }
                if (gTFCredit.DoubleClickFieldName == "new_contract_id")
                {
                    System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
                    gTFCredit.ReturnSelectedData("new_contract_id");
                    cGlobals.ReturnParms.Add("GridContracts.xGrid");
                    //cGlobals.ReturnParms.Add("GridAgingDetail.CustomerView");
                    RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                    //args.Source = GridAgingDetail.xGrid;
                    args.Source = gTFCredit.xGrid;
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

            if (gTFCredit.DoubleClickFieldName == "invoice_to_be_credited")
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
                    gTFCredit.ActiveRecord.Cells["invoice_to_be_credited"].Value = cGlobals.ReturnParms[0].ToString();

                    // Clear the parms
                    cGlobals.ReturnParms.Clear();
                }
            }
        }

        public void TFDetailClearGrid()
        {
            this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Clear();
        }

        private void TFCreditGridAddDelegate()
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
            gTFCredit.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = gTFCredit.xGrid.RecordManager.CurrentAddRecord;
            string tf_number = "";
            //Set the default values for the columns
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                tf_number = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
            }
            else
            {
                Messages.ShowWarning("No BCF Number set up to add detail to.  TF General tab must first be saved");
                return;
            }
            row.Cells["TF_number"].Value = tf_number;
            ////need to set it to report_id selected in gReports
            row.Cells["invoice_to_be_credited"].Value = "";
            row.Cells["first_billing_period_effected"].Value = "01/01/1900";
            row.Cells["last_billing_period_effected"].Value = "01/01/1900";
            row.Cells["credit_amount"].Value = 0;
            row.Cells["document_id"].Value = "";
            row.Cells["rebill_flag"].Value = 0;
            //Commit the add new record - required to make this record active
            gTFCredit.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            gTFCredit.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            gTFCredit.xGrid.ActiveRecord = gTFCredit.xGrid.Records[0];
            //Set the field as active
            (gTFCredit.xGrid.Records[gTFCredit.ActiveRecord.Index] as DataRecord).Cells["invoice_to_be_credited"].IsActive = true;
            gTFCredit.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
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
            //if (cGlobals.ReturnParms.Count > 0 && cGlobals.ReturnParms[0] != null)
            //{
            //    //load current parms

            //    txtLocationInvoice.Text = cGlobals.ReturnParms[0].ToString();
            //    InvoiceNumber = txtLocationInvoice.Text.ToString();

            //    // Clear the parms
            //    cGlobals.ReturnParms.Clear();
            //    //General.Focus();
            //}
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
            MessageBoxResult result = Messages.ShowYesNo("Detail record will be deleted from this TF. Once deleted, to make the changes to the database, you will need to do a Save. Are you sure you want to delete?",
                 System.Windows.MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DataRecord r = gTFCredit.ActiveRecord;
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

        private void gTFCredit_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
        {
            ForceScreenDirty = true;
            CreditDirty = true;
            //Messages.ShowInformation("contact_information_updated updated.");
            //if (e.Cell.Field.Name == "contact_information_updated")
            //{
            //    if (gTFCredit.xGrid.SelectedItems.Records.Count > 1)
            //    {
            //        foreach (DataRecord r in gTFCredit.xGrid.Records)
            //        {
            //            if (r.IsSelected == true)
            //                r.Cells["contact_information_updated"].Value = e.Cell.Value;
            //        }
            //    }
            //    //Messages.ShowInformation("contact_information_updated updated.");
            //    //DataRecord GridRecord = null;
            //    //GridRecord = e.Cell.Record;
            //    //GridRecord.IsSelected = true;
            //    //if (GridRecord != null)
            //    //{
            //    //    //if (GridRecord.IsSelected == false)
            //    //    //    GridRecord.IsSelected = true;
            //    //    if (GridRecord.Cells["move"].Value.ToString() == "1")
            //    //    {

            //    //    }
            //    //    else
            //    //    {
            //    //        //foreach (DataRecord r in gTFLocationsPNI.xGrid.Records)
            //    //        //{
            //    //        //    if ((r.Cells["move"].Value.ToString() == "1") && (r.Cells["cs_id"].Value.ToString() == GridRecord.Cells["cs_id"].Value.ToString()))
            //    //        //        r.Cells["move"].Value = 0;
            //    //        //}
            //    //        //bUncheckLine = true;
            //    //        //chkSelectAll.IsChecked = 0;
            //    //    }
            //    //    gTFCredit.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToAllRecords);
            //    //    //btnSave.IsEnabled = true;
            //    //}
            //}
        }

        //private void TFCopyCellDelegate(object sender, Infragistics.Windows.DataPresenter.Events.CellActivatedEventArgs e)
        private void TFCopyCell(object sender, System.Windows.RoutedEventArgs e)
        {
            DataRecord r = gTFCredit.ActiveRecord;
            string check;

            if (gTFCredit.xGrid.ActiveCell.Field.Name == "contact_information_updated")
            {
                if (gTFCredit.xGrid.ActiveCell.Value.ToString() == "1")
                    check = "checked";
                else
                    check = "unchecked";
                MessageBoxResult result = Messages.ShowYesNo("This will cause the Invoice Delivery Updated column on all rows to be " + check + ". Are you sure you want to copy the data?",
                                          System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (DataRecord x in gTFCredit.xGrid.Records)
                    {
                            x.Cells["contact_information_updated"].Value = gTFCredit.xGrid.ActiveCell.Value;
                    }
                }
            }

            if (gTFCredit.xGrid.ActiveCell.Field.Name == "subscriber_counts_updated")
            {
                if (gTFCredit.xGrid.ActiveCell.Value.ToString() == "1")
                    check = "checked";
                else
                    check = "unchecked";
                MessageBoxResult result = Messages.ShowYesNo("This will cause the Subscriber Counts Updated column on all rows to be " + check + ". Are you sure you want to copy the data?",
                                          System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (DataRecord x in gTFCredit.xGrid.Records)
                    {
                        x.Cells["subscriber_counts_updated"].Value = gTFCredit.xGrid.ActiveCell.Value;
                    }
                }
            }

            if (gTFCredit.xGrid.ActiveCell.Field.Name == "new_contract_id")
            {
                MessageBoxResult result = Messages.ShowYesNo("This will cause the New Contract ID column on all rows to be set to " + 
                                          gTFCredit.xGrid.ActiveCell.Value.ToString() + ". Are you sure you want to copy the data?",
                                          System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (DataRecord x in gTFCredit.xGrid.Records)
                    {
                        x.Cells["new_contract_id"].Value = gTFCredit.xGrid.ActiveCell.Value;
                    }
                }
            }

            if (gTFCredit.xGrid.ActiveCell.Field.Name == "date_owner_paid_through")
            {
                MessageBoxResult result = Messages.ShowYesNo("This will cause the Last Date Paid column on all rows to be set to " +
                                          //gTFCredit.xGrid.ActiveCell.Value.ToString() + ". Are you sure you want to copy the data?",
                                          gTFCredit.xGrid.ActiveCell.Value.ToString().Replace(" 12:00:00 AM","") + ". Are you sure you want to copy the data?",
                                          System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (DataRecord x in gTFCredit.xGrid.Records)
                    {
                        x.Cells["date_owner_paid_through"].Value = gTFCredit.xGrid.ActiveCell.Value;
                    }
                }
            }

            if (gTFCredit.xGrid.ActiveCell.Field.Name == "date_to_begin_invoicing_new_owner")
            {
                MessageBoxResult result = Messages.ShowYesNo("This will cause the Date to Begin Invoicing column on all rows to be set to " +
                                          gTFCredit.xGrid.ActiveCell.Value.ToString().Replace(" 12:00:00 AM", "") + ". Are you sure you want to copy the data?",
                                          System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (DataRecord x in gTFCredit.xGrid.Records)
                    {
                        x.Cells["date_to_begin_invoicing_new_owner"].Value = gTFCredit.xGrid.ActiveCell.Value;
                    }
                }
            }

        }

        public void gTFCredit_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            DataRecord row = gTFCredit.ActiveRecord;

            //This code runs a query to verify that the document id is valid when attempting to leave the apply to doc field
            if (e.Cell.Field.Name == "new_contract_id")
            {
                CurrentBusObj.Parms.UpdateParmValue("@tf_credit_id", gTFCredit.ActiveRecord.Cells["tf_credit_id"].Value.ToString() ?? string.Empty);
                CurrentBusObj.Parms.UpdateParmValue("@new_contract_id", e.Cell.Value.ToString() ?? string.Empty);
                CurrentBusObj.LoadTable("tf_customer");
                if (CurrentBusObj.ObjectData.Tables["tf_customer"] != null && CurrentBusObj.ObjectData.Tables["tf_customer"].Rows.Count > 0)
                {
                    //@@Need to complete this code
                    if (CurrentBusObj.ObjectData.Tables["tf_customer"].Rows.Count > 1) //More than one row that can be applied to
                    {
                        gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = true;
                    }
                    else  //Single apply to row
                    {
                        gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                        gTFCredit.ActiveRecord.Cells["new_receivable_account"].Value = CurrentBusObj.ObjectData.Tables["tf_customer"].Rows[0]["receivable_account"];
                    }
                }
                else  //No data returned
                {
                    gTFCredit.ActiveRecord.Cells["new_receivable_account"].Value = "";
                }
            }
        }
     }

}

