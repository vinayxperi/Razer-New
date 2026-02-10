

#region using statements

using RazerBase;

#endregion

namespace RazerTestProject
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
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            // Change this if your folder has item directly bound to it.
            // In many cases the Tabs have data binding, the Folders
            // do not.
            this.DoNotSetDataContext = true;
        }
        #endregion

        #endregion

    }
    #endregion

}
