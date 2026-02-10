

#region using statements

using RazerBase;
using System.Windows.Controls;
using Infragistics.Windows.DockManager;

#endregion

namespace Cash
{

    #region class CashScreen
    /// <summary>
    /// This class is the main folder for this project.
    /// </summary>
    public partial class CashScreen : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructors

            #region Default Construcotr
            /// <summary>
            /// This constructor creates a new instance of a 'CashScreen' object.
            /// The ScreenBase constructor is also called.
            /// </summary>
            public CashScreen() : base()
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
            public CashScreen(cBaseBusObject businessObject) : base()
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

                // add the Tabs
                TabCollection.Add(this.GeneralTab);
                TabCollection.Add(this.DetailTab);

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
                //if (!string.IsNullOrEmpty(this.txtb))
                //{
                //    p.Header = tAcctName.Text;
                //}
                //else
                //{
                //    p.Header = "Recevable Account";
                //}
            }
            #endregion

            private void ucLabelTextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {

            }
            
        #endregion

    }
    #endregion

}
