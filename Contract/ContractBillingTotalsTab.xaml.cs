using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RazerBase;
using RazerInterface;

namespace Contract
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ContractBillingTotalsTab : ScreenBase
    {
        public ContractBillingTotalsTab()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "invoice";

            //Establist the COmpany Contract Grid
            gInvoices.xGrid.FieldSettings.AllowEdit = false;
            gInvoices.MainTableName = "invoice";
            gInvoices.ConfigFileName = "ContractBillingTotalsInvoicesGrid";
            gInvoices.SetGridSelectionBehavior(false, true);
            gInvoices.FieldLayoutResourceString = "ContractBillingTotalsInvoices";
            gInvoices.WindowZoomDelegate = GridDoubleClickDelegate;

            gLineItems.xGrid.FieldSettings.AllowEdit = false;
            gLineItems.MainTableName = "invoice_line";
            gLineItems.ConfigFileName = "ContractBillingTotalsLineItemsGrid";
            gLineItems.SetGridSelectionBehavior(true, false);
            gLineItems.FieldLayoutResourceString = "ContractBillingTotalsLineItems";
            gInvoices.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "invoice_number" }, 
                ChildGrids = { gLineItems }, ParentFilterOnColumnNames = { "invoice_number" } });

            gRevenueRecognition.xGrid.FieldSettings.AllowEdit = false;
            gRevenueRecognition.MainTableName = "invoice_acct_detail";
            gRevenueRecognition.ConfigFileName = "ContractBillingTotalsRevenueRecognitionGrid";
            gRevenueRecognition.SetGridSelectionBehavior(true, false);
            gRevenueRecognition.FieldLayoutResourceString = "ContractBillingTotalsRevenueRecognition";
            gInvoices.ChildSupport.Add(new ChildSupport
            {
                ChildFilterOnColumnNames = { "invoice_number" },
                ChildGrids = { gRevenueRecognition },
                ParentFilterOnColumnNames = { "invoice_number" }
            });

            gLocations.xGrid.FieldSettings.AllowEdit = false;
            gLocations.MainTableName = "invoice_location";
            gLocations.ConfigFileName = "ContractBillingTotalsLocationGrid";
            gLocations.SetGridSelectionBehavior(true, false);
            gLocations.FieldLayoutResourceString = "ContractBillingTotalsLocation";
            gInvoices.ChildSupport.Add(new ChildSupport { ChildFilterOnColumnNames = { "invoice_number" }, ChildGrids = { gLocations }, ParentFilterOnColumnNames = { "invoice_number" } });

            GridCollection.Add(gInvoices);
            GridCollection.Add(gLineItems);
            GridCollection.Add(gLocations);
            GridCollection.Add(gRevenueRecognition);
        }

        public void GridDoubleClickDelegate()
        {
            if (gInvoices.xGrid.ActiveRecord != null)
            {
                // RES 1/24/18 determine if invoice or custom invoice and call appropriate folder
                //    //call invoice folder
                gInvoices.ReturnSelectedData("invoice_number");
                cGlobals.ReturnParms.Add("GridLocationBillingTotal.xGrid");
                if (cGlobals.ReturnParms[0].ToString().ToUpper().Substring(0,3) == "INV")
                    cGlobals.ReturnParms[1] = ("InvoiceZoom");
                else
                    cGlobals.ReturnParms[1] = ("CustomInvoiceZoom");
                
                RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
                args.Source = gInvoices.xGrid;
                EventAggregator.GeneratedClickHandler(this, args);
            }
        }
    }
}
