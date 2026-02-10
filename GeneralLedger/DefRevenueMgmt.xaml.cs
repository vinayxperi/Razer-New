using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Linq;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;





using Infragistics.Windows.DockManager;


using Infragistics.Windows.Controls;

using System.ComponentModel;


namespace GeneralLedger
{

    /// Interaction logic for DefRevenueMgmt.xaml

    public partial class DefRevenueMgmt : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "GLVchrFieldLayouts";
        public string DefAcct = "";
        public string PoolID = "";
        public string Company = "";
        public string Customer = "";
        public string Document = "";
        DataRecord record = default(DataRecord);
        public string defReason = "";
        public int closedFlag = 0;



        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
        }


        public DefRevenueMgmt()
            : base()
        {
            InitializeComponent();
        }


        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //set isscreendirty = false to prevent save message
            //this.IsScreenDirty = false;

            
            this.CanExecuteDeleteCommand = false;
            this.CanExecuteNewCommand = false;
            this.CanExecuteRefreshCommand = false;
           
            this.CanExecuteSaveCommand = false;
            GridDefPoolMgmt.WindowZoomDelegate = ReturnSelectedData;
            GridDefPoolMgmt.xGrid.FieldLayoutSettings = layouts;
            GridDefPoolMgmt.FieldLayoutResourceString = "DefPoolMgmt";
            GridDefPoolMgmt.SetGridSelectionBehavior(true, false);
            this.MainTableName = "main";
            GridDefPoolMgmt.MainTableName = "main";
            GridDefPoolMgmt.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "pool_id" }, ChildGrids = { GridDefPoolMgmtDetail }, ParentFilterOnColumnNames = { "pool_id" } });
            GridDefPoolMgmtDetail.FieldLayoutResourceString = "DefPoolMgmtDetail";
             
            GridDefPoolMgmtDetail.SetGridSelectionBehavior(false, false);
            GridDefPoolMgmtDetail.MainTableName = "detail";
            GridDefPoolMgmtDetail.xGrid.FieldSettings.AllowEdit = true;
            GridDefPoolMgmtDetail.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);

            GridDefPoolMgmtDetail.xGrid.RecordActivated += new EventHandler<Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs>(xGrid_RecordActivated);

            GridCollection.Add(GridDefPoolMgmt);
            GridCollection.Add(GridDefPoolMgmtDetail);

            //this.Load(businessObject);
            this.CurrentBusObj = businessObject;
            //This.Load();
            //if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            //{
            //    string DefAcct = this.CurrentBusObj.Parms.ParmList.Rows[0]["def_account_code"].ToString();
            //    //GridDefPool.LoadGrid(businessObject, GridDefPool.MainTableName);

            //    //GridDefPoolDetail.LoadGrid(businessObject, GridDefPoolDetail.MainTableName);
            //    this.Load();

            //    System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            //}

        }



        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {



            GridDefPoolMgmtDetail.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);

        }

        void xGrid_RecordActivated(object sender, Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs e)
        {


           
            
            FieldLengthConverter converter = new FieldLengthConverter();
		

            //Set edit only on amount_actual_deferred or percent column depending on deferred type.
            record = (DataRecord)(GridDefPoolMgmtDetail.xGrid.ActiveRecord);




            if (record != null)
            {

                if (record.Cells["def_reason"].Value == null)
                {
                    return;
                }
                
                
                defReason = record.Cells["def_reason"].Value.ToString().Trim();

                

                //Messages.ShowInformation("This is Activerecord Def Reason: " + defReason );

                if (defReason == "Percentage Complete" || defReason == "Completed Contract")
                {

                    if (defReason == "Percentage Complete")
                    {
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["percent_delivered"].Settings.AllowEdit = true;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["percent_delivered"].Visibility = Visibility.Visible;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["completed_contract"].Visibility = Visibility.Collapsed;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_deferred"].Visibility = Visibility.Collapsed;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["po_number"].Visibility = Visibility.Collapsed;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_override"].Visibility = Visibility.Collapsed;

                        
                        
                    }
                                        
                    if (defReason == "Completed Contract")
                    {
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["completed_contract"].Settings.AllowEdit = true;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["completed_contract"].Visibility = Visibility.Visible;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["percent_delivered"].Visibility = Visibility.Collapsed;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_deferred"].Visibility = Visibility.Collapsed;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["po_number"].Visibility = Visibility.Collapsed;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_override"].Visibility = Visibility.Collapsed;

                    }
                   
                }
                else 
                    
                {   if (defReason == "Rate Dispute")
                    {
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_deferred"].Settings.AllowEdit = false;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_deferred"].Visibility = Visibility.Visible;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["percent_delivered"].Visibility = Visibility.Collapsed;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["completed_contract"].Visibility = Visibility.Collapsed;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["po_number"].Visibility = Visibility.Collapsed;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_override"].Visibility = Visibility.Visible;
                        GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_override"].Settings.AllowEdit = true;
                        
                    }
                    else
                    {
                        if (defReason == "PO Required")
                        {
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["percent_delivered"].Visibility = Visibility.Collapsed;
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["completed_contract"].Visibility = Visibility.Collapsed;
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_deferred"].Visibility = Visibility.Collapsed;
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["po_number"].Visibility = Visibility.Visible;
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["po_number"].Settings.AllowEdit = true;
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_override"].Visibility = Visibility.Collapsed;

                        }
                        else
                        {
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["percent_delivered"].Visibility = Visibility.Collapsed;
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["completed_contract"].Visibility = Visibility.Collapsed;
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_deferred"].Visibility = Visibility.Collapsed;
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["po_number"].Visibility = Visibility.Collapsed;
                            GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_override"].Visibility = Visibility.Collapsed;
                        }

                    }

                }
                closedFlag = (int)(record.Cells["status_flag"].Value);

                if (closedFlag == 1)
                {
                    GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_deferred"].Settings.AllowEdit = false;
                    GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["percent_delivered"].Settings.AllowEdit = false;
                    GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["completed_contract"].Settings.AllowEdit = false;
                    GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["po_number"].Settings.AllowEdit = false;
                    GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["amt_override"].Settings.AllowEdit = false;

                   


                }

            }

        }








        public override void Save()
        {

            //Get records that were modified and do validation before saving.
            if (this.CurrentBusObj.ObjectData != null && this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0)
            {
                GridDefPoolMgmtDetail.xGrid.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

                foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["detail"].Rows)
                {
                    bool rowIsModified = (dr.RowState == DataRowState.Modified);
                    if (rowIsModified)
                    {
                       
                        //Logic to ensure only Completed Contract is 0 or 100.
                        if (dr["def_reason"].ToString() == "Completed Contract")
                        {
                            //Ensure Perecent is either 100 or 0
                            if ((float)Convert.ToDouble(dr["completed_contract"]) == 1.0)
                            {

                                continue;

                            }
                            else
                                if ((float)Convert.ToDouble(dr["completed_contract"])== 0.0)
                                {

                                    continue;

                                }
                                else
                                {
                                    Messages.ShowInformation("Completed Contract must be 0 or 100 Percent");
                                    return;
                                }
                        }

                        if (dr["def_reason"].ToString() == "Percentage Complete")
                        {
                            //Ensure Perecent is either 100 or 0
                            if ((float)Convert.ToDouble(dr["percent_delivered"]) > 1.0)
                            {
                                Messages.ShowInformation("Percent Delivered cannot be greater than 100");
                                return;

                            }
                            else
                                if ((float)Convert.ToDouble(dr["percent_delivered"]) < 0.0)
                                {

                                    Messages.ShowInformation("Percent Delivered cannot be less than 0");
                                    return;

                                }   
                        }                        
                        //Logic to check if amt_deferred is greater than amount_total 
                        if (dr["def_reason"].ToString() == "Rate Dispute")
                        {
                            //Ensure Perecent is either 100 or 0
                            if ((decimal)dr["amt_deferred"] > (decimal)dr["amount_total"])
                            {
                                Messages.ShowInformation("Amount Deferred cannot be greater than invoice line total: " + dr["amount_total"].ToString());
                                return;

                            }
                            else
                                if ((decimal)dr["amt_deferred"] < 0)
                                {

                                    Messages.ShowInformation("Amount Deferred cannot be less than 0");
                                    return;

                                }
                        }                        


                        
               





                    }


                }


            }
            else
            {
                return;
            }







            base.Save();
            if (SaveSuccessful)
            {

                Messages.ShowInformation("Save Successful!");

            }





        }



        /// Handler for double click on grid to return the app selected to run
        public void ReturnSelectedData()
        {

            //gCashBatchesToRun.ReturnSelectedData();
            //if (cGlobals.ReturnParms.Count > 0)
            //{
            //    jobName = cGlobals.ReturnParms[0].ToString();

            //}



        }


      
       

        public override void Close()
        {
             
        
             
            SaveSuccessful = true;
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (this.CurrentBusObj != null && (IsScreenDirty || ForceScreenDirty)
                && (this.SecurityContext == AccessLevel.ViewUpdate || this.SecurityContext == AccessLevel.ViewUpdateDelete))
            {
                var result = MessageBox.Show("Do you want to save your changes?", "Save", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                {
                    this.Save();
                    //@@Need to add code here to stop the window from closing if save fails
                    StopCloseIfCancelCloseOnSaveConfirmationTrue = true;
                }
            }
            //Set the business object to null so that we do not receive any false trues on future app close checks
            //if (this.CurrentBusObj != null && this.CurrentBusObj.ObjectData != null)
            //    this.CurrentBusObj.ObjectData = null;
        }

        
        



     
  

        //private void loadParms(string GLJournalID)
        //{
        //    try
        //    {
        //        //Clear the current parameters
        //        this.CurrentBusObj.Parms.ClearParms();
        //        //if adjustment number passed load document id
        //       if (!string.IsNullOrEmpty(DefAcct))
        //        {
        //            this.CurrentBusObj.Parms.AddParm("@def_account_code", DefAcct);
        //            //this.CurrentBusObj.Parms.AddParm("@pool_id", PoolID);
               

        //        }
        //        else
        //        {
        //            //if adjustmentid NOT passed load   with global parm adjustmentid if exists
        //            if (cGlobals.ReturnParms.Count > 0)
        //            {
        //                this.CurrentBusObj.Parms.AddParm("@def_account_code", cGlobals.ReturnParms[0].ToString());
        //                //this.CurrentBusObj.Parms.AddParm("@pool_id", cGlobals.ReturnParms[1].ToString());
                        
        //            }
        //            //set dummy vals
        //            else
        //            {

        //                this.CurrentBusObj.Parms.AddParm("@def_account_code", "-1");
        //                //this.CurrentBusObj.Parms.AddParm("@related_to_char_id", "-1");
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
        //    }

        //}




        //private bool chkForData()
        //{
        //    //if (this.CurrentBusObj.ObjectData.Tables["defPool"].Rows.Count != 0)
        //    //{

        //    //    return true;
        //    //}
        //    //else
        //    //{
        //    //    Messages.ShowWarning("Deferred Revenue Account Not Found");
        //    //    return false;
        //    //}
        //}


        //private void ReturnData(string SearchValue, string DbParm)
        //{
        //    ////if no value do nothing
        //    //if (SearchValue == "") return;
        //    ////Add new parameters
        //    //loadParms(SearchValue);
        //    ////load data
        //    ////if coming from save do not do this...
        //    //this.Load();
        //    ////if invoiceNumber found then set header and pop otherwise send message
        //    //if (chkForData())
        //    //{
        //    //    SetHeaderName();

        //    //}
        //}

        //private void SetHeaderName()
        //{
        //    //Sets the header name when being called from another folder
        //    //if (txtDefAcct.Text == null)
        //    //{
        //    //    txtDefAcct.Text = this.CurrentBusObj.ObjectData.Tables["defDetail"].Rows[0]["def_account_code"].ToString();
        //    //    windowCaption = "Deferred Account -" + txtDefAcct.Text ;
  
        //    //}
        //    ////Sets the header name from within same folder
        //    //else
        //    //{
        //    //    windowCaption = "Deferred Account -" + txtDefAcct.Text;
        //    //    //txtDefAcct.Text = this.CurrentBusObj.ObjectData.Tables["defDetail"].Rows[0]["def_account_code"].ToString();
             
        //    //}
        //}

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentBusObj.Parms.ClearParms();
            //Check to see if at least Customer or Document Id have been selected...
            //if ((String.IsNullOrEmpty(txtCustomer.Text)) && (String.IsNullOrEmpty(txtDocument.Text)))
            //{
            //    Messages.ShowWarning("A Customer ID or a Document ID must be selected.");
            //    return;
            //}


            //CurrentBusObj = new cBaseBusObject(this.CurrentBusObj.BusObjectName);
            if (String.IsNullOrEmpty(txtCompany.Text))
            {
                CurrentBusObj.Parms.AddParm("@company_code", " ");
            }
            else
            {
                CurrentBusObj.Parms.AddParm("@company_code", txtCompany.Text);
            }

            if (String.IsNullOrEmpty(txtCustomer.Text))
            {
                CurrentBusObj.Parms.AddParm("@receivable_account"," ");
            }
            else
            {
                CurrentBusObj.Parms.AddParm("@receivable_account", txtCustomer.Text);
            }

            if (String.IsNullOrEmpty(txtDocument.Text))
            {
                CurrentBusObj.Parms.AddParm("@document_id", " ");
            }
            else
            {
                CurrentBusObj.Parms.AddParm("@document_id", txtDocument.Text);
            }

            if (chkIncludeClosed.IsChecked == 1)
            {
                CurrentBusObj.Parms.AddParm("@status_flag", "1");
                GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["status_flag"].Visibility = Visibility.Visible;
            }
            else
            {
                CurrentBusObj.Parms.AddParm("@status_flag", "0");
                GridDefPoolMgmtDetail.xGrid.FieldLayouts[0].Fields["status_flag"].Visibility = Visibility.Collapsed;

            }



            this.Load();

            //GridDefPoolMgmt.LoadGrid(CurrentBusObj, GridDefPoolMgmt.MainTableName);

            //GridDefPoolMgmtDetail.LoadGrid(CurrentBusObj, GridDefPoolMgmtDetail.MainTableName);


            if (CurrentBusObj.HasObjectData)
            {
                GridDefPoolMgmt.LoadGrid(CurrentBusObj, GridDefPoolMgmt.MainTableName);

                GridDefPoolMgmtDetail.LoadGrid(CurrentBusObj, GridDefPoolMgmtDetail.MainTableName);

            }
            else
            {

                Messages.ShowWarning("No data met search criteria.");
                
            }
        }

        private void txtCompany_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            string currentCompany = "";
            if (txtCompany.Text != "")
            {
                currentCompany = txtCompany.Text;
                Company = txtCompany.Text;
            }

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            DefRevMgmtCompanyLookup f = new DefRevMgmtCompanyLookup();
            f.Init(new cBaseBusObject("DefRevMgmtCompanyLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtCompany.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                if (txtCompany.Text!= Company)
                {
                    //ReturnData(txtCompany.Text, "@Company");
                }
            }        

        }

        private void txtCustomer_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            string currentCustomer = "";
            if (txtCustomer.Text != "")
            {
                currentCustomer = txtCustomer.Text;
                Customer = txtCustomer.Text;
            }

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            DefRevMgmtCustomerLookup f = new DefRevMgmtCustomerLookup();
            f.Init(new cBaseBusObject("DefRevMgmtCustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtCustomer.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                if (txtCustomer.Text != Customer)
                {
                    //ReturnData(txtCustomer.Text, "@Customer");
                }
            }
        }

        private void txtDocument_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            string currentDocument = "";
            if (txtDocument.Text != "")
            {
                currentDocument = txtDocument.Text;
                Document = txtDocument.Text;
            }

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            DefRevMgmtDocumentLookup f = new DefRevMgmtDocumentLookup();
            f.Init(new cBaseBusObject("DefRevMgmtDocumentLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtDocument.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                if (txtDocument.Text != Document)
                {
                    //ReturnData(txtDocument.Text, "@Document");
                }
            }
        }



    }
}
