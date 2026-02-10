

#region using statements


using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;

#endregion

namespace Invoice
{

    #region class InvoiceDefRevTab
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class InvoiceDefRevTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public InvoiceDefRevTab()
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
            CurrentBusObj = new cBaseBusObject("Invoice");
            CurrentBusObj.Parms.ClearParms();

            //Establish the DefPool Grid
            gInvoiceDefRev.MainTableName = "defpool";
            gInvoiceDefRev.ConfigFileName = "InvoicedefpoolTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gInvoiceDefRev.SetGridSelectionBehavior(false, true);
            gInvoiceDefRev.FieldLayoutResourceString = "InvoiceDefPool";
            gInvoiceDefRev.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "pool_id" }, ChildGrids = { gInvoiceDefRevDetail }, ParentFilterOnColumnNames = { "pool_id"  } });
            

            //GridCustomerDocumentDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "apply_to_doc", "seq_code" }, ChildGrids = { GridCustomerDocumentApplied }, ParentFilterOnColumnNames = { "document_id", "seq_code" } });
            //Establish the DefPool Detail Grid
            gInvoiceDefRevDetail.MainTableName = "defpooldtl";
            gInvoiceDefRevDetail.ConfigFileName = "InvoicedefpooldtlTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gInvoiceDefRevDetail.SetGridSelectionBehavior(false, true);
            gInvoiceDefRevDetail.FieldLayoutResourceString = "InvoiceDefPoolDtl";

            //Establish the DefPool Reason Grid
            gInvoiceDefRevReason.MainTableName = "defpoolrsn";
            gInvoiceDefRevReason.ConfigFileName = "InvoicedefpoolrsnTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gInvoiceDefRevReason.SetGridSelectionBehavior(false, true);
            gInvoiceDefRevReason.FieldLayoutResourceString = "InvoiceDefPoolRsn";
            gInvoiceDefRevDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "pool_id", "pool_detail_id" }, ChildGrids = { gInvoiceDefRevReason }, ParentFilterOnColumnNames = { "pool_id", "pool_detail_id" } });

            //Establish the DefPool Reason Total Grid
            gInvoiceDefReasonTot.MainTableName = "defpoolrsntot";
            gInvoiceDefReasonTot.ConfigFileName = "InvoicedefpoolrsntotTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gInvoiceDefReasonTot.SetGridSelectionBehavior(false, true);
            gInvoiceDefReasonTot.FieldLayoutResourceString = "InvoiceDefPoolRsnTot";
            GridCollection.Add(gInvoiceDefRev);
            GridCollection.Add(gInvoiceDefRevDetail);
            GridCollection.Add(gInvoiceDefRevReason);
            GridCollection.Add(gInvoiceDefReasonTot);
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