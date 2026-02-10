

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;

#endregion

namespace Customer
{

    #region class CustomerContractsTab
    /// <summary>
    /// This class represents a 'CustomerContractsTab' object.
    /// </summary>
    public partial class CustomerContractsTab : ScreenBase
    {

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'CustomerContractsTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerContractsTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "customer_contracts";
            GridContracts.ConfigFileName = "CustomerContracts";
            GridContracts.MainTableName = "customer_contracts";
            GridContracts.WindowZoomDelegate = GridDoubleClickDelegate;
            GridContracts.ContextMenuAddIsVisible = false;
            GridContracts.ContextMenuRemoveIsVisible = false;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridContracts.SetGridSelectionBehavior(true, false);
            GridContracts.FieldLayoutResourceString = "CustomerContractsGrid";
            GridCollection.Add(GridContracts);
        }

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        public void GridDoubleClickDelegate()
        {
            //call contracts folder
            GridContracts.ReturnSelectedData("contract_id");
            cGlobals.ReturnParms.Add("GridContracts.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = GridContracts.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);            


        }

        private void ContextMenuAddDelegate()
        {

        }

        private void ContextMenuRemoveDelegate()
        {

        }

        #endregion

    }
    #endregion

}
