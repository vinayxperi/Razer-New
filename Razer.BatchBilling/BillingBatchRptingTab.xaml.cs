

#region using statements
using RazerBase.Interfaces;
using RazerBase;
using Razer.Common;
using RazerBase.Lookups;
using RazerInterface;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Linq;
 
#endregion

namespace Razer.BatchBilling
{


    /// <summary>
    /// This class represents a 'BillingBatchRptingTab' object.
    /// </summary>
    public partial class BillingBatchRptingTab : ScreenBase
    {

        int BatchID = 0;
        int ContractID = 0;
        private static readonly string dataKey = "batch_id";
        private static readonly string dataKey2 = "contract_id";


        /// <summary>
        /// Create a new instance of a 'BillingBatchSummaryTab' object and call the ScreenBase's constructor.
        /// </summary>
        public BillingBatchRptingTab()
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
            MainTableName = "rptunposted";

            //Create the Summary base object
            CurrentBusObj = new cBaseBusObject("BillingBatchFolder");
            CurrentBusObj.Parms.ClearParms();


            //Establish the Billing Batch Unposted Reporting Grid
            gBillingBatchRpting.MainTableName = "rptunposted";
            gBillingBatchRpting.SetGridSelectionBehavior(false, true);
            gBillingBatchRpting.FieldLayoutResourceString = "allUnpostedBilling";
            gBillingBatchRpting.ConfigFileName = "allUnpostedBilling";
            gBillingBatchRpting.IsFilterable = false;

            //Establish the Billing Batch Unposted Detail Reporting Grid
            gBillingBatchDtlRpting.MainTableName = "rptunposteddtl";
            gBillingBatchDtlRpting.SetGridSelectionBehavior(false, true);
            gBillingBatchDtlRpting.FieldLayoutResourceString = "allUnpostedBillingDetail";
            gBillingBatchDtlRpting.ConfigFileName = "allUnpostedBillingDetail";
            gBillingBatchDtlRpting.IsFilterable = false;

            //Establish the Billing Batch Unposted Acct Detail Reporting Grid
            gBillingBatchAcctRpting.MainTableName = "rptacctdetail";
            gBillingBatchAcctRpting.SetGridSelectionBehavior(false, true);
            gBillingBatchAcctRpting.FieldLayoutResourceString = "BillingBatchAcctDetail";
            gBillingBatchAcctRpting.ConfigFileName = "BillingBatchAcctDetailRpt";
            gBillingBatchAcctRpting.IsFilterable = false;


            //Add grid to Collection
            GridCollection.Add(gBillingBatchRpting);
            GridCollection.Add(gBillingBatchDtlRpting);
            GridCollection.Add(gBillingBatchAcctRpting);

        }
    }
}


       