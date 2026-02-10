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
using System.Diagnostics;
using Infragistics.Windows.DataPresenter;

namespace Razer.CustomInvoice
{
    /// <summary>
    /// Interaction logic for CustomInvoiceAdjustments.xaml
    /// </summary>
    public partial class CustomInvoiceAdjustments : ScreenBase
    {
        private static readonly string fieldLayoutResource = "CustomInvoiceAdjustments";
        private static readonly string mainTableName = "adjustments";
        

        public CustomInvoiceAdjustments()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            layouts.AllowAddNew = false;

            this.MainTableName = mainTableName;
            idgAdjustments.xGrid.FieldLayoutSettings = layouts;
            idgAdjustments.FieldLayoutResourceString = fieldLayoutResource;
            idgAdjustments.MainTableName = mainTableName;
            idgAdjustments.xGrid.FieldSettings.AllowEdit = false;
            idgAdjustments.SetGridSelectionBehavior(true, false);

            GridCollection.Add(idgAdjustments);

            idgAdjustments.WindowZoomDelegate = GridDoubleClickDelegate;
        }


        public void GridDoubleClickDelegate()
        {

            //determine what folder to call based on detail type
            cGlobals.ReturnParms.Clear();
            idgAdjustments.ReturnSelectedData("document_id");
            cGlobals.ReturnParms.Add("GridInvoiceAdjustment.xGrid");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = idgAdjustments.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);
            
            
            //DataRecord record = idgAdjustments.xGrid.ActiveRecord as DataRecord;

            //if (record != null)
            //{
            //    string serverLocation = record.Cells[server].Value.ToString();
            //    string fileName = record.Cells[file].Value.ToString();
            //    string folder = record.Cells[directory].Value.ToString();

            //    StringBuilder pathBuilder = new StringBuilder(serverLocation);
            //    pathBuilder.Append(folder);
            //    pathBuilder.Append(@"\");
            //    pathBuilder.Append(fileName);

            //    Process.Start(pathBuilder.ToString());
            //}
        }

    }
}
