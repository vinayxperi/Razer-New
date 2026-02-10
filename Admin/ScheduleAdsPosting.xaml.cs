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

namespace Admin
{

    /// Interaction logic for ScheduleBillPosting.xaml

    public partial class ScheduleAdsPosting : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "AdsProcessToSchedule";
        private static readonly string mainTableName = "ScheduleAdsPosting";
        public static readonly string postingJob = "Ads Posting";
        public static readonly string loadJob = "Ads Data Load";
        public string jobName = "";
        public int AdsPost = 0;
        public int AdsLoad = 0;
        public int AdsRollback = 0;
        public string sinvDate = "";
        public DateTime dt = new DateTime();
        public DateTime dtInv = new DateTime();
        



        public string WindowCaption { get { return string.Empty; } }


        public ScheduleAdsPosting()
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

            this.CanExecuteSaveCommand = false;
            string userValueField = "user_id";
            string userDisplayField = "user_name";
            gAdsProcessToRun.WindowZoomDelegate = ReturnSelectedData;
            gAdsProcessToRun.xGrid.FieldLayoutSettings = layouts;
            gAdsProcessToRun.FieldLayoutResourceString = fieldLayoutResource;
            gAdsProcessToRun.MainTableName = "scheduleAdsPosting";
            gAdsProcessToRun.ContextMenuSaveToExcelIsVisible = false;
           

            this.MainTableName = mainTableName;

            this.Load(businessObject);
            this.CurrentBusObj = businessObject;
            if (this.CurrentBusObj.HasObjectData)
            {

                //Security User listbox

                //this.lkUser.BindingObject = EstablishListObjectBinding(this.CurrentBusObj.GetTable("email_user") as DataTable, true, "user_name",
                //    "user_id", "Select Business User");
                //this.lkUser.SelectedValue = cGlobals.UserName;
                cmbUser.SetBindingExpression(userValueField, userDisplayField, this.CurrentBusObj.ObjectData.Tables["emailUser"]);
                cmbUser.SelectedValue = cGlobals.UserName;

                //cmbUser.SelectedText = 

                //this.lkUser.Text = cGlobals.UserName;

            }

