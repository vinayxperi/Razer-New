

#region using statements


using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;

#endregion

namespace Customer
{

    #region class CusomterRelationshipsTab
    /// <summary>
    /// This class represents a 'CusomterRelationshipsTab' object.
    /// </summary>
    public partial class CusomterRelationshipsTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'CusomterRelationshipsTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CusomterRelationshipsTab()
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
            MainTableName = "Relationships";
            GridParentRelationships.MainTableName = "Relationships";
            GridParentRelationships.ConfigFileName = "CustomerRelationships";
            GridParentRelationships.WindowZoomDelegate = GridDoubleClickDelegate;
            GridParentRelationships.ContextMenuAddIsVisible = false;
            GridParentRelationships.ContextMenuRemoveIsVisible = false;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridParentRelationships.SetGridSelectionBehavior(true, false);
            GridParentRelationships.FieldLayoutResourceString = "CustomerRelationshipsGrid";
            //This particular style is used for color coded grids.
            Style CellStyle = (Style)TryFindResource("ColorCodeGridStyle");
            GridParentRelationships.GridCellValuePresenterStyle = CellStyle;
            GridParentRelationships.xGrid.FieldLayoutSettings.HighlightAlternateRecords = false;

            GridCollection.Add(GridParentRelationships);
        }

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        public void ReturnSelectedData()
        {
            //add zoom fucntionality
        }


        #endregion
         public void GridDoubleClickDelegate()
        {
            
            
      


            if (GridParentRelationships.xGrid.ActiveRecord != null)
            {
                cGlobals.ReturnParms.Clear();
                //call customer  folder for account
                GridParentRelationships.ReturnSelectedData("receivable_account");
                cGlobals.ReturnParms.Add("CustZoom");
                RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                args.Source = GridParentRelationships.xGrid;
                EventAggregator.GeneratedClickHandler(this, args);
                                 
            }

            }

         private void ScreenBase_Loaded(object sender, RoutedEventArgs e)
         {

         }
    }
    #endregion

}
