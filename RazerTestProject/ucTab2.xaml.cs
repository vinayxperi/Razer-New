

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace RazerTestProject
{

    #region class ucTab2
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class ucTab2 : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public ucTab2() : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            //Add any initialization after the InitializeComponent() call.
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "[CHANGE]";
        }
        #endregion

        #region Methods



        #endregion

    }
    #endregion

}
