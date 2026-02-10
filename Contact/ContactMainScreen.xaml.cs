



using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using System.Windows.Controls;
using System.Windows.Input;
using Infragistics.Windows.DockManager;
using System;
using System.Data;



namespace Contact
{


    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class ContactMainScreen : ScreenBase, IScreen
    {



        private string ContactId { get; set; }
        private string ContactName { get; set; }

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }

        //flag used to helps set screen state when inserting new recs
        private bool IsScreenInserting { get; set; }


        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBase constructor is also called.
        /// </summary>
        public ContactMainScreen()
            : base()
        {
            // Create Controls
            InitializeComponent();

            // performs initializations for this object.
            //Init();
        }


        /// <summary>
        /// Event calls CustomerLookup and passes value for lookukp when Customer screen is double clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContactId_DoubleClick(object sender, MouseButtonEventArgs e)
        {
                        //Event handles opening of the lookup window upon double click on contact ID field
            //ContactLookup f = new ContactLookup();

            ContactSearch f = new ContactSearch();

            cGlobals.ReturnParms.Clear();
            f.Init(new cBaseBusObject("ContactLookup"));
            //this.CurrentBusObj.Parms.ClearParms();
            

            // gets the users response
            //f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtContactId .Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                if (txtContactId.Text != ContactId)
                    ReturnData(txtContactId.Text);
                ////load parm
                //loadParms(cGlobals.ReturnParms[0].ToString());
                //txtContactId.Text = cGlobals.ReturnParms[0].ToString();

                //// Call load 
                //this.Load();
                //setEditScreenState();
                //windowCaption = "People-" + txtContactId.Text;
                ContactId = txtContactId.Text;
                //SetHeaderName();
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        private void txtContactId_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            ContactId = txtContactId.Text;
        }

