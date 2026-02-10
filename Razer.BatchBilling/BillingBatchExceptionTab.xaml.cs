



using RazerBase;
using RazerInterface;
using System;
using System.Data;
using System.Windows;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;


namespace Razer.BatchBilling
{

   
    /// <summary>
 
    /// </summary>
    public partial class BillingBatchExceptionTab : ScreenBase
    {




        /// <summary>
        /// Create a new instance of a 'BillingBatchDetailTab' object and call the ScreenBase's constructor.
        /// </summary>
        public BillingBatchExceptionTab()
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
            {
                // Set the ScreenBaseType
                this.ScreenBaseType = ScreenBaseTypeEnum.Tab;
                // Change this setting to the name of the DataTable that will be used for Binding.
                MainTableName = "BillingBatchFolder";

                //Establish the exception Grid
                gBillingBatchException.xGrid.FieldSettings.AllowEdit = false;
                gBillingBatchException.MainTableName = "exceptions";
                gBillingBatchException.SetGridSelectionBehavior(false, true);
                gBillingBatchException.FieldLayoutResourceString = "BillingBatchExceptions";
                gBillingBatchException.WindowZoomDelegate = GridDoubleClickDelegate;
                gBillingBatchException.ConfigFileName = "BillingBatchExceptions";

                //Establish the exception Grid
                gBillingBatchContractException.xGrid.FieldSettings.AllowEdit = false;
                gBillingBatchContractException.MainTableName = "contract_exceptions";
                gBillingBatchContractException.SetGridSelectionBehavior(false, true);
                gBillingBatchContractException.FieldLayoutResourceString = "BillingBatchContractExceptions";
                gBillingBatchContractException.ConfigFileName = "BillingBatchContractExceptions";

                //Establish the exception Grid
                gBillingBatchLocationException.xGrid.FieldSettings.AllowEdit = false;
                gBillingBatchLocationException.MainTableName = "location_exceptions";
                gBillingBatchLocationException.SetGridSelectionBehavior(false, true);
                gBillingBatchLocationException.FieldLayoutResourceString = "BillingBatchLocationExceptions";
                gBillingBatchLocationException.ConfigFileName = "BillingBatchLocationExceptions";


                GridCollection.Add(gBillingBatchException);
                GridCollection.Add(gBillingBatchContractException);
                GridCollection.Add(gBillingBatchLocationException);

            }
        }





        public void GridDoubleClickDelegate()
        {
            //call invoiceactruletier detail screen
            //if double click on contract_id, go to contract folder, else call invoiceacctruletier detail screen


            Cell activecell = gBillingBatchException.xGrid.ActiveCell;
            if (activecell == null)
            {
            }
            else
            {
                Record activeRecord = gBillingBatchException.xGrid.Records[gBillingBatchException.ActiveRecord.Index];

                if (activecell.Field.Name == "contract_id")
                {
                    //call contract folder
                    gBillingBatchException.ReturnSelectedData("contract_id");
                    cGlobals.ReturnParms.Add("GridContracts.xGrid");
                    RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                    args.Source = gBillingBatchException.xGrid;
                    EventAggregator.GeneratedClickHandler(this, args);



                }
                else
                {
                    cGlobals.ReturnParms.Clear();

                }

            }
        }
    }


}
