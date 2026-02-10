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

namespace Admin
{

    /// Interaction logic for AdjustmentQueue

    public partial class SetDates : ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "setDates";
        private static readonly string mainTableName = "setDates";
        public string jobname = "Bill Period Close";
        public DateTime dtToRun;




        public string WindowCaption { get { return string.Empty; } }


        public SetDates()
            : base()
        {
            InitializeComponent();
        }


        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            //Set up generic context menu selections
           
                gSetDates.ContextMenuGenericDelegate1 = ContextMenuIncrementPeriod;
                gSetDates.ContextMenuGenericDisplayName1 = "Increment Period";
                if (base.SecurityContext == AccessLevel.ViewOnly || base.SecurityContext == AccessLevel.NoAccess)
                   gSetDates.ContextMenuGenericIsVisible1 = false;
                else
                   gSetDates.ContextMenuGenericIsVisible1 = true;

                //add delegates to be enabled
                gSetDates.ContextMenuAddDelegate = ContextMenuIncrementPeriod;
                //disable add Record
                gSetDates.ContextMenuAddIsVisible = false;
          

            //gAdjustmentQueue.xGrid.FieldLayoutSettings = layouts;
            gSetDates.FieldLayoutResourceString = fieldLayoutResource;
            gSetDates.MainTableName = mainTableName;

            this.MainTableName = mainTableName;

            this.Load(businessObject);
            GridCollection.Add(gSetDates);

            this.CurrentBusObj = businessObject;

            gSetDates.SetGridSelectionBehavior(true, false);
            gSetDates.LoadGrid(businessObject, gSetDates.MainTableName);


            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);

        }

        private void ContextMenuIncrementPeriod()
        {
            //for row selected, increment date one they say to

            string datetype = "";
            DateTime dtConvert = new DateTime();
            DateTime chgDate = new DateTime();
            //Have to reload in case the screen was kept open and they go schedule GL Post job
            CurrentBusObj.LoadTable("chkGLholding");

            gSetDates.ReturnSelectedData("date_type");
            if (cGlobals.ReturnParms.Count > 0)
                datetype = cGlobals.ReturnParms[0].ToString();
            gSetDates.ReturnSelectedData("date_value");

            dtConvert = Convert.ToDateTime(cGlobals.ReturnParms[0].ToString());
            chgDate = dtConvert.AddMonths(1);
            //sDate = dtConvert.Month.ToString() + "-" + dtConvert.Day.ToString() + "-" + dtConvert.Year.ToString();
            dtToRun = DateTime.Today;
                        
            if (dtToRun.Month.ToString() != chgDate.Month.ToString())
            {
                Messages.ShowError("Update not allowed because you are trying to move the period month to " + chgDate.ToString("MMMM") + " and the current calendar month is " + dtToRun.ToString("MMMM"));
                return;
            }
            //RES 10/4/19 Move both periods at once instead of making the user do it one at a time
            //MessageBoxResult result = Messages.ShowYesNo("WARNING!!! This will close the Accounting and Billing periods and increment them to " + chgDate.ToString() + ".  *** The Bill Period Close job AND the Def Pool Unapplied Cash Job need to run for this period before incrementing the period ***  Do you wish to proceed?",
            MessageBoxResult result = Messages.ShowYesNo("WARNING!!! This will close the " + datetype.ToString() + " and increment the period to " + chgDate.ToString() + ".  *** The Bill Period Close job AND the Def Pool Unapplied Cash Job need to run for this period before incrementing the period ***  Do you wish to proceed?",
                           System.Windows.MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MessageBoxResult result2 = Messages.ShowYesNo("WARNING!!! Did the Bill Period Close and the Def Pool Unapplied Cash job run for this period?",
                          System.Windows.MessageBoxImage.Question);

                if (result2 == MessageBoxResult.Yes)
                {

                    if (Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["chkGLholding"].Rows[0][0].ToString()) > 0)
                    {
                        Messages.ShowError("The periods cannot be moved forward until the GL Posting job has successfully run.");
                        return;
                    }
                    else
                    {
                        //Schedule Bill Period Close job
                        //cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobname, "", dtToRun, cGlobals.UserName);

                        //RES 10/4/19 Move both periods at once instead of making the user do it one at a time
                        if (datetype.ToString() == "ACCTPERIOD")
                        {
                            this.CurrentBusObj.ObjectData.Tables["setDates"].Rows[0]["date_value"] = chgDate.ToString();
                            this.CurrentBusObj.ObjectData.Tables["setDates"].Rows[0]["last_update_date"] = DateTime.Today.ToString();
                        }
                        else
                        {
                            this.CurrentBusObj.ObjectData.Tables["setDates"].Rows[1]["date_value"] = chgDate.ToString();
                            this.CurrentBusObj.ObjectData.Tables["setDates"].Rows[1]["last_update_date"] = DateTime.Today.ToString();
                        }
                        //Save changes
                        this.Save();
                    }
                }
                else
                {
                    Messages.ShowMessage("Period not incremented.", MessageBoxImage.Information);
                }
                                 
            }

            else
            {

                Messages.ShowMessage("Period not incremented.", MessageBoxImage.Information);
            }







        }

        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
                this.Load(this.CurrentBusObj);
                gSetDates.LoadGrid(this.CurrentBusObj, gSetDates.MainTableName);
                Messages.ShowInformation("Save Successful");
               
               
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }
    }
}
       
       

       

       



