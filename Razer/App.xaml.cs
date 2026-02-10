using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Razer.Common;
using Microsoft.Practices.Unity;

namespace Razer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Load Field Layouts

            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = new Uri("Adjustment;component/Resources/AdjustmentFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Admin;component/Resources/AdminFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("BCF;component/Resources/BCFFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("TF;component/Resources/TFFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Razer.BatchBilling;component/Resources/BatchBillingFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Cash;component/Resources/CashFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Cash;component/Resources/CashEntryFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("COLA;component/Resources/COLAFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Contact;component/Resources/ContactFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Contract;component/Resources/ContractFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Customer;component/Resources/CustomerFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Razer.CustomInvoice;component/Resources/CustomInvoiceFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Entity;component/Resources/EntityFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("GeneralLedger;component/Resources/GLVchrFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Invoice;component/Resources/InvoiceFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Location;component/Resources/LocationFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("MiscFolders;component/Resources/MiscFolderFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Razer.NationalAds;component/Resources/NationalAdsLayout.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("SecurityMaintenance;component/Resources/SecurityMaintenanceLayout.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Razer.TableMaintenance;component/Resources/TableMaintenanceLayout.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);
            rd = new ResourceDictionary();
            rd.Source = new Uri("Units;component/Resources/UnitFieldLayouts.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(rd);

        }
    }
}
