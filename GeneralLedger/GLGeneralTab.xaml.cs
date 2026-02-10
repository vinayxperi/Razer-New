

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;

#endregion

namespace GeneralLedger
{

    #region class GLGeneralTab
    /// <summary>
    /// This class represents a 'ucTab1' object.
    /// </summary>
    public partial class GLGeneralTab : ScreenBase
    {

        

       
        public GLGeneralTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
   

      

        
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "general";
            //Establish the Adjustment Detail Grid

            gridVchrGeneralTab.MainTableName = "generalDtl";
            gridVchrGeneralTab.SetGridSelectionBehavior(false, true);
            gridVchrGeneralTab.FieldLayoutResourceString = "GLVchrGeneralDetail";
            //gridVchrDetailTab.WindowZoomDelegate = GridDoubleClickDelegate;

            GridCollection.Add(gridVchrGeneralTab);
        }
        #endregion

        

       

    }
   

}
