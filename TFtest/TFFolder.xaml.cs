

#region using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RazerInterface;
using RazerBase;
using RazerBase.Lookups;
using RazerBase.Interfaces;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using Infragistics.Windows.Controls;
using System.Data;
using System.ComponentModel;


#endregion

namespace BCF
{

    #region class BCFFolder 
    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class BCFFolder : ScreenBase, IScreen 

    {
        private static readonly string approvalObjectName = "BCFApproval";
        private static readonly string documentIdParameter = "@document_id";
        public cBaseBusObject BCFFolderBusObject { get; private set; }
        private string BCFNumber { get; set; }


        private string windowCaption;
        //flag used to helps set screen state when inserting new recs
        public bool IsScreenInserting { get; set; }
        public bool notComingFromApproval = true;

        public int BCFStatus = 0;
        public string wfStatus;
        public string BCFType;
        public string adjTypeshort;
       
        public string WindowCaption
            
        {
            get { return windowCaption; }
        }
        
        
        /// </summary>
        public BCFFolder()
            : base()
        {
            // Create Controls
            InitializeComponent();

       
        }
        

        public void Init(cBaseBusObject businessObject)
        {
            // set the businessObject
            this.CurrentBusObj = businessObject;
            //this.CanExecuteRefreshCommand = false;
            //Setting intial screenstate to empty
            CurrentState = ScreenState.Empty;
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "general";
            this.DoNotSetDataContext = true;
            IsScreenInserting = false;
           
            //allow deletes
            this.CanExecuteDeleteCommand = true;
            // add the Tabs
            TabCollection.Add(BCFGeneralTab);
            TabCollection.Add(BCFDetailTab);
            TabCollection.Add(BCFAttachTab);
            TabCollection.Add(BCFCommentsTab);
            TabCollection.Add(BCFViewTab);
            TabCollection.Add(BCFApprovalTab);
            TabCollection.Add(BCFLocationsTab);


            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                BCFDetailTab.btnAutoCreate.IsEnabled = false;
                BCFLocationsTab.btnAutoCreate.IsEnabled = false;
               
            }
            else
            {
                BCFDetailTab.btnAutoCreate.IsEnabled = true;
                BCFLocationsTab.btnAutoCreate.IsEnabled = true;
            }
            // if there are parameters than we need to load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                

                //string BCFNumber = (from x in this.CurrentBusObj.Parms.ParmList.AsEnumerable()
                 //                      where x.Field<string>("ParmName") == "@bcf_number"
                 //                      select x.Field<string>("Value")).FirstOrDefault();

                string BCFNumber = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();

                //set document_id for View tab
                this.loadParms(BCFNumber);
                // load the data
                this.Load();
                // Set the Header
                //Need to chack 
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
                {
                    windowCaption = "BCF Number -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_number"].ToString();
                    txtBCFNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_number"].ToString();
                    BCFApprovalTab.BCFNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_number"].ToString();
                    //cmbBCFType.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_type"];
                    BCFGeneralTab.cmbProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString().Trim();
                    BCFStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]);
                    //BCFApprovalTab.BCFNumber = txtBCFNumber.Text;
                    //BCFApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                    wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["multiple_locations"].ToString() == "1")
                    {
                        BCFGeneralTab.txtcsID.IsEnabled = false;
                    }

                    BCFApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                    BCFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
                    BCFApprovalTab.ApprovalBusinessObject.Parms.AddParm(documentIdParameter, BCFNumber);
                    BCFApprovalTab.ApprovalBusinessObject.LoadData();
                    BCFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();

                 
                    if (BCFStatus.ToString() == "1" || wfStatus[0] == 'A')
                    {
                       //need to not allow changes
                        
                        //BCFGeneralTab.txtAdjReason.IsReadOnly = true;
                        ////need to set adjreason to enabled = false
                        //BCFGeneralTab.txtAdjReason.IsReadOnly = true;
                        BCFApprovalTab.btnSubmit.IsEnabled = false;
                        BCFApprovalTab.btnAddApprover.IsEnabled = false;
                        BCFApprovalTab.btnApprove.IsEnabled = false;
                        BCFApprovalTab.btnInquiry.IsEnabled = false;
                        BCFApprovalTab.btnReject.IsEnabled = false;
                        BCFApprovalTab.btnReply.IsEnabled = false;
                        BCFDetailTab.gBCFDetails.xGrid.FieldSettings.AllowEdit = false;
                        //BCFLocationsTab.gBCFLocations.IsEnabled = false;
                        BCFLocationsTab.btnAutoCreate.IsEnabled = false;
                        BCFLocationsTab.gBCFLocations.xGrid.FieldSettings.AllowEdit = false;
                        BCFLocationsTab.gBCFLocations.ContextMenuRemoveIsVisible = false;
                        BCFLocationsTab.gBCFLocations.ContextMenuAddIsVisible = false;
                        //BCFDetailTab.gBCFDetails.IsEnabled = false;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                        BCFDetailTab.btnAutoCreate.IsEnabled = false;
                        BCFDetailTab.gBCFDetails.ContextMenuAddIsVisible = false;
                        BCFDetailTab.gBCFDetails.ContextMenuRemoveIsVisible = false;
                        BCFGeneralTab.IsEnabled = false;
                        BCFGeneralTab.txtCustomerID.IsEnabled = false;
                        BCFGeneralTab.txtCustomerID.IsReadOnly = true;
                        BCFGeneralTab.txtCustomerName.IsReadOnly = true;
                        BCFGeneralTab.cmbBCFType.IsEnabled = false;
                        BCFGeneralTab.txtcsID.IsEnabled = false;
                        BCFGeneralTab.txtcsName.IsReadOnly = true;
                        BCFGeneralTab.txtMSOID.IsEnabled = false;
                        BCFGeneralTab.txtMSOName.IsReadOnly = true;
                        BCFGeneralTab.txtBCFDescription.IsEnabled = false;
                        BCFGeneralTab.txtContractID.IsEnabled = false;
                        BCFGeneralTab.txtContractExecutedFlag.IsEnabled = false;
                        BCFGeneralTab.txtDateRequested.IsEnabled = false;
                        BCFGeneralTab.txtBCFEffectiveDate.IsEnabled = false;
                        BCFGeneralTab.txtLastValidInvoicedMonth.IsEnabled = false;
                        BCFGeneralTab.txtSpecialComments.IsEnabled = false;
                        BCFGeneralTab.txtHEActive.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID1.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID2.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID3.IsEnabled = false;
                        BCFGeneralTab.cmbProductCode.IsEnabled = false;
                        BCFGeneralTab.txtPSACity.IsReadOnly = true;
                        BCFGeneralTab.txtPSAState.IsReadOnly = true;
                        BCFGeneralTab.cmbPSACountry.IsEnabled = false;
                        BCFGeneralTab.txtMultipleLocations.IsEnabled = false;
                        BCFGeneralTab.txtToCSID.IsEnabled = false;
                        BCFGeneralTab.cmbServiceType.IsEnabled = false;
                        BCFGeneralTab.txtToService.IsEnabled = false;
                        BCFGeneralTab.txtToHeadEndID.IsEnabled = false;
                        BCFGeneralTab.txtToMCA.IsEnabled = false;
                        BCFGeneralTab.txtContactName.IsEnabled = false;
                        BCFGeneralTab.txtContactPhone.IsEnabled = false;
                        BCFGeneralTab.txtContactEmail.IsEnabled = false;
                        BCFGeneralTab.txtNbrofSubs.IsEnabled = false;
                        BCFGeneralTab.gPNI.xGrid.FieldSettings.AllowEdit = false;
                        BCFGeneralTab.gPNI.ContextMenuAddIsVisible = false;
                        BCFGeneralTab.txtToHeadEndID.IsReadOnly = true;
                        BCFGeneralTab.txtToMCA.IsReadOnly = true;
                        BCFGeneralTab.txtContactName.IsReadOnly = true;
                        BCFGeneralTab.txtContactPhone.IsReadOnly = true;
                        BCFGeneralTab.txtContactEmail.IsReadOnly = true;


                       




                    }
                        else
                    {
                        //RES 10/8/13 phase 3.0.2 added to lock down in process BCF's
                        //Need to allow workflow buttons to work for in process
                        if (wfStatus[0] == 'I')
                        {
                            BCFApprovalTab.btnSubmit.IsEnabled = true;
                            BCFApprovalTab.btnAddApprover.IsEnabled = true;
                            BCFApprovalTab.btnApprove.IsEnabled = true;
                            BCFApprovalTab.btnInquiry.IsEnabled = true;
                            BCFApprovalTab.btnReject.IsEnabled = true;
                            BCFApprovalTab.btnReply.IsEnabled = true;
                            //BCFLocationsTab.gBCFLocations.IsEnabled = false;
                            BCFLocationsTab.gBCFLocations.xGrid.FieldSettings.AllowEdit = false;
                            BCFLocationsTab.btnAutoCreate.IsEnabled = false;
                            BCFDetailTab.gBCFDetails.xGrid.FieldSettings.AllowEdit = true;
                            //BCFDetailTab.gBCFDetails.IsEnabled = true;
                            BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = true;
                            BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                            BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                            BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                            BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                            BCFDetailTab.btnAutoCreate.IsEnabled = true;
                            BCFDetailTab.gBCFDetails.ContextMenuAddIsVisible = true;
                            BCFGeneralTab.IsEnabled = false;
                            //BCFGeneralTab.txtCustomerID.IsEnabled = false;
                            BCFGeneralTab.txtCustomerID.IsReadOnly = true;
                            BCFGeneralTab.txtCustomerName.IsReadOnly = true;
                            BCFGeneralTab.cmbBCFType.IsEnabled = false;
                          
                            BCFGeneralTab.txtcsID.IsEnabled = false;
                            BCFGeneralTab.txtcsName.IsReadOnly = true;
                            BCFGeneralTab.txtMSOID.IsEnabled = false;
                            BCFGeneralTab.txtMSOName.IsReadOnly = true;
                            BCFGeneralTab.txtBCFDescription.IsEnabled = false;
                            BCFGeneralTab.txtContractID.IsEnabled = false;
                            BCFGeneralTab.txtContractExecutedFlag.IsEnabled = false;
                            BCFGeneralTab.txtDateRequested.IsEnabled = false;
                            BCFGeneralTab.txtBCFEffectiveDate.IsEnabled = false;
                            BCFGeneralTab.txtLastValidInvoicedMonth.IsEnabled = false;
                            BCFGeneralTab.txtSpecialComments.IsEnabled = false;
                            BCFGeneralTab.txtHEActive.IsEnabled = false;
                            BCFGeneralTab.txtRemServiceID1.IsEnabled = false;
                            BCFGeneralTab.txtRemServiceID2.IsEnabled = false;
                            BCFGeneralTab.txtRemServiceID3.IsEnabled = false;
                            BCFGeneralTab.cmbProductCode.IsEnabled = false;
                            BCFGeneralTab.txtPSACity.IsReadOnly = true;
                            BCFGeneralTab.txtPSAState.IsReadOnly = true;
                            BCFGeneralTab.cmbPSACountry.IsEnabled = false;
                            BCFGeneralTab.txtMultipleLocations.IsEnabled = false;
                            BCFGeneralTab.txtToCSID.IsEnabled = false;
                            BCFGeneralTab.cmbServiceType.IsEnabled = false;
                            BCFGeneralTab.txtToService.IsEnabled = false;
                            BCFGeneralTab.txtNbrofSubs.IsEnabled = false;
                            BCFGeneralTab.gPNI.xGrid.FieldSettings.AllowEdit = false;
                            BCFGeneralTab.gPNI.ContextMenuAddIsVisible = false;
                            BCFGeneralTab.txtToHeadEndID.IsReadOnly = true;
                            BCFGeneralTab.txtToMCA.IsReadOnly = true;
                            BCFGeneralTab.txtContactName.IsReadOnly = true;
                            BCFGeneralTab.txtContactPhone.IsReadOnly = true;
                            BCFGeneralTab.txtContactEmail.IsReadOnly = true;

                        }
                    }


                    if (wfStatus[0] == 'N' || wfStatus[0] == 'R')
                    {
                        BCFApprovalTab.btnSubmit.IsEnabled = true;
                        BCFApprovalTab.btnAddApprover.IsEnabled = false;
                        BCFApprovalTab.btnApprove.IsEnabled = false;
                        BCFApprovalTab.btnInquiry.IsEnabled = false;
                        BCFApprovalTab.btnReject.IsEnabled = false;
                        BCFApprovalTab.btnReply.IsEnabled = false;
                        BCFDetailTab.gBCFDetails.xGrid.FieldSettings.AllowEdit = true;
                        BCFDetailTab.btnAutoCreate.IsEnabled = true;
                        BCFDetailTab.gBCFDetails.ContextMenuAddIsVisible = true;
                        BCFDetailTab.gBCFDetails.ContextMenuRemoveIsVisible = true;
                        BCFLocationsTab.gBCFLocations.ContextMenuAddIsVisible = true;
                        BCFLocationsTab.gBCFLocations.ContextMenuRemoveIsVisible = true;
                        BCFGeneralTab.txtCustomerID.IsEnabled = true;
                        BCFGeneralTab.txtCustomerID.IsReadOnly = true;
                        BCFGeneralTab.txtCustomerName.IsReadOnly = false;
                        BCFGeneralTab.cmbBCFType.IsEnabled = true;
                        BCFGeneralTab.txtcsID.IsEnabled = true;
                        BCFGeneralTab.txtcsName.IsReadOnly = false;
                        BCFGeneralTab.txtMSOID.IsEnabled = false;
                        BCFGeneralTab.txtMSOName.IsReadOnly = false;
                        BCFGeneralTab.txtBCFDescription.IsEnabled = true;
                        BCFGeneralTab.txtContractID.IsEnabled = true;
                        BCFGeneralTab.txtContractExecutedFlag.IsEnabled = true;
                        BCFGeneralTab.txtDateRequested.IsEnabled = true;
                        BCFGeneralTab.txtBCFEffectiveDate.IsEnabled = true;
                        BCFGeneralTab.txtLastValidInvoicedMonth.IsEnabled = true;
                        BCFGeneralTab.txtSpecialComments.IsEnabled = true;
                        BCFGeneralTab.txtHEActive.IsEnabled = true;
                        BCFGeneralTab.txtRemServiceID1.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID2.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID3.IsEnabled = false;
                        BCFGeneralTab.cmbProductCode.IsEnabled = true;
                        BCFGeneralTab.txtPSACity.IsReadOnly = false;
                        BCFGeneralTab.txtPSAState.IsReadOnly = false;
                        BCFGeneralTab.txtMultipleLocations.IsEnabled = true;
                        if (BCFGeneralTab.txtMultipleLocations.ToString() == "1")
                            BCFGeneralTab.txtcsID.IsEnabled = false;
                        else
                             BCFGeneralTab.txtcsID.IsEnabled = true;
                        BCFGeneralTab.txtToService.IsEnabled = true;
                        BCFGeneralTab.txtNbrofSubs.IsEnabled = true;
                        BCFGeneralTab.cmbPSACountry.IsEnabled = true;
                        BCFGeneralTab.cmbServiceType.IsEnabled = true;
                        BCFGeneralTab.txtToHeadEndID.IsEnabled = true;
                        BCFGeneralTab.txtToMCA.IsEnabled = true;
                        BCFGeneralTab.txtContactName.IsEnabled = true;
                        BCFGeneralTab.txtContactPhone.IsEnabled = true;
                        BCFGeneralTab.txtContactEmail.IsEnabled = true;
                        BCFGeneralTab.gPNI.xGrid.FieldSettings.AllowEdit = true;
                        BCFGeneralTab.gPNI.ContextMenuAddIsVisible = true;
                        BCFGeneralTab.txtToHeadEndID.IsReadOnly = false;
                        BCFGeneralTab.txtToMCA.IsReadOnly = false;
                        BCFGeneralTab.txtContactName.IsReadOnly = false;
                        BCFGeneralTab.txtContactPhone.IsReadOnly = false;
                        BCFGeneralTab.txtContactEmail.IsReadOnly = false;

                    }


                }
                else
                    Messages.ShowWarning("BCF not found!");
                if (this.CurrentBusObj.ObjectData.Tables["pni"].Rows.Count > 0)
                {

                }
                else
                    BCFGeneralTab.BCFpniClearGrid();

                if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)

                    BCFDetailTab.gBCFDetails.xGrid.FieldSettings.AllowEdit = false;

                if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0)
                {

                }
                else
                    BCFDetailTab.BCFdetailClearGrid();
                //Unhide comment type on comment grid
                //BCFCommentsTab.GridUnHideCommentType();

            }
            
            else
            {
                SetWindowStatus();
            }
            
        }

        void SetWindowStatus()
        {
            //don't allow edits on general tab fields
            this.BCFGeneralTab.IsEnabled = false;
            this.BCFDetailTab.IsEnabled = false;
            this.BCFCommentsTab.IsEnabled = true;
            this.BCFAttachTab.IsEnabled = true;
            this.BCFViewTab.IsEnabled = false;
            this.BCFApprovalTab.IsEnabled = false;
            txtBCFNumber.IsEnabled = true;
             
        }

        public void SetMultiLocation()
        {
            this.BCFGeneralTab.txtMultipleLocations.IsChecked = 1;

        }
       
        private void loadParms(string BCFNumber)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //if adjustment number passed load document id
                if (BCFNumber != "")
                {
                    this.CurrentBusObj.Parms.AddParm("@BCF_number", BCFNumber);
                    this.CurrentBusObj.Parms.AddParm("@external_char_id", BCFNumber);
                    
                }
                else
                {
                    //if adjustmentid NOT passed load   with global parm adjustmentid if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.AddParm("@BCF_number", cGlobals.ReturnParms[0].ToString());
                      
                        //for attachments
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", cGlobals.ReturnParms[0].ToString());
                    }
                    //set dummy vals
                    else
                    {
                        this.CurrentBusObj.Parms.AddParm("@BCF_number", "-1");
                     
                        //for attachments
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");
                    }
                }
                //comment tab parms
                this.CurrentBusObj.Parms.AddParm("@comment_type", "BC");
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
                //attachment parms
                this.CurrentBusObj.Parms.AddParm("@attachment_type", "BATTACH");
                this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@contract_id", "0");
                //this.CurrentBusObj.Parms.AddParm("@cs_id", "-1");
                BCFCommentsTab.CommentCode = "BC";
               
            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }


        private void BCFNumber_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            string currentBCF = "";
            if (txtBCFNumber.Text != "")
            {
                currentBCF = txtBCFNumber.Text;
                BCFNumber = txtBCFNumber.Text;
            }

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            BCFLookup f = new BCFLookup();
            f.Init(new cBaseBusObject("BCFLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtBCFNumber.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                if (txtBCFNumber.Text != BCFNumber)
                {
                    ReturnData(txtBCFNumber.Text, "@BCF_number");
                }
            }

            ////Event handles opening of the lookup window upon double click on BCFNumber field
            //BCFLookup f = new BCFLookup();



            //// gets the users response
            //f.ShowDialog();

            //// Check if a value is returned
            //if (cGlobals.ReturnParms.Count > 0)
            //{

            //    txtBCFNumber.Text = cGlobals.ReturnParms[0].ToString();
            //    cGlobals.ReturnParms.Clear();
            //    if (txtBCFNumber.Text != BCFNumber)
            //        ReturnData(txtBCFNumber.Text, "@BCF_number");


            //}

        }

        private void txtBCFNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            BCFNumber = txtBCFNumber.Text;
            //GeneralMain.Focus();

        }



        

        private void txtBCFNumber_LostFocus(object sender, RoutedEventArgs e)
        {
             if (txtBCFNumber.Text != BCFNumber)

                ReturnData(txtBCFNumber.Text, "@BCF_number");
                //GeneralMain.Focus();

             
        }
        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                BCFGeneralTab.NewFlag = false;
                //BCFStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]);
                BCFGeneralTab.LoadDDDWProductsbyContractID();
                BCFGeneralTab.cmbProductCode.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString().Trim();
                BCFGeneralTab.cmbProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"].ToString().Trim();
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                if (BCFStatus.ToString() == "1" || wfStatus[0] == 'A')
                {
                    
                   
                    BCFApprovalTab.btnSubmit.IsEnabled = false;
                    BCFApprovalTab.btnAddApprover.IsEnabled = false;
                    BCFApprovalTab.btnApprove.IsEnabled = false;
                    BCFApprovalTab.btnInquiry.IsEnabled = false;
                    BCFApprovalTab.btnReject.IsEnabled = false;
                    BCFApprovalTab.btnReply.IsEnabled = false;
                    //BCFLocationsTab.gBCFLocations.IsEnabled = false;
                    BCFLocationsTab.btnAutoCreate.IsEnabled = false;
                    BCFLocationsTab.gBCFLocations.xGrid.FieldSettings.AllowEdit = false;
                    BCFLocationsTab.gBCFLocations.ContextMenuRemoveIsVisible = false;
                    BCFLocationsTab.gBCFLocations.ContextMenuAddIsVisible = false;


                    BCFDetailTab.btnAutoCreate.IsEnabled = false;
                    BCFDetailTab.gBCFDetails.xGrid.FieldSettings.AllowEdit = false;
                    //BCFDetailTab.gBCFDetails.IsEnabled = false;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                   
                    BCFDetailTab.btnAutoCreate.IsEnabled = false;
                    BCFDetailTab.gBCFDetails.ContextMenuAddIsVisible = false;
                    BCFDetailTab.gBCFDetails.ContextMenuRemoveIsVisible = false;
                    BCFGeneralTab.IsEnabled = false;
                    BCFGeneralTab.txtCustomerID.IsReadOnly = true;
                    BCFGeneralTab.txtCustomerID.IsEnabled = false;
                    BCFGeneralTab.txtCustomerName.IsReadOnly = true;
                    BCFGeneralTab.cmbBCFType.IsEnabled = false;
                    BCFGeneralTab.txtcsID.IsReadOnly = true;
                    BCFGeneralTab.txtcsName.IsReadOnly = true;
                    BCFGeneralTab.txtMSOID.IsReadOnly = true;
                    BCFGeneralTab.txtMSOName.IsReadOnly = true;
                    BCFGeneralTab.txtBCFDescription.IsReadOnly = true;
                    BCFGeneralTab.txtContractID.IsReadOnly = true;
                    BCFGeneralTab.txtContractExecutedFlag.IsEnabled = false;
                    BCFGeneralTab.txtDateRequested.IsReadOnly = true;
                    BCFGeneralTab.txtBCFEffectiveDate.IsReadOnly = true;
                    BCFGeneralTab.txtLastValidInvoicedMonth.IsReadOnly = true;
                    BCFGeneralTab.txtSpecialComments.IsEnabled = false;
                    BCFGeneralTab.txtHEActive.IsEnabled = false;
                    BCFGeneralTab.txtRemServiceID1.IsEnabled = false;
                    BCFGeneralTab.txtRemServiceID2.IsEnabled = false;
                    BCFGeneralTab.txtRemServiceID3.IsEnabled = false;
                    BCFGeneralTab.cmbProductCode.IsEnabled = false;
                    BCFGeneralTab.txtPSACity.IsReadOnly = true;
                    BCFGeneralTab.txtPSAState.IsReadOnly = true;
                    BCFGeneralTab.cmbPSACountry.IsEnabled = false;
                    BCFGeneralTab.txtMultipleLocations.IsEnabled = false;
                    BCFGeneralTab.txtToCSID.IsEnabled = false;
                    BCFGeneralTab.txtToService.IsEnabled = false;
                    BCFGeneralTab.txtToHeadEndID.IsEnabled = false;
                    BCFGeneralTab.txtToMCA.IsEnabled = false;
                    BCFGeneralTab.txtContactName.IsEnabled = false;
                    BCFGeneralTab.txtContactPhone.IsEnabled = false;
                    BCFGeneralTab.txtContactEmail.IsEnabled = false;
                    BCFGeneralTab.txtNbrofSubs.IsEnabled = false;
                    BCFGeneralTab.cmbServiceType.IsEnabled = false;
                    BCFGeneralTab.gPNI.xGrid.FieldSettings.AllowEdit = false;
                    BCFGeneralTab.gPNI.ContextMenuAddIsVisible = false;
                    BCFGeneralTab.gPNI.IsEnabled = false;
                    BCFGeneralTab.txtToHeadEndID.IsReadOnly = true;
                    BCFGeneralTab.txtToMCA.IsReadOnly = true;
                    BCFGeneralTab.txtContactName.IsReadOnly = true;
                    BCFGeneralTab.txtContactPhone.IsReadOnly = true;
                    BCFGeneralTab.txtContactEmail.IsReadOnly = true;






                }
                else
                {
                    //BCFGeneralTab.txtAdjReason.IsReadOnly = false;
                }

                if (wfStatus[0] == 'N' || wfStatus[0] == 'R')
                {
                    BCFApprovalTab.btnSubmit.IsEnabled = true;
                    BCFApprovalTab.btnAddApprover.IsEnabled = false;
                    BCFApprovalTab.btnApprove.IsEnabled = false;
                    BCFApprovalTab.btnInquiry.IsEnabled = false;
                    BCFApprovalTab.btnReject.IsEnabled = false;
                    BCFApprovalTab.btnReply.IsEnabled = false;

                    BCFLocationsTab.gBCFLocations.IsEnabled = true;
                    BCFLocationsTab.btnAutoCreate.IsEnabled = true;
                    BCFLocationsTab.gBCFLocations.xGrid.FieldSettings.AllowEdit = true;


                    BCFDetailTab.gBCFDetails.xGrid.FieldSettings.AllowEdit = true;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = true;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                    BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                    BCFDetailTab.btnAutoCreate.IsEnabled = true;
                    BCFDetailTab.gBCFDetails.IsEnabled = true;
                    BCFDetailTab.btnAutoCreate.IsEnabled = true;
                    BCFDetailTab.gBCFDetails.ContextMenuAddIsVisible = true;
                    BCFDetailTab.gBCFDetails.ContextMenuRemoveIsVisible = true;
                    BCFGeneralTab.txtCustomerID.IsEnabled = true;
                    BCFGeneralTab.txtCustomerID.IsReadOnly = false;
                    BCFGeneralTab.txtCustomerName.IsReadOnly = false;
                    BCFGeneralTab.cmbBCFType.IsEnabled = true;
                    //BCFGeneralTab.txtcsID.IsReadOnly = false;
                    if (BCFGeneralTab.txtMultipleLocations.IsChecked == 1)
                    {
                        BCFGeneralTab.txtcsID.IsEnabled = false;
                    }
                    else
                    {
                        BCFGeneralTab.txtcsID.IsEnabled = true;
                    }
                    BCFGeneralTab.txtcsName.IsReadOnly = false;
                    //BCFGeneralTab.txtMSOID.IsReadOnly = false;
                    BCFGeneralTab.txtMSOID.IsEnabled = true;
                    BCFGeneralTab.txtMSOName.IsReadOnly = false;
                    BCFGeneralTab.txtBCFDescription.IsEnabled = true;
                    BCFGeneralTab.txtContractID.IsEnabled = true;
                    BCFGeneralTab.txtContractExecutedFlag.IsEnabled = true;
                    BCFGeneralTab.txtDateRequested.IsEnabled = true;
                    BCFGeneralTab.txtBCFEffectiveDate.IsEnabled = true;
                    BCFGeneralTab.txtLastValidInvoicedMonth.IsEnabled = true;
                    BCFGeneralTab.txtSpecialComments.IsEnabled = true;
                    BCFGeneralTab.txtHEActive.IsEnabled = true;
                    if (BCFGeneralTab.txtHEActive.IsChecked == 1)
                    {
                        BCFGeneralTab.txtRemServiceID1.IsEnabled = true;
                        BCFGeneralTab.txtRemServiceID2.IsEnabled = true;
                        BCFGeneralTab.txtRemServiceID3.IsEnabled = true;
                    }
                    else
                    {
                        BCFGeneralTab.txtRemServiceID1.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID2.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID3.IsEnabled = false;
                    }
                    BCFGeneralTab.txtMultipleLocations.IsEnabled = true;
                    BCFGeneralTab.txtToCSID.IsEnabled = true;
                    BCFGeneralTab.txtToService.IsEnabled = true;
                    BCFGeneralTab.txtNbrofSubs.IsEnabled = true;
                    BCFGeneralTab.cmbServiceType.IsEnabled = true;
                    BCFGeneralTab.cmbProductCode.IsEnabled = true;
                    BCFGeneralTab.txtPSACity.IsReadOnly = false;
                    BCFGeneralTab.txtPSAState.IsReadOnly = false;
                    BCFGeneralTab.cmbPSACountry.IsEnabled = true;
                    BCFGeneralTab.txtToHeadEndID.IsEnabled = true;
                    BCFGeneralTab.txtToMCA.IsEnabled = true;
                    BCFGeneralTab.txtContactName.IsEnabled = true;
                    BCFGeneralTab.txtContactPhone.IsEnabled = true;
                    BCFGeneralTab.txtContactEmail.IsEnabled = true;
                    BCFGeneralTab.gPNI.xGrid.FieldSettings.AllowEdit = true;
                    BCFGeneralTab.gPNI.ContextMenuAddIsVisible = true;
                    BCFGeneralTab.gPNI.IsEnabled = true;
                    BCFGeneralTab.txtToHeadEndID.IsReadOnly = false;
                    BCFGeneralTab.txtToMCA.IsReadOnly = false;
                    BCFGeneralTab.txtContactName.IsReadOnly = false;
                    BCFGeneralTab.txtContactPhone.IsReadOnly = false;
                    BCFGeneralTab.txtContactEmail.IsReadOnly = false;



                }
                else
                {

                    //Need to allow workflow buttons to work for in process
                    if (wfStatus[0] == 'I')
                    {
                        BCFApprovalTab.btnSubmit.IsEnabled = true;
                        BCFApprovalTab.btnAddApprover.IsEnabled = true;
                        BCFApprovalTab.btnApprove.IsEnabled = true;
                        BCFApprovalTab.btnInquiry.IsEnabled = true;
                        BCFApprovalTab.btnReject.IsEnabled = true;
                        BCFApprovalTab.btnReply.IsEnabled = true;
                        //BCFLocationsTab.gBCFLocations.IsEnabled = false;
                        BCFLocationsTab.btnAutoCreate.IsEnabled = false;
                        BCFLocationsTab.gBCFLocations.xGrid.FieldSettings.AllowEdit = false;
                        BCFDetailTab.gBCFDetails.xGrid.FieldSettings.AllowEdit = true;
                        //BCFDetailTab.gBCFDetails.IsEnabled = true;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = true;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                        BCFDetailTab.gBCFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                        BCFDetailTab.btnAutoCreate.IsEnabled = true;
                        BCFDetailTab.gBCFDetails.ContextMenuAddIsVisible = true;
                        BCFDetailTab.gBCFDetails.ContextMenuRemoveIsVisible = true;
                        BCFGeneralTab.IsEnabled = false;
                        BCFGeneralTab.txtCustomerID.IsEnabled = false;
                        BCFGeneralTab.txtCustomerID.IsReadOnly = true;
                        BCFGeneralTab.txtCustomerName.IsReadOnly = true;
                        BCFGeneralTab.cmbBCFType.IsEnabled = false;
                        BCFGeneralTab.txtcsID.IsEnabled = false;
                        BCFGeneralTab.txtcsName.IsReadOnly = true;
                        BCFGeneralTab.txtMSOID.IsEnabled = false;
                        BCFGeneralTab.txtMSOName.IsReadOnly = true;
                        BCFGeneralTab.txtBCFDescription.IsEnabled = false;
                        BCFGeneralTab.txtContractID.IsEnabled = false;
                        BCFGeneralTab.txtContractExecutedFlag.IsEnabled = false;
                        BCFGeneralTab.txtDateRequested.IsEnabled = false;
                        BCFGeneralTab.txtBCFEffectiveDate.IsEnabled = false;
                        BCFGeneralTab.txtLastValidInvoicedMonth.IsEnabled = false;
                        BCFGeneralTab.txtSpecialComments.IsEnabled = false;
                        BCFGeneralTab.txtHEActive.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID1.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID2.IsEnabled = false;
                        BCFGeneralTab.txtRemServiceID3.IsEnabled = false;
                        BCFGeneralTab.cmbProductCode.IsEnabled = false;
                        BCFGeneralTab.txtPSACity.IsReadOnly = true;
                        BCFGeneralTab.txtPSAState.IsReadOnly = true;
                        BCFGeneralTab.cmbPSACountry.IsEnabled = false;
                        BCFGeneralTab.txtMultipleLocations.IsEnabled = false;
                        BCFGeneralTab.txtToCSID.IsEnabled = false;
                        BCFGeneralTab.cmbServiceType.IsEnabled = false;
                        BCFGeneralTab.txtToService.IsEnabled = false;
                        BCFGeneralTab.txtNbrofSubs.IsEnabled = false;
                        BCFGeneralTab.gPNI.xGrid.FieldSettings.AllowEdit = false;
                        BCFGeneralTab.gPNI.ContextMenuAddIsVisible = false;
                        BCFGeneralTab.txtToHeadEndID.IsReadOnly = true;
                        BCFGeneralTab.txtToMCA.IsReadOnly = true;
                        BCFGeneralTab.txtContactName.IsReadOnly = true;
                        BCFGeneralTab.txtContactPhone.IsReadOnly = true;
                        BCFGeneralTab.txtContactEmail.IsReadOnly = true;
                    }
                }





                return true;
            }
            else
            {
                Messages.ShowWarning("BCF Not Found");

                return false;
            }
        }

        public void ReturnData(string SearchValue, string DbParm)
        {
            //This section will check to see if changes have been made and if save is desired in the event of a
            //double click lookup or a change of the contract id field.
            //Verify that no save is needed
            Prep_ucBaseGridsForSave();
            PrepareFreeformForSave();
            if (IsScreenDirty  && notComingFromApproval)
            {
                //Establish a temporary BCFNumber for storing the ID the user wanted to go to.  This will be used in the final retrieval in the event of a
                //Yes or No answer to the question below.
                String NewBCFNumber = "";
                System.Windows.MessageBoxResult result = Messages.ShowYesNoCancel("Would you like to save existing changes?",
                           System.Windows.MessageBoxImage.Question);
                //Save existing BCF information and then load BCF customer
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    NewBCFNumber = txtBCFNumber.Text;
                    Save();
                    //If Save fails then reset BCF Number to original value and exit retrieve so that changes will not be lost.
                    if (!SaveSuccessful)
                    {
                        txtBCFNumber.Text = BCFNumber;
                        
                    }
                    else if (NewBCFNumber != "")
                    {
                        txtBCFNumber.Text = NewBCFNumber.ToString();
                        ReturnData(txtBCFNumber.Text, "@BCF_number");
                    }
                }
                //Returns the user to the current BCF window and resets the txtBCFNumber field to original value.
                else if (result == System.Windows.MessageBoxResult.Cancel)
                {
                    txtBCFNumber.Text = BCFNumber;
                    return;

                }
            }

            //notComingFromApproval = true;

            //if no value do nothing
            if (SearchValue == "") return;
            //Add new parameters
            loadParms(SearchValue);
            //KSH - 8/24/12 clear comments/attachments grid if applicable bus obj
            clrCommentsAttachmentsObj();
            //load data
            //if coming from save do not do this...
            this.Load();
            //if BCFNumber found then set header and pop otherwise send message
            //if customer number found then set header and pop otherwise send message
            if (chkForData())
            {

                if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0)
                {

                }
                else
                
                    BCFDetailTab.BCFdetailClearGrid();
                    SetHeaderName();
                    setEditScreenState();
                    //Adding this to see if it helps with the SAVE issue
                    BCFNumber = txtBCFNumber.Text;
                    BCFApprovalTab.BCFNumber = txtBCFNumber.Text;
                    BCFApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
               
                //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count >  0)
                //{
                //    this.CurrentBusObj.Parms.UpdateParmValue("@cs_id",this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["cs_id"]);
                     
                //}
            }
            //Unhide comment type on comment grid
            //BCFCommentsTab.GridUnHideCommentType();

            //Empty out the invoice text box on the detail tab
            BCFDetailTab.txtLocationInvoice.Text = "";
            
        }


       

        
        /// <summary>
        private void SetHeaderName()
        {
            ContentPane p = (ContentPane)this.Parent;
            //Sets the header name when being called from another folder
            if (txtBCFNumber.Text == null)
            {
                windowCaption = "BCF Number -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_number"].ToString();
                txtBCFNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_number"].ToString();
               
               //cmbBCFType.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_type"];



            }
            ////Sets the header name from within same folder
            else
            {
                //ContentPane p = (ContentPane)this.Parent;
                //p.Header = "Adjustment ID -" + txtAdjustmentID.Text;
                txtBCFNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_number"].ToString();
                p.Header = "BCF Number -" + txtBCFNumber.Text;
                //cmbBCFType.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_type"];

            }
        }

       

      

        private void clrCommentsAttachmentsObj()
        {
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();
        }
        private void txtBCFNumber_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public override void Delete()
        {

            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                BCFStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_status_flag"]);
                //if (AdjustmentID == null)
                BCFNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"].ToString();
                //If posted cannot delete adjustment
                if (BCFStatus.ToString() == "1")
                    Messages.ShowWarning("Cannot delete a BCF that has been closed");
                else
                {


                    MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to delete BCF " + BCFNumber.ToString() + "?", System.Windows.MessageBoxImage.Question);


                    if (result == MessageBoxResult.Yes)
                    {

                        if (cGlobals.BillService.DeleteBCF(BCFNumber) == true)
                        {
                            Messages.ShowWarning("BCF Number " + BCFNumber + " Deleted");
                            //KSH - 8/24/12 clear comments/attachments grid if applicable bus obj
                            clrCommentsAttachmentsObj();
                            this.Load();
                            txtBCFNumber.Text = "";

                            ContentPane p = (ContentPane)this.Parent;
                            p.Header = "BCF Number";

                        }
                        else
                            Messages.ShowWarning("Error Deleting BCF");
                    }
                    else
                    {

                        Messages.ShowMessage("BCF not deleted", MessageBoxImage.Information);
                    }
                }
            }
            else
                Messages.ShowWarning("No BCF to delete");
        }
     
            
       

        public override void Save()
        {
            //call the validation on the General Tab
            BCFGeneralTab.ValidatebeforeSave();
            if (BCFGeneralTab.errorsExist != true)
            {
                this.Cursor = Cursors.Wait;
                base.Save();
               
                if (SaveSuccessful)
                {




                    var localBCFInfo = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                       where x.Field<string>("parmName") == "@BCF_number"
                                       select new
                                       {
                                           parmName = x.Field<string>("parmName"),
                                           parmValue = x.Field<string>("parmValue")
                                       };

                    foreach (var info in localBCFInfo)
                    {
                        if (info.parmName == "@BCF_number")
                            txtBCFNumber.Text = info.parmValue;
                        

                    }

                   
                    if (IsScreenInserting == false)
                    {
                        //if contact id found then set header and pop otherwise send message
                        if (chkForData()) SetHeaderName();
                    }
                    else
                    {
                        //Insert has occurred
             
                        //ReturnData(txtBCFNumber.Text, "@BCF_number");
                        txtBCFNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"].ToString();
                        BCFNumber = txtBCFNumber.Text;
                        this.CurrentBusObj.ObjectData.Tables["ParmTable"].Rows[1]["parmValue"] = BCFNumber.ToString();
                        //turn off inserting flag
                        IsScreenInserting = false;
                        loadParms(BCFNumber);
                      
                        Messages.ShowInformation("New BCF - " + BCFNumber.ToString() + " Save Successful.");
                         
                        //reset business obj for load of new contact
                        //this.CurrentBusObj.ObjectData = null;
                        ////load new BCF
                        //this.loadParms(BCFNumber);
                        //// load the data
                        //txtBCFNumber.Text = BCFNumber;
                        //this.Load();
                        BCFApprovalTab.IsEnabled = true;
                        BCFApprovalTab.btnSubmit.IsEnabled = true;
                        BCFApprovalTab.btnApprove.IsEnabled = false;
                        BCFApprovalTab.btnReject.IsEnabled = false;
                        BCFApprovalTab.btnAddApprover.IsEnabled = false;
                        BCFApprovalTab.btnInquiry.IsEnabled = false;
                        BCFApprovalTab.btnReply.IsEnabled = false;
                        BCFApprovalTab.BCFNumber = BCFNumber;
                        BCFApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                        BCFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
                        BCFApprovalTab.ApprovalBusinessObject.Parms.AddParm(documentIdParameter, BCFNumber);
                        BCFApprovalTab.ApprovalBusinessObject.LoadData();
                        BCFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
                        this.CurrentBusObj.changeParm("@external_char_id", BCFNumber);
                        //turn off inserting flag
                        IsScreenInserting = false;
                    }
                    Messages.ShowInformation("Save Successful");
                }
            }
                else
                {
                    Messages.ShowInformation("Save Failed");
                    SaveSuccessful = false;
                    //need to cleanup if a detail or pni row were inserted
                    cBaseBusObject BCFVerification = new cBaseBusObject("BCFVerification");

                    BCFVerification.Parms.ClearParms();
                    BCFVerification.Parms.AddParm("@BCF_number", BCFNumber);
                    BCFVerification.LoadTable("cleanup");
                    GeneralMain.Focus();
                    return;

                    
                    
                }
                this.Cursor = Cursors.Arrow;
            }
       

        /// <summary>
        /// Override of New command
        /// </summary>
        public override void New()
        {
            IsScreenInserting = true;
            
            //Set the screen state to inserting
            CurrentState = ScreenState.Inserting;

           
            //set focus to general tab
            setInsertScreenState();
            
            //If inserting to the screen then we will pass a dummy parm so that we can return empty tables
            if (CurrentState == ScreenState.Inserting)
            {
              //    cBaseBusObject BCFNextNbr = new cBaseBusObject("BCFNextNbr");

              //   BCFNextNbr.Parms.ClearParms();
              //   BCFNextNbr.Parms.AddParm("@BCF_number", " ");
              //   BCFNextNbr.LoadTable("nextNbr");
              //  if (BCFNextNbr.ObjectData.Tables["nextNbr"] == null || BCFNextNbr.ObjectData.Tables["nextNbr"].Rows.Count < 1)
              //   {
              //         Messages.ShowInformation("Could not retrieve next BCF Number");
              //         txtBCFNumber.Text = "0";
              //         return;
               
              //   }
              //else
              // {
                  
              //    BCFNumber = BCFNextNbr.ObjectData.Tables["nextNbr"].Rows[0]["BCF_Number"].ToString();
                 
              //    BCFGeneralTab.BCFNumber = BCFNumber;
              //    BCFApprovalTab.BCFNumber = BCFNumber;
              //      //have to do this to clear the grid on any previous BCFs viewed
              //    if (BCFApprovalTab.ApprovalBusinessObject != null)
              //    {
              //        BCFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
              //        BCFApprovalTab.ApprovalBusinessObject.Parms.AddParm(documentIdParameter, BCFNumber);
              //        BCFApprovalTab.ApprovalBusinessObject.LoadData();
              //        BCFApprovalTab.idgAdjustments.LoadGrid(BCFApprovalTab.ApprovalBusinessObject, "approval");
              //        BCFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
              //    }

               
              
                  txtBCFNumber.Text = "NEW";
                  BCFNumber = "NEW";
           
                //Remove any existing parameters
                CurrentBusObj.Parms.ClearParms();
                //Add all parameters back in
                CurrentBusObj.Parms.AddParm("@BCF_number", "");
                this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@comment_type", "BC");
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
                //attachment parms
                this.CurrentBusObj.Parms.AddParm("@attachment_type", "BATTACH");
                this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@contract_id", "0");
          
                //this.CurrentBusObj.Parms.AddParm("@cs_id", "-1");
               
            
            
          
           
           

            base.New();
            ContentPane p = (ContentPane)this.Parent;
            p.Header = "BCFNumber -" + BCFNumber.ToString();
            txtBCFNumber.Text = BCFNumber.ToString();
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_number"] = BCFNumber.ToString();
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"] = 0;
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_code"] = "";
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["product_description"] = "";
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_status_flag"] = 0;
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["headends_active_flag"] = 0;
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contract_executed_flag"] = 0;
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["special_comments"] = "";
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_flag"] = 0;
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["multiple_locations"] = 0;
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contact_name"] = "";
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contact_phone"] = "";
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contact_email"] = "";
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["to_head_id"] = 0;
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["to_service_id"] = 0;
            this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"] = "Not Submitted For Approval"; 


            
            CurrentBusObj.Parms.UpdateParmValue("@BCF_number", BCFNumber.ToString());
            CurrentBusObj.Parms.UpdateParmValue("@external_char_id", BCFNumber.ToString());
            BCFGeneralTab.SetDefaultValues();
            this.clrCommentsAttachmentsObj();
                //need to clear grids

            if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0)
            {
                BCFDetailTab.txtLocationInvoice.Text = "";
                BCFDetailTab.BCFdetailClearGrid();
            }

            if (this.CurrentBusObj.ObjectData.Tables["locations"].Rows.Count > 0)
            {
                BCFLocationsTab.txtLocation.Text = "";
                BCFLocationsTab.BCFlocationsClearGrid();
            }
            if (this.CurrentBusObj.ObjectData.Tables["view"].Rows.Count > 0)
                this.CurrentBusObj.ObjectData.Tables["view"].Rows.Clear();
            if (this.CurrentBusObj.ObjectData.Tables["pni"].Rows.Count > 0)
                this.CurrentBusObj.ObjectData.Tables["pni"].Rows.Clear();
            if (this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Rows.Count > 0)
                this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Rows.Clear();
            if (this.CurrentBusObj.ObjectData.Tables["comments_char"].Rows.Count > 0)
                this.CurrentBusObj.ObjectData.Tables["comments_char"].Rows.Clear();
            if (this.CurrentBusObj.ObjectData.Tables["attachments"].Rows.Count > 0)
                this.CurrentBusObj.ObjectData.Tables["attachments"].Rows.Clear();
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();

            BCFGeneralTab.cmbBCFType.Focus();   
            

            }
        }

        /// <summary>
        /// sets screen state for init
        /// </summary>
        private void setInitScreenState()
        {
            //don't allow edits on general tab fields
            this.BCFGeneralTab.IsEnabled = false;
            this.BCFDetailTab.IsEnabled = false;
            //don't allow header edits
            txtBCFNumber.IsEnabled = true;
            //cmbBCFType.IsEnabled = true;
        }

        /// <summary>
        /// sets screen state for insert
        /// </summary>
        private void setInsertScreenState()
        {
            //go to general tab
            BCFGeneralTab.Focus();
            //set focus to first field
            //cmbBCFType.Focus();
            //allow edits on general tab fields
            this.BCFGeneralTab.IsEnabled = true;
            this.BCFDetailTab.IsEnabled = true;
            this.BCFCommentsTab.IsEnabled = false;
            this.BCFAttachTab.IsEnabled = false;
            this.BCFViewTab.IsEnabled = false;
            this.BCFApprovalTab.IsEnabled = false;
            //don't allow header edits
            txtBCFNumber.IsEnabled = true;
            //cmbBCFType.IsEnabled = true;
            BCFDetailTab.gBCFDetails.xGrid.FieldSettings.AllowEdit = true;
            BCFDetailTab.gBCFDetails.IsEnabled = true;
            BCFDetailTab.txtLocationInvoice.Text = "";
            BCFDetailTab.btnAutoCreate.IsEnabled = true;
            BCFDetailTab.gBCFDetails.ContextMenuAddIsVisible = true;
           
            BCFGeneralTab.cmbBCFType.IsEnabled = true;
            BCFGeneralTab.txtContractID.IsReadOnly = false;
            BCFGeneralTab.cmbProductCode.IsEnabled = true;
            BCFGeneralTab.txtcsID.IsReadOnly = false;
            BCFGeneralTab.txtcsName.IsReadOnly = false;
            BCFGeneralTab.txtPSACity.IsReadOnly = false;
            BCFGeneralTab.txtPSAState.IsReadOnly = false;
            BCFGeneralTab.cmbPSACountry.IsEnabled = true;
            BCFGeneralTab.txtMSOID.IsReadOnly = false;
            BCFGeneralTab.txtMSOName.IsReadOnly = false;
            BCFGeneralTab.txtCustomerID.IsReadOnly = false;
            BCFGeneralTab.txtCustomerID.IsEnabled = true;
            BCFGeneralTab.txtCustomerName.IsReadOnly = false;
            BCFGeneralTab.txtBCFDescription.IsReadOnly = false;
           
            BCFGeneralTab.txtContractExecutedFlag.IsEnabled = true;
            BCFGeneralTab.txtDateRequested.IsReadOnly = false;
            BCFGeneralTab.txtBCFEffectiveDate.IsReadOnly = false;
            BCFGeneralTab.txtLastValidInvoicedMonth.IsReadOnly = false;
            BCFGeneralTab.txtSpecialComments.IsEnabled = true;
            BCFGeneralTab.txtHEActive.IsEnabled = true;
            BCFGeneralTab.txtRemServiceID1.IsEnabled = false;
            BCFGeneralTab.txtRemServiceID2.IsEnabled = false;
            BCFGeneralTab.txtRemServiceID3.IsEnabled = false;
            BCFGeneralTab.txtMultipleLocations.IsEnabled = true;
            BCFGeneralTab.txtToCSID.IsEnabled = true;
            BCFGeneralTab.txtToService.IsEnabled = true;
            BCFGeneralTab.txtNbrofSubs.IsEnabled = true;
            BCFGeneralTab.cmbServiceType.IsEnabled = true;
            BCFGeneralTab.txtContactName.IsEnabled = true;
            BCFGeneralTab.txtContactPhone.IsEnabled = true;
            BCFGeneralTab.txtContactEmail.IsEnabled = true;
            BCFGeneralTab.txtToHeadEndID.IsEnabled = true;
            BCFGeneralTab.txtToMCA.IsEnabled = true;
            BCFGeneralTab.gPNI.xGrid.FieldSettings.AllowEdit = true;
            BCFGeneralTab.gPNI.ContextMenuAddIsVisible = true;
            BCFGeneralTab.gPNI.IsEnabled = true;
            BCFGeneralTab.txtToHeadEndID.IsReadOnly = false;
            BCFGeneralTab.txtToMCA.IsReadOnly = false;
            BCFGeneralTab.txtContactName.IsReadOnly = false;
            BCFGeneralTab.txtContactPhone.IsReadOnly = false;
            BCFGeneralTab.txtContactEmail.IsReadOnly = false;
           
        }

        /// <summary>
        /// sets screen state for edit
        /// </summary>
        private void setEditScreenState()
        {
            //go to general tab
            //allow edits
            this.BCFGeneralTab.IsEnabled = true;
            this.BCFDetailTab.IsEnabled = true;
            this.BCFCommentsTab.IsEnabled = true;
            this.BCFAttachTab.IsEnabled = true;
            this.BCFViewTab.IsEnabled = true;
            this.BCFApprovalTab.IsEnabled = true;
            //enable header
            txtBCFNumber.IsEnabled = true;
            //cmbBCFType.IsEnabled = true;
            
        }

        private void tcMainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object source = e.Source;



           //need to re-retrieve if the status changed




            if (e.RemovedItems.Count > 0)
            {
                if (e.RemovedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.RemovedItems[0]).Header.Equals("Approval"))
                {
                    //Only need to Load data if an invoice has already been saved.
                    //This is to display the correct workflow status on the general tab if it has changed since the inoice was
                    //was created or loaded on the general tab.

                    notComingFromApproval = false; 
                    ReturnData(BCFNumber, "@BCF_number");
                    notComingFromApproval = true; 


                }


                if (e.RemovedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.RemovedItems[0]).Header.Equals("Detail"))
                {
                    //Only need to Load data if an invoice has already been saved.
                    //This is to display the correct workflow status on the general tab if it has changed since the inoice was
                    //was created or loaded on the general tab.


                    if (this.CurrentBusObj != null)
                    {
                        //this.CurrentBusObj.ObjectData.Tables["detail"].AcceptChanges();
                        this.CurrentBusObj.SaveTable("detail");
                    }
                      


                }

                if (e.RemovedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.RemovedItems[0]).Header.Equals("Locations"))
                {
                    //Only need to Load data if an invoice has already been saved.
                    //This is to display the correct workflow status on the general tab if it has changed since the inoice was
                    //was created or loaded on the general tab.

                    //notComingFromApproval = false;

                    if (this.CurrentBusObj != null)
                    {
                        //this.CurrentBusObj.ObjectData.Tables["locations"].AcceptChanges();
                        //this.CurrentBusObj.ObjectData.Tables["locations_pni"].AcceptChanges();
                        this.CurrentBusObj.SaveTable("locations");
                        this.CurrentBusObj.SaveTable("locations_pni");
                    }
                      


                }

                if (e.RemovedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.RemovedItems[0]).Header.Equals("General"))
                {
                    //Only need to Load data if an invoice has already been saved.
                    //This is to display the correct workflow status on the general tab if it has changed since the inoice was
                    //was created or loaded on the general tab.
                    if (IsScreenInserting)
                    {
                        Save();
                        if (SaveSuccessful)
                        {
                            if (CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                            {
                                BCFNumber = CurrentBusObj.ObjectData.Tables["general"].Rows[0]["BCF_Number"].ToString();
                                txtBCFNumber.Text = BCFNumber;
                                CurrentBusObj.Parms.UpdateParmValue("@BCF_number", BCFNumber.ToString());
                                CurrentBusObj.Parms.UpdateParmValue("@external_char_id", BCFNumber.ToString());

                            }

                            Messages.ShowInformation("New BCF - " + BCFNumber.ToString() + " Save Successful.");
                            IsScreenInserting = false;
                          
                        }
                        else
                        {
                            GeneralMain.Focus();
                            return;
                        }
                    }
                    else
                    {
                       
                    }
                }
            }




        }


       
    }
    #endregion

}
