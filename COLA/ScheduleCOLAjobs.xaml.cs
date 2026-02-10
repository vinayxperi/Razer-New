using RazerBase;
using RazerInterface;
using System;
using System.Windows;
using System.Data;
using Infragistics.Windows.Editors;
using System.Windows.Input;
using Infragistics.Windows.DataPresenter;
using System.Collections.Generic;


namespace COLA
{

    /// Interaction logic for ScheduleBillPosting.xaml

    public partial class ScheduleCOLAjobs : ScreenBase 
    {
        public cBaseBusObject ScheduleCOLAjobsBusObject = new cBaseBusObject();
        
        cBaseBusObject Cola;
        private static readonly string mainTableName = "ScheduleCOLAjobs";
        public static readonly string candidateListJob = "Cola Check";
        public static readonly string applyCOLAJob = "Cola Processing";
        public string jobName = "";
        public int COLAList = 0;
        public int COLAApply = 0;
        public string scheduleJobToRun;
        
        public DateTime dt = new DateTime();
        
        



        public string WindowCaption { get { return string.Empty; } }


        public ScheduleCOLAjobs(   cBaseBusObject _Cola, string JobToSchedule) 
            : base()
        {
            this.CurrentBusObj = ScheduleCOLAjobsBusObject;
            //name obj
            this.CurrentBusObj.BusObjectName = "ScheduleCOLAjobs";
            Cola = _Cola;
            //get handle to contract obj
            Cola = _Cola;
            scheduleJobToRun = JobToSchedule;
           

            InitializeComponent();

            Init();
        }


        public void Init()
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //set isscreendirty = false to prevent save message
            //this.IsScreenDirty = false;

            this.CanExecuteSaveCommand = false;
            string userValueField = "user_id";
            string userDisplayField = "user_name";
            chkApply.IsChecked = 0;  
            chkCandidate.IsChecked = 0;  
            this.MainTableName = mainTableName;

            this.Load();
            if (scheduleJobToRun == "Apply")
            {
                chkApply.IsChecked = 1;
                chkCandidate.IsChecked = 0;
            }
            if (scheduleJobToRun == "List")
            {
                chkApply.IsChecked = 0;
                chkCandidate.IsChecked = 1;
            }

             
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

           System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);

        }




      

        // Schedule the job
        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {

           string sParms = "";

           if (COLAList == 1)  
               if (COLAApply == 1)
           {
                Messages.ShowWarning("Only one job can be selected to be scheduled!");
                return;
            }
          
           if (COLAList == 0)
               if (COLAApply == 0)
               {
                   Messages.ShowWarning("Please select a job to be scheduled!");
                   return;
               }
           
            if (txtDaysOut.Text == "")
            {
                Messages.ShowWarning("Days Out is Required!");
               txtDaysOut.Focus();
                return;
            }
                     
                    if (COLAList == 1)
                    {
                        jobName = candidateListJob;

                        sParms = "/A " + txtDaysOut.Text;
                    }


                    if (COLAApply == 1)
                    {
                        jobName = applyCOLAJob;
                        sParms = "/A " + txtDaysOut.Text;
                    }
            


                        MessageBoxResult result2 = Messages.ShowYesNo("Schedule Job " + jobName.ToString() + " for " + ldtDatetoSchedule.SelText,
                        System.Windows.MessageBoxImage.Question);
                        if (result2 == MessageBoxResult.Yes)
                        {

                           if (cGlobals.BillService.ScheduleJob(cGlobals.UserName, jobName, sParms, ldtDatetoSchedule.SelText, this.cmbUser.SelectedValue.ToString()) == true)
                            {
                                Messages.ShowWarning("Job Scheduled to Run");
                              
                                Cola.LoadTable("general");
                                
                                CloseScreen();
                            }
                            else
                                Messages.ShowWarning("Error Scheduling Job");
                        }

                        else
                        {

                            Messages.ShowMessage("Job Not Scheduled", MessageBoxImage.Information);
                        }

                    }
                
      
     
      
        private void chkCandidate_Checked(object sender, RoutedEventArgs e)
        {
            COLAList = 1;
            COLAApply = 0;
            chkApply.IsChecked = 0;
            chkCandidate.IsChecked = 1;
            
        }

        private void chkCandidate_UnChecked(object sender, RoutedEventArgs e)
        {
            COLAList = 0;
        }

        private void chkApply_Checked(object sender, RoutedEventArgs e)
        {
            COLAApply = 1;
            chkApply.IsChecked = 1;
            chkCandidate.IsChecked = 0;
        }

        private void chkApply_UnChecked(object sender, RoutedEventArgs e)
        {
            COLAApply = 0;
        }


        private void CloseScreen()
        {
            System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                AdjParent.Close();
            }
        
        }


        





    }
}
 
