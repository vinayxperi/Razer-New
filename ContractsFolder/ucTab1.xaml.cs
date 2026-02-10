

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace ContractsFolder
{

    #region class ucTab1
    /// <summary>
    /// This class represents a Tab object.
    /// </summary>
    public partial class ucTab1 : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a ucRecvAcctGeneralTab and call the ScreenBase's constructor
        /// </summary>
        public ucTab1()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            //Add any initialization after the InitializeComponent() call.
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "General";
        }
        #endregion

        #region Methods



        #endregion

    }
    #endregion

}
