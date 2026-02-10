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
using Razer.Common;
using RazerBase.Interfaces;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter.Events;
using System.Data;
using RazerInterface;
using RazerConverters;
using RazerWS;

namespace Razer.NationalAds
{
    /// <summary>
    /// Interaction logic for NationalAdsGeneralTab.xaml
    /// </summary>
    public partial class NationalAdsGeneralTab : ScreenBase
    {
        public string invoiceNumber = string.Empty;
        public int statusFlag = 0;
        public int printedFlag = 0;
        private static readonly string failedMessage = "The reprint job failed.";
        public NationalAdsGeneralTab()
        {
            InitializeComponent();
            Init();
            
        }

        public void Init()
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            layouts.AllowAddNew = false;

            //this.DoNotSetDataContext = true;
            gSales.xGrid.FieldLayoutSettings = layouts;
            gSales.FieldLayoutResourceString = "NationalAdsSales";
            gSales.MainTableName = "sales";
            gSales.xGrid.FieldSettings.AllowEdit = false;
            gSales.SetGridSelectionBehavior(false, false);      

            GridCollection.Add(gSales);
        }

        //private void btnReprint_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!SaveRevision())
        //    {
        //        Messages.ShowInformation("If you want a hardcopy only you will need to print from the PDF.  Otherwise check Save Revision to generate the PDF.");
        //    }
        //    else
        //    {
        //        if (!cGlobals.BillService.ReprintNationalAdInvoice(invoiceNumber, false, SaveRevision(), GetDefaultPrinter()))
        //        {
        //            Messages.ShowInformation(failedMessage);
        //        }
        //        else
        //        {
        //            Messages.ShowInformation("Reprint Job Scheduled");
        //        }
        //    }
        //}

        private void btnReprintRevised_Click(object sender, RoutedEventArgs e)
        {
            //RES 9/17/20 Take out Save Revision checkbox
            //if (!SaveRevision())
            //{
            //    Messages.ShowInformation("If you want a hardcopy only you will need to print from the PDF.  Otherwise check Save Revision to generate the PDF.");
            //}
            //else
            //{
                //if (!cGlobals.BillService.ReprintNationalAdInvoice(invoiceNumber, true, SaveRevision(), GetDefaultPrinter()))
            if (!cGlobals.BillService.ReprintNationalAdInvoice(invoiceNumber, true, true, GetDefaultPrinter()))
                {
                    Messages.ShowInformation(failedMessage);
                }
                else
                {
                    Messages.ShowInformation("Revised PDF Job Scheduled");
                }
            //}
        }

        //private bool SaveRevision()
        //{
        //    bool save = chkSaveRevision.IsChecked == 1 ? true : false;
        //    return save;
        //}

        private string GetDefaultPrinter()
        {
            return Global.GetDefaultPrinter();
        }
    }
}
