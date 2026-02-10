

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Contact;



#endregion

namespace Contract
{

   
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class ContractBillingTab : ScreenBase, IPreBindable
     {

        public ComboBoxItemsProvider cmbCompanyGridCombo { get; set; }
        public ComboBoxItemsProvider cmbContractLocation { get; set; }
        public ComboBoxItemsProvider cmbContractMilestone { get; set; }
        public ComboBoxItemsProvider cmbProductItem { get; set; }
        public ComboBoxItemsProvider cmbSalesperson { get; set; }
        public ComboBoxItemsProvider cmbProductCode { get; set; }
        public ComboBoxItemsProvider cmbDealType { get; set; }
        public ComboBoxItemsProvider cmbAttribute { get; set; }

        public bool CLMFlag = true;

       
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public ContractBillingTab()
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
            CLMFlag = true;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "billing";

            ////Establist the Company Contract Grid
            //gCompany.MainTableName = "contract_company";
            //gCompany.ConfigFileName = "ContractCompanyGrid";
            //gCompany.SetGridSelectionBehavior(false, true );
            //gCompany.FieldLayoutResourceString = "ContractCompany";

            //This grid is editable and has a dropdown within the grid
            //A field layoutsettings file will establish that the grid can be added to and deleted from
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.AllowAddNew = true;
            f.AllowDelete = true;
            f.AddNewRecordLocation = AddNewRecordLocation.OnTop ;
            f.SelectionTypeField = SelectionType.Single ;
            f.HighlightAlternateRecords = true;

            //gCompany.xGrid.FieldLayoutSettings = f;
            ////Make the grid editable
            //gCompany.xGrid.FieldSettings.AllowEdit = true;

           
            gLCR.MainTableName = "legal_agreement";
            gLCR.ConfigFileName = "ContractLegalAgreementGrid";
            gLCR.FieldLayoutResourceString="ContractLegalAgreement";
            gLCR.xGrid.FieldLayoutSettings = f;
            //Make the grid editable
            gLCR.xGrid.FieldSettings.AllowEdit = true;
            gLCR.SetGridSelectionBehavior(false, false);

            gPurchaseOrder.MainTableName = "purchase_order";
            gPurchaseOrder.ConfigFileName = "ContractPurchaseOrderGrid";
            gPurchaseOrder.SetGridSelectionBehavior(false, true);
            gPurchaseOrder.FieldLayoutResourceString = "ContractPurchaseOrder";
            gPurchaseOrder.xGrid.FieldLayoutSettings = f;
            //Make the grid editable
            gPurchaseOrder.xGrid.FieldSettings.AllowEdit = true;
                     
            gContractMilestones.MainTableName = "milestones";
            gContractMilestones.ConfigFileName = "ContractMilestonesGrid";
            gContractMilestones.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(gContractMilestones_EditModeEnded);
            gContractMilestones.SetGridSelectionBehavior(false, true);
            gContractMilestones.FieldLayoutResourceString = "ContractMilestones";
            gContractMilestones.xGrid.FieldLayoutSettings = f;
            //Make the grid editable
            gContractMilestones.xGrid.FieldSettings.AllowEdit = true;

            gInvoiceDistribution.MainTableName = "invoice_distribution";
            gInvoiceDistribution.ConfigFileName = "InvoiceDistributionGrid";
            gInvoiceDistribution.SetGridSelectionBehavior(true,false);
            gInvoiceDistribution.FieldLayoutResourceString = "ContractInvoiceDistribution";

            //assign delegate for adding contacts
            gInvoiceDistribution.ContextMenuAddDelegate = InvoiceDistGridAddDelegate;
            gInvoiceDistribution.ContextMenuAddDisplayName = "Add New Contact Association";
            //assign delegate for removing contacts
            gInvoiceDistribution.ContextMenuRemoveDelegate = InvoiceDistGridRemoveDelegate;
            gInvoiceDistribution.ContextMenuRemoveDisplayName = "Remove Contact Association";
            gInvoiceDistribution.xGrid.FieldSettings.AllowEdit = false;
            gInvoiceDistribution.xGrid.FieldLayoutSettings.AllowAddNew = false;
            gInvoiceDistribution.xGrid.FieldLayoutSettings.AllowDelete = true;
            gInvoiceDistribution.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            
            //assign delegate for adding contacts
            //gInvoiceDistribution.ContextMenuAddDelegate = InvoiceDistGridAddDelegate;
            //gInvoiceDistribution.ContextMenuAddDisplayName = "Add New Contact Association";
            //assign delegate for removing contacts
            gSalesperson.MainTableName = "contract_salesperson";
            gSalesperson.ConfigFileName = "ContractSalespersonGrid";
            gSalesperson.SetGridSelectionBehavior(false, true);
            gSalesperson.FieldLayoutResourceString = "ContractSalesperson";
            gSalesperson.xGrid.FieldLayoutSettings = f;
            gSalesperson.ContextMenuRemoveDelegate = SalespersonGridRemoveDelegate;
            gSalesperson.ContextMenuRemoveDisplayName = "Remove Contract Salesperson";
            gSalesperson.xGrid.FieldSettings.AllowEdit = true;
            gSalesperson.xGrid.FieldLayoutSettings.AllowDelete = true;
            gSalesperson.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;

            gAttributes.MainTableName = "attribute";
            gAttributes.ConfigFileName = "ContractAttributeGrid";
            gAttributes.SetGridSelectionBehavior(false, true);
            gAttributes.FieldLayoutResourceString = "ContractAttribute";
            gAttributes.xGrid.FieldLayoutSettings = f;
            //Make the grid editable
            gAttributes.xGrid.FieldSettings.AllowEdit = true;

                      
            //gInvoiceDistribution.xGrid.Columns.DataColumns["lcr_Number"] as CustomDisplayEditableColumn.EditorDisplayBehavior = EditorDisplayBehaviors.Always;
                     

            //GridCollection.Add(gCompany );
            GridCollection.Add(gLCR);
            GridCollection.Add(gPurchaseOrder );
            GridCollection.Add(gAttributes);
            GridCollection.Add(gInvoiceDistribution);
            GridCollection.Add(gContractMilestones);
            GridCollection.Add(gSalesperson);

            //this.Load("dddwcontractattributes");

            //if (this.CurrentBusObj.ObjectData.Tables["legal_agreement"].Rows.Count > 1)
            //    CLMFlag = true;
            

        }

