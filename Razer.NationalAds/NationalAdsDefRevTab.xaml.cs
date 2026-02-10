

#region using statements


using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;

#endregion

namespace Razer.NationalAds
{

    #region class NationalAdsRevTab
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class NationalAdsDefRevTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public NationalAdsDefRevTab()
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
            CurrentBusObj = new cBaseBusObject("NatlAds");
            CurrentBusObj.Parms.ClearParms();

            //Establish the DefPool Grid
            gNationalAdsDefRev.MainTableName = "defpool";
            gNationalAdsDefRev.ConfigFileName = "NationalAdsdefpoolTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gNationalAdsDefRev.SetGridSelectionBehavior(false, true);
            gNationalAdsDefRev.FieldLayoutResourceString = "NationalAdsDefPool";
            gNationalAdsDefRev.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "pool_id" }, ChildGrids = { gNationalAdsDefRevDetail }, ParentFilterOnColumnNames = { "pool_id"  } });
            

            //GridCustomerDocumentDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "apply_to_doc", "seq_code" }, ChildGrids = { GridCustomerDocumentApplied }, ParentFilterOnColumnNames = { "document_id", "seq_code" } });
            //Establish the DefPool Detail Grid
            gNationalAdsDefRevDetail.MainTableName = "defpooldtl";
            gNationalAdsDefRevDetail.ConfigFileName = "NationalAdsdefpooldtlTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gNationalAdsDefRevDetail.SetGridSelectionBehavior(false, true);
            gNationalAdsDefRevDetail.FieldLayoutResourceString = "NationalAdsDefPoolDtl";

            //Establish the DefPool Reason Grid
            gNationalAdsDefRevReason.MainTableName = "defpoolrsn";
            gNationalAdsDefRevReason.ConfigFileName = "NationalAdsdefpoolrsnTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gNationalAdsDefRevReason.SetGridSelectionBehavior(false, true);
            gNationalAdsDefRevReason.FieldLayoutResourceString = "NationalAdsDefPoolRsn";
            gNationalAdsDefRevDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "pool_id", "pool_detail_id" }, ChildGrids = { gNationalAdsDefRevReason }, ParentFilterOnColumnNames = { "pool_id", "pool_detail_id" } });

            //Establish the DefPool Reason Total Grid
            gNationalAdsDefReasonTot.MainTableName = "defpoolrsntot";
            gNationalAdsDefReasonTot.ConfigFileName = "NationalAdsdefpoolrsntotTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gNationalAdsDefReasonTot.SetGridSelectionBehavior(false, true);
            gNationalAdsDefReasonTot.FieldLayoutResourceString = "NationalAdsDefPoolRsnTot";
            GridCollection.Add(gNationalAdsDefRev);
            GridCollection.Add(gNationalAdsDefRevDetail);
            GridCollection.Add(gNationalAdsDefRevReason);
            GridCollection.Add(gNationalAdsDefReasonTot);
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