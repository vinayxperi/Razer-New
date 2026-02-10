

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace RazerTestProject
{

    #region class ucContactInfoTab
    /// <summary>
    /// This class represents a Tab object.
    /// </summary>
    public partial class ucContactInfoTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a ucRecvAcctGeneralTab and call the ScreenBase's constructor
        /// </summary>
        public ucContactInfoTab() : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform Initializations for this object
            Init();
        }
        #endregion

        #region Methods

            #region Init()
            /// <summary>
            /// This method performs initializations for this object
            /// </summary>
            public void Init()
            {
                // Set to a tab
                this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

                // The main table used on the tab item is the Customers table. 
                // This value will be used to determine what table to pull from the base business object dataset
                MainTableName = "Customers";
            }
            #endregion
            
        #endregion

    }
    #endregion

}
