

#region using statements

using RazerBase;
using RazerConverters;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace Adjustment
{

    #region class AdjustmentAcctgTab
    /// <summary>
    /// This class represents AdjustmentAcctgTab object.
    /// </summary>
    public partial class AdjustmentAcctgTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'RazerTab1' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentAcctgTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            this.CanExecuteNewCommand = false;
            this.CanExecuteSaveCommand = false;
            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "adjtotal";

            SetParentChildAttributes();








        }



        private void SetParentChildAttributes()
        {
            //Create the WHT Tracking object
            CurrentBusObj = new cBaseBusObject("Adjustmemt");
            CurrentBusObj.Parms.ClearParms();
            //setup parent grid

            //Establish the WHT Tracking Grid

            //Establish the HeadEnd Grid
            //GridAcctSmry.MainTableName = "acctsmry";
            //GridAcctSmry.SetGridSelectionBehavior(false, true);
            //GridAcctSmry.FieldLayoutResourceString = "AdjustmentAcct";
            //Add Tab to collection
            //GridCollection.Add(GridAcctSmry);

            //GridAcctSmry.SetGridSelectionBehavior(false, false);
            //GridAcctSmry.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "adj_acct_id" }, ChildGrids = { GridAcctDetail }, ParentFilterOnColumnNames = { "adj_acct_id" } });




            //setup attributes for Child
            GridAcctDetail.MainTableName = "acctdetail";

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

             GridAcctDetail.FieldLayoutResourceString = "AdjustmentAcctDetail";

            GridAcctDetail.SetGridSelectionBehavior(false, false);

            //Add Tab to collection
            GridCollection.Add(GridAcctDetail);
        }

        private void txtDebits_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void txtCredits_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

       
    }
        #endregion



}
    #endregion
