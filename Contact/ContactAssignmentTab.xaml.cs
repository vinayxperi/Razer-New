

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;

#endregion

namespace Contact
{

    #region class ContactAssignmentTab
    /// <summary>
    /// This class represents a ContactAssignmentTab object.
    /// </summary>
    public partial class ContactAssignmentTab : ScreenBase
    {

        #region Constructor
        /// <summary>
        /// Create a new instance of a ContactAssignmentTab object and call the ScreenBase's constructor.
        /// </summary>
        public ContactAssignmentTab()
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
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the account_history table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "assignment";
            GridAssignments.MainTableName = "assignment";
            GridAssignments.WindowZoomDelegate = GridDoubleClickDelegate;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            GridAssignments.SetGridSelectionBehavior(true, false);
            GridAssignments.IsFilterable = true;
            GridAssignments.FieldLayoutResourceString = "ContactAssignmentGrid";
            GridCollection.Add(GridAssignments);
        }

        /// <summary>
        /// Delegate for grid 
        /// </summary>
        public void GridDoubleClickDelegate()
        {
            //call contracts folder
            GridAssignments.ReturnSelectedData("related_to_char_id");
            cGlobals.ReturnParms.Add("GridContactAssignment.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = GridAssignments.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        }

        #endregion

    }
    #endregion

}
