

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;

#endregion

namespace Entity
{

    #region class EntityContractsTab
    /// <summary>
    /// This class represents a 'EntityContractsTab' object.
    /// </summary>
    public partial class EntityContractsTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'EntityContractsTab' object and call the ScreenBase's constructor.
        /// </summary>
        public EntityContractsTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion
 

        #region Init()
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            layouts.AllowAddNew = false;

            GridEntityContracts.xGrid.FieldLayoutSettings = layouts;
            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "contracts";
            GridEntityContracts.MainTableName = "contracts";
            //GridEntityContracts.ConfigFileName = "GridEntityContracts";
            GridEntityContracts.SetGridSelectionBehavior(false, true);
            GridEntityContracts.FieldLayoutResourceString = "contracts";
            //set to allow double click to transfer to contract folder
            GridEntityContracts.WindowZoomDelegate = GridDoubleClickDelegate;

            GridCollection.Add(GridEntityContracts);
        }
        #endregion

        //logic to transfer control to the Contracts Folder
        public void GridDoubleClickDelegate()
        {
            //call customer document folder
            GridEntityContracts.ReturnSelectedData("contract_id");
            cGlobals.ReturnParms.Add("GridEntityContracts.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = GridEntityContracts.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        } 

    }
    #endregion

}
