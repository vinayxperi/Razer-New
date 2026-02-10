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
using Infragistics.Windows.DataPresenter;
using System.Data;

namespace Invoice
{
    /// <summary>
    /// Interaction logic for InvoiceEmailCountriesLookup.xaml
    /// </summary>
    public partial class InvoiceEmailCountriesLookup : DialogBase
    {
        //RES 2/19/15 Company Consolidation
        //public string CompanyCode { get; private set; }
        //public string CompanyDescription { get; private set; }
        public string BillingOwnerID { get; private set; }
        public string BillingOwnerName { get; private set; }

        public InvoiceEmailCountriesLookup(cBaseBusObject CurrentBusObj)
        {
            InitializeComponent();

            this.CurrentBusObj = CurrentBusObj;
            this.MainTableName = "InvoiceCompanies";
            InvoiceEmailCountriesGrid.MainTableName = this.MainTableName;
            InvoiceEmailCountriesGrid.xGrid.FieldLayoutSettings.AllowDelete = false;
            InvoiceEmailCountriesGrid.xGrid.FieldSettings.AllowEdit = false;

            InvoiceEmailCountriesGrid.SetGridSelectionBehavior(false, false);
            InvoiceEmailCountriesGrid.FieldLayoutResourceString = "InvoiceEmailCountriesGrid";

            InvoiceEmailCountriesGrid.WindowZoomDelegate = GridDoubleClickDelegate;

            GridCollection.Add(InvoiceEmailCountriesGrid);

            this.LoadExistingData();
        }

        protected void GridDoubleClickDelegate()
        {
            DataRecord r = InvoiceEmailCountriesGrid.xGrid.ActiveCell.Record;

            if (r != null)
            {
                //RES 2/19/15 Company Consolidation
                BillingOwnerID = r.Cells["billing_owner_id"].Value.ToString();
                BillingOwnerName = r.Cells["billing_owner_name"].Value.ToString();
                //CompanyCode = r.Cells["company_code"].Value.ToString();
                //CompanyDescription = r.Cells["company_description"].Value.ToString();
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnShowAllCompanies_Click(object sender, RoutedEventArgs e)
        {
            //RES 2/19/15 Company Consolidation
            BillingOwnerID = "";
            BillingOwnerName = "";
            //CompanyCode = "";
            //CompanyDescription = "";
            this.DialogResult = true;
            this.Close();
        }

        public void HideShowAllCompaniesButton()
        {
            this.btnShowAllCompanies.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
