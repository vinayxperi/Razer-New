using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
using Infragistics.Windows.DataPresenter;
using System.Diagnostics;

namespace Razer.CustomInvoice
{
    /// <summary>
    /// Interaction logic for CustomInvoiceView.xaml
    /// </summary>
    public partial class CustomInvoiceView : ScreenBase
    {
        private static readonly string fieldLayoutResource = "CustomInvoiceViewInvoices";
        private static readonly string mainTableName = "view";
        private static readonly string server = "server_loc";
        private static readonly string file = "file_name";
        private static readonly string directory = "directory";

        
        public CustomInvoiceView()
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
            idgCustomInvoiceView.xGrid.FieldLayoutSettings = layouts;
            idgCustomInvoiceView.FieldLayoutResourceString = fieldLayoutResource;
            idgCustomInvoiceView.MainTableName = mainTableName;
            idgCustomInvoiceView.xGrid.FieldSettings.AllowEdit = false;
            idgCustomInvoiceView.xGrid.FieldLayoutSettings.AllowDelete = false;
            idgCustomInvoiceView.SetGridSelectionBehavior(true, false);

            GridCollection.Add(idgCustomInvoiceView);

            idgCustomInvoiceView.WindowZoomDelegate = GridDoubleClickDelegate;

        }

        public void GridDoubleClickDelegate()
        {
            DataRecord record = idgCustomInvoiceView.xGrid.ActiveRecord as DataRecord;

            if (record != null)
            {
                string serverLocation = record.Cells[server].Value.ToString();
                string fileName = record.Cells[file].Value.ToString();
                string folder = record.Cells[directory].Value.ToString();

                StringBuilder pathBuilder = new StringBuilder(serverLocation);
                pathBuilder.Append(folder);
                pathBuilder.Append(@"\");
                pathBuilder.Append(fileName);

                if (File.Exists(pathBuilder.ToString()))
                    Process.Start(pathBuilder.ToString());
                else
                    MessageBox.Show("File does not exist");
            }
        }
       
    }
}
