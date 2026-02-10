

#region using statements

using System.Windows.Controls;
using Infragistics.Windows.DockManager;
using RazerBase;
using Infragistics.Windows.Controls;

#endregion

namespace ContractsFolder
{

    #region class FolderMainScreen
    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class FolderMainScreen : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBase constructor is also called.
        /// </summary>
        public FolderMainScreen()
            : base()
        {
            // Create Controls
            InitializeComponent();

            // performs initializations for this object.
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

        }
        #endregion

        #endregion

    }
    #endregion

}
