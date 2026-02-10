

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace GeneralLedger
{

    #region class GLDetailTab
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class GLDetailTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public GLDetailTab()
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

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "GLFolder";
            //Set up Parent Child Relationship
            //Create the Customer Document object
            CurrentBusObj = new cBaseBusObject("GLFolder");
            CurrentBusObj.Parms.ClearParms();

            //Establish the Adjustment Detail Grid

            gridVchrDetailTab.MainTableName = "detail";
            gridVchrDetailTab.SetGridSelectionBehavior(false, true);
            gridVchrDetailTab.FieldLayoutResourceString = "GLVchrDetail";
            //gridVchrDetailTab.WindowZoomDelegate = GridDoubleClickDelegate;

            GridCollection.Add(gridVchrDetailTab);
        }
        #endregion

        #endregion

    }
    #endregion

}