            gAdsProcessToRun.SetGridSelectionBehavior(false, true);
            gAdsProcessToRun.LoadGrid(businessObject, gAdsProcessToRun.MainTableName);


            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);

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

        // Schedule the job
        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {





            string sParms = "";

            AdsPost = ((Int32)this.CurrentBusObj.ObjectData.Tables["scheduleAdsPosting"].Rows[2]["ProcessSelected"]);
            AdsLoad = ((Int32)this.CurrentBusObj.ObjectData.Tables["scheduleAdsPosting"].Rows[0]["ProcessSelected"]);
            AdsRollback = ((Int32)this.CurrentBusObj.ObjectData.Tables["scheduleAdsPosting"].Rows[1]["ProcessSelected"]);

            int sumTot = 0;

            sumTot = AdsPost + AdsLoad + AdsRollback;
            if (sumTot == 0)
                Messages.ShowWarning("Must select Ads Process to schedule!");
            else
                if (sumTot > 1)

                    Messages.ShowWarning("Only one Ads Process can be selected to schedule!");

                else
                {
                    string dtToPrint = "";
                    dtToPrint = ldInvoiceDate.SelText.ToString();
                    int blankpos = dtToPrint.IndexOf(" ");
                    sinvDate = ldInvoiceDate.SelText.ToString();

                    if (sinvDate == "" || sinvDate == "1/1/1900 12:00:00 AM")
                    {
                        if (AdsPost == 1)
                        {
                            Messages.ShowError("Date to Print on Invoice is Required for Posting!");
                            ldInvoiceDate.Focus();
                            return;
                        }
                    }
                    if (AdsPost == 1)
                    {
                        //RES 3/31/16 Verify date is last day of current month
                        //var result = MessageBoxResult.OK;
                        //DateTime today = DateTime.Today;
                        //DateTime endOfMonth = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
                        DateTime AcctPeriod;
                        AcctPeriod = Convert.ToDateTime(this.CurrentBusObj.ObjectData.Tables["adsvalidate"].Rows[0]["acct_period"]);
                        DateTime endOfMonth = new DateTime(AcctPeriod.Year, AcctPeriod.Month, 1).AddMonths(1).AddDays(-1);
                        if (sinvDate != endOfMonth.ToString())
                        {
                            //result = MessageBox.Show("Invoice date is not the last day of the current account period.  Do you want to proceed?", "Invoice Date", MessageBoxButton.YesNo);
                            //if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                            //{
                            //}
                            //else return;
                            Messages.ShowError("Invoice date is not the last day of the current account period!");
                            ldInvoiceDate.Focus();
                            return;
                        }



                        jobName = "Ads Posting";

                        sParms = "/A " + dtToPrint.Substring(0, blankpos);
                        string no_post_flag = "";
                        string no_alloc_flag = "";
                        no_post_flag = this.CurrentBusObj.ObjectData.Tables["adsvalidate"].Rows[0]["no_post_flag"].ToString();
                        no_alloc_flag = this.CurrentBusObj.ObjectData.Tables["adsvalidate"].Rows[0]["no_alloc_flag"].ToString();
                        if (no_post_flag == "1")
                        {
                            Messages.ShowWarning("No invoices exist to post.");
                            return;
                        }
                        if (no_alloc_flag == "1")
                        {
                            Messages.ShowWarning("Revenue Allocation Percentages Must be set up for this Broadcast Period.");
                            return;
                        }


                    }


                    if (AdsLoad == 1)
                    {
                        jobName = "Ads Data Load";
                        sParms = "/A LOAD";
                    }
                    if (AdsRollback == 1)
                    {
                        jobName = "Ads Data Load";
                        sParms = "/A ROLLBACK";
                    }

                    if (jobName == "Ads Posting")
                    {
                        sinvDate = ldInvoiceDate.SelText.ToString();
                        if (sinvDate == "")
                        {
                            Messages.ShowWarning("Invoice Date to Print on Invoice is Required!");
                            ldInvoiceDate.Focus();
                        }
                        else
                        {
                            dtInv = Convert.ToDateTime(ldInvoiceDate.SelText);


                            dt = dtInv;

                            int monthpart = dt.Month;
                            int dayspart = dt.Day;


                            if ((monthpart == 4) || (monthpart == 6) || (monthpart == 9) || (monthpart == 11))
                            {
                                if (dayspart != 30)
                                {
                                    var result = MessageBox.Show("Date entered is not the last day of the month. Do you want to continue?", "Schedule Ads Posting", MessageBoxButton.YesNo);

                                    if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                                    {
                                    }
                                    else
                                    {
                                        ldtDatetoSchedule.Focus();
                                        return;
                                    }
                                }
                            }
                            else
                                if (monthpart == 2)
                                {
                                    if ((dayspart != 28) && (dayspart != 29))
                                    {
                                        var result = MessageBox.Show("Date entered is not the last day of the month. Do you want to continue?", "Schedule Ads Posting", MessageBoxButton.YesNo);

                                        if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                                        {
                                        }
                                        else
                                        {
                                            ldtDatetoSchedule.Focus();
                                            return;
                                        }
                                    }
                                }
                                else
                                    if (monthpart != 1)
                                    {
                                        if ((dayspart != 31))
                                        {
                                            var result = MessageBox.Show("Date entered is not the last day of the month. Do you want to continue?", "Schedule Ads Posting", MessageBoxButton.YesNo);

                                            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                                            {
                                            }
                                            else
                                            {
                                                ldtDatetoSchedule.Focus();
                                                return;
                                            }
                                        }


                                    }

                        }
                    }

                        MessageBoxResult result2 = Messages.ShowYesNo("Schedule Job " + jobName.ToString() + " for " + ldtDatetoSchedule.SelText,
                        System.Windows.MessageBoxImage.Question);
                        if (result2 == MessageBoxResult.Yes)
                        {





                            if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, sParms, ldtDatetoSchedule.SelText, this.cmbUser.SelectedValue.ToString()) == true)
                            {
                                Messages.ShowWarning("Job Scheduled to Run");
                                this.CallScreenClose();
                            }
                            else
                                Messages.ShowWarning("Error Scheduling Job");
                        }

                        else
                        {

                            Messages.ShowMessage("Job Not Scheduled", MessageBoxImage.Information);
                        }

                    }
                }
      
                    
               

               
        
 






        public override void Close()
        {



        }





        //private void ldInvoiceDate_Loaded(object sender, RoutedEventArgs e)
        //{

        //}





    }
}
 
