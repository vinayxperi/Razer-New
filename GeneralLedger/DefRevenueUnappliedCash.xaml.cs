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

    /// Interaction logic for DefRevenueUnappliedCash.xaml

    public partial class DefRevenueUnappliedCash : ScreenBase, IScreen
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

        cBaseBusObject DefPoolRevUnappliedCashPop = new cBaseBusObject("DefPoolRevUnappliedCashPop");




        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
        }


        public DefRevenueUnappliedCash()
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


           
            
            GridDefPoolMgmt.WindowZoomDelegate = ReturnSelectedData;
            //GridDefPoolMgmt.xGrid.FieldLayoutSettings = layouts;
            GridDefPoolMgmt.xGrid.FieldLayoutSettings.SelectionTypeField = Infragistics.Windows.Controls.SelectionType.Single;
            GridDefPoolMgmt.FieldLayoutResourceString = "DefPoolUnappliedCash";
            GridDefPoolMgmt.SetGridSelectionBehavior(false, false);
            //new
            GridDefPoolMgmt.xGrid.FieldSettings.AllowEdit = true;
            //GridDefPoolMgmt.IsEnabled = false;
            GridDefPoolMgmt.SkipReadOnlyCellsOnTab = true;
            GridDefPoolMgmt.CellUpdatedDelegate = GridDefPoolMgmt_CellUpdated;
            //new end
            this.MainTableName = "main";
            GridDefPoolMgmt.MainTableName = "main";
            //this.DoNotSetDataContext = false;
            this.btnFinal.IsEnabled = false;
          
            GridCollection.Add(GridDefPoolMgmt);
          

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
            //this.CanExecuteSaveCommand = false;
            this.CancelCloseOnSaveConfirmation = false;
            this.CanExecuteDeleteCommand = false;
            this.CanExecuteNewCommand = false;
            this.CanExecuteRefreshCommand = false;
            GridDefPoolMgmt.xGrid.EditModeEnded += new EventHandler<Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs>(xGrid_EditModeEnded);


        }



        void xGrid_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {

            GridDefPoolMgmt.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);

            

        }

        void xGrid_RecordActivated(object sender, Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs e)
        {
            
            GridDefPoolMgmt.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
        }

            
           

        public override void Save()
        {

            Messages.ShowWarning("Save");
            return;


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


        private void ReturnData(string SearchValue, string DbParm)
        {
            ////if no value do nothing
            //if (SearchValue == "") return;
            ////Add new parameters
            //loadParms(SearchValue);
            ////load data
            ////if coming from save do not do this...
            //this.Load();
            ////if invoiceNumber found then set header and pop otherwise send message
            //if (chkForData())
            //{
            //    SetHeaderName();

            //}
        }

        

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DefPoolRevUnappliedCashPop.LoadTable("main");             
            
            this.CurrentBusObj.Parms.ClearParms();
            this.Load();

            GridDefPoolMgmt.LoadGrid(CurrentBusObj, GridDefPoolMgmt.MainTableName);
            
            if (CurrentBusObj.HasObjectData)
            {
                if (CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows.Count == 0)
                {
                    Messages.ShowInformation("No Deferred Unapplied Cash Records to Process for the Current Accounting Period.");
                }
                else
                {

                    this.btnFinal.IsEnabled = true;

                }
                //    txtRows.Text = CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows.Count.ToString() ?? string.Empty;

                //    decimal total = 0m;

                //    foreach (DataRow row in CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows)
                //    {
                //        total += (decimal)row[netAmtField];
                //    }

                //    txtTotal.Text = total.ToString("C2");
                //    txtTotal.TextBoxHorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                //    idgNationalAdsSearch.LoadGrid(CurrentBusObj, this.MainTableName);

            }
            else
            {
                 Messages.ShowInformation("No Deferred Unapplied Records to Process for the Current Accounting Period.");

            }


            //this.CanExecuteSaveCommand = false;
        }

           private void btnFinalize_Click(object sender, RoutedEventArgs e)
        {
            
                System.Windows.MessageBoxResult result = Messages.ShowYesNoCancel("WARNING! This will create and post the Month End GL Deferred Unapplied Cash related records. Are you sure you want to Finalize and Post these records for the this accounting period?",
                           System.Windows.MessageBoxImage.Question);
                //Save existing customer information and then load new customer
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    //CurrentBusObj.ObjectData.Tables[this.MainTableName].AcceptChanges();
                    btnFinal.IsEnabled = false;
                    btnSearch.IsEnabled = false;

                    int cnt = 0;


                    //Loop through and set the status_flag from 0 to 9. To indicate the records have been finalized.
                    //The records with a 9 will be used in the new DefPoolUnappliedCash posting job.
                        foreach (DataRow row in CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows)
                        {
                            CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["status_flag"] = 9;
                            cnt ++;

                        }
                        //CurrentBusObj.ObjectData.Tables[this.MainTableName].AcceptChanges();
                    
                    base.Save();

                    cGlobals.BillService.ScheduleJob(cGlobals.UserName, "Deferred Pool Unapplied Cash Report", "", DateTime.Now, cGlobals.UserName.ToString());

                    cGlobals.BillService.ScheduleJob(cGlobals.UserName, "DefPoolUnappliedCash", "", DateTime.Now, cGlobals.UserName.ToString());
                   



                    Messages.ShowInformation("The Report and Batch Job have been submitted.");
                }
                else
                {
                    Messages.ShowInformation("The Report and Batch Job have NOT been submitted.");
                    return;
                }


        }

           private void GridDefPoolMgmt_CellUpdated(object sender, Infragistics.Windows.DataPresenter.Events.CellUpdatedEventArgs e)
           {
               //if (e.Cell.Field.Name == "include_flag")
               //{
               DataRecord GridRecord = null;
               GridDefPoolMgmt.xGrid.ExecuteCommand(DataPresenterCommands.CommitChangesToActiveRecord);
               GridRecord = GridDefPoolMgmt.ActiveRecord;
               //    if (GridRecord != null)
               //    {
               if (GridRecord.Cells["include_flag"].Value.ToString() == "1")
               {
                   if (Convert.ToInt32(GridRecord.Cells["lines"].Value) > 1)
                   {
                       int cnt = 0;
                       //Loop through and set the include_flag value to be the same for documents with multiple rows that have the same
                       //recv_doc,product_code,reason_code,account_code
                       foreach (DataRow row in CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows)
                       {
                           if (CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["recv_doc"].ToString() ==
                               GridRecord.Cells["recv_doc"].Value.ToString() &&
                               CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["product_code"].ToString() ==
                               GridRecord.Cells["product_code"].Value.ToString() &&
                               CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["reason_code"].ToString() ==
                               GridRecord.Cells["reason_code"].Value.ToString() &&
                               CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["account_code"].ToString() ==
                               GridRecord.Cells["account_code"].Value.ToString())
                           {
                               CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["include_flag"] =
                                                                 Convert.ToInt32(GridRecord.Cells["include_flag"].Value);
                           }
                           cnt++;

                       }
                       GridDefPoolMgmt.LoadGrid(this.CurrentBusObj, "main");
                       Messages.ShowWarning("Checking this row caused " + GridRecord.Cells["lines"].Value.ToString() + " rows to be checked!");
                   }
               }
               else
               {
                   if (Convert.ToInt32(GridRecord.Cells["lines"].Value) > 1)
                   {
                       int cnt = 0;
                       //Loop through and set the include_flag value to be the same for documents with multiple rows that have the same
                       //recv_doc,product_code,reason_code,account_code
                       foreach (DataRow row in CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows)
                       {
                           if (CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["recv_doc"].ToString() ==
                               GridRecord.Cells["recv_doc"].Value.ToString() &&
                               CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["product_code"].ToString() ==
                               GridRecord.Cells["product_code"].Value.ToString() &&
                               CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["reason_code"].ToString() ==
                               GridRecord.Cells["reason_code"].Value.ToString() &&
                               CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["account_code"].ToString() ==
                               GridRecord.Cells["account_code"].Value.ToString())
                           {
                               CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows[cnt]["include_flag"] =
                                                                 Convert.ToInt32(GridRecord.Cells["include_flag"].Value);
                           }
                           cnt++;

                       }
                       GridDefPoolMgmt.LoadGrid(this.CurrentBusObj, "main");
                       Messages.ShowWarning("Unchecking this row caused " + GridRecord.Cells["lines"].Value.ToString() + " rows to be unchecked!");
                   }
                      
               }

           }

     





    }
}
