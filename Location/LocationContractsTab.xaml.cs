

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

    #region class LocationContractsTab
    /// <summary>
    /// This class represents a 'LocationContracts' object.
    /// </summary>
    public partial class LocationContractsTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'LocationContracts' object and call the ScreenBase's constructor.
        /// </summary>
        public LocationContractsTab()
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
            
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            this.CanExecuteNewCommand = false;
            this.CanExecuteSaveCommand = false;
            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "contracts";
            GridLocationContract.MainTableName = "contracts";
            GridLocationContract.ConfigFileName = "GridLocationContract";
            GridLocationContract.SetGridSelectionBehavior(false, true);
            GridLocationContract.FieldLayoutResourceString = "LocationContracts";
            //set to allow double click to transfer to contract folder
            GridLocationContract.WindowZoomDelegate = GridDoubleClickDelegate;

            GridCollection.Add(GridLocationContract);
        }
       
        #endregion
        public void GridDoubleClickDelegate()
        {
            if (GridLocationContract.xGrid.ActiveRecord != null)
            {
                //call contractfolder
                GridLocationContract.ReturnSelectedData("contract_id");
                cGlobals.ReturnParms.Add("GridLocationContracts.xGrid");
                RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                args.Source = GridLocationContract.xGrid;
                EventAggregator.GeneratedClickHandler(this, args);
            }

        } 
        #endregion

    }
    #endregion

}
