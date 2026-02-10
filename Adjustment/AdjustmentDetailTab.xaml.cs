

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;


#endregion

namespace Adjustment
{

    #region class AdjustmentDetailTab
    /// <summary>
    /// This class represents AdjustmentDetailTab object.
    /// </summary>
    public partial class AdjustmentDetailTab : ScreenBase
    {
        public static bool RMCOn = false;
        #region Private Variables
         public int adjStatus = 0;
         public string adjType;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public AdjustmentDetailTab()
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
            MainTableName = "Adjustment";
            //Set up Parent Child Relationship
            //Create the Customer Document object
            CurrentBusObj = new cBaseBusObject("Adjustment");
            CurrentBusObj.Parms.ClearParms();

            //Establish the Adjustment Detail Grid
          
            gridAdjustmentDetailTab.MainTableName = "detail";
           
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            gridAdjustmentDetailTab.SetGridSelectionBehavior(false, true);
            gridAdjustmentDetailTab.FieldLayoutResourceString = "adjustmentDetail";
            gridAdjustmentDetailTab.WindowZoomDelegate = GridDoubleClickDelegate;
            gridAdjustmentDetailTab.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "line_id" }, ChildGrids = { gridAdjustmentDetailAcctTab }, ParentFilterOnColumnNames = { "line_id" } });
            gridAdjustmentDetailAcctTab.FieldLayoutResourceString = "adjustmentDetailAcct";
            gridAdjustmentDetailAcctTab.MainTableName = "detailacctgrid";
            gridAdjustmentDetailAcctTab.ContextMenuGenericDelegate1 = UpdateAccountStringDelegate;
            gridAdjustmentDetailAcctTab.ContextMenuGenericDisplayName1 = "Update Account Code";
            

            if (base.SecurityContext == AccessLevel.NoAccess || base.SecurityContext == AccessLevel.ViewOnly)
            {
                gridAdjustmentDetailAcctTab.ContextMenuGenericIsVisible1 = false;
                RMCOn = false;
                 
            }
            else
            {
                gridAdjustmentDetailAcctTab.ContextMenuGenericIsVisible1 = true;
                    RMCOn = true;
                
            }
            //Capture if posted or conversion invoice type of adjustment
            //adjStatus = Convert.ToInt32(this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"]);

            //adjType = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["adjustment_type_desc"].ToString();
            //If Posted, cannot change account code
                    

             //if (adjStatus.ToString() == "1")
             //{
             //    gridAdjustmentDetailAcctTab.ContextMenuGenericIsVisible1 = false;
             //}
            //Only allow for Conversion Invoice Adjustment
             //if (adjType.ToString() != "Conversion Invoice - Accounting")
             //{
            //gridAdjustmentDetailAcctTab.ContextMenuGenericIsVisible1 = true;
             //}
          
            GridCollection.Add(gridAdjustmentDetailTab);
            GridCollection.Add(gridAdjustmentDetailAcctTab);

            
        }
        #endregion

        private void UpdateAccountStringDelegate()
        {
            //instance location service screen
            AdjustmentChangeAccount AdjustmentChangeScreen = new AdjustmentChangeAccount(getAdjId(), this.CurrentBusObj);
            //////////////////////////////////////////////////////////////
            //create a new window and embed ProductRulesScreen usercontrol, show it as a dialog
            System.Windows.Window AdjustmentChangeAccountWindow = new System.Windows.Window();
            //set new window properties///////////////////////////
            AdjustmentChangeAccountWindow.Title = "Update Account Code Screen";
            AdjustmentChangeAccountWindow.MaxHeight = 1280;
            AdjustmentChangeAccountWindow.MaxWidth = 1280;
            /////////////////////////////////////////////////////
            //set screen as content of new window
            AdjustmentChangeAccountWindow.Content = AdjustmentChangeScreen;
            //open new window with embedded user control
            AdjustmentChangeAccountWindow.ShowDialog();
        }


        private string getAdjId()
        {
            var localDocumentId = from x in this.CurrentBusObj.ObjectData.Tables["ParmTable"].AsEnumerable()
                                  where x.Field<string>("parmName") == "@document_id"
                                  select x.Field<string>("parmValue");

            foreach (var id in localDocumentId)
            {
                string DocumentId = Convert.ToString(id);
                //return adjustment id
                return DocumentId;
            }
            return " ";
        }
        public void GridDoubleClickDelegate()
        {
            //call invoiceactruletier detail screen
           




        } 

        #endregion

    }
    #endregion

}
