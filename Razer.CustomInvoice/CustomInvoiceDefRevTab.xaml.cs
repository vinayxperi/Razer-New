

#region using statements


using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;

#endregion

namespace Razer.CustomInvoice
{

    #region class CustomInvoiceRevTab
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class CustomInvoiceDefRevTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public CustomInvoiceDefRevTab()
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
            MainTableName = "defpool";
            //Set up Parent Child Relationship
            //Create the Customer Document object
            CurrentBusObj = new cBaseBusObject("CustomInvoice");
            CurrentBusObj.Parms.ClearParms();

            //Establish the DefPool Grid
            gCustomInvoiceDefRev.MainTableName = "defpool";
            gCustomInvoiceDefRev.ConfigFileName = "CustomInvoicedefpoolTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gCustomInvoiceDefRev.SetGridSelectionBehavior(false, true);
            gCustomInvoiceDefRev.FieldLayoutResourceString = "CustomInvoiceDefPool";
            gCustomInvoiceDefRev.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "pool_id" }, ChildGrids = { gCustomInvoiceDefRevDetail }, ParentFilterOnColumnNames = { "pool_id"  } });
            

            //GridCustomerDocumentDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "apply_to_doc", "seq_code" }, ChildGrids = { GridCustomerDocumentApplied }, ParentFilterOnColumnNames = { "document_id", "seq_code" } });
            //Establish the DefPool Detail Grid
            gCustomInvoiceDefRevDetail.MainTableName = "defpooldtl";
            gCustomInvoiceDefRevDetail.ConfigFileName = "CustomInvoicedefpooldtlTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gCustomInvoiceDefRevDetail.SetGridSelectionBehavior(false, true);
            gCustomInvoiceDefRevDetail.FieldLayoutResourceString = "CustomInvoiceDefPoolDtl";

            //Establish the DefPool Reason Grid
            gCustomInvoiceDefRevReason.MainTableName = "defpoolrsn";
            gCustomInvoiceDefRevReason.ConfigFileName = "CustomInvoicedefpoolrsnTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gCustomInvoiceDefRevReason.SetGridSelectionBehavior(false, true);
            gCustomInvoiceDefRevReason.FieldLayoutResourceString = "CustomInvoiceDefPoolRsn";
            gCustomInvoiceDefRevDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "pool_id", "pool_detail_id" }, ChildGrids = { gCustomInvoiceDefRevReason }, ParentFilterOnColumnNames = { "pool_id", "pool_detail_id" } });

            //Establish the DefPool Reason Total Grid
            gCustomInvoiceDefReasonTot.MainTableName = "defpoolrsntot";
            gCustomInvoiceDefReasonTot.ConfigFileName = "CustomInvoicedefpoolrsntotTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gCustomInvoiceDefReasonTot.SetGridSelectionBehavior(false, true);
            gCustomInvoiceDefReasonTot.FieldLayoutResourceString = "CustomInvoiceDefPoolRsnTot";
            GridCollection.Add(gCustomInvoiceDefRev);
            GridCollection.Add(gCustomInvoiceDefRevDetail);
            GridCollection.Add(gCustomInvoiceDefRevReason);
            GridCollection.Add(gCustomInvoiceDefReasonTot);
            //Establish the Invoice Detail with adjustments Grid
            //gInvoicewithAdjustment.MainTableName = "invwithadj";
            ////xaml file name
            //gInvoicewithAdjustment.ConfigFileName = "InvoiceAdjustmentTab";
            //gInvoicewithAdjustment.SetGridSelectionBehavior(false, true);
            //gInvoicewithAdjustment.FieldLayoutResourceString = "InvoiceWithAdjustment";
            ////gInvoicewithAdjustment.WindowZoomDelegate = ReturnSelectedData;

            //GridCollection.Add(gInvoicewithAdjustment);


        }
        public void ReturnSelectedData()
        {
            //Zoom Functionality

        }
       

       

        #endregion

        #endregion

    }
    #endregion

}