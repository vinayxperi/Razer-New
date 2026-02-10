using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RazerBase;
using RazerBase.Interfaces;
using Infragistics.Windows.DataPresenter;
using System.Data;

namespace Razer.CustomInvoice
{
    /// <summary>
    /// Interaction logic for CustomInvoiceLookup.xaml
    /// </summary>
    public partial class CustomInvoiceLookup : DialogBase, IScreen
    {
        private static readonly string fieldLayoutResource = "CustomInvoiceLookup";
        private static readonly string mainTableName = "custom_invoice_list";
        private static readonly string dataKey = "invoice_number";
        private static readonly string accountName = "@account_name";
        private static readonly string customerId = "@customer_id";
        private static readonly string description = "@description";
        private static readonly string contract_description = "@contract_description";
        private static readonly string productCode = "@product_code";
        private static readonly string invoiceFromDate = "@invoice_date_from";
        private static readonly string invoiceToDate = "@invoice_date_to";
        private static readonly string invoiceAmountFrom = "@invoice_amount_from";
        private static readonly string invoiceAmountTo = "@invoice_amount_to";
        private static readonly string productsObject = "ReportManager";
        //private static readonly string productCodeField = "product_code";
        private static readonly string productsTable = "cmbProductsAll";

        private string windowCaption;

        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }

        public CustomInvoiceLookup()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Implement the Init method of IScreen
        /// </summary>
        /// <param name="businessObject">Then base busniess object</param>
        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;


            cBaseBusObject productsBusObject = new cBaseBusObject(productsObject);
            productsBusObject.LoadTable(productsTable);

            //productsBusObject.LoadData();

            DataTable source = productsBusObject.ObjectData.Tables[productsTable] as DataTable;
            txtProductCode.ItemsSource = source.DefaultView;

            txtProductCode.SelectedValue = " ALL";

            //Hook the key down event to date picker's internal text box. 

        }
        /// <summary>
        /// Handler for double click on grid or buttton click.
        /// </summary>
        public void ReturnSelectedData()
        {
            idgCustomInvoices.ReturnSelectedData(dataKey);
            this.Close();
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = EventAggregator.GeneratedClickEvent;
            args.Source = idgCustomInvoices.Name;
            EventAggregator.GeneratedClickHandler(this, args);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ReturnSelectedData();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            decimal minimumAmount, maximumAmount;

            CurrentBusObj = new cBaseBusObject(this.CurrentBusObj.BusObjectName);
            CurrentBusObj.Parms.AddParm(accountName, txtCustomerName.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(customerId, txtCustomerID.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(description, txtDescription.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(contract_description, txtContractDescription.Text ?? string.Empty);


            DataRowView selectedRow = txtProductCode.SelectedItem as DataRowView;

            if (selectedRow != null)
            {
                string selProductCode = selectedRow["product_code"].ToString();
                if (selProductCode == "ALL" || selProductCode == "-")
                    CurrentBusObj.Parms.AddParm(productCode, "");
                else
                    CurrentBusObj.Parms.AddParm(productCode, selProductCode);
            }
            else
            {
                CurrentBusObj.Parms.AddParm(productCode, "");
            }


            if (ldpInvoiceFrom.SelectedDate != null)
                CurrentBusObj.Parms.AddParm(invoiceFromDate, ldpInvoiceFrom.SelectedDate);
            else
                CurrentBusObj.Parms.AddParm(invoiceFromDate, new DateTime(1900, 1, 1));

            if ((ldpInvoiceTo.SelectedDate != null) && (ldpInvoiceTo.SelectedDate.ToString() != "1/1/1900 12:00:00 AM"))
                CurrentBusObj.Parms.AddParm(invoiceToDate, ldpInvoiceTo.SelectedDate);
            else
                CurrentBusObj.Parms.AddParm(invoiceToDate, new DateTime(2100, 1, 1));


            if (txtMinimumAmount.Text != null && decimal.TryParse(txtMinimumAmount.Text, out minimumAmount))
                CurrentBusObj.Parms.AddParm(invoiceAmountFrom, minimumAmount.ToString());
            else
                CurrentBusObj.Parms.AddParm(invoiceAmountFrom, "0.0");

            if (txtMaximumAmount.Text != null && decimal.TryParse(txtMaximumAmount.Text, out maximumAmount))
                CurrentBusObj.Parms.AddParm(invoiceAmountTo, maximumAmount.ToString());
            else
                CurrentBusObj.Parms.AddParm(invoiceAmountTo, "999999999");

            this.MainTableName = mainTableName;

            FieldLayoutSettings layouts = new FieldLayoutSettings();

            idgCustomInvoices.WindowZoomDelegate = ReturnSelectedData;
            idgCustomInvoices.xGrid.FieldLayoutSettings = layouts;
            idgCustomInvoices.FieldLayoutResourceString = fieldLayoutResource;
            idgCustomInvoices.MainTableName = mainTableName;

            this.Load(CurrentBusObj);

            if (CurrentBusObj.HasObjectData)
            {
                idgCustomInvoices.LoadGrid(CurrentBusObj, this.MainTableName);

                if (CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows.Count > 0)
                    btnOk.IsEnabled = true;
                else
                    btnOk.IsEnabled = false;
            }
            else
            {
                btnOk.IsEnabled = false;
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtCustomerName.Text = String.Empty;
            txtCustomerID.Text = String.Empty;
            txtDescription.Text = String.Empty;
            txtContractDescription.Text = String.Empty;
            txtProductCode.SelectedValue = " ALL";

            ldpInvoiceFrom.SelectedDate = null;
            ldpInvoiceFrom.Text = String.Empty;
            ldpInvoiceFrom.DisplayDate = DateTime.Today;

            ldpInvoiceTo.SelectedDate = null;
            ldpInvoiceTo.Text = String.Empty;
            ldpInvoiceTo.DisplayDate = DateTime.Today;


            txtMinimumAmount.Text = String.Empty;
            txtMaximumAmount.Text = String.Empty;

            idgCustomInvoices.xGrid.DataSource = null;

            btnOk.IsEnabled = false;

        }

        private void ldpInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
