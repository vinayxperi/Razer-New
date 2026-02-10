

#region using statements

using RazerBase;

#endregion

namespace RecvAcctFolder
{

    #region class ucSystem
    /// <summary>
    /// Interaction logic for ucSystem.xaml
    /// </summary>
    public partial class ucSystem : ScreenBase
    {
        
        #region Private Variables
        #endregion
        
        #region Constructor
        /// <summary>
        /// This constructor [enter description here].
        /// </summary>
        public ucSystem() : base()
        {
            // Create controls
            InitializeComponent();
            
            // This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            ////The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "Systems";
            gSystem.MainTableName = "Systems";
            gSystem.ConfigFileName = "ReceivableAccountSystemGrid";  //must be unique from other UC objs
            ////gSystem.WindowZoomDelegate = ReturnSelectedData;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gSystem.SetGridSelectionBehavior(true, false);
            gSystem.FieldLayoutResourceString = "RecvAcctSystemGrid";
            
            
            GridCollection.Add(gSystem);
        }
        #endregion
        
    }
    #endregion

}
