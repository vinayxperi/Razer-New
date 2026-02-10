

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using System.Windows.Shapes;
using Contact;

#endregion

namespace Customer
{

    #region class CustomerContactsTab
    /// <summary>
    /// This class represents a 'CustomerContactsTab' object.
    /// </summary>
    public partial class CustomerContactsTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'CustomerContactsTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerContactsTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "customer_contacts";
            GridContacts.MainTableName = "customer_contacts";
            GridContacts.xGrid.FieldLayoutSettings.AllowDelete = true;

            GridContacts.ConfigFileName = "CustomerContactsGrid";
            GridContacts.WindowZoomDelegate = GridDoubleClickDelegate;
            //assign delegate for adding contacts
            GridContacts.ContextMenuAddDelegate = ContactGridAddDelegate;
            GridContacts.ContextMenuAddDisplayName = "Add New Contact Association";
            //assign delegate for removing contacts
            GridContacts.ContextMenuRemoveDelegate = ContactGridRemoveDelegate;
            GridContacts.ContextMenuRemoveDisplayName = "Remove Contact Association";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridContacts.SetGridSelectionBehavior(true, false);
            GridContacts.FieldLayoutResourceString = "CustomerContactsGrid";

            GridCollection.Add(GridContacts);
        }

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        public void GridDoubleClickDelegate()
        {
            //call contact folder
            GridContacts.ReturnSelectedData("contact_id");
            cGlobals.ReturnParms.Add("GridContacts.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = GridContacts.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);            

        }

        public void ContactGridAddDelegate()
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

        public void ContactGridRemoveDelegate()
        {
                //remove contact
                object nothing = null;
                DataPresenterCommands.DeleteSelectedDataRecords.Execute(nothing, GridContacts);
        }

        private void openContactSearchScreen()
        {
            GridContacts.xGrid.FieldLayoutSettings.AllowAddNew = false;
            DataView dataSource = this.GridContacts.xGrid.DataSource as DataView;

            //Event handles opening of the lookup window upon double click on contact ID field
            //RazerBase.Lookups.ContactLookup lookup = new RazerBase.Lookups.ContactLookup();

            //this.CurrentBusObj.Parms.ClearParms();
            ContactSearch f = new ContactSearch();

            cGlobals.ReturnParms.Clear();
            f.Init(new cBaseBusObject("ContactLookup"));
            // gets the users response
            //lookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                string ContactId = cGlobals.ReturnParms[0].ToString();
                //set new parms for contact 
                this.CurrentBusObj.changeParm("@contact_id_temp", ContactId);
                //pop the contact temp table with newly selected contact info
                this.CurrentBusObj.LoadTable("customer_contacts_temp");
                if (this.CurrentBusObj.HasObjectData == true)
                {
                    //return contact info
                    if (this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows.Count > 0)
                    {
                        //Add new contact to grid
                        GridContacts.xGrid.FieldSettings.AllowEdit = true;
                        DataRow dr = this.CurrentBusObj.ObjectData.Tables["customer_contacts"].NewRow();
                        //add contact_id
                        dr[0] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[0].ToString().Trim();
                        //add contact name
                        dr[1] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[1].ToString().Trim();
                        //add addr1
                        dr[2] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[2].ToString().Trim();
                        //add addr2
                        dr[3] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[3].ToString().Trim();
                        //add addr3
                        dr[4] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[4].ToString().Trim();
                        //add city
                        dr[5] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[5].ToString().Trim();
                        //add state
                        dr[6] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[6].ToString().Trim();
                        //add zip
                        dr[7] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[7].ToString().Trim();
                        //add phone
                        dr[8] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[8].ToString().Trim();
                        //add email
                        dr[9] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[9].ToString().Trim();
                        //add fax
                        dr[10] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[10].ToString().Trim();
                        dr[11] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[11].ToString().Trim();
                        dr[12] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[12].ToString().Trim();
                        dr[13] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[13].ToString().Trim();
                        dr[14] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[14].ToString().Trim();
                        dr[15] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[15].ToString().Trim();
                        dr[16] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[16].ToString().Trim();
                        dr[17] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[17].ToString().Trim();
                        dr[18] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[18].ToString().Trim();
                        dr[19] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[19].ToString().Trim();
                        dr[20] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[20].ToString().Trim();
                        dr[22] = 1;
                        dr[23] = 0;
                        dr[24] = getNewCustNumFromParms();
                        dr[26] = this.CurrentBusObj.ObjectData.Tables["customer_contacts_temp"].Rows[0].ItemArray[25].ToString().Trim();

                        this.CurrentBusObj.ObjectData.Tables["customer_contacts"].Rows.Add(dr);
                        GridContacts.xGrid.FieldSettings.AllowEdit = false;
                    }
                }
            }
        }

        /// <summary>
        /// used to find customerId in objectData
        /// </summary>
        /// <returns></returns>
        private string getNewCustNumFromParms()
        {
            var newCustomerNumParm = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                     where x.Field<string>("parmName") == "@receivable_account"
                                     select new
                                     {
                                         parmName = x.Field<string>("parmName"),
                                         parmValue = x.Field<string>("parmValue")
                                     };

            foreach (var info in newCustomerNumParm)
            {
                if (info.parmName == "@receivable_account")
                    return info.parmValue;
            }
            return "";

        }

        #endregion

    }
    #endregion

}