        public void InvoiceDistGridAddDelegate()
        {
            //TODO: take out don't need here prompt user for confirmation
            //MessageBoxResult result = Messages.ShowYesNo("Add a new contact?", System.Windows.MessageBoxImage.Question);
            //if (result == MessageBoxResult.Yes)
            //{
            //add new contact, pull up contact dialog for selecting contacts
            //Messages.ShowMessage("Select a contact", MessageBoxImage.Information);
            openContactSearchScreen();
            //}
            //else
            //{
            //    //do nothing
            //    Messages.ShowMessage("I did nothing", MessageBoxImage.Information);
            //}
        }

        public void InvoiceDistGridRemoveDelegate()
        {
            //remove contact
            object nothing = null;
            //gInvoiceDistribution.xGrid.FieldSettings.AllowEdit = true;
            DataPresenterCommands.DeleteSelectedDataRecords.Execute(nothing, gInvoiceDistribution );
            //gInvoiceDistribution.xGrid.FieldSettings.AllowEdit = false;
        }

        public void SalespersonGridRemoveDelegate()
        {
            //remove contact
            object nothing = null;
            //gInvoiceDistribution.xGrid.FieldSettings.AllowEdit = true;
            DataPresenterCommands.DeleteSelectedDataRecords.Execute(nothing, gSalesperson);
            //gInvoiceDistribution.xGrid.FieldSettings.AllowEdit = false;
        }

