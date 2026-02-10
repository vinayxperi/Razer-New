

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;

#endregion

namespace Location
{

    #region class LocationBillingTotalTab
    /// <summary>
    /// This class represents a 'LocationBillingTotalTab' object.
    /// </summary>
    public partial class LocationBillingTotalTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'LocationBillingTotalTab' object and call the ScreenBase's constructor.
        /// </summary>
        public LocationBillingTotalTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        public void Init()
        {

            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            this.CanExecuteNewCommand = false;
            this.CanExecuteSaveCommand = false;
            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "location";
            GridLocationBillingTotal.MainTableName = "billbatch";
            GridLocationBillingTotal.SetGridSelectionBehavior(false, true);
            GridLocationBillingTotal.ConfigFileName = "locationBillingTotals";
            GridLocationBillingTotal.FieldLayoutResourceString = "locationbillingtotal";
            //set to allow double click to transfer to contract folder
            GridLocationBillingTotal.WindowZoomDelegate = GridDoubleClickDelegate;

            GridCollection.Add(GridLocationBillingTotal);
        }

    #endregion
        public void GridDoubleClickDelegate()
        {
            if (GridLocationBillingTotal.xGrid.ActiveRecord != null)
            {
                //    //call invoice folder
                GridLocationBillingTotal.ReturnSelectedData("invoice_number");
                cGlobals.ReturnParms.Add("GridLocationBillingTotal.xGrid");
                RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                args.Source = GridLocationBillingTotal.xGrid;
                EventAggregator.GeneratedClickHandler(this, args);
            }
        }

    }
}
   

 
 