

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows;


#endregion

namespace Adjustment
{
    #region class AdjustmentGeneralTab
    /// <summary>
    /// This class represents AdjustmentGeneralTab
    /// </summary>
    public partial class AdjustmentGeneralTab : ScreenBase
    {

        #region Private Variables
        #endregion
        public bool FirstTime = false;
        public string sSQL;

        #region Constructor
        /// <summary>
        /// Create a new instance of a AdjustmentGeneralTab object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentGeneralTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

        #region Init()
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "general";
            // RES DoubleClick BCF number to go to BCF Folder
            txtBCF.DoubleClickDelegate = BCFDoubleClickHandler;
            //if (CurrentBusObj.ObjectData.Tables["holdsubs"] != null && CurrentBusObj.ObjectData.Tables["holdsubs"].Rows.Count > 0)
            //if (CurrentBusObj != null)
            //{
            //    if (CurrentBusObj.ObjectData.Tables["holdsubs"] != null && CurrentBusObj.ObjectData.Tables["holdsubs"].Rows.Count > 0)
            //        chkSubsHold.IsChecked = Convert.ToInt32(CurrentBusObj.ObjectData.Tables["holdsubs"].Rows[0]["count"]);
            //}
            //if (CurrentBusObj.ObjectData.Tables["holdsubs"].Rows[0]["count"] = "1")
            //    chkSubsHold.IsChecked = 1;

        }
        #endregion

        private void txtAdjReason_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void txtAmount_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Convert.ToDouble(txtAmount.Text) < 0)
                 txtAmount.TextColor = "Red";
            //otherwise black
            else
                 txtAmount.TextColor = "Black";
        }

        #endregion

       
        private void txtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAmount.Text)) { return; }
            string sAmount = txtAmount.Text;
            sAmount = sAmount.Replace("$", "");
            sAmount = sAmount.Replace(",", "");
            sAmount = sAmount.Replace("(", "-");
            sAmount = sAmount.Replace(")", "");
            if (Convert.ToDouble(sAmount) < 0)
                txtAmount.TextColor = "Red";
            //otherwise black
            else
                txtAmount.TextColor = "Black";
        }

        private void BCFDoubleClickHandler()
        {
            //don't try to go to an empty folder
            if (txtBCF.Text == "") return;
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(txtBCF.Text);
            if (txtBCF.Text.StartsWith("BCF"))
                cGlobals.ReturnParms.Add(txtBCF.Name);
            else
                cGlobals.ReturnParms.Add("TF.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = txtBCF;
            EventAggregator.GeneratedClickHandler(this, args);
        }

        //Set flag to use current tax rates when calculating sales tax
        private void chkSubsHold_Checked(object sender, RoutedEventArgs e)
        {
            if (FirstTime)
            {
                FirstTime = false;
                return;
            }
            if (chkSubsHold.IsChecked == 1)
            {
                CurrentBusObj.LoadTable("prevhold");
                if (Convert.ToInt32(CurrentBusObj.ObjectData.Tables["prevhold"].Rows[0]["count"]) > 0)
                {
                    Messages.ShowWarning("Adjustment has been put on hold once.  Cannot put it on hold again.");
                    FirstTime = true;
                    chkSubsHold.IsChecked = 0;
                    return;
                }
                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to add Subs on Billing Hold comment?", System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //string sSQL;
                    sSQL = "insert dbo.comment select 15,'" + cGlobals.UserName + "',getdate(),'Subs on Billing Hold','AJ',0,'" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString() +
                    "','Subscriber updates on billing hold'";
                    if (cGlobals.BillService.GenericSQL(sSQL) == true)
                    {
                        Messages.ShowWarning("Subs on Billing Hold comment added");
                        CurrentBusObj.LoadTable("comments_char");
                    }
                    else
                        Messages.ShowWarning("Error Inserting Comment");
                }
                else
                {
                    FirstTime = true;
                    chkSubsHold.IsChecked = 0;
                    return;
                }
            }         
          }

        private void chkSubsHold_UnChecked(object sender, RoutedEventArgs e)
        {
            if (FirstTime)
            {
                FirstTime = false;
                return;
            }
            if (chkSubsHold.IsChecked != 1)
            {
                MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to add Subs off Billing Hold comment?", System.Windows.MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    //string sSQL;
                    sSQL = "insert dbo.comment select 15,'" + cGlobals.UserName + "',getdate(),'Subs off Billing Hold','AJ',0,'" + this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["document_id"].ToString() +
                    "','Subscriber updates no longer on billing hold'";
                    if (cGlobals.BillService.GenericSQL(sSQL) == true)
                    {
                        Messages.ShowWarning("Subs off Billing Hold comment added");
                        CurrentBusObj.LoadTable("comments_char");
                    }
                    else
                        Messages.ShowWarning("Error Inserting Comment");
                }
                else
                {
                    FirstTime = true;
                    chkSubsHold.IsChecked = 1;
                    return;
                }
            }
       }

 
    }
    #endregion

}
