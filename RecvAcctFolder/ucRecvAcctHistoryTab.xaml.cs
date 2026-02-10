

#region using statements

using RazerBase;

#endregion

namespace RecvAcctFolder
{

    #region class ucRecvAcctHistoryTab
    /// <summary>
    /// Interaction logic for ucRecvAcctHistoryTab.xaml
    /// </summary>
    public partial class ucRecvAcctHistoryTab : ScreenBase
    {
        
        #region Private Variables
        #endregion
        
        #region Constructor
        /// <summary>
        /// This constructor creates a new ucRecvAcctHistoryTab object and calls the ScreenBase's Constructor
        /// which in turn call ScreenBase's constructor
        /// </summary>
        public ucRecvAcctHistoryTab() : base()
        {
            // Create controls
            InitializeComponent();
            
            //This User control is being used as a tab item so the value is set to true
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            //The main table used on the tab item is the General table.  This value will be used to determine what table to pull from the base business object dataset
            MainTableName = "AccountHistory";
            gAcctHistory.MainTableName = "AccountHistory";
            gAcctHistory.ConfigFileName = "ReceivableAccountHistoryGrid";
            gAcctHistory.WindowZoomDelegate = ReturnSelectedData;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gAcctHistory.SetGridSelectionBehavior(true, false);
            gAcctHistory.FieldLayoutResourceString = "RecvAcctHistory";
            
            GridCollection.Add(gAcctHistory);
        }
        #endregion
        
        #region Methods
            
            #region ReturnSelectedData()
            /// <summary>
            /// This method [enter description here].
            /// </summary>
            public void ReturnSelectedData()
            {
                //add zoom fucntionality
            }
            #endregion
            
        #endregion
        
    }
    #endregion

}
