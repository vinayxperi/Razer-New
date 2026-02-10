

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

    #region class InvoiceAdjustmentTab
    /// <summary>
    /// This class represents a 'ucTab2' object.
    /// </summary>
    public partial class InvoiceAdjustmentTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucTab2' object and call the ScreenBase's constructor.
        /// </summary>
        public InvoiceAdjustmentTab()
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
            MainTableName = "adjustment";
            //Set up Parent Child Relationship
            //Create the Customer Document object
            CurrentBusObj = new cBaseBusObject("Invoice");
            CurrentBusObj.Parms.ClearParms();

            //Establish the Invoice Detail Grid
            gInvoiceAdjustment.MainTableName = "adjustment";
            gInvoiceAdjustment.ConfigFileName = "InvoiceAdjustmentTab";
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            gInvoiceAdjustment.SetGridSelectionBehavior(false, true);
            gInvoiceAdjustment.FieldLayoutResourceString = "InvoiceAdjustment";
            gInvoiceAdjustment.WindowZoomDelegate = GridDetailDoubleClickDelegate;
            

            //GridCustomerDocumentDetail.ChildSupport.Add(new ChildSupport() { ChildFilterOnColumnNames = { "apply_to_doc", "seq_code" }, ChildGrids = { GridCustomerDocumentApplied }, ParentFilterOnColumnNames = { "document_id", "seq_code" } });




            GridCollection.Add(gInvoiceAdjustment);

            this.HasPrintReport = true;

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
        public void GridDetailDoubleClickDelegate()
        {
            //determine what folder to call based on detail type
            cGlobals.ReturnParms.Clear();
            gInvoiceAdjustment.ReturnSelectedData("document_id");
            cGlobals.ReturnParms.Add("GridInvoiceAdjustment.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = gInvoiceAdjustment.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        }

        public override void PrintReport()
        {
            string validInvNbr;

            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["general"] != null && CurrentBusObj.ObjectData.Tables["general"].Rows.Count > 0)
            {
                validInvNbr = this.CurrentBusObj.ObjectData.Tables["general"].Rows[0]["inv_nbr"].ToString();
                if (validInvNbr == "")
                {
                    MessageBox.Show("Invoice Number required to run Report.");
                    return;
                }
                else
                {
                    MessageBoxResult result = Messages.ShowYesNo("Do you want to save the adjusted invoice?", MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (!cGlobals.BillService.ReprintCustomInvoice(validInvNbr))
                        {
                            Messages.ShowInformation("The reprint job failed.");
                        }
                        else
                        {
                            //Messages.ShowInformation("Screen Report Sent to Printer.");
                            Messages.ShowInformation("Invoice PDF generated.");
                        }
                    }
                    else
                    {
                        PrintReportJobName = "Invoice Adjustment Tab Print";
                        PrintReportParms.Add(CurrentBusObj.ObjectData.Tables["general"].Rows[0]["inv_nbr"].ToString());
                        base.PrintReport();
                    }
                }
            }

        }

        #endregion

        #endregion

    }
    #endregion

}