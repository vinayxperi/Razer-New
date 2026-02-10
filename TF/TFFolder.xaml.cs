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


namespace TF
{
    /// <summary>
    /// Interaction logic for TFFolder.xaml
    /// </summary>
    public partial class TFFolder : ScreenBase, IScreen
    {
        private static readonly string approvalObjectName = "TFApproval";
        private static readonly string documentIdParameter = "@document_id";
        public cBaseBusObject TFFolderBusObject { get; private set; }
        private string TFNumber { get; set; }


        private string windowCaption;
        //flag used to helps set screen state when inserting new recs
        public bool IsScreenInserting { get; set; }
        public bool notComingFromApproval = true;

        public int TFStatus = 0;
        public string wfStatus;
        public string TFType;
        public string adjTypeshort;
        //public bool CalledFromAnotherFolder = false;

        public string WindowCaption
        {
            get { return windowCaption; }
        }

        public TFFolder()
            : base()
        {
             InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty Folder ");
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
            TabCollection.Add(TFGeneralTab);
            TabCollection.Add(TFAttachTab);
            TabCollection.Add(TFDetailTab);
            TabCollection.Add(TFCreditTab);
            TabCollection.Add(TFCommentsTab);
            TabCollection.Add(TFViewTab);
            TabCollection.Add(TFApprovalTab);
            TabCollection.Add(TFLocationsTab);

            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {

                string TFNumber = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();

                ////set document_id for View tab
                this.loadParms(TFNumber);
                // load the data
                this.Load();
                //CalledFromAnotherFolder = true;
                //txtTFNumber.Text = TFNumber;
                //ReturnData(txtTFNumber.Text, "@tf_number");
                //CalledFromAnotherFolder = false;
                //TFLocationsTab.TFShowActiveDelegate();
                // Set the Header
                //Need to chack 
                //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
                //{
                //    TFLocationsTab.txtOldOwnerMSOID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_id"].ToString();
                //    TFLocationsTab.txtOldOwnerMSOName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_name"].ToString();
                //    TFApprovalTab.TFNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                //}
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
                {
                    TFGeneralTab.NewFlag = false;
                    windowCaption = "TF Number -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                    txtTFNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                    //txtTFDesc.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_description"].ToString();
                    txtTFDesc.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_description"].ToString();
                    TFLocationsTab.txtOldOwnerMSOID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_id"].ToString();
                    TFLocationsTab.txtOldOwnerMSOName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_name"].ToString();
                    TFApprovalTab.TFNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                    //TFLocationsTab.TFShowActiveDelegate();
                    //TFLocationsTab.txtOldOwnerMSOID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_id"].ToString();
                    //TFLocationsTab.txtOldOwnerMSOName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_name"].ToString();
                    //TFGeneralTab.cmbNewProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_product_code"].ToString().Trim();
                    //TFGeneralTab.cmbOldProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"].ToString().Trim();
                    TFStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_status_flag"]);
                    //BCFApprovalTab.BCFNumber = txtBCFNumber.Text;
                    //BCFApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                    wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                    string Approver = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approver"].ToString();
                    //string RoleID = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString();
                    TFApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                    TFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
                    TFApprovalTab.ApprovalBusinessObject.Parms.AddParm(documentIdParameter, TFNumber);
                    TFApprovalTab.ApprovalBusinessObject.LoadData();
                    TFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();


                    if (TFStatus.ToString() == "1" || wfStatus[0] == 'A')
                    {
                        //need to not allow changes
                        TFApprovalTab.btnSubmit.IsEnabled = false;
                        TFApprovalTab.btnAddApprover.IsEnabled = false;
                        TFApprovalTab.btnApprove.IsEnabled = false;
                        TFApprovalTab.btnInquiry.IsEnabled = false;
                        TFApprovalTab.btnReject.IsEnabled = false;
                        TFApprovalTab.btnReply.IsEnabled = false;

                        //TFGeneralTab.IsEnabled = false;
                        TFGeneralTab.txtDateRequested.IsEnabled = false;
                        TFGeneralTab.txtTFEffectiveDate.IsEnabled = false;
                        TFGeneralTab.txtOldOwnerMSOID.IsEnabled = false;
                        TFGeneralTab.txtOldOwnerMSOName.IsEnabled = false;
                        TFGeneralTab.txtOldBillMSOID.IsEnabled = false;
                        TFGeneralTab.txtOldBillMSOName.IsEnabled = false;
                        TFGeneralTab.txtOldSystemPhone.IsEnabled = false;
                        TFGeneralTab.txtOldSystemContact.IsEnabled = false;
                        TFGeneralTab.txtNewOwnerMSOID.IsEnabled = false;
                        TFGeneralTab.txtNewOwnerMSOName.IsEnabled = false;
                        TFGeneralTab.txtNewBillMSOID.IsEnabled = false;
                        TFGeneralTab.txtNewBillMSOName.IsEnabled = false;
                        TFGeneralTab.txtNewSystemPhone.IsEnabled = false;
                        TFGeneralTab.txtNewSystemContact.IsEnabled = false;
                        TFGeneralTab.txtSpecialComments.IsEnabled = false;
                        TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;

                        TFLocationsTab.gTFLocations.IsEnabled = false;
                        //TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = true;
                        //TFLocationsTab.gTFLocations.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                        TFLocationsTab.gTFLocationsPNI.IsEnabled = false;
                        //TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = false;
                        //TFLocationsTab.gTFLocationsPNI.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                        TFLocationsTab.chkSelectAll.IsEnabled = false;

                        TFDetailTab.gTFDetails.IsEnabled = false;
                        //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                        //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                        //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                        //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                        //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                        TFDetailTab.btnAutoCreate.IsEnabled = false;
                        TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                        TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = false;

                        TFCreditTab.gTFCredit.IsEnabled = false;
                        //TFCreditTab.gTFCredit.IsEnabled = true;
                        //TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = false;
                        TFCreditTab.btnCopyAll.IsEnabled = false;

                    }
                    else
                    {
                        //RES 10/8/13 phase 3.0.2 added to lock down in process BCF's
                        //Need to allow workflow buttons to work for in process
                        if (wfStatus[0] == 'I')
                        {
                            txtTFDesc.IsEnabled = false;

                            TFApprovalTab.btnSubmit.IsEnabled = true;
                            TFApprovalTab.btnAddApprover.IsEnabled = true;
                            TFApprovalTab.btnApprove.IsEnabled = true;
                            TFApprovalTab.btnInquiry.IsEnabled = true;
                            TFApprovalTab.btnReject.IsEnabled = true;
                            TFApprovalTab.btnReply.IsEnabled = true;

                            //TFGeneralTab.IsEnabled = false;
                            TFGeneralTab.txtDateRequested.IsEnabled = false;
                            TFGeneralTab.txtTFEffectiveDate.IsEnabled = false;
                            TFGeneralTab.txtOldOwnerMSOID.IsEnabled = false;
                            TFGeneralTab.txtOldOwnerMSOName.IsEnabled = false;
                            TFGeneralTab.txtOldBillMSOID.IsEnabled = false;
                            TFGeneralTab.txtOldBillMSOName.IsEnabled = false;
                            TFGeneralTab.txtOldSystemPhone.IsEnabled = false;
                            TFGeneralTab.txtOldSystemContact.IsEnabled = false;
                            TFGeneralTab.txtNewOwnerMSOID.IsEnabled = false;
                            TFGeneralTab.txtNewOwnerMSOName.IsEnabled = false;
                            TFGeneralTab.txtNewBillMSOID.IsEnabled = false;
                            TFGeneralTab.txtNewBillMSOName.IsEnabled = false;
                            TFGeneralTab.txtNewSystemPhone.IsEnabled = false;
                            TFGeneralTab.txtNewSystemContact.IsEnabled = false;
                            TFGeneralTab.txtSpecialComments.IsEnabled = false;
                            TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;

                            //TFLocationsTab.IsEnabled = true;
                            TFLocationsTab.gTFLocations.IsEnabled = false;
                            //TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = true;
                            //TFLocationsTab.gTFLocations.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                            TFLocationsTab.gTFLocationsPNI.IsEnabled = false;
                            //TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = false;
                            //TFLocationsTab.gTFLocationsPNI.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                            TFLocationsTab.chkSelectAll.IsEnabled = false;

                            //if (RoleID == "25" || RoleID == "14")
                            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "25" ||
                                   this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "14")
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                            else
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = false;

                            if (Approver == "USCredit")
                            {
                                TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                                TFDetailTab.gTFDetails.IsEnabled = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                                TFDetailTab.btnAutoCreate.IsEnabled = true;
                                TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                                TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = true;
                            }
                            else
                                if (Approver == "Billing")
                                {
                                    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                                    TFDetailTab.gTFDetails.IsEnabled = true;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                                    TFDetailTab.btnAutoCreate.IsEnabled = true;
                                    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                                    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = true;
                                }
                                else
                                {
                                    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                                    TFDetailTab.gTFDetails.IsEnabled = true;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = false;
                                    TFDetailTab.btnAutoCreate.IsEnabled = false;
                                    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                                    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = false;
                                }

                            //if (Approver == "Billing")
                            //{
                            //    TFDetailTab.btnAutoCreate.IsEnabled = false;
                            //    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                            //    TFDetailTab.gTFDetails.IsEnabled = true;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                            //    TFDetailTab.btnAutoCreate.IsEnabled = true;
                            //    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                            //    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = true;
                            //}
                            //else
                            //{
                            //    TFDetailTab.btnAutoCreate.IsEnabled = false;
                            //    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                            //    TFDetailTab.gTFDetails.IsEnabled = true;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                            //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = false;
                            //    TFDetailTab.btnAutoCreate.IsEnabled = false;
                            //    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                            //    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = false;
                            //}

                            if (Approver == "TFApprover")
                            {
                                TFCreditTab.gTFCredit.IsEnabled = true;
                                TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = true;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = false;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = false;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = false;
                                //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "25" ||
                                //    this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "14")
                                //    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                                //else
                                //    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = false;
                                TFCreditTab.btnCopyAll.IsEnabled = true;
                                //if (cGlobals.SecurityDT.
                            }
                            else
                                if (Approver == "USCredit")
                                {
                                    TFCreditTab.gTFCredit.IsEnabled = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = true;
                                    //TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                                    TFCreditTab.btnCopyAll.IsEnabled = true;
                                }
                                else
                                {
                                    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "14")
                                    {
                                        TFCreditTab.gTFCredit.IsEnabled = true;
                                        TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                                        TFCreditTab.btnCopyAll.IsEnabled = true;
                                    }
                                    else
                                    {
                                        TFCreditTab.gTFCredit.IsEnabled = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = false;
                                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = false;
                                        //TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = false;
                                        TFCreditTab.btnCopyAll.IsEnabled = false;
                                    }
                                }
                        }
                    }
                    if (wfStatus[0] == 'N' || wfStatus[0] == 'R')
                    {
                        txtTFDesc.IsEnabled = true;

                        TFApprovalTab.btnSubmit.IsEnabled = true;
                        TFApprovalTab.btnAddApprover.IsEnabled = false;
                        TFApprovalTab.btnApprove.IsEnabled = false;
                        TFApprovalTab.btnInquiry.IsEnabled = false;
                        TFApprovalTab.btnReject.IsEnabled = false;
                        TFApprovalTab.btnReply.IsEnabled = false;

                        //TFGeneralTab.IsEnabled = true;
                        TFGeneralTab.txtDateRequested.IsEnabled = false;
                        TFGeneralTab.txtTFEffectiveDate.IsEnabled = true;
                        TFGeneralTab.txtOldOwnerMSOID.IsEnabled = true;
                        TFGeneralTab.txtOldOwnerMSOName.IsReadOnly = true;
                        TFGeneralTab.txtOldBillMSOID.IsEnabled = true;
                        TFGeneralTab.txtOldBillMSOName.IsReadOnly = true;
                        TFGeneralTab.txtOldSystemPhone.IsEnabled = true;
                        TFGeneralTab.txtOldSystemContact.IsEnabled = true;
                        TFGeneralTab.txtOldSystemPhone.IsTabStop = true;
                        TFGeneralTab.txtOldSystemPhone.IsReadOnly = false;
                        TFGeneralTab.txtOldSystemContact.IsReadOnly = false;
                        TFGeneralTab.txtNewOwnerMSOID.IsEnabled = true;
                        TFGeneralTab.txtNewOwnerMSOName.IsReadOnly = true;
                        TFGeneralTab.txtNewBillMSOID.IsEnabled = true;
                        TFGeneralTab.txtNewBillMSOName.IsReadOnly = true;
                        TFGeneralTab.txtNewSystemPhone.IsEnabled = true;
                        TFGeneralTab.txtNewSystemContact.IsEnabled = true;
                        TFGeneralTab.txtSpecialComments.IsEnabled = true;
                        TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;

                        //TFLocationsTab.IsEnabled = true;
                        TFLocationsTab.gTFLocations.IsEnabled = true;
                        TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = true;
                        TFLocationsTab.gTFLocationsPNI.IsEnabled = true;
                        TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = true;
                        TFLocationsTab.chkSelectAll.IsEnabled = true;

                        TFDetailTab.gTFDetails.IsEnabled = true;
                        TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = true;
                        TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                        TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                        TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                        TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                        TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                        TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                        TFDetailTab.btnAutoCreate.IsEnabled = true;
                        TFDetailTab.gTFDetails.ContextMenuAddIsVisible = true;

                        //TFCreditTab.IsEnabled = true;
                        TFCreditTab.gTFCredit.IsEnabled = true;
                        TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_contract_id"].Settings.AllowEdit = false;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_receivable_account"].Settings.AllowEdit = false;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["product_code"].Settings.AllowEdit = false;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_bill_mso_id"].Settings.AllowEdit = false;
                        //TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_account_name"].Settings.AllowEdit = false;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["cs_id"].Settings.AllowEdit = false;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = true;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = true;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = true;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = true;
                        TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                        TFCreditTab.btnCopyAll.IsEnabled = true;
                    }

                }
                else
                {
                    Messages.ShowWarning("TF not found!");
                    txtTFDesc.Text = "";
                    TFGeneralTab.IsEnabled = false;
                    TFCreditTab.IsEnabled = false;
                    TFDetailTab.IsEnabled = false;
                    TFLocationsTab.IsEnabled = false;
                    TFCommentsTab.IsEnabled = false;
                    TFAttachTab.IsEnabled = false;
                    TFApprovalTab.IsEnabled = false;
                }
                //if (this.CurrentBusObj.ObjectData.Tables["pni"].Rows.Count > 0)
                //{

                //}
                //else
                //    TFGeneralTab.TFpniClearGrid();
                //this.loadParms(TFNumber);
                //// load the data
                //this.Load();
            }
            else
            {
                SetWindowStatus();
            }
            //if (IsScreenDirty) Messages.ShowInformation("Screen Dirty Folder ");
        }

        private void loadParms(string TFNumber)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.AddParm("@status", "A");
                this.CurrentBusObj.Parms.AddParm("@tf_credit_id", "0");
                this.CurrentBusObj.Parms.AddParm("@new_contract_id", "0");
                this.CurrentBusObj.Parms.AddParm("@user_id", cGlobals.UserName);
                //if adjustment number passed load document id
                if (TFNumber != "")
                {
                    this.CurrentBusObj.Parms.AddParm("@tf_number", TFNumber);
                    this.CurrentBusObj.Parms.AddParm("@external_char_id", TFNumber);
                    TFDetailTab.txtLocationInvoice.Text = "";

                }
                else
                {
                    //if adjustmentid NOT passed load   with global parm adjustmentid if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.AddParm("@tf_number", cGlobals.ReturnParms[0].ToString());

                        //for attachments
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", cGlobals.ReturnParms[0].ToString());
                    }
                    //set dummy vals
                    else
                    {
                        this.CurrentBusObj.Parms.AddParm("@tf_number", "-1");

                        //for attachments
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");
                    }
                }
                //comment tab parms
                this.CurrentBusObj.Parms.AddParm("@comment_type", "TF");
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
                //attachment parms
                this.CurrentBusObj.Parms.AddParm("@attachment_type", "TATTACH");
                this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@contract_id", "0");
                this.CurrentBusObj.Parms.AddParm("@old_mso_id", "0");
                this.CurrentBusObj.Parms.AddParm("@mso_id", "0");
                //this.CurrentBusObj.Parms.AddParm("@cs_id", "-1");
                TFCommentsTab.CommentCode = "TF";
                //credit tab parms
                //this.CurrentBusObj.Parms.AddParm("@show_inactive", "0");

            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }

        private void TFNumber_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            string currentTF = "";
            if (txtTFNumber.Text != "")
            {
                currentTF = txtTFNumber.Text;
                TFNumber = txtTFNumber.Text;
            }

            cGlobals.ReturnParms.Clear();
            ////Event handles opening of the lookup window upon double click on TF Number field
            TFLookup f = new TFLookup();
            f.Init(new cBaseBusObject("TFLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtTFNumber.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                if (txtTFNumber.Text != TFNumber)
                {
                    ReturnData(txtTFNumber.Text, "@tf_number");
                    TFLocationsTab.txtOldOwnerMSOID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_id"].ToString();
                    TFLocationsTab.txtOldOwnerMSOName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_name"].ToString();
                    setAccess();
                    //TFLocationsTab.TFShowActiveDelegate();
                }
            }

        }

         //private void tcMainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                    if (TFApprovalTab.approval_btn_clicked)
                    {
                        ReturnData(TFNumber, "@tf_number");
                        TFApprovalTab.approval_btn_clicked = false;
                    }
                    notComingFromApproval = true;
                }

                if (e.RemovedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.RemovedItems[0]).Header.Equals("Locations"))
                {
                    //Only need to Load data if an invoice has already been saved.
                    //This is to display the correct workflow status on the general tab if it has changed since the inoice was
                    //was created or loaded on the general tab.
                    //notComingFromApproval = false;
                    if (this.CurrentBusObj != null)
                    {
                        if (IsScreenDirty && !IsScreenInserting && TFLocationsTab.gTFLocations.IsEnabled == true)
                        {
                            var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                            {
                                this.Save();
                                if (SaveSuccessful == false)
                                    return;
                                StopCloseIfCancelCloseOnSaveConfirmationTrue = true;
                            }
                        }
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
                                TFNumber = CurrentBusObj.ObjectData.Tables["general"].Rows[0]["TF_Number"].ToString();
                                txtTFNumber.Text = TFNumber;
                                CurrentBusObj.Parms.UpdateParmValue("@tf_number", TFNumber.ToString());
                                CurrentBusObj.Parms.UpdateParmValue("@external_char_id", TFNumber.ToString());
                            }
                        //    Messages.ShowInformation("New TF - " + TFNumber.ToString() + " Save Successful.");
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
                if (e.RemovedItems[0].GetType() == typeof(TabItemEx) && ((TabItemEx)e.RemovedItems[0]).Header.Equals("Credit"))
                {
                    //if (this.CurrentBusObj != null && (IsScreenDirty || ForceScreenDirty))
                    if (this.CurrentBusObj != null && (TFCreditTab.CreditDirty))
                    {
                        var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                        {
                            this.Save();
                            if (SaveSuccessful == false)
                                return;
                            //@@Need to add code here to stop the window from closing if save fails
                            StopCloseIfCancelCloseOnSaveConfirmationTrue = true;
                            //TFCreditTab.CreditDirty = false;
                        }
                        TFCreditTab.CreditDirty = false;
                    }
                }
            }
        }

        private void txtTFNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            TFNumber = txtTFNumber.Text;
            //GeneralMain.Focus();

        }

        private void txtTFNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtTFNumber.Text != TFNumber)
            {
                ReturnData(txtTFNumber.Text, "@tf_number");
                if (this.CurrentBusObj.ObjectData.Tables["general"] != null && this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
                //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
                {
                    TFLocationsTab.txtOldOwnerMSOID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_id"].ToString();
                    TFLocationsTab.txtOldOwnerMSOName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_name"].ToString();
                    TFApprovalTab.TFNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                }
                //GeneralMain.Focus();
            }


        }

        private void txtTFDesc_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtTFDesc.Text != "")
                CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_description"] = txtTFDesc.Text.ToString();
            //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            //if (txtTFDesc.Text != TFNumber)

                //ReturnData(txtTFDesc.Text, "@tf_description");
            //GeneralMain.Focus();


        }

        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                TFGeneralTab.NewFlag = false;
                //BCFStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]);
                //TFGeneralTab.LoadDDDWProductsbyContractID();
                //TFGeneralTab.cmbOldProductCode.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"].ToString().Trim();
                //TFGeneralTab.cmbOldProductCode.SelectedText = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"].ToString().Trim();
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                string Approver = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approver"].ToString();
                //string RoleID = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString();
                if (TFStatus.ToString() == "1" || wfStatus[0] == 'A')
                {
                    txtTFDesc.IsEnabled = false;

                    TFApprovalTab.btnSubmit.IsEnabled = false;
                    TFApprovalTab.btnAddApprover.IsEnabled = false;
                    TFApprovalTab.btnApprove.IsEnabled = false;
                    TFApprovalTab.btnInquiry.IsEnabled = false;
                    TFApprovalTab.btnReject.IsEnabled = false;
                    TFApprovalTab.btnReply.IsEnabled = false;

                    //TFGeneralTab.IsEnabled = false;
                    TFGeneralTab.txtDateRequested.IsEnabled = false;
                    TFGeneralTab.txtTFEffectiveDate.IsEnabled = false;
                    TFGeneralTab.txtOldOwnerMSOID.IsEnabled = false;
                    TFGeneralTab.txtOldOwnerMSOName.IsEnabled = false;
                    TFGeneralTab.txtOldBillMSOID.IsEnabled = false;
                    TFGeneralTab.txtOldBillMSOName.IsEnabled = false;
                    TFGeneralTab.txtOldSystemPhone.IsEnabled = false;
                    TFGeneralTab.txtOldSystemContact.IsEnabled = false;
                    TFGeneralTab.txtNewOwnerMSOID.IsEnabled = false;
                    TFGeneralTab.txtNewOwnerMSOName.IsEnabled = false;
                    TFGeneralTab.txtNewBillMSOID.IsEnabled = false;
                    TFGeneralTab.txtNewBillMSOName.IsEnabled = false;
                    TFGeneralTab.txtNewSystemPhone.IsEnabled = false;
                    TFGeneralTab.txtNewSystemContact.IsEnabled = false;
                    TFGeneralTab.txtSpecialComments.IsEnabled = false;
                    TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;

                    TFLocationsTab.gTFLocations.IsEnabled = false;
                    //TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = true;
                    //TFLocationsTab.gTFLocations.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                    TFLocationsTab.gTFLocationsPNI.IsEnabled = false;
                    //TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = false;
                    //TFLocationsTab.gTFLocationsPNI.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                    TFLocationsTab.chkSelectAll.IsEnabled = false;

                    TFDetailTab.gTFDetails.IsEnabled = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = false;
                    TFDetailTab.btnAutoCreate.IsEnabled = false;
                    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = false;

                    TFCreditTab.gTFCredit.IsEnabled = false;
                    //TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = false;
                    TFCreditTab.btnCopyAll.IsEnabled = false;
                    
                }
                else
                {
                    //BCFGeneralTab.txtAdjReason.IsReadOnly = false;
                }

                if (wfStatus[0] == 'N' || wfStatus[0] == 'R')
                {
                    txtTFDesc.IsEnabled = true;

                    TFApprovalTab.btnSubmit.IsEnabled = true;
                    TFApprovalTab.btnAddApprover.IsEnabled = false;
                    TFApprovalTab.btnApprove.IsEnabled = false;
                    TFApprovalTab.btnInquiry.IsEnabled = false;
                    TFApprovalTab.btnReject.IsEnabled = false;
                    TFApprovalTab.btnReply.IsEnabled = false;

                    //TFGeneralTab.IsEnabled = true;
                    TFGeneralTab.txtDateRequested.IsEnabled = false;
                    TFGeneralTab.txtTFEffectiveDate.IsEnabled = true;
                    TFGeneralTab.txtOldOwnerMSOID.IsEnabled = true;
                    TFGeneralTab.txtOldOwnerMSOName.IsReadOnly = true;
                    TFGeneralTab.txtOldBillMSOID.IsEnabled = true;
                    TFGeneralTab.txtOldBillMSOName.IsReadOnly = true;
                    TFGeneralTab.txtOldSystemPhone.IsEnabled = true;
                    TFGeneralTab.txtOldSystemPhone.IsTabStop = true;
                    TFGeneralTab.txtOldSystemContact.IsEnabled = true;
                    TFGeneralTab.txtOldSystemPhone.IsReadOnly = false;
                    TFGeneralTab.txtOldSystemContact.IsReadOnly = false;
                    TFGeneralTab.txtNewOwnerMSOID.IsEnabled = true;
                    TFGeneralTab.txtNewOwnerMSOName.IsReadOnly = true;
                    TFGeneralTab.txtNewBillMSOID.IsEnabled = true;
                    TFGeneralTab.txtNewBillMSOName.IsReadOnly = true;
                    TFGeneralTab.txtNewSystemPhone.IsEnabled = true;
                    TFGeneralTab.txtNewSystemContact.IsEnabled = true;
                    TFGeneralTab.txtSpecialComments.IsEnabled = true;
                    TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;

                    //TFLocationsTab.IsEnabled = true;
                    TFLocationsTab.gTFLocations.IsEnabled = true;
                    TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = true;
                    TFLocationsTab.gTFLocationsPNI.IsEnabled = true;
                    TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = true;
                    TFLocationsTab.chkSelectAll.IsEnabled = true;

                    TFDetailTab.gTFDetails.IsEnabled = true;
                    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.IsEnabled = true;
                    TFDetailTab.btnAutoCreate.IsEnabled = true;
                    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = true;

                    TFCreditTab.gTFCredit.IsEnabled = true;
                    TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_contract_id"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_receivable_account"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["product_code"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_bill_mso_id"].Settings.AllowEdit = false;
                    //TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_account_name"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["cs_id"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                    TFCreditTab.btnCopyAll.IsEnabled = true;
                }
                else
                {

                    //Need to allow workflow buttons to work for in process
                    if (wfStatus[0] == 'I')
                    {
                        txtTFDesc.IsEnabled = false;

                        TFApprovalTab.btnSubmit.IsEnabled = true;
                        TFApprovalTab.btnAddApprover.IsEnabled = true;
                        TFApprovalTab.btnApprove.IsEnabled = true;
                        TFApprovalTab.btnInquiry.IsEnabled = true;
                        TFApprovalTab.btnReject.IsEnabled = true;
                        TFApprovalTab.btnReply.IsEnabled = true;

                        //TFGeneralTab.IsEnabled = false;
                        TFGeneralTab.txtDateRequested.IsEnabled = false;
                        TFGeneralTab.txtTFEffectiveDate.IsEnabled = false;
                        TFGeneralTab.txtOldOwnerMSOID.IsEnabled = false;
                        TFGeneralTab.txtOldOwnerMSOName.IsEnabled = false;
                        TFGeneralTab.txtOldBillMSOID.IsEnabled = false;
                        TFGeneralTab.txtOldBillMSOName.IsEnabled = false;
                        TFGeneralTab.txtOldSystemPhone.IsEnabled = false;
                        TFGeneralTab.txtOldSystemContact.IsEnabled = false;
                        TFGeneralTab.txtNewOwnerMSOID.IsEnabled = false;
                        TFGeneralTab.txtNewOwnerMSOName.IsEnabled = false;
                        TFGeneralTab.txtNewBillMSOID.IsEnabled = false;
                        TFGeneralTab.txtNewBillMSOName.IsEnabled = false;
                        TFGeneralTab.txtNewSystemPhone.IsEnabled = false;
                        TFGeneralTab.txtNewSystemContact.IsEnabled = false;
                        TFGeneralTab.txtSpecialComments.IsEnabled = false;
                        TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;
                        
                        TFLocationsTab.gTFLocations.IsEnabled = false;
                        //TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = true;
                        //TFLocationsTab.gTFLocations.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                        TFLocationsTab.gTFLocationsPNI.IsEnabled = false;
                        //TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = false;
                        //TFLocationsTab.gTFLocationsPNI.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                        TFLocationsTab.chkSelectAll.IsEnabled = false;

                        //if (RoleID == "25" || RoleID == "14")
                        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "25" ||
                               this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "14")
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                        else
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = false;

                        if (Approver == "USCredit")
                        {
                            TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                            TFDetailTab.gTFDetails.IsEnabled = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                            TFDetailTab.btnAutoCreate.IsEnabled = true;
                            TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                            TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = true;
                        }
                        else
                            if (Approver == "Billing")
                            {
                                TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                                TFDetailTab.gTFDetails.IsEnabled = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                                TFDetailTab.btnAutoCreate.IsEnabled = true;
                                TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                                TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = true;
                            }
                            else
                            {
                                TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                                TFDetailTab.gTFDetails.IsEnabled = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = false;
                                TFDetailTab.btnAutoCreate.IsEnabled = false;
                                TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                                TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = false;
                            }

                        //if (Approver == "Billing")
                        //{
                        //    TFDetailTab.btnAutoCreate.IsEnabled = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.IsEnabled = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                        //    TFDetailTab.btnAutoCreate.IsEnabled = true;
                        //    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                        //    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = true;
                        //}
                        //else
                        //{
                        //    TFDetailTab.btnAutoCreate.IsEnabled = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.IsEnabled = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = false;
                        //    TFDetailTab.btnAutoCreate.IsEnabled = false;
                        //    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                        //    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = false;
                        //}

                        if (Approver == "TFApprover")
                        {
                            TFCreditTab.gTFCredit.IsEnabled = true;
                            TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = true;
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = false;
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = false;
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = false;
                            //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "25" ||
                            //    this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "14")
                            //    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                            //else
                            //    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = false;
                            TFCreditTab.btnCopyAll.IsEnabled = true;
                            //if (cGlobals.SecurityDT.
                        }
                        else
                            if (Approver == "USCredit")
                            {
                                TFCreditTab.gTFCredit.IsEnabled = true;
                                TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = false;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = true;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = true;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = true;
                                //TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                                TFCreditTab.btnCopyAll.IsEnabled = true;
                            }
                            else
                            {
                                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "14")
                                {
                                    TFCreditTab.gTFCredit.IsEnabled = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                                    TFCreditTab.btnCopyAll.IsEnabled = true;
                                }
                                else
                                {
                                    TFCreditTab.gTFCredit.IsEnabled = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = false;
                                    //TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = false;
                                    TFCreditTab.btnCopyAll.IsEnabled = false;
                                }
     
                            }
                    }
                }
                return true;
            }
            else
            {
                Messages.ShowWarning("TF Not Found");
                txtTFDesc.Text = "";
                TFGeneralTab.IsEnabled = false;
                TFCreditTab.IsEnabled = false;
                TFDetailTab.IsEnabled = false;
                TFLocationsTab.IsEnabled = false;
                TFCommentsTab.IsEnabled = false;
                TFAttachTab.IsEnabled = false;
                TFApprovalTab.IsEnabled = false;

                return false;
            }
        }

        void SetWindowStatus()
        {
            //don't allow edits on general tab fields
            this.TFGeneralTab.IsEnabled = false;
            this.TFCommentsTab.IsEnabled = true;
            this.TFAttachTab.IsEnabled = true;
            this.TFViewTab.IsEnabled = false;
            this.TFApprovalTab.IsEnabled = false;
            txtTFNumber.IsEnabled = true;
            txtTFDesc.IsEnabled = false;
        }

        public void ReturnData(string SearchValue, string DbParm)
        {
            //This section will check to see if changes have been made and if save is desired in the event of a
            //double click lookup or a change of the contract id field.
            //Verify that no save is needed
            Prep_ucBaseGridsForSave();
            PrepareFreeformForSave();
            if (IsScreenDirty && notComingFromApproval)
            {
                //Establish a temporary BCFNumber for storing the ID the user wanted to go to.  This will be used in the final retrieval in the event of a
                //Yes or No answer to the question below.
                String NewTFNumber = "";
                System.Windows.MessageBoxResult result = Messages.ShowYesNoCancel("Would you like to save existing changes?",
                           System.Windows.MessageBoxImage.Question);
                //Save existing BCF information and then load TF customer
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    NewTFNumber = txtTFNumber.Text;
                    Save();
                    //If Save fails then reset BCF Number to original value and exit retrieve so that changes will not be lost.
                    if (!SaveSuccessful)
                    {
                        txtTFNumber.Text = TFNumber;

                    }
                    //else if (NewTFNumber != "")
                    //{
                    //    txtTFNumber.Text = NewTFNumber.ToString();
                    //    ReturnData(txtTFNumber.Text, "@tf_number");
                    //}
                }
                //Returns the user to the current BCF window and resets the txtBCFNumber field to original value.
                else if (result == System.Windows.MessageBoxResult.Cancel)
                {
                    txtTFNumber.Text = TFNumber;
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
            //RES 11/6/15 if coming from approval tab load general to reflect approval status change
            this.Load();
            //if (notComingFromApproval)
            //    this.Load();
            //else
            //{
            //    //CurrentBusObj.Parms.ClearParms();
            //    //CurrentBusObj.Parms.AddParm("@tf_number", TFNumber);
            //    //this.CurrentBusObj.Parms.AddParm("@user_id", cGlobals.UserName);
            //    //CurrentBusObj.LoadTable("general");
            //    //CurrentBusObj.LoadData("general");
            //    //this.Load("general");
            //    this.Load();
            //}
                
            //if BCFNumber found then set header and pop otherwise send message
            //if customer number found then set header and pop otherwise send message
            if (chkForData())
            {
                SetHeaderName();
                setEditScreenState();
                //Adding this to see if it helps with the SAVE issue
                TFNumber = txtTFNumber.Text;
                TFApprovalTab.TFNumber = txtTFNumber.Text;
                TFApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);

                //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count >  0)
                //{
                //    this.CurrentBusObj.Parms.UpdateParmValue("@cs_id",this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["cs_id"]);

                //}
            }
            //Unhide comment type on comment grid
            //BCFCommentsTab.GridUnHideCommentType();
       }

        private void clrCommentsAttachmentsObj()
        {
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();
        }

        private void SetHeaderName()
        {
            ContentPane p = (ContentPane)this.Parent;
            //Sets the header name when being called from another folder
            if (txtTFNumber.Text == null)
            {
                windowCaption = "TF Number -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                txtTFNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                txtTFDesc.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_description"].ToString();
            }
            ////Sets the header name from within same folder
            else
            {
                //ContentPane p = (ContentPane)this.Parent;
                //p.Header = "Adjustment ID -" + txtAdjustmentID.Text;
                txtTFNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                txtTFDesc.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_description"].ToString();
                p.Header = "TF Number -" + txtTFNumber.Text;
                //cmbBCFType.SelectedValue = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["bcf_type"];
            }
        }

        private void setEditScreenState()
        {
            //go to general tab
            //allow edits
            this.TFGeneralTab.IsEnabled = true;
            this.TFCommentsTab.IsEnabled = true;
            this.TFAttachTab.IsEnabled = true;
            this.TFViewTab.IsEnabled = true;
            this.TFApprovalTab.IsEnabled = true;
            //enable header
            txtTFNumber.IsEnabled = true;
            //cmbBCFType.IsEnabled = true;
        }

        public override void Delete()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                TFStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_status_flag"]);
                //if (AdjustmentID == null)
                TFNumber = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["TF_number"].ToString();
                //If posted cannot delete adjustment
                if (TFStatus.ToString() == "2")
                    Messages.ShowWarning("Cannot delete a TF that has been closed");
                else
                {
                    MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to delete TF " + TFNumber.ToString() + "?", System.Windows.MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (cGlobals.BillService.DeleteBCF(TFNumber) == true)
                        {
                            Messages.ShowWarning("TF Number " + TFNumber + " Deleted");
                            //KSH - 8/24/12 clear comments/attachments grid if applicable bus obj
                            clrCommentsAttachmentsObj();
                            this.Load();
                            txtTFNumber.Text = "";
                            ContentPane p = (ContentPane)this.Parent;
                            p.Header = "TF Number";
                        }
                        else
                            Messages.ShowWarning("Error Deleting TF");
                    }
                    else
                        Messages.ShowMessage("TF not deleted", MessageBoxImage.Information);
               }
            }
            else
                Messages.ShowWarning("No TF to delete");
        }

        public override void Save()
        {
            //if (TFStatus.ToString() == "1" || wfStatus[0] == 'A')
            //call the validation on the General Tab

            TFGeneralTab.ValidatebeforeSave();

            object ob = Keyboard.FocusedElement;
            if (ob is TextBox)
            {
                TextBox tb = ob as TextBox;
                BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }


            if (TFGeneralTab.errorsExist != true)
            {
                //if (txtTFDesc.Text == "")
                //{
                //    MessageBox.Show("TF must have a description");
                //    TFGeneralTab.Focus();
                //    txtTFDesc.CntrlFocus();
                //    return;
                //}

                //CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_description"] = txtTFDesc.Text;
                 CurrentBusObj.Parms.UpdateParmValue("@tf_description", txtTFDesc.Text);

                this.Cursor = Cursors.Wait;
                base.Save();

                if (SaveSuccessful)
                {
                   if (this.CurrentBusObj.ObjectData.Tables["credit"].Rows.Count != 0)
                        TFCreditTab.CreditDirty = false;

                   var localTFInfo = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                       where x.Field<string>("parmName") == "@tf_number"
                                       select new
                                       {
                                           parmName = x.Field<string>("parmName"),
                                           parmValue = x.Field<string>("parmValue")
                                       };

                    foreach (var info in localTFInfo)
                    {
                        if (info.parmName == "@tf_number")
                            txtTFNumber.Text = info.parmValue;
                    }

                    if (IsScreenInserting == false)
                    {
                        //if contact id found then set header and pop otherwise send message
                        if (chkForData()) SetHeaderName();
                        Messages.ShowInformation("Save Successful");
                        if (txtTFNumber.IsEnabled == false)
                            txtTFNumber.IsEnabled = true;
                    }
                    else
                    {
                        //Insert has occurred
                        //ReturnData(txtBCFNumber.Text, "@BCF_number");
                        txtTFNumber.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_number"].ToString();
                        TFNumber = txtTFNumber.Text;
                        this.CurrentBusObj.ObjectData.Tables["ParmTable"].Rows[1]["parmValue"] = TFNumber.ToString();
                        //turn off inserting flag
                        IsScreenInserting = false;
                        loadParms(TFNumber);

                        Messages.ShowInformation("New TF - " + TFNumber.ToString() + " Save Successful.");
                        txtTFNumber.IsEnabled = true;
                        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
                        {
                            TFLocationsTab.txtOldOwnerMSOID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_id"].ToString();
                            TFLocationsTab.txtOldOwnerMSOName.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_owner_mso_name"].ToString();
                        }

                        TFApprovalTab.IsEnabled = true;
                        TFApprovalTab.btnSubmit.IsEnabled = true;
                        TFApprovalTab.btnApprove.IsEnabled = false;
                        TFApprovalTab.btnReject.IsEnabled = false;
                        TFApprovalTab.btnAddApprover.IsEnabled = false;
                        TFApprovalTab.btnInquiry.IsEnabled = false;
                        TFApprovalTab.btnReply.IsEnabled = false;
                        TFApprovalTab.TFNumber = TFNumber;
                        TFApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                        TFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
                        TFApprovalTab.ApprovalBusinessObject.Parms.AddParm(documentIdParameter, TFNumber);
                        TFApprovalTab.ApprovalBusinessObject.LoadData();
                        TFApprovalTab.ApprovalBusinessObject.Parms.ClearParms();
                        this.CurrentBusObj.changeParm("@external_char_id", TFNumber);
                        //turn off inserting flag
                        IsScreenInserting = false;
                    }
                    //Messages.ShowInformation("Save Successful");
                }
            }
            else
            {
                Messages.ShowInformation("Save Failed");
                SaveSuccessful = false;
                //need to cleanup if a detail or pni row were inserted
                cBaseBusObject TFVerification = new cBaseBusObject("TFVerification");

                TFVerification.Parms.ClearParms();
                TFVerification.Parms.AddParm("@tf_number", TFNumber);
                TFVerification.LoadTable("cleanup");
                GeneralMain.Focus();
                return;
            }
            this.Cursor = Cursors.Arrow;
        }

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
                txtTFNumber.Text = "NEW";
                TFNumber = "NEW";
                txtTFDesc.Text = "";

                //Remove any existing parameters
                CurrentBusObj.Parms.ClearParms();
                //Add all parameters back in
               
                //this.CurrentBusObj.Parms.AddParm("@move", "0");
                this.CurrentBusObj.Parms.AddParm("@new_contract_id", "0");
                this.CurrentBusObj.Parms.AddParm("@user_id", cGlobals.UserName);
                TFLocationsTab.txtOldOwnerMSOID.Text = "";
                TFLocationsTab.txtOldOwnerMSOName.Text = "";
                CurrentBusObj.Parms.AddParm("@tf_number", "");
                //CurrentBusObj.Parms.AddParm("@tf_description", "");
                this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@comment_type", "TF");
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
                //attachment parms
                this.CurrentBusObj.Parms.AddParm("@attachment_type", "TATTACH");
                this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");
                this.CurrentBusObj.Parms.AddParm("@contract_id", "0");
                this.CurrentBusObj.Parms.AddParm("@old_mso_id", "0");
                this.CurrentBusObj.Parms.AddParm("@mso_id", "0");
                this.CurrentBusObj.Parms.AddParm("@status", "A");
                this.CurrentBusObj.Parms.AddParm("@tf_credit_id", "0");
 
                base.New();
                ContentPane p = (ContentPane)this.Parent;
                p.Header = "TFNumber -" + TFNumber.ToString();
                txtTFNumber.Text = TFNumber.ToString();
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["TF_number"] = TFNumber.ToString();
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_status_flag"] = 0;
                //this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_product_code"] = "";
                //this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_product_code"] = "";
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_status_flag"] = 0;
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["special_comments"] = "";
                //this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_flag"] = 0;
                //this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["multiple_locations"] = 0;
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_system_contact"] = "";
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["old_system_phone"] = "";
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_system_contact"] = "";
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["new_system_phone"] = "";
                //this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["contact_email"] = "";
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"] = "Not Submitted For Approval";
                this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["multiple_locations"] = 0;

                CurrentBusObj.Parms.UpdateParmValue("@tf_number", TFNumber.ToString());
                CurrentBusObj.Parms.UpdateParmValue("@external_char_id", TFNumber.ToString());
                TFGeneralTab.SetDefaultValues();
                this.clrCommentsAttachmentsObj();
                //need to clear grids

                if (this.CurrentBusObj.ObjectData.Tables["locations"].Rows.Count > 0)
                {
                    //TFLocationsTab.txtLocation.Text = "";
                    TFLocationsTab.TFlocationsClearGrid();
                }
                if (this.CurrentBusObj.ObjectData.Tables["view"].Rows.Count > 0)
                    this.CurrentBusObj.ObjectData.Tables["view"].Rows.Clear();
                if (this.CurrentBusObj.ObjectData.Tables["general_pni"].Rows.Count > 0)
                    this.CurrentBusObj.ObjectData.Tables["general_pni"].Rows.Clear();
                if (this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Rows.Count > 0)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Rows.Clear();
                if (this.CurrentBusObj.ObjectData.Tables["comments_char"].Rows.Count > 0)
                    this.CurrentBusObj.ObjectData.Tables["comments_char"].Rows.Clear();
                if (this.CurrentBusObj.ObjectData.Tables["attachments"].Rows.Count > 0)
                    this.CurrentBusObj.ObjectData.Tables["attachments"].Rows.Clear();
                if (this.CurrentBusObj != null)
                    if (this.CurrentBusObj.ObjectData != null)
                        this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();

                //TFGeneralTab.txtTFEffectiveDate.Focus();
                txtTFDesc.CntrlFocus();
           }
        }

        private void setInitScreenState()
        {
            //don't allow edits on general tab fields
            this.TFGeneralTab.IsEnabled = false;
            //don't allow header edits
            txtTFNumber.IsEnabled = true;
        }

        private void setInsertScreenState()
        {
            //go to general tab
            //TFGeneralTab.Focus();
            //set focus to first field
            //this.Focus();
            txtTFNumber.IsReadOnly = true;
            txtTFNumber.IsEnabled = false;
            txtTFDesc.IsReadOnly = false;
            txtTFDesc.IsEnabled = true;
            txtTFDesc.CntrlFocus();
            //cmbBCFType.Focus();
            //allow edits on general tab fields
            this.TFGeneralTab.IsEnabled = true;
            this.TFCommentsTab.IsEnabled = false;
            this.TFAttachTab.IsEnabled = false;
            this.TFViewTab.IsEnabled = false;
            this.TFApprovalTab.IsEnabled = false;
            //don't allow header edits
            //txtTFNumber.IsReadOnly = true;
            //txtTFNumber.IsEnabled = false;
            //txtTFDesc.IsEnabled = true;
            //txtTFDesc.IsReadOnly = false;
            //txtTFDesc.Focus();
            TFGeneralTab.txtOldOwnerMSOID.IsEnabled = true;
            TFGeneralTab.txtOldOwnerMSOName.IsReadOnly = false;
            TFGeneralTab.txtNewOwnerMSOID.IsEnabled = true;
            TFGeneralTab.txtNewOwnerMSOName.IsReadOnly = false;
            TFGeneralTab.txtOldBillMSOID.IsEnabled = true;
            TFGeneralTab.txtOldBillMSOName.IsReadOnly = false;
            TFGeneralTab.txtNewBillMSOID.IsEnabled = true;
            TFGeneralTab.txtNewBillMSOName.IsReadOnly = false;
            TFGeneralTab.txtDateRequested.IsEnabled = true;
            TFGeneralTab.txtTFEffectiveDate.IsEnabled = true;
            TFGeneralTab.txtOldSystemPhone.IsReadOnly = false;
            TFGeneralTab.txtOldSystemPhone.IsTabStop = true;
            TFGeneralTab.txtOldSystemContact.IsReadOnly = false;
            TFGeneralTab.txtOldSystemPhone.IsEnabled = true;
            TFGeneralTab.txtOldSystemContact.IsEnabled = true;
            TFGeneralTab.txtNewSystemPhone.IsReadOnly = false;
            TFGeneralTab.txtNewSystemContact.IsReadOnly = false;
            TFGeneralTab.txtNewSystemPhone.IsEnabled = true;
            TFGeneralTab.txtNewSystemContact.IsEnabled = true;
            TFGeneralTab.txtSpecialComments.IsEnabled = true;
            TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;
            TFGeneralTab.gTFPNI.ContextMenuAddIsVisible = true;

            TFLocationsTab.gTFLocations.IsEnabled = true;
            TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = true;
            TFLocationsTab.gTFLocationsPNI.IsEnabled = true;
            TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = true;
            TFLocationsTab.chkSelectAll.IsEnabled = true;

            TFDetailTab.txtLocationInvoice.Text = "";  
        }

        private void setAccess()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                TFStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["tf_status_flag"]);
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                string Approver = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approver"].ToString();
                //string RoleID = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString();
                if (TFStatus.ToString() == "1" || wfStatus[0] == 'A')
                {
                    //need to not allow changes
                    txtTFDesc.IsEnabled = false;

                    TFApprovalTab.btnSubmit.IsEnabled = false;
                    TFApprovalTab.btnAddApprover.IsEnabled = false;
                    TFApprovalTab.btnApprove.IsEnabled = false;
                    TFApprovalTab.btnInquiry.IsEnabled = false;
                    TFApprovalTab.btnReject.IsEnabled = false;
                    TFApprovalTab.btnReply.IsEnabled = false;

                    //TFGeneralTab.IsEnabled = false;
                    TFGeneralTab.txtDateRequested.IsEnabled = false;
                    TFGeneralTab.txtTFEffectiveDate.IsEnabled = false;
                    TFGeneralTab.txtOldOwnerMSOID.IsEnabled = false;
                    TFGeneralTab.txtOldOwnerMSOName.IsEnabled = false;
                    TFGeneralTab.txtOldBillMSOID.IsEnabled = false;
                    TFGeneralTab.txtOldBillMSOName.IsEnabled = false;
                    TFGeneralTab.txtOldSystemPhone.IsEnabled = false;
                    TFGeneralTab.txtOldSystemContact.IsEnabled = false;
                    TFGeneralTab.txtNewOwnerMSOID.IsEnabled = false;
                    TFGeneralTab.txtNewOwnerMSOName.IsEnabled = true;
                    TFGeneralTab.txtNewBillMSOID.IsEnabled = false;
                    TFGeneralTab.txtNewBillMSOName.IsEnabled = false;
                    TFGeneralTab.txtNewSystemPhone.IsEnabled = false;
                    TFGeneralTab.txtNewSystemContact.IsEnabled = false;
                    TFGeneralTab.txtSpecialComments.IsEnabled = false;
                    TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;

                    TFLocationsTab.gTFLocations.IsEnabled = false;
                    TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = false;
                    TFLocationsTab.gTFLocationsPNI.IsEnabled = false;
                    TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = false;
                    TFLocationsTab.txtOldOwnerMSOID.IsEnabled = false;
                    TFLocationsTab.txtOldOwnerMSOName.IsReadOnly = true;
                    TFLocationsTab.chkSelectAll.IsEnabled = false;

                    TFDetailTab.gTFDetails.IsEnabled = false;
                    //TFDetailTab.btnAutoCreate.IsEnabled = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                    //TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = false;
                    TFDetailTab.btnAutoCreate.IsEnabled = false;
                    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = false;

                    TFCreditTab.gTFCredit.IsEnabled = false;
                    //TFCreditTab.gTFCredit.IsEnabled = true;
                    //TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = false;
                    TFCreditTab.btnCopyAll.IsEnabled = false;
 
                }
                else
                {
                    //RES 10/8/13 phase 3.0.2 added to lock down in process BCF's
                    //Need to allow workflow buttons to work for in process
                    if (wfStatus[0] == 'I')
                    {
                        txtTFDesc.IsEnabled = false;

                        TFApprovalTab.btnSubmit.IsEnabled = true;
                        TFApprovalTab.btnAddApprover.IsEnabled = true;
                        TFApprovalTab.btnApprove.IsEnabled = true;
                        TFApprovalTab.btnInquiry.IsEnabled = true;
                        TFApprovalTab.btnReject.IsEnabled = true;
                        TFApprovalTab.btnReply.IsEnabled = true;

                        //TFGeneralTab.IsEnabled = false;
                        TFGeneralTab.txtDateRequested.IsEnabled = false;
                        TFGeneralTab.txtTFEffectiveDate.IsEnabled = false;
                        TFGeneralTab.txtOldOwnerMSOID.IsEnabled = false;
                        TFGeneralTab.txtOldOwnerMSOName.IsEnabled = false;
                        TFGeneralTab.txtOldBillMSOID.IsEnabled = false;
                        TFGeneralTab.txtOldBillMSOName.IsEnabled = false;
                        TFGeneralTab.txtOldSystemPhone.IsEnabled = false;
                        TFGeneralTab.txtOldSystemContact.IsEnabled = false;
                        TFGeneralTab.txtNewOwnerMSOID.IsEnabled = false;
                        TFGeneralTab.txtNewOwnerMSOName.IsEnabled = false;
                        TFGeneralTab.txtNewBillMSOID.IsEnabled = false;
                        TFGeneralTab.txtNewBillMSOName.IsEnabled = false;
                        TFGeneralTab.txtNewSystemPhone.IsEnabled = false;
                        TFGeneralTab.txtNewSystemContact.IsEnabled = false;
                        TFGeneralTab.txtSpecialComments.IsEnabled = false;
                        TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;

                        //TFLocationsTab.IsEnabled = true;
                        TFLocationsTab.gTFLocations.IsEnabled = false;
                        //TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = true;
                        //TFLocationsTab.gTFLocations.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                        TFLocationsTab.gTFLocationsPNI.IsEnabled = false;
                        //TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = false;
                        //TFLocationsTab.gTFLocationsPNI.xGrid.FieldLayouts[0].Fields["move"].Settings.AllowEdit = false;
                        TFLocationsTab.chkSelectAll.IsEnabled = false;

                        if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "25" ||
                               this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "14")
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                        else
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = false;

                        if (Approver == "USCredit")
                        {
                            TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                            TFDetailTab.gTFDetails.IsEnabled = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                            TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                            TFDetailTab.btnAutoCreate.IsEnabled = true;
                            TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                            TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = true;
                        }
                        else
                            if (Approver == "Billing")
                            {
                                TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                                TFDetailTab.gTFDetails.IsEnabled = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                                TFDetailTab.btnAutoCreate.IsEnabled = true;
                                TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                                TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = true;
                            }
                            else
                            {
                                TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                                TFDetailTab.gTFDetails.IsEnabled = true;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                                TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = false;
                                TFDetailTab.btnAutoCreate.IsEnabled = false;
                                TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                                TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = false;
                            }

                        //if (Approver == "Billing")
                        //{
                        //    TFDetailTab.btnAutoCreate.IsEnabled = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.IsEnabled = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                        //    TFDetailTab.btnAutoCreate.IsEnabled = true;
                        //    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                        //    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = true;
                        //}
                        //else
                        //{
                        //    TFDetailTab.btnAutoCreate.IsEnabled = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.IsEnabled = true;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = false;
                        //    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = false;
                        //    TFDetailTab.btnAutoCreate.IsEnabled = false;
                        //    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = false;
                        //    TFDetailTab.gTFDetails.ContextMenuRemoveIsVisible = false;
                        //}

                        if (Approver == "TFApprover")
                        {
                            TFCreditTab.gTFCredit.IsEnabled = true;
                            TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = false;
                            TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = true;
                            //if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "25" ||
                            //    this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "14")
                            //    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                            //else
                            //    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = false;
                            TFCreditTab.btnCopyAll.IsEnabled = true;
                        }
                        else
                            if (Approver == "USCredit")
                            {
                                TFCreditTab.gTFCredit.IsEnabled = true;
                                TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = false;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = true;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = true;
                                TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = true;
                                //TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                                TFCreditTab.btnCopyAll.IsEnabled = true;
                            }
                            else
                            {
                                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["roleid"].ToString() == "14")
                                {
                                    TFCreditTab.gTFCredit.IsEnabled = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                                    TFCreditTab.btnCopyAll.IsEnabled = true;
                                }
                                else
                                {
                                    TFCreditTab.gTFCredit.IsEnabled = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = false;
                                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = false;
                                    //TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = false;
                                    TFCreditTab.btnCopyAll.IsEnabled = false;
                                }
                            }
                    }
                }
                if (wfStatus[0] == 'N' || wfStatus[0] == 'R')
                {
                    if (CurrentState == ScreenState.Inserting)
                    {
                        txtTFNumber.IsReadOnly = true;
                        txtTFNumber.IsEnabled = false;
                        txtTFDesc.IsEnabled = true;
                        txtTFDesc.IsReadOnly = false;
                        txtTFDesc.CntrlFocus();
                    }
                    txtTFDesc.IsEnabled = true;
                    txtTFDesc.IsReadOnly = false;

                    TFApprovalTab.btnSubmit.IsEnabled = true;
                    TFApprovalTab.btnAddApprover.IsEnabled = false;
                    TFApprovalTab.btnApprove.IsEnabled = false;
                    TFApprovalTab.btnInquiry.IsEnabled = false;
                    TFApprovalTab.btnReject.IsEnabled = false;
                    TFApprovalTab.btnReply.IsEnabled = false;

                    //TFGeneralTab.IsEnabled = true;
                    TFGeneralTab.txtDateRequested.IsEnabled = false;
                    TFGeneralTab.txtTFEffectiveDate.IsEnabled = true;
                    TFGeneralTab.txtOldOwnerMSOID.IsEnabled = true;
                    TFGeneralTab.txtOldOwnerMSOName.IsReadOnly = true;
                    TFGeneralTab.txtOldBillMSOID.IsEnabled = true;
                    TFGeneralTab.txtOldBillMSOName.IsReadOnly = true;
                    TFGeneralTab.txtOldSystemPhone.IsEnabled = true;
                    TFGeneralTab.txtOldSystemContact.IsEnabled = true;
                    TFGeneralTab.txtOldSystemPhone.IsReadOnly = false;
                    TFGeneralTab.txtOldSystemPhone.IsTabStop = true;
                    TFGeneralTab.txtOldSystemContact.IsReadOnly = false;
                    TFGeneralTab.txtNewOwnerMSOID.IsEnabled = true;
                    TFGeneralTab.txtNewOwnerMSOName.IsReadOnly = true;
                    TFGeneralTab.txtNewBillMSOID.IsEnabled = true;
                    TFGeneralTab.txtNewBillMSOName.IsReadOnly = true;
                    TFGeneralTab.txtNewSystemPhone.IsEnabled = true;
                    TFGeneralTab.txtNewSystemContact.IsEnabled = true;
                    //TFGeneralTab.txtNewSystemPhone.IsReadOnly = false;
                    //TFGeneralTab.txtNewSystemContact.IsReadOnly = false;
                    TFGeneralTab.txtSpecialComments.IsEnabled = true;
                    TFGeneralTab.gTFPNI.xGrid.FieldSettings.AllowEdit = false;

                    //TFLocationsTab.IsEnabled = true;
                    TFLocationsTab.gTFLocations.IsEnabled = true;
                    TFLocationsTab.gTFLocations.xGrid.FieldSettings.AllowEdit = true;
                    TFLocationsTab.gTFLocationsPNI.IsEnabled = true;
                    TFLocationsTab.gTFLocationsPNI.xGrid.FieldSettings.AllowEdit = true;
                    TFLocationsTab.chkSelectAll.IsEnabled = true;

                    TFDetailTab.gTFDetails.IsEnabled = true;
                    TFDetailTab.gTFDetails.xGrid.FieldSettings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["invoice_to_be_credited"].Settings.AllowEdit = false;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["first_billing_period_effected"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["last_billing_period_effected"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["credit_amount"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["document_id"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.xGrid.FieldLayouts[0].Fields["rebill_flag"].Settings.AllowEdit = true;
                    TFDetailTab.gTFDetails.IsEnabled = true;
                    TFDetailTab.btnAutoCreate.IsEnabled = true;
                    TFDetailTab.gTFDetails.ContextMenuAddIsVisible = true;

                    TFCreditTab.gTFCredit.IsEnabled = true;
                    TFCreditTab.gTFCredit.xGrid.FieldSettings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_contract_id"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_receivable_account"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["product_code"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_bill_mso_id"].Settings.AllowEdit = false;
                    //TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["old_account_name"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["cs_id"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_contract_id"].Settings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["contact_information_updated"].Settings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["new_receivable_account"].Settings.AllowEdit = false;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["subscriber_counts_updated"].Settings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_owner_paid_through"].Settings.AllowEdit = true;
                    TFCreditTab.gTFCredit.xGrid.FieldLayouts[0].Fields["date_to_begin_invoicing_new_owner"].Settings.AllowEdit = true;
                    TFCreditTab.btnCopyAll.IsEnabled = true; ;
                    
                }
            }
        }

    }
}
