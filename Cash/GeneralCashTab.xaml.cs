

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows.Input;
using RazerBase.Lookups;

#endregion

namespace Cash
{

    #region class ucTab1
    /// <summary>
    /// This class represents a 'ucTab1' object.
    /// </summary>
    public partial class GeneralCashTab : ScreenBase, IPreBindable, IPostBindable
    {

        #region Private Variables
        public BatchObject _newobject = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab1' object and call the ScreenBase's constructor.
        /// </summary>
        public GeneralCashTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

       
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "cash_batch";
            this.DoNotSetDataContext = false;
         

        }
      
        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    this.ltBank.SetBindingExpression("bank_id", "bank_name", this.CurrentBusObj.GetTable("cash_bank") as DataTable, "");
                    ltBank.CntrlFocus();
                     
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }
        public void PostBind()
        {
 
                if (lcbPosted.IsChecked == 1)
                {
                    ltAcctPeriod.IsEnabled = false;
                    ltBank.IsEnabled = false;
                    ltTotal.IsEnabled = false;
                    lcbPosted.IsEnabled = false;
                    ucLabelDatePicker1.IsEnabled = false;


                }
                else
                {
                    ltAcctPeriod.IsEnabled = false;
                    ltBank.IsEnabled = true;
                    ltTotal.IsEnabled = true;
                    lcbPosted.IsEnabled = false;
                    ucLabelDatePicker1.IsEnabled = true;

                }

          
        }
        private void button1_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void Customer_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            CashLookup f = new CashLookup();
            f.ShowDialog();
        }
      
        
        private void change_object()
        {
        }

        private void ltBank_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedValue")
            {
            }
        }

        private void ltTotal_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentBusObj.ObjectData.Tables["cash_batch"].Rows[0]["batch_total"] = decimal.Parse(ltTotal.Text.Replace("$", ""));
            CashFolder cf = findcashfolder(this.Parent);
            cf.Detail.PostBind();
        }
        public CashFolder findcashfolder(System.Windows.DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }
            else if (element is CashFolder)
            {
                return element as CashFolder;
            }
            else
            {
                return findcashfolder(System.Windows.Media.VisualTreeHelper.GetParent(element));
            }

        }

    }
    #endregion

}
