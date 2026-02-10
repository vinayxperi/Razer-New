

#region using statements

using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
#endregion

namespace Cash
{

    #region class CashEntryTab
    /// <summary>
    /// This class represents a 'CashEntryTab' object.
    /// </summary>
    public partial class CashEntryTab : ScreenBase, IPreBindable
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'CashEntryTab' object and call the ScreenBase's constructor.
        /// </summary>
        public CashEntryTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
             this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
            MainTableName = "cash_remit";
           SetParentChildAttributes();
           
        }
        #endregion

        private void SetParentChildAttributes()
        {
            CurrentBusObj = new cBaseBusObject("CashFolder");
            CurrentBusObj.Parms.ClearParms();
            //setup parent grid

            //Establish the WHT Tracking Grid
            FieldLayoutSettings f = new FieldLayoutSettings();
            f.HighlightAlternateRecords = true;
            gRemittance.xGrid.FieldLayoutSettings = f;
            
            gRemittance.MainTableName = "cash_remit";
            gRemittance.ConfigFileName = "RemitGrid";
            gRemittance.FieldLayoutResourceString = "remits";
            //Set the grid to allow edits, for readonly columns set the allowedit to false in the field layouts file
            gRemittance.xGrid.FieldSettings.AllowEdit = false;
            //add delegate to doubleclick and transfer control to customer document folder
            gRemittance.WindowZoomDelegate = GridDoubleClickDelegate;
            // gWHTTracking.IsFilterable = true;
            gRemittance.SetGridSelectionBehavior(false, false);
            GridCollection.Add(gRemittance);
           // GridCollection.Add(gAllocation);
        }
        public void GridDoubleClickDelegate()
        {
            //call customer document folder
            gRemittance.ReturnSelectedData("remit_id");
            cGlobals.ReturnParms.Add("gRemit.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gRemittance.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        }
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
           SetParentChildAttributes();
        }
        #endregion

        public void PreBind()
        {
            try
            {
                // if the object data was loaded
                if (this.CurrentBusObj.HasObjectData)
                {
                    this.lktbBankID.SetBindingExpression("bank_id", "bank_name", this.CurrentBusObj.GetTable("cash_bank") as DataTable, "");
                    this.ltbCompanyCode.SetBindingExpression("company_code", "company_description", this.CurrentBusObj.GetTable("cash_company") as DataTable, "");
                    this.ltbSourceID.SetBindingExpression("source_id", "description", this.CurrentBusObj.GetTable("cash_source") as DataTable, ""); 
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();
            }
        }

    }

}
