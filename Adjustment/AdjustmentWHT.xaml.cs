using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter;
using RazerBase.Lookups;
using System.Windows.Input;
using System.ComponentModel;
using Infragistics.Windows.DataPresenter.Events;
using System.Windows;

namespace Adjustment
{
    /// <summary>
    /// This class represents a 'AdjustmentWHT' object.
    /// </summary>
    public partial class AdjustmentWHT : ScreenBase, IPreBindable 
    {
        public cBaseBusObject  WHTAdjustmentBusObj = new cBaseBusObject();
        public AdjustmentFolder AdjFolderScreen;
        public Int32 AdjType;
        public ComboBoxItemsProvider cmbProductItemCombo { get; set; }
        private string DocumentId { get; set; }
        ////This datatable is being added so that the Amount to adjust text box can have a binding
        ////Do this for fields that contain informational data but that will not be saved
        ////so that you can use converters or other benefits of binding
        //DataTable dtMiscInfo = new DataTable("MiscInfo");
        decimal AdjTotal = 0.00M;
        decimal whtRate = 0;
        decimal incomeSubjectWHT = 0;
        decimal whtComputed = 0;
        decimal overrideamount = 0;
        decimal prevapplied = 0;
        decimal totcashamount = 0;
        decimal totcashdoc = 0;
        decimal cashremaining = 0;
        private bool IsSingleClickOrigin { get; set; }
        
        public AdjustmentWHT(AdjustmentFolder _AdjFolderScreen)
            : base()
        {
        //    //set obj
            this.CurrentBusObj = WHTAdjustmentBusObj;
            //name obj
            this.CurrentBusObj.BusObjectName = "AdjustmentWHT";
        //    //set Adj Folder screen obj
            AdjFolderScreen = _AdjFolderScreen;
        //    // This call is required by the designer.
            InitializeComponent();
        //    // Perform initializations for this object
            Init();
        }

    
        public void Init()
        {
        //    // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;
            this.MainTableName = "main";

            GridWHTINVDetail.MainTableName = "detail";
            GridWHTINVDetail.ConfigFileName = "AdjustmentWHTDetail";
            GridWHTINVDetail.FieldLayoutResourceString = "GridWHTINVDetail";
            GridWHTINVDetail.SkipReadOnlyCellsOnTab = true;
            //GridWHTINVDetail.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(GridWHTINVDetail_EditModeEnded);
            //Set the grid to allow edits, for readonly columns set the allowedit to false in the field layouts file
            GridWHTINVDetail.xGrid.FieldSettings.AllowEdit = true;
            GridWHTINVDetail.SetGridSelectionBehavior(false, false);
            GridWHTINVDetail.IsEnabled = true;
            GridCollection.Add(GridWHTINVDetail);

            GridRemittance.MainTableName = "remit";
            GridRemittance.ConfigFileName = "AdjustmentWHTRemit";
            GridRemittance.FieldLayoutResourceString = "GridWHTRemit";
            GridRemittance.SkipReadOnlyCellsOnTab = true;
            GridRemittance.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(GridRemittance_EditModeEnded);
            //    //Set the grid to allow edits, for readonly columns set the allowedit to false in the field layouts file
            GridRemittance.xGrid.FieldSettings.AllowEdit = true;
            GridRemittance.SetGridSelectionBehavior(false, false);
            GridRemittance.IsEnabled = true;
            GridCollection.Add(GridRemittance);

            GridWHTSearch.xGrid.FieldLayoutSettings.HighlightAlternateRecords = true;
            GridWHTSearch.DoNotSelectFirstRecordOnLoad = true;
            GridWHTSearch.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridWHTSearch.ContextMenuAddIsVisible = false;
            GridWHTSearch.ContextMenuRemoveIsVisible = false;
            //GridSearch.SingleClickZoomDelegate = SingleClickZoomDelegateSearch;
            GridWHTSearch.WindowZoomDelegate = DoubleClickZoomDelegateSearch;
            GridWHTSearch.MainTableName = "search";
            GridWHTSearch.ConfigFileName = "CashWHTSearch";
            GridWHTSearch.SetGridSelectionBehavior(true, false);
            GridWHTSearch.xGrid.FieldSettings.AllowEdit = false;
         
            GridWHTSearch.FieldLayoutResourceString = "GridWHTCashSearch";
            
            GridCollection.Add(GridWHTSearch);
            loadParms(DocumentId);
        //    //set initial screen state

            setInitScreenState();
            txtDocument.CntrlFocus();

     

        }

