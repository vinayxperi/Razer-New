

#region using statements

using RazerBase;
using System.Windows.Controls;
using Infragistics.Windows.DockManager;

#endregion

namespace Cash
{

    #region class CashBatchEntryScreen
    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class CashBatchEntryScreen : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructors

            #region Default Construcotr
            /// <summary>
            /// This constructor creates a new instance of a 'CashBatchEntryScreen' object.
            /// The ScreenBase constructor is also called.
            /// </summary>
            public CashBatchEntryScreen() : base()
            {
                // Create Controls
                InitializeComponent();

                // performs initializations for this object.
                Init();
            } 
            #endregion

            #region Parametertized Constructor(cBaseBusObject businessObject)
            /// <summary>
            /// Parameterized Constructor
            /// </summary>
            /// <param name="businessObject"></param>
            public CashBatchEntryScreen(cBaseBusObject businessObject) : base()
            {
                // Create Controls
                InitializeComponent();

                // Perform Initializations for this object.
                Init();

                // set the businessObject
                this.CurrentBusObj = businessObject;

                // if there are parameters than we need to load the data
                if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
                {
                    // load the data
                    this.Load();
                }
            } 
            #endregion

        #endregion

        #region Events

            #region ucLabelTextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            /// <summary>
            /// This event is used to launch the Cash Lookup Screen
            /// </summary>
            private void ucLabelTextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                
            }
            #endregion
            
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

                // Set the Header
                SetHeaderName();
            }
            #endregion

            #region SetHeaderName()
            /// <summary>
            /// This method [enter description here].
            /// </summary>
            private void SetHeaderName()
            {
                ContentPane p = (ContentPane)this.Parent;
                if (p == null)
                {
                    return;
                }
                
                // Set the header text
                p.Header = "Cash Batch Entry";
            }
            #endregion
            
        #endregion

    }
    #endregion

}
