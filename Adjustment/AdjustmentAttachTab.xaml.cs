

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace Adjustment
{

    #region class AdjustmentAttachTab
    /// <summary>
    /// This class represents a 'AdjustmentAttachTab' object.
    /// </summary>
    public partial class AdjustmentAttachTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'AdjustmentAttachTab' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentAttachTab()
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
            MainTableName = "[TableName]";
        }
        #endregion

        #endregion

    }
    #endregion

}
