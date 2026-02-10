

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Controls;
using Infragistics.Windows.Editors;

#endregion

namespace Contact
{

    #region class ContactGeneralTab
    /// <summary>
    /// This class represents a 'ContactGeneralTab' object.
    /// </summary>
    public partial class ContactGeneralTab : ScreenBase, IPreBindable
    {

        public ComboBoxItemsProvider cmbPhoneTypeGridCombo { get; set; }
        public bool errorsExist = false;
        bool newPhone = false;
        bool newEmail = false;

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ContactGeneralTab' object and call the ScreenBase's constructor.
        /// </summary>
        public ContactGeneralTab() : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

       
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "general";
            buildGrids();
        }

        public void buildGrids()
        {
            FieldLayoutSettings f = new FieldLayoutSettings();
            //f.AllowAddNew = true;
            f.AllowDelete = true;
            f.AddNewRecordLocation = AddNewRecordLocation.OnTop;
            //f.SelectionTypeField = SelectionType.Single;
            GridPhone.xGrid.FieldLayoutSettings = f;
            GridPhone.MainTableName = "phone";
            GridPhone.ContextMenuAddDelegate = ContactPhoneAddDelegate;
            GridPhone.ContextMenuRemoveDelegate = ContactPhoneRemoveDelegate;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridPhone.SetGridSelectionBehavior(false, false);
            GridPhone.xGrid.FieldSettings.AllowEdit = true;
            GridPhone.FieldLayoutResourceString = "ContactPhoneGrid";
            GridPhone.ConfigFileName = "ContactPhoneConfig";
            GridCollection.Add(GridPhone);

            //pop email grid
            GridEmail.MainTableName = "email";

            GridEmail.xGrid.FieldLayoutSettings = f;
         
            GridEmail.ContextMenuAddDelegate = ContactEmailAddDelegate;
            GridEmail.ContextMenuRemoveDelegate = ContactEmailRemoveDelegate;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridEmail.SetGridSelectionBehavior(false, false);
            GridEmail.xGrid.FieldSettings.AllowEdit = true;
            GridEmail.ConfigFileName = "ContactEmailGrid";
            GridEmail.FieldLayoutResourceString = "ContactEmailGrid";
            GridCollection.Add(GridEmail);
        }