        private void openContactSearchScreen()
        {
            gInvoiceDistribution.xGrid.FieldLayoutSettings.AllowAddNew = false;
            DataView dataSource = this.gInvoiceDistribution.xGrid.DataSource as DataView;

            //Event handles opening of the lookup window upon double click on contact ID field
            //RazerBase.Lookups.ContactLookup lookup = new RazerBase.Lookups.ContactLookup();

            // gets the users response
            //lookup.ShowDialog();

            ContactSearch f = new ContactSearch();

            cGlobals.ReturnParms.Clear();
            f.Init(new cBaseBusObject("ContactLookup"));

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                string ContactId = cGlobals.ReturnParms[0].ToString();
                //set new parms for contact 
                this.CurrentBusObj.changeParm("@contact_id_temp", ContactId);
                //pop the contact temp table with newly selected contact info
                this.CurrentBusObj.LoadTable("invoice_distribution_temp");
                if (this.CurrentBusObj.HasObjectData == true)
                {
                    //return contact info
                    if (this.CurrentBusObj.ObjectData.Tables["invoice_distribution_temp"].Rows.Count > 0)
                    {
                        //Add new contact to grid
                        gInvoiceDistribution.xGrid.FieldSettings.AllowEdit = true;
                        DataRow dr = this.CurrentBusObj.ObjectData.Tables["invoice_distribution"].NewRow();
                        //add relationship id
                        dr[0] = this.CurrentBusObj.ObjectData.Tables["invoice_distribution_temp"].Rows[0].ItemArray[0].ToString().Trim();
                        //add contact id
                        dr[1] = this.CurrentBusObj.ObjectData.Tables["invoice_distribution_temp"].Rows[0].ItemArray[1].ToString().Trim();
                        //add name
                        dr[2] = this.CurrentBusObj.ObjectData.Tables["invoice_distribution_temp"].Rows[0].ItemArray[2].ToString().Trim();
                        //add email
                        dr[3] = this.CurrentBusObj.ObjectData.Tables["invoice_distribution_temp"].Rows[0].ItemArray[3].ToString().Trim();
                        //Relationship type
                        dr[4] = this.CurrentBusObj.ObjectData.Tables["invoice_distribution_temp"].Rows[0].ItemArray[4].ToString().Trim();
                        //Contract
                        dr[5] = 0;
                            //String Relationship
                        dr[6] = "";
                      

                        this.CurrentBusObj.ObjectData.Tables["invoice_distribution"].Rows.Add(dr);
                        gInvoiceDistribution.xGrid.FieldSettings.AllowEdit = false;
                    }
                }
            }
        }

        void gContractMilestones_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            DateTime dt = new DateTime();
            dt = DateTime.Today;

            int edit_index = gContractMilestones.ActiveRecord.Cells.IndexOf(e.Cell);


            gContractMilestones.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            DataRecord GridRecord = null;
            GridRecord = gContractMilestones.ActiveRecord;
            if (GridRecord.DataItem == null)
                return;
            DataRowView dr = GridRecord.DataItem as DataRowView;
   

            DataView dv = dr.DataView;
            if (GridRecord != null)
            {
                if (GridRecord.Cells["fulfilled_flag"].Value.ToString() == "1")
                {
                    GridRecord.Cells["fulfilled_date"].Value = dt;
                }
                else
                    GridRecord.Cells["fulfilled_date"].Value = "01/01/1900";


            }
        }
        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    //This code is used to tie a drop down to a grid when the grid needs to use a lookup value for editing or review
                    //This screen event is placed here because a load is required of the base object before this will work
                    //Add code to populate the grid combobox on the Company Contract Grid
                    ComboBoxItemsProvider ip = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip.ItemsSource = this.CurrentBusObj.ObjectData.Tables["company"].DefaultView;

                    //set the value and display path
                    ip.ValuePath = "company_code";
                    ip.DisplayMemberPath = "company_description";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbCompanyGridCombo = ip;
                    ComboBoxItemsProvider ip2 = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip2.ItemsSource = this.CurrentBusObj.ObjectData.Tables["po_dddw"].DefaultView;

                    //set the value and display path
                    ip2.ValuePath = "contract_location_id";
                    ip2.DisplayMemberPath = "cs_id";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbContractLocation = ip2;

                    ComboBoxItemsProvider ip3 = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip3.ItemsSource = this.CurrentBusObj.ObjectData.Tables["dddwmilestones"].DefaultView;

                    //set the value and display path
                    ip3.ValuePath = "milestone_type";
                    ip3.DisplayMemberPath = "milestone_type_desc";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbContractMilestone = ip3;

                    ComboBoxItemsProvider ip4 = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip4.ItemsSource = this.CurrentBusObj.ObjectData.Tables["dddwproductitem"].DefaultView;

                    //set the value and display path
                    ip4.ValuePath = "product_item_id";
                    ip4.DisplayMemberPath = "product_item_description";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbProductItem = ip4;

                    ComboBoxItemsProvider ip5 = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip5.ItemsSource = this.CurrentBusObj.ObjectData.Tables["dddw_contract_products"].DefaultView;

                    //set the value and display path
                    ip5.ValuePath = "product_code";
                    ip5.DisplayMemberPath = "product_description";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbProductCode = ip5;

                    ComboBoxItemsProvider ip6 = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip6.ItemsSource = this.CurrentBusObj.ObjectData.Tables["salesperson"].DefaultView;

                    //set the value and display path
                    ip6.ValuePath = "contact_id";
                    ip6.DisplayMemberPath = "name";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbSalesperson = ip6;

                    ComboBoxItemsProvider ip7 = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip7.ItemsSource = this.CurrentBusObj.ObjectData.Tables["dddwproductitem"].DefaultView;

                    ComboBoxItemsProvider ip8 = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip8.ItemsSource = this.CurrentBusObj.ObjectData.Tables["dddwdealtype"].DefaultView;

                    //set the value and display path
                    ip8.ValuePath = "fkey_int";
                    ip8.DisplayMemberPath = "code_value";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbDealType = ip8;

                    ComboBoxItemsProvider ip9 = new ComboBoxItemsProvider();
                    //Set the items source to be the databale of the DDDW
                    ip9.ItemsSource = this.CurrentBusObj.ObjectData.Tables["dddwcontractattributes"].DefaultView;

                    //set the value and display path
                    ip9.ValuePath = "attribute_id";
                    ip9.DisplayMemberPath = "attribute";
                    //Set the property that the grid combo will bind to
                    //This value is in the binding in the layout resources file for the grid.
                    cmbAttribute = ip9;

                    if (this.CurrentBusObj.ObjectData.Tables["legal_agreement"].Rows.Count > 0)
                        CLMFlag = false;
                    else
                        CLMFlag = true;
                }

            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }

        private void gAttributes_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        //public void CheckPOAmount()
        //{
        //    //foreach (DataRecord r in gPurchaseOrder.xGrid.Records)
        //    //        {
        //    //            //sum adj amts
        //    //            if (Convert.ToDecimal(r.Cells["total_po_amount_charged"].Value) >  Convert.ToDecimal(r.Cells["amount"].Value))
        //    //            {
        //    //                r.Cells["total_po_amount_charged"].Value = Convert.ToDecimal(r.Cells["total_po_amount_charged"].Value) * -1;

        //    //            }
                           
        //    //        }
        //    foreach (DataRow r in CurrentBusObj.ObjectData.Tables["purchase_order"].Rows)
        //    {
        //        if (Convert.ToDecimal(r["total_po_amount_charged"]) > Convert.ToDecimal(r["amount"]))
        //        {
        //            r["total_po_amount_charged"] = Convert.ToDecimal(r["total_po_amount_charged"]) * -1;
        //        }
        //    }
        //}

        
    }
   
}
