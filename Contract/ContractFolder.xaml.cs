



using RazerBase;



namespace Contract
{

    
    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class ContractFolderMainScreen : ScreenBase
    {

 
        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBase constructor is also called.
        /// </summary>
        public ContractFolderMainScreen()
            : base()
        {
            // Create Controls
            InitializeComponent();

            // performs initializations for this object.
            Init();
        }
 
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
      
    }
   

}
