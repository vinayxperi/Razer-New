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
using RazerBase.Interfaces;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Razer.NationalAds
{
    /// <summary>
    /// Interaction logic for NationalAdsSearch.xaml
    /// </summary>
    public partial class NationalAdsSearch : ScreenBase, IScreen
    {
        private static readonly string productsTable = "products";
        private static readonly string productsObject = "NatlAdsProducts";
        private static readonly string adsSearchTable = "natl_ads_search";
        private static readonly string onlineLeadsSearchTable = "natl_ads_search_onlineleads";
        private static readonly string adsLayout = "NationalAdsSearch";
        private static readonly string adsOnlineLeadsLayout = "NationalAdsSearchOnlineLeads";
        private static readonly string onlineLeadsProductCode = "ONLINEAD";
        private static readonly string adProduct = "@ad_product";
        private static readonly string customerId = "@customer_id";
        private static readonly string advertiser = "@advertiser";
        private static readonly string accountName = "@account_name";
        private static readonly string productCode = "@product_code";
        private static readonly string invoiceFromDate = "@invoice_date_from";
        private static readonly string invoiceToDate = "@invoice_date_to";
        private static readonly string netAmtField = "net_amt";
        private static readonly string productCodeField = "product_code";
        private static readonly string productCodeTitle = "Product Code";
        private static readonly string invoiceNumberField = "invoice_number";        

        public string WindowCaption { get { return string.Empty; } }
        

        public NationalAdsSearch()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;

            cBaseBusObject productsBusObject = new cBaseBusObject(productsObject);
            productsBusObject.LoadData();

            DataTable source = productsBusObject.ObjectData.Tables[productsTable] as DataTable;            
            txtProductCode.SetBindingExpression(productCodeField, productCodeField, source);

            idgNationalAdsSearch.WindowZoomDelegate = GridDoubleClickDelegate;
           
        }        

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            CurrentBusObj = new cBaseBusObject(this.CurrentBusObj.BusObjectName);
            CurrentBusObj.Parms.AddParm(adProduct, txtProduct.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(customerId, txtReceivableAccount.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(advertiser, txtAdvertiser.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(accountName, txtAccountName.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(productCode, txtProductCode.SelectedValue);

            if (ldpInvoiceFrom.SelText != null)            
                CurrentBusObj.Parms.AddParm(invoiceFromDate, ldpInvoiceFrom.SelText);            
            else            
                CurrentBusObj.Parms.AddParm(invoiceFromDate, new DateTime(1900, 1, 1));

            if ((ldpInvoiceTo.SelText != null) && (ldpInvoiceTo.SelText.ToString() != "1/1/1900 12:00:00 AM"))
                CurrentBusObj.Parms.AddParm(invoiceToDate, ldpInvoiceTo.SelText);
            else
                CurrentBusObj.Parms.AddParm(invoiceToDate, new DateTime(2100, 1, 1));
                       

            if (txtProductCode.SelectedValue != null && txtProductCode.SelectedValue.ToString() == onlineLeadsProductCode)
            {
                this.MainTableName = onlineLeadsSearchTable;
                idgNationalAdsSearch.FieldLayoutResourceString = adsOnlineLeadsLayout;
            }
            else
            {
                this.MainTableName = adsSearchTable;
                idgNationalAdsSearch.FieldLayoutResourceString = adsLayout;
            }

            FieldLayoutSettings layouts = new FieldLayoutSettings();
            idgNationalAdsSearch.xGrid.FieldLayoutSettings = layouts;
            layouts.HighlightAlternateRecords = true;
            idgNationalAdsSearch.xGrid.FieldSettings.AllowEdit = false;
            idgNationalAdsSearch.SetGridSelectionBehavior(true, false);
            
            this.Load(CurrentBusObj);

            if (CurrentBusObj.HasObjectData)
            {
                txtRows.Text = CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows.Count.ToString() ?? string.Empty;

                decimal total = 0m;

                foreach (DataRow row in CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows)
                {
                    total += (decimal)row[netAmtField];
                }

                txtTotal.Text = total.ToString("C2");
                txtTotal.TextBoxHorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                idgNationalAdsSearch.LoadGrid(CurrentBusObj, this.MainTableName);

            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtProduct.Text = string.Empty;
            txtReceivableAccount.Text = string.Empty;
            txtAdvertiser.Text = string.Empty;
            txtAccountName.Text = string.Empty;
            txtProductCode.SelectedValue = null;
            ldpInvoiceFrom.SelText = null;
            ldpInvoiceTo.SelText = null;
        }

        public void GridDoubleClickDelegate()
        {
            idgNationalAdsSearch.ReturnSelectedData(invoiceNumberField);
            cGlobals.ReturnParms.Add(idgNationalAdsSearch.Name);
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = idgNationalAdsSearch.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);
        }
    }
}
