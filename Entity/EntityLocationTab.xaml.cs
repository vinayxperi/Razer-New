

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

    #region class EntityLocationsTab
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class EntityLocationTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
         public EntityLocationTab()
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

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "locations";
            //Establish the WHT Tracking Grid
            GridEntityLocation.MainTableName = "locations";
            GridEntityLocation.ConfigFileName = "GridEntityLocation";
            GridEntityLocation.WindowZoomDelegate = GridDoubleClickDelegate;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridEntityLocation.SetGridSelectionBehavior(false, true);
            GridEntityLocation.FieldLayoutResourceString = "locations";

            GridCollection.Add(GridEntityLocation);


            GridOwnerEntityLocation.MainTableName = "locationsOwner";
            GridOwnerEntityLocation.WindowZoomDelegate = GridDoubleClickOwnerDelegate;
            GridOwnerEntityLocation.SetGridSelectionBehavior(false, true);
            GridOwnerEntityLocation.FieldLayoutResourceString = "locationForOwnerEntity";

            GridCollection.Add(GridOwnerEntityLocation);
            
        }
        #endregion
     
        public void GridDoubleClickDelegate()
        {
            //call customer document folder
            GridEntityLocation.ReturnSelectedData("cs_id");
            cGlobals.ReturnParms.Add("GridEntityLocation.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = GridEntityLocation.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        }

        public void GridDoubleClickOwnerDelegate()
        {
            //call customer document folder
            GridOwnerEntityLocation.ReturnSelectedData("cs_id");
            cGlobals.ReturnParms.Add("GridEntityLocation.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = GridOwnerEntityLocation.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        } 

        #endregion

    }
  

}