        private void ContactEmailAddDelegate()
        {
            //DataRow dr = this.CurrentBusObj.ObjectData.Tables["email"].NewRow();
            //dr["contact_id"] = getContactIdFromParms();
            //this.CurrentBusObj.ObjectData.Tables["email"].Rows.Add(dr);
            newEmail = true;
            GridEmail.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = GridEmail.xGrid.RecordManager.CurrentAddRecord;
            //Set the default values for the columns
            row.Cells["contact_id"].Value = getContactIdFromParms();
            row.Cells["email"].Value = ""; 
            row.Cells["description"].Value = "";
            row.Cells["primary_flag"].Value = 0;
             //Commit the add new record - required to make this record active
            GridEmail.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            GridEmail.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            GridEmail.xGrid.ActiveRecord = GridEmail.xGrid.Records[0];
            //Set the field as active
            (GridEmail.xGrid.Records[GridEmail.ActiveRecord.Index] as DataRecord).Cells["email"].IsActive = true;
            //Moves the cursor into the active cell 
            GridEmail.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);

        }

        private void ContactEmailRemoveDelegate()
        {
            DataRecord r = GridEmail.ActiveRecord;
          
            if (r != null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;
                if (row != null)
                {
                    row.Delete();
                }
            }
        }

        private void ContactPhoneAddDelegate()
        {
            //DataRow dr = this.CurrentBusObj.ObjectData.Tables["phone"].NewRow();
            //this.CurrentBusObj.ObjectData.Tables["phone"].Rows.Add(dr);
            newPhone = true;
            //DataRow dr = this.CurrentBusObj.ObjectData.Tables["email"].NewRow();
            //dr["contact_id"] = getContactIdFromParms();
            //this.CurrentBusObj.ObjectData.Tables["email"].Rows.Add(dr);
            //newEmail = true;
            GridPhone.xGrid.FieldLayoutSettings.AllowAddNew = true;
            DataRecord row = GridPhone.xGrid.RecordManager.CurrentAddRecord;
            //Set the default values for the columns
            row.Cells["contact_id"].Value = getContactIdFromParms();
            row.Cells["phone_type_id"].Value = 0;
            row.Cells["phone"].Value = "";
            row.Cells["description"].Value = "";
            row.Cells["primary_flag"].Value = 0;
            //Commit the add new record - required to make this record active
            GridPhone.xGrid.RecordManager.CommitAddRecord();
            //Remove the add new record row
            GridPhone.xGrid.FieldLayoutSettings.AllowAddNew = false;
            //Set the row just created to the active record
            GridPhone.xGrid.ActiveRecord = GridPhone.xGrid.Records[0];
            //Set the field as active
            (GridPhone.xGrid.Records[GridPhone.ActiveRecord.Index] as DataRecord).Cells["phone"].IsActive = true;
            //Moves the cursor into the active cell 
            GridPhone.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
        }

        private void ContactPhoneRemoveDelegate()
        {
            DataRecord r = GridPhone.ActiveRecord;

            if (r != null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;
                if (row != null)
                {
                    row.Delete();
                }
            }
        }

        /// <summary>
        /// used to find contactId in objectData
        /// </summary>
        /// <returns></returns>
        private string getContactIdFromParms()
        {
            try
            {
                var ContactIdParm = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                         where x.Field<string>("parmName") == "@contact_id"
                                         select new
                                         {
                                             parmName = x.Field<string>("parmName"),
                                             parmValue = x.Field<string>("parmValue")
                                         };

                foreach (var info in ContactIdParm)
                {
                    if (info.parmName == "@contact_id")
                        return info.parmValue;
                }
                return "";
            }
            catch (Exception ex)
            {
                Messages.ShowError("Cannot find contact id. Error : " + ex.Message);
                return "";
            }
        }

        public void PreBind()
        {
            // if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                //Contact Type Lookup
                this.cmbType.SetBindingExpression("contact_type_id", "description", this.CurrentBusObj.ObjectData.Tables["contact_type_lookup"]);
                //Gender Lookup
                this.cmbGender.SetBindingExpression("gender", "genderdesc", this.CurrentBusObj.ObjectData.Tables["contact_gender_lookup"]);
                //State Lookup
                this.cmbState.SetBindingExpression("state", "description", this.CurrentBusObj.ObjectData.Tables["contact_state_lookup"]);
                //Country Lookup
                this.cmbCountry.SetBindingExpression("country_id", "country", this.CurrentBusObj.ObjectData.Tables["contact_country_lookup"]);

                //for phone grid
                ComboBoxItemsProvider provider = new ComboBoxItemsProvider();
                //Set the items source to be the databale of the DDDW
                provider.ItemsSource = this.CurrentBusObj.ObjectData.Tables["contact_phone_type_lookup"].DefaultView;

                //set the value and display path
                provider.ValuePath = "phone_type_id";
                provider.DisplayMemberPath = "description";
                //Set the property that the grid combo will bind to
                //This value is in the binding in the layout resources file for the grid.
                cmbPhoneTypeGridCombo = provider;

               
            }
        }


        #endregion

        private void cmbType_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            setCommFlagState();
        }

        private void cmbType_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            setCommFlagState();
        }

        /// <summary>
        /// sets comm flag based on cmbType selection
        /// </summary>
        private void setCommFlagState()
        {
            if (cmbType.SelectedText == "Salesperson")
                chkCommFlag.Visibility = System.Windows.Visibility.Visible;
            else
                chkCommFlag.Visibility = System.Windows.Visibility.Hidden;
        }

        private void chkInactiveFlag_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.CurrentBusObj.ObjectData.Tables["assignment"].Rows.Count > 0)
            {
                Messages.ShowInformation("Contact is assigned. Cannot make inactive until the assignment has been removed");
                chkInactiveFlag.IsChecked = 0;
            }
            
        }

        public void ValidatebeforeSave()
        {
            //Need to validate type is entered otherwise default
            if (cmbType.SelectedText == "")
            {
                cmbType.SelectedValue = 6;
                cmbType.SelectedText = "InvoiceRecipient";
            }
            //Need to validate only one primary phone
            int primaryFlag = 0;
            foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["phone"].Rows)
            {

                if (dr.RowState != DataRowState.Deleted && Convert.ToInt32(dr["primary_flag"]) == 1)
                    primaryFlag++;
                //Need to validate the phone type <> 0
                if (dr.RowState != DataRowState.Deleted && Convert.ToInt32(dr["phone_type_id"]) == 0)
                {
                    errorsExist = true;
                    Messages.ShowInformation("Phone Type must be selected");
                    return;
                }


            }
            if (primaryFlag > 1)
            {
                errorsExist = true;
                Messages.ShowInformation("Can only have one Primary phone");
                return;
            }
            primaryFlag = 0;
            //Need to validate only one primary email
            foreach (DataRow drE in this.CurrentBusObj.ObjectData.Tables["email"].Rows)
            {
                //if datarowEnty row state is deleted do not do this
               if (drE.RowState != DataRowState.Deleted && Convert.ToInt32(drE["primary_flag"]) == 1)
                    primaryFlag++;
            }
            if (primaryFlag > 1)
            {
                errorsExist = true;
                Messages.ShowInformation("Can only have one Primary Email address");
                return;
            }

        }

        

        

    }
    #endregion

}
