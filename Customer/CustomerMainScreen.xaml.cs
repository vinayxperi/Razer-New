



using RazerBase.Interfaces;
using RazerInterface;
using RazerBase;
using RazerBase.Lookups;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DataPresenter;
using System;
using System.Data;
using System.Windows;
using System.Linq;
using System.Drawing.Printing;

using Infragistics.Windows.Editors;
using System.Collections.Generic;



namespace Customer
{


    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class CustomerMainScreen : ScreenBase, IScreen, IPreBindable
    {
        public Int32 CreditRating = -1;
        public string CustomerNum { get; set; }
        private string CustomerName { get; set; }
        //flag used to helps set screen state when inserting new recs
        private bool IsScreenInserting { get; set; }
        public ComboBoxItemsProvider cmbCommentCode { get; set; }
        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }




        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBase constructor is also called.
        /// </summary>
        public CustomerMainScreen()
            : base()
        {
            // Create Controls
            InitializeComponent();
        }



        /// <summary>
        /// Event calls CustomerLookup and passes value for lookukp when Customer screen is double clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Customer_DoubleClick(object sender, MouseButtonEventArgs e)
        {

            //Event handles opening of the lookup window upon double click on contract ID field
            CustomerLookup f = new CustomerLookup();
            f.Init(new cBaseBusObject("CustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();
            cGlobals.ReturnParms.Clear();
            uGeneral.OrigLoad = true;
            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtCustomerNum.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                if (txtCustomerNum.Text != CustomerNum)
                    ReturnData(txtCustomerNum.Text, "@receivable_account");
                uGeneral.OrigLoad = false;

                //DWR - 1/15/13 Removed the below to make the retrieve run through the ReturnData method so that checking for saves would work properly.
                //load current parms
                //loadParms("");
                //txtCustomerNum.Text = cGlobals.ReturnParms[0].ToString();
                ////KSH - 8/21/12 clear comments/attachments grid to fix bug
                //clrCommentsAttachmentsObj();
                //// Call load 
                //this.Load();
                //setEditScreenState();
                ////HeaderName = "Customer-" + txtCustomerNum.Text;
                //windowCaption = "Customer-" + txtCustomerNum.Text;
                //CustomerNum = txtCustomerNum.Text;
                ////revenue detail grid needs blanked out
                //if (this.CurrentBusObj.ObjectData.Tables["revenue"].Rows.Count > 0)
                //{

                //}
                //else
                //    uRevenue.revdetailClearGrid();
                ////Unhide comment type on comment grid
                //uComments.GridUnHideCommentType();
                //SetHeaderName();
                //// Clear the parms
                //cGlobals.ReturnParms.Clear();

            }
        }

        private void txtCustomerNum_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            CustomerNum = txtCustomerNum.Text;
        }