        private void txtDocument_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtDocument.Text != "")
            {
                //do this to keep grid from loading and binding re-running clearing user entered values
                if (DocumentId != txtDocument.Text)
                {
                    //Need to clear both grids and all screens
                    if (this.CurrentBusObj.HasObjectData)
                    {
                        SearchClear();
                        RemitClear();
                    }

                    //populate grid
                    popScreen();
                    //txtRunningAdjAmt.CntrlFocus();
                }
            }
        }

        private void popScreen()
        {
            //load parms 
            loadParms(DocumentId);
            //load the object
            ReturnData(txtDocument.Text, DocumentId);
        
     
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                txtAdjAmt.IsReadOnly = true;
                setInitScreenState();
                
                
            }
            else
            {
                Messages.ShowInformation("Document Not Found");
                txtAdjAmt.IsReadOnly = true;
                setInitScreenState();
                txtDocument.CntrlFocus();
            }
            
        }

        public void DoubleClickZoomDelegateSearch()
        {
            //set this to prevent GridWHTSearch single click delegate from firing when GridRemittance.xGrid.ActiveDataItem = row; runs
            IsSingleClickOrigin = true;
            DataRecord GridRecord = null;
            GridRecord = GridWHTSearch.ActiveRecord;
            DataRowView dr = GridRecord.DataItem as DataRowView;

            if (GridRecord != null)
            {
              
                //need to set the parms from the selected cash document and re-retrieve the remit table
                this.CurrentBusObj.changeParm("@cash_document_id", GridRecord.Cells["document_id"].Value.ToString());
                this.CurrentBusObj.changeParm("@seq_code", GridRecord.Cells["seq_code"].Value.ToString());
                this.CurrentBusObj.changeParm("@searchind", "1");
                this.CurrentBusObj.LoadData("remit");

                //if (AdjWHTVerification.ObjectData.Tables["search"] == null || AdjWHTVerification.ObjectData.Tables["search"].Rows.Count < 1)
                if (this.CurrentBusObj.ObjectData.Tables["remit"] == null || this.CurrentBusObj.ObjectData.Tables["remit"].Rows.Count < 1)
                {
                    Messages.ShowInformation("No records found");
                }
                else
                {
                    //this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["cash_applied_to_wht"] = Convert.ToDouble(GridRecord.Cells["wht_cash_used"].Value.ToString());
                    //this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["override_amount"] = Convert.ToDouble(GridRecord.Cells["wht_cash_remaining"].Value.ToString());
                    GridRemittance.LoadGrid(this.CurrentBusObj, "remit");
                    if (this.CurrentBusObj.ObjectData != null)
                        if (this.CurrentBusObj.ObjectData.Tables["remit"].Rows.Count > 0)
                        {
                            //prevapplied = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["cash_used"]);
                            //txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", prevapplied);
                            GridRemittance.xGrid.Focus();
                            GridRemittance.xGrid.FieldSettings.AllowEdit = true;
                            
                            (GridRemittance.xGrid.Records[GridRemittance.ActiveRecord.Index] as DataRecord).Cells["override_amount"].IsActive = true;
                            GridRemittance.xGrid.ExecuteCommand(DataPresenterCommands.StartEditMode);
                            totcashdoc = 0;
                            foreach (DataRecord r in GridRemittance.xGrid.Records)
                            {
                                //get original cash amount
                                totcashdoc += Convert.ToDecimal(r.Cells["amount"].Value);
                            }
                            prevapplied = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["cash_used"]);
                            cashremaining = totcashdoc - prevapplied;
                            if (cashremaining < 0) cashremaining = 0;
                            txtAdjAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", cashremaining);
                            this.Calculate_WHT();
                            
                        }
                }

               
            }
            else
                Messages.ShowInformation("A row must be selected from the Search Grid");

     
    
        }

        void GridRemittance_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            
            //commit user entered value to datatable
            GridRemittance.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            DataRecord GridRecord = null;
            GridRecord = GridRemittance.ActiveRecord;
            DataRowView dr = GridRecord.DataItem as DataRowView;
            DataView dv = dr.DataView;
            if (GridRecord != null)
            {
               
                if (GridRecord.Cells["override_amount"].Value.ToString() == "")
                {
                    GridRecord.Cells["override_amount"].Value = 0.00;
                }
            
           }
           
            // need to move the amount * 100 - WHT Rate to the Income Subject to WHT column and when you tab off of it, calculate the WHT Amount and populate
            // the Amount to adjust with that amount.  They can change the amount to adjust and that is used to create the adjustment
            this.Calculate_WHT();
        }

        void GridWHTINVDetail_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {

            ////commit user entered value to datatable
            //GridRemittance.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
            //DataRecord GridRecord = null;
            //GridRecord = GridRemittance.ActiveRecord;
            //DataRowView dr = GridRecord.DataItem as DataRowView;
            //DataView dv = dr.DataView;
            //if (GridRecord != null)
            //{

            //    if (GridRecord.Cells["override_amount"].Value.ToString() == "")
            //    {
            //        GridRecord.Cells["override_amount"].Value = 0.00;
            //    }

            //}

            //// need to move the amount * 100 - WHT Rate to the Income Subject to WHT column and when you tab off of it, calculate the WHT Amount and populate
            //// the Amount to adjust with that amount.  They can change the amount to adjust and that is used to create the adjustment
            //this.Calculate_WHT();
        }


        private void Calculate_WHT()
        {
            //decimal whtRate = 0;
            //decimal incomeSubjectWHT = 0;
            //decimal whtComputed = 0;
            //decimal Dincome = 0.00M;

            incomeSubjectWHT = 0;
            overrideamount = 0;
            totcashamount = 0;
            //prevapplied = 0;
            whtRate = Convert.ToDecimal(txtWHTRate.Text);
            foreach (DataRecord r in GridRemittance.xGrid.Records)
            {
                //sum adj amts
                incomeSubjectWHT += Convert.ToDecimal(r.Cells["override_amount"].Value) / ((100 - whtRate) / 100);
                overrideamount += Convert.ToDecimal(r.Cells["override_amount"].Value);
                totcashamount += Convert.ToDecimal(r.Cells["amount"].Value);
                //prevapplied += Convert.ToDecimal(r.Cells["cash_applied_to_wht"].Value);
            }
        
            //incomeSubjectWHT = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["override_amount"]) / (( 100 - whtRate) / 100);
            whtComputed = Math.Round(incomeSubjectWHT * (whtRate / 100),2);
            txtWHTAmount.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", whtComputed);
            //txtAdjAmt.Text = txtWHTAmount.Text;
            //RES 5/15/14 fill in amount to adjust if there is only 1 detail row
            if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count == 1)
            {
                string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
                //this.CurrentBusObj.ObjectData.Tables["detail"].Rows[0]["amount_to_adjust"] = Convert.ToDecimal(sAdjAmt) * -1;
                this.CurrentBusObj.ObjectData.Tables["detail"].Rows[0]["amount_to_adjust"] = Convert.ToDecimal(whtComputed) * -1; 
                //GridWHTINVDetail.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
                //GridWHTSearch.LoadGrid(this.CurrentBusObj, "detail");
            }


            txtIncomeWHT.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", incomeSubjectWHT);
            btnSave.IsEnabled = true;

            //txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
        }

        private void RemitClear()
        {
             
            //clear parms
            this.CurrentBusObj.changeParm("@cash_document_id", "");
            this.CurrentBusObj.changeParm("@seq_code","0");
            this.CurrentBusObj.changeParm("@searchind", "0");
            ////clear grid
            this.CurrentBusObj.LoadData("remit");
            GridWHTSearch.LoadGrid(this.CurrentBusObj, "remit");
        }

        private void SearchClear()
        {
            //clear search criteria
            txtCustNumSearch.Text = "";
            //txtCustNameSearch.Text = "";
           
            txtAmtSearch.Text = "";
            //clear parms
            this.CurrentBusObj.Parms.UpdateParmValue("@receivable_account", txtCustNumSearch.Text);
            ////this.CurrentBusObj.changeParm("@invoice_number", txtCustNameSearch.Text);

            this.CurrentBusObj.Parms.UpdateParmValue("@amount", "0");
            this.CurrentBusObj.Parms.UpdateParmValue("@searchind", "0");
            ////clear grid
            this.CurrentBusObj.LoadData("search");
            GridWHTSearch.LoadGrid(this.CurrentBusObj, "search");
        }

      
     
        //        //if amounts offset then enable save button
        //        //txtRunningOffsetAmt.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", Convert.ToDecimal(txtRunningOffsetAmt.Text));
       
        //}

             

       
        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Save();
        }

        public override void Save()
        {
            this.SaveSuccessful = false;
            decimal whtlimit = 0;
            decimal adjustamt = 0;
            //decimal prevapplied = 0;
            //decimal totcashamount = 0;
            decimal totapplied = 0;
            decimal totavail = 0;
            //decimal overrideamount = 0;
            //NEED TO COMPARE THE amount and amount override.  If they are different, need to update the comment on the adjustment
            //Need to make sure the amount applied to WHT plus the WHT adjustment amount do not exceed 
            //Potentially do this in the insert SP plus write to the wht_adjustment table in the SP
              if (this.CurrentBusObj.ObjectData != null)
                 if (this.CurrentBusObj.ObjectData.Tables["remit"].Rows.Count > 0)
                 {
                     if (Convert.ToDecimal(txtWHTRate.Text) == 0)
                     {
                         Messages.ShowWarning("Withholding Tax Rate not found for country/province/product item combination.  It must be set up before adjustment will save.");
                         return;
                     }
                     this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["item_id"] = Convert.ToInt32(cmbProductItem.SelectedValue);
                     //this.CurrentBusObj.ObjectData.Tables["detail"].Rows[0]["item_id"] = Convert.ToInt32(cmbProductItem.SelectedValue);
                     string sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
                     adjustamt = Convert.ToDecimal(whtComputed);
                     if (this.CurrentBusObj.ObjectData.Tables["whtlimit"].Rows.Count > 0)
                         whtlimit = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["whtlimit"].Rows[0]["value"]);
                     else
                         whtlimit = 0;
                     //overrideamount = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["override_amount"]);
                     if (overrideamount == 0 || overrideamount < 0)
                     {
                         Messages.ShowWarning("Override amount has to be greater than 0");
                         return;
                     }
                     //totcashamount = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["amount"]);
                     //prevapplied = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["cash_applied_to_wht"]);
                     if ((overrideamount + prevapplied) > (totcashamount + whtlimit))
                     {
                         Messages.ShowWarning("Cannot override the cash amount by more than $" + whtlimit.ToString());
                         return;
                     }
                     //prevapplied = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["cash_applied_to_wht"]);
                     //totcashamount = Convert.ToDecimal(this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["amount"]);
                     //totapplied = (adjustamt + prevapplied);
                     totapplied = (overrideamount + prevapplied);
                     //totavail = (totcashamount + whtlimit);
                     totavail = (totcashdoc + whtlimit);
                     if (totapplied > totavail)
                     {
                         Messages.ShowWarning ("Insufficient Remaining Cash Available");
                         return;
                     }
                     AdjTotal = 0.00M;
                     //loop through each grid record and add adj totals
                     foreach (DataRecord r in GridWHTINVDetail.xGrid.Records)
                     {
                         AdjTotal += Convert.ToDecimal(r.Cells["amount_to_adjust"].Value);
                     }
                     //if (Math.Abs(adjustamt) != Math.Abs(AdjTotal))
                     if (adjustamt != (AdjTotal * -1))
                     {
                         Messages.ShowWarning("Total of detail adjusted amounts does not equal WHT Adjust Amount");
                         return;
                     }
                     //update remit table with values from the textbox
                     //RES 12/8/15 use the wht rate from variable whtRate since that is what was used in the calculation
                     //this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["wht_rate"] = Convert.ToDecimal(txtWHTRate.Text);
                     this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["wht_rate"] = whtRate;
                     string sIncAmt = UnformatTextField(txtIncomeWHT.Text.Trim());
                     this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["income_subject_to_wht"] = Convert.ToDecimal(sIncAmt);
                     this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["apply_to_doc"] = txtDocument.Text.ToString();
                     //this.CurrentBusObj.ObjectData.Tables["detail"].Rows[0]["wht_rate"] = Convert.ToDecimal(txtWHTRate.Text);
                     //this.CurrentBusObj.ObjectData.Tables["detail"].Rows[0]["income_subject_to_wht"] = Convert.ToDecimal(sIncAmt);
                     //this.CurrentBusObj.ObjectData.Tables["detail"].Rows[0]["apply_to_doc"] = txtDocument.Text.ToString();
                            
 
                    string NewDocID = "";
                    string SPErrMsg = "";
                    sAdjAmt = UnformatTextField(txtAdjAmt.Text.Trim());
                    NewDocID = cGlobals.BillService.InsertNewAdjustmentPreamble(txtDocument.Text, 9, cGlobals.UserName.ToLower(), whtComputed * -1);

                    //insert the new doc id in all existing grid rows for ins sp
                    if (NewDocID != "")
                    {
                    //set adjustment number on remit table to NewDocID
                        this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["adj_document_id"] = NewDocID;
                        //this.CurrentBusObj.ObjectData.Tables["detail"].Rows[0]["adj_document_id"] = NewDocID;
                    }

                    else
                    {
                        Messages.ShowError("Problem Creating New Adjustment");
                        return;
                    }

                    this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["country_id"] = cmbCountry.SelectedValue;
                    this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["province_id"] = cmbProvince.SelectedValue;
                    string sAmtAdjust = UnformatTextField(txtWHTAmount.Text.Trim());
                    this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["amount_to_adjust"] = sAmtAdjust;
                    this.CurrentBusObj.Parms.ClearParms();
                    this.CurrentBusObj.Parms.AddParm("@document_id", txtDocument.Text);
                    this.CurrentBusObj.Parms.AddParm("@country_id", this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["country_id"]);
                    this.CurrentBusObj.changeParm("@country_id", cmbCountry.SelectedValue.ToString());
                    //this.CurrentBusObj.Parms.AddParm("@country_id", cmbCountry.SelectedValue);
                    this.CurrentBusObj.Parms.AddParm("@province_id", this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["province_id"]);
                    this.CurrentBusObj.changeParm("@province_id", this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["province_id"].ToString());
                    //this.CurrentBusObj.Parms.AddParm("@amount", this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["amount"]);
                    //this.CurrentBusObj.changeParm("@amount", this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["amount"].ToString());
                    this.CurrentBusObj.Parms.AddParm("@amount", totcashdoc);
                    this.CurrentBusObj.changeParm("@amount", totcashdoc.ToString());
                    //this.CurrentBusObj.Parms.AddParm("@receivable_account", this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["receivable_account"]);
                    //this.CurrentBusObj.Parms.UpdateParmValue("@receivable_account", this.CurrentBusObj.ObjectData.Tables["detail"].Rows[0]["receivable_account"].ToString());
                    this.CurrentBusObj.changeParm("@receivable_account", this.CurrentBusObj.ObjectData.Tables["detail"].Rows[0]["receivable_account"].ToString());
                    this.CurrentBusObj.Parms.AddParm("@searchind", 0);
                    this.CurrentBusObj.Parms.AddParm("@cash_document_id", this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["cash_document"]);
                    this.CurrentBusObj.Parms.AddParm("@seq_code", this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["seq_code"]);

                    //loop through detail data table and update columns
                    foreach (DataRow dr in this.CurrentBusObj.ObjectData.Tables["detail"].Rows)
                    {
                         bool rowIsNotModified = (dr.RowState != DataRowState.Modified);
                         if (Convert.ToDecimal(dr["amount_to_adjust"]) != 0)
                         {
                            dr["adj_document_id"] = NewDocID;
                            dr["country_id"] = cmbCountry.SelectedValue;
                            dr["province_id"] = cmbProvince.SelectedValue;
                            //RES 12/8/15 use the wht rate from variable whtRate since that is what was used in the calculation
                            //dr["wht_rate"] = Convert.ToDecimal(txtWHTRate.Text);
                            dr["wht_rate"] = whtRate;
                            dr["income_subject_to_wht"] = Convert.ToDecimal(sIncAmt);
                            dr["cash_document"] = this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["cash_document"];
                            dr["item_id"] = Convert.ToInt32(cmbProductItem.SelectedValue);
                            dr["seq_code"] = this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["seq_code"];
                            //dr["amount"] = this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["amount"];
                            dr["amount"] = totcashdoc;
                            //dr["override_amount"] = this.CurrentBusObj.ObjectData.Tables["remit"].Rows[0]["override_amount"];
                            dr["override_amount"] = overrideamount;
                         }
                         if (rowIsNotModified)
                         {
                             dr.AcceptChanges();
                         }
                         else
                         {
                             dr.AcceptChanges();
                             dr.SetAdded();
                         }
                    }

                    base.Save();
                    if (SaveSuccessful)
                    {
                    //error messages from Stored Proc
                        string strSPErrMg = getInfoFromStoredProc();
                        if (strSPErrMg != null && strSPErrMg != "")
                        {
                            Messages.ShowError(strSPErrMg);
                            //do rollback
                            rollbackAdj(NewDocID);
                         
                        }
                        else
                        {
                            Messages.ShowInformation("Save Successful--New Adjustment ID = " + NewDocID);
                            //pop new adjustment in folder
                            AdjFolderScreen.ReturnData(NewDocID, "");
                            //add user name to AdjGeneralTab
                            AdjFolderScreen.AdjustmentGeneralTab.txtCreatedBy.Text = cGlobals.UserName.ToLower();
                            //close me/////////////////////////////////////////////////////////////////////////////////
                            System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);
                            this.CurrentBusObj.ObjectData.AcceptChanges();
                            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;
                            this.SaveSuccessful = true;
                            if (!ScreenBaseIsClosing)
                            {
                                AdjParent.Close();
                            }
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        }
                }
                else
                {
                    Messages.ShowInformation("Data Error, Save Failed");
                    //do rollback
                    rollbackAdj(NewDocID);
                  
                }
            }
       
        }

      
       private string getInfoFromStoredProc()
        {
            var SPErrorMsg = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                             where x.Field<string>("parmName") == "@error_message"
                             select new
                             {
                                 parmName = x.Field<string>("parmName"),
                                 parmValue = x.Field<string>("parmValue")
                             };
            foreach (var info in SPErrorMsg)
            {
                if (info.parmName == "@error_message")
                    return info.parmValue;
            }
            return "";
        }



      
           private void rollbackAdj(string NewDocID)
        {
            //delete adj header
            bool retVal = cGlobals.BillService.DeleteNewAdjusmentPreamble(NewDocID);
        }



       
               private void loadParms(string DocumentID)
        {
        //    //clear parm     

          try
            {

            this.CurrentBusObj.Parms.ClearParms();
            this.CurrentBusObj.Parms.AddParm("@document_id", txtDocument.Text);
            this.CurrentBusObj.Parms.AddParm("@country_id", 0);
            this.CurrentBusObj.Parms.AddParm("@province_id", 0);
            this.CurrentBusObj.Parms.AddParm("@amount", 0);
            this.CurrentBusObj.Parms.AddParm("@receivable_account", "");
            this.CurrentBusObj.Parms.AddParm("@searchind", 0);
            this.CurrentBusObj.Parms.AddParm("@cash_document_id", "");
            this.CurrentBusObj.Parms.AddParm("@seq_code", 0);
            this.CurrentBusObj.Parms.AddParm("@apply_to_doc", txtDocument.Text);
            DocumentId = txtDocument.Text;

            }
          catch (Exception ex)
          {
              Messages.ShowMessage(ex.Message, System.Windows.MessageBoxImage.Information);
          }

        }

    
        private void setInitScreenState()
        {
             
            btnSave.IsEnabled = false;
            btnSearch.IsEnabled = false;
            btnClear.IsEnabled = false;
            txtAdjAmt.Text = "";
            txtIncomeWHT.Text = "";
            txtWHTAmount.Text = "";
            txtWHTRate.Text = "0.00";
            //if (cGlobals.SetSecurity.ToString() == 
            //if (Convert.ToInt32(cGlobals.SecurityDT.Rows[0]["role_id"]) == 14)
            //    cmbCountry.IsEnabled = true;
           
         
        }

        ///// <summary>
        ///// frees up objects for edit
        ///// </summary>
        private void setEditScreenState()
        {
        //    //GridCreditDebitSource.IsEnabled = true;
        //    GridCreditDebitDestination.IsEnabled = true;
        //    //btnClearDest.IsEnabled = true;
        //    BorderDestination.IsEnabled = true;
        //    //Set the amount_adjusted field as active
        //    if (this.CurrentBusObj.ObjectData != null)
        //        if (this.CurrentBusObj.ObjectData.Tables["detail"].Rows.Count > 0)
        //            (GridCreditDebitDestination.xGrid.Records[GridCreditDebitDestination.ActiveRecord.Index] as DataRecord).Cells["amount_adjusted"].IsActive = true;
        }

        ///// <summary>
        ///// check that amounts are offsetting
        ///// </summary>
        ///// <returns></returns>
       

   
        public void PreBind()
        {
         
                if (this.CurrentBusObj.HasObjectData)
                {
                   this.cmbProvince.SetBindingExpression("province_id", "province", this.CurrentBusObj.ObjectData.Tables["province"]);
                   this.cmbCountry.SetBindingExpression("country_id", "country", this.CurrentBusObj.ObjectData.Tables["country"]);
                   this.cmbProductItem.SetBindingExpression("item_id", "item_description", this.CurrentBusObj.ObjectData.Tables["dddwproduct"]);
                }
           
   
        }

        private void txtDocument_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            string documentID = "";
            if (txtDocument.Text != "")
            {
                DocumentId = txtDocument.Text;
                //Need to clear both grids and all screens
                if (this.CurrentBusObj.HasObjectData)
                {
                    txtAdjAmt.Text = "";
                    txtIncomeWHT.Text = "";
                    txtWHTAmount.Text = "";
                    txtWHTRate.Text = "0.00";
           
                    SearchClear();
                    RemitClear();
                }


            }

            cGlobals.ReturnParms.Clear();
            //Event handles opening of the lookup window upon double click on BCF Number field
            WHTAdjInvoiceLookup f = new WHTAdjInvoiceLookup();
            f.Init(new cBaseBusObject("ADJWHTInvoiceLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //DWR-Added 1/15/13 to replace previous retreival code
                txtDocument.Text = cGlobals.ReturnParms[0].ToString();
                cGlobals.ReturnParms.Clear();
                if (txtDocument.Text != DocumentId)
                {
                    ReturnData(txtDocument.Text, "@document_id");
                }
            }
        }


        public void ReturnData(string SearchValue, string DbParm)
        {
            
            
            //if no value do nothing
            if (SearchValue == "") return;
            //Add new parameters
            loadParms(SearchValue);
            //load data
            //if coming from save do not do this...
            this.Load();
            //if BCFNumber found then set header and pop otherwise send message
            //if customer number found then set header and pop otherwise send message
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)
            {

                txtAdjAmt.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["amount_to_adjust"].ToString();             
                DocumentId = txtDocument.Text;
                txtCustomer.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["receivable_account"].ToString().Trim();
                txtCustNumSearch.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["receivable_account"].ToString().Trim();
                cmbProvince.SelectedValue = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["province_id"];
                cmbCountry.SelectedValue = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["country_id"];
                int countryparm = 0;
                int provinceparm = 0;
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
                {
                    countryparm = Convert.ToInt16(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["country_id"]);
                    provinceparm = Convert.ToInt16(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["province_id"]);
                }
                this.CurrentBusObj.Parms.UpdateParmValue("@country_id", countryparm);
                this.CurrentBusObj.Parms.UpdateParmValue("@province_id", provinceparm);
                this.Load("dddwproduct");
           
               

                cmbProductItem.SelectedValue = this.CurrentBusObj.ObjectData.Tables["dddwproduct"].Rows[0]["item_id"];
                cmbProductItem.SelectedText = this.CurrentBusObj.ObjectData.Tables["dddwproduct"].Rows[0]["item_description"].ToString();
                         
                             
                            
               
            }
          
        }
        private void txtAdjAmt_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is RazerBase.ucLabelTextBox)
                (sender as RazerBase.ucLabelTextBox).SelectAll();

            if (string.IsNullOrEmpty(txtAdjAmt.Text)) { return; }

            txtAdjAmt.Text = txtAdjAmt.Text.Replace("$", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(",", "");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace("(", "-");
            txtAdjAmt.Text = txtAdjAmt.Text.Replace(")", "");
        }

        private string UnformatTextField(string FormattedTextField)
        {
            if (FormattedTextField == null || FormattedTextField == "") return "0";

            string sUnformattedTextField = FormattedTextField.Replace("$", "");
            sUnformattedTextField = sUnformattedTextField.Replace(",", "");
            sUnformattedTextField = sUnformattedTextField.Replace("(", "-");
            sUnformattedTextField = sUnformattedTextField.Replace(")", "");

            return sUnformattedTextField;
        }

      


 
     
 
        private void txtCustNumSearch_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Event handles opening of the lookup window upon double click on contract ID field
            RazerBase.Lookups.CustomerLookup custLookup = new RazerBase.Lookups.CustomerLookup();
            custLookup.Init(new cBaseBusObject("CustomerLookup"));

            //this.CurrentBusObj.Parms.ClearParms();

            // gets the users response
            custLookup.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                //load current parms
                //loadParms("");
                txtCustNumSearch.Text = cGlobals.ReturnParms[0].ToString();
                //txtCustNameSearch.Text = cGlobals.ReturnParms[1].ToString();

                this.CurrentBusObj.Parms.UpdateParmValue("@receivable_account", txtCustNumSearch.Text);
                this.CurrentBusObj.Parms.UpdateParmValue("@searchind", 1);
                if (txtAmtSearch.Text == null || txtAmtSearch.Text == "")
                {
                    txtAmtSearch.Text = "";
                    this.CurrentBusObj.Parms.UpdateParmValue("@amount", "0");
                }
                else
                {
                    this.CurrentBusObj.Parms.UpdateParmValue("@amount", txtAmtSearch.Text);
                }
            }
        }

        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
          
            if (txtCustNumSearch.Text == null) txtCustNumSearch.Text = "";
         
 
            if (this.CurrentBusObj.HasObjectData)
            {
                if (this.CurrentBusObj.ObjectData.Tables["main"] == null || this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count == 0)
                    return;
               
                //cBaseBusObject AdjWHTVerification = new cBaseBusObject("AdjWHTVerification");

                //AdjWHTVerification.Parms.ClearParms();
                this.CurrentBusObj.Parms.UpdateParmValue ("@receivable_account", txtCustNumSearch.Text);
                this.CurrentBusObj.Parms.UpdateParmValue("@searchind", 1);
                if (txtAmtSearch.Text == null || txtAmtSearch.Text == "")
                {
                    txtAmtSearch.Text = "";
                    this.CurrentBusObj.Parms.UpdateParmValue("@amount", "0");
                }
                else
                {
                    this.CurrentBusObj.Parms.UpdateParmValue("@amount", txtAmtSearch.Text);
                }

                this.CurrentBusObj.LoadData("search");
               
                //if (AdjWHTVerification.ObjectData.Tables["search"] == null || AdjWHTVerification.ObjectData.Tables["search"].Rows.Count < 1)
                if (this.CurrentBusObj.ObjectData.Tables["search"] == null || this.CurrentBusObj.ObjectData.Tables["search"].Rows.Count < 1)
                {
                    Messages.ShowInformation("No records found");
                }
                else
                    GridWHTSearch.LoadGrid(this.CurrentBusObj, GridWHTSearch.MainTableName);
            }

           
             
            
        }

      
    
        private void btnClear_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SearchClear();
        }

        //private void cmbProductItem_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    return;
        //}
        /// <summary>
        /// This event handles the double click launching of a lookup window.  If a value is returned from the lookup window
        /// then the data for the window is requiered.
        /// </summary>

        private void cmbProductItem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.CurrentBusObj.HasObjectData)
            {
                if (this.CurrentBusObj.ObjectData.Tables["main"] == null || this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count == 0)
                    return;
                //Need to clear grid so WHT amounts can be recalculated
                //if (this.CurrentBusObj.HasObjectData && Convert.ToDecimal(txtWHTRate.Text) != whtRate)
                //{
                //    this.CurrentBusObj.changeParm("@cash_document_id", "");
                //    this.CurrentBusObj.changeParm("@seq_code", "0");
                //    this.CurrentBusObj.LoadData("detail");
                //    this.CurrentBusObj.LoadData("remit");
                //    //clear amounts and disable Save button until new WHT is caclulated
                //    txtWHTAmount.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", 0);
                //    txtAdjAmt.Text = txtWHTAmount.Text;
                //    txtIncomeWHT.Text = txtWHTAmount.Text;
                //    btnSave.IsEnabled = false;
                //}
                //Need to populate wht rate based on product item selected
                //Need to load Verification Bus object to populate product item
                cBaseBusObject AdjWHTVerification = new cBaseBusObject("AdjWHTVerification");

                AdjWHTVerification.Parms.ClearParms();
                AdjWHTVerification.Parms.AddParm("@country_id", this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["country_id"]);
                AdjWHTVerification.Parms.AddParm("@province_id", this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["province_id"]);
                AdjWHTVerification.Parms.AddParm("@item_id", cmbProductItem.SelectedValue);
                AdjWHTVerification.LoadTable("whtrate");
                if (AdjWHTVerification.ObjectData.Tables["whtrate"] == null || AdjWHTVerification.ObjectData.Tables["whtrate"].Rows.Count < 1)
                {
                    txtWHTRate.Text = "0.00";
                }
                else
                    txtWHTRate.Text = AdjWHTVerification.ObjectData.Tables["whtrate"].Rows[0]["wht_rate"].ToString();

                //Need to clear grid so WHT amounts can be recalculated
                if (this.CurrentBusObj.HasObjectData && Convert.ToDecimal(txtWHTRate.Text) != whtRate)
                {
                    this.CurrentBusObj.changeParm("@cash_document_id", "");
                    this.CurrentBusObj.changeParm("@seq_code", "0");
                    this.CurrentBusObj.LoadData("detail");
                    this.CurrentBusObj.LoadData("remit");
                    //clear amounts and disable Save button until new WHT is caclulated
                    txtWHTAmount.Text = String.Format("{0:$###,##0.00;($###,##0.00)}", 0);
                    txtAdjAmt.Text = txtWHTAmount.Text;
                    txtIncomeWHT.Text = txtWHTAmount.Text;
                    btnSave.IsEnabled = false;
                }

                btnSearch.IsEnabled = true;
                btnClear.IsEnabled = true;
           }

                
        }

        private void txtCustNumSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            this.CurrentBusObj.Parms.UpdateParmValue("@receivable_account", txtCustNumSearch.Text);
            this.CurrentBusObj.Parms.UpdateParmValue("@searchind", 1);
            if (txtAmtSearch.Text == null || txtAmtSearch.Text == "")
            {
                txtAmtSearch.Text = "";
                this.CurrentBusObj.Parms.UpdateParmValue("@amount", "0");
            }
            else
            {
                this.CurrentBusObj.Parms.UpdateParmValue("@amount", txtAmtSearch.Text);
            }
        }   
        }

      
 
}

