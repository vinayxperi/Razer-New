

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
using RazerBase;
using RazerBase.Interfaces;
using Infragistics.Windows.DockManager;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.ComponentModel;

namespace Adjustment
{


    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class AdjustmentFolder : ScreenBase, IScreen
    {
        private static readonly string approvalObjectName = "AdjustmentApproval";

        private string AdjustmentID { get; set; }


        private string windowCaption;
        public bool IsScreenInserting = false;
        public int adjStatus = 0;
        public string wfStatus;
        public string adjType;
        public string adjTypeshort;
        public string WindowCaption
        {
            get { return windowCaption; }
        }

        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBase constructor is also called.
        /// </summary>
        public AdjustmentFolder()
            : base()
        {
            // Create Controls
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "Adjustment";
            this.DoNotSetDataContext = false;
            // set the businessObject
            this.CurrentBusObj = businessObject;
            //allow deletes
            this.CanExecuteDeleteCommand = true;
            // add the Tabs
            TabCollection.Add(AdjustmentGeneralTab);
            TabCollection.Add(AdjustmentDetailTab);
            TabCollection.Add(AdjustmentAcctgTab);
            TabCollection.Add(AdjustmentAttachTab);
            TabCollection.Add(AdjustmentCommentsTab);
            

            // if there are parameters than we need to load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                //string AdjustmentID = this.CurrentBusObj.Parms.ParmList.Rows[0]["document_id"].ToString();

                string AdjustmentID = (from x in this.CurrentBusObj.Parms.ParmList.AsEnumerable()
                                       where x.Field<string>("ParmName") == "@document_id"
                                       select x.Field<string>("Value")).FirstOrDefault();

                //set document_id for View tab
                this.loadParms(AdjustmentID);
                // load the data
                this.Load();
                //AdjustmentGeneralTab.FirstTime = true;
                if (CurrentBusObj.ObjectData.Tables["holdsubs"] != null && CurrentBusObj.ObjectData.Tables["holdsubs"].Rows.Count > 0)
                {
                    AdjustmentGeneralTab.FirstTime = true;
                    AdjustmentGeneralTab.chkSubsHold.IsChecked = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["holdsubs"].Rows[0]["count"]);
                }
                AdjustmentGeneralTab.FirstTime = false;
                // Set the Header
                //Need to chack               
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
                {
                    windowCaption = "Adjustment ID -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
                    txtAdjustmentID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
                    txtAdjustmentType.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["adjustment_type_desc"].ToString();
                    adjType = Convert.ToString(txtAdjustmentType.Text);
                    adjStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]);
                    AdjustmentApprovalTab.invoiceNumber = txtAdjustmentID.Text;
                    AdjustmentApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
                    wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];

                    //RES 7/31/13 Phase 3.1 set the rebill column heading to reverse on reverse adjustments
                    DataRecord GridRecord = null;
                    DataRecord GridRecord2 = null;
                    if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["adjustment_type_desc"].ToString() == "Reverse Invoice")
                    {
                        GridRecord = AdjustmentDetailTab.gridAdjustmentDetailAcctTab.ActiveRecord;
                        GridRecord.Cells["rebill_type"].Field.Label = "Reverse";
                        GridRecord2 = AdjustmentDetailTab.gridAdjustmentDetailTab.ActiveRecord;
                        GridRecord2.Cells["rebill_type"].Field.Label = "Reverse";
                    }
                    else
                    {
                        if (this.CurrentBusObj.ObjectData.Tables["acct"].Rows.Count != 0)
                        {
                            GridRecord2 = AdjustmentDetailTab.gridAdjustmentDetailTab.ActiveRecord;
                            GridRecord2.Cells["rebill_type"].Field.Label = "Rebill";
                        }
                        if (this.CurrentBusObj.ObjectData.Tables["acctdetail"].Rows.Count != 0)
                        {
                            GridRecord = AdjustmentDetailTab.gridAdjustmentDetailAcctTab.ActiveRecord;
                            GridRecord.Cells["rebill_type"].Field.Label = "Rebill";
                        }
                    }

                    if (adjType == "NSF")
                        adjTypeshort = "NSF";
                    else
                        adjTypeshort = adjType.Substring(0, 4);
                    if (adjTypeshort.ToString()  != "Conv")
                    {
                        //only valid for conversion invoice adjustment
                        AdjustmentDetailTab.gridAdjustmentDetailAcctTab.ContextMenuGenericIsVisible1 = false;

                    }
                    else
                    {
                        //only valid for conversion invoice adjustment and if they have more than View Only
                        if ( Adjustment.AdjustmentDetailTab.RMCOn == true)
                            AdjustmentDetailTab.gridAdjustmentDetailAcctTab.ContextMenuGenericIsVisible1 = true;

                    }
                    if (adjStatus.ToString() == "1" || wfStatus[0] == 'A')
                    {
                       //need to set adjreason to enabled = false
                        AdjustmentGeneralTab.txtAdjReason.IsReadOnly = true;
                        //need to set adjreason to enabled = false
                        AdjustmentGeneralTab.txtAdjReason.IsReadOnly = true;
                        AdjustmentApprovalTab.btnSubmit.IsEnabled = false;
                        AdjustmentApprovalTab.btnAddApprover.IsEnabled = false;
                        AdjustmentApprovalTab.btnApprove.IsEnabled = false;
                        AdjustmentApprovalTab.btnInquiry.IsEnabled = false;
                        AdjustmentApprovalTab.btnReject.IsEnabled = false;
                        AdjustmentApprovalTab.btnReply.IsEnabled = false;
                        //cannot change account code if posted
                        AdjustmentDetailTab.gridAdjustmentDetailAcctTab.ContextMenuGenericIsVisible1 = false;




                    }
                        else
                    {
                         AdjustmentGeneralTab.txtAdjReason.IsReadOnly = false;
                    }


                    if (wfStatus[0] == 'N' || wfStatus[0] == 'R')
                    {
                        AdjustmentApprovalTab.btnSubmit.IsEnabled = true;
                        AdjustmentApprovalTab.btnAddApprover.IsEnabled = false;
                        AdjustmentApprovalTab.btnApprove.IsEnabled = false;
                        AdjustmentApprovalTab.btnInquiry.IsEnabled = false;
                        AdjustmentApprovalTab.btnReject.IsEnabled = false;
                        AdjustmentApprovalTab.btnReply.IsEnabled = false;
                    }


                }
                else
                    Messages.ShowWarning("Adjustment not found!");
            }
            //if (this.CurrentBusObj.ObjectData != null)
            //{
            //    if (this.CurrentBusObj.ObjectData.Tables["attachments"].Rows.Count != 0)
            //        foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["attachments"].Rows)
            //        {
            //            string PathFile = dr["path"].ToString() + dr["prod_filename"].ToString();
            //            if (!System.IO.File.Exists(PathFile)) dr["color_status"] = 3;
            //        }
            //}
        }


        private void loadParms(string AdjustmentID)
        {
            try
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //if adjustment number passed load document id
                if (AdjustmentID != "")
                {
                    this.CurrentBusObj.Parms.AddParm("@document_id", AdjustmentID);
                    this.CurrentBusObj.Parms.AddParm("@external_char_id", AdjustmentID);
                    this.CurrentBusObj.Parms.AddParm("@user_id", cGlobals.UserName);
                }
                else
                {
                    //if adjustmentid NOT passed load   with global parm adjustmentid if exists
                    if (cGlobals.ReturnParms.Count > 0)
                    {
                        this.CurrentBusObj.Parms.AddParm("@document_id", cGlobals.ReturnParms[0].ToString());
                        //for attachments
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", cGlobals.ReturnParms[0].ToString());
                    }
                    //set dummy vals
                    else
                    {
                        this.CurrentBusObj.Parms.AddParm("@document_id", "-1");
                        //for attachments
                        this.CurrentBusObj.Parms.AddParm("@external_char_id", "-1");
                    }
                }
                //comment tab parms
                this.CurrentBusObj.Parms.AddParm("@comment_type", "AJ");
                this.CurrentBusObj.Parms.AddParm("@comment_attachments_id", "-1");
                //attachment parms
                this.CurrentBusObj.Parms.AddParm("@attachment_type", "AATTACH");
                this.CurrentBusObj.Parms.AddParm("@external_int_id", "0");
                this.CurrentBusObj.Parms.AddParm("@location_id", "-1");

            }
            catch (Exception ex)
            {
                Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
            }

        }      


        private void txtAdjustmentID_GotFocus(object sender, RoutedEventArgs e)
        {
            AdjustmentID = txtAdjustmentID.Text;
        }



        private void txtAdjustmentID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAdjustmentID.Text != AdjustmentID)

                ReturnData(txtAdjustmentID.Text, "@document_id");
            //RES 11/25/19 change color to red on missing attachments
            //if (this.CurrentBusObj.ObjectData != null)
            //{
            //    if (this.CurrentBusObj.ObjectData.Tables["attachments"].Rows.Count != 0)
            //    {
            //        foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["attachments"].Rows)
            //        {
            //            string PathFile = dr["path"].ToString() + dr["prod_filename"].ToString();
            //            if (!System.IO.File.Exists(PathFile))
            //                dr["color_status"] = 3;
            //        }
            //    }
            //}
        }

        private void txtAdjustmentID_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                adjStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]);
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                if (adjStatus.ToString() == "1" || wfStatus[0] == 'A')
                {
                    //need to set adjreason to enabled = false
                    AdjustmentGeneralTab.txtAdjReason.IsReadOnly = true;
                    AdjustmentApprovalTab.btnSubmit.IsEnabled = false;
                    AdjustmentApprovalTab.btnAddApprover.IsEnabled = false;
                    AdjustmentApprovalTab.btnApprove.IsEnabled = false;
                    AdjustmentApprovalTab.btnInquiry.IsEnabled = false;
                    AdjustmentApprovalTab.btnReject.IsEnabled = false;
                    AdjustmentApprovalTab.btnReply.IsEnabled = false;


                }
                else
                {
                    AdjustmentGeneralTab.txtAdjReason.IsReadOnly = false;
                }

                if (wfStatus[0] == 'N' || wfStatus[0] == 'R')
                {
                    AdjustmentApprovalTab.btnAddApprover.IsEnabled = false;
                    AdjustmentApprovalTab.btnApprove.IsEnabled = false;
                    AdjustmentApprovalTab.btnInquiry.IsEnabled = false;
                    AdjustmentApprovalTab.btnReject.IsEnabled = false;
                    AdjustmentApprovalTab.btnReply.IsEnabled = false;
                    AdjustmentApprovalTab.btnSubmit.IsEnabled = true;
                }
                //RES 7/31/13 Phase 3.1 set the rebill column heading to reverse on reverse adjustments
                DataRecord GridRecord = null;
                DataRecord GridRecord2 = null;
                if (this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["adjustment_type_desc"].ToString() == "Reverse Invoice")
                {
                    GridRecord = AdjustmentDetailTab.gridAdjustmentDetailAcctTab.ActiveRecord;
                    GridRecord.Cells["rebill_type"].Field.Label = "Reverse";
                    GridRecord2 = AdjustmentDetailTab.gridAdjustmentDetailTab.ActiveRecord;
                    GridRecord2.Cells["rebill_type"].Field.Label = "Reverse";
                }
                else
                {
                    if (this.CurrentBusObj.ObjectData.Tables["acct"].Rows.Count != 0)
                    {
                        GridRecord2 = AdjustmentDetailTab.gridAdjustmentDetailTab.ActiveRecord;
                        GridRecord2.Cells["rebill_type"].Field.Label = "Rebill";
                    }
                    if (this.CurrentBusObj.ObjectData.Tables["acctdetail"].Rows.Count != 0)
                    {
                        GridRecord = AdjustmentDetailTab.gridAdjustmentDetailAcctTab.ActiveRecord;
                        GridRecord.Cells["rebill_type"].Field.Label = "Rebill";
                    }
                }
                return true;
            }
            else
            {
                Messages.ShowWarning("Adjustment Not Found");
                return false;
            }
        }


        /// <summary>
        /// pop screen must be public because used in modal screens
        /// </summary>
        /// <param name="SearchValue"></param>
        /// <param name="DbParm"></param>
        public void ReturnData(string SearchValue, string DbParm)
        {
            //if no value do nothing
            if (SearchValue == "") return;
            //Add new parameters
            loadParms(SearchValue);
            //KSH - 8/24/12 clear comments/attachments grid if applicable bus obj
            clrCommentsAttachmentsObj();
            //load data
            //if coming from save do not do this...
            this.Load();
            //AdjustmentGeneralTab.FirstTime = true;
            if (CurrentBusObj.ObjectData.Tables["holdsubs"] != null && CurrentBusObj.ObjectData.Tables["holdsubs"].Rows.Count > 0)
            {
                AdjustmentGeneralTab.FirstTime = true;
                AdjustmentGeneralTab.chkSubsHold.IsChecked = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["holdsubs"].Rows[0]["count"]);                
            }
            AdjustmentGeneralTab.FirstTime = false;
            //if invoiceNumber found then set header and pop otherwise send message
            if (chkForData())
            {
                SetHeaderName();
                AdjustmentApprovalTab.invoiceNumber = txtAdjustmentID.Text;
                AdjustmentApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
            }
        }

        private void SetHeaderName()
        {
            ContentPane p = (ContentPane)this.Parent;
            //Sets the header name when being called from another folder
            if (txtAdjustmentID.Text == null)
            {
                windowCaption = "Adjustment ID -" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
                txtAdjustmentID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
                p.Header = "Adjustment ID -" + txtAdjustmentID.Text;
                txtAdjustmentType.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["adjustment_type_desc"].ToString();



            }
            ////Sets the header name from within same folder
            else
            {
                //ContentPane p = (ContentPane)this.Parent;
                //p.Header = "Adjustment ID -" + txtAdjustmentID.Text;
                txtAdjustmentID.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
                p.Header = "Adjustment ID -" + txtAdjustmentID.Text;
                txtAdjustmentType.Text = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["adjustment_type_desc"].ToString();

            }
        }

        public override void New()
        {
            //turn on inserting flag, needed for setting proper screen state
            IsScreenInserting = true;
            AdjustmentApprovalTab.ApprovalBusinessObject = new cBaseBusObject(approvalObjectName);
           //load dialog Enter Adjustment form
        }

        public override void Delete()
        {
            this.Load();

            int DeleteFlag = 1;
            string SubmitID;
            string RoleDesc;

            if (this.CurrentBusObj.ObjectData.Tables["general"].Rows.Count != 0)
            {
                adjStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]);
                //if (AdjustmentID == null)
                AdjustmentID =  this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString();
                wfStatus = (string)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["approval_description"];
                DeleteFlag = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["delete_flag"]);
                SubmitID = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["user_id"].ToString();
                RoleDesc = this.CurrentBusObj.ObjectData.Tables["role"].Rows[0]["role_description"].ToString();
                //If adjustment Approved or In Process, can not delete.
     
                //If posted cannot delete adjustment
                if (((adjStatus.ToString() == "1") || (wfStatus[0] == 'A') || ((wfStatus[0] == 'I') & DeleteFlag > 0) || ((SubmitID.Trim().ToUpper()) != cGlobals.UserName.Trim().ToUpper())) &&
                   ((wfStatus != "Posting Error") && (RoleDesc != "Accounting Manager" && RoleDesc != "Accounting Mgr with Approval")))
                    //(wfStatus != "Posting Error"))
                    Messages.ShowWarning("Cannot delete an adjustment that has been posted or approved or you are not the submitter");
                else
                {
                    //if (((wfStatus == "Posting Error") && (RoleDesc == "Accounting Manager" || RoleDesc == "Accounting Mgr with Approval")) || (wfStatus != "Posting Error"))
                    if (((wfStatus == "Posting Error" || wfStatus == "Rejected") && (RoleDesc == "Accounting Manager" || RoleDesc == "Accounting Mgr with Approval")) ||
                        (wfStatus != "Posting Error" && wfStatus[0] != 'I'))
                    {
                        MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to delete Adjustment ID " + AdjustmentID.ToString() + "?", System.Windows.MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {

                            if (cGlobals.BillService.DeleteAdjustment(AdjustmentID) == true)
                            {
                                Messages.ShowWarning("Adjustment ID " + AdjustmentID + " Deleted");
                                //KSH - 8/24/12 clear comments/attachments grid if applicable bus obj
                                clrCommentsAttachmentsObj();
                                this.Load();
                                txtAdjustmentID.Text = "";
                                txtAdjustmentType.Text = "";
                                ContentPane p = (ContentPane)this.Parent;
                                p.Header = "Adjustment ID";
                            }
                            else
                                Messages.ShowWarning("Error Deleting Adjustment");
                        }
                        else
                            Messages.ShowMessage("Adjustment not deleted", MessageBoxImage.Information);
                    }
                    else
                        if (wfStatus == "Posting Error")
                            Messages.ShowWarning("Only Accounting Manager can delete adjustment with posting error");
                        if (wfStatus[0] == 'I')
                            Messages.ShowWarning("Cannot delete adjustment that is in process.  It must be rejected");
                }
            }
            else
                Messages.ShowWarning("No adjustment to delete");
        }
     
        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
                ////check if attachment tab files need to be copied
                //if (cGlobals.GlobalAttachmentsStorageList.Count > 0)
                //{
                //    //if so pass attachment data table to attachment helper class
                //    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                //    //attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables["Attachments"]);

                //}

                ////check if comment attachment files need to be copied
                //if (cGlobals.GlobalCommentAttachmentsStorageList.Count > 0)
                //{
                //    //if so pass attachment data table to attachment helper class
                //    AttachmentHelper attachHelp = new AttachmentHelper(this.CurrentBusObj);
                //    attachHelp.doAttachments(this.CurrentBusObj.ObjectData.Tables["Comment_Attachment"]);

                //}
                Messages.ShowInformation("Save Successful");
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem m = (MenuItem)sender;
            //Interaction.MsgBox(m.Header);

            switch (m.Name)
            {
                case "ConvInv":
                    //open Conversion Invoice Screen
                    AdjustmentConversionInvoice2 ConvInvScreen = new AdjustmentConversionInvoice2(this);
                    callScreen("Accounting - Conversion Screen", ConvInvScreen, 600, 1240);
                    break;
                case "AcctAdjUnappliedCash":
                    //open Accounting Adjustment Unapplied Cash Entry Screen
                    AdjustmentAccountingUACash AcctAdjUACashScreen = new AdjustmentAccountingUACash(this);
                    callScreen("Accounting - Credit Screen", AcctAdjUACashScreen, 650, 980);
                    break;
                case "AcctAdjDocument":
                    //open Accounting Adjustment Document Entry Screen
                    AdjustmentAccountingDocument AcctAdjDocScreen = new AdjustmentAccountingDocument(this);
                    callScreen("Accounting - Debit Screen", AcctAdjDocScreen, 650, 1300);
                    break;
                case "MiscCrDb":
                    //open Misc Credit Debit
                    AdjustmentMiscCreditDebit MiscCrDbScreen = new AdjustmentMiscCreditDebit(this);
                    callScreen("Miscellaneous Credit/Debit Screen", MiscCrDbScreen, 650, 995);
                    break;
                case "Cash":
                    //open Cash Adj
                    AdjustmentCash CashAdjScreen = new AdjustmentCash(this);
                    callScreen("Cash Adjustment Screen", CashAdjScreen, 880, 1020);
                    break;
                case "UnappliedCash":
                    //open ApplyUACash Adj
                    AdjustmentApplyUACash ApplyUACashAdjScreen = new AdjustmentApplyUACash(this);
                    callScreen("Apply Unapplied Cash Adjustment Screen", ApplyUACashAdjScreen, 880, 1020);
                    break;
                case "CreditInvoice":
                    //open Credit Adj
                    AdjustmentCreditDebit CreditDebitAdjScreen = new AdjustmentCreditDebit(this);
                    CreditDebitAdjScreen.AdjType = 1;
                    callScreen("Credit Invoice Adjustment Screen", CreditDebitAdjScreen, 880, 1020);
                    break;
                case "DebitInvoice":
                    //open Debit Adj
                    AdjustmentCreditDebit DebitCreditAdjScreen = new AdjustmentCreditDebit(this);
                    DebitCreditAdjScreen.AdjType = 2;
                    callScreen("Debit Invoice Adjustment Screen", DebitCreditAdjScreen, 880, 1020);
                    break;
                //case "CreditWHT":
                //    //open WHT Adj
                //    WHTAdjustment WHTAdjustmentScreen = new WHTAdjustment(this);
                //    WHTAdjustmentScreen.whtType = 0;
                //    callScreen("Credit WHT Adjustment Screen", WHTAdjustmentScreen, 880, 1020);
                //    break;
                    //CLB Release 3.1 changing to WHT and changing to AdjsutmentWHT
                case "WHT":
                    //open WHT Adj
                    AdjustmentWHT AdjustmentWHTScreen = new AdjustmentWHT(this);
                    //AdjustmentWHTScreen.whtType = 0;
                    callScreen("WHT Adjustment Screen", AdjustmentWHTScreen, 880, 1020);
                    break;
                case "ReverseWHT":
                    //open WHT Adj
                    AdjustmentReverseWHT AdjustmentReverseWHTScreen = new AdjustmentReverseWHT(this);
                    callScreen("Reverse WHT Adjustment Screen", AdjustmentReverseWHTScreen, 880, 1020);
                    break;
                case "WHTCountry":
                    //open WHTCountry Adj
                    AdjustmentWHTCountry AdjustmentWHTCountryScreen = new AdjustmentWHTCountry(this);
                    callScreen("WHT Adjustment with Country Update Screen", AdjustmentWHTCountryScreen, 880, 1020);
                    break;
                case "RebillInvoice":
                    //open Debit Adj
                    AdjustmentRebill RebillAdjScreen = new AdjustmentRebill(this);
                    //RebillAdjScreen.AdjType = 2;
                    callScreen("Rebill Invoice Adjustment Screen", RebillAdjScreen, 880, 1020);
                    break;
                case "WriteOff":
                    //open WHT Adj
                    AdjustmentWriteoff AdjustmentWriteoffScreen = new AdjustmentWriteoff(this);
                    callScreen("Write Off Adjustment Screen", AdjustmentWriteoffScreen, 880, 1020);
                    break;
                case "NSF":
                    //open WHT Adj
                    AdjustmentNSF AdjustmentNSFScreen = new AdjustmentNSF(this);

                    callScreen("Insufficient Funds Adjustment Screen", AdjustmentNSFScreen, 880, 1020);
                    break;
                case "Upload":
                    Messages.ShowInformation("Under Construction");
                    break;
                case "CustomInvoiceCredit":
                    //open Credit Adj
                    AdjustmentCustomInvoice CustomInvoiceCreditAdjScreen = new AdjustmentCustomInvoice(this);
                    CustomInvoiceCreditAdjScreen.AdjType = 1;
                    callScreen("Custom Invoice Credit Adjustment Screen", CustomInvoiceCreditAdjScreen, 880, 1020);
                    break;
                case "CustomInvoiceDebit":
                    //open Debit Adj
                    AdjustmentCustomInvoice CustomInvoiceDebitAdjScreen = new AdjustmentCustomInvoice(this);
                    CustomInvoiceDebitAdjScreen.AdjType = 2;
                    callScreen("Custom Invoice Debit Adjustment Screen", CustomInvoiceDebitAdjScreen, 880, 1020);
                    break;
                case "Refund":
                    //open Debit Adj
                    AdjustmentRefund AdjustmentRefundScreen = new AdjustmentRefund(this);
                    //AdjustmentRefundScreen.AdjType = 12;
                    callScreen("Refund Unapplied Cash Adjustment Screen", AdjustmentRefundScreen, 880, 1020);
                    break;
                case "ReverseInvoice":
                    //open Debit Adj
                    AdjustmentReverse ReverseAdjScreen = new AdjustmentReverse(this);
                    //RebillAdjScreen.AdjType = 2;
                    callScreen("Reverse Invoice Adjustment Screen", ReverseAdjScreen, 880, 1020);
                    break;
                case "CorrectBankAcct":
                    //open Correct Bank Acct
                    AdjustmentCorrectBankAcct AdjustmentCorrectBankAcctScreen = new AdjustmentCorrectBankAcct(this);
                    //AdjustmentRefundScreen.AdjType = 7;
                    callScreen("Correct Bank Account Adjustment Screen", AdjustmentCorrectBankAcctScreen, 880, 1020);
                    break;
                case "BankFee":
                    //open Bank Fee 
                    AdjustmentBankFee AdjustmentBankFeeScreen = new AdjustmentBankFee(this);
                    //AdjustmentRefundScreen.AdjType = 7;
                    callScreen("Bank Fee Adjustment Screen", AdjustmentBankFeeScreen, 880, 1020);
                    break;
                case "SLACreditInvoice":
                    //open SLA Credit Adj
                    AdjustmentCreditDebit SLACreditDebitAdjScreen = new AdjustmentCreditDebit(this);
                    SLACreditDebitAdjScreen.AdjType = 22;
                    SLACreditDebitAdjScreen.txtZeroTax.Visibility = Visibility.Hidden;
                    SLACreditDebitAdjScreen.txtCurrentTaxRate.Visibility = Visibility.Hidden;
                    callScreen("SLA Credit Invoice Adjustment Screen", SLACreditDebitAdjScreen, 880, 1020);
                    break;
                case "BillingDiff":
                    //open Billing Differential Adjustment
                    AdjustmentBillingDifferential BillingDiffScreen = new AdjustmentBillingDifferential(this);
                    callScreen("Billing Differential Screen", BillingDiffScreen, 650, 995);
                    break;
                case "FX":
                    //open Currency Conversion Adjustment
                    AdjustmentFX FXScreen = new AdjustmentFX(this);
                    callScreen("Currency Conversion Adjustment", FXScreen, 1000, 1200);
                    break;
           }  
        } 

        /// <summary>
        /// Do everything necessary to show the modal adj screen
        /// </summary>
        /// <param name="RulesObj"></param>
        private void callScreen(string strScreenTitle, UserControl ScreenObj, int screenHeight, int intScreenWidth)
        {
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window WindowHandle = new System.Windows.Window();
            //set new window properties///////////////////////////
            WindowHandle.Title = strScreenTitle;
            WindowHandle.MaxHeight = screenHeight;
            //WindowHandle.MaxWidth = intScreenWidth;
            //WindowHandle.ShowInTaskbar = false;
            WindowHandle.Width = intScreenWidth;

            if (ScreenObj is ScreenBase)
            {
                ScreenBase sb = ScreenObj as ScreenBase;
                sb.CancelCloseOnSaveConfirmation = true;
                sb.Name = "WindowHandleDialog";
            }

            WindowHandle.Closing += new CancelEventHandler(WindowHandle_Closing);
            /////////////////////////////////////////////////////
            //set user control as content of new window
            WindowHandle.Content = ScreenObj;
            //open new window with embedded user control
            WindowHandle.ShowDialog();
        }

        private void WindowHandle_Closing(object sender, CancelEventArgs e)
        {
            if (sender is DependencyObject)
            {
                ScreenBase screen = UIHelper.FindChild<ScreenBase>(sender as DependencyObject, "WindowHandleDialog");


                if ((screen != null) && (screen.CanExecuteCloseCommand))
                {
                    screen.ScreenBaseIsClosing = true;
                    screen.Prep_ucBaseGridsForSave();

                    screen.Close();
                    if (!screen.SaveSuccessful)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        e.Cancel = false; //screen.StopCloseIfCancelCloseOnSaveConfirmationTrue;
                    }
                }
            }
        }

        private void clrCommentsAttachmentsObj()
        {
            if (this.CurrentBusObj != null)
                if (this.CurrentBusObj.ObjectData != null)
                    this.CurrentBusObj.ObjectData.Tables["comment_attachment"].Clear();
        }

    }
}


 