        private void txtCustomerNum_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtCustomerNum.Text != "New")
            {
                uGeneral.OrigLoad = true;
                //If the Customer # changed then load a new customer
                if (txtCustomerNum.Text != CustomerNum)
                    ReturnData(txtCustomerNum.Text, "@receivable_account");
                uGeneral.OrigLoad = false;
            }
        }

        private void txtCustomerName_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //CustomerName = txtCustomerName.Text;
        }

        private void txtCustomerName_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //If the Customer Name changed then load a new customer
            //if (txtCustomerName.Text != CustomerName)
            //    ReturnData(txtCustomerName.Text, "@account_name");
        }




        /// <summary>
        /// This method performs initializations for the Customer folder
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            //set screen inserting flag to false
            IsScreenInserting = false;
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "Customer";
            // Change this if your folder has item directly bound to it.
            // In many cases the Tabs have data binding, the Folders
            // do not.
            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            // add the Tabs
            TabCollection.Add(uHistory);
            TabCollection.Add(uGeneral);
            TabCollection.Add(uAging);
            TabCollection.Add(uAttachments);
            TabCollection.Add(uContracts);
            TabCollection.Add(uContacts);
            TabCollection.Add(uComments);
            TabCollection.Add(uRelationships);
            TabCollection.Add(uReviewDates);
            TabCollection.Add(uProforma);
            TabCollection.Add(uRevenue);



            // if there are parameters load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                //load Custnum from bus obj
                CustomerNum = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();
                //load current parms
                loadParms(CustomerNum);
                uGeneral.OrigLoad = true;
                // load the data
                this.Load();
                uGeneral.OrigLoad = false;
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                    CreditRating = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["credit_rating"].ToString());
                else
                    CreditRating = -1;

                //if customer number found then set header and pop otherwise send message
                if (chkForData())
                    SetHeaderName();
                else
                    return;
                //revenue detail grid needs blanked out



                if (this.CurrentBusObj.ObjectData.Tables["revenue"].Rows.Count > 0)
                {

                }
                else
                    uRevenue.revdetailClearGrid();

                //RES 3/21/19 Display proforma checkbox if open proforma invoices exist
                //if (this.CurrentBusObj.ObjectData.Tables["proforma"].Rows.Count > 0)
                //{

                //    uProforma.IsEnabled = true;

                //}
                //else
                //    uProforma.IsEnabled = false;
                //Unhide comment type on comment grid
                uComments.GridUnHideCommentType();

            }
            else
            {
                setInitScreenState();
            }

            this.HasPrintReport = true;


        }

        /// <summary>
        /// Load bus obj parms, used in multiple places
        /// </summary>
        private void loadParms(string customerId)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //if custId passed load external_char_id and recv_acct with passed customerId
                if (customerId != "")
                {
                    this.CurrentBusObj.Parms.AddParm("@external_char_id", customerId);
                    this.CurrentBusObj.Parms.AddParm("@receivable_account", customerId);
                    //this.CurrentBusObj.Parms.AddParm("@related_to_char_id", customerId);
                }
                else
                {
                    //if custId NOT passed load external_char_id and recv_acct with global parm CustId if exists
                    //12/22/14 res check for new row
                    if ((cGlobals.ReturnParms.Count > 0) && (!IsScreenInserting))
                    {
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", cGlobals.ReturnParms[0].ToString());
                        this.CurrentBusObj.Parms.AddParm("@receivable_account", cGlobals.ReturnParms[0].ToString());
                        //this.CurrentBusObj.Parms.AddParm("@related_to_char_id", cGlobals.ReturnParms[0].ToString());
                    }
                    //doing an insert setup dummy vals
                    else
                    {
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");
                        this.CurrentBusObj.Parms.AddParm("@receivable_account", "-1");
                        //this.CurrentBusObj.Parms.AddParm("@related_to_char_id", "-1");
                    }
                }
                //constant parms//////////////////////////////////////////
                //new customer insert parm
                //this.CurrentBusObj.Parms.AddParm("@new_receivable_account", "-1");
                //comment tab parms
                this.CurrentBusObj.Parms.AddParm("@comment_type", "RA");
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
                //attachment tab parms
                this.CurrentBusObj.Parms.AddParm("@attachment_type", "RATTACH");
                this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
                //contact tab default parms
                //this.CurrentBusObj.Parms.AddParm("@contact_id", "0");
                this.CurrentBusObj.Parms.AddParm("@contact_id_temp", "-1");
                this.CurrentBusObj.Parms.AddParm("@invoice_number", "");
                this.CurrentBusObj.Parms.AddParm("@document_id", "");
                this.CurrentBusObj.Parms.AddParm("@country_id_lookup", 0);
                this.CurrentBusObj.Parms.AddParm("@province_id_lookup", 0);
                //this.CurrentBusObj.Parms.AddParm("@related_to_int_id", "-1");
                //this.CurrentBusObj.Parms.AddParm("@relationship_type_id", "4");
                //////////////////////////////////////////////////////////
                //Set the comment code for the comments tab
                //CLB need to clear bottom grid on Revenue tab



                uComments.CommentCode = "RA";
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }


        public void PreBind()
        {
            //if the object data was loaded
            if (this.CurrentBusObj.HasObjectData)
            {
                //ComboBoxItemsProvider provider = new ComboBoxItemsProvider();
                ////Set the items source to be the databale of the DDDW
                //provider.ItemsSource = this.CurrentBusObj.ObjectData.Tables["dddwcommentcode"].DefaultView;

                ////set the value and display path
                //provider.ValuePath = "comment_code";
                //provider.DisplayMemberPath = "description";
                ////Set the property that the grid combo will bind to
                ////This value is in the binding in the layout resources file for the grid.
                //cmbCommentCode = provider;
            }
        }
        /// <summary>
        /// Checks that data exists before trying to SetHeader()
        /// </summary>
        /// <returns></returns>
        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                return true;
            }
            else
            {
                Messages.ShowWarning("Customer Not Found");
                return false;
            }
        }

        /// <summary>
        /// Used to return Customer Acct info and set header
        /// </summary>
        /// <param name="SearchValue">User entered search value from text box</param>
        /// <param name="DbParm">Database Parameter based on search criteria</param>
        private void ReturnData(string SearchValue, string DbParm)
        {
            //DWR--Added 1/15/13 - This section will check to see if changes have been made and if save is desired in the event of a
            //double click lookup or a change of the contract id field.
            //Verify that no save is needed
            Prep_ucBaseGridsForSave();
            PrepareFreeformForSave();
            if (IsScreenDirty)
            {

                //Establish a temporary customer id for storing the ID the user wanted to go to.  This will be used in the final retrieval in the event of a
                //Yes or No answer to the question below.
                String NewCustomerID = "";
                System.Windows.MessageBoxResult result = Messages.ShowYesNoCancel("Would you like to save existing changes?",
                           System.Windows.MessageBoxImage.Question);
                //Save existing customer information and then load new customer
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    NewCustomerID = txtCustomerNum.Text;
                    Save();
                    //If Save fails then reset contract id to original value and exit retrieve so that changes will not be lost.
                    if (!SaveSuccessful)
                    {
                        txtCustomerNum.Text = CustomerNum;
                        return;
                    }
                    else if (NewCustomerID != "")
                    {
                        txtCustomerNum.Text = NewCustomerID.ToString();
                        ReturnData(txtCustomerNum.Text, "@receivable_account");
                    }
                }
                //Returns the user to the current customer window and resets the txtCustomerNum field to original value.
                else if (result == System.Windows.MessageBoxResult.Cancel)
                {
                    txtCustomerNum.Text = CustomerNum;
                    return;

                }
            }
            //if no value do nothing
            if (SearchValue == "") return;
            //Add new parameters
            loadParms(SearchValue);
            //KSH - 8/21/12 clear comments/attachments grid to fix bug
            clrCommentsAttachmentsObj();
            //load data
            //if coming from save do not do this...
            uGeneral.OrigLoad = true;
            //uGeneral.txtNSInternalID.Text = "";
            this.Load();

            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                CreditRating = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["credit_rating"].ToString());
            else
                CreditRating = -1;

            //if customer number found then set header and pop otherwise send message
            if (chkForData())
            {//revenue detail grid needs blanked out
                //revenue detail grid needs blanked out
                if (this.CurrentBusObj.ObjectData.Tables["revenue"].Rows.Count > 0)
                {

                }
                else
                    uRevenue.revdetailClearGrid();
                SetHeaderName();

                //RES 3/21/19 Display proforma checkbox if open proforma invoices exist
                if (this.CurrentBusObj.ObjectData.Tables["proforma"].Rows.Count > 0)
                    ProformaInvoiceColorKey.Visibility = System.Windows.Visibility.Visible;
                ////Background = System.Windows.Media.Brushes.Azure;
                else
                    ProformaInvoiceColorKey.Visibility = System.Windows.Visibility.Collapsed;
                //    uProforma.Focusable = false;
                //uProforma.HeaderName = "Proforma";
                //uProforma.Background = System.Windows.Media.Brushes.Transparent;

                //Check if child rows exis. If yes, display children exist. 

                if (this.CurrentBusObj.ObjectData.Tables["account_history"].Rows.Count > 0)
                {
                    DataTable accountHistoryTable = this.CurrentBusObj.ObjectData.Tables["account_history"];
                    bool childExists = accountHistoryTable.AsEnumerable()
                            .Any(row => row.Field<int>("child_ind") == 1);

                    if (childExists)
                        ChildAcctColorKey.Visibility = System.Windows.Visibility.Visible;
                    else
                        ChildAcctColorKey.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    ChildAcctColorKey.Visibility = System.Windows.Visibility.Collapsed;
                }
                //set folder back to edit mode when data is present
                setEditScreenState();
                CustomerNum = txtCustomerNum.Text;

            }
            //Unhide comment type on comment grid
            uComments.GridUnHideCommentType();


        }

        //}
        /// <summary>
        /// Sets HeaderName based on value entered into customerName textbox
        /// </summary>
        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (txtCustomerNum.Text == null)
            {
                windowCaption = "Customer-" + this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[0].ToString();
                txtCustomerNum.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0].ItemArray[0].ToString();
                txtCustomerName.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0]["account_name"].ToString();
                CustomerNum = txtCustomerNum.Text;
            }
            //Sets the header name from within same folder
            else
            {
                ContentPane p = (ContentPane)this.Parent;
                p.Header = "Customer-" + txtCustomerNum.Text;
                txtCustomerName.Text = this.CurrentBusObj.ObjectData.Tables["General"].Rows[0]["account_name"].ToString();
            }
        }

        /// <summary>
        /// Verifies required fields
        /// </summary>
        /// <param name="MissingField"></param>
        /// <returns></returns>
        private bool RequiredFieldsOK(ref string MissingField)
        {

            if ((uGeneral.txtCustAcctName.Text == "") | (uGeneral.txtCustAcctName.Text == null))
            {
                MissingField = "Customer Name";
                return false;
            }
            if ((uGeneral.txtAddress1.Text == "") | (uGeneral.txtAddress1.Text == null))
            {
                MissingField = "Address 1";
                return false;
            }
            if ((uGeneral.txtCity.Text == "") | (uGeneral.txtCity.Text == null))
            {
                MissingField = "City";
                return false;
            }
            if ((uGeneral.txtSFCustAcctName.Text == "") | (uGeneral.txtSFCustAcctName.Text == null))
            {
                MissingField = "SF Customer Name";
                return false;
            }
            if ((uGeneral.txtNSCustAcctName.Text == "") | (uGeneral.txtNSCustAcctName.Text == null))
            {
                MissingField = "NS Customer Name";
                return false;
            }
            if ((uGeneral.txtNSInternalID.Text == "") | (uGeneral.txtNSInternalID.Text == null))
            {
                MissingField = "NS Internal ID";
                return false;
            }
            if ((uGeneral.txtSFID.Text == "") | (uGeneral.txtSFID.Text == null))
            {
                MissingField = "SF ID";
                return false;
            }
            if(!uGeneral.txtNSInternalID.Text.All(char.IsDigit))
            {
                Messages.ShowInformation("NetSuite Internal ID allows only digits.");
                uGeneral.txtNSInternalID.Text = "";
                return false;
            }

            //if (uGeneral.cmbState.SelectedText == "") 
            //{
            //    MissingField = "State";
            //    return false;
            //}
            //if (uGeneral.txtPostalCode.Text == "")
            //{
            //    MissingField = "Postal Code";
            //    return false;
            //}
            //TODO: check for acct type when implemented

            return true;

        }

        private bool VerifyData()
        {
            //validate if country is entered and more than one province for that country, a province was entered
            int validProvince = 0;
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {
                if ((this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["unposted_adjustments"] != null)
                    && (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["unposted_adjustments"].ToString() != ""))
                    if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["unposted_adjustments"].ToString()) > 0
                    && (CreditRating == 1 || CreditRating == 10 || CreditRating == 11 || CreditRating == 12)
                    && (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["credit_rating"].ToString() == "9"
                       || this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["credit_rating"].ToString() == "0"
                       || this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["credit_rating"].ToString() == "8")
                   )
                    {
                        MessageBox.Show("Credit rating cannot be changed to a deferred status because there are unposted debit apply cash adjustments for this customer");
                        return false;
                    }
                //else
                //    CreditRating = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["credit_rating"].ToString());
                //if dddwprovince has row other than None, make it required
                if (uGeneral.cmbProvince.SelectedText == "None" | uGeneral.cmbProvince.SelectedText == null)
                {
                    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["country_id"].ToString() == "0")
                    {
                        return true;
                    }
                    else
                    {
                        ////If provinces exist to be selected, it is required
                        //CurrentBusObj.Parms.UpdateParmValue("@country_id_lookup", uGeneral.cmbCountry.SelectedValue);


                        //CurrentBusObj.LoadTable("provincevalidate");
                        //if (this.CurrentBusObj.ObjectData.Tables["provincevalidate"].Rows.Count > 0)
                        //    if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["provincevalidate"].Rows[0]["provincecount"]) > 0)
                        //    {

                        //        int provinceID = Convert.ToInt32(uGeneral.cmbProvince.SelectedValue);
                        //        foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["customer_province_lookup"].Rows)

                        //            if (Convert.ToInt32(r["province_id"]) == provinceID)
                        //            {
                        //                validProvince = 1;


                        //            }
                        //        if (validProvince == 0)
                        //        {
                        //            Messages.ShowInformation("Please select a valid province for the country from the dropdown");
                        //            return false;
                        //        }
                        //        else
                        //        {
                        //            uGeneral.cmbProvince.SelectedValue = 0;
                        //            uGeneral.cmbProvince.SelectedText = "None";
                        //            return true;
                        //        }
                        //    }

                    }
                }
                else
                {
                    return true;
                }

            }
            //need this to save even if they do not tab off the field
            uReviewDates.GridReviewDates.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndCommitRecord);
            //verify the targeted reviewer is not blank on the Review Dates tab
            //Verify that all allocations have either product,account and unapplied flag checked or have document and seq

            foreach (DataRow r in CurrentBusObj.ObjectData.Tables["customer_review_dates"].Rows)
            {

                if (r["targeted_reviewer"] == null)
                {
                    MessageBox.Show("Targeted Reviewer is required on the Review Dates tab.");
                    return false;

                }
                if (r["targeted_reviewer"].ToString().Trim() == "")
                {
                    MessageBox.Show("Targeted Reviewer is required on the Review Dates tab.");
                    return false;

                }
                if (r["targeted_reviewer"].ToString() == " ")
                {
                    MessageBox.Show("Targeted Reviewer is required on the Review Dates tab.");
                    return false;

                }
                if (r["review_date"] == null)
                {
                    MessageBox.Show("Review Date is required on the Review Dates tab.");
                    return false;

                }
                if (r["review_date"].ToString() == "01/01/1900")
                {
                    MessageBox.Show("Review Date is required on the Review Dates tab.");
                    return false;

                }
                if (r["review_date"].ToString().Trim() == "1/1/1900 12:00:00 AM")
                {
                    MessageBox.Show("Review Date is required on the Review Dates tab.");
                    return false;

                }
                if (r["review_date"].ToString() == " ")
                {
                    MessageBox.Show("Review Date is required on the Review Dates tab.");
                    return false;

                }





            }



            return true;

        }

        /// <summary>
        /// Override of save method handles save functionality for folder
        /// </summary>
        public override void Save()
        {
            if (IsScreenInserting)
            {
                uGeneral.NewSave = true;
            }
            this.PrepareFreeformForSave();
            string MissingField = "";
            //chk reqd fields
            if (RequiredFieldsOK(ref MissingField) == false)
            {
                Messages.ShowWarning("Required Field: " + MissingField + " must have a value");
                //This is required in order for the application to prevent exiting if teh save was done on exit
                SaveSuccessful = false;
                return;
            }
            //Verify that the data to be saved is valid
            if (!VerifyData())
            {
                MessageBox.Show("Unable to save data until all errors are corrected.");
                SaveSuccessful = false;
                return;
            }
            uGeneral.SaveCancelSelectionChanged = true;
            base.Save();
            if (SaveSuccessful)
            {
                uGeneral.SaveCancelSelectionChanged = false;
                //this.CurrentBusObj.Parms.AddParm("@comment_type", "RA");
                var localCustomerInfo = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                        where x.Field<string>("parmName") == "@receivable_account" ||
                                        x.Field<string>("parmName") == "@customer_name"
                                        select new
                                        {
                                            parmName = x.Field<string>("parmName"),
                                            parmValue = x.Field<string>("parmValue")
                                        };

                foreach (var info in localCustomerInfo)
                {
                    if (info.parmName == "@receivable_account")
                        txtCustomerNum.Text = info.parmValue;
                    if (info.parmName == "@customer_name")
                        txtCustomerName.Text = info.parmValue;
                    if (IsScreenInserting == false)
                    {
                        //if customer number found then set header and pop otherwise send message
                        if (chkForData()) SetHeaderName();
                    }
                    else
                    {
                        //Insert has occurred
                        //reset business obj for load of new cust
                        this.CurrentBusObj.ObjectData = null;
                        ReturnData(txtCustomerNum.Text, "@receivable_account");
                        //turn off inserting flag
                        IsScreenInserting = false;
                    }

                }
                Messages.ShowInformation("Save Successful");
                uGeneral.NewSave = false;
                CreditRating = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["credit_rating"].ToString());
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
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
            txtCustomerNum.Text = "New";
            txtCustomerName.Text = "New";
            //uGeneral.txtNSInternalID.Text = "";
            //set focus to general tab
            setInsertScreenState();
            //Unhide comment type on comment grid
            uComments.GridUnHideCommentType();
            ContentPane p = (ContentPane)this.Parent;
            p.Header = "Customer-" + txtCustomerNum.Text;
            base.New();
            uGeneral.SetDefaultValues();
        }

        /// <summary>
        /// sets screen state for init
        /// </summary>
        private void setInitScreenState()
        {
            //don't allow edits on general tab fields
            this.uGeneral.IsEnabled = false;
            this.uAging.IsEnabled = false;
            this.uComments.IsEnabled = false;
            this.uContacts.IsEnabled = false;
            this.uAttachments.IsEnabled = false;
            this.uContracts.IsEnabled = false;
            this.uHistory.IsEnabled = false;
            txtCustomerNum.IsEnabled = true;
            txtCustomerName.IsEnabled = true;
        }

        /// <summary>
        /// sets screen state for insert
        /// </summary>
        private void setInsertScreenState()
        {
            //go to general tab
            General.Focus();
            //set focus to first field
            this.uGeneral.txtCustAcctName.Focus();
            //allow edits on general tab fields
            this.uGeneral.IsEnabled = true;
            //KSH 6.29.12 allow Natn'l Ad flag to be changed on new cust
            this.uGeneral.chkNAFlag.IsEnabled = true;
            this.uAging.IsEnabled = false;
            this.uComments.IsEnabled = false;
            this.uContacts.IsEnabled = false;
            this.uAttachments.IsEnabled = false;
            this.uContracts.IsEnabled = false;
            this.uHistory.IsEnabled = false;
            //don't allow header edits
            txtCustomerNum.IsEnabled = false;
            txtCustomerName.IsEnabled = false;


        }

        /// <summary>
        /// sets screen state for edit
        /// </summary>
        private void setEditScreenState()
        {
            //go to general tab
            //General.Focus();
            //set focus to first field
            //txtCustomerNum.Focus();
            //allow edits
            this.uGeneral.IsEnabled = true;
            //KSH 6.29.12 don't allow Natn'l Ad flag to be changed on new cust
            this.uGeneral.chkNAFlag.IsEnabled = false;
            this.uAging.IsEnabled = true;
            this.uComments.IsEnabled = true;
            this.uContacts.IsEnabled = true;
            this.uAttachments.IsEnabled = true;
            this.uContracts.IsEnabled = true;
            this.uHistory.IsEnabled = true;
            //enable header
            txtCustomerNum.IsEnabled = true;
            txtCustomerName.IsEnabled = true;
        }

        /// <summary>
        /// KSH - 8/21/12 clear comments/attachments grid to fix bug
        /// </summary>
        private void clrCommentsAttachmentsObj()
        {
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();

        }

    }
}

