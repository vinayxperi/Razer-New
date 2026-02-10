

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace MiscFolders
{

    #region class CustomerDocumentGeneralTab base
    /// <summary>
    /// This class represents a 'CustomerDocumentGeneralTab' object.
    /// </summary>
    public partial class CustomerDocumentGeneralTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'CustomerDocumentGeneralTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomerDocumentGeneralTab()
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
            MainTableName = "general";
        }
        #endregion

        #endregion

    }
    #endregion

}