        private void txtContactId_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            int valChk = -1;
            //make sure contact id is valid
            bool result = Int32.TryParse(txtContactId.Text, out valChk);
            if (result)
            {
                //If the Customer # changed then load a new customer
                if (txtContactId.Text != ContactId)
                {
                    ReturnData(txtContactId.Text, "@contact_id");
                    ContactId = txtContactId.Text;
                }
            }
            else
            {
                Messages.ShowInformation("Invalid Contact Id Entered");
            }
        }



        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "Contact";
            // Change this if your folder has item directly bound to it.
            // In many cases the Tabs have data binding, the Folders
            // do not.
            this.DoNotSetDataContext = true;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            // add the Tabs
            TabCollection.Add(uGeneral);
            TabCollection.Add(uAssignment);
            //TODO: For Phase II
            //TabCollection.Add(uCopy);

            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                ContactId = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();
                //this.CurrentBusObj.Parms.AddParm("@p_contact_id", ContactId.ToString());
                //this.CurrentBusObj.Parms.AddParm("@contact_id", ContactId.ToString());
                // load the data
                this.Load();
                // Set the Header
                SetHeaderName();
            }
            else
            {
                setInitScreenState();
            }
        }

        /// <summary>
        /// Load bus obj parms, used in multiple places
        /// </summary>
        private void loadParms(string contactId)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //if custId passed load external_char_id and recv_acct with passed customerId
                if (contactId != "")
                {
                    this.CurrentBusObj.Parms.AddParm("@contact_id", contactId);
                }
                else
                {
                    //if custId NOT passed load external_char_id and recv_acct with global parm CustId if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.AddParm("@contact_id", cGlobals.ReturnParms[0].ToString());
                    }
                    //doing an insert setup dummy vals
                    else
                    {
                        this.CurrentBusObj.Parms.AddParm("@contact_id", "-1");
                    }
                }
                //constant parms//////////////////////////////////////////
                //this.CurrentBusObj.Parms.AddParm("@new_contact_id", "-1");
                //this.CurrentBusObj.Parms.AddParm("@phone_type_id", "-1");
                //////////////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }

        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                return true;
            }
            else
            {
                Messages.ShowWarning("Contact Not Found");
                return false;
            }
        }

        /// <summary>
        /// Sets HeaderName based on value entered into customerName textbox
        /// </summary>
        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (txtContactId.Text == null)
            {
                windowCaption = "People-" + this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[0].ToString();
                txtContactId.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[0].ToString();
                txtContactName.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[1].ToString() + " " + this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[2].ToString();
                //@@NEED CODE - Error for entering invalid contract value
                //ContactId = Convert.ToInt32(txtContactId.Text);
            }
            //Sets the header name from within same folder
            else
            {
                ContentPane p = (ContentPane)this.Parent;
                p.Header = "People-" + txtContactId.Text;
                if (this.CurrentBusObj.ObjectData.Tables["General"] != null)
                {
                //set contact name
                txtContactName.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[1].ToString() + " " + this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[2].ToString();
                //if Salesperson type, make comm flag visible
                if  (this.CurrentBusObj.ObjectData.Tables["General"].Rows[0]["type"].ToString()   == "1")
                
                    uGeneral.chkCommFlag.Visibility = System.Windows.Visibility.Visible;
                else
                    uGeneral.chkCommFlag.Visibility = System.Windows.Visibility.Hidden;

                }
            }
        }

        /// <summary>
        /// Used to return Contact info and set header
        /// </summary>
        /// <param name="SearchValue">User entered search value from text box</param>
        /// <param name="DbParm">Database Parameter based on search criteria</param>
        private void ReturnData(string SearchValue, string DbParm = null)
        {

            //if no value do nothing
            if (SearchValue == "") return;

            //DWR--Added 1/15/13 - This section will check to see if changes have been made and if save is desired in the event of a
            //double click lookup or a change of the contract id field.
            //Verify that no save is needed
            Prep_ucBaseGridsForSave();
            PrepareFreeformForSave();
            if (IsScreenDirty)
            {
                //Establish a temporary customer id for storing the ID the user wanted to go to.  This will be used in the final retrieval in the event of a
                //Yes or No answer to the question below.
                String NewContactID = "";
                System.Windows.MessageBoxResult result = Messages.ShowYesNoCancel("Would you like to save existing changes?",
                           System.Windows.MessageBoxImage.Question);
                //Save existing customer information and then load new customer
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    NewContactID = txtContactId.Text;
                    Save();
                    //If Save fails then reset contract id to original value and exit retrieve so that changes will not be lost.
                    if (!SaveSuccessful)
                    {
                        txtContactId.Text = ContactId;
                        return;
                    }
                    else if (NewContactID != "")
                    {
                        txtContactId.Text = NewContactID.ToString();
                        ReturnData(txtContactId.Text);
                    }
                }
                //Returns the user to the current customer window and resets the txtCustomerNum field to original value.
                else if (result == System.Windows.MessageBoxResult.Cancel)
                {
                    txtContactId.Text = ContactId;
                    return;

                }

                ContactId = txtContactId.Text;
            }


            loadParms(SearchValue);
            //load data
            this.Load();
            if (chkForData())
            {
                //Set the header name
                SetHeaderName();
                //set folder back to edit mode when data is present
                setEditScreenState();
            }
        }

        public override void Save()
        {
            //call the validation on the General Tab
            uGeneral.ValidatebeforeSave();
            if (uGeneral.errorsExist != true)
            {

                base.Save();
                if (SaveSuccessful)
                {
                    var localCustomerInfo = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                            where x.Field<string>("parmName") == "@contact_id" ||
                                            x.Field<string>("parmName") == "@contact_name"
                                            select new
                                            {
                                                parmName = x.Field<string>("parmName"),
                                                parmValue = x.Field<string>("parmValue")
                                            };

                    foreach (var info in localCustomerInfo)
                    {
                        if (info.parmName == "@contact_id")
                            txtContactId.Text = info.parmValue;
                        if (info.parmName == "@contact_name")
                            txtContactName.Text = info.parmValue;
                    }
                    if (IsScreenInserting == false)
                    {
                        //if contact id found then set header and pop otherwise send message
                        if (chkForData()) SetHeaderName();
                    }
                    else
                    {
                        //Insert has occurred
                        //reset business obj for load of new contact
                        this.CurrentBusObj.ObjectData = null;
                        //load new contact
                        ReturnData(txtContactId.Text, "@contact_id");
                        //turn off inserting flag
                        IsScreenInserting = false;
                    }
                    Messages.ShowInformation("Save Successful");
                }
                else
                {
                    Messages.ShowInformation("Save Failed");
                }
            }
            else
                uGeneral.errorsExist = false;
        }

        /// <summary>
        /// Override of New command
        /// </summary>
        public override void New()
        {
            //turn on inserting flag, needed for setting proper screen state
            IsScreenInserting = true;
            //load dummy parms for insert
            loadParms("");
            txtContactId.Text = "0";
            txtContactName.Text = "New";
            //set focus to general tab
            setInsertScreenState();
            ContentPane p = (ContentPane)this.Parent;
            p.Header = "People-" + txtContactId.Text;
            base.New();
        }

        /// <summary>
        /// sets screen state for init
        /// </summary>
        private void setInitScreenState()
        {
            //don't allow edits on general tab fields
            this.uGeneral.IsEnabled = false;
            this.uAssignment.IsEnabled = false;
            //don't allow header edits
            txtContactId.IsEnabled = true;
            txtContactName.IsEnabled = true;
        }

        /// <summary>
        /// sets screen state for insert
        /// </summary>
        private void setInsertScreenState()
        {
            //go to general tab
            uGeneral.Focus();
            //set focus to first field
            this.uGeneral.txtFirstName.Focus();
            //allow edits on general tab fields
            this.uGeneral.IsEnabled = true;
            this.uAssignment.IsEnabled = false;
            //don't allow header edits
            txtContactId.IsEnabled = false;
            txtContactName.IsEnabled = false;
            uGeneral.GridPhone.IsEnabled = false;
            uGeneral.GridEmail.IsEnabled = false;
        }

        /// <summary>
        /// sets screen state for edit
        /// </summary>
        private void setEditScreenState()
        {
            //go to general tab
            //allow edits
            this.uGeneral.IsEnabled = true;
            this.uAssignment.IsEnabled = true;
            //enable header
            txtContactId.IsEnabled = true;
            txtContactName.IsEnabled = true;
            uGeneral.GridPhone.IsEnabled = true;
            uGeneral.GridEmail.IsEnabled = true;
        }


    }


}
